using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDB.Repl.Utilities
{
    public static class ConsoleEx
    {
        public static void WriteLine(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
