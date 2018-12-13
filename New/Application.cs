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
using SolidWorks = SolidWorks.Interop.sldworks.SldWorks;

namespace Course
{
    public class Application
    {
        private SolidWorksApi api = null;

        public Application() {
            this.api = SolidWorksApi.GetSolidWorks();
            while (this.api == null) {
                Message.Info("Пожалуйста, запустите приложение SolidWorks.");
                Input.Key("После этого, нажмите любую клавишу для продолжения..");
            }
        }

        public void Start()
        {
            var insertedComponentsList = new List<Component>();
            var isSmthChanged = false;

            if (this.api == null)
            {
                Message.Error("Не удалось получить дескриптор SolidWorks, завершаю выполнение!..");
                return;
            } else {
                Assembly assembly = null;
                Face2 first = null; // Отв.1 // Опорная
                Face2 second = null; // Отв. 2 // Опроная 2
                Face2 third = null; // Пл-ть // Уст. база

                while (!api.TryGetActiveAssembly()) {
                    var assemblyDocument = (AssemblyDoc)api.OpenDocument(new ExampleAssembly(), DocumentTypes.Assembly);

                    if (assemblyDocument == null) {
                        Message.Warning("не удалось открыть сборку " + assembly.Pathway + "!");
                        Message.Info("Создаю новую сборку..");
                        api.CreateNewAssembly();
                    } else {
                        Message.Info("Удалось открыть сборку " + assembly.Pathway + "!");
                    }
                }

                while (api.GetSelectedObjects() == null) {
                    Message.Warning("Не удалось найти выбранные плоскости!");
                    Input.Key("Выберите плоскости:" + System.Environment.NewLine + "[Продолжить]");
                }

                {
                    var selectedObjectsList = api.GetSelectedObjects();

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
                        var modelDocument = api.InsertComponent(finger);
                        var componentsList = api.FindComponents(finger);
                        var component = componentsList.FirstOrDefault();
                        var targetFace = api.FindCylinderByParams(api.GetFaces(component), new Double[7] { 0, 0.05, 0, 0, -1, 0, 0.00999 });
                        var isDone = api.Mate(first, targetFace, Mates.Concentric, Aligns.AntiAlign);
                        if (true) {
                            var plane = api.FindPlaneByParams(api.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });
                            api.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign);
                        }

                        var cylinder = new Cylinder(first);
                        {
                            api.SetEquation(component, 1, new Equation() { Name = "D", Value = cylinder.Radius * 2 * 1000 });
                        }
                        insertedComponentsList.Add(finger);
                        isSmthChanged = true;
                    }
                    if (true) {
                        var modelDocument = api.InsertComponent(prism);
                        var componentsList = api.FindComponents(prism);
                        var component = componentsList.FirstOrDefault();
                        var targetFace = api.FindCylinderByParams(api.GetFaces(component), new Double[7] { 0, 0.021, 0, 0, -1, 0, 0.015 });
                        var isDone = api.Mate(second, targetFace, Mates.Concentric, Aligns.AntiAlign);
                        if (true) {var plane = api.FindPlaneByParams(api.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });
                            api.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign);
                        }

                        var cylinder = new Cylinder(second);
                        {
                            api.SetEquation(component, 1, new Equation() { Name = "D", Value = cylinder.Radius * 2 * 1000 });
                        }
                        insertedComponentsList.Add(prism);
                        isSmthChanged = true;
                    }
                    if (true) {
                        for (var c = 0; c < 3; c++) {
                            var modelDocument = api.InsertComponent(boss);
                            var componentsList = api.FindComponents(boss, c + 1);
                            var component = componentsList.FirstOrDefault();
                            var targetFace = api.FindPlaneByNormal(api.GetFaces(component), new Point(0, 1, 0));
                            var isDone = api.Mate(third, targetFace, Mates.Coincident, Aligns.AntiAlign);

                            insertedComponentsList.Add(boss);
                            isSmthChanged = true;
                        }
                    }
                } else {
                    Message.Error("Выбранные плоскости не удовлетворяют заданию!");
                }

                Message.Info("Выполнение алгоритма завершено!");
                if (isSmthChanged) {
                    var key = Input.Key("Нажмите [Space], чтобы закрыть все документы деталей и сборок..");

                    if (key != ConsoleKey.Spacebar) {
                        return;
                    }

                    Message.Info("Закрываю все документы деталей и сборок..");
                    api.CloseAssemblies();
                }
            }
        }
    }
}