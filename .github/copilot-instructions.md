# Project Guidelines

## Build And Test
- Build with `dotnet build snakebite.csproj`.
- Clean with `dotnet clean snakebite.csproj`.
- Before build, set `<Sts2Dir>` in `snakebite.csproj` to a valid local Slay the Spire 2 path.
- This repository currently has no automated test project; validate behavior with in-game testing.

## Architecture
- `Cards/`: custom cards inheriting `CustomCardModel`.
- `Powers/`: custom powers inheriting `PowerModel` or `CustomPowerModel`.
- `Patches/`: Harmony patches that alter vanilla behavior.
- `snakebite/localization/zhs/`: localization text (`cards.json`, `powers.json`).
- `snakebite/images/`: card and power art assets loaded through `res://snakebite/...` paths.

## Conventions
- Every non-token custom card must declare a proper `[Pool(typeof(...))]` attribute.
- Keep card and power classes `sealed`, and keep gameplay logic in `OnPlay`/power hooks.
- When adding/updating cards or powers, update localization keys in `snakebite/localization/zhs/cards.json` and/or `snakebite/localization/zhs/powers.json`.
- For generated Snakebite cards, apply `SnakebiteCardUtils.ApplySnakebiteTimeIfActive(...)` when relevant.
- For exhaust-on-play behavior, follow `SnakebiteCardUtils.SetExhaustOnPlay(...)`.

## Common Pitfalls
- Missing `[Pool(...)]` makes cards not appear in normal rewards.
- Wrong `res://` resource path causes missing portraits/icons in game.
- For Harmony prefixes, ensure return value matches intent (`false` skips original, `true` continues original).
- Ensure localization placeholder names match dynamic vars used in code.

## References
- Developer patterns and pitfalls: `CLAUDE.md`.
- User-facing install/build usage: `README.md`.
- Card-specific implementation workflow: `.github/skills/.sts2-card-construction example/SKILL.md`.