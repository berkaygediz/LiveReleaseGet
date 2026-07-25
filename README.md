# LiveReleaseGet

CLI tool to fetch and download the latest release assets and source code from GitHub, Codeberg, and Gitea repositories. Files are saved to a local Downloads folder.

## Build

Create a single-file executable using the .NET 10 SDK:

```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true
dotnet publish -c Release -r win-arm64 -p:PublishSingleFile=true
```

Add `--self-contained true` for standalone executables that run without .NET installed.