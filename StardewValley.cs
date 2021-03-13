/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TheTexanTesla
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

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

    public override Game Game => new Game(94, "Stardew Valley", "StardewValley", "PC", ConnectorType.SimpleTCPConnector);

    public override List<Effect> Effects => new List<Effect> {
        new Effect("Downgrade the Player's Axe", "downgrade_axe"),
        new Effect("Downgrade the Player's Boots", "downgrade_boots"),
        new Effect("Downgrade the Player's Fishing Rod", "downgrade_fishingrod"),
        new Effect("Downgrade the Player's Hoe", "downgrade_hoe"),
        new Effect("Downgrade the Player's Pickaxe", "downgrade_pickaxe"),
        new Effect("Downgrade the Player's Trash Can", "downgrade_trashcan"),
        new Effect("Downgrade the Player's Watering Can", "downgrade_wateringcan"),
        new Effect("Downgrade the Player's Weapon", "downgrade_weapon"),
        new Effect("Energized the Player by 10%", "energize_10"),
        new Effect("Energized the Player by 25%", "energize_25"),
        new Effect("Energized the Player by 50%", "energize_50"),
        new Effect("Fully energize the Player", "energize_full"),
        new Effect("Give the Player the Adrenaline Rush effect for 30 seconds", "give_buff_adrenaline"),
        new Effect("Give the Player the Darkness effect for 30 seconds", "give_buff_darkness"),
        new Effect("Give the Player the Frozen effect for 10 seconds", "give_buff_frozen"),
        new Effect("Give the Player the Invincibility effect for 30 seconds", "give_buff_invincibility"),
        new Effect("Give the Player the Nauseous effect for 1 minute", "give_buff_nauseous"),
        new Effect("Give the Player the Slimed effect for 10 seconds", "give_buff_slime"),
        new Effect("Give the Player the Speed Buff effect for 2 minutes", "give_buff_speed"),
        new Effect("Give the Player the Tipsy effect for 2 minutes", "give_buff_tipsy"),
        new Effect("Give the Player the Warrior Energy effect for 30 seconds", "give_buff_warrior"),
        new Effect("Give the Player 100 coins", "give_money_100"),
        new Effect("Give the Player 1,000 coins", "give_money_1000"),
        new Effect("Give the Player 10,000 coins", "give_money_10000"),
        new Effect("Give the Player a Stardrop (+34 Max Energy)", "give_stardrop"),
        new Effect("Heal the Player by 10%", "heal_10"),
        new Effect("Heal the Player by 25%", "heal_25"),
        new Effect("Heal the Player by 50%", "heal_50"),
        new Effect("Fully heal the Player", "heal_full"),
        new Effect("Hurt the Player by 10%", "hurt_10"),
        new Effect("Hurt the Player by 25%", "hurt_25"),
        new Effect("Hurt the Player by 50%", "hurt_50"),
        new Effect("Kill the Player", "kill"),
        new Effect("Make the Player pass out", "passout"),
        new Effect("Remove 100 coins from the Player", "remove_money_100"),
        new Effect("Remove 1,000 coins from the Player", "remove_money_1000"),
        new Effect("Remove 10,000 coins from the Player", "remove_money_10000"),
        new Effect("Remove a Stardrop from the Player (-34 Max Energy)", "remove_stardrop"),
        new Effect("Spawn a Bat near the Player", "spawn_bat"),
        new Effect("Spawn a Cave Fly near the Player", "spawn_fly"),
        new Effect("Spawn a Frost Bat near the Player", "spawn_frostbat"),
        new Effect("Spawn a Ghost near the Player", "spawn_ghost"),
        new Effect("Spawn a Lava Bat near the Player", "spawn_lavabat"),
        new Effect("Spawn a Serpent near the Player", "spawn_serpent"),
        new Effect("Tire the Player by 10%", "tire_10"),
        new Effect("Tire the Player by 25%", "tire_25"),
        new Effect("Tire the Player by 50%", "tire_50"),
        new Effect("Upgrade the Player's Axe", "upgrade_axe"),
        new Effect("Upgrade the Player's Backpack", "upgrade_backpack"),
        new Effect("Upgrade the Player's Boots", "upgrade_boots"),
        new Effect("Upgrade the Player's Fishing Rod", "upgrade_fishingrod"),
        new Effect("Upgrade the Player's Hoe", "upgrade_hoe"),
        new Effect("Upgrade the Player's Pickaxe", "upgrade_pickaxe"),
        new Effect("Upgrade the Player's Trash Can", "upgrade_trashcan"),
        new Effect("Upgrade the Player's Watering Can", "upgrade_wateringcan"),
        new Effect("Upgrade the Player's Weapon", "upgrade_weapon"),
        new Effect("Warp the Player to the Beach", "warp_beach"),
        new Effect("Warp the Player to the Desert", "warp_desert"),
        new Effect("Warp the Player to the Farm", "warp_farm"),
        new Effect("Warp the Player to the Island", "warp_island"),
        new Effect("Warp the Player to the Mountain", "warp_mountain"),
        new Effect("Warp the Player to the Railroad", "warp_railroad"),
        new Effect("Warp the Player to the Sewer", "warp_sewer"),
        new Effect("Warp the Player to the Wizard's Tower", "warp_tower"),
        new Effect("Warp the Player to the Town", "warp_town"),
        new Effect("Warp the Player to the Woods", "warp_woods")
    };
}
