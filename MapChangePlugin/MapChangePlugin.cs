using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Events;
using Microsoft.Extensions.Logging;

namespace MapChangePlugin;
public class MapChangePlugin : BasePlugin
{
    public override string ModuleName => "MapChangePlugin";

    public override string ModuleVersion => "1.0.0";

    private DateTime _lastMapChange = DateTime.MinValue;
    private const int CooldownSeconds = 5;

    private static readonly Regex MapCommandRegex = new(@"^[.!]map(?:\s+(\S+))?$", RegexOptions.Compiled);
    private static readonly Regex ValidMapName = new(@"^[\w\-]+$", RegexOptions.Compiled);
    private static readonly string[] MapPrefixes = ["de_", "cs_", "ar_", "gd_"];

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("MapChangePlugin loaded!");
    }

    [GameEventHandler]
    public HookResult OnPlayerChat(EventPlayerChat @event, GameEventInfo info) {
        var player = Utilities.GetPlayerFromUserid(@event.Userid);
        if (player == null || !player.IsValid) return HookResult.Continue;

        string text = @event.Text;
        var match = MapCommandRegex.Match(text);
        if (!match.Success) return HookResult.Continue;

        if (!match.Groups[1].Success) {
            player.PrintToChat($" \x06Usage: .map <mapname>");
            return HookResult.Continue;
        }

        string mapInput = match.Groups[1].Value;

        if (!ValidMapName.IsMatch(mapInput)) {
            player.PrintToChat($" \x02Invalid map name.");
            return HookResult.Continue;
        }

        // Check cooldown
        var timeSinceLastChange = DateTime.Now - _lastMapChange;
        if (timeSinceLastChange.TotalSeconds < CooldownSeconds) {
            int remaining = (int)(CooldownSeconds - timeSinceLastChange.TotalSeconds);
            player.PrintToChat($" \x02Please wait {remaining} seconds before changing map again.");
            return HookResult.Continue;
        }

        // Resolve and change map
        string? mapName = ResolveMapName(mapInput);
        if (mapName != null) {
            string playerName = player.PlayerName;
            Server.PrintToChatAll($" \x06{playerName} \x01is changing map to \x0C{mapName}\x01...");
            Logger.LogInformation("[MapChange] {PlayerName} changed map to {MapName}", playerName, mapName);
            _lastMapChange = DateTime.Now;
            Server.ExecuteCommand($"changelevel {mapName}");
        } else {
            player.PrintToChat($" \x02Map '{mapInput}' not found!");
        }

        return HookResult.Continue;
    }

    private static string? ResolveMapName(string input)
    {
        if (Server.IsMapValid(input))
            return input;

        foreach (var prefix in MapPrefixes)
        {
            string candidate = prefix + input;
            if (Server.IsMapValid(candidate))
                return candidate;
        }

        return null;
    }
}
