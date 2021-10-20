using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StubbedConverter
{
    public static class ShaderConverter
    {

        #region newPublic
        public static void ConvertStubbedShaders(AssetBundle inAssetBundle, bool debug = false)
        {
            if (debug) { Debug.Log("StubbedConverter: Attempting to convert shaders of AssetBundle: " + inAssetBundle + "."); }

            Material[] bundleMaterials = inAssetBundle.LoadAllAssets<Material>();
            ConvertMaterialShaders(debug, bundleMaterials);
        }

        public static void ConvertStubbedShaders(GameObject inGameObject, bool debug = false)
        {
            if (debug) { Debug.Log("StubbedConverter: Attempting to convert shaders of GameObject: " + inGameObject + "."); }

            Material[] materials = GetGameObjectMaterials(debug, inGameObject);
            ConvertMaterialShaders(debug, materials);
        }

        public static void ConvertStubbedShaders(GameObject[] inGameObjects, bool debug = false)
        {
            if (debug) { Debug.Log("StubbedConverter: Attempting to convert shaders of: " + inGameObjects.Length + " GameObjects."); }

            Material[] materials = GetGameObjectMaterials(debug, inGameObjects);
            ConvertMaterialShaders(debug, materials);
        }

        public static void ConvertStubbedShaders(Material inMaterial, bool debug = false)
        {
            if (debug) { Debug.Log("StubbedConverter: Attempting to convert shader of Material: " + inMaterial + "."); }

            ConvertMaterialShaders(debug, inMaterial);
        }

        public static void ConvertStubbedShaders(Material[] inMaterials, bool debug = false)
        {
            if (debug) { Debug.Log("StubbedConverter: Attempting to convert shaders of: " + inMaterials.Length + " Materials."); }

            ConvertMaterialShaders(debug, inMaterials);
        }
        #endregion

        #region newInternal
        internal static Material[] GetGameObjectMaterials(bool debug = false, params GameObject[] inObjects)
        {
            Renderer[] objectRenderers = GetGameObjectRenderers(debug, inObjects);

            return GetRendererMaterials(debug, objectRenderers);
        }

        internal static Material[] GetRendererMaterials(bool debug = false, params Renderer[] inRenderers)
        {
            List<Material> matList = new List<Material>();

            foreach (Renderer renderer in inRenderers)
            {
                matList.AddRange(renderer.materials);
            }

            return matList.ToArray();
        }

        internal static Renderer[] GetGameObjectRenderers(bool debug = false, params GameObject[] inObjects)
        {
            List<Renderer> objectRenderers = new List<Renderer>();

            foreach (GameObject gameObject in inObjects)
            {
                objectRenderers.AddRange(gameObject.GetComponents<Renderer>());
                objectRenderers.AddRange(gameObject.GetComponentsInChildren<Renderer>());
            }

            return objectRenderers.ToArray();
        }

        internal static void ConvertMaterialShaders(bool debug = false, params Material[] inMaterials)
        {
            int i = 0;
            foreach (Material material in inMaterials)
            {
                if (MaterialPassesFilter(material, i, debug))
                {
                    var replacementShader = Resources.Load<Shader>(ShaderConverterPlugin.stubbedShaderLookup[material.shader.name]);
                    if (replacementShader)
                    {
                        material.shader = replacementShader;
                        if (debug) Debug.Log("StubbedConverter: Matching stubbed shader of " + material.name + " to: " + replacementShader + ". Replacing!");
                        if (material.shader.name.ToLower().Contains("Cloud Remap"))
                        {
                            if (material.GetFloat("_SrcBlend") == (float)UnityEngine.Rendering.BlendMode.Zero) material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                            if (material.GetFloat("_DstBlend") == (float)UnityEngine.Rendering.BlendMode.Zero) material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
                        }
                    }
                }
                i++;
            }
        }

        internal static bool MaterialPassesFilter(Material material, int iteration, bool debug)
        {
            if (!material) { 
                if (debug) Debug.Log("StubbedConverter: ERROR: Material (#" + iteration +") is missing. Wack. Skipping!"); 
                return false; }
            if (!material.shader) { 
                if (debug) Debug.Log("StubbedConverter: ERROR: Material (#" + iteration + ") " + material.name + "'s shader is missing. Skipping!"); 
                return false; }
            if (material.shader.name.ToLower().StartsWith("hopoo")) { 
                if (debug) Debug.Log("StubbedConverter: WARNING: Material (#" + iteration + ") " + material.name + "'s shader " + material.shader + " is ALREADY a Hopoo shader. Skipping"); 
                return false; }
            if (!material.shader.name.ToLower().StartsWith("stubbed")) { 
                if (debug) Debug.Log("StubbedConverter: WARNING: Material (#" + iteration + ") " + material.name + "'s shader " + material.shader + " is NOT a stubbed Hopoo shader. Skipping."); 
                return false; }

            return true;
        }
        #endregion
    }
}
