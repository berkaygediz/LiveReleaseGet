using Octokit;
using System.Net;

var github = new GitHubClient(new ProductHeaderValue("LiveReleaseGet"));

Console.WriteLine("Live Release Get - Berkay Gediz");
start:
Console.WriteLine("\nGitHub Username:");
#pragma warning disable CS8600
string username = Console.ReadLine();
#pragma warning restore CS8600

Console.WriteLine("Repo:");
#pragma warning disable CS8600
string repo = Console.ReadLine();
#pragma warning restore CS8600

Console.WriteLine($"Do you want the {username}/{repo} source code? (1 or 0)");
#pragma warning disable CS8600
string request = Console.ReadLine();
#pragma warning restore CS8600

static void SourceCodeDownload(string username, string repo, string branch)
{
    if (username != null || repo != null)
    {
        string url = $"https://github.com/{username}/{repo}/archive/{branch}.zip";
        string filename = $"{repo}_{branch}.zip";
        string filepath = Path.Combine(Environment.CurrentDirectory, filename);

        using WebClient client = new();
        try
        {
            client.DownloadFile(url, filepath);
            Console.WriteLine("Source code downloaded: " + filename);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}

try
{
    Console.WriteLine("->Download started.");
    var latestrelease = github.Repository.Release.GetLatest(username, repo).Result;

    if (latestrelease != null)
    {
        var asset = latestrelease.Assets[0];
        string api_downloadurl = asset.BrowserDownloadUrl;
        string assetname = asset.Name;
        string filepath = Path.Combine(Environment.CurrentDirectory, assetname);

        using (WebClient client = new())
        {
            client.DownloadFile(api_downloadurl, filepath);
        }

        Console.WriteLine("Downloaded: " + assetname);
        if (request == "1")
        {
            Console.WriteLine("\nEnter the branch name: (Default: master)");
#pragma warning disable CS8600
            string branch = Console.ReadLine();
#pragma warning restore CS8600

            if (string.IsNullOrEmpty(branch))
            {
                branch = "master";
            }

#pragma warning disable CS8604
            SourceCodeDownload(username, repo, branch);
#pragma warning restore CS8604
        }
    }
    else
    {
        Console.WriteLine("No releases found.");
        if (request == "1")
        {
            Console.WriteLine("\nBranch: (Default: master)");
#pragma warning disable CS8600
            string branch = Console.ReadLine();
#pragma warning restore CS8600

            if (string.IsNullOrEmpty(branch))
            {
                branch = "master";
            }
#pragma warning disable CS8604
            SourceCodeDownload(username, repo, branch);
#pragma warning restore CS8604
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: " + ex.Message);
}
goto start;
