using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TsSDK;

namespace TeslasuitAlyx
{
    public class TsAlyxApplication
    {
        private struct Config
        {
            public int port;
            public int appid;
            public string path;
            public string filename;

            public static Config Default = new Config() { port = DEFAULT_PORT, appid = ALYX_ID, filename = ALYX_FILENAME };
        }

        private const int DEFAULT_PORT = 2121;
        private const int ALYX_ID = 546560;
        private const string ALYX_FILENAME = "hlvr";
        private const string ALYX_CFG_FILENAME = "skill_manifest.cfg";
        private const string ALYX_SCRIPT_FILENAME = "alyx_damage_listener.lua";
        private const string ALYX_SCRIPT_RELOAD = "script_reload_code";
        private HLAlyxTelnetConsole hl_console;
        private HLAlyxFeedbackEventProvider eventProvider;
        private HlAlyxCmdHandler cmdHandler;
        private HLAlyxCmdParser cmdParser;
        private TsFeedbackPlayer player;
        private TsRoot tsRoot;

        public int Run(string[] args)
        {
            var cfgObj = (object)Config.Default;
            CommandLineArgsHelper.ParseArgs<Config>(args, ref cfgObj);
            var cfg = (Config)cfgObj;
            InitGame(cfg);

            tsRoot = new TsRoot(false);
            hl_console = new HLAlyxTelnetConsole(cfg.port);
            eventProvider = new HLAlyxFeedbackEventProvider();
            cmdHandler = new HlAlyxCmdHandler(eventProvider);
            cmdParser = new HLAlyxCmdParser();
            player = new TsFeedbackPlayer(tsRoot.HapticAssetManager);
            eventProvider.OnFeedbackStart += Vr_OnFeedbackStart;
            eventProvider.OnFeedbackStop += Vr_OnFeedbackStop;

            while (!hl_console.IsConnected)
            {
                try
                {
                    Console.WriteLine("Waiting for Half Life Alyx app...");
                    hl_console.Connect();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Connection timeout. Make sure that hlvr is running with -netconport {DEFAULT_PORT} arg");
                }
            }

            Console.WriteLine("Game connection established.");

            tsRoot.SuitManager.OnSuitConnected += SuitManager_OnSuitConnected;
            tsRoot.SuitManager.OnSuitDisconnected += SuitManager_OnSuitDisconnected;
            foreach(var device in tsRoot.SuitManager.Suits)
            {
                SuitManager_OnSuitConnected(device);
            }

            while(currentSuit == null)
            {
                Console.WriteLine("Waiting for device connection...");
                Thread.Sleep(1000);
            }

            hl_console.Write($"{ALYX_SCRIPT_RELOAD} {ALYX_SCRIPT_FILENAME}");

            while (hl_console.IsConnected)
            {
                var str = hl_console.ReadLine();
                if (!hl_console.IsConnected)
                {
                    break;
                }
                var lines = str.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

                foreach (var line in lines)
                {
                    if (cmdParser.Parse(line, out var cmd))
                    {
                        cmdHandler.EnqueueHandleCmd(cmd);
                        Console.WriteLine(line);
                    }
                    else if (line.Contains("unpaused the game"))
                    {
                        cmdHandler.menuOpen = false;
                    }
                    else if (line.Contains("paused the game"))
                    {
                        cmdHandler.menuOpen = true;
                    }
                    else if (line.Contains("Quitting"))
                    {
                        cmdHandler.Reset();
                    }
                }
            }
            Console.WriteLine("HL Alyx disconnected.");
            return 0;
        }

        private ISuit currentSuit = null;

        private void SuitManager_OnSuitConnected(ISuit obj)
        {
            if(currentSuit != null)
            {
                return;
            }
            Console.WriteLine("Device connected: " + obj.Ssid);
            currentSuit = obj;
            player.CurrentPlayer = obj.HapticPlayer;
        }

        private void SuitManager_OnSuitDisconnected(ISuit obj)
        {
            if(currentSuit != null && currentSuit.Index == obj.Index)
            {
                Console.WriteLine("Device disconnected: " + obj.Ssid);
                currentSuit = null;
                player.CurrentPlayer = null;
            }
        }

        private void Vr_OnFeedbackStop(HLAlyxFeedbackEventArgs obj)
        {
            Console.WriteLine($"=========Feedback stop: {obj}");
            player.Stop(obj);
        }

        private void Vr_OnFeedbackStart(HLAlyxFeedbackEventArgs obj)
        {
            Console.WriteLine($"=========Feedback start: {obj}");
            player.Play(obj);
        }

        private void InitGame(Config cfg)
        {
            var gameDir = GetGameDir(cfg);
            InitGamePrerequisites(gameDir);

            var process = FindProcess(cfg.filename);
            if (process == null)
            {
                process = RunGameProcess(gameDir, cfg);
            }
            
        }

        private Process RunGameProcess(string gameDir, Config cfg)
        {
            var gamePath = CombinePathToExe(gameDir);
            Process hlvrProcess = new Process();
            hlvrProcess.StartInfo.FileName = gamePath;
            hlvrProcess.StartInfo.Arguments = $"-console -vconsole -vr -netconport {cfg.port}";
            hlvrProcess.Start();
            return hlvrProcess;
        }

        private string CombinePathToExe(string gameDir)
        {
            if (string.IsNullOrEmpty(gameDir))
            {
                gameDir = "";
            }
            return Path.Combine(gameDir, "game", "bin", "win64", "hlvr.exe");
        }

        private string GetGameDir(Config cfg)
        {
            var pathToExe = CombinePathToExe(cfg.path);
            if (!string.IsNullOrEmpty(pathToExe) && File.Exists(pathToExe))
            {
                return cfg.path;
            }

            var gameDir = GetAppDirFromProcess(cfg.filename);
            if (!string.IsNullOrEmpty(gameDir))
            {
                return gameDir;
            }

            return GetAppDirFromSteamapps(cfg.appid);
        }

        private void InitGamePrerequisites(string gameDir)
        {
            if(string.IsNullOrEmpty(gameDir) && !Directory.Exists(gameDir))
            {
                Console.WriteLine("Failed to init game prerequisites. Game dir not found.");
                return;
            }

            var scriptsDir = CreateDirectoryCombinedSafe(gameDir, "game", "hlvr", "scripts", "vscripts");
            var cfgDir = CreateDirectoryCombinedSafe(gameDir, "game", "hlvr", "cfg");

            var targetCfgPath = Path.Combine(cfgDir, ALYX_CFG_FILENAME);
            var localScriptPath = Path.Combine(Directory.GetCurrentDirectory(), "scripts", ALYX_SCRIPT_FILENAME);
            var targetScriptPath = Path.Combine(scriptsDir, ALYX_SCRIPT_FILENAME);

            if (File.Exists(targetCfgPath))
            {
                var alyxCfg = File.ReadAllText(targetCfgPath);
                if (!alyxCfg.Contains($"{ALYX_SCRIPT_RELOAD} {ALYX_SCRIPT_FILENAME}"))
                {
                    File.AppendAllLines(targetCfgPath, new[]{$"{ALYX_SCRIPT_RELOAD} {ALYX_SCRIPT_FILENAME}"});
                    Console.WriteLine($"{ALYX_CFG_FILENAME} updated.");
                }
            }

            if(!File.Exists(targetScriptPath))
            {
                Console.WriteLine("Copying script file to: " + targetScriptPath);
                File.Copy(localScriptPath, targetScriptPath);
            }
        }

        private string CreateDirectoryCombinedSafe(params string[] args)
        {
            var dir = Path.Combine(args);
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        private string GetAppDirFromProcess(string name)
        {
            var process = FindProcess(name);
            if(process != null)
            {
                var path = process.MainModule.FileName;

                DirectoryInfo dir = new DirectoryInfo(Path.Combine(path, "..", "..", "..", ".."));
                return dir.FullName;
            }
            return null;
        }

        private string GetAppDirFromSteamapps(int appid)
        {
            var apps = SteamHelper.GetApps();
            var alyxApp = apps.Where((item) => item.appid == appid);
            if(alyxApp.Any())
            {
                return alyxApp.First().fullPath;
            }
            return null;
        }

        private Process FindProcess(string name)
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (process.ProcessName.ToLowerInvariant().Contains(name))
                {
                    return process;
                }
            }
            return null;
        }

        ~TsAlyxApplication()
        {
            Console.WriteLine("~TsAlyxApp");
        }
    }


}
