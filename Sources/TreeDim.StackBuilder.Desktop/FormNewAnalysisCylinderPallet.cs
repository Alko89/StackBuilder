#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Engine;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewAnalysisCylinderPallet
        : FormNewAnalysis, IItemBaseFilter
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisCylinderPallet));
        #endregion

        #region Constructor
        public FormNewAnalysisCylinderPallet(Document doc, Analysis analysis)
            : base(doc, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region FormNewBase override
        public override string ItemDefaultName
        { get { return Resources.ID_ANALYSIS; } }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbCylinders.Initialize(_document, this, null);
            cbPallets.Initialize(_document, this, null);

            // event handling
            uCtrlLayerList.LayerSelected += onLayerSelected;
            uCtrlLayerList.RefreshFinished += onLayerSelected;
            uCtrlLayerList.ButtonSizes = new Size(100, 100);

            if (null == AnalysisBase)
            {
                tbName.Text = _document.GetValidNewAnalysisName(ItemDefaultName);
                tbDescription.Text = tbName.Text;

                uCtrlMaximumHeight.Value = Settings.Default.MaximumPalletHeight;
                uCtrlOptMaximumWeight.Value = new OptDouble(false, Settings.Default.MaximumPalletWeight);

                uCtrlOverhang.ValueX = Settings.Default.OverhangX;
                uCtrlOverhang.ValueY = Settings.Default.OverhangY;

                checkBoxBestLayersOnly.Checked = Settings.Default.KeepBestSolutions;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Settings.Default.MaximumPalletHeight = uCtrlMaximumHeight.Value;
            if (uCtrlOptMaximumWeight.Value.Activated)
                Settings.Default.MaximumPalletWeight = uCtrlOptMaximumWeight.Value.Value;

            Settings.Default.OverhangX = uCtrlOverhang.ValueX;
            Settings.Default.OverhangY = uCtrlOverhang.ValueY;

            Settings.Default.KeepBestSolutions = checkBoxBestLayersOnly.Checked;
        }
        #endregion

        #region FormNewAnalysis override
        public override void OnNext()
        {
            base.OnNext();
            try
            {
                List<LayerDesc> layerDescs = new List<LayerDesc>();
                foreach (ILayer2D layer2D in uCtrlLayerList.Selected)
                    layerDescs.Add(layer2D.LayerDescriptor);

                Solution.SetSolver(new LayerSolver());

                AnalysisCylinderPallet analysis = AnalysisCast;
                if (null == analysis)
                    _document.CreateNewAnalysisCylinderPallet(
                        ItemName, ItemDescription
                        , SelectedCylinderProperties, SelectedPallet
                        , new List<InterlayerProperties>()
                        , BuildConstraintSet()
                        , layerDescs
                        );
                else
                {
                    analysis.ID.SetNameDesc(ItemName, ItemDescription);
                    analysis.Content = SelectedCylinderProperties;
                    analysis.PalletProperties = SelectedPallet;
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
            if (ctrl == cbCylinders)
            {
                CylinderProperties cylProp = itemBase as CylinderProperties;
                return null != cylProp;
            }
            if (ctrl == cbPallets)
            {
                PalletProperties palletProperties = itemBase as PalletProperties;
                return null != palletProperties; 
            }
            return false;
        }
        #endregion

        #region Event handlers
        protected void onLayerSelected(object sender, EventArgs e)
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
        private void onInputChanged(object sender, EventArgs e)
        {
            try
            {
                // get cylinder / case
                CylinderProperties cylinder = cbCylinders.SelectedType as CylinderProperties;
                PalletProperties palletProperties = cbPallets.SelectedType as PalletProperties;
                if (null == cylinder || null == palletProperties)
                    return;
                // compute
                LayerSolver solver = new LayerSolver();
                List<Layer2DCyl> layers = solver.BuildLayers(
                    cylinder.RadiusOuter, cylinder.Height
                    , new Vector2D(palletProperties.Length + 2.0 * uCtrlOverhang.ValueX, palletProperties.Width + 2.0 * uCtrlOverhang.ValueY)
                    , palletProperties.Height
                    , BuildConstraintSet()
                    , checkBoxBestLayersOnly.Checked
                    );
                //  update control
                uCtrlLayerList.Packable = cylinder;
                uCtrlLayerList.ContainerHeight = uCtrlMaximumHeight.Value - palletProperties.Height;
                uCtrlLayerList.FirstLayerSelected = true;
                uCtrlLayerList.LayerList = layers.Cast<ILayer2D>().ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        protected void onCylinderChanged(object sender, EventArgs e)
        {
            uCtrlPackable.PackableProperties = cbCylinders.SelectedType as Packable;
            onInputChanged(sender, e);
            onLayerSelected(sender, e);
        }
        #endregion

        #region Private properties
        private CylinderProperties SelectedCylinderProperties
        {
            get { return cbCylinders.SelectedType as CylinderProperties; }
        }
        private PalletProperties SelectedPallet
        {
            get { return cbPallets.SelectedType as PalletProperties; }
        }
        private AnalysisCylinderPallet AnalysisCast
        {
            get { return _item as AnalysisCylinderPallet; }
        }
        #endregion

        #region Helpers
        private ConstraintSetPackablePallet BuildConstraintSet()
        {
            // constraint set
            ConstraintSetPackablePallet constraintSet = new ConstraintSetPackablePallet()
            {
                // overhang
                Overhang = new Vector2D(uCtrlOverhang.ValueX, uCtrlOverhang.ValueY)
            };
            // conditions
            constraintSet.SetMaxHeight(new OptDouble(true, uCtrlMaximumHeight.Value));
            constraintSet.OptMaxWeight = uCtrlOptMaximumWeight.Value;
            return constraintSet;
        }
        #endregion
    }
}
