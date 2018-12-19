using System;

namespace Course.Api {
    public class Bounds {
        public Double U { get; set; }
        public Double V { get; set; }

        public override String ToString() {
            return String.Format("U: {0}; V: {1}", this.U, this.V);
        }
    }
}
