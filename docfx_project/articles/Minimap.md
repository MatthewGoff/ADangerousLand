# Minimap

### Structure:
The minimap is a quilt of textures, one for each chunk. In Unity a "texture" refers to a raster image. These textures are used to create materials that are then applied to "Image" or "Sprite" game objects which are responsible for the image's size and position in the scene among other things. While there are separate Image game objects in the minimap and the full screen map, both share the same texture objects. When we modify the texture of one chunk both the minimap and the full screen map will both show the change.

### Update system:
I'm working this out now