using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace DarkPixelGD.BehaviourTree
{
    public class GetNextWaypointNode : BTNode
    {
        [Tooltip("The Blackboard key storing the array of Transform waypoints.")]
        public string waypointsKey = "waypoints";

        [Tooltip("The Blackboard key tracking which waypoint we are currently on.")]
        public string indexKey = "index";

        [Tooltip("The Blackboard key where we will save the chosen waypoint so MoveTo can use it.")]
        public string targetKey = "target";

        public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
        {
            return new GetNextWaypointRuntime(waypointsKey, indexKey, targetKey, bb, owner);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            container.style.paddingLeft = 6;
            container.style.paddingTop = 4;
            container.style.marginBottom = 8;

            var waypointsField = new TextField("Waypoints Array Key") { value = waypointsKey };
            waypointsField.RegisterValueChangedCallback(evt => { waypointsKey = evt.newValue; EditorUtility.SetDirty(this); });

            var indexField = new TextField("Current Index Key") { value = indexKey };
            indexField.RegisterValueChangedCallback(evt => { indexKey = evt.newValue; EditorUtility.SetDirty(this); });

            var targetField = new TextField("Output Target Key") { value = targetKey };
            targetField.RegisterValueChangedCallback(evt => { targetKey = evt.newValue; EditorUtility.SetDirty(this); });

            container.Add(waypointsField);
            container.Add(indexField);
            container.Add(targetField);

            return container;
        }
    }

    public class GetNextWaypointRuntime : BTNodeRuntime
    {
        private string waypointsKey;
        private string indexKey;
        private string targetKey;

        public GetNextWaypointRuntime(string wKey, string iKey, string tKey, Blackboard bb, GameObject owner)
            : base(bb, owner)
        {
            waypointsKey = wKey;
            indexKey = iKey;
            targetKey = tKey;
        }

        protected override BTStatus OnUpdate()
        {
            // 1. Get the array of waypoints from the blackboard
            var waypoints = blackboard.Get<Transform[]>(waypointsKey);

            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogWarning($"[GetNextWaypoint] No Waypoints array found for key: {waypointsKey}");
                return BTStatus.Failure;
            }

            // 2. Get the current patrol index
            int index = blackboard.Get<int>(indexKey);

            // Safety check in case the array size changed at runtime
            if (index >= waypoints.Length || index < 0)
            {
                index = 0;
            }

            // 3. Grab the actual Transform for that index and save it as our new Target
            Transform target = waypoints[index];
            blackboard.Set(targetKey, target);

            // 4. Increment the index for next time, looping back to 0 if we hit the end
            index = (index + 1) % waypoints.Length;
            blackboard.Set(indexKey, index);

            return BTStatus.Success;
        }
    }
}