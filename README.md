# StubbedShaderConverter (UNRELEASED)
This is a Thunderkit Built project that enables access to RoR2's shaders through a stubbed version, and tools for converting any stubbed shader in an asset bundle or object into Hopoo equivalents.

If you want to print debug information, add `, true` to any method you call.

## Usage
(this is a short temp tutorial for use. Upon release, I'll write a more in-depth tutorial for it).

1. Install StubbedShaderConverter (unreleased) as a mod from Thunderstore. Reference this mod's dll within your project.
2. Import the StubbedShaders folder into your unity project. This will allow you to set your material shaders to stubbed hopoo versions.
3. Edit the properties of your material to desired values.
4. Within the code of your mod, in your BaseUnityPlugin's 'Awake()' code:
    - Make sure you have `using StubbedConverter;` at the top of your script.
    - After getting a reference to your asset bundle, call `ShaderConvert.AddBundleToConvertQueue(<yourAssetBundleHere>);`.
    - If you want add debug information, use `ShaderConvert.AddBundleToConvertQueue(<yourAssetBundleHere>, true);` instead.
5. Done!

<sub>Using `AddBundleToConvertQueue` within Awake() instead of `ConvertAllBundleShadersImmediate` is advised because this allows the use of CloudFix. CloudFix is specifically for CloudRemap Materials (which are used for VFX or transparent materials) and fixes some issues with using stubbed versions. If you don't use cloud remap materials in your mod you can use `ConvertAllBundleShadersimmediate` instead. Thanks to Kevin for providing the CloudFix solution!</sub>

__In the case that GameObjects are still using their stubbed shader versions:__

You can make use of `ShaderConvert.ConvertGameObjectShaders` to convert individual game objects, or you can use `UpdateGameObjectMaterials` to refresh materials.

`ShaderConvert.ConvertGameObjectShaders` runs the same conversion process used for AssetBundles but on a singular GameObject. It gathers all renderers in a GameObject and processes the materials within them.  
`ShaderConvert.UpdateGameObjectMaterial` finds the materials within a GameObject and then looks for matching materials within your AssetBundle. It then applies the found materials to the GameObject.  

## KomradeSpectre's MaterialControllerComponent

### THIS TOOL ONLY WORKS WITH STANDARD, CLOUD REMAP AND INTERSECTION SHADERS.
This is a tool that can be used with [Twiner's RuntimeInspector](https://thunderstore.io/package/Twiner/RuntimeInspector/) to control/customise materials in-game. This is extremely useful in getting the exact settings you want/need for materials. 

1. Call `MaterialController.AddMaterialController` and plug in the specific GameObject you want to customise the materials of.
2. In-game, open RuntimeInspector with `ctrl + [` and `ctrl + ]`.
3. Select your GameObject in the heirarchy view on the right window.
4. Select the relevant `Controller Component` to edit the material's shaders.
5. Apply resulting shader properties to the material within your unity project!
