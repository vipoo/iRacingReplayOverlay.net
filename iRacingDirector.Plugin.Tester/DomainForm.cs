using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace iRacingDirector.Plugin.Tester
{
    public class DomainForm
    {
        RemoteImageViewer frm;
        AppDomain domain;

        string pluginPath;
        string sessionDataPath;

        public static DomainForm CreateRemote()
        {
            return new DomainForm();
        }

        private DomainForm()
        {
        }

        private void Create(bool recreate = false)
        {
            if (domain != null && !recreate)
                return;

            frm = null;

            if (domain != null)
            {
                AppDomain.Unload(domain);
                domain = null;
            }

            var myLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var info = new AppDomainSetup
            {
                ShadowCopyFiles = "true"
            };

            domain = AppDomain.CreateDomain("TranscodingDomain", null, info);
            frm = (RemoteImageViewer)domain.CreateInstanceFromAndUnwrap(typeof(RemoteImageViewer).Assembly.Location, typeof(RemoteImageViewer).FullName);
        }

        public void SetOnError(Action<string, string> onError)
        {
            Create();

            frm.SetOnError(onError);
        }

        public void SetPluginFileName(string pluginPath)
        {
            Create(true);

            this.pluginPath = pluginPath;
            frm.InitPlugin(pluginPath, sessionDataPath);
            frm.Refresh();
        }

        public void SetBackgroundImage(string fileName)
        {
            Create();

            frm.SetBackgroundImage(fileName);
            frm.Show();
            frm.Activate();
        }

        internal void SetClientSize(Size size)
        {
            Create();

            frm.ClientSize = size;
            frm.Show();
            frm.Activate();
        }

        internal void SetPosition(int left, int top)
        {
            Create();

            frm.SetPosition(left, top);
        }

        internal void Recreate()
        {
            Create(true);
        }

        internal void SetSessionDataFileName(string sessionDataPath)
        {
            Create();
            this.sessionDataPath = sessionDataPath;
            if( pluginPath != null)
                frm.InitPlugin(pluginPath, sessionDataPath);
        }
    }
}
