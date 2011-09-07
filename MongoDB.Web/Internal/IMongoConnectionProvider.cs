using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoDB.Web.Internal
{
    public interface IMongoConnectionProvider
    {
        IMongoCollection GetCollection(string connectionString, string database, string collection);
    }

    public interface IMongoCollection
    {
        void EnsureIndex(params string[] keyNames);
        int Count(IMongoQuery query);
        MongoCursor<T> FindAs<T>(IMongoQuery query);
        T FindOneAs<T>(IMongoQuery query);
        void Insert<T>(T document);
        void InsertBatch<T>(IEnumerable<T> documents);
        SafeModeResult Remove(IMongoQuery query);
        SafeModeResult Update(IMongoQuery query, IMongoUpdate update);
        void Save(BsonDocument document);
    }
}