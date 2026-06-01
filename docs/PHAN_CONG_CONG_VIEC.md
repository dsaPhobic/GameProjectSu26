# PHÂN CÔNG CÔNG VIỆC — Magic Farm Defender

> Cập nhật: 01/06/2026  
> Engine: Unity 2022.3.62f3 LTS | Branch: tạo `feature/<tên>` → PR vào `develop`

---

## Tình trạng hiện tại

| Hạng mục                          | Trạng thái |
| --------------------------------- | ---------- |
| Scripts (60+ file .cs)            | XONG       |
| Scene setup                       | CHƯA LÀM   |
| Prefabs (Player, Enemy, Crop, UI) | CHƯA LÀM   |
| ScriptableObject data assets      | CHƯA LÀM   |
| Sprites / Animation               | CHƯA LÀM   |
| Audio (BGM + SFX)                 | CHƯA LÀM   |

---

## Xuntacdor — Scene Integration + GitHub Lead

**Branch:** `feature/xuntacdor-scene-integration`

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

### Build Settings

- [ ] Thêm đủ 3 scene vào Build Settings: `MainMenu` (index 0), `GameScene` (index 1), `GameOver` (index 2)

### Testing & GitHub

- [ ] Test end-to-end sau khi merge các branch
- [ ] Review PR của 3 thành viên còn lại
- [ ] Fix bug tích hợp

---

## HMQuang — Player & Enemy Prefabs + ScriptableObject Combat

**Branch:** `feature/hmquang-combat-prefabs`

### Import Sprites

- [ ] Download [Hero Knight (Penzilla)](https://penzilla.itch.io/hero-knight) → import vào `Sprites/Player/`
- [ ] Download [Monsters Creatures Fantasy (LuizMelo)](https://luizmelo.itch.io/monsters-creatures-fantasy) → import vào `Sprites/Enemies/`
- [ ] Slice sprite sheet cho Player (Idle 4f, Run 8f, Attack 6f, Dash 4f, Death 6f)
- [ ] Slice sprite sheet cho Slime, Goblin, Beast, DemonBoss (Idle, Walk, Attack, Death)

### Player Prefab

- [ ] Tạo `Prefabs/Player/Player.prefab`
  - Gắn: `PlayerController.cs`, `PlayerStats.cs`, `PlayerInput.cs`, `PlayerAnimator.cs`, `PlayerToolHandler.cs`, `HitFlash.cs`
  - Thêm: `Rigidbody2D` (Gravity Scale = 0), `CircleCollider2D`
  - Child object `"Drone"` → gắn `Drone.cs`
- [ ] Tạo `Prefabs/Player/Bullet.prefab` → gắn `Bullet.cs`, `Rigidbody2D`, `CircleCollider2D`
- [ ] Tạo Animator Controller cho Player → setup clip Idle, Run, Attack, Dash, Death

### Enemy Prefabs (4 loại)

- [ ] `Prefabs/Enemies/Slime.prefab` → `Slime.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] `Prefabs/Enemies/GoblinArcher.prefab` → `GoblinArcher.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] `Prefabs/Enemies/Beast.prefab` → `Beast.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] `Prefabs/Enemies/DemonBoss.prefab` → `DemonBoss.cs`, `HitFlash.cs`, `Rigidbody2D`, `Collider2D`
- [ ] Tạo Animator Controller cho từng loại enemy
- [ ] Đặt `EnemyPool` GameObject trong GameScene → gắn `EnemyPool.cs`, kéo 4 prefab vào

### ScriptableObject — EnemyData (tạo tại `Data/Enemies/`)

- [ ] `SO_Slime.asset` — HP: 30, Damage: 5, Speed: 2.5, XP: 10
- [ ] `SO_GoblinArcher.asset` — HP: 25, Damage: 8, Speed: 2, XP: 15
- [ ] `SO_Beast.asset` — HP: 50, Damage: 12, Speed: 4, XP: 20
- [ ] `SO_DemonBoss.asset` — HP: 500, Damage: 20, Speed: 2.5, XP: 200

---

## WaanTh — Farming System + Wave Data + PowerUps

**Branch:** `feature/waanth-farming-world`

### Import Sprites

- [ ] Download [Cozy Farm Asset Pack (shubibubi)](https://shubibubi.itch.io/cozy-farm) → import vào `Sprites/Tiles/`
- [ ] Download [Stylized Top-Down Grass (Stealthix)](https://stealthix.itch.io/grass) → import vào `Sprites/Tiles/`
- [ ] Cắt sprite crop: Wheat (4 stage), Pumpkin (4 stage), MagicHerb (4 stage)
- [ ] Cắt tile: Empty dirt, Tilled dirt, Watered dirt

### FarmTile + Crop Prefabs

- [ ] `Prefabs/Crops/FarmTile.prefab` → gắn `FarmTile.cs`, `BoxCollider2D`
- [ ] `Prefabs/Crops/Crop_Wheat.prefab` → gắn `Crop.cs`
- [ ] `Prefabs/Crops/Crop_Pumpkin.prefab` → gắn `Crop.cs`
- [ ] `Prefabs/Crops/Crop_MagicHerb.prefab` → gắn `Crop.cs`
- [ ] Tạo Animation clip cho mỗi crop (4 stage → đổi sprite)

### Setup FarmManager trong GameScene

- [ ] Đặt Empty `"FarmManager"` → gắn `FarmManager.cs`
- [ ] Kéo `FarmTile.prefab` vào field `tilePrefab`, cấu hình grid 10×10

### PowerUp Prefabs (tại `Prefabs/PowerUps/`)

- [ ] `PowerUp_Speed.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_DoubleDamage.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_Shield.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_HealthRestore.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_GrowthMagic.prefab` → gắn `PowerUp.cs`
- [ ] `PowerUp_Magnet.prefab` → gắn `PowerUp.cs`

### ScriptableObject — CropData (tại `Data/Crops/`)

- [ ] `SO_Wheat.asset` — GrowTime: 60s, GoldValue: 10, XP: 5
- [ ] `SO_Pumpkin.asset` — GrowTime: 90s, GoldValue: 20, XP: 10
- [ ] `SO_MagicHerb.asset` — GrowTime: 120s, GoldValue: 40, XP: 20

### ScriptableObject — WaveData (tại `Data/Waves/`)

- [ ] `SO_Wave_Day1.asset` — 5 Slime
- [ ] `SO_Wave_Day2.asset` — 8 Slime, 2 Goblin
- [ ] `SO_Wave_Day3.asset` — 5 Slime, 5 Goblin, 2 Beast
- [ ] `SO_Wave_Day4.asset` — 10 Goblin, 5 Beast
- [ ] `SO_Wave_Day5.asset` — 5 Beast, 1 DemonBoss

### ScriptableObject — PowerUpData (tại `Data/PowerUps/`)

- [ ] 6 file SO tương ứng 6 loại PowerUp (duration, multiplier)

---

## PhmHai0702 — UI Scenes + Audio + Polish

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

### ScriptableObject — UpgradeData (tại `Data/Upgrades/`)

- [ ] `SO_Upgrade_MaxHP.asset` — +20 HP, cost: 1 perk point
- [ ] `SO_Upgrade_Speed.asset` — +0.5 speed
- [ ] `SO_Upgrade_Damage.asset` — +5 damage
- [ ] `SO_Upgrade_AttackSpeed.asset` — +0.3/s
- [ ] `SO_Upgrade_DashCooldown.asset` — -0.5s cooldown
- [ ] `SO_Upgrade_PickupRange.asset` — +0.5 range
- [ ] `SO_Upgrade_DragonArmor.asset` — đổi sprite Player (req #11)
- [ ] `SO_Upgrade_Drone.asset` — mở khóa Drone companion
- [ ] `SO_Upgrade_ExtraLife.asset` — hồi sinh 1 lần

### Audio

- [ ] Đặt Empty `"AudioManager"` → gắn `AudioManager.cs`
- [ ] Download BGM từ [OpenGameArt](https://opengameart.org/) → import vào `Audio/BGM/`
  - `bgm_main_menu.mp3`, `bgm_day.mp3`, `bgm_night.mp3`, `bgm_boss.mp3`
- [ ] Download SFX từ [Mixkit](https://mixkit.co/free-sound-effects/game/) → import vào `Audio/SFX/`
  - `sfx_attack.wav`, `sfx_hit.wav`, `sfx_harvest.wav`, `sfx_levelup.wav`, `sfx_die.wav`
- [ ] Kéo clip vào AudioManager fields

### DamageNumber Prefab

- [ ] `Prefabs/UI/DamageNumber.prefab` → TextMeshPro + gắn `DamageNumber.cs`

---

## Thứ tự ưu tiên

```
Giai đoạn 1 (làm song song):
  HMQuang    → Import sprites + Player/Enemy prefabs
  WaanTh     → Import tileset + Crop/FarmTile prefabs + SO Crops & Waves
  PhmHai0702 → MainMenu scene + GameOver scene + HUD layout
  Xuntacdor  → GameScene skeleton + wiring managers + SpawnPoints

Giai đoạn 2 (sau khi giai đoạn 1 xong):
  Tất cả     → Merge feature branch vào develop
  Xuntacdor  → Test tích hợp, fix bug, review PR

Giai đoạn 3 (hoàn thiện):
  Tất cả     → Polish, balance tuning, final test
  Xuntacdor  → Merge develop → main, tag release
```

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

## Liên hệ / Ghi chú

| Thành viên | GitHub          | Email                   | Vai trò                       |
| ---------- | --------------- | ----------------------- | ----------------------------- |
| Xuntacdor  | @Xuntacdor      | nhatquang1223@gmail.com | Tech Lead / Scene Integration |
| HMQuang    | @khongthichcode | hmquang9805@gmail.com   | Combat / Player               |
| WaanTh     | @WaanTh         | —                       | Farming / World               |
| PhmHai0702 | @PhmHai0702     | —                       | UI / Audio                    |
