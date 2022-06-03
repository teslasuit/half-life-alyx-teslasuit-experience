public static class ArrayStringExtensions
{
    public static string TryGetString(this string[] args, int num, string defaultVal = "")
    {
        if(args.Length > num)
            return args[num].Trim();
        return defaultVal;
    }

    public static int TryGetInt(this string[] args, int num, int defaultVal = 0)
    {
        try
        {
            if (args.Length > num)
                return int.Parse(args[num].Trim());
        }
        catch
        {
            return defaultVal;
        }
        return defaultVal;
    }

    public static float TryGetFloat(this string[] args, int num, float defaultVal = 0)
    {
        try
        {
            if (args.Length > num)
                return float.Parse(args[num].Trim());
        }
        catch
        {
            return defaultVal;
        }
        return defaultVal;
    }
}
