using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api {
    public class Box {
        public Point Start { get; set; }
        public Point End { get; set; }
        public Double Dx {
            get {
                return Math.Abs(this.Start.X - this.End.X);
            }
        }
        public Double Dy {
            get {
                return Math.Abs(this.Start.Y - this.End.Y);
            }
        }
        public Double Dz {
            get {
                return Math.Abs(this.Start.Z - this.End.Z);
            }
        }

        public static Box FromArray(Double[] boxParamsArray) {
            var startPoint = new Point(boxParamsArray[0], boxParamsArray[1], boxParamsArray[2]);
            var endPoint = new Point(boxParamsArray[3], boxParamsArray[4], boxParamsArray[5]);

            return new Box() { Start = startPoint, End = endPoint };
        }

        public override String ToString() {
            return String.Format("Dx: {0}; Dy: {1}; Dz: {2}", this.Dx, this.Dy, this.Dz);
        }
    }
}
