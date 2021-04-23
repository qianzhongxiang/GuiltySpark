using System;
using System.Configuration;
using System.Reflection;
using System.Xml;

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

    public class PatchLauncher : GuiltySpark.PatchLauncher
    {
        public PatchLauncher()
        {
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationManager.AppSettings["noapp"] = "true";
            ConfigurationManager.AppSettings["data_path"] = LocalDataDirectory();
        }
        public override string DBLinkString()
        {
            return GetConfig("ConnStrProfile").Attributes["value"].Value;
        }


        public override string LocalDataDirectory(string relativePath=null)
        {
            if (relativePath is null)
                return GetConfig("data_path").Attributes["value"].Value;
            return System.IO.Path.Combine(GetConfig("data_path").Attributes["value"].Value, relativePath);
        }

        private XmlNode GetConfig(string data_path)
        {

            var xd = new XmlDocument();
            xd.Load(System.IO.Path.Combine(TargetRootDir, "Tornado2000S.exe.config"));
            var dataPathNode = xd.SelectSingleNode($"configuration/appSettings/add[@key=\"{data_path}\"]");
            return dataPathNode;
        }
    }
}
