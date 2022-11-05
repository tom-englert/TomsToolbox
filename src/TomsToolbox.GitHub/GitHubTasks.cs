namespace TomsToolbox.GitHub;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Octokit;

using TomsToolbox.Essentials;

public static class GitHubTasks
{
    public static async Task<string?> FindUpdate(string owner, string name)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
            return null;

        var appVersion = SemanticVersion.Parse(entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
        if (appVersion.Version == new Version())
            return null;

        var productHeaderValue = new ProductHeaderValue(entryAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "UpdateCheck");

        var connection = new Connection(productHeaderValue);
        var apiConnection = new ApiConnection(connection);
        var client = new ReleasesClient(apiConnection);

        var latestRelease = (await client.GetAll(owner, name))
            .OrderByDescending(r => SemanticVersion.Parse(r.TagName))
            .FirstOrDefault();

        if (SemanticVersion.Parse(latestRelease?.TagName) <= appVersion)
            return null;

        return latestRelease?.Assets
            .Where(asset => string.Equals(asset.Name, Path.ChangeExtension(Path.GetFileName(entryAssembly.Location), ".exe"), StringComparison.OrdinalIgnoreCase))
            .Select(asset => asset.BrowserDownloadUrl)
            .FirstOrDefault();
    }
}
