using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace DarkPixelGD.BehaviourTree
{
    public abstract class BTNode : ScriptableObject
    {
        public string guid;
        public Vector2 position;
        [System.NonSerialized]
        public BTNodeRuntime runtime;

        public abstract BTNodeRuntime CreateRuntime(Blackboard blackboard, GameObject owner);

        protected virtual void OnEnable()
        {
            if (this is CompositeNode composite)
            {
                if (composite.children == null)
                    composite.children = new List<BTNode>();
            }
        }

        public virtual VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.Add(new Label(GetType().Name)
            {
                style = { unityFontStyleAndWeight = FontStyle.Bold }
            });

            return container;
        }


    }
}