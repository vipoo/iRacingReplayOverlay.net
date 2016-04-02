// This file is part of iRacingReplayOverlay.
//
// Copyright 2016 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net

using iRacingSDK.Support;
using System;
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

        public static Version AssemblyVersionStamp
        {
            get
            {
                var assName = Assembly.GetExecutingAssembly().GetName();
                return assName.Version;
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return AssemblyVersionStamp.ToString();
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
