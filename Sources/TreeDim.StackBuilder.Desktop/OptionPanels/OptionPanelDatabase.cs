#region Using directives
using System;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class OptionPanelDatabase : GLib.Options.OptionsPanel
    {
        #region Constructor
        public OptionPanelDatabase()
        {
            InitializeComponent();
        }
        #endregion

        #region OptionPanel override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // initialize
            chkbCloseAfterImport.Checked = Properties.Settings.Default.CloseDbBrowserAfterImport;
            // register event handler
            OptionsForm.OptionsSaving += new EventHandler(OptionsForm_OptionsSaving);
        }
        #endregion

        #region Handlers
        private void OptionsForm_OptionsSaving(object sender, EventArgs e)
        {
            Properties.Settings.Default.CloseDbBrowserAfterImport = chkbCloseAfterImport.Checked;
            Properties.Settings.Default.Save();            
        }
        #endregion
    }
}
