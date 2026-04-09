using UnityEngine;

namespace DarkPixelGD.BehaviourTree
{

    public class DemoEnemy : BehaviorTreeAgent
    {
        [Header("Demo References")]
        public Transform player;
        public Transform[] patrolWaypoints;
        float distanceToPlayer;

        protected override void SetupBlackboard(Blackboard bb)
        {
            distanceToPlayer = Vector3.Distance(this.transform.position, player.position);
            // Inject the references into the brain!
            bb.Set("player", player);
            bb.Set("waypoints", patrolWaypoints);
            bb.Set("index", 0); // Start at the first waypoint
            bb.Set("DistanceToPlayer", distanceToPlayer);
            // Ensure cooldown starts at 0 so it can attack immediately
            bb.Set("MeleeCooldown", 0f);
        }


    }
}
