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
                Message.Info("Пожалуйста, запустите приложение SolidWorks..");
                Input.Key("[Продолжить]");
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
                    assembly = new ExampleAssembly();

                    {
                        var assemblyDocument = (AssemblyDoc)this.api.OpenDocument(assembly, DocumentTypes.Assembly);

                        if (assemblyDocument == null) {
                            Message.Warning("не удалось открыть сборку " + assembly.Pathway + "!");
                            Message.Info("Создаю новую сборку..");
                            this.api.CreateNewAssembly();
                        } else {
                            Message.Info("Удалось открыть сборку " + assembly.Pathway + "!");
                        }
                    }
                }

                while (this.api.GetSelectedObjects() == null) {
                    Message.Warning("Не удалось найти выбранные плоскости!");
                    Input.Key("Пожалуйства, выберите плоскости!" + System.Environment.NewLine + "[Продолжить]");
                }

                {
                    var cylinderFacesList = new List<Face2>();
                    var planeFacesList = new List<Face2>();
                    var selectedObjectsList = this.api.GetSelectedObjects();

                    if (selectedObjectsList != null) {
                        foreach (var obj in selectedObjectsList) {
                            var face = obj as Face2;

                            if (face != null) {
                                var surface = face.IGetSurface();
                                if (surface.IsCylinder()) {
                                    cylinderFacesList.Add(face);
                                } else if (surface.IsPlane()) {
                                    planeFacesList.Add(face);
                                }
                            }
                        }
                    } else {
                        Message.Info("Нет выбранных обьектов!");
                        return;
                    }
                    {
                        var isFits = false;

                        if (planeFacesList.Count == 1 && cylinderFacesList.Count == 2) {
                            if (SolidWorksApi.Compare(cylinderFacesList[0], cylinderFacesList[1])) {
                                first = cylinderFacesList[0];
                                second = cylinderFacesList[1];
                            } else {
                                first = cylinderFacesList[1];
                                second = cylinderFacesList[0];
                            }
                            third = planeFacesList[0];
                            var pl = new Plane(third);

                            if (SolidWorksApi.Compare(third, first) && SolidWorksApi.Compare(third, second)) {
                                Message.Info("Плоскость является наиболее развитой базой.");
                                isFits = true;
                            }
                            if (
                                (SolidWorksApi.Compare(first, second) && SolidWorksApi.Compare(first, third)) ||
                                (SolidWorksApi.Compare(second, first) && SolidWorksApi.Compare(second, third))
                            ) {
                                Message.Error("Цилиндрическая поверхность является наиболее развитой базой!");
                            }
                        }

                        if (isFits) {
                            var selectedFacesList = new List<Face2>() {
                                first,
                                second,
                                third
                            };
                            var finger = new CustomUnit("Locator1");
                            var prism = new CustomUnit("Locator2");
                            var boss = new CustomUnit("Locator7");
                            var boundaryBox = SolidWorksApi.GetBox(this.api.ModelDocument);
                            var height = (20 / 100.0) * boundaryBox.Dz;
                            var components = new List<Component>() {
                                finger,
                                prism,
                                boss,
                                boss,
                                boss
                            };

                            foreach (var component in components) {
                                var modelDocument = this.api.InsertComponent(component);
                            }
                            Input.Key("Интерактивная пауза.." + '\n' + "[Продолжить]");

                            if (true) {
                                var componentsList = this.api.FindComponents(finger);
                                var component = componentsList.FirstOrDefault();
                                var targetFace = this.api.FindCylinderByParams(this.api.GetFaces(component), new Double[7] { 0, 0.05, 0, 0, -1, 0, 0.00999 });
                                var isDone = this.api.Mate(first, targetFace, Mates.Concentric, Aligns.AntiAlign);
                                if (true) {
                                    var plane = this.api.FindPlaneByParams(this.api.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });
                                    this.api.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign);
                                }
                                var bounds = SolidWorksApi.GetBounds(first);
                                var cylinder = new Cylinder(first);
                                {
                                    this.api.SetEquation(component, new Equation() {
                                        Name = "H",
                                        Value = height
                                    });
                                    this.api.SetEquation(component, new Equation() {
                                        Name = "D",
                                        Value = cylinder.Radius * 2
                                    });
                                    this.api.SetEquation(component, new Equation() {
                                        Name = "L",
                                        Value = bounds.V
                                    });
                                }
                                insertedComponentsList.Add(finger);
                                isSmthChanged = true;
                            }
                            if (true) {
                                var componentsList = this.api.FindComponents(prism);
                                var component = componentsList.FirstOrDefault();
                                var targetFace = this.api.FindCylinderByParams(this.api.GetFaces(component), new Double[7] { 0, 0.021, 0, 0, -1, 0, 0.015 });
                                var isDone = this.api.Mate(second, targetFace, Mates.Concentric, Aligns.AntiAlign);
                                if (true) {
                                    var plane = this.api.FindPlaneByParams(this.api.GetFaces(component), new Double[6] { 0, -1, 0, 0, 0, 0 });

                                    this.api.Mate(plane, third, Mates.Coincident, Aligns.AntiAlign);
                                }
                                var bounds = SolidWorksApi.GetBounds(second);
                                var cylinder = new Cylinder(second);
                                {
                                    this.api.SetEquation(component, new Equation() {
                                        Name = "H",
                                        Value = boundaryBox.Dz * height
                                    });
                                    this.api.SetEquation(component, new Equation() {
                                        Name = "D",
                                        Value = cylinder.Radius * 2
                                    });
                                    this.api.SetEquation(component, new Equation() {
                                        Name = "L",
                                        Value = bounds.V
                                    });
                                }
                                insertedComponentsList.Add(prism);
                                isSmthChanged = true;
                            }
                            if (true) {
                                for (var c = 0; c < 3; c++) {
                                    var componentsList = this.api.FindComponents(boss, c + 1);
                                    var component = componentsList.FirstOrDefault();
                                    var targetFace = this.api.FindPlaneByNormal(this.api.GetFaces(component), new Point(0, 1, 0));
                                    var isDone = this.api.Mate(third, targetFace, Mates.Coincident, Aligns.AntiAlign);
                                    {
                                        var bounds = SolidWorksApi.GetBounds(third);
                                        var bossDiameterValue = (20 / 100.0) * Math.Sqrt(Math.Pow(bounds.U, 2) + Math.Pow(bounds.V, 2));

                                        this.api.SetEquation(component, new Equation() {
                                            Name = "H",
                                            Value = boundaryBox.Dz * height
                                        });
                                        this.api.SetEquation(component, new Equation() {
                                            Name = "D",
                                            Value = bossDiameterValue
                                        });
                                    }
                                    insertedComponentsList.Add(boss);
                                    isSmthChanged = true;
                                }
                            }
                        } else {
                            Message.Error("Выбранные плоскости не удовлетворяют заданию!");
                        }
                    }
                }

                Message.Info("Выполнение алгоритма завершено!");
                if (isSmthChanged) {
                    Message.Text("Нажмите [Space], чтобы закрыть все открытые документы деталей и сборок..");
                    if (ConsoleKey.Spacebar != Input.Key("[Продолжить]")) {
                        return;
                    }

                    Message.Info("Закрываю все открытые документы деталей и сборок..");
                    this.api.CloseAssemblies();
                }
            }
        }
    }
}