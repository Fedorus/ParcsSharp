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
            ControlSpace cs = new ControlSpace("Simple stuff");
            //ControlSpace cs = new ControlSpace("Simple stuff", new List<string>(){ "net.tcp://192.168.50.210:666" });
            var sw = new Stopwatch();
            sw.Start();
            var points = new List<Point>(400);
            
            await cs.AddDirectoryAsync(Directory.GetCurrentDirectory());
            for (int i = 0; i < 1; i++)
            {
                var point = await cs.CreatePointAsync(i.ToString(), PointType.Any, ChannelType.TCP);
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
            
            var list = new List<ExampleClass>()
            {
                new ExampleClass() {Id = 1, Name = "1"},
                new ExampleClass() {Id = 2, Name = "2"},
                new ExampleClass() {Id = 3, Name = "3"},
                new ExampleClass() {Id = 4, Name = "4"},
                new ExampleClass() {Id = 5, Name = "5"},
            };
            Expression<Func<List<ExampleClass>, object>> expr = (i =>
                i.Select(x => new ExampleClassShort() {Id = x.Id, Name = x.Name}).ToList());
            Console.WriteLine(expr.Compile().Invoke(list));
            
            
            for (int j = 0; j < 1; j++)
            {
                await points[j].SendAsync<Expression<Func<List<ExampleClass>, object>>>(i =>
                    i.Select(x => new ExampleClassShort() {Id = x.Id, Name = x.Name}).ToList()
                );

            }

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

            var list = new List<ExampleClass>()
            {
                new ExampleClass() {Id = 1, Name = "1"},
                new ExampleClass() {Id = 2, Name = "2"},
                new ExampleClass() {Id = 3, Name = "3"},
                new ExampleClass() {Id = 4, Name = "4"},
                new ExampleClass() {Id = 5, Name = "5"},
            };
            
            var item = await info.ParentPoint.GetAsync<Expression<Func<List<ExampleClass>, object>>>();
            Console.WriteLine(item.Compile().Invoke(list));
        }

        public static int F(int a)
        {
            return a * a * a;
        }
        
        
    }
    public class ExampleClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; } = "value";
    }
    public class ExampleClassShort
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}