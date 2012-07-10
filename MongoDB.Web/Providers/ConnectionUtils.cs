using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Collections.Specialized;
using System.Configuration;
using MongoDB.Bson;

namespace MongoDB.Web.Providers
{
    /// <summary>
    /// MongoDB connection helpers
    /// </summary>
    internal class ConnectionUtils
    {
        /// <summary>
        /// Returns MongoDb collection specified in config setting ("collections")
        /// or default one
        /// </summary>
        /// <param name="config"></param>
        /// <param name="defaultCollection"></param>
        /// <returns></returns>
        public static MongoCollection<BsonDocument> GetCollection(NameValueCollection config, string defaultCollection)
        {
            return GetDatabase(config).GetCollection(config["collection"] ?? defaultCollection);
        }

        /// <summary>
        /// Returns MongoDatabase instance using config settings.
        /// If "database" setting is not specified then it's assumed that
        /// connection string contains database name
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static MongoDatabase GetDatabase(NameValueCollection config)
        {
            string database = config["database"];
            return string.IsNullOrEmpty(database) ?
                MongoDatabase.Create(GetConnectionString(config)) :
                MongoServer.Create(GetConnectionString(config)).GetDatabase(database);
        }

        /// <summary>
        /// Returns connection string to MongoDb by checking whether "connectionString" 
        /// contains connection string name or connection string itself
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetConnectionString(NameValueCollection config)
        {
            string connectionString = null;
            var nameOrConnectionString = config["connectionString"];
            if (!string.IsNullOrEmpty(nameOrConnectionString))
            {
                connectionString = nameOrConnectionString;
                if (ConfigurationManager.ConnectionStrings[nameOrConnectionString] != null)
                {
                    connectionString = ConfigurationManager.ConnectionStrings[nameOrConnectionString].ConnectionString;
                }
            }
            return connectionString ?? "mongodb://localhost";
        }
    }
    }
}
