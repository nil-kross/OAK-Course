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
                textString = String.Concat(textString, '\n', Debuger.WriteMember(property.Name, property.GetValue(@object, null).ToString()));
            }
            textString = String.Concat(textString, '\n', "}");
            Typewriter.Output(textString, ConsoleColor.DarkGreen);
        }

        private static String WriteMember(String name, String value, Int32? indents = 1) {
            String indentString = "";
            
            for (var i = 0; i < indents; i++) {
                indentString += ' ';
            }

            return String.Format("{0}{1}: {2},", indentString, name, value);
        }
    }
}