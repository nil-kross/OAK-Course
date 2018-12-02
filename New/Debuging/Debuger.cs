using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Debug {
    public static class Debuger {
        public static void Show(Object @object) {
            Type type = @object.GetType();
            var textString = "";

            Console.WriteLine();
            textString = String.Concat(textString, "} ", type.Name);
            foreach (var property in type.GetProperties()) {
                textString = String.Concat(textString, '\n', "\t{0},", property.Name);
            }
            textString = String.Concat(textString, '\n', "}");
            Typewriter.Output(textString, ConsoleColor.DarkGreen);
        }
    }
}