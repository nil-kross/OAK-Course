using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SW = SolidWorks.Interop.sldworks.SldWorks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace Course
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

        public static ModelDoc2 InsertComponent(String unitFilePathway, SW solidWorks, AssemblyDoc assemblyDocument)
        {
            Int32 errorValue = 0;
            Int32 warningValue = 0;
            ModelDoc2 modelDocument = null;

            modelDocument = solidWorks.OpenDoc6(unitFilePathway, 1, 1, "", ref errorValue, ref warningValue);
            if (modelDocument != null)
            {
                assemblyDocument.AddComponent(unitFilePathway, 0.0, 0.0, 0.1);
                modelDocument.Close();
            }

            return modelDocument;
        }
    }
}