using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDB.Repl.Client
{
    public class CosmosResponse : CosmosResponse<object>
    {

    }

    public class CosmosResponse<T>
    {
        public Exception Error { get; set; }

        public bool IsSuccessful { get { return Error == null; } }

        public T Result { get; set; }
        public double RU { get; set; }
    }
}
