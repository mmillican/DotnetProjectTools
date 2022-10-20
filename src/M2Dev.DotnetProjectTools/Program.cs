using System.Diagnostics;
using M2Dev.DotnetProjectTools.Commands;
using McMaster.Extensions.CommandLineUtils;

[Command]
[Subcommand(
    typeof(GetProjectVersionCommand)
)]
class ProjectTools : ProjectToolsCommandBase
{
    public static void Main(string[] args) => CommandLineApplication.Execute<ProjectTools>(args);

    protected override int OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
        return 1;
    }

    public override List<string> CreateArgs() => new List<string>();    
}
