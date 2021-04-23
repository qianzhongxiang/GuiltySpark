using System;
using System.Collections.Generic;
using System.Text;

namespace GuiltySpark
{
    public class DataPatchOptions
    {
        public string LocalDataDir { get; set; }
        public string ApplicationDir { get; set; }
        public IEnumerable<DataItem> DataItems;
        public Action<DataItem, DataPatchBase> ItemFinishCallback;
    }
}
