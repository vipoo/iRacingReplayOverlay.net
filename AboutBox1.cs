// This file is part of iRacingReplayOverlay.
//
// Copyright 2016 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net

using iRacingSDK.Support;
using System.Reflection;
using System.Windows.Forms;

namespace iRacingReplayOverlay
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = "About {0}".F(AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = "Version {0}".F(AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.textBoxDescription.Text = AssemblyDescription;
            this.trackingId.Text = Settings.Default.TrackingID;
        }

        public string AssemblyTitle
        {
            get
            {
                return GetCustomAttribute<AssemblyTitleAttribute>().Title;
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                var assName = Assembly.GetExecutingAssembly().GetName();
                var version = assName.Version;
                var name = assName.Name;

                var isBeta = name.ToLower().Contains("beta");
                var isTest = name.ToLower().Contains("test");

                var betaText = " stable";
                if (isBeta)
                    betaText = " beta";
                if (isTest)
                    betaText = " test";

                return "{0}.{1}.{2}.{3}{4}".F(version.Major, version.MajorRevision, version.Minor, version.MinorRevision, betaText);
            }
        }

        public string AssemblyDescription
        {
            get
            {
                return GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                return GetCustomAttribute<AssemblyProductAttribute>().Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                return GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            }
        }

        private static T GetCustomAttribute<T>()
        {
            return (T)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false)[0];
        }
    }
}
