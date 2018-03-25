using CosmosDB.Repl.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDB.Repl.Utilities
{
    /// <summary>
    /// Represents a collection of configuration settings for DocDb connection
    /// </summary>
    public class CosmosDBConfig
    {
        private Lazy<ICosmosGraphClient> _graphClient;

        public CosmosDBConfig()
        {
            _graphClient = new Lazy<ICosmosGraphClient>(
               () =>
               {
                   var endpoint = $"{Account}.gremlin.cosmosdb.azure.com";
                   return CosmosGraphClient.GetCosmosClient(endpoint, AuthKey, Database, Collection);
               });
        }

        /// <summary>
        /// Friendly name for the connection (will be shown as the connection name)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Account name - do not include the "gremlin.cosmosdb.azure.net" or "documents.azure.com"
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Autentication key for the account  (you can use a read-only key)
        /// </summary>
        public string AuthKey { get; set; }
        /// <summary>
        /// Database to connect to
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// Collection to connect to
        /// </summary>
        public string Collection { get; set; }


        public Lazy<ICosmosGraphClient> GraphClient { get { return _graphClient; } }
    }
}
