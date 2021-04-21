using System;
using System.Collections.Generic;
using System.Text;

namespace GuiltySpark
{
    public enum DataItemType
    {
        DB,
        Local
    }



    [Serializable]

    public class DataItem : MarshalByRefObject
    {
        public DataItemType Type { get; protected set; }
    }

    [Serializable]

    public class LocalDataItem : DataItem
    {
        public LocalDataItem()
        {
            Type = DataItemType.Local;
        }
        public string Directory { get; set; }
    }

    [Serializable]

    public class DBDataItem : DataItem
    {
        public DBDataItem()
        {
            Type = DataItemType.DB;
        }
        public string LinkString { get; set; }

    }
}
