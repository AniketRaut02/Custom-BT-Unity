using UnityEngine;
using System.Collections.Generic;

namespace DarkPixelGD.BehaviourTree
{



    [CreateAssetMenu(menuName = "BT/Blackboard")]
    public class BlackboardAsset : ScriptableObject
    {
        public List<BlackboardKey> keys = new();
    }

    [System.Serializable]
    public class BlackboardKey
    {
        public string name;
        public BlackboardValueType type;

        public bool boolValue;
        public float floatValue;
        public int intValue;
    }

    public enum BlackboardValueType
    {
        Bool,
        Float,
        Int
    }
}