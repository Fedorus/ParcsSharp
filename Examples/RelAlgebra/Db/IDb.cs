using System.Collections.Generic;
using MongoDB.Bson;

namespace RelAlgebra.Db
{
    public interface IDb
    {
        IEnumerable<RawBsonDocument> ReadAll();
        void WriteAll(IEnumerable<RawBsonDocument> items);
        void StartWrite();
        void Write(RawBsonDocument item);
        void Write(IEnumerable<RawBsonDocument> items);
        void EndWrite();

        byte[] ToByteArray();
    }
}