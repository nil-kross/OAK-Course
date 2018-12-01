using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorks_Test {
	public static class LabMaker {
		public static void MakeLab() {
			String[] componentsNamesStringsArray = {
				"Locator1.SLDPRT",
				"Locator1.SLDPRT",
				"Locator7.SLDPRT"
			};
			SldWorks solidWorksApplication = null;
			String diskString = "G";
			AssemblyDoc assemblyDocument = null;
			SelectionMgr selectionManager = null;
			List<IComponent2> locatorsComponentsList = new List<IComponent2>();
			List<IComponent2> stumpsComponentsList = new List<IComponent2>();
			List<Face2> selectedFacesList = null;
			List<Face2> bottomFacesList = null;
			Int32 height = 30;
			Face2 partSideFace = null;
			
			// Получаем приложение SW:
			{
				bool   isDone      = true;
				String errorString = "";

				try {
					solidWorksApplication = (SldWorks)Marshal.GetActiveObject("SldWorks.Application"); //
				} catch (Exception exception) {
					errorString += exception.Message;
					isDone = false;
				}
				if (!isDone) {
					System.Windows.Forms.MessageBox.Show("Не удалось запустить приложение SolidWorks!");
				}
			}

			// Получаем открытую сборку:
			{
				ModelDoc2 solidWorksDocument = null;
				ModelView myModelView = null;
				
				solidWorksDocument = ((ModelDoc2)(solidWorksApplication.ActiveDoc));
				myModelView = ((ModelView)(solidWorksDocument.ActiveView));
				myModelView.FrameState = ((int)(swWindowState_e.swWindowMaximized));
				assemblyDocument = ((AssemblyDoc)(solidWorksDocument));
				selectionManager = solidWorksDocument.SelectionManager;
			}

			// Сохраняем выбранные обьекты:
			if (true) {
				Int32 selectedObjectsAmount = selectionManager.GetSelectedObjectCount();

				if (selectedObjectsAmount > 0) {
					selectedFacesList = new List<Face2>();
				}
				for (Int32 i = 1; i <= selectedObjectsAmount; i++) {
					Object selectedObject  = selectionManager.GetSelectedObject(i);
					Int32 objectTypeValue = selectionManager.GetSelectedObjectType(i);
					swSelectType_e objectTypeEnum = (swSelectType_e)objectTypeValue;

					// Если выбранный обьект принадлежит к типу Face2:
					if (objectTypeEnum == swSelectType_e.swSelFACES) {
						Face2 selectedFace = selectedObject as Face2;

						selectedFacesList.Add(selectedFace);
						if (true) { // DEBUG
							Surface surf = selectedFace.GetSurface() as Surface;

							if (surf != null) {
								if (surf.IsPlane()) {
									Double[] surfParams = surf.PlaneParams;
									String str = "N: ";
									for (Int32 k = 0; k < 6; k++) {
										str += "[" + surfParams[k] + "],";
									}
									MessageBox.Show(str);
								}
							}
						}
					}
				}
			}
			
			// Добавляем детали на сборку:
			if (true) {
				foreach (String componentNameString in componentsNamesStringsArray) {
					Int32     statusValue     = 0;
					Int32     warningsValue   = 0;
					ModelDoc2 locatorDocument = null;
					String locatorPathwayString = diskString + @":\Other\SW3\" + componentNameString;

					locatorDocument = solidWorksApplication.OpenDoc6(locatorPathwayString, 1, 1, "", statusValue, warningsValue);
					if (locatorDocument != null) {
						assemblyDocument.AddComponent(locatorPathwayString, 0.0, 0.0, 0.1);
						//locatorDocument.Close();
					}
				}
			}

			// Находим детали-локаторы:
			if (true) {
				if (assemblyDocument != null) {
					Object[] objectArray = assemblyDocument.GetComponents(true);

					foreach (Object componentObject in objectArray) {
						IComponent2 currComponent = componentObject as IComponent2;
						Face2 bottomFace = null;
						Double[] bottomFaceSurfaceParametersValuesArray = null;

						if (currComponent != null) {
							String compName = currComponent.Name;

							if (currComponent.Name.Contains("Locator1")) {
								locatorsComponentsList.Add(currComponent);
								bottomFaceSurfaceParametersValuesArray = new double[6] {
									+0.0,
									-1.0,
									+0.0,
									+0.0,
									+0.0,
									+0.0
								};
							}
							if (currComponent.Name.Contains("Locator7")) {
								stumpsComponentsList.Add(currComponent);
								bottomFaceSurfaceParametersValuesArray = new double[6] {
									+0.0,
									+1.0,
									+0.0,
									+0.0,
									-0.03,
									+0.0
								};
							}

							currComponent.MakeVirtual();
						}
						
						if (bottomFaceSurfaceParametersValuesArray != null) {
							Body currComponentBody = currComponent.GetBody() as Body;
							Face2 curFace = currComponentBody.GetFirstFace() as Face2;

							while (curFace != null) {
								Surface curFaceSurface = curFace.GetSurface() as Surface;

								if (curFaceSurface.IsPlane()) {
									Double[] curFaceSurfaceParametersValuesArray = curFaceSurface.PlaneParams;
									Boolean isFits = true;

									for (Int32 k = 0; k < 6; k++) {
										Double val = Math.Abs(curFaceSurfaceParametersValuesArray[k] - bottomFaceSurfaceParametersValuesArray[k]);
										if (val > 0.001) {
											isFits = false;
										}
									}

									if (isFits) {
										if (bottomFacesList == null) {
											bottomFacesList = new List<Face2>();
										}
										bottomFacesList.Add(curFace);
									}
								}

								curFace = curFace.GetNextFace() as Face2;
							}
						}
					}
				}
			}

			// Перебираем выбранные обьекты:
			if (true) {

				// Ищем грань боковой стороны детали:
				foreach (Face2 selectedFace in selectedFacesList) {
					Surface selectedFaceSurface = selectedFace.GetSurface() as Surface;

                    if (selectedFaceSurface != null) {
                        if (selectedFaceSurface.IsPlane()) {
						    partSideFace = selectedFace;
					    }
                    }
				}

				foreach (Face2 selectedFace in selectedFacesList) {
					Surface selectedSurface = selectedFace.GetSurface() as Surface;

					if (selectedSurface != null) {
						// Если плоскость прямая..
						if (selectedSurface.IsPlane() == true) {
							foreach (IComponent2 currStumpComponent in stumpsComponentsList) {
								Face topStumpFace = null; 

								// Задаём уравнения размеров:
								{
									ModelDoc2 currStumpModelDocument = currStumpComponent.GetModelDoc2();
									EquationMgr equationManager = ((IModelDoc2)currStumpModelDocument).GetEquationMgr();
									String dEquationString = String.Format(@"""D""={0}", 42);
									String hEquationString = String.Format(@"""H""={0}", height);

									equationManager.Equation[0] = hEquationString;
									equationManager.Equation[1] = dEquationString;
									equationManager.EvaluateAll();
									currStumpModelDocument.EditRebuild3();
									currStumpModelDocument.GraphicsRedraw();
								}

								// Находим верхнюю грань для сопряжения:
								{
									ModelDoc2 currStumpModelDocument = currStumpComponent.GetModelDoc2();
									Body currStumpBody = currStumpComponent.GetBody() as Body;
									Face[] faces = new Face[1];
									Face face = currStumpBody.GetFirstFace() as Face;

									while (face != null) {
										faces[faces.Length - 1] = face;
										face = face.GetNextFace() as Face;
										if (face != null) {
											Array.Resize(ref faces, (faces.Length + 1));
										}
									}

									foreach (Face currFace in faces) {
										Surface surface = currFace.GetSurface() as Surface;

										if (surface.IsPlane()) {
											Double[] etalonParametersValuesArray = {
												+0.0,
												+1.0,
												+0.0,
												+0.0,
												+0.0,
												+0.0
											};
											Double[] parametersValuesArray0 = (Double[])surface.PlaneParams;
											Boolean isFits = true;

											for (Int32 k = 0; k < 6; k++) {
												if (etalonParametersValuesArray[k] != parametersValuesArray0[k]) {
													isFits = false;
												}
											}

											if (isFits) {
												topStumpFace = currFace;
											}
										}
									}
								}

								(assemblyDocument as ModelDoc2).ClearSelection();
								((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(topStumpFace, null);
								((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(partSideFace, null);

								// Выполняем сопряжение:
								{
									Int32 statusValue = 0;
									Mate2 myMate = ((Mate2)(assemblyDocument.AddMate3(0, 1, false, 0.051126624765061614, 0.001, 0.001, 0.001, 0.001, 1.5707963267948966, 0.52359877559830004, 0.52359877559830004, false, out statusValue)));
										
									if (statusValue == 1) {
										if (false) { // DEBUG
											MessageBox.Show("Пенёк успешно сопряжён!");
										}
									}
								}
							}
						}

						// Если плоскость цилиндрическая..
						if (selectedSurface.IsCylinder() == true) {
							Double[] parametersValuesArray = (Double[])selectedSurface.CylinderParams;
							Double radiusValue = parametersValuesArray[6];

							// Выбираем доступный локатор:
							if ((locatorsComponentsList.Count > 0) && (locatorsComponentsList[0] != null)) {
								IComponent2 currLocator = locatorsComponentsList[0];
								ModelDoc2 currLocatorModelDocument = currLocator.GetModelDoc2();
								EquationMgr equationManager = ((IModelDoc2)currLocatorModelDocument).GetEquationMgr();
								String dEquationString = String.Format(@"""D""={0}", (Int32)(Math.Round(radiusValue * 1000 * 2, 0)));
								String hEquationString = String.Format(@"""H""={0}", height);

								equationManager.Equation[0] = hEquationString;
								equationManager.Equation[1] = dEquationString;
								equationManager.EvaluateAll();
								currLocatorModelDocument.EditRebuild3();
								currLocatorModelDocument.GraphicsRedraw();
								
								locatorsComponentsList.RemoveAt(0);
									
								// Ищем цилиндрическую поверхность пальца и сопрягаем:
								if (true) {
									Body2 body = currLocator.GetBody() as Body2;
									Face[] faces = new Face[1];

									(assemblyDocument as ModelDoc2).ClearSelection();
									equationManager.EvaluateAll();

									{
										Face face = body.GetFirstFace() as Face;

										while (face != null) {
											faces[faces.Length - 1] = face;
											face = face.GetNextFace() as Face;
											if (face != null) {
												Array.Resize(ref faces, (faces.Length + 1));
											}
										}
									}

									foreach (Face face in faces) {
										Surface surface = face.GetSurface() as Surface;

										if (surface.IsCylinder()) {
											Double[] parametersValuesArray0 = (Double[])surface.CylinderParams;
											Double radiusValue0 = parametersValuesArray0[6];

											if (Math.Abs(radiusValue0 - radiusValue) < 0.0001) {
												Boolean isFoundedFaceSelected = ((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(face, null);
											}
										}
									}

									// Выполняем сопряжение:
									{
										Int32 statusValue = 0;
										Boolean bol2 = ((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(selectedFace, null);
										Mate2 myMate = ((Mate2)(assemblyDocument.AddMate3(1, 1, false, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, false, out statusValue)));
										
										if (statusValue == 1) {
											if (false) {
												MessageBox.Show("OK");
											}
										}
									}
								}

								// Ищем плоскость и садим её на бутылку:
								if (true) {
									
									Body2 body = currLocator.GetBody() as Body2;
									Face[] faces = new Face[1];

									(assemblyDocument as ModelDoc2).ClearSelection();

									{
										Face face = body.GetFirstFace() as Face;

										while (face != null) {
											faces[faces.Length - 1] = face;
											face = face.GetNextFace() as Face;
											if (face != null) {
												Array.Resize(ref faces, (faces.Length + 1));
											}
										}
									}

									foreach (Face face in faces) {
										Surface surface = face.GetSurface() as Surface;

										if (surface.IsPlane()) {
											Double[] etalonParametersValuesArray = {
												+0.0,
												-1.0,
												+0.0,
												+0.0,
												+0.0,
												+0.0
											};
											Double[] parametersValuesArray0 = (Double[])surface.PlaneParams;
											Boolean isFits = true;

											for (Int32 k = 0; k < 6; k++) {
												if (etalonParametersValuesArray[k] != parametersValuesArray0[k]) {
													isFits = false;
												}
											}

											if (isFits) {
												Boolean isFoundedFaceSelected = ((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(face, null);
											}
										}
									}
									
									// Выполняем сопряжение:
									{
										Int32 statusValue = 0;
										Boolean bol2 = ((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(partSideFace, null);
										Mate2 myMate = ((Mate2)(assemblyDocument.AddMate3(0, 1, false, 0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, false, out statusValue)));
										
										if (statusValue == 1) {
											if (false) {
												MessageBox.Show("Бокавая грань успешно посажена на бутылку!");
											}
										}
									}
								}
							}
						}
					}
				}
			}

			// Вставляем и сопрягаем корпусную плиту:
			if (true) {
				Double[] boundingBoxDimensionsValuesArray = assemblyDocument.GetBox(0);
				Double boundingBoxLength = Math.Abs(boundingBoxDimensionsValuesArray[0] - boundingBoxDimensionsValuesArray[3]);
				Double boundingBoxWidth = Math.Abs(boundingBoxDimensionsValuesArray[1] - boundingBoxDimensionsValuesArray[4]);

				// Вставляем плиту:
				{
					Int32     statusValue     = 0;
					Int32     warningsValue   = 0;
					ModelDoc2 locatorDocument = null;
					String locatorPathwayString = diskString + @":\Other\SW3\" + "Korpus.SLDPRT";

					locatorDocument = solidWorksApplication.OpenDoc6(locatorPathwayString, 1, 1, "", statusValue, warningsValue);
					if (locatorDocument != null) {
						assemblyDocument.AddComponent(locatorPathwayString, 0.0, -0.2, 0.2);
					}
				}

				// Изменяем размер плиты:
				if (true) {
					if (assemblyDocument != null) {
						Object[] objectArray = assemblyDocument.GetComponents(true);

						foreach (Object componentObject in objectArray) {
							IComponent2 currComponent = componentObject as IComponent2;

							if (currComponent.Name.Contains("Korpus")) {
								ModelDoc2 currLocatorModelDocument = currComponent.GetModelDoc2();
								EquationMgr equationManager = ((IModelDoc2)currLocatorModelDocument).GetEquationMgr();
								String lEquationString = String.Format(@"""L""={0}", (Int32)(Math.Round(boundingBoxLength * 1000, 0)));
								String bEquationString = String.Format(@"""B""={0}", (Int32)(Math.Round(boundingBoxWidth * 1000, 0)));

								equationManager.Equation[0] = lEquationString;
								equationManager.Equation[1] = bEquationString;
								equationManager.EvaluateAll();
								currLocatorModelDocument.EditRebuild3();
								currLocatorModelDocument.GraphicsRedraw();

								// Выполняем сопряжение:
								{
									Face corpseTopFace = null;

									{
										Face face = (currComponent.GetBody() as Body).GetFirstFace() as Face;

										while (face != null) {
											Surface faceSurface = face.GetSurface() as Surface;

											if (faceSurface.IsPlane()) {
												Double[] etalonParametersValuesArray = new Double[6] {
													+0.0,
													+1.0,
													+0.0,
													-0.064,
													+0.0,
													+0.064
												};
												Double[] faceSurfaceParametersValuesArray = faceSurface.PlaneParams;
												Boolean isFits = true;

												for (Int32 k = 0; k < 6; k++) {
													Double val = Math.Abs(etalonParametersValuesArray[k] - faceSurfaceParametersValuesArray[k]);
													if (val > 0.001) {
														isFits = false;
													}
												}

												if (isFits) {
													corpseTopFace = face;
												}
											}
											face = face.GetNextFace() as Face;
										}
									}

									if (bottomFacesList != null) {
										foreach (Face bottomFace in bottomFacesList) {
											(assemblyDocument as ModelDoc2).ClearSelection();
											((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(bottomFace, null);
											((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(corpseTopFace, null);

											{
												Int32 statusValue = 0;
												Mate2 myMate = ((Mate2)(assemblyDocument.AddMate3(0, 1, false, 0.051126624765061614, 0.001, 0.001, 0.001, 0.001, 1.5707963267948966, 0.52359877559830004, 0.52359877559830004, false, out statusValue)));
										
												if (statusValue == 1) {
													if (false) { // DEBUG
														MessageBox.Show("Плита успешно сопряжена!");
													}
												}
											}
										}

										if (true) { // DEBUG
											(assemblyDocument as ModelDoc2).ClearSelection();
											foreach (Face f in bottomFacesList) {
												((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(f, null);
											}
										}

										if (true) { // DEBUG
											(assemblyDocument as ModelDoc2).ClearSelection();
											((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(partSideFace, null);
											((assemblyDocument as ModelDoc2).SelectionManager as SelectionMgr).AddSelectionListObject(corpseTopFace, null);

											{
												Int32 statusValue = 0;
												Mate2 myMate = ((Mate2)(assemblyDocument.AddMate3(5, -1, false, (Double)(height / 1000.0), (Double)(height / 1000.0), (Double)(height / 1000.0), 0.001, 0.001, 0, 0.52359877559830004, 0.52359877559830004, false, out statusValue)));
										
												if (statusValue == 1) {
													if (false) { // DEBUG
														MessageBox.Show("Плита успешно сопряжена!");
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
