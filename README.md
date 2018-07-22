# A Dangerous Land

A Dangerouos Land is top-down 2D RPG focusing on intricate combat mechanics,
crafting, exploration, and survival.

## Setup Instructions:

1) Have current versions of Unity and Visual Studio Installed. When you install
   Visual Studio also install the "Game Development with Unity" module. If you
   already have VS installed download and run the VS installer (not VS), select
   "Modify" and then install "Game Developmnt with Unity".

2) Clone the repo to your desired working directory. By default Unity projects
   are created in ~/Documents/UnityProjects, so that's a good choice.

3) Start Unity, select "Open" at the top right and select the project folder.

4) This project is written in C#7 which Unity does not yet support as of Unity
   2018. Follow the instructions here to replace the default Unity compiler:
      https://bitbucket.org/alexzzzz/unity-c-5.0-and-6.0-integration/src

6) Open VS from Unity by selecting "Assets">"Open C# Project". The first time
   you do so you may be prompted to update your .NET version. Go ahead and do
   that.

5) In Unity select "File">"Open Scene" and open Assets/Scenes/StartingScene.
   Discard changes to the existing scene. You should be all set now.
