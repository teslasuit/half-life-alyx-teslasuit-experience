using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeslasuitAlyx
{
    public static class CommandLineArgsHelper
    {
        public static T ParseArgs<T>(string[] args) where T : new()
        {
            object t = new T();
            try
            {
                var kvArgs = ArgsToCmdArgs(args);

                foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    var cmd = kvArgs.Where((item) => item.Key == field.Name);

                    if (!cmd.Any())
                    {
                        continue;
                    }
                    var cmdKV = cmd.First();
                    var key = cmdKV.Key;
                    var value = cmdKV.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    object valueToSet = null;
                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(t, Convert.ToInt32(value));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(t, valueToSet);
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(t, value);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse args: " + ex.ToString());
            }
            return (T)t;
        }

        public static void ParseArgs<T>(string[] args, ref object obj)
        {
            object t = obj;
            try
            {
                var kvArgs = ArgsToCmdArgs(args);

                foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    var cmd = kvArgs.Where((item) => item.Key == field.Name);

                    if (!cmd.Any())
                    {
                        continue;
                    }
                    var cmdKV = cmd.First();
                    var key = cmdKV.Key;
                    var value = cmdKV.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    object valueToSet = null;
                    if (field.FieldType == typeof(int))
                    {
                        field.SetValue(t, Convert.ToInt32(value));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(t, valueToSet);
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(t, value);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse args: " + ex.ToString());
            }
        }

        public static Dictionary<string, string> ArgsToCmdArgs(string[] args)
        {
            Dictionary<string, string> kvArgs = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; ++i)
            {
                string key = args[i];
                string value = "";
                if (!key.StartsWith("-"))
                {
                    continue;
                }
                key = key.Substring(1);
                if (i + 1 < args.Length)
                {
                    if (!args[i + 1].StartsWith("-"))
                    {
                        value = args[i + 1];
                        ++i;
                    }
                    kvArgs[key] = value;
                }
            }
            return kvArgs;
        }
    }
}
