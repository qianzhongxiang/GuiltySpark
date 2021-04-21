using System;
using System.Collections.Generic;

namespace GuiltySpark
{
    [Serializable]
    public abstract class DataPatchBase: MarshalByRefObject
    {

        public string RootDirectory;

        public abstract Guid ID { get; }

        public abstract int DataVersion { get; }

        public abstract string Product { get; }

        public abstract int MiniTargetDataVersion { get; set; }
        public abstract int MaxTargetDataVersion { get; set; }

        public DataPatchOptions Options;

        public abstract void Run();


        public abstract void Backup();


        public abstract void Restore();

    }

    public class PatchInfo
    {
        public Guid ID { get; set; }
        public int MiniTargetDataVersion { get; set; }
        public int MaxTargetDataVersion { get; set; }
    }
}
