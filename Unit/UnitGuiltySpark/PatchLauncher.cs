using System;
using System.Configuration;
using System.Xml;
using Xunit;

namespace UnitGuiltySpark
{
    public class PatchLauncher
    {
        [Fact]
        public void Init()
        {
            new PatchLauncherT().Run();
        }
    }

    [Serializable]
    class PatchLauncherT : GuiltySpark.PatchLauncher
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
