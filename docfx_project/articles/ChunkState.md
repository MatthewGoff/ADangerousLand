# Chunk State

As the player moves through the wold game objects are created and destoryed as
needed. Additionally we want enemies to spawn in chunks that are out of sight
but only those which the player approaches. For this reason we consider a chunk
to be in one of three states:
1) Live: This is a chunk that the player can see, all game objects should be
active.
2) Spawning Grounds: This is a chunk that the player cannot see where we want
to spawn enemies. In addition, because we don't want to instantiate a chunks
contents all at once but we want them to be fully instantiated when it becomes live, we require a state in which a chunk instantiates or destoryes its tiles over a period of time (the time it takes a player to travers one full chunk). This
comes at the cost of having more game objects in the scene at any one time.
3) Inactive: This chunk is very far from the player and should have no game
objects and no update calls. It is still kept in memory which I might try to 
address at a later point.

Of course state 1 is the most expensive and state 3 is the least expensive and
we'd generally like states to be as low on the list as possible. Additionally
we're assuming at 1920x1080 resolution on which the player can see 60 tiles
across at any one time. For these reasons we maintain the following invarient
stated in the simplified case of one dimention:

| -3 | -2 | -1 |  0 |  1 |  2 |  3 |

chunks -3 onward are inactive
chunk -2 is spawning grounds
chunk -1 is live
chunk 0 is live and contains the player
chunk 1 is live
chunk 2 is spawning gounds
chunks 3 onward are inactive

Here is the lifecycle so to speak as the player moves from chunk -1 to 1
accross chunk 0.
The following happen in the first fixed update in which the
players position is in chunk 0.
1) Chunks 3 and -3 (namely -3) check that all of their tiles and enemies have
been destroyted.
2) Chunk 2 and -2 (namely 2) check that all of their preexisting enemies have been created
3) Chunks 2 and -2 (namely 2) spawn up to their maximum enemies.

The following happen while the player is in chunk 0
1) Chunks 1, 0, -1 have Update() called to update fog of war.
2) In chunks 2 and -2 every fixed update any tile less than 32f distance
from the player is created and any tile more than 32f distance is
destroyed

By the time the player reaches chunk 1, chunk -2 has had almost all its tiles destroyed and is ready to become inactive. Chunk -1 is out of view and is ready to
have its enemies replenihed and its tiles destroyed. Chunk 2 has had its enemies replenished and tiles instantiated and is
ready to come into view. Chunk is comming up so it is about to become spawning grounds