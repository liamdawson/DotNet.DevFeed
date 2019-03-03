**Deprecated**: Try https://github.com/loic-sharma/BaGet instead.

# dotnet dev-feed

## Usage:

You should have ready two projects: one producing the tool package, and a client one using the tool.

### One-time setup

#### In tool package project

1. Make sure `version` property in `project.json` ends with a `*` (dev-feed takes advantage of dotnet-pack `--version-suffix` option)
1. Add to `project.json`:

    ```
    "tools": {
      "Dawsonsoft.DotNet.DevFeed.Tools": {
        "version": "1.0.0-beta1-*",
        "imports": "portable-net45+win8+dnxcore50"
       }
    }`
    ```

1. `dotnet restore`
1. `dotnet dev-feed . --port 5001`

#### In client project#

1. Edit `nuget.config` to add a package source pointing to `http://localhost:5001/`
1. `dotnet restore`

### Development cycle

#### In tool package project 

1. Edit code
1. `dotnet build` (necessarily from a new shell, not from the one where *dev-feed* is still running)
1. At this point, local dev feed is up-to-date with the upgraded version, **with no need to step up version number**

#### In client project

1. `dotnet restore`
1. At this point, client project gets installed the modified version of package, **with no need to uninstall it first**
