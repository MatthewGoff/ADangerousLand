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

4) In Unity select "File">"Open Scene" and open Assets/Scenes/MainScene.

5) Open VS from Unity by selecting "Assets">"Open C# Project". The first time
   you do so you may be prompted to update your .NET version. Go ahead and do
   that (you may need to restart your computer).

6) In VS go to "Poject">"Manager Nuget Packages" and install MessagePack at the
least. If you plan on working with MessagePack you can also install
MessagePackAnalyzer as well.

## DevelopmentNotes

See DevelopmentNotes.txt in the top level directory. 

## Coding Conventions

I try to strickly adhere to the "official" recomendations for C# at https://docs.microsoft.com

[Naming Convensions](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)
[Layout, Comments etc.](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)

## Tutorial: Adding enemies

### Import Sprite:

1) Open unity and have the following panels open: Hierarchy, Project, Scene, Inspector (Likely open by default).
2) Have a .PNG file with transparency if desired. For convention, name it with an enemy type and the world sprite (e.g. "DragonSprite").
2) Drag and drop your .PNG file into the Assets/Sprites/Enemies folder in the Project panel.
3) Click on the sprite to bring it up in the inspector. Set "Pixels Per Unit" to 32; "Filter Mode" to "Point (no filter); and "Compression" to "None". Hit apply.

### Create Prefab:

1) Drag and drop the sprite from the Project panel into the scene, this will make it appear in the hierarchy and the Inspector.
2) In the Inspector, set the "Sorting Layer" to "Players".
3) Drag and drop the sprite from the Hierarchy into the Assets/Resources/Prefabs/Enemies folder in the Project panel.

### Create Configuration:

1) Open Assets/Scripts/Enemies/EnemyTypes.cs and add the name of your enemy to the list.
```cs
public enum EnemyType
{
    Soldier,
    Werewolf,
    Dragon,
}
```
2) Open Assets/Scripts/Configuration.cs and scroll to the definition of ENEMY_CONFIGURATIONS.
3) Copy and paste the braketed block of another enemy configuration onto the end of the list.
4) Enter your desired configuration minding that the EnemyType is the same that you put in EnemyType.cs and the SpriteLocation is the same name as the sprite.
```cs
{EnemyType.Dragon, new EnemyConfiguration(){
   SpriteLocation = "DragonSprite",
   AIType = AIType.Basic,
   MaxHealth = 1000,
   MoveSpeed = 1f,
   ExperienceReward = 200,
   Damage = 5f,
   AttackSpeed = 0.5f,
   Aoe = 10f
}},
```

### Edit Spawn Probabilities:

1) Just below the ENEMY_CONFIGURATIONS is SPAWN_PROBABILITIES. This is a map from a dificulty rating (int) to an array of tuples. Each tuple is a type of enemy and the probability that it will spawn (or more accuratly the probability that IF an enemy spawns in an area of that dificulty rating it will be of that type). In this example, when a chunk with dificulty rating 9 spawns an enemy there is a 45% it will be a soldier, 45% that it will be a werewolf, and 10% that it will be a dragon.

```cs
{9, new (float, EnemyType)[]
{
   (0.45f, EnemyType.Soldier),
   (0.45f, EnemyType.Werewolf),
   (0.1f, EnemyType.Dragon),
}},
```