using System.Collections.Generic;
using UnityEngine;

namespace StubbedConverter
{
    public static class MaterialController
    {
        // An tool to add KomradeSpectre's Material Controller Component to any object's renderers.
        // Make use of RuntimeInspector to inspect a gameobject in question after adding the compoennt.
        // inspecting the details of the Material Controller will allow control of any material in game while playing.

        /// <summary>
        /// Adds a Material Controller (by KomradeSpectre) to all inObject's Renderers. Enables control of materials in game through RuntimeInspector.
        /// </summary>
        public static void AddMaterialController(GameObject inObject, bool debug = false)
        {
            List<Renderer> objectRenderers = new List<Renderer>();
            objectRenderers.AddRange(inObject.GetComponents<Renderer>());
            objectRenderers.AddRange(inObject.GetComponentsInChildren<Renderer>());
            if (debug) Debug.Log("RuntimeMaterialController: Found " + objectRenderers.Count + " renderers within GameObject: " + inObject);
            foreach (Renderer r in objectRenderers.ToArray())
            {
                AddMaterialControllerRenderer(r, debug);
            }
        }
        internal static void AddMaterialControllerRenderer(Renderer inRenderer, bool debug = false)
        {
            var finder = inRenderer.gameObject.AddComponent<Utils.MaterialControllerComponents.HGControllerFinder>();
            finder.Renderer = inRenderer;
            if (debug) Debug.Log("RuntimeMaterialController: Added RuntimeMaterialController to GameObject: " + inRenderer.gameObject + " for Renderer:" + inRenderer);
        }
    }
}
