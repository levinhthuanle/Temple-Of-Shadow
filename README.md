# Temple of Shadow

> **Midterm Project — Game Development Course**
> A 2D action-adventure platformer developed with Unity.

![Project banner placeholder — add `docs/images/00-project-banner.png`](docs/images/00-project-banner.png)

---

## Table of Contents

- [Team Members](#team-members)
- [Project Overview](#project-overview)
- [Game Premise and Objective](#game-premise-and-objective)
- [Implemented Gameplay Features](#implemented-gameplay-features)
- [Game Flow](#game-flow)
- [Levels and Progression](#levels-and-progression)
- [Controls](#controls)
- [User Interface](#user-interface)
- [Software and Technologies](#software-and-technologies)
- [Project Structure](#project-structure)
- [How to Run the Project](#how-to-run-the-project)
- [Screenshot Gallery](#screenshot-gallery)
- [Team Contribution Record](#team-contribution-record)
- [Testing Checklist](#testing-checklist)
- [Resources and Attribution](#resources-and-attribution)
- [Submission Checklist](#submission-checklist)

---

## Team Members

Replace every bracketed field with the official information of each group member before submission.

| No. | Student ID | Full Name | 
| --- | --- | --- |
| 1 | 23125019 | Le Vinh Thuan | 
| 2 | 23125009 | Nguyen Minh Khoa | 
| 3 | 23125066 | Nguyen Hoang Phat | 
| 4 | 23125043 | Nguyen Van Hoang Nhat | 

---

## Project Overview

**Temple of Shadow** is a single-player 2D action-adventure platformer. The player controls a hero exploring a dangerous temple filled with enemies, combat encounters, collectibles, equipment, and environmental platforming challenges. The campaign is structured around three levels and concludes with a final boss encounter.

The project focuses on the complete playable experience currently included in the Unity build: menu navigation, save-slot selection, character selection, shop preparation, level progression, real-time combat, inventory management, equipment statistics, audio feedback, and a victory flow.

| Category | Description |
| --- | --- |
| Genre | 2D action-adventure platformer |
| Play Mode | Single player |
| Camera and Gameplay Style | Side-view 2D platforming with real-time combat |
| Target Platform | Windows Standalone (64-bit) |
| Engine | Unity 6.3 (`6000.3.16f1`) |
| Programming Language | C# |
| Campaign Content | Three playable levels, shop preparation, enemies, and a final boss |

---

## Game Premise and Objective

Centuries ago, the kingdom of Eldoria sealed a powerful dark entity known as the **Shadow King** inside the Temple of Shadow. The seal was sustained by ancient crystals. After a mysterious earthquake weakens the seal, shadow creatures begin to spread across the surrounding land.

The player enters the temple to survive its challenges, defeat hostile enemies, collect useful items and equipment, and stop the Shadow King before the darkness consumes the kingdom.

### Player Objectives

1. Select a save slot and character, then prepare in the shop.
2. Explore each level and defeat enemies using movement and combat abilities.
3. Collect gold, items, equipment, and health resources.
4. Improve the character's combat and movement statistics through equipment.
5. Complete the three campaign levels and reach the final encounter.

### Core Gameplay Loop

```text
Prepare character
      ↓
Enter level
      ↓
Explore and platform
      ↓
Fight enemies and collect rewards
      ↓
Manage inventory / equipment
      ↓
Reach the level goal
      ↓
Progress to the next level
```

---

## Implemented Gameplay Features

### Movement and Platforming

- Horizontal movement with directional character facing.
- Ground detection using Unity 2D physics.
- Double-jump capability.
- Movement, jump, landing, and footstep sound effects.
- Animation parameters for movement speed, vertical velocity, and grounded state.

### Combat

- Real-time player combat with three attack actions:
  - **Slash attack** for close-range damage.
  - **Projectile throw** for ranged attacks.
  - **Kick attack** for an additional close-range option.
- Attack cooldowns and movement locking during attack animations.
- Enemy hit detection through 2D physics overlap checks.
- Projectile support for both player and enemy attacks.
- Player and enemy health systems, damage handling, and death feedback.
- Boss encounters as part of the campaign progression.

### Enemies and Challenges

- Enemy behaviour and attack components.
- Enemy health bars and damage feedback.
- Enemy drops and collectible rewards.
- Level hazards and platforming challenges.
- Final boss gameplay at the end of the campaign.

### Items, Inventory, and Equipment

- Inventory with a limited number of item slots.
- Stackable item support.
- Health-potion consumption and healing.
- Equipment management for weapons, armour, and rings.
- Stat-based bonuses affecting the player, including damage, movement, jump, armour, and attack timing.
- Gold rewards for completing levels.

### Menus and Progression

- Main menu.
- Save-slot selection and game-profile flow.
- Character-selection scene.
- Shop scene for preparation and purchasing.
- Pause menu during gameplay.
- Level completion and victory screen.
- Three campaign levels configured in the Unity Build Settings.

### Audio and Presentation

- Background music for menu, shop, forest, dungeon, and boss contexts.
- Sound effects for movement, attacks, enemy events, and user-interface actions.
- Animated character and enemy visuals.
- HUD and inventory user-interface components.

---

## Game Flow

The playable campaign uses the following scene sequence:

```text
Main Menu
    ↓
Save Slot Selection
    ↓
Character Selection
    ↓
Shop
    ↓
Level 1
    ↓
Level 2
    ↓
Level 3 / Final Boss
    ↓
Victory Screen
```

| Build Order | Unity Scene | Purpose |
| --- | --- | --- |
| 0 | `MainMenu.unity` | Entry point and main navigation. |
| 1 | `SaveSlotSelect.unity` | Select or manage a game profile. |
| 2 | `CharacterSelect.unity` | Choose the character for the campaign. |
| 3 | `Shop.unity` | Prepare with items and equipment. |
| 4 | `Level1.unity` | First playable campaign level. |
| 5 | `Level 2.unity` | Second playable campaign level. |
| 6 | `Level 3.unity` | Final playable campaign level and boss progression. |

---

## Levels and Progression

| Level | Theme | Player Focus | Key Content |
| --- | --- | --- | --- |
| Level 1 — Forgotten Forest | Introductory temple approach | Learn movement, jumping, combat, and item collection. | Basic enemies, rewards, and first campaign progression. |
| Level 2 — Ancient Dungeon | Deeper temple area | Increased combat and platforming difficulty. | Additional enemy challenges, hazards, and exploration. |
| Level 3 — Heart of the Temple | Final temple area | Apply the full set of learned mechanics. | Advanced encounters and the final Shadow King confrontation. |

### Progression Systems

- The player earns gold as a level-completion reward.
- Gold and collected items support preparation in the shop.
- Equipment improves character statistics and enables different play styles.
- Health potions provide recovery during gameplay.
- Completing a level opens the path to the next stage of the campaign.

---

## Controls

| Action | Keyboard Input | Notes |
| --- | --- | --- |
| Move Left / Right | `A` / `D` or Left / Right Arrow | Uses the horizontal input axis. |
| Jump | `Space` or `W` | Double jump is supported. |
| Slash Attack | `J` | Close-range attack. |
| Throw Projectile | `K` | Ranged attack. |
| Kick Attack | `L` | Close-range attack. |
| Open / Close Inventory | `I` | Opens the inventory interface. |
| Use Health Potion | `H` | Uses the first available health potion. |
| Pause / Resume | `Esc` | Opens or closes the pause menu. |


---

## User Interface

The project contains separate menu and gameplay interfaces to support a clear player journey.

| Interface | Purpose |
| --- | --- |
| Main Menu | Starts the game and provides the main navigation entry point. |
| Save Slot Selection | Lets the player select a profile before beginning a session. |
| Character Selection | Lets the player select the character used in the campaign. |
| Shop | Displays purchasable or usable items and equipment before gameplay. |
| In-Game HUD | Communicates health, gameplay state, and item-related information. |
| Inventory | Shows collected items, consumables, and equipment. |
| Pause Menu | Lets the player pause, resume, restart, or return to the main flow. |
| Victory Screen | Appears after a level goal is completed. |

---

## Software and Technologies

| Item | Details |
| --- | --- |
| Game Engine | Unity 6.3 (`6000.3.16f1`) |
| Programming Language | C# |
| Physics | Unity Physics 2D and `Rigidbody2D` / `Collider2D` components |
| Animation | Unity Animator and animation events |
| Data Design | ScriptableObjects for items and equipment data |
| User Interface | Unity UI menus, inventory UI, shop UI, and HUD elements |
| Audio | Unity Audio system for background music and sound effects |
| Version Control | Git |
| Repository Hosting | GitHub |
| Target Build | Windows Standalone (64-bit) |

### Technical Implementation Highlights

- **Component-based architecture:** Gameplay behaviour is organised into focused C# components, such as player movement, combat, health, enemy behaviour, inventory, shop, and UI controllers.
- **Data-driven equipment:** Equipment and item data are stored as ScriptableObjects, allowing statistics and item properties to be adjusted without rewriting gameplay code.
- **Event-driven updates:** Inventory and character-stat changes notify dependent systems and UI components.
- **2D collision systems:** Movement, ground checks, attacks, projectiles, triggers, and level goals use Unity 2D physics.
- **Scene-based game flow:** Each primary menu and campaign stage is represented by a dedicated Unity scene.

---

## Project Structure

```text
Temple Of Shadow/
├── Assets/
│   └── _Project/
│       ├── Scenes/                 # Menus, shop, and campaign levels
│       ├── Scripts/
│       │   ├── Player/             # Movement, combat, health, stats
│       │   ├── Enemy/              # Enemy AI, attack, health, drops
│       │   ├── Combat/              # Projectile and combat behaviour
│       │   ├── Manager/             # Inventory, equipment, sound systems
│       │   ├── Shop/                # Shop data and navigation
│       │   ├── UI/                  # Menus, HUD, inventory, victory UI
│       │   └── Collectable/         # Item and equipment definitions
│       ├── ScriptableObjects/       # Equipment and item data assets
│       ├── Resources/               # Runtime content configuration
│       └── Imported/                # Imported art and third-party assets
├── ProjectSettings/                 # Unity project configuration
└── Packages/                        # Unity package dependencies
```

---

## How to Run the Project

### Requirements

- Unity Hub.
- Unity Editor version `6000.3.16f1`.
- A Windows computer for the configured Standalone Windows target.

### Run in the Unity Editor

1. Clone or download this repository.
2. Open **Unity Hub** and select **Add**.
3. Select the `Temple Of Shadow` project folder.
4. Open the project using Unity version `6000.3.16f1`.
5. Wait for Unity to import all assets and compile scripts.
6. Open `Assets/_Project/Scenes/MainMenu.unity`.
7. Select the **Play** button in the Unity Editor.

### Create a Windows Build

1. Open **File → Build Profiles** or **File → Build Settings** in Unity.
2. Select **Windows, Mac, Linux** and set the target to **Windows**.
3. Confirm that the menu, shop, and all three levels are enabled in the build-scene list.
4. Select **Build** and choose an output folder.
5. Run the generated Windows executable.

---

## Screenshot Gallery


### 1. Main Menu

![Main menu screenshot placeholder — add `docs/images/01-main-menu.png`](docs/images/01-main-menu.png)



### 2. Save Slot Selection

![Save-slot screen placeholder — add `docs/images/02-save-slot-select.png`](docs/images/02-save-slot-select.png)



### 3. Character Selection

![Character-selection screen placeholder — add `docs/images/03-character-select.png`](docs/images/03-character-select.png)

### 4. Shop

![Shop screen placeholder — add `docs/images/04-shop.png`](docs/images/04-shop.png)


### 5. Inventory

![Inventory screen placeholder — add `docs/images/05-inventory.png`](docs/images/05-inventory.png)



### 6. Boss Encounter

![Boss encounter placeholder — add `docs/images/11-boss.png`](docs/images/11-boss.png)



## Resources and Attribution

### Audio Resources

| Resource | Author | Source | Licence | Used For |
| --- | --- | --- | --- | --- |
| RPG Essentials SFX | [Author name] | [Source URL] | [Licence] | Gameplay and interface sound effects. |
| Ultimate UI SFX Pack | JDSherbert | [Source URL] | [Licence] | User-interface sounds. Attribution required; include the exact wording from the licence. |
| Jump SFX | Ambroggio | [Source URL] | [Licence] | Player jump sounds. |
| Super Dialogue Audio Pack | [Author name] | [Source URL] | [Licence] | Damage, death, and dialogue cues. |
| 16-bit Fantasy & Adventure Music (2025) | xDeviruchi | [Source URL or bundled-pack reference] | [Exact licence text] | Menu, forest, dungeon, boss, and shop music. |
| 16 Monster Growls | StarNinjas | https://opengameart.org/content/16-monster-growls | CC0 | Enemy hurt and death vocal variations. |

### Art Resources

| Resource | Author | Source | Licence | Used For |
| --- | --- | --- | --- | --- |
| Free 2D Monster Sprites | CraftPix.net | https://craftpix.net/ | CraftPix Free License | Enemy sprite artwork. |
| Free Slime Mobs Pixel Art Top-Down Sprite Pack | CraftPix.net | https://craftpix.net/ | CraftPix Free License | Slime, boss, and bat enemy visuals. |



### Game Design Inspirations

The original project brief identifies the following titles as gameplay and artistic inspirations:

- **Magic Rampage**
- **Hollow Knight**
- **Castlevania**

These titles are referenced as creative inspiration only. 
---

