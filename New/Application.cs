using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
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
                Message.Warning("Пожалуйста, запустите приложение SolidWorks.");
                return;
            }
            else
            {
                AssemblyDoc assemblyDocument = null;
                SelectionMgr selectionManager = null;

                if (SolidWorksApi.TryGetActiveAssembly(solidWorks, ref assemblyDocument, ref selectionManager))
                {
                    var selectedObjectsList = SolidWorksApi.GetSelectedObjects(selectionManager);

                    if (selectedObjectsList != null) {
                        foreach (var obj in selectedObjectsList) {
                            Message.Text(obj.ToString());
                        }
                    } else {
                        Message.Warning("Нет выбранных обьектов!");
                    }
                }
                else
                {
                    Message.Warning("Пожалуйста, откройте сборку!");
                }
            }
        }
    }
}