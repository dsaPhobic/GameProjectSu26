# Magic Farm Defender

A 2D top-down action farming game built with Unity 2022.3 LTS for PRU213 course project.

## Concept

You are a magic farmer defending your enchanted crops from nightly monster attacks. Farm during the day, fight at night. Survive 5 days and defeat the Demon Boss.

**Core Loop:** Dawn → Day (farm) → Dusk (prepare) → Night (defend) → Level up → Repeat

## Controls

| Input       | Action                                    |
| ----------- | ----------------------------------------- |
| WASD        | Move                                      |
| Mouse       | Aim                                       |
| Left Click  | Shoot magic bullet                        |
| Right Click | Use farm tool                             |
| Space       | Dash                                      |
| 1 / 2 / 3   | Switch tools (Hoe / Watering Can / Sword) |

## Features

- **Day/Night Cycle** — 4 phases (Dawn, Day, Dusk, Night) with automatic BGM transitions
- **Farming System** — Till, water, and harvest 3 crop types (Wheat, Pumpkin, Magic Herb) on a 10×10 grid
- **Wave System** — Enemy waves scale in HP and count each day
- **4 Enemy Types** — Slime, Goblin Archer, Beast, Demon Boss (3-phase boss fight)
- **Level Up & Perks** — Gain XP to unlock upgrades (Max HP, Speed, Damage, Drone companion, etc.)
- **Power-ups** — 6 random drops during combat (Speed, Double Damage, Shield, Health Restore, Growth Magic, Magnet)
- **Save System** — JSON save/load via Continue button in Main Menu
- **Drone Companion** — Child GameObject that auto-targets and shoots nearby enemies

## Game Over Conditions

- Player HP reaches 0
- All crops are destroyed

## Scenes

| Index | Scene     | Description                          |
| ----- | --------- | ------------------------------------ |
| 0     | MainMenu  | Start, Continue, Settings, Quit      |
| 1     | GameScene | Main gameplay                        |
| 2     | GameOver  | Stats display with Retry / Main Menu |

## Tech Stack

- **Engine**: Unity 2022.3.62f3 LTS
- **Language**: C# (.NET)
- **Patterns**: Service Locator, Event Bus, State Machine, Object Pooling, ScriptableObject data-driven design

## Project Structure

```
Assets/_Project/
├── Scripts/
│   ├── Core/          # GameManager, ServiceLocator, GameEvents, SaveSystem, SceneLoader
│   ├── Player/        # PlayerController, PlayerInput, PlayerStats, PlayerAnimator, Bullet
│   ├── Combat/        # Enemy base class + Slime, GoblinArcher, Beast, DemonBoss
│   ├── Farming/       # FarmManager, FarmTile, Crop, CropData
│   ├── World/         # WaveManager, DayNightCycle, PowerUp, GameOverDetector
│   ├── Upgrade/       # UpgradeManager, PerkSystem, UpgradeData
│   ├── UI/            # HUD, MainMenu, GameOver, PauseMenu, UpgradeScreen
│   ├── Audio/         # AudioManager
│   └── Polish/        # CameraFollow, CameraShake, HitFlash, DamageNumber
├── Prefabs/
├── Scenes/
├── Sprites/
├── Data/              # ScriptableObject assets
└── Audio/
```

## Team

| Member                        | Role                                                               |
| ----------------------------- | ------------------------------------------------------------------ |
| Nguyen Nhat Quang (Xuntacdor) | Tech Lead — Scene Integration, Player mechanics, Camera, Bug fixes |
| Ha Minh Quang (dsaPhobic)     | Core Architecture — All C# scripts, Implementation Plan            |
| Nguyen Thanh Quan (WaanTh)    | Enemy Prefabs, Farming System, Wave ScriptableObjects              |
| Pham Tan Hai(PhmHai0702)      | UI Scenes, Audio, PowerUp & Upgrade data                           |

## Repository

https://github.com/dsaPhobic/GameProjectSu26

**Branch strategy:** `feature/*` → PR → `develop` → `main`
