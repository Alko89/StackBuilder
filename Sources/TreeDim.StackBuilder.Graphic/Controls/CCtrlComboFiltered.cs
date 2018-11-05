#region Using directives
using System;
using System.Windows.Forms;

using log4net;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Graphics.Controls
{
    public partial class CCtrlComboFiltered : ComboBox
    {
        public CCtrlComboFiltered()
        {
            InitializeComponent();
        }

        public void Initialize(Document doc, IItemBaseFilter filter, ItemBase initiallySelectedItem)
        {
            Items.Clear();
            try
            {
                int index = 0, iSelected = -1;
                foreach (ItemBase itemBase in doc.TypeList)
                    if (null == filter || filter.Accept(this, itemBase))
                    {
                        Items.Add(new ItemBaseWrapper(itemBase));
                        if (initiallySelectedItem == itemBase)
                            iSelected = index;
                        ++index;
                    }

                Analysis analysisInitial = null;
                if (null != initiallySelectedItem)
                {
                    if (initiallySelectedItem is PackableLoaded packableLoadedInitial)
                        analysisInitial = packableLoadedInitial.ParentAnalysis;
                }

                foreach (Analysis analysis in doc.Analyses)
                {
                    PackableLoaded eqvtPackable = analysis.EquivalentPackable;
                    if (null == eqvtPackable) continue;
                    if (null == filter || filter.Accept(this, eqvtPackable))
                    {
                        Items.Add(new ItemBaseWrapper(eqvtPackable));
                        if (analysisInitial == analysis)
                            iSelected = index;
                        ++index;
                    }
                }
                if (-1 == iSelected && Items.Count > 0)
                    SelectedIndex = 0;
                else
                    SelectedIndex = iSelected;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        public ItemBase SelectedType => !(SelectedItem is ItemBaseWrapper wrapper) ? null : wrapper.ItemBase;

        static readonly ILog _log = LogManager.GetLogger(typeof(CCtrlComboFiltered));
    }
}
