using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


namespace DarkPixelGD.BehaviourTree
{


    public class MoveToNode : BTNode
    {
        public string targetKey = "player";
        public float speed = 3f;
        public float stoppingDistance = 2f; // Should be slightly less than your CheckDistance threshold

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new MoveToRuntime(targetKey, speed, stoppingDistance, bb, owner);
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

            var speedField = new FloatField("Move Speed");
            speedField.value = speed;
            speedField.RegisterValueChangedCallback(evt => { speed = evt.newValue; EditorUtility.SetDirty(this); });

            var stopField = new FloatField("Stopping Distance");
            stopField.value = stoppingDistance;
            stopField.RegisterValueChangedCallback(evt => { stoppingDistance = evt.newValue; EditorUtility.SetDirty(this); });

            container.Add(targetField);
            container.Add(speedField);
            container.Add(stopField);

            return container;
        }
    }

    public class MoveToRuntime : BTNodeRuntime
    {
        private string targetKey;
        private float speed;
        private float stoppingDistance;

        public MoveToRuntime(string tKey, float speed, float stopDist, Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
            targetKey = tKey;
            this.speed = speed;
            stoppingDistance = stopDist;
        }

        protected override BTStatus OnUpdate()
        {
            var target = blackboard.Get<Transform>(targetKey);

            if (target == null)
                return BTStatus.Failure;

            float distance = Vector3.Distance(owner.transform.position, target.position);

            if (distance <= stoppingDistance)
                return BTStatus.Success;

            // Move towards the target, keeping the Y position the same to avoid floating/sinking
            Vector3 targetPos = new Vector3(target.position.x, owner.transform.position.y, target.position.z);
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, targetPos, speed * Time.deltaTime);

            // Also face the target while moving
            Vector3 direction = (targetPos - owner.transform.position).normalized;
            if (direction != Vector3.zero)
            {
                owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
            }

            return BTStatus.Running;
        }
    }
}