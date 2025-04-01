using System;
using StardewModdingAPI;
using StardewValley;

namespace CrowdControl;

public class TimedBuff(string buff, SITimeSpan duration) : TimedEvent
{
    public Buff Buff { get; } = new(buff) { millisecondsDuration = (int)duration.TotalMilliseconds };

    public override void Start() => Game1.player.applyBuff(Buff);

    /*public override void Tick()
    {
        if (!Game1.player.hasBuff(buff)) Game1.player.applyBuff(Buff);
    }*/

    public override void Stop() => Game1.player.buffs.Remove(Buff.id);
}