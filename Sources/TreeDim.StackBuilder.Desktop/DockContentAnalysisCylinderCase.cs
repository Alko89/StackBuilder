#region Using directives

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentAnalysisCylinderCase : DockContentAnalysisEdit
    {
        #region Constructor
        public DockContentAnalysisCylinderCase(IDocument document, AnalysisCylinderCase analysis)
            : base(document, analysis)
        {
            InitializeComponent();
        }
        #endregion

        #region Override DockContentAnalysisEdit
        public override string GridCaption
        { get { return Resources.ID_CASE; } }
        #endregion
    }
}
