using Course.Api;
using Course.Components;
using Course.Debug;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Component = Course.Components.Component;

namespace Course {
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

                while (!this.api.TryGetActiveAssembly()) {
                    var assemblyDocument = (AssemblyDoc)this.api.OpenDocument(new ExampleAssembly(), DocumentTypes.Assembly);

                    if (assemblyDocument == null) {
                        Message.Warning("не удалось открыть сборку " + assembly.Pathway + "!");
                        Message.Info("Создаю новую сборку..");
                        this.api.CreateNewAssembly();
                    } else {
                        Message.Info("Удалось открыть сборку " + assembly.Pathway + "!");
                    }
                }

                while (this.api.GetSelectedObjects() == null) {
                    Message.Warning("Не удалось найти выбранные плоскости!");
                    Input.Key("Выберите плоскости:" + System.Environment.NewLine + "[Продолжить]");
                }

                {
                    var selectedObjectsList = this.api.GetSelectedObjects();

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
                        var modelDocument = this.api.InsertComponent(finger);
                        var componentsList = this.api.FindComponents(finger);
                        var component = componentsList.FirstOrDefault();
                        var targetFace = this.api.FindCylinderByParams(this.api.GetFaces(component), new Double[7] { 0, 0.05, 0, 0, -1, 0, 0.00999 });
                        var isDone = this.api.Mate(first, targetFace, Mates.Concentric, Aligns.AntiAlign);
                        if (true) {
                            var plane = this.api.FindPlaneByParams(this.api.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });
                            this.api.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign);
                        }

                        var cylinder = new Cylinder(first);
                        {
                            this.api.SetEquation(component, 1, new Equation() { Name = "D", Value = cylinder.Radius * 2 * 1000 });
                        }
                        insertedComponentsList.Add(finger);
                        isSmthChanged = true;
                    }
                    if (true) {
                        var modelDocument = this.api.InsertComponent(prism);
                        var componentsList = this.api.FindComponents(prism);
                        var component = componentsList.FirstOrDefault();
                        var targetFace = this.api.FindCylinderByParams(this.api.GetFaces(component), new Double[7] { 0, 0.021, 0, 0, -1, 0, 0.015 });
                        var isDone = this.api.Mate(second, targetFace, Mates.Concentric, Aligns.AntiAlign);
                        if (true) {
                            var plane = this.api.FindPlaneByParams(this.api.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });

                            this.api.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign);
                        }

                        var cylinder = new Cylinder(second);
                        {
                            this.api.SetEquation(component, 1, new Equation() { Name = "D", Value = cylinder.Radius * 2 * 1000 });
                        }
                        insertedComponentsList.Add(prism);
                        isSmthChanged = true;
                    }
                    if (true) {
                        for (var c = 0; c < 3; c++) {
                            var modelDocument = this.api.InsertComponent(boss);
                            var componentsList = this.api.FindComponents(boss, c + 1);
                            var component = componentsList.FirstOrDefault();
                            var targetFace = this.api.FindPlaneByNormal(this.api.GetFaces(component), new Point(0, 1, 0));
                            var isDone = this.api.Mate(third, targetFace, Mates.Coincident, Aligns.AntiAlign);

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
                    this.api.CloseAssemblies();
                }
            }
        }
    }
}