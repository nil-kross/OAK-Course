using Course.Api;
using Course.Components;
using Course.Debug;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Component = Course.Components.Component;
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
            var isSmthChanged = false;

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
                Surface first = null; // Отв.1 // Опорная
                Surface second = null; // Отв. 2 // Опроная 2
                Surface third = null; // Пл-ть // Уст. база

                while (!SolidWorksApi.TryGetActiveAssembly(solidWorks, ref assemblyDocument, ref selectionManager)) {
                    var defaultComponent = new CustomComponent("2", "Assembly");

                    assemblyDocument = (AssemblyDoc)SolidWorksApi.OpenDocument(solidWorks, DocumentTypes.Assembly, defaultComponent);

                    if (assemblyDocument == null) {
                        Message.Info("Создаю новую сборку..");
                        SolidWorksApi.CreateNewAssembly(solidWorks);
                    }
                }

                Input.Key("Выберите плоскости:" + System.Environment.NewLine + "[Продолжить]");

                if (selectionManager == null) {
                    Message.Warning("Selection Manager был null!");
                } else {
                    var selectedObjectsList = SolidWorksApi.GetSelectedObjects(selectionManager);

                    if (selectedObjectsList != null) {
                        foreach (var obj in selectedObjectsList) {
                            var face = obj as IFace2;

                            var surface = face.IGetSurface();
                            if (surface.IsCylinder()) {
                                if (first == null) {
                                    first = surface;
                                } else {
                                    second = surface;
                                }
                            } else if (surface.IsPlane()) {
                                third = surface;
                            }
                        }
                    } else {
                        Message.Info("Нет выбранных обьектов!");
                    }
                    
                    if (first != null && second != null && third != null) {
                        var componentsList = new List<Component>()
                        {
                            new CustomComponent("Locator1", "Units"),
                            new CustomComponent("Locator1", "Units"),
                        };

                        Debuger.Show(first);
                        Debuger.Show(second);
                        Debuger.Show(third);

                        var cc = new CustomComponent("Locator1", "Units");
                        var md = SolidWorksApi.InsertComponent(cc, solidWorks, assemblyDocument);
                        

                        // TO DO: вставить локаторы, отрегулировать размеры, сопрячь, добавить плиту
                    }
                    // Dev zone is over!
                }

                if (isSmthChanged) {
                    SolidWorksApi.CloseAssemblies(this.solidWorks);
                }
            }
        }
    }
}