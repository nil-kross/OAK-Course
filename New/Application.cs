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
                    var assemblyPathwayString = @"Assembly\Assembly 2.SLDASM";

                    assemblyDocument = (AssemblyDoc)SolidWorksApi.OpenDocument(this.solidWorks, DocumentTypes.Assembly, Pathway.Resolve(assemblyPathwayString));

                    if (assemblyDocument == null) {
                        Message.Warning("не удалось открыть сборку " + assemblyPathwayString + "!");
                        Message.Info("Создаю новую сборку..");
                        SolidWorksApi.CreateNewAssembly(this.solidWorks);
                    } else {
                        Message.Info("Удалось открыть сборку " + assemblyPathwayString + "!");
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

                    if (true) {
                        var finger = new CustomUnit("Locator1");

                        var modelDocument = SolidWorksApi.InsertComponent(finger, this.solidWorks, assemblyDocument);
                        var componentsList = SolidWorksApi.FindComponents(finger, assemblyDocument);
                        var component = componentsList[0];
                        var targetFace = SolidWorksApi.FindCylinder(SolidWorksApi.GetFaces(component), new Double[7] { 0, 0.05, 0, 0, -1, 0, 0.00999 });
                        var isDone = SolidWorksApi.Mate(first, targetFace, Mates.Concentric, Aligns.Align, assemblyDocument);

                        isSmthChanged = true;
                    }
                    if (true) {
                        var finger = new CustomUnit("Locator2");

                        var modelDocument = SolidWorksApi.InsertComponent(finger, this.solidWorks, assemblyDocument);
                        var componentsList = SolidWorksApi.FindComponents(finger, assemblyDocument);
                        var component = componentsList[0];
                        var targetFace = SolidWorksApi.FindCylinder(SolidWorksApi.GetFaces(component), new Double[7] { 0, 0.021, 0, 0, -1, 0, 0.015 });
                        var isDone = SolidWorksApi.Mate(second, targetFace, Mates.Concentric, Aligns.Align, assemblyDocument);

                        isSmthChanged = true;
                    }
                    if (true) {
                        var penek = new CustomUnit("Locator7");

                        var modelDocument = SolidWorksApi.InsertComponent(penek, this.solidWorks, assemblyDocument);
                        var componentsList = SolidWorksApi.FindComponents(penek, assemblyDocument);
                        var component = componentsList[0];
                        var targetFace = SolidWorksApi.FindFace(SolidWorksApi.GetFaces(component), new Point(0, 1, 0));
                        var isDone = SolidWorksApi.Mate(third, targetFace, Mates.Coincident, Aligns.AntiAlign, assemblyDocument);

                        isSmthChanged = true;
                    }
                } else {
                    Message.Error("Выбранные плоскости не удовлетворяют заданию!");
                }

                if (isSmthChanged) {
                    var key = Input.Key("Нажмите [Space], чтобы закрыть все документы деталей и сборок..");

                    if (key == ConsoleKey.Spacebar) {
                        return;
                    }

                    Message.Info("Закрываю все документы деталей и сборок..");
                    SolidWorksApi.CloseAssemblies(this.solidWorks);
                }
            }
        }
    }
}