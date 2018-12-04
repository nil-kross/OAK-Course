using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Components
{
    public class CustomUnit : Unit
    {
        private readonly String fileNameString = null; 

        public override string FileName => this.fileNameString;

        public CustomUnit(String fileName)
        {
            this.fileNameString = fileName;
        }
    }
}