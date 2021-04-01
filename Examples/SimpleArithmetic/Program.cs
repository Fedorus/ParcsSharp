using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Parcs;

namespace SimpleArithmetic
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            
            ControlSpace cs = new ControlSpace("Simple stuff");
            await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
            var sw = new Stopwatch();
            sw.Start();
            var points = new List<Point>(400);
            for (int i = 0; i < 1; i++)
            {
                var point = await cs.CreatePointAsync(i.ToString(), PointType.Any, ChannelType.TCP);
                points.Add(point);
                await point.RunAsync(new PointStartInfo(TestMethod));
            }

            int result = 0;

            foreach (var item in points)
            {
                for (int i = 0; i < N; i++)
                {
                    result += await item.GetAsync<int>();
                }
            }
            Console.WriteLine(result);
            Console.WriteLine(sw.Elapsed);
            Console.ReadKey();
        }
        public static async Task RecurciveMethod(PointInfo info)
        {
            var point = await info.ControlSpace.CreatePointAsync(info.Point.Channel.Name + ".1");
            await point.RunAsync(new PointStartInfo(TestMethod));
            int res = await point.GetAsync<int>();
            await info.ParentPoint.SendAsync(res);
        }
        const int N = 10;
        public static async Task TestMethod(PointInfo info)
        {
            for (int i = 0; i < N; i++)
            {
                await info.ParentPoint.SendAsync(i);
            }
        }
    }
}