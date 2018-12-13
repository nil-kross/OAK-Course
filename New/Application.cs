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
            var insertedComponentsList = new List<Component>();
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
                Assembly assembly = null;
                AssemblyDoc assemblyDocument = null;
                SelectionMgr selectionManager = null;
                ModelDoc2 document = null;
                Face2 first = null; // Отв.1 // Опорная
                Face2 second = null; // Отв. 2 // Опроная 2
                Face2 third = null; // Пл-ть // Уст. база

                while (!SolidWorksApi.TryGetActiveAssembly(this.solidWorks, ref assemblyDocument, ref selectionManager, ref document)) {
                    assembly = new ExampleAssembly();
                    assemblyDocument = (AssemblyDoc)SolidWorksApi.OpenDocument(this.solidWorks, DocumentTypes.Assembly, assembly);

                    if (assemblyDocument == null) {
                        Message.Warning("не удалось открыть сборку " + assembly.Pathway + "!");
                        Message.Info("Создаю новую сборку..");
                        SolidWorksApi.CreateNewAssembly(this.solidWorks);
                    } else {
                        Message.Info("Удалось открыть сборку " + assembly.Pathway + "!");
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
                                var cylinder = new Cylinder(face);

                                if (first == null) {
                                    first = face;
                                } else {
                                    second = face;
                                }
                            } else if (surface.IsPlane()) {
                                var plane = new Plane(face);

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
                    var finger = new CustomUnit("Locator1");
                    var prism = new CustomUnit("Locator2");
                    var boss = new CustomUnit("Locator7");

                    if (true) {
                        var modelDocument = SolidWorksApi.InsertComponent(finger, this.solidWorks, assemblyDocument);
                        var componentsList = SolidWorksApi.FindComponents(finger, assemblyDocument);
                        var component = componentsList.FirstOrDefault();
                        var targetFace = SolidWorksApi.FindCylinder(SolidWorksApi.GetFaces(component), new Double[7] { 0, 0.05, 0, 0, -1, 0, 0.00999 });
                        var isDone = SolidWorksApi.Mate(first, targetFace, Mates.Concentric, Aligns.AntiAlign, assemblyDocument);
                        if (true) {
                            var plane = SolidWorksApi.FindPlane(SolidWorksApi.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });
                            SolidWorksApi.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign, assemblyDocument);
                        }

                        var cylinder = new Cylinder(first);
                        {
                            SolidWorksApi.SetEquation(component, 1, new Equation() { Name = "D", Value = cylinder.Radius * 2 * 1000 });
                        }
                        insertedComponentsList.Add(finger);
                        isSmthChanged = true;
                    }
                    if (true) {
                        var modelDocument = SolidWorksApi.InsertComponent(prism, this.solidWorks, assemblyDocument);
                        var componentsList = SolidWorksApi.FindComponents(prism, assemblyDocument);
                        var component = componentsList.FirstOrDefault();
                        var targetFace = SolidWorksApi.FindCylinder(SolidWorksApi.GetFaces(component), new Double[7] { 0, 0.021, 0, 0, -1, 0, 0.015 });
                        var isDone = SolidWorksApi.Mate(second, targetFace, Mates.Concentric, Aligns.AntiAlign, assemblyDocument);
                        if (true) {var plane = SolidWorksApi.FindPlane(SolidWorksApi.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });
                            SolidWorksApi.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign, assemblyDocument);
                        }

                        var cylinder = new Cylinder(second);
                        {
                            SolidWorksApi.SetEquation(component, 1, new Equation() { Name = "D", Value = cylinder.Radius * 2 * 1000 });
                        }
                        insertedComponentsList.Add(prism);
                        isSmthChanged = true;
                    }
                    if (true) {
                        for (var c = 0; c < 3; c++) {
                            var modelDocument = SolidWorksApi.InsertComponent(boss, this.solidWorks, assemblyDocument);
                            var componentsList = SolidWorksApi.FindComponents(boss, assemblyDocument, c + 1);
                            var component = componentsList.FirstOrDefault();
                            var targetFace = SolidWorksApi.FindFace(SolidWorksApi.GetFaces(component), new Point(0, 1, 0));
                            var isDone = SolidWorksApi.Mate(third, targetFace, Mates.Coincident, Aligns.AntiAlign, assemblyDocument);

                            insertedComponentsList.Add(boss);
                            isSmthChanged = true;
                        }
                    }
                } else {
                    Message.Error("Выбранные плоскости не удовлетворяют заданию!");
                }

                Message.Info("Выполнение алгоритма завершено!");
                document.IActiveView.Activate();
                if (isSmthChanged) {
                    var key = Input.Key("Нажмите [Space], чтобы закрыть все документы деталей и сборок..");

                    if (key != ConsoleKey.Spacebar) {
                        return;
                    }

                    Message.Info("Закрываю все документы деталей и сборок..");
                    SolidWorksApi.CloseAssemblies(this.solidWorks);
                }
            }
        }
    }
}