/*
 * CrowdControl
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
using StardewValley.Tools;
using StardewValley.Menus;
using xTile.Tiles;
using xTile.ObjectModel;
using StardewValley.Locations;
using System.Linq;
using System.Reflection;
using ConnectorLib.JSON;
using CrowdControl.Effects;
using StardewValley.TerrainFeatures;
using System.Xml.Linq;

namespace CrowdControl
{
    public delegate void EffectDelegate(ControlClient client, EffectRequest req);

    public class EffectDelegates
    {
        public static void DowngradeAxe(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "downgraded", "axes", "Axe");

        public static void DowngradeBoots(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            StardewBoots boots = Game1.player.boots.Get();
            if (boots == null)
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is not currently wearing Boots";
            }
            else
            {
                boots = Boots.GetDowngrade(boots.getStatsIndex());
                if (boots == null)
                {
                    status = EffectStatus.Failure;
                    message = Game1.player.Name + "'s Boots are already at the lowest upgrade level";
                }
                else
                {
                    Game1.player.boots.Value = boots;
                    Game1.player.changeShoeColor(boots.indexInColorSheet.ToString());
                    UI.ShowInfo($"{req.viewer} downgraded {Game1.player.Name}'s Boots");
                }
            }

            client.Respond(req, status, message);
        }

        public static void UpdateEquipment(ControlClient client, EffectRequest req, string action, string equipment, string defaultItemName)
        {

            Dictionary<string, int> fishingRods = new()
            {
                {"Advanced Iridium Rod", 4},
                {"Iridium Rod", 3},
                {"Fiberglass Rod", 2},
                {"Training Rod", 1},
                {"Bamboo Pole", 0},
                {"Fishing Rod", -1}
            };

            Dictionary<string, int> axes = new()
            {
                {"Iridium Axe", 4},
                {"Gold Axe", 3},
                {"Steel Axe", 2},
                {"Copper Axe", 1},
                {"Axe", 0},
            };

            Dictionary<string, int> pickaxes = new()
            {
                {"Iridium Pickaxe", 4},
                {"Gold Pickaxe", 3},
                {"Steel Pickaxe", 2},
                {"Copper Pickaxe", 1},
                {"Pickaxe", 0},
            };

            Dictionary<string, int> hoes = new()
            {
                {"Iridium Hoe", 4},
                {"Gold Hoe", 3},
                {"Steel Hoe", 2},
                {"Copper Hoe", 1},
                {"Hoe", 0},
            };

            Dictionary<string, int> wateringCans = new()
            {
                {"Iridium Watering Can", 4},
                {"Gold Watering Can", 3},
                {"Steel Watering Can", 2},
                {"Copper Watering Can", 1},
                {"Watering Can", 0},
            };

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
            }

            EffectStatus status = EffectStatus.Failure;
            string message = "";

            Dictionary<string, int> itemList = new Dictionary<string, int>();

            if (equipment == "fishingRods") itemList = fishingRods;
            if (equipment == "axes") itemList = axes;
            if (equipment == "pickaxes") itemList = pickaxes;
            if (equipment == "hoes") itemList = hoes;
            if (equipment == "wateringCans") itemList = wateringCans;

            int direction = action == "downgraded" ? 1 : -1;
            int dictionarySize = itemList.Keys.ToList().Count;
            Tool existingTool = null;
            int inventoryPosition = -1;
            int itemLevel = -1;
            string newItemName = "";

            foreach (var item in itemList.OrderByDescending(rl => rl.Value))
            {
                if (Game1.player.Items.Any(invItem => invItem?.Name == item.Key))
                {
                    existingTool = Game1.player.getToolFromName(item.Key);
                    inventoryPosition = Game1.player.Items.IndexOf(Game1.player.getToolFromName(item.Key));


                    // If the existing tool is already at the lowest upgrade level, we can't downgrade it
                    if (existingTool.UpgradeLevel == 0 && direction == 1)
                    {
                        message = $"{Game1.player.Name}'s {equipment} is already at the lowest upgrade level";
                        client.Respond(req, status, message); return;
                    }

                    // If the Key matches the default item name, we will get it's current level and use that instead of the full item name
                    // This is mainly for "Fishing Rod" as it can be added as just "Fishing Rod" in some instances
                    // But could also happen for others?
                    if (item.Key == defaultItemName && equipment == "fishingRods")
                    {
                        string realItemName = itemList.FirstOrDefault(x => x.Value == 2).Key;
                        int updatedItemIndex = itemList.Keys.ToList().IndexOf(realItemName) + direction;
                        newItemName = itemList.Keys.ToList()[updatedItemIndex];
                        if (updatedItemIndex <= dictionarySize && updatedItemIndex >= 0)
                        {
                            itemLevel = itemList.Values.ToList()[updatedItemIndex];
                        }
                    }
                    else
                    {

                        int updatedItemIndex = itemList.Keys.ToList().IndexOf(item.Key) + direction;
                        if (updatedItemIndex <= dictionarySize && updatedItemIndex >= 0)
                        {
                            //for some reason unknown to us and the spirits sometimes getting the item name from the list
                            //will cause a crash, we don't really need to set a name, but it's nice
                            try { newItemName = itemList.Keys.ToList()[updatedItemIndex]; }
                            catch (Exception) { newItemName = ""; }

                            if (action == "downgraded" ) itemLevel = existingTool.UpgradeLevel - 1;
                            if (action == "upgraded" ) itemLevel = existingTool.UpgradeLevel + 1;
                        }
                    }
                    break;
                }
            }

            if (inventoryPosition == -1 || existingTool == null || itemLevel == -1)
            {
                message = $"{Game1.player.Name}'s {equipment} is not updatable or missing";
                client.Respond(req, status, message); return;
            }


            Game1.player.removeItemFromInventory(existingTool);
            Tool newItem = null;
            switch (equipment)
            {
                case "fishingRods":
                    newItem = new FishingRod() { Name = newItemName, UpgradeLevel = itemLevel };
                    break;
                case "axes":
                    newItem = new Axe() { Name = newItemName, UpgradeLevel = itemLevel };
                    break;
                case "pickaxes":
                    newItem = new Pickaxe() { Name = newItemName, UpgradeLevel = itemLevel };
                    break;
                case "hoes":
                    newItem = new Hoe() { Name = newItemName, UpgradeLevel = itemLevel };
                    break;
                case "wateringCans":
                    newItem = new WateringCan() { Name = newItemName, UpgradeLevel = itemLevel };
                    break;
            }

            if (newItem != null)
            {
                Game1.player.addItemToInventory(newItem, inventoryPosition);
                UI.ShowInfo($"{req.viewer} {action} {Game1.player.Name}'s {defaultItemName}");
                status = EffectStatus.Success;
                client.Respond(req, status, message);
            }
            else
            {
                message = $"{Game1.player.Name}'s {equipment} is not updatable or missing";
                client.Respond(req, status, message);
            }
        }

        public static void DowngradeHoe(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "downgraded", "hoes", "Hoe");

        public static void DowngradePickaxe(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "downgraded", "pickaxes", "Pickaxe");

        public static void DowngradeTrashCan(ControlClient client, EffectRequest req)
        {

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
            }

            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (Game1.player.trashCanLevel > 0)
            {
                Interlocked.Decrement(ref Game1.player.trashCanLevel);
                UI.ShowInfo($"{req.viewer} downgraded {Game1.player.Name}'s Trash Can");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + "'s Trash Can is already at the lowest upgrade level";
            }

            client.Respond(req, status, message);
        }

        public static void DowngradeWateringCan(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "downgraded", "wateringCans", "Watering Can");

        public static void DowngradeWeapon(ControlClient client, EffectRequest req)
        {

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
            }

            if (WeaponClass.Club.DoDowngrade() || WeaponClass.Sword.DoDowngrade() || WeaponClass.Dagger.DoDowngrade())
            {
                UI.ShowInfo($"{req.viewer} downgraded {Game1.player.Name}'s Weapon");
                client.Respond(req, EffectStatus.Success);
                return;
            }

            client.Respond(req, EffectStatus.Failure, Game1.player.Name + "'s Weapon is already at the lowest upgrade level");
        }

        public static void Energize10(ControlClient client, EffectRequest req)
            => DoEnergizeBy(client, req, 0.1f);

        public static void Energize25(ControlClient client, EffectRequest req)
            => DoEnergizeBy(client, req, 0.25f);

        public static void Energize50(ControlClient client, EffectRequest req)
            => DoEnergizeBy(client, req, 0.5f);

        public static void EnergizeFull(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                Game1.player.Stamina = max;
                UI.ShowInfo($"{req.viewer} fully energized {Game1.player.Name}");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is already at maximum energy";
            }

            client.Respond(req, status, message);
        }

        public static void GiveBuffAdrenaline(ControlClient client, EffectRequest req)
        {
            float dur = 30;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.adrenalineRush, dur, "Adrenaline Rush");
        }

        public static void GiveBuffDarkness(ControlClient client, EffectRequest req)
        {
            float dur = 30;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            if (Game1.IsRainingHere())
            {
                client.Respond(req, EffectStatus.Retry, "Raining");
                return;
            }

            DoGiveBuff(client, req, Buff.darkness, dur, "Darkness");
        }

        public static void GiveMonsterMuskBuff(ControlClient client, EffectRequest req)
        {
            float dur = 30;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.spawnMonsters, dur, "Monster Musk Buff");
        }


        public static void GiveBuffFrozen(ControlClient client, EffectRequest req)
        {
            float dur = 10;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.frozen, dur, "Frozen");
        }

        public static void GiveBuffInvincibility(ControlClient client, EffectRequest req)
        {
            float dur = 30;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.yobaBlessing, dur, "Invincibility");
        }

        public static void GiveBuffNauseous(ControlClient client, EffectRequest req)
        {
            float dur = 60;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.nauseous, dur, "Nauseous");
        }

        public static void GiveBuffSlime(ControlClient client, EffectRequest req)
        {
            float dur = 10;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.slimed, dur, "Slimed");
        }

        public static void GiveBuffSpeed(ControlClient client, EffectRequest req)
        {
            float dur = 120;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.speed.ToString(), dur, "Speed Buff");
        }

        public static void GiveBuffTipsy(ControlClient client, EffectRequest req)
        {
            float dur = 120;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.tipsy, dur, "Tipsy");
        }

        public static void GiveBuffWarrior(ControlClient client, EffectRequest req)
        {
            float dur = 30;
            if (req.duration > 0) dur = req.duration.Value / 1000f;

            DoGiveBuff(client, req, Buff.warriorEnergy, dur, "Warrior Energy");
        }

        public static void GiveMoney100(ControlClient client, EffectRequest req)
            => DoGiveMoney(client, req, 100);

        public static void GiveMoney1000(ControlClient client, EffectRequest req)
            => DoGiveMoney(client, req, 1000);

        public static void GiveMoney10000(ControlClient client, EffectRequest req)
            => DoGiveMoney(client, req, 10000);
        
        public static void PlayHorseRace(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {

            }
            catch (Exception e)
            {
                UI.ShowInfo(e.ToString());
            }

            client.Respond(req, status, message);
        }

        public static void StopHorseRace(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            try
            {

            }
            catch (Exception e)
            {
                UI.ShowInfo(e.ToString());
            }

            client.Respond(req, status, message);
        }

        public static void GiveStardrop(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 508)
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is already at the highest energy maximum";
            }
            else
            {
                stamina += 34;
                try
                {
                    FieldInfo? field = typeof(Farmer).GetField("maxStamina", BindingFlags.Instance | BindingFlags.Public);
                    field.SetValue(Game1.player, new Netcode.NetInt(stamina));
                }
                catch (Exception e)
                {
                    UI.ShowError(e.ToString());
                }

                Game1.player.Stamina = stamina;
                UI.ShowInfo($"{req.viewer} gave {Game1.player.Name} a Stardrop");
            }

            client.Respond(req, status, message);
        }

        public static void Heal10(ControlClient client, EffectRequest req)
            => DoHealBy(client, req, 0.1f);

        public static void Heal25(ControlClient client, EffectRequest req)
            => DoHealBy(client, req, 0.25f);

        public static void Heal50(ControlClient client, EffectRequest req)
            => DoHealBy(client, req, 0.5f);

        public static void HealFull(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (Interlocked.Exchange(ref Game1.player.health, Game1.player.maxHealth) == 0)
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.viewer} fully healed {Game1.player.Name}");

            client.Respond(req, status, message);
        }

        public static void Hurt10(ControlClient client, EffectRequest req)
            => DoHurtBy(client, req, 0.1f);

        public static void Hurt25(ControlClient client, EffectRequest req)
            => DoHurtBy(client, req, 0.25f);

        public static void Hurt50(ControlClient client, EffectRequest req)
            => DoHurtBy(client, req, 0.5f);

        public static void Kill(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (Game1.isFestival())
            {
                client.Respond(req, EffectStatus.Failure, "Player is at the festival");
                return;
            }


            if (Interlocked.Exchange(ref Game1.player.health, 0) == 0)
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.viewer} killed {Game1.player.Name}");

            client.Respond(req, status, message);
        }

        public static void PassOut(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > -16)
            {
                Game1.player.Stamina = -16;
                UI.ShowInfo($"{req.viewer} made {Game1.player.Name} pass out");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is currently passed out";
            }

            client.Respond(req, status, message);
        }

        public static void RemoveMoney100(ControlClient client, EffectRequest req)
            => DoRemoveMoney(client, req, 100);

        public static void RemoveMoney1000(ControlClient client, EffectRequest req)
            => DoRemoveMoney(client, req, 1000);

        public static void RemoveMoney10000(ControlClient client, EffectRequest req)
            => DoRemoveMoney(client, req, 10000);

        public static void Divorce(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";


            if (Game1.player.isMarriedOrRoommates() && !Game1.player.divorceTonight.Value && !(Game1.currentLocation is FarmHouse))
            {

                int money = Game1.player.Money;
                if (money > 2)
                {
                    money = money / 2;
                    Game1.player.Money = (money < 0) ? 0 : money;
                }

                Game1.player.doDivorce();
                UI.ShowInfo($"{req.viewer} caused {Game1.player.Name} to get a divorce.");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is not currently married.";
            }


            client.Respond(req, status, message);
        }


        public static void TurnChildrenToDoves(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";


            if (Game1.player.getNumberOfChildren() >= 1)
            {

                int money = Game1.player.Money;
                Game1.player.Money = money + 3;
                Game1.player.getRidOfChildren();
                UI.ShowInfo($"{req.viewer} has turned {Game1.player.Name}'s children into doves!");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is currently lucky and does not have any children... Yet...";
            }


            client.Respond(req, status, message);
        }



        public static void ChangeSwimClothes(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            string code = req.code;
            int underScore = code.IndexOf("_");
            string swimwear = code.Substring(underScore + 1);

            if (swimwear == "on" && !Game1.player.bathingClothes.Value)
            {
                Game1.player.changeIntoSwimsuit();
                UI.ShowInfo($"{req.viewer} forced you to wear swimwear!");
            }
            else if (swimwear == "off" && Game1.player.bathingClothes.Value)
            {
                Game1.player.changeOutOfSwimSuit();
                UI.ShowInfo($"{req.viewer} has returned your clothes to you.");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " can't change their clothes for some reason.";
            }



            client.Respond(req, status, message);
        }


        public static void PlayerEmote(ControlClient client, EffectRequest req)
        {

            EffectStatus status = EffectStatus.Success;
            string message = "";
            //if (!Game1.eventUp && !UsingTool && (!IsLocalPlayer || Game1.activeClickableMenu == null) && !isRidingHorse() && !IsSitting() && !base.IsEmoting && CanMove)

            if (Game1.player.CanEmote() && Game1.player.CanMove && !Game1.player.IsBusyDoingSomething() && !Game1.player.usingTool.Value)
            {
                string code = req.code;
                int underScore = code.IndexOf("_");
                string emote = code.Substring(underScore + 1);


                //Game1.player.FarmerSprite.PauseForSingleAnimation = false;
                //Game1.player.faceDirection(1);
                //Game1.player.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>
                //{
                //    new FarmerSprite.AnimationFrame(101, 1000, 0, secondaryArm: false, Game1.player.FacingDirection == 3),
                //    new FarmerSprite.AnimationFrame(6, 1, secondaryArm: false, Game1.player.FacingDirection == 3)
                //}.ToArray());

                //Game1.player.changeHat(1);


                Game1.player.performPlayerEmote(emote);
                UI.ShowInfo($"{req.viewer} forced {Game1.player.Name} to emote {emote}!");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " currently cannot emote/move.";
            }


            client.Respond(req, status, message);
        }


        public static void RemoveStardrop(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            int stamina = Game1.player.MaxStamina;
            if (stamina == 270)
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is already at lowest energy maximum";
            }
            else
            {
                stamina -= 34;

                try
                {
                    FieldInfo? field = typeof(Farmer).GetField("maxStamina", BindingFlags.Instance | BindingFlags.Public);
                    field.SetValue(Game1.player, new Netcode.NetInt(stamina));
                }
                catch (Exception e)
                {
                    UI.ShowError(e.ToString());
                }


                if (Game1.player.Stamina > stamina)
                    Game1.player.Stamina = stamina;
                UI.ShowInfo($"{req.viewer} removed a Stardrop from {Game1.player.Name}");

            }

            client.Respond(req, status, message);
        }
        public static bool CanSpawnHere(Vector2 tile)
        {
            Vector2 tilePosition = tile;

            bool isWalkable = true;

            // Check the Back layer for passability
            Tile backLayerTile = Game1.currentLocation.map.GetLayer("Back").Tiles[(int)tilePosition.X, (int)tilePosition.Y];
            if (backLayerTile != null)
            {
                backLayerTile.TileIndexProperties.TryGetValue("Passable", out PropertyValue isPassable);
                if (isPassable == null)
                {
                    isWalkable = false;
                }
            }

            // Check the Buildings layer
            Tile buildingLayerTile = Game1.currentLocation.map.GetLayer("Buildings").Tiles[(int)tilePosition.X, (int)tilePosition.Y];
            if (buildingLayerTile != null)
            {
                isWalkable = false;
            }

            // Optionally, check the Front and AlwaysFront layers
            Tile frontLayerTile = Game1.currentLocation.map.GetLayer("Front").Tiles[(int)tilePosition.X, (int)tilePosition.Y];
            if (frontLayerTile != null)
            {
                isWalkable = false;
            }

            //this will attempt to prevent monsters from spawning on tiles they shouldnt
            //it wont make it impossible, just more unlikely in theory


            return isWalkable;


        }


        public static void SpawnMonsters(ControlClient client, EffectRequest req, Func<Vector2, Monster> createMonster, bool spawnClose)
        {
            if (Game1.isFestival())
            {
                client.Respond(req, EffectStatus.Failure, "Player is at the festival");
                return;
            }

            uint quantity = req.quantity ?? 1;

            List<Monster> monsters = new List<Monster>();
            Span<Vector2> locations = stackalloc Vector2[(int)quantity];

            for (int i = 0; i < quantity; i++)
            {
                if (!TryGetRandomClose(out Vector2 location))
                {
                    client.Respond(req, EffectStatus.Failure, "Not enough valid spawn locations found");
                    return;
                }
                locations[i] = location;
            }

            for (int i = 0; i < quantity; i++) monsters.Add(createMonster(locations[i]));
            DoSpawn(client, req, monsters);
        }

        public static void SpawnBat(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Bat(location, 20), false);

        public static void SpawnBlueSquid(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new BlueSquid(location), false);


        public static void SpawnSkeleton(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Skeleton(location, true), false);

        public static void SpawnSkeletonMage(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Skeleton(location, true), false);
        
        public static void SpawnRedSlime(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new GreenSlime(location, 105), false);

        public static void SpawnGreenSlime(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new GreenSlime(location, 2), true);

        public static void SpawnFrostJelly(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new GreenSlime(location, 40), true);

        public static void SpawnRedSludge(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new GreenSlime(location, 85), true);

        public static void SpawnFly(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Fly(location), false);

        public static void DowngradeFishingRod(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "downgraded", "fishingRods", "Fishing Rod");

        public static void UpgradeFishingRod(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "upgraded", "fishingRods", "Fishing Rod");

        public static void SpawnBug(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Bug(location, 20), false);

        public static void SpawnWildernessGolem(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new RockGolem(location, 35), true);
        
        public static void SpawnGhost(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Ghost(location), false);

        public static void SpawnLavaBat(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Bat(location, 100), false);

        public static void SpawnFrostBat(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Bat(location, 60), false);

        public static void SpawnSerpent(ControlClient client, EffectRequest req)
            => SpawnMonsters(client, req, location => new Serpent(location), false);

        public static void SpawnBomb(ControlClient client, EffectRequest req)
        {
            if (!TryGetRandomClose(out Vector2 location))
            {
                client.Respond(req, EffectStatus.Failure, "No valid spawn location found");
                return;
            }
            DoBomb(client, req, location);
        }

        public static void Tire10(ControlClient client, EffectRequest req)
            => DoTireBy(client, req, 0.1f);

        public static void Tire25(ControlClient client, EffectRequest req)
            => DoTireBy(client, req, 0.25f);

        public static void Tire50(ControlClient client, EffectRequest req)
            => DoTireBy(client, req, 0.5f);

        public static void UpgradeAxe(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "upgraded", "axes", "Axe");

        public static void UpgradeBackpack(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (Game1.player.Items.Count >= 36)
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + "'s Backpack is already at maximum capacity";
            }
            else
            {
                Game1.player.increaseBackpackSize(12);
                UI.ShowInfo($"{req.viewer} upgraded {Game1.player.Name}'s Backpack");
            }

            client.Respond(req, status, message);
        }

        public static void UpgradeBoots(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            StardewBoots boots = Game1.player.boots.Get();
            if (boots == null)
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is not currently wearing Boots";
            }
            else
            {
                boots = Boots.GetUpgrade(boots.getStatsIndex());
                if (boots == null)
                {
                    status = EffectStatus.Failure;
                    message = Game1.player.Name + "'s Boots are already at the highest upgrade level";
                }
                else
                {
                    Game1.player.boots.Value = boots;
                    Game1.player.changeShoeColor(boots.indexInColorSheet.ToString());
                    UI.ShowInfo($"{req.viewer} upgraded {Game1.player.Name}'s Boots");
                }
            }

            client.Respond(req, status, message);
        }
        
        public static void UpgradeHoe(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "upgraded", "hoes", "Hoe");

        public static void UpgradePickaxe(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "upgraded", "pickaxes", "Pickaxe");

        public static void UpgradeTrashCan(ControlClient client, EffectRequest req)
        {

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
            }

            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (Game1.player.trashCanLevel < 4)
            {
                Interlocked.Increment(ref Game1.player.trashCanLevel);
                UI.ShowInfo($"{req.viewer} upgraded {Game1.player.Name}'s Trash Can");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + "'s Trash Can is already at the highest upgrade level";
            }

            client.Respond(req, status, message);
        }

        public static void UpgradeWeapon(ControlClient client, EffectRequest req)
        {

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
            }
            
            if (WeaponClass.Club.DoUpgrade() || WeaponClass.Sword.DoUpgrade() || WeaponClass.Dagger.DoUpgrade())
            {
                UI.ShowInfo($"{req.viewer} upgraded {Game1.player.Name}'s Weapon");
                client.Respond(req, EffectStatus.Success);
                return;
            }

            client.Respond(req, EffectStatus.Failure, Game1.player.Name + "'s Weapon is already at the highest upgrade level");
        }

        public static void UpgradeWateringCan(ControlClient client, EffectRequest req)
            => UpdateEquipment(client, req, "upgraded", "wateringCans", "Watering Can");

        public static void WarpBeach(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "Beach", 20, 4);

        public static void WarpDesert(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "Desert", 35, 43);

        public static void WarpFarm(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "Farm", 64, 15);

        public static void WarpIsland(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "IslandSouth", 11, 11);

        public static void WarpMountain(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "Mountain", 31, 20);

        public static void WarpRailroad(ControlClient client, EffectRequest req)
        {
            if (Game1.stats.DaysPlayed < 31U)
            {
                client.Respond(req, EffectStatus.Failure, "Blocked");
                return;
            }
            DoWarp(client, req, "Railroad", 35, 52);
        }

        public static void WarpSewer(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "Sewer", 16, 13);

        public static void WarpTower(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "Forest", 5, 29);

        public static void WarpTown(ControlClient client, EffectRequest req)
            => DoWarp(client, req, "Town", 29, 67);

        public static void WarpWoods(ControlClient client, EffectRequest req)
        {
            Forest? forest = (StardewValley.Locations.Forest)Game1.getLocationFromName("Forest");

            //forest.obsolete_log;

            foreach (ResourceClump? res in forest.resourceClumps)
            {
                //UI.ShowInfo($"resource: {res.ToString()}");
                if (!res.isPassable() || res.health.Value > 0)
                {
                    client.Respond(req, EffectStatus.Failure, "Blocked");
                    return;
                }
            }

            DoWarp(client, req, "Woods", 55, 15);
        }

        private static void DoUpgrade(ControlClient client, EffectRequest req, string toolName, int max = 4)
        {

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
            }

            EffectStatus status = EffectStatus.Success;
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = EffectStatus.Failure;
                message = $"{Game1.player.Name}'s {toolName} is already at the highest upgrade level";
            }
            else
            {
                int level = tool.UpgradeLevel;
                if (level == max)
                    status = EffectStatus.Failure;
                else
                {
                    int index = Game1.player.Items.IndexOf(tool);
                    Game1.player.removeItemFromInventory(tool);
                    Tool add = null;

                    if (toolName == "Axe")
                        add = new Axe() { UpgradeLevel = level + 1 };

                    if (toolName == "Hoe")
                        add = new Hoe() { UpgradeLevel = level + 1 };

                    if (toolName == "Pickaxe")
                        add = new Pickaxe() { UpgradeLevel = level + 1 };

                    if (toolName == "Watering Can")
                        add = new WateringCan() { UpgradeLevel = level + 1 };

                    Game1.player.addItemToInventory(add, index);

                    UI.ShowInfo($"{req.viewer} upgraded {Game1.player.Name}'s {toolName}");
                }
            }

            client.Respond(req, status, message);
        }

        private static void DoDowngrade(ControlClient client, EffectRequest req, string toolName)
        {

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
            }


            EffectStatus status = EffectStatus.Success;
            string message = "";

            Tool tool = Game1.player.getToolFromName(toolName);
            if (tool == null)
            {
                status = EffectStatus.Failure;
                message = $"{Game1.player.Name}'s {toolName} is already at the lowest upgrade level";
            }
            else
            {
                int level = tool.UpgradeLevel;
                if (level == 0)
                    status = EffectStatus.Failure;
                else
                {

                    int index = Game1.player.Items.IndexOf(tool);
                    Game1.player.removeItemFromInventory(tool);
                    Tool add = null;

                    if (toolName == "Axe")
                        add = new Axe() { UpgradeLevel = level + 1 };

                    if (toolName == "Hoe")
                        add = new Hoe() { UpgradeLevel = level - 1 };

                    if (toolName == "Pickaxe")
                        add = new Pickaxe() { UpgradeLevel = level - 1 };

                    if (toolName == "Watering Can")
                        add = new WateringCan() { UpgradeLevel = level - 1 };

                    Game1.player.addItemToInventory(add, index);

                    UI.ShowInfo($"{req.viewer} downgraded {Game1.player.Name}'s {toolName}!!");
                }
            }

            client.Respond(req, status, message);
        }

        private static void DoEnergizeBy(ControlClient client, EffectRequest req, float percent)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            int max = Game1.player.MaxStamina;
            float stamina = Game1.player.Stamina;
            if (stamina < max)
            {
                stamina += percent * max;
                Game1.player.Stamina = (stamina > max) ? max : stamina;
                UI.ShowInfo($"{req.viewer} energized {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is already at maximum energy";
            }
            
            client.Respond(req, status, message);
        }

        private static void DoGiveBuff(ControlClient client, EffectRequest req, string buff, SITimeSpan duration, string name)
        {
            if (!TimedThread.TryEnqueue(req, new TimedBuff(buff), duration))
            {
                client.Respond(req, EffectStatus.Retry, $"The {name} buff is already active.");
                return;
            }
            UI.ShowInfo($"{req.viewer} gave {Game1.player.Name} the {name} effect for {duration.TotalSeconds} seconds");
            client.Respond(req, EffectStatus.Success, duration);
        }

        private static void DoGiveMoney(ControlClient client, EffectRequest req, int amount)
        {
            Game1.player.addUnearnedMoney(amount);
            UI.ShowInfo($"{req.viewer} gave {Game1.player.Name} {amount} coins");
            client.Respond(req, EffectStatus.Success);
        }

        private static void DoHealBy(ControlClient client, EffectRequest req, float percent)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            int max = Game1.player.maxHealth;
            int health = (int)Math.Floor(percent * max) + Game1.player.health;
            if (Interlocked.Exchange(ref Game1.player.health, (health > max) ? max : health) == 0)
            {
                Game1.player.health = 0;
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is currently dead";
            }
            else
                UI.ShowInfo($"{req.viewer} healed {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");

            client.Respond(req, status, message);
        }

        private static void DoHurtBy(ControlClient client, EffectRequest req, float percent)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            int health = Game1.player.health - (int)Math.Floor(percent * Game1.player.maxHealth);
            if (Interlocked.Exchange(ref Game1.player.health, (health < 0) ? 0 : health) == 0)
            {
                Game1.player.health = 0;
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is already dead";
            }
            else
                UI.ShowInfo($"{req.viewer} hurt {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");

            client.Respond(req, status, message);
        }

        private static void DoRemoveMoney(ControlClient client, EffectRequest req, int amount)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            int money = Game1.player.Money;
            if (money > 0)
            {
                money -= amount;
                Game1.player.Money = (money < 0) ? 0 : money;
                UI.ShowInfo($"{req.viewer} removed {amount} coins from {Game1.player.Name}");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " currently has no money";
            }

            client.Respond(req, status, message);
        }

        public static void DoSpawn(ControlClient client, EffectRequest req, List<Monster> monsters)

        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            uint quantity = req.quantity ?? 1;

            if (client.CanSpawn())
            {
                int showMessage = 0;
                foreach (Monster monster in monsters)
                {
                    Game1.currentLocation.addCharacter(monster);
                    client.TrackMonster(monster);

                    if (quantity == 1 && showMessage == 0)
                    {
                        showMessage = 1;
                        UI.ShowInfo($"{req.viewer} spawned a {monster.Name} near {Game1.player.Name}");
                    }
                    else if (quantity >= 1 && showMessage == 0)
                    {
                        showMessage = 1;
                        UI.ShowInfo($"{req.viewer} spawned {quantity} {monster.Name}s near {Game1.player.Name}");
                    }
                }
            }
            else
            {
                status = EffectStatus.Failure;
                message = $"Cannot spawn monster because {Game1.player.Name} is at {Game1.player.currentLocation.Name}";
            }

            client.Respond(req, status, message);
        }

        private static void DoBomb(ControlClient client, EffectRequest req, Vector2 loc)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (client.CanSpawn())
            {
                TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite(286, 100f, 1, 24, loc, true, false, Game1.player.currentLocation, Game1.player)
                {
                    extraInfoForEndBehavior = 286,
                    endFunction = new TemporaryAnimatedSprite.endBehavior(Game1.player.currentLocation.removeTemporarySpritesWithID)
                };

                Game1.player.currentLocation.temporarySprites.Add(sprite);

                UI.ShowInfo($"{req.viewer} spawned a bomb near {Game1.player.Name}");
            }
            else
            {
                status = EffectStatus.Failure;
                message = $"Cannot spawn bomb because {Game1.player.Name} is at {Game1.player.currentLocation.Name}";
            }

            client.Respond(req, status, message);
        }

        private static void DoTireBy(ControlClient client, EffectRequest req, float percent)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            float stamina = Game1.player.Stamina;
            if (stamina > 0)
            {
                stamina -= percent * Game1.player.MaxStamina;
                Game1.player.Stamina = (stamina < 0) ? 0 : stamina;
                UI.ShowInfo($"{req.viewer} tired {Game1.player.Name} by {(int)Math.Floor(100 * percent)}%");
            }
            else
            {
                status = EffectStatus.Failure;
                message = Game1.player.Name + " is already passed out";
            }

            client.Respond(req, status, message);
        }


        public static void GiveSword(ControlClient client, EffectRequest req)
        {
            bool found = false;



            if (Game1.player.Items.Any(item => item?.Name == "Rusty Sword")) found = true;
            if (Game1.player.Items.Any(item => item?.Name == "Steel Smallsword")) found = true;
            if (Game1.player.Items.Any(item => item?.Name == "Wooden Blade")) found = true;
            if (Game1.player.Items.Any(item => item?.Name == "Pirate's Sword")) found = true;
            if (Game1.player.Items.Any(item => item?.Name == "Silver Saber")) found = true;
            if (Game1.player.Items.Any(item => item?.Name == "Cutlass")) found = true;
            if (Game1.player.Items.Any(item => item?.Name == "Forest Sword")) found = true;
            if (Game1.player.Items.Any(item => item?.Name == "Iron Edge")) found = true;

            if (found)
            {
                client.Respond(req, EffectStatus.Failure, "Already have sword");
                return;
            }

            GiveItem(client, req, new MeleeWeapon("0"));
        }

        public static void GiveCookie(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "223");

        public static void GiveSuperMeal(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "237");

        public static void GiveDiamond(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "72");

        public static void GiveCopperBar(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "334");

        public static void GiveIronBar(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "335");

        public static void GiveGoldBar(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "336");

        public static void GiveWood(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "388", 5);

        public static void GiveStone(ControlClient client, EffectRequest req)
            => GiveItem(client, req, "390", 5);

        public static void SantaMSG(ControlClient client, EffectRequest req)
            => SendMail(client, req, "Dear @,^^    You have been very naughty this year, so all you get is this lump of coal.^^                                       -Santa%item object 382 1 %%", "Christmas");

        public static void CarMSG(ControlClient client, EffectRequest req)
            => SendMail(client, req, "Hello @!^^    We have been trying to reach you about your car's extended warranty...", "car");

        public static void PizzaMSG(ControlClient client, EffectRequest req)
            => SendMail(client, req, "I've got a uh... large pepperoni pizza for... @?^^That'll be $21.59.%item object 206 1 %%", "pizza");

        public static void GrowMSG(ControlClient client, EffectRequest req)
            => SendMail(client, req, "Do you have have sad crops that just take forever to grow?!^^You need Miracle Grow, the latest in fertilization technology!^^Try it today!%item object 368 1 %%", "grow");

        public static void LotteryMSG(ControlClient client, EffectRequest req)
            => SendMail(client, req, "Mr. or Mrs. @,^^I am writing to inform you that you have won $100 in the Malaysian National lottery!^^To collect your winnings, please remit a $5 processing fee to the postmarked address.%item money 100 101 %%", "lottery");

        public static void TechMSG(ControlClient client, EffectRequest req)
            => SendMail(client, req, $"Hello, is this @?^^This is {req.viewer} with Microsoft technical support and I am calling you today about a problem we have detected in your computer.^^If you could allow me to remote access your computer I can walk you through fixing the issue for a support fee of $1.99 a minute.", "tech");

        public static void CrowdControlProMSG(ControlClient client, EffectRequest req)
            => SendMail(client, req, "Dear @,^^    This is your reminder to purchase Crowd Control Pro!", "tech");


        private static void GiveItem(ControlClient client, EffectRequest req, Item item)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            Game1.player.addItemByMenuIfNecessary(item);

            client.Respond(req, status, message);
        }

        private static void GiveItem(ControlClient client, EffectRequest req, string item, int qty = 1)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";


            Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(item, qty));

            client.Respond(req, status, message);
        }

        private static void SendMail(ControlClient client, EffectRequest req, string text, string title)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (Game1.activeClickableMenu != null)
            {
                client.Respond(req, EffectStatus.Retry, "Menu is currently open.");
                return;
            }

            Game1.activeClickableMenu = new LetterViewerMenu(text, title, false);

            client.Respond(req, status, message);
        }

        public static void BrownHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(0.4f, 0.2f, 0.1f));

        public static void BlondeHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(1.0f, 0.85f, 0.55f));

        public static void RedHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(1.0f, 0, 0));

        public static void GreenHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(0, 1.0f, 0));

        public static void BlueHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(0, 0, 1.0f));

        public static void YellowHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(1.0f, 1.0f, 0));

        public static void PurpleHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(0.5f, 0, 1.0f));

        public static void OrangeHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(1.0f, 0.45f, 0));

        public static void TealHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(0, 1.0f, 1.0f));

        public static void PinkHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(1.0f, 0.5f, 0.8f));

        public static void BlackHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(0.15f, 0.15f, 0.15f));

        public static void WhiteHair(ControlClient client, EffectRequest req)
            => DoHairColor(client, req, new Color(1.0f, 1.0f, 1.0f));

        private static void DoHairColor(ControlClient client, EffectRequest req, Color c)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            Game1.player.changeHairColor(c);

            UI.ShowInfo($"{req.viewer} changed {Game1.player.Name}'s hair color.");

            client.Respond(req, status, message);
        }

        public static void HairStyle(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            Game1.player.changeHairStyle(new Random().Next(1, 73));

            UI.ShowInfo($"{req.viewer} changed {Game1.player.Name}'s hair style.");

            client.Respond(req, status, message);
        }

        public static void Gender(ControlClient client, EffectRequest req)
        {
            EffectStatus status = EffectStatus.Success;
            string message = "";

            Game1.player.changeGender(!Game1.player.IsMale);

            UI.ShowInfo($"{req.viewer} changed {Game1.player.Name}'s gender.");

            client.Respond(req, status, message);
        }

        private static void DoWarp(ControlClient client, EffectRequest req, string name, int targetX, int targetY)
        {
            //UI.ShowInfo($"Can Move: {Game1.player.canMove} Using Tool: {Game1.player.usingTool}");
            EffectStatus status = EffectStatus.Success;
            string message = "";

            if (Game1.isFestival())
            {
                client.Respond(req, EffectStatus.Failure, "Player is at the festival");
                return;
            }

            if (!Game1.player.canMove || Game1.player.IsBusyDoingSomething() || Game1.player.usingTool.Value)
            {
                client.Respond(req, EffectStatus.Retry, "Player Busy");
                return;
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
                status = EffectStatus.Failure;
            }

            if (name == "Forest")
                name = "Wizard's Tower";
            else if (name == "IslandSouth")
                name = "Island";
            UI.ShowInfo($"{req.viewer} warped {Game1.player.Name} to the {name}");



            client.Respond(req, status, message);
        }

        private const int MAX_RADIUS = 200;
        private const int MAX_SPAWN_ATTEMPTS = 10;

        private static readonly Random RNG = new();

        private static bool TryGetRandomClose(out Vector2 result)
        {
            result = default;
            for (int attempt = 0; attempt < MAX_SPAWN_ATTEMPTS; attempt++)
            {
                result = Game1.player.Position + new Vector2(RNG.Next(-MAX_RADIUS, MAX_RADIUS), RNG.Next(-MAX_RADIUS, MAX_RADIUS));
                if (CanSpawnHere(result)) return true;
            }

            return false;
        }

        public static void HypeTrain(ControlClient client, EffectRequest req)
        {
            if (!ModEntry.Instance.TrySetActive(req, typeof(EffectHypeTrain), out _))
            {
                client.Respond(req, EffectStatus.Failure, "Hype train already in progress.");
                return;
            }

            UI.ShowInfo("A hype train is here! Choo! Choo!");
            client.Respond(req, EffectStatus.Success);
        }
    }
}
