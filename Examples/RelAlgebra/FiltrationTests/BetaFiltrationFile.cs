using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Parcs;
using RelAlgebra.Db;
using RelAlgebra.Items;

namespace RelAlgebra
{
    public static class BetaFiltrationFile
    {
        private const int ItemsNumber = 1_000_00;
        private const int RelPointNumber = 1;
        public static async Task StartAsync(List<string> daemonsUrls=null)
        {
            generateFiles();
            TestOneThreaded();
            await TestBetaFiltrationFileAsync(daemonsUrls);
            await TestBetaFiltrationFileAsync(daemonsUrls);
            await TestBetaFiltrationFileAsync(daemonsUrls);
            await TestBetaFiltrationFileAsync(daemonsUrls);
            await TestBetaFiltrationFileAsync(daemonsUrls);
            await TestBetaFiltrationFileAsync(daemonsUrls);
            await TestBetaFiltrationFileAsync(daemonsUrls);
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            
            //await TestBetaFiltrationAsync(daemonsUrls);
        }

        static void generateFiles()
        {
            var data = ItemsGenerator.GenerateSimple(ItemsNumber, simpleItem => simpleItem);
            data[0].Number = 12;
            using (Database db = new Database("E://test.bson"))
            {
                db.WriteAll(data.Select(x=>new LazyBsonDocument(x.ToBson())));
            }

            for (int i = 0; i < RelPointNumber; i++)
            {
                var data2 = ItemsGenerator.GenerateSimple(ItemsNumber/RelPointNumber, simpleItem => simpleItem);
                data2[0].Number = 5;
                using (var db = new Database($"E://R[1][{i}].bson"))
                {
                    db.WriteAll(data2.Select(x=>new LazyBsonDocument(x.ToBson())));
                }
            }
            
        }

        static void TestOneThreaded()
        {
            var data = ItemsGenerator.GenerateSimple(ItemsNumber, simpleItem => simpleItem);
            data[0].Number = 5;
            using Database db = new Database("E://test.bson");
            db.WriteAll(data.Select(x=>new LazyBsonDocument(x.ToBson())));
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            var filterResult = db.ReadAll().Where(x => x["Number"] == 5).ToList();
            Console.WriteLine($"1th done in {sw.Elapsed} count = {filterResult.Count}");
        }

        public static async Task TestBetaFiltrationFileAsync(List<string> daemonsUrls)
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
            await c.Daemons[0].DestroyControlSpaceAsync(c);
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
            /*var data = ItemsGenerator.GenerateSimple(ItemsNumber, simpleItem => simpleItem);
            data[0].Number = 5;
            int n = ItemsNumber;*/
            Database db = new Database($"E://{info.Channel.Name}.bson");
            //db.WriteAll(data.Select(x=>new LazyBsonDocument(x.ToBson())));
            
            await parent.SendAsync(true);
            var command = await parent.GetAsync<RelCommand>();
            if (command.CommandType == CommandType.Filter)
            {
                var filterResult = db.ReadAll().Where(x => x["Number"] == 5).Select(x=>BsonSerializer.Deserialize<SimpleItem>(x)).ToList();
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
                //Console.WriteLine($"Waiting from: {waitFromPoints.Count()}");
                foreach (var point in waitFromPoints.Select(x => x.Channel))
                {
                    var result = await info.GetPoint(point).GetAsync<List<SimpleItem>>();
                }
            }

            await parent.SendAsync(true);
        }
    }
}