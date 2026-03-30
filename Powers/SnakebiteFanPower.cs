using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace snakebite.Powers;

public sealed class SnakebiteFanPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override string? CustomPackedIconPath => "res://snakebite/images/ability/snake.png";

    public override string? CustomBigIconPath => "res://snakebite/images/ability/snake.png";

    public override string? CustomBigBetaIconPath => "res://snakebite/images/ability/snake.png";
}
