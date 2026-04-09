#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{


    public class NodeAssetEditor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);

                if (mainAsset is BehaviorTree tree)
                {
                    ApplyIcons(tree, path);
                }
            }
        }

        private static void ApplyIcons(BehaviorTree tree, string path)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);

            var icon = BTIconUtility.LoadIcon("node");

            if (icon == null)
            {
                Debug.LogError("BTNode icon missing!");
                return;
            }

            foreach (var asset in assets)
            {
                if (asset is BTNode node)
                {
                    EditorGUIUtility.SetIconForObject(node, icon);
                }
            }

            EditorApplication.delayCall += () =>
            {
                AssetDatabase.SaveAssets();
            };
        }
    }
}
#endif