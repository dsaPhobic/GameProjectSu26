# PHÂN CÔNG CÔNG VIỆC — Magic Farm Defender

> Cập nhật: 01/06/2026
> Engine: Unity 2022.3.62f3 LTS | Branch: tạo `feature/<tên>` → PR vào `develop`

---

## Tình trạng hiện tại

| Hạng mục                          | Trạng thái |
| --------------------------------- | ---------- |
| Scripts (60+ file .cs)            | XONG       |
| Scene setup                       | CHƯA LÀM  |
| Prefabs (Player, Enemy, Crop, UI) | CHƯA LÀM  |
| ScriptableObject data assets      | CHƯA LÀM  |
| Sprites / Animation               | CHƯA LÀM  |
| Audio (BGM + SFX)                 | CHƯA LÀM  |

---

## HMQuang — ĐÃ HOÀN THÀNH (nghỉ ngơi)

> HMQuang đã thực hiện phần lớn công việc nặng nhất của project. Không cần làm thêm gì nữa.

### Đã commit (22/05/2026 — 3,548 dòng code, 88 files)

**Core Architecture**
- [x] `ServiceLocator.cs` — Dependency injection
- [x] `GameManager.cs` — State machine (Menu/Playing/Paused/LevelUp/GameOver)
- [x] `GameEvents.cs` — Event bus 15+ events
- [x] `SaveSystem.cs` — Lưu/tải JSON
- [x] `SceneLoader.cs` — Chuyển scene
- [x] `Entity.cs` — Abstract base class (HP, IDamageable)
- [x] Toàn bộ Enums (9 file) + Interfaces (4 file)

**Player System**
- [x] `PlayerController.cs` — Di chuyển, dash
- [x] `PlayerInput.cs` — WASD, mouse aim, phím tắt
- [x] `PlayerStats.cs` — HP, damage, XP, gold, level
- [x] `PlayerAnimator.cs` — Animation state machine
- [x] `PlayerToolHandler.cs` — Hoe, Watering Can, Sword
- [x] `Bullet.cs` — Projectile với object pooling

**Combat System**
- [x] `Enemy.cs` — Base class Idle/Chase/Attack/Dead
- [x] `Slime.cs`, `GoblinArcher.cs`, `Beast.cs`, `DemonBoss.cs` — 4 loại quái
- [x] `EnemyData.cs`, `EnemyPool.cs`, `DamageCalculator.cs`

**Farming System**
- [x] `Crop.cs` — 4 giai đoạn tăng trưởng
- [x] `CropData.cs`, `FarmTile.cs`, `FarmManager.cs` — Grid 10×10

**World & Wave System**
- [x] `DayNightCycle.cs` — Dawn/Day/Dusk/Night
- [x] `WaveManager.cs` — Spawn enemy, tăng độ khó theo ngày
- [x] `EnemyWaveData.cs`, `SpawnPoint.cs`, `GameOverDetector.cs`
- [x] `PowerUp.cs`, `PowerUpData.cs`, `PowerUpSpawner.cs` — 6 loại buff

**Upgrade System**
- [x] `UpgradeData.cs`, `UpgradeManager.cs`, `PerkSystem.cs`

**Companion System**
- [x] `Drone.cs` — Child object tự bắn enemy gần nhất
- [x] `Scarecrow.cs`

**UI System**
- [x] `HUDController.cs`, `DayCounter.cs`, `WaveIndicator.cs`, `PowerUpTimerUI.cs`
- [x] `MainMenuController.cs`, `PauseMenuController.cs`, `GameOverScreen.cs`, `SettingsMenu.cs`
- [x] `UpgradeScreen.cs`, `UpgradeCard.cs`

**Audio & Polish**
- [x] `AudioManager.cs` — BGM/SFX với fade
- [x] `CameraShake.cs`, `DamageNumber.cs`, `HitFlash.cs`

**Tài liệu**
- [x] `docs/ImplementationPlan.md` — 1,225 dòng kế hoạch triển khai chi tiết

---

## Xuntacdor — Player Prefab + GameScene Setup + GitHub Lead

**Branch:** `feature/xuntacdor-scene-integration`

### Import & Player Prefab

- [ ] Download [Hero Knight (Penzilla)](https://penzilla.itch.io/hero-knight) → import vào `Sprites/Player/`
- [ ] Slice sprite sheet Player: Idle (4f), Run (8f), Attack (6f), Dash (4f), Death (6f)
- [ ] Tạo `Prefabs/Player/Player.prefab`
  - Gắn: `PlayerController.cs`, `PlayerStats.cs`, `PlayerInput.cs`, `PlayerAnimator.cs`, `PlayerToolHandler.cs`, `HitFlash.cs`
  - Thêm: `Rigidbody2D` (Gravity Scale = 0), `CircleCollider2D`
  - Child object `"Drone"` → gắn `Drone.cs`
- [ ] Tạo `Prefabs/Player/Bullet.prefab` → gắn `Bullet.cs`, `Rigidbody2D`, `CircleCollider2D`
- [ ] Tạo Animator Controller cho Player → 5 clip: Idle, Run, Attack, Dash, Death

### GameScene.unity

- [ ] Tạo `GameScene.unity`
- [ ] Empty `"GameManager"` → gắn `GameManager.cs`, `ServiceLocator.cs`
- [ ] Empty `"WaveManager"` → gắn `WaveManager.cs`
- [ ] Empty `"DayNightCycle"` → gắn `DayNightCycle.cs`
- [ ] Empty `"SaveSystem"` → gắn `SaveSystem.cs`
- [ ] Empty `"GameOverDetector"` → gắn `GameOverDetector.cs`
- [ ] Empty `"SceneLoader"` → gắn `SceneLoader.cs`
- [ ] Empty `"PowerUpSpawner"` → gắn `PowerUpSpawner.cs`
- [ ] Đặt 8–12 GameObject `"SpawnPoint"` quanh bản đồ → gắn `SpawnPoint.cs`
- [ ] Camera → gắn `CameraShake.cs`

### Build Settings & GitHub

- [ ] Thêm đủ 3 scene vào Build Settings: `MainMenu` (index 0), `GameScene` (index 1), `GameOver` (index 2)
- [ ] Test end-to-end sau khi merge các branch
- [ ] Review PR của 2 thành viên còn lại
- [ ] Fix bug tích hợp

---

## WaanTh — Enemy Prefabs + Farming System + Wave Data

**Branch:** `feature/waanth-enemy-farming`

### Import Sprites Enemy

- [ ] Download [Monsters Creatures Fantasy (LuizMelo)](https://luizmelo.itch.io/monsters-creatures-fantasy) → import vào `Sprites/Enemies/`
- [ ] Slice sprite sheet: Slime, Goblin, Beast, DemonBoss (Idle, Walk, Attack, Death mỗi loại)

### Enemy Prefabs (4 loại)

- [ ] `Prefabs/Enemies/Slime.prefab` → `Slime.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] `Prefabs/Enemies/GoblinArcher.prefab` → `GoblinArcher.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] `Prefabs/Enemies/Beast.prefab` → `Beast.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] `Prefabs/Enemies/DemonBoss.prefab` → `DemonBoss.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] Tạo Animator Controller cho từng loại enemy (4 controller)
- [ ] Đặt `"EnemyPool"` trong GameScene → gắn `EnemyPool.cs`, kéo 4 prefab vào

### ScriptableObject — EnemyData (tạo tại `Data/Enemies/`)

- [ ] `SO_Slime.asset` — HP: 30, Damage: 5, Speed: 2.5, XP: 10
- [ ] `SO_GoblinArcher.asset` — HP: 25, Damage: 8, Speed: 2, XP: 15
- [ ] `SO_Beast.asset` — HP: 50, Damage: 12, Speed: 4, XP: 20
- [ ] `SO_DemonBoss.asset` — HP: 500, Damage: 20, Speed: 2.5, XP: 200

### Import Sprites Farm

- [ ] Download [Cozy Farm Asset Pack (shubibubi)](https://shubibubi.itch.io/cozy-farm) → import vào `Sprites/Tiles/`
- [ ] Download [Stylized Top-Down Grass (Stealthix)](https://stealthix.itch.io/grass) → import vào `Sprites/Tiles/`
- [ ] Cắt sprite crop: Wheat (4 stage), Pumpkin (4 stage), MagicHerb (4 stage)
- [ ] Cắt tile: Empty dirt, Tilled dirt, Watered dirt

### FarmTile + Crop Prefabs

- [ ] `Prefabs/Crops/FarmTile.prefab` → gắn `FarmTile.cs`, `BoxCollider2D`
- [ ] `Prefabs/Crops/Crop_Wheat.prefab` → gắn `Crop.cs`
- [ ] `Prefabs/Crops/Crop_Pumpkin.prefab` → gắn `Crop.cs`
- [ ] `Prefabs/Crops/Crop_MagicHerb.prefab` → gắn `Crop.cs`
- [ ] Tạo Animation clip cho mỗi crop (4 stage → đổi sprite, 3 controller)
- [ ] Đặt `"FarmManager"` trong GameScene → gắn `FarmManager.cs`, cấu hình grid 10×10

### ScriptableObject — CropData & WaveData

- [ ] `SO_Wheat.asset` — GrowTime: 60s, GoldValue: 10, XP: 5
- [ ] `SO_Pumpkin.asset` — GrowTime: 90s, GoldValue: 20, XP: 10
- [ ] `SO_MagicHerb.asset` — GrowTime: 120s, GoldValue: 40, XP: 20
- [ ] `SO_Wave_Day1.asset` — 5 Slime
- [ ] `SO_Wave_Day2.asset` — 8 Slime, 2 Goblin
- [ ] `SO_Wave_Day3.asset` — 5 Slime, 5 Goblin, 2 Beast
- [ ] `SO_Wave_Day4.asset` — 10 Goblin, 5 Beast
- [ ] `SO_Wave_Day5.asset` — 5 Beast, 1 DemonBoss

---

## PhmHai0702 — UI Scenes + Audio + PowerUps + Upgrade Data

**Branch:** `feature/phmhai-ui-audio`

### MainMenu Scene

- [ ] Tạo `MainMenu.unity`
- [ ] Canvas → Panel → Button "Play", "Continue", "Settings", "Quit"
- [ ] Gắn `MainMenuController.cs` vào Canvas
- [ ] Background sprite + Title text (TextMeshPro)

### GameOver Scene

- [ ] Tạo `GameOver.unity`
- [ ] Canvas → Panel stats (Days survived, Gold earned, Crops harvested)
- [ ] Button "Restart", "Main Menu"
- [ ] Gắn `GameOverScreen.cs`

### HUD trong GameScene

- [ ] Canvas (Screen Space - Overlay)
- [ ] HP Bar: `Image` (fill) → gắn `HUDController.cs`
- [ ] XP Bar: `Image` (fill)
- [ ] Gold Text, Day Text → gắn `DayCounter.cs`
- [ ] Wave Text → gắn `WaveIndicator.cs`
- [ ] PowerUp timer slots (6 slot) → gắn `PowerUpTimerUI.cs`
- [ ] Pause Menu panel → Button Resume, Settings, Quit → gắn `PauseMenuController.cs`
- [ ] Upgrade Screen panel → 3 `UpgradeCard` slots → gắn `UpgradeScreen.cs`, `UpgradeCard.cs`

### PowerUp Prefabs + ScriptableObject

- [ ] `PowerUp_Speed.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_DoubleDamage.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_Shield.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_HealthRestore.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_GrowthMagic.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_Magnet.prefab` → gắn `PowerUp.cs`
- [ ] 6 file `PowerUpData` SO tương ứng (duration, multiplier)

### ScriptableObject — UpgradeData (tạo tại `Data/Upgrades/`)

- [ ] `SO_Upgrade_MaxHP.asset` — +20 HP
- [ ] `SO_Upgrade_Speed.asset` — +0.5 speed
- [ ] `SO_Upgrade_Damage.asset` — +5 damage
- [ ] `SO_Upgrade_AttackSpeed.asset` — +0.3/s
- [ ] `SO_Upgrade_DashCooldown.asset` — -0.5s cooldown
- [ ] `SO_Upgrade_PickupRange.asset` — +0.5 range
- [ ] `SO_Upgrade_DragonArmor.asset` — đổi sprite Player (req #11)
- [ ] `SO_Upgrade_Drone.asset` — mở khóa Drone companion
- [ ] `SO_Upgrade_ExtraLife.asset` — hồi sinh 1 lần

### Audio

- [ ] Đặt Empty `"AudioManager"` trong GameScene → gắn `AudioManager.cs`
- [ ] Download BGM từ [OpenGameArt](https://opengameart.org/) → import vào `Audio/BGM/`
  - `bgm_main_menu.mp3`, `bgm_day.mp3`, `bgm_night.mp3`, `bgm_boss.mp3`
- [ ] Download SFX từ [Mixkit](https://mixkit.co/free-sound-effects/game/) → import vào `Audio/SFX/`
  - `sfx_attack.wav`, `sfx_hit.wav`, `sfx_harvest.wav`, `sfx_levelup.wav`, `sfx_die.wav`
- [ ] Kéo clip vào AudioManager fields

### Polish Prefab

- [ ] `Prefabs/UI/DamageNumber.prefab` → TextMeshPro + gắn `DamageNumber.cs`

---

## Thứ tự ưu tiên

```
Giai đoạn 1 — làm song song:
  Xuntacdor  → Import Player sprites + tạo Player prefab + GameScene managers
  WaanTh     → Import Enemy/Farm sprites + tạo Enemy & Crop prefabs
  PhmHai0702 → Tạo MainMenu + GameOver scene + HUD layout

Giai đoạn 2 — sau khi Giai đoạn 1 xong:
  Xuntacdor  → Test tích hợp + review PR + fix bug
  WaanTh     → Tạo tất cả SO data (Enemy, Crop, Wave)
  PhmHai0702 → Tạo SO data (PowerUp, Upgrade) + import Audio

Giai đoạn 3 — hoàn thiện:
  Tất cả     → Merge develop → main, polish, balance
  Xuntacdor  → Tag release, nộp bài
```

---

## Tổng kết đóng góp

| Thành viên | Đã làm                         | Còn làm              | Tổng tải |
| ---------- | ------------------------------ | -------------------- | -------- |
| HMQuang    | Toàn bộ 60+ scripts + plan     | Nghỉ — đã xong phần mình | Nhiều nhất |
| Xuntacdor  | Unity setup + Game Design Doc  | Player prefab + GameScene + testing | Vừa |
| WaanTh     | —                              | Enemy prefabs + Farming + SO data | Nhiều |
| PhmHai0702 | —                              | UI scenes + Audio + SO data | Nhiều |

---

## Git Workflow

```bash
# Mỗi người tạo branch riêng từ develop
git checkout develop
git pull
git checkout -b feature/<tên-branch>

# Commit theo convention
git commit -m "feat: add Player prefab with Drone child"
git commit -m "feat: create CropData ScriptableObjects"
git commit -m "fix: correct WaveManager spawn timing"

# Tạo PR vào develop khi xong (không push thẳng vào main)
```

---

## Liên hệ

| Thành viên | GitHub          | Email                   | Vai trò                       |
| ---------- | --------------- | ----------------------- | ----------------------------- |
| Xuntacdor  | @Xuntacdor      | nhatquang1223@gmail.com | Tech Lead / Scene Integration |
| HMQuang    | @khongthichcode | hmquang9805@gmail.com   | Đã hoàn thành — nghỉ          |
| WaanTh     | @WaanTh         | —                       | Enemy Prefabs + Farming       |
| PhmHai0702 | @PhmHai0702     | —                       | UI / Audio / Data             |
