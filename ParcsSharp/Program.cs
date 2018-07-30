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
                int N = 3;
                await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
                var points = new Point[N];
                var tasks = new Task<string>[N];
                for (int i = 0; i < N; i++)
                {
                    points[i] = await cs.CreatePointAsync(i.ToString(), PointType.Remote, ChannelType.TCP);
                    await points[i].AddChannelAsync(cs.CurrentPoint.Channel);
                    await points[i].RunAsync(new PointStartInfo(Tests.TestParcsPoints));
                    points[i].SendAsync(i);
                    tasks[i] = points[i].GetAsync<string>();
                }
                Task.WaitAll(tasks);
                //await Task.WhenAll(tasks).ContinueWith((s)=>Console.WriteLine("Магия"));
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
                await info.GetPoint(info.Channels[0]).SendAsync(info.CurrentPoint.Channel.Name).ConfigureAwait(false);
                Console.WriteLine($"Point {info.CurrentPoint.Channel.Name} done");
            }
        }
    }
}