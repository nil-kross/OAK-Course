using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Api
{
    public class Target
    {
        public Role Role { get; protected set; }
        public BaseSurface Surface { get; protected set; }
    }
}
