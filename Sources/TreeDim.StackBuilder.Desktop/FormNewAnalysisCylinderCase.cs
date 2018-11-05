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
    public partial class FormNewAnalysisCylinderCase
        : FormNewAnalysis, IItemBaseFilter
    {
        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewAnalysisCylinderCase));
        #endregion

        #region Constructor
        public FormNewAnalysisCylinderCase(Document doc, Analysis analysis)
            : base(doc, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region FormNewBase overrides
        public override string ItemDefaultName
        {   get { return Resources.ID_ANALYSIS; } }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbCylinders.Initialize(_document, this, null != AnalysisCast ? AnalysisCast.Content : null);
            cbCases.Initialize(_document, this, null != AnalysisCast ? AnalysisCast.CaseProperties : null);

            // event handling
            uCtrlLayerList.LayerSelected += onLayerSelected;
            uCtrlLayerList.RefreshFinished += onLayerSelected;
            uCtrlLayerList.ButtonSizes = new Size(100, 100);

            if (null == AnalysisBase)
            {             
            }
            checkBoxBestLayersOnly.Checked = Settings.Default.KeepBestSolutions;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
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
                foreach (ILayer2D layer in uCtrlLayerList.Selected)
                    layerDescs.Add(layer.LayerDescriptor);

                Solution.SetSolver(new LayerSolver());

                AnalysisCylinderCase analysis = _item as AnalysisCylinderCase;

                if (null == analysis)
                {
                    _item = _document.CreateNewAnalysisCylinderCase(
                        ItemName, ItemDescription
                        , SelectedCylinder, SelectedCase
                        , new List<InterlayerProperties>()
                        , BuildConstraintSet()
                        , layerDescs);
                }
                else
                {
                    analysis.ID.SetNameDesc(ItemName, ItemDescription);
                    analysis.Content = SelectedCylinder;
                    analysis.CaseProperties = SelectedCase;
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
                return itemBase is CylinderProperties;
            }
            else if (ctrl == cbCases)
            {
                PackableBrick packable = itemBase as PackableBrick;
                return (null != packable) && (packable.IsCase);
            }
            return false;
        }
        #endregion

        #region Event handlers
        private void onLayerSelected(object sender, EventArgs e)
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
                BoxProperties caseProperties = cbCases.SelectedType as BoxProperties;
                if (null == cylinder || null == caseProperties)
                    return;
                // compute
                LayerSolver solver = new LayerSolver();
                List<Layer2DCyl> layers = solver.BuildLayers(
                    cylinder.RadiusOuter, cylinder.Height
                    , new Vector2D(caseProperties.InsideLength, caseProperties.InsideWidth)
                    , 0.0
                    , BuildConstraintSet()
                    , checkBoxBestLayersOnly.Checked
                    );
                uCtrlLayerList.Packable = cylinder;
                uCtrlLayerList.ContainerHeight = caseProperties.InsideHeight;
                uCtrlLayerList.FirstLayerSelected = true;
                uCtrlLayerList.LayerList = layers.Cast<ILayer2D>().ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        private void onCylinderChanged(object sender, EventArgs e)
        {
            uCtrlPackable.PackableProperties = cbCylinders.SelectedType as Packable;
            onInputChanged(sender, e);
            onLayerSelected(sender, e);
        }
        #endregion

        #region Private properties
        private CylinderProperties SelectedCylinder
        {
            get { return cbCylinders.SelectedType as CylinderProperties; }
        }
        private BoxProperties SelectedCase
        {
            get { return cbCases.SelectedType as BoxProperties; }
        }
        private AnalysisCylinderCase AnalysisCast
        {
            get { return _item as AnalysisCylinderCase;  }
        }
        #endregion

        #region Helpers
        private ConstraintSetCylinderCase BuildConstraintSet()
        {
            ConstraintSetCylinderCase constraintSet = new ConstraintSetCylinderCase(SelectedCase);
            return constraintSet;
        }
        #endregion
    }
}
