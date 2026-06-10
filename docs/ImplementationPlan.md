# MAGIC FARM DEFENDER — KẾ HOẠCH TRIỂN KHAI CHI TIẾT

> Dựa trên GDD v1.0 + 16 yêu cầu PRU213  
> Mục tiêu điểm: **9–10**  
> Engine: Unity 2022.3.62f3 LTS | C# OOP

---

## MỤC LỤC

1. [Mapping 16 Yêu cầu → Module](#1-mapping-16-yêu-cầu)
2. [Assets cần chuẩn bị](#2-assets-cần-chuẩn-bị)
3. [Cấu trúc dự án](#3-cấu-trúc-dự-án)
4. [Kiến trúc code (Architecture)](#4-kiến-trúc-code)
5. [Thứ tự implement (Dependency Graph)](#5-thứ-tự-implement)
6. [Plan chi tiết từng tuần](#6-plan-chi-tiết-từng-tuần)
7. [Quy trình làm việc hàng ngày](#7-quy-trình-làm-việc)
8. [Extra features cho điểm 10](#8-extra-features)

---

## 1. MAPPING 16 YÊU CẦU

| #   | Yêu cầu                                   | Giải pháp trong game                                  | File chính                          | Người |
| --- | ----------------------------------------- | ----------------------------------------------------- | ----------------------------------- | ----- |
| 1   | Input bàn phím điều khiển hướng/hành động | WASD move + Mouse aim + 1/2/3 tool + Space dash       | `PlayerInput.cs`                    | P2    |
| 2   | Game Over                                 | Player chết **hoặc** tất cả crop bị phá               | `GameOverDetector.cs`               | P3+P4 |
| 3   | Animation, Collision, Coroutine           | Animator crop growth + Physics2D hit + WaitForSeconds | Toàn bộ                             | All   |
| 4   | Hiển thị điểm (score)                     | Gold counter + XP bar + Day counter trên HUD          | `HUDController.cs`                  | P4    |
| 5   | Âm thanh                                  | BGM day/night + SFX attack/harvest/levelup            | `AudioManager.cs`                   | P4    |
| 6   | C# + OOP                                  | Entity → Player/Enemy inheritance + Interfaces        | Toàn bộ                             | All   |
| 7   | Nhiều scene (≥2)                          | MainMenu + GameScene + GameOver (3 scene)             | `SceneLoader.cs`                    | P1    |
| 8   | Độ khó tăng dần                           | Wave ngày sau nhiều enemy hơn, HP/damage scale        | `WaveManager.cs`                    | P3    |
| 9   | Promotion (buff)                          | Power-up drops: Speed/DoubleDmg/Shield/HealthRestore  | `PowerUp.cs`                        | P3    |
| 10  | Upgrade nhân vật trong scene              | Level-up → chọn perk → stats thay đổi ngay            | `UpgradeManager.cs`                 | P4    |
| 11  | Thay đổi appearance                       | Dragon Armor upgrade → đổi sprite Player              | `PlayerAnimator.cs`                 | P2+P4 |
| 12  | Spawn object mới                          | Enemy spawn ban đêm + Crop spawn khi trồng            | `WaveManager.cs` + `FarmManager.cs` | P3    |
| 13  | Child object hỗ trợ                       | Drone Companion là child của Player, tự bắn           | `Drone.cs`                          | P4    |
| 14  | Save/Load                                 | JSON serialize game state → persist giữa session      | `SaveSystem.cs`                     | P1    |
| 15  | GitHub                                    | Branch strategy: main/develop/feature/\*              | Git workflow                        | P1    |
| 16  | Extra features                            | Boss fight, Shop, Hub Town, Achievement, Leaderboard  | (xem Phần 8)                        | All   |

---

## 2. ASSETS CẦN CHUẨN BỊ

### 2.1 Download ngay (tất cả MIỄN PHÍ)

> Tất cả đã được lưu tại Google Drive: https://drive.google.com/drive/folders/1WB6Sgto-bJQuglTEzoX1SjKBnsOZtVP2

| Loại        | Pack                                     | Link                                                           | Dùng cho                         |
| ----------- | ---------------------------------------- | -------------------------------------------------------------- | -------------------------------- |
| **Player**  | Hero Knight (Penzilla)                   | https://penzilla.itch.io/hero-knight                           | Player sprite + animation        |
| **Enemy**   | Monsters Creatures Fantasy (LuizMelo)    | https://luizmelo.itch.io/monsters-creatures-fantasy            | Slime, Goblin, Beast, Demon Boss |
| **Tileset** | Cozy Farm Asset Pack (shubibubi)         | https://shubibubi.itch.io/cozy-farm                            | Farm tiles + crop sprites        |
| **Tileset** | Free Stylized Top-Down Grass (Stealthix) | https://stealthix.itch.io/grass                                | Grass background                 |
| **Tileset** | Pixel Crawler Dungeon (Anokolisa)        | https://anokolisa.itch.io/dungeon-crawler-pixel-art-asset-pack | Dungeon/forest border            |
| **UI**      | Kenney UI RPG                            | https://kenney.nl/assets/ui-pack-rpg-expansion                 | Buttons, panels, icons           |
| **Effects** | Battle Effects Pack 1 (Pimen)            | https://pimen.itch.io/battle-effects-pack-1                    | Particle, hit, fire, frost       |
| **SFX**     | Mixkit Free Game SFX                     | https://mixkit.co/free-sound-effects/game/                     | Click, shoot, hit, harvest       |
| **BGM**     | OpenGameArt                              | https://opengameart.org/                                       | Day theme, night theme, boss     |

### 2.2 Sprite cần slice/prepare trong Unity

```
Player (Hero Knight):
  - Idle (4 frame)         → PlayerIdle
  - Run (8 frame)          → PlayerRun
  - Attack (6 frame)       → PlayerAttack
  - Dash (4 frame)         → PlayerDash
  - Death (6 frame)        → PlayerDeath
  - DragonArmor variants   → req #11 (appearance change)

Enemies:
  - Slime: Idle, Walk, Attack, Death
  - Goblin: Idle, Walk, Attack, Death
  - Beast: Idle, Run, Attack, Death
  - Demon Boss: Idle, Walk, Phase1Attack, Phase2Attack, Phase3Berserk, Death

Crops (3 loại × 4 stage):
  - Wheat:     Stage0(seed) → Stage1 → Stage2 → Stage3(mature)
  - Pumpkin:   Stage0 → Stage1 → Stage2 → Stage3
  - MagicHerb: Stage0 → Stage1 → Stage2 → Stage3 (GLOW effect)

Tiles:
  - Empty dirt
  - Tilled dirt
  - Watered dirt (darker)

UI Icons:
  - Tool icons: Hoe, WateringCan, Sword
  - Power-up icons: Speed, Damage, Shield, Health, Growth, Magnet
  - Upgrade card backgrounds
```

### 2.3 Audio cần chuẩn bị

```
BGM (nhạc nền):
  bgm_main_menu.mp3       ← calm, cozy
  bgm_day.mp3             ← upbeat farming
  bgm_night.mp3           ← tense, dark
  bgm_boss.mp3            ← epic battle
  bgm_game_over.mp3       ← sad/dramatic

SFX (hiệu ứng âm thanh):
  sfx_shoot.wav           ← bắn đạn
  sfx_hit_enemy.wav       ← trúng enemy
  sfx_hit_player.wav      ← player bị đánh
  sfx_enemy_die.wav       ← enemy chết
  sfx_harvest.wav         ← thu hoạch cây
  sfx_water.wav           ← tưới cây
  sfx_plant.wav           ← trồng hạt
  sfx_levelup.wav         ← lên level
  sfx_powerup_pickup.wav  ← nhặt power-up
  sfx_dash.wav            ← dash
  sfx_button_click.wav    ← UI click
  sfx_boss_phase.wav      ← boss đổi phase
  sfx_drone_shoot.wav     ← drone bắn
```

---

## 3. CẤU TRÚC DỰ ÁN

```
Assets/
├── _Project/                          ← TẤT CẢ code/asset TEAM đặt ở đây
│   │
│   ├── Animations/
│   │   ├── Player/
│   │   │   ├── Player.animator
│   │   │   ├── PlayerIdle.anim
│   │   │   ├── PlayerRun.anim
│   │   │   ├── PlayerAttack.anim
│   │   │   ├── PlayerDash.anim
│   │   │   └── PlayerDeath.anim
│   │   ├── Enemies/
│   │   │   ├── Slime.animator
│   │   │   ├── Goblin.animator
│   │   │   ├── Beast.animator
│   │   │   └── DemonBoss.animator
│   │   └── Crops/
│   │       ├── Wheat.animator
│   │       ├── Pumpkin.animator
│   │       └── MagicHerb.animator
│   │
│   ├── Audio/
│   │   ├── BGM/
│   │   └── SFX/
│   │
│   ├── Data/                          ← ScriptableObjects (data-driven)
│   │   ├── Crops/
│   │   │   ├── SO_Wheat.asset
│   │   │   ├── SO_Pumpkin.asset
│   │   │   └── SO_MagicHerb.asset
│   │   ├── Enemies/
│   │   │   ├── SO_Slime.asset
│   │   │   ├── SO_Goblin.asset
│   │   │   ├── SO_Beast.asset
│   │   │   └── SO_DemonBoss.asset
│   │   ├── Waves/
│   │   │   ├── SO_Wave_Day1.asset
│   │   │   ├── SO_Wave_Day2.asset
│   │   │   └── ...
│   │   ├── Upgrades/
│   │   │   ├── SO_Upgrade_Damage.asset
│   │   │   ├── SO_Upgrade_AttackSpeed.asset
│   │   │   └── ... (15+ upgrades)
│   │   └── PowerUps/
│   │       ├── SO_PowerUp_Speed.asset
│   │       └── ...
│   │
│   ├── Prefabs/
│   │   ├── Player/
│   │   │   ├── Player.prefab
│   │   │   ├── Bullet.prefab
│   │   │   └── Drone.prefab            ← child of Player (req #13)
│   │   ├── Enemies/
│   │   │   ├── Slime.prefab
│   │   │   ├── Goblin.prefab
│   │   │   ├── Beast.prefab
│   │   │   └── DemonBoss.prefab
│   │   ├── Crops/
│   │   │   ├── Wheat.prefab
│   │   │   ├── Pumpkin.prefab
│   │   │   └── MagicHerb.prefab
│   │   ├── PowerUps/
│   │   │   └── PowerUp.prefab
│   │   └── UI/
│   │       ├── DamageNumber.prefab
│   │       ├── UpgradeCard.prefab
│   │       └── FloatingText.prefab
│   │
│   ├── Scenes/
│   │   ├── MainMenu.unity              ← Scene 1
│   │   ├── GameScene.unity             ← Scene 2 (main gameplay)
│   │   ├── HubTown.unity               ← Scene 3 (optional, extra)
│   │   └── GameOver.unity              ← Scene 4
│   │
│   ├── Scripts/
│   │   │
│   │   ├── Core/                       ← P1 phụ trách
│   │   │   ├── Interfaces/
│   │   │   │   ├── IDamageable.cs
│   │   │   │   ├── IInteractable.cs
│   │   │   │   ├── IUpgradable.cs
│   │   │   │   └── ISaveable.cs
│   │   │   ├── Enums/
│   │   │   │   ├── DayPhase.cs         (Dawn/Day/Dusk/Night)
│   │   │   │   ├── GameState.cs        (Menu/Playing/Paused/GameOver)
│   │   │   │   ├── CropType.cs
│   │   │   │   ├── EnemyType.cs
│   │   │   │   ├── ToolType.cs
│   │   │   │   ├── TileState.cs
│   │   │   │   ├── CropStage.cs
│   │   │   │   └── UpgradeCategory.cs
│   │   │   ├── GameEvents.cs           ← Event Bus (decoupling)
│   │   │   ├── ServiceLocator.cs       ← DI Container
│   │   │   ├── GameManager.cs          ← State machine
│   │   │   ├── SceneLoader.cs          ← Scene transitions
│   │   │   ├── SaveSystem.cs           ← JSON save/load (req #14)
│   │   │   └── Entity.cs               ← Base class mọi entity
│   │   │
│   │   ├── Player/                     ← P2 phụ trách
│   │   │   ├── PlayerController.cs     ← Main, kế thừa Entity
│   │   │   ├── PlayerInput.cs          ← Input handling (req #1)
│   │   │   ├── PlayerStats.cs          ← HP/XP/Level/Gold
│   │   │   ├── PlayerAnimator.cs       ← Animation + Appearance (req #11)
│   │   │   ├── PlayerToolHandler.cs    ← Hoe/Water/Sword logic
│   │   │   └── Bullet.cs               ← Projectile
│   │   │
│   │   ├── Farming/                    ← P3 phụ trách
│   │   │   ├── CropData.cs             ← ScriptableObject
│   │   │   ├── Crop.cs                 ← Coroutine growth (req #3)
│   │   │   ├── FarmTile.cs             ← Tile state machine
│   │   │   └── FarmManager.cs          ← Grid 10x10
│   │   │
│   │   ├── Combat/                     ← P2 phụ trách
│   │   │   ├── Enemy.cs                ← Abstract base class
│   │   │   ├── Slime.cs                ← Melee enemy
│   │   │   ├── GoblinArcher.cs         ← Ranged enemy
│   │   │   ├── Beast.cs                ← Fast melee
│   │   │   ├── DemonBoss.cs            ← 3 phases boss
│   │   │   ├── EnemyPool.cs            ← Object pooling (req #12)
│   │   │   ├── EnemyData.cs            ← ScriptableObject
│   │   │   └── DamageCalculator.cs     ← Tính damage (critical, burn...)
│   │   │
│   │   ├── World/                      ← P3 phụ trách
│   │   │   ├── DayNightCycle.cs        ← 4 phases Dawn/Day/Dusk/Night
│   │   │   ├── WaveManager.cs          ← Spawn waves (req #8, #12)
│   │   │   ├── EnemyWaveData.cs        ← ScriptableObject
│   │   │   ├── SpawnPoint.cs           ← Marker spawn locations
│   │   │   ├── PowerUp.cs              ← Power-up logic (req #9)
│   │   │   ├── PowerUpData.cs          ← ScriptableObject
│   │   │   ├── PowerUpSpawner.cs       ← Spawn ngẫu nhiên
│   │   │   └── GameOverDetector.cs     ← Detect game over (req #2)
│   │   │
│   │   ├── Upgrade/                    ← P4 phụ trách
│   │   │   ├── UpgradeData.cs          ← ScriptableObject
│   │   │   ├── UpgradeManager.cs       ← Apply lên Player (req #10)
│   │   │   └── PerkSystem.cs           ← Random 3 perks từ pool
│   │   │
│   │   ├── Companion/                  ← P4 phụ trách
│   │   │   ├── Drone.cs                ← Child object (req #13)
│   │   │   └── Scarecrow.cs            ← Placed companion
│   │   │
│   │   ├── UI/
│   │   │   ├── HUD/
│   │   │   │   ├── HUDController.cs    ← HP/XP/Gold (req #4)
│   │   │   │   ├── DayCounter.cs
│   │   │   │   ├── WaveIndicator.cs
│   │   │   │   └── PowerUpTimerUI.cs
│   │   │   ├── Menu/
│   │   │   │   ├── MainMenuController.cs
│   │   │   │   ├── PauseMenuController.cs
│   │   │   │   ├── GameOverScreen.cs   ← Req #2
│   │   │   │   └── SettingsMenu.cs
│   │   │   └── UpgradeUI/
│   │   │       ├── UpgradeScreen.cs    ← 3 cards chọn 1 (req #10)
│   │   │       └── UpgradeCard.cs
│   │   │
│   │   ├── Audio/                      ← P4 phụ trách
│   │   │   └── AudioManager.cs         ← Singleton (req #5)
│   │   │
│   │   └── Polish/                     ← P4 phụ trách
│   │       ├── CameraShake.cs
│   │       ├── DamageNumber.cs         ← Số damage bay lên
│   │       └── HitFlash.cs             ← Flash khi bị đánh
│   │
│   ├── Sprites/
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Crops/
│   │   ├── Tiles/
│   │   ├── UI/
│   │   └── Effects/
│   │
│   └── Materials/
│       └── MagicGlow.mat               ← MagicHerb glow effect
│
└── ThirdParty/                         ← Asset Store packages (không sửa)
```

---

## 4. KIẾN TRÚC CODE

### 4.1 Class Hierarchy (Inheritance — req #6 OOP)

```
MonoBehaviour
├── Entity (abstract)                   ← Base cho mọi thứ có HP
│   ├── PlayerController                ← implements IDamageable, ISaveable
│   └── Enemy (abstract)               ← implements IDamageable
│       ├── Slime
│       ├── GoblinArcher
│       ├── Beast
│       └── DemonBoss
│
├── Crop                                ← implements IDamageable, ISaveable
├── FarmTile
├── Bullet                              ← implements IPoolable
├── PowerUp
└── Drone                              ← Child of Player (req #13)

ScriptableObject
├── CropData                           ← name, growTime, sellPrice, HP
├── EnemyData                          ← HP, damage, speed, XP, gold
├── EnemyWaveData                      ← list<SpawnEntry>, dayNumber
├── UpgradeData                        ← id, name, description, category
└── PowerUpData                        ← type, duration, magnitude
```

### 4.2 Interfaces

```csharp
// IDamageable.cs
public interface IDamageable {
    void TakeDamage(int damage);
    bool IsDead { get; }
}

// IInteractable.cs
public interface IInteractable {
    void Interact(PlayerToolHandler player);
    bool CanInteract(ToolType tool);
}

// IUpgradable.cs
public interface IUpgradable {
    void ApplyUpgrade(UpgradeData upgrade);
}

// ISaveable.cs
public interface ISaveable {
    object GetSaveData();
    void LoadSaveData(object data);
}
```

### 4.3 Event Bus (GameEvents.cs) — Decoupling

```csharp
// Tất cả events trong game — tránh coupling trực tiếp
public static class GameEvents {
    // Player events
    public static event Action<int> OnPlayerHPChanged;
    public static event Action<int> OnPlayerXPChanged;
    public static event Action<int> OnGoldChanged;
    public static event Action<int> OnPlayerLevelUp;
    public static event Action OnPlayerDied;

    // Game phase events
    public static event Action<DayPhase> OnDayPhaseChanged;
    public static event Action<int> OnDayChanged;
    public static event Action<int> OnWaveStarted;
    public static event Action OnWaveCompleted;

    // Combat events
    public static event Action<Enemy> OnEnemyDied;
    public static event Action OnAllCropsDestroyed;

    // Upgrade events
    public static event Action OnLevelUpScreenOpen;

    // Raise methods
    public static void RaiseGoldChanged(int gold) => OnGoldChanged?.Invoke(gold);
    // ... etc
}
```

### 4.4 GameManager — State Machine

```
States: MainMenu → Playing → Paused → LevelUp → GameOver
                                     ↑
                              (Upgrade screen)

Playing sub-states: Dawn → Day → Dusk → Night → Dawn → ...
```

### 4.5 Save Data Structure (req #14)

```csharp
[Serializable]
public class GameSaveData {
    // Player
    public int playerHP;
    public int playerMaxHP;
    public int playerLevel;
    public int playerXP;
    public int gold;
    public List<string> appliedUpgradeIds;
    public bool hasDragonArmor;         // req #11 appearance
    public bool hasDrone;               // req #13 child object

    // Farm
    public List<TileSaveData> tiles;

    // World
    public int currentDay;
    public float dayTimer;

    // Meta
    public int totalEnemiesKilled;
    public int totalGoldEarned;
    public System.DateTime saveTime;
}
```

---

## 5. THỨ TỰ IMPLEMENT (Dependency Graph)

```
LAYER 0 — Setup (Tuần 1, cả team)
─────────────────────────────────
[Git repo] → [Folder structure] → [Unity project settings]
                                        ↓
[Core/Enums] → [Core/Interfaces] → [Entity base class]

LAYER 1 — Foundation (Tuần 2, song song)
─────────────────────────────────────────
P1: [GameEvents] → [ServiceLocator]
P2: [PlayerInput] → [PlayerController (movement only)]
P3: [CropData SO] → [Crop (growth coroutine)]
P4: [MainMenu scene] → [SceneLoader]

LAYER 2 — Core Gameplay (Tuần 3-4)
────────────────────────────────────
P1: [GameManager state machine]
P2: [Bullet] → [PlayerStats] → [Combat: hit detection]
P3: [FarmTile] → [FarmManager grid] → [PlayerToolHandler]
P4: [HUDController] → [GameEvents subscribe]

Phụ thuộc: PlayerToolHandler cần PlayerController xong
           HUD cần GameEvents xong

LAYER 3 — Enemy & World (Tuần 4-5)
────────────────────────────────────
P2: [Enemy base] → [Slime] → [GoblinArcher] → [Beast] → [EnemyPool]
P3: [DayNightCycle] → [WaveManager] → [EnemyWaveData SO]
P1: [SaveSystem] → [GameSaveData DTO]

Phụ thuộc: WaveManager cần Enemy xong
           DayNightCycle cần GameManager xong

LAYER 4 — Progression (Tuần 5-6)
───────────────────────────────────
P4: [UpgradeData SO] → [UpgradeScreen] → [UpgradeManager] → [PerkSystem]
P3: [PowerUp] → [PowerUpSpawner] → [GameOverDetector]
P4: [Drone companion] ← cần PlayerController xong

Phụ thuộc: UpgradeManager cần PlayerStats xong
           Drone cần Player prefab xong

LAYER 5 — Polish & Boss (Tuần 7-8)
────────────────────────────────────
P2: [DemonBoss 3 phases]
P4: [AudioManager] → [gán SFX/BGM]
P4: [CameraShake] → [DamageNumber] → [HitFlash]
P1: [Full Save/Load integration]

LAYER 6 — Final (Tuần 9-10)
─────────────────────────────
Performance, bug fixes, balance, build
```

---

## 6. PLAN CHI TIẾT TỪNG TUẦN

---

### TUẦN 1: SETUP & FOUNDATION 📦

**Mục tiêu:** Git sạch, project structure hoàn chỉnh, cả team code được.

#### P1 — Tech Lead:

```
[ ] Tạo GitHub repo "magic-farm-defender"
[ ] Setup .gitignore Unity (Library/, Temp/, Builds/, *.csproj, *.sln)
[ ] Tạo folder structure đầy đủ (tạo .gitkeep để git track empty folder)
[ ] Branch strategy: main + develop + branch protection
[ ] Tạo GitHub Project board (Kanban: Todo/InProgress/Review/Done)
[ ] Code Core/Enums/ (tất cả enum: DayPhase, GameState, CropType, ...)
[ ] Code Core/Interfaces/ (IDamageable, IInteractable, IUpgradable, ISaveable)
[ ] Code Entity.cs (base class với HP, TakeDamage, Die)
[ ] Push lên develop, tạo PR, merge
```

#### P2, P3, P4:

```
[ ] Clone repo, cài Unity 2022.3.62f1
[ ] Xem Brackeys 2D Game Tutorial Series (8 video)
[ ] Tự làm mini test: spawn object, di chuyển WASD
[ ] Đọc về: Coroutine, ScriptableObject, Animator
[ ] Download và import assets (xem Phần 2.1)
```

**Milestone:** ✅ Repo sạch ✅ Folder structure ✅ Enums + Interfaces ✅ Cả team setup xong

---

### TUẦN 2: FIRST PLAYABLE 🎮

**Mục tiêu:** Nhìn thấy Player di chuyển trên màn hình.

#### P1:

```
[ ] Code GameEvents.cs (static class với tất cả events)
[ ] Code ServiceLocator.cs (Register<T>/Get<T>)
[ ] Code SceneLoader.cs (LoadScene async với loading screen)
[ ] Tạo 3 scene trống: MainMenu, GameScene, GameOver
```

#### P2:

```
[ ] Import Hero Knight sprites, slice spritesheet
[ ] Setup Player prefab: GameObject + Rigidbody2D + Collider2D + SpriteRenderer
[ ] Code PlayerInput.cs:
    - ReadMovementInput() → Vector2 (WASD)  [req #1]
    - ReadAimDirection() → Vector2 (mouse world pos)
    - ReadAttackInput() → bool (Left Click)
    - ReadToolInput() → bool (Right Click)
    - ReadDashInput() → bool (Space)
    - ReadToolSwitch() → int (1/2/3)
[ ] Code PlayerController.cs:
    - Movement với Rigidbody2D.MovePosition
    - Flip sprite khi đi trái/phải
    - Dash với cooldown
[ ] Test: Player chạy, flip, dash trong scene trống
```

#### P3:

```
[ ] Code CropData.cs (ScriptableObject):
    - cropName, cropType, growthTime, sellPrice, XP, maxHP
    - Sprite[] stageSprites (4 stage)
[ ] Tạo 3 SO assets: SO_Wheat, SO_Pumpkin, SO_MagicHerb
[ ] Code Crop.cs:
    - Coroutine GrowthCoroutine() — WaitForSeconds [req #3]
    - Sprite thay đổi theo stage
    - TakeDamage() implements IDamageable
[ ] Test: 1 cây mọc qua 4 stage tự động
```

#### P4:

```
[ ] Tạo MainMenu scene với Canvas
[ ] Code MainMenuController.cs:
    - Button Play → GameScene
    - Button Settings → modal
    - Button Quit → Application.Quit
[ ] Setup UI fonts (Kenney pack)
[ ] Test: bấm Play → vào GameScene
```

**Milestone:** ✅ Player di chuyển ✅ Cây mọc ✅ Main Menu chạy

---

### TUẦN 3: CORE GAMEPLAY LOOP 🌱⚔️

**Mục tiêu:** Vòng lặp farming cơ bản hoạt động.

#### P1:

```
[ ] Code GameManager.cs:
    - State machine: Menu → Playing → Paused → LevelUp → GameOver
    - Singleton pattern (nhưng không abusive — dùng ServiceLocator)
    - OnApplicationPause() để handle pause
[ ] Setup PR review process: checklist cho PR
```

#### P2:

```
[ ] Import + slice bullet sprite
[ ] Code Bullet.cs:
    - Prefab với Rigidbody2D + CircleCollider2D + trigger
    - Move forward theo direction
    - OnTriggerEnter2D → gọi IDamageable.TakeDamage
    - Destroy sau 5 giây (hoặc return pool)
[ ] Thêm shooting vào PlayerController:
    - Tạo BulletPool (ObjectPool<Bullet>)   [performance]
    - Instantiate/Get bullet từ pool
    - Hướng theo mouse
[ ] Code PlayerStats.cs:
    - currentHP, maxHP, damage, attackSpeed, moveSpeed
    - Property với getter/setter, raise events khi thay đổi
    - TakeDamage() → raise OnPlayerHPChanged
    - Die() → raise OnPlayerDied
[ ] Test: Player bắn đạn theo hướng chuột
```

#### P3:

```
[ ] Code TileState.cs enum: Empty, Tilled, Watered, Planted
[ ] Code FarmTile.cs:
    - State machine: Empty → Tilled → Watered/Planted
    - SetState(TileState) → đổi sprite tile
    - IInteractable: CanInteract, Interact
[ ] Code FarmManager.cs:
    - Grid 10×10 với Array2D<FarmTile>
    - GetTileAt(Vector2 worldPos) → FarmTile
    - Tạo 100 FarmTile GameObjects trong Awake
[ ] Code PlayerToolHandler.cs:
    - currentTool (Hoe/WateringCan/Sword)
    - SwitchTool(1/2/3)
    - UseTool() → raycast tới FarmTile gần nhất
[ ] Test: Player cày → trồng → tưới → cây mọc
```

#### P4:

```
[ ] Code HUDController.cs:
    - Thanh HP (Image.fillAmount)  [req #4]
    - Thanh XP
    - Text gold counter
    - Subscribe: OnPlayerHPChanged, OnPlayerXPChanged, OnGoldChanged
[ ] Setup Canvas trong GameScene: HUD layer
[ ] Test: HUD cập nhật khi stats thay đổi
```

**Milestone:** ✅ Farming loop hoàn chỉnh ✅ Bắn đạn ✅ HUD hiển thị stats

---

### TUẦN 4: ENEMY & SAVE 👹💾

**Mục tiêu:** Enemy đầu tiên, save/load hoạt động.

#### P1:

```
[ ] Code SaveSystem.cs:
    - Save(GameSaveData) → JSON → Application.persistentDataPath
    - Load() → đọc JSON → deserialize GameSaveData
    - DeleteSave()
[ ] Code GameSaveData.cs (DTO với [Serializable])
[ ] Unit test: save → load → verify data khớp
```

#### P2:

```
[ ] Import Slime sprites, setup Animator:
    - States: Idle → Walk → Attack → Death  [req #3 animation]
    - Parameters: IsMoving (bool), IsAttacking (bool), IsDead (bool)
[ ] Code Enemy.cs (abstract):
    - Kế thừa Entity, implements IDamageable
    - abstract: AttackTarget(), GetTarget()
    - Coroutine: AttackCoroutine()
    - State machine enum: Idle, Chase, Attack, Dead
[ ] Code Slime.cs:
    - GetTarget() → tìm Crop gần nhất
    - Move tới target
    - Attack khi trong range
    - Die() → drop gold + XP, raise event  [req #12 spawn drops]
[ ] Test: Slime đi tới cây, đánh cây, cây mất HP
```

#### P3:

```
[ ] Code EnemyWaveData.cs ScriptableObject:
    - dayNumber, List<SpawnEntry> (enemyType, count, interval)
[ ] Tạo SO_Wave_Day1, Day2, Day3
[ ] Code WaveManager.cs:
    - LoadWaveData(dayNumber)
    - SpawnEnemy() → Instantiate tại SpawnPoint
    - Coroutine SpawnWave()
[ ] Code SpawnPoint.cs (empty marker script)
[ ] Đặt 4 SpawnPoint ở 4 góc map
[ ] Test: Wave spawn đúng số enemy đúng vị trí
```

#### P4:

```
[ ] Code UpgradeData.cs ScriptableObject:
    - id, displayName, description, category (Combat/Defense/Utility)
    - upgradeEffect enum + value
[ ] Tạo 10 SO upgrade assets (5 combat, 5 defense)
[ ] Code UpgradeScreen.cs:
    - Show 3 random cards  [req #10]
    - OnCardSelected → gọi UpgradeManager
[ ] Code UpgradeCard.cs (hiển thị 1 perk)
```

**Milestone:** ✅ Slime tấn công farm ✅ Wave spawn ✅ Save/Load test pass

---

### TUẦN 5: DAY/NIGHT & MORE ENEMIES 🌙

**Mục tiêu:** Full day/night cycle, 3 loại enemy.

#### P1:

```
[ ] Integrate SaveSystem với PlayerStats, FarmManager
[ ] Code Pause functionality:
    - Time.timeScale = 0 khi pause
    - PauseMenu show/hide
```

#### P2:

```
[ ] Code GoblinArcher.cs:
    - Keep distance với Player (orbit behavior)
    - Bắn Arrow (projectile riêng, damage=12)
    - Range check trước khi bắn
[ ] Code Beast.cs:
    - Target Player (không target crop)
    - Charge: tăng speed × 1.5 khi cách < 5 unit
    - Mạnh nhưng chậm turning
[ ] Code EnemyPool.cs:
    - ObjectPool<Enemy> cho từng loại
    - Get() / Return()
[ ] Test: 3 loại enemy hoạt động đúng AI
```

#### P3:

```
[ ] Code DayNightCycle.cs:
    - Timer: Dawn(15s) → Day(90s) → Dusk(15s) → Night(120s) → loop
    - Raise OnDayPhaseChanged khi chuyển phase
    - Update ambient light (2DLight intensity)
    - Night: gọi WaveManager.StartWave()
    - Day: enable farming, disable enemy spawn
[ ] Code difficulty scaling trong WaveManager:
    - dayNumber tăng → enemy count × 1.2, HP × 1.1
    - Sau day 5: thêm Goblin vào wave
    - Sau day 10: thêm Beast vào wave  [req #8]
[ ] Test: Day/Night cycle chạy tự động
```

#### P4:

```
[ ] Code UpgradeManager.cs:
    - ApplyUpgrade(UpgradeData) → switch case theo effect
    - Tăng PlayerStats.damage, attackSpeed, maxHP, ...
    - Lưu list appliedUpgrades  [req #10]
[ ] Code PerkSystem.cs:
    - Pool tất cả UpgradeData
    - GetRandomThree() → 3 upgrades không trùng nhau
[ ] Test: Level up → 3 card → chọn → stats tăng ngay
[ ] Code DamageNumber.cs:
    - Tween số lên trên, fade out
    - Pool DamageNumber prefabs
```

**Milestone:** ✅ 3 enemy ✅ Day/Night cycle ✅ Level up + perk system

---

### TUẦN 6: COMPANION & POWER-UPS ✨

**Mục tiêu:** Drone companion, power-up drops.

#### P1:

```
[ ] Code review toàn team (checklist: naming, access modifiers, no magic numbers)
[ ] Fix bugs critical từ tuần trước
[ ] Refactor: tách code quá dài (>100 dòng) thành helper classes
```

#### P2:

```
[ ] Polish combat:
    - HitFlash (SpriteRenderer color flash) khi bị đánh
    - Screen shake khi boss đánh  [polish]
    - Enemy knockback khi bị bắn
[ ] Improve enemy AI với proper State Machine:
    - EnemyState enum: Idle, Patrol, Chase, Attack, Flee, Dead
    - Transition logic rõ ràng
[ ] Thêm Dragon Armor appearance change:
    - UpgradeData "DragonArmor" → PlayerAnimator.SetArmored(true)
    - PlayerAnimator load sprite sheet khác  [req #11]
```

#### P3:

```
[ ] Code PowerUpData.cs ScriptableObject
[ ] Code PowerUp.cs:
    - OnPickup(Player) → apply effect ngay
    - Duration timer → revert sau khi hết
    - Visual: icon + glow + floating animation  [req #9]
[ ] Code PowerUpSpawner.cs:
    - OnEnemyDied → 10% chance spawn PowerUp
    - Có thể rơi dù ngẫu nhiên mỗi 30s ban đêm
[ ] 6 loại: Speed, DoubleDmg, Shield, HealthRestore, GrowthMagic, Magnet
[ ] Test: Kill enemy → power-up rơi → nhặt → buff active → hết hạn
```

#### P4:

```
[ ] Code Drone.cs:
    - Là child object của Player prefab  [req #13]
    - Orbit quanh Player (sin/cos rotation)
    - Tự tìm Enemy gần nhất trong range 8
    - Bắn bullet mỗi 2s
    - Tắt/bật khi mua upgrade "Summon Drone"
[ ] Code PowerUpTimerUI.cs:
    - Hiển thị countdown bar khi buff active
[ ] Test: Drone bay quanh, bắn enemy
```

**Milestone:** ✅ Drone companion ✅ Power-ups ✅ Dragon Armor appearance change

---

### TUẦN 7: SCENES & STORY 🎬

**Mục tiêu:** Full scene flow, shop system.

#### P1:

```
[ ] Code full scene flow:
    MainMenu → [Play] → GameScene
    GameScene → [Die/AllCropsDestroyed] → GameOver
    GameOver → [Retry] → GameScene (fresh)
    GameOver → [MainMenu] → MainMenu
[ ] DontDestroyOnLoad cho GameManager, AudioManager
[ ] Test: full flow không bị null reference
```

#### P2:

```
[ ] Balance enemy stats dựa trên playtest
[ ] Bug fixes từ feedback P3, P4
[ ] Thêm melee attack animation cho Player (Sword tool)
```

#### P3:

```
[ ] (Optional) Tạo HubTown scene đơn giản:
    - NPC Merchant: dialogue → mở Shop
    - Portal từ Farm → HubTown
[ ] Code simple NPC dialogue (không cần full system):
    - Array<string> dialogueLines, hiển thị tuần tự
[ ] Code GameOverDetector.cs:
    - Subscribe OnPlayerDied → trigger GameOver
    - Đếm living crops, OnAllCropsDestroyed → trigger GameOver
```

#### P4:

```
[ ] Code GameOverScreen.cs:
    - Hiển thị: Days Survived, Gold Earned, Enemies Killed  [req #4]
    - Buttons: Retry, Main Menu
[ ] Code PauseMenuController.cs:
    - Esc → Time.timeScale=0, show panel
    - Resume, Settings, Quit
[ ] Code SettingsMenu.cs:
    - Volume sliders (BGM / SFX)
    - Fullscreen toggle
```

**Milestone:** ✅ Full scene flow ✅ Game Over đầy đủ ✅ Pause/Settings

---

### TUẦN 8: BOSS FIGHT & SAVE/LOAD HOÀN THIỆN 👑

**Mục tiêu:** Boss ấn tượng, save/load production-ready.

#### P1:

```
[ ] Full save/load integration:
    - Save khi: qua dawn (mỗi ngày)
    - Load khi: vào GameScene từ "Continue" button
    - Hiển thị save time trong Main Menu
[ ] Test: play → save → quit → reopen → load → đúng state
```

#### P2:

```
[ ] Code DemonBoss.cs:
    Phase 1 (100% → 50% HP):
        - Melee slam (CircleCollider AoE)
        - Tốc độ bình thường
    Phase 2 (50% → 20% HP):
        - Summon 2 Slime mỗi 10s  [req #12 spawn objects]
        - Bắn 3 projectile theo hình quạt
        - Thay đổi màu sprite → báo hiệu phase
    Phase 3 (<20% HP):
        - Berserk: speed × 2, damage × 1.5
        - Particle effect (lửa, tia sét)
        - BGM đổi sang boss phase 3 track
[ ] Boss spawn: Day 7 hoặc day cuối cùng của wave set
[ ] Test: boss fight đầy đủ 3 phases
```

#### P3:

```
[ ] Polish farming:
    - Particle khi tưới cây (water droplet effect)
    - Particle khi thu hoạch (gold sparkle)
    - MagicHerb: Point Light 2D tím phát sáng
[ ] Visual feedback day/night:
    - Global Light 2D: ban ngày sáng → dusk vàng cam → đêm xanh tím
    - Cinemachine vignette khi đêm (optional)
```

#### P4:

```
[ ] Polish upgrade screen:
    - Animation card slide in
    - Hover effect trên card
    - Sound effect khi chọn
[ ] Code Achievement system (bonus điểm):
    - "First Blood": Kill enemy đầu tiên
    - "Survivor": Sống sót 5 ngày
    - "Exterminator": Kill 100 enemy
    - Hiển thị popup khi đạt
```

**Milestone:** ✅ Boss fight 3 phases ✅ Save/Load hoàn hảo ✅ Achievements

---

### TUẦN 9: AUDIO & POLISH ✨🔊

**Mục tiêu:** Game đầy đủ âm thanh, visual polish chuyên nghiệp.

#### P1:

```
[ ] Performance optimization:
    - Profile với Unity Profiler
    - Object pooling cho tất cả bullets, particles
    - Giới hạn enemy count tối đa (30)
[ ] Fix memory leaks (unsubscribe events trong OnDestroy)
[ ] Build test trên máy khác
```

#### P2:

```
[ ] Polish combat:
    - Screen shake khi player bị đánh (CameraShake.cs)
    - Hit pause: Time.timeScale = 0.1f trong 0.05s khi hit
    - Particle explosion khi enemy chết
    - Bullet trail effect (Line Renderer)
```

#### P3:

```
[ ] Polish farming:
    - Ripple animation trên watered tile
    - Level up flash effect toàn màn hình
[ ] Ambient sounds:
    - Ngày: chim hót, gió
    - Đêm: howling, insect sounds
```

#### P4:

```
[ ] Code AudioManager.cs:
    - Singleton + DontDestroyOnLoad
    - PlayBGM(clip, fade=true) → smooth transition
    - PlaySFX(clip, volume=1f) → AudioSource.PlayOneShot
    - Mixer: BGM volume + SFX volume riêng
[ ] Import và gán 20+ SFX vào đúng chỗ:
    - PlayerController.cs → sfx_shoot, sfx_dash, sfx_hit
    - Enemy.cs → sfx_enemy_die
    - Crop.cs → sfx_harvest, sfx_water, sfx_plant
    - UpgradeManager.cs → sfx_levelup
    - PowerUp.cs → sfx_powerup_pickup
[ ] BGM transitions theo DayPhase:
    - Dawn: fade out night, fade in day
    - Night: fade out day, fade in night
    - Boss spawn: fade in boss BGM
[ ] Test: toàn bộ SFX/BGM hoạt động đúng  [req #5]
```

**Milestone:** ✅ Đủ âm thanh ✅ Visual polish ✅ Performance ổn

---

### TUẦN 10: BUILD & DEMO 🎬

**Mục tiêu:** Build .exe sẵn sàng nộp, demo ấn tượng.

#### Cả team:

```
[ ] Bug bash day (2 tiếng): mỗi người chơi → list bugs
[ ] Fix all P0/P1 bugs
[ ] Balance final: enemy HP/damage, crop timing, upgrade cost
[ ] Họp prep demo: ai nói phần gì
```

#### P1:

```
[ ] Build → Windows x64 .exe
[ ] Test trên 2 máy khác nhau
[ ] Viết README.md:
    - Game description
    - How to play
    - Screenshots
    - Team members + roles
    - Architecture overview
    - Build instructions
[ ] GitHub Release với .exe + source
```

#### P2:

```
[ ] Full playthrough test (Day 1 → Boss → Win)
[ ] Quay video gameplay 2 phút
[ ] Document combat system cho slide
```

#### P3:

```
[ ] Test all gameplay loops (farming, waves, power-ups)
[ ] Document farming + world system cho slide
[ ] Vẽ architecture diagram
```

#### P4:

```
[ ] Làm slide thuyết trình (10-15 slides):
    1. Title + Team
    2. Game Concept (30s)
    3. 16 Requirements mapping
    4. Architecture diagram
    5. Demo screenshots
    6. Extra features
    7. What we learned
[ ] Quay trailer 1-2 phút (screen record + edit)
[ ] Tutorial text in-game cho người mới
```

**Milestone:** ✅ .exe sẵn sàng ✅ Slide + Video ✅ README ✅ DEMO DAY!

---

## 7. QUY TRÌNH LÀM VIỆC

### 7.1 Daily (mỗi ngày có code)

```bash
# 1. Sync code mới nhất
git checkout develop
git pull origin develop

# 2. Tạo branch mới
git checkout -b feature/ten-tinh-nang

# 3. Code + commit thường xuyên (mỗi 30-60 phút)
git add Assets/_Project/Scripts/...   # KHÔNG git add .
git commit -m "feat: add slime chase AI"

# 4. Push và tạo PR
git push origin feature/ten-tinh-nang
# → Tạo PR trên GitHub, tag P1 review
```

### 7.2 Commit Message Convention

```
feat: thêm tính năng mới
fix: sửa bug
refactor: tái cấu trúc code (không thêm feature)
style: format code
docs: cập nhật docs
chore: việc vặt (.gitignore, settings)
test: thêm test

Ví dụ:
feat: add goblin archer with ranged AI
fix: drone not stopping when player dies
refactor: extract damage calculation to DamageCalculator
```

### 7.3 PR Checklist (P1 review)

```
[ ] Code build không lỗi
[ ] Không có public field không cần thiết
[ ] Không có magic numbers (dùng [SerializeField] float)
[ ] Event unsubscribe trong OnDestroy
[ ] Không có FindObjectOfType trong Update()
[ ] Naming convention đúng (PascalCase class, _camelCase private)
[ ] Không edit scene chính (chỉ edit prefab riêng)
```

### 7.4 Scene Management Rule

```
GameScene.unity là SHARED — KHÔNG ai edit cùng lúc!

Quy tắc:
- Mỗi người làm trên PREFAB riêng
- Chỉ P1 mới được edit GameScene trực tiếp
- Muốn add object vào scene → nói P1 add prefab vào
- Thông báo trên Discord khi bắt đầu/kết thúc edit scene
```

### 7.5 Weekly Sync Agenda

```
Mỗi tuần (30-60 phút):
1. Demo: mỗi người show feature vừa làm (5 phút)
2. Blockers: có ai cần giúp không?
3. Review milestone tuần này đạt chưa?
4. Plan tuần tới: ai làm gì?
5. Dependencies: có phải đợi nhau không?
```

---

## 8. EXTRA FEATURES (Điểm 10)

### Priority 1 — Nên làm (điểm cao nhất)

| Feature                 | Mô tả                                   | Tuần | Người |
| ----------------------- | --------------------------------------- | ---- | ----- |
| **Boss Fight 3 Phases** | Demon Boss đổi AI theo HP               | T8   | P2    |
| **Dragon Armor**        | Upgrade đổi appearance player hoàn toàn | T6   | P2+P4 |
| **Achievement System**  | 5-10 achievement với popup              | T8   | P4    |
| **Local Leaderboard**   | Top 10 high score lưu JSON              | T9   | P1    |
| **Difficulty Selector** | Easy/Normal/Hard ở Main Menu            | T7   | P3+P4 |

### Priority 2 — Nếu có thời gian

| Feature            | Mô tả                                     | Tuần | Người |
| ------------------ | ----------------------------------------- | ---- | ----- |
| **Shop System**    | Mua upgrade vĩnh viễn bằng gold ở HubTown | T7   | P4    |
| **Hub Town Scene** | Scene thứ 3 với NPC + shop                | T7   | P3    |
| **Minimap**        | RenderTexture minimap góc phải            | T9   | P4    |
| **Inventory UI**   | Xem items đang có                         | T8   | P4    |
| **Scarecrow**      | Companion đặt tại farm, làm chậm enemy    | T8   | P4    |

### Priority 3 — Bonus tốt

| Feature                   | Mô tả                                       |
| ------------------------- | ------------------------------------------- |
| **Particle System đẹp**   | Glow herb, harvest sparkle, death explosion |
| **Screen Shake**          | Camera shake khi boss đánh                  |
| **Hit Pause**             | 0.05s freeze khi hit mạnh                   |
| **Day Transition Effect** | Fade to black + Day X text                  |
| **Tutorial**              | First-time dialogue guide                   |

---

## 9. CHECKLIST TRƯỚC KHI NỘP

### Code Quality

```
[ ] Không có TODO/FIXME còn sót
[ ] Không có Debug.Log trong production code
[ ] Tất cả magic number → [SerializeField]
[ ] Event unsubscribe đầy đủ
[ ] Không có null reference exception khi chơi bình thường
```

### 16 Requirements

```
[ ] 1.  WASD + Mouse + Keys hoạt động
[ ] 2.  Game Over screen hiện ra đúng
[ ] 3.  Animation chạy, collision đúng, coroutine không memory leak
[ ] 4.  HUD hiển thị HP, XP, Gold, Day
[ ] 5.  SFX + BGM đầy đủ
[ ] 6.  Tất cả script là C#, có inheritance + interface
[ ] 7.  ≥ 3 scenes khác nhau
[ ] 8.  Day 1 dễ hơn Day 7 rõ ràng
[ ] 9.  Power-up drops + timer + effect
[ ] 10. Level up → chọn perk → stats thay đổi ngay
[ ] 11. Dragon Armor → sprite thay đổi
[ ] 12. Enemy spawn ban đêm, crop spawn khi trồng
[ ] 13. Drone là child object, tự bắn
[ ] 14. Save + Load hoạt động giữa các session
[ ] 15. GitHub có commit history rõ ràng, branch đúng quy trình
[ ] 16. ≥ 3 extra features hoạt động
```

### Build

```
[ ] Build Windows x64 chạy được
[ ] Không crash trong 10 phút đầu
[ ] FPS ổn định ≥ 60 trên máy trung bình
[ ] Resolution đúng (1920×1080)
[ ] Fullscreen/Windowed hoạt động
```

---

## 10. QUICK START — LÀM GÌ ĐẦU TIÊN?

```
Ngay bây giờ (Tuần 1, Ngày 1):

P1:  git init + folder structure + Enums + Interfaces → push develop
P2:  Download assets → Import → Slice sprites → PlayerInput.cs
P3:  Download Cozy Farm pack → CropData SO → Crop.cs growth coroutine
P4:  Tạo MainMenu scene → Canvas → 3 buttons → SceneLoader test

Không nên làm trước:
❌ Đừng làm UI đẹp trước khi có gameplay
❌ Đừng làm Boss trước khi có Enemy base
❌ Đừng làm Save trước khi có data cần save
❌ Đừng polish trước khi feature xong
```

---

_Plan version: 1.0 | Ngày tạo: 2026-05-22 | Owner: Team 3_
