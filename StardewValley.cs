using System;
using System.Collections.Generic;
using CrowdControl.Common;
using CrowdControl.Games.Packs;
using ConnectorType = CrowdControl.Common.ConnectorType;

public class StardewValley : SimpleTCPPack
{
    public override string Host => "127.0.0.1";

    public override ushort Port => 51337;

    public StardewValley(IPlayer player, Func<CrowdControlBlock, bool> responseHandler, Action<Object> statusUpdateHandler)
        : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new(94, "Stardew Valley", "StardewValley", "PC", ConnectorType.SimpleTCPConnector);

    public override List<Effect> Effects { get; } = new()
    {
        new Effect("Downgrade Items", "downgrade", ItemKind.Folder),
            // 1.3.0
            new Effect("Downgrade the Player's Backpack", "downgrade_backpack"),
            // 1.2.0
            new Effect("Downgrade the Player's Axe", "downgrade_axe", "downgrade"),
            new Effect("Downgrade the Player's Boots", "downgrade_boots", "downgrade"),
            new Effect("Downgrade the Player's Fishing Rod", "downgrade_fishingrod", "downgrade"),
            new Effect("Downgrade the Player's Hoe", "downgrade_hoe", "downgrade"),
            new Effect("Downgrade the Player's Pickaxe", "downgrade_pickaxe", "downgrade"),
            new Effect("Downgrade the Player's Trash Can", "downgrade_trashcan", "downgrade"),
            new Effect("Downgrade the Player's Watering Can", "downgrade_wateringcan", "downgrade"),
            new Effect("Downgrade the Player's Weapon", "downgrade_weapon", "downgrade"),

        new Effect("Status Effects", "status", ItemKind.Folder),
            new Effect("Adrenaline Rush (30 seconds)", "give_buff_adrenaline", "status"),
            new Effect("Darkness (30 seconds)", "give_buff_darkness", "status"),
            new Effect("Frozen (10 seconds)", "give_buff_frozen", "status"),
            new Effect("Invincibility (30 seconds)", "give_buff_invincibility", "status"),
            new Effect("Nauseous (1 minute)", "give_buff_nauseous", "status"),
            new Effect("Slimed effect (10 seconds)", "give_buff_slime", "status"),
            new Effect("Speed Buff (2 minutes)", "give_buff_speed", "status"),
            new Effect("Tipsy (2 minutes)", "give_buff_tipsy", "status"),
            new Effect("Warrior Energy (30 seconds)", "give_buff_warrior", "status"),

        new Effect("Give/Take Life", "life", ItemKind.Folder),
            new Effect("Heal the Player by 10%", "heal_10", "life"),
            new Effect("Heal the Player by 25%", "heal_25", "life"),
            new Effect("Heal the Player by 50%", "heal_50", "life"),
            new Effect("Fully heal the Player", "heal_full", "life"),
            new Effect("Hurt the Player by 10%", "hurt_10", "life"),
            new Effect("Hurt the Player by 25%", "hurt_25", "life"),
            new Effect("Hurt the Player by 50%", "hurt_50", "life"),
            new Effect("Kill the Player", "kill", "life"),

        new Effect("Give/Take Energy", "energy", ItemKind.Folder),
            new Effect("Energized the Player by 10%", "energize_10", "energy"),
            new Effect("Energized the Player by 25%", "energize_25", "energy"),
            new Effect("Energized the Player by 50%", "energize_50", "energy"),
            new Effect("Fully energize the Player", "energize_full", "energy"),

            new Effect("Tire the Player by 10%", "tire_10", "energy"),
            new Effect("Tire the Player by 25%", "tire_25", "energy"),
            new Effect("Tire the Player by 50%", "tire_50", "energy"),
            new Effect("Make the Player pass out", "passout", "energy"),

            new Effect("Give the Player a Stardrop (+34 Max Energy)", "give_stardrop", "energy"),
            new Effect("Remove a Stardrop from the Player (-34 Max Energy)", "remove_stardrop", "energy"),

        new Effect("Give/Take Coins", "money", ItemKind.Folder),
            new Effect("Give the Player 100 coins", "give_money_100", "money"),
            new Effect("Give the Player 1,000 coins", "give_money_1000", "money"),
            new Effect("Give the Player 10,000 coins", "give_money_10000", "money"),
            new Effect("Remove 100 coins from the Player", "remove_money_100", "money"),
            new Effect("Remove 1,000 coins from the Player", "remove_money_1000", "money"),
            new Effect("Remove 10,000 coins from the Player", "remove_money_10000", "money"),

        new Effect("Spawn Creature", "spawn", ItemKind.Folder),
            new Effect("Spawn a Bat near the Player", "spawn_bat", "spawn"),
            new Effect("Spawn a Cave Fly near the Player", "spawn_fly", "spawn"),
            new Effect("Spawn a Frost Bat near the Player", "spawn_frostbat", "spawn"),
            new Effect("Spawn a Ghost near the Player", "spawn_ghost", "spawn"),
            new Effect("Spawn a Lava Bat near the Player", "spawn_lavabat", "spawn"),
            new Effect("Spawn a Serpent near the Player", "spawn_serpent", "spawn"),

        new Effect("Upgrade Items", "upgrade", ItemKind.Folder),
            new Effect("Upgrade the Player's Axe", "upgrade_axe", "upgrade"),
            new Effect("Upgrade the Player's Backpack", "upgrade_backpack", "upgrade"),
            new Effect("Upgrade the Player's Boots", "upgrade_boots", "upgrade"),
            new Effect("Upgrade the Player's Fishing Rod", "upgrade_fishingrod", "upgrade"),
            new Effect("Upgrade the Player's Hoe", "upgrade_hoe", "upgrade"),
            new Effect("Upgrade the Player's Pickaxe", "upgrade_pickaxe", "upgrade"),
            new Effect("Upgrade the Player's Trash Can", "upgrade_trashcan", "upgrade"),
            new Effect("Upgrade the Player's Watering Can", "upgrade_wateringcan", "upgrade"),
            new Effect("Upgrade the Player's Weapon", "upgrade_weapon", "upgrade"),

        new Effect("Warp Player", "warp", ItemKind.Folder),
            new Effect("Warp the Player to the Beach", "warp_beach", "warp"),
            new Effect("Warp the Player to the Desert", "warp_desert", "warp"),
            new Effect("Warp the Player to the Farm", "warp_farm", "warp"),
            new Effect("Warp the Player to the Island", "warp_island", "warp"),
            new Effect("Warp the Player to the Mountain", "warp_mountain", "warp"),
            new Effect("Warp the Player to the Railroad", "warp_railroad", "warp"),
            new Effect("Warp the Player to the Sewer", "warp_sewer", "warp"),
            new Effect("Warp the Player to the Wizard's Tower", "warp_tower", "warp"),
            new Effect("Warp the Player to the Town", "warp_town", "warp"),
            new Effect("Warp the Player to the Woods", "warp_woods", "warp")
    };
}
