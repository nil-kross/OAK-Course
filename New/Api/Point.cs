using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api {
    public class Point {
        public Double X { get; set; }
        public Double Y { get; set; }
        public Double Z { get; set; }

        public Point(Double x = 0, Double y = 0, Double z = 0) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override String ToString() {
            return String.Format("{0}; {1}; {2}", this.X, this.Y, this.Z);
        }
    }
}