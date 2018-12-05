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
            while (this.solidWorks == null) {
                Message.Info("Пожалуйста, запустите приложение SolidWorks.");
                Input.Key("После этого, нажмите любую клавишу для продолжения..");
            }

            if (this.solidWorks == null)
            {
                Message.Error("Не удалось получить дескриптор SolidWorks, завершаю выполнение!..");
                return;
            } else {
                AssemblyDoc assemblyDocument = null;
                SelectionMgr selectionManager = null;
                Face2 first = null; // Отв.1 // Опорная
                Face2 second = null; // Отв. 2 // Опроная 2
                Face2 third = null; // Пл-ть // Уст. база

                while (!SolidWorksApi.TryGetActiveAssembly(this.solidWorks, ref assemblyDocument, ref selectionManager)) {
                    var assemblyPathwayString = @"L:\2 Definitions\ОАК\2\Assembly 2.SLDASM";

                    assemblyDocument = (AssemblyDoc)SolidWorksApi.OpenDocument(this.solidWorks, DocumentTypes.Assembly, assemblyPathwayString);

                    if (assemblyDocument == null) {
                        Message.Info("Создаю новую сборку..");
                        SolidWorksApi.CreateNewAssembly(this.solidWorks);
                    } else {
                        Message.Info("Удалось открыть сборку " + assemblyPathwayString);
                    }
                }

                while (SolidWorksApi.GetSelectedObjects(selectionManager) == null) {
                    Message.Warning("Не удалось найти выбранные плоскости!");
                    Input.Key("Выберите плоскости:" + System.Environment.NewLine + "[Продолжить]");
                }

                if (selectionManager == null) {
                    Message.Error("Selection Manager был null!");
                } else {
                    var selectedObjectsList = SolidWorksApi.GetSelectedObjects(selectionManager);

                    if (selectedObjectsList != null) {
                        foreach (var obj in selectedObjectsList) {
                            var face = obj as Face2;

                            var surface = face.IGetSurface();
                            if (surface.IsCylinder()) {
                                if (first == null) {
                                    first = face;
                                } else {
                                    second = face;
                                }
                            } else if (surface.IsPlane()) {
                                third = face;
                            }
                        }
                    } else {
                        Message.Info("Нет выбранных обьектов!");
                        return;
                    }
                }

                if (first != null && second != null && third != null) {
                    var selectedFacesList = new List<Face2>() {
                        first,
                        second,
                        third
                    };

                    if (false) {
                        var baseFinger = new CustomUnit("Locator1");

                        var md = SolidWorksApi.InsertComponent(baseFinger, this.solidWorks, assemblyDocument);
                        var c = SolidWorksApi.FindComponents(baseFinger, assemblyDocument)[0];
                    }
                    if (true) {
                        var penek = new CustomUnit("Locator7");

                        var md = SolidWorksApi.InsertComponent(penek, this.solidWorks, assemblyDocument);
                        var cs = SolidWorksApi.FindComponents(penek, assemblyDocument);
                        var c = cs[0];

                        var m = SolidWorksApi.FindFace(SolidWorksApi.GetFaces(c), new Point(0, 1, 0));
                        var res = SolidWorksApi.Mate(third, m, assemblyDocument);
                    }

                    Message.Text("It's time to stop!");
                    return;

                    // TO DO: вставить локаторы, отрегулировать размеры, сопрячь, добавить плиту
                } else {
                    Message.Error("Выбранные плоскости не удовлетворяют заданию!");
                }

                if (isSmthChanged) {
                    SolidWorksApi.CloseAssemblies(this.solidWorks);
                }
            }
        }
    }
}