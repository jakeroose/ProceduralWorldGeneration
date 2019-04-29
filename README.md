# Procedural World Generation
This is a project where I can explore procedurally generating a terrain by utilizing different noise functions, mainly SimpleNoise. 

I am developing this to be used as a tool that I can use in other projects, so I have also done some Unity Editor scripting to make sure everything is tidy in the inspector. 

![alt text](https://raw.githubusercontent.com/jakeroose/ProceduralWorldGeneration/scriptableObjects/githubImages/TerrainExample.png)

### Features
*This is for the scripts listed under Scripts/NoiseBasedGeneration*
- Generates a 3D mesh of the terrain
- Ability to adjust the size and resolution of the mesh
- Colors the terrain based on elevation via a custom shader and a color gradient
- Ability to add multiple noise filters
- Ability to edit settings for the noise filters via the inspector
- Ability to load/save settings for the terrain via scriptable objects
- Auto updates terrain in the editor when settings are updated (toggleable)
- Adjust water level

Noise filters:
- Simple - for generic looking hills
- Rigid  - for generating rigid terrain like mountian peaks


### Project Info
Unity Version: 2018.2.5f1
If you wish to download and demo this project, keep in mind that this is something I have been playing around with for a while, so there are quite a few other scripts in this project that aren't used for the current world generation.

To set up your own scene using the terrain generation tool, you will need the following files/folders:
- Assets/Editor
- Assets/Graphics
- Assets/Scripts/NoiseBasedGeneration
- Assets/Scripts/res/OpenSimplexNoise.cs

Then, create an empty game object in the scene and attach the Terrain.cs script to it. Set the color settings and terrain settings to the corresponding files in NoiseBasedGeneration/settings. 

This project was adapted from Sebastian Lague's [YouTube series](https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8) on procedural planets.
