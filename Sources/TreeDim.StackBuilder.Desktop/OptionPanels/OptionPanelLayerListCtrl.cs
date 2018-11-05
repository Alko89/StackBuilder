#region Using directives
using System;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class OptionPanelLayerListCtrl : GLib.Options.OptionsPanel
    {
        #region Constructor
        public OptionPanelLayerListCtrl()
        {
            InitializeComponent();
        }
        #endregion

        #region Handlers
        private void OptionsForm_OptionsSaving(object sender, EventArgs e)
        {
            Graphics.Properties.Settings.Default.LayerView3D = rbView2.Checked;
            Graphics.Properties.Settings.Default.LayerViewThumbSizeIndex = cbThumbSize.SelectedIndex;
            // save combo box
            Graphics.Properties.Settings.Default.Save();
        }
        private void OptionPanelLayerListCtrl_Load(object sender, EventArgs e)
        {
            // set radio buttons 2D/3D
            rbView1.Checked = !Graphics.Properties.Settings.Default.LayerView3D;
            rbView2.Checked = Graphics.Properties.Settings.Default.LayerView3D;
            // set size
            cbThumbSize.SelectedIndex = Graphics.Properties.Settings.Default.LayerViewThumbSizeIndex;
            // events
            OptionsForm.OptionsSaving += new EventHandler(OptionsForm_OptionsSaving);
        }
        #endregion
    }
}
