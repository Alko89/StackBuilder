#region Using directives
// docking
using WeifenLuo.WinFormsUI.Docking;
// IView
using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class DockContentView : DockContent, IView, IItemListener
    {
        #region Data members
        /// <summary>
        /// document
        /// </summary>
        protected IDocument _document;
        #endregion

        #region Constructor
        public DockContentView()
        { 
            InitializeComponent();        
        }
        public DockContentView(IDocument document)
        {
            _document = document;

            InitializeComponent();
        }
        #endregion

        #region IItemListener implementation
        public virtual void Update(ItemBase item)
        {
        }
        public virtual void Kill(ItemBase item)
        {
            Close();
        }
        #endregion

        #region IView implementation
        public IDocument Document
        {
            get { return _document; }
        }
        #endregion
    }
}
