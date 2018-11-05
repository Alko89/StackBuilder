#region Using directives
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

using Sharp3D.Math.Core;
using log4net;
#endregion

namespace treeDiM.StackBuilder.Basics
{
    /// <summary>
    /// Classes that encapsulates data
    /// The application is MDI and might host several Document instance
    /// </summary>
    public class Document
    {
        #region Data members
        private UnitsManager.UnitSystem _unitSystem;
        private List<ItemBase> _typeList = new List<ItemBase>();
        private List<AnalysisLegacy> _analysesLegacy = new List<AnalysisLegacy>();
 
        private List<IDocumentListener> _listeners = new List<IDocumentListener>();
        private static ILayerSolver _solver;
        protected static readonly ILog _log = LogManager.GetLogger(typeof(Document));

        public delegate void DocumentClosing(Document document);
        public event DocumentClosing DocumentClosed;
        #endregion

        #region Constructor
        public Document(string filePath, IDocumentListener listener)
        {
            // set name from file path
            Name = Path.GetFileNameWithoutExtension(filePath);
            if (null != listener)
            {
                // add listener
                AddListener(listener);
                // notify listener of document creation
                listener.OnNewDocument(this);
            }
            // load file
            Load(filePath);            
            // rechange name to match filePath
            Name = Path.GetFileNameWithoutExtension(filePath);
        }

        public Document(string name, string description, string author, DateTime dateCreated, IDocumentListener listener)
        {
            Name = name;
            Description = description;
            Author = author;
            DateOfCreation = dateCreated;
            if (null != listener)
            {
                // add listener
                AddListener(listener);
                // notify listener of document creation
                listener.OnNewDocument(this);
            }
        }
        #endregion

        #region Name checking / Getting new name
        public bool IsValidNewTypeName(string name, ItemBase itemToName)
        {
            // make sure is not empty
            if (name.Trim() == string.Empty)
                return false;
            // make sure it is not already used
            return null == _typeList.Find(
                delegate(ItemBase item)
                {   return (item != itemToName)
                    && string.Equals(item.ID.Name.Trim(), name.Trim(), StringComparison.CurrentCultureIgnoreCase); }
                );
        }
        public string GetValidNewTypeName(string prefix)
        {
            int index = 0;
            string name = string.Empty;
            while (!IsValidNewTypeName(name = string.Format("{0}{1}", prefix, index), null))
                ++index;
            while (!IsValidNewAnalysisName(name = string.Format("{0}{1}", prefix, index), null))
                ++index;
            return name;
        }
        public bool IsValidNewAnalysisName(string name, ItemBase analysisToRename)
        {
            string trimmedName = name.Trim();
            return (null == Analyses.Find(
                delegate(Analysis analysis)
                {
                    return analysis != analysisToRename
                        && string.Equals(analysis.ID.Name, trimmedName, StringComparison.InvariantCultureIgnoreCase);
                }
                ))
                && (null == _analysesLegacy.Find(
                delegate(AnalysisLegacy analysis)
                {
                    return analysis != analysisToRename
                        && string.Equals(analysis.ID.Name, trimmedName, StringComparison.InvariantCultureIgnoreCase);
                }
                ));
        }
        public string GetValidNewAnalysisName(string prefix)
        {
            int index = 0;
            string name = string.Empty;
            while (!IsValidNewAnalysisName(name = string.Format("{0}{1}", prefix, index), null))
                ++index;
            return name;
        }
        #endregion

        #region Public instantiation methods
        public Packable CreateNewPackable(Packable packable)
        {
            if (packable is BoxProperties boxProperties)
                return CreateNewCase(boxProperties);
            if (packable is BundleProperties bundleProperties)
                return CreateNewBundle(bundleProperties);
            if (packable is PackProperties packProperties)
                return CreateNewPack(packProperties);
            return null;
        }
        /// <summary>
        /// Create a new box
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="length">Length</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="weight">Weight</param>
        /// <param name="colors">Name</param>
        /// <returns>created BoxProperties instance</returns>
        public BoxProperties CreateNewBox(
            string name, string description
            , double length, double width, double height
            , double weight
            , Color[] colors)
        {
            // instantiate and initialize
            BoxProperties boxProperties = new BoxProperties(this, length, width, height);
            boxProperties.SetWeight( weight );
            boxProperties.ID.SetNameDesc( name, description);
            boxProperties.SetAllColors(colors);
            // insert in list
            _typeList.Add(boxProperties);
            // notify listeners
            NotifyOnNewTypeCreated(boxProperties);
            Modify();
            return boxProperties;
        }
        public BoxProperties CreateNewBox(BoxProperties boxProp)
        { 
            // instantiate and initialize
            BoxProperties boxPropClone = new BoxProperties(this
                , boxProp.Length
                , boxProp.Width
                , boxProp.Height);
            boxPropClone.SetWeight( boxProp.Weight );
            boxPropClone.SetNetWeight( boxProp.NetWeight );
            boxPropClone.ID.SetNameDesc( boxProp.ID.Name, boxProp.ID.Description );
            boxPropClone.SetAllColors(boxProp.Colors);
            // insert in list
            _typeList.Add(boxPropClone);
            // notify listeners
            NotifyOnNewTypeCreated(boxPropClone);
            Modify();

            return boxPropClone;
        }
        /// <summary>
        /// Create a new case
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="insideLength"></param>
        /// <param name="insideWidth"></param>
        /// <param name="insideHeight"></param>
        /// <param name="weight"></param>
        /// <param name="colors"></param>
        /// <returns></returns>
        public BoxProperties CreateNewCase(
            string name, string description
            , double length, double width, double height
            , double insideLength, double insideWidth, double insideHeight
            , double weight
            , Color[] colors)
        {
            // instantiate and initialize
            BoxProperties boxProperties = new BoxProperties(this, length, width, height, insideLength, insideWidth, insideHeight);
            boxProperties.SetWeight( weight );
            boxProperties.ID.SetNameDesc( name, description);
            boxProperties.SetAllColors(colors);
            // insert in list
            _typeList.Add(boxProperties);
            // notify listeners
            NotifyOnNewTypeCreated(boxProperties);
            Modify();
            return boxProperties;
        }
        public BoxProperties CreateNewCase(
            string name, string description
            , double length, double width, double height
            , double weight
            , Color[] colors)
        {
            // instantiate and initialize
            BoxProperties boxProperties = new BoxProperties(this, length, width, height);
            boxProperties.SetWeight(weight);
            boxProperties.ID.SetNameDesc(name, description);
            boxProperties.SetAllColors(colors);
            // insert in list
            _typeList.Add(boxProperties);
            // notify listeners
            NotifyOnNewTypeCreated(boxProperties);
            Modify();
            return boxProperties;
        }
        public BoxProperties CreateNewCase(BoxProperties boxProp)
        {
            // instantiate and initialize
            BoxProperties boxPropClone = new BoxProperties(this
                , boxProp.Length
                , boxProp.Width
                , boxProp.Height
                , boxProp.InsideLength
                , boxProp.InsideWidth
                , boxProp.InsideHeight);
            boxPropClone.SetWeight( boxProp.Weight );
            boxPropClone.SetNetWeight( boxProp.NetWeight );
            boxPropClone.ID.SetNameDesc( boxProp.ID.Name, boxProp.ID.Description );
            boxPropClone.SetAllColors(boxProp.Colors);
            boxPropClone.TapeWidth = boxProp.TapeWidth;
            boxPropClone.TapeColor = boxProp.TapeColor;
            // insert in list
            _typeList.Add(boxPropClone);
            // notify listeners
            NotifyOnNewTypeCreated(boxPropClone);
            Modify();
            return boxPropClone;
        }
        /// <summary>
        /// Create a new pack
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="box">Inner box</param>
        /// <param name="arrangement">Arrangement</param>
        /// <param name="axis">Axis</param>
        /// <param name="wrapper">Wrapper</param>
        /// <returns></returns>
        public PackProperties CreateNewPack(
            string name, string description
            , BoxProperties box
            , PackArrangement arrangement
            , HalfAxis.HAxis axis
            , PackWrapper wrapper)
        { 
            // instantiate and initialize
            PackProperties packProperties = new PackProperties(this
                , box
                , arrangement
                , axis
                , wrapper);
            packProperties.ID.SetNameDesc( name, description );
            // insert in list
            _typeList.Add(packProperties);
            // notify listeners
            NotifyOnNewTypeCreated(packProperties);
            Modify();
            return packProperties;
        }
        public PackProperties CreateNewPack(PackProperties pack)
        {
            PackProperties packProperties = new PackProperties(this, pack.Box, pack.Arrangement, pack.BoxOrientation, pack.Wrap);
            packProperties.ID.SetNameDesc(pack.ID.Name, pack.ID.Description);
            // insert in list
            _typeList.Add(packProperties);
            // notify listeners
            NotifyOnNewTypeCreated(packProperties);
            return packProperties;
        }
        public BundleProperties CreateNewBundle(
            string name, string description
            , double length, double width, double thickness
            , double weight
            , Color color
            , int noFlats)
        {
            // instantiate and initialize
            BundleProperties bundle = new BundleProperties(this, name, description, length, width, thickness, weight, noFlats, color);
            // insert in list
            _typeList.Add(bundle);
            // notify listeners
            NotifyOnNewTypeCreated(bundle);
            Modify();
            return bundle;
        }
        public BundleProperties CreateNewBundle(BundleProperties bundleProp)
        { 
            // instantiate
            BundleProperties bundlePropClone = new BundleProperties(this,
                bundleProp.Name, bundleProp.Description,
                bundleProp.Length, bundleProp.Width, bundleProp.UnitThickness, bundleProp.UnitWeight,
                bundleProp.NoFlats, bundleProp.Color);
            _typeList.Add(bundlePropClone);
            NotifyOnNewTypeCreated(bundlePropClone);
            Modify();
            return bundlePropClone;
        }
        public CylinderProperties CreateNewCylinder(CylinderProperties cyl)
        {
            // cylinder
            CylinderProperties cylinder = new CylinderProperties(this
                , cyl.ID.Name, cyl.ID.Description
                , cyl.RadiusOuter, cyl.RadiusInner, cyl.Height
                , cyl.Weight
                , cyl.ColorTop, cyl.ColorWallOuter, cyl.ColorWallInner);
            // insert in list
            _typeList.Add(cylinder);
            // notify listeners
            NotifyOnNewTypeCreated(cylinder);
            Modify();
            return cylinder;
        }
        public CylinderProperties CreateNewCylinder(
            string name, string description
            , double radiusOuter, double radiusInner, double height
            , double weight
            , Color colorTop, Color colorWallOuter, Color colorWallInner)
        {
            CylinderProperties cylinder = new CylinderProperties(this, name, description
                , radiusOuter, radiusInner, height, weight
                , colorTop, colorWallOuter, colorWallInner);
            // insert in list
            _typeList.Add(cylinder);
            // notify listeners
            NotifyOnNewTypeCreated(cylinder);
            Modify();
            return cylinder;        
        }

        public void AddType(ItemBase item)
        {
            // insert in list
            _typeList.Add(item);
            // notify listeners
            NotifyOnNewTypeCreated(item);
            Modify();
        }
        public InterlayerProperties CreateNewInterlayer(
            string name, string description
            , double length, double width, double thickness
            , double weight
            , Color color)
        { 
            // instantiate and intialize
            InterlayerProperties interlayer = new InterlayerProperties(
                this, name, description
                , length, width, thickness
                , weight, color);
            // insert in list
            _typeList.Add(interlayer);
            // notify listeners
            NotifyOnNewTypeCreated(interlayer);
            Modify();
            return interlayer;
        }
        public PalletCornerProperties CreateNewPalletCorners(string name, string description,
            double length, double width, double thickness,
            double weight,
            Color color)
        {
            // instantiate and initialize
            PalletCornerProperties palletCorners = new PalletCornerProperties(
                this,
                name, description,
                length, width, thickness,
                weight,
                color);
            // insert in list
            _typeList.Add(palletCorners);
            // notify listeners
            NotifyOnNewTypeCreated(palletCorners);
            Modify();
            return palletCorners;
        }
        public PalletCapProperties CreateNewPalletCap(PalletCapProperties palletCap)
        { 
            // instantiate and initialize
                PalletCapProperties palletCapClone = new PalletCapProperties(
                    this,
                    palletCap.ID.Name, palletCap.ID.Description,
                    palletCap.Length, palletCap.Width, palletCap.Height,
                    palletCap.InsideLength, palletCap.InsideWidth, palletCap.InsideHeight,
                    palletCap.Weight, palletCap.Color);
                // insert in list
                _typeList.Add(palletCapClone);
                // notify listeners
                NotifyOnNewTypeCreated(palletCapClone);
                Modify();
                return palletCapClone;
        }

        public PalletCapProperties CreateNewPalletCap(
            string name, string description,
            double length, double width, double height,
            double innerLength, double innerWidth, double innerHeight,
            double weight,
            Color color)
        {
            // instantiate and initialize
            PalletCapProperties palletCap = new PalletCapProperties(
                this,
                name, description,
                length, width, height,
                innerLength, innerWidth, innerHeight,
                weight, color);
            // insert in list
            _typeList.Add(palletCap);
            // notify listeners
            NotifyOnNewTypeCreated(palletCap);
            Modify();
            return palletCap;
        }

        public PalletFilmProperties CreateNewPalletFilm(PalletFilmProperties palletFilm)
        {
            // instantiate and initialize
            PalletFilmProperties palletFilmClone = new PalletFilmProperties(
                this,
                palletFilm.ID.Name, palletFilm.ID.Description,
                palletFilm.UseTransparency, palletFilm.UseHatching,
                palletFilm.HatchSpacing, palletFilm.HatchAngle,
                palletFilm.Color);
            // insert in list
            _typeList.Add(palletFilmClone);
            // notify listeners
            NotifyOnNewTypeCreated(palletFilmClone);
            Modify();
            return palletFilmClone; 
        }

        public PalletFilmProperties CreateNewPalletFilm(
            string name, string description,
            bool useTransparency,
            bool useHatching, double hatchSpacing, double hatchAngle,
            Color color)
        {
            // instantiate and initialize
            PalletFilmProperties palletFilm = new PalletFilmProperties(
                this,
                name, description,
                useTransparency,
                useHatching, hatchSpacing, hatchAngle,
                color);
            // insert in list
            _typeList.Add(palletFilm);
            // notify listeners
            NotifyOnNewTypeCreated(palletFilm);
            Modify();
            return palletFilm;
        }
        public InterlayerProperties CreateNewInterlayer(InterlayerProperties interlayerProp)
        {
            // instantiate and intialize
            InterlayerProperties interlayerClone = new InterlayerProperties(
                this, interlayerProp.ID.Name, interlayerProp.ID.Description
                , interlayerProp.Length, interlayerProp.Width, interlayerProp.Thickness
                , interlayerProp.Weight
                , interlayerProp.Color);
            // insert in list
            _typeList.Add(interlayerClone);
            // notify listeners
            NotifyOnNewTypeCreated(interlayerClone);
            Modify();
            return interlayerClone;       
        }
        public PalletProperties CreateNewPallet(
            string name, string description,
            string typeName,
            double length, double width, double height,
            double weight,
            Color palletColor)
        {
            PalletProperties palletProperties = new PalletProperties(this, typeName, length, width, height);
            palletProperties.ID.SetNameDesc( name, description );
            palletProperties.Weight = weight;
            palletProperties.Color = palletColor;
            // insert in list
            _typeList.Add(palletProperties);
            // notify listeners
            NotifyOnNewTypeCreated(palletProperties);
            Modify();
            return palletProperties;
        }

        public PalletProperties CreateNewPallet(PalletProperties palletProp)
        {
            PalletProperties palletPropClone = new PalletProperties(this, palletProp.TypeName, palletProp.Length, palletProp.Width, palletProp.Height);
            palletPropClone.ID.SetNameDesc( palletProp.ID.Name, palletProp.ID.Description);
            palletPropClone.Weight = palletProp.Weight;
            palletPropClone.Color = palletProp.Color;
            palletPropClone.AdmissibleLoadWeight = palletProp.AdmissibleLoadWeight;
            // insert in list
            _typeList.Add(palletPropClone);
            // notify listeners
            NotifyOnNewTypeCreated(palletPropClone);
            Modify();
            return palletPropClone;           
        }

        /// <summary>
        /// Creates a new truck in this document
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="length">Length</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="admissibleLoadWeight">AdmissibleLoadWeight</param>
        /// <param name="color">Color</param>
        /// <returns>TruckProperties</returns>
        public TruckProperties CreateNewTruck(
            string name, string description
            , double length
            , double width
            , double height
            , double admissibleLoadWeight
            , Color color)
        {
            TruckProperties truckProperties = new TruckProperties(this, length, width, height);
            truckProperties.ID.SetNameDesc( name, description);
            truckProperties.AdmissibleLoadWeight = admissibleLoadWeight;
            truckProperties.Color = color;
            // insert in list
            _typeList.Add(truckProperties);
            // notify listeners
            NotifyOnNewTypeCreated(truckProperties);
            Modify();
            return truckProperties;
        }
        #endregion

        #region Public static methods
        public static void SetSolver(ILayerSolver solver)
        {
            _solver = solver;
        }
        #endregion

        #region Analyses instantiation method
        public Analysis CreateNewAnalysisCasePallet(
            string name, string description
            , Packable packable, PalletProperties pallet
            , List<InterlayerProperties> interlayers
            , PalletCornerProperties palletCorners, PalletCapProperties palletCap, PalletFilmProperties palletFilm
            , ConstraintSetCasePallet constraintSet
            , List<LayerDesc> layerDescs
            )
        {
            AnalysisCasePallet analysis = new AnalysisCasePallet(packable, pallet, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.PalletCornerProperties     = palletCorners;
            analysis.PalletCapProperties        = palletCap;
            analysis.PalletFilmProperties       = palletFilm;
            analysis.AddSolution(layerDescs);

            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisCasePallet(
            string name, string description
            , Packable packable, PalletProperties pallet
            , List<InterlayerProperties> interlayers
            , PalletCornerProperties palletCorners, PalletCapProperties palletCap, PalletFilmProperties palletFilm
            , ConstraintSetCasePallet constraintSet
            , List<KeyValuePair<LayerDesc, int>> listLayers
            )
        {
            AnalysisCasePallet analysis = new AnalysisCasePallet(packable, pallet, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            if (null != interlayers)
            {
                foreach (InterlayerProperties interlayer in interlayers)
                    analysis.AddInterlayer(interlayer);
            }
            analysis.PalletCornerProperties = palletCorners;
            analysis.PalletCapProperties = palletCap;
            analysis.PalletFilmProperties = palletFilm;
            analysis.AddSolution(listLayers);

            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisBoxCase(
            string name, string description
            , Packable packable, BoxProperties caseProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetBoxCase constraintSet
            , List<LayerDesc> layerDescs
            )
        {
            AnalysisBoxCase analysis = new AnalysisBoxCase(
                this, packable, caseProperties, constraintSet);
            analysis.ID.SetNameDesc( name, description );
            analysis.AddSolution(layerDescs);

            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisCaseTruck(
            string name, string description
            , Packable packable, TruckProperties truckProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetCaseTruck constraintSet
            , List<LayerDesc> layerDescs)
        {
            AnalysisCaseTruck analysis = new AnalysisCaseTruck(
                this, packable, truckProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            analysis.AddSolution(layerDescs);
            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisPalletTruck(
            string name, string description
            , Packable loadedPallet, TruckProperties truckProperties
            , ConstraintSetPalletTruck constraintSet
            , List<LayerDesc> layerDescs)
        {
            AnalysisPalletTruck analysis = new AnalysisPalletTruck(
                loadedPallet, truckProperties, constraintSet);
            analysis.ID.SetNameDesc( name, description );
            analysis.AddSolution(layerDescs);

            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisCylinderPallet(
            string name, string description
            , CylinderProperties cylinder, PalletProperties palletProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetPackablePallet constraintSet
            , List<LayerDesc> layerDescs
            )
        { 
            // analysis
            AnalysisCylinderPallet analysis = new AnalysisCylinderPallet(
                cylinder, palletProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            analysis.AddSolution(layerDescs);

            return InsertAnalysis(analysis);
        }
        public Analysis CreateNewAnalysisCylinderCase(
            string name, string description
            , CylinderProperties cylinder, BoxProperties caseProperties
            , List<InterlayerProperties> interlayers
            , ConstraintSetCylinderCase constraintSet
            , List<LayerDesc> layerDescs)
        {
            // analysis
            AnalysisCylinderCase analysis = new AnalysisCylinderCase(
                this, cylinder, caseProperties, constraintSet);
            analysis.ID.SetNameDesc(name, description);
            analysis.AddSolution(layerDescs);

            return InsertAnalysis(analysis);
        }

        public HAnalysis CreateNewHAnalysisCasePallet(
            string name, string description,
            List<ContentItem> contentItems,
            PalletProperties palletProperties,
            HConstraintSetPallet constraintSet,
            HSolution solution
            )
        {
            HAnalysisPallet analysis = new HAnalysisPallet(this)
            {
                Content = contentItems,
                Pallet = palletProperties,
                ConstraintSet = constraintSet,
                Solution = solution
            };
            analysis.ID.SetNameDesc(name, description);
            return InsertAnalysis(analysis);
        }

        private Analysis InsertAnalysis(Analysis analysis)
        {
            Analyses.Add(analysis);
            // notify listeners
            NotifyOnNewAnalysisCreated(analysis);
            // set document dirty
            Modify();
            return analysis;
        }
        private HAnalysis InsertAnalysis(HAnalysis analysis)
        {
            HAnalyses.Add(analysis);
            // notify listeners
            NotifyOnNewAnalysisCreated(analysis);
            // set document dirty
            Modify();
            return analysis;
        }
        public void UpdateAnalysis(Analysis analysis)
        {
            // notify listeners
            NotifyAnalysisUpdated(analysis);
            // set document dirty
            Modify();
        }
        public void UpdateAnalysis(HAnalysis analysis)
        {
            // notify listeners
            NotifyAnalysisUpdated(analysis);
            // set document dirty
            Modify();
        }

        #endregion

        #region Legacy analyses instantiation method
        /// <summary>
        /// Creates a new analysis in this document + compute solutions
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="box"></param>
        /// <param name="pallet"></param>
        /// <param name="interlayer"></param>
        /// <param name="constraintSet"></param>
        /// <param name="solver">Node : analysis creation requires a solver</param>
        /// <returns>An analysis</returns>
        public Analysis CreateNewCasePalletAnalysis(
            string name, string description
            , BProperties box, PalletProperties pallet
            , InterlayerProperties interlayer, InterlayerProperties interlayerAntiSlip
            , PalletCornerProperties palletCorners, PalletCapProperties palletCap, PalletFilmProperties palletFilm
            , PalletConstraintSet constraintSet
            , ILayerSolver solver)
        {
            ConstraintSetCasePallet constraintSetNew = new ConstraintSetCasePallet();
            constraintSetNew.SetMaxHeight( new OptDouble(true, constraintSet.MaximumHeight) );
            constraintSetNew.OptMaxWeight = new OptDouble(constraintSet.UseMaximumPalletWeight, constraintSet.MaximumPalletWeight);
            constraintSetNew.Overhang = new Vector2D(constraintSet.OverhangX, constraintSet.OverhangY);
            
            List<InterlayerProperties> listInterlayers = new List<InterlayerProperties>();
            if (null != interlayerAntiSlip)
                listInterlayers.Add(interlayerAntiSlip);
            if (null != interlayer)
                listInterlayers.Add(interlayer);

            List<LayerDesc> layerDescs = new List<LayerDesc>();
            if (null != solver)
            {
                List<Layer2D> layers = solver.BuildLayers(
                    box.OuterDimensions
                    , new Vector2D(pallet.Length + constraintSetNew.Overhang.X, pallet.Width + constraintSetNew.Overhang.Y), pallet.Height
                    , constraintSetNew
                    , true);
                if (layers.Count > 0)
                    layerDescs.Add(layers[0].LayerDescriptor);
            }

            Analysis analysis = CreateNewAnalysisCasePallet(name, description, box, pallet
                , listInterlayers, palletCorners, palletCap, palletFilm
                , constraintSetNew, layerDescs);

            Modify();
            return analysis;
        }

        public Analysis CreateNewPackPalletAnalysis(
            string name, string description
            , PackProperties pack, PalletProperties pallet
            , InterlayerProperties interlayer
            , PackPalletConstraintSet constraintSet
            , ILayerSolver solver)
        {
            ConstraintSetCasePallet constraintSetNew = new ConstraintSetCasePallet();
            constraintSetNew.SetMaxHeight(constraintSet.MaximumPalletHeight);
            constraintSetNew.OptMaxWeight = constraintSet.MaximumPalletWeight;
            constraintSetNew.Overhang = new Vector2D(constraintSet.OverhangX, constraintSet.OverhangY);
            constraintSetNew.SetAllowedOrientations(new bool[3] { false, false, true });

            List<InterlayerProperties> listInterlayers = new List<InterlayerProperties>();
            if (null != interlayer)
                listInterlayers.Add(interlayer);

            List<LayerDesc> layerDescs = new List<LayerDesc>();
            if (null != solver)
            {
                List<Layer2D> layers = solver.BuildLayers(
                    pack.OuterDimensions
                    , new Vector2D(pallet.Length + constraintSetNew.Overhang.X, pallet.Width + constraintSetNew.Overhang.Y), pallet.Height
                    , constraintSetNew
                    , true);
                if (layers.Count > 0)
                    layerDescs.Add(layers[0].LayerDescriptor);
            }

            Analysis analysis = CreateNewAnalysisCasePallet(
                name, description
                , pack, pallet
                , listInterlayers, null, null, null
                , constraintSetNew, layerDescs);

            Modify();
            return analysis;
        }

        public Analysis CreateNewPackPalletAnalysis(
            string name, string description
            , PackProperties pack, PalletProperties pallet
            , InterlayerProperties interlayer
            , PackPalletConstraintSet constraintSet
            , List<PackPalletSolution> solutions
            , ILayerSolver solver)
        {
            ConstraintSetCasePallet constraintSetNew = new ConstraintSetCasePallet();
            constraintSetNew.SetMaxHeight(constraintSet.MaximumPalletHeight);
            constraintSetNew.OptMaxWeight = constraintSet.MaximumPalletWeight;
            constraintSetNew.Overhang = new Vector2D(constraintSet.OverhangX, constraintSet.OverhangY);

            List<InterlayerProperties> listInterlayers = new List<InterlayerProperties>();
            if (null != interlayer)
                listInterlayers.Add(interlayer);

            List<LayerDesc> layerDescs = new List<LayerDesc>();
            if (null != solver)
            {
                List<Layer2D> layers = solver.BuildLayers(
                    pack.OuterDimensions
                    , new Vector2D(pallet.Length + constraintSetNew.Overhang.X, pallet.Width + constraintSetNew.Overhang.Y), pallet.Height
                    , constraintSetNew
                    , true);
                if (layers.Count > 0)
                    layerDescs.Add(layers[0].LayerDescriptor);
            }

            Analysis analysis = CreateNewAnalysisCasePallet(
                name, description
                , pack, pallet
                , listInterlayers, null, null, null
                , constraintSetNew, layerDescs);

            Modify();
            return analysis;
        }
 
        public void RemoveItem(ItemBase item)
        {
            // sanity check
            if (null == item)
            {
                Debug.Assert(false);
                return;
            }
            // dispose item first as it may remove dependancies itself
            _log.Debug(string.Format("Disposing {0}...", item.ID.Name));
            item.Dispose();

            // notify listeners / remove
            if (item.GetType() == typeof(BoxProperties)
                || item.GetType() == typeof(BundleProperties)
                || item.GetType() == typeof(PackProperties)
                || item.GetType() == typeof(PalletProperties)
                || item.GetType() == typeof(InterlayerProperties)
                || item.GetType() == typeof(PalletCornerProperties)
                || item.GetType() == typeof(PalletCapProperties)
                || item.GetType() == typeof(PalletFilmProperties)
                || item.GetType() == typeof(TruckProperties)
                || item.GetType() == typeof(CylinderProperties))
            {
                NotifyOnTypeRemoved(item);
                if (!_typeList.Remove(item))
                    _log.Warn(string.Format("Failed to properly remove item {0}", item.ID.Name));
            }
            else if (item is Analysis)
            {
                NotifyOnAnalysisRemoved(item as Analysis);
                if (!Analyses.Remove(item as Analysis))
                    _log.Warn(string.Format("Failed to properly remove analysis {0}", item.ID.Name));
            }
            else if (item is AnalysisLegacy)
            { 
                NotifyOnAnalysisRemoved(item);
                if (!_analysesLegacy.Remove(item as AnalysisLegacy))
                    _log.Warn(string.Format("Failed to properly remove analysis {0}", item.ID.Name));
            }
            else
                _log.Error(string.Format("Removing document {0} of unknown type {1}...", item.ID.Name, item.GetType()));
            Modify();
        }
        #endregion

        #region Public properties
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime DateOfCreation { get; set; }
        public ReadOnlyCollection<ItemBase> TypeList
        {
            get { return new ReadOnlyCollection<ItemBase>(_typeList); }
        }
        public IEnumerable<BoxProperties> Bricks =>
            _typeList.OfType<BoxProperties>();

        public IEnumerable<BoxProperties> Boxes =>
            _typeList.OfType<BoxProperties>().Where(x => !x.HasInsideDimensions);

        public IEnumerable<BoxProperties> Cases =>
            _typeList.OfType<BoxProperties>().Where(x => x.HasInsideDimensions);

        public IEnumerable<BundleProperties> Bundles =>
            _typeList.OfType<BundleProperties>();

        public IEnumerable<CylinderProperties> Cylinders =>
            _typeList.OfType<CylinderProperties>();

        public IEnumerable<PalletProperties> Pallets =>
            _typeList.OfType<PalletProperties>();

        public IEnumerable<InterlayerProperties> Interlayers =>
            _typeList.OfType<InterlayerProperties>();

        public IEnumerable<TruckProperties> Trucks =>
            _typeList.OfType<TruckProperties>();

        public IEnumerable<ItemBase> GetByType(Type t) =>
            _typeList.Where(item => item.GetType() == t);

        public List<Analysis> Analyses { get; } = new List<Analysis>();
        public List<HAnalysis> HAnalyses { get; } = new List<HAnalysis>();

        /// <summary>
        /// Get list of case/pallet analyses
        /// </summary>
        public IEnumerable<AnalysisLegacy> AnalysesCasePallet =>
            _analysesLegacy.OfType<AnalysisLegacy>();
        #endregion

        #region Allowing analysis/opti
        public bool CanCreatePack => Boxes.Any();
        public bool CanCreateAnalysisCasePallet
        { get { return Cases.Any() && Pallets.Any(); } }
        public bool CanCreateAnalysisBundlePallet
        { get { return Bundles.Any() && Pallets.Any(); } }
        public bool CanCreateAnalysisBoxCase
        { get { return (Boxes.Any() || Cases.Any()) && Cases.Any(); } }
        public bool CanCreateAnalysisBundleCase
        { get { return Bundles.Any() && Cases.Any(); } }
        public bool CanCreateAnalysisCylinderPallet
        { get { return Cylinders.Any() && Pallets.Any(); } }
        public bool CanCreateAnalysisCylinderCase
        { get { return Cylinders.Any() && Cases.Any(); } }
        public bool CanCreateAnalysisPalletTruck
        { get { return Trucks.Any(); } }
        public bool CanCreateAnalysisCaseTruck
        { get { return Trucks.Any() && Cases.Any(); } }
        public bool CanCreateOptiCasePallet
        { get { return Boxes.Any() && Pallets.Any(); } }
        public bool CanCreateOptiPack
        { get { return Boxes.Any() && Pallets.Any(); } }
        public bool CanCreateOptiMulticase
        { get { return (Bundles.Any()) || this.Boxes.Any() || (Cases.Any()); } }
        #endregion

        #region Load methods
        public void Load(string filePath)
        {
            try
            {
                // instantiate XmlDocument
                XmlDocument xmlDoc = new XmlDocument();
                // load xml file in document and parse document
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    xmlDoc.Load(fileStream);
                    XmlElement xmlRootElement = xmlDoc.DocumentElement;
                    LoadDocumentElement(xmlRootElement);
                }
            }
            catch (FileNotFoundException ex)
            {
                _log.Error("Caught FileNotFoundException in Document.Load() -> rethrowing...");
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        void LoadDocumentElement(XmlElement docElement)
        {
            if (docElement.HasAttribute("Name"))
                Name = docElement.Attributes["Name"].Value;
            if (docElement.HasAttribute("Description"))
                Description = docElement.Attributes["Description"].Value;
            if (docElement.HasAttribute("Description"))
                Author = docElement.Attributes["Author"].Value;
            if (docElement.HasAttribute("DateCreated"))
            {
                try
                {
                    DateOfCreation = Convert.ToDateTime(docElement.Attributes["DateCreated"].Value, new CultureInfo("en-US"));
                }
                catch (Exception /*ex*/)
                {
                    DateOfCreation = DateTime.Now;
                    _log.Debug("Failed to load date of creation correctly: Loading file generated with former version?");
                }
            }
            if (docElement.HasAttribute("UnitSystem"))
                _unitSystem = (UnitsManager.UnitSystem)int.Parse(docElement.Attributes["UnitSystem"].Value);
            else
                _unitSystem = UnitsManager.UnitSystem.UNIT_METRIC1;

            foreach (XmlNode docChildNode in docElement.ChildNodes)
            {
                // load item properties
                if (string.Equals(docChildNode.Name, "ItemProperties", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (XmlNode itemPropertiesNode in docChildNode.ChildNodes)
                    {
                        try
                        {
                            if (string.Equals(itemPropertiesNode.Name, "BoxProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadBoxProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "CylinderProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadCylinderProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "InterlayerProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadInterlayerProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletCornerProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletCornerProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletCapProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletCapProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PalletFilmProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPalletFilmProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "BundleProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadBundleProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "TruckProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadTruckProperties(itemPropertiesNode as XmlElement);
                            else if (string.Equals(itemPropertiesNode.Name, "PackProperties", StringComparison.CurrentCultureIgnoreCase))
                                LoadPackProperties(itemPropertiesNode as XmlElement);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.ToString());
                        }
                    }
                }

                // load analyses
                if (string.Equals(docChildNode.Name, "Analyses", StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (XmlNode analysisNode in docChildNode.ChildNodes)
                    {
                        try
                        {
                            LoadAnalysis(analysisNode as XmlElement);
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.ToString());
                        }
                    }
                }
            }
        }

        #region Load containers / basics element
        private void LoadBoxProperties(XmlElement eltBoxProperties)
        {
            string sid = eltBoxProperties.Attributes["Id"].Value;
            string sname = eltBoxProperties.Attributes["Name"].Value;
            string sdescription = eltBoxProperties.Attributes["Description"].Value;
            string slength = eltBoxProperties.Attributes["Length"].Value;
            string swidth = eltBoxProperties.Attributes["Width"].Value;
            string sheight = eltBoxProperties.Attributes["Height"].Value;
            string sInsideLength = string.Empty, sInsideWidth = string.Empty, sInsideHeight = string.Empty;
            if (eltBoxProperties.HasAttribute("InsideLength"))
            {
                sInsideLength = eltBoxProperties.Attributes["InsideLength"].Value;
                sInsideWidth = eltBoxProperties.Attributes["InsideWidth"].Value;
                sInsideHeight = eltBoxProperties.Attributes["InsideHeight"].Value;
            }
            string sweight = eltBoxProperties.Attributes["Weight"].Value;
            OptDouble optNetWeight = LoadOptDouble(eltBoxProperties, "NetWeight", UnitsManager.UnitType.UT_MASS);

            bool hasInsideDimensions = eltBoxProperties.HasAttribute("InsideLength");
            if (hasInsideDimensions)
            { }

            Color[] colors = new Color[6];
            List<Pair<HalfAxis.HAxis, Texture>> listTexture = new List<Pair<HalfAxis.HAxis,Texture>>();
            bool hasTape = false;
            double tapeWidth = 0.0;
            Color tapeColor = Color.Black;
            foreach (XmlNode node in eltBoxProperties.ChildNodes)
            {
                if (string.Equals(node.Name, "FaceColors", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement faceColorList = node as XmlElement;
                    LoadFaceColors(faceColorList, ref colors);
                }
                else if (string.Equals(node.Name, "Textures", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement textureElt = node as XmlElement;
                    LoadTextureList(textureElt, ref listTexture);
                }
                else if (string.Equals(node.Name, "Tape", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement tapeElt = node as XmlElement;
                    hasTape = LoadTape(tapeElt, out tapeWidth,  out tapeColor);
                }
            }
            // create new BoxProperties instance
            BoxProperties boxProperties = null;
            if (!string.IsNullOrEmpty(sInsideLength)) // case
                boxProperties = CreateNewCase(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sInsideLength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sInsideWidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sInsideHeight, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), _unitSystem)
                , colors);
            else
                boxProperties = CreateNewBox(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), _unitSystem)
                , colors);
            boxProperties.ID.IGuid = new Guid(sid);
            boxProperties.TextureList = listTexture;
            // tape
            boxProperties.TapeColor = tapeColor;
            boxProperties.TapeWidth = new OptDouble(hasTape, UnitsManager.ConvertLengthFrom(tapeWidth, _unitSystem));
            boxProperties.SetNetWeight( optNetWeight );
        }

        private void LoadPackProperties(XmlElement eltPackProperties)
        {
            string sid = eltPackProperties.Attributes["Id"].Value;
            string sname = eltPackProperties.Attributes["Name"].Value;
            string sdescription = eltPackProperties.Attributes["Description"].Value;
            string sBoxId = eltPackProperties.Attributes["BoxProperties"].Value;
            string sOrientation = eltPackProperties.Attributes["Orientation"].Value;
            string sArrangement = eltPackProperties.Attributes["Arrangement"].Value;
            PackWrapper wrapper = null;
            foreach (XmlElement wrapperNode in eltPackProperties.ChildNodes)
                 wrapper = LoadWrapper(wrapperNode as XmlElement);
            PackProperties packProperties = CreateNewPack(
                sname
                , sdescription
                , GetTypeByGuid(new Guid(sBoxId)) as BoxProperties
                , PackArrangement.TryParse(sArrangement)
                , HalfAxis.Parse(sOrientation)
                , wrapper);
            packProperties.ID.IGuid = new Guid(sid);
            if (eltPackProperties.HasAttribute("OuterDimensions"))
            {
                Vector3D outerDimensions = Vector3D.Parse(eltPackProperties.Attributes["OuterDimensions"].Value);
                packProperties.ForceOuterDimensions(outerDimensions);
            }
        }

        private PackWrapper LoadWrapper(XmlElement xmlWrapperElt)
        {
            if (null == xmlWrapperElt) return null;

            string sType = xmlWrapperElt.Attributes["Type"].Value;
            string sColor = xmlWrapperElt.Attributes["Color"].Value;
            string sWeight = xmlWrapperElt.Attributes["Weight"].Value;
            string sUnitThickness = xmlWrapperElt.Attributes["UnitThickness"].Value;

            double thickness = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sUnitThickness, CultureInfo.InvariantCulture), _unitSystem);
            Color wrapperColor = Color.FromArgb(Convert.ToInt32(sColor));
            double weight = UnitsManager.ConvertMassFrom(Convert.ToDouble(sWeight, CultureInfo.InvariantCulture), _unitSystem);

            if (sType == "WT_POLYETHILENE")
            {
                bool transparent = bool.Parse(xmlWrapperElt.Attributes["Transparent"].Value);
                return new WrapperPolyethilene(thickness, weight, wrapperColor, transparent);
            }
            else if (sType == "WT_PAPER")
            {
                return new WrapperPaper(thickness, weight, wrapperColor);
            }
            else if (sType == "WT_CARDBOARD")
            {
                string sWalls = "1 1 1";
                if (xmlWrapperElt.HasAttribute("NumberOfWalls"))
                    sWalls = xmlWrapperElt.Attributes["NumberOfWalls"].Value;
                int[] walls = sWalls.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
                WrapperCardboard wrapper = new WrapperCardboard(thickness, weight, wrapperColor);
                wrapper.SetNoWalls(walls[0], walls[1], walls[2]);
                return wrapper;
            }
            else if (sType == "WT_TRAY")
            {
                string sWalls = "1 1 1";
                if (xmlWrapperElt.HasAttribute("NumberOfWalls"))
                    sWalls = xmlWrapperElt.Attributes["NumberOfWalls"].Value;
                int[] walls = sWalls.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                string sHeight = xmlWrapperElt.Attributes["Height"].Value;
                double height = UnitsManager.ConvertLengthFrom(Convert.ToDouble(sHeight, CultureInfo.InvariantCulture), _unitSystem);
                WrapperTray wrapper = new WrapperTray(thickness, weight, wrapperColor);
                wrapper.SetNoWalls(walls[0], walls[1], walls[2]);
                wrapper.Height = height;
                return wrapper;
            }
            else
                return null;
        }


        private void LoadCylinderProperties(XmlElement eltCylinderProperties)
        {
            string sid = eltCylinderProperties.Attributes["Id"].Value;
            string sname = eltCylinderProperties.Attributes["Name"].Value;
            string sdescription = eltCylinderProperties.Attributes["Description"].Value;
            string sRadiusOuter = string.Empty, sRadiusInner = string.Empty;
            if (eltCylinderProperties.HasAttribute("RadiusOuter"))
            {
                sRadiusOuter = eltCylinderProperties.Attributes["RadiusOuter"].Value;
                sRadiusInner = eltCylinderProperties.Attributes["RadiusInner"].Value;
            }
            else
            {
                sRadiusOuter = eltCylinderProperties.Attributes["Radius"].Value;
                sRadiusInner = "0.0";
            }
            string sheight = eltCylinderProperties.Attributes["Height"].Value;
            string sweight = eltCylinderProperties.Attributes["Weight"].Value;
            string sColorTop = eltCylinderProperties.Attributes["ColorTop"].Value;
            string sColorWallOuter = string.Empty, sColorWallInner = string.Empty;
            if (eltCylinderProperties.HasAttribute("ColorWall"))
            {
                sColorWallOuter = eltCylinderProperties.Attributes["ColorWall"].Value;
                sColorWallInner = eltCylinderProperties.Attributes["ColorWall"].Value;
            }
            else
            { 
                sColorWallOuter = eltCylinderProperties.Attributes["ColorWallOuter"].Value;
                sColorWallInner = eltCylinderProperties.Attributes["ColorWallInner"].Value;
            }

            CylinderProperties cylinderProperties = CreateNewCylinder(
                sname,
                sdescription,
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sRadiusOuter, CultureInfo.InvariantCulture), _unitSystem),
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sRadiusInner, CultureInfo.InvariantCulture), _unitSystem),
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), _unitSystem),
                UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), _unitSystem),
                Color.FromArgb(Convert.ToInt32(sColorTop)),
                Color.FromArgb(Convert.ToInt32(sColorWallOuter)),
                Color.FromArgb(Convert.ToInt32(sColorWallInner))
                );
            cylinderProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadFaceColors(XmlElement eltColors, ref Color[] colors)
        {
            foreach (XmlNode faceColorNode in eltColors.ChildNodes)
            {
                XmlElement faceColorElt = faceColorNode as XmlElement;
                string sFaceIndex = faceColorElt.Attributes["FaceIndex"].Value;
                string sColorArgb = faceColorElt.Attributes["Color"].Value;
                int iFaceIndex = Convert.ToInt32(sFaceIndex);
                Color faceColor = Color.FromArgb(Convert.ToInt32(sColorArgb));
                colors[iFaceIndex] = faceColor;
            }
        }
        private void LoadTextureList(XmlElement eltTextureList, ref List<Pair<HalfAxis.HAxis, Texture>> listTexture)
        {
            foreach (XmlNode faceTextureNode in eltTextureList.ChildNodes)
            {
                try
                {
                    XmlElement xmlFaceTexture = faceTextureNode as XmlElement;
                    // face normal
                    HalfAxis.HAxis faceNormal = HalfAxis.Parse(xmlFaceTexture.Attributes["FaceNormal"].Value);
                    // position
                    Vector2D position = Vector2D.Parse(xmlFaceTexture.Attributes["Position"].Value);
                    // size
                    Vector2D size = Vector2D.Parse(xmlFaceTexture.Attributes["Size"].Value);
                    // angle
                    double angle = Convert.ToDouble(xmlFaceTexture.Attributes["Angle"].Value, CultureInfo.InvariantCulture);
                    // bitmap
                    Bitmap bmp = Document.StringToBitmap(xmlFaceTexture.Attributes["Bitmap"].Value);
                    // add texture pair
                    listTexture.Add(new Pair<HalfAxis.HAxis, Texture>(faceNormal
                        , new Texture(
                            bmp
                            , UnitsManager.ConvertLengthFrom(position, _unitSystem)
                            , UnitsManager.ConvertLengthFrom(size, _unitSystem)
                            , angle)));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
        }
        private bool LoadTape(XmlElement eltTape, out double tapeWidth, out Color tapeColor)
        {
            tapeWidth = 0.0;
            tapeColor = Color.Black;
            try
            {
                tapeWidth = Convert.ToDouble(eltTape.Attributes["TapeWidth"].Value, CultureInfo.InvariantCulture);
                string sColorArgb = eltTape.Attributes["TapeColor"].Value;
                tapeColor = Color.FromArgb(Convert.ToInt32(sColorArgb));
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return false;
            }
            return true;
        }
        private void LoadCaseDefinition(XmlElement eltCaseDefinition, out CaseDefinition caseDefinition)
        {
            string sArrangement = eltCaseDefinition.Attributes["Arrangement"].Value;
            string sDim = eltCaseDefinition.Attributes["Orientation"].Value;
            int[] iOrientation = Document.ParseInt2(sDim);
            caseDefinition = new CaseDefinition(
                PackArrangement.TryParse(sArrangement)
                , iOrientation[0]
                , iOrientation[1]);
        }
        private void LoadPalletProperties(XmlElement eltPalletProperties)
        {
            string sid = eltPalletProperties.Attributes["Id"].Value;
            string sname = eltPalletProperties.Attributes["Name"].Value;
            string sdescription = eltPalletProperties.Attributes["Description"].Value;
            string slength = eltPalletProperties.Attributes["Length"].Value;
            string swidth = eltPalletProperties.Attributes["Width"].Value;
            string sheight = eltPalletProperties.Attributes["Height"].Value;
            string sweight = eltPalletProperties.Attributes["Weight"].Value;
            string stype = eltPalletProperties.Attributes["Type"].Value;
            string sColor = eltPalletProperties.Attributes["Color"].Value;

            if ("0" == stype)
                stype = "Block";
            else if ("1" == stype)
                stype = "UK Standard";

            // create new PalletProperties instance
            PalletProperties palletProperties = CreateNewPallet(
                sname
                , sdescription
                , stype
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), _unitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor)));
            palletProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadInterlayerProperties(XmlElement eltInterlayerProperties)
        {
            string sid = eltInterlayerProperties.Attributes["Id"].Value;
            string sname = eltInterlayerProperties.Attributes["Name"].Value;
            string sdescription = eltInterlayerProperties.Attributes["Description"].Value;
            string slength = eltInterlayerProperties.Attributes["Length"].Value;
            string swidth = eltInterlayerProperties.Attributes["Width"].Value;
            string sthickness = eltInterlayerProperties.Attributes["Thickness"].Value;
            string sweight = eltInterlayerProperties.Attributes["Weight"].Value;
            string sColor = eltInterlayerProperties.Attributes["Color"].Value;

            // create new InterlayerProperties instance
            InterlayerProperties interlayerProperties = CreateNewInterlayer(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sthickness, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), _unitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor)));
            interlayerProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadPalletCornerProperties(XmlElement eltPalletCornerProperties)
        {
            string sid = eltPalletCornerProperties.Attributes["Id"].Value;
            string sname = eltPalletCornerProperties.Attributes["Name"].Value;
            string sdescription = eltPalletCornerProperties.Attributes["Description"].Value;
            string slength = eltPalletCornerProperties.Attributes["Length"].Value;
            string swidth = eltPalletCornerProperties.Attributes["Width"].Value;
            string sthickness = eltPalletCornerProperties.Attributes["Thickness"].Value;
            string sweight = eltPalletCornerProperties.Attributes["Weight"].Value;
            string sColor = eltPalletCornerProperties.Attributes["Color"].Value;

            // create new PalletCornerProperties instance
            PalletCornerProperties palletCornerProperties = CreateNewPalletCorners(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sthickness, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), _unitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor))
                );
            palletCornerProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadPalletCapProperties(XmlElement eltPalletCapProperties)
        {
            string sid = eltPalletCapProperties.Attributes["Id"].Value;
            string sname = eltPalletCapProperties.Attributes["Name"].Value;
            string sdescription = eltPalletCapProperties.Attributes["Description"].Value;
            string slength = eltPalletCapProperties.Attributes["Length"].Value;
            string swidth = eltPalletCapProperties.Attributes["Width"].Value;
            string sheight = eltPalletCapProperties.Attributes["Height"].Value;
            string sinnerlength = eltPalletCapProperties.Attributes["InsideLength"].Value;
            string sinnerwidth = eltPalletCapProperties.Attributes["InsideWidth"].Value;
            string sinnerheight = eltPalletCapProperties.Attributes["InsideHeight"].Value;
            string sweight = eltPalletCapProperties.Attributes["Weight"].Value;
            string sColor = eltPalletCapProperties.Attributes["Color"].Value;

            PalletCapProperties palletCapProperties = CreateNewPalletCap(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(slength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(swidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sheight, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sinnerlength, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sinnerwidth, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertLengthFrom(Convert.ToDouble(sinnerheight, CultureInfo.InvariantCulture), _unitSystem)
                , UnitsManager.ConvertMassFrom(Convert.ToDouble(sweight, CultureInfo.InvariantCulture), _unitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor))
                );
            palletCapProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadPalletFilmProperties(XmlElement eltPalletFilmProperties)
        {
            string sid = eltPalletFilmProperties.Attributes["Id"].Value;
            string sname = eltPalletFilmProperties.Attributes["Name"].Value;
            string sdescription = eltPalletFilmProperties.Attributes["Description"].Value;
            bool useTransparency = bool.Parse(eltPalletFilmProperties.Attributes["Transparency"].Value);
            bool useHatching = bool.Parse(eltPalletFilmProperties.Attributes["Hatching"].Value);
            string sHatchSpacing = eltPalletFilmProperties.Attributes["HatchSpacing"].Value;
            string sHatchAngle = eltPalletFilmProperties.Attributes["HatchAngle"].Value;
            string sColor = eltPalletFilmProperties.Attributes["Color"].Value;

            PalletFilmProperties palletFilmProperties = CreateNewPalletFilm(
                sname,
                sdescription,
                useTransparency,
                useHatching,
                UnitsManager.ConvertLengthFrom(Convert.ToDouble(sHatchSpacing, CultureInfo.InvariantCulture), _unitSystem),
                Convert.ToDouble(sHatchAngle, CultureInfo.InvariantCulture),
                Color.FromArgb(Convert.ToInt32(sColor))
                );
            palletFilmProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadBundleProperties(XmlElement eltBundleProperties)
        {
            string sid = eltBundleProperties.Attributes["Id"].Value;
            string sname = eltBundleProperties.Attributes["Name"].Value;
            string sdescription = eltBundleProperties.Attributes["Description"].Value;
            double length = double.Parse(eltBundleProperties.Attributes["Length"].Value, CultureInfo.InvariantCulture);
            double width = double.Parse(eltBundleProperties.Attributes["Width"].Value, CultureInfo.InvariantCulture);
            double unitThickness = double.Parse(eltBundleProperties.Attributes["UnitThickness"].Value, CultureInfo.InvariantCulture);
            double unitWeight = double.Parse(eltBundleProperties.Attributes["UnitWeight"].Value, CultureInfo.InvariantCulture);
            Color color = Color.FromArgb(Int32.Parse(eltBundleProperties.Attributes["Color"].Value));
            int noFlats = int.Parse(eltBundleProperties.Attributes["NumberFlats"].Value);
            BundleProperties bundleProperties = CreateNewBundle(
                sname
                , sdescription
                , UnitsManager.ConvertLengthFrom(length, _unitSystem)
                , UnitsManager.ConvertLengthFrom(width, _unitSystem)
                , UnitsManager.ConvertLengthFrom(unitThickness, _unitSystem)
                , UnitsManager.ConvertMassFrom(unitWeight, _unitSystem)
                , color
                , noFlats);
            bundleProperties.ID.IGuid = new Guid(sid);
        }
        private void LoadTruckProperties(XmlElement eltTruckProperties)
        {
            string sid = eltTruckProperties.Attributes["Id"].Value;
            string sName = eltTruckProperties.Attributes["Name"].Value;
            string sDescription = eltTruckProperties.Attributes["Description"].Value;
            double length = double.Parse(eltTruckProperties.Attributes["Length"].Value, CultureInfo.InvariantCulture);
            double width = double.Parse(eltTruckProperties.Attributes["Width"].Value, CultureInfo.InvariantCulture);
            double height = double.Parse(eltTruckProperties.Attributes["Height"].Value, CultureInfo.InvariantCulture);
            double admissibleLoadWeight = double.Parse(eltTruckProperties.Attributes["AdmissibleLoadWeight"].Value, CultureInfo.InvariantCulture);
            string sColor = eltTruckProperties.Attributes["Color"].Value;

            // create new truck
            TruckProperties truckProperties = CreateNewTruck(
                sName
                , sDescription
                , UnitsManager.ConvertLengthFrom(length, _unitSystem)
                , UnitsManager.ConvertLengthFrom(width, _unitSystem)
                , UnitsManager.ConvertLengthFrom(height, _unitSystem)
                , UnitsManager.ConvertMassFrom(admissibleLoadWeight, _unitSystem)
                , Color.FromArgb(Convert.ToInt32(sColor)));
            truckProperties.ID.IGuid = new Guid(sid);
        }
        #endregion

        #region Load case optimisation
        private void LoadOptimConstraintSet(XmlElement eltConstraintSet, out ParamSetPackOptim constraintSet)
        {
            string sNoWalls = eltConstraintSet.Attributes["NumberOfWalls"].Value;
            int[] iNoWalls = ParseInt3(sNoWalls);
            double wallThickness = UnitsManager.ConvertLengthFrom(
                Convert.ToDouble(eltConstraintSet.Attributes["WallThickness"].Value, CultureInfo.InvariantCulture)
                , _unitSystem);
            double wallSurfaceMass = UnitsManager.ConvertSurfaceMassFrom(
                Convert.ToDouble(eltConstraintSet.Attributes["WallSurfaceMass"].Value, CultureInfo.InvariantCulture)
                , _unitSystem);
            constraintSet = new ParamSetPackOptim(0, Vector3D.Zero, Vector3D.Zero, false, PackWrapper.WType.WT_CARDBOARD, iNoWalls, wallThickness, wallSurfaceMass, 0.0); 
        }
        #endregion

        #region Load analysis
        private void LoadAnalysis(XmlElement eltAnalysis)
        {
            string sId = string.Empty;
            if (eltAnalysis.HasAttribute("Id"))
                sId = eltAnalysis.Attributes["Id"].Value;
            string sName = eltAnalysis.Attributes["Name"].Value;
            string sDescription = eltAnalysis.Attributes["Description"].Value;
            string sInterlayerId = string.Empty;
            if (eltAnalysis.HasAttribute("InterlayerId"))
                sInterlayerId = eltAnalysis.Attributes["InterlayerId"].Value;
            string sInterlayerAntiSlipId = string.Empty;
            if (eltAnalysis.HasAttribute("InterlayerAntiSlipId"))
                sInterlayerAntiSlipId = eltAnalysis.Attributes["InterlayerAntiSlipId"].Value;
            string sPalletCornerId = string.Empty;
            if (eltAnalysis.HasAttribute("PalletCornerId"))
                sPalletCornerId = eltAnalysis.Attributes["PalletCornerId"].Value;
            string sPalletCapId = string.Empty;
            if (eltAnalysis.HasAttribute("PalletCapId"))
                sPalletCapId = eltAnalysis.Attributes["PalletCapId"].Value;
            string sPalletFilmId = string.Empty;
            if (eltAnalysis.HasAttribute("PalletFilmId"))
                sPalletFilmId = eltAnalysis.Attributes["PalletFilmId"].Value;
            // content
            string sContentId = string.Empty;
            if (eltAnalysis.HasAttribute("ContentId"))
                sContentId = eltAnalysis.Attributes["ContentId"].Value;
            // container
            string sContainerId = string.Empty;
            if (eltAnalysis.HasAttribute("ContainerId"))
                sContainerId = eltAnalysis.Attributes["ContainerId"].Value;
            // interlayers
            List<InterlayerProperties> interlayers = new List<InterlayerProperties>();
            List<LayerDesc> listLayerDesc = null;
            List<int> listInterlayers = new List<int>();
            List<SolutionItem> listSolItems = null;

            if (string.Equals(eltAnalysis.Name, "AnalysisCasePallet", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                PalletProperties palletProperties = GetTypeByGuid(sContainerId) as PalletProperties;
                PalletCornerProperties palletCorners = GetTypeByGuid(sPalletCornerId) as PalletCornerProperties;
                PalletCapProperties palletCap = GetTypeByGuid(sPalletCapId) as PalletCapProperties;
                PalletFilmProperties palletFilm = GetTypeByGuid(sPalletFilmId) as PalletFilmProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCasePallet( node as XmlElement );
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerDesc, out listSolItems);                        
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }

                Analysis analysis = CreateNewAnalysisCasePallet(
                    sName, sDescription
                    , packable, palletProperties
                    , interlayers
                    , palletCorners, palletCap, palletFilm
                    , constraintSet as ConstraintSetCasePallet, listLayerDesc);
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.Solution.SolutionItems = listSolItems;
                
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisBoxCase", StringComparison.CurrentCultureIgnoreCase))
            {

                Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                BoxProperties caseProperties = GetTypeByGuid(sContainerId) as BoxProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetBoxCase( node as XmlElement, caseProperties );
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerDesc, out listSolItems);                        
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }

                Analysis analysis = CreateNewAnalysisBoxCase(
                    sName, sDescription
                    , packable, caseProperties
                    , interlayers
                    , constraintSet as ConstraintSetBoxCase, listLayerDesc);
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.Solution.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisCylinderPallet", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable cylinderProperties = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                PalletProperties palletProperties = GetTypeByGuid(sContainerId) as PalletProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCasePallet( node as XmlElement );
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerDesc, out listSolItems);                        
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }

                Analysis analysis = CreateNewAnalysisCylinderPallet(
                    sName, sDescription
                    , cylinderProperties as CylinderProperties, palletProperties
                    , interlayers
                    , constraintSet as ConstraintSetPackablePallet, listLayerDesc);
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.Solution.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisCylinderCase", StringComparison.CurrentCultureIgnoreCase))
            { 
                Packable cylinderProperties = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                BoxProperties caseProperties = GetTypeByGuid(sContainerId) as BoxProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCasePallet(node as XmlElement);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerDesc, out listSolItems);
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }

                Analysis analysis = CreateNewAnalysisCylinderCase(
                    sName, sDescription
                    , cylinderProperties as CylinderProperties, caseProperties
                    , interlayers
                    , constraintSet as ConstraintSetCylinderCase
                    , listLayerDesc);
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.Solution.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisPalletTruck", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable loadedPallet = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                TruckProperties truckProperties = GetTypeByGuid(sContainerId) as TruckProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetPalletTruck(node as XmlElement, truckProperties);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerDesc, out listSolItems);
                }

                Analysis analysis = CreateNewAnalysisPalletTruck(sName, sDescription
                    , loadedPallet, truckProperties
                    , constraintSet as ConstraintSetPalletTruck
                    , listLayerDesc);
                if (!string.IsNullOrEmpty(sId))
                    analysis.ID.IGuid = Guid.Parse(sId);
                analysis.Solution.SolutionItems = listSolItems;
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisCaseTruck", StringComparison.CurrentCultureIgnoreCase))
            {
                Packable packable = GetContentByGuid(Guid.Parse(sContentId)) as Packable;
                TruckProperties truckProperties = GetTypeByGuid(sContainerId) as TruckProperties;

                ConstraintSetAbstract constraintSet = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadConstraintSetCaseTruck(node as XmlElement, truckProperties);
                    else if (string.Equals(node.Name, "Solution", StringComparison.CurrentCultureIgnoreCase))
                        LoadSolution(node as XmlElement, out listLayerDesc, out listSolItems);
                    else if (string.Equals(node.Name, "Interlayers", StringComparison.CurrentCultureIgnoreCase))
                        interlayers = LoadInterlayers(node as XmlElement);
                }
                Analysis analysis = CreateNewAnalysisCaseTruck(sName, sDescription
                    , packable, truckProperties, interlayers
                    , constraintSet as ConstraintSetCaseTruck, listLayerDesc);
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisPallet", StringComparison.CurrentCultureIgnoreCase))
            {
                string sBoxId = eltAnalysis.Attributes["BoxId"].Value;
                string sPalletId = eltAnalysis.Attributes["PalletId"].Value;

                // load constraint set / solution list
                PalletConstraintSet constraintSet = null;
                List<int> selectedIndices = new List<int>();

                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    // load constraint set
                    if (string.Equals(node.Name, "ConstraintSetBox", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadCasePalletConstraintSet_Box(node as XmlElement);
                    else if (string.Equals(node.Name, "ConstraintSetBundle", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadCasePalletConstraintSet_Bundle(node as XmlElement);
                    // load solutions
                    else if (string.Equals(node.Name, "Solutions", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int indexSol = 0;
                        foreach (XmlNode solutionNode in node.ChildNodes)
                        {
                            XmlElement eltSolution = solutionNode as XmlElement;
                            // is solution selected ?
                            if (null != eltSolution.Attributes["Selected"] && "true" == eltSolution.Attributes["Selected"].Value)
                                selectedIndices.Add(indexSol);
                            ++indexSol;
                        }
                    }
                    if (null == constraintSet)
                        throw new Exception("Failed to load a valid ConstraintSet");
                }

                // instantiate analysis
                Analysis analysis = CreateNewCasePalletAnalysis(
                    sName
                    , sDescription
                    , GetTypeByGuid(new Guid(sBoxId)) as BProperties
                    , GetTypeByGuid(new Guid(sPalletId)) as PalletProperties
                    , string.IsNullOrEmpty(sInterlayerId) ? null : GetTypeByGuid(new Guid(sInterlayerId)) as InterlayerProperties
                    , string.IsNullOrEmpty(sInterlayerAntiSlipId) ? null : GetTypeByGuid(new Guid(sInterlayerAntiSlipId)) as InterlayerProperties
                    , string.IsNullOrEmpty(sPalletCornerId) ? null : GetTypeByGuid(new Guid(sPalletCornerId)) as PalletCornerProperties
                    , string.IsNullOrEmpty(sPalletCapId) ? null : GetTypeByGuid(new Guid(sPalletCapId)) as PalletCapProperties
                    , string.IsNullOrEmpty(sPalletFilmId) ? null : GetTypeByGuid(new Guid(sPalletFilmId)) as PalletFilmProperties
                    , constraintSet
                    , _solver);

            }
            else if (string.Equals(eltAnalysis.Name, "PackPalletAnalysis", StringComparison.CurrentCultureIgnoreCase))
            {
                string sPackId = eltAnalysis.Attributes["PackId"].Value;
                string sPalletId = eltAnalysis.Attributes["PalletId"].Value;

                // load constraint set / solution list
                PackPalletConstraintSet constraintSet = null;
                List<PackPalletSolution> solutions = new List<PackPalletSolution>();
                List<int> selectedIndices = new List<int>();

                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    // load constraint set
                    if (string.Equals(node.Name, "ConstraintSet", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadPackPalletConstraintSet(node as XmlElement);
                    // load solutions
                    else if (string.Equals(node.Name, "Solutions", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int indexSol = 0;
                        foreach (XmlNode solutionNode in node.ChildNodes)
                        {
                            XmlElement eltSolution = solutionNode as XmlElement;
                            solutions.Add(LoadPackPalletSolution(eltSolution));
                            // is solution selected ?
                            if (null != eltSolution.Attributes["Selected"] && "true" == eltSolution.Attributes["Selected"].Value)
                                selectedIndices.Add(indexSol);
                            ++indexSol;
                        }
                    }
                }
                Analysis analysis = CreateNewPackPalletAnalysis(
                    sName
                    , sDescription
                    , GetTypeByGuid(new Guid(sPackId)) as PackProperties
                    , GetTypeByGuid(new Guid(sPalletId)) as PalletProperties
                    , string.IsNullOrEmpty(sInterlayerId) ? null : GetTypeByGuid(new Guid(sInterlayerId)) as InterlayerProperties
                    , constraintSet
                    , _solver
                    );
            }
            else if (string.Equals(eltAnalysis.Name, "AnalysisBoxCase", StringComparison.CurrentCultureIgnoreCase))
            {
                // load caseId
                string sBoxId = eltAnalysis.Attributes["BoxId"].Value;
                string sCaseId = eltAnalysis.Attributes["CaseId"].Value;

                // load constraint set / solution list
                BCaseConstraintSet constraintSet = null;
                List<BoxCaseSolution> solutions = new List<BoxCaseSolution>();
                List<int> selectedIndices = new List<int>();

                // first load BoxCaseConstraintSet / BoxCaseSolution(s)
                XmlElement boxCaseSolutionsElt = null;
                foreach (XmlNode node in eltAnalysis.ChildNodes)
                {
                    // load constraint set
                    if (string.Equals(node.Name, "ConstraintSetCase", StringComparison.CurrentCultureIgnoreCase))
                        constraintSet = LoadBoxCaseConstraintSet(node as XmlElement);
                    // load solutions
                    else if (string.Equals(node.Name, "Solutions", StringComparison.CurrentCultureIgnoreCase))
                    {
                        boxCaseSolutionsElt = node as XmlElement;

                        int indexSol = 0;
                        foreach (XmlNode solutionNode in boxCaseSolutionsElt.ChildNodes)
                        {
                            XmlElement eltSolution = solutionNode as XmlElement;
                            solutions.Add(LoadBoxCaseSolution(eltSolution));
                            // is solution selected ?
                            if (null != eltSolution.Attributes["Selected"] && "true" == eltSolution.Attributes["Selected"].Value)
                                selectedIndices.Add(indexSol);
                            ++indexSol;
                        }
                    }
                }
            }
        } 

        #region ConstraintSet loading
        private ConstraintSetAbstract LoadConstraintSetCasePallet(XmlElement eltConstraintSet)
        {
            ConstraintSetCasePallet constraintSet = new ConstraintSetCasePallet
            {
                OptMaxWeight = LoadOptDouble(eltConstraintSet, "MaximumWeight", UnitsManager.UnitType.UT_MASS),
                OptMaxNumber = LoadOptInt(eltConstraintSet, "MaximumNumberOfItems"),

                Overhang = LoadVectorLength(eltConstraintSet, "Overhang"),
                MinimumSpace = LoadOptDouble(eltConstraintSet, "MinSpace", UnitsManager.UnitType.UT_LENGTH)
            };
            constraintSet.SetMaxHeight(LoadOptDouble(eltConstraintSet, "MaximumPalletHeight", UnitsManager.UnitType.UT_LENGTH));
            constraintSet.PalletFilmTurns = LoadInt(eltConstraintSet, "PalletFilmTurns");
            return constraintSet;
        }
        private ConstraintSetAbstract LoadConstraintSetBoxCase(XmlElement eltConstraintSet, Packable container)
        {
            ConstraintSetBoxCase constraintSet = new ConstraintSetBoxCase(container);
            return constraintSet;
        }
        private ConstraintSetAbstract LoadConstraintSetCylinderCase(XmlElement eltConstraintSet, Packable container)
        {
            ConstraintSetCylinderCase constraintSet = new ConstraintSetCylinderCase(container);
            return constraintSet;
        }
        private ConstraintSetAbstract LoadConstraintSetPalletTruck(XmlElement eltConstraintSet, IPackContainer container)
        {
            ConstraintSetPalletTruck constraintSet = new ConstraintSetPalletTruck(container)
            {
                MinDistanceLoadWall = LoadVectorLength(eltConstraintSet, "MinDistanceLoadWall"),
                MinDistanceLoadRoof = LoadDouble(eltConstraintSet, "MinDistanceLoadRoof", UnitsManager.UnitType.UT_LENGTH),
                AllowMultipleLayers = (1 == LoadInt(eltConstraintSet, "AllowMultipleLayers"))
            };
            return constraintSet;
        }
        private ConstraintSetAbstract LoadConstraintSetCaseTruck(XmlElement eltConstraintSet, IPackContainer container)
        {
            ConstraintSetCaseTruck constraintSet = new ConstraintSetCaseTruck(container)
            {
                MinDistanceLoadWall = LoadVectorLength(eltConstraintSet, "MinDistanceLoadWall"),
                MinDistanceLoadRoof = LoadDouble(eltConstraintSet, "MinDistanceLoadRoof", UnitsManager.UnitType.UT_LENGTH)
            };
            return constraintSet;
        }
        #endregion

        #region Load helpers
        private List<InterlayerProperties> LoadInterlayers(XmlElement eltInterlayers)
        {
            List<InterlayerProperties> interlayers = new List<InterlayerProperties>();
            foreach (XmlNode nodeInterlayer in eltInterlayers.ChildNodes)
            {
                XmlElement eltInterlayer = nodeInterlayer as XmlElement;
                interlayers.Add(GetTypeByGuid(eltInterlayer.InnerText) as InterlayerProperties);
            }
            return interlayers;
        }
        private OptInt LoadOptInt(XmlElement xmlElement, string attribute)
        {
            if (!xmlElement.HasAttribute(attribute))
                return new OptInt(false, 0);
            return OptInt.Parse(xmlElement.Attributes[attribute].Value);
        }
        private double LoadDouble(XmlElement xmlElement, string attribute, UnitsManager.UnitType unitType)
        {
            if (xmlElement.HasAttribute(attribute))
            {
                switch (unitType)
                {
                case UnitsManager.UnitType.UT_LENGTH:
                    return UnitsManager.ConvertLengthFrom(double.Parse(xmlElement.Attributes[attribute].Value), _unitSystem);
                case UnitsManager.UnitType.UT_MASS:
                    return UnitsManager.ConvertMassFrom(double.Parse(xmlElement.Attributes[attribute].Value), _unitSystem);
                default:
                    Debug.Assert(false);
                    break;
                }
            }
            _log.Warn(string.Format("Double type attribute {0} was not found!", attribute));
            return 0.0;
        }
        private Vector2D LoadVectorLength(XmlElement xmlElement, string attribute)
        {
            if (xmlElement.HasAttribute(attribute))
            {
                Vector2D v0 = Vector2D.Parse(xmlElement.Attributes[attribute].Value);
                return new Vector2D(UnitsManager.ConvertLengthFrom(v0.X, _unitSystem), UnitsManager.ConvertLengthFrom(v0.Y, _unitSystem));
            }
            return Vector2D.Zero;
        }
        private int LoadInt(XmlElement xmlElement, string attribute)
        {
            if (xmlElement.HasAttribute(attribute))
                return int.Parse( xmlElement.Attributes[attribute].Value );
            return 0;
        }
        private OptDouble LoadOptDouble(XmlElement xmlElement, string attribute, UnitsManager.UnitType unitType)
        {
            if (!xmlElement.HasAttribute(attribute))
                return new OptDouble(false, 0.0);
            else
            {
                OptDouble optD = OptDouble.Parse(xmlElement.Attributes[attribute].Value);
                switch (unitType)
                {
                    case UnitsManager.UnitType.UT_LENGTH:
                        optD.Value = UnitsManager.ConvertLengthFrom(optD.Value, _unitSystem);
                        break;
                    case UnitsManager.UnitType.UT_MASS:
                        optD.Value = UnitsManager.ConvertMassFrom(optD.Value, _unitSystem);
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
                return optD;
            }
        }
        #endregion

        #region Legacy loading
        private BoxCasePalletConstraintSet LoadCaseConstraintSet(XmlElement eltConstraintSet)
        {
            BoxCasePalletConstraintSet constraints = new BoxCasePalletConstraintSet();
            // align layers allowed
            if (eltConstraintSet.HasAttribute("AlignedLayersAllowed"))
                constraints.AllowAlignedLayers = string.Equals(eltConstraintSet.Attributes["AlignedLayersAllowed"].Value, "true", StringComparison.CurrentCultureIgnoreCase);
            // alternate layers allowed
            if (eltConstraintSet.HasAttribute("AlternateLayersAllowed"))
                constraints.AllowAlternateLayers = string.Equals(eltConstraintSet.Attributes["AlternateLayersAllowed"].Value, "true", StringComparison.CurrentCultureIgnoreCase);
            // allowed orthogonal axes
            if (eltConstraintSet.HasAttribute("AllowedBoxPositions"))
                constraints.AllowOrthoAxisString = eltConstraintSet.Attributes["AllowedBoxPositions"].Value;
            // allowed patterns
            if (eltConstraintSet.HasAttribute("AllowedPatterns"))
                constraints.AllowedPatternString = eltConstraintSet.Attributes["AllowedPatterns"].Value;
            // stop criterions
            if (constraints.UseMaximumNumberOfItems = eltConstraintSet.HasAttribute("ManimumNumberOfItems"))
                constraints.MaximumNumberOfItems = int.Parse(eltConstraintSet.Attributes["ManimumNumberOfItems"].Value);
            // maximum case weight
            if (constraints.UseMaximumCaseWeight = eltConstraintSet.HasAttribute("MaximumCaseWeight"))
                constraints.MaximumCaseWeight = UnitsManager.ConvertMassFrom(double.Parse(eltConstraintSet.Attributes["MaximumCaseWeight"].Value), _unitSystem);
            // number of solutions to keep
            if (constraints.UseNumberOfSolutionsKept = eltConstraintSet.HasAttribute("NumberOfSolutions"))
                constraints.NumberOfSolutionsKept = int.Parse(eltConstraintSet.Attributes["NumberOfSolutions"].Value);
            // minimum number of items
            if (constraints.UseMinimumNumberOfItems = eltConstraintSet.HasAttribute("MinimumNumberOfItems"))
                constraints.MinimumNumberOfItems = int.Parse(eltConstraintSet.Attributes["MinimumNumberOfItems"].Value);
            // sanity check
            if (!constraints.IsValid)
                throw new Exception("Invalid constraint set");
            return constraints;
        }

        private BCaseConstraintSet LoadBoxCaseConstraintSet(XmlElement eltConstraintSet)
        {
            BCaseConstraintSet constraints = null;
            
            // allowed orthogonal axes
            if (eltConstraintSet.HasAttribute("AllowedBoxPositions"))
            {
                constraints = new BoxCaseConstraintSet();
                BoxCaseConstraintSet boxCaseContraintSet = constraints as BoxCaseConstraintSet;
                boxCaseContraintSet.AllowOrthoAxisString = eltConstraintSet.Attributes["AllowedBoxPositions"].Value;
            }
            else
                constraints = new BundleCaseConstraintSet();
            // maximum case weight
            if (constraints.UseMaximumCaseWeight = eltConstraintSet.HasAttribute("MaximumCaseWeight"))
                constraints.MaximumCaseWeight = UnitsManager.ConvertMassFrom(double.Parse(eltConstraintSet.Attributes["MaximumCaseWeight"].Value), _unitSystem);
            // allowed patterns
            if (constraints.UseMaximumNumberOfBoxes = eltConstraintSet.HasAttribute("ManimumNumberOfItems"))
                constraints.MaximumNumberOfBoxes = int.Parse(eltConstraintSet.Attributes["ManimumNumberOfItems"].Value);
            // sanity check
            if (!constraints.IsValid)
                throw new Exception("Invalid constraint set");
            return constraints;
        }

        private PalletConstraintSet LoadCasePalletConstraintSet_Box(XmlElement eltConstraintSet)
        {
            CasePalletConstraintSet constraints = new CasePalletConstraintSet();
            // align layers allowed
            if (eltConstraintSet.HasAttribute("AlignedLayersAllowed"))
                constraints.AllowAlignedLayers = string.Equals(eltConstraintSet.Attributes["AlignedLayersAllowed"].Value, "true", StringComparison.CurrentCultureIgnoreCase);
            // alternate layers allowed
            if (eltConstraintSet.HasAttribute("AlternateLayersAllowed"))
                constraints.AllowAlternateLayers = string.Equals(eltConstraintSet.Attributes["AlternateLayersAllowed"].Value, "true", StringComparison.CurrentCultureIgnoreCase);
            // allowed orthogonal axes
            if (eltConstraintSet.HasAttribute("AllowedBoxPositions"))
            {
                string allowedOrthoAxes = eltConstraintSet.Attributes["AllowedBoxPositions"].Value;
                string[] sAxes = allowedOrthoAxes.Split(',');
                foreach (string sAxis in sAxes)
                    constraints.SetAllowedOrthoAxis(HalfAxis.Parse(sAxis), true);
            }
            // allowed patterns
            if (eltConstraintSet.HasAttribute("AllowedPatterns"))
                constraints.AllowedPatternString = eltConstraintSet.Attributes["AllowedPatterns"].Value;
            // stop criterions
            if (constraints.UseMaximumHeight = eltConstraintSet.HasAttribute("MaximumHeight"))
                constraints.MaximumHeight = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["MaximumHeight"].Value), _unitSystem);
            if (constraints.UseMaximumNumberOfCases = eltConstraintSet.HasAttribute("ManimumNumberOfItems"))
                constraints.MaximumNumberOfItems = int.Parse(eltConstraintSet.Attributes["ManimumNumberOfItems"].Value);
            if (constraints.UseMaximumPalletWeight = eltConstraintSet.HasAttribute("MaximumPalletWeight"))
                constraints.MaximumPalletWeight = UnitsManager.ConvertMassFrom(double.Parse(eltConstraintSet.Attributes["MaximumPalletWeight"].Value), _unitSystem);
            if (constraints.UseMaximumWeightOnBox = eltConstraintSet.HasAttribute("MaximumWeightOnBox"))
                constraints.MaximumWeightOnBox = UnitsManager.ConvertMassFrom(double.Parse(eltConstraintSet.Attributes["MaximumWeightOnBox"].Value), _unitSystem);
            // overhang / underhang
            if (eltConstraintSet.HasAttribute("OverhangX"))
                constraints.OverhangX = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["OverhangX"].Value), _unitSystem);
            if (eltConstraintSet.HasAttribute("OverhangY"))
                constraints.OverhangY = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["OverhangY"].Value), _unitSystem);
            // number of solutions to keep
            if (constraints.UseNumberOfSolutionsKept = eltConstraintSet.HasAttribute("NumberOfSolutions"))
                constraints.NumberOfSolutionsKept = int.Parse(eltConstraintSet.Attributes["NumberOfSolutions"].Value);
            // pallet film turns
            if (eltConstraintSet.HasAttribute("PalletFilmTurns"))
                constraints.PalletFilmTurns = int.Parse(eltConstraintSet.Attributes["PalletFilmTurns"].Value);
            // sanity check
            if (!constraints.IsValid)
                throw new Exception("Invalid constraint set");
            return constraints;
        }
        PalletConstraintSet LoadCasePalletConstraintSet_Bundle(XmlElement eltConstraintSet)
        {
            BundlePalletConstraintSet constraints = new BundlePalletConstraintSet();
            // aligned layers allowed
            if (eltConstraintSet.HasAttribute("AlignedLayersAllowed"))
                constraints.AllowAlignedLayers = string.Equals(eltConstraintSet.Attributes["AlignedLayersAllowed"].Value, "true", StringComparison.CurrentCultureIgnoreCase);
            // alternate layers allowed
            if (eltConstraintSet.HasAttribute("AlternateLayersAllowed"))
                constraints.AllowAlternateLayers = string.Equals(eltConstraintSet.Attributes["AlternateLayersAllowed"].Value, "true", StringComparison.CurrentCultureIgnoreCase);
            // allowed patterns
            if (eltConstraintSet.HasAttribute("AllowedPatterns"))
                constraints.AllowedPatternString = eltConstraintSet.Attributes["AllowedPatterns"].Value;
            // stop criterions
            if (constraints.UseMaximumHeight = eltConstraintSet.HasAttribute("MaximumHeight"))
                constraints.MaximumHeight = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["MaximumHeight"].Value), _unitSystem);
            if (constraints.UseMaximumNumberOfCases = eltConstraintSet.HasAttribute("ManimumNumberOfItems"))
                constraints.MaximumNumberOfItems = int.Parse(eltConstraintSet.Attributes["ManimumNumberOfItems"].Value);
            if (constraints.UseMaximumPalletWeight = eltConstraintSet.HasAttribute("MaximumPalletWeight"))
                constraints.MaximumPalletWeight = UnitsManager.ConvertMassFrom(double.Parse(eltConstraintSet.Attributes["MaximumPalletWeight"].Value), _unitSystem);
            // overhang / underhang
            if (eltConstraintSet.HasAttribute("OverhangX"))
                constraints.OverhangX = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["OverhangX"].Value), _unitSystem);
            if (eltConstraintSet.HasAttribute("OverhangY"))
                constraints.OverhangY = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["OverhangY"].Value), _unitSystem);
            // number of solutions to keep
            if (constraints.UseNumberOfSolutionsKept = eltConstraintSet.HasAttribute("NumberOfSolutions"))
                constraints.NumberOfSolutionsKept = int.Parse(eltConstraintSet.Attributes["NumberOfSolutions"].Value);
            // sanity check
            if (!constraints.IsValid)
                throw new Exception("Invalid constraint set");
            return constraints;
        }
        private PackPalletConstraintSet LoadPackPalletConstraintSet(XmlElement eltContraintSet)
        {
            PackPalletConstraintSet constraints = new PackPalletConstraintSet();
            if (eltContraintSet.HasAttribute("OverhangX"))
                constraints.OverhangX = UnitsManager.ConvertLengthFrom(double.Parse(eltContraintSet.Attributes["OverhangX"].Value), _unitSystem);
            if (eltContraintSet.HasAttribute("OverhangY"))
                constraints.OverhangY = UnitsManager.ConvertLengthFrom(double.Parse(eltContraintSet.Attributes["OverhangY"].Value), _unitSystem);
            constraints.MinOverhangX = LoadOptDouble(eltContraintSet, "MinOverhangX", UnitsManager.UnitType.UT_LENGTH);
            constraints.MinOverhangY = LoadOptDouble(eltContraintSet, "MinOverhangY", UnitsManager.UnitType.UT_LENGTH);
            constraints.MinimumSpace = LoadOptDouble(eltContraintSet, "MinimumSpace", UnitsManager.UnitType.UT_LENGTH);
            constraints.MaximumSpaceAllowed = LoadOptDouble(eltContraintSet, "MaximumSpaceAllowed", UnitsManager.UnitType.UT_LENGTH);
            constraints.MaximumPalletHeight = LoadOptDouble(eltContraintSet, "MaximumHeight", UnitsManager.UnitType.UT_LENGTH);
            if (!constraints.MaximumPalletHeight.Activated || constraints.MaximumPalletHeight.Value < 1)
                constraints.MaximumPalletHeight = new OptDouble(true, 1700);
            constraints.MaximumPalletWeight = LoadOptDouble(eltContraintSet, "MaximumPalletWeight", UnitsManager.UnitType.UT_MASS);
            constraints.LayerSwapPeriod = int.Parse( eltContraintSet.Attributes["LayerSwapPeriod"].Value );
            constraints.InterlayerPeriod = int.Parse( eltContraintSet.Attributes["InterlayerPeriod"].Value );
            if (!constraints.IsValid)
                throw new Exception("Invalid constraint set");
            return constraints;
        }
        #endregion

        private HCylinderPalletConstraintSet LoadHCylinderPalletConstraintSet(XmlElement eltConstraintSet)
        {
            HCylinderPalletConstraintSet constraints = new HCylinderPalletConstraintSet();
            // stop criterions
            if (constraints.UseMaximumPalletHeight = eltConstraintSet.HasAttribute("MaximumHeight"))
                constraints.MaximumPalletHeight = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["MaximumHeight"].Value), _unitSystem);
            if (constraints.UseMaximumNumberOfItems = eltConstraintSet.HasAttribute("ManimumNumberOfItems"))
                constraints.MaximumNumberOfItems = int.Parse(eltConstraintSet.Attributes["ManimumNumberOfItems"].Value);
            if (constraints.UseMaximumPalletWeight = eltConstraintSet.HasAttribute("MaximumPalletWeight"))
                constraints.MaximumPalletWeight = UnitsManager.ConvertMassFrom(double.Parse(eltConstraintSet.Attributes["MaximumPalletWeight"].Value), _unitSystem);
            // overhang / underhang
            if (eltConstraintSet.HasAttribute("OverhangX"))
                constraints.OverhangX = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["OverhangX"].Value), _unitSystem);
            if (eltConstraintSet.HasAttribute("OverhangY"))
                constraints.OverhangY = UnitsManager.ConvertLengthFrom(double.Parse(eltConstraintSet.Attributes["OverhangY"].Value), _unitSystem);
            return constraints;
        }

        private void LoadSolution(
            XmlElement eltSolution
            , out List<LayerDesc> listDesc
            , out List<SolutionItem> listSolItems)
        {
            listDesc = new List<LayerDesc>();
            listSolItems = new List<SolutionItem>();

            foreach (XmlNode node in eltSolution.ChildNodes)
            {
                if (string.Equals(node.Name, "LayerDescriptors", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement eltLayerDescriptors = node as XmlElement;
                    foreach (XmlNode nodeLayerDesc in eltLayerDescriptors.ChildNodes)
                    {
                        XmlElement eltLayerDesc = nodeLayerDesc as XmlElement;
                        if (string.Equals(eltLayerDesc.Name, "LayerDescBox", StringComparison.CurrentCultureIgnoreCase))
                            listDesc.Add( LayerDescBox.Parse(eltLayerDesc.InnerText) );
                        else if (string.Equals(eltLayerDesc.Name, "LayerDescDisk", StringComparison.CurrentCultureIgnoreCase))
                            listDesc.Add( LayerDescCyl.Parse(eltLayerDesc.InnerText) );
                    }
                }
                else if (string.Equals(node.Name, "SolutionItems", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlElement eltSolutionItems = node as XmlElement;
                    foreach (XmlNode nodeSolutionItem in eltSolutionItems.ChildNodes)
                    {
                        if (nodeSolutionItem is XmlElement eltSolutionItem)
                            listSolItems.Add(SolutionItem.Parse(eltSolutionItem.InnerText));
                    }
                }
            }
        }

        private PackPalletSolution LoadPackPalletSolution(XmlElement eltSolution)
        { 
            // title -> instantiation
            string stitle = eltSolution.Attributes["Title"].Value;
            // layer and list
            ILayer layer = null;
            List<LayerDescriptor> layerDescriptors = new List<LayerDescriptor>();

            foreach (XmlNode nodeSolChild in eltSolution.ChildNodes)
            {
                if (nodeSolChild is XmlElement eltChild)
                {
                    if (string.Equals(eltChild.Name, "BoxLayers", StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (XmlNode nodeLayer in eltChild.ChildNodes)
                        {
                            if (nodeLayer is XmlElement eltLayer)
                                layer = LoadLayer(eltLayer);
                        }
                    }
                    else if (string.Equals(eltChild.Name, "LayerRefs", StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (XmlNode nodeLayerRef in eltChild.ChildNodes)
                        {
                            if (nodeLayerRef is XmlElement eltLayerRef)
                            {
                                bool swapped = bool.Parse(eltLayerRef.Attributes["Swapped"].Value);
                                bool hasInterlayer = bool.Parse(eltLayerRef.Attributes["HasInterlayer"].Value);
                                layerDescriptors.Add(new LayerDescriptor(swapped, hasInterlayer));
                            }
                        }
                    }
                }
            }
            // create solution
            PackPalletSolution sol = new PackPalletSolution(null, stitle, layer as Layer3DBox);
            foreach (LayerDescriptor desc in layerDescriptors)
                sol.AddLayer(desc.Swapped, desc.HasInterlayer);
            return sol;
        }
        private CylPosition LoadCylPosition(XmlElement eltCylPosition)
        {
            string sPosition = eltCylPosition.Attributes["Position"].Value;
            string sAxisDir = eltCylPosition.Attributes["AxisDir"].Value;

            return new CylPosition( Vector3D.Parse(sPosition), HalfAxis.Parse(sAxisDir));
        }

        private BoxCaseSolution LoadBoxCaseSolution(XmlElement eltSolution)
        {
            // pattern
            string patternName = eltSolution.Attributes["Pattern"].Value;
            // orientation
            HalfAxis.HAxis orthoAxis = HalfAxis.Parse(eltSolution.Attributes["OrthoAxis"].Value);
            // instantiate box case solution
            BoxCaseSolution sol = new BoxCaseSolution(null, orthoAxis, patternName);
            // limit reached
            if (eltSolution.HasAttribute("LimitReached"))
            {
                string sLimitReached = eltSolution.Attributes["LimitReached"].Value;
                sol.LimitReached = (BoxCaseSolution.Limit)(int.Parse(sLimitReached));
            }
            // layers
            XmlElement eltLayers = eltSolution.ChildNodes[0] as XmlElement;
            foreach (XmlNode nodeLayer in eltLayers.ChildNodes)
            {
                Layer3DBox boxLayer = LoadLayer(nodeLayer as XmlElement) as Layer3DBox;
                sol.Add(boxLayer);
            }
            return sol;
        }
        private ILayer LoadLayer(XmlElement eltLayer)
        {
            ILayer layer = null;
            double zLow = UnitsManager.ConvertLengthFrom(
                Convert.ToDouble(eltLayer.Attributes["ZLow"].Value, CultureInfo.InvariantCulture)
                , _unitSystem);
            double maxSpace = 0.0;
            if (eltLayer.HasAttribute("MaximumSpace"))
                maxSpace = UnitsManager.ConvertLengthFrom(
                    Convert.ToDouble(eltLayer.Attributes["MaximumSpace"].Value, CultureInfo.InvariantCulture)
                    , _unitSystem);
            string patternName = string.Empty;
            if (eltLayer.HasAttribute("PatternName"))
                patternName = eltLayer.Attributes["PatternName"].Value;
            if (string.Equals(eltLayer.Name, "BoxLayer", StringComparison.CurrentCultureIgnoreCase))
            {
                Layer3DBox boxLayer = new Layer3DBox(UnitsManager.ConvertLengthFrom(zLow, _unitSystem), 0)
                {
                    MaximumSpace = maxSpace
                };
                foreach (XmlNode nodeBoxPosition in eltLayer.ChildNodes)
                {
                    XmlElement eltBoxPosition = nodeBoxPosition as XmlElement;
                    string sPosition = eltBoxPosition.Attributes["Position"].Value;
                    string sAxisLength = eltBoxPosition.Attributes["AxisLength"].Value;
                    string sAxisWidth = eltBoxPosition.Attributes["AxisWidth"].Value;
                    try
                    {
                        boxLayer.AddPosition(UnitsManager.ConvertLengthFrom(Vector3D.Parse(sPosition), _unitSystem), HalfAxis.Parse(sAxisLength), HalfAxis.Parse(sAxisWidth));
                    }
                    catch (Exception /*ex*/)
                    {
                        _log.Error(string.Format("Exception thrown: Position = {0} | AxisLength = {1} | AxisWidth = {2}",
                            sPosition, sAxisLength, sAxisWidth ));
                    }
                }
                layer = boxLayer;
            }
            else if (string.Equals(eltLayer.Name, "CylLayer", StringComparison.CurrentCultureIgnoreCase))
            {
                Layer3DCyl cylLayer = new Layer3DCyl(UnitsManager.ConvertLengthFrom(zLow, _unitSystem));
                foreach (XmlNode nodePosition in eltLayer.ChildNodes)
                {
                    XmlElement eltBoxPosition = nodePosition as XmlElement;
                    string sPosition = eltBoxPosition.Attributes["Position"].Value;
                    cylLayer.Add(UnitsManager.ConvertLengthFrom(Vector3D.Parse(sPosition), _unitSystem));
                    layer = cylLayer;
                }
            }
            else if (string.Equals(eltLayer.Name, "InterLayer", StringComparison.CurrentCultureIgnoreCase))
            {
                int typeId = 0;
                if (eltLayer.HasAttribute("TypeId"))
                    typeId = Convert.ToInt32(eltLayer.Attributes["TypeId"].Value);
                layer = new InterlayerPos(UnitsManager.ConvertLengthFrom(zLow, _unitSystem), typeId);
            }

            return layer;
        }
        #endregion

        #region TruckAnalysis
        private TruckConstraintSet LoadTruckConstraintSet(XmlElement eltTruckConstraintSet)
        {
            TruckConstraintSet constraintSet = new TruckConstraintSet();
            // multi layer allowed
            if (eltTruckConstraintSet.HasAttribute("MultilayerAllowed"))
                constraintSet.MultilayerAllowed = string.Equals(eltTruckConstraintSet.Attributes["MultilayerAllowed"].Value, "true", StringComparison.CurrentCultureIgnoreCase);
            if (eltTruckConstraintSet.HasAttribute("MinDistancePalletWall"))
            constraintSet.MinDistancePalletTruckWall = UnitsManager.ConvertLengthFrom(double.Parse(eltTruckConstraintSet.Attributes["MinDistancePalletWall"].Value), _unitSystem);
            if (eltTruckConstraintSet.HasAttribute("MinDistancePalletRoof"))
                constraintSet.MinDistancePalletTruckRoof = UnitsManager.ConvertLengthFrom(double.Parse(eltTruckConstraintSet.Attributes["MinDistancePalletRoof"].Value), _unitSystem);
            if (eltTruckConstraintSet.HasAttribute("AllowedPalletOrientations"))
            {
                string sAllowedPalletOrientations = eltTruckConstraintSet.Attributes["AllowedPalletOrientations"].Value;
                constraintSet.AllowPalletOrientationX = sAllowedPalletOrientations.Contains("X");
                constraintSet.AllowPalletOrientationY = sAllowedPalletOrientations.Contains("Y");
            }
            return constraintSet;
        }
        #endregion // Load truck analysis

        #endregion // load methods

        #region Save methods
        #region Main methods
        public void Write(string filePath)
        {
            try
            {
                // instantiate XmlDocument
                XmlDocument xmlDoc = new XmlDocument();
                // let's add the XML declaration section
                XmlNode xmlnode = xmlDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmlDoc.AppendChild(xmlnode);
                // create Document (root) element
                XmlElement xmlRootElement = xmlDoc.CreateElement("Document");
                xmlDoc.AppendChild(xmlRootElement);
                // name
                XmlAttribute xmlDocNameAttribute = xmlDoc.CreateAttribute("Name");
                xmlDocNameAttribute.Value = Name;
                xmlRootElement.Attributes.Append(xmlDocNameAttribute);
                // description
                XmlAttribute xmlDocDescAttribute = xmlDoc.CreateAttribute("Description");
                xmlDocDescAttribute.Value = Description;
                xmlRootElement.Attributes.Append(xmlDocDescAttribute);
                // author
                XmlAttribute xmlDocAuthorAttribute = xmlDoc.CreateAttribute("Author");
                xmlDocAuthorAttribute.Value = Author;
                xmlRootElement.Attributes.Append(xmlDocAuthorAttribute);
                // dateCreated
                XmlAttribute xmlDateCreatedAttribute = xmlDoc.CreateAttribute("DateCreated");
                xmlDateCreatedAttribute.Value = Convert.ToString(DateOfCreation, new CultureInfo("en-US"));
                xmlRootElement.Attributes.Append(xmlDateCreatedAttribute);
                // unit system
                XmlAttribute xmlUnitSystem = xmlDoc.CreateAttribute("UnitSystem");
                xmlUnitSystem.Value = string.Format("{0}", (int)UnitsManager.CurrentUnitSystem);
                xmlRootElement.Attributes.Append(xmlUnitSystem);
                // create ItemProperties element
                XmlElement xmlItemPropertiesElt = xmlDoc.CreateElement("ItemProperties");
                xmlRootElement.AppendChild(xmlItemPropertiesElt);
                foreach (ItemBase itemProperties in _typeList)
                {
                    CaseOfBoxesProperties caseOfBoxesProperties = itemProperties as CaseOfBoxesProperties;
                    if (null != caseOfBoxesProperties)
                        Save(caseOfBoxesProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is BoxProperties boxProperties && null == caseOfBoxesProperties)
                        Save(boxProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is BundleProperties bundleProperties)
                        Save(bundleProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is CylinderProperties cylinderProperties)
                        Save(cylinderProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is PalletProperties palletProperties)
                        Save(palletProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is InterlayerProperties interlayerProperties)
                        Save(interlayerProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is PalletCornerProperties cornerProperties)
                        Save(cornerProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is PalletCapProperties capProperties)
                        Save(capProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is PalletFilmProperties filmProperties)
                        Save(filmProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is TruckProperties truckProperties)
                        Save(truckProperties, xmlItemPropertiesElt, xmlDoc);
                    if (itemProperties is PackProperties packProperties) { }
                }
                foreach (ItemBase itemProperties in _typeList)
                {
                    if (itemProperties is PackProperties packProperties)
                        Save(packProperties, xmlItemPropertiesElt, xmlDoc);
                }
                // create Analyses element
                XmlElement xmlAnalysesElt = xmlDoc.CreateElement("Analyses");
                xmlRootElement.AppendChild(xmlAnalysesElt);
                foreach (Analysis analysis in Analyses)
                    SaveAnalysis(analysis, xmlAnalysesElt, xmlDoc);
                // finally save XmlDocument
                xmlDoc.Save(filePath);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Saving types
        public void Save(BoxProperties boxProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlBoxProperties element
            XmlElement xmlBoxProperties = xmlDoc.CreateElement("BoxProperties");
            parentElement.AppendChild(xmlBoxProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = boxProperties.ID.IGuid.ToString();
            xmlBoxProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = boxProperties.ID.Name;
            xmlBoxProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = boxProperties.ID.Description;
            xmlBoxProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Length);
            xmlBoxProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Width);
            xmlBoxProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Height);
            xmlBoxProperties.Attributes.Append(heightAttribute);
            // inside dimensions
            if (boxProperties.HasInsideDimensions)
            {
                // length
                XmlAttribute insideLengthAttribute = xmlDoc.CreateAttribute("InsideLength");
                insideLengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.InsideLength);
                xmlBoxProperties.Attributes.Append(insideLengthAttribute);
                // width
                XmlAttribute insideWidthAttribute = xmlDoc.CreateAttribute("InsideWidth");
                insideWidthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.InsideWidth);
                xmlBoxProperties.Attributes.Append(insideWidthAttribute);
                // height
                XmlAttribute insideHeightAttribute = xmlDoc.CreateAttribute("InsideHeight");
                insideHeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.InsideHeight);
                xmlBoxProperties.Attributes.Append(insideHeightAttribute);
            }
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.Weight);
            xmlBoxProperties.Attributes.Append(weightAttribute);
            // net weight
            XmlAttribute netWeightAttribute = xmlDoc.CreateAttribute("NetWeight");
            netWeightAttribute.Value = boxProperties.NetWeight.ToString();
            xmlBoxProperties.Attributes.Append(netWeightAttribute);
            // colors
            SaveColors(boxProperties.Colors, xmlBoxProperties, xmlDoc);
            // texture
            SaveTextures(boxProperties.TextureList, xmlBoxProperties, xmlDoc);
            // tape
            XmlAttribute tapeAttribute = xmlDoc.CreateAttribute("ShowTape");
            tapeAttribute.Value = string.Format("{0}", boxProperties.TapeWidth.Activated);
            xmlBoxProperties.Attributes.Append(tapeAttribute);
            if (boxProperties.TapeWidth.Activated)
            {
                XmlElement tapeElt = xmlDoc.CreateElement("Tape");
                xmlBoxProperties.AppendChild(tapeElt);

                XmlAttribute tapeWidthAttribute = xmlDoc.CreateAttribute("TapeWidth");
                tapeWidthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxProperties.TapeWidth.Value);
                tapeElt.Attributes.Append(tapeWidthAttribute);

                XmlAttribute tapeColorAttribute = xmlDoc.CreateAttribute("TapeColor");
                tapeColorAttribute.Value = string.Format("{0}", boxProperties.TapeColor.ToArgb());
                tapeElt.Attributes.Append(tapeColorAttribute);
            }
        }
        public void Save(PackProperties packProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPackProperties element
            XmlElement xmlPackProperties = xmlDoc.CreateElement("PackProperties");
            parentElement.AppendChild(xmlPackProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = packProperties.ID.IGuid.ToString();
            xmlPackProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = packProperties.ID.Name;
            xmlPackProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = packProperties.ID.Description;
            xmlPackProperties.Attributes.Append(descAttribute);
            // boxProperties
            XmlAttribute boxPropAttribute = xmlDoc.CreateAttribute("BoxProperties");
            boxPropAttribute.Value = packProperties.Box.ID.IGuid.ToString();
            xmlPackProperties.Attributes.Append(boxPropAttribute);
            // box orientation
            XmlAttribute orientationAttribute = xmlDoc.CreateAttribute("Orientation");
            orientationAttribute.Value = HalfAxis.ToString( packProperties.BoxOrientation );
            xmlPackProperties.Attributes.Append(orientationAttribute);
            // arrangement
            XmlAttribute arrAttribute = xmlDoc.CreateAttribute("Arrangement");
            arrAttribute.Value = packProperties.Arrangement.ToString();
            xmlPackProperties.Attributes.Append(arrAttribute);
            // wrapper
            XmlElement wrapperElt = xmlDoc.CreateElement("Wrapper");
            xmlPackProperties.AppendChild(wrapperElt);

            PackWrapper packWrapper = packProperties.Wrap;
            SaveWrapper(packWrapper as WrapperPolyethilene, wrapperElt, xmlDoc);
            SaveWrapper(packWrapper as WrapperPaper, wrapperElt, xmlDoc);
            SaveWrapper(packWrapper as WrapperCardboard, wrapperElt, xmlDoc);

            // outer dimensions
            if (packProperties.HasForcedOuterDimensions)
            {
                XmlAttribute outerDimAttribute = xmlDoc.CreateAttribute("OuterDimensions");
                outerDimAttribute.Value = packProperties.OuterDimensions.ToString();
                xmlPackProperties.Attributes.Append(outerDimAttribute);
            }
        }
        public void Save(CylinderProperties cylinderProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlBoxProperties element
            XmlElement xmlBoxProperties = xmlDoc.CreateElement("CylinderProperties");
            parentElement.AppendChild(xmlBoxProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = cylinderProperties.ID.IGuid.ToString();
            xmlBoxProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = cylinderProperties.ID.Name;
            xmlBoxProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = cylinderProperties.ID.Description;
            xmlBoxProperties.Attributes.Append(descAttribute);
            // radius outer
            XmlAttribute radiusOuterAttribute = xmlDoc.CreateAttribute("RadiusOuter");
            radiusOuterAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.RadiusOuter);
            xmlBoxProperties.Attributes.Append(radiusOuterAttribute);
            // radius inner
            XmlAttribute radiusInnerAttribute = xmlDoc.CreateAttribute("RadiusInner");
            radiusInnerAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.RadiusInner);
            xmlBoxProperties.Attributes.Append(radiusInnerAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.Height);
            xmlBoxProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.Weight);
            xmlBoxProperties.Attributes.Append(weightAttribute);
            // colorTop
            XmlAttribute topAttribute = xmlDoc.CreateAttribute("ColorTop");
            topAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.ColorTop.ToArgb());
            xmlBoxProperties.Attributes.Append(topAttribute);
            // colorWall
            XmlAttribute outerWallAttribute = xmlDoc.CreateAttribute("ColorWallOuter");
            outerWallAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.ColorWallOuter.ToArgb());
            xmlBoxProperties.Attributes.Append(outerWallAttribute);
            // color inner wall
            XmlAttribute innerWallAttribute = xmlDoc.CreateAttribute("ColorWallInner");
            innerWallAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylinderProperties.ColorWallInner.ToArgb());
            xmlBoxProperties.Attributes.Append(innerWallAttribute);
        }
        public void Save(PalletProperties palletProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement xmlPalletProperties = xmlDoc.CreateElement("PalletProperties");
            parentElement.AppendChild(xmlPalletProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = palletProperties.ID.IGuid.ToString();
            xmlPalletProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = palletProperties.ID.Name;
            xmlPalletProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = palletProperties.ID.Description;
            xmlPalletProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Length);
            xmlPalletProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Width);
            xmlPalletProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Height);
            xmlPalletProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.Weight);
            xmlPalletProperties.Attributes.Append(weightAttribute);
            // admissible load weight
            XmlAttribute admLoadWeightAttribute = xmlDoc.CreateAttribute("AdmissibleLoadWeight");
            admLoadWeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.AdmissibleLoadWeight);
            xmlPalletProperties.Attributes.Append(admLoadWeightAttribute);
            // admissible load height
            XmlAttribute admLoadHeightAttribute = xmlDoc.CreateAttribute("AdmissibleLoadHeight");
            admLoadHeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", palletProperties.AdmissibleLoadHeight);
            xmlPalletProperties.Attributes.Append(admLoadHeightAttribute);
            // type
            XmlAttribute typeAttribute = xmlDoc.CreateAttribute("Type");
            typeAttribute.Value = string.Format("{0}", palletProperties.TypeName);
            xmlPalletProperties.Attributes.Append(typeAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", palletProperties.Color.ToArgb());
            xmlPalletProperties.Attributes.Append(colorAttribute);
        }
        public void Save(InterlayerProperties interlayerProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement xmlInterlayerProperties = xmlDoc.CreateElement("InterlayerProperties");
            parentElement.AppendChild(xmlInterlayerProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = interlayerProperties.ID.IGuid.ToString();
            xmlInterlayerProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = interlayerProperties.ID.Name;
            xmlInterlayerProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = interlayerProperties.ID.Description;
            xmlInterlayerProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Length);
            xmlInterlayerProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Width);
            xmlInterlayerProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Thickness");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Thickness);
            xmlInterlayerProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", interlayerProperties.Weight);
            xmlInterlayerProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", interlayerProperties.Color.ToArgb());
            xmlInterlayerProperties.Attributes.Append(colorAttribute);
        }

        public void Save(PalletCornerProperties cornerProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create PalletCornerProperties element
            XmlElement xmlCornerProperties = xmlDoc.CreateElement("PalletCornerProperties");
            parentElement.AppendChild(xmlCornerProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = cornerProperties.ID.IGuid.ToString();
            xmlCornerProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = cornerProperties.ID.Name;
            xmlCornerProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = cornerProperties.ID.Description;
            xmlCornerProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Length);
            xmlCornerProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Width);
            xmlCornerProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Thickness");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Thickness);
            xmlCornerProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cornerProperties.Weight);
            xmlCornerProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", cornerProperties.Color.ToArgb());
            xmlCornerProperties.Attributes.Append(colorAttribute);
        }

        public void Save(PalletCapProperties capProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create PalletCornerProperties element
            XmlElement xmlCapProperties = xmlDoc.CreateElement("PalletCapProperties");
            parentElement.AppendChild(xmlCapProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = capProperties.ID.IGuid.ToString();
            xmlCapProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = capProperties.ID.Name;
            xmlCapProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = capProperties.ID.Description;
            xmlCapProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Length);
            xmlCapProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Width);
            xmlCapProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Height);
            xmlCapProperties.Attributes.Append(heightAttribute);
            // inside length
            XmlAttribute insideLengthAttribute = xmlDoc.CreateAttribute("InsideLength");
            insideLengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Length);
            xmlCapProperties.Attributes.Append(insideLengthAttribute);
            // inside width
            XmlAttribute insideWidthAttribute = xmlDoc.CreateAttribute("InsideWidth");
            insideWidthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Width);
            xmlCapProperties.Attributes.Append(insideWidthAttribute);
            // inside height
            XmlAttribute insideHeightAttribute = xmlDoc.CreateAttribute("InsideHeight");
            insideHeightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Height);
            xmlCapProperties.Attributes.Append(insideHeightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", capProperties.Weight);
            xmlCapProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", capProperties.Color.ToArgb());
            xmlCapProperties.Attributes.Append(colorAttribute); 
        }

        public void Save(PalletFilmProperties filmProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create PalletFilmProperties element
            XmlElement xmlFilmProperties = xmlDoc.CreateElement("PalletFilmProperties");
            parentElement.AppendChild(xmlFilmProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = filmProperties.ID.IGuid.ToString();
            xmlFilmProperties.Attributes.Append(guidAttribute);
            // Name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = filmProperties.ID.Name;
            xmlFilmProperties.Attributes.Append(nameAttribute);
            // Description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = filmProperties.ID.Description;
            xmlFilmProperties.Attributes.Append(descAttribute);
            // Transparency
            XmlAttribute transparencyAttribute = xmlDoc.CreateAttribute("Transparency");
            transparencyAttribute.Value = filmProperties.UseTransparency.ToString();
            xmlFilmProperties.Attributes.Append(transparencyAttribute);
            // Hatching
            XmlAttribute hatchingAttribute = xmlDoc.CreateAttribute("Hatching");
            hatchingAttribute.Value = filmProperties.UseHatching.ToString();
            xmlFilmProperties.Attributes.Append(hatchingAttribute);
            // HatchSpacing
            XmlAttribute hatchSpacingAttribute = xmlDoc.CreateAttribute("HatchSpacing");
            hatchSpacingAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", filmProperties.HatchSpacing);
            xmlFilmProperties.Attributes.Append(hatchSpacingAttribute);
            // HatchAngle
            XmlAttribute hatchAngleAttribute = xmlDoc.CreateAttribute("HatchAngle");
            hatchAngleAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", filmProperties.HatchAngle);
            xmlFilmProperties.Attributes.Append(hatchAngleAttribute);
            // Color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", filmProperties.Color.ToArgb());
            xmlFilmProperties.Attributes.Append(colorAttribute); 
        }

        public void Save(BundleProperties bundleProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement xmlBundleProperties = xmlDoc.CreateElement("BundleProperties");
            parentElement.AppendChild(xmlBundleProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = bundleProperties.ID.IGuid.ToString();
            xmlBundleProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = bundleProperties.ID.Name;
            xmlBundleProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = bundleProperties.ID.Description;
            xmlBundleProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.Length);
            xmlBundleProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.Width);
            xmlBundleProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("UnitThickness");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.UnitThickness);
            xmlBundleProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("UnitWeight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", bundleProperties.UnitWeight);
            xmlBundleProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", bundleProperties.Color.ToArgb());
            xmlBundleProperties.Attributes.Append(colorAttribute);
            // numberFlats
            XmlAttribute numberFlatsAttribute = xmlDoc.CreateAttribute("NumberFlats");
            numberFlatsAttribute.Value = string.Format("{0}", bundleProperties.NoFlats);
            xmlBundleProperties.Attributes.Append(numberFlatsAttribute);
        }

        public void Save(TruckProperties truckProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlPalletProperties element
            XmlElement xmlTruckProperties = xmlDoc.CreateElement("TruckProperties");
            parentElement.AppendChild(xmlTruckProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = truckProperties.ID.IGuid.ToString();
            xmlTruckProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = truckProperties.ID.Name;
            xmlTruckProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = truckProperties.ID.Description;
            xmlTruckProperties.Attributes.Append(descAttribute);
            // length
            XmlAttribute lengthAttribute = xmlDoc.CreateAttribute("Length");
            lengthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.Length);
            xmlTruckProperties.Attributes.Append(lengthAttribute);
            // width
            XmlAttribute widthAttribute = xmlDoc.CreateAttribute("Width");
            widthAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.Width);
            xmlTruckProperties.Attributes.Append(widthAttribute);
            // height
            XmlAttribute heightAttribute = xmlDoc.CreateAttribute("Height");
            heightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.Height);
            xmlTruckProperties.Attributes.Append(heightAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("AdmissibleLoadWeight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", truckProperties.AdmissibleLoadWeight);
            xmlTruckProperties.Attributes.Append(weightAttribute);
            // color
            XmlAttribute colorAttribute = xmlDoc.CreateAttribute("Color");
            colorAttribute.Value = string.Format("{0}", truckProperties.Color.ToArgb());
            xmlTruckProperties.Attributes.Append(colorAttribute);
        }
        #endregion

        #region Save helpers
        private void SaveInt(int i, XmlDocument xmlDoc, XmlElement xmlElement, string attributeName)
        {
            XmlAttribute att = xmlDoc.CreateAttribute(attributeName);
            att.Value = i.ToString();
            xmlElement.Attributes.Append(att);
        }
        private void SaveDouble(double d, XmlDocument xmlDoc, XmlElement xmlElement, string attributeName)
        {
            XmlAttribute att = xmlDoc.CreateAttribute(attributeName);
            att.Value = string.Format(CultureInfo.InvariantCulture, "{0}", d);
            xmlElement.Attributes.Append(att);
        }
        private void SaveOptDouble(OptDouble optD, XmlDocument xmlDoc, XmlElement xmlElement, string attributeName)
        {
            XmlAttribute att = xmlDoc.CreateAttribute(attributeName);
            att.Value = optD.ToString();
            xmlElement.Attributes.Append(att);
        }
        private void SaveTextures(List<Pair<HalfAxis.HAxis, Texture>> textureList, XmlElement xmlBoxProperties, XmlDocument xmlDoc)
        { 
            XmlElement xmlTexturesElement = xmlDoc.CreateElement("Textures");
            xmlBoxProperties.AppendChild(xmlTexturesElement);
            foreach (Pair<HalfAxis.HAxis, Texture> texPair in textureList)
            {
                XmlElement xmlFaceTexture = xmlDoc.CreateElement("FaceTexture");
                xmlTexturesElement.AppendChild(xmlFaceTexture);
                // face index
                XmlAttribute xmlFaceNormal = xmlDoc.CreateAttribute("FaceNormal");
                xmlFaceNormal.Value = HalfAxis.ToString(texPair.first);
                xmlFaceTexture.Attributes.Append(xmlFaceNormal);
                // texture position
                XmlAttribute xmlPosition = xmlDoc.CreateAttribute("Position");
                xmlPosition.Value = texPair.second.Position.ToString();
                xmlFaceTexture.Attributes.Append(xmlPosition);
                // texture size
                XmlAttribute xmlSize = xmlDoc.CreateAttribute("Size");
                xmlSize.Value = texPair.second.Size.ToString();
                xmlFaceTexture.Attributes.Append(xmlSize);
                // angle
                XmlAttribute xmlAngle = xmlDoc.CreateAttribute("Angle");
                xmlAngle.Value = string.Format(CultureInfo.InvariantCulture, "{0}", texPair.second.Angle);
                xmlFaceTexture.Attributes.Append(xmlAngle);
                // bitmap
                XmlAttribute xmlBitmap = xmlDoc.CreateAttribute("Bitmap");
                xmlBitmap.Value = Document.BitmapToString(texPair.second.Bitmap);
                xmlFaceTexture.Attributes.Append(xmlBitmap);
            }
        }

        private void SaveColors(Color[] colors, XmlElement eltBoxProperties, XmlDocument xmlDoc)
        { 
            // face colors
            XmlElement xmlFaceColors = xmlDoc.CreateElement("FaceColors");
            eltBoxProperties.AppendChild(xmlFaceColors);
            short i = 0;
            foreach (Color color in colors)
            {
                XmlElement xmlFaceColor = xmlDoc.CreateElement("FaceColor");
                xmlFaceColors.AppendChild(xmlFaceColor);
                // face index
                XmlAttribute xmlFaceIndex = xmlDoc.CreateAttribute("FaceIndex");
                xmlFaceIndex.Value = string.Format("{0}", i);
                xmlFaceColor.Attributes.Append(xmlFaceIndex);
                // color
                XmlAttribute xmlColor = xmlDoc.CreateAttribute("Color");
                xmlColor.Value = string.Format("{0}", color.ToArgb());
                xmlFaceColor.Attributes.Append(xmlColor);
                ++i;
            }
        }
        #endregion

        #region Save Wrappers
        private void SaveWrapperBase(PackWrapper wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
            // type
            XmlAttribute typeAttrib = xmlDoc.CreateAttribute("Type");
            typeAttrib.Value = wrapper.Type.ToString();
            wrapperElt.Attributes.Append(typeAttrib);
            // color
            XmlAttribute colorAttrib = xmlDoc.CreateAttribute("Color");
            colorAttrib.Value = string.Format("{0}", wrapper.Color.ToArgb());
            wrapperElt.Attributes.Append(colorAttrib);
            // weight
            XmlAttribute weightAttrib = xmlDoc.CreateAttribute("Weight");
            weightAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", wrapper.Weight);
            wrapperElt.Attributes.Append(weightAttrib);
            // thickness
            XmlAttribute thicknessAttrib = xmlDoc.CreateAttribute("UnitThickness");
            thicknessAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", wrapper.UnitThickness);
            wrapperElt.Attributes.Append(thicknessAttrib);
        }
        private void SaveWrapper(WrapperPolyethilene wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
            SaveWrapperBase(wrapper, wrapperElt, xmlDoc);
            // transparency
            XmlAttribute transparentAttrib = xmlDoc.CreateAttribute("Transparent");
            transparentAttrib.Value = wrapper.Transparent.ToString();
            wrapperElt.Attributes.Append(transparentAttrib);
        }
        private void SaveWrapper(WrapperPaper wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
            SaveWrapperBase(wrapper, wrapperElt, xmlDoc);
        }
        private void SaveWrapper(WrapperCardboard wrapper, XmlElement wrapperElt, XmlDocument xmlDoc)
        {
            if (null == wrapper) return;
           SaveWrapperBase(wrapper, wrapperElt, xmlDoc);
           // wall distribution
           XmlAttribute wallDistribAttrib = xmlDoc.CreateAttribute("NumberOfWalls");
           wallDistribAttrib.Value = string.Format("{0} {1} {2}", wrapper.Wall(0), wrapper.Wall(1), wrapper.Wall(2));
           wrapperElt.Attributes.Append(wallDistribAttrib);
            // tray specific
            if (wrapper is WrapperTray wrapperTray)
            {
                XmlAttribute heightAttrib = xmlDoc.CreateAttribute("Height");
                heightAttrib.Value = string.Format(CultureInfo.InvariantCulture, "{0}", wrapperTray.Height);
                wrapperElt.Attributes.Append(heightAttrib);
            }
        }
        #endregion

        #region Save analysis
        private void SaveAnalysis(Analysis analysis, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // analysis name
            AnalysisCasePallet analysisCasePallet = analysis as AnalysisCasePallet;
            AnalysisBoxCase analysisBoxCase = analysis as AnalysisBoxCase;
            AnalysisCylinderPallet analysisCylinderPallet = analysis as AnalysisCylinderPallet;
            AnalysisCylinderCase analysisCylinderCase = analysis as AnalysisCylinderCase;
            AnalysisPalletTruck analysisPalletTruck = analysis as AnalysisPalletTruck;
            AnalysisCaseTruck analysisCaseTruck = analysis as AnalysisCaseTruck;

            string analysisName = string.Empty;
            if (null != analysisCasePallet) analysisName = "AnalysisCasePallet";
            else if (null != analysisBoxCase) analysisName = "AnalysisBoxCase";
            else if (null != analysisCylinderPallet) analysisName = "AnalysisCylinderPallet";
            else if (null != analysisCylinderCase) analysisName = "AnalysisCylinderCase";
            else if (null != analysisPalletTruck) analysisName = "AnalysisPalletTruck";
            else if (null != analysisCaseTruck) analysisName = "AnalysisCaseTruck";
            else throw new Exception("Unexpected analysis type");

            // create analysis element
            XmlElement xmlAnalysisElt = xmlDoc.CreateElement(analysisName);
            parentElement.AppendChild(xmlAnalysisElt);

            // guid
            XmlAttribute analysisGuidAttribute = xmlDoc.CreateAttribute("Id");
            analysisGuidAttribute.Value = analysis.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisGuidAttribute);
            // name
            XmlAttribute analysisNameAttribute = xmlDoc.CreateAttribute("Name");
            analysisNameAttribute.Value = analysis.ID.Name;
            xmlAnalysisElt.Attributes.Append(analysisNameAttribute);
            // description
            XmlAttribute analysisDescriptionAttribute = xmlDoc.CreateAttribute("Description");
            analysisDescriptionAttribute.Value = analysis.ID.Description;
            xmlAnalysisElt.Attributes.Append(analysisDescriptionAttribute);
            // contentId
            XmlAttribute analysisContentId = xmlDoc.CreateAttribute("ContentId");
            analysisContentId.Value = analysis.Content.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisContentId);
            // containerId
            XmlAttribute analysisContainerId = xmlDoc.CreateAttribute("ContainerId");
            analysisContainerId.Value = analysis.Container.ID.IGuid.ToString();
            xmlAnalysisElt.Attributes.Append(analysisContainerId);
            // interlayers
            XmlElement eltInterlayers = xmlDoc.CreateElement("Interlayers");
            xmlAnalysisElt.AppendChild(eltInterlayers);
            foreach (InterlayerProperties interlayer in analysis.Interlayers)
            {
                XmlElement eltInterlayer = xmlDoc.CreateElement("Interlayer");
                eltInterlayer.InnerText = interlayer.ID.IGuid.ToString();
                eltInterlayers.AppendChild(eltInterlayer);
            }
            if (null != analysisCasePallet)
            {
                // PalletCornerId
                if (null != analysisCasePallet.PalletCornerProperties)
                {
                    XmlAttribute palletCornerAttribute = xmlDoc.CreateAttribute("PalletCornerId");
                    palletCornerAttribute.Value = string.Format("{0}", analysisCasePallet.PalletCornerProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(palletCornerAttribute);
                }
                // PalletCapId
                if (null != analysisCasePallet.PalletCapProperties)
                {
                    XmlAttribute palletCapIdAttribute = xmlDoc.CreateAttribute("PalletCapId");
                    palletCapIdAttribute.Value = string.Format("{0}", analysisCasePallet.PalletCapProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(palletCapIdAttribute);
                }
                // PalletFilmId
                if (null != analysisCasePallet.PalletFilmProperties)
                {
                    XmlAttribute palletFilmIdAttribute = xmlDoc.CreateAttribute("PalletFilmId");
                    palletFilmIdAttribute.Value = string.Format("{0}", analysisCasePallet.PalletFilmProperties.ID.IGuid);
                    xmlAnalysisElt.Attributes.Append(palletFilmIdAttribute);
                }
            }
            // constraint set
            ConstraintSetAbstract constraintSet = analysis.ConstraintSet; 
            XmlElement eltContraintSet = xmlDoc.CreateElement("ConstraintSet");
            xmlAnalysisElt.AppendChild(eltContraintSet);
            // allowed orientation
            XmlAttribute attOrientations = xmlDoc.CreateAttribute("Orientations");
            attOrientations.Value = constraintSet.AllowedOrientationsString;
            xmlAnalysisElt.Attributes.Append(attOrientations);
            // maximum weight
            XmlAttribute attMaximumWeight = xmlDoc.CreateAttribute("MaximumWeight");
            attMaximumWeight.Value = constraintSet.OptMaxWeight.ToString();
            eltContraintSet.Attributes.Append(attMaximumWeight);
            // maximum number of items
            XmlAttribute attMaximumNumber = xmlDoc.CreateAttribute("MaximumNumberOfItems");
            attMaximumNumber.Value = constraintSet.OptMaxNumber.ToString();
            eltContraintSet.Attributes.Append(attMaximumNumber);


            if (null != analysisCasePallet)
            {
                ConstraintSetCasePallet constraintSetCasePallet = analysisCasePallet.ConstraintSet as ConstraintSetCasePallet;

                XmlAttribute attOverhang = xmlDoc.CreateAttribute("Overhang");
                attOverhang.Value = constraintSetCasePallet.Overhang.ToString();
                eltContraintSet.Attributes.Append(attOverhang);

                XmlAttribute attMinSpace = xmlDoc.CreateAttribute("MinSpace");
                attMinSpace.Value = constraintSetCasePallet.MinimumSpace.ToString();
                eltContraintSet.Attributes.Append(attMinSpace);

                XmlAttribute attMaximumHeight = xmlDoc.CreateAttribute("MaximumPalletHeight");
                attMaximumHeight.Value = constraintSetCasePallet.OptMaxHeight.ToString();
                eltContraintSet.Attributes.Append(attMaximumHeight);

                XmlAttribute attPalletFilmTurns = xmlDoc.CreateAttribute("PalletFilmTurns");
                attPalletFilmTurns.Value = constraintSetCasePallet.PalletFilmTurns.ToString();
                eltContraintSet.Attributes.Append(attPalletFilmTurns);
            }
            else if (null != analysisBoxCase)
            {
                ConstraintSetBoxCase constraintSetBoxCase = analysisBoxCase.ConstraintSet as ConstraintSetBoxCase;
            }
            else if (null != analysisPalletTruck)
            {
                ConstraintSetPalletTruck constraintSetPalletTruck = analysisPalletTruck.ConstraintSet as ConstraintSetPalletTruck;

                XmlAttribute attMinDistanceLoadWall = xmlDoc.CreateAttribute("MinDistanceLoadWall");
                attMinDistanceLoadWall.Value = constraintSetPalletTruck.MinDistanceLoadWall.ToString();
                eltContraintSet.Attributes.Append(attMinDistanceLoadWall);

                XmlAttribute attMinDiastanceLoadRoof = xmlDoc.CreateAttribute("MinDistanceLoadRoof");
                attMinDiastanceLoadRoof.Value = constraintSetPalletTruck.MinDistanceLoadRoof.ToString();
                eltContraintSet.Attributes.Append(attMinDiastanceLoadRoof);

                XmlAttribute attAllowMultipleLayers = xmlDoc.CreateAttribute("AllowMultipleLayers");
                attAllowMultipleLayers.Value = constraintSetPalletTruck.AllowMultipleLayers ? "1" : "0";
                eltContraintSet.Attributes.Append(attAllowMultipleLayers);                
            }
            // solution
            SaveSolution(analysis.Solution, xmlAnalysisElt, xmlDoc);
        }
        private void SaveSolution(Solution sol, XmlElement parentElement, XmlDocument xmlDoc)
        {
            XmlElement eltSolution = xmlDoc.CreateElement("Solution");
            parentElement.AppendChild(eltSolution);

            // layer descriptors
            XmlElement eltLayerDescriptors = xmlDoc.CreateElement("LayerDescriptors");
            eltSolution.AppendChild(eltLayerDescriptors);
            foreach (LayerDesc layerDesc in sol.LayerDescriptors)
            {
                string eltLayerDescName = string.Empty;
                if (layerDesc is LayerDescBox) eltLayerDescName = "LayerDescBox";
                else if (layerDesc is LayerDescCyl) eltLayerDescName = "LayerDescCyl";
                else throw new Exception("Unexpected LayerDesc type!");
                XmlElement eltLayerDesc = xmlDoc.CreateElement("LayerDescBox");
                eltLayerDesc.InnerText = layerDesc.ToString();
                eltLayerDescriptors.AppendChild(eltLayerDesc);
            }
            // solution items
            XmlElement eltSolutionItems = xmlDoc.CreateElement("SolutionItems");
            eltSolution.AppendChild(eltSolutionItems);
            foreach (SolutionItem solItem in sol.SolutionItems)
            {
                XmlElement eltSolutionItem = xmlDoc.CreateElement("SolutionItem");
                eltSolutionItem.InnerText = solItem.ToString();
                eltSolutionItems.AppendChild(eltSolutionItem);
            }
        }
        #endregion

        #region Save legacy analysis
        public void Save(CaseOfBoxesProperties caseOfBoxesProperties, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create xmlBoxProperties element
            XmlElement xmlBoxProperties = xmlDoc.CreateElement("CaseOfBoxesProperties");
            parentElement.AppendChild(xmlBoxProperties);
            // Id
            XmlAttribute guidAttribute = xmlDoc.CreateAttribute("Id");
            guidAttribute.Value = caseOfBoxesProperties.ID.IGuid.ToString();
            xmlBoxProperties.Attributes.Append(guidAttribute);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = caseOfBoxesProperties.ID.Name;
            xmlBoxProperties.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descAttribute = xmlDoc.CreateAttribute("Description");
            descAttribute.Value = caseOfBoxesProperties.ID.Description;
            xmlBoxProperties.Attributes.Append(descAttribute);
            // weight
            XmlAttribute weightAttribute = xmlDoc.CreateAttribute("Weight");
            weightAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", caseOfBoxesProperties.Weight);
            xmlBoxProperties.Attributes.Append(weightAttribute);
            // save inside ref to box properties
            XmlAttribute insideBoxId = xmlDoc.CreateAttribute("InsideBoxId");
            insideBoxId.Value = caseOfBoxesProperties.InsideBoxProperties.ID.IGuid.ToString();
            xmlBoxProperties.Attributes.Append(insideBoxId);
            // save case definition
            SaveCaseDefinition(caseOfBoxesProperties.CaseDefinition, xmlBoxProperties, xmlDoc);
            // save optim constraintset
            SaveCaseOptimConstraintSet(caseOfBoxesProperties.CaseOptimConstraintSet, xmlBoxProperties, xmlDoc);
            // colors
            SaveColors(caseOfBoxesProperties.Colors, xmlBoxProperties, xmlDoc);
            // texture
            SaveTextures(caseOfBoxesProperties.TextureList, xmlBoxProperties, xmlDoc);
        }
        private void SaveCaseDefinition(CaseDefinition caseDefinition, XmlElement xmlBoxProperties, XmlDocument xmlDoc)
        {
            XmlElement xmlCaseDefElement = xmlDoc.CreateElement("CaseDefinition");
            xmlBoxProperties.AppendChild(xmlCaseDefElement);
            // case arrangement
            XmlAttribute xmlArrangement = xmlDoc.CreateAttribute("Arrangement");
            xmlArrangement.Value = caseDefinition.Arrangement.ToString();
            xmlCaseDefElement.Attributes.Append(xmlArrangement);
            // box orientation
            XmlAttribute xmlOrientation = xmlDoc.CreateAttribute("Orientation");
            xmlOrientation.Value = string.Format("{0} {1}", caseDefinition.Dim0, caseDefinition.Dim1);
            xmlCaseDefElement.Attributes.Append(xmlOrientation);
        }
        private void SaveCaseOptimConstraintSet(ParamSetPackOptim caseOptimConstraintSet, XmlElement xmlBoxProperties, XmlDocument xmlDoc)
        {
            XmlElement xmlCaseOptimConstraintSet = xmlDoc.CreateElement("OptimConstraintSet");
            xmlBoxProperties.AppendChild(xmlCaseOptimConstraintSet);
            // wall thickness
            XmlAttribute xmlWallThickness = xmlDoc.CreateAttribute("WallThickness");
            xmlWallThickness.Value = string.Format(CultureInfo.InvariantCulture, "{0}", caseOptimConstraintSet.WallThickness);
            xmlCaseOptimConstraintSet.Attributes.Append(xmlWallThickness);
            // wall surface mass
            XmlAttribute xmlWallSurfaceMass = xmlDoc.CreateAttribute("WallSurfaceMass");
            xmlWallSurfaceMass.Value = string.Format(CultureInfo.InvariantCulture, "{0}", caseOptimConstraintSet.WallSurfaceMass);
            xmlCaseOptimConstraintSet.Attributes.Append(xmlWallSurfaceMass);
            // no walls
            XmlAttribute xmlNumberOfWalls = xmlDoc.CreateAttribute("NumberOfWalls");
            xmlNumberOfWalls.Value = string.Format("{0} {1} {2}"
                , caseOptimConstraintSet.NoWalls[0]
                , caseOptimConstraintSet.NoWalls[1]
                , caseOptimConstraintSet.NoWalls[2]);
            xmlCaseOptimConstraintSet.Attributes.Append(xmlNumberOfWalls);
        }
        public void SavePackPalletAnalysis(PackPalletAnalysis analysis, XmlElement parentElement, XmlDocument xmlDoc)
        {
            // create analysis element
            XmlElement xmlAnalysisElt = xmlDoc.CreateElement("PackPalletAnalysis");
            parentElement.AppendChild(xmlAnalysisElt);
            // Name
            XmlAttribute analysisNameAttribute = xmlDoc.CreateAttribute("Name");
            analysisNameAttribute.Value = analysis.ID.Name;
            xmlAnalysisElt.Attributes.Append(analysisNameAttribute);
            // Description
            XmlAttribute analysisDescriptionAttribute = xmlDoc.CreateAttribute("Description");
            analysisDescriptionAttribute.Value = analysis.ID.Description;
            xmlAnalysisElt.Attributes.Append(analysisDescriptionAttribute);
            // BoxId
            XmlAttribute packIdAttribute = xmlDoc.CreateAttribute("PackId");
            packIdAttribute.Value = string.Format("{0}", analysis.PackProperties.ID.IGuid);
            xmlAnalysisElt.Attributes.Append(packIdAttribute);
            // PalletId
            XmlAttribute palletIdAttribute = xmlDoc.CreateAttribute("PalletId");
            palletIdAttribute.Value = string.Format("{0}", analysis.PalletProperties.ID.IGuid);
            xmlAnalysisElt.Attributes.Append(palletIdAttribute);
            // InterlayerId
            if (null != analysis.InterlayerProperties)
            {
                XmlAttribute interlayerIdAttribute = xmlDoc.CreateAttribute("InterlayerId");
                interlayerIdAttribute.Value = string.Format("{0}", analysis.InterlayerProperties.ID.IGuid);
                xmlAnalysisElt.Attributes.Append(interlayerIdAttribute);
            }
            // Constraint set
            XmlElement constraintSetElt = xmlDoc.CreateElement("ConstraintSet");
            xmlAnalysisElt.AppendChild(constraintSetElt);
            SaveDouble(analysis.ConstraintSet.OverhangX, xmlDoc, constraintSetElt, "OverhangX");
            SaveDouble(analysis.ConstraintSet.OverhangY, xmlDoc, constraintSetElt, "OverhangY");
            SaveOptDouble(analysis.ConstraintSet.MinOverhangX, xmlDoc, constraintSetElt, "MinOverhangX");
            SaveOptDouble(analysis.ConstraintSet.MinOverhangY, xmlDoc, constraintSetElt, "MinOverhangY");
            SaveOptDouble(analysis.ConstraintSet.MinimumSpace, xmlDoc, constraintSetElt, "MinimumSpace");
            SaveOptDouble(analysis.ConstraintSet.MaximumSpaceAllowed, xmlDoc, constraintSetElt, "MaximumSpaceAllowed");
            SaveOptDouble(analysis.ConstraintSet.MaximumPalletHeight, xmlDoc, constraintSetElt, "MaximumPalletHeight");
            SaveOptDouble(analysis.ConstraintSet.MaximumPalletWeight, xmlDoc, constraintSetElt, "MaximumPalletWeight");
            SaveOptDouble(analysis.ConstraintSet.MaximumLayerWeight, xmlDoc, constraintSetElt, "MaximumLayerWeight");
            SaveInt(analysis.ConstraintSet.LayerSwapPeriod, xmlDoc, constraintSetElt, "LayerSwapPeriod");
            SaveInt(analysis.ConstraintSet.InterlayerPeriod, xmlDoc, constraintSetElt, "InterlayerPeriod");
            // solutions
            XmlElement solutionsElt = xmlDoc.CreateElement("Solutions");
            xmlAnalysisElt.AppendChild(solutionsElt);
            int solIndex = 0;
            foreach (PackPalletSolution sol in analysis.Solutions)
            {
                SavePackPalletSolution(
                    sol
                    , analysis.GetSelSolutionBySolutionIndex(solIndex) // null if not selected
                    , solutionsElt
                    , xmlDoc);
                ++solIndex;
            }
        }
        public void SaveBoxCaseAnalysis(BoxCaseAnalysis analysis, XmlElement parentElement, XmlDocument xmlDoc)
        { 
            // create analysis element
            XmlElement xmlAnalysisElt = xmlDoc.CreateElement("AnalysisBoxCase");
            parentElement.AppendChild(xmlAnalysisElt);
            // Name
            XmlAttribute analysisNameAttribute = xmlDoc.CreateAttribute("Name");
            analysisNameAttribute.Value = analysis.ID.Name;
            xmlAnalysisElt.Attributes.Append(analysisNameAttribute);
            // Description
            XmlAttribute analysisDescriptionAttribute = xmlDoc.CreateAttribute("Description");
            analysisDescriptionAttribute.Value = analysis.ID.Description;
            xmlAnalysisElt.Attributes.Append(analysisDescriptionAttribute);
            // BoxId
            XmlAttribute boxIdAttribute = xmlDoc.CreateAttribute("BoxId");
            boxIdAttribute.Value = string.Format("{0}", analysis.BProperties.ID.IGuid);
            xmlAnalysisElt.Attributes.Append(boxIdAttribute);
            // PalletId
            XmlAttribute palletIdAttribute = xmlDoc.CreateAttribute("CaseId");
            palletIdAttribute.Value = string.Format("{0}", analysis.CaseProperties.ID.IGuid);
            xmlAnalysisElt.Attributes.Append(palletIdAttribute);
            // Constraint set
            SaveBoxCaseConstraintSet(analysis.ConstraintSet, xmlAnalysisElt, xmlDoc);
            // Solutions
            int solIndex = 0;
            XmlElement solutionsElt = xmlDoc.CreateElement("Solutions");
            xmlAnalysisElt.AppendChild(solutionsElt);
            if (null != analysis.Solutions)
                foreach (BoxCaseSolution sol in analysis.Solutions)
                {
                    SaveBoxCaseSolution(
                        analysis
                        , sol
                        , analysis.GetSelSolutionBySolutionIndex(solIndex) // null if not selected
                        , solutionsElt
                        , xmlDoc);
                    ++solIndex;
                }
        }
        public void SaveBoxCaseConstraintSet(BCaseConstraintSet constraintSet, XmlElement xmlAnalysisElt, XmlDocument xmlDoc)
        { 
            // ConstraintSet
            XmlElement constraintSetElement = xmlDoc.CreateElement("ConstraintSetCase");
            xmlAnalysisElt.AppendChild(constraintSetElement);
            if (constraintSet is BoxCaseConstraintSet boxCaseContraintSet)
            {
                // allowed box positions
                XmlAttribute allowedAxisAttribute = xmlDoc.CreateAttribute("AllowedBoxPositions");
                constraintSetElement.Attributes.Append(allowedAxisAttribute);
                allowedAxisAttribute.Value = boxCaseContraintSet.AllowOrthoAxisString;
            }
            // stop criterions
            // 1. maximum number of boxes
            if (constraintSet.UseMaximumNumberOfBoxes)
            {
                XmlAttribute maximumNumberOfBoxes = xmlDoc.CreateAttribute("ManimumNumberOfBoxes");
                maximumNumberOfBoxes.Value = string.Format("{0}", constraintSet.MaximumNumberOfBoxes);
                constraintSetElement.Attributes.Append(maximumNumberOfBoxes);
            }
            // 2. maximum case weight
            if (constraintSet.UseMaximumCaseWeight)
            {
                XmlAttribute maximumPalletWeight = xmlDoc.CreateAttribute("MaximumCaseWeight");
                maximumPalletWeight.Value = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}",
                    constraintSet.MaximumCaseWeight);
                constraintSetElement.Attributes.Append(maximumPalletWeight);
            }
            xmlAnalysisElt.AppendChild(constraintSetElement);
        }
        public void SaveBoxCaseSolution(BoxCaseAnalysis analysis, BoxCaseSolution sol, SelBoxCaseSolution selSolution, XmlElement solutionsElt, XmlDocument xmlDoc)
        {
            // Solution
            XmlElement solutionElt = xmlDoc.CreateElement("Solution");
            solutionsElt.AppendChild(solutionElt);
            // Pattern name
            XmlAttribute patternNameAttribute = xmlDoc.CreateAttribute("Pattern");
            patternNameAttribute.Value = sol.PatternName;
            solutionElt.Attributes.Append(patternNameAttribute);
            // ortho axis
            XmlAttribute orthoAxisAttribute = xmlDoc.CreateAttribute("OrthoAxis");
            orthoAxisAttribute.Value = HalfAxis.ToString(sol.OrthoAxis);
            solutionElt.Attributes.Append(orthoAxisAttribute);
            // limit
            XmlAttribute limitReached = xmlDoc.CreateAttribute("LimitReached");
            limitReached.Value = string.Format("{0}", (int)sol.LimitReached);
            solutionElt.Attributes.Append(limitReached);
            // layers
            XmlElement layersElt = xmlDoc.CreateElement("Layers");
            solutionElt.AppendChild(layersElt);

            foreach (Layer3DBox boxLayer in sol)
                Save(boxLayer, layersElt, xmlDoc);

            // Is selected ?
            if (null != selSolution)
            {
                // selected attribute
                XmlAttribute selAttribute = xmlDoc.CreateAttribute("Selected");
                selAttribute.Value = "true";
                solutionElt.Attributes.Append(selAttribute);
            }
        }
        private void SavePackPalletSolution(
            PackPalletSolution sol
            , SelPackPalletSolution selSolution
            , XmlElement solutionsElt
            , XmlDocument xmlDoc)
        { 
            // solution
            XmlElement solutionElt = xmlDoc.CreateElement("Solution");
            solutionsElt.AppendChild(solutionElt);
            // title
            XmlAttribute titleAttribute = xmlDoc.CreateAttribute("Title");
            titleAttribute.Value = sol.Title;
            solutionElt.Attributes.Append(titleAttribute);
            // layers
            XmlElement boxLayersElt = xmlDoc.CreateElement("BoxLayers");
            solutionElt.AppendChild(boxLayersElt);
            Save(sol.Layer, boxLayersElt, xmlDoc);
            // layerRefs
            XmlElement layerRefsElt = xmlDoc.CreateElement("LayerRefs");
            solutionElt.AppendChild(layerRefsElt);
            // layers
            foreach (LayerDescriptor layerDesc in sol.Layers)
            {
                XmlElement layerRefElt = xmlDoc.CreateElement("LayerRef");
                layerRefsElt.AppendChild(layerRefElt);
                XmlAttribute attributeSwapped = xmlDoc.CreateAttribute("Swapped");
                attributeSwapped.Value = layerDesc.Swapped.ToString();
                layerRefElt.Attributes.Append(attributeSwapped);
                XmlAttribute attributeHasInterlayer = xmlDoc.CreateAttribute("HasInterlayer");
                attributeHasInterlayer.Value = layerDesc.HasInterlayer.ToString();
                layerRefElt.Attributes.Append(attributeHasInterlayer);
            }
            // Is selected ?
            if (null != selSolution)
            {
                // selected attribute
                XmlAttribute selAttribute = xmlDoc.CreateAttribute("Selected");
                selAttribute.Value = "true";
                solutionElt.Attributes.Append(selAttribute);
            }
        }

        public void Save(ECTAnalysis ectAnalysis, bool unique, XmlElement ectAnalysesElt, XmlDocument xmlDoc)
        {
            XmlElement ectAnalysisElt = xmlDoc.CreateElement("EctAnalysis");
            ectAnalysesElt.AppendChild(ectAnalysisElt);
            // name
            XmlAttribute nameAttribute = xmlDoc.CreateAttribute("Name");
            nameAttribute.Value = ectAnalysis.ID.Name;
            ectAnalysisElt.Attributes.Append(nameAttribute);
            // description
            XmlAttribute descriptionAttribute = xmlDoc.CreateAttribute("Description");
            descriptionAttribute.Value = ectAnalysis.ID.Description;
            ectAnalysisElt.Attributes.Append(descriptionAttribute);
            // cardboard
            XmlElement cardboardElt = xmlDoc.CreateElement("Cardboard");
            ectAnalysesElt.AppendChild(cardboardElt);
            // - name
            XmlAttribute nameCardboardAttribute = xmlDoc.CreateAttribute("Name");
            nameCardboardAttribute.Value = ectAnalysis.Cardboard.Name;
            cardboardElt.Attributes.Append(nameCardboardAttribute);
            // - thickness
            XmlAttribute thicknessAttribute = xmlDoc.CreateAttribute("Thickness");
            thicknessAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", ectAnalysis.Cardboard.Thickness);
            cardboardElt.Attributes.Append(thicknessAttribute);
             // - ect
            XmlAttribute ectAttribute = xmlDoc.CreateAttribute("ECT");
            ectAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", ectAnalysis.Cardboard.ECT);
            cardboardElt.Attributes.Append(ectAttribute);
            // - stiffnessX
            XmlAttribute stiffnessAttributeX = xmlDoc.CreateAttribute("StiffnessX");
            stiffnessAttributeX.Value = string.Format(CultureInfo.InvariantCulture, "{0}", ectAnalysis.Cardboard.RigidityDX);
            cardboardElt.Attributes.Append(stiffnessAttributeX);
            // - stiffnessY
            XmlAttribute stiffnessAttributeY = xmlDoc.CreateAttribute("StiffnessY");
            stiffnessAttributeY.Value = string.Format(CultureInfo.InvariantCulture, "{0}", ectAnalysis.Cardboard.RigidityDY);
            cardboardElt.Attributes.Append(stiffnessAttributeY);
            // case type
            XmlAttribute caseTypeAttribute = xmlDoc.CreateAttribute("CaseType");
            caseTypeAttribute.Value = ectAnalysis.CaseType;
            ectAnalysisElt.Attributes.Append(caseTypeAttribute);
            // print surface
            XmlAttribute printSurfaceAttribute = xmlDoc.CreateAttribute("PrintSurface");
            printSurfaceAttribute.Value = ectAnalysis.PrintSurface;
            ectAnalysesElt.Attributes.Append(printSurfaceAttribute);
            // mc kee formula mode
            XmlAttribute mcKeeFormulaAttribute = xmlDoc.CreateAttribute("McKeeFormulaMode");
            mcKeeFormulaAttribute.Value = ectAnalysis.McKeeFormulaText;
            ectAnalysisElt.Attributes.Append(mcKeeFormulaAttribute);
        }
        public void Save(Layer3DBox boxLayer, XmlElement layersElt, XmlDocument xmlDoc)
        {
            // BoxLayer
            XmlElement boxlayerElt = xmlDoc.CreateElement("BoxLayer");
            layersElt.AppendChild(boxlayerElt);
            // ZLow
            XmlAttribute zlowAttribute = xmlDoc.CreateAttribute("ZLow");
            zlowAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxLayer.ZLow);
            boxlayerElt.Attributes.Append(zlowAttribute);
            // maximum space
            XmlAttribute attributeMaxSpace = xmlDoc.CreateAttribute("MaximumSpace");
            attributeMaxSpace.Value = string.Format(CultureInfo.InvariantCulture, "{0}", boxLayer.MaximumSpace);
            boxlayerElt.Attributes.Append(attributeMaxSpace);

            foreach (BoxPosition boxPosition in boxLayer)
            {
                // BoxPosition
                XmlElement boxPositionElt = xmlDoc.CreateElement("BoxPosition");
                boxlayerElt.AppendChild(boxPositionElt);
                // Position
                XmlAttribute positionAttribute = xmlDoc.CreateAttribute("Position");
                positionAttribute.Value = boxPosition.Position.ToString();
                boxPositionElt.Attributes.Append(positionAttribute);
                // AxisLength
                XmlAttribute axisLengthAttribute = xmlDoc.CreateAttribute("AxisLength");
                axisLengthAttribute.Value = HalfAxis.ToString(boxPosition.DirectionLength);
                boxPositionElt.Attributes.Append(axisLengthAttribute);
                // AxisWidth
                XmlAttribute axisWidthAttribute = xmlDoc.CreateAttribute("AxisWidth");
                axisWidthAttribute.Value = HalfAxis.ToString(boxPosition.DirectionWidth);
                boxPositionElt.Attributes.Append(axisWidthAttribute);
            }
        }
        public void Save(Layer3DCyl cylLayer, XmlElement layersElt, XmlDocument xmlDoc)
        {
            // BoxLayer
            XmlElement cylLayerElt = xmlDoc.CreateElement("CylLayer");
            layersElt.AppendChild(cylLayerElt);
            // ZLow
            XmlAttribute zlowAttribute = xmlDoc.CreateAttribute("ZLow");
            zlowAttribute.Value = string.Format(CultureInfo.InvariantCulture, "{0}", cylLayer.ZLow);
            cylLayerElt.Attributes.Append(zlowAttribute);
            foreach (Vector3D boxPosition in cylLayer)
            {
                // BoxPosition
                XmlElement cylPositionElt = xmlDoc.CreateElement("CylPosition");
                cylLayerElt.AppendChild(cylPositionElt);
                // Position
                XmlAttribute positionAttribute = xmlDoc.CreateAttribute("Position");
                positionAttribute.Value = boxPosition.ToString();
                cylPositionElt.Attributes.Append(positionAttribute);
            }            
        }
        #endregion
        #endregion

        #region Close
        public virtual void Close()
        {
            // remove all analysis and items
            // -> this should close any listening forms
            while (Analyses.Count > 0)
                RemoveItem(Analyses[0]);
            while (_typeList.Count > 0)
                RemoveItem(_typeList[0]);
            DocumentClosed?.Invoke(this);
        }
        #endregion

        #region Helpers
        private ItemBase GetTypeByGuid(Guid guid)
        {
            return _typeList.FirstOrDefault(x => x.ID.IGuid == guid)
                ?? throw new ArgumentException($"No type with Guid = {guid}", nameof(guid));
        }
        private ItemBase GetTypeByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return null;
            return GetTypeByGuid(Guid.Parse(guid));
        }
        private Packable GetContentByGuid(Guid guid)
        {
            foreach (ItemBase type in _typeList)
            {
                if (type.ID.IGuid == guid)
                {
                    if (type is Packable packable)
                        return packable;
                    else
                        throw new ArgumentException($"Guid {guid} found but not a PackableBrick", nameof(guid));
                }
            }
            foreach (Analysis analysis in Analyses)
            {
                if (analysis.ID.IGuid == guid)
                    return analysis.EquivalentPackable;                
            }
            throw new ArgumentException($"No type with Guid = {guid.ToString()}", nameof(guid));
        }
        private static string BitmapToString(Bitmap bmp)
        {
            byte[] bmpBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmpBytes = ms.GetBuffer();
                ms.Close();
            }
            return Convert.ToBase64String(bmpBytes);
        }
        private static Bitmap StringToBitmap(string bmpData)
        {
            byte[] bytes = Convert.FromBase64String(bmpData);
            return new Bitmap(new System.IO.MemoryStream(bytes));
        }
        private static int[] ParseInt2(string value)
        {
            string regularExp = "(?<i1>.*) (?<i2>.*)";
            Regex r = new Regex(regularExp, RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                int[] iArray = new int[2];
                iArray[0] = int.Parse(m.Result("${i1}"));
                iArray[1] = int.Parse(m.Result("${i2}"));
                return iArray;
            }
            else
                throw new Exception("Failed parsing int[2] from " + value);
        }
        private static int[] ParseInt3(string value)
        {
            string regularExp = "(?<i1>.*) (?<i2>.*) (?<i3>.*)";
            Regex r = new Regex(regularExp, RegexOptions.Singleline);
            Match m = r.Match(value);
            if (m.Success)
            {
                int[] iArray = new int[3];
                iArray[0] = int.Parse(m.Result("${i1}"));
                iArray[1] = int.Parse(m.Result("${i2}"));
                iArray[2] = int.Parse(m.Result("${i3}"));
                return iArray;
            }
            else
                throw new Exception("Failed parsing int[3] from " + value);
        }
        #endregion

        #region Methods to be overriden
        public virtual void Modify()
        {
        }
        #endregion

        #region Listener notification methods
        public void AddListener(IDocumentListener listener)
        {
            _listeners.Add(listener);
        }
        private void NotifyOnNewTypeCreated(ItemBase item)
        {
            foreach (IDocumentListener listener in _listeners)
                listener.OnNewTypeCreated(this, item);
        }
        private void NotifyOnNewAnalysisCreated(Analysis analysis)
        {
            foreach (IDocumentListener listener in _listeners)
                listener.OnNewAnalysisCreated(this, analysis);
        }
        private void NotifyAnalysisUpdated(Analysis analysis)
        {
            foreach (IDocumentListener listener in _listeners)
                listener.OnAnalysisUpdated(this, analysis);
        }
        private void NotifyOnNewAnalysisCreated(HAnalysis analysis)
        {
            foreach (IDocumentListener listener in _listeners)
                listener.OnNewAnalysisCreated(this, analysis);
        }
        private void NotifyAnalysisUpdated(HAnalysis analysis)
        {
            foreach (IDocumentListener listener in _listeners)
                listener.OnAnalysisUpdated(this, analysis);
        }
        private void NotifyOnTypeRemoved(ItemBase item)
        {
            foreach (IDocumentListener listener in _listeners)
                listener.OnTypeRemoved(this, item);
        }
        private void NotifyOnAnalysisRemoved(ItemBase analysis)
        {
            foreach (IDocumentListener listener in _listeners)
                listener.OnAnalysisRemoved(this, analysis);
        }
        #endregion
    }
}
