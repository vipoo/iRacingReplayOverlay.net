using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iRacingSDK.Support;

namespace iRacingReplayDirector
{
    public partial class AdvancedGeneralSettingsDlg : Form
    {
        private Action onSave;
        private Settings settings;

        public AdvancedGeneralSettingsDlg(Settings settings)
        {
            this.settings = settings;
            InitializeComponent();

            this.tbCameraStickyPeriod.DataBindings.Add("Text", this.settings, "CameraStickyPeriod", true, DataSourceUpdateMode.OnPropertyChanged);
            this.tbBattleStickyPeriod.DataBindings.Add("Text", this.settings, "BattleStickyPeriod", true, DataSourceUpdateMode.OnPropertyChanged);
            this.tbBattleGap.DataBindings.Add("Text", this.settings, "BattleGap", true, DataSourceUpdateMode.OnPropertyChanged);
            this.tbHighlightVideoTargetDuration.DataBindings.Add("Text", this.settings, "HighlightVideoTargetDuration", true, DataSourceUpdateMode.OnPropertyChanged);
            this.tbFollowLeaderBeforeRaceEndPeriod.DataBindings.Add("Text", this.settings, "FollowLeaderBeforeRaceEndPeriod", true, DataSourceUpdateMode.OnPropertyChanged);
            this.tbFollowLeaderAtRaceStartPeriod.DataBindings.Add("Text", this.settings, "FollowLeaderAtRaceStartPeriod", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
      
        private void textBoxSettingDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbCameraStickyPeriod_MouseHover(object sender, EventArgs e)
        {
            textBoxSettingDescription.Text = "The time period that must elapse before a new random camera is selected";
        }

        private void AdvanceGeneralSettingsDlg_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void checkBox_PerferedDriversOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                //enable/disable groupBox Drivers depending on status of checkbox
                Boolean bChecked = ((CheckBox)sender).Checked;
                groupBox_Drivers.Enabled = bChecked;
                foreach (Control ctrl in groupBox_Drivers.Controls)
                    ctrl.Enabled = bChecked;
            }
        }

        private void tabPageTiming_Click(object sender, EventArgs e)
        {

        }

        private void tbBattleGap_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void tbCameraStickyPeriod_MouseLeave(object sender, EventArgs e)
        {
            ClearToolTipTextbox();
        }

        private void ClearToolTipTextbox()
        {
            textBoxSettingDescription.Text = "";
        }

        private void tbBattleStickyPeriod_MouseHover(object sender, EventArgs e)
        {
            textBoxSettingDescription.Text = "The time period that must elapsed before a new battle is randomly selected.";
        }

        private void tbBattleStickyPeriod_MouseLeave(object sender, EventArgs e)
        {
            ClearToolTipTextbox();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
