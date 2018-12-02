using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Component {
    public abstract class Unit : Component {
        protected override String FolderPathwayString => "Units";
    }
}