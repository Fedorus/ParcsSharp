using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Parcs;

namespace ParcsExpressions
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            ControlSpace cs = new ControlSpace("Simple stuff", new List<string>(){ "net.tcp://192.168.50.210:666" });
            var sw = new Stopwatch();
            sw.Start();
            var points = new List<Point>(400);
            for (int i = 0; i < 1; i++)
            {
                var point = await cs.CreatePointAsync(i.ToString(), PointType.Any, ChannelType.TCP);
                await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
                point.Serializer = new ExpressionSerializer();
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
            await points[0].SendAsync<Expression<Func<int, int>>>(i => F(i));
            
            Console.ReadKey();
        }
        const int N = 10;
        public static async Task TestMethod(PointInfo info)
        {
            info.ParentPoint.Serializer = new ExpressionSerializer();
            for (int i = 0; i < N; i++)
            {
                await info.ParentPoint.SendAsync(i);
            }

            List<string> items = new List<string>() {"a", "b"};
            var item = await info.ParentPoint.GetAsync<Expression<Func<int, int>>>();
            Console.WriteLine(item.Compile().Invoke(10));

        }

        public static int F(int a)
        {
            return a * a * a;
        }
    }
}