# Survival Shooter

A Unity survival shooter where the player fights off waves of enemies across multiple levels, with a persistent leaderboard tracking top runs.

## Requirements

- Unity **6000.3.16f1** (Unity 6)

## Getting Started

1. Clone the repository:
   ```
   git clone https://github.com/Blandin3/SurvivalShooter.git
   ```
2. Open the project folder in Unity Hub (matching the editor version above).
3. Open `Assets/Scenes/MainMenu.unity` and press Play.

## Scenes

- `MainMenu` — main menu and leaderboard entry point
- `Level_01` — first survival level
- `Level_02` — second survival level

## Core Systems

- **Player** — `PlayerMovement`, `PlayerShoot`, `PlayerHealth`, `MouseLook`
- **Enemies** — `EnemyBase`, `EnemyMelee`, `ShooterEnemy`, `EnemyHealth`, `EnemySpawner`
- **Game Flow** — `GameManager`, `GameState`, `GameSceneStarter`, `DifficultySettings`
- **UI** — `HUDUI`, `MainMenuUI`, `EndGameUI`, `LeaderboardUI`
- **Leaderboard** — `LeaderboardManager`, `LeaderboardEntry`
- **Shared Utilities** — `ObjectPool`, `Projectile`, `IDamageable`, `DamageFlash`
