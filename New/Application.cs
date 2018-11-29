using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW = SolidWorks.Interop.sldworks.SldWorks;

namespace Course
{
    public class Application
    {
        private SW solidWorks = null;

        public Application()
        {
        }

        public void Start()
        {
            this.solidWorks = SolidWorksApi.GetSolidWorks();

            if (this.solidWorks == null)
            {
                Message.Show("Не удалось получить дескриптор приложения SolidWorks!", MessageType.Error);
                Message.Show("Пожалуйста, запустите приложение SolidWorks.", MessageType.Info);
                return;
            }
        }
    }
}