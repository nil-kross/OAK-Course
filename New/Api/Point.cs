using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api {
    public class Point {
        private readonly Double[] valuesArray = null;

        public Double X { get; set; }
        public Double Y { get; set; }
        public Double Z { get; set; }

        public Double this[Int32 index] {
            get {
                if (index >= 0 && index < this.valuesArray.Length) {
                    return this.valuesArray[index];
                } else {
                    throw new ArgumentOutOfRangeException();
                }
            }
            set {
                if (index >= 0 && index < this.valuesArray.Length) {
                    this.valuesArray[index] = value;
                } else {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static Int32 AxisesCount => 3;

        public Point() {
            this.valuesArray = new Double[3] { 0, 0, 0 };
        }

        public Point(Double x, Double y, Double z) : this() {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Double[] ToArray() {
            return this.valuesArray;
        }

        public override String ToString() {
            return String.Format("{0}; {1}; {2}", this.X, this.Y, this.Z);
        }
    }
}