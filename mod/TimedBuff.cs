using StardewValley;

namespace CrowdControl;

public class TimedBuff(string buff) : TimedEvent
{
    public Buff Buff { get; } = new(buff);

    public override void Start() => Game1.player.applyBuff(buff);

    public override void Stop() => Game1.player.buffs.Remove(buff);
}