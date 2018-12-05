using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api
{
    public static class Pathway
    {
        public static String Resolve(String pathway) {
            return AppDomain.CurrentDomain.BaseDirectory + '\\' + pathway;
        }
    }
}
