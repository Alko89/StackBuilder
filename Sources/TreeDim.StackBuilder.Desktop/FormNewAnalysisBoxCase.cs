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
    public partial class FormNewAnalysisBoxCase : FormNewAnalysis, IItemBaseFilter
    {
        #region Constructor
        public FormNewAnalysisBoxCase(Document doc, Analysis analysis)
            : base(doc, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region FormNewBase overrides
        public override string ItemDefaultName
        { get { return Resources.ID_ANALYSIS; } }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbBoxes.Initialize(_document, this, AnalysisCast?.Content);
            cbCases.Initialize(_document, this, AnalysisCast?.Container);

            // event handling
            uCtrlLayerList.LayerSelected += OnLayerSelected;
            uCtrlLayerList.RefreshFinished += OnLayerSelected;
            uCtrlLayerList.ButtonSizes = new Size(100, 100);

            if (null == AnalysisBase)
            {
                uCtrlCaseOrientation.AllowedOrientations = new bool[]
                { Settings.Default.AllowVerticalX, Settings.Default.AllowVerticalY, Settings.Default.AllowVerticalZ };
            }
            checkBoxBestLayersOnly.Checked = Settings.Default.KeepBestSolutions;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Settings.Default.AllowVerticalX = uCtrlCaseOrientation.AllowedOrientations[0];
            Settings.Default.AllowVerticalY = uCtrlCaseOrientation.AllowedOrientations[1];
            Settings.Default.AllowVerticalZ = uCtrlCaseOrientation.AllowedOrientations[2];

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

                AnalysisBoxCase analysis = AnalysisCast;
                if (null == analysis)
                    AnalysisBase = _document.CreateNewAnalysisBoxCase(
                        ItemName, ItemDescription
                        , SelectedBoxProperties, SelectedCaseProperties
                        , new List<InterlayerProperties>()
                        , BuildConstraintSet()
                        , layerDescs
                        );
                else
                {
                    analysis.ID.SetNameDesc(ItemName, ItemDescription);
                    analysis.Content = SelectedBoxProperties;
                    analysis.CaseProperties = SelectedCaseProperties;
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
            if (ctrl == cbBoxes)
            {
                PackableBrick packable = itemBase as PackableBrick;
                return null != packable
                    && (
                    (packable is BoxProperties)
                    || (packable is BundleProperties)
                    || (packable is PackProperties)
                    || (packable is LoadedCase)
                    );
            }
            else if (ctrl == cbCases)
            {
                Packable packable = itemBase as Packable;
                return null != packable
                    && packable.IsCase
                    && (packable is BoxProperties)
                    && (packable != SelectedBoxProperties);
            }
            return false;
        }
        #endregion

        #region Private properties
        private Packable SelectedBoxProperties
        {
            get { return cbBoxes.SelectedType as Packable; }
        }
        private BoxProperties SelectedCaseProperties
        {
            get { return cbCases.SelectedType as BoxProperties; }
        }
        private AnalysisBoxCase AnalysisCast
        {
            get { return _item as AnalysisBoxCase; }
        }
        #endregion

        #region Event handlers
        private void OnBoxChanged(object sender, EventArgs e)
        {
            cbCases.Initialize(_document, this, null);
            OnInputChanged(sender, e);
        }

        private void OnInputChanged(object sender, EventArgs e)
        {
            try
            {
                // get box / case
                PackableBrick packable = cbBoxes.SelectedType as PackableBrick;
                BoxProperties caseProperties = cbCases.SelectedType as BoxProperties;
                if (null == packable || null == caseProperties)
                    return;

                // update orientation control
                uCtrlCaseOrientation.BProperties = packable;

                // compute
                LayerSolver solver = new LayerSolver();
                List<Layer2D> layers = solver.BuildLayers(
                    packable.OuterDimensions
                    , new Vector2D(caseProperties.InsideLength, caseProperties.InsideWidth)
                    , 0.0 /* offsetZ */
                    , BuildConstraintSet()
                    , checkBoxBestLayersOnly.Checked
                    );
                // update control
                uCtrlLayerList.Packable = packable;
                uCtrlLayerList.ContainerHeight = caseProperties.InsideHeight;
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
        private ConstraintSetBoxCase BuildConstraintSet()
        { 
            // constraint set
            ConstraintSetBoxCase constraintSet = new ConstraintSetBoxCase(SelectedCaseProperties);
            constraintSet.SetAllowedOrientations(uCtrlCaseOrientation.AllowedOrientations);
            return constraintSet;        
        }
        #endregion

        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisBoxCase));
        #endregion
    }
}
