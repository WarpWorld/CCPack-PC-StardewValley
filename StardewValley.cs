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
            //new Effect("Backpack", "downgrade_backpack", "downgrade"),
            // 1.2.0
            new Effect("Axe", "downgrade_axe", "downgrade"),
            new Effect("Boots", "downgrade_boots", "downgrade"),
            new Effect("Fishing Rod", "downgrade_fishingrod", "downgrade"),
            new Effect("Hoe", "downgrade_hoe", "downgrade"),
            new Effect("Pickaxe", "downgrade_pickaxe", "downgrade"),
            new Effect("Trash Can", "downgrade_trashcan", "downgrade"),
            new Effect("Watering Can", "downgrade_wateringcan", "downgrade"),
            new Effect("Weapon", "downgrade_weapon", "downgrade"),


        new Effect("Upgrade Items", "upgrade", ItemKind.Folder),
            new Effect("Axe", "upgrade_axe", "upgrade"),
            //new Effect("Backpack", "upgrade_backpack", "upgrade"),
            new Effect("Boots", "upgrade_boots", "upgrade"),
            new Effect("Fishing Rod", "upgrade_fishingrod", "upgrade"),
            new Effect("Hoe", "upgrade_hoe", "upgrade"),
            new Effect("Pickaxe", "upgrade_pickaxe", "upgrade"),
            new Effect("Trash Can", "upgrade_trashcan", "upgrade"),
            new Effect("Watering Can", "upgrade_wateringcan", "upgrade"),
            new Effect("Weapon", "upgrade_weapon", "upgrade"),

        new Effect("Status Effects", "status", ItemKind.Folder),
            new Effect("Adrenaline Rush", "give_buff_adrenaline", "status") { Duration = TimeSpan.FromSeconds(30) },
            new Effect("Darkness", "give_buff_darkness", "status") { Duration = TimeSpan.FromSeconds(30) },
            new Effect("Frozen", "give_buff_frozen", "status") { Duration = TimeSpan.FromSeconds(10) },
            new Effect("Invincibility", "give_buff_invincibility", "status") { Duration = TimeSpan.FromSeconds(30) },
            new Effect("Nauseous", "give_buff_nauseous", "status") { Duration = TimeSpan.FromSeconds(60) },
            new Effect("Slimed effect", "give_buff_slime", "status") { Duration = TimeSpan.FromSeconds(10) },
            new Effect("Speed Buff", "give_buff_speed", "status") { Duration = TimeSpan.FromSeconds(120) },
            new Effect("Tipsy", "give_buff_tipsy", "status") { Duration = TimeSpan.FromSeconds(120) },
            new Effect("Warrior Energy", "give_buff_warrior", "status") { Duration = TimeSpan.FromSeconds(30) },

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
            new Effect("Give 100 coins", "give_money_100", "money"),
            new Effect("Give 1,000 coins", "give_money_1000", "money"),
            new Effect("Give 10,000 coins", "give_money_10000", "money"),
            new Effect("Remove 100 coins", "remove_money_100", "money"),
            new Effect("Remove 1,000 coins ", "remove_money_1000", "money"),
            new Effect("Remove 10,000 coins", "remove_money_10000", "money"),

        new Effect("Spawn Creature", "spawn", ItemKind.Folder),
            new Effect("Bat", "spawn_bat", "spawn"),
            new Effect("Cave Fly", "spawn_fly", "spawn"),
            new Effect("Frost Bat", "spawn_frostbat", "spawn"),
            new Effect("Ghost", "spawn_ghost", "spawn"),
            new Effect("Lava Bat", "spawn_lavabat", "spawn"),
            new Effect("Serpent", "spawn_serpent", "spawn"),
            new Effect("Bomb", "spawn_bomb", "spawn"),

        new Effect("Change Appearance", "appear", ItemKind.Folder),
            new Effect("Hair Color", "hair", ItemKind.Folder, "appear"),
                new Effect("Brown", "hair_brown", "hair"){Price = 15, Description = "Swap the player's hair color to brown!"},
                new Effect("Blonde", "hair_blonde", "hair"){Price = 15, Description = "Swap the player's hair color to blonde!"},
                new Effect("Red", "hair_red", "hair"){Price = 15, Description = "Swap the player's hair color to red!"},
                new Effect("Green", "hair_green", "hair"){Price = 15, Description = "Swap the player's hair color to green!"},
                new Effect("Blue", "hair_blue", "hair"){Price = 15, Description = "Swap the player's hair color to blue!"},
                new Effect("Yellow", "hair_yellow", "hair"){Price = 15, Description = "Swap the player's hair color to yellow!"},
                new Effect("Purple", "hair_purple", "hair"){Price = 15, Description = "Swap the player's hair color to purple!"},
                new Effect("Orange", "hair_orange", "hair"){Price = 15, Description = "Swap the player's hair color to orange!"},
                new Effect("Teal", "hair_teal", "hair"){Price = 15, Description = "Swap the player's hair color to teal!"},
                new Effect("Pink", "hair_pink", "hair"){Price = 15, Description = "Swap the player's hair color to pink!"},
                new Effect("Black", "hair_black", "hair"){Price = 15, Description = "Swap the player's hair color to black!"},
                new Effect("White", "hair_white", "hair"){Price = 15, Description = "Swap the player's hair color to white!"},
                new Effect("Random Hair Style", "hair_style", "appear"){Price = 25, Description = "Randomly change around the player's hair style!"},
                new Effect("Change Gender", "gender", "appear"){Price = 25, Description = "Change the players gender!"},
                //random fun fact: trans streamers are actually *more* likely to disable genderswap effects

        new Effect("Give Items", "items", ItemKind.Folder),
            new Effect("Sword", "give_sword", "items"){Price = 50, Description = "Give the player a Rusty Sword!"},
            new Effect("Cookie", "give_cookie", "items"){Price = 15, Description = "Give the player a cookie!"},
            new Effect("Super Meal", "give_supermeal", "items"){Price = 25, Description = "Give the player a Super Meal!"},
            new Effect("Diamond", "give_diamond", "items"){Price = 100, Description = "Give the player a Diamond!"},
            new Effect("Copper Bar", "give_copperbar", "items"){Price = 15, Description = "Give the player a Copper Bar!"},
            new Effect("Iron Bar", "give_ironbar", "items"){Price = 25, Description = "Give the player a Iron Bar!"},
            new Effect("Gold Bar", "give_goldbar", "items"){Price = 100, Description = "Give the player a Gold Bar!"},
            new Effect("Wood (5)", "give_wood", "items"){Price = 10, Description = "Give the player a piece of wood!"},
            new Effect("Stone (5)", "give_stone", "items"){Price = 10, Description = "Give the player a stone!"},

        new Effect("Warp Player", "warp", ItemKind.Folder),
            new Effect("Beach", "warp_beach", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Desert", "warp_desert", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Farm", "warp_farm", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Island", "warp_island", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Mountain", "warp_mountain", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Railroad", "warp_railroad", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Sewer", "warp_sewer", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Wizard's Tower", "warp_tower", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Town", "warp_town", "warp"){Price = 150, Description = "Warp the player!"},
            new Effect("Woods", "warp_woods", "warp"){Price = 150, Description = "Warp the player!"},

        new Effect("Send Message", "msg", ItemKind.Folder),
            new Effect("Lump of Coal", "msg_santa", "msg"){Price = 25, Description = "Send a special message to the player!"},
            new Effect("Extended Warranty", "msg_car", "msg"){Price = 25, Description = "Send a special message to the player!"},
            new Effect("Pizza Delivery", "msg_pizza", "msg"){Price = 50, Description = "Send a special message to the player!"},
            new Effect("Fertilizer Ad", "msg_grow", "msg"){Price = 25, Description = "Send a special message to the player!"},
            new Effect("Lottery", "msg_lottery", "msg"){Price = 25, Description = "Send a special message to the player!"},
            new Effect("Tech Support", "msg_tech", "msg"){Price = 25, Description = "Send a special message to the player!"}
    };
}
