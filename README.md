# Project Version Tool

This is a simple dotnet tool to retrieve the project version for a specified project. It supports reading the version from the `.csproj` file or props file, such as `Directory.Build.props`.

It is primarily intended to be used for CI environments where you want to create artifacts that include the project version in their identifiers (e.g. Docker image versions).

Special thanks to [@DamianEdwards](https://twitter.com/DamianEdwards) for his help figuring this out.

## Installing

The tool can be installed from Nuget, by executing:

```
dotnet tool install --global m2dev.projecttools
```

## Running

To use the tool, run:

```
dotnet project version [path_to_csproj]
```

If `path_to_csproj` is not provided, the tool will search the current directory for a valid `.csproj` file.