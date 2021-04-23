using System;
using System.Configuration;
using System.Xml;
using Xunit;

namespace UnitGuiltySpark
{
    public class PatchLauncher
    {
        [Fact]
        public void Init()
        {
            var launcher = new PatchClient.PatchLauncher();
            //launcher.Logger = (ss) => { Console.WriteLine(ss); };
            launcher.Run();
        }
    }

}
