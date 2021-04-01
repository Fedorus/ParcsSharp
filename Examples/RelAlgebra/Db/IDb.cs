using System.Collections.Generic;
using MongoDB.Bson;

namespace RelAlgebra.Db
{
    public interface IDb
    {
        IEnumerable<LazyBsonDocument> ReadAll();
        void WriteAll(IEnumerable<LazyBsonDocument> items);
        void StartWrite();
        void Write(LazyBsonDocument item);
        void Write(IEnumerable<LazyBsonDocument> items);
        void EndWrite();

        byte[] ToByteArray();
    }
}