using GuiltySpark;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Patcher.Tornado2000S
{
    public class PatchLauncher : GuiltySpark.PatchLauncher
    {
        public override string TargetRootDir
        {
            get => _TargetRootDir; 
            set {
                _TargetRootDir = value;
                ConfigurationManager.AppSettings["data_path"] = LocalDataDirectory();
            }
        }

        public PatchLauncher()
        {
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationManager.AppSettings["noapp"] = "true";
            ConfigurationManager.AppSettings["data_path"] = LocalDataDirectory();
        }
        public override string DBLinkString()
        {
            var node = GetConfig("ConnStrProfile");
            if (node is null) {
                return default;
            }
            return node.Attributes["value"].Value;
        }

        public override IEnumerable<DataItem> GetItems(int minDataVersion, int maxDataVersion, bool db = true)
        {
            var dirct = LocalDataDirectory("recipes");
            if (dirct is null) yield break;
            var datas = new System.IO.DirectoryInfo(dirct).GetDirectories();

            DataInfo dataInfo = default;
            foreach (var item in datas)
            {
                var file = item.GetFiles(DATAINFOFILENAME).FirstOrDefault();
                if (file is null)
                {
                    dataInfo = new DataInfo { DataVersion = 0 };
                }
                else
                {
                    using (var streamReader = new System.IO.StreamReader(file.FullName))
                    using (var jsonTxtReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var jser = new Newtonsoft.Json.JsonSerializer();
                        dataInfo = jser.Deserialize<DataInfo>(jsonTxtReader);
                    }
                }
                if (dataInfo.DataVersion < minDataVersion || dataInfo.DataVersion > maxDataVersion)
                {
                    continue;
                }
                yield return new LocalDataItem { Info = dataInfo, Directory = item.FullName };
            }
            if (db)
            {
                yield return new DBDataItem { LinkString = DBLinkString() };
            }
        }
        public override string LocalDataDirectory(string relativePath = null)
        {
            var node = GetConfig("data_path");
            if (node is null) return default;
            if (relativePath is null)
                return node.Attributes["value"].Value;
            return System.IO.Path.Combine(node.Attributes["value"].Value, relativePath);
        }

        private XmlNode GetConfig(string data_path)
        {
            var config = System.IO.Path.Combine(TargetRootDir, "Tornado2000S.exe.config");
            if (!System.IO.File.Exists(config)) {
                return default;
            }
            var xd = new XmlDocument();
            xd.Load(config);
            var dataPathNode = xd.SelectSingleNode($"configuration/appSettings/add[@key=\"{data_path}\"]");
            return dataPathNode;
        }
    }
}
