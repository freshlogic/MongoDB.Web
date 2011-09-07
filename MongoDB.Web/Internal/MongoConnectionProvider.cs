using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Web.Internal
{
    public class MongoConnectionProvider : IMongoConnectionProvider
    {
        public IMongoCollection GetCollection(string connectionString, string database, string collection)
        {
            return new WrappedMongoCollection(
                MongoServer.Create(connectionString)
                    .GetDatabase(database)
                    .GetCollection(collection));
        }

        class WrappedMongoCollection : IMongoCollection
        {
            private readonly MongoCollection collection;

            public WrappedMongoCollection(MongoCollection collection)
            {
                this.collection = collection;
            }

            public int Count(IMongoQuery query)
            {
                return collection.Count(query);
            }

            public void EnsureIndex(params string[] keyNames)
            {
                collection.EnsureIndex(keyNames);
            }

            public MongoCursor<T> FindAs<T>(IMongoQuery query)
            {
                return collection.FindAs<T>(query);
            }

            public T FindOneAs<T>(IMongoQuery query)
            {
                return collection.FindOneAs<T>(query);
            }

            public void Insert<T>(T document)
            {
                collection.Insert(document);
            }

            public void InsertBatch<T>(IEnumerable<T> documents)
            {
                collection.InsertBatch(documents);
            }

            public SafeModeResult Remove(IMongoQuery query)
            {
                return collection.Remove(query);
            }

            public SafeModeResult Update(IMongoQuery query, IMongoUpdate update)
            {
                return collection.Update(query, update);
            }

            public void Save(BsonDocument document)
            {
                collection.Save(document);
            }
        }
    }
}