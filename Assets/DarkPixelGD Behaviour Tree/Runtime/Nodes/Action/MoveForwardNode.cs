using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


namespace DarkPixelGD.BehaviourTree
{


   
    public class MoveForwardNode : BTNode
    {
        public float speed = 2f;

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new MoveForwardRuntime(speed, bb, owner);
        }
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;
            container.style.marginBottom = 8;

            var speedField = new FloatField("Speed");
            speedField.value = speed;
            speedField.RegisterValueChangedCallback(evt =>
            {
                speed = evt.newValue;
                EditorUtility.SetDirty(this);
            });

            container.Add(speedField);

            return container;
        }
    }

    public class MoveForwardRuntime : BTNodeRuntime
    {
        private float speed;

        public MoveForwardRuntime(float speed, Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
            this.speed = speed;
        }

        protected override BTStatus OnUpdate()
        {
            owner.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return BTStatus.Running;
        }
    }
}