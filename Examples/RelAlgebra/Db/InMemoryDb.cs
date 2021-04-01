using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RelAlgebra.Items;

namespace RelAlgebra.Db
{
    public class InMemoryDb : IDisposable, IDb
    {
        private MemoryStream ms;
        public InMemoryDb(byte[] bytes)
        {
            ms = new MemoryStream(bytes);
        }

        public InMemoryDb()
        {
            ms = new MemoryStream();
        }

        public IEnumerable<LazyBsonDocument> ReadAll()
        {
            ms.Seek(0, SeekOrigin.Begin);
            using BsonBinaryReader reader = new BsonBinaryReader(ms);
            reader.ReadStartDocument();
            reader.ReadStartArray();
            while (reader.State != BsonReaderState.EndOfArray)
            {
                yield return new LazyBsonDocument(reader.ReadRawBsonDocument());
                reader.ReadBsonType();
            }
        }

        public void WriteAll(IEnumerable<LazyBsonDocument> items)
        {
            ms.Dispose();
            ms = new MemoryStream();
            using BsonBinaryWriter writer = new BsonBinaryWriter(ms);
            
            writer.WriteStartDocument();

            writer.WriteStartArray("Docs");

            foreach (var item in items)
            {
                BsonSerializer.Serialize(writer, new BsonDocument(item.Elements));
                
                //writer.WriteRawBsonDocument( item.Slice);
            }
            
            writer.WriteEndArray();
            
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        private BsonBinaryWriter _writer = null;
        public void StartWrite()
        {
            ms.Dispose();
            ms = new MemoryStream();
            _writer = new BsonBinaryWriter(ms);
            _writer.WriteStartDocument();
            _writer.WriteStartArray("Docs");
        }

        public void Write(LazyBsonDocument item)
        {
            _writer.WriteRawBsonDocument(item.Slice);
        }

        public void Write(IEnumerable<LazyBsonDocument> items)
        {
            foreach (var item in items)
            {
                Write(item);
            }
        }

        public void EndWrite()
        {
            _writer.WriteEndArray();
            _writer.WriteEndDocument();
            
            _writer.Dispose();
            _writer = null;
        }
        
        public void Dispose()
        {
            ms?.Dispose();
        }

        public byte[] ToByteArray()
        {
            return ms.ToArray();
        }

        public void ToFile(string path)
        {
            using var file = File.OpenWrite(path);
            ms.CopyTo(file);
        }
    }
}