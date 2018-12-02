using Course.Api;
using Course.Debug;
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

                if (!SolidWorksApi.TryGetActiveAssembly(solidWorks, ref assemblyDocument, ref selectionManager)) {
                    var assemblyPathwayString = @"L:\2 Definitions\ОАК\2\Assembly 2.SLDASM";
                    
                    assemblyDocument = (AssemblyDoc)SolidWorksApi.OpenDocument(solidWorks, DocumentTypes.Assembly, assemblyPathwayString);

                    if (assemblyDocument == null) {
                        Message.Info("Создаю новую сборку..");
                        SolidWorksApi.CreateNewAssembly(solidWorks);
                    }
                }

                Input.Key("Выберите плоскости:" + System.Environment.NewLine + "[Продолжить]");

                {
                    var selectedObjectsList = SolidWorksApi.GetSelectedObjects(selectionManager);

                    if (selectedObjectsList != null) {
                        foreach (var obj in selectedObjectsList) {
                            Message.Text(obj.ToString());
                            Debuger.Show(obj);
                        }
                    } else {
                        Message.Warning("Нет выбранных обьектов!");
                    }
                }

                SolidWorksApi.CloseAssemblies(this.solidWorks);
            }
        }
    }
}