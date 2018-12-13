using System;

namespace Course.Components {
    public class CustomUnit : Unit
    {
        private readonly String fileNameString = null;

        public override String File
        {
            get { return this.fileNameString; }
        }

        public CustomUnit(String fileName)
        {
            this.fileNameString = fileName;
        }
    }
}