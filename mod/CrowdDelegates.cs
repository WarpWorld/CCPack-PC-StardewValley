/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TerribleTable
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
using System.Threading;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using StardewBoots = StardewValley.Objects.Boots;
using StardewChest = StardewValley.Objects.Chest;
using StardewValley.Tools;
using StardewValley.Menus;

namespace ControlValley
{
    public delegate CrowdResponse CrowdDelegate(ControlClient client, CrowdRequest req);

    public class CrowdDelegates
    {
        private static readonly List<KeyValuePair<string, int>> downgradeFishingRods = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("Iridium Rod", 2),
            new KeyValuePair<string, int>("Fiberglass Rod", 0),
            new KeyValuePair<string, int>("Bamboo Pole", 1)
        };

        private static readonly List<KeyValuePair<string, int>> upgradeFishingRods = new List<KeyValuePair<string, int>>
        {
            new KeyValuePair<string, int>("Training Rod", 0),
            new KeyValuePair<string, int>("Bamboo Pole", 2),
            new KeyValuePair<string, int>("Fiberglass Rod", 3)
        };

        public static CrowdResponse DowngradeAxe(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Axe");
        }

        public static CrowdResponse DowngradeBoots(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            StardewBoots boots = Game1.player.boots.Get();
            if (boots == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is not currently wearing Boots";
            }
            else
            {
                boots = Boots.GetDowngrade(boots.getStatsIndex());
                if (boots == null)
                {
                    status = CrowdResponse.Status.STATUS_FAILURE;
                    message = Game1.player.Name + "'s Boots are already at the lowest upgrade level";
                }
                else
                {
                    Game1.player.boots.Value = boots;
                    Game1.player.changeShoeColor(boots.indexInColorSheet);
                    UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Boots");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DowngradeFishingRod(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            foreach (KeyValuePair<string, int> downgrade in downgradeFishingRods)
            {
                Tool tool = Game1.player.getToolFromName(downgrade.Key);
                if (tool != null)
                {
                    tool.UpgradeLevel = downgrade.Value;
                    UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Fishing Rod");

                    return new CrowdResponse(id);
                }
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Fishing Rod is already at the lowest upgrade level");
        }

        public static CrowdResponse DowngradeHoe(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Hoe");
        }

        public static CrowdResponse DowngradePickaxe(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Pickaxe");
        }

        public static CrowdResponse DowngradeTrashCan(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.trashCanLevel > 0)
            {
                Interlocked.Decrement(ref Game1.player.trashCanLevel);
                UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Trash Can");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Trash Can is already at the lowest upgrade level";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DowngradeWateringCan(ControlClient client, CrowdRequest req)
        {
            return DoDowngrade(req, "Watering Can");
        }

        public static CrowdResponse DowngradeWeapon(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            if (WeaponClass.Club.DoDowngrade() || WeaponClass.Sword.DoDowngrade() || WeaponClass.Dagger.DoDowngrade())
            {
                UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s Weapon");
                return new CrowdResponse(id);
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Weapon is already at the lowest upgrade level");
        }

        public static CrowdResponse Energize10(ControlClient client, CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.1f);
        }

        public static CrowdResponse Energize25(ControlClient client, CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.25f);
        }

        public static CrowdResponse Energize50(ControlClient client, CrowdRequest req)
        {
            return DoEnergizeBy(req, 0.5f);
        }

        public static CrowdResponse EnergizeFull(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                Game1.player.Stamina = max;
                UI.ShowInfo($"{req.GetReqViewer()} fully energized {Game1.player.Name}");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at maximum energy";
            }
            
            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveBuffAdrenaline(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.adrenalineRush, dur, "Adrenaline Rush");
        }

        public static CrowdResponse GiveBuffDarkness(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (Game1.IsRainingHere()) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Raining");

            return DoGiveBuff(req, Buff.darkness, dur, "Darkness");
        }

        public static CrowdResponse GiveBuffFrozen(ControlClient client, CrowdRequest req)
        {
            int dur = 10;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.frozen, dur, "Frozen");
        }

        public static CrowdResponse GiveBuffInvincibility(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.yobaBlessing, dur, "Invincibility");
        }

        public static CrowdResponse GiveBuffNauseous(ControlClient client, CrowdRequest req)
        {
            int dur = 60;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.nauseous, dur, "Nauseous");
        }

        public static CrowdResponse GiveBuffSlime(ControlClient client, CrowdRequest req)
        {
            int dur = 10;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.slimed, dur, "Slimed");
        }

        public static CrowdResponse GiveBuffSpeed(ControlClient client, CrowdRequest req)
        {
            int dur = 120;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.speed, dur, "Speed Buff");
        }

        public static CrowdResponse GiveBuffTipsy(ControlClient client, CrowdRequest req)
        {
            int dur = 120;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.tipsy, dur, "Tipsy");
        }

        public static CrowdResponse GiveBuffWarrior(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            return DoGiveBuff(req, Buff.warriorEnergy, dur, "Warrior Energy");
        }

        public static CrowdResponse GiveMoney100(ControlClient client, CrowdRequest req)
        {
            return DoGiveMoney(req, 100);
        }

        public static CrowdResponse GiveMoney1000(ControlClient client, CrowdRequest req)
        {
            return DoGiveMoney(req, 1000);
        }

        public static CrowdResponse GiveMoney10000(ControlClient client, CrowdRequest req)
        {
            return DoGiveMoney(req, 10000);
        }

        public static CrowdResponse GiveStardrop(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 508)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at the highest energy maximum";
            }
            else
            {
                stamina += 34;
                Game1.player.MaxStamina = stamina;
                Game1.player.Stamina = stamina;
                UI.ShowInfo($"{req.GetReqViewer()} gave {Game1.player.Name} a Stardrop");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Heal10(ControlClient client, CrowdRequest req)
        {
            return DoHealBy(req, 0.1f);
        }

        public static CrowdResponse Heal25(ControlClient client, CrowdRequest req)
        {
            return DoHealBy(req, 0.25f);
        }

        public static CrowdResponse Heal50(ControlClient client, CrowdRequest req)
        {
            return DoHealBy(req, 0.5f);
        }

        public static CrowdResponse HealFull(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Interlocked.Exchange(ref Game1.player.health, Game1.player.maxHealth) == 0)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} fully healed {Game1.player.Name}");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Hurt10(ControlClient client, CrowdRequest req)
        {
            return DoHurtBy(req, 0.1f);
        }

        public static CrowdResponse Hurt25(ControlClient client, CrowdRequest req)
        {
            return DoHurtBy(req, 0.25f);
        }

        public static CrowdResponse Hurt50(ControlClient client, CrowdRequest req)
        {
            return DoHurtBy(req, 0.5f);
        }

        public static CrowdResponse Kill(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Interlocked.Exchange(ref Game1.player.health, 0) == 0)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} killed {Game1.player.Name}");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse PassOut(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > -16)
            {
                Game1.player.Stamina = -16;
                UI.ShowInfo($"{req.GetReqViewer()} made {Game1.player.Name} pass out");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently passed out";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse RemoveMoney100(ControlClient client, CrowdRequest req)
        {
            return DoRemoveMoney(req, 100);
        }

        public static CrowdResponse RemoveMoney1000(ControlClient client, CrowdRequest req)
        {
            return DoRemoveMoney(req, 1000);
        }

        public static CrowdResponse RemoveMoney10000(ControlClient client, CrowdRequest req)
        {
            return DoRemoveMoney(req, 10000);
        }

        public static CrowdResponse RemoveStardrop(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 270)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at lowest energy maximum";
            }
            else
            {
                stamina -= 34;
                Game1.player.MaxStamina = stamina;
                if (Game1.player.Stamina > stamina)
                    Game1.player.Stamina = stamina;
                UI.ShowInfo($"{req.GetReqViewer()} removed a Stardrop from {Game1.player.Name}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpawnBat(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Bat(GetRandomNear(), 20));
        }

        public static CrowdResponse SpawnFly(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Fly(GetRandomNear()));
        }

        public static CrowdResponse SpawnGhost(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Ghost(GetRandomNear()));
        }

        public static CrowdResponse SpawnLavaBat(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Bat(GetRandomNear(), 100));
        }

        public static CrowdResponse SpawnFrostBat(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Bat(GetRandomNear(), 60));
        }

        public static CrowdResponse SpawnSerpent(ControlClient client, CrowdRequest req)
        {
            return DoSpawn(client, req, new Serpent(GetRandomNear()));
        }

        public static CrowdResponse SpawnBomb(ControlClient client, CrowdRequest req)
        {
            return DoBomb(client, req, GetRandomNear());
        }
        public static CrowdResponse Tire10(ControlClient client, CrowdRequest req)
        {
            return DoTireBy(req, 0.1f);
        }

        public static CrowdResponse Tire25(ControlClient client, CrowdRequest req)
        {
            return DoTireBy(req, 0.25f);
        }

        public static CrowdResponse Tire50(ControlClient client, CrowdRequest req)
        {
            return DoTireBy(req, 0.5f);
        }

        public static CrowdResponse UpgradeAxe(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Axe");
        }

        public static CrowdResponse UpgradeBackpack(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.items.Capacity == 36)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Backpack is already at maximum capacity";
            }
            else
            {
                Game1.player.increaseBackpackSize(12);
                UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Backpack");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeBoots(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            StardewBoots boots = Game1.player.boots.Get();
            if (boots == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is not currently wearing Boots";
            }
            else
            {
                boots = Boots.GetUpgrade(boots.getStatsIndex());
                if (boots == null)
                {
                    status = CrowdResponse.Status.STATUS_FAILURE;
                    message = Game1.player.Name + "'s Boots are already at the highest upgrade level";
                }
                else
                {
                    Game1.player.boots.Value = boots;
                    Game1.player.changeShoeColor(boots.indexInColorSheet);
                    UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Boots");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeFishingRod(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            foreach (KeyValuePair<string, int> upgrade in upgradeFishingRods)
            {
                Tool tool = Game1.player.getToolFromName(upgrade.Key);
                if (tool != null)
                {
                    tool.UpgradeLevel = upgrade.Value;
                    UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Fishing Rod");

                    return new CrowdResponse(id);
                }
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Fishing Rod is already at the highest upgrade level");
        }

        public static CrowdResponse UpgradeHoe(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Hoe");
        }

        public static CrowdResponse UpgradePickaxe(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Pickaxe");
        }

        public static CrowdResponse UpgradeTrashCan(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.player.trashCanLevel < 4)
            {
                Interlocked.Increment(ref Game1.player.trashCanLevel);
                UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Trash Can");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + "'s Trash Can is already at the highest upgrade level";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeWeapon(ControlClient client, CrowdRequest req)
        {
            int id = req.GetReqID();

            if (WeaponClass.Club.DoUpgrade() || WeaponClass.Sword.DoUpgrade() || WeaponClass.Dagger.DoUpgrade())
            {
                UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s Weapon");
                return new CrowdResponse(id);
            }

            return new CrowdResponse(id, CrowdResponse.Status.STATUS_FAILURE, Game1.player.Name + "'s Weapon is already at the highest upgrade level");
        }

        public static CrowdResponse UpgradeWateringCan(ControlClient client, CrowdRequest req)
        {
            return DoUpgrade(req, "Watering Can");
        }

        public static CrowdResponse WarpBeach(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Beach", 20, 4);
        }

        public static CrowdResponse WarpDesert(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Desert", 35, 43);
        }

        public static CrowdResponse WarpFarm(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Farm", 64, 15);
        }

        public static CrowdResponse WarpIsland(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "IslandSouth", 11, 11);
        }

        public static CrowdResponse WarpMountain(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Mountain", 31, 20);
        }

        public static CrowdResponse WarpRailroad(ControlClient client, CrowdRequest req)
        {
            if (Game1.stats.DaysPlayed < 31U) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Blocked");
            return DoWarp(req, "Railroad", 35, 52);
        }

        public static CrowdResponse WarpSewer(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Sewer", 16, 13);
        }

        public static CrowdResponse WarpTower(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Forest", 5, 29);
        }

        public static CrowdResponse WarpTown(ControlClient client, CrowdRequest req)
        {
            return DoWarp(req, "Town", 29, 67);
        }

        public static CrowdResponse WarpWoods(ControlClient client, CrowdRequest req)
        {
            var forest = (StardewValley.Locations.Forest) Game1.getLocationFromName("Forest");

            if (forest!=null)
            {
                if (forest.log != null)
                {
                    if(!forest.log.isPassable() || forest.log.health > 0) if (Game1.stats.DaysPlayed < 31U) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Blocked");
                }
            }


            return DoWarp(req, "Woods", 55, 15);
        }

        private static CrowdResponse DoDowngrade(CrowdRequest req, string toolName)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = $"{Game1.player.Name}'s {toolName} is already at the lowest upgrade level";
            }
            else
            {
                int level = tool.UpgradeLevel;
                if (level == 0)
                    status = CrowdResponse.Status.STATUS_FAILURE;
                else
                {
                    tool.UpgradeLevel = level - 1;
                    UI.ShowInfo($"{req.GetReqViewer()} downgraded {Game1.player.Name}'s {toolName}");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoEnergizeBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                stamina += percent * max;
                Game1.player.Stamina = (stamina > max) ? max : stamina;
                UI.ShowInfo($"{req.GetReqViewer()} energized {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already at maximum energy";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoGiveBuff(CrowdRequest req, int buff, int duration, string name)
        {
            new Thread(new BuffThread(buff, duration * 1000).Run).Start();
            UI.ShowInfo($"{req.GetReqViewer()} gave {Game1.player.Name} the {name} effect for {duration} seconds");
            return new CrowdResponse(req.GetReqID());
        }

        private static CrowdResponse DoGiveMoney(CrowdRequest req, int amount)
        {
            Game1.player.addUnearnedMoney(amount);
            UI.ShowInfo($"{req.GetReqViewer()} gave {Game1.player.Name} {amount} coins");
            return new CrowdResponse(req.GetReqID());
        }

        private static CrowdResponse DoHealBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int max = Game1.player.maxHealth;
            int health = (int)Math.Floor(percent * max) + Game1.player.health;
            if (Interlocked.Exchange(ref Game1.player.health, (health > max) ? max : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} healed {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoHurtBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int health = Game1.player.health - (int)Math.Floor(percent * Game1.player.maxHealth);
            if (Interlocked.Exchange(ref Game1.player.health, (health < 0) ? 0 : health) == 0)
            {
                Game1.player.health = 0;
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already dead";
            }
            else
                UI.ShowInfo($"{req.GetReqViewer()} hurt {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoRemoveMoney(CrowdRequest req, int amount)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            int money = Game1.player.Money;
            if (money > 0)
            {
                money -= amount;
                Game1.player.Money = (money < 0) ? 0 : money;
                UI.ShowInfo($"{req.GetReqViewer()} removed {amount} coins from {Game1.player.Name}");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " currently has no money";
            }
            
            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoSpawn(ControlClient client, CrowdRequest req, Monster monster)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (client.CanSpawn())
            {
                Game1.player.currentLocation.addCharacter(monster);
                client.TrackMonster(monster);
                UI.ShowInfo($"{req.GetReqViewer()} spawned a {monster.Name} near {Game1.player.Name}");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = $"Cannot spawn {monster.Name} because {Game1.player.Name} is at {Game1.player.currentLocation.Name}";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoBomb(ControlClient client, CrowdRequest req, Vector2 loc)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (client.CanSpawn())
            {
                var sprite = new TemporaryAnimatedSprite(286, 100f, 1, 24, loc, true, false, Game1.player.currentLocation, Game1.player)
                                    {
                                        extraInfoForEndBehavior = 286,
                                        endFunction = new TemporaryAnimatedSprite.endBehavior(Game1.player.currentLocation.removeTemporarySpritesWithID)
                                    };

                Game1.player.currentLocation.temporarySprites.Add(sprite);

                UI.ShowInfo($"{req.GetReqViewer()} spawned a bomb near {Game1.player.Name}");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = $"Cannot spawn bomb because {Game1.player.Name} is at {Game1.player.currentLocation.Name}";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoTireBy(CrowdRequest req, float percent)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > 0)
            {
                stamina -= percent * Game1.player.MaxStamina;
                Game1.player.Stamina = (stamina < 0) ? 0 : stamina;
                UI.ShowInfo($"{req.GetReqViewer()} tired {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");
            }
            else
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = Game1.player.Name + " is already passed out";
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoUpgrade(CrowdRequest req, string toolName, int max = 4)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = $"{Game1.player.Name}'s {toolName} is already at the highest upgrade level";
            }
            else
            {
                int level = tool.UpgradeLevel;
                if (level == max)
                    status = CrowdResponse.Status.STATUS_FAILURE;
                else
                {
                    tool.UpgradeLevel = level + 1;
                    UI.ShowInfo($"{req.GetReqViewer()} upgraded {Game1.player.Name}'s {toolName}");
                }
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse GiveSword(ControlClient client, CrowdRequest req)
        {
            bool found = false;

            if (Game1.player.hasItemInInventoryNamed("Rusty Sword")) found = true;
            if (Game1.player.hasItemInInventoryNamed("Steel Smallsword")) found = true;
            if (Game1.player.hasItemInInventoryNamed("Wooden Blade")) found = true;
            if (Game1.player.hasItemInInventoryNamed("Pirate's Sword")) found = true;
            if (Game1.player.hasItemInInventoryNamed("Silver Saber")) found = true;
            if (Game1.player.hasItemInInventoryNamed("Cutlass")) found = true;
            if (Game1.player.hasItemInInventoryNamed("Forest Sword")) found = true;
            if (Game1.player.hasItemInInventoryNamed("Iron Edge")) found = true;

            if(found)
                return new CrowdResponse(req.id, CrowdResponse.Status.STATUS_FAILURE, "Already have sword");

            return GiveItem(req, new MeleeWeapon(0));
        }

        public static CrowdResponse GiveCookie(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 223);
        }

        public static CrowdResponse GiveSuperMeal(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 237);
        }

        public static CrowdResponse GiveDiamond(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 72);
        }

        public static CrowdResponse GiveCopperBar(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 334);
        }

        public static CrowdResponse GiveIronBar(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 335);
        }

        public static CrowdResponse GiveGoldBar(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 336);
        }

        public static CrowdResponse GiveWood(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 388, 5);
        }

        public static CrowdResponse GiveStone(ControlClient client, CrowdRequest req)
        {
            return GiveItem(req, 390, 5);
        }

        public static CrowdResponse SantaMSG(ControlClient client, CrowdRequest req)
        {
            return SendMail(req, "Dear @,^^    You have been very naughty this year, so all you get is this lump of coal.^^                                       -Santa%item object 382 1 %%", "Christmas");
        }

        public static CrowdResponse CarMSG(ControlClient client, CrowdRequest req)
        {
            return SendMail(req, "Hello @!^^    We have been trying to reach you about your car's extended warranty...", "car");
        }

        public static CrowdResponse PizzaMSG(ControlClient client, CrowdRequest req)
        {
            return SendMail(req, "I've got a uh... large pepperoni pizza for... @?^^That'll be $21.59.%item object 206 1 %%", "pizza");
        }

        public static CrowdResponse GrowMSG(ControlClient client, CrowdRequest req)
        {
            return SendMail(req, "Do you have have sad crops that just take forever to grow?!^^You need Miracle Grow, the latest in fertilization technology!^^Try it today!%item object 368 1 %%", "grow");
        }

        public static CrowdResponse LotteryMSG(ControlClient client, CrowdRequest req)
        {
            return SendMail(req, "Mr. or Mrs. @,^^I am writing to inform you that you have won $100 in the Malaysian National lottery!^^To collect your winnings, please remit a $5 processing fee to the postmarked address.%item money 100 101 %%", "lottery");
        }

        public static CrowdResponse TechMSG(ControlClient client, CrowdRequest req)
        {
            return SendMail(req, $"Hello, is this @?^^This is {req.GetReqViewer()} with Microsoft technical support and I am calling you today about a problem we have detected in your computer.^^If you could allow me to remote access your computer I can walk you through fixing the issue for a support fee of $1.99 a minute.", "tech");
        }

        private static CrowdResponse GiveItem(CrowdRequest req, Item item)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Game1.player.addItemByMenuIfNecessary(item);

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse GiveItem(CrowdRequest req, int item, int qty = 1)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(item,qty));

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse SendMail(CrowdRequest req, string text, string title)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (Game1.activeClickableMenu!=null)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "menu open");
            }

            Game1.activeClickableMenu = new LetterViewerMenu(text, title, false);

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse BrownHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(0.4f, 0.2f, 0.1f));
        }

        public static CrowdResponse BlondeHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(1.0f, 0.85f, 0.55f));
        }

        public static CrowdResponse RedHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(1.0f, 0, 0));
        }

        public static CrowdResponse GreenHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(0, 1.0f, 0));
        }

        public static CrowdResponse BlueHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(0, 0, 1.0f));
        }

        public static CrowdResponse YellowHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(1.0f, 1.0f, 0));
        }

        public static CrowdResponse PurpleHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(0.5f, 0, 1.0f));
        }

        public static CrowdResponse OrangeHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(1.0f, 0.45f, 0));
        }

        public static CrowdResponse TealHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(0, 1.0f, 1.0f));
        }

        public static CrowdResponse PinkHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(1.0f, 0.5f, 0.8f));
        }

        public static CrowdResponse BlackHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(0.15f, 0.15f, 0.15f));
        }

        public static CrowdResponse WhiteHair(ControlClient client, CrowdRequest req)
        {
            return DoHairColor(req, new Color(1.0f, 1.0f, 1.0f));
        }

        private static CrowdResponse DoHairColor(CrowdRequest req, Color c)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Game1.player.changeHairColor(c);

            UI.ShowInfo($"{req.GetReqViewer()} changed {Game1.player.Name}'s hair color.");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse HairStyle(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Game1.player.changeHairStyle(new Random().Next(1,73));

            UI.ShowInfo($"{req.GetReqViewer()} changed {Game1.player.Name}'s hair style.");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Gender(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            Game1.player.changeGender(!Game1.player.IsMale);

            UI.ShowInfo($"{req.GetReqViewer()} changed {Game1.player.Name}'s gender.");

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        private static CrowdResponse DoWarp(CrowdRequest req, string name, int targetX, int targetY)
        {
            //UI.ShowInfo($"Can Move: {Game1.player.canMove} Using Tool: {Game1.player.usingTool}");
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if(!Game1.player.canMove || Game1.player.usingTool)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "Player Busy");
            }

            try
            {
                Game1.player.resetState();
                Game1.warpFarmer(name, targetX, targetY, false);
            }
            catch (Exception e)
            {
                UI.ShowError(e.Message);
            }

            if (Game1.isWarping == false)
            {
                message = "Warp failed";
                status = CrowdResponse.Status.STATUS_FAILURE;
            }

            if (name == "Forest")
                name = "Wizard's Tower";
            else if (name == "IslandSouth")
                name = "Island";
            UI.ShowInfo($"{req.GetReqViewer()} warped {Game1.player.Name} to the {name}");

            

            return new CrowdResponse(req.GetReqID(),status, message);
        }

        private static readonly float MAX_RADIUS = 400;

        private static Vector2 GetRandomNear()
        {
            Random random = new Random();
            return Game1.player.Position + new Vector2(
                (float)((random.NextDouble() * 2 * MAX_RADIUS) - MAX_RADIUS),
                (float)((random.NextDouble() * 2 * MAX_RADIUS) - MAX_RADIUS));
        }
    }
}
