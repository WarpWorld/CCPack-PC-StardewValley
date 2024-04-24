using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.StardewValley;

public class StardewValley : SimpleTCPPack
{
    public override string Host => "127.0.0.1";

    public override ushort Port => 51337;

    public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

    public StardewValley(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new("Stardew Valley", "StardewValley", "PC", ConnectorType.SimpleTCPServerConnector);

    public override EffectList Effects => new List<Effect>
    {

            new("Play Horse Race", "horserace"),
            new("Stop Horse Race", "horseraceend"),

            new("Axe", "downgrade_axe"){ Category = "Downgrade Items"},
            new("Boots", "downgrade_boots"){ Category = "Downgrade Items"},
            new("Fishing Rod", "downgrade_fishingrod"){ Category = "Downgrade Items"},
            new("Hoe", "downgrade_hoe"){ Category = "Downgrade Items"},
            new("Pickaxe", "downgrade_pickaxe"){ Category = "Downgrade Items"},
            new("Trash Can", "downgrade_trashcan"){ Category = "Downgrade Items"},
            new("Watering Can", "downgrade_wateringcan"){ Category = "Downgrade Items"},
            new("Weapon", "downgrade_weapon"){ Category = "Downgrade Items"},


            new("Axe", "upgrade_axe"){ Category = "Upgrade Items"},
            //new Effect("Backpack", "upgrade_backpack"){ Category = "Upgrade Items"},
            new("Boots", "upgrade_boots"){ Category = "Upgrade Items"},
            new("Fishing Rod", "upgrade_fishingrod"){ Category = "Upgrade Items"},
            new("Hoe", "upgrade_hoe"){ Category = "Upgrade Items"},
            new("Pickaxe", "upgrade_pickaxe"){ Category = "Upgrade Items"},
            new("Trash Can", "upgrade_trashcan"){ Category = "Upgrade Items"},
            new("Watering Can", "upgrade_wateringcan"){ Category = "Upgrade Items"},
            new("Weapon", "upgrade_weapon"){ Category = "Upgrade Items"},

            new("Adrenaline Rush", "give_buff_adrenaline") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },
            new("Darkness", "give_buff_darkness") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },
            new("Frozen", "give_buff_frozen") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(10) },
            new("Invincibility", "give_buff_invincibility") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },
            new("Nauseous", "give_buff_nauseous") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(60) },
            new("Slimed effect", "give_buff_slime") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(10) },
            new("Speed Buff", "give_buff_speed") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(120) },
            new("Tipsy", "give_buff_tipsy") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(120) },
            new("Warrior Energy", "give_buff_warrior") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(30) },

            new("Heal the Player by 10%", "heal_10") { Category = "Give/Take Life"},
            new("Heal the Player by 25%", "heal_25") { Category = "Give/Take Life"},
            new("Heal the Player by 50%", "heal_50") { Category = "Give/Take Life"},
            new("Fully heal the Player", "heal_full") { Category = "Give/Take Life"},
            new("Hurt the Player by 10%", "hurt_10") { Category = "Give/Take Life"},
            new("Hurt the Player by 25%", "hurt_25") { Category = "Give/Take Life"},
            new("Hurt the Player by 50%", "hurt_50") { Category = "Give/Take Life"},
            new("Kill the Player", "kill") { Category = "Give/Take Life"},

            new("Energized the Player by 10%", "energize_10") { Category = "Give/Take Energy"},
            new("Energized the Player by 25%", "energize_25") { Category = "Give/Take Energy"},
            new("Energized the Player by 50%", "energize_50") { Category = "Give/Take Energy"},
            new("Fully energize the Player", "energize_full") { Category = "Give/Take Energy"},

            new("Tire the Player by 10%", "tire_10") { Category = "Give/Take Energy"},
            new("Tire the Player by 25%", "tire_25") { Category = "Give/Take Energy"},
            new("Tire the Player by 50%", "tire_50") { Category = "Give/Take Energy"},
            new("Make the Player pass out", "passout") { Category = "Give/Take Energy"},

            new("Give the Player a Stardrop (+34 Max Energy)", "give_stardrop") { Category = "Give/Take Energy"},
            new("Remove a Stardrop from the Player (-34 Max Energy)", "remove_stardrop") { Category = "Give/Take Energy"},

            new("Give 100 coins", "give_money_100") { Category = "Give/Take Coins"},
            new("Give 1,000 coins", "give_money_1000") { Category = "Give/Take Coins"},
            new("Give 10,000 coins", "give_money_10000") { Category = "Give/Take Coins"},
            new("Remove 100 coins", "remove_money_100") { Category = "Give/Take Coins"},
            new("Remove 1,000 coins ", "remove_money_1000") { Category = "Give/Take Coins"},
            new("Remove 10,000 coins", "remove_money_10000") { Category = "Give/Take Coins"},

            new("Bat", "spawn_bat") { Category = "Spawn Creature", Quantity = 100},
            new("Cave Fly", "spawn_fly") { Category = "Spawn Creature", Quantity = 100},
            new("Frost Bat", "spawn_frostbat") { Category = "Spawn Creature", Quantity = 100},
            new("Ghost", "spawn_ghost") { Category = "Spawn Creature", Quantity = 100},
            new("Lava Bat", "spawn_lavabat") { Category = "Spawn Creature", Quantity = 100},
            new("Serpent", "spawn_serpent") { Category = "Spawn Creature", Quantity = 100},
            new("Green Slime", "spawn_slime") { Category = "Spawn Creature", Quantity = 100},
            new("Red Slime", "spawn_redslime") { Category = "Spawn Creature", Quantity = 100},
            new("Frost Jelly", "spawn_frostjelly") { Category = "Spawn Creature", Quantity = 100},
            new("Red Sludge", "spawn_redsludge") { Category = "Spawn Creature", Quantity = 100},
            new("Blue Squid", "spawn_bluesquid") { Category = "Spawn Creature", Quantity = 100},
            new("Skelton", "spawn_skelton") { Category = "Spawn Creature", Quantity = 100},
            new("Skeleton Mage", "spawn_skeletonmage") { Category = "Spawn Creature", Quantity = 100},


            new("Bug", "spawn_bug") { Category = "Spawn Creature", Quantity = 100},
            new("Wilderness Golem", "spawn_wildernessgolem") { Category = "Spawn Creature", Quantity = 100},
            new("Monster Musk", "give_buff_monstermusk") { Category = "Status Effects", Duration = TimeSpan.FromSeconds(60), Description = "Doubles the enemies encountered." },
            new("Crowd Control Pro", "msg_crowdcontrolpro"){ Category = "Send Message", Price = 1, Description = "Send a reminder to the streamer to purchase Crowd Control Pro!"},
            new("Emote Sad", "emote_sad"){ Category = "Force Emote", Price = 1, Description = "Force the player to the sad emote."},
            new("Emote Heart", "emote_heart"){ Category = "Force Emote", Price = 1, Description = "Force the player to the heart emote."},
            new("Emote Exclamation", "emote_exclamation"){ Category = "Force Emote", Price = 1, Description = "Force the player to the sad emote."},
            new("Emote Sleep", "emote_sleep"){ Category = "Force Emote", Price = 1, Description = "Force the player to the sleep emote."},
            new("Emote Game", "emote_game"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the game emote."},
            new("Emote Note", "emote_note"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the note emote."},
            new("Emote Question", "emote_question"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the note emote."},
            new("Emote X", "emote_x"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the question emote."},
            new("Emote Pause", "emote_pause"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the pause emote."},
            new("Emote Blush", "emote_blush"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the blush emote."},
            new("Emote Angry", "emote_angry"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the angry emote."},
            new("Emote yes", "emote_yes"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the yes emote."},
            new("Emote No", "emote_no"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the no emote."},
            new("Emote Sick", "emote_sick"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the sick emote."},
            new("Emote Laugh", "emote_laugh"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the laugh emote."},
            new("Emote Taunt", "emote_taunt"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the taunt emote."},
            new("Emote Surprised", "emote_surprised"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the surprised emote."},
            new("Emote Hi", "emote_hi"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the hi emote."},
            new("Emote Uh", "emote_uh"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the uh emote."},
            new("Emote Play Music", "emote_music"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the music emote."},
            new("Emote Open Jar", "emote_jar"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the open jar emote."},
            new("Emote Happy", "emote_happy"){ Category = "Force Emote", Price = 1, Description = "Force the player to use the happy emote.!"},

            new("Force Divorce Ofcourse", "divorce"){ Category = "Heartbreak", Price = 100000, Note = "Does not work when player is in the house.", Description = "Force the player to divorce their spouse if they are married."},
            new("Remove Children", "removechildren"){ Category = "Heartbreak", Price = 100000, Description = "Force the player's children to turn to doves or something."},
            new("Force Swimware", "swimware_on"){ Price = 500, Description = "Force the player to wear their swimware."},
            new("Remove Swimware", "swimware_off"){ Price = 200, Description = "Removes the players swimware."},


            new("Bomb", "spawn_bomb") { Category = "Spawn Creature"},

                new("Brown", "hair_brown"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to brown!"},
                new("Blonde", "hair_blonde"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to blonde!"},
                new("Red", "hair_red"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to red!"},
                new("Green", "hair_green"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to green!"},
                new("Blue", "hair_blue"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to blue!"},
                new("Yellow", "hair_yellow"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to yellow!"},
                new("Purple", "hair_purple"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to purple!"},
                new("Orange", "hair_orange"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to orange!"},
                new("Teal", "hair_teal"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to teal!"},
                new("Pink", "hair_pink"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to pink!"},
                new("Black", "hair_black"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to black!"},
                new("White", "hair_white"){ Category = "Hair Color", Price = 15, Description = "Swap the player's hair color to white!"},
                new("Random Hair Style", "hair_style"){ Category = "Change Appearance", Price = 25, Description = "Randomly change around the player's hair style!"},
                new("Change Gender", "gender"){ Category = "Change Appearance", Price = 25, Description = "Change the players gender!"},
                //random fun fact: trans streamers are actually *more* likely to disable genderswap effects

            new("Sword", "give_sword") { Category = "Give Items", Price = 50, Description = "Give the player a Rusty Sword!"},
            new("Cookie", "give_cookie") { Category = "Give Items", Price = 15, Description = "Give the player a cookie!"},
            new("Super Meal", "give_supermeal") { Category = "Give Items", Price = 25, Description = "Give the player a Super Meal!"},
            new("Diamond", "give_diamond") { Category = "Give Items", Price = 100, Description = "Give the player a Diamond!"},
            new("Copper Bar", "give_copperbar") { Category = "Give Items", Price = 15, Description = "Give the player a Copper Bar!"},
            new("Iron Bar", "give_ironbar") { Category = "Give Items", Price = 25, Description = "Give the player a Iron Bar!"},
            new("Gold Bar", "give_goldbar") { Category = "Give Items", Price = 100, Description = "Give the player a Gold Bar!"},
            new("Wood (5)", "give_wood") { Category = "Give Items", Price = 10, Description = "Give the player a piece of wood!"},
            new("Stone (5)", "give_stone") { Category = "Give Items", Price = 10, Description = "Give the player a stone!"},

            new("Beach", "warp_beach"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Desert", "warp_desert"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Farm", "warp_farm"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Island", "warp_island"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Mountain", "warp_mountain"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Railroad", "warp_railroad"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Sewer", "warp_sewer"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Wizard's Tower", "warp_tower"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Town", "warp_town"){ Category = "Warps", Price = 150, Description = "Warp the player!"},
            new("Woods", "warp_woods"){ Category = "Warps", Price = 150, Description = "Warp the player!"},

            new("Lump of Coal", "msg_santa"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new("Extended Warranty", "msg_car"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new("Pizza Delivery", "msg_pizza"){ Category = "Send Message", Price = 50, Description = "Send a special message to the player!"},
            new("Fertilizer Ad", "msg_grow"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new("Lottery", "msg_lottery"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"},
            new("Tech Support", "msg_tech"){ Category = "Send Message", Price = 25, Description = "Send a special message to the player!"}
    };
}