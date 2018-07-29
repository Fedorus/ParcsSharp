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
            using (ControlSpace cs = new ControlSpace("Example"))
            {
                await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
                var points = new Point[6];
                var tasks = new Task<string>[6];
                for (int i = 0; i < 6; i++)
                {
                    points[i] = await cs.CreatePointAsync(i.ToString(), PointType.Remote, ChannelType.TCP);
                    await points[i].AddChannelAsync(cs.CurrentPoint.Channel);
                    await points[i].RunAsync(new PointStartInfo(Tests.TestParcsPoints));
                    await points[i].SendAsync(i);
                    tasks[i] =  points[i].GetAsync<string>();
                }
                await Task.WhenAll(tasks);
                foreach (var item in tasks)
                {
                    Console.WriteLine(item.Result);
                }
                Console.WriteLine("Done");
                Console.ReadKey();
            }
        }
        public class Tests
        {
            public async static Task TestParcsPoints(PointInfo info)
            {
                int sended = await info.ParentPoint.GetAsync<int>();
                await info.GetPoint(info.Channels[0]).SendAsync(sended.ToString()).ConfigureAwait(true) ;
                Console.WriteLine($"Point {sended} done");
            }
        }
    }
}