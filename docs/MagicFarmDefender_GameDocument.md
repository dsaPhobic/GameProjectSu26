# 🌾⚔️ MAGIC FARM DEFENDER

## Game Design Document & Team Workflow

**Môn học:** PRU213 \- Game Programming with C\# **Thời gian:** 10 tuần **Engine:** Unity 2022.3.62f3 LTS **Team:** 4 người **Ngôn ngữ:** C\# (OOP)

---

# 📖 PHẦN 1: GAME OVERVIEW

## 1.1 Concept

**Magic Farm Defender** là game 2D top-down kết hợp giữa **farming simulation** và **action combat**, lấy cảm hứng từ **Atomicrops** và **Farmfence**.

### Story:

Người chơi là một **pháp sư nông dân** sống ở vùng đất ma thuật. Ban ngày, họ trồng cây phép thuật để bán lấy vàng. Ban đêm, quái vật từ rừng tối tấn công nông trại \- người chơi phải bảo vệ mùa màng và sống sót qua đêm để tiếp tục phát triển.

### Core Loop:

🌅 Bình minh → 🌞 Ngày (farming) → 🌆 Hoàng hôn (chuẩn bị) →

🌙 Đêm (combat) → 🌄 Bình minh (lên cấp) → Lặp lại

## 1.2 Target Player Experience

- **Cảm giác progression:** Mỗi đêm sống sót → mạnh hơn, farm lớn hơn
- **Tension cycle:** Yên bình ngày → căng thẳng đêm → thỏa mãn sáng hôm sau
- **Strategic depth:** Cân nhắc trồng cây nào, nâng cấp gì, ưu tiên defense hay offense

## 1.3 Unique Selling Points

1. **Day/Night cycle** với 2 gameplay khác biệt
2. **Magic theme** \- cây phát sáng, đạn phép, companion
3. **Upgrade tree** với nhiều build khác nhau
4. **Boss fight** kết thúc ấn tượng

---

# 🎮 PHẦN 2: GAMEPLAY MECHANICS

## 2.1 Player Character

### Stats cơ bản (Level 1):

| Stat          | Giá trị | Mô tả                  |
| :------------ | :------ | :--------------------- |
| Max HP        | 100     | Máu tối đa             |
| Move Speed    | 5       | Tốc độ di chuyển       |
| Attack Damage | 10      | Sát thương cơ bản      |
| Attack Speed  | 2/s     | Số phát bắn mỗi giây   |
| Dash Cooldown | 3s      | Hồi chiêu lướt         |
| Pickup Range  | 1.5     | Bán kính nhặt vật phẩm |

### Controls:

| Key         | Action                         |
| :---------- | :----------------------------- |
| W A S D     | Di chuyển                      |
| Mouse       | Hướng aim                      |
| Left Click  | Tấn công (bắn đạn phép)        |
| Right Click | Sử dụng tool (Hoe/Water/Sword) |
| 1, 2, 3     | Chuyển tool                    |
| Space       | Dash (lướt nhanh)              |
| E           | Tương tác (NPC, portal)        |
| Esc         | Pause menu                     |
| I           | Mở inventory                   |

### Tools:

1. **Hoe (Cuốc)** \- Cày đất Empty → Tilled
2. **Watering Can (Bình tưới)** \- Tưới đất Tilled hoặc cây đang lớn
3. **Sword (Kiếm)** \- Thu hoạch cây Mature, đánh enemy cận chiến

## 2.2 Farming System

### Tile States:

Empty (đất hoang) → \[Hoe\] → Tilled (đã cày) →

\[Plant Seed\] → Planted → \[Water\] → Watered →

\[Time passes\] → Crop grows → Mature (sẵn sàng thu hoạch)

### 3 loại cây cơ bản:

#### 🌾 Wheat (Lúa mì)

- Thời gian lớn: **20 giây**
- HP: 25
- Giá bán: 10 gold
- XP: 5

#### 🎃 Pumpkin (Bí ngô)

- Thời gian lớn: **40 giây**
- HP: 45
- Giá bán: 25 gold
- XP: 15

#### 🌿 Magic Herb (Thảo dược ma thuật)

- Thời gian lớn: **60 giây**
- HP: 15 (dễ bị phá hủy\!)
- Giá bán: 50 gold
- XP: 30
- Phát sáng tím

### Grid size: 10x10 (100 ô đất)

## 2.3 Combat System

### Enemy Types:

#### 🟢 Slime (Cấp 1 \- đơn giản nhất)

- HP: 30
- Damage: 8
- Speed: Chậm (2.0)
- AI: Đi thẳng tới crop gần nhất, đánh melee
- XP drop: 5
- Gold drop: 3

#### 🏹 Goblin Archer (Cấp 2 \- tầm xa)

- HP: 40
- Damage: 12 (đạn tầm xa)
- Speed: Trung bình (3.0)
- Range: 7 unit
- AI: Giữ khoảng cách với player, bắn arrow
- XP drop: 10
- Gold drop: 8

#### 🐺 Beast (Cấp 3 \- tank)

- HP: 80
- Damage: 20
- Speed: Nhanh (4.0)
- AI: Charge tới player, melee mạnh
- XP drop: 20
- Gold drop: 15

#### 👹 Demon Boss (Boss cuối)

- HP: 500
- 3 phases (100%, 50%, 20% HP)
- Phase 1: Đánh melee mạnh
- Phase 2: Triệu hồi minion \+ đánh xa
- Phase 3: Berserk \- tốc độ x2, damage x1.5

## 2.4 Day/Night Cycle

| Phase    | Thời gian | Hoạt động                                      |
| :------- | :-------- | :--------------------------------------------- |
| 🌅 Dawn  | 15s       | Hiển thị "Day X", chuẩn bị, không enemy        |
| 🌞 Day   | 90s       | Farming time \- trồng, tưới, thu hoạch         |
| 🌆 Dusk  | 15s       | Cảnh báo "Night incoming\!", buff trước combat |
| 🌙 Night | 120s      | Combat \- enemy spawn theo wave                |

**Tổng 1 ngày \= 4 phút**

## 2.5 Upgrade System

Khi level up, chọn 1 trong 3 perk ngẫu nhiên từ pool:

### Combat Upgrades:

- **\+20% Damage** (Tăng sát thương)
- **\+15% Attack Speed** (Bắn nhanh hơn)
- **\+10% Critical Chance** (Tỷ lệ chí mạng)
- **Piercing Shot** (Đạn xuyên 2 enemy)
- **Multi Shot** (Bắn 3 viên hình nón)
- **Flame Bullets** (Đạn gây Burn 5s)
- **Frost Bullets** (Đạn làm chậm 30%)

### Defense Upgrades:

- **\+25 Max HP** (Tăng máu)
- **\+5 HP Regen/s** (Hồi máu mỗi giây)
- **Dragon Armor** (Đổi sprite player → \+50% max HP)
- **Shield Burst** (Bất tử 2s khi dưới 25% HP)

### Utility Upgrades:

- **\+15% Move Speed** (Đi nhanh hơn)
- **Reduced Dash Cooldown** (Lướt nhiều hơn)
- **\+1 Pickup Range** (Hút item từ xa)
- **Lucky Farmer** (Cây ra \+50% gold)
- **Green Thumb** (Cây lớn nhanh hơn 20%)

### Special Upgrades:

- **Summon Drone** (Triệu hồi drone bay theo, tự bắn) ⭐ Yêu cầu 13
- **Scarecrow** (Đặt bù nhìn bảo vệ farm)
- **Magic Wall** (Tạo tường ma thuật quanh farm)

## 2.6 Power-ups (Buff tạm thời)

Drop ngẫu nhiên từ enemy hoặc rơi dù xuống:

| Power-up          | Hiệu ứng                  | Thời gian |
| :---------------- | :------------------------ | :-------- |
| ⚡ Speed Boost    | \+50% Move Speed          | 20s       |
| 🔥 Double Damage  | x2 Damage                 | 15s       |
| 🛡️ Shield         | Bất tử                    | 10s       |
| 💚 Health Restore | Hồi 30 HP                 | Instant   |
| 🌱 Growth Magic   | Cây lớn x3 tốc độ         | 30s       |
| 🧲 Magnet         | Tự động hút loot trong 5m | 15s       |

---

# 🎨 PHẦN 3: ART & ASSETS

## 3.1 Art Style

- **Pixel art 16x16** hoặc **32x32**
- **Top-down 2D**
- **Color palette:** Tươi sáng cho ngày, tối/tím cho đêm
- **Magic theme:** Hiệu ứng phát sáng, particle

## 3.2 Asset Packs (MIỄN PHÍ, đã chọn sẵn)

File Assets: [PRU213-Project-Group3](https://drive.google.com/drive/folders/1WB6Sgto-bJQuglTEzoX1SjKBnsOZtVP2?usp=drive_link)

### Character \+ Enemies:

🔗 **LuizMelo \- Monsters Creatures Fantasy** [https://luizmelo.itch.io/monsters-creatures-fantasy](https://luizmelo.itch.io/monsters-creatures-fantasy)

🔗 **Penzilla \- Hero Knight** (Player) [https://penzilla.itch.io/hero-knight](https://penzilla.itch.io/hero-knight)

### Tilesets (Farm \+ Forest):

🔗 **Pixel Crawler Dungeon** [https://anokolisa.itch.io/dungeon-crawler-pixel-art-asset-pack](https://anokolisa.itch.io/dungeon-crawler-pixel-art-asset-pack)

🔗 **Free Stylized Top-Down Grass** [https://stealthix.itch.io/grass](https://stealthix.itch.io/grass)

### Crops & Farming:

🔗 **Cozy Farm Asset Pack** [https://shubibubi.itch.io/cozy-farm](https://shubibubi.itch.io/cozy-farm)

### UI:

🔗 **Kenney UI RPG** [https://kenney.nl/assets/ui-pack-rpg-expansion](https://kenney.nl/assets/ui-pack-rpg-expansion)

### Effects:

🔗 **Pixel Effects Pack** [https://pimen.itch.io/battle-effects-pack-1](https://pimen.itch.io/battle-effects-pack-1)

### Audio:

🔗 **Mixkit Game SFX** [https://mixkit.co/free-sound-effects/game/](https://mixkit.co/free-sound-effects/game/)

🔗 **OpenGameArt \- Music** [https://opengameart.org/](https://opengameart.org/)

---

# 🏗️ PHẦN 4: TECHNICAL ARCHITECTURE

## 4.1 Folder Structure

Assets/

├── \_Project/ ← TẤT CẢ code/asset team đặt ở đây

│ ├── Animations/ ← AnimationController, Animation files

│ ├── Audio/ ← BGM, SFX files

│ ├── Data/ ← ScriptableObject files

│ │ ├── Crops/

│ │ ├── Enemies/

│ │ ├── Waves/

│ │ └── Upgrades/

│ ├── Prefabs/ ← Player, Enemy, Crop, UI prefabs

│ ├── Scenes/

│ │ ├── MainMenu.unity

│ │ ├── GameScene.unity

│ │ └── GameOver.unity

│ ├── Scripts/

│ │ ├── Core/ ← Người 1 phụ trách

│ │ │ ├── Interfaces/

│ │ │ ├── Enums/

│ │ │ ├── GameEvents.cs

│ │ │ ├── ServiceLocator.cs

│ │ │ ├── GameManager.cs

│ │ │ ├── SaveSystem.cs

│ │ │ └── Entity.cs

│ │ ├── Player/ ← Người 2 phụ trách

│ │ │ ├── PlayerController.cs

│ │ │ ├── PlayerInput.cs

│ │ │ ├── PlayerStats.cs

│ │ │ ├── PlayerAnimator.cs

│ │ │ └── Bullet.cs

│ │ ├── Farming/ ← Người 3 phụ trách

│ │ │ ├── CropData.cs

│ │ │ ├── Crop.cs

│ │ │ ├── FarmTile.cs

│ │ │ └── FarmManager.cs

│ │ ├── Combat/ ← Người 2 phụ trách

│ │ │ ├── Enemy.cs

│ │ │ ├── Slime.cs

│ │ │ ├── GoblinArcher.cs

│ │ │ ├── Beast.cs

│ │ │ ├── DemonBoss.cs

│ │ │ └── EnemyPool.cs

│ │ ├── World/ ← Người 3 phụ trách

│ │ │ ├── DayNightCycle.cs

│ │ │ ├── WaveManager.cs

│ │ │ └── PowerUp.cs

│ │ ├── Upgrade/ ← Người 4 phụ trách

│ │ │ ├── UpgradeData.cs

│ │ │ ├── UpgradeManager.cs

│ │ │ └── PerkSystem.cs

│ │ ├── Companion/ ← Người 4 phụ trách

│ │ │ ├── Drone.cs

│ │ │ └── Scarecrow.cs

│ │ ├── UI/ ← Người 4 phụ trách

│ │ │ ├── HUD/

│ │ │ ├── Menu/

│ │ │ └── UpgradeUI/

│ │ └── Audio/ ← Người 4 phụ trách

│ │ └── AudioManager.cs

│ └── Materials/

└── ThirdParty/ ← Asset Store packages

## 4.2 Architecture Patterns (BẮT BUỘC dùng)

### Pattern 1: Event Bus (Decoupling)

// Thay vì gọi trực tiếp:

uiManager.UpdateGold(100); // ❌ Coupling cao

// Dùng event bus:

GameEvents.RaiseGoldChanged(100); // ✅ UI tự subscribe

### Pattern 2: ScriptableObject (Data-driven)

// Crop, Enemy, Upgrade đều là ScriptableObject

// → Designer có thể tạo/chỉnh data trong Editor, không cần code

### Pattern 3: Service Locator (DI)

// Register service

ServiceLocator.Register\<IAudioService\>(audioManager);

// Get anywhere

var audio \= ServiceLocator.Get\<IAudioService\>();

### Pattern 4: Object Pooling (Performance)

// Cho Bullet, Enemy, Particle \- tránh Instantiate/Destroy nhiều

private ObjectPool\<Bullet\> \_bulletPool;

### Pattern 5: State Machine (AI, Game States)

// Enemy AI: Idle → Chase → Attack → Die

// Game: Menu → Playing → Paused → GameOver

## 4.3 Coding Standards

// ✅ ĐÚNG:

public class PlayerController : Entity

{

    \[SerializeField\] private float \_moveSpeed \= 5f;

    \[SerializeField\] private Rigidbody2D \_rigidbody;

    private Vector2 \_moveInput;

    private void Awake()

    {

        \_rigidbody \= GetComponent\<Rigidbody2D\>();

    }

    public void SetMoveInput(Vector2 input)

    {

        \_moveInput \= input.normalized;

    }

}

// ❌ SAI:

public class playercontroller : MonoBehaviour

{

    public float speed \= 5f;  // public không cần thiết

    Rigidbody2D rb;  // không có underscore, không readonly

    void start() {  // sai chính tả Start

        rb \= GetComponent\<Rigidbody2D\>();

    }

}

### Naming Rules:

- **Class:** `PascalCase` → `PlayerController`
- **Public Method:** `PascalCase` → `TakeDamage()`
- **Private Method:** `PascalCase` → `Die()`
- **Public Field:** `PascalCase` → `MaxHP` (tránh, dùng property)
- **Private Field:** `_camelCase` → `_moveSpeed`
- **Local Variable:** `camelCase` → `currentHealth`
- **Constant:** `UPPER_SNAKE_CASE` → `MAX_LEVEL`

---

# 👥 PHẦN 5: PHÂN CHIA CÔNG VIỆC 4 NGƯỜI

## Vai trò Tổng quan

| Người       | Vai trò           | Module chính                             |
| :---------- | :---------------- | :--------------------------------------- |
| **Người 1** | Tech Lead         | Core Systems, Architecture, Git workflow |
| **Người 2** | Combat Programmer | Player, Combat, Enemy AI                 |
| **Người 3** | World Programmer  | Farming, Day/Night, Waves, Spawning      |
| **Người 4** | UI/UX \+ Designer | UI, Upgrades, Companion, Audio, Polish   |

---

## 👤 NGƯỜI 1: TECH LEAD (Foundation)

### Trách nhiệm chính:

- **Setup project, Git workflow**
- **Code Core architecture** (mọi người dùng)
- **Code review** Pull Requests
- **Save/Load system**
- **Scene transitions**
- **Build & Release**

### Files phụ trách:

#### Core/Interfaces/

- `IDamageable.cs`
- `IInteractable.cs`
- `IUpgradable.cs`
- `ISaveable.cs`

#### Core/Enums/

- `DayPhase.cs`
- `GameOverReason.cs`
- `CropType.cs`
- `EnemyType.cs`
- `ToolType.cs`
- `UpgradeCategory.cs`

#### Core/

- `GameEvents.cs` \- Event bus
- `ServiceLocator.cs` \- DI container
- `GameManager.cs` \- Game state machine
- `SaveSystem.cs` \- JSON save/load
- `SceneLoader.cs` \- Scene transitions
- `Entity.cs` \- Base class cho mọi entity

### Timeline:

**Tuần 1:** Setup project \+ Git \+ Folder structure \+ Core interfaces/enums **Tuần 2:** GameEvents \+ ServiceLocator \+ Entity base **Tuần 3:** GameManager \+ Game states (Menu, Playing, Paused, GameOver) **Tuần 4:** SaveSystem JSON **Tuần 5-6:** Hỗ trợ team, review PRs, integration **Tuần 7:** SceneLoader \+ transition giữa scenes **Tuần 8:** Save/Load integration với tất cả modules **Tuần 9:** Performance optimization, bug fixes **Tuần 10:** Build .exe, deployment, README

### Deliverables:

- ✅ Folder structure setup hoàn chỉnh
- ✅ Git repo \+ .gitignore \+ branch protection
- ✅ Core architecture chạy được
- ✅ Save/Load test pass
- ✅ Build .exe cuối kỳ

---

## 👤 NGƯỜI 2: COMBAT PROGRAMMER (Player \+ Enemy)

### Trách nhiệm chính:

- **Player Controller hoàn chỉnh**
- **Combat system** (damage, projectile, hit detection)
- **Enemy AI** (3 loại \+ 1 boss)
- **Bullet/Projectile system**

### Files phụ trách:

#### Player/

- `PlayerController.cs` \- Main controller, kế thừa Entity
- `PlayerInput.cs` \- Input handling (WASD, mouse, keys)
- `PlayerStats.cs` \- HP, Speed, Damage, Level, XP
- `PlayerAnimator.cs` \- Animation state management
- `Bullet.cs` \- Projectile logic
- `PlayerSaveData.cs` \- DTO cho save

#### Combat/

- `Enemy.cs` \- Abstract base class
- `Slime.cs` \- Enemy melee đơn giản
- `GoblinArcher.cs` \- Enemy ranged
- `Beast.cs` \- Enemy fast melee mạnh
- `DemonBoss.cs` \- Boss với 3 phases
- `EnemyPool.cs` \- Object pooling
- `EnemyData.cs` \- ScriptableObject
- `DamageCalculator.cs` \- Logic tính damage

### Timeline:

**Tuần 1:** Học Unity basics (Brackeys tutorial) **Tuần 2:** PlayerController (movement, animation) **Tuần 3:** Bullet system \+ attack **Tuần 4:** PlayerStats \+ Level system **Tuần 5:** Enemy base class \+ Slime **Tuần 6:** GoblinArcher \+ Beast \+ EnemyPool **Tuần 7:** Enemy AI improvements (state machine) **Tuần 8:** DemonBoss với 3 phases **Tuần 9:** Combat polish (hit effects, screen shake, hit pause) **Tuần 10:** Bug fixes, balance

### Deliverables:

- ✅ Player điều khiển mượt mà
- ✅ Bắn đạn, đánh enemy hoạt động
- ✅ 3 loại enemy với AI khác nhau
- ✅ Boss fight 3 phases ấn tượng
- ✅ Object pooling cho performance

---

## 👤 NGƯỜI 3: WORLD PROGRAMMER (Farming \+ Spawning)

### Trách nhiệm chính:

- **Farming system** (cây, tile, grid)
- **Day/Night cycle**
- **Wave spawning** enemy theo thời gian
- **Power-up drops**
- **Level/Map design**

### Files phụ trách:

#### Farming/

- `CropData.cs` \- ScriptableObject định nghĩa loại cây
- `Crop.cs` \- Cây cụ thể trong scene
- `FarmTile.cs` \- Tile đất đai
- `FarmManager.cs` \- Quản lý grid
- `CropStage.cs` \- Enum stages
- `TileState.cs` \- Enum tile states

#### World/

- `DayNightCycle.cs` \- Manage Dawn → Day → Dusk → Night
- `WaveManager.cs` \- Spawn enemy waves
- `EnemyWaveData.cs` \- ScriptableObject định nghĩa wave
- `SpawnPoint.cs` \- Marker cho spawn locations
- `PowerUp.cs` \- Power-up logic
- `PowerUpData.cs` \- ScriptableObject
- `PowerUpSpawner.cs` \- Spawn ngẫu nhiên

### Timeline:

**Tuần 1:** Học Unity basics **Tuần 2:** CropData \+ Crop class (growth coroutine) **Tuần 3:** FarmTile \+ FarmManager grid 10x10 **Tuần 4:** Integration với PlayerController (tool interactions) **Tuần 5:** DayNightCycle (Dawn/Day/Dusk/Night transitions) **Tuần 6:** WaveManager \+ spawn theo wave data **Tuần 7:** PowerUp system \+ drops từ enemy **Tuần 8:** Map design \- làm map đẹp cho Scene chính **Tuần 9:** Polish \- ambient lighting cho day/night **Tuần 10:** Bug fixes, balance

### Deliverables:

- ✅ Cây mọc theo thời gian
- ✅ Player có thể cày/trồng/tưới/thu hoạch
- ✅ Day/Night cycle chạy mượt
- ✅ Enemy spawn theo wave ban đêm
- ✅ Power-ups rơi ngẫu nhiên

---

## 👤 NGƯỜI 4: UI/UX \+ DESIGNER (Polish & Feel)

### Trách nhiệm chính:

- **UI/UX hoàn chỉnh** (HUD, menus, upgrade screen)
- **Upgrade system**
- **Companion (Drone)**
- **Audio system**
- **Polish & juice** (particles, screen shake)

### Files phụ trách:

#### UI/HUD/

- `HUDController.cs` \- HP bar, XP bar, gold counter
- `DayCounter.cs` \- Hiển thị "Day X"
- `WaveIndicator.cs` \- Wave counter
- `MinimapController.cs` \- Bonus: minimap

#### UI/Menu/

- `MainMenuController.cs`
- `PauseMenuController.cs`
- `GameOverScreen.cs`
- `SettingsMenu.cs`
- `InventoryUI.cs`

#### UI/UpgradeUI/

- `UpgradeScreen.cs` \- Hiện 3 perk khi level up
- `UpgradeCard.cs` \- UI card cho từng perk

#### Upgrade/

- `UpgradeData.cs` \- ScriptableObject
- `UpgradeManager.cs` \- Apply effect lên Player
- `PerkSystem.cs` \- Random 3 perks từ pool

#### Companion/

- `Drone.cs` \- Companion drone bay theo, tự bắn
- `Scarecrow.cs` \- Bù nhìn (companion option 2\)

#### Audio/

- `AudioManager.cs` \- Singleton play SFX/BGM
- `AudioMixer.cs` \- Volume settings

#### Polish/

- `CameraShake.cs`
- `DamageNumber.cs` \- Số damage bay lên
- `HitFlash.cs` \- Flash trắng khi bị đánh

### Timeline:

**Tuần 1:** Học Unity basics \+ UI Canvas **Tuần 2:** Main Menu \+ Pause Menu cơ bản **Tuần 3:** HUD (HP/XP/Gold bars) **Tuần 4:** Upgrade screen \+ system **Tuần 5:** Apply upgrades lên Player (work với Người 2\) **Tuần 6:** Drone companion **Tuần 7:** Audio Manager \+ import sound effects **Tuần 8:** Settings menu, inventory UI **Tuần 9:** Polish \- particles, screen shake, damage numbers **Tuần 10:** Final polish, fix UI bugs

### Deliverables:

- ✅ UI đẹp, chuyên nghiệp
- ✅ Upgrade screen ấn tượng
- ✅ Companion drone hoạt động
- ✅ Audio đầy đủ (BGM \+ SFX)
- ✅ Game cảm giác "juicy"

---

# 📅 PHẦN 6: ROADMAP 10 TUẦN

## TUẦN 1: SETUP & LEARNING 📚

### Cả team:

- **Họp kickoff** (1 buổi 2 tiếng)
- Mỗi người **học Unity** qua Brackeys tutorial (8 video)
- Cài Unity 2022.3.62f1 LTS (BẮT BUỘC cùng version)
- Cài Visual Studio 2022 \+ Game dev with Unity workload

### Người 1:

- [x] Tạo GitHub repo
- [x] Setup .gitignore Unity
- [x] Setup folder structure
- [x] Tạo branch strategy (main, develop, feature branches)
- [x] Code Core/Interfaces/Enums

### Người 2, 3, 4:

- Xem Brackeys 2D Game Tutorial Series (full)
- Tự làm 1 mini project test (spawn cube, move với WASD)
- Đọc về Coroutine, ScriptableObject

### Milestone:

✅ Cả team biết Unity basics ✅ Repo Git sẵn sàng ✅ Code base Core có Interfaces \+ Enums

---

## TUẦN 2: FIRST PROTOTYPE 🎮

### Người 1:

- [x] Code `GameEvents.cs` (Event bus)
- [x] Code `ServiceLocator.cs`
- [x] Code `Entity.cs` (base class)
- [x] Push lên develop branch

### Người 2:

- [x] Tạo Player GameObject \+ Rigidbody \+ Collider
- [x] Code `PlayerInput.cs` (đọc WASD \+ mouse)
- [x] Code `PlayerController.cs` (movement)
- [x] Test Player di chuyển trong scene trống

### Người 3:

- [x] Code `CropData.cs` ScriptableObject
- [x] Tạo 3 crop assets (Wheat, Pumpkin, MagicHerb)
- [x] Code `Crop.cs` với coroutine growth
- [x] Test 1 crop mọc qua các stage

### Người 4:

- [x] Tạo Main Menu scene
- [x] Code `MainMenuController.cs`
- [x] Tạo button: Play, Settings, Quit
- [x] Test bấm Play → load scene game

### Milestone:

✅ Player di chuyển được ✅ 1 cây có thể mọc lên ✅ Main Menu chạy được

---

## TUẦN 3: CORE GAMEPLAY 🌱⚔️

### Người 1:

- [x] Code `GameManager.cs` với state machine
- [x] Code `SceneLoader.cs` (chuyển scene mượt mà)
- [x] Setup PR review process

### Người 2:

- [x] Code `Bullet.cs` \+ bullet prefab
- [x] Player bắn được đạn theo hướng chuột
- [x] Code `PlayerStats.cs` (HP, Damage, Speed)
- [x] Code combat: bullet hit enemy → trừ HP

### Người 3:

- [x] Code `FarmTile.cs` \+ `FarmManager.cs`
- [x] Tạo grid 10x10 farm
- [x] Player có thể cày/trồng/tưới/thu hoạch
- [x] Test full farming loop

### Người 4:

- [x] Code `HUDController.cs`
- [x] HP bar, XP bar, Gold counter UI
- [x] Subscribe events từ GameEvents

### Milestone:

✅ Combat cơ bản hoạt động ✅ Farming loop hoàn chỉnh ✅ HUD hiển thị stats

---

## TUẦN 4: ENEMY & PROGRESSION 👹📈

### Người 1:

- [x] Code `SaveSystem.cs` (JSON)
- [x] Tạo `GameSaveData.cs` DTO
- [x] Test save/load với fake data

### Người 2:

- [x] Code `Enemy.cs` base class
- [x] Code `Slime.cs` \- enemy đầu tiên
- [x] AI: Slime đi tới crop gần nhất → đánh
- [x] Test slime tấn công farm

### Người 3:

- [x] Code `EnemyWaveData.cs` ScriptableObject
- [x] Tạo 3 wave assets (Wave_01, 02, 03\)
- [x] Code `WaveManager.cs` spawn theo wave
- [x] Test 1 wave spawn enemy

### Người 4:

- [x] Code `UpgradeData.cs` ScriptableObject
- [x] Tạo 10 upgrade assets (5 combat, 5 defense)
- [x] Code `UpgradeScreen.cs` UI (3 cards chọn 1\)

### Milestone:

✅ Enemy có thể tấn công farm ✅ Wave system spawn enemy theo thời gian ✅ Save/Load test pass

---

## TUẦN 5: DAY/NIGHT & MORE ENEMIES 🌙

### Người 1:

- [x] Tích hợp SaveSystem vào Player, FarmManager
- [x] Code Pause functionality

### Người 2:

- [x] Code `GoblinArcher.cs` (ranged enemy)
- [x] Code `Beast.cs` (fast melee)
- [x] Code `EnemyPool.cs` cho performance
- [x] Test cả 3 enemy types

### Người 3:

- [x] Code `DayNightCycle.cs`
- [x] 4 phases: Dawn/Day/Dusk/Night
- [x] Visual effect: thay đổi ambient light
- [x] Enemy chỉ spawn vào Night

### Người 4:

- [x] Code `UpgradeManager.cs` apply lên PlayerStats
- [x] Test level up → chọn perk → stats tăng
- [x] Code damage numbers bay lên

### Milestone:

✅ 3 loại enemy đa dạng ✅ Day/Night cycle chạy ✅ Level up \+ upgrade system hoạt động

---

## TUẦN 6: COMPANION & POLISH 🐺✨

### Người 1:

- [x] Review code toàn team, refactor
- [x] Fix bugs critical

### Người 2:

- [x] Bug fix combat
- [x] Improve enemy AI (state machine)
- [x] Hit feedback (screen flash, particle)

### Người 3:

- [x] Code `PowerUp.cs` \+ 5 power-up types
- [x] Enemy có 10% drop power-up khi chết
- [x] Power-up có duration, hiển thị timer

### Người 4:

- [x] Code `Drone.cs` companion
- [x] Drone bay quanh player, tự bắn enemy gần nhất
- [x] Drone là child object của Player ⭐ YC 13
- [x] UI hiển thị drone status

### Milestone:

✅ Có companion drone ✅ Power-up drops hoạt động ✅ Game feel "juicy" hơn

---

## TUẦN 7: SCENES & STORY 🎬

### Người 1:

- [x] Code Main Menu → Game → GameOver flow
- [x] Test scene transitions mượt mà
- [x] Persistent data giữa scenes (DontDestroyOnLoad)

### Người 2:

- [x] Bug fixes
- [x] Balance enemy stats

### Người 3:

- [x] Tạo Hub Town scene (NPC, shop)
- [x] Code NPC dialogue đơn giản
- [x] Portal giữa Farm Scene ↔ Hub Town

### Người 4:

- [x] Code shop UI (mua upgrade vĩnh viễn bằng gold)
- [x] Code Settings menu (volume, fullscreen)
- [x] Code Pause menu

### Milestone:

✅ Có 2-3 scenes hoàn chỉnh ✅ Story intro ✅ Settings/Pause menus

---

## TUẦN 8: BOSS FIGHT & SAVE/LOAD 👑

### Người 1:

- [x] Hoàn thiện Save/Load full game state
- [x] Test save → exit → reopen → load → đúng state
- [x] Hiển thị save time trong menu

### Người 2:

- [x] Code `DemonBoss.cs` với 3 phases
- [x] Boss spawn ở wave cuối cùng
- [x] Boss AI patterns khác nhau theo phase
- [x] Test boss fight đầy đủ

### Người 3:

- [x] Code Game Over conditions  
       - Player chết  
       - Tất cả crop bị phá
- [x] Trigger GameOver scene đúng cách

### Người 4:

- [x] Code Game Over screen
- [x] Hiển thị stats: days survived, gold earned, enemies killed
- [x] Buttons: Retry, Main Menu

### Milestone:

✅ Boss fight ấn tượng ✅ Save/Load hoạt động hoàn hảo ✅ Game Over flow đầy đủ

---

## TUẦN 9: AUDIO & POLISH ✨

### Người 1:

- [x] Performance optimization
- [x] Fix memory leaks
- [x] Build test trên máy khác

### Người 2:

- [x] Polish combat: screen shake, hit pause
- [x] Particle effects khi enemy chết
- [x] Bullet trail effects

### Người 3:

- [x] Polish farming: particle khi tưới, thu hoạch
- [x] Visual feedback cho ngày/đêm

### Người 4:

- [x] Code `AudioManager.cs`
- [x] Import & gán 20+ sound effects
- [x] BGM cho menu, day, night, boss
- [x] Audio mixer \+ volume controls

### Milestone:

✅ Game đầy đủ âm thanh ✅ Visual polish ấn tượng ✅ Cảm giác "professional"

---

## TUẦN 10: BUILD & DEMO 🎬

### Cả team:

- [x] **Bug bash day** (cả team chơi test, ghi bug)
- [x] Fix all critical bugs
- [x] Balance final
- [x] Họp prep demo

### Người 1:

- [x] Build .exe Release
- [x] Test trên Windows khác
- [x] Viết README.md GitHub chuyên nghiệp
- [x] Setup release page trên GitHub

### Người 2:

- [x] Full playthrough test
- [x] Quay video gameplay highlights
- [x] Document combat system trong slide

### Người 3:

- [x] Test all gameplay loops
- [x] Document farming/wave system trong slide
- [x] Vẽ diagram architecture cho slide

### Người 4:

- [x] Làm slide thuyết trình (10-15 slides)
- [x] Quay trailer video (1-2 phút)
- [x] Tutorial dialogue cho người mới chơi

### Milestone:

✅ Build .exe sẵn sàng nộp ✅ Slide \+ Video demo ✅ README ngon lành ✅ Ready for presentation\!

---

# 🔧 PHẦN 7: GIT WORKFLOW

## 7.1 Branch Strategy

main ← Production (chỉ merge từ develop khi stable)

└── develop ← Branch chính team làm việc

    ├── feature/core-events           (Người 1\)

    ├── feature/player-controller     (Người 2\)

    ├── feature/enemy-slime           (Người 2\)

    ├── feature/crop-system           (Người 3\)

    ├── feature/wave-manager          (Người 3\)

    ├── feature/hud                   (Người 4\)

    ├── feature/upgrade-screen        (Người 4\)

    └── hotfix/critical-bug           (Khi cần)

## 7.2 Quy tắc Commit

### Commit message format:

\<type\>: \<description\>

\[optional body\]

### Types:

- `feat:` Tính năng mới
- `fix:` Sửa bug
- `refactor:` Refactor cưode (không thêm feature)
- `docs:` Cập nhật documentation
- `style:` Format code
- `test:` Thêm/sửa tests
- `chore:` Việc lặt vặt (cập nhật .gitignore, etc.)

### Ví dụ:

git commit \-m "feat: add slime enemy with melee AI"

git commit \-m "fix: player movement not working when game paused"

git commit \-m "refactor: extract damage calculation to separate class"

## 7.3 Workflow hàng ngày

\# Sáng \- bắt đầu code

git checkout develop

git pull origin develop

git checkout \-b feature/your-feature

\# Trong khi code (commit thường xuyên)

git add .

git commit \-m "feat: add X"

\# Trước khi nghỉ \- push lên

git push origin feature/your-feature

\# Khi xong feature \- tạo PR trên GitHub

\# → Tag Người 1 review

\# → Người 1 review xong merge vào develop

\# → Bạn xóa branch cũ

## 7.4 Rules quan trọng

1. ❌ **KHÔNG push thẳng vào main hoặc develop**
2. ❌ **KHÔNG edit cùng Scene file cùng lúc** (file .unity khó merge)
3. ✅ **Commit mỗi ngày** (đừng để dồn 1 tuần)
4. ✅ **Pull develop trước khi code mới**
5. ✅ **Mỗi feature 1 branch riêng**
6. ✅ **PR phải có description rõ ràng**

## 7.5 Conflict Resolution

Khi conflict scene .unity:

1. Discord/Zalo báo team STOP edit scene
2. Người 1 (Tech Lead) merge tay
3. Test kỹ trước khi push

→ **Phòng tránh:** Mỗi người làm trên prefab riêng, hạn chế edit scene chính cùng lúc.

---

# 📋 PHẦN 8: TEAM COMMUNICATION

## 8.1 Họp định kỳ

| Loại họp          | Thời gian                 | Nội dung                                         |
| :---------------- | :------------------------ | :----------------------------------------------- |
| **Daily standup** | 15 phút mỗi ngày (online) | Hôm qua làm gì, hôm nay làm gì, có blocker không |
| **Weekly sync**   | 1 tiếng mỗi tuần          | Demo progress, plan tuần tới                     |
| **Sprint review** | 30 phút mỗi 2 tuần        | Đánh giá milestone, điều chỉnh                   |
| **Emergency**     | Khi cần                   | Khi có blocker lớn                               |

## 8.2 Communication Channels

### Discord/Zalo channels:

- `#general` \- Trao đổi chung
- `#code-help` \- Hỏi đáp kỹ thuật
- `#git-prs` \- Notify khi tạo PR
- `#bugs` \- Báo bug
- `#resources` \- Share link, asset

### GitHub:

- Issues \- Track bugs \+ tasks
- Projects \- Kanban board (Todo, In Progress, Done)
- Pull Requests \- Code review

## 8.3 Task Tracking

Dùng **GitHub Projects** hoặc **Trello**:

\[Todo\] \[In Progress\] \[Review\] \[Done\]

───────────── ───────────── ───────────── ─────────────

Player anim PlayerController Enemy AI base Setup project

Save UI GameEvents

Boss phase 2 Crop growth

---

# 🎯 PHẦN 9: ĐÁNH GIÁ 16 YÊU CẦU PRU213

## Mapping yêu cầu → Module trong game

| \#  | Yêu cầu                       | Module                    | Người phụ trách |
| :-- | :---------------------------- | :------------------------ | :-------------- |
| 1   | Input bàn phím                | PlayerInput               | Người 2         |
| 2   | Game Over                     | GameOverDetector          | Người 3 \+ 4    |
| 3   | Animation/Collision/Coroutine | Toàn bộ                   | Tất cả          |
| 4   | Hiển thị điểm                 | HUDController             | Người 4         |
| 5   | Âm thanh                      | AudioManager              | Người 4         |
| 6   | C\# \+ OOP                    | Toàn bộ kiến trúc         | Người 1 design  |
| 7   | Nhiều scene                   | SceneLoader (3 scenes)    | Người 1         |
| 8   | Độ khó tăng                   | WaveManager               | Người 3         |
| 9   | Promotion (buff)              | PowerUp system            | Người 3         |
| 10  | Upgrade character             | UpgradeManager            | Người 4         |
| 11  | Đổi appearance                | Dragon Armor upgrade      | Người 2 \+ 4    |
| 12  | Spawn objects                 | EnemySpawner, CropSpawner | Người 3         |
| 13  | Child object hỗ trợ           | Drone Companion           | Người 4         |
| 14  | Save/Load                     | SaveSystem JSON           | Người 1         |
| 15  | GitHub                        | Repo \+ workflow          | Người 1         |
| 16  | Extra features                | Achievement, leaderboard  | Tất cả          |

## Extra features (yêu cầu 16\) \- điểm cộng:

- ✅ **Boss fight** với 3 phases
- ✅ **Hub Town** với NPC dialogue
- ✅ **Shop system** mua upgrade vĩnh viễn
- ✅ **Achievement system** (giết 100 enemy, sống 5 ngày)
- ✅ **Leaderboard** local top 10
- ✅ **Settings menu** đầy đủ (volume, fullscreen, key binding)
- ✅ **Difficulty selector** (Easy/Normal/Hard)
- ✅ **Particle effects** ấn tượng
- ✅ **Screen shake \+ hit pause**
- ✅ **Vietnamese localization** (extra impress)

---

# 🚀 PHẦN 10: GETTING STARTED CHECKLIST

## Trước khi bắt đầu Tuần 1

### Mọi người làm cùng:

- [ ] Tải Unity Hub: [https://unity.com/download](https://unity.com/download)
- [ ] Cài Unity **2022.3.62f1** (KHÔNG được version khác\!)
- [ ] Cài Visual Studio 2022 Community
- [ ] Trong VS Installer: tick **"Game development with Unity"**
- [ ] Cài Git: [https://git-scm.com/](https://git-scm.com/)
- [ ] Tạo GitHub account (nếu chưa có)
- [ ] Cài Discord/Zalo cho team

### Người 1 làm thêm:

- [ ] Tạo GitHub repo `magic-farm-defender`
- [ ] Setup `.gitignore` cho Unity:
      \[Ll\]ibrary/

      \[Tt\]emp/

      \[Oo\]bj/

      \[Bb\]uild/

      \[Bb\]uilds/

      \[Ll\]ogs/

      \[Uu\]ser\[Ss\]ettings/

      .vs/

      \*.csproj

      \*.sln

- [ ] Add 3 thành viên còn lại làm collaborators
- [ ] Setup branch protection cho `main` và `develop`
- [ ] Tạo Project Board trên GitHub (Kanban)
- [ ] Share repo link cho cả team

### Họp kickoff tuần 1:

- [ ] Đọc full document này cùng nhau
- [ ] Thống nhất vai trò 4 người
- [ ] Setup channel communication
- [ ] Lịch họp weekly
- [ ] Mỗi người tự confirm: "Tôi sẽ làm \[vai trò X\], có thể dành Y giờ/tuần"

---

# 🎓 PHẦN 11: LEARNING RESOURCES

## Tutorials phải xem (TUẦN 1\)

### Cốt lõi \- Cả team xem:

1. **Brackeys \- How to make a 2D Game (Series)** \- 8 video, 3h tổng [https://www.youtube.com/playlist?list=PLPV2KyIb3jR5QFsefuO2RlAgWEz6EvVi6](https://www.youtube.com/playlist?list=PLPV2KyIb3jR5QFsefuO2RlAgWEz6EvVi6)

### Theo vai trò:

#### Người 1 (Architecture):

- Game Programming Patterns (free book): [http://gameprogrammingpatterns.com/](http://gameprogrammingpatterns.com/)
- Code Monkey \- Unity OOP best practices

#### Người 2 (Combat):

- Sebastian Lague \- Enemy AI series
- Code Monkey \- Object Pooling

#### Người 3 (World):

- Brackeys \- Tilemap tutorial
- Tarodev \- 2D Top-Down movement

#### Người 4 (UI/Polish):

- Game Dev Guide \- Unity UI Tutorials series
- Code Monkey \- Make game JUICY

## Cheatsheet C\# → Unity (cho team .NET background)

| .NET                | Unity tương đương                  |
| :------------------ | :--------------------------------- |
| `Main()`            | `Start()`                          |
| `Console.WriteLine` | `Debug.Log`                        |
| `Task.Delay()`      | `Coroutine + WaitForSeconds`       |
| `Event/Delegate`    | `UnityEvent` hoặc C\# Action       |
| DI Container        | ServiceLocator \+ ScriptableObject |
| `[Serializable]`    | `[SerializeField]`                 |
| Class library DLL   | Assembly Definition (.asmdef)      |
| Logger              | `Debug.Log/LogWarning/LogError`    |

---

# 💡 PHẦN 12: TIPS & TRICKS

## Productivity tips:

### 1\. Unity Editor shortcuts:

- `Ctrl + S` \- Save scene
- `Ctrl + P` \- Play/Stop
- `Ctrl + Shift + P` \- Pause
- `Ctrl + D` \- Duplicate
- `F` \- Focus camera vào object
- `Q W E R T` \- Toggle tool (hand/move/rotate/scale/rect)

### 2\. Visual Studio shortcuts:

- `F12` \- Go to definition
- `Shift + F12` \- Find all references
- `Ctrl + .` \- Quick fix
- `Ctrl + R, Ctrl + R` \- Rename symbol

### 3\. Git tips:

\# Xem trạng thái

git status

\# Tạo branch và checkout

git checkout \-b feature/abc

\# Lưu tạm changes không commit

git stash

git stash pop

\# Xem log đẹp

git log \--oneline \--graph \--all

\# Hủy changes chưa add

git checkout \-- .

## Common pitfalls (tránh ngay):

### ❌ Pitfall 1: Edit scene cùng lúc

→ Chia prefab cho mỗi người, hạn chế chạm scene chính

### ❌ Pitfall 2: Hardcode magic numbers

// Sai

if (player.HP \< 30\) { ... }

// Đúng

\[SerializeField\] private float \_lowHPThreshold \= 30f;

if (player.HP \< \_lowHPThreshold) { ... }

### ❌ Pitfall 3: Singleton lạm dụng

→ Dùng ServiceLocator hoặc dependency injection

### ❌ Pitfall 4: Update() làm quá nhiều việc

→ Dùng Coroutine cho task định kỳ

### ❌ Pitfall 5: Instantiate liên tục

→ Dùng Object Pooling cho bullet, enemy

### ❌ Pitfall 6: Đặt tên dở

// Sai

public void Do(int x) { ... }

public class Manager { ... }

// Đúng

public void TakeDamage(int damage) { ... }

public class EnemySpawnManager { ... }

---

# 🏆 PHẦN 13: SUCCESS CRITERIA

## Định nghĩa "Hoàn thành" cho project:

### Mức 5 điểm (Pass):

- ✅ Game chạy được, không crash
- ✅ Có đủ 16 yêu cầu PRU213
- ✅ Push lên GitHub

### Mức 7 điểm (Khá):

- ✅ Mức 5 điểm \+
- ✅ Code clean, có comment
- ✅ Architecture rõ ràng
- ✅ Có art/audio cơ bản

### Mức 9 điểm (Giỏi):

- ✅ Mức 7 điểm \+
- ✅ Polish ấn tượng (particles, screen shake)
- ✅ Có boss fight
- ✅ Có 3-5 extra features
- ✅ Slide thuyết trình chuyên nghiệp

### Mức 10 điểm (Xuất sắc):

- ✅ Mức 9 điểm \+
- ✅ Code architecture xuất sắc (test được Unit Test)
- ✅ Game có replay value cao
- ✅ Documentation đầy đủ
- ✅ Video trailer ấn tượng
- ✅ Thuyết trình tự tin, trả lời được mọi câu hỏi technical

## Mục tiêu team: **9-10 điểm**

---

# 📞 PHẦN 14: APPENDIX

## A. Quick links

- Unity Hub: [https://unity.com/download](https://unity.com/download)
- Visual Studio: [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/)
- Git: [https://git-scm.com/](https://git-scm.com/)
- GitHub Desktop: [https://desktop.github.com/](https://desktop.github.com/)

## B. Inspiration games (chơi để hiểu vibe):

- **Atomicrops** (Steam) \- Reference chính
- **Farmfence Demo** (Steam, FREE) \- Concept tương tự
- **Vampire Survivors** \- Wave-based gameplay
- **Brotato** \- Roguelike \+ upgrade

## C. Backup plans

### Nếu chậm tiến độ:

- **Tuần 5 còn chậm:** Cắt boss fight đến tuần 9
- **Tuần 7 còn chậm:** Bỏ Hub Town, chỉ 1 scene chính
- **Tuần 9 còn chậm:** Bỏ một số polish, focus core gameplay

### Nếu mất 1 thành viên:

- Người 1 thay vai trò
- Cắt scope của module người đó
- Communicate với giáo viên về team size

---

# ✍️ KẾT LUẬN

Document này là **bible** của team trong 10 tuần tới. Mỗi người **PHẢI**:

1. ✅ Đọc kỹ phần vai trò của mình
2. ✅ Đồng ý với timeline và deliverables
3. ✅ Commit dành đủ thời gian (15-20 giờ/tuần)
4. ✅ Communicate khi gặp blocker
5. ✅ Code clean theo standards

**Chúc team thành công và làm ra game đỉnh\! 🎮🌾⚔️**

---

_Document version: 1.0_ _Last updated: Tuần 1_ _Maintainer: Người 1 (Tech Lead)_
