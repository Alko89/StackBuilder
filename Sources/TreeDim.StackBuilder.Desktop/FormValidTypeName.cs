#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormValidTypeName : Form
    {
        #region Constructor
        public FormValidTypeName()
        {
            InitializeComponent();
        }
        public FormValidTypeName(Document document)
        {
            InitializeComponent();

            _document = document;
        }
        #endregion

        #region Form overrides
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateStatus(string.Empty);

        }
        #endregion

        #region Public properties
        public string ItemName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        public DocumentSB Document
        {
            set { _document = value; }
        }
        #endregion

        #region Event handlers
        private void onNameChanged(object sender, EventArgs e)
        {
            UpdateStatus(string.Empty);
        }
        #endregion

        #region Status
        private void UpdateStatus(string message)
        {
            // status + message
            if (string.IsNullOrEmpty(tbName.Text))
                message = Resources.ID_FIELDNAMEEMPTY;
            else if (!_document.IsValidNewTypeName(tbName.Text, null))
                message = string.Format(Resources.ID_INVALIDNAME, tbName.Text);
            // status label
            statusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            statusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
            // OK button
            bnOK.Enabled = string.IsNullOrEmpty(message);
        }
        #endregion

        #region Data members
        private Document _document;
        #endregion
    }
}
