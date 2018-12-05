using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Components {
    public class Part : Component {
        protected override String FolderPathwayString {
            get { return "Parts"; }
        }
        public override String FileName
        {
            get { return "TestPART3"; }
        }
    }
}