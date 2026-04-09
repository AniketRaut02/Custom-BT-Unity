using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace DarkPixelGD.BehaviourTree
{


    public class CheckDistanceNode : BTNode
    {
        public string targetKey = "player";
        public float distanceThreshold = 3f;

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new CheckDistanceRuntime(targetKey, distanceThreshold, bb, owner);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;
            container.style.marginBottom = 8;

            var targetField = new TextField("Target Key");
            targetField.value = targetKey;
            targetField.RegisterValueChangedCallback(evt => { targetKey = evt.newValue; EditorUtility.SetDirty(this); });

            var distField = new FloatField("Distance Threshold");
            distField.value = distanceThreshold;
            distField.RegisterValueChangedCallback(evt => { distanceThreshold = evt.newValue; EditorUtility.SetDirty(this); });

            container.Add(targetField);
            container.Add(distField);

            return container;
        }
    }

    public class CheckDistanceRuntime : BTNodeRuntime
    {
        private string targetKey;
        private float distanceThreshold;

        public CheckDistanceRuntime(string tKey, float threshold, Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
            targetKey = tKey;
            distanceThreshold = threshold;
        }

        protected override BTStatus OnUpdate()
        {
            var target = blackboard.Get<Transform>(targetKey);

            if (target == null)
            {
                Debug.LogWarning($"[CheckDistance] No Transform found on blackboard for key: {targetKey}");
                return BTStatus.Failure;
            }

            float distance = Vector3.Distance(owner.transform.position, target.position);
            if (distance <= distanceThreshold)
                return BTStatus.Success;

            return BTStatus.Failure;
        }
    }
}