using SolidWorks.Interop.sldworks;
using System;

namespace Course.Api {
    public class Plane : BaseSurface
    {
        public Point Normal
        {
            get
            {
                var planeParams = this.surface.PlaneParams;

                return new Point(planeParams[0], planeParams[1], planeParams[2]);
            }
        }
        public Point Root
        {
            get
            {
                var planeParams = this.surface.PlaneParams;

                return new Point(planeParams[3], planeParams[4], planeParams[5]);
            }
        }

        public Plane(IFace2 face) : base(face) {}

        public override String ToString()
        {
            return this.surface != null ? String.Format("Normal: {0}, Root: {1}", this.Normal, this.Root) : "null";
        }
    }
}
