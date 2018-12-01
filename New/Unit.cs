using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course {
    public abstract class Unit {
        private static String folderPathwayString = "Units";

        public abstract String FileName { get; }
        public String FileNamePathway {
            get {
                return String.Format(@"{0}\{1}.SLDPRT", Unit.folderPathwayString, this.FileName);
            }
        }
    }
}