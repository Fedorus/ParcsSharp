using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Parcs;
using Parcs.WCF;
using ParcsSharp;
using System.IO;

namespace SimpleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Console.ReadKey();
            ControlSpace controlSpace = new ControlSpace("Works"); //, "net.tcp://192.168.0.101:666"
            Console.WriteLine();//controlSpace.daemons[0].Name
            
            var point = controlSpace.CreatePoint();

            await controlSpace.AddDirectoryAsync(Directory.GetCurrentDirectory(), false);

            point.Run(new PointStartInfo(Program.Run));
            point.Send(10);
            Console.WriteLine();
            Console.ReadKey();
        }

        public static void Run(PointInfo info)
        {
            Console.Clear();
            Console.WriteLine(info.CurrentControlSpace.ID);
            Console.WriteLine(info.CurrentControlSpace.Name);
            Console.WriteLine(info.CurrentControlSpace.PointDirectory);
            Console.WriteLine("I`m ALIVE!!!!");
            if (File.Exists(info.CurrentControlSpace.PointDirectory+"Parcs.dll"))
            {
                Console.WriteLine("Yes it does");
            }
            var subPoint = info.CurrentControlSpace.CreatePoint();
            subPoint.Run(new PointStartInfo(Run2));
            
        }

        public static void Run2(PointInfo info)
        {
            Console.WriteLine("I`m ALIVE!!!!2222222");
        }
    }
}