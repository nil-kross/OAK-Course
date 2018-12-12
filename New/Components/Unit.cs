using System;

namespace Course.Components {
    public abstract class Unit : Part {
        protected override String Folder
        {
            get { return "Units"; }
        }
        public override String Name {
            get { return this.File; }
        }
    }
}