using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{


    [AddComponentMenu("Behavior Tree/BT Agent")]
    public class BehaviorTreeAgent : MonoBehaviour
    {
        [Header("Brain Setup")]
        [Tooltip("The Behavior Tree asset to execute.")]
        public BehaviorTree treeAsset;

        [Tooltip("Optional: Default Blackboard values.")]
        public BlackboardAsset blackboardAsset;

        // We store the clone here so multiple agents don't share the exact same SO memory
        public BehaviorTree RuntimeTree { get; private set; }
        public Blackboard Blackboard { get; private set; }

        protected virtual void Start()
        {
            if (treeAsset == null)
            {
                Debug.LogWarning($"[BT Agent] No Behavior Tree assigned to {gameObject.name}!");
                return;
            }

            // 1. Clone the tree so this specific agent has its own instance
            RuntimeTree = Instantiate(treeAsset);
            RuntimeTree.name = $"{treeAsset.name} (Runtime)";

            // 2. Initialize the Blackboard
            Blackboard = new Blackboard();
            if (blackboardAsset != null)
            {
                Blackboard.Initialize(blackboardAsset);
            }

            // 3. User Setup Hook (Sets dynamic variables like target transforms)
            SetupBlackboard(Blackboard);

            // 4. Initialize the Tree
            RuntimeTree.Initialize(Blackboard, gameObject);
        }

        protected virtual void Update()
        {
            if (RuntimeTree != null && RuntimeTree.rootRuntime != null)
            {
                RuntimeTree.ResetExecutionState();
                RuntimeTree.rootRuntime.Tick();
            }
        }

        /// <summary>
        /// Override this method in your own custom script to inject dynamic scene references
        /// (like the Player, NavMeshAgent, or spawn points) into the blackboard before the tree starts.
        /// </summary>
        protected virtual void SetupBlackboard(Blackboard bb)
        {
            // Example: 
            // bb.Set("self", this.gameObject);
        }
    }
}