#region Using directives
using System;
using System.Drawing;
using System.Windows.Forms;

using log4net;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;

//using treeDiM.PLMPack.DBClient;
//using treeDiM.PLMPack.DBClient.PLMPackSR;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewPalletFilm : FormNewBase
    {
        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(FormNewPalletFilm));
        #endregion

        #region Constructor
        public FormNewPalletFilm(Document doc, PalletFilmProperties item)
            : base(doc, item)
        {
            InitializeComponent();

            if (null != item)
            {
                chkbTransparency.Checked = item.UseTransparency;
                chkbHatching.Checked = item.UseHatching;
                HatchSpacing = UnitsManager.ConvertLengthFrom(item.HatchSpacing, UnitsManager.UnitSystem.UNIT_METRIC1);
                HatchAngle = item.HatchAngle;
                FilmColor = item.Color;
            }
            else
            {
                chkbTransparency.Checked = true;
                chkbHatching.Checked = true;
                HatchSpacing = UnitsManager.ConvertLengthFrom(150.0, UnitsManager.UnitSystem.UNIT_METRIC1);
                HatchAngle = 45.0;
                FilmColor = Color.LightSkyBlue;
            }
            OnChkbHatchingCheckedChanged(this, null);
            UpdateStatus(string.Empty);
            // units
            UnitsManager.AdaptUnitLabels(this);
        }
        #endregion

        #region FormNewBase overrides
        public override string ItemDefaultName
        {   get { return Resources.ID_PALLETFILM; } }
        public override void UpdateStatus(string message)
        {
            if (!UseTransparency && !UseHatching)
                message = Resources.ID_USETRANSPARENCYORHATCHING;

            base.UpdateStatus(message);
        }
        #endregion

        #region Public properties
        public Color FilmColor
        {
            get { return cbColor.Color; }
            set { cbColor.Color = value; }
        }
        public bool UseTransparency
        {
            get { return chkbTransparency.Checked; }
            set { chkbTransparency.Checked = value; }
        }
        public bool UseHatching
        {
            get { return chkbHatching.Checked; }
            set { chkbHatching.Checked = value; }
        }
        public double HatchSpacing
        {
            get { return (double)uCtrlSpacing.Value; }
            set { uCtrlSpacing.Value = value; }
        }
        public double HatchAngle
        {
            get { return (double)uCtrlAngle.Value; }
            set { uCtrlAngle.Value = value; }
        }
        #endregion

        #region Event handlers
        private void OnChkbHatchingCheckedChanged(object sender, EventArgs e)
        {
            uCtrlSpacing.Enabled = UseHatching;
            uCtrlAngle.Enabled = UseHatching;
            UpdateStatus(string.Empty);
        }
        #endregion

        #region Send to database
        private void OnSendToDatabase(object sender, EventArgs e)
        {
            try
            {
                FormSetItemName form = new FormSetItemName()
                {
                    ItemName = ItemName
                };
                if (DialogResult.OK == form.ShowDialog())
                {
                    /*using (WCFClient wcfClient = new WCFClient())
                    {
                        wcfClient.Client.CreateNewPalletFilm(new DCSBPalletFilm()
                        {
                            Name = form.ItemName,
                            Description = ItemDescription,
                            UnitSystem = (int)UnitsManager.CurrentUnitSystem,
                            UseTransparency = this.UseTransparency,
                            UseHatching = this.UseHatching,
                            HatchingSpace = HatchSpacing,
                            HatchingAngle = HatchAngle,
                            Color = FilmColor.ToArgb(),
                            AutoInsert = false
                        }
                            );
                    }*/
                    Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion
    }
}
