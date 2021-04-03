using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using RelAlgebra.Items;

namespace RelAlgebra.SetOperations
{
    public class HashMapSetOperations
    {
        public HashSet<byte[]> Items { get; set; }
        public HashMapSetOperations(IEnumerable<byte[]> items)
        {
            Items = new HashSet<byte[]>(items, new ArrayEqualityComparer<byte>());
        }

        public void Except(IEnumerable<byte[]> items)
        {
            Items.ExceptWith(items);
        }

        public void Union(IEnumerable<byte[]> items)
        {
            Items.UnionWith(items);
        }

        public void Intersect(IEnumerable<byte[]> items)
        {
            Items.IntersectWith(items);
        }
        public static void Test()
        {
            var doc1 = new SimpleItem(){ Number = 1}.ToBson();
            var doc2 = new SimpleItem(){ Number = 2}.ToBson();
            var doc3 = new SimpleItem(){ Number = 2}.ToBson();
            
            var a = new List<byte[]>()
            {
                doc1,
                doc3
            };
            var b = new List<byte[]>()
            {
                doc1,
                doc2,
            };
            var opt = new HashMapSetOperations(a);
            opt.Except(b);
            Console.WriteLine(opt.Items.Count);
        }
    }

    public sealed class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
    {
        // You could make this a per-instance field with a constructor parameter
        private static readonly EqualityComparer<T> elementComparer
            = EqualityComparer<T>.Default;

        public bool Equals(T[] first, T[] second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            if (first.Length != second.Length)
            {
                return false;
            }
            for (int i = 0; i < first.Length; i++)
            {
                if (!elementComparer.Equals(first[i], second[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(T[] array)
        {
            unchecked
            {
                int hash = 17;
                foreach (T element in array)
                {
                    hash = hash * 31 + elementComparer.GetHashCode(element);
                }
                return hash;
            }
        }
    }
}