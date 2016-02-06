using iRacingSDK.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    public partial class ConfigurePlugins : Form
    {
        public struct PluginDetails
        {
            public string FriendlyName;
            public string Owner;
            public string Repo;
            public string User;

            public string Id { get { return "{0}/{1}".F(User, Repo); } }
        }

        bool hasInited = false;

        public ConfigurePlugins()
        {
            InitializeComponent();
            //twoColumnDropDown(this.pluginVersions);
            twoColumnDropDown<PluginDetails>(pluginNames, (v, i) => i == 0 ? v.FriendlyName : "by {0}".F(v.Owner));
        }

        private void ConfigurePlugins_Load(object sender, EventArgs e)
        {
            pluginNames.Items.Clear();


        }

        private void ConfigurePlugins_Activated(object sender, EventArgs e)
        {
            if (hasInited)
                return;

            hasInited = true;

            var plugins = new[]
            {
                new PluginDetails {
                    User = "vipoo",
                    Repo = "iRacingDirector.Plugin.StandardOverlays",
                    Owner = "Dean Netherton",
                    FriendlyName = "StandardOverlay"
                }
            };

            pluginNames.Items.Add(plugins[0]);

            pluginNames.SelectedItem = plugins.FirstOrDefault(p => p.Id == Settings.Default.OverlayPluginId);
        }

        void twoColumnDropDown<T>(ComboBox comboBox, Func<T, int, string> getValue)
        {
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.DrawItem += delegate (object cmb, DrawItemEventArgs args)
            {
                args.DrawBackground();

                if (args.Index == -1)
                    return;

                var obj = (T)comboBox.Items[args.Index];

                var first = getValue(obj, 0);
                var second = " " + getValue(obj, 1);

                var r1 = args.Bounds;
                r1.Width = 130;

                using (SolidBrush sb = new SolidBrush(args.ForeColor))
                    args.Graphics.DrawString(first, args.Font, sb, r1);

                using (Pen p = new Pen(Color.Black))
                    args.Graphics.DrawLine(p, r1.Right, 0, r1.Right, r1.Bottom);

                var r2 = args.Bounds;
                r2.X = r1.Width;
                r2.Width /= 2;

                using (SolidBrush sb = new SolidBrush(args.ForeColor))
                    args.Graphics.DrawString(second, args.Font, sb, r2);
            };
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if(pluginNames.SelectedItem == null)
            {
                Settings.Default.OverlayPluginId = null;
                return;
            }

            var selectedItem = (PluginDetails)pluginNames.SelectedItem;

            Settings.Default.OverlayPluginId = selectedItem.Id;
            Settings.Default.Save();
        }
    }
}
