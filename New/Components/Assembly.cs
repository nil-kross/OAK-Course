using System;

namespace Course.Components {
    public abstract class Assembly : Component {
        protected override String Dimension {
            get { return "SLDASM"; }
        }
    }
}