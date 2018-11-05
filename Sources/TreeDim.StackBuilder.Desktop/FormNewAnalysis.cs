#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewAnalysis : Form
    {
        #region Data members
        protected Document _document;
        protected Analysis _item;
        #endregion

        #region Constructors
        public FormNewAnalysis()
        {
            InitializeComponent();
        }
        public FormNewAnalysis(Document document, Analysis item)
        {
            InitializeComponent();

            _document = document;
            _item = item;
        }
        #endregion

        #region Override form
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (null != _item)
            {
                tbName.Text = _item.Name;
                tbDescription.Text = _item.Description;
            }
            else if (null != _document)
            {
                tbName.Text = _document.GetValidNewTypeName(ItemDefaultName);
                tbDescription.Text = tbName.Text;
            }

            tbDescription.Size = new Size(Size.Width - tbDescription.Location.X  - 30, tbDescription.Size.Height);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
        #endregion

        #region Properties
        public string ItemName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        public string ItemDescription
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }
        public virtual ItemBase Item { get { return _item; } }

        public virtual string ItemDefaultName { get { return "Item"; } }
        #endregion

        #region Status toolstrip updating
        public virtual void UpdateStatus(string message)
        {
            // status + message
            if (string.IsNullOrEmpty(tbName.Text))
                message = Resources.ID_FIELDNAMEEMPTY;
            else if (!_document.IsValidNewAnalysisName(tbName.Text, Item))
                message = string.Format(Resources.ID_INVALIDNAME, tbName.Text);
            // description
            else if (string.IsNullOrEmpty(tbDescription.Text))
                message = Resources.ID_FIELDDESCRIPTIONEMPTY;
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;

            bnNext.Enabled = string.IsNullOrEmpty(message);
        }
        private void onTextChanged(object sender, EventArgs e)
        {
            UpdateStatus(string.Empty);
        }
        public virtual bool IsValid
        {
            get { return false; }
        }
        public virtual void OnNext()
        {        
        }
        #endregion

        #region Public properties
        public Analysis AnalysisBase
        {
            get { return _item; }
            set { _item = value; }
        }
        #endregion

        #region Handlers
        private void onButtonNext(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            OnNext();
        }
        #endregion
    }
}
