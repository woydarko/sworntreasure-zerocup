# Sworn Treasure

Sworn Treasure is a casual 2D digging game made with Unity. You play as a small dog that digs underground, manages paw durability, collects points, and discovers Indonesian artifacts hidden at different depths.

The project was created as a student game project and focuses on simple gameplay, readable UI, resource management, and light cultural education through artifact discovery.

## Gameplay Overview

The player starts on the surface with full energy. From there, the player can dig down, left, or right through a tile-based underground map. Each dig uses paw durability, so the player has to manage energy carefully while collecting points and looking for rare artifacts.

When the player finds an artifact, the game gives points, shows an alert with the artifact rarity color, and plays a dog bark sound as a reward feedback.

## Features

- Grid-based 2D digging system
- Paw durability / energy management
- Points from gold, diamond, and artifact rewards
- Indonesian artifact discovery system
- Artifact rarity, depth range, chance, and reward values
- Shop for buying energy and character skins
- Info / Bag panel for artifact information
- Back to Top button with gameplay conditions
- Non-blocking alert UI
- Digging sound effects and particle feedback
- Dog bark sound when an artifact is found
- WebGL-friendly UI scaling for itch.io

## Controls

| Input | Action |
|---|---|
| A | Dig / move left |
| S | Dig / move down |
| D | Dig / move right |
| Shop Button | Open the shop when on the surface |
| Info Button | Open artifact information |
| Back to Top Button | Return to the surface when conditions are met |

## Core Loop

1. Start on the surface.
2. Dig underground tiles.
3. Use energy with every dig.
4. Collect points, energy, or artifacts.
5. Go deeper to find rarer artifacts.
6. Use Back to Top when energy runs out or the player is deep enough.
7. Buy energy in the Shop.
8. Dig again and repeat.

## Artifact System

Artifacts are inspired by Indonesian cultural objects. Each artifact has its own rarity, recommended depth, spawn chance, and point reward.

| Rarity | Artifact | Main Depth | Reward |
|---|---|---:|---:|
| Common | Pecahan Tembikar Trowulan | 0-40 | +30 |
| Uncommon | Koin Gobog Wayang | 40-80 | +40 |
| Rare | Kapak Corong Perunggu | 80-120 | +60 |
| Ultra Rare | Arca Perunggu Ganesha | 120-150 | +75 |
| Mythical | Bokor Emas Wonoboyo | 150-300 | +100 |

## Rules and Conditions

- Shop can only be opened on the surface.
- Back to Top can be used when energy is empty or the player is deeper than depth 50.
- Back to Top does not automatically refill energy.
- Energy can be restored from energy tiles or by buying energy in the Shop.
- Alert UI should not block other buttons.
- Artifact bark sounds only play when an artifact is found.

## Built With

- Unity 2D
- C#
- Unity Tilemap
- TextMesh Pro

## Project Structure

```text
Assets/              Unity assets, scenes, scripts, sprites, audio, and prefabs
Packages/            Unity package manifest
ProjectSettings/     Unity project settings
Flowchart_*.md/png   Gameplay and system flowcharts
Proposal_*.md/docx   Project proposal documentation
```

## How to Run in Unity

1. Clone this repository.
2. Open the project folder in Unity.
3. Open the main menu scene from `Assets/Scenes`.
4. Press Play in the Unity Editor.

Recommended Unity target:

```text
Platform: Windows or WebGL
Resolution: 1280 x 720
Color Space: Gamma
```

## Build Targets

### WebGL

Used for playing directly on itch.io.

Recommended settings:

```text
Platform: WebGL
Default Canvas Width: 1280
Default Canvas Height: 720
Compression Format: Disabled
Data Caching: On
```

### Windows

Used for executable builds.

Recommended settings:

```text
Platform: Windows, Mac, Linux
Target Platform: Windows
Architecture: x86_64
Fullscreen Mode: Windowed
Default Screen Width: 1280
Default Screen Height: 720
```

## Testing Notes

Main tested areas:

- Movement and digging
- Energy usage and restoration
- Point collection
- Artifact reward and bark audio
- Shop surface restriction
- Back to Top conditions
- Alert UI behavior
- WebGL UI scaling

Known fixed issues:

- Alert panel blocking other UI buttons
- Shop opening one block below the surface
- Back to Top being spammable on the surface
- UI size mismatch in WebGL

## Documentation

This repository includes supporting project documents:

- `Proposal_Sworn_Treasure_Revisi.md`
- `Flowchart_Sworn_Treasure.md`
- `flowchart_gameplay.png`
- `flowchart_artefak.png`
- `flowchart_backtotop.png`

## Status

Prototype / student project.

The current version is playable and focuses on the main digging loop, artifact discovery, energy management, and UI feedback.

## Author

Nabil Pratama Hidayat

