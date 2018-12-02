using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Debug {
    public static class Typewriter {
        public static void Write(String textString, ConsoleColor consoleColor) {
            var oldColor = Console.ForegroundColor;
            
            if (Console.ForegroundColor != consoleColor) {
                Console.ForegroundColor = consoleColor;
            }
            Console.Write(textString);
            if (Console.ForegroundColor != consoleColor) {
                Console.ForegroundColor = oldColor;
            }
        }

        public static void Output(String textString, ConsoleColor consoleColor) {
            var lines = textString.Split('\n');
            var isFirstLine = true;

            foreach (var line in lines) {
                Typewriter.Write(
                    (isFirstLine ? DateTime.Now.ToString("[hh:mm.ss] ") : "           ") + line,
                    consoleColor
                );
                Console.WriteLine();

                isFirstLine = false;
            }
        }
    }
}