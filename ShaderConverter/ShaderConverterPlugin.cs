using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StubbedConverter
{
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    internal class ShaderConverterPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.valex.ShaderConverter";
        public const string MODNAME = "ShaderConverter";
        public const string MODVERSION = "0.0.2";

        public static ShaderConverterPlugin instance;

        internal static Material cloudMat = null;

        // This Dictionary ties every stubbed version of a shader to the one that vanilla RoR2 uses. Yes, even the shaders that don't necessarily need this dictionary or stubbed conversion.
        // I've already written them all down. If it works when you use the stubbed version, then it works.
        /// <summary>
        /// Key = Stubbed Shader name. Value = Vanilla hopoo shader path.
        /// </summary>
        public readonly static Dictionary<string, string> stubbedShaderLookup = new Dictionary<string, string>()
        {
            //Shader internal name (Input shader)                               Path name (Output shader)
            {"stubbed_Hidden/FastBlur Proxy",                                   "shaders/mobileblur" },
            {"stubbed_TextMeshPro/Bitmap",                                      "shaders/tmp_bitmap"},
            {"stubbed_TextMeshPro/Bitmap Custom Atlas",                         "shaders/tmp_bitmap-custom-atlas"},
            {"stubbed_TextMeshPro/Mobile/Bitmap",                               "shaders/tmp_bitmap-mobile" },
            {"stubbed_TextMeshPro/Distance Field",                              "shaders/tmp_sdf"},
            {"stubbed_TextMeshPro/Distance Field Overlay",                      "shaders/tmp_sdf overlay"},
            {"stubbed_TextMeshPro/Mobile/Distance Field",                       "shaders/tmp_sdf-mobile"},
            {"stubbed_TextMeshPro/Mobile/Distance Field - Masking",             "shaders/tmp_sdf-mobile masking"},
            {"stubbed_TextMeshPro/Mobile/Distance Field Overlay",               "shaders/tmp_sdf-mobile overlay"},
            {"stubbed_TextMeshPro/Distance Field (Surface)",                    "shaders/tmp_sdf-surface"},
            {"stubbed_TextMeshPro/Mobile/Distance Field (Surface)",             "shaders/tmp_sdf-surface-mobile"},
            {"stubbed_TextMeshPro/Sprite",                                      "shaders/tmp_sprite"},
            {"stubbed_Hopoo Games/Deferred/Snow Topped Proxy",                  "shaders/deferred/hgsnowtopped"},
            {"stubbed_Hopoo Games/Deferred/Standard Proxy",                     "shaders/deferred/hgstandard"},
            {"stubbed_Hopoo Games/Deferred/Triplanar Terrain Blend Proxy",      "shaders/deferred/hgtriplanarterrainblend"},
            {"stubbed_Hopoo Games/Environment/Distant Water Proxy",             "shaders/environment/hgdistantwater"},
            {"stubbed_Hopoo Games/Environment/Waving Grass Proxy",              "shaders/environment/hggrass"},
            {"stubbed_Hopoo Games/Environment/Waterfall Proxy",                 "shaders/environment/hgwaterfall"},
            {"stubbed_Hopoo Games/FX/Cloud Remap Proxy",                        "shaders/fx/hgcloudremap"},
            {"stubbed_Hopoo Games/FX/Damage Number Proxy",                      "shaders/fx/hgdamagenumber"},
            {"stubbed_Hopoo Games/FX/Distortion Proxy",                         "shaders/fx/hgdistortion"},
            {"stubbed_Hopoo Games/FX/Forward Planet Proxy",                     "shaders/fx/hgforwardplanet"},
            {"stubbed_Hopoo Games/FX/Cloud Intersection Remap Proxy",           "shaders/fx/hgintersectioncloudremap"},
            {"stubbed_Hopoo Games/FX/Opaque Cloud Remap Proxy",                 "shaders/fx/hgopaquecloudremap"},
            {"stubbed_Hopoo Games/FX/Solid Parallax Proxy",                     "shaders/fx/hgsolidparallax"},
            {"stubbed_Hopoo Games/FX/Vertex Colors Only Proxy",                 "shaders/fx/hgvertexonly"},
            {"stubbed_Hopoo Games/Internal/Outline Highlight Proxy",            "shaders/postprocess/hgoutlinehighlight"},
            {"stubbed_Hopoo Games/Post Process/Scope Distortion Proxy",         "shaders/postprocess/hgscopeshader"},
            {"stubbed_Hopoo Games/Post Process/Screen Damage Proxy",            "shaders/postprocess/hgscreendamage"},
            {"stubbed_Hopoo Games/Internal/SobelBuffer Proxy",                  "shaders/postprocess/hgsobelbuffer"},
            {"stubbed_Hopoo Games/UI/Animate Alpha Proxy",                      "shaders/ui/hguianimatealpha"},
            {"stubbed_Hopoo Games/UI/UI Bar Remap Proxy",                       "shaders/ui/hguibarremap"},
            {"stubbed_Hopoo Games/UI/Masked UI Blur Proxy",                     "shaders/ui/hguiblur"},
            {"stubbed_Hopoo Games/UI/Custom Blend Proxy",                       "shaders/ui/hguicustomblend"},
            {"stubbed_Hopoo Games/UI/Debug Ignore Z Proxy",                     "shaders/ui/hguiignorez"},
            {"stubbed_Hopoo Games/UI/Default Overbrighten Proxy",               "shaders/ui/hguioverbrighten"},
            {"StubbedShader/fx/hgcloudremap",                                   "shaders/fx/hgcloudremap"}
        };

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            //cloudMat = ;
        }
    }
}
