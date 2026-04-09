#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace DarkPixelGD.BehaviourTree
{


    public static class BTIconUtility
    {
        public static Texture2D LoadIcon(string name)
        {
            string path = $"Assets/DarkPixelGD Behaviour Tree/Editor/Icons/{name}.png";

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            return icon;
        }

        public static void SetIcon(Object obj, string iconName)
        {
            var icon = LoadIcon(iconName);
            if (icon != null)
            {
                EditorGUIUtility.SetIconForObject(obj, icon);
            }
        }
    }
}
#endif