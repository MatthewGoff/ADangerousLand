# World Generation

Because the world is infinite we do not generate it all at once; instead we
generate individual chunks as needed. And because the player can see chunks they
are not presetly in we make sure to initialize chunks in a radius around the
player (presently a radius of 2 or rather a 5x5 square with each chunk 32x32
tiles). This initializatin is started when the enemy first enteres a chunk or
rather on the first frame update in which the player is in a new chunk. With the
exception of rivers a chunk's terrain is generated without knowledge of
neighboring chunks. This is desireable because we can't count on a chunk's
neighbors existing when we need to initialize it (generally half of a chunks
neighbors exist when we initialize it as the player has approached continuously
from one direction, of course it isn't always the same half).

Now to address river generation imagine that a river starts in chunk A and
continues into chunk B. If chunk A is generated first then when B is initialized
it can check if there is a river in A and then continue the river into B.
Unfortunatly it can be the case that B is generated first so when it comes time
for A to generate the river either stops at the boarder to the chunk or we
trigger a "re-initializatin" of B with newfound knowledge of the river (and you
can imagine that the re-initialization may propogate to trigger chunk C to
reiniitialize if the river travels through B). But if C or even D is
re-initialized while the player is present they will witness the river spawn
which is undesireable.

So we do choose to propagate rivers into initialized chunks with the following
workaround. When the player enters chunk 0,0 from the west (from chunk -1,0) we
initialize the rivers in chunk 0,8. The rivers may propagate and reform the
terrain but only for a certain length which is dependant on the topographical
parameters. Once we do this we finalize the terrain in chunk 0,2. And per the
design pattern "Treadmill" the tiles in chunk 0,2 don't actually manifest as
game objects until the player is approximately in the middle chunk 0,1.

More explicitely, when a player enters chunk chunk 0,0 for the first time we
loop over every chunk in a 17x17 chunk radius and initialize its rivers if they
are not already initialized. This function is called "public void
InitializeRiverLocality(ChunkIndex chunkIndex)". Once that completes we loop
over every chunk in a 5x5 radius and initialize the terrain. That function is
called "public void FinalLocalityInitialization(ChunkIndex chunkIndex)".
Obviously when the player is moving in an established world the flamefront of
chunks generated is small compared to the job size when the start in a
completely new world location via spawn or teleport. But even the generation of
the flamefront is wall time expensive so we thread the job to avoid the game
pausing to generate chunks when the player moves between chunks. This means
distant chunks are being generated concurent with player movement.