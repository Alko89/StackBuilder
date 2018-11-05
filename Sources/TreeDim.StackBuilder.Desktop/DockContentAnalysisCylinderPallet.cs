#region Using directives

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisCylinderPallet : DockContentAnalysisEdit
    {
        #region Constructor
        public DockContentAnalysisCylinderPallet(IDocument document, AnalysisCylinderPallet analysis)
            : base(document, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region Override DockContentAnalysisEdit
        public override string GridCaption
        {   get { return Properties.Resources.ID_PALLET; } }
        #endregion
    }
}
