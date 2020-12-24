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

            //this.cameraSwitchTimeInput.DataBindings.Add("Text", this.settings.CameraStickyPeriod, "Text");
        }

        private void button1_Click(object sender, EventArgs e)
        {

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

        private void cameraSwitchTimeInput_Enter(object sender, EventArgs e)
        {
            textBoxSettingDescription.Text = "The time period that must elapse before a new random camera is selected";
        }

        private void textBoxSettingDescription_TextChanged(object sender, EventArgs e)
        {

        }

        private void cameraSwitchTimeInput_MouseHover(object sender, EventArgs e)
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
    }
}
