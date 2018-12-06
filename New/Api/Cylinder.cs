using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api
{
    public class Cylinder : BaseSurface
    {
        public Point Origin
        {
            get
            {
                var cylinderParams = this.surface.CylinderParams;

                return new Point(cylinderParams[0], cylinderParams[1], cylinderParams[2]);
            }
        }
        public Point Axis
        {
            get
            {
                var cylinderParams = this.surface.CylinderParams;

                return new Point(cylinderParams[3], cylinderParams[4], cylinderParams[5]);
            }
        }
        public Double Radius
        {
            get
            {
                var cylinderParams = this.surface.CylinderParams;

                return cylinderParams[6];
            }
        }

        public Cylinder(IFace2 face) : base(face) {}

        public override string ToString()
        {
            return this.surface != null ? String.Format("R: {2}, Origin: {0}, Axis: {1}", this.Origin, this.Axis, this.Radius) : null;
        }
    }
}
