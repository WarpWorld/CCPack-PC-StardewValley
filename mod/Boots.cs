﻿/*
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
using StardewBoots = StardewValley.Objects.Boots;

namespace ControlValley
{
    public class Boots
    {
        private static readonly List<IBootTier> boots = new List<IBootTier>
        {
            new MultiBootTier() { "504", "505" },
            new MultiBootTier() { "506", "507" },
            new MultiBootTier() { "508", "509", "510", "806" },
            new MultiBootTier() { "511", "512" },
            new MultiBootTier() { "513", "855" },
            new MultiBootTier() { "514", "804", "878" },
            new SingleBootTier("853"),
            new SingleBootTier("854")
        };

        public static StardewBoots GetDowngrade(string index)
        {
            for (int i = boots.Count - 1; i > 0; --i)
            {
                if (boots[i].Contains(index))
                    return boots[i - 1].GetBoots();
            }

            return null;
        }

        public static StardewBoots GetUpgrade(string index)
        {
            for (int i = 0; i < boots.Count - 1; ++i)
            {
                if (boots[i].Contains(index))
                    return boots[i + 1].GetBoots();
            }

            return null;
        }

        public interface IBootTier
        {
            bool Contains(string index);
            StardewBoots GetBoots();
        }

        class MultiBootTier : List<string>, IBootTier
        {
            private Random Random { get; set; }

            public MultiBootTier() : base() {
                Random = new Random();
            }

            public StardewBoots GetBoots()
            {
                return new StardewBoots(this[Random.Next(this.Count)]);
            }
        }

        class SingleBootTier : IBootTier
        {
            private readonly string index;

            public SingleBootTier(string index)
            {
                this.index = index;
            }

            public bool Contains(string index)
            {
                return index == this.index;
            }

            public StardewBoots GetBoots()
            {
                return new StardewBoots(index);
            }
        }
    }
}
