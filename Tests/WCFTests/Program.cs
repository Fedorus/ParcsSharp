using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Parcs;

namespace WCFTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BenchCSCreation();
            await Task.Delay(100);
        }

        static void BenchCSCreation()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            ControlSpace cs = new ControlSpace("test");
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();
        }
    }
}
