---
uid: FAQ
---

# FAQ

## Table of Contents

* [CS Questions](#CS_Questions)
* [Unity Questions](#Unity_Questions)
  * [What Are MonoBehaviours and their significance?](#UnityQuestion1)
  * [How does FixedUpdate work?](#UnityQuestion2)
* [Project Specific Questions](#Project_Specific_Questions)
  * [There are various annotations in the code which are standalone lines wrapped in square brackets. What do these do?](#ProjectQuestion1)

## CS Questions <a name="CS_Questions"></a>

## Unity Questions <a name="Unity_Questions"></a>

####  What are MonoBehaviors and their significance? <a name="UnityQuestion1"></a>

Many of the scripts in the code base inherit from the class MonoBehaviour. This signifies that the class belongs to a "Game Object". A "Game Object" is the most general type of object that can exist in the "Scene" (the game world). Sprites would be the most typical sort of Game Object but the camera and even game manager happen to be game objects. Not all Game Objects own a MonoBehaviour, but you wouldn't have a MonoBehaviour without a GameObject, at least I can't think of a reason why you would. Inheriting from MonoBehaviour gives you access to all the properties and methods of the GameObject, such as controlling its position, checking if the user clicked it, and recieving Update and FixedUpdate calls.

####  How does FixedUpdate work? <a name="UnityQuestion2"></a>

MonoBehaviours, in addition to giving you control over a GameObject, and access to all of its properties, have Awake() and Update() (among other similar methods). Awake() gets called when the object is created and Update() gets called once per frame (60 times per second). But in addition to Update() there is FixedUpdate() which gets called once per physics tick. Unity makes no guarantees that the game's framerate will stay at 60, but it does strictly enforce FixedUpdate() frequency (how it does this I don't know). According to best practices input logic and logic to do with visual changes goes in Update(), and logic to do with physics goes in FixedUpdate().

## Project Specific Questions <a name="Project_Specific_Questions"></a>

#### There are various annotations in the code which are standalone lines and wrapped in square brackets. What do these do? <a name="ProjectQuestion1"></a>

The annotations define a serializable object. The serializer I'm using is called MessagePack. [MessagePackObject] preceding a class indicates that class is "MessagePack-serializable". [IgnoreMember] indicates that when the object is serialized, that member is NOT serialized, it's just ignored. [Key(0)]  thorugh [Key(n)] are the members to be serialized. [SerializationConstructor] is the constructor used to deserialize the object (it isn't mandatory, MessagePack will try its best to pick an appropriate constructor). The number of the key simply indicates the order of the parameters in the constructor. Maybe has some functionality for advanced usage scenarios, not sure.