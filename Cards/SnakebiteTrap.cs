using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using OriginalKnifeTrap = MegaCrit.Sts2.Core.Models.Cards.KnifeTrap;
using OriginalSnakebite = MegaCrit.Sts2.Core.Models.Cards.Snakebite;

namespace snakebite.Cards;

[Pool(typeof(SilentCardPool))]
public sealed class SnakebiteTrap : CustomCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<OriginalSnakebite>(true)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override string PortraitPath => ModelDb.Card<OriginalKnifeTrap>().PortraitPath;

    public SnakebiteTrap() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var owner = Owner;
        if (owner == null)
        {
            return;
        }

        List<CardModel> snakebites = new();
        foreach (PileType pileType in new[] { PileType.Hand, PileType.Draw, PileType.Discard, PileType.Exhaust })
        {
            snakebites.AddRange(pileType.GetPile(owner).Cards.Where((CardModel card) => card is OriginalSnakebite).ToList());
        }

        foreach (CardModel snakebite in snakebites.Distinct())
        {
            if (CombatManager.Instance.IsOverOrEnding)
            {
                break;
            }

            if (snakebite.Pile == null || snakebite.Pile.Type == PileType.None || snakebite.Pile.Type == PileType.Play)
            {
                continue;
            }

            await CardCmd.AutoPlay(choiceContext, snakebite, cardPlay.Target, AutoPlayType.Default, skipXCapture: false, skipCardPileVisuals: false);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}