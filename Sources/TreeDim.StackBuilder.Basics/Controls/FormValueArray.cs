#region Using directives
using System.Collections.Generic;
using System.Windows.Forms;
#endregion

namespace treeDiM.StackBuilder.Basics.Controls
{
    public partial class FormValueArray : Form
    {
        public FormValueArray(List<double> abscissa)
        {
            ValueArray = abscissa;
            InitializeComponent();
        }

        public List<double> ValueArray { get; set; }
    }
}
