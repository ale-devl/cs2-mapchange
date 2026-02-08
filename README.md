# cs2-mapchange

Simple [CounterStrikeSharp](https://docs.cssharp.dev/) plugin that lets players change maps via chat commands.

**Warning:** There is no permission system. Any player on the server can change the map.

## Usage

- `.map de_dust2` or `!map de_dust2` — change to a specific map
- `.map inferno` — short names work too (tries `de_`, `cs_`, `ar_`, `gd_` prefixes)

## Build

```
dotnet build MapChangePlugin/MapChangePlugin.csproj --configuration Release
```

Output: `MapChangePlugin/bin/Release/net8.0/MapChangePlugin.dll`
