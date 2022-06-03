using System;
using TeslasuitAlyx;

static class Program
{
    static int Main(string[] args)
    {
        TsAlyxApplication app = new TsAlyxApplication();
        try
        {
            return app.Run(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return -1;
        }
    }
}
