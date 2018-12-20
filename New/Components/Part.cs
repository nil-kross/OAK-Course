using System;

namespace Course.Components {
    public abstract class Part : Component {
        protected override String Extension {
            get { return "SLDPRT"; }
        }
    }
}