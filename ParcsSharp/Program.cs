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
            var cs = new ControlSpace("Test");
            {
                var firstPoint = await cs.CreatePointAsync("1", PointType.Local, ChannelType.NamedPipe);
                await cs.AddDirectoryAsync(Directory.GetCurrentDirectory(), false);
                // await cs.AddDirectoryAsync(Directory.GetCurrentDirectory(), false);
                Console.WriteLine("SENDER: " + cs.CurrentPoint.Channel.PointID);
                Console.WriteLine("RECEIVER: " + firstPoint.Channel.PointID);
                await firstPoint.RunAsync(new PointStartInfo(Tests.TestParcsPoint));
                await firstPoint.SendAsync(42);
                var res= await Task.WhenAll(firstPoint.GetAsync<string>(), firstPoint.GetAsync<string>());
                Console.WriteLine(res[0]);
                Console.WriteLine(res[1]);
            }
            Console.ReadKey();
        }

        public class Tests
        {
            public async static Task TestParcsPoint(PointInfo info)
            {
                await Task.Delay(5000);
                int sended = await info.ParentPoint.GetAsync<int>();
                Console.WriteLine("received: "+sended);
                await info.ParentPoint.SendAsync("666");
                await Task.Delay(10000);
                await info.ParentPoint.SendAsync("667");
                Console.ReadKey();
            }
        }
    }
}