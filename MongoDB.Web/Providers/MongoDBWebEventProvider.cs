using System.Collections.Specialized;
using System.Linq;
using System.Web.Management;
using MongoDB.Driver;

namespace MongoDB.Web.Providers
{
    public class MongoDBWebEventProvider : BufferedWebEventProvider
    {
        private MongoCollection mongoCollection;

        public override void Initialize(string name, NameValueCollection config)
        {
            this.mongoCollection = MongoDatabase.Create(ConnectionHelper.GetDatabaseConnectionString(config)).GetCollection(config["collection"] ?? "WebEvents");

            config.Remove("collection");
            config.Remove("connectionString");
            config.Remove("database");

            base.Initialize(name, config);
        }

        public override void ProcessEventFlush(WebEventBufferFlushInfo flushInfo)
        {
            this.mongoCollection.InsertBatch<WebEvent>(flushInfo.Events.Cast<WebBaseEvent>().ToList().ConvertAll<WebEvent>(WebEvent.FromWebBaseEvent));
        }
    }
}