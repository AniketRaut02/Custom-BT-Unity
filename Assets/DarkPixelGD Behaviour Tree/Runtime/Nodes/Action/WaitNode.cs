using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace DarkPixelGD.BehaviourTree
{


    public class WaitNode : BTNode
    {
        public float duration = 1f;

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new WaitRuntime(duration, bb, owner);
        }
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;

            container.Add(new Label("Wait Node")
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            });

            // Duration field
            var durationField = new FloatField("Duration (seconds)");
            durationField.value = duration;

            durationField.RegisterValueChangedCallback(evt =>
            {
                duration = evt.newValue;
                EditorUtility.SetDirty(this);
            });

            container.Add(durationField);

            return container;
        }
    }

    public class WaitRuntime : BTNodeRuntime
    {
        private float duration;
        private float timer;

        public WaitRuntime(float duration, Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
            this.duration = duration;
        }

        protected override void OnStart()
        {
            timer = 0f;
        }

        protected override BTStatus OnUpdate()
        {
            timer += Time.deltaTime;

            if (timer >= duration)
                return BTStatus.Success;

            return BTStatus.Running;
        }
    }
}