# Gunvalanche

Gunvalanche is a small Serious Sam-like shooter prototype built with Unity. The repository contains all Unity assets and sources needed to open and run the project.

For the reasoning behind architectural choices (and known open issues), see [`DECISIONS.md`](DECISIONS.md).

## Prerequisites
- **Unity 2022.3.59f1** or newer (see `ProjectSettings/ProjectVersion.txt`).
- Unity Hub installed to manage Unity editors and open the project.

## Opening the Project
1. Clone or download this repository.
2. In **Unity Hub**, choose **Add project** and select the `Gunvalanche` folder.
3. When prompted, open the project using Unity 2022.3.59f1 or a compatible version.

## Running in the Editor
1. Open the `Menu.unity` scene located in `Assets/Scenes`.
2. Press **Play** in the Unity Editor to start the game from the main menu.

## Building
1. Open **File ▸ Build Settings** in the Unity Editor.
2. Add the scenes `Menu.unity`, `Loading.unity`, and `Gameplay.unity` to the build list in that order.
3. Choose your target platform and click **Build** to produce a standalone build.

