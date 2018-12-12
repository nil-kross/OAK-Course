using System;

namespace Course.Components {
    public abstract class Component {
        protected abstract String Dimension { get; }
        protected abstract String Folder { get; }
        public abstract String File { get; }
        public virtual String Name { get {
                return this.GetType().Name;
            }
        }
        public String Pathway {
            get {
                var pathwayString = Api.Pathway.Resolve(String.Format("{0}{2}{1}.{3}", this.Folder, this.File, '\\', this.Dimension));

                return pathwayString;
            }
        }

        public override String ToString() {
            return String.Format(
                "{0}: {1}",
                this.Name,
                this.Pathway
            );
        }
    }
}