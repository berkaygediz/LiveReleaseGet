using System.Net.Http.Json;

using HttpClient http = new();
http.DefaultRequestHeaders.UserAgent.ParseAdd("LiveReleaseGet");

Directory.CreateDirectory("Downloads");
Console.WriteLine("Live Release Get - Berkay Gediz");

while (true)
{
    Console.WriteLine("\nHost (github.com, codeberg.org, gitea.com - default: github.com):");
    string? host = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(host)) host = "github.com";
    if (host.ToLower() == "exit") break;

    Console.WriteLine("Username/Org:");
    string? username = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(username)) continue;

    Console.WriteLine("Repo:");
    string? repo = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(repo)) continue;

    Console.WriteLine("Download source code? (1 or 0)");
    string? getSource = Console.ReadLine();

    try
    {
        Console.WriteLine("-> Fetching latest release...");
        string apiUrl = host == "github.com"
            ? $"https://api.github.com/repos/{username}/{repo}/releases/latest"
            : $"https://{host}/api/v1/repos/{username}/{repo}/releases/latest";

        var release = await http.GetFromJsonAsync<Release>(apiUrl);

        if (release?.assets != null && release.assets.Count > 0)
        {
            int index = 0;
            if (release.assets.Count > 1)
            {
                Console.WriteLine("Select file:");
                for (int i = 0; i < release.assets.Count; i++)
                    Console.WriteLine($"[{i}] {release.assets[i].name}");

                string? input = Console.ReadLine();
                if (int.TryParse(input, out int selectedIndex) && selectedIndex >= 0 && selectedIndex < release.assets.Count)
                    index = selectedIndex;
            }

            var asset = release.assets[index];
            string path = Path.Combine("Downloads", asset.name!);

            Console.WriteLine($"Downloading {asset.name}...");
            using var stream = await http.GetStreamAsync(asset.browser_download_url!);
            using var file = new FileStream(path, FileMode.Create);
            await stream.CopyToAsync(file);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Downloaded: " + path);
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("No files found in this release.");
            Console.ResetColor();
        }

        if (getSource == "1")
        {
            Console.WriteLine("Branch name (default: main):");
            string? branch = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(branch)) branch = "main";
            else if (branch.ToLower() is "main" or "master") branch = branch.ToLower();

            string branchPath = host == "github.com" ? $"refs/heads/{branch}" : branch;
            string url = $"https://{host}/{username}/{repo}/archive/{branchPath}.zip";
            string sourcePath = Path.Combine("Downloads", $"{repo}_{branch}.zip");

            Console.WriteLine($"Downloading source ({branch})...");
            try
            {
                using var sStream = await http.GetStreamAsync(url);
                using var sFile = new FileStream(sourcePath, FileMode.Create);
                await sStream.CopyToAsync(sFile);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Source downloaded: " + sourcePath);
                Console.ResetColor();
            }
            catch (HttpRequestException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: Branch '{branch}' not found.");
                Console.ResetColor();
            }
        }
    }
    catch (HttpRequestException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("ERROR: Repository or host not found.");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("ERROR: " + ex.Message);
        Console.ResetColor();
    }
}

class Release { public List<Asset>? assets { get; set; } }
class Asset { public string? name { get; set; } public string? browser_download_url { get; set; } }