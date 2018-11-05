#region Using directives
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentLogConsole : DockContent
    {
        #region Constructor
        public DockContentLogConsole()
        {
            InitializeComponent();
        }
        #endregion

        #region Set rich text box to RichTextBoxAppender
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
           log4net.Appender.RichTextBoxAppender.SetRichTextBox(richTextBoxLog, "RichTextBoxAppender");
        }
        #endregion

        #region Menu event handlers
        private void FloatingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockState = DockState.Float;
        }

        private void DockableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockState = ShowHint;
        }

        private void TabbedDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockState = DockState.Document;
        }

        private void AutoHideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (DockState)
            {
                case DockState.DockBottom:
                    DockState = DockState.DockBottomAutoHide;
                    break;
                case DockState.DockTop:
                    DockState = DockState.DockTopAutoHide;
                    break;
                case DockState.DockLeft:
                    DockState = DockState.DockLeftAutoHide;
                    break;
                case DockState.DockRight:
                    DockState = DockState.DockRightAutoHide;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Public properties
        public RichTextBox RichTextBox
        {
            get { return richTextBoxLog; }
        }
        #endregion
    }
}
