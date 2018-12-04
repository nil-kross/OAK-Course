using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Components {
    public abstract class Unit : Component {
        protected override String FolderPathwayString => @"L:\3 Repositories\OAK-Course\New\bin\Debug\Units"; // TO DO: Replace with relative pathway
    }
}