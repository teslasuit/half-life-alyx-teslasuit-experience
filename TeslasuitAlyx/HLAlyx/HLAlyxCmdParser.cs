using System;
using System.Diagnostics;
using System.Linq;

namespace TeslasuitAlyx
{
    public partial class HLAlyxCmdParser
    {
        private const string HLALYX_CMD = "[Teslasuit]";
        private const char HLALYX_ARG_SEP = ',';
        private const char HLALYX_ARGS_BEG = '{';
        private const char HLALYX_ARGS_END = '}';

        public bool Parse(string line, out HLAlyxCmd cmd)
        {
            cmd = new HLAlyxCmd();
            if(!line.Contains(HLALYX_CMD))
            {
                return false;
            }

            try
            {
                string argsBody = line.Substring(line.IndexOf(HLALYX_CMD) + HLALYX_CMD.Length + 1);
                argsBody = argsBody.Replace(" ", "");
                string argsListBody = argsBody.Substring(argsBody.IndexOf(HLALYX_ARGS_BEG) + 1, argsBody.IndexOf(HLALYX_ARGS_END) - 1);
                string[] args = argsListBody.Split(HLALYX_ARG_SEP);
                if(args.Length == 0)
                {
                    return false;
                }
                cmd.header = args[0];
                cmd.args = args.Skip(1).ToArray();
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed to parse cmd: " + ex);
                return false;
            }
        }

    }
}
