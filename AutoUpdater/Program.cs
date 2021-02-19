using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);
            CallUpdater(args[0]);
        }
        public static void Test()
        {
            Console.WriteLine("Is a test");
        }

        private static void CallUpdater(string path)
        {
            Process.Start(path);
        }
    }
}
