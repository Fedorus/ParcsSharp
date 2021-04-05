using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Parcs;
using RelAlgebra.Db;
using RelAlgebra.Items;
using RelAlgebra.SetOperations;

namespace RelAlgebra.MultitableOperationsTests
{
    public class ExistTest
    {
        private const int ItemsNumber = 5_000;
        private const int RelPointNumber = 6;

        public static async Task StartAsync(List<string> daemonsUrls = null)
        {
            generateFiles();
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            TestOneThreaded();
            await StartParcsCSAsync(daemonsUrls);
            await StartParcsCSAsync(daemonsUrls);
            await StartParcsCSAsync(daemonsUrls);
            await StartParcsCSAsync(daemonsUrls);
            await StartParcsCSAsync(daemonsUrls);
            //await StartParcsCSAsync(daemonsUrls);
            //await TestBetaFiltrationAsync(daemonsUrls);
        }

        static void generateFiles()
        {
            var data = ItemsGenerator.GenerateSimple(ItemsNumber, simpleItem => simpleItem);
            
            using (Database db = new Database("E://1.bson"))
            {
                db.WriteAll(data.Select(x => new RawBsonDocument(x.ToBson())));
            }

            data = ItemsGenerator.GenerateSimple(ItemsNumber, simpleItem => simpleItem);
            using (Database db = new Database("E://2.bson"))
            {
                db.WriteAll(data.Select(x => new RawBsonDocument(x.ToBson())));
            }

            for (int i = 0; i < RelPointNumber; i++)
            {
                var data2 = ItemsGenerator.GenerateSimple(ItemsNumber / RelPointNumber, simpleItem => simpleItem);
                data2[0].Number = 5;
                if (File.Exists($"E://R[1][{i}].bson"))
                    File.Delete($"E://R[1][{i}].bson");
                using (var db = new Database($"E://R[1][{i}].bson"))
                {
                    db.WriteAll(data2.Select(x => new RawBsonDocument(x.ToBson())));
                }
            }

            for (int i = 0; i < RelPointNumber; i++)
            {
                var data2 = ItemsGenerator.GenerateSimple(ItemsNumber / RelPointNumber, simpleItem => simpleItem);
                data2[0].Number = 5;
                if (File.Exists($"E://R[2][{i}].bson"))
                    File.Delete($"E://R[2][{i}].bson");
                using (var db = new Database($"E://R[2][{i}].bson"))
                {
                    db.WriteAll(data2.Select(x => new RawBsonDocument(x.ToBson())));
                }
            }
        }

        static void TestOneThreaded()
        {
            using Database db1 = new Database("E://1.bson");
            using Database db2 = new Database("E://2.bson");
            InMemoryDb idb1 = new InMemoryDb(db1.ToByteArray());
            InMemoryDb idb2 = new InMemoryDb(db2.ToByteArray());
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //var filterResult = RelAlgebra.SetOperations.SetOperations.Union(list1, list2).ToList();
            var name = "Number";
            
            InMemoryDb result = idb1.WhereFieldWithSameValue(idb2.ReadAll(), name);

            Console.WriteLine($"1th done in {sw.Elapsed} count = {result.ToByteArray().Length}");
        }

        public static async Task StartParcsCSAsync(List<string> daemonsUrls)
        {
            ControlSpace c = new ControlSpace("ExistParcs", daemonsUrls);
            await c.AddDirectoryAsync(Directory.GetCurrentDirectory());
            var r1 = await c.CreatePointAsync("R[1]");
            var r2 = await c.CreatePointAsync("R[2]");
            await r1.RunAsync(new PointStartInfo(RelPoint));
            await r2.RunAsync(new PointStartInfo(RelPoint));

            await r1.GetAsync<bool>();
            await r2.GetAsync<bool>();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var resPoint = await c.CreatePointAsync("L[1]");
            await resPoint.RunAsync(new PointStartInfo(ResultPoint));

            //send 1st point 
            var command = new RelCommand() {CommandType = CommandType.Exist, DataTo = r2.Channel};
            await r1.SendAsync(command);

            //send 2nd point
            await r2.SendAsync(new RelCommand()
                {CommandType = CommandType.Exist, DataFrom = r1.Channel, DataTo = resPoint.Channel});

            //send result point
            var command2 = new RelCommand() {DataFrom = r2.Channel, CommandType = CommandType.ExistReceiveData};
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
                if (command.CommandType == CommandType.Filter || command.CommandType == CommandType.Union ||
                    command.CommandType == CommandType.SendData || command.CommandType == CommandType.Intersect)
                {
                    foreach (var subPoint in subPoints)
                    {
                        await subPoint.SendAsync(command);
                    }
                }
                else if (command.CommandType == CommandType.Exist)
                {
                    if (command.DataFrom == null)
                    {
                        subPoints.ForEach(x=>x.SendAsync(command));
                    }
                    else
                    {
                        controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();
                        var waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                            .SelectMany(x => x.Data.Where(y => y.Channel.Name.StartsWith(command.DataFrom.Name + "[")))
                            .Select(x => x.Channel).Select(info.GetPoint);

                        var hashCollection = new HashMapSetOperations(new List<byte[]>());
                        foreach (var point in waitFromPoints)
                        {
                            var data = await point.GetAsync<byte[]>();

                            foreach (var subPoint in subPoints)
                            {
                                await subPoint.SendAsync(new RelCommand() {CommandType = CommandType.Exist});
                                await subPoint.SendAsync(data);
                            }

                            var tasks = subPoints.Select(x => x.GetAsync<byte[]>()).ToList();
                            while (tasks.Any())
                            {
                                var res = await Task.WhenAny(tasks);
                                var db = new InMemoryDb(await res);
                                hashCollection.Union(db.ReadAll().Select(x => x.Slice.AccessBackingBytes(0).Array));
                                tasks.Remove(res);
                            }
                        }
                        var resDb = new InMemoryDb();
                        resDb.WriteAll(hashCollection.Items.Select(x => new RawBsonDocument(new ByteArrayBuffer(x))));
                        await info.GetPoint(command.DataTo).SendAsync(resDb.ToByteArray());
                    }
                }
                else
                {
                    throw new Exception("Unknown command");
                }

            } while (!info.CancellationToken.IsCancellationRequested);
        }
        
        

        public static async Task RelSubPoint(PointInfo info)
        {
            var parent = info.ParentPoint;
            var space = info.ControlSpace;

            Database db = new Database($"E://{info.Channel.Name}.bson");
            
            await parent.SendAsync(true);
            do
            {
                var command = await parent.GetAsync<RelCommand>();
                if (command.CommandType == CommandType.Filter)
                {
                    var filterResult = db.ReadAll().Where(x => x["Number"] == 5)
                        .Select(x => BsonSerializer.Deserialize<SimpleItem>(x)).ToList();
                    var l1 = info.GetPoint(command.DataTo);
                    await l1.SendAsync(filterResult);
                }
                else if (command.CommandType == CommandType.SendData)
                {
                    var controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();
                    var waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                        .SelectMany(x => x.Data.Where(y => y.Channel.Name.StartsWith(command.DataTo.Name + "[")))
                        .Select(x => x.Channel).ToList();
                    if (waitFromPoints.Count == 0)
                    {
                        waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                            .SelectMany(x => x.Data.Where(y => y.Channel.Name == command.DataTo.Name))
                            .Select(x => x.Channel).ToList();
                    }

                    foreach (var point in waitFromPoints)
                    {
                        await info.GetPoint(point).SendAsync(db.ToByteArray());
                    }
                }
                else if (command.CommandType == CommandType.Union)
                {
                    var controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();
                    var waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                        .SelectMany(x => x.Data.Where(y => y.Channel.Name.StartsWith(command.DataFrom.Name + "[")))
                        .ToList();
                    //Console.WriteLine($"Waiting from: {waitFromPoints.Count()}");

                    HashMapSetOperations hashCollection =
                        new HashMapSetOperations(db.ReadAll().Select(x => x.Slice.AccessBackingBytes(0).Array));
                    var data = db.ReadAll().ToList();
                    foreach (var point in waitFromPoints.Select(x => x.Channel))
                    {
                        var result = await info.GetPoint(point).GetAsync<byte[]>();
                        var results = new InMemoryDb(result).ReadAll().Select(x => x.Slice.AccessBackingBytes(0).Array);
                        //data = RelAlgebra.SetOperations.SetOperations.Except(data, new InMemoryDb(result).ReadAll().ToList()).ToList();
                        hashCollection.Except(results);
                    }

                    var l1 = info.GetPoint(command.DataTo);
                    var finalResult = new InMemoryDb();

                    finalResult.WriteAll(hashCollection.Items.Select(x => new RawBsonDocument(x)));
                    await l1.SendAsync(finalResult.ToByteArray());
                }
                else if (command.CommandType == CommandType.Intersect)
                {
                    var controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();
                    var waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                        .SelectMany(x => x.Data.Where(y => y.Channel.Name.StartsWith(command.DataFrom.Name + "[")))
                        .ToList();

                    var finalResult = new InMemoryDb();
                    finalResult.StartWrite();
                    foreach (var point in waitFromPoints.Select(x => x.Channel))
                    {
                        var result = await info.GetPoint(point).GetAsync<byte[]>();

                        HashMapSetOperations hashCollection =
                            new HashMapSetOperations(db.ReadAll().Select(x => x.Slice.AccessBackingBytes(0).Array));
                        var results = new InMemoryDb(result).ReadAll().Select(x => x.Slice.AccessBackingBytes(0).Array);

                        hashCollection.Intersect(results);
                        finalResult.Write(hashCollection.Items.Select(x => new RawBsonDocument(x)));
                    }

                    finalResult.EndWrite();

                    var l1 = info.GetPoint(command.DataTo);
                    await l1.SendAsync(finalResult.ToByteArray());
                }
                else if (command.CommandType == CommandType.Exist)
                {
                    if (command.DataTo != null)
                    {
                        await info.GetPoint(command.DataTo).SendAsync(db.ToByteArray());
                    }
                    else
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        var data = await parent.GetAsync<byte[]>();
                        InMemoryDb inMemoryDb = new InMemoryDb(data);
                        inMemoryDb.WhereFieldWithSameValue(db.ReadAll(), "Number");
                        
                        await parent.SendAsync(inMemoryDb.ToByteArray());
                        
                    }
                }
            } while (!info.CancellationToken.IsCancellationRequested);
        }

        public static async Task ResultPoint(PointInfo info)
        {
            var parent = info.ParentPoint;
            var space = info.ControlSpace;
            var controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();

            do
            {
                var command = await parent.GetAsync<RelCommand>();

                if (command.CommandType == CommandType.ReceiveData)
                {
                    controlSpaceInfo = await space.Daemons[0].GetControlSpacesAsync();
                    var waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                        .SelectMany(x => x.Data.Where(y => y.Channel.Name.StartsWith(command.DataFrom.Name + "[")))
                        .ToList();
                    //Console.WriteLine($"Waiting from: {waitFromPoints.Count()}");

                    var resultTasks = new List<Task<byte[]>>();
                    foreach (var point in waitFromPoints.Select(x => x.Channel))
                    {
                        resultTasks.Add(info.GetPoint(point).GetAsync<byte[]>());
                    }

                    //union results gathering
                    if (command.DataTo != null)
                    {
                        waitFromPoints = controlSpaceInfo.Where(x => x.Name == space.Name)
                            .SelectMany(x => x.Data.Where(y => y.Channel.Name.StartsWith(command.DataTo.Name + "[")))
                            .ToList();
                        foreach (var point in waitFromPoints.Select(x => x.Channel))
                        {
                            resultTasks.Add(info.GetPoint(point).GetAsync<byte[]>());
                        }
                    }

                    while (resultTasks.Any())
                    {
                        var res = await Task.WhenAny(resultTasks);
                        Console.WriteLine(res.Result.Length);
                        resultTasks.Remove(res);
                    }
                }
                else if (command.CommandType == CommandType.ExistReceiveData)
                {
                    var data = await info.GetPoint(command.DataFrom).GetAsync<byte[]>();
                    Console.WriteLine(data.Length);
                }

                await parent.SendAsync(true);
                Console.WriteLine("Done");
            } while (!info.CancellationToken.IsCancellationRequested);
        }
    }
}