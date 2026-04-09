using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


namespace DarkPixelGD.BehaviourTree
{


    public class SetCooldownNode : BTNode
    {
        public string cooldownKey = "MeleeCooldown";
        public float duration = 3f; // Seconds before they can attack again

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new SetCooldownRuntime(cooldownKey, duration, bb, owner);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;
            container.style.marginBottom = 8;

            var keyField = new TextField("Cooldown Key");
            keyField.value = cooldownKey;
            keyField.RegisterValueChangedCallback(evt => { cooldownKey = evt.newValue; EditorUtility.SetDirty(this); });

            var durationField = new FloatField("Duration");
            durationField.value = duration;
            durationField.RegisterValueChangedCallback(evt => { duration = evt.newValue; EditorUtility.SetDirty(this); });

            container.Add(keyField);
            container.Add(durationField);
            return container;
        }
    }

    public class SetCooldownRuntime : BTNodeRuntime
    {
        private string cooldownKey;
        private float duration;

        public SetCooldownRuntime(string key, float duration, Blackboard bb, GameObject owner) : base(bb, owner)
        {
            cooldownKey = key;
            this.duration = duration;
        }

        protected override BTStatus OnUpdate()
        {
            // Record the time when this attack will be available next
            blackboard.Set(cooldownKey, Time.time + duration);
            return BTStatus.Success;
        }
    }
}