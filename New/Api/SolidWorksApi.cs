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

        public static Boolean TryGetActiveAssembly(SW solidWorks, ref AssemblyDoc assemblyDocument, ref SelectionMgr selectionManager)
        {
            var isDone = false;

            if (solidWorks != null) {
                isDone = true;
                try
                {
                    ModelDoc2 solidWorksDocument = null;
                    ModelView myModelView = null;

                    solidWorksDocument = ((ModelDoc2)(solidWorks.ActiveDoc));
                    myModelView = ((ModelView)(solidWorksDocument.ActiveView));
                    myModelView.FrameState = ((int)(swWindowState_e.swWindowMaximized));
                    assemblyDocument = ((AssemblyDoc)(solidWorksDocument));
                    selectionManager = solidWorksDocument.SelectionManager;
                }
                catch
                {
                    Message.Error("Не удалось получить активную сборку!");
                    isDone = false;
                    assemblyDocument = null;
                    selectionManager = null;
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
                    assemblyDocument.AddComponent(filePathway, centerPoint.X, centerPoint.Y, centerPoint.Z);

                    try {
                        modelDocument.Close();
                    } catch {}
                }
            } else {
                Message.Error("Файл указанного компонента не найден!");
            }

            return modelDocument;
        }

        public static ModelDoc2 InsertComponent(Component component, SW solidWorks, AssemblyDoc assemblyDocument, Point point = null) {
            return SolidWorksApi.InsertComponent(component.FileNamePathway, solidWorks, assemblyDocument, point);
        }
    }
}