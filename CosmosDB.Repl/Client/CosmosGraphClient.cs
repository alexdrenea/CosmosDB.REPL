using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB.Repl.Client
{
    public class CosmosGraphClient : ICosmosGraphClient
    {
        private GremlinServer _server;
        private string _databaseName;
        private string _collectionName;

        private CosmosGraphClient() { }

        public async Task<CosmosResponse> ExecuteGremlingSingle(string queryString)
        {
            try
            {
                using (var client = new GremlinClient(_server))
                {
                    var result = await client.SubmitWithSingleResultAsync<dynamic>(queryString);
                    //TODO: not geting RUs for now.
                    return new CosmosResponse { Result = result, RU = -1 };
                }
            }
            catch (Exception e)
            {
                return new CosmosResponse { Error = e };
            }
        }

        public async Task<CosmosResponse<T>> ExecuteGremlingSingle<T>(string queryString)
        {
            try
            {
                using (var client = new GremlinClient(_server))
                {
                    var result = await client.SubmitWithSingleResultAsync<T>(queryString);
                    return new CosmosResponse<T> { Result = result, RU = -1 };
                }
            }
            catch (Exception e)
            {
                return new CosmosResponse<T> { Error = e };
            }
        }

        public async Task<CosmosResponse<IEnumerable<T>>> ExecuteGremlingMulti<T>(string queryString)
        {
            try
            {
                using (var client = new GremlinClient(_server))
                {

                    var result = await client.SubmitAsync<T>(queryString);
                    return new CosmosResponse<IEnumerable<T>> { Result = result.ToArray(), RU = -1 };
                }
            }
            catch (Exception e)
            {
                return new CosmosResponse<IEnumerable<T>> { Error = e };
            }
        }

        public GraphTraversalSource GetGremlinClientGraph()
        {
            var graph = new Graph();
            return graph.Traversal().WithRemote(new DriverRemoteConnection(new GremlinClient(_server)));
        }


        public static ICosmosGraphClient GetCosmosClient(string account, string dbName, string collectionName, string key)
        {
            var server = new GremlinServer(
                $"{account}.gremlin.cosmosdb.azure.com",
                443,
                enableSsl: true,
                username: $"/dbs/{dbName}/colls/{collectionName}",
                password: key);

            return new CosmosGraphClient
            {
                _server = server,
                _databaseName = dbName,
                _collectionName = collectionName
            };
        }
    }
}
