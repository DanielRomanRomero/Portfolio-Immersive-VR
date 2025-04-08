# VR Game Prototype - Unity 6 (Meta Quest 3)

Welcome to the source code of a small but ambitious VR prototype developed in Unity 6, tailored for Meta Quest 3. 

**Note:** This prototype was created as a portfolio project. The systems implemented are deliberately robust and scalable, intended to reflect the architecture of a larger game. However, the final result is a small experience that doesn't use every system to its full potential — by design.

On the other hand, it doesn't cover every possible situation that can occur while playing the game, even though they could easily be covered, since the ultimate goal was to create a video for the portfolio and I basically don't have infinite time, I had to make decisions to leave things out in order to continue with other more important goals - like getting a job or continue learning XR development -.


This project blends basic VR movements/interactions, puzzle solving, immersive state-based progression, shooter elements, and  a Beat Saber-style gameplay. 
It has been designed not only as a technical challenge but as a learning and showcase tool.

Access is open so it can be analyzed by recruiters or reviewers interested in the structure, coding style, and system design.

---

## Game Structure & State Machines

This prototype is structured into two main scenes:

🧩 Scene 1 – Dani Dev Room
Acts as an introductory space where the player completes a sequence of objectives (lights, notebook, interaction, sofa).

Driven by:

A global Game StateMachine (DaniDevRoomState) controlling the room flow.

An Objective StateMachine (ObjectiveStateMachine) managing sub-goals like turning on lights or placing the notebook.

Each objective is modular (IObjectiveState), allowing easy expansion or reordering. (Look for references of GameManager.Objectives.ChangeObjective to find the triggers that are changing to new objectives)

🧩 Scene 2 – Main
A larger, more varied environment divided into 6 rooms, each represented by a specific state class:

Room0State, Room1State, ..., Room5State

The main Statemachine handles room transitions, gameplay events, and pacing.

Each room encapsulates different mechanics:

Room 0: Intro

Room 1: Puzzle (Desk keys have to be placed in right order)

Room 2: Shooter section

Room 3: Beat Saber-inspired rhythm minigame

Room 4: Find and press the 2 buttons on the room

Room 5: Endgame puzzle



State Logic Embedding
State transitions are not managed from a central switch, but from gameplay events (puzzle solved, teleport triggered, interaction completed).

This leads to a natural embedding of state logic within each room's core mechanic.

Despite the small scope of the game, this setup was chosen to mimic larger scalable architectures found in real-world projects. 

Anyway, it can be modified easily to be handled directly by the State classes themselves or centrally by the GameManager, as demonstrated in the Main scene.

---

### Layer Configuration
Make sure your layers match this setup:
![Layer Setup](Docs/Layers.png)

### Physics Layer Collision Matrix
Set your physics matrix like this:
![Physics Matrix](Docs/Layer Collision Matrix.png)

## 🎮 Key Gameplay Features

- **VR Interaction & Locomotion**  
  Movement, grab, teleport, climb and interact with objects naturally using XR Interaction Toolkit.

- **Puzzles**  
  Solve environmental and logic puzzles with physical interactions (sliders, buttons, combination locks, object placement).

- **Shooter Mechanics**  
  Pick up pistols or dart guns and shoot enemies/objects or targets with feedback.

- **Beat Saber-Inspired Rhythm Minigame**  
  Use color-coded sabers to slice cubes in sync with music.

- **Subtitles & Audio System**  
  Centralized robot audio system with synchronized subtitles, powered by ScriptableObjects.

- **State Machine Architecture**  
  Modular game state & objective handling through custom state machines.

- **Visual Feedback**  
  Dissolve shaders, VFX transitions, highlights, and dynamic fog for mood.

- **Enemy AI**  
  Simple FSM-based enemies that chase, attack, and react to hits. (Player death is not handled -but prepared- since it was for a video portfolio purpose)

---

## 🧠 Code Highlights

- **State Machines**
  - Global Game State (`StateMachine`) and Objective-specific State (`ObjectiveStateMachine`)
  - Easily extendable with interfaces (`IState`, `IObjectiveState`)

- **Audio & Subtitle Sync**
  - `RobotSoundManager` handles VO clips and displays subtitles based on custom timing, can be easily extended with scriptable objects.

- **Beat Event System**
  - `BeatController` schedules cube spawns based on BPM using `AudioSettings.dspTime`

- **Weapon System**
  - Includes dissolve transitions between hands, pistols, and sabers
  - Uses pooling (`ProjectilePooling`, `HitPooling`) for optimized performance

- **VR Interaction**
  - XR Grab, Socket, Slider, and custom Push Button implementations

- **Scene Flow**
  - `GameManager` handles room transitions and state progressions

---

## 🧠 Optimization Notes

This project has been carefully optimized using numerous techniques—many of them taking advantage of the performance enhancements introduced in Unity 6, such as:

⚙️ GPU Resident Drawer for efficient GPU-side mesh batching  
💡 Adaptive Probe Volumes instead of traditional Light Probes  
🕳️ GPU Occlusion Culling to minimize overdraw  

Additional techniques:

🎯 Pooling Systems — Projectiles, enemies, hit VFX, rhythm cubes  
💤 Manual Activation/Deactivation of gameplay systems  
🎚️ Lightweight shaders and dissolve effects without runtime cost  
🧩 Separated scenes for main menu and gameplay  

💡 If you're experiencing performance drops, try:

🔻 Lowering mesh poly counts  
❌ Disabling Post Processing in the Main scene  
🌙 Reducing light settings further (they’re already tuned for VR)

---

## 🚀 Getting Started (Optional)

If you're looking to explore or fork the project:

- Requires Unity 6 and XR Plugin Management setup
- Designed for Meta Quest 3
- Plug in XR Hands/Controllers to test interactions

---

## 🤝 License & Portfolio Use

This repository is shared for portfolio and educational purposes.  
Please do not redistribute the content as your own.  
Feedback and critique are welcome!

Third Party Assets

The project may include third-party assets, some of which are paid. Therefore, redistribution or commercial use is strictly prohibited.
I am not responsible for how others might use these assets.

## 🙏 Acknowledgments

This project was created after completing the Unity Learn VR course and the *Become a VR Developer* program by Immersive Insiders.

Most of the assets were provided as part of those two courses, with a few exceptions.

Thanks to both for providing the resources to get started in the exciting world of VR.


**Made with ❤️ by a passionate Unity developer.**
