# StubbedShaderConverter
This is a [ThunderKit](https://github.com/PassivePicasso/ThunderKit) Built project that enables access to RoR2's shaders through a stubbed version, and tools for converting any stubbed shader in an asset bundle or object into Hopoo equivalents.

If you want to print debug information, each method has a `debug` parameter that you can enable. Please set `debug` to false before you release your mod!

## Usage

1. Install [StubbedShaderConverter](https://thunderstore.io/package/ValeX/StubbedShaderConverter/) as a mod from Thunderstore. Reference this mod's dll within your project.
    - If using [ThunderKit](https://github.com/PassivePicasso/ThunderKit), make sure to add StubbedShaderConverter through ThunderKit's Plugin Manager within Unity.
2. Extract the folder from the .zip within the mod and import the StubbedShaders folder into your unity project. This will allow you to set your material shaders to stubbed hopoo versions.
3. Edit the properties of your material to desired values.
4. Within the code of your mod:
    - Make sure your mod has `[BepInDependency("com.valex.ShaderConverter", BepInDependency.DependencyFlags.HardDependency)]` in your mod's BaseUnityPlugin script.
	- Make sure you are `using StubbedConverter;`.
	- Within your mod's `Awake()`, use `ShaderConverter.ConvertStubbedShaders()` and plug in your AssetBundle.
5. Done!

## KomradeSpectre's MaterialControllerComponent

### THIS TOOL ONLY WORKS WITH STANDARD, CLOUD REMAP, SNOW TOPPED AND INTERSECTION SHADERS.
This is a tool that can be used with [Twiner's RuntimeInspector](https://thunderstore.io/package/Twiner/RuntimeInspector/) to control/customise materials in-game. This is extremely useful in getting the exact settings you want/need for materials. 

1. Make sure you are `using StubbedConverter;`
2. Call `MaterialController.AddMaterialController` and plug in the specific `GameObject` you want to customise the materials of.
    - If you are using[ThunderKit](https://github.com/PassivePicasso/ThunderKit), you can attach the `AddMaterialController` component to the desired `GameObject`.
3. In-game, open RuntimeInspector.
4. Select your GameObject in the heirarchy view on the left window.
5. Select the relevant Renderer GameObject, which now has a `Controller Component` to edit the material's shaders.
6. Apply resulting shader properties to the material within your unity project!
