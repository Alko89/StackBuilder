#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewBase : Form
    {
        #region Data members
        protected Document _document;
        protected ItemBase _item;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        public FormNewBase()
        {
            InitializeComponent();
        }
        public FormNewBase(Document document, ItemBase item)
        {
            InitializeComponent();

            _document = document;
            _item = item;
        }
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

            bnOk.Location = new Point( Size.Width - bnOk.Size.Width - 20, bnOk.Location.Y);
            bnCancel.Location = new Point(Size.Width - bnCancel.Size.Width - 20, bnCancel.Location.Y);
            tbDescription.Size = new Size(Size.Width - tbDescription.Location.X - bnCancel.Size.Width - 30, tbDescription.Size.Height);
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
            else if (!_document.IsValidNewTypeName(tbName.Text, Item))
                message = string.Format(Resources.ID_INVALIDNAME, tbName.Text);
            // description
            else if (string.IsNullOrEmpty(tbDescription.Text))
                message = Resources.ID_FIELDDESCRIPTIONEMPTY;
            bnOk.Enabled = string.IsNullOrEmpty(message);
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
        }
        private void onTextChanged(object sender, EventArgs e)
        {
            UpdateStatus(string.Empty);
        }
        #endregion
    }
}
