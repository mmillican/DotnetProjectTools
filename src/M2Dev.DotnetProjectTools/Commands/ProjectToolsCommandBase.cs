using McMaster.Extensions.CommandLineUtils;

namespace M2Dev.DotnetProjectTools.Commands;

[HelpOption("--help")]
abstract class ProjectToolsCommandBase
{
    public abstract List<string> CreateArgs();

    protected virtual int OnExecute(CommandLineApplication app)
    {
        var args = CreateArgs();
        return 0;
    }
}