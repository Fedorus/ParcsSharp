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
    public class Database : IDisposable, IDb
    {
        private readonly string _path;

        public Database(string path)
        {
            _path = path;
        }

        public IEnumerable<RawBsonDocument> ReadAll()
        {
            using var file = File.OpenRead(_path);
            using BsonBinaryReader reader = new BsonBinaryReader(file);
            reader.ReadStartDocument();
            reader.ReadStartArray();
            while (reader.State != BsonReaderState.EndOfArray)
            {
                yield return new RawBsonDocument(reader.ReadRawBsonDocument());
                reader.ReadBsonType();
                //Console.WriteLine();
                //Console.WriteLine(reader.State);
            }
        }

        public void WriteAll(IEnumerable<RawBsonDocument> items)
        {
            using FileStream file = File.OpenWrite(_path);
            using BsonBinaryWriter writer = new BsonBinaryWriter(file);
            
            writer.WriteStartDocument();

            writer.WriteStartArray("Docs");

            foreach (var item in items)
            {
                writer.WriteRawBsonDocument(item.Slice);
            }
            
            writer.WriteEndArray();
            
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
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
        public void Write(RawBsonDocument item)
        {
            _writer.WriteRawBsonDocument(item.Slice);
        }

        public void Write(IEnumerable<RawBsonDocument> items)
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

        public byte[] ToByteArray()
        {
            return File.ReadAllBytes(_path);
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _writerFileStream?.Dispose();
        }
    }
}
