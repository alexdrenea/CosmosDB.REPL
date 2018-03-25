using CosmosDB.Repl.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDB.Repl
{
    class Program
    {
        private CosmosDBConfig _selectedConnection;
        private Dictionary<string, CosmosDBConfig> _connections;
        private Dictionary<string, Func<string, Task>> _actions;

        private ILogger _logger = new ConsoleLogger();
        private List<JObject> _lastResultSet;

        static void Main(string[] args)
        {
            try
            {
                new Program().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run()
        {
            SetupActions();
            _connections = AppSettings.Instance.GetSection<CosmosDBConfig[]>("CosmosDBConfig")?.ToDictionary(d => d.Name);
            if (_connections == null || !_connections.Any())
            {
                _logger.Warning("No Connections defined. Please define a connection in appsettings.json.");
                return;
            }
            _selectedConnection = _connections.First().Value;

            while (true)
            {
                ConsoleEx.WriteLine("Submit Gremlin:", ConsoleColor.DarkYellow);
                var queryString = Console.ReadLine();

                if (queryString == "q") break;

                var queryCommand = queryString.Split(' ').FirstOrDefault();
                if (_actions.ContainsKey(queryCommand))
                {
                    await _actions[queryCommand](queryString);
                }
                else
                {
                    Query(queryString);
                }
            }
        }



        #region Actions

        private void SetupActions()
        {
            _actions = new Dictionary<string, Func<string, Task>>
            {
                { "?", Help },
                { "help", Help },
                { "h", Help },
                { "list", ListConnections },
                { "l", ListConnections },
                { "select", SelectConnection },
                { "s", SelectConnection },
                //{ "csv", ExportCsv }
            };

            ConsoleEx.WriteLine("Type '?' or 'help' for additional commands...", ConsoleColor.DarkGray);
        }

        private async Task Help(string text)
        {
            Console.WriteLine($"Available commands:");
            Console.WriteLine(string.Join(Environment.NewLine, _actions.Keys));
            Console.WriteLine($"---------------");
        }

        private async Task ListConnections(string text)
        {
            Console.WriteLine($"Available connections:");
            Console.WriteLine(string.Join($"{Environment.NewLine  }", _connections.Keys));
            Console.WriteLine($"Selected connection:  {_selectedConnection.Name}");
            Console.WriteLine($"---------------");
        }

        private async Task SelectConnection(string text)
        {
            var name = text.Split(' ').Skip(1).FirstOrDefault();
            if (int.TryParse(name, out var index))
            {
                name = _connections.Keys.ElementAt(index);
            }

            if (name == null || !_connections.ContainsKey(name))
            {
                Console.WriteLine("Can't find connection with the given name.");
            }
            else
            {
                _selectedConnection = _connections[name];
                Console.WriteLine($"Selected {_selectedConnection.Name}");
            }
        }

        private async Task ExportCsv(string text)
        {
            var queryParam = text.Split(' ').Skip(1).FirstOrDefault();
            if (_lastResultSet == null || !_lastResultSet.Any())
            {
                ConsoleEx.WriteLine("No result in cache. run a query first", ConsoleColor.Yellow);
                return;
            }
            var fName = string.IsNullOrEmpty(queryParam) ? "default.csv" : $"{queryParam}.csv";
            var lines = new List<string>();
            lines.Add(string.Join(",", _lastResultSet.FirstOrDefault().Properties().Select(s => s.Name)));
            lines.AddRange(_lastResultSet.Select(r => string.Join(",", r.Properties().Select(p => p.Value))));
            File.WriteAllLines(fName, lines);

            Console.WriteLine($"Exported {_lastResultSet.Count} items to {fName}");
        }

        #endregion Actions

        #region Query

        private async Task Query(string text)
        {
            if (_selectedConnection == null)
            {
                ConsoleEx.WriteLine($"No Connection selected. Use 'select _name_' to select a connection.", ConsoleColor.Red);
                return;
            }
            try
            {
                var sw = new Stopwatch();
                sw.Restart();
                var queryResult = await _selectedConnection.GraphClient.Value.ExecuteGremlingMulti<dynamic>(text);
                if (!queryResult.IsSuccessful)
                {
                    Console.WriteLine($"Query failed! {queryResult.Error}");
                    return;
                }
                sw.Stop();

                _lastResultSet = new List<JObject>();
                foreach (var result in queryResult.Result)
                {
                    if (!result.GetType().IsPrimitive)
                    {
                        _lastResultSet.Add(GraphsonToFlatJObject(result));
                    }
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                }

                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffff")} - Total request charge: {queryResult.RU}. Executed in {(sw.ElapsedMilliseconds / 1000.0).ToString("#.###")}s");
            }
            catch (Exception e)
            {
                ConsoleEx.WriteLine($"Exeption running query.{Environment.NewLine}{e.Message}", ConsoleColor.Red);
            }
        }

        public static JObject GraphsonToFlatJObject(dynamic obj)
        {
            var instance = new JObject();
            foreach (var p in obj)
            {
                if (p.Key == "properties")
                {
                    foreach (var sp in p.Value)
                    {
                        instance[sp.Key] = ((sp.Value as IEnumerable<object>).First() as Dictionary<string, object>)["value"].ToString();
                    }
                }
                else
                {
                    instance[p.Key] = (p.Value as JToken)?.Values()?.FirstOrDefault()?.ToString() ?? p.Value.ToString();
                }
            }
            return instance;
        }

        #endregion
    }
}

