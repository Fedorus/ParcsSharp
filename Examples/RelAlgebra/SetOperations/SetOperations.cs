using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using RelAlgebra.Items;

namespace RelAlgebra.SetOperations
{
    public class SetOperations
    {
        public static void Test()
        {
            var doc1 = new LazyBsonDocument(new SimpleItem(){ Number = 1}.ToBson());
            var doc2 = new LazyBsonDocument(new SimpleItem(){ Number = 2}.ToBson());
            var doc3 = new LazyBsonDocument(new SimpleItem(){ Number = 2}.ToBson());
            
            var a = new List<LazyBsonDocument>()
            {
                doc1,
                doc3
            };
            var b = new List<LazyBsonDocument>()
            {
                doc1,
                doc2,
            };          
            
            var result = Union(a, b);

            DisplaySet("Union", result);
            
            result = Except(a, b);
            
            DisplaySet("Except", result);
            
            result = Intersect(a, b).ToList();

            DisplaySet("Intersect", result);
        }

        private static IEnumerable<LazyBsonDocument> Union(List<LazyBsonDocument> a, List<LazyBsonDocument> b)
        {
            foreach (var aItem in a)
            {
                if (!b.Any(x =>
                {
                    return aItem.Slice.AccessBackingBytes(0).Array.SequenceEqual(x.Slice.AccessBackingBytes(0).Array);
                }))
                {
                    yield return aItem;
                }
            }

            foreach (var item in b)
            {
                yield return item;
            }
        }
        private static IEnumerable<LazyBsonDocument> Except(List<LazyBsonDocument> a, List<LazyBsonDocument> b)
        {
            foreach (var aItem in a)
            {
                if (!b.Any(x =>
                {
                    return aItem.Slice.AccessBackingBytes(0).Array.SequenceEqual(x.Slice.AccessBackingBytes(0).Array);
                }))
                {
                    yield return aItem;
                }
            }
        }
        private static IEnumerable<LazyBsonDocument> Intersect(List<LazyBsonDocument> a, List<LazyBsonDocument> b)
        {
            foreach (var aItem in a)
            {
                if (b.Any(x =>
                {
                    return aItem.Slice.AccessBackingBytes(0).Array.SequenceEqual(x.Slice.AccessBackingBytes(0).Array);
                }))
                {
                    yield return aItem;
                }
            }
        }

        private static void DisplaySet(string message, IEnumerable<LazyBsonDocument> array)
        {
            Console.WriteLine(message);
            foreach (var item in array)
            {
                Console.WriteLine(item.IsMaterialized);
                //Console.WriteLine(item[0]);
            }
            Console.WriteLine();
        }
    }
}