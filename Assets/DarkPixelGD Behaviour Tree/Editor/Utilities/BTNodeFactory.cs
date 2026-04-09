// Editor/Utilities/BTNodeFactory.cs
using UnityEngine;


namespace DarkPixelGD.BehaviourTree
{


    public static class BTNodeFactory
    {
        public static BTNode CreateNode(string type)
        {
            switch (type)
            {
                case "Sequence":
                    return ScriptableObject.CreateInstance<SequenceNode>();

                case "Selector":
                    return ScriptableObject.CreateInstance<SelectorNode>();

                case "Wait":
                    return ScriptableObject.CreateInstance<WaitNode>();

                default:
                    Debug.LogError($"Unknown node type: {type}");
                    return null;
            }
        }
    }
}