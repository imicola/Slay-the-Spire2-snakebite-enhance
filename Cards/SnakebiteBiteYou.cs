using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using OriginalBelieveInYou = MegaCrit.Sts2.Core.Models.Cards.BelieveInYou;
using OriginalSnakebite = MegaCrit.Sts2.Core.Models.Cards.Snakebite;

namespace snakebite.Cards;

[Pool(typeof(SilentCardPool))]
public sealed class SnakebiteBiteYou : CustomCardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<OriginalSnakebite>(IsUpgraded)];

        public override string PortraitPath => "res://snakebite/images/cards/snakebite_you.png";

    public SnakebiteBiteYou() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        ArgumentNullException.ThrowIfNull(cardPlay.Target.Player);

        var combatState = CombatState;
        if (combatState == null)
        {
            return;
        }

        var targetPlayer = cardPlay.Target.Player;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        List<CardModel> generatedCards = new(2);
        for (int i = 0; i < 2; i++)
        {
            CardModel snakebite = combatState.CreateCard<OriginalSnakebite>(targetPlayer);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(snakebite);
            }

            SnakebiteCardUtils.ApplySnakebiteTimeIfActive(snakebite);
            generatedCards.Add(snakebite);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(generatedCards, PileType.Hand, addedByPlayer: true);
    }
}
