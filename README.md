## dotnet dev-feed

### Usage:

1. Add the tools to your project.json:

    "tools": {
        "Dawsonsoft.DotNet.DevFeed.Tools": {
          "version": "1.0.0-beta1-*",
          "imports": "portable-net45+win8+dnxcore50"
        }
    },
    
2. `dotnet restore`
3. `dotnet dev-feed . --port 5001`
