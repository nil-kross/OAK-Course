using Course.Debug;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Component = Course.Components.Component;
using SW = SolidWorks.Interop.sldworks.SldWorks;

namespace Course.Api {
    public class SolidWorksApi
    {
        private static String swApplicationNameString = "SldWorks.Application";

        private SW solidWorks;
        private AssemblyDoc assemblyDocument;
        private SelectionMgr selectionManager;
        private ModelDoc2 modelDocument;

        public ModelDoc2 ModelDocument {
            get { return this.modelDocument; }
        }
        public AssemblyDoc AssemblyDocument {
            get { return this.assemblyDocument; }
        }

        public static SolidWorksApi GetSolidWorks()
        {
            SolidWorksApi api = null;

            try
            {
                api = new SolidWorksApi
                {
                    solidWorks = (SW)Marshal.GetActiveObject(SolidWorksApi.swApplicationNameString)
                };
            }
            catch
            {
                Message.Error("Не удалось получить дескриптор приложения SolidWorks!");
                api = null;
            }

            return api;
        }

        public Boolean TryGetActiveAssembly()
        {
            var isDone = false;

            if (this.solidWorks != null)
            {
                try
                {
                    ModelView myModelView = null;

                    isDone = true;
                    this.modelDocument = (ModelDoc2)(this.solidWorks.ActiveDoc);
                    this.assemblyDocument = (AssemblyDoc)(this.modelDocument);
                    this.selectionManager = this.modelDocument.SelectionManager;

                    myModelView = (ModelView)(this.modelDocument.ActiveView);
                    myModelView.FrameState = (Int32)(swWindowState_e.swWindowMaximized);
                }
                catch
                {
                    Message.Error("Не удалось получить активную сборку!");
                    isDone = false;
                    this.assemblyDocument = null;
                    this.selectionManager = null;
                    this.modelDocument = null;
                }
            }

            return isDone;
        }

        public AssemblyDoc CreateNewAssembly()
        {
            return this.solidWorks.INewAssembly();
        }

        public Boolean CloseAssemblies()
        {
            return this.solidWorks.CloseAllDocuments(true);
        }

        public IList<Object> GetSelectedObjects()
        {
            IList<Object> selectedObjectsList = null;

            if (this.selectionManager != null)
            {
                Int32 selectedObjectsAmount = this.selectionManager.GetSelectedObjectCount();

                if (selectedObjectsAmount > 0)
                {
                    selectedObjectsList = new Object[selectedObjectsAmount];
                }
                for (Int32 i = 1; i <= selectedObjectsAmount; i++)
                {
                    Object selectedObject = this.selectionManager.GetSelectedObject(i);

                    selectedObjectsList[i - 1] = selectedObject;
                }
            } else
            {
                Message.Error("Selection manager имеет значение null!");
            }

            return selectedObjectsList;
        }

        public ModelDoc2 OpenDocument(String filePathway, DocumentTypes documentType)
        {
            return this.OpenDocumentByFilePathway(filePathway, documentType);
        }

        public ModelDoc2 OpenDocument(Component component, DocumentTypes documentType)
        {
            return this.OpenDocumentByFilePathway(component.Pathway, documentType);
        }

        public ModelDoc2 InsertComponent(Component component, Point point = null)
        {
            return this.InsertComponentByFilePathway(component.Pathway, point);
        }

        public Face2 FindPlaneByParams(IEnumerable<Face2> faces, Double[] planeParams) {
            Func<Face2, Boolean> searchPredicate = (face) => {
                if (face != null && face.IGetSurface() != null && face.IGetSurface().IsPlane()) {
                    var array = face.GetSurface().PlaneParams;

                    return this.CompareParams(planeParams, array, 6);
                } else {
                    return false;
                }
            };

            return this.FindFaces(faces, searchPredicate).FirstOrDefault();
        }

        public Face2 FindPlaneByNormal(IEnumerable<Face2> faces, Point normal) {
            Face2 face = null;

            foreach (var currFace in faces) {
                if (currFace != null) {
                    if (currFace.GetSurface() != null && currFace.GetSurface().IsPlane()) {
                        Plane plane = new Plane(currFace);
                        var isFits =  plane.Normal.X == normal.X &&
                                      plane.Normal.Y == normal.Y &&
                                      plane.Normal.Z == normal.Z;

                        if (isFits) {
                            face = currFace;
                            break;
                        }
                    }
                }
            }

            return face;
        }

        public Face2 FindCylinderByParams(IEnumerable<Face2> faces, Double[] cylinderParams) {
            Func<Face2, Boolean> searchPredicate = (face) => {
                if (face != null && face.GetSurface() != null && face.GetSurface().IsCylinder) {
                    var cylinderParamsArray = face.GetSurface().CylinderParams;

                    return this.CompareParams(cylinderParams, cylinderParamsArray, 7);
                } else {
                    return false;
                }
            };

            return this.FindFaces(faces, searchPredicate).FirstOrDefault();
        }

        public IEnumerable<Component2> FindComponents(Component component, Int32? orderNumber = null) {
            return this.FindComponents(component.File, orderNumber);
        }

        public void SetEquation(IComponent2 component, Equation equation) {
            if (component != null) {
                ModelDoc2 modelDocument = component.GetModelDoc2();
                EquationMgr equationManager = ((IModelDoc2)modelDocument).GetEquationMgr();

                {
                    var regex = new Regex("(.*)=.*");

                    for (var i = 0; i < 10; i++) {
                        var isFounded = false;

                        try {
                            var equationString = equationManager.Equation[i];
                            var matchString = regex.Match(equationString).Groups[1].Value;

                            if (matchString.Contains(equation.Name)) {
                                isFounded = true;
                            }
                        } catch {}

                        if (isFounded) {
                            equationManager.Equation[i] = equation.ToString();

                            equationManager.EvaluateAll();
                            modelDocument.EditRebuild3();
                            modelDocument.GraphicsRedraw();

                            return;
                        }
                    }
                }
            } else {
                Message.Error("Компонент был null!");
            }
        }

        public void SetEquations(IComponent2 component, IList<Equation> equations) {
            if (component != null) {
                foreach (var equation in equations) {
                    this.SetEquation(component, equation);
                }
            } else {
                Message.Error("Компонент был null!");
            }
        }

        public Boolean Mate(Face2 one, Face2 another, Mates mate, Aligns align) {
            var isDone = false;

            if (this.assemblyDocument != null) {
                if (one != null && another != null) {
                    (this.assemblyDocument as ModelDoc2).ClearSelection();
                    ((this.assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(one, null);
                    ((this.assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(another, null);

                    Message.Text(String.Format("Пытаюсь выполнить сопряжение {0}..", mate.ToString()));
                    if (true) {
                        Int32 statusValue = 0;
                        Mate2 myMate = ((Mate2)(this.assemblyDocument.AddMate3((Int32)mate, (Int32)align, false, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, false, out statusValue)));

                        isDone = statusValue == 1;
                        if (!isDone) {
                            Message.Error("Не удалось выполнить сопряжение!\n (Код ошибки: " + statusValue + ")");
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

        public static Bounds GetBounds(Face2 face) {
            Double[] boundsArray = face.GetUVBounds();

            return new Bounds {
                U = boundsArray[1] - boundsArray[0],
                V = boundsArray[3] - boundsArray[2]
            };
        }

        public static Boolean Compare(Face2 first, Face2 second) {
            Func<Face2, Double> func = face => {
                Double value = 0;

                if (face.IGetSurface().IsPlane()) {
                    Bounds bounds = SolidWorksApi.GetBounds(face);

                    value = Math.Sqrt(Math.Pow(bounds.U, 2) + Math.Pow(bounds.V, 2));
                }
                if (face.IGetSurface().IsCylinder()) {
                    Bounds bounds = SolidWorksApi.GetBounds(face);

                    value = bounds.V;
                }

                return value;
            };
            Double firstValue = func(first);
            Double secondValue = func(second);

            return firstValue > secondValue;
        }

        public static Box GetBox(dynamic @object) {
            Box box = null;

            try {
                Double[] boxParams = null;

                if (@object as AssemblyDoc != null) {
                    boxParams = (@object as AssemblyDoc).GetBox(1);
                }
                if (@object as Face2 != null) {
                    boxParams = (@object as Face2).GetBox();
                }

                box = Box.FromArray(boxParams);
            } catch {
                Message.Error("Не удалось получить граничный короб!");
            }

            return box;
        }

        protected ModelDoc2 OpenDocumentByFilePathway(String filePathway, DocumentTypes documentType)
        {
            ModelDoc2 modelDocument = null;

            if (this.solidWorks != null)
            {
                try
                {
                    Message.Text("Пытаюсь открыть сборку " + filePathway + "..");
                    modelDocument = this.solidWorks.OpenDoc(filePathway, (Int32)documentType);
                }
                catch
                {
                    Message.Error("Не удалось открыть указанный файл!");
                }
            }

            return modelDocument;
        }

        protected ModelDoc2 InsertComponentByFilePathway(String filePathway, Point point = null)
        {
            Point centerPoint = point ?? new Point();
            Int32 errorValue = 0;
            Int32 warningValue = 0;
            ModelDoc2 modelDocument = null;

            if (File.Exists(filePathway))
            {
                modelDocument = this.solidWorks.OpenDoc6(filePathway, 1, 1, "", ref errorValue, ref warningValue);
                if (modelDocument != null)
                {

                    try
                    {
                        Message.Text("Пытаюсь вставить компонент из файла '" + filePathway + "'..");
                        this.assemblyDocument.AddComponent(filePathway, centerPoint.X, centerPoint.Y, centerPoint.Z);
                        modelDocument.EditCopy();
                    }
                    catch {
                        Message.Error("Не удалось вставить компонент из файла '" + filePathway + "'..");
                    }
                }
                else
                {
                    Message.Error("Не удалось открыть деталь из файла '" + filePathway + "'!");
                }
            }
            else
            {
                Message.Error("Файл указанного компонента не найден!");
            }

            return modelDocument;
        }

        protected IList<Face2> FindFaces(IEnumerable<Face2> faces, Func<Face2, Boolean> predicate)
        {
            IList<Face2> facesList = null;

            if (faces != null)
            {
                foreach (var currFace in faces)
                {
                    if (predicate(currFace))
                    {
                        if (facesList == null)
                        {
                            facesList = new List<Face2>();
                        }
                        facesList.Add(currFace);
                    }
                }
            }
            else
            {
                Message.Error("Перечисление граней имело значение null!");
            }

            return facesList;
        }

        protected Boolean CompareParams(Double[] current, Double[] etalon, Int32 count)
        {
            var isEqual = false;

            if (current.Length == count && etalon.Length == count)
            {
                isEqual = true;

                for (var i = 0; i < count; i++)
                {
                    if (Math.Abs(current[i] - etalon[i]) > 0.0001f)
                    {
                        isEqual = false;
                    }
                }
            }
            else
            {
                Message.Error("Массивы значений имеют разную размерность!");
            }

            return isEqual;
        }

        public IEnumerable<Face2> GetFaces(Component2 component) {
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
                Message.Error("Значение компонента  null!");
            }

            return facesList;
        }

        protected IList<Component2> FindComponents(String fileName, Int32? orderNumber = null) {
            IList<Component2> components = null;

            if (this.assemblyDocument != null) {
                Object[] objectsArray = this.assemblyDocument.GetComponents(true);

                foreach (Object componentObject in objectsArray) {
                    var currComponent = componentObject as Component2;

                    if (componentObject != null) {
                        var endingString = String.Format("{1}{0}", orderNumber != null ? orderNumber.ToString() : "", orderNumber != null ? "-" : "");

                        if (currComponent.Name.Contains(fileName) && currComponent.Name.Contains(endingString)) {
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
    }
}