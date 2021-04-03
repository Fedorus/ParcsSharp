using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Parcs;
using RelAlgebra.Db;
using RelAlgebra.Items;
using RelAlgebra.SetOperations;

namespace RelAlgebra
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //HashMapSetOperations.Test();
            //SetOperations.SetOperations.Test();
            //await SetOperationsTests.UnionParcs.StartAsync();
            await SetOperationsTests.IntersectParcs.StartAsync();
            //TestInmemoryDatabaseClass();
            //await TestBinaryBson();
            //await TestDiscovery(null);
            //await BetaFiltration.StartAsync(null);
            //TestDatabaseClass();
            //await BetaFiltrationFilePercentage.StartAsync();
            //await BetaFiltrationFile.StartAsync();
            //TestBsonDocuments();
        }

        public static void TestInmemoryDatabaseClass()
        {
            int n = 100;
            var db = new InMemoryDb();

            var item = ItemsGenerator.GenerateSimple(n, simpleItem => simpleItem);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            db.WriteAll(item.Select(x=>new LazyBsonDocument(x.ToBson())));
            sw.Stop();
            Console.WriteLine($"{n} written in {sw.Elapsed}");
            
            sw.Restart();
            int i = 0;
            foreach (var c in db.ReadAll())
            {  
                if (c["Number"] > 0L)
                    i++;
                c.Dispose();
            }
            sw.Stop();
            
            Console.WriteLine($"{n} read {i} in {sw.Elapsed}");
        }

        public static void TestDatabaseClass()
        {
            int n = 1_000_000;
            Database db = new Database("E://test.bson");

            var item = ItemsGenerator.GenerateSimple(n, simpleItem => simpleItem);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            db.StartWrite();
            db.Write(item.Select(x=>new LazyBsonDocument(x.ToBson())));
            item = null;
            db.EndWrite();
            sw.Stop();
            Console.WriteLine($"{n} written in {sw.Elapsed}");
            
            sw.Restart();
            int i = 0;
            foreach (var c in db.ReadAll())
            {  
                if (c["Number"] > 0L)
                    i++;
                c.Dispose();
            }
            sw.Stop();
            Console.WriteLine($"{n} read {i} in {sw.Elapsed}");
        }


        public static void TestBsonReader()
        {
            BsonBinaryReader reader = new BsonBinaryReader(File.OpenRead("2.bson"));
            
            reader.ReadStartDocument();
            reader.ReadStartArray();
            Console.WriteLine();
            while (reader.State != BsonReaderState.EndOfArray)
            {
                
                //var array = new LazyBsonArray(reader.ReadRawBsonArray());
                Console.WriteLine(new RawBsonDocument(reader.ReadRawBsonDocument()));
                Console.WriteLine(reader.ReadBsonType());
                Console.WriteLine(reader.State);
            }
        }

        public static void TestBsonDocuments()
        {
            BsonObject bobj = new BsonObject();
            bobj.Docs = new List<BsonDocument>();
            bobj.Docs = ItemsGenerator.GenerateSmall(20_000_000).Select(x => x.ToBsonDocument()).ToList();

            using (var file = File.Create("1.bson"))
            {
                BsonBinaryWriter w = new BsonBinaryWriter(file);
                BsonSerializer.Serialize(w, bobj);
                w.Close();
            }

            bobj = null;
            //Console.ReadKey();
            BsonBinaryReader r = new BsonBinaryReader(File.OpenRead("1.bson"));
            var a = BsonSerializer.Deserialize<LazyBsonDocument>(r);
            var array = a["Docs"].AsBsonArray;
            Console.WriteLine(array.AsBsonArray.First()[1] == 0);
        }

        public static async Task TestBinaryBson()
        {
            var bobj = new BsonBinaryObject();
            bobj.Docs = new List<RawBsonDocument>();
            bobj.Docs = ItemsGenerator.GenerateNormal(10).Select(x => new RawBsonDocument(x.ToBson())).ToList();
            using (var file = File.Create("2.bson"))
            {
                BsonBinaryWriter w = new BsonBinaryWriter(file);
                BsonSerializer.Serialize(w, bobj);
                w.Close();
            }

            BsonBinaryReader r = new BsonBinaryReader(File.OpenRead("1.bson"));
            var a = BsonSerializer.Deserialize<BsonBinaryObject>(r);
            var sdasd = a.Docs[0].Elements;
            Console.WriteLine(a.Docs.Where(x => x == 1));
        }

        public static async Task TestDiscovery(List<string> daemonsUrls)
        {
            ControlSpace c = new ControlSpace("BetaFiltration", daemonsUrls);
            await c.CreatePointAsync("1");
            ControlSpace c2 = new ControlSpace("other", daemonsUrls);
            await c2.CreatePointAsync("2");
            c.Daemons.ForEach(async (item) =>
            {
                var info = await item.GetControlSpacesAsync();
                Console.WriteLine(info.Count);
            });
            Console.ReadKey();
        }
    }

    public class RelInfo
    {
    }

    public class BsonObject
    {
        public List<BsonDocument> Docs { get; set; }
    }
    
    public class BsonBinaryObject
    {
        public List<RawBsonDocument> Docs { get; set; }
    }
}