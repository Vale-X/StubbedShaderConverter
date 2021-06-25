 using UnityEngine;
using System;
using System.Collections.Generic;

namespace StubbedConverter
{
    public static class ShaderConvert
    {
        internal static List<AssetBundle> assetBundles = new List<AssetBundle>();
        internal static List<bool> debugList = new List<bool>();

        // If you make use of the StubbedShaders provided in the zip for the StubbedShaderConverter mod in Unity, and then call AddBundleToConvertQueue in your code, plugging in your bundle...
        // It will replace the stubbed shaders with functional ones that work in game when it comes around to Start.
        // The reason it does this on Start is so it can make use of a vanilla game material for the CloudFix.
        // If you don't care about CloudRemap materials, or for some other reason, you can use ConvertAllBundleShadersImmediate.

        #region old
        /*/// <summary>
        /// Adds inAssetBundle to a queue that converts all material shaders in an asset bundle to Hopoo Equivelents. Applies CloudFix. 
        /// Conversion process happens within ShaderConverterPlugin.Start(). Asset bundle must be added to queue before then (within your plugin Awake).
        /// </summary>
        public static void AddBundleToConvertQueue(AssetBundle inAssetBundle, bool debug)
        {
            assetBundles.Add(inAssetBundle);
            debugList.Add(debug);
            if (debug) Debug.Log("ShaderConverter: Added " + inAssetBundle + " to ShaderConverter queue.");
        }

        public static void AddBundleToConvertQueue(AssetBundle inAssetBundle)
        {
            AddBundleToConvertQueue(inAssetBundle, false);
        }

        /// <summary>
        /// Immediately converts all material shaders in an asset bundle to Hopoo Equivelents, but without CloudFix.
        /// Cloud Remap materials may not render correctly.
        /// </summary>
        public static void ConvertAllBundleShadersImmediate(AssetBundle inAssetBundle, bool debug)
        {
            ProcessAssetBundleShaders(inAssetBundle, debug);
        }

        public static void ConvertAllBundleShadersImmediate(AssetBundle inAssetBundle)
        {
            ConvertAllBundleShadersImmediate(inAssetBundle, false);
        }*/
        #endregion

        /// <summary>
        /// Converts all stubbed shaders in materials in an AssetBundle into Hopoo Equivelents.
        /// Only enable cloudFix if used within your BaseUnityPlugin's Start() or later.
        /// </summary>
        /// <param name="cloudFix"> Do NOT enable if used within Awake of BaseUnityPlugin. Will fail if you do.</param>
        public static void ConvertAssetBundleShaders(AssetBundle inAssetBundle, bool cloudFix, bool debug)
        {
            ProcessAssetBundleShaders(inAssetBundle, cloudFix, debug);
        }

        /// <summary>
        /// Converts all stubbed shaders in materials in an AssetBundle into Hopoo Equivelents.
        /// Only enable cloudFix if used within your BaseUnityPlugin's Start() or later.
        /// </summary>
        /// <param name="cloudFix"> Do NOT enable if used within Awake of BaseUnityPlugin. Will fail if you do.</param>
        public static void ConvertAssetBundleShaders(AssetBundle inAssetBundle, bool cloudFix)
        {
            ConvertAssetBundleShaders(inAssetBundle, cloudFix, false);
        }
        /// <summary>
        /// Converts all stubbed shaders in materials in an AssetBundle into Hopoo Equivelents.
        /// Only enable cloudFix if used within your BaseUnityPlugin's Start() or later.
        /// </summary>
        public static void ConvertAssetBundleShaders(AssetBundle inAssetBundle)
        {
            ConvertAssetBundleShaders(inAssetBundle, false, false);
        }

        /// <summary>
        /// Converts all shaders in materials of a GameObject into Hopoo Equivelents.
        /// </summary>
        public static void ConvertGameObjectShaders(GameObject inObject, bool debug)
        {
            Debug.Log("ShaderConverter: Converting " + inObject.name + "'s Shaders...");
            Renderer[] objectRenderers = GetGameObjectRenderers(inObject, debug);
            RendererShaderSwap(objectRenderers, debug, true);
            if (debug) Debug.Log("ShaderConverter: Finished converting " + inObject.name + "'s Shaders!");
        }

        /// <summary>
        /// Converts all shaders in materials of a GameObject into Hopoo Equivelents.
        /// </summary>
        public static void ConvertGameObjectShaders(GameObject inObject)
        {
            ConvertGameObjectShaders(inObject, false);
        }

        /// <summary>
        /// Converts all shaders in materials of multiple GameObjects into Hopoo Equivelents.
        /// </summary>
        public static void ConvertGameObjectShaders(GameObject[] inObjects, bool debug)
        {
            Debug.Log("ShaderConverter: Converting " + inObjects.Length + " GameObject shaders...");
            List<Renderer> allRenderers = new List<Renderer>();

            foreach (GameObject g in inObjects)
            {
                Renderer[] objectRenderers = GetGameObjectRenderers(g, debug);
                allRenderers.AddRange(objectRenderers);
            }
            RendererShaderSwap(allRenderers.ToArray(), debug, true);
            if (debug) Debug.Log("ShaderConverter: Finished converting " + inObjects.Length + " GameObject shaders!");
        }

        /// <summary>
        /// Converts all shaders in materials of multiple GameObjects into Hopoo Equivelents.
        /// </summary>
        public static void ConvertGameObjectShaders(GameObject[] inObjects)
        {
            ConvertGameObjectShaders(inObjects, false);
        }

        /// <summary>
        /// Refreshes materials of a specified gameobject with matching materials from an asset bundle.
        /// </summary>
        public static void UpdateGameObjectMaterials(AssetBundle inAssetBundle, GameObject inObject, bool debug)
        {
            Debug.Log("ShaderConverter: Updating GameObject: " + inObject + " Materials with Materials from AssetBundle: " + inAssetBundle);
            Renderer[] objectRenderers = GetGameObjectRenderers(inObject, debug);
            RendererUpdateMaterials(objectRenderers, inAssetBundle, debug);
        }

        /// <summary>
        /// Refreshes materials of a specified gameobject with matching materials from an asset bundle.
        /// </summary>
        public static void UpdateGameObjectMaterials(AssetBundle inAssetBundle, GameObject inObject)
        {
            UpdateGameObjectMaterials(inAssetBundle, inObject, false);
        }


        //Internal scripts

        //Gets all Renderers from a specified GameObject, including those of children.
        internal static Renderer[] GetGameObjectRenderers(GameObject inObject, bool debug)
        {
            List<Renderer> objectRenderers = new List<Renderer>();
            objectRenderers.AddRange(inObject.GetComponents<Renderer>());
            objectRenderers.AddRange(inObject.GetComponentsInChildren<Renderer>());
            if (debug) Debug.Log("ShaderConverter: Found " + objectRenderers.Count + " renderers within GameObject: " + inObject);
            return objectRenderers.ToArray();
        }

        // This is used to manually replace each shader in a list of renderers,
        // because unity likes to copy materials at runtime and the resulting materials might not be included in the original shader conversion process.
        // An alternative for this is Updating the materials.
        internal static void RendererShaderSwap(Renderer[] renderers, bool debug, bool cloudFix)
        {
            #region oldMethod
            /*foreach (Renderer renderer in renderers)
            {
                if (renderer is MeshRenderer meshRenderer)
                {
                    foreach (Material material in meshRenderer.materials)
                    {
                        if (!material.shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: MeshRenderer's material " + material.name + "'s shader is not a stubbed hopoo shader. Skipping."); continue; }

                        var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[material.shader.name]);
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                            if (debug) Debug.Log("ShaderConverter: Matching stubbed shader of MeshRenderer's material " + material.name + " to: " + replacementShader + ". Replacing!");
                        }
                    }
                }
                else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    foreach (Material material in skinnedMeshRenderer.sharedMaterials)
                    {
                        if (!material.shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: SkinnedMeshRenderer's material " + material.name + "'s shader is not a stubbed hopoo shader. Skipping."); continue; }

                        var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[material.shader.name]);
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                            if (debug) Debug.Log("ShaderConverter: Matching stubbed shader of SkinnedMeshRednerer's material " + material.name + " to: " + replacementShader + ". Replacing!");
                        }
                    }
                }
                else if (renderer is ParticleSystemRenderer particleRenderer)
                {
                    foreach (Material material in particleRenderer.materials)
                    {
                        if (!material.shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: ParticleSystemRenderer's material " + material.name + "'s shader is not a stubbed hopoo shader. Skipping."); continue; }

                        var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[material.shader.name]);
                        if (replacementShader)
                        {
                            material.shader = replacementShader;
                            if (debug) Debug.Log("ShaderConverter: Matching stubbed shader of ParticleSystemRenderer's material " + material.name + " to: " + replacementShader + ". Replacing!");
                        }
                    }
                }
            }*/
            #endregion

            foreach (Renderer renderer in renderers)
            {
                if (!renderer) { if (debug) Debug.Log("ShaderConverter: ERROR: Renderer is missing. Wack. Skipping!"); continue;  }
                List<Material> materials = new List<Material>();
                if (renderer is MeshRenderer meshRenderer) { materials.AddRange(meshRenderer.materials); }
                else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer) { materials.AddRange(skinnedMeshRenderer.sharedMaterials); }
                else if (renderer is ParticleSystemRenderer particleSystemRenderer) { materials.AddRange(particleSystemRenderer.sharedMaterials); }
                else { if (debug)  Debug.Log("ShaderConverter: ERROR: Renderer: " + renderer + "is of type: " + renderer.GetType().Name + " and is not supported for conversion!"); continue; }

                if (materials.Count != 0)
                {
                    List<Material> newMats = new List<Material>();
                    for (int i = 0; i < materials.Count; i++)
                    {
                        if (!materials[i]) { if (debug) Debug.Log("ShaderConverter: ERROR: Material in "+ renderer.GetType().Name + " of " + renderer.gameObject + " is missing. Wack. Skipping!"); continue; }
                        if (!materials[i].shader) { if (debug) Debug.Log("ShaderConverter: ERROR: Material " + materials[i].name + "'s shader is missing. Skipping!"); continue; }
                        if (!materials[i].shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + materials[i].name + "'s shader in " + renderer.GetType().Name + " of " + renderer.gameObject + " is NOT a stubbed Hopoo shader. Skipping."); continue; }
                        if (materials[i].shader.name.ToLower().StartsWith("Hopoo")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + materials[i].name + "'s shader in "+ renderer.GetType().Name + " of " + renderer.gameObject + " is ALREADY a Hopoo shader. Skipping"); continue; }



                        var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[materials[i].shader.name]);
                        if (replacementShader)
                        {
                            materials[i].shader = replacementShader;
                            if (debug) Debug.Log("ShaderConverter: Matching stubbed shader of " + renderer.gameObject + "'s " + renderer.GetType().Name + " material " + materials[i].name + " to: " + replacementShader + ". Replacing!");
                        }

                        if (cloudFix)
                        {
                            // Check if the material is Cloud Remap. If it is, use CloudFix (thanks Kevin) instead of normal replacement.
                            if (materials[i].shader.name.Contains("Cloud Remap"))
                            {
                                // Cloud fixer code adapted from Kevin's code he sent me. Necessary in order for Cloud Remap materials to not appear black.
                                // Cloudfix is used to get values generated at runtime and apply them to any material.
                                //if (debug) Debug.Log("ShaderConverter: Message from Kevin: Fuck you Cloud Remap.");
                                Material tempMat = materials[i];
                                var cloudFixer = new CloudFix(tempMat);
                                materials[i].CopyPropertiesFromMaterial(ShaderConverterPlugin.cloudMat);
                                cloudFixer.SetMaterialValues(ref tempMat);
                                materials[i] = tempMat;
                                if (debug) Debug.Log("ShaderConverter: " + materials[i].name + " is a Cloud Remap shader. CloudFix applied!");
                            }
                        }
                        newMats.Add(materials[i]);
                    }
                    if (renderer is MeshRenderer meshRenderer2) { meshRenderer2.materials = newMats.ToArray(); if (debug) Debug.Log("ShaderConverter: Successfully Updated Materials of MeshRenderer within GameObject: " + meshRenderer2.gameObject); }
                    else if (renderer is SkinnedMeshRenderer skinnedMeshRenderer2) { skinnedMeshRenderer2.sharedMaterials = newMats.ToArray(); Debug.Log("ShaderConverter: Successfully Updated Materials of MeshRenderer within GameObject: " + skinnedMeshRenderer2.gameObject); }
                    else if (renderer is ParticleSystemRenderer particleSystemRenderer2) { particleSystemRenderer2.sharedMaterials = newMats.ToArray(); Debug.Log("ShaderConverter: Successfully Updated Materials of ParticleSystemRenderer within GameObject: " + particleSystemRenderer2.gameObject); }
                    else { if (debug) Debug.Log("ShaderConverter: ERROR: Renderer: " + renderer + "is of type: " + renderer.GetType().Name + " and is not supported for conversion!"); continue; }
                }
                else if (debug) Debug.Log("ShaderConverter: ERROR: No materials found in: " + renderer.gameObject + "!");
            }
        }

        // Replaces old materials with freshly instantiated versions of the same material.
        // Basically refreshes the material, with any changes to the original material in the asset bundle applied.
        internal static void RendererUpdateMaterials(Renderer[] renderers, AssetBundle inAssetBundle, bool debug)
        {
            #region oldMethod
            /*foreach (Renderer renderer in renderers)
            {
                if (renderer is MeshRenderer meshRenderer)
                {
                    List<Material> newMats = new List<Material>();

                    foreach (Material material in meshRenderer.materials)
                    {
                        var matName = material.name;
                        matName.Replace(" (Instance)", "");
                        matName.Replace(" (Copy)", "");
                        var replacementMat = UnityEngine.Material.Instantiate(inAssetBundle.LoadAsset<Material>(matName));
                        if (debug) Debug.Log("ShaderConverter: Found matching Material: "+ replacementMat +" in AssetBundle: " + inAssetBundle + ".");
                        newMats.Add(replacementMat);
                    }
                    meshRenderer.materials = newMats.ToArray();
                    if (debug) Debug.Log("ShaderConverter: Successfully Updated Materials of MeshRenderer within GameObject: " + meshRenderer.gameObject);
                }
                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    List<Material> newMats = new List<Material>();

                    foreach (Material material in skinnedMeshRenderer.sharedMaterials)
                    {
                        var matName = material.name;
                        matName.Replace(" (Instance)", "");
                        matName.Replace(" (Copy)", "");
                        var replacementMat = UnityEngine.Material.Instantiate(inAssetBundle.LoadAsset<Material>(matName));
                        if (debug) Debug.Log("ShaderConverter: Found matching Material: " + replacementMat + " in AssetBundle: " + inAssetBundle + ".");
                        newMats.Add(replacementMat);
                    }
                    skinnedMeshRenderer.sharedMaterials = newMats.ToArray();
                    if (debug) Debug.Log("ShaderConverter: Successfully Updated Materials of SkinnedMeshRenderer within GameObject: " + skinnedMeshRenderer.gameObject);
                }
                if (renderer is ParticleSystemRenderer particleSystemRenderer)
                {

                }
            }*/
            #endregion

            foreach (Renderer renderer1 in renderers)
            {
                if (!renderer1) { if (debug) Debug.Log("ShaderConverter: ERROR: Renderer is missing. Wack. Skipping!"); continue; }
                List<Material> materials = new List<Material>();
                if (renderer1 is MeshRenderer meshRenderer) { materials.AddRange(meshRenderer.materials); }
                else if (renderer1 is SkinnedMeshRenderer skinnedMeshRenderer) { materials.AddRange(skinnedMeshRenderer.sharedMaterials); }
                else if (renderer1 is ParticleSystemRenderer particleSystemRenderer) { materials.AddRange(particleSystemRenderer.sharedMaterials); }
                else { if (debug) Debug.Log("ShaderConverter: ERROR: Renderer: " + renderer1 + "is of type: " + renderer1.GetType().Name + " and is not currently supported for updating!"); continue; }

                if (materials.Count != 0)
                {
                    List<Material> newMats = new List<Material>();
                    foreach (Material material in materials)
                    {

                        if (!material) { if (debug) Debug.Log("ShaderConverter: ERROR: Material in " + renderer1.GetType().Name + " of " + renderer1.gameObject + " is missing. Wack. Skipping!"); continue; }
                        if (!material.shader) { if (debug) Debug.Log("ShaderConverter: ERROR: Material " + material.name + "'s shader is missing. Skipping!"); continue; }
                        if (!material.shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader in " + renderer1.GetType().Name + " of " + renderer1.gameObject + " is NOT a stubbed Hopoo shader. Skipping."); continue; }
                        if (material.shader.name.ToLower().StartsWith("Hopoo")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader in " + renderer1.GetType().Name + " of " + renderer1.gameObject + " is ALREADY a Hopoo shader. Skipping"); continue; }

                        var matName = material.name;
                        matName.Replace(" (Instance)", "");
                        matName.Replace(" (Copy)", "");
                        if (UnityEngine.Material.Instantiate(inAssetBundle.LoadAsset<Material>(matName)) == null) { if (debug) Debug.Log("ShaderConverter: ERROR: Unable to find Material: " + matName + " within Bundle: " + inAssetBundle); continue; }
                        var replacementMat = UnityEngine.Material.Instantiate(inAssetBundle.LoadAsset<Material>(matName));
                        if (debug) Debug.Log("ShaderConverter: Found matching Material: " + replacementMat + " in AssetBundle: " + inAssetBundle + ".");
                        newMats.Add(replacementMat);
                    }
                    if (renderer1 is MeshRenderer meshRenderer2) { meshRenderer2.materials = newMats.ToArray(); if (debug) Debug.Log("ShaderConverter: Successfully Updated Materials of MeshRenderer within GameObject: " + meshRenderer2.gameObject); }
                    else if (renderer1 is SkinnedMeshRenderer skinnedMeshRenderer2) { skinnedMeshRenderer2.sharedMaterials = newMats.ToArray(); Debug.Log("ShaderConverter: Successfully Updated Materials of MeshRenderer within GameObject: " + skinnedMeshRenderer2.gameObject); }
                    else if (renderer1 is ParticleSystemRenderer particleSystemRenderer2) { particleSystemRenderer2.sharedMaterials = newMats.ToArray(); Debug.Log("ShaderConverter: Successfully Updated Materials of ParticleSystemRenderer within GameObject: " + particleSystemRenderer2.gameObject);  }
                    else { if (debug) Debug.Log("ShaderConverter: ERROR: Renderer: " + renderer1 + "is of type: " + renderer1.GetType().Name + " and is not supported for conversion!"); continue; }
                }
                else if (debug) Debug.Log("ShaderConverter: ERROR: No materials found in: " + renderer1.gameObject + "!");
            }
        }

        #region old
        /*//Used internally for every asset bundle added to the assetBundles list at start. This is where the magic happens!
        //now uses ProcessAssetBundleShadersCloudFix instead, with a toggle for CloudFix.
        internal static void ProcessAssetBundleShaders(AssetBundle inAssetBundle, bool debug)
        {
            if (debug) Debug.Log("ShaderConverter: Attempting to convert shaders of bundle: " + inAssetBundle + "...");

            // Get all materials in bundle.
            var materialAssets = inAssetBundle.LoadAllAssets<Material>();

            // For every material in said bundle...
            foreach (Material material in materialAssets)
            {
                if (!material) { if (debug) Debug.Log("ShaderConverter: ERROR: Material is missing. Wack. Skipping!"); continue; }
                if (!material.shader) { if (debug) Debug.Log("ShaderConverter: ERROR: Material " + material.name + "'s shader is missing. Skipping!"); continue; }
                if (!material.shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader is NOT a stubbed Hopoo shader. Skipping."); continue; }
                if (material.shader.name.StartsWith("Hopoo Games")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader is ALREADY a Hopoo shader. Skipping"); continue; }

                // Replace the shader with the appropriate Hopoo shader used in-game.
                var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[material.shader.name]);
                if (replacementShader)
                {
                    material.shader = replacementShader;
                    if (debug) Debug.Log("ShaderConverter: Matching stubbed shader of " + material.name + " to: " + replacementShader + ". Replacing!");
                }
            }
            // Done!
            if (debug) Debug.Log("ShaderConverter: Finished converting " + inAssetBundle + "'s shaders.");
        }

        // The same as the method above, but uses a for loop instead, which allows for CloudFix. If the Cloudfix bool is true, then this can only be used with Start() or later.
        // This is because Cloudfix requires a material from the vanilla game, which is not accessible during Awake.
        internal static void ProcessAssetBundleShadersCloudFix(AssetBundle inAssetBundle, bool debug)
        {
            if (debug) Debug.Log("ShaderConverter: Attempting to convert shaders of bundle: " + inAssetBundle + ". CloudFix for Cloud Remap Materials.");
            var materialAssets = inAssetBundle.LoadAllAssets<Material>();

            for (int i = 0; i < materialAssets.Length; i++)
            {
                // Get material and check if the material is correct.
                if (!materialAssets[i]) { if (debug) Debug.Log("ShaderConverter: ERROR: Material is missing. Wack. Skipping!"); continue; }
                var material = materialAssets[i];
                if (!material.shader) { if (debug) Debug.Log("ShaderConverter: ERROR: Material " + material.name + "'s shader is missing. Skipping!"); continue; }
                if (!material.shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader is NOT a stubbed Hopoo shader. Skipping."); continue; }
                if (material.shader.name.StartsWith("Hopoo Games")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader is ALREADY a Hopoo shader. Skipping"); continue; }

                // Replace the shader with the appropriate Hopoo shader used in-game.
                var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[material.shader.name]);
                if (replacementShader)
                {
                    material.shader = replacementShader;
                    if (debug) Debug.Log("ShaderConverter: Matching stubbed shader of " + material.name + " to: " + replacementShader + ". Replacing!");
                }

                // Check if the material is Cloud Remap. If it is, use CloudFix (thanks Kevin) instead of normal replacement.
                if (material.shader.name.Contains("Cloud Remap"))
                {
                    // Cloud fixer code adapted from Kevin's code he sent me. Necessary in order for Cloud Remap materials to not appear black.
                    // Cloudfix is used to get values generated at runtime and apply them to any material.
                    //if (debug) Debug.Log("ShaderConverter: Message from Kevin: Fuck you Cloud Remap.");
                    var cloudFixer = new CloudFix(material);
                    material.CopyPropertiesFromMaterial(ShaderConverterPlugin.cloudMat);
                    cloudFixer.SetMaterialValues(ref material);
                    materialAssets[i] = material;
                    if (debug) Debug.Log("ShaderConverter: " + material.name + " is a Cloud Remap shader. CloudFix applied!");
                }
                //Apply the temporary variable copy to the original
                materialAssets[i] = material;
                if (debug) Debug.Log("ShaderConverter: Finished converting " + inAssetBundle + "'s shaders with CloudFix");
            }
        }*/
        #endregion

        internal static void ProcessAssetBundleShaders(AssetBundle inAssetBundle, bool cloudFix, bool debug)
        {
            if (cloudFix == true && ShaderConverterPlugin.cloudMat == null) { Debug.Log("ShaderConverter: ERROR: cloudFix is enabled, but cloudMat not found! Only enable CloudFix bool in Start() or later, and make sure your mod has StubbedShaderConverter as a Hard Dependency!"); return; }
            if (debug) Debug.Log("ShaderConverter: Attempting to convert shaders of bundle: " + inAssetBundle + ".");
            var materialAssets = inAssetBundle.LoadAllAssets<Material>();

            for (int i = 0; i < materialAssets.Length; i++)
            {
                // Get material and check if the material is correct.
                if (!materialAssets[i]) { if (debug) Debug.Log("ShaderConverter: ERROR: Material is missing. Wack. Skipping!"); continue; }
                var material = materialAssets[i];
                if (!material.shader) { if (debug) Debug.Log("ShaderConverter: ERROR: Material " + material.name + "'s shader is missing. Skipping!"); continue; }
                if (!material.shader.name.ToLower().StartsWith("stubbed")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader is NOT a stubbed Hopoo shader. Skipping."); continue; }
                if (material.shader.name.StartsWith("Hopoo Games")) { if (debug) Debug.Log("ShaderConverter: WARNING: Material " + material.name + "'s shader is ALREADY a Hopoo shader. Skipping"); continue; }

                // Replace the shader with the appropriate Hopoo shader used in-game.
                var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[material.shader.name]);
                if (replacementShader)
                {
                    material.shader = replacementShader;
                    if (debug) Debug.Log("ShaderConverter: Matching stubbed shader of " + material.name + " to: " + replacementShader + ". Replacing!");
                }

                if (cloudFix)
                {
                    // Check if the material is Cloud Remap. If it is, use CloudFix (thanks Kevin) instead of normal replacement.
                    if (material.shader.name.Contains("Cloud Remap"))
                    {
                        // Cloud fixer code adapted from Kevin's code he sent me. Necessary in order for Cloud Remap materials to not appear black.
                        // Cloudfix is used to get values generated at runtime and apply them to any material.
                        //if (debug) Debug.Log("ShaderConverter: Message from Kevin: Fuck you Cloud Remap.");
                        var cloudFixer = new CloudFix(material);
                        material.CopyPropertiesFromMaterial(ShaderConverterPlugin.cloudMat);
                        cloudFixer.SetMaterialValues(ref material);
                        materialAssets[i] = material;
                        if (debug) Debug.Log("ShaderConverter: " + material.name + " is a Cloud Remap shader. CloudFix applied!");
                    }
                }
                else if (material.shader.name.Contains("Cloud Remap")) { if (debug) Debug.Log("ShaderConverter: " + material.name + " is a Cloud Remap shader, but CloudFix is not enabled! Issues may occur!"); }
                //Apply the temporary variable copy to the original
                materialAssets[i] = material;
                if (debug) Debug.Log("ShaderConverter: Finished converting " + inAssetBundle + "'s shaders with CloudFix");
            }
        }
    }

    internal class CloudFix
    {
        // Thank you to Kevin (Kevin from HP Customer Service#0569) and KomradeSpectre#8468 from discord for this!
        // This was originally made to be used with RunTimeInspector, but in our case we use this to store/save material information and reapply it where we need.
        public string[] shaderKeywords;
        private static string[] keywordStrings = new string[]
        {
            "DISABLEREMAP",
            "USE_UV1",
            "FADECLOSE",
            "USE_CLOUDS",
            "CLOUDOFFSET",
            "VERTEXCOLOR",
            "VERTEXALPHA",
            "CALCTEXTUREALPHA",
            "VERTEXOFFSET",
            "FRESNEL",
            "SKYBOX_ONLY"
        };

        public Color _Tint = Color.white;
        public Texture _MainTex;
        public Vector2 _MainTexScale = Vector2.one;
        public Vector2 _MainTexOffset = Vector2.zero;
        public Texture _RemapTex;
        public Vector2 _RemapTexScale = Vector2.one;
        public Vector2 _RemapTexOffset = Vector2.zero;

        [Range(0f, 2f)]
        public float _InvFade = 0.1f;

        [Range(1f, 20f)]
        public float _BrightnessBoost = 1f;

        [Range(0f, 20f)]
        public float _AlphaBoost = 1f;

        [Range(0f, 1f)]
        public float _AlphaBias = 0;

        [Range(0f, 1f)]
        public float _FadeCloseDistance = 0.5f;

        public enum _CullEnum
        {
            Off,
            Front,
            Back
        }
        public _CullEnum _Cull_Mode;

        public enum _ZTestEnum
        {
            Disabled,
            Never,
            Less,
            Equal,
            LessEqual,
            Greater,
            NotEqual,
            GreaterEqual,
            Always
        }
        public _ZTestEnum _ZTest_Mode = _ZTestEnum.LessEqual;

        [Range(-10f, 10f)]
        public float _DepthOffset;


        [Range(-2f, 2f)]
        public float _DistortionStrength = 0.1f;

        public Texture _Cloud1Tex;
        public Vector2 _Cloud1TexScale;
        public Vector2 _Cloud1TexOffset;
        public Texture _Cloud2Tex;
        public Vector2 _Cloud2TexScale;
        public Vector2 _Cloud2TexOffset;
        public Vector4 _CutoffScroll;

        [Range(-20f, 20f)]
        public float _FresnelPower;

        [Range(0f, 3f)]
        public float _VertexOffsetAmount;

        public CloudFix(Material material)
        {
            GetMaterialValues(material);
        }

        public void GetMaterialValues(Material material)
        {
            shaderKeywords = material.shaderKeywords;
            _Tint = material.GetColor("_TintColor");
            _MainTex = material.GetTexture("_MainTex");
            _MainTexScale = material.GetTextureScale("_MainTex");
            _MainTexOffset = material.GetTextureOffset("_MainTex");
            _RemapTex = material.GetTexture("_RemapTex");
            _RemapTexScale = material.GetTextureScale("_RemapTex");
            _RemapTexOffset = material.GetTextureOffset("_RemapTex");
            _InvFade = material.GetFloat("_InvFade");
            _BrightnessBoost = material.GetFloat("_Boost");
            _AlphaBoost = material.GetFloat("_AlphaBoost");
            _AlphaBias = material.GetFloat("_AlphaBias");
            _FadeCloseDistance = material.GetFloat("_FadeCloseDistance");
            _Cull_Mode = (_CullEnum)(int)material.GetFloat("_Cull");
            _ZTest_Mode = (_ZTestEnum)(int)material.GetFloat("_ZTest");
            _DepthOffset = material.GetFloat("_DepthOffset");
            _DistortionStrength = material.GetFloat("_DistortionStrength");
            _Cloud1Tex = material.GetTexture("_Cloud1Tex");
            _Cloud1TexScale = material.GetTextureScale("_Cloud1Tex");
            _Cloud1TexOffset = material.GetTextureOffset("_Cloud1Tex");
            _Cloud2Tex = material.GetTexture("_Cloud2Tex");
            _Cloud2TexScale = material.GetTextureScale("_Cloud2Tex");
            _Cloud2TexOffset = material.GetTextureOffset("_Cloud2Tex");
            _CutoffScroll = material.GetVector("_CutoffScroll");
            _FresnelPower = material.GetFloat("_FresnelPower");
            _VertexOffsetAmount = material.GetFloat("_OffsetAmount");
        }

        public void SetMaterialValues(ref Material material)
        {
            foreach (var keyword in keywordStrings)
                if (material.IsKeywordEnabled(keyword))
                    material.DisableKeyword(keyword);
            foreach (var keyword in shaderKeywords)
                material.EnableKeyword(keyword);

            material.SetColor("_TintColor", _Tint);

            if (_MainTex)
            {
                material.SetTexture("_MainTex", _MainTex);
                material.SetTextureScale("_MainTex", _MainTexScale);
                material.SetTextureOffset("_MainTex", _MainTexOffset);
            }
            else
            {
                material.SetTexture("_MainTex", null);
            }

            if (_RemapTex)
            {
                material.SetTexture("_RemapTex", _RemapTex);
                material.SetTextureScale("_RemapTex", _RemapTexScale);
                material.SetTextureOffset("_RemapTex", _RemapTexOffset);
            }
            else
            {
                material.SetTexture("_RemapTex", null);
            }

            material.SetFloat("_InvFade", _InvFade);
            material.SetFloat("_Boost", _BrightnessBoost);
            material.SetFloat("_AlphaBoost", _AlphaBoost);
            material.SetFloat("_AlphaBias", _AlphaBias);
            material.SetFloat("_FadeCloseDistance", _FadeCloseDistance);
            material.SetFloat("_Cull", Convert.ToSingle(_Cull_Mode));
            material.SetFloat("_ZTest", Convert.ToSingle(_ZTest_Mode));
            material.SetFloat("_DepthOffset", _DepthOffset);
            material.SetFloat("_DistortionStrength", _DistortionStrength);

            if (_Cloud1Tex)
            {
                material.SetTexture("_Cloud1Tex", _Cloud1Tex);
                material.SetTextureScale("_Cloud1Tex", _Cloud1TexScale);
                material.SetTextureOffset("_Cloud1Tex", _Cloud1TexOffset);
            }
            else
            {
                material.SetTexture("_Cloud1Tex", null);
            }

            if (_Cloud2Tex)
            {
                material.SetTexture("_Cloud2Tex", _Cloud2Tex);
                material.SetTextureScale("_Cloud2Tex", _Cloud2TexScale);
                material.SetTextureOffset("_Cloud2Tex", _Cloud2TexOffset);
            }
            else
            {
                material.SetTexture("_Cloud2Tex", null);
            }

            material.SetVector("_CutoffScroll", _CutoffScroll);
            material.SetFloat("_FresnelPower", _FresnelPower);
            material.SetFloat("_OffsetAmount", _VertexOffsetAmount);


        }
    }
}
