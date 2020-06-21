using GitHubReleases;
using iRacingReplayDirector;

internal static class GitHubAccessSettings
{
    public static GitHubCachedReleases[] GitHubCachedReleases
    {
        get
        {
            return Settings.Default.GitHubCachedReleases;
        }
        set
        {
            Settings.Default.GitHubCachedReleases = value;
        }
    }

    public static void Save()
    {
        Settings.Default.Save();
    }
}
