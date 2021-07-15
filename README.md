# StubbedShaderConverter
This is a Thunderkit Built project that enables access to RoR2's shaders through a stubbed version, and tools for converting any stubbed shader in an asset bundle or object into Hopoo equivalents.

If you want to print debug information, each method has a `debug` parameter that you can enable.

## Usage

1. Install [StubbedShaderConverter](https://thunderstore.io/package/ValeX/StubbedShaderConverter/) as a mod from Thunderstore. Reference this mod's dll within your project.
    - If using [ThunderKit](https://github.com/risk-of-thunder/R2Wiki/wiki/Creating-Mods-with-Thunderkit), add this mod as a package to your ThunderKit unity project.
3. Import the StubbedShaders folder into your unity project's `Assets` folder. This will allow you to set your material shaders to stubbed hopoo versions.
4. Edit the properties of your material to desired values.
5. Within the code of your mod:
    - Make sure your mod has `[BepInDependency("com.valex.ShaderConverter", BepInDependency.DependencyFlags.HardDependency)]` in your mod's BaseUnityPlugin script.
    - Make sure you are `using StubbedConverter;`.
    - us `ShaderConvert.ConvertAssetBundleShaders` and plug in your AssetBundle.
    - If you want to use CloudFix, enable the cloudFix bool and call the method in your mods `Start()` or later. __Do not use CloudFix in Awake(), it will fail!__
6. Done!

<sub>CloudFix is specifically for CloudRemap Materials (which are used for VFX or transparent materials) and fixes some issues with using stubbed versions. If you don't use cloud remap materials in your mod you can call `ShaderConvert.ConvertAssetBundleShaders` in Awake just fine. Thanks to Kevin for providing the CloudFix solution!</sub>

### In the case that GameObjects are still using their stubbed shader versions:

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
