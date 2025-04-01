/*
 * CrowdControl
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TerribleTable
 * Copyright (C) 2021-2024 Warp World, Inc. (dtothefourth, jaku, KatDevsGames)
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
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConnectorLib.JSON;
using Newtonsoft.Json;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

namespace CrowdControl
{
    public class ControlClient
    {
        public static readonly string CV_HOST = "127.0.0.1";
        public static readonly int CV_PORT = 51337;

        private static readonly string[] _no_spawn = ["hospital", "islandsouth"];

        private Dictionary<string, EffectDelegate> EffectDelegates { get; }
        private Dictionary<string, MetadataDelegate> MetadataDelegates { get; }

        private IPEndPoint Endpoint { get; }
        private Dictionary<GameLocation, List<Monster>> Monsters { get; }
        private Queue<SimpleJSONRequest> Requests { get; }
        private bool Running { get; set; }
        private bool Saving { get; set; }
        private bool Spawn { get; set; }
        public static Socket Socket { get; set; }

        public ControlClient()
        {
            Endpoint = new IPEndPoint(IPAddress.Parse(CV_HOST), CV_PORT);
            Monsters = new Dictionary<GameLocation, List<Monster>>();
            Requests = new Queue<SimpleJSONRequest>();
            Running = true;
            Saving = false;
            Spawn = true;

            EffectDelegates = new Dictionary<string, EffectDelegate>()
            {
                {"horserace", CrowdControl.EffectDelegates.PlayHorseRace},
                {"horseraceend", CrowdControl.EffectDelegates.StopHorseRace},

                {"downgrade_axe", CrowdControl.EffectDelegates.DowngradeAxe},
                {"downgrade_boots", CrowdControl.EffectDelegates.DowngradeBoots},
                {"downgrade_fishingrod", CrowdControl.EffectDelegates.DowngradeFishingRod},
                {"downgrade_hoe", CrowdControl.EffectDelegates.DowngradeHoe},
                {"downgrade_pickaxe", CrowdControl.EffectDelegates.DowngradePickaxe},
                {"downgrade_trashcan", CrowdControl.EffectDelegates.DowngradeTrashCan},
                {"downgrade_wateringcan", CrowdControl.EffectDelegates.DowngradeWateringCan},
                {"downgrade_weapon", CrowdControl.EffectDelegates.DowngradeWeapon},
                {"energize_10", CrowdControl.EffectDelegates.Energize10},
                {"energize_25", CrowdControl.EffectDelegates.Energize25},
                {"energize_50", CrowdControl.EffectDelegates.Energize50},
                {"energize_full", CrowdControl.EffectDelegates.EnergizeFull},
                {"give_buff_adrenaline", CrowdControl.EffectDelegates.GiveBuffAdrenaline},
                {"give_buff_darkness", CrowdControl.EffectDelegates.GiveBuffDarkness},
                {"give_buff_frozen", CrowdControl.EffectDelegates.GiveBuffFrozen},
                {"give_buff_invincibility", CrowdControl.EffectDelegates.GiveBuffInvincibility},
                {"give_buff_nauseous", CrowdControl.EffectDelegates.GiveBuffNauseous},
                {"give_buff_slime", CrowdControl.EffectDelegates.GiveBuffSlime},
                {"give_buff_speed", CrowdControl.EffectDelegates.GiveBuffSpeed},
                {"give_buff_tipsy", CrowdControl.EffectDelegates.GiveBuffTipsy},
                {"give_buff_warrior", CrowdControl.EffectDelegates.GiveBuffWarrior},
                {"give_money_100", CrowdControl.EffectDelegates.GiveMoney100},
                {"give_money_1000", CrowdControl.EffectDelegates.GiveMoney1000},
                {"give_money_10000", CrowdControl.EffectDelegates.GiveMoney10000},
                {"give_stardrop", CrowdControl.EffectDelegates.GiveStardrop},
                {"heal_10", CrowdControl.EffectDelegates.Heal10},
                {"heal_25", CrowdControl.EffectDelegates.Heal25},
                {"heal_50", CrowdControl.EffectDelegates.Heal50},
                {"heal_full", CrowdControl.EffectDelegates.HealFull},
                {"hurt_10", CrowdControl.EffectDelegates.Hurt10},
                {"hurt_25", CrowdControl.EffectDelegates.Hurt25},
                {"hurt_50", CrowdControl.EffectDelegates.Hurt50},
                {"kill", CrowdControl.EffectDelegates.Kill},
                {"passout", CrowdControl.EffectDelegates.PassOut},
                {"remove_money_100", CrowdControl.EffectDelegates.RemoveMoney100},
                {"remove_money_1000", CrowdControl.EffectDelegates.RemoveMoney1000},
                {"remove_money_10000", CrowdControl.EffectDelegates.RemoveMoney10000},
                {"remove_stardrop", CrowdControl.EffectDelegates.RemoveStardrop},
                {"spawn_bat", CrowdControl.EffectDelegates.SpawnBat},
                {"spawn_slime", CrowdControl.EffectDelegates.SpawnGreenSlime},
                {"spawn_redslime", CrowdControl.EffectDelegates.SpawnRedSlime},
                {"spawn_frostjelly", CrowdControl.EffectDelegates.SpawnFrostJelly},
                {"spawn_redsludge", CrowdControl.EffectDelegates.SpawnRedSludge},
                {"spawn_bluesquid", CrowdControl.EffectDelegates.SpawnBlueSquid},
                {"spawn_skelton", CrowdControl.EffectDelegates.SpawnSkeleton},
                {"spawn_skeletonmage", CrowdControl.EffectDelegates.SpawnSkeletonMage},
                {"spawn_fly", CrowdControl.EffectDelegates.SpawnFly},
                {"spawn_frostbat", CrowdControl.EffectDelegates.SpawnFrostBat},
                {"spawn_ghost", CrowdControl.EffectDelegates.SpawnGhost},
                {"spawn_lavabat", CrowdControl.EffectDelegates.SpawnLavaBat},
                {"spawn_serpent", CrowdControl.EffectDelegates.SpawnSerpent},
                {"spawn_bomb", CrowdControl.EffectDelegates.SpawnBomb},


                {"tire_10", CrowdControl.EffectDelegates.Tire10},
                {"tire_25", CrowdControl.EffectDelegates.Tire25},
                {"tire_50", CrowdControl.EffectDelegates.Tire50},
                {"upgrade_axe", CrowdControl.EffectDelegates.UpgradeAxe},
                {"upgrade_backpack", CrowdControl.EffectDelegates.UpgradeBackpack},
                {"upgrade_boots", CrowdControl.EffectDelegates.UpgradeBoots},
                {"upgrade_fishingrod", CrowdControl.EffectDelegates.UpgradeFishingRod},
                {"upgrade_hoe", CrowdControl.EffectDelegates.UpgradeHoe},
                {"upgrade_pickaxe", CrowdControl.EffectDelegates.UpgradePickaxe},
                {"upgrade_trashcan", CrowdControl.EffectDelegates.UpgradeTrashCan},
                {"upgrade_wateringcan", CrowdControl.EffectDelegates.UpgradeWateringCan},
                {"upgrade_weapon", CrowdControl.EffectDelegates.UpgradeWeapon},
                {"warp_beach", CrowdControl.EffectDelegates.WarpBeach},
                {"warp_desert", CrowdControl.EffectDelegates.WarpDesert},
                {"warp_farm", CrowdControl.EffectDelegates.WarpFarm},
                {"warp_island", CrowdControl.EffectDelegates.WarpIsland},
                {"warp_mountain", CrowdControl.EffectDelegates.WarpMountain},
                {"warp_railroad", CrowdControl.EffectDelegates.WarpRailroad},
                {"warp_sewer", CrowdControl.EffectDelegates.WarpSewer},
                {"warp_tower", CrowdControl.EffectDelegates.WarpTower},
                {"warp_town", CrowdControl.EffectDelegates.WarpTown},
                {"warp_woods", CrowdControl.EffectDelegates.WarpWoods},
                {"give_sword", CrowdControl.EffectDelegates.GiveSword},
                {"give_cookie", CrowdControl.EffectDelegates.GiveCookie},
                {"give_supermeal", CrowdControl.EffectDelegates.GiveSuperMeal},
                {"give_diamond", CrowdControl.EffectDelegates.GiveDiamond},
                {"give_copperbar", CrowdControl.EffectDelegates.GiveCopperBar},
                {"give_ironbar", CrowdControl.EffectDelegates.GiveIronBar},
                {"give_goldbar", CrowdControl.EffectDelegates.GiveGoldBar},
                {"give_wood", CrowdControl.EffectDelegates.GiveWood},
                {"give_stone", CrowdControl.EffectDelegates.GiveStone},
                {"msg_santa", CrowdControl.EffectDelegates.SantaMSG},
                {"msg_car", CrowdControl.EffectDelegates.CarMSG},
                {"msg_pizza", CrowdControl.EffectDelegates.PizzaMSG},
                {"msg_grow", CrowdControl.EffectDelegates.GrowMSG},
                {"msg_lottery", CrowdControl.EffectDelegates.LotteryMSG},
                {"msg_tech", CrowdControl.EffectDelegates.TechMSG},
                {"hair_brown", CrowdControl.EffectDelegates.BrownHair},
                {"hair_blonde", CrowdControl.EffectDelegates.BlondeHair},
                {"hair_red", CrowdControl.EffectDelegates.RedHair},
                {"hair_green", CrowdControl.EffectDelegates.GreenHair},
                {"hair_blue", CrowdControl.EffectDelegates.BlueHair},
                {"hair_yellow", CrowdControl.EffectDelegates.YellowHair},
                {"hair_purple", CrowdControl.EffectDelegates.PurpleHair},
                {"hair_orange", CrowdControl.EffectDelegates.OrangeHair},
                {"hair_teal", CrowdControl.EffectDelegates.TealHair},
                {"hair_pink", CrowdControl.EffectDelegates.PinkHair},
                {"hair_black", CrowdControl.EffectDelegates.BlackHair},
                {"hair_white", CrowdControl.EffectDelegates.WhiteHair},
                {"hair_style", CrowdControl.EffectDelegates.HairStyle},
                {"gender", CrowdControl.EffectDelegates.Gender},


                {"spawn_bug", CrowdControl.EffectDelegates.SpawnBug},
                {"spawn_wildernessgolem", CrowdControl.EffectDelegates.SpawnWildernessGolem},
                {"give_buff_monstermusk", CrowdControl.EffectDelegates.GiveMonsterMuskBuff},
                {"msg_crowdcontrolpro", CrowdControl.EffectDelegates.CrowdControlProMSG},
                {"emote_sad", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_heart", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_exclamation", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_note", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_sleep", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_game", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_question", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_x", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_pause", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_blush", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_angry", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_yes", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_no", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_sick", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_laugh", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_taunt", CrowdControl.EffectDelegates.PlayerEmote},

                {"emote_surprised", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_hi", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_uh", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_music", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_jar", CrowdControl.EffectDelegates.PlayerEmote},
                {"emote_happy", CrowdControl.EffectDelegates.PlayerEmote},

                {"divorce", CrowdControl.EffectDelegates.Divorce},
                {"removechildren", CrowdControl.EffectDelegates.TurnChildrenToDoves},
                {"swimwear_on", CrowdControl.EffectDelegates.ChangeSwimClothes},
                {"swimwear_off", CrowdControl.EffectDelegates.ChangeSwimClothes},
                
                {"event-hype-train", CrowdControl.EffectDelegates.HypeTrain},
            };
        }
        public bool HideEffect(string code)
            => Send(new EffectUpdate(code, EffectStatus.NotVisible));

        public bool ShowEffect(string code)
            => Send(new EffectUpdate(code, EffectStatus.Visible));

        public bool DisableEffect(string code)
            => Send(new EffectUpdate(code, EffectStatus.NotSelectable));

        public bool EnableEffect(string code)
            => Send(new EffectUpdate(code, EffectStatus.Selectable));

        private void ClientLoop()
        {
            ModEntry.Instance.Monitor.Log("Connected to Crowd Control");
            UI.ShowInfo("Connected to Crowd Control");
            try
            {
                while (Running)
                {
                    SimpleJSONRequest? req = Recieve(this, Socket);
                    if (req?.IsKeepAlive ?? true)
                    {
                        Thread.Sleep(0); //probably not advisable but better than not doing anything
                        continue;
                    }

                    lock (Requests)
                        Requests.Enqueue(req);
                }
            }
            catch (Exception)
            {
                ModEntry.Instance.Monitor.Log("Disconnected from Crowd Control");
                UI.ShowInfo("Disconnected from Crowd Control");
                Socket?.Close();
            }
        }

        public static readonly int RECV_BUF = 4096;
        public static readonly int RECV_TIME = 5000000;

        public SimpleJSONRequest? Recieve(ControlClient client, Socket socket)
        {
            byte[] buf = new byte[RECV_BUF];
            string content = "";
            int read = 0;

            do
            {
                if (!client.IsRunning()) return null;

                if (socket.Poll(RECV_TIME, SelectMode.SelectRead))
                {
                    read = socket.Receive(buf);
                    if (read < 0) return null;

                    content += Encoding.UTF8.GetString(buf);
                }
                else
                    KeepAlive();
            } while (read == 0 || (read == RECV_BUF && buf[RECV_BUF - 1] != 0));

            if (!SimpleJSONRequest.TryParse(content, out SimpleJSONRequest? request)) return null;
            return request;
        }

        private static readonly EmptyResponse KEEPALIVE = new() { type = ResponseType.KeepAlive };

        public bool KeepAlive() => Send(KEEPALIVE);
        
        public bool CanSpawn() => Spawn;
        public bool IsRunning() => Running;

        public void NetworkLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            int maxAttempts = 3;
            int attempts = 0;

            while (Running && attempts < maxAttempts)
            {
                UI.ShowInfo("Attempting to connect to Crowd Control");
                if (attempts == maxAttempts - 1) UI.ShowError("Final connection attempt. Make sure Crowd Control is running.");


                try
                {
                    Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                        ClientLoop();
                    else
                        UI.ShowError("Failed to connect to Crowd Control");
                    Socket.Close();
                }
                catch (Exception e)
                {
                    UI.ShowError(e.GetType().Name);
                    UI.ShowError("Failed to connect to Crowd Control");
                }

                attempts++;
                Thread.Sleep(5000);
            }
        }

        public async Task KeepAliveLoop()
        {
            while (Running)
            {
                await Task.Delay(2000);
                KeepAlive();
            }
        }

        public void OnSaved(object? sender, SavedEventArgs args)
        {
            Saving = false;
        }

        public void OnSaving(object? sender, SavingEventArgs args)
        {
            Saving = true;
            foreach (KeyValuePair<GameLocation, List<Monster>> pair in Monsters)
            {
                foreach (Monster monster in pair.Value)
                    pair.Key.characters.Remove(monster);
                pair.Value.Clear();
            }
        }

        public void OnWarped(object? sender, WarpedEventArgs args)
        {
            Spawn = Array.IndexOf(_no_spawn, args.NewLocation.Name.ToLower()) < 0;
        }

        private const int LOOP_YIELD_DELAY = 200;//ms
        
        public void RequestLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {
                try
                {
                    while (Saving || Game1.isTimePaused)
                        Thread.Sleep(LOOP_YIELD_DELAY);

                    SimpleJSONRequest req;
                    lock (Requests)
                    {
                        if (Requests.Count == 0)
                            continue;
                        req = Requests.Dequeue();
                    }

                    try
                    {
                        switch (req)
                        {
                            case DataRequest dataRequest:
                            {
                                string code = dataRequest.key;
                                if (!MetadataDelegates.TryGetValue(code, out MetadataDelegate? del))
                                {
                                    Respond(req, EffectStatus.Failure, $"Could not find the effect delegate for ID \"{code}\".");
                                    break;
                                }
                                del(this);
                                break;
                            }
                            case EffectRequest effectRequest:
                            {
                                string code = effectRequest.code;
                                if (!EffectDelegates.TryGetValue(code, out EffectDelegate? del))
                                {
                                    Respond(req, EffectStatus.Failure, $"Could not find the effect delegate for ID \"{code}\".");
                                    break;
                                }
                                del(this, effectRequest);
                                break;
                            }
                        }
                    }
                    catch { Respond(req, EffectStatus.Retry); }
                }
                catch (Exception)
                {
                    UI.ShowError("Disconnected from Crowd Control");
                    Socket.Close();
                }
            }
        }

        public bool Respond(SimpleJSONRequest request, EffectStatus status, string? message = null)
            => Send(new EffectResponse(request.ID, status, 0, message));

        public bool Respond(SimpleJSONRequest request, EffectStatus status, SITimeSpan timeRemaining, string? message = null)
            => Send(new EffectResponse(request.ID, status, (long)timeRemaining.TotalMilliseconds, message));

        public bool Respond(SimpleJSONRequest request, EffectStatus status, long timeRemaining, string? message = null)
            => Send(new EffectResponse(request.ID, status, timeRemaining, message));

        public bool Send(SimpleJSONResponse response)
            => Socket.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response) + "\0")) > 0;

        public void Start()
        {
            Running = true;
            Task.Run(NetworkLoop).Forget();
            Task.Run(RequestLoop).Forget();
            Task.Run(KeepAliveLoop).Forget();
        }

        public void Stop()
        {
            Running = false;
        }

        public void TrackMonster(Monster monster)
        {
            GameLocation location = Game1.player.currentLocation;
            if (!Monsters.ContainsKey(location))
                Monsters[location] = new List<Monster>();
            Monsters[location].Add(monster);
        }
    }
}
