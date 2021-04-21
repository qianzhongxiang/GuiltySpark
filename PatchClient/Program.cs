using System;
using System.Configuration;
using System.Reflection;

namespace PatchClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            new PatchLauncher().Run();
        }
    }

    class PatchLauncher : GuiltySpark.PatchLauncher
    {
        protected override string DBLinkString()
        {
            return GetConfig().AppSettings.Settings["ConnStrProfile"]?.Value;
        }

        protected override string LocalDataDirectory()
        {
            return GetConfig().AppSettings.Settings["data_path"]?.Value;
        }

        private Configuration GetConfig()
        {
            return ConfigurationManager.OpenExeConfiguration(System.IO.Path.Combine(TargetRootDir, "Tornado2000S.exe.config"));
        }
    }
}
