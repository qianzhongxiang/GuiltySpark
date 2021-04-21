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




    public class DataItem 
    {
        public DataItemType Type { get; internal protected set; }
        public DataInfo Info { get; internal protected set; }
    }

    public class LocalDataItem : DataItem
    {
        public LocalDataItem()
        {
            Type = DataItemType.Local;
        }
        public string Directory { get; set; }
    }

    public class DBDataItem : DataItem
    {
        public DBDataItem()
        {
            Type = DataItemType.DB;
        }
        public string LinkString { get; set; }

    }
}
