﻿using System;

namespace Course.Components {
    public abstract class Part : Component {
        protected override String Dimension {
            get { return "SLDPRT"; }
        }
    }
}