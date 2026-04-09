#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BTAssetEditor : Editor
    {
        private void OnEnable()
        {
            var tree = (BehaviorTree)target;

            BTIconUtility.SetIcon(tree, "BT");
        }
    }
}
#endif