﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using treeDiM.StackBuilder;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class OptionPanelDimensions : GLib.Options.OptionsPanel
    {
        #region Constructor
        public OptionPanelDimensions()
        {
            InitializeComponent();

            CategoryPath = Properties.Resources.ID_OPTIONSDIMENSIONS;
            DisplayName = Properties.Resources.ID_DISPLAYDIMENSIONSCASEPALLET;

            // initialize combo box
            cbDim1.SelectedIndex = Graphics.Properties.Settings.Default.DimCasePalletSol1;
            cbDim2.SelectedIndex = Graphics.Properties.Settings.Default.DimCasePalletSol2;
        }
        void OptionsForm_OptionsSaving(object sender, EventArgs e)
        {
            // save combo box
            Graphics.Properties.Settings.Default.DimCasePalletSol1 = cbDim1.SelectedIndex;
            Graphics.Properties.Settings.Default.DimCasePalletSol2 = cbDim2.SelectedIndex;

            Graphics.Properties.Settings.Default.Save();
        }
        #endregion

        #region Handlers
        private void OptionPanelDimensions_Load(object sender, EventArgs e)
        {
            // events
            OptionsForm.OptionsSaving += new EventHandler(OptionsForm_OptionsSaving);
        }
        #endregion
    }
}
