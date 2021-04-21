using GuiltySpark;
using System;

namespace Test.DataPatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    [Serializable]
    public class OneDataPatch : DataPatchBase
    {
        public override Guid ID => new Guid("45B45208-4304-4A80-AB53-B21683BBB7A3");

        public override int DataVersion => 2;

        public override string Product => "Tornado2000S";

        public override int MiniTargetDataVersion { get => 0; set => throw new NotImplementedException(); }
        public override int MaxTargetDataVersion { get => 1; set => throw new NotImplementedException(); }

        public override void Backup()
        {
           

        }

        public override void Restore()
        {
        }

        public override void Run()
        {
            var enumer = Options.DataItems.GetEnumerator();
            while (enumer.MoveNext())
            {
                System.Diagnostics.Debug.WriteLine($"type:{Enum.GetName(typeof(DataItemType), enumer.Current.Type)}");
                this.Options.ItemFinishCallback?.Invoke(enumer.Current,this);
            }
        }
    }
}
