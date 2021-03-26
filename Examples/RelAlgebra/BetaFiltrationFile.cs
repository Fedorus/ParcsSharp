using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Parcs;
using RelAlgebra.Items;

namespace RelAlgebra
{
    public static class BetaFiltrationFile
    {
         private const int ItemsNumber = 15_000_000;
        private const int RelPointNumber = 2;
        public static async Task StartAsync(List<string> daemonsUrls)
        {
            TestOneThreaded();
            await TestBetaFiltrationAsync(daemonsUrls);
            TestOneThreaded();
            //await TestBetaFiltrationAsync(daemonsUrls);
        }
        static void TestOneThreaded()
        {
            var data = ItemsGenerator.GenerateSmall(ItemsNumber*RelPointNumber);
            data[0].NItems = 5;
            
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var filterResult = data.Where(x => 
                x.NItems == 5 ).ToList();
            Console.WriteLine($"1th done in {sw.Elapsed} count = {filterResult.Count}");
        }

        public static async Task TestBetaFiltrationAsync(List<string> daemonsUrls)
        {
            ControlSpace c = new ControlSpace("BetaFiltration", daemonsUrls);
            await c.AddDirectoryAsync(Directory.GetCurrentDirectory());
            var r1 = await c.CreatePointAsync("R[1]");
            var r2 = await c.CreatePointAsync("R[2]");
            await r1.RunAsync(new PointStartInfo(RelPoint));

            await r1.GetAsync<bool>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var resPoint = await c.CreatePointAsync("L[1]");
            await resPoint.RunAsync(new PointStartInfo(ResultPoint));

            var command = new RelCommand() {CommandType = CommandType.Filter, DataTo = resPoint.Channel};
            await r1.SendAsync(command);

            var command2 = new RelCommand() {CommandType = CommandType.ReceiveData};
            await resPoint.SendAsync(command2);

            await resPoint.GetAsync<bool>();
            Console.WriteLine($"FiltrationFile done in {sw.Elapsed}");
        }


        public static async Task RelPoint(PointInfo info)
        {
            var parent = info.ParentPoint;
            var space = info.ControlSpace;
            var controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();

            var subPoints = new List<Point>(RelPointNumber);
            var listReady = new List<Task>();
            for (int i = 0; i < RelPointNumber; i++)
            {
                subPoints.Add(await space.CreatePointAsync($"{info.Channel.Name}[{i}]"));
                await subPoints[i].RunAsync(new PointStartInfo(RelSubPoint));
                listReady.Add(subPoints[i].GetAsync<bool>());
            }

            await Task.WhenAll(listReady);
            await parent.SendAsync(true);
            do
            {
                var command = await parent.GetAsync<RelCommand>();
                if (command.CommandType == CommandType.Filter)
                {
                    foreach (var subPoint in subPoints)
                    {
                        await subPoint.SendAsync(command);
                    }
                }
            } while (!info.CancellationToken.IsCancellationRequested);
        }

        public static async Task RelSubPoint(PointInfo info)
        {
            var parent = info.ParentPoint;
            var space = info.ControlSpace;
            //var controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();
            var data = ItemsGenerator.GenerateSmall(ItemsNumber);
            data[0].NItems = 5;
            await parent.SendAsync(true);
            var command = await parent.GetAsync<RelCommand>();
            if (command.CommandType == CommandType.Filter)
            {
                var filterResult = data.Where(x => x.NItems == 5).ToList();
                var l1 = info.GetPoint(command.DataTo);
                await l1.SendAsync(filterResult);
            }
        }

        public static async Task ResultPoint(PointInfo info)
        {
            var parent = info.ParentPoint;
            var space = info.ControlSpace;
            var controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();

            var command = await parent.GetAsync<RelCommand>();
            if (command.CommandType == CommandType.ReceiveData)
            {
                controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();
                var waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                    .SelectMany(x => x.Data.Where(y => y.Channel.Name.StartsWith("R[1]["))).ToList();
                Console.WriteLine($"Waiting from: {waitFromPoints.Count()}");
                foreach (var point in waitFromPoints.Select(x => x.Channel))
                {
                    var result = await info.GetPoint(point).GetAsync<List<SmallItem>>();
                }
            }

            await parent.SendAsync(true);
        }
    }
}