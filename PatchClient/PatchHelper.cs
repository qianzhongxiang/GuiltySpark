using GuiltySpark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patcher.Tornado2000S
{
    public class DataHelper
    {
        public static DataInfo GetDataInfo(string directory)
        {
            var file = new System.IO.DirectoryInfo(directory).GetFiles(PatchLauncher.DATAINFOFILENAME).FirstOrDefault();
            DataInfo dataInfo = default;
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
            return dataInfo;
        }

        public static DataInfo GetDBInfo()
        {
            throw new NotImplementedException();
        }

        public static int MiniLocalDataVersion { get; internal protected set; }
        public static int MaxLocalDataVersion { get; internal protected set; }
        public static int MiniDBVersion { get; internal protected set; }
        public static int MaxDBVersion { get; internal protected set; }
        public static int DefaultLocalDataVersion { get; internal protected set; }
        public static int DefaultDBVersion { get; internal protected set; }

        public static void SetAppDataVersion(int minLocalDataVersion, int maxLocalDataVersion,int defaultLocalDataVersion, int miniDBVersion, int maxDBVersion,int defaultDBVersion)
        {
            MiniLocalDataVersion = minLocalDataVersion;
            MaxLocalDataVersion = maxLocalDataVersion;
            DefaultLocalDataVersion = defaultLocalDataVersion;
            MiniDBVersion = miniDBVersion;
            MaxDBVersion = maxDBVersion;
            DefaultDBVersion = defaultDBVersion;
        }

        public static void NewLocalData(string dataDirectory) {
            var file = new System.IO.DirectoryInfo(dataDirectory).GetFiles(PatchLauncher.DATAINFOFILENAME).FirstOrDefault();
            DataInfo dataInfo = default;
            if (file is null)
            {
                dataInfo = new DataInfo { DataVersion = DefaultLocalDataVersion };
            }
            else
            {
                using (var streamReader = new System.IO.StreamReader(file.FullName))
                using (var jsonTxtReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                {
                    var jser = new Newtonsoft.Json.JsonSerializer();
                    dataInfo = jser.Deserialize<DataInfo>(jsonTxtReader);
                    dataInfo.DataVersion = DefaultLocalDataVersion;
                }
                file.Delete();
            }
            using (var stream= System.IO.File.OpenWrite(System.IO.Path.Combine(dataDirectory, PatchLauncher.DATAINFOFILENAME)))
            using (var streamWriter = new System.IO.StreamWriter(stream))
            using (var jsonTxtWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
            {
                var jser = new Newtonsoft.Json.JsonSerializer();
                jser.Serialize(jsonTxtWriter, dataInfo);
            }
        }
    }
}
