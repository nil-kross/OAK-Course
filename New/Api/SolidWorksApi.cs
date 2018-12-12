using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SW = SolidWorks.Interop.sldworks.SldWorks;
using SolidWorks.Interop.swconst;
using Course.Debug;
using Course.Api;
using Course.Components;
using SolidWorks.Interop.sldworks;
using Component = Course.Components.Component;
using System.IO;

namespace Course.Api
{
    public static class SolidWorksApi
    {
        private static String swApplicationNameString = "SldWorks.Application";

        public static SW GetSolidWorks()
        {
            SW solidWorks = null;

            try
            {
                solidWorks = (SW)Marshal.GetActiveObject(SolidWorksApi.swApplicationNameString);
            }
            catch {
                Message.Error("Не удалось получить дескриптор приложения SolidWorks!");
            }

            return solidWorks;
        }

        public static ModelDoc2 OpenDocument(SW solidWorks, DocumentTypes documentType, String fileNamePathway) {
            ModelDoc2 modelDocument = null;

            if (solidWorks != null) {
                try {
                    Message.Text("Пытаюсь открыть сборку " + fileNamePathway + "..");
                    modelDocument = solidWorks.OpenDoc(fileNamePathway, (Int32)documentType);
                } catch {
                    Message.Error("Не удалось открыть указанный файл!");
                }
            }

            return modelDocument;
        }

        public static ModelDoc2 OpenDocument(SW solidWorks, DocumentTypes documentType, Component component)
        {
            return SolidWorksApi.OpenDocument(solidWorks, documentType, component.FileNamePathway);
        }

        public static void CloseAssemblies(SW solidWorks) {
            solidWorks.CloseAllDocuments(true);
        }

        public static AssemblyDoc CreateNewAssembly(SW solidWorks) {
            AssemblyDoc assemblyDocument = null;

            if (solidWorks != null) {
                assemblyDocument = solidWorks.INewAssembly();
            }

            return assemblyDocument;
        }

        public static Boolean TryGetActiveAssembly(SW solidWorks, ref AssemblyDoc assemblyDocument, ref SelectionMgr selectionManager, ref ModelDoc2 modelDocument)
        {
            var isDone = false;

            if (solidWorks != null) {
                isDone = true;

                try
                {
                    ModelView myModelView = null;

                    modelDocument = (ModelDoc2)(solidWorks.ActiveDoc);
                    assemblyDocument = (AssemblyDoc)(modelDocument);
                    selectionManager = modelDocument.SelectionManager;

                    myModelView = (ModelView)(modelDocument.ActiveView);
                    myModelView.FrameState = (Int32)(swWindowState_e.swWindowMaximized);
                }
                catch
                {
                    Message.Error("Не удалось получить активную сборку!");
                    isDone = false;
                    assemblyDocument = null;
                    selectionManager = null;
                    modelDocument = null;
                }
            }

            return isDone;
        }

        public static IList<Object> GetSelectedObjects(SelectionMgr selectionManager)
        {
            IList<Object> selectedObjectsList = null;

            if (selectionManager != null)
            {
                Int32 selectedObjectsAmount = selectionManager.GetSelectedObjectCount();

                if (selectedObjectsAmount > 0)
                {
                    selectedObjectsList = new Object[selectedObjectsAmount];
                }
                for (Int32 i = 1; i <= selectedObjectsAmount; i++)
                {
                    Object selectedObject = selectionManager.GetSelectedObject(i);

                    selectedObjectsList[i - 1] = selectedObject;
                }
            }
            
            return selectedObjectsList;
        }

        public static ModelDoc2 InsertComponent(String filePathway, SW solidWorks, AssemblyDoc assemblyDocument, Point point = null)
        {
            Point centerPoint = point ?? new Point();
            Int32 errorValue = 0;
            Int32 warningValue = 0;
            ModelDoc2 modelDocument = null;

            if (File.Exists(filePathway)) {
                modelDocument = solidWorks.OpenDoc6(filePathway, 1, 1, "", ref errorValue, ref warningValue);
                if (modelDocument != null) {

                    try {
                        Message.Text("Пытаюсь вставить компонент " + filePathway + "..");
                        assemblyDocument.AddComponent(filePathway, centerPoint.X, centerPoint.Y, centerPoint.Z);
                        modelDocument.EditCopy();
                        //modelDocument.Close();
                    } catch {}
                } else {
                    Message.Error("Не удалось открыть деталь из файла '" + filePathway + "'!");
                }
            } else {
                Message.Error("Файл указанного компонента не найден!");
            }

            return modelDocument;
        }

        public static ModelDoc2 InsertComponent(Component component, SW solidWorks, AssemblyDoc assemblyDocument, Point point = null) {
            return SolidWorksApi.InsertComponent(component.FileNamePathway, solidWorks, assemblyDocument, point);
        }

        public static Face2 FindFace(IEnumerable<Face2> faces, Point normal) {
            Face2 face = null;

            if (faces != null) {
                foreach (var currFace in faces) {
                    var surface = currFace.GetSurface() as Surface;

                    if (!surface.IsCylinder()) {
                        Double[] normalValuesArray = currFace.Normal;

                        if (SolidWorksApi.CompareParams(normalValuesArray, normal.ToArray(), Point.AxisesCount)) {
                            face = currFace;
                        }
                    }
                }
            } else {
                Message.Error("Перечисление граней было null!");
            }

            return face;
        }

        public static Face2 FindPlane(IEnumerable<Face2> faces, Double[] etalon) {
            Face2 face = null;

            if (faces != null) {
                foreach (var currFace in faces) {
                    Surface plane = currFace.IGetSurface();

                    if (plane != null && plane.IsPlane()) {
                        Double[] planeParamsArray = plane.PlaneParams;

                        if (SolidWorksApi.CompareParams(planeParamsArray, etalon, 6)) {
                            face = currFace;
                        }
                    }
                }
            } else {
                Message.Error("Перечисление граней было null!");
            }

            return face;
        }

        public static Face2 FindCylinder(IEnumerable<Face2> faces, Double[] etalon) {
            Face2 face = null;

            if (faces != null) {
                foreach (var currFace in faces) {
                    Surface plane = currFace.IGetSurface();

                    if (plane != null && plane.IsCylinder()) {
                        Double[] planeParamsArray = plane.CylinderParams;

                        if (SolidWorksApi.CompareParams(planeParamsArray, etalon, 7)) {
                            face = currFace;
                        }
                    }
                }
            } else {
                Message.Error("Перечисление граней было null!");
            }

            return face;
        }

        public static IEnumerable<Face2> GetFaces(Component2 component) {
            IList<Face2> facesList = null;

            if (component != null) {
                var body = component.GetBody() as Body;
                var curFace = body.GetFirstFace() as Face2;

                while (curFace != null) {
                    if (facesList == null) {
                        facesList = new List<Face2>();
                    }
                    facesList.Add(curFace);

                    curFace = curFace.GetNextFace() as Face2;
                }
            } else {
                Message.Error("Компонент был null!");
            }

            return facesList;
        }

        public static Boolean CompareParams(Double[] current, Double[] etalon, Int32 count) {
            var isEqual = false;

            if (current.Length == count && etalon.Length == count) {
                isEqual = true;

                for (var i = 0; i < count; i++) {
                    if (Math.Abs(current[i] - etalon[i]) > 0.0001f) {
                        isEqual = false;
                    }
                }
            } else {
                Message.Error("Параметры имели разную размерность!");
            }

            return isEqual;
        }

        public static IList<Component2> FindComponents(String fileName, AssemblyDoc assemblyDocument, Int32? orderNumber = null) {
            IList<Component2> components = null;

            if (assemblyDocument != null) {
                Object[] objectsArray = assemblyDocument.GetComponents(true);

                foreach (Object componentObject in objectsArray) {
                    var currComponent = componentObject as Component2;

                    if (componentObject != null) {
                        var componentString = String.Format("{0}{2}{1}", fileName, orderNumber != null ? orderNumber.ToString() : "", orderNumber != null ? "-" : "");

                        if (currComponent.Name.Contains(componentString)) {
                            if (components == null) {
                                components = new List<Component2>();
                            }
                            components.Add(currComponent);
                            currComponent.MakeVirtual();
                        }
                    }
                }
            } else {
                Message.Error("Документ сборки был null!");
            }

            return components;
        }

        public static IList<Component2> FindComponents(Component component, AssemblyDoc assemblyDocument, Int32? orderNumber = null) {
            return SolidWorksApi.FindComponents(component.FileName, assemblyDocument, orderNumber);
        }

        public static Boolean Mate(Face2 one, Face2 another, Mates mate, Aligns align, AssemblyDoc assemblyDocument) {
            var isDone = false;

            if (assemblyDocument != null) {
                if (one != null && another != null) {
                    (assemblyDocument as ModelDoc2).ClearSelection();
                    ((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(one, null);
                    ((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(another, null);

                    Message.Text(String.Format("Пытаюсь выполнить сопряжение {0}..", mate.ToString()));
                    if (true) {
                        Int32 statusValue = 0;
                        Mate2 myMate = ((Mate2)(assemblyDocument.AddMate3((Int32)mate, (Int32)align, false, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, false, out statusValue)));

                        isDone = statusValue == 1;
                        if (!isDone) {
                            Message.Error("Не удалось выполнить сопряжение!");
                        }
                    }
                } else {
                    Message.Error("Одна из граней была null!");
                }
            } else {
                Message.Error("Документ сборки был null!");
            }

            return isDone;
        }

        public static void SetEquations(IComponent2 component, IList<Equation> equations) {
            if (component != null) {
                ModelDoc2 modelDocument = component.GetModelDoc2();
                EquationMgr equationManager = ((IModelDoc2)modelDocument).GetEquationMgr();

                for (var i = 0; i < equations.Count; i++) {
                    equationManager.Equation[i] = String.Format("{0}{1}{0}={2}", '"', equations[i].Name, equations[i].Value);
                }

                equationManager.EvaluateAll();
                modelDocument.EditRebuild3();
                modelDocument.GraphicsRedraw();
            } else {
                Message.Error("Компонент был null!");
            }
        }

        public static void SetEquation(IComponent2 component, Int32 index, Equation equation) {
            if (component != null) {
                ModelDoc2 modelDocument = component.GetModelDoc2();
                EquationMgr equationManager = ((IModelDoc2)modelDocument).GetEquationMgr();

                equationManager.Equation[index] = String.Format("{0}{1}{0}={2}", '"', equation.Name, equation.Value);

                equationManager.EvaluateAll();
                modelDocument.EditRebuild3();
                modelDocument.GraphicsRedraw();
            } else {
                Message.Error("Компонент был null!");
            }
        }
    }
}