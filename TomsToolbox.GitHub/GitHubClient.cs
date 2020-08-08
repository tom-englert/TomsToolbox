namespace TomsToolbox.GitHub
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Octokit;

    using TomsToolbox.Essentials;

    public static class GitHubClient
    {
        public static async Task<string?> IsUpdateAvailable(string owner, string name)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
                return null;

            var appVersion = SemanticVersion.Parse(entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            if (appVersion.Version == new Version())
                return null;

            var client = new Octokit.GitHubClient(new ProductHeaderValue(entryAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "UpdateCheck"));
            var latestRelease = (await client.Repository.Release.GetAll(owner, name))
                .OrderByDescending(r => SemanticVersion.Parse(r.TagName))
                .FirstOrDefault();

            if (SemanticVersion.Parse(latestRelease.TagName) <= appVersion)
                return null;

            return latestRelease.Assets
                .Where(asset => string.Equals(asset.Name, Path.ChangeExtension(entryAssembly.Location, ".exe"), StringComparison.OrdinalIgnoreCase))
                .Select(asset => asset.BrowserDownloadUrl)
                .FirstOrDefault();
        }
    }
}
