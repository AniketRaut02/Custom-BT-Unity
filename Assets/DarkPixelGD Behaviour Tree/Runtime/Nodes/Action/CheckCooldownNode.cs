using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace DarkPixelGD.BehaviourTree
{


    public class CheckCooldownNode : BTNode
    {
        public string cooldownKey = "MeleeCooldown";

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new CheckCooldownRuntime(cooldownKey, bb, owner);
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

            container.Add(keyField);
            return container;
        }
    }

    public class CheckCooldownRuntime : BTNodeRuntime
    {
        private string cooldownKey;

        public CheckCooldownRuntime(string key, Blackboard bb, GameObject owner) : base(bb, owner)
        {
            cooldownKey = key;
        }

        protected override BTStatus OnUpdate()
        {
            // Defaults to 0 if not set yet, so the first attack is always ready
            float nextAllowedTime = blackboard.Get<float>(cooldownKey);

            if (Time.time >= nextAllowedTime)
                return BTStatus.Success;

            return BTStatus.Failure;
        }
    }
}