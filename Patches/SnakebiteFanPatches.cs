using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using snakebite.Powers;

namespace snakebite.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.TargetType), MethodType.Getter)]
internal static class SnakebiteTargetTypePatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel __instance, ref TargetType __result)
    {
        if (__instance is not Snakebite snakebite)
        {
            return true;
        }

        if (snakebite.Owner?.Creature == null || !snakebite.Owner.Creature.HasPower<SnakebiteFanPower>())
        {
            return true;
        }

        __result = TargetType.AllEnemies;
        return false;
    }
}

[HarmonyPatch(typeof(Snakebite), "OnPlay")]
internal static class SnakebiteOnPlayPatch
{
    [HarmonyPrefix]
    private static bool Prefix(Snakebite __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        if (__instance.Owner?.Creature == null || !__instance.Owner.Creature.HasPower<SnakebiteFanPower>())
        {
            return true;
        }

        __result = PlayAllEnemies(__instance, choiceContext);
        return false;
    }

    private static async Task PlayAllEnemies(Snakebite snakebite, PlayerChoiceContext choiceContext)
    {
        var owner = snakebite.Owner;
        var combatState = snakebite.CombatState;
        if (owner == null || combatState == null)
        {
            return;
        }

        await CreatureCmd.TriggerAnim(owner.Creature, "Cast", owner.Character.CastAnimDelay);

        foreach (Creature enemy in combatState.HittableEnemies)
        {
            VfxCmd.PlayOnCreatureCenter(enemy, "vfx/vfx_bite");
            await PowerCmd.Apply<PoisonPower>(enemy, snakebite.DynamicVars.Poison.BaseValue, owner.Creature, snakebite);
        }
    }
}
