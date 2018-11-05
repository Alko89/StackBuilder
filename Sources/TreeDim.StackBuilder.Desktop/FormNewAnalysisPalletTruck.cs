#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewAnalysisPalletTruck : FormNewAnalysis, IItemBaseFilter
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisPalletTruck));
        #endregion

        #region Constructor
        public FormNewAnalysisPalletTruck(Document doc, Analysis analysis)
            : base(doc, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName
        { get { return Resources.ID_ANALYSIS; } }
        #endregion

        #region Public properties
        public AnalysisPalletTruck AnalysisCast
        {
            get { return _item as AnalysisPalletTruck; }
            set { _item = value; }
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbPallets.Initialize(_document, this, null != AnalysisBase ? AnalysisBase.Content : null);
            cbTrucks.Initialize(_document, this, null != AnalysisBase ? AnalysisBase.Container : null);

            // event handling
            uCtrlLayerList.LayerSelected += OnLayerSelected;
            uCtrlLayerList.RefreshFinished += OnLayerSelected;
            uCtrlLayerList.ButtonSizes = new Size(200, 100);

            if (null == AnalysisBase)
            {
                tbName.Text = _document.GetValidNewAnalysisName(ItemDefaultName);
                tbDescription.Text = tbName.Text;
                
                uCtrlMinDistanceLoadWall.ValueX = Settings.Default.MinDistancePalletTruckWallX;
                uCtrlMinDistanceLoadWall.ValueY = Settings.Default.MinDistancePalletTruckWallY;
                uCtrlMinDistanceLoadRoof.Value = Settings.Default.MinDistancePalletTruckRoof;
                chkbAllowMultipleLayers.Checked = Settings.Default.AllowMultipleLayers;
                uCtrlMaxNoPallets.Value = new OptInt(false, 1);
            }
            else
            {
                tbName.Text = AnalysisBase.Name;
                tbDescription.Text = AnalysisBase.Description;

                ConstraintSetPalletTruck  constraintSet = AnalysisBase.ConstraintSet as ConstraintSetPalletTruck;
                if (null == constraintSet) return;

                uCtrlMinDistanceLoadWall.ValueX = constraintSet.MinDistanceLoadWall.X;
                uCtrlMinDistanceLoadWall.ValueY = constraintSet.MinDistanceLoadWall.Y;
                uCtrlMinDistanceLoadRoof.Value = constraintSet.MinDistanceLoadRoof;
                chkbAllowMultipleLayers.Checked = constraintSet.AllowMultipleLayers;
                uCtrlMaxNoPallets.Value = constraintSet.OptMaxNumber;
            }
            checkBoxBestLayersOnly.Checked = Settings.Default.KeepBestSolutions;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Settings.Default.MinDistancePalletTruckWallX = uCtrlMinDistanceLoadWall.ValueX;
            Settings.Default.MinDistancePalletTruckWallY = uCtrlMinDistanceLoadWall.ValueY;
            Settings.Default.MinDistancePalletTruckRoof = uCtrlMinDistanceLoadRoof.Value;
            Settings.Default.KeepBestSolutions = checkBoxBestLayersOnly.Checked;
            Settings.Default.Save();
        }
        #endregion

        #region FormNewAnalysis override
        public override void OnNext()
        {
            base.OnNext();
            try
            {
                List<LayerDesc> layerDescs = new List<LayerDesc>();
                foreach (Layer2D layer2D in uCtrlLayerList.Selected)
                    layerDescs.Add(layer2D.LayerDescriptor);

                Solution.SetSolver(new LayerSolver());

                AnalysisPalletTruck analysis = AnalysisCast;
                if (null == analysis)
                    AnalysisBase = _document.CreateNewAnalysisPalletTruck(
                        ItemName, ItemDescription
                        , SelectedLoadedPallet, SelectedTruckProperties
                        , BuildConstraintSet()
                        , layerDescs);
                else
                {
                    analysis.ID.SetNameDesc(ItemName, ItemDescription);
                    analysis.Content = SelectedLoadedPallet;
                    analysis.TruckProperties = SelectedTruckProperties;
                    analysis.ConstraintSet = BuildConstraintSet();
                    analysis.AddSolution(layerDescs);

                    _document.UpdateAnalysis(analysis);
                }
                Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region IItemBaseFilter implementation
        public bool Accept(Control ctrl, ItemBase itemBase)
        {
            if (ctrl == cbPallets)
                return itemBase is LoadedPallet;
            else if (ctrl == cbTrucks)
                return itemBase is TruckProperties; 
            return false;
        }
        #endregion

        #region Private properties
        private Packable SelectedLoadedPallet
        {
            get { return cbPallets.SelectedType as Packable; }
        }
        private TruckProperties SelectedTruckProperties
        {
            get { return cbTrucks.SelectedType as TruckProperties; }
        }
        #endregion

        #region Event handlers
        private void OnInputChanged(object sender, EventArgs e)
        {
            try
            {
                // get loaded pallet / truck
                PackableBrick packable = cbPallets.SelectedType as PackableBrick;
                TruckProperties truckProperties = cbTrucks.SelectedType as TruckProperties;
                if (null == packable || null == truckProperties)
                    return;
                // compute
                LayerSolver solver = new LayerSolver();
                List<Layer2D> layers = solver.BuildLayers(
                    packable.OuterDimensions
                    , new Vector2D(
                        truckProperties.InsideLength - 2.0 * uCtrlMinDistanceLoadWall.ValueX
                        , truckProperties.InsideWidth - 2.0 * uCtrlMinDistanceLoadWall.ValueY)
                    , 0.0 /* offsetZ */
                    , BuildConstraintSet()
                    , checkBoxBestLayersOnly.Checked
                    );
                // update control
                uCtrlLayerList.SingleSelection = true;
                uCtrlLayerList.Packable = packable;
                uCtrlLayerList.ContainerHeight = truckProperties.InsideHeight - uCtrlMinDistanceLoadRoof.Value;
                uCtrlLayerList.FirstLayerSelected = true;
                uCtrlLayerList.LayerList = layers.Cast<ILayer2D>().ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        protected void OnLayerSelected(object sender, EventArgs e)
        {
            try
            {
                ILayer2D[] layers = uCtrlLayerList.Selected;
                UpdateStatus(layers.Length > 0 ? string.Empty : Resources.ID_SELECTATLEASTONELAYOUT);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Helpers
        private ConstraintSetPalletTruck BuildConstraintSet()
        {
            ConstraintSetPalletTruck constraintSet = new ConstraintSetPalletTruck(SelectedTruckProperties)
            {
                MinDistanceLoadWall = new Vector2D(uCtrlMinDistanceLoadWall.ValueX, uCtrlMinDistanceLoadWall.ValueY),
                MinDistanceLoadRoof = uCtrlMinDistanceLoadRoof.Value,
                OptMaxNumber = uCtrlMaxNoPallets.Value,
                AllowMultipleLayers = chkbAllowMultipleLayers.Checked
            };
            return constraintSet;
        }
        #endregion
    }
}
