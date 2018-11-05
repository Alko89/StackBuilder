using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentHAnalysisCasePallet : DockContentHAnalysis
    {
        public DockContentHAnalysisCasePallet(IDocument document, HAnalysis analysis)
            : base(document, analysis)
        {
            InitializeComponent();
        }

        public override void UpdateGrid()
        {
            base.UpdateGrid();

        }
    }
}
