using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SW = SolidWorks.Interop.sldworks.SldWorks;

namespace Course
{
    public static class SolidWorksApi
    {
        private static String swApplicationNameString = "SldWorks.Application";

        public static SW GetSolidWorks()
        {
            SW solidWorks = null;

            try
            {
                solidWorks = (SW)Marshal.GetActiveObject(SolidWorksApi.swApplicationNameString);
            }
            catch {}

            return solidWorks;
        }
    }
}