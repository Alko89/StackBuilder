#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Reporting;

// Docking
using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public class DocumentSB : Document, IDocument
    {
        #region Data members
        private string _filePath;
        private bool _dirty = false;
        private List<IView> _views = new List<IView>();
        private IView _activeView;
        public event EventHandler Modified;
        #endregion

        #region Constructor
        public DocumentSB(string filePath, IDocumentListener listener)
            :base(filePath, listener)
        {
            _filePath = filePath;
            _dirty = false;
        }
        public DocumentSB(string name, string description, string author, IDocumentListener listener)
            : base(name, description, author, DateTime.Now, listener)
        {
            _dirty = false;
        }
        #endregion

        #region Public properties
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        #endregion

        #region IDocument implementation
        public bool IsDirty { get { return _dirty; } }
        public bool IsNew { get { return string.IsNullOrEmpty(_filePath); } }
        public bool HasValidPath  {   get { return System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_filePath)); } }
        public void Save()
        {
            if (IsNew) return;
            if (!HasValidPath)
                throw new System.IO.DirectoryNotFoundException(
                    string.Format("Directory {0} could not be found!", System.IO.Path.GetDirectoryName(_filePath))
                    );
            Write(_filePath);
            _dirty = false;
        }

        public void SaveAs(string filePath)
        {
            _filePath = filePath;
            Save();
        }

        public void Close(CancelEventArgs e)
        {
            while (_views.Count > 0)
                RemoveView(_views[0]);
            base.Close();
        }

        public override void Modify()
        {
            _dirty = true;
            Modified?.Invoke(this, new EventArgs());
        }

        public List<IView> Views { get { return _views; } }

        public IView ActiveView
        {
            set
            {
                _activeView = value;
                _activeView.Activate();
            }
            get
            {
                return _activeView;
            }
        }

        public void AddView(IView view)
        {
            _views.Add(view);
        }

        public void RemoveView(IView view)
        {
            // remove from list of attached views
            _views.Remove(view);
        }
        #endregion

        #region View creation methods
        public DockContentView CreateViewAnalysis(Analysis analysis)
        {
            DockContentView form = null;
            if (analysis is AnalysisCasePallet) form = new DockContentAnalysisCasePallet(this, analysis as AnalysisCasePallet);
            else if (analysis is AnalysisBoxCase) form = new DockContentAnalysisBoxCase(this, analysis as AnalysisBoxCase);
            else if (analysis is AnalysisCylinderPallet) form = new DockContentAnalysisCylinderPallet(this, analysis as AnalysisCylinderPallet);
            else if (analysis is AnalysisCylinderCase) form = new DockContentAnalysisCylinderCase(this, analysis as AnalysisCylinderCase);
            else if (analysis is AnalysisPalletTruck) form = new DockContentAnalysisPalletTruck(this, analysis as AnalysisPalletTruck);
            else if (analysis is AnalysisCaseTruck) form = new DockContentAnalysisCaseTruck(this, analysis as AnalysisCaseTruck);
            else
            {
                _log.Error(string.Format("Analysis ({0}) type not handled", analysis.Name));
                return null;
            }
            AddView(form);
            return form;
        }
        /// <summary>
        /// creates a new DockContentECTAnalysis view
        /// </summary>
        public DockContentECTAnalysis CreateECTAnalysisView(ECTAnalysis analysis)
        {
            DockContentECTAnalysis form = new DockContentECTAnalysis(this, analysis);
            AddView(form);
            return form;
        }
        #endregion

        #region UI item creation
        /// <summary>
        /// Creates a new BoxProperties object with MODE_BOX
        /// </summary>
        public void CreateNewBoxUI()
        {
            FormNewBox form = new FormNewBox(this, FormNewBox.Mode.MODE_BOX);
            if (DialogResult.OK == form.ShowDialog())
            {
                BoxProperties boxProperties = CreateNewBox(form.BoxName, form.Description
                    , form.BoxLength, form.BoxWidth, form.BoxHeight
                    , form.Weight, form.Colors);
                boxProperties.TextureList = form.TextureList;
            }
        }
        /// <summary>
        /// Creates a new BoxProperties object with MODE_CASE
        /// </summary>
        public void CreateNewCaseUI()
        {
            FormNewBox form = new FormNewBox(this, FormNewBox.Mode.MODE_CASE);
            if (DialogResult.OK == form.ShowDialog())
            {
                BoxProperties boxProperties = CreateNewCase(form.BoxName, form.Description
                    , form.BoxLength, form.BoxWidth, form.BoxHeight
                    , form.InsideLength, form.InsideWidth, form.InsideHeight
                    , form.Weight, form.Colors);
                boxProperties.TapeColor = form.TapeColor;
                boxProperties.TapeWidth = form.TapeWidth;
                boxProperties.TextureList = form.TextureList;
            }
        }
        /// <summary>
        /// Creates a new PackProperties object
        /// </summary>
        public void CreateNewPackUI()
        {
            FormNewPack form = new FormNewPack(this, null)
            {
                Boxes = Boxes.ToList()
            };
            if (DialogResult.OK == form.ShowDialog())
            {
                PackProperties packProperties = CreateNewPack(
                    form.ItemName, form.ItemDescription
                    , form.SelectedBox
                    , form.Arrangement
                    , form.BoxOrientation
                    , form.Wrapper);
                if (form.HasForcedOuterDimensions)
                    packProperties.ForceOuterDimensions(form.OuterDimensions);
            }
        }

        public void CreateNewCylinderUI()
        {
            FormNewCylinder form = new FormNewCylinder(this, null);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewCylinder(
                    form.ItemName, form.ItemDescription
                    , form.RadiusOuter, form.RadiusInner, form.CylinderHeight, form.Weight
                    , form.ColorTop, form.ColorWallOuter, form.ColorWallInner);

        }
        /// <summary>
        /// Creates a new BundleProperties object
        /// </summary>
        public void CreateNewBundleUI()
        { 
            FormNewBundle form = new FormNewBundle(this);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewBundle(
                    form.BundleName, form.Description
                    , form.BundleLength, form.BundleWidth, form.UnitThickness
                    , form.UnitWeight
                    , form.Color
                    , form.NoFlats);
        }
        /// <summary>
        /// Creates a new InterlayerProperties object
        /// </summary>
        public void CreateNewInterlayerUI()
        { 
            FormNewInterlayer form = new FormNewInterlayer(this, null);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewInterlayer(
                    form.ItemName, form.ItemDescription
                    , form.InterlayerLength, form.InterlayerWidth, form.Thickness
                    , form.Weight
                    , form.Color);
        }
        /// <summary>
        /// Creates new pallet corners
        /// </summary>
        public void CreateNewPalletCornersUI()
        {
            FormNewPalletCorners form = new FormNewPalletCorners(this, null);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewPalletCorners(form.ItemName, form.ItemDescription,
                    form.CornerLength, form.CornerWidth, form.CornerThickness,
                    form.CornerWeight,
                    form.CornerColor);
        }
        /// <summary>
        /// Creates a new pallet cap
        /// </summary>
        public void CreateNewPalletCapUI()
        {
            FormNewPalletCap form = new FormNewPalletCap(this, null);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewPalletCap(form.ItemName, form.ItemDescription, 
                    form.CapLength, form.CapWidth, form.CapHeight,
                    form.CapInnerLength, form.CapInnerWidth, form.CapInnerHeight,
                    form.CapWeight, form.CapColor);
        }
        /// <summary>
        /// Creates new pallet film
        /// </summary>
        public void CreateNewPalletFilmUI()
        {
            FormNewPalletFilm form = new FormNewPalletFilm(this, null);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewPalletFilm(form.ItemName, form.ItemDescription,
                    form.UseTransparency,
                    form.UseHatching, form.HatchSpacing, form.HatchAngle,
                    form.FilmColor);
        }
        /// <summary>
        /// creates a new PalletProperties object
        /// </summary>
        public void CreateNewPalletUI()
        {        
            FormNewPallet form = new FormNewPallet(this, null);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewPallet(form.ItemName, form.ItemDescription
                    , form.PalletTypeName
                    , form.PalletLength, form.PalletWidth, form.PalletHeight
                    , form.Weight
                    , form.PalletColor);
        }
        /// <summary>
        /// Creates a new TruckProperties
        /// </summary>
        public void CreateNewTruckUI()
        {
            FormNewTruck form = new FormNewTruck(this, null);
            if (DialogResult.OK == form.ShowDialog())
                CreateNewTruck(form.ItemName, form.ItemDescription
                    , form.TruckLength, form.TruckWidth, form.TruckHeight
                    , form.TruckAdmissibleLoadWeight
                    , form.TruckColor);
        }
        /// <summary>
        /// Creates a new palet analysis
        /// </summary>
        /// <returns>created palet analysis</returns>
        public void CreateNewAnalysisCasePalletUI()
        {
            if (!CanCreateAnalysisCasePallet && !CanCreateAnalysisBundlePallet) return;
            if (Properties.Settings.Default.DummyMode)
            {
                FormNewAnalysisCasePalletDM form = new FormNewAnalysisCasePalletDM(this, null);
                if (DialogResult.OK == form.ShowDialog()) {}
            }
            else
            {
                FormNewAnalysisCasePallet form = new FormNewAnalysisCasePallet(this, null);
                if (DialogResult.OK == form.ShowDialog()) { }
            }
        }
        public void CreateNewAnalysisBoxCaseUI()
        {
            if (!CanCreateAnalysisBoxCase && !CanCreateAnalysisBundleCase) return;
            FormNewAnalysisBoxCase form = new FormNewAnalysisBoxCase(this, null);
            if (DialogResult.OK == form.ShowDialog()) {}
        }
        public void CreateNewAnalysisCylinderPalletUI()
        {
            if (!CanCreateAnalysisCylinderPallet) return;
            FormNewAnalysisCylinderPallet form = new FormNewAnalysisCylinderPallet(this, null);
            if (DialogResult.OK == form.ShowDialog()) {}
        }
        public void CreateNewAnalysisCylinderCaseUI()
        {
            if (!CanCreateAnalysisCylinderCase) return;
            FormNewAnalysisCylinderCase form = new FormNewAnalysisCylinderCase(this, null);
            if (DialogResult.OK == form.ShowDialog()) {}
        }
        public AnalysisPalletTruck CreateNewAnalysisPalletTruckUI()
        {
            if (!CanCreateAnalysisPalletTruck) return null;
            FormNewAnalysisPalletTruck form = new FormNewAnalysisPalletTruck(this, null);
            if (DialogResult.OK == form.ShowDialog()) {}
            return null;
        }
        public AnalysisCaseTruck CreateNewAnalysisCaseTruckUI()
        {
            if (!CanCreateAnalysisCaseTruck) return null;
            FormNewAnalysisCaseTruck form = new FormNewAnalysisCaseTruck(this, null);
            if (DialogResult.OK == form.ShowDialog()) {}
            return null;
        }
        public HAnalysis CreateNewHAnalysisPalletUI()
        {
            if (!CanCreateAnalysisCasePallet) return null;
            FormNewHAnalysis form = new FormNewHAnalysisCasePallet(this, null);
            if (DialogResult.OK == form.ShowDialog()) {}
            return null;
        }
        #endregion

        #region UI item edition
        public void EditAnalysis(Analysis analysis)
        {
            // search for any DockContentAnalysis window
            var seq = (from view in Views
                       where view is DockContentAnalysisEdit && analysis == (view as DockContentAnalysisEdit).Analysis
                       select view);
            while (seq.Count() > 0) seq.First().Close();

            // instantiate a form to edit analysis
            Form form = null;
            if (analysis is AnalysisCasePallet) form = new FormNewAnalysisCasePallet(this, analysis);
            else if (analysis is AnalysisBoxCase) form = new FormNewAnalysisBoxCase(this, analysis);
            else if (analysis is AnalysisCylinderPallet) form = new FormNewAnalysisCylinderPallet(this, analysis);
            else if (analysis is AnalysisCylinderCase) form = new FormNewAnalysisCylinderCase(this, analysis);
            else if (analysis is AnalysisPalletTruck) form = new FormNewAnalysisPalletTruck(this, analysis);
            else if (analysis is AnalysisCaseTruck) form = new FormNewAnalysisCaseTruck(this, analysis);
            else
            {
                MessageBox.Show("Unexepected analysis type!");
                return;
            }
            if (DialogResult.OK == form.ShowDialog())
            {
                DockContentView formAnalysis = CreateViewAnalysis(analysis);
                formAnalysis.Show();
            }
        }
        #endregion

        #region Legacy
        #endregion
    }
}
