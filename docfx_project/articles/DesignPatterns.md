---
uid: DesignPatterns
---

# Design Patterns

## Table of Contents
* [Immunization](#Immunization)
* [Manager/MonoBehaviour](#Manager/MonoBehaviour)
* [Pixel Perfect Movement](#PixelPerfectMovement)
* [Treadmill](#Treadmill)


## Immunization <a name="Immunization"></a>
Attacks generally use a collider which persists in the world for some duration to determine what the attack hits. Some attacks might collide with an enemy multiple times but we may only want one hit to occur. For this reason when a combatant is hit with an attack we will "Immunize" it to that attack by saving it in a list of AttackManagers. When we immunize an combatant the combatant also registers itself as an "Expiration Listener" of the AttackManager and when the AttackManager "Expires", it will call all of its expiration listeners so that they can remove the AttackManager from their immunization list.

This may ultimately be uneccesary since collisions are usually detected with "OnTriggerEnter" which only gets called once when two colliders pass over each other.

## Manager/MonoBehaviour <a name="Manager/MonoBehaviour"></a>
Lets say we want a class to represent
something that exists in the scene like an enemy. Naturally we have a game
object with a component script with one class which inherits from MonoDevelop.
This script is neccesary to provide us with the MonoDevelop API: Update(),
OnMouseEnter() etc. . But this script does not persist through object
creation/deletion and cannot use a constructor. For this reason we use an
additional script which owns the MonoDevelop script as a property. This script
serves two purposes. First, it accepts initialization input via its constructor.
Second, it manages the instantiation and destruction of the
Prefab/MonoBehaviour. We will call the wrapper script "EnemyManager", the
MonoBahaviour script "EnemyMonoBahaviour", the prefab "EnemyPrefab", and the
GameObject doesn't matter (It's "Enemy (clone)" by default). Note that there
are some classes called Managers which do not follow this pattern.

## Pixel Perfect Movement <a name="PixelPerfectMovement"></a>
Because we want to render sprites exactly the same
as the original artwork we need the sprites to be located on the pixel grid,
instead of inbetween pixels. At the same time the objects the sprites represent
have a floating point position and we do not want to impose artificial
constraints by forcing the physics body to be located on the pixel grid as this
causes jerky, unatural movement. To solve this we use two game objects, one with
a rigid body and a collider but no sprite and one with a sprite but no rigid
body or collider. The two objects are controlled by the same monobehavior. In
conjunction with the design pattern "Two classes for one game object" this means
moving entities such as the player or the world have two scripts and to game
objects.

## Treadmill <a name="Treadmill"></a>
In order to improve performance we do not want to have the whole
world active in the hierarchy. For this reason we create objects only when the
player is "nearby" and delete them when they are "distant". We use the idea of a
"treadmill" to describe the rectangular area around the player that is active in
the hierarchy because as the player moves things will be created on one side and
deleted on the other. See the section "Chunk State" for a rational behind the
dimentions of the treadmill.
