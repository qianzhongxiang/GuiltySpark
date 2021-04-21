using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GuiltySpark
{
    [Serializable]
    public abstract class PatchLauncher : MarshalByRefObject
    {
        public static string DATAINFOFILENAME = "DataInfo.json";
        public Action<string> Logger;
        /// <summary>
        /// 默认程序集更目录的上级
        /// </summary>
        public string TargetRootDir { get; set; } = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(new Uri(Assembly.GetAssembly(typeof(PatchLauncher)).CodeBase).LocalPath)).Parent.FullName;

        //protected abstract DataPatchBase GetDataPatchInstance(Type patchType,AssemblyName assemblyName, AppDomain appDomain);

        private void ItemFinishedCallback(DataItem dataItem, DataPatchBase dataPatch)
        {
            switch (dataItem.Type)
            {
                case DataItemType.DB:
                    DBItemFinishedCallback(dataItem as DBDataItem, dataPatch);
                    break;
                case DataItemType.Local:
                    LocalItemFinishedCallback(dataItem as LocalDataItem, dataPatch);
                    break;
                default:
                    break;
            }
        }

        protected void DBItemFinishedCallback(DBDataItem dataItem, DataPatchBase dataPatch)
        {
        }

        protected void LocalItemFinishedCallback(LocalDataItem dataItem, DataPatchBase dataPatch)
        {
            var dir = new System.IO.DirectoryInfo(dataItem.Directory);
            var file = dir.GetFiles(DATAINFOFILENAME).FirstOrDefault();
            DataInfo dataInfo = null;
            if (file is null)
            {
                dataInfo = new DataInfo();
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
            dataInfo.DataVersion = dataPatch.DataVersion;
            using (var streamWriter = new System.IO.StreamWriter(System.IO.Path.Combine(dir.FullName, DATAINFOFILENAME)))
            using (var jsonTxtWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
            {
                var jser = new Newtonsoft.Json.JsonSerializer();
                jser.Serialize(jsonTxtWriter, dataInfo);
            }
            Console.WriteLine($"{dataItem.Directory}: convert successfully");
        }
        public virtual void Run()
        {
            var enumer = GetPatchs().GetEnumerator();
            AppDomain domain = null;
            while (enumer.MoveNext())
            {
                try
                {
                    System.Security.Policy.Evidence evidence = new System.Security.Policy.Evidence(AppDomain.CurrentDomain.Evidence);
                    AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                    domain = AppDomain.CreateDomain(new System.IO.DirectoryInfo(enumer.Current).Name + "Domain", evidence, setup);
                    var dLauncher = domain.CreateInstanceFromAndUnwrap(new Uri(this.GetType().Assembly.CodeBase).LocalPath, this.GetType().FullName) as PatchLauncher;
                    dLauncher.Logger = this.Logger;
                    dLauncher.Run(enumer.Current);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (domain != null)
                    {
                        AppDomain.Unload(domain);
                    }
                }
            }
        }

        public virtual void Run(string patchDir)
        {
            DataPatchBase instance = null;
            try
            {
                var filepath = System.IO.Path.Combine(patchDir, "patch.exe");
                var sss = Assembly.Load(AssemblyName.GetAssemblyName(filepath));
                var types = sss.GetTypes();
                foreach (var item in types)
                {
                    if (item.IsSubclassOf(typeof(DataPatchBase)))
                    {

                        instance = sss.CreateInstance(item.FullName) as DataPatchBase;
                        var options = new DataPatchOptions();
                        options.DataItems = GetItems(instance.MiniTargetDataVersion, instance.MaxTargetDataVersion);
                        options.ItemFinishCallback = ItemFinishedCallback;
                        instance.Options = options;
                        instance.WriteLog = this.Logger;
                        instance.RootDirectory = System.IO.Path.Combine(LocalDataDirectory(), $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.{instance.DataVersion}");
                        //instance
                        instance.Backup();
                        instance.Run();
                        break;
                    }
                }

            }
            catch (Exception)
            {
                instance?.Restore();
                throw;
            }
        }
        public virtual IEnumerable<DataItem> GetItems(int minDataVersion, int maxDataVersion, bool db = true)
        {
            var dirct = LocalDataDirectory();
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


        protected abstract string LocalDataDirectory();
        protected abstract string DBLinkString();

        public virtual IEnumerable<string> GetPatchs()
        {
            var dir = System.IO.Path.GetDirectoryName(new Uri(typeof(PatchLauncher).Assembly.CodeBase).LocalPath);
            List<PatchInfo> patchInfos = GetPatchInfos();

            var patchDir = new System.IO.DirectoryInfo(System.IO.Path.Combine(dir, "patchs"));
            foreach (var item in patchInfos)
            {
                var patchFolder = patchDir.GetDirectories(item.ID.ToString()).FirstOrDefault();
                if (patchFolder is null)
                {
                    continue;
                }
                yield return UnCompress(patchFolder.FullName);
            }
        }

        public virtual List<PatchInfo> GetPatchInfos()
        {
            var dir = System.IO.Path.GetDirectoryName(new Uri(typeof(PatchLauncher).Assembly.CodeBase).LocalPath);
            var list = new System.IO.DirectoryInfo(dir).GetFiles("list.json").FirstOrDefault();
            if (list is null)
            {
                return new List<PatchInfo>();
            }
            List<PatchInfo> patchInfos = default;
            using (var streamReader = new System.IO.StreamReader(list.FullName))
            using (var jsonTxtReader = new Newtonsoft.Json.JsonTextReader(streamReader))
            {
                var jser = new Newtonsoft.Json.JsonSerializer();
                patchInfos = jser.Deserialize<List<PatchInfo>>(jsonTxtReader);
            }

            return patchInfos;
        }

        public virtual string UnCompress(string filePath)
        {
            return filePath;
        }
    }
}
