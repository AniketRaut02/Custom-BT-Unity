using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{


    public abstract class BTNodeRuntime
    {
        protected Blackboard blackboard;
        protected GameObject owner;

        private bool started = false;
        public BTStatus lastStatus;
        public bool executedThisFrame = false;
        public BTNodeRuntime parent;

        public enum BTVisualState
        {
            Inactive,
            Running,
            Success,
            Failure
        }
        public BTVisualState visualState;

        public bool isRunning;
        public BTNodeRuntime(Blackboard blackboard, GameObject owner)
        {
            this.blackboard = blackboard;
            this.owner = owner;
        }

        public BTStatus Tick()
        {
            executedThisFrame = true;

            if (!started)
            {
                OnStart();
                started = true;
            }

            var status = OnUpdate();

            lastStatus = status;
            // Set visual state
            switch (status)
            {
                case BTStatus.Running:
                    visualState = BTVisualState.Running;
                    break;

                case BTStatus.Success:
                    visualState = BTVisualState.Success;
                    break;

                case BTStatus.Failure:
                    visualState = BTVisualState.Failure;
                    break;
            }

            if (status != BTStatus.Running)
            {
                OnStop();
                started = false;
            }
            return status;
        }

        protected virtual void OnStart() { }
        protected abstract BTStatus OnUpdate();
        protected virtual void OnStop() { }

        public void ResetNode()
        {
            started = false;
            visualState = BTVisualState.Inactive;
            OnStop(); // ensure cleanup
        }
        public virtual void ForceStop()
        {
            if (started)
            {
                OnStop();
                started = false;
            }

            lastStatus = BTStatus.Failure;
        }

    }
}
