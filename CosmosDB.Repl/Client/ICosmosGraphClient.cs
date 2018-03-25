using Gremlin.Net.Process.Traversal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDB.Repl.Client
{
    public interface ICosmosGraphClient
    {
        Task<CosmosResponse> ExecuteGremlingSingle(string queryString);
        Task<CosmosResponse<T>> ExecuteGremlingSingle<T>(string queryString);
        Task<CosmosResponse<IEnumerable<T>>> ExecuteGremlingMulti<T>(string queryString);
        GraphTraversalSource GetGremlinClientGraph();
    }
}