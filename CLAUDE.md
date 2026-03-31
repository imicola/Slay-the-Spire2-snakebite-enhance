# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Slay the Spire 2 (STS2)** mod written in C# that adds cards and powers themed around the "Snakebite" card. The mod uses:
- **BaseLib** - A mod framework providing `CustomCardModel`, `CustomPowerModel`, and auto-registration
- **Harmony** - For patching vanilla game behavior
- **Godot .NET SDK 4.5.1** - Build system

## Build Commands

```bash
# Build the mod (auto-copies to STS2 mods folder after build)
dotnet build snakebite.csproj

# Clean build artifacts
dotnet clean snakebite.csproj
```

The STS2 directory path must be set in `snakebite.csproj`:
```xml
<Sts2Dir>E:\SteamLibrary\steamapps\common\Slay the Spire 2</Sts2Dir>
```

Post-build automatically copies:
- `snakebite.dll` → `<Sts2Dir>/mods/snakebite/`
- `snakebite.json` → `<Sts2Dir>/mods/snakebite/`
- `snakebite.pck` → `<Sts2Dir>/mods/snakebite/` (if exists)

## Project Structure

```
snakebite/
├── Cards/           # Custom card classes (CustomCardModel)
├── Powers/          # Power/buff classes (PowerModel/CustomPowerModel)
├── Patches/         # Harmony patches for modifying vanilla behavior
└── localization/    # JSON files for card/power text (zhs = Chinese)
    └── zhs/
        ├── cards.json
        └── powers.json
snakebite/
└── images/
    ├── cards/       # Card portraits (res://snakebite/images/cards/*.png)
    └── ability/     # Power icons (res://snakebite/images/ability/*.png)
```

## Card Implementation Pattern

All cards inherit from `BaseLib.Abstracts.CustomCardModel`:

```csharp
[Pool(typeof(SilentCardPool))]  // Required: defines which card pool
public sealed class MyCard : CustomCardModel
{
    // Optional: Define variables (damage, block, etc.)
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m)];

    // Optional: Card portrait path
    public override string PortraitPath => "res://snakebite/images/cards/mycard.png";

    // Required: Cost, type, rarity, target
    public MyCard() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    // Core effect when played
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    // Upgrade logic
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
```

### Common Card Pools

- `IroncladCardPool`, `SilentCardPool`, `DefectCardPool`, `RegentCardPool`, `NecrobinderCardPool` - Character-specific
- `ColorlessCardPool` - Appearing for all characters
- `StatusCardPool`, `CurseCardPool` - Negative effects
- `TokenCardPool` - Temporary generated cards (don't put regular cards here)

### Common Commands

- `DamageCmd.Attack()` - Deal damage
- `CreatureCmd.GainBlock()` - Add block
- `PowerCmd.Apply<TPower>()` - Apply power/debuff
- `CardPileCmd.Draw()` - Draw cards
- `CardPileCmd.AddGeneratedCardsToCombat()` - Create cards
- `CardCmd.Discard()`, `CardCmd.Exhaust()` - Discard/exile cards

## Power Implementation Pattern

Powers modify behavior through hook overrides:

```csharp
public sealed class MyPower : PowerModel  // or CustomPowerModel for custom icons
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;  // or Single/Default

    // Hook examples:
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState) { }
    public override Task BeforeAttack(AttackCommand command) { }
    public override async Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw) { }
}
```

## Harmony Patches

Used in `Patches/` to modify vanilla game behavior:

```csharp
[HarmonyPatch(typeof(CardModel), nameof(CardModel.TargetType), MethodType.Getter)]
internal static class MyPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, ref TargetType __result)
    {
        // Return false to skip original, return true to continue
        __result = TargetType.AllEnemies;
        return false;
    }
}
```

## Localization

Card and power text is defined in `snakebite/localization/zhs/`:

```json
{
  "MODNAME-CLASSNAME.title": "Chinese Title",
  "MODNAME-CLASSNAME.description": "Description with {Cards:diff()} for variables"
}
```

Variable placeholders use curly braces: `{Damage}`, `{Cards:diff()}`, `{Amount}`

## Adding New Content

When adding a new card:
1. Create class in `Cards/` inheriting `CustomCardModel`
2. Add `[Pool(typeof(...))]` attribute
3. Implement `OnPlay` and `OnUpgrade`
4. Add entries to `snakebite/localization/zhs/cards.json`
5. Add portrait to `snakebite/images/cards/` (or reuse via `ModelDb.Card<OriginalCard>()`)

When adding a new power:
1. Create class in `Powers/` inheriting `PowerModel` or `CustomPowerModel`
2. Override relevant hooks
3. Add entries to `snakebite/localization/zhs/powers.json`
4. Add icon to `snakebite/images/ability/` (if `CustomPowerModel`)

## Mod Metadata

`snakebite.json` defines mod info:
```json
{
  "id": "snakebite",
  "name": "Snakebite",
  "dependencies": ["BaseLib"],
  "has_pck": true,
  "has_dll": true
}
```

## Common Pitfalls

1. **Missing `[Pool]` attribute** - Card won't appear in game
2. **Wrong pool type** - Putting reward cards in Token pool prevents them from appearing
3. **Null `cardPlay.Target`** - Always check for null with `AnyEnemy` cards
4. **Variable name mismatch** - `CanonicalVars` must match description placeholders
5. **Forgetting to copy resources** - .pck file must be built or images won't load
