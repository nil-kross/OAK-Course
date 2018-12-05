using Course.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Components {
    public abstract class Component {
        protected abstract String FolderPathwayString { get; }
        public abstract String FileName { get; }
        public String Name { get {
                return this.GetType().Name;
            }
        }
        public String FileNamePathway {
            get {
                var a = Pathway.Resolve(String.Format("{0}{2}{1}.SLDPRT", this.FolderPathwayString, this.FileName, '\\'));

                return a;
            }
        }

        public override String ToString() {
            return this.FileNamePathway;
        }
    }
}