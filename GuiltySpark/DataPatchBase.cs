using System;

namespace GuiltySpark
{
    public abstract class DataPatchBase
    {

        internal protected string RootDirectory;

        public Action<string> WriteLog { get; internal protected set; }
        public abstract Guid ID { get; }

        public abstract int DataVersion { get; }

        public abstract string Product { get; }


        public abstract int MiniTargetDataVersion { get;  }
        public abstract int MaxTargetDataVersion { get;  }

        public DataPatchOptions Options { get; internal protected set; }

        public abstract void Run();


        public abstract void Backup();


        public abstract void Restore();

    }

    public class PatchInfo
    {
        public Guid ID { get; set; }

        public int MiniTargetDataVersion { get; set; }
        public int MaxTargetDataVersion { get; set; }

        public int DataVersion { get; set; }
    }
}
