namespace iRacingReplayDirector
{
    internal class IracingReplayDirectorProvider : PortableSettingsProvider
    {
        public override string GetAppSettingsFilename()
        {
            return "iracing-replay-director.settings";
        }
    }

    internal class IAVMSettingsProvider : PortableSettingsProvider
    {
        public override string Name
        {
            get { return "IAVMSettingsProvider"; }
        }

        public override string GetAppSettingsFilename()
        {
            return "iracing-application-version-manager.settings";
        }
    }
}
