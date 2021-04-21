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
        protected override string DBLinkString()
        {
            return GetConfig("ConnStrProfile").Attributes["value"].Value;
        }


        protected override string LocalDataDirectory()
        {
            return GetConfig("data_path").Attributes["value"].Value;
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
