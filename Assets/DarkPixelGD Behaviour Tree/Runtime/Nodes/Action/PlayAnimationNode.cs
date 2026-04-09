using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements; // Required for EnumField



namespace DarkPixelGD.BehaviourTree
{


    public enum AnimParameterMode
    {
        Trigger,
        Bool,
        Float,
        Int,
        CrossFadeState
    }

 
    public class PlayAnimationNode : BTNode
    {
        public AnimParameterMode mode = AnimParameterMode.Trigger;

        [Tooltip("The name of the Trigger, Bool, Float, Int, or State Name")]
        public string parameterName = "Attack";

        // Variables for the different modes
        public bool boolValue = true;
        public float floatValue = 1f;
        public int intValue = 1;
        public float crossFadeDuration = 0.2f;

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            // We pass all values into the runtime so it knows exactly what to execute
            return new PlayAnimationRuntime(mode, parameterName, boolValue, floatValue, intValue, crossFadeDuration, bb, owner);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;
            container.style.marginBottom = 8;

            // 1. Create the fields
            var modeField = new EnumField("Mode", mode);
            var paramField = new TextField("Parameter / State Name") { value = parameterName };
            var boolField = new Toggle("Bool Value") { value = boolValue };
            var floatField = new FloatField("Float Value") { value = floatValue };
            var intField = new IntegerField("Int Value") { value = intValue };
            var crossFadeField = new FloatField("CrossFade Duration") { value = crossFadeDuration };

            // 2. Helper function to show/hide fields based on the selected mode
            void UpdateFieldVisibility(AnimParameterMode currentMode)
            {
                boolField.style.display = currentMode == AnimParameterMode.Bool ? DisplayStyle.Flex : DisplayStyle.None;
                floatField.style.display = currentMode == AnimParameterMode.Float ? DisplayStyle.Flex : DisplayStyle.None;
                intField.style.display = currentMode == AnimParameterMode.Int ? DisplayStyle.Flex : DisplayStyle.None;
                crossFadeField.style.display = currentMode == AnimParameterMode.CrossFadeState ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // 3. Register value changes to save data and update UI
            modeField.RegisterValueChangedCallback(evt =>
            {
                mode = (AnimParameterMode)evt.newValue;
                UpdateFieldVisibility(mode);
                EditorUtility.SetDirty(this);
            });

            paramField.RegisterValueChangedCallback(evt => { parameterName = evt.newValue; EditorUtility.SetDirty(this); });
            boolField.RegisterValueChangedCallback(evt => { boolValue = evt.newValue; EditorUtility.SetDirty(this); });
            floatField.RegisterValueChangedCallback(evt => { floatValue = evt.newValue; EditorUtility.SetDirty(this); });
            intField.RegisterValueChangedCallback(evt => { intValue = evt.newValue; EditorUtility.SetDirty(this); });
            crossFadeField.RegisterValueChangedCallback(evt => { crossFadeDuration = evt.newValue; EditorUtility.SetDirty(this); });

            // 4. Add them to the container
            container.Add(modeField);
            container.Add(paramField);
            container.Add(boolField);
            container.Add(floatField);
            container.Add(intField);
            container.Add(crossFadeField);

            // Run once to set initial visibility
            UpdateFieldVisibility(mode);

            return container;
        }
    }

    public class PlayAnimationRuntime : BTNodeRuntime
    {
        private AnimParameterMode mode;
        private string parameterName;
        private bool boolValue;
        private float floatValue;
        private int intValue;
        private float crossFadeDuration;

        private Animator animator;

        public PlayAnimationRuntime(
            AnimParameterMode mode, string param, bool bVal, float fVal, int iVal, float fade,
            Blackboard bb, GameObject owner) : base(bb, owner)
        {
            this.mode = mode;
            this.parameterName = param;
            this.boolValue = bVal;
            this.floatValue = fVal;
            this.intValue = iVal;
            this.crossFadeDuration = fade;

            animator = owner.GetComponentInChildren<Animator>();
        }

        protected override BTStatus OnUpdate()
        {
            if (animator == null)
            {
                Debug.LogWarning($"[PlayAnimation] No Animator found on {owner.name}!");
                return BTStatus.Failure;
            }

            // Execute the correct animator function based on the mode
            switch (mode)
            {
                case AnimParameterMode.Trigger:
                    animator.SetTrigger(parameterName);
                    break;

                case AnimParameterMode.Bool:
                    animator.SetBool(parameterName, boolValue);
                    break;

                case AnimParameterMode.Float:
                    animator.SetFloat(parameterName, floatValue);
                    break;

                case AnimParameterMode.Int:
                    animator.SetInteger(parameterName, intValue);
                    break;

                case AnimParameterMode.CrossFadeState:
                    // This ignores parameters entirely and forces the animator to blend directly into a state by name
                    animator.CrossFade(parameterName, crossFadeDuration);
                    break;
            }

            return BTStatus.Success;
        }
    }
}