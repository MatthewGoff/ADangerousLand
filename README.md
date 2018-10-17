# A Dangerous Land

## Setup Instructions:

If you are just looking to play the game, you can visit the 
[releases repository](https://github.com/MatthewGoff/ADangerousLand) and follow 
the README instructions.

1) Have current versions of [Unity](https://unity3d.com/get-unity/download) and 
   [Visual Studio](https://visualstudio.microsoft.com/downloads/) installed. When 
   you install Visual Studio, also install the "Game Development with Unity" module. 
   If you already have VS installed, download and run the VS installer (not VS), 
   select "Modify" and then install "Game Developmnt with Unity".

2) Clone the repo to your desired working directory. By default Unity projects
   are created in `~/Documents/UnityProjects`, so that's a good choice.

3) Start Unity, select "Open" at the top right and select the project folder.

4) In Unity select "File">"Open Scene" and open Assets/Scenes/MainScene.

5) Open VS from Unity by selecting "Assets">"Open C# Project". The first time
   you do so you may be prompted to update your .NET version. Go ahead and do
   that (you may need to restart your computer).

6) In VS go to "Project">"Manager NuGet Packages" and install MessagePack at the
   least. If you plan on working with MessagePack you can also install
   MessagePackAnalyzer as well.

## Documentation:

[Visit the documentation website!](https://matthewgoff.github.io/ADangerousLand-private/)


