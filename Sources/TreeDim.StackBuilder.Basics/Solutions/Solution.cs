#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Sharp3D.Math.Core;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    #region ILayerSolver
    public interface ILayerSolver
    {
        List<Layer2D> BuildLayers(Vector3D dimBox, Vector2D dimContainer, double offsetZ, ConstraintSetAbstract constraintSet, bool keepOnlyBest);
        Layer2D BuildLayer(Vector3D dimBox, Vector2D actualDimensions, LayerDescBox layerDesc, double minSpace);
        Layer2D BuildLayer(Vector3D dimBox, Vector2D dimContainer, LayerDescBox layerDesc, Vector2D actualDimensions, double minSpace);
        ILayer2D BuildLayer(Packable packable, Vector2D dimContainer, LayerDesc layerDesc, double minSpace);
        ILayer2D BuildLayer(Packable packable, Vector2D dimContainer, LayerDesc layerDesc, Vector2D actualDimensions, double minSpace);
        bool GetDimensions(List<LayerDesc> layers, Packable packable, Vector2D dimContainer, double minSpace, out Vector2D actualDimensions);
    }
    #endregion

    #region SolutionItem
    public class SolutionItem
    {
        #region Data members
        private int _indexLayer = 0, _indexInterlayer = -1;
        private bool _symetryX = false, _symetryY = false;
        #endregion

        #region Constructors
        public SolutionItem(int indexLayer, int indexInterlayer, bool symetryX, bool symetryY)
        {
            _indexLayer = indexLayer;
            _indexInterlayer = indexInterlayer;
            _symetryX = symetryX;
            _symetryY = symetryY;
        }
        public SolutionItem(SolutionItem solItem)
        {
            _indexLayer = solItem._indexLayer;
            _indexInterlayer = solItem._indexInterlayer;
            _symetryX = solItem._symetryX;
            _symetryY = solItem._symetryY;
        }
        #endregion

        #region Public properties
        public bool SymetryX
        {
            get { return _symetryX; }
            set { _symetryX = value; }
        }
        public bool SymetryY
        {
            get { return _symetryY; }
            set { _symetryY = value; }
        }
        public bool HasInterlayer   { get { return _indexInterlayer != -1; } }
        public int InterlayerIndex { get { return _indexInterlayer; } set { _indexInterlayer = value; } }
        public int LayerIndex { get { return _indexLayer; } set { _indexLayer = value; } }
        #endregion

        #region Public methods
        public void SetInterlayer(int indexInterlayer)
        {
            _indexInterlayer = indexInterlayer;
        }
        public void InverseSymetry(int axis)
        {
            if (axis == 0) _symetryX = !_symetryX;
            else if (axis == 1) _symetryY = !_symetryY;
            else throw new Exception("Invalid axis of symetry");
        }
        #endregion

        #region Object overrides
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}"
                , _indexLayer
                , _indexInterlayer
                , _symetryX ? 1 : 0
                , _symetryY ? 1 : 0);
        }
        #endregion

        #region Public static parse methods
        public static SolutionItem Parse(string value)
        {
            Regex r = new Regex(@"(?<l>.*),(?<i>.*),(?<x>.*),(?<y>.*)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                return new SolutionItem(
                    int.Parse(m.Result("${l}"))
                    , int.Parse(m.Result("${i}"))
                    , int.Parse(m.Result("${x}")) == 1
                    , int.Parse(m.Result("${y}")) == 1);
            }
            else
                throw new Exception("Failed to parse SolutionItem!");
        }
        public static bool TryParse(string value, out SolutionItem solItem)
        {
            Regex r = new Regex(@"(?<l>),(?<i>),(?<x>),(?<y>)", RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                solItem = new SolutionItem(
                    int.Parse(m.Result("${l}"))
                    , int.Parse(m.Result("${i}"))
                    , int.Parse(m.Result("${x}")) == 1
                    , int.Parse(m.Result("${y}")) == 1);
                return true;
            }
            solItem = null;
            return false;
        }
        #endregion
    }
    #endregion

    #region LayerSummary
    public class LayerSummary
    {
        #region Constructor
        public LayerSummary(Solution sol, int indexLayer, bool symetryX, bool symetryY)
        {
            Sol = sol;
            IndexLayer = indexLayer;
            SymetryX = symetryX;
            SymetryY = symetryY;
        }
        #endregion

        #region Public properties
        public int ItemCount
        { get { return Sol.LayerBoxCount(IndexLayer); } }
        public Vector3D LayerDimensions
        { get { return new Vector3D(Sol._layerTypes[IndexLayer].Length, Sol._layerTypes[IndexLayer].Width, Sol._layerTypes[IndexLayer].LayerHeight); } }
        public double LayerWeight
        { get { return Sol.LayerWeight(IndexLayer); } }
        public double LayerNetWeight
        { get { return Sol.LayerNetWeight(IndexLayer); } }
        public double Space
        { get { return Sol.LayerMaximumSpace(IndexLayer); } }
        public List<int> LayerIndexes { get; } = new List<int>();

        public string LayerIndexesString => string.Join(",", LayerIndexes.ToArray());

        public ILayer Layer3D => Sol.GetILayer(IndexLayer, SymetryX, SymetryY);

        public bool SymetryX { get; }
        public bool SymetryY { get; }
        public int IndexLayer { get; }
        public Solution Sol { get; set; }
        #endregion

        #region Public methods
        public bool IsLayerTypeOf(SolutionItem solItem)
        {
            return IndexLayer == solItem.LayerIndex
                && SymetryX == solItem.SymetryX
                && SymetryY == solItem.SymetryY;
        }
        public void AddIndex(int index)
        {
            LayerIndexes.Add(index);
        }
        #endregion
    }
    #endregion

    #region LayerPhrase
    // used for JJA
    public struct LayerPhrase : IEquatable<LayerPhrase>
    {
        public HalfAxis.HAxis Axis { get; set; }
        public int Count { get; set; }
        public bool Equals(LayerPhrase other) => Axis == other.Axis && Count == other.Count;
    }
    #endregion

    #region Solution
    public class Solution
    {

        #region Data members
        private List<SolutionItem> _solutionItems;
        internal List<ILayer2D> _layerTypes;
        private static ILayerSolver _solver;

        // cached data
        private BBox3D _bbox = BBox3D.Initial;
        #endregion

        #region Static methods
        public static void SetSolver(ILayerSolver solver)
        { _solver = solver; }
        public static ILayerSolver Solver
        {
            get
            {
                if (null == _solver)
                    throw new Exception("Solver not initialized -> Call Solution.SetSolver() static function.");
                return _solver;
            }
        }
        #endregion

        #region Constructor
        public Solution(Analysis analysis, List<LayerDesc> layerDescs)
        {
            Analysis = analysis;
            LayerDescriptors = layerDescs;

            RebuildLayers();
            InitializeSolutionItemList();
        }
        public Solution(Analysis analysis, List<KeyValuePair<LayerDesc, int>> layerList)
        {
            Analysis = analysis;
            LayerDescriptors = layerList.ConvertAll(l => l.Key);

            RebuildLayers();
            InitializeSolutionItemList(layerList);
            RebuildLayers();
        }
        #endregion

        #region Reprocessing
        public void RebuildLayers()
        {
            // sanity checks
            if ((null == LayerDescriptors) || (0 == LayerDescriptors.Count))
                throw new Exception("No layer descriptors available");

            // build list of used layers
            List<LayerDesc> usedLayers = new List<LayerDesc>();
            if (null != _solutionItems && _solutionItems.Count > 0)
            {
                foreach (SolutionItem item in _solutionItems)
                {
                    if (!usedLayers.Contains(LayerDescriptors[item.LayerIndex]))
                        usedLayers.Add(LayerDescriptors[item.LayerIndex]);
                }
            }
            if (0 == usedLayers.Count)
                usedLayers.Add(LayerDescriptors[0]);
            // get dimensions
            Vector2D actualDimensions = Vector2D.Zero;
            Solver.GetDimensions(usedLayers, Analysis.Content, Analysis.ContainerDimensions, ConstraintSet.MinimumSpace.Value, out actualDimensions);

            // actually build layers
            _layerTypes = new List<ILayer2D>();
            foreach (LayerDesc layerDesc in LayerDescriptors)
                _layerTypes.Add(Solver.BuildLayer(Analysis.Content, Analysis.ContainerDimensions, layerDesc, actualDimensions, ConstraintSet.MinimumSpace.Value));
        }
        private void InitializeSolutionItemList()
        {
            _solutionItems = new List<SolutionItem>();

            ConstraintSetAbstract constraintSet = Analysis.ConstraintSet;
            double zTop = Analysis.Offset.Z;
            double weight = Analysis.ContainerWeight;
            int number = 0;
            bool allowMultipleLayers = true;
            if (constraintSet is ConstraintSetPalletTruck constraintSetPalletTruck)
                allowMultipleLayers = constraintSetPalletTruck.AllowMultipleLayers;

            bool symetryX = false, symetryY = false;

            while (!constraintSet.CritHeightReached(zTop)
                && !constraintSet.CritWeightReached(weight)
                && !constraintSet.CritNumberReached(number))
            {
                number += _layerTypes[0].Count;
                weight += _layerTypes[0].Count * Analysis.ContentWeight;
                zTop += _layerTypes[0].LayerHeight;

                if (!constraintSet.CritHeightReached(zTop) && (allowMultipleLayers || _solutionItems.Count < 1))
                    _solutionItems.Add(new SolutionItem(0, -1, symetryX, symetryY));
                else
                    break;
                symetryX = Analysis.AlternateLayersPref ? !symetryX : symetryX;
                symetryY = Analysis.AlternateLayersPref ? !symetryY : symetryY;
            }
        }
        private void InitializeSolutionItemList(List<KeyValuePair<LayerDesc, int>> listLayers)
        {
            _solutionItems = new List<SolutionItem>();
            foreach (KeyValuePair<LayerDesc, int> kvp in listLayers)
            {
                bool symetryX = false, symetryY = false;
                for (int i = 0; i < kvp.Value; ++i)
                {
                    _solutionItems.Add(new SolutionItem(GetLayerIndexFromLayerDesc(kvp.Key), -1, symetryX, symetryY));
                    symetryX = Analysis.AlternateLayersPref ? !symetryX : symetryX;
                    symetryY = Analysis.AlternateLayersPref ? !symetryY : symetryY;
                }
            }
        }
        public void RebuildSolutionItemList()
        {
            try
            {
                ConstraintSetAbstract constraintSet = Analysis.ConstraintSet;
                double zTop = Analysis.Offset.Z;
                double weight = Analysis.ContainerWeight;
                int number = 0;

                List<SolutionItem> solutionItems = new List<SolutionItem>();
                foreach (SolutionItem solItem in _solutionItems)
                {
                    number += _layerTypes[index: solItem.LayerIndex].Count;
                    weight += _layerTypes[index: solItem.LayerIndex].Count * Analysis.ContentWeight;
                    zTop += _layerTypes[solItem.LayerIndex].LayerHeight + ((-1 != solItem.InterlayerIndex) ? Analysis.Interlayer(solItem.InterlayerIndex).Thickness : 0.0);

                    solutionItems.Add(solItem);

                    if (constraintSet.OneCriterionReached(zTop, weight, number, solutionItems.Count))
                        break;
                }

                // add layers until 
                while (!constraintSet.OneCriterionReached(zTop, weight, number, solutionItems.Count))
                {
                    SolutionItem solItem = null;
                    if (solutionItems.Count > 0)
                        solItem = solutionItems.Last();
                    else
                        solItem = new SolutionItem(0, -1, false, false);

                    if (solItem.LayerIndex >= _layerTypes.Count)
                        throw new Exception(string.Format("Layer index out of range!"));

                    number += _layerTypes[solItem.LayerIndex].Count;
                    weight += _layerTypes[solItem.LayerIndex].Count * Analysis.ContentWeight;

                    // using zTopAdded because zTop must not be incremented if SolutionItem object is 
                    // not actually added
                    double zTopIfAdded = zTop + _layerTypes[solItem.LayerIndex].LayerHeight
                        + ((-1 != solItem.InterlayerIndex) ? Analysis.Interlayer(solItem.InterlayerIndex).Thickness : 0.0);

                    // only checking on height because weight / number can be modified without removing 
                    // a layer (while outputing solution as a list of case)
                    if (!constraintSet.CritHeightReached(zTopIfAdded))
                    {
                        solutionItems.Add(new SolutionItem(solItem));
                        zTop = zTopIfAdded;
                    }
                    else
                        break;
                }
                // remove unneeded layer 
                while (constraintSet.CritHeightReached(zTop) && solutionItems.Count > 0)
                {
                    SolutionItem solItem = solutionItems.Last();

                    if (solItem.LayerIndex >= _layerTypes.Count)
                        throw new Exception(string.Format("Layer index out of range!"));
                    number -= _layerTypes[solItem.LayerIndex].Count;
                    weight -= _layerTypes[solItem.LayerIndex].Count * Analysis.ContentWeight;
                    zTop -= _layerTypes[solItem.LayerIndex].LayerHeight
                        + ((-1 != solItem.InterlayerIndex) ? Analysis.Interlayer(solItem.InterlayerIndex).Thickness : 0.0);

                    solutionItems.Remove(solItem);
                }
                _solutionItems.Clear();
                _solutionItems = solutionItems;

                // reset bounding box to force recompute
                _bbox.Reset();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private int GetInterlayerIndex(InterlayerProperties interlayer)
        {
            return Analysis.GetInterlayerIndex(interlayer);
        }
        #endregion

        #region Apply selection
        public void SelectLayer(int index)
        {
            SelectedLayerIndex = index;
            // rebuild layers
            RebuildLayers();
            // rebuild solution item list
            RebuildSolutionItemList();
        }
        public SolutionItem SelectedSolutionItem
        {
            get
            {
                if (!HasValidSelection) return null;
                return _solutionItems[SelectedLayerIndex];
            }
        }
        public void SetLayerTypeOnSelected(int iLayerType)
        {
            // check selected layer
            if (!HasValidSelection) return;
            if (SelectedLayerIndex < 0 || SelectedLayerIndex >= _solutionItems.Count)
            {
                _log.Error(string.Format("Calling SetLayerTypeOnSelected() with SelectedLayerIndex = {0}", SelectedLayerIndex));
                return;
            }
            // get selected solution item
            SolutionItem item = _solutionItems[SelectedLayerIndex];
            item.LayerIndex = iLayerType;
            // rebuild layers
            RebuildLayers();
            // rebuild solution item list
            RebuildSolutionItemList();
        }
        public void SetInterlayerOnSelected(InterlayerProperties interlayer)
        {
            // check selected layer
            if (!HasValidSelection) return;
            // get solution item
            SolutionItem item = _solutionItems[SelectedLayerIndex];
            item.SetInterlayer(GetInterlayerIndex(interlayer));
            // rebuild solution item list
            RebuildSolutionItemList();
        }
        public void ApplySymetryOnSelected(int axis)
        {
            // check selected layer
            if (!HasValidSelection) return;
            // get solution item
            SolutionItem item = _solutionItems[SelectedLayerIndex];
            item.InverseSymetry(axis);
        }
        public InterlayerProperties SelectedInterlayer
        {
            get
            {
                if (!HasValidSelection) return null;
                if (-1 == _solutionItems[SelectedLayerIndex].InterlayerIndex) return null;
                if (_solutionItems[SelectedLayerIndex].InterlayerIndex >= Analysis.Interlayers.Count) return null;
                return Analysis.Interlayer(_solutionItems[SelectedLayerIndex].InterlayerIndex);
            }
        }
        #endregion

        #region Public properties
        public int SelectedLayerIndex { get; private set; } = -1;

        public Analysis Analysis { get; }
        public ConstraintSetAbstract ConstraintSet
        {
            get { return Analysis.ConstraintSet; }
        }
        public List<LayerDesc> LayerDescriptors { get; }
        public List<InterlayerProperties> Interlayers
        {
            get { return Analysis.Interlayers; }
        }
        public List<SolutionItem> SolutionItems
        {
            get { return _solutionItems; }
            set
            {
                _solutionItems = value;
                _bbox.Reset();
            }
        }
        /// <summary>
        /// returns 3D layer. This is only used in LayerSummary
        /// </summary>
        internal ILayer GetILayer(int layerIndex, bool symX, bool symY)
        {
            ILayer2D currentLayer = _layerTypes[layerIndex];

            if (currentLayer is Layer2D)
            {
                Layer2D layer2DBox = currentLayer as Layer2D;
                Layer3DBox boxLayer = new Layer3DBox(0.0, layerIndex);
                foreach (LayerPosition layerPos in layer2DBox)
                {
                    LayerPosition layerPosTemp = AdjustLayerPosition(layerPos, symX, symY);
                    boxLayer.Add(new BoxPosition(
                        layerPosTemp.Position + Analysis.Offset
                        , layerPosTemp.LengthAxis
                        , layerPosTemp.WidthAxis
                        ));
                }
                return boxLayer;
            }

            if (currentLayer is Layer2DCyl)
            {
                Layer2DCyl layer2DCyl = currentLayer as Layer2DCyl;
                Layer3DCyl cylLayer = new Layer3DCyl(0.0);
                foreach (Vector2D vPos in layer2DCyl)
                {
                    cylLayer.Add(
                        AdjustPosition(new Vector3D(vPos.X, vPos.Y, 0.0), symX, symY)
                        + Analysis.Offset);
                }
                return cylLayer;
            }
            return null;
        }
        public List<ILayer> Layers
        {
            get
            {
                List<ILayer> llayers = new List<ILayer>();

                int iBoxCount = 0, iInterlayerCount = 0;
                double zLayer = 0.0, weight = Analysis.ContainerWeight;
                bool stop = false;

                // build layers
                foreach (SolutionItem solItem in _solutionItems)
                {
                    if (solItem.HasInterlayer)
                    {
                        InterlayerProperties interlayer = Analysis.Interlayer(solItem.InterlayerIndex);
                        llayers.Add(new InterlayerPos(zLayer + Analysis.Offset.Z, solItem.InterlayerIndex));
                        zLayer += interlayer.Thickness;
                        ++iInterlayerCount;
                    }

                    System.Diagnostics.Debug.Assert(solItem.LayerIndex < _layerTypes.Count);
                    ILayer2D currentLayer = _layerTypes[solItem.LayerIndex];

                    if (currentLayer is Layer2D layer2DBox)
                    {
                        Layer3DBox boxLayer = new Layer3DBox(zLayer, solItem.LayerIndex);
                        foreach (LayerPosition layerPos in layer2DBox)
                        {
                            if (!ConstraintSet.CritNumberReached(iBoxCount + 1)
                                && !ConstraintSet.CritWeightReached(weight + Analysis.ContentWeight))
                            {
                                LayerPosition layerPosTemp = AdjustLayerPosition(layerPos, solItem.SymetryX, solItem.SymetryY);
                                boxLayer.Add(new BoxPosition(
                                    layerPosTemp.Position + Analysis.Offset + zLayer * Vector3D.ZAxis
                                    , layerPosTemp.LengthAxis
                                    , layerPosTemp.WidthAxis
                                    ));

                                ++iBoxCount;
                                weight += Analysis.ContentWeight;
                            }
                            else
                                stop = true;
                        }
                        if (boxLayer.Count > 0)
                            llayers.Add(boxLayer);
                    }
                    if (currentLayer is Layer2DCyl layer2DCyl)
                    {
                        Layer3DCyl cylLayer = new Layer3DCyl(zLayer);
                        foreach (Vector2D vPos in layer2DCyl)
                        {
                            if (!ConstraintSet.CritNumberReached(iBoxCount + 1)
                                && !ConstraintSet.CritWeightReached(weight + Analysis.ContentWeight))
                            {
                                cylLayer.Add(
                                    AdjustPosition(new Vector3D(vPos.X, vPos.Y, zLayer), solItem.SymetryX, solItem.SymetryY)
                                    + Analysis.Offset);
                                ++iBoxCount;
                                weight += Analysis.ContentWeight;
                            }
                            else
                                stop = true;
                        }
                        if (cylLayer.Count > 0)
                            llayers.Add(cylLayer);
                    }

                    zLayer += currentLayer.LayerHeight;
                    if (stop)
                        break;
                }
                return llayers;
            }
        }
        public BBox3D BBoxLoad
        {
            get
            {
                if (!_bbox.IsValid)
                {
                    foreach (ILayer layer in Layers)
                    {
                        if (layer is Layer3DBox || layer is Layer3DCyl)
                            _bbox.Extend(layer.BoundingBox(Analysis.Content));
                        else if (layer is InterlayerPos)
                        {
                            InterlayerPos interLayerPos = layer as InterlayerPos;
                            InterlayerProperties interlayerProp = Interlayers[interLayerPos.TypeId];
                            Vector3D vecMin = new Vector3D(
                                0.5 * (Analysis.ContainerDimensions.X - interlayerProp.Length)
                                , 0.5 * (Analysis.ContainerDimensions.Y - interlayerProp.Width)
                                , 0.0)
                                + Analysis.Offset;
                            _bbox.Extend(new BBox3D(vecMin, vecMin + interlayerProp.Dimensions));
                        }
                    }
                    // sanity check
                    if (!_bbox.IsValid)
                        _bbox.Extend(Vector3D.Zero);
                }
                return _bbox.Clone();
            }
        }

        public BBox3D BBoxLoadWDeco
        {
            get
            {
                return Analysis.BBoxLoadWDeco(BBoxLoad);
            }
        }
        public BBox3D BBoxGlobal
        { get { return Analysis.BBoxGlobal(BBoxLoad); } }
        public int InterlayerCount
        {
            get
            {
                int layerCount = 0, interlayerCount = 0, boxCount = 0;
                GetCounts(ref layerCount, ref interlayerCount, ref boxCount);
                return interlayerCount;
            }
        }
        public int ItemCount
        {
            get
            {
                int layerCount = 0, interlayerCount = 0, itemCount = 0;
                GetCounts(ref layerCount, ref interlayerCount, ref itemCount);
                return itemCount;
            }
        }
        public int LayerCount
        {
            get
            {
                int layerCount = 0, interlayerCount = 0, itemCount = 0;
                GetCounts(ref layerCount, ref interlayerCount, ref itemCount);
                return layerCount;
            }
        }
        private void GetCounts(ref int layerCount, ref int interlayerCount, ref int itemCount)
        {
            layerCount = 0;
            interlayerCount = 0;
            itemCount = 0;
            foreach (ILayer layer in Layers)
            {
                if (layer is Layer3DBox blayer)
                {
                    ++layerCount;
                    itemCount += blayer.BoxCount;
                }
                if (layer is Layer3DCyl clayer)
                {
                    ++layerCount;
                    itemCount += clayer.CylinderCount;
                }
                if (layer is InterlayerPos iLayer)
                    ++interlayerCount;
            }
        }
        public double LoadVolume => ItemCount * Analysis.Content.Volume;
        public bool HasNetWeight => Analysis.Content.NetWeight.Activated;
        public double LoadWeight => ItemCount * Analysis.ContentWeight;
        public double Weight => LoadWeight + Analysis.ContainerWeight;
        public OptDouble NetWeight => ItemCount * Analysis.Content.NetWeight;
        public double VolumeEfficiency => 100.0 * (ItemCount * Analysis.ContentVolume) / Analysis.ContainerLoadingVolume;
        public OptDouble WeightEfficiency
        {
            get
            {
                if (Analysis.ConstraintSet.OptMaxWeight.Activated)
                    return new OptDouble(false, 0.0);
                else
                    return new OptDouble(true, 100.0 * (ItemCount * Analysis.ContentWeight) / Analysis.ConstraintSet.OptMaxWeight.Value);
            }
        }
        public Vector3D LoadCOfG
        {
            get { return Vector3D.Zero; }
        }
        public Vector3D COfG
        {
            get { return Vector3D.Zero; }
        }
        public Dictionary<LayerPhrase, int> LayerPhrases
        {
            get
            {
                Dictionary<LayerPhrase, int> dict = new Dictionary<LayerPhrase, int>();
                foreach (SolutionItem solItem in _solutionItems)
                {
                    Layer2D layer = _layerTypes[solItem.LayerIndex] as Layer2D;
                    LayerPhrase lp = new LayerPhrase() { Count = layer.Count, Axis = layer.VerticalAxisProp };
                    if (dict.ContainsKey(lp))
                        dict[lp] += 1;
                    else
                        dict.Add(lp, 1);
                }
                return dict;
            }
        }
        public string NoLayersPerNoCasesString
        {
            get
            {
                // *** get dictionnary Number of cases <-> Number of layers
                Dictionary<int, int> dictLayerCount = new Dictionary<int, int>();
                foreach (ILayer layer in Layers)
                {
                    int boxCount = layer.BoxCount;
                    int noLayers = 1;
                    if (dictLayerCount.ContainsKey(boxCount))
                        noLayers += dictLayerCount[boxCount];
                    dictLayerCount[boxCount] = noLayers;
                }
                // *** build return string
                string s = string.Empty;
                foreach (KeyValuePair<int, int> kvp in dictLayerCount)
                {
                    if (!string.IsNullOrEmpty(s))
                        s += " + ";
                    s += $"{kvp.Value} x {kvp.Key}";
                }
                return s;
            }
        }
        public bool HasConstantNoCasesPerLayer
        {
            get
            {
                List<ILayer> layers = Layers;
                if (layers.Count < 1)
                    return false;
                int iCaseCount = layers[0].BoxCount;
                foreach (ILayer layer in layers)
                {
                    if (layer.BoxCount != iCaseCount)
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region Layer type methods
        public List<int> LayerTypeUsed(int layerTypeIndex)
        {
            List<int> solItemIndexes = new List<int>();
            int index = 0;
            foreach (SolutionItem solItem in _solutionItems)
            {
                if (solItem.LayerIndex == layerTypeIndex)
                    solItemIndexes.Add(index+1);
                ++index;
            }
            return solItemIndexes;            
        }
        public int NoLayerTypesUsed
        {
            get
            {
                int noLayerTypesUsed = 0;
                for (int i = 0; i < _layerTypes.Count; ++i)
                {
                    List<int> listLayerUsingType = LayerTypeUsed(i);
                    noLayerTypesUsed += listLayerUsingType.Count > 0 ? 1 : 0;
                }
                return noLayerTypesUsed;
            }
        }
        public string LayerCaption(int layerTypeIndex)
        {

            List<int> layerIndexes = LayerTypeUsed(layerTypeIndex);
            if (layerIndexes.Count == LayerCount)
                return Properties.Resources.ID_LAYERSALL;


            StringBuilder sb = new StringBuilder();
            sb.Append(layerIndexes.Count > 1 ? Properties.Resources.ID_LAYERS : Properties.Resources.ID_LAYER);
            int iCountIndexes = layerIndexes.Count;
            for (int j = 0; j < iCountIndexes; ++j)
            {
                sb.AppendFormat("{0}", layerIndexes[j]);
                if (j != iCountIndexes - 1)
                {
                    sb.Append(",");
                    if (j != 0 && 0 == j % 10)
                        sb.Append("\n");
                }
            }
            return sb.ToString();
        }
        public int LayerBoxCount(int layerTypeIndex)
        {
            return _layerTypes[layerTypeIndex].Count;
        }
        public double LayerWeight(int layerTypeIndex)
        {
            return LayerBoxCount(layerTypeIndex) * Analysis.ContentWeight;
        }
        public double LayerNetWeight(int layerTypeIndex)
        {
            return LayerBoxCount(layerTypeIndex) * Analysis.Content.NetWeight.Value;
        }
        public double LayerMaximumSpace(int LayerTypeIndex)
        {
            return _layerTypes[LayerTypeIndex].MaximumSpace;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// if box bottom oriented to Z+, reverse box
        /// </summary>
        private LayerPosition AdjustLayerPosition(LayerPosition layerPos, bool reflectionX, bool reflectionY)
        {
            
            Vector3D dimensions = Analysis.ContentDimensions;
            Vector2D containerDims = Analysis.ContainerDimensions;

            // implement change
            LayerPosition layerPosTemp = new LayerPosition(layerPos);

            // apply symetry in X
            if (reflectionX)
            {
                Matrix4D matRot = new Matrix4D(
                  1.0, 0.0, 0.0, 0.0
                  , 0.0, -1.0, 0.0, 0.0
                  , 0.0, 0.0, 1.0, 0.0
                  , 0.0, 0.0, 0.0, 1.0
                  );
                Vector3D vTranslation = new Vector3D(0.0, containerDims.Y, 0.0);
                layerPosTemp = ApplyReflection(layerPosTemp, matRot, vTranslation);
            }
            // apply symetry in Y
            if (reflectionY)
            {
                Matrix4D matRot = new Matrix4D(
                    -1.0, 0.0, 0.0, 0.0
                    , 0.0, 1.0, 0.0, 0.0
                    , 0.0, 0.0, 1.0, 0.0
                    , 0.0, 0.0, 0.0, 1.0
                    );
                Vector3D vTranslation = new Vector3D(containerDims.X, 0.0, 0.0);
                layerPosTemp = ApplyReflection(layerPosTemp, matRot, vTranslation);
            }
            return layerPosTemp.Adjusted(dimensions);
        }

        private Vector3D AdjustPosition(Vector3D v, bool reflectionX, bool reflectionY)
        {
            Vector2D containerDims = Analysis.ContainerDimensions;
            Vector3D posTemp = new Vector3D(v);
            // apply symetry in X
            if (reflectionX)
            {
                Matrix4D matRot = new Matrix4D(
                  1.0, 0.0, 0.0, 0.0
                  , 0.0, -1.0, 0.0, 0.0
                  , 0.0, 0.0, 1.0, 0.0
                  , 0.0, 0.0, 0.0, 1.0
                  );
                Vector3D vTranslation = new Vector3D(0.0, containerDims.Y, 0.0);
                posTemp = ApplyReflection(posTemp, matRot, vTranslation);
            }
            // apply symetry in Y
            if (reflectionY)
            {
                Matrix4D matRot = new Matrix4D(
                    -1.0, 0.0, 0.0, 0.0
                    , 0.0, 1.0, 0.0, 0.0
                    , 0.0, 0.0, 1.0, 0.0
                    , 0.0, 0.0, 0.0, 1.0
                    );
                Vector3D vTranslation = new Vector3D(containerDims.X, 0.0, 0.0);
                posTemp = ApplyReflection(posTemp, matRot, vTranslation);
            }
            return posTemp;
        }

        private LayerPosition ApplyReflection(LayerPosition layerPos, Matrix4D matRot, Vector3D vTranslation)
        {
            Vector3D dimensions = Analysis.ContentDimensions;
            Transform3D transfRot = new Transform3D(matRot);
            HalfAxis.HAxis axisLength = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(layerPos.LengthAxis)));
            HalfAxis.HAxis axisWidth = HalfAxis.ToHalfAxis(transfRot.transform(HalfAxis.ToVector3D(layerPos.WidthAxis)));
            matRot.M14 = vTranslation[0];
            matRot.M24 = vTranslation[1];
            matRot.M34 = vTranslation[2];
            Transform3D transfRotTranslation = new Transform3D(matRot);

            Vector3D transPos = transfRotTranslation.transform( new Vector3D(layerPos.Position.X, layerPos.Position.Y, layerPos.Position.Z) );
            return new LayerPosition(
                new Vector3D(transPos.X, transPos.Y, transPos.Z)
                    - dimensions.Z * Vector3D.CrossProduct(HalfAxis.ToVector3D(axisLength), HalfAxis.ToVector3D(axisWidth))
                , axisLength
                , axisWidth);
        }

        private Vector3D ApplyReflection(Vector3D vPos, Matrix4D matRot, Vector3D vTranslation)
        {
            Transform3D transfRot = new Transform3D(matRot);
            matRot.M14 = vTranslation[0];
            matRot.M24 = vTranslation[1];
            matRot.M34 = vTranslation[2];
            Transform3D transfRotTranslation = new Transform3D(matRot);
            return transfRotTranslation.transform(vPos);  
        }

        private bool HasValidSelection
        {
            get { return SelectedLayerIndex >= 0 && SelectedLayerIndex < _solutionItems.Count; }
        }

        public List<LayerSummary> ListLayerSummary
        {
            get
            {
                List<LayerSummary> _layerSummaries = new List<LayerSummary>();
                int layerCount = 0;
                foreach (SolutionItem solItem in _solutionItems)
                {
                    LayerSummary layerSum = _layerSummaries.Find(delegate(LayerSummary lSum) { return lSum.IsLayerTypeOf(solItem); });
                    if (null == layerSum)
                    {
                        layerSum = new LayerSummary(this, solItem.LayerIndex, solItem.SymetryX, solItem.SymetryY);
                        _layerSummaries.Add(layerSum);
                    }
                    layerSum.AddIndex(++layerCount);
                }
                return _layerSummaries;
            }
        }

        private int GetLayerIndexFromLayerDesc(LayerDesc layerDesc)
        {
            int index =_layerTypes.FindIndex(l => l.LayerDescriptor.ToString() == layerDesc.ToString());
            if (-1 == index)
                throw new Exception("No valid layer with desc {layerDesc}");
            return index;
        }
        #endregion

        #region Static members
        private static ILog _log = LogManager.GetLogger(typeof(Solution));
        #endregion
    }
    #endregion
}
