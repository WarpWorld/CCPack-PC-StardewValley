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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using ConnectorLib.JSON;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ControlValley
{
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public ControlClient? Client { get; private set; }

        public ConcurrentDictionary<Guid, Behavior> ActiveBehaviors { get; } = new();

        public ConcurrentDictionary<Type, Behavior> KnownBehaviors { get; } = new();

        public ModEntry()
        {
            Instance = this;

            foreach (Type type in typeof(Behavior).Assembly.GetTypes())
                if (type.IsSubclassOf(typeof(Behavior)))
                    KnownBehaviors[type] = (Behavior)Activator.CreateInstance(type, this)!;
        }

        public bool TrySetActive(EffectRequest request, Type type, [MaybeNullWhen(false)] out Behavior behavior)
        {
            if (!KnownBehaviors.TryGetValue(type, out behavior)) return false;
            if (!ActiveBehaviors.GetOrAdd(behavior.ID, behavior).Equals(behavior)) return false;
            behavior.Start(request);
            return true;
        }

        public override void Entry(IModHelper helper)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            if (Context.IsMultiplayer)
            {
                Monitor.Log("Crowd Control is unavailable in multiplayer. Skipping mod.", LogLevel.Info);
                return;
            }

            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            foreach (Behavior behavior in ActiveBehaviors.Values)
                behavior.Update(Game1.currentGameTime);
        }

        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            foreach (Behavior behavior in ActiveBehaviors.Values)
                behavior.Draw(Game1.currentGameTime, e.SpriteBatch);
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            if (Client == null) return;
            Helper.Events.GameLoop.Saved -= Client.OnSaved;
            Helper.Events.GameLoop.Saving -= Client.OnSaving;
            Helper.Events.Player.Warped -= Client.OnWarped;
            Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            Helper.Events.Display.Rendered -= OnRendered;
            Client.Stop();
            Client = null;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            if (!Context.IsWorldReady || Client != null) return;
            Client = new ControlClient();
            Helper.Events.GameLoop.Saved += Client.OnSaved;
            Helper.Events.GameLoop.Saving += Client.OnSaving;
            Helper.Events.Player.Warped += Client.OnWarped;
            Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            Helper.Events.Display.Rendered += OnRendered;
            Client.Start();
        }
    }
}
