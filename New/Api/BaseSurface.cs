using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api
{
    public class BaseSurface : ISurface
    {
        protected readonly IFace2 face = null;
        protected readonly ISurface surface = null;

        public BaseSurface(IFace2 face)
        {
            this.face = face;
            this.surface = face.IGetSurface();
        }
    }
}
