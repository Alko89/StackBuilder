#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using log4net;

//using treeDiM.PLMPack.DBClient;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewDocument : Form
    {
        #region Constructor
        public FormNewDocument()
        {
            InitializeComponent();

            tbDateCreated.Text = string.Format("{0}", DateTime.Now);
            OnDocumentNameChanged(this, null);
        }
        #endregion

        #region Public properties
        public string DocName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        public string DocDescription
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }
        public string Author
        {
            get { return tbAuthor.Text; }
            set { tbAuthor.Text = value; }
        }
        public DateTime DateCreated
        {
            get { return Convert.ToDateTime(tbDateCreated.Text); }
            set { tbDateCreated.Text = string.Format("{0}", value); }
        }
        public string UserName
        {
            get
            {
                try
                {
                    /*using (WCFClient wcfClient = new WCFClient())
                    { return wcfClient.User.Name; }*/
                    return "Infotrans";
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                    return string.Empty;
                }
            }
        }
        #endregion

        #region Event handlers
        private void OnDocumentNameChanged(object sender, EventArgs e)
        {
            string message = string.Empty;
            // check name
            if (string.IsNullOrEmpty(tbName.Text))
                message = Resources.ID_FIELDNAMEEMPTY;

            // update button OK
            bnOk.Enabled = string.IsNullOrEmpty(message);
            // update status bar
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
        }
        #endregion

        #region Load / FormClosing event
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // author
            Author = string.IsNullOrEmpty(Settings.Default.DocumentAuthor) ? UserName : Settings.Default.DocumentAuthor;
            // windows settings
            if (null != Settings.Default.FormNewDocumentPosition)
                Settings.Default.FormNewDocumentPosition.Restore(this);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // author
            Settings.Default.DocumentAuthor = Author;
            // window position
            if (null == Settings.Default.FormNewDocumentPosition)
                Settings.Default.FormNewDocumentPosition = new WindowSettings();
            Settings.Default.FormNewDocumentPosition.Record(this);
        }
        #endregion

        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(FormNewDocument));
        #endregion
    }
}