#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;

using treeDiM.StackBuilder.Basics;

using Sharp3D.Math.Core;
#endregion

namespace treeDiM.StackBuilder.Graphics
{
    public abstract class Viewer : IDisposable
    {
        #region Data members
        /// <summary>
        /// list of picking boxes with id
        /// </summary>
        private List<Tuple<BBox3D, uint>> _listPickingBox = new List<Tuple<BBox3D, uint>>();
        private Transform3D _currentTransf;
        private Vector3D _viewDir;
        #endregion

        #region Abstract methods
        public abstract void Draw(Graphics3D graphics, Transform3D transform);
        public abstract void Draw(Graphics2D graphics);
        #endregion

        #region Picking
        protected void ClearPickingBoxes()
        {
            _listPickingBox.Clear();
        }
        protected void AddPickingBox(BBox3D bbox, uint id)
        {
            _listPickingBox.Add(new Tuple<BBox3D, uint>(bbox, id));
        }
        protected Ray GetPickingRay(int x, int y)
        {
            // normalised_x = 2 * mouse_x / win_width - 1
            // normalised_y = 1 - 2 * mouse_y / win_height
            // note the y pos is inverted, so +y is at the top of the screen
            // unviewMat = (projectionMat * modelViewMat).inverse()
            // near_point = unviewMat * Vec(normalised_x, normalised_y, 0, 1)
            // camera_pos = ray_origin = modelViewMat.inverse().col(4)
            // ray_dir = near_point - camera_pos
            Transform3D eye2worldTransf = CurrentTransformation.Inverse();
            Vector3D vNear = eye2worldTransf.transform(new Vector3D((double)x, (double)y, 0.0));
            Vector3D vFar = eye2worldTransf.transform(new Vector3D((double)x, (double)y, 1.0));
            return new Ray(vNear, vFar);
        }
        public Transform3D CurrentTransformation
        {
            get { return _currentTransf; }
            set { _currentTransf = value; }
        }
        public Vector3D ViewDir
        {
            get { return _viewDir; }
            set { _viewDir = value; }
        }

        public bool TryPicking(int x, int y, out uint index)
        {
            Ray ray = GetPickingRay(x, y);
            index = 0;
            foreach (Tuple<BBox3D, uint> tBox in _listPickingBox)
            {
                index = tBox.Item2;
                Vector3D ptMin = tBox.Item1.PtMin;
                Vector3D ptMax = tBox.Item1.PtMax;

                Vector3D[] vertices = {
                    new Vector3D(ptMin.X, ptMin.Y, ptMin.Z)       // 0
                    , new Vector3D(ptMax.X, ptMin.Y, ptMin.Z)     // 1
                    , new Vector3D(ptMax.X, ptMax.Y, ptMin.Z)     // 2
                    , new Vector3D(ptMin.X, ptMax.Y, ptMin.Z)     // 3
                    , new Vector3D(ptMin.X, ptMin.Y, ptMax.Z)     // 4
                    , new Vector3D(ptMax.X, ptMin.Y, ptMax.Z)     // 5
                    , new Vector3D(ptMax.X, ptMax.Y, ptMax.Z)     // 6
                    , new Vector3D(ptMin.X, ptMax.Y, ptMax.Z)     // 7
                };
                Face[] faces = new Face[]
                {
                    new Face(0, vertices[0], vertices[1], vertices[5], vertices[4], false),
                    new Face(0, vertices[1], vertices[2], vertices[6], vertices[5], false),
                    new Face(0, vertices[2], vertices[3], vertices[7], vertices[6], false),
                    new Face(0, vertices[3], vertices[0], vertices[4], vertices[7], false),
                };

                foreach (Face f in faces)
                {
                    if (f.IsVisible(ViewDir) && f.RayIntersect(ray, out Vector3D ptInter))
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
        }
        #endregion
    }

    public class ViewerSolution : Viewer
    {
        #region Data members
        private Solution _solution;
        #endregion

        #region Constructor
        public ViewerSolution(Solution solution)
        {
            _solution = solution;
        }
        #endregion

        #region Viewer
        public override void Draw(Graphics3D graphics, Transform3D transform)
        {
            // clear list of picking box
            ClearPickingBoxes();

            if (null == _solution) return;
            Analysis analysis = _solution.Analysis;
            AnalysisCasePallet analysisCasePallet = analysis as AnalysisCasePallet;
            AnalysisBoxCase analysisBoxCase = analysis as AnalysisBoxCase;
            AnalysisCylinderCase analysisCylinderCase = analysis as AnalysisCylinderCase;
            AnalysisCylinderPallet analysisCylinderPallet = analysis as AnalysisCylinderPallet;
            AnalysisPalletTruck analysisPalletTruck = analysis as AnalysisPalletTruck;
            AnalysisCaseTruck analysisCaseTruck = analysis as AnalysisCaseTruck;

            if (null != analysisCasePallet)
            {
                // ### draw pallet
                Pallet pallet = new Pallet(analysisCasePallet.PalletProperties);
                pallet.Draw(graphics, transform);
            }
            else if (null != analysisBoxCase)
            {
                // draw case (inside)
                Case case_ = new Case(analysisBoxCase.CaseProperties);
                case_.DrawInside(graphics, transform);
            }
            else if (null != analysisCylinderCase)
            {
                // ### draw case (inside)
                Case case_ = new Case(analysisCylinderCase.CaseProperties);
                case_.DrawInside(graphics, transform);
            }
            else if (null != analysisCylinderPallet)
            {
                // ### draw pallet
                Pallet pallet = new Pallet(analysisCylinderPallet.PalletProperties);
                pallet.Draw(graphics, transform);
            }
            else if (null != analysisPalletTruck)
            {
                // ### draw truck
                Truck truck = new Truck(analysisPalletTruck.TruckProperties);
                truck.DrawBegin(graphics);
            }
            else if (null != analysisCaseTruck)
            {
                // ### draw truck
                Truck truck = new Truck(analysisCaseTruck.TruckProperties);
                truck.DrawBegin(graphics);
            }

            // ### draw solution
            uint layerId = 0, pickId = 0;
            List<ILayer> layers = _solution.Layers;
            foreach (ILayer layer in layers)
            {
                BBox3D bbox = new BBox3D();
                // ### layer of boxes
                Layer3DBox layerBox = layer as Layer3DBox;
                if (null != layerBox)
                {
                    if (analysis.Content is LoadedPallet)
                    {
                        LoadedPallet loadedPallet = analysis.Content as LoadedPallet;
                        BBox3D solBBox = loadedPallet.ParentAnalysis.Solution.BBoxGlobal;
                        foreach (BoxPosition bPosition in layerBox)
                        {
                            bool simplified = false;
                            if (simplified)
                            {
                                BoxProperties bProperties = new BoxProperties(null, solBBox.Dimensions);
                                bProperties.SetColor(Color.Chocolate);
                                Box b = new Box(pickId++, bProperties, bPosition.Transform(transform));
                                graphics.AddBox(b);
                                bbox.Extend(b.BBox);
                            }
                            else
                                graphics.AddImage(loadedPallet.ParentAnalysis, solBBox.DimensionsVec, bPosition.Transform(transform));
                        }
                    }
                    else
                    {
                        bool aboveSelectedLayer = (_solution.SelectedLayerIndex != -1) && (layerId > _solution.SelectedLayerIndex);
                        Transform3D upTranslation = Transform3D.Translation(new Vector3D(0.0, 0.0, aboveSelectedLayer ? DistanceAboveSelectedLayer : 0.0));

                        foreach (BoxPosition bPosition in layerBox)
                        {
                            BoxPosition boxPositionModified = bPosition.Transform(transform * upTranslation);
                            Box b = null;
                            if (analysis.Content is PackProperties)
                                b = new Pack(pickId++, analysis.Content as PackProperties, boxPositionModified);
                            else
                                b = new Box(pickId++, analysis.Content as PackableBrick, boxPositionModified);
                            graphics.AddBox(b);
                            bbox.Extend(b.BBox);
                        }
                    }
                }
                Layer3DCyl layerCyl = layer as Layer3DCyl;
                if (null != layerCyl)
                {
                    foreach (Vector3D vPos in layerCyl)
                    {
                        Cylinder c = new Cylinder(pickId++, analysis.Content as CylinderProperties, new CylPosition(transform.transform(vPos), HalfAxis.HAxis.AXIS_Z_P));
                        graphics.AddCylinder(c);
                        bbox.Extend(c.BBox);
                    }
                }
                if (null != layerBox || null != layerCyl)
                {
                    // add layer BBox
                    AddPickingBox(bbox, layerId);
                    // draw bounding box around selected layer
                    if (layerId == _solution.SelectedLayerIndex)
                        DrawLayerBoundingBox(graphics, bbox);
                    ++layerId;
                }

                // ### interlayer
                if (layer is InterlayerPos interlayerPos)
                {
                    InterlayerProperties interlayerProp = _solution.Interlayers[interlayerPos.TypeId];
                    if (null != interlayerProp)
                    {
                        bool aboveSelectedLayer = (_solution.SelectedLayerIndex != -1) && (layerId > _solution.SelectedLayerIndex);
                        Transform3D upTranslation = Transform3D.Translation(new Vector3D(0.0, 0.0, aboveSelectedLayer ? DistanceAboveSelectedLayer : 0.0));

                        BoxPosition bPosition = new BoxPosition(
                            new Vector3D(
                            analysis.Offset.X + 0.5 * (analysis.ContainerDimensions.X - interlayerProp.Length)
                            , analysis.Offset.Y + 0.5 * (analysis.ContainerDimensions.Y - interlayerProp.Width)
                            , interlayerPos.ZLow
                            ), HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P);
                        Box box = new Box(pickId++, interlayerProp, bPosition.Transform(transform * upTranslation));
                        graphics.AddBox(box);
                        bbox.Extend(box.BBox);
                    }
                }
            }
            BBox3D loadBBox = _solution.BBoxLoad;
            BBox3D loadBBoxWDeco = _solution.BBoxLoadWDeco;
            if (null != analysisCasePallet)
            {
                #region Pallet corners
                // ### pallet corners : Begin
                Corner[] corners = new Corner[4];
                if (analysisCasePallet.HasPalletCorners)
                {
                    // positions
                    Vector3D[] cornerPositions =
                {
                    loadBBox.PtMin
                    , new Vector3D(loadBBox.PtMax.X, loadBBox.PtMin.Y, loadBBox.PtMin.Z)
                    , new Vector3D(loadBBox.PtMax.X, loadBBox.PtMax.Y, loadBBox.PtMin.Z)
                    , new Vector3D(loadBBox.PtMin.X, loadBBox.PtMax.Y, loadBBox.PtMin.Z)
                };
                    // length axes
                    HalfAxis.HAxis[] lAxes =
                {
                    HalfAxis.HAxis.AXIS_X_P,
                    HalfAxis.HAxis.AXIS_Y_P,
                    HalfAxis.HAxis.AXIS_X_N,
                    HalfAxis.HAxis.AXIS_Y_N
                };
                    // width axes
                    HalfAxis.HAxis[] wAxes =
                {
                    HalfAxis.HAxis.AXIS_Y_P,
                    HalfAxis.HAxis.AXIS_X_N,
                    HalfAxis.HAxis.AXIS_Y_N,
                    HalfAxis.HAxis.AXIS_X_P
                };
                    // corners
                    if (analysisCasePallet.HasPalletCorners)
                    {
                        for (int i = 0; i < 4; ++i)
                        {
                            corners[i] = new Corner(0, analysisCasePallet.PalletCornerProperties);
                            corners[i].Height = Math.Min(analysisCasePallet.PalletCornerProperties.Length, loadBBox.Height);
                            corners[i].SetPosition(
                                transform.transform(cornerPositions[i])
                                , HalfAxis.Transform(lAxes[i], transform), HalfAxis.Transform(wAxes[i], transform)
                                );
                            corners[i].DrawBegin(graphics);
                        }
                    }
                }
                #endregion

                #region Pallet film
                // ### pallet film
                Film film = null;
                if (analysisCasePallet.HasPalletFilm && -1 == _solution.SelectedLayerIndex)
                {
                    // instantiate film
                    PalletFilmProperties palletFilmProperties = analysisCasePallet.PalletFilmProperties;
                    film = new Film(
                        palletFilmProperties.Color,
                        palletFilmProperties.UseTransparency,
                        palletFilmProperties.UseHatching,
                        palletFilmProperties.HatchSpacing,
                        palletFilmProperties.HatchAngle);
                    film.AddRectangle(new FilmRectangle(transform.transform(loadBBoxWDeco.PtMin)
                        , HalfAxis.Transform(HalfAxis.HAxis.AXIS_X_P, transform), HalfAxis.Transform(HalfAxis.HAxis.AXIS_Z_P, transform)
                        , new Vector2D(loadBBoxWDeco.Length, loadBBoxWDeco.Height), 0.0));
                    film.AddRectangle(new FilmRectangle(transform.transform(loadBBoxWDeco.PtMin + loadBBoxWDeco.Length * Vector3D.XAxis)
                        , HalfAxis.Transform(HalfAxis.HAxis.AXIS_Y_P, transform), HalfAxis.Transform(HalfAxis.HAxis.AXIS_Z_P, transform)
                        , new Vector2D(loadBBoxWDeco.Width, loadBBoxWDeco.Height), 0.0));
                    film.AddRectangle(new FilmRectangle(transform.transform(loadBBoxWDeco.PtMin + loadBBoxWDeco.Length * Vector3D.XAxis + loadBBoxWDeco.Width * Vector3D.YAxis)
                        , HalfAxis.Transform(HalfAxis.HAxis.AXIS_X_N, transform), HalfAxis.Transform(HalfAxis.HAxis.AXIS_Z_P, transform)
                        , new Vector2D(loadBBoxWDeco.Length, loadBBoxWDeco.Height), 0.0));
                    film.AddRectangle(new FilmRectangle(transform.transform(loadBBoxWDeco.PtMin + loadBBoxWDeco.Width * Vector3D.YAxis)
                        , HalfAxis.Transform(HalfAxis.HAxis.AXIS_Y_N, transform), HalfAxis.Transform(HalfAxis.HAxis.AXIS_Z_P, transform)
                        , new Vector2D(loadBBoxWDeco.Width, loadBBoxWDeco.Height), 0.0));
                    film.AddRectangle(new FilmRectangle(transform.transform(loadBBoxWDeco.PtMin + loadBBoxWDeco.Height * Vector3D.ZAxis)
                        , HalfAxis.Transform(HalfAxis.HAxis.AXIS_X_P, transform), HalfAxis.Transform(HalfAxis.HAxis.AXIS_Y_P, transform)
                        , new Vector2D(loadBBoxWDeco.Length, loadBBoxWDeco.Width)
                        , UnitsManager.ConvertLengthFrom(200.0, UnitsManager.UnitSystem.UNIT_METRIC1)));
                    film.DrawBegin(graphics);
                }
                #endregion

                #region Pallet corners
                // pallet corners : End
                if (analysisCasePallet.HasPalletCorners)
                {
                    for (int i = 0; i < 4; ++i)
                        corners[i].DrawEnd(graphics);
                }
                #endregion

                #region Pallet Cap
                // ### pallet cap
                if (analysisCasePallet.HasPalletCap)
                {

                    PalletCapProperties capProperties = analysisCasePallet.PalletCapProperties;
                    BoxPosition bPosition = new BoxPosition(new Vector3D(
                        0.5 * (analysisCasePallet.PalletProperties.Length - capProperties.Length),
                        0.5 * (analysisCasePallet.PalletProperties.Width - capProperties.Width),
                        loadBBox.PtMax.Z - capProperties.InsideHeight)
                        , HalfAxis.Transform(HalfAxis.HAxis.AXIS_X_P, transform)
                        , HalfAxis.Transform(HalfAxis.HAxis.AXIS_Y_P, transform)
                        );

                    Transform3D upTranslation = Transform3D.Translation(new Vector3D(0.0, 0.0, -1 != _solution.SelectedLayerIndex ? DistanceAboveSelectedLayer : 0.0));
                    PalletCap cap = new PalletCap(0, capProperties, bPosition.Transform(upTranslation));
                    cap.DrawEnd(graphics);
                }
                #endregion

                #region Pallet film
                // pallet film : End
                if (analysisCasePallet.HasPalletFilm && null != film)
                    film.DrawEnd(graphics);
                #endregion
            }

            if (null != analysisPalletTruck)
            {
                Truck truck = new Truck(analysisPalletTruck.TruckProperties);
                truck.DrawEnd(graphics);
            }
            else if (null != analysisCaseTruck)
            {
                Truck truck = new Truck(analysisCaseTruck.TruckProperties);
                truck.DrawEnd(graphics);
            }

            // ### dimensions
            // dimensions should only be shown when no layer is selected
            if (graphics.ShowDimensions && (-1 == _solution.SelectedLayerIndex))
            {
                graphics.AddDimensions(new DimensionCube(BoundingBoxDim(DimCasePalletSol1), Color.Black, false));
                graphics.AddDimensions(new DimensionCube(BoundingBoxDim(DimCasePalletSol2), Color.Red, true));
            }
            // ###
        }
        public override void Draw(Graphics2D graphics)
        {
            if (null == _solution) return;
        }
        #endregion

        #region Public static methods
        public static void DrawILayer(Graphics3D graphics, ILayer layer, Packable packable, bool showDimensions)
        {
            bool show3D = Graphics.Properties.Settings.Default.LayerView3D;
            graphics.CameraPosition = show3D ? Graphics3D.Corner_0 : Graphics3D.Top;

            uint pickId = 0;
            BBox3D bbox = new BBox3D();
            // ### layer of boxes ###
            if (layer is Layer3DBox layerBox)
            {
                foreach (BoxPosition bPosition in layerBox)
                {
                    Box b = null;
                    if (packable is PackProperties)
                        b = new Pack(pickId++, packable as PackProperties, bPosition);
                    else
                        b = new Box(pickId++, packable as PackableBrick, bPosition);
                    graphics.AddBox(b);
                    bbox.Extend(b.BBox);
                }
            }
            // ###
            // ### layer of cylinders ###
            Layer3DCyl layerCyl = layer as Layer3DCyl;
            if (null != layerCyl)
            {
                foreach (Vector3D vPos in layerCyl)
                {
                    Cylinder c = new Cylinder(pickId++, packable as CylinderProperties, new CylPosition(vPos, HalfAxis.HAxis.AXIS_Z_P));
                    graphics.AddCylinder(c);
                    bbox.Extend(c.BBox);
                }
            }
            // ###
            if (showDimensions)
            {
                graphics.AddDimensions(new DimensionCube(bbox, Color.Black, false));
            }
        }
        #endregion

        #region Static properties
        public static double DistanceAboveSelectedLayer { get; set; }
        public static int DimCasePalletSol1 { get; set; } = 0;
        public static int DimCasePalletSol2 { get; set; } = 1;
        #endregion

        #region Helpers
        private BBox3D BoundingBoxDim(int index)
        {
            PalletProperties palletProperties = null;
            if (_solution.Analysis is AnalysisCasePallet analysisCasePallet)
                palletProperties = analysisCasePallet.PalletProperties;
            if (_solution.Analysis is AnalysisCylinderPallet analysisCylinderPallet)
                palletProperties = analysisCylinderPallet.PalletProperties;

            switch (index)
            {
                case 0: return _solution.BBoxGlobal;
                case 1: return _solution.BBoxLoadWDeco;
                case 2: return palletProperties.BoundingBox;
                case 3: return new BBox3D(0.0, 0.0, 0.0, palletProperties.Length, palletProperties.Width, 0.0);
                default: return _solution.BBoxGlobal;
            }
        }

        private void DrawLayerBoundingBox(Graphics3D graphics, BBox3D bbox)
        {
            Vector3D ptMin = bbox.PtMin;
            Vector3D ptMax = bbox.PtMax;

            Vector3D[] vertices = {
                            new Vector3D(ptMin.X, ptMin.Y, ptMin.Z)       // 0
                            , new Vector3D(ptMax.X, ptMin.Y, ptMin.Z)     // 1
                            , new Vector3D(ptMax.X, ptMax.Y, ptMin.Z)     // 2
                            , new Vector3D(ptMin.X, ptMax.Y, ptMin.Z)     // 3
                            , new Vector3D(ptMin.X, ptMin.Y, ptMax.Z)     // 4
                            , new Vector3D(ptMax.X, ptMin.Y, ptMax.Z)     // 5
                            , new Vector3D(ptMax.X, ptMax.Y, ptMax.Z)     // 6
                            , new Vector3D(ptMin.X, ptMax.Y, ptMax.Z)     // 7
                        };
            Face[] faces = {
                            new Face(0, vertices[0], vertices[1], vertices[5], vertices[4], false),
                            new Face(0, vertices[1], vertices[2], vertices[6], vertices[5], false),
                            new Face(0, vertices[2], vertices[3], vertices[7], vertices[6], false),
                            new Face(0, vertices[3], vertices[0], vertices[4], vertices[7], false),
                        };

            foreach (Face f in faces)
            {
                if (f.IsVisible(ViewDir))
                {
                    Vector3D[] points = f.Points;
                    graphics.AddSegment(new Segment(points[0], points[1], Color.Red));
                    graphics.AddSegment(new Segment(points[1], points[2], Color.Red));
                    graphics.AddSegment(new Segment(points[2], points[3], Color.Red));
                    graphics.AddSegment(new Segment(points[3], points[0], Color.Red));
                }
            }
        }

        #endregion
    }
}
