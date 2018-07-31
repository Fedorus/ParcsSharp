using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int N = 3;
                await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
                var points = new Point[N];
                var tasks = new Task<string>[N];
                for (int i = 0; i < N; i++)
                {
                    points[i] = await cs.CreatePointAsync(i.ToString(), PointType.Remote, ChannelType.TCP);
                    await points[i].AddChannelAsync(cs.CurrentPoint.Channel);
                    await points[i].RunAsync(new PointStartInfo(Tests.TestParcsPoints));
                    await points[i].SendAsync(i);
                    tasks[i] = points[i].GetAsync<string>();
                }
                //Task.WaitAll(tasks); // Working
                await Task.WhenAll(tasks); //not Working
                await Task.Delay(1000);
                //await Task.WhenAll(tasks).ContinueWith((s)=>Console.WriteLine("Magic")); //Working o_O
                foreach (var item in tasks)
                {
                    Console.WriteLine(item.Result);
                }
                Console.WriteLine("Done " + sw.Elapsed);
                Console.ReadKey();
            }
        }
        public class Tests
        {
            public async static Task TestParcsPoints(PointInfo info)
            {
                var point = info.GetPoint(info.Channels[0]);
                await point.SendAsync(info.CurrentPoint.Channel.Name);
                Console.WriteLine($"Point {info.CurrentPoint.Channel.Name} done");
            }
        }
    }
}