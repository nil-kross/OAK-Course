using System;

namespace Course.Components {
    public class ExampleAssembly : Assembly {
        public override String File {
            get { return "Example"; }
        }
        protected override String Folder {
            get { return "Assembly"; }
        }
        public override String Name {
            get { return "Part 2"; }
        }
    }
}