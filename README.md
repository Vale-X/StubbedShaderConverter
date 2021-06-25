# StubbedShaderConverter (UNRELEASED)
This is a Thunderkit Built project that enables access to RoR2's shaders through a stubbed version, and tools for converting any stubbed shader in an asset bundle or object into Hopoo equivalents.

### Usage
(this is a short temp tutorial for use. Upon release, I'll write a more in-depth tutorial for it).

1. Install StubbedShaderConverter as a mod from Thunderstore. Reference this mod's dll within your project.
2. Import the StubbedShaders folder into your unity project. This will allow you to set your material shaders to stubbed hopoo versions.
3. Edit the properties of your material to desired values.
4. Within the code of your mod, in your BaseUnityPlugin's 'Awake()' code:
  *. After getting a reference to your asset bundle, call `StubbedConverter.AddBundleToConvertQueue(<yourAssetBundleHere>);`.
  *. If you want add debug information, use `StubbedConverter.AddBundleToConvertQueue(<yourAssetBundleHere>, true);` instead.
5. Done!

__In the case that GameObjects are still using their stubbed shader versions:__

You can make use of `ConvertGameObjectShaders` to convert individual game objects, or you can use `UpdateGameObjectMaterials` to refresh materials.
`ConvertGameObjectShaders` runs the same conversion process used for AssetBundles but on a singular GameObject. It gathers all renderers in a GameObject and processes the materials within them.
`UpdateGameObjectMaterial` finds the materials within a GameObject and then looks for matching materials within your AssetBundle. It then applies the found materials to the GameObject.
