using CounterStrikeSharp.API.Core;

public class MapChangePlugin : BasePlugin
{
    public override string ModuleName => "MapChangePlugin";
    public override string ModuleDescription => "A simple plugin that implements the commonly used .map command to change maps.";
    public override string ModuleAuthor => "ale";
    public override string ModuleVersion => "1.0.0";

    [ConsoleCommand("css_map", "Changes the map to the specified map.")]
    [CommandHelper(minArgs: 1, usage: "[map]", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public async Task void OnMapCommand(CCSPlayerController? caller, CommandInfo command)
    {
        commandInfo.ReplyToCommand($"Changing map to {command.Args[0]}...");
        await Task.Delay(3000);
        Server.ExecuteCommand($"changelevel {command.Args[0]}");
    }
}