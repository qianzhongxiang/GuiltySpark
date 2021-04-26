using System;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace Patcher.Tornado2000S
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Master Chief!");

            new PatchLauncher().Run();
        }
    }

   
}
