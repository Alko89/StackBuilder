﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormOptionsSettings : GLib.Options.OptionsForm
    {
        public FormOptionsSettings()
        {
            InitializeComponent();

            Panels.Add(new OptionPanelUnits());
            Panels.Add(new OptionPanelDimensions());
            Panels.Add(new OptionPanelDatabase());
            Panels.Add(new OptionPanelReporting());
            Panels.Add(new OptionPanelLayerListCtrl());
            Panels.Add(new OptionPanelDebugging());
            Panels.Add(new OptionPanelPlugins());
        }
    }
}
