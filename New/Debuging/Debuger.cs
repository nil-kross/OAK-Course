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
            var valueString = "";

            if (type.IsArray) {
                foreach (var value in @object as Array) {
                    valueString += value.ToString() + ", ";
                }
            } else {
                valueString = @object.ToString();
            }

            textString += String.Format("{0}: {1}\n", type.Name, valueString);
            textString = String.Concat(textString, "} ");
            foreach (var property in type.GetProperties()) {
                if (property.GetAccessors() != null && property.GetIndexParameters().Count() <= 0) {
                    var value = property.GetValue(@object);
                    
                    textString = String.Concat(textString, '\n', Debuger.WriteMember(property.Name, value.ToString()));
                }
            }
            textString = String.Concat(textString, '\n', "}");
            Typewriter.Output(textString, ConsoleColor.DarkGreen);
        }

        private static String WriteMember(String name, String value, Int32? indents = 1) {
            String indentString = "";
            
            for (var i = 0; i < indents * 2; i++) {
                indentString += ' ';
            }

            return String.Format("{0}{1}: {2},", indentString, name, value);
        }
    }
}