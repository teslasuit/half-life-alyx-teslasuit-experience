using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslasuitAlyx
{
    public class SteamHelper
    {
        private const string STEAM_REG_KEY_WIN32 = "SOFTWARE\\WOW6432Node\\Valve\\Steam";
        private const string STEAM_INSTALL_PATH_KEY_WIN32 = "InstallPath";
        private const string STEAMAPPS_FOLDER = "steamapps";
        private const string LIBRARY_FOLDERS = "libraryfolders.vdf";
        private const string APPMANIFEST_KEY = "appmanifest_*.acf";

        private static string GetSteamLocationWin32()
        {
            var steamKey = Registry.LocalMachine.OpenSubKey(STEAM_REG_KEY_WIN32);
            if(steamKey == null)
            {
                return null;
            }

            return steamKey.GetValue(STEAM_INSTALL_PATH_KEY_WIN32) as string;
        }


        public static string[] GetSteamDirs()
        {
            var steamPath = GetSteamLocationWin32();
            var appsPath = Path.Combine(steamPath, STEAMAPPS_FOLDER);
            var libFoldersPath = Path.Combine(appsPath, LIBRARY_FOLDERS);

            KeyValue kv = KeyValueFromPath(libFoldersPath);
            var dirs = new List<string>(kv.Children.Count);
            foreach(var dir in kv.Children)
            {
                if(dir.HasChild("path"))
                {
                    dirs.Add(dir["path"].AsString());
                }
            }
            return dirs.ToArray();
        }

        public static AppInfo[] GetApps()
        {
            List<AppInfo> apps = new List<AppInfo>();
            var dirs = GetSteamDirs();
            foreach(var dir in dirs)
            {
                var appsPath = Path.Combine(dir, STEAMAPPS_FOLDER);
                DirectoryInfo dirInfo = new DirectoryInfo(appsPath);
                var files = dirInfo.GetFiles(APPMANIFEST_KEY);

                foreach(var fileInfo in files)
                {
                    var kv = KeyValueFromPath(fileInfo.FullName);
                    AppInfo app = new AppInfo();
                    app.appid = kv["appid"].AsInteger();
                    app.name = kv["name"].AsString();
                    app.installdir = kv["installdir"].AsString();
                    app.fullPath = Path.Combine(appsPath, "common", app.installdir);
                    apps.Add(app);
                }
            }

            return apps.ToArray();
        }

        private static KeyValue KeyValueFromPath(string path)
        {
            KeyValue kv = new KeyValue();
            using(var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                kv.ReadAsText(stream);
            }
            return kv;
        }

        public struct AppInfo
        {
            public int appid;
            public string name;
            public string installdir;
            public string fullPath;
        }
        
    }
}
