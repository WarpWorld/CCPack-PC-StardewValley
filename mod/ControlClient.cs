/*
 * ControlValley
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

namespace ControlValley
{
    public class ControlClient
    {
        public static readonly string CV_HOST = "127.0.0.1";
        public static readonly int CV_PORT = 51337;

        private static readonly string[] _no_spawn = { "hospital", "islandsouth" };

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
                {"horserace", ControlValley.EffectDelegates.PlayHorseRace},
                {"horseraceend", ControlValley.EffectDelegates.StopHorseRace},

                {"downgrade_axe", ControlValley.EffectDelegates.DowngradeAxe},
                {"downgrade_boots", ControlValley.EffectDelegates.DowngradeBoots},
                {"downgrade_fishingrod", ControlValley.EffectDelegates.DowngradeFishingRod},
                {"downgrade_hoe", ControlValley.EffectDelegates.DowngradeHoe},
                {"downgrade_pickaxe", ControlValley.EffectDelegates.DowngradePickaxe},
                {"downgrade_trashcan", ControlValley.EffectDelegates.DowngradeTrashCan},
                {"downgrade_wateringcan", ControlValley.EffectDelegates.DowngradeWateringCan},
                {"downgrade_weapon", ControlValley.EffectDelegates.DowngradeWeapon},
                {"energize_10", ControlValley.EffectDelegates.Energize10},
                {"energize_25", ControlValley.EffectDelegates.Energize25},
                {"energize_50", ControlValley.EffectDelegates.Energize50},
                {"energize_full", ControlValley.EffectDelegates.EnergizeFull},
                {"give_buff_adrenaline", ControlValley.EffectDelegates.GiveBuffAdrenaline},
                {"give_buff_darkness", ControlValley.EffectDelegates.GiveBuffDarkness},
                {"give_buff_frozen", ControlValley.EffectDelegates.GiveBuffFrozen},
                {"give_buff_invincibility", ControlValley.EffectDelegates.GiveBuffInvincibility},
                {"give_buff_nauseous", ControlValley.EffectDelegates.GiveBuffNauseous},
                {"give_buff_slime", ControlValley.EffectDelegates.GiveBuffSlime},
                {"give_buff_speed", ControlValley.EffectDelegates.GiveBuffSpeed},
                {"give_buff_tipsy", ControlValley.EffectDelegates.GiveBuffTipsy},
                {"give_buff_warrior", ControlValley.EffectDelegates.GiveBuffWarrior},
                {"give_money_100", ControlValley.EffectDelegates.GiveMoney100},
                {"give_money_1000", ControlValley.EffectDelegates.GiveMoney1000},
                {"give_money_10000", ControlValley.EffectDelegates.GiveMoney10000},
                {"give_stardrop", ControlValley.EffectDelegates.GiveStardrop},
                {"heal_10", ControlValley.EffectDelegates.Heal10},
                {"heal_25", ControlValley.EffectDelegates.Heal25},
                {"heal_50", ControlValley.EffectDelegates.Heal50},
                {"heal_full", ControlValley.EffectDelegates.HealFull},
                {"hurt_10", ControlValley.EffectDelegates.Hurt10},
                {"hurt_25", ControlValley.EffectDelegates.Hurt25},
                {"hurt_50", ControlValley.EffectDelegates.Hurt50},
                {"kill", ControlValley.EffectDelegates.Kill},
                {"passout", ControlValley.EffectDelegates.PassOut},
                {"remove_money_100", ControlValley.EffectDelegates.RemoveMoney100},
                {"remove_money_1000", ControlValley.EffectDelegates.RemoveMoney1000},
                {"remove_money_10000", ControlValley.EffectDelegates.RemoveMoney10000},
                {"remove_stardrop", ControlValley.EffectDelegates.RemoveStardrop},
                {"spawn_bat", ControlValley.EffectDelegates.SpawnBat},
                {"spawn_slime", ControlValley.EffectDelegates.SpawnGreenSlime},
                {"spawn_redslime", ControlValley.EffectDelegates.SpawnRedSlime},
                {"spawn_frostjelly", ControlValley.EffectDelegates.SpawnFrostJelly},
                {"spawn_redsludge", ControlValley.EffectDelegates.SpawnRedSludge},
                {"spawn_bluesquid", ControlValley.EffectDelegates.SpawnBlueSquid},
                {"spawn_skelton", ControlValley.EffectDelegates.SpawnSkeleton},
                {"spawn_skeletonmage", ControlValley.EffectDelegates.SpawnSkeletonMage},
                {"spawn_fly", ControlValley.EffectDelegates.SpawnFly},
                {"spawn_frostbat", ControlValley.EffectDelegates.SpawnFrostBat},
                {"spawn_ghost", ControlValley.EffectDelegates.SpawnGhost},
                {"spawn_lavabat", ControlValley.EffectDelegates.SpawnLavaBat},
                {"spawn_serpent", ControlValley.EffectDelegates.SpawnSerpent},
                {"spawn_bomb", ControlValley.EffectDelegates.SpawnBomb},


                {"tire_10", ControlValley.EffectDelegates.Tire10},
                {"tire_25", ControlValley.EffectDelegates.Tire25},
                {"tire_50", ControlValley.EffectDelegates.Tire50},
                {"upgrade_axe", ControlValley.EffectDelegates.UpgradeAxe},
                {"upgrade_backpack", ControlValley.EffectDelegates.UpgradeBackpack},
                {"upgrade_boots", ControlValley.EffectDelegates.UpgradeBoots},
                {"upgrade_fishingrod", ControlValley.EffectDelegates.UpgradeFishingRod},
                {"upgrade_hoe", ControlValley.EffectDelegates.UpgradeHoe},
                {"upgrade_pickaxe", ControlValley.EffectDelegates.UpgradePickaxe},
                {"upgrade_trashcan", ControlValley.EffectDelegates.UpgradeTrashCan},
                {"upgrade_wateringcan", ControlValley.EffectDelegates.UpgradeWateringCan},
                {"upgrade_weapon", ControlValley.EffectDelegates.UpgradeWeapon},
                {"warp_beach", ControlValley.EffectDelegates.WarpBeach},
                {"warp_desert", ControlValley.EffectDelegates.WarpDesert},
                {"warp_farm", ControlValley.EffectDelegates.WarpFarm},
                {"warp_island", ControlValley.EffectDelegates.WarpIsland},
                {"warp_mountain", ControlValley.EffectDelegates.WarpMountain},
                {"warp_railroad", ControlValley.EffectDelegates.WarpRailroad},
                {"warp_sewer", ControlValley.EffectDelegates.WarpSewer},
                {"warp_tower", ControlValley.EffectDelegates.WarpTower},
                {"warp_town", ControlValley.EffectDelegates.WarpTown},
                {"warp_woods", ControlValley.EffectDelegates.WarpWoods},
                {"give_sword", ControlValley.EffectDelegates.GiveSword},
                {"give_cookie", ControlValley.EffectDelegates.GiveCookie},
                {"give_supermeal", ControlValley.EffectDelegates.GiveSuperMeal},
                {"give_diamond", ControlValley.EffectDelegates.GiveDiamond},
                {"give_copperbar", ControlValley.EffectDelegates.GiveCopperBar},
                {"give_ironbar", ControlValley.EffectDelegates.GiveIronBar},
                {"give_goldbar", ControlValley.EffectDelegates.GiveGoldBar},
                {"give_wood", ControlValley.EffectDelegates.GiveWood},
                {"give_stone", ControlValley.EffectDelegates.GiveStone},
                {"msg_santa", ControlValley.EffectDelegates.SantaMSG},
                {"msg_car", ControlValley.EffectDelegates.CarMSG},
                {"msg_pizza", ControlValley.EffectDelegates.PizzaMSG},
                {"msg_grow", ControlValley.EffectDelegates.GrowMSG},
                {"msg_lottery", ControlValley.EffectDelegates.LotteryMSG},
                {"msg_tech", ControlValley.EffectDelegates.TechMSG},
                {"hair_brown", ControlValley.EffectDelegates.BrownHair},
                {"hair_blonde", ControlValley.EffectDelegates.BlondeHair},
                {"hair_red", ControlValley.EffectDelegates.RedHair},
                {"hair_green", ControlValley.EffectDelegates.GreenHair},
                {"hair_blue", ControlValley.EffectDelegates.BlueHair},
                {"hair_yellow", ControlValley.EffectDelegates.YellowHair},
                {"hair_purple", ControlValley.EffectDelegates.PurpleHair},
                {"hair_orange", ControlValley.EffectDelegates.OrangeHair},
                {"hair_teal", ControlValley.EffectDelegates.TealHair},
                {"hair_pink", ControlValley.EffectDelegates.PinkHair},
                {"hair_black", ControlValley.EffectDelegates.BlackHair},
                {"hair_white", ControlValley.EffectDelegates.WhiteHair},
                {"hair_style", ControlValley.EffectDelegates.HairStyle},
                {"gender", ControlValley.EffectDelegates.Gender},


                {"spawn_bug", ControlValley.EffectDelegates.SpawnBug},
                {"spawn_wildernessgolem", ControlValley.EffectDelegates.SpawnWildernessGolem},
                {"give_buff_monstermusk", ControlValley.EffectDelegates.GiveMonsterMuskBuff},
                {"msg_crowdcontrolpro", ControlValley.EffectDelegates.CrowdControlProMSG},
                {"emote_sad", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_heart", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_exclamation", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_note", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_sleep", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_game", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_question", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_x", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_pause", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_blush", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_angry", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_yes", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_no", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_sick", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_laugh", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_taunt", ControlValley.EffectDelegates.PlayerEmote},

                {"emote_surprised", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_hi", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_uh", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_music", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_jar", ControlValley.EffectDelegates.PlayerEmote},
                {"emote_happy", ControlValley.EffectDelegates.PlayerEmote},

                {"divorce", ControlValley.EffectDelegates.Divorce},
                {"removechildren", ControlValley.EffectDelegates.TurnChildrenToDoves},
                {"swimwear_on", ControlValley.EffectDelegates.ChangeSwimClothes},
                {"swimwear_off", ControlValley.EffectDelegates.ChangeSwimClothes},
                
                {"event-hype-train", ControlValley.EffectDelegates.HypeTrain},
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
