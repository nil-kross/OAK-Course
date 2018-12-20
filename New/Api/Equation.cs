using System;

namespace Course.Api {
    public class Equation {
        public String Name { get; set; }
        public Double Value { get; set; }

        public override String ToString() {
            return String.Format("{0}{1}{0}={2}", '"', this.Name, (Int32)(this.Value * 1000));
        }
    }
}