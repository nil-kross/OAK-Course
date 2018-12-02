using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Component {
    public abstract class Component {
        protected abstract String FolderPathwayString { get; }
        public abstract String FileName { get; }
        public String Name { get {
                return this.GetType().Name;
            }
        }
        public String FileNamePathway {
            get {
                return String.Format(@"{0}\{1}.SLDPRT", this.FolderPathwayString, this.FileName);
            }
        }
    }
}