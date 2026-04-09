using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
namespace DarkPixelGD.BehaviourTree
{


    public class FaceTargetNode : BTNode
    {
        public string targetKey = "player";
        public float turnSpeed = 5f;
        public float angleTolerance = 5f; // Stops turning when within this many degrees

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new FaceTargetRuntime(targetKey, turnSpeed, angleTolerance, bb, owner);
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

            var speedField = new FloatField("Turn Speed");
            speedField.value = turnSpeed;
            speedField.RegisterValueChangedCallback(evt => { turnSpeed = evt.newValue; EditorUtility.SetDirty(this); });

            var angleField = new FloatField("Angle Tolerance");
            angleField.value = angleTolerance;
            angleField.RegisterValueChangedCallback(evt => { angleTolerance = evt.newValue; EditorUtility.SetDirty(this); });

            container.Add(targetField);
            container.Add(speedField);
            container.Add(angleField);

            return container;
        }
    }

    public class FaceTargetRuntime : BTNodeRuntime
    {
        private string targetKey;
        private float turnSpeed;
        private float angleTolerance;

        public FaceTargetRuntime(string tKey, float speed, float tolerance, Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
            targetKey = tKey;
            turnSpeed = speed;
            angleTolerance = tolerance;
        }

        protected override BTStatus OnUpdate()
        {
            var target = blackboard.Get<Transform>(targetKey);

            if (target == null)
                return BTStatus.Failure;

            // Calculate direction, but flatten the Y axis so the enemy stays upright
            Vector3 directionToTarget = (target.position - owner.transform.position).normalized;
            directionToTarget.y = 0;

            // Edge case: if we are exactly on top of the target
            if (directionToTarget == Vector3.zero)
                return BTStatus.Success;

            // Smoothly rotate towards the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Check how close we are to looking directly at the target
            float angle = Vector3.Angle(owner.transform.forward, directionToTarget);

            if (angle <= angleTolerance)
                return BTStatus.Success; // Finished turning

            return BTStatus.Running; // Still turning
        }
    }
}