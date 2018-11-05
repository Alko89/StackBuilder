#region Using directives
using WeifenLuo.WinFormsUI.Docking;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentStartPage : DockContent
    {
        #region constructor
        public DockContentStartPage()
        {
            InitializeComponent();
        }
        #endregion



        #region Public properties
        public System.Uri Url
        {
            get { return webBrowserStartPage.Url; }
            set { webBrowserStartPage.Url = value; }
        }
        #endregion
    }
}
