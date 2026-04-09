#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{


    [CustomEditor(typeof(BlackboardAsset))]
    public class BlackboardAssetEditor : Editor
    {
        private void OnEnable()
        {
            var asset = (BlackboardAsset)target;

            BTIconUtility.SetIcon(asset, "blackboard");
        }
    }
}
#endif