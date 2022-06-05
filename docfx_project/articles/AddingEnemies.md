# Tutorial: Adding Enemies

Four Steps:
 * [Import Sprite](#Import_Sprite)
 * [Create Prefabs](#Create_Prefabs)
 * [Create Configuration](#Create_Configuration)
 * [Edit Spawn Probabilities](#Edit_Spawn_Probabilities)

## Import Sprite: <a name="Import_Sprite"></a>

1) Open Unity and have the following panels open: Hierarchy, Project, Scene, Inspector. These will likely open by default.
2) Have a .PNG file, which can have transparency if desired. Name it with an enemy type and the word "Sprite" (e.g. "DragonSprite").
2) Drag and drop your .PNG file into the `Assets/Sprites/Enemies` folder in the Project panel.
3) Click on the sprite to bring it up in the Inspector. Set "Pixels Per Unit" to 32; "Pitov" to "Bottom"; "Filter Mode" to "Point (no filter); and "Compression" to "None". Hit apply.

## Create Prefabs: <a name="Create_Prefabs"></a>

Your enemy needs three constituent "Unity Game Objects": The sprite, the hitbox, and the "master" game object which contains the physics components and the script which will operate the enemy.

Start by creating a new folder in `Assets/Resources/Prefabs/Enemies` named for your enemy (do this from within Unity).

#### Create Sprite
1) Drag and drop the sprite from the Project panel into the scene, this will make it appear in the Hierarchy and the Inspector.
2) In the Inspector, set the "Sorting Layer" to "Terrain Foreground".
3) Drag and drop the sprite from the Hierarchy into the prefabs folder you created for this enemy.

#### Create Hitbox
1) In the Hierarchy, choose "Create">"Create Empty" to create a new empty game object.
2) Name it with the enemy type and the word "Hitbox" (e.g. "DragonHitbox").
3) In the Inspector, click "Add Component" and type "collider". Select your desired 2D collider shape (e.g. Box Collider 2D).
4) In the Inspector, edit the collider parameters so that it is the desired dimensions. You can set the hitbox position and sprite position to be the same so that they overlap for better perspective (the "reset" option in the transform gear menu will put any object at the orgin). Remember that the center of the hitbox is the bottom of the sprite, so you may want to include an offset (an offset of half the height of the enemy will center the hitbox with the sprite).
5) At the top of the Inspector set the Tag of your hitbox to "Hitbox" and the Layer to "Combat".
5) Drag and drop the hitbox from the Hierarchy into the prefabs folder you created for this enemy.

#### Create Master Prefab
1) In the Hierarchy choose "Create">"Create Empty" to create a new empty game object.
2) Name it with the enemy type and the word "Prefab" (e.g. "DragonPrefab")
3) Add a collider to the game object. Unlike the hitbox which is used in combat interactions, this collider defines the physical space that the enemy occupies (e.g. the player hitbox is 2 units tall but its feet only ocupy one square unit).
4) Add a Rigidbody 2D component. Set the Gravity Scale to 0 and under Constraints check Z.
5) Click "Add Component", type "Enemy Mono Behaviour" and hit enter.
6) In the "Enemy Mono Behaviour" section of the Inspector you will see a field for "Sprite Prefab" and "Hitbox Prefab". Drag and drop the Sprite Prefab from the prefabs folder you created for this enemy (not the Hierarchy) into the sprite field and likewise with the hitbox prefab. This will give the script control over both the sprite and the hitbox.
7) Drag and drop the master prefab from the Hierarchy into the prefabs folder you created for this enemy.

Lastly, delete the sprite, hitbox, and master prefab from the Hierarchy.

## Create Configuration: <a name="Create_Configuration"></a>

1) Open `Assets/Scripts/Enemies/EnemyTypes.cs` and add the name of your enemy to the list.
```cs
public enum EnemyType
{
    Soldier,
    Werewolf,
    Dragon,
}
```
2) Open `Assets/Scripts/Configuration.cs` and scroll to the definition of `ENEMY_CONFIGURATIONS`.
3) Copy and paste the bracketed block of another enemy configuration onto the end of the list.
4) Enter your desired configuration, making sure that the EnemyType is the same that you put in `EnemyType.cs` and the Prefab Location is the location you put the master prefab relative to the `Assets/Resources/Prefabs` folder.
5) Most of the fields should be self explainatory. The attack origin is the position on the sprite that attack will originate from **relative to the bottom center**.

```cs
{EnemyType.Dragon, new EnemyConfiguration(){
   PrefabLocation = "Dragon/DragonPrefab",
   AIType = AIType.Basic,
   MaxHealth = 1000,
   MoveSpeed = 1f,
   ExperienceReward = 200,
   Damage = 5f,
   AttackSpeed = 0.5f,
   Aoe = 10f,
   AgroDistance = 10f,
   DeAgroDistance = 15f,
   MinAgroDuration = 3f,
   Width = 3f,
   Height = 3f,
   AttackOrigin = new Vector2(0, 1.5f),
}},
```

## Edit Spawn Probabilities: <a name="Edit_Spawn_Probabilities"></a>

1) Just below the `ENEMY_CONFIGURATIONS` is `SPAWN_PROBABILITIES`. This is a map from a difficulty rating (int) to an array of tuples. Each tuple is a type of enemy and the probability that it will spawn (or more accuratly the probability that IF an enemy spawns in an area of that difficulty rating it will be of that type). In this example, when a chunk with dificulty rating 9 spawns an enemy there is a 45% chance it will be a soldier, 45% chance that it will be a werewolf, and 10% chance that it will be a dragon.

```cs
{9, new (float, EnemyType)[]
{
   (0.45f, EnemyType.Soldier),
   (0.45f, EnemyType.Werewolf),
   (0.1f, EnemyType.Dragon),
}},
```