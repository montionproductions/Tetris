<div align="center">

# 🧩 Tetris — Power-Up Edition

### A modern Tetris implementation built from scratch with Unity & C#

A classic grid-based puzzle game extended with **power-ups, progression, hold mechanics, local leaderboards, visual feedback, and Android support**.

<br/>

![Unity](https://img.shields.io/badge/Unity-6000.2-000000?style=for-the-badge\&logo=unity\&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Programming-512BD4?style=for-the-badge\&logo=csharp\&logoColor=white)
![Android](https://img.shields.io/badge/Android-Supported-3DDC84?style=for-the-badge\&logo=android\&logoColor=white)

</div>

---

## 🎮 About the project

This project started as an implementation of the classic **Tetris gameplay loop**, but evolved into an experiment around extending a familiar game through new mechanics.

Instead of only reproducing the original gameplay, the project adds a **power-up system** that rewards multi-line clears and introduces new strategic decisions.

The goal was to build the core systems myself and explore:

* Grid-based game logic
* Tetromino movement and collision
* Piece rotation
* Game-state management
* Progressive difficulty
* Power-up mechanics
* Persistent player data
* Game feel and visual feedback
* Mobile / Android deployment

---

## ✨ Features

### 🧱 Complete Tetris gameplay

Implements the seven standard tetrominoes:

`I` · `J` · `L` · `O` · `S` · `T` · `Z`

The board uses the traditional:

```text
10 × 20 grid
```

with custom systems for:

* Collision detection
* Piece placement
* Row detection
* Row destruction
* Grid reconstruction
* Rotation validation
* Game-over detection

---

### ⚡ Power-Up System

Clearing multiple lines rewards the player with special abilities.

| Lines cleared | Reward           |
| ------------- | ---------------- |
| 2 lines       | 🧱 Complete Row  |
| 3 lines       | 💥 Delete Column |
| 4 lines       | 🔥 Delete Row    |

Power-ups can be stored and activated later using a **drag-and-drop interaction system**.

This adds an additional strategic layer:

```text
Clear lines
     │
     ▼
Earn Power-Up
     │
     ▼
Store it
     │
     ▼
Choose when to use it
     │
     ▼
Manipulate the board
```

---

## 👻 Ghost Piece

The active tetromino projects its landing position onto the board.

The ghost position is calculated dynamically by testing valid grid positions until the lowest available position is found.

This provides immediate visual feedback without changing the actual board state.

---

## ⬇️ Hard Drop

Players can instantly drop a piece to its lowest valid position.

The system combines:

* Collision validation
* Grid updates
* Trail effects
* Camera feedback
* Sound feedback

to make the action feel more responsive.

---

## 🔄 Hold System

Players can temporarily store the current tetromino and swap it with another piece later.

The system handles:

* Current piece state
* Hold preview
* Grid cleanup
* Piece replacement
* Tetromino type preservation

---

## 🔮 Next Piece Queue

The game maintains a queue containing the **next three tetrominoes**.

```text
Current Piece

      ↓

┌─────────┐
│ Next #1 │
├─────────┤
│ Next #2 │
├─────────┤
│ Next #3 │
└─────────┘
```

The queue is continuously updated as new pieces enter the board.

---

## 📈 Progressive Difficulty

The game becomes progressively faster as the player survives longer.

| Level | Fall interval |
| ----: | ------------: |
|     1 |        1.00 s |
|     2 |        0.85 s |
|     3 |        0.65 s |
|     4 |        0.45 s |
|     5 |        0.35 s |
|     6 |        0.15 s |

Difficulty progression is controlled independently from the tetromino logic, allowing the gameplay speed to evolve without modifying individual pieces.

---

## 🏆 Local Leaderboard

High scores are persisted locally using JSON.

```text
Player finishes game
        │
        ▼
Score validation
        │
        ▼
Enter player name
        │
        ▼
Leaderboard.json
        │
        ▼
Sort by score
        │
        ▼
Leaderboard UI
```

The leaderboard automatically:

* Loads persisted scores
* Creates storage when none exists
* Handles empty or invalid data
* Sorts players by score
* Highlights top rankings
* Saves new high scores

---

## 🎨 Game Feel

The project includes several systems focused on making interactions feel more responsive:

* Particle effects for multi-line clears
* Hard-drop trails
* Camera shake
* Movement sounds
* Rotation sounds
* Hard-drop audio
* Multi-line clear audio
* UI transitions
* High-score feedback

These systems are separated from the core board logic so gameplay rules and presentation can evolve independently.

---

## 📱 Android Support

The project includes Android-specific integration for **Google Play In-App Updates**.

It supports both:

```text
Flexible Updates
```

and

```text
Immediate Updates
```

depending on availability and configuration.

Platform-specific code is isolated using Unity's compile-time platform directives.

---

# 🧠 Architecture

The gameplay is split into multiple systems instead of placing the complete game loop inside a single controller.

```text
                  ┌─────────────┐
                  │    Game     │
                  │ Game State  │
                  └──────┬──────┘
                         │
          ┌──────────────┼──────────────┐
          │              │              │
          ▼              ▼              ▼
   ┌────────────┐  ┌────────────┐  ┌──────────────┐
   │ Tetromino  │  │    Grid    │  │   PowerUps   │
   │   Logic    │  │   System   │  │    System    │
   └─────┬──────┘  └──────┬─────┘  └──────┬───────┘
         │                │               │
         └────────────────┼───────────────┘
                          │
                          ▼
                  ┌──────────────┐
                  │      UI      │
                  │ Score / HUD  │
                  └──────┬───────┘
                         │
            ┌────────────┼────────────┐
            ▼            ▼            ▼
       Leaderboard     Audio       Effects
```

### Main systems

| System                      | Responsibility                               |
| --------------------------- | -------------------------------------------- |
| `Game`                      | Game state, levels, spawning and progression |
| `Grup`                      | Tetromino movement, rotation and placement   |
| `GridGenerator`             | 10×20 grid state and line operations         |
| `PowerUps`                  | Board manipulation abilities                 |
| `PowerUpsMenu`              | Power-up inventory                           |
| `Hold`                      | Tetromino hold / swap system                 |
| `LeaderboardController`     | Local score persistence                      |
| `SoundSystem`               | Gameplay audio                               |
| `AndroidInAppUpdateManager` | Google Play updates                          |

---

# 🎯 Engineering Highlights

Some of the most interesting parts of the project were not rendering the pieces, but managing the interaction between independent gameplay systems.

### Grid State

The board is represented through a two-dimensional structure:

```csharp
Transform[,] grid;
```

Every movement validates the new position before modifying the authoritative grid state.

This keeps rendering and game rules synchronized.

### Rotation Validation

Piece rotation performs collision validation and attempts horizontal corrections when a rotation would otherwise collide with the board.

```text
Rotate
  │
  ▼
Valid?
 ├─ Yes → Apply
 │
 └─ No
      │
      ▼
 Try horizontal adjustment
      │
      ├─ Valid → Apply
      └─ Invalid → Revert
```

### Separation of Systems

Gameplay responsibilities are distributed across specialized components instead of relying on one monolithic game controller.

This made it easier to add systems such as:

* Hold
* Power-ups
* Leaderboards
* Android integrations
* Audio
* Visual effects

without rewriting the core grid implementation.

---

# 🛠 Tech Stack

<p>

![Unity](https://img.shields.io/badge/Unity-6000.2.13f1-000000?style=flat-square\&logo=unity)
![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square\&logo=csharp)
![Android](https://img.shields.io/badge/Android-3DDC84?style=flat-square\&logo=android)
![Google Play](https://img.shields.io/badge/Google_Play-414141?style=flat-square\&logo=googleplay)

</p>

**Core technologies**

* Unity 6000.2
* C#
* Unity UI
* TextMesh Pro
* Unity Particle System
* JSON persistence
* Google Play In-App Updates
* Android

---

# 📂 Project Structure

```text
Assets/
│
├── Scripts/
│   ├── Gameplay/
│   │   ├── Game.cs
│   │   ├── Grup.cs
│   │   ├── Box.cs
│   │   ├── Hold.cs
│   │   ├── PowerUps.cs
│   │   └── PowerUpsMenu.cs
│   │
│   ├── UI/
│   │   ├── GridGenerator.cs
│   │   ├── LeaderboardController.cs
│   │   ├── UIController.cs
│   │   └── Transition/
│   │
│   ├── Sound/
│   │
│   └── Android/
│
├── Prefabs/
│   ├── GameElements/
│   ├── PowerUps/
│   ├── Leaderboard/
│   └── Settings/
│
└── Scenes/
```

---

# 🚀 Running the project

### Requirements

* Unity **6000.2.13f1** or compatible version
* Git

Clone the repository:

```bash
git clone https://github.com/montionproductions/Tetris.git
```

Open the cloned directory using **Unity Hub**.

The main project scene is located at:

```text
Assets/Scenes/SampleScene.unity
```

Run the scene from the Unity Editor.

---

# 🎮 Keyboard Controls

| Input | Action     |
| ----- | ---------- |
| ←     | Move left  |
| →     | Move right |
| ↓     | Soft drop  |
| ↑     | Rotate     |
| Space | Hard drop  |

Additional gameplay interactions such as power-ups and hold are handled through the game's UI.

---

# 💡 What I learned

This project was an opportunity to go beyond reproducing Tetris and think about how multiple gameplay systems interact around a shared state.

Some of the areas I explored include:

* Designing deterministic grid logic
* Separating game state from presentation
* Handling collision and rotation edge cases
* Building reusable gameplay systems
* Persistent local data
* Creating responsive game feedback
* Extending an established gameplay loop without breaking its core mechanics
* Preparing a Unity application for Android distribution

---

<div align="center">

### Built by Francisco Montion

Software Engineer · C++ / C# · Full-Stack · AI Systems

</div>
