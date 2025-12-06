# Gender Diversity

A simple Mount & Blade II: Bannerlord mod that allows women to be recruited and trained as soldiers throughout Calradia.

## Features

- **Configurable percentage**: Set the chance (0-100%) for troops to spawn as female
- **Lore-friendly exceptions**: Optionally keep historically male-only units (Skolderbroda, Ghilman) as male
- **No new troops added**: Works with existing troop trees, compatible with other mods
- **Safe to add/remove**: Does not modify save files

## Requirements

- Bannerlord 1.2.x or later
- [Harmony](https://www.nexusmods.com/mountandblade2bannerlord/mods/2006)
- [ButterLib](https://www.nexusmods.com/mountandblade2bannerlord/mods/2018)
- [Mod Configuration Menu (MCM)](https://www.nexusmods.com/mountandblade2bannerlord/mods/612)

## Installation

1. Download the latest release
2. Extract to your `Modules` folder
3. Enable in the launcher (load after MCM)

## Configuration

Open Mod Options in-game to configure:

- **Enable Gender Diversity**: Toggle the mod on/off
- **Female Troop Percentage**: 0-100% chance for troops to be female (default: 50%)
- **Lore-Friendly Exceptions**: Keep certain units male-only (default: enabled)

## Compatibility

- Compatible with War Sails DLC
- Compatible with troop overhaul mods
- May conflict with other gender modification mods

## Building from Source

```bash
dotnet restore
dotnet build --configuration Release
```

## License

MIT License
