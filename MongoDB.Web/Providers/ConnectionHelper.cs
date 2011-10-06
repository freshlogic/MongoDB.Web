using System;
using System.Collections.Specialized;
using System.Configuration;
using MongoDB.Driver;

namespace MongoDB.Web.Providers
{
    internal class ConnectionHelper
    {
        /// <summary>
        /// Gets the configured connection string.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetConnectionString(NameValueCollection config)
        {
            string connectionString = config["connectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                string appSettingsKey = config["appSettingsConnectionStringKey"];
                connectionString = string.IsNullOrWhiteSpace(appSettingsKey) ? "mongodb://localhost" : ConfigurationManager.AppSettings[appSettingsKey];
            }

            return connectionString;
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetDatabaseName(string connectionString, NameValueCollection config)
        {
            MongoUrl mongoUrl = MongoUrl.Create(connectionString);
            string databaseName = string.IsNullOrEmpty(mongoUrl.DatabaseName)
                                      ? config["database"] ?? "ASPNETDB"
                                      : mongoUrl.DatabaseName;

            return databaseName;
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetDatabaseName(NameValueCollection config)
        {
            return GetDatabaseName(GetConnectionString(config), config);
        }

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        internal static string GetDatabaseConnectionString(NameValueCollection config)
        {
            string connectionString = GetConnectionString(config);
            var builder = new MongoUrlBuilder(connectionString);
            builder.DatabaseName = GetDatabaseName(connectionString, config);

            return builder.ToString();
        }
    }
}
