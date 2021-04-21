using System;
using System.Collections.Generic;
using System.Text;

namespace GuiltySpark
{
    [Serializable]
    public class DataPatchOptions : MarshalByRefObject
    {
        public IEnumerable<DataItem> DataItems;
        public Action<DataItem, DataPatchBase> ItemFinishCallback;
    }
}
