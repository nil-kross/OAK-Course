using SolidWorks.Interop.sldworks;
using System;

namespace Course.Api {
    public class BaseSurface
    {
        protected readonly IFace2 face = null;
        protected readonly ISurface surface = null;

        public BaseSurface(IFace2 face)
        {
            this.face = face;
            this.surface = face.GetSurface();
        }
    }
}