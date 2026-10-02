# Stardew Valley

## Pack metadata

- **Game:** Stardew Valley
- **Crowd Control game ID:** `StardewValley`
- **Connector:** `SimpleTCPServerConnector`
- **Port:** `51337`
- **Mod framework:** SMAPI 4.1.10 or later

This repository contains the Crowd Control pack and the **CrowdControl** SMAPI
mod for Stardew Valley. The mod connects to the Crowd Control desktop app over
the local JSON connector.

## Requirements

- Stardew Valley with SMAPI installed.
- SMAPI 4.1.10 or later, as declared by the included mod manifest.
- Crowd Control with the **Stardew Valley** pack selected.

## Installation and setup

1. Copy the included `CrowdControl` folder into Stardew Valley's `Mods`
   directory. The folder must contain `manifest.json`, `CrowdControl.dll`, and
   the included Crowd Control DLL dependencies.
2. Launch Stardew Valley through SMAPI.
3. Start the Crowd Control desktop app and select Stardew Valley.
4. Load a save and return to normal gameplay before accepting effects.

## Connection behavior

The mod connects to `127.0.0.1:51337`, where the Crowd Control pack listens.
It reports loading while saving, pauses effects during menus or when time is
paused, and reports a cutscene during festivals. Effects are ready only during
an eligible gameplay state.

## Troubleshooting

- **The mod does not appear in SMAPI:** verify that the `CrowdControl` folder
  is directly under `Mods`, not nested an extra directory deep.
- **The app does not connect:** start Crowd Control, select the Stardew Valley
  pack, then restart the game/mod so it can connect locally on port `51337`.
- **Effects are waiting:** close menus and wait until saving, festivals, and
  other paused states have ended.
