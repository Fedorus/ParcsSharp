using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Parcs;

namespace ParcsSharp
{
    public class Program
    {
        public static async Task Main()
        {
            using (var cs = new ControlSpace("Test"))
            {
                var firstPoint = await cs.CreatePointAsync("1", PointType.Local, ChannelType.NamedPipe);
                cs.AddDirectory(Directory.GetCurrentDirectory(), false);
               // await cs.AddDirectoryAsync(Directory.GetCurrentDirectory(), false);
                await firstPoint.RunAsync(new PointStartInfo(Tests.TestParcsPoint));
                await firstPoint.SendAsync(42);
            }
            Console.ReadKey();
        }

        public class Tests
        {
            public async static Task TestParcsPoint(PointInfo info)
            { 
                int sended = await info.ParentPoint.GetAsync<int>();
                Console.WriteLine(sended);
                Console.ReadKey();
            }
        }
    }
}