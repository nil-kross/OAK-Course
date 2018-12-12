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
                Typewriter.WriteTab(isFirstLine);
                Typewriter.Write(line, consoleColor);
                Console.WriteLine();

                isFirstLine = false;
            }
        }

        private static void WriteTab(Boolean isTimeStamp = false) {
            var format = "[hh:mm.ss] ";

            if (isTimeStamp) {
                Typewriter.Write("[", ConsoleColor.White);
                Typewriter.Write(DateTime.Now.ToString("hh"), ConsoleColor.DarkGray);
                Typewriter.Write(":", ConsoleColor.White);
                Typewriter.Write(DateTime.Now.ToString("mm"), ConsoleColor.DarkGray);
                Typewriter.Write(".", ConsoleColor.White);
                Typewriter.Write(DateTime.Now.ToString("ss"), ConsoleColor.Gray);
                Typewriter.Write("] ", ConsoleColor.White);
            } else {
                var emptyString = "";

                for (var i = 0; i < format.Length; i++) {
                    emptyString += ' ';
                }

                Typewriter.Write(emptyString, ConsoleColor.Black);
            }
        }
    }
}