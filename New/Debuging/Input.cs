using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Debug {
    public static class Input {
        public static ConsoleKey Key(String textString) {
            ConsoleKey consoleKey = ConsoleKey.Escape;

            Typewriter.Output(textString, ConsoleColor.White);
            consoleKey = Console.ReadKey().Key;
            Console.WriteLine(">>> {0}", consoleKey.ToString());

            return consoleKey;
        }
    }
}