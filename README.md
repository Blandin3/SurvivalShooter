# Survival Shooter

A mobile **AR (augmented reality) survival shooter** built in Unity. The player taps a real-world surface to anchor the arena, then fights off timed waves of melee and ranged enemies for as long as they can survive, competing for a spot on a local top-5 leaderboard.

Built with **Unity 6**, **AR Foundation** (ARCore/ARKit), and the **Universal Render Pipeline**.

## Requirements

- Unity **6000.3.16f1** (Unity 6)
- An ARCore-capable Android device or ARKit-capable iOS device, for on-device testing and builds
- Unity's **XR Simulation** environment can be used to test AR flow in the Editor without a physical device

## Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/Blandin3/SurvivalShooter.git
   ```
2. Open the project folder in Unity Hub (matching the editor version above). Let Unity resolve packages on first open.
3. Open `Assets/Scenes/MainMenu.unity` and press Play (or build to a device — AR placement needs a real camera feed / XR Simulation to behave correctly).

## Scenes

- **`MainMenu`** — main menu: start a level, pick a difficulty, view the leaderboard, or quit.
- **`Level_01`**, **`Level_02`** — the two playable survival arenas.

## Gameplay Loop

1. From the main menu, the player picks **Easy / Normal / Hard** and a level, then presses Start.
2. On loading into a level, `GameManager` moves to the `Placement` state and `ARPlacementManager` waits for a tap on a detected AR plane.
3. Tapping a plane anchors the game world (`gameRoot`) at that point, hides the AR plane visualization, and kicks off `EnemySpawner`, which spawns melee and ranged enemies at a random position around the anchor on a timer.
4. The match runs for a fixed duration (`GameManager.matchDuration`, default 120s). The player earns score by killing enemies and loses health when hit; if health hits zero, the match ends immediately.
5. When the timer runs out (or the player dies), `GameManager.EndGame()` fires: all enemies are cleared, the result is submitted to `LeaderboardManager`, and the End Game summary panel is shown.
6. The player can restart the same level or return to the main menu; both paths cleanly shut down the AR session first to avoid a hung scene load.

### Controls

- **On-screen touch buttons** (`TouchButton` + `PlayerMovement`) — hold to walk forward/backward, relative to where the camera is facing.
- **Device rotation** — AR tracking drives look direction on-device.
- **Mouse look / right-click to fire** (`MouseLook`, `PlayerShoot`) — Editor/desktop-only convenience for testing without a device; has no effect on-device.
- A UI "Fire" button can also be wired directly to `PlayerShoot.Fire()`.

### Difficulty

`DifficultySettings` (a static class, set from the main menu) scales three things multiplicatively as enemies spawn:

| Difficulty | Enemy Health | Enemy Damage | Spawn Interval |
|---|---|---|---|
| Easy | ×0.75 | ×0.75 | ×1.3 (slower) |
| Normal | ×1 | ×1 | ×1 |
| Hard | ×1.5 | ×1.5 | ×0.6 (faster) |

### Enemies

Both enemy types share chase/detect/face-player logic in the abstract `EnemyBase` (direct kinematic movement — there's no baked NavMesh, since AR planes are only known at runtime):

- **Melee** (`EnemyMelee`) — closes to attack range and hits the player on a cooldown, with attacks staggered by spawn order so a group doesn't all swing in the same frame.
- **Shooter** (`ShooterEnemy`) — stops at range and fires pooled projectiles at the player's camera position, backing away if the player gets too close instead of standing still.

Enemies don't respawn from scratch on death — `EnemyHealth` disables visuals/collider/AI, then resets position and health after a delay, so a spawner's enemy count stays effectively constant.

### Leaderboard

`LeaderboardManager` persists the top 5 sessions (score, kills, time survived, date) to `PlayerPrefs` as JSON, newest-first, trimmed to 5 entries on every submission. `LeaderboardUI` renders them as numbered rows on the main menu.

## Project Structure

```
Assets/
  Scenes/            MainMenu, Level_01, Level_02
  Scripts/           Gameplay, UI, and AR systems (see below)
  Prefabs/           Enemies, projectiles, UI rows, etc.
```

### Core Systems (`Assets/Scripts`)

**Game flow**
- `GameManager` — singleton (`DontDestroyOnLoad`) owning match state (`GameState`: MainMenu → Placement → Playing → GameOver), score, timer, and background music; survives scene reloads and reacts to `SceneManager.sceneLoaded` so restarts work correctly.
- `GameState` — the state enum above.
- `GameSceneStarter` — Editor/no-AR-device fallback that auto-starts a match when a level loads, without waiting for a real plane-tap.
- `DifficultySettings` — static difficulty multipliers, set by the main menu.

**AR**
- `ARPlacementManager` — waits for a tap on a detected AR plane, anchors the game root there, hides plane visualization, and starts spawning + the match.

**Player**
- `PlayerMovement` — moves the AR rig (not the camera) via held touch buttons, relative to camera facing.
- `PlayerShoot` — raycasts from screen center to find the aim point, then fires a pooled bullet from the gun's muzzle bone toward it (falling back to the camera if the muzzle geometry would otherwise aim behind the player).
- `PlayerHealth` — implements `IDamageable`; fires damage/death events and hands off to `GameManager.EndGame()` on death.
- `MouseLook` — Editor/desktop-only look control.

**Enemies**
- `EnemyBase` (abstract) — shared chase/detect/face/animate logic; subclasses only define engage range and attack behavior.
- `EnemyMelee`, `ShooterEnemy` — the two concrete enemy types.
- `EnemyHealth` — implements `IDamageable`; handles hit reactions, death (score + respawn timer), and being wiped instantly at match end.
- `EnemySpawner` — spawns a random enemy type near the anchor on an interval, scaled by `DifficultySettings`.

**UI**
- `MainMenuUI` — level/difficulty selection, leaderboard panel toggle, music, quit.
- `HUDUI` — live score, timer, and health bar during a match.
- `EndGameUI` — post-match summary; restarts/returns to menu only after cleanly disabling the AR camera/session to avoid a hung scene load.
- `LeaderboardUI` — renders the persisted top-5 list.
- `DamageFlash` — red screen-flash hit indicator, wired to `PlayerHealth.onDamaged`.

**Shared utilities**
- `ObjectPool` — generic pool (one instance per prefab type) used for all bullets; no `Instantiate`/`Destroy` during gameplay.
- `Projectile` — poolable projectile; player bullets only damage `"Enemy"`-tagged colliders, enemy bullets only damage `"Player"`-tagged colliders.
- `IDamageable` — shared damage interface implemented by `PlayerHealth` and `EnemyHealth`.
- `LeaderboardManager`, `LeaderboardEntry` — persistence layer for the leaderboard.
- `TouchButton` — press-and-hold state for a UI button, used for movement controls.

## Key Dependencies

- `com.unity.xr.arfoundation`, `com.unity.xr.arcore`, `com.unity.xr.arkit` (6.3.4) — AR plane detection and tracking
- `com.unity.render-pipelines.universal` (17.3.0) — URP
- `com.unity.xr.interaction.toolkit`, `com.unity.xr.management`
- TextMeshPro — UI text throughout menus, HUD, and leaderboard
