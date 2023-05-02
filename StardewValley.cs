﻿using System;
using System.Collections.Generic;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs;

public class StardewValley : SimpleTCPPack
{
    public override string Host => "127.0.0.1";

    public override ushort Port => 51337;

    public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

    public StardewValley(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new("Stardew Valley", "StardewValley", "PC", ConnectorType.SimpleTCPServerConnector);

    public override EffectList Effects => new List<Effect>
    {
            new Effect("Axe", "downgrade_axe"){ Category = "Downgrade Items"},
            new Effect("Boots", "downgrade_boots"){ Category = "Downgrade Items"},
            new Effect("Fishing Rod", "downgrade_fishingrod"){ Category = "Downgrade Items"},
            new Effect("Hoe", "downgrade_hoe"){ Category = "Downgrade Items"},
            new Effect("Pickaxe", "downgrade_pickaxe"){ Category = "Downgrade Items"},
            new Effect("Trash Can", "downgrade_trashcan"){ Category = "Downgrade Items"},
            new Effect("Watering Can", "downgrade_wateringcan"){ Category = "Downgrade Items"},
            new Effect("Weapon", "downgrade_weapon"){ Category = "Downgrade Items"},


            new Effect("Axe", "upgrade_axe"){ Category = "Upgrade Items"},
            //new Effect("Backpack", "upgrade_backpack"){ Category = "Upgrade Items"},
            new Effect("Boots", "upgrade_boots"){ Category = "Upgrade Items"},
            new Effect("Fishing Rod", "upgrade_fishingrod"){ Category = "Upgrade Items"},
            new Effect("Hoe", "upgrade_hoe"){ Category = "Upgrade Items"},
            new Effect("Pickaxe", "upgrade_pickaxe"){ Category = "Upgrade Items"},
            new Effect("Trash Can", "upgrade_trashcan"){ Category = "Upgrade Items"},
            new Effect("Watering Can", "upgrade_wateringcan"){ Category = "Upgrade Items"},
            new Effect("Weapon", "upgrade_weapon"){ Category = "Upgrade Items"},

            new Effect("Adrenaline Rush", "give_buff_adrenaline") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },
            new Effect("Darkness", "give_buff_darkness") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },
            new Effect("Frozen", "give_buff_frozen") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(10) },
            new Effect("Invincibility", "give_buff_invincibility") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },
            new Effect("Nauseous", "give_buff_nauseous") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(60) },
            new Effect("Slimed effect", "give_buff_slime") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(10) },
            new Effect("Speed Buff", "give_buff_speed") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(120) },
            new Effect("Tipsy", "give_buff_tipsy") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(120) },
            new Effect("Warrior Energy", "give_buff_warrior") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },

            new Effect("Heal the Player by 10%", "heal_10") { Category = "Give/Take Life"},
            new Effect("Heal the Player by 25%", "heal_25") { Category = "Give/Take Life"},
            new Effect("Heal the Player by 50%", "heal_50") { Category = "Give/Take Life"},
            new Effect("Fully heal the Player", "heal_full") { Category = "Give/Take Life"},
            new Effect("Hurt the Player by 10%", "hurt_10") { Category = "Give/Take Life"},
            new Effect("Hurt the Player by 25%", "hurt_25") { Category = "Give/Take Life"},
            new Effect("Hurt the Player by 50%", "hurt_50") { Category = "Give/Take Life"},
            new Effect("Kill the Player", "kill") { Category = "Give/Take Life"},

            new Effect("Energized the Player by 10%", "energize_10") { Category = "Give/Take Energy"},
            new Effect("Energized the Player by 25%", "energize_25") { Category = "Give/Take Energy"},
            new Effect("Energized the Player by 50%", "energize_50") { Category = "Give/Take Energy"},
            new Effect("Fully energize the Player", "energize_full") { Category = "Give/Take Energy"},

            new Effect("Tire the Player by 10%", "tire_10") { Category = "Give/Take Energy"},
            new Effect("Tire the Player by 25%", "tire_25") { Category = "Give/Take Energy"},
            new Effect("Tire the Player by 50%", "tire_50") { Category = "Give/Take Energy"},
            new Effect("Make the Player pass out", "passout") { Category = "Give/Take Energy"},

            new Effect("Give the Player a Stardrop (+34 Max Energy)", "give_stardrop") { Category = "Give/Take Energy"},
            new Effect("Remove a Stardrop from the Player (-34 Max Energy)", "remove_stardrop") { Category = "Give/Take Energy"},

            new Effect("Give 100 coins", "give_money_100") { Category = "Give/Take Coins"},
            new Effect("Give 1,000 coins", "give_money_1000") { Category = "Give/Take Coins"},
            new Effect("Give 10,000 coins", "give_money_10000") { Category = "Give/Take Coins"},
            new Effect("Remove 100 coins", "remove_money_100") { Category = "Give/Take Coins"},
            new Effect("Remove 1,000 coins ", "remove_money_1000") { Category = "Give/Take Coins"},
            new Effect("Remove 10,000 coins", "remove_money_10000") { Category = "Give/Take Coins"},

            new Effect("Bat", "spawn_bat") { Category = "Spawn Creature"},
            new Effect("Cave Fly", "spawn_fly") { Category = "Spawn Creature"},
            new Effect("Frost Bat", "spawn_frostbat") { Category = "Spawn Creature"},
            new Effect("Ghost", "spawn_ghost") { Category = "Spawn Creature"},
            new Effect("Lava Bat", "spawn_lavabat") { Category = "Spawn Creature"},
            new Effect("Serpent", "spawn_serpent") { Category = "Spawn Creature"},
            new Effect("Bomb", "spawn_bomb") { Category = "Spawn Creature"},

                new Effect("Brown", "hair_brown"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to brown!"},
                new Effect("Blonde", "hair_blonde"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to blonde!"},
                new Effect("Red", "hair_red"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to red!"},
                new Effect("Green", "hair_green"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to green!"},
                new Effect("Blue", "hair_blue"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to blue!"},
                new Effect("Yellow", "hair_yellow"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to yellow!"},
                new Effect("Purple", "hair_purple"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to purple!"},
                new Effect("Orange", "hair_orange"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to orange!"},
                new Effect("Teal", "hair_teal"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to teal!"},
                new Effect("Pink", "hair_pink"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to pink!"},
                new Effect("Black", "hair_black"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to black!"},
                new Effect("White", "hair_white"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to white!"},
                new Effect("Random Hair Style", "hair_style"){ Category = "Change Appearance", Price = 25, Description = "Randomly change around the player's hair style!"},
                new Effect("Change Gender", "gender"){ Category = "Change Appearance", Price = 25, Description = "Change the players gender!"},
                //random fun fact: trans streamers are actually *more* likely to disable genderswap effects

            new Effect("Sword", "give_sword") { Category = "Give Items", Price = 50, Description = "Give the player a Rusty Sword!"},
            new Effect("Cookie", "give_cookie") { Category = "Give Items", Price = 15, Description = "Give the player a cookie!"},
            new Effect("Super Meal", "give_supermeal") { Category = "Give Items", Price = 25, Description = "Give the player a Super Meal!"},
            new Effect("Diamond", "give_diamond") { Category = "Give Items", Price = 100, Description = "Give the player a Diamond!"},
            new Effect("Copper Bar", "give_copperbar") { Category = "Give Items", Price = 15, Description = "Give the player a Copper Bar!"},
            new Effect("Iron Bar", "give_ironbar") { Category = "Give Items", Price = 25, Description = "Give the player a Iron Bar!"},
            new Effect("Gold Bar", "give_goldbar") { Category = "Give Items", Price = 100, Description = "Give the player a Gold Bar!"},
            new Effect("Wood (5)", "give_wood") { Category = "Give Items", Price = 10, Description = "Give the player a piece of wood!"},
            new Effect("Stone (5)", "give_stone") { Category = "Give Items", Price = 10, Description = "Give the player a stone!"},

            new Effect("Beach", "warp_beach"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Desert", "warp_desert"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Farm", "warp_farm"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Island", "warp_island"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Mountain", "warp_mountain"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Railroad", "warp_railroad"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Sewer", "warp_sewer"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Wizard's Tower", "warp_tower"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Town", "warp_town"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new Effect("Woods", "warp_woods"){ Category = "Warps", Price = 150, Description = "Warp the player!"},

            new Effect("Lump of Coal", "msg_santa"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new Effect("Extended Warranty", "msg_car"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new Effect("Pizza Delivery", "msg_pizza"){ Category = "Send Message", Price = 50, Description = "Send a special message to the player!"},
            new Effect("Fertilizer Ad", "msg_grow"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new Effect("Lottery", "msg_lottery"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new Effect("Tech Support", "msg_tech"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"}
    };
}