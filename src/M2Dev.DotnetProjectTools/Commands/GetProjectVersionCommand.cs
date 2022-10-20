using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;

namespace M2Dev.DotnetProjectTools.Commands;

[Command("version")]
class GetProjectVersionCommand : ProjectToolsCommandBase
{
    [Argument(1, "ProjectFilePath", "Optional: Path to the csproj file to query. If not provided, will search current directory.")]
    public string? ProjectFilePath { get; set; }

    protected override int OnExecute(CommandLineApplication app)
    {
        string? projectFilePath = null;

        if (!string.IsNullOrEmpty(ProjectFilePath))
        {
            projectFilePath = ProjectFilePath;
            if (!File.Exists(projectFilePath))
            {
                WriteError($"The specified project file '{projectFilePath}' was not found.");
                Environment.Exit(1);
            }
        }
        else
        {
            var projectFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.csproj");
            if (projectFiles.Length == 1)
            {
                projectFilePath = projectFiles[0];
            }
            else if (projectFiles.Length > 1)
            {
                WriteError("More than one .csproj file found. Please specify a project file via dotnet version MyProject.csproj");
                Environment.Exit(1);
            }
            else
            {
                WriteError("No project file found. Run this command in the project directory or specify the path to a project file.");
                Environment.Exit(1);
            }
        }

        var configuration = "Debug";
        var targetsFile = FindTargetsFile();
        if (targetsFile is null)
        {
            Environment.Exit(1);
        }

        var outputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        Console.WriteLine(outputFile);

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                ArgumentList =
                {
                    "msbuild",
                    projectFilePath,
                    "/nologo",
                    "/t:_ExtractVersionMetadata", // defined in GetProjectVersion.targets
                    "/p:_ProjectVersionMetadataFile=" + outputFile,
                    "/p:Configuration=" + configuration,
                    "/p:CustomAfterMicrosoftCommonTargets=" + targetsFile,
                    "/p:CustomAfterMicrosoftCommonCrossTargetingTargets=" + targetsFile,
                    "-verbosity:detailed",
                }
            };

            using var process = new Process
            {
                StartInfo = psi,
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                WriteError($"Exit code: {process.ExitCode}");
            }

            if (!File.Exists(outputFile))
            {
                WriteError("Output file not found");
                Environment.Exit(1);
            }

            var version = File.ReadAllText(outputFile)?.Trim();
            if (string.IsNullOrEmpty(version))
            {
                WriteError("Output file was empty");
            }

            Console.WriteLine($"Project version: {version}");
        }
        catch(Exception ex)
        {
            WriteError(ex.ToString());
            Environment.Exit(1);
        }

        return base.OnExecute(app);
    }

    void WriteError(string message)
    {
        WriteLine(message, ConsoleColor.Red);
    }

    void WriteLine(string message, ConsoleColor color)
    {
        Write(message, color);
        Console.WriteLine();
    }

    void Write(string message, ConsoleColor color)
    {
        var origColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ForegroundColor = origColor;
    }

    string? FindTargetsFile()
    {
        var assemblyDir = Path.GetDirectoryName(typeof(ProjectTools).Assembly.Location);
        var searchPaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "assets"),
            Path.Combine(assemblyDir, "assets"),
            AppContext.BaseDirectory,
            assemblyDir,
        };

        var targetPath = searchPaths.Select(x => Path.Combine(x, "GetProjectVersion.targets")).FirstOrDefault(File.Exists);
        if (targetPath == null)
        {
            WriteError("Fatal error: could not find GetProjectVersion.targets");
            return null;
        }

        return targetPath;
    }

    public override List<string> CreateArgs() => new List<string>();
}