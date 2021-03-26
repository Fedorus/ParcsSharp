using System;
using System.Collections.Generic;
using System.Text;

namespace RelAlgebra.Items
{
    public static class ItemsGenerator
    {
        public static Random _random = new Random();
        
        public static List<LargeItem> GenerateNormal(int n)
        {
            var list = new List<LargeItem>(n);
            for (int i = 0; i < n; i++)
            {
                var item = new LargeItem
                {
                    OrderId = Guid.NewGuid(),
                    OrderNumber = (ulong) _random.Next(int.MaxValue),
                    EmailAddress = RandomString(10),
                    CreateNonce = Guid.NewGuid(),
                    ShippingCosts = (decimal) _random.NextDouble() * 10_000,
                    LastModified = DateTimeOffset.FromUnixTimeMilliseconds(_random.Next())
                };
                item.RequestedDeliveryDate = item.LastModified.AddMonths(_random.Next(1, 13));
                
                list.Add(item);
            }
            return list;
        }
        
        public static List<LargeItem> GenerateHuge(int n)
        { 
            var list = new List<LargeItem>(n);
            for (int i = 0; i < n; i++)
            {
                var item = new LargeItem
                {
                    OrderId = Guid.NewGuid(),
                    OrderNumber = (ulong) _random.Next(int.MaxValue),
                    EmailAddress = RandomString(10),
                    CreateNonce = Guid.NewGuid(),
                    ShippingCosts = (decimal) _random.NextDouble() * 10_000,
                    LastModified = DateTimeOffset.FromUnixTimeMilliseconds(_random.Next()), 
                    InvoiceAddress = GenerateAddress(), ShippingAddress = GenerateAddress(), 
                    OrderLines = GenerateOrderLines(1)
                };
                item.RequestedDeliveryDate = item.LastModified.AddMonths(_random.Next(1, 13));
                
                list.Add(item);
            }
            return list;
        }

        private static List<OrderLine> GenerateOrderLines(int n)
        {
            var list = new List<OrderLine>(n);
            for (int i = 0; i < n; i++)
            {
                var item = new OrderLine();
                item.Price = _random.Next();
                item.Product = RandomString(10);
                item.Quantity = _random.Next();
                item.Sku = RandomString(10);
                list.Add(item);
            }
            return list;
        }

        private static Address GenerateAddress()
        {
            return new Address()
            {
                Name = RandomString(10),
                City = RandomString(10),
                Country = RandomString(10),
                HouseNumber = RandomString(10),
                PostalCode = RandomString(10),
                Street = RandomString(10)
            };
        }


        public static List<SmallItem> GenerateSmall(int n)
        {
            var list = new List<SmallItem>(n);
            for (int i = 0; i < n; i++)
            {
                list.Add(new SmallItem {Id = Guid.NewGuid()});
            }
            return list;
        }

        public static List<SimpleItem> GenerateSimple(int n, Func<SimpleItem, SimpleItem> action)
        {
            var list = new List<SimpleItem>(n);
            for (int i = 0; i < n; i++)
            {
                var item = new SimpleItem
                {
                    Id = Guid.NewGuid(),
                    Number = _random.Next(),
                    LNumber = _random.Next(),
                    DateTime = DateTime.Now,
                    String = RandomString(10)
                };
                
                list.Add(action(item));
            }
            return list;
        }

        public static string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char) _random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }
}