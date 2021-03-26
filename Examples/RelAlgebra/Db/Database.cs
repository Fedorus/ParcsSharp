using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace RelAlgebra.Db
{
    public class Database : IDisposable
    {
        private readonly string _path;

        public Database(string path)
        {
            _path = path;
        }

        public IEnumerable<LazyBsonDocument> ReadAll()
        {
            using var file = File.OpenRead(_path);
            using BsonBinaryReader reader = new BsonBinaryReader(file);
            reader.ReadStartDocument();
            reader.ReadStartArray();
            while (reader.State != BsonReaderState.EndOfArray)
            {
                yield return new LazyBsonDocument(reader.ReadRawBsonDocument());
                reader.ReadBsonType();
                //Console.WriteLine();
                //Console.WriteLine(reader.State);
            }
        }

        public void WriteAllAsync(IEnumerable<LazyBsonDocument> items)
        {
            using BsonBinaryWriter writer = new BsonBinaryWriter(File.OpenWrite(_path));
            
            writer.WriteStartDocument();

            writer.WriteStartArray("Docs");

            foreach (var item in items)
            {
                writer.WriteRawBsonDocument(item.Slice);
            }
            
            writer.WriteEndArray();
            
            writer.WriteEndDocument();
            writer.Flush();
            
        }

        private BsonBinaryWriter _writer = null;
        private FileStream _writerFileStream = null;
        public void StartWrite()
        {
            _writerFileStream = File.OpenWrite(_path);
            _writer = new BsonBinaryWriter(_writerFileStream);
            _writer.WriteStartDocument();
            _writer.WriteStartArray("Docs");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            _writerFileStream.Dispose();
            _writerFileStream = null;
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _writerFileStream?.Dispose();
        }
    }
}
