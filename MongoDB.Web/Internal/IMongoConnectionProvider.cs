using System;
using MongoDB.Driver;

namespace MongoDB.Web.Internal
{
    public interface IMongoConnectionProvider
    {
        MongoCollection GetCollection(string connectionString, string database, string collection);
    }
}