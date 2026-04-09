using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace DarkPixelGD.BehaviourTree
{


    public class BTConnectionListener : IEdgeConnectorListener
    {
        private BTGraphView graphView;

        public BTConnectionListener(BTGraphView graphView)
        {
            this.graphView = graphView;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // Do nothing
        }

        public void OnDrop(GraphView graphViewRef, Edge edge)
        {
            //  This is called when connection is completed
            // graphView.OnEdgeCreated(edge);
        }
    }
}