using System;
using MongoDB.Driver;

namespace MongoDB.Web.Internal
{
    public class MongoConnectionProvider : IMongoConnectionProvider
    {
        public MongoCollection GetCollection(string connectionString, string database, string collection)
        {
            return MongoServer.Create(connectionString)
                .GetDatabase(database)
                .GetCollection(collection);
        }
    }
}