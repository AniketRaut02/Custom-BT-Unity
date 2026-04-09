using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DarkPixelGD.BehaviourTree // Using your proper namespace!
{
    public class BlackboardDebugWindow : EditorWindow
    {
        private Blackboard blackboard;

        [MenuItem("Tools/BT/Blackboard Debug")]
        public static void ShowWindow()
        {
            GetWindow<BlackboardDebugWindow>("Blackboard Debug");
        }

        private void OnEnable()
        {
            EditorApplication.update += Repaint; // Live refresh for variables
            Selection.selectionChanged += Repaint; // Instantly refresh when clicking a new object
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
            Selection.selectionChanged -= Repaint;
        }

        private void OnGUI()
        {
            GUILayout.Label("Blackboard Debug", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 1. Ensure we are in Play Mode
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play Mode required. Enter Play Mode and select an Agent to view its Blackboard.", MessageType.Info);
                return;
            }

            // 2. Grab whatever the user currently has clicked on in the Hierarchy
            GameObject selectedGO = Selection.activeGameObject;

            if (selectedGO == null)
            {
                EditorGUILayout.HelpBox("Select an AI Agent in the Hierarchy to inspect its brain.", MessageType.Warning);
                return;
            }

            // 3. Try to find the Agent component on the selected object
            BehaviorTreeAgent agent = selectedGO.GetComponent<BehaviorTreeAgent>();

            if (agent == null)
            {
                EditorGUILayout.HelpBox($"The selected object '{selectedGO.name}' does not have a BehaviorTreeAgent attached.", MessageType.Warning);
                return;
            }

            // 4. Grab that specific agent's runtime clone blackboard
            blackboard = agent.Blackboard;

            if (blackboard == null)
            {
                EditorGUILayout.HelpBox($"Agent on '{selectedGO.name}' has no active Blackboard.", MessageType.Info);
                return;
            }

            // 5. Draw the UI
            EditorGUILayout.LabelField($"Target: {selectedGO.name}", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawBlackboard(blackboard);
        }

        private void DrawBlackboard(Blackboard bb)
        {
            EditorGUILayout.BeginVertical("box");

            foreach (var kvp in bb.GetAll())
            {
                EditorGUILayout.BeginHorizontal();

                // Draw the Key
                EditorGUILayout.LabelField(kvp.Key, GUILayout.Width(150));

                // Draw the Value
                if (kvp.Value is bool b)
                {
                    GUI.color = b ? Color.green : Color.red;
                    EditorGUILayout.LabelField(b.ToString());
                    GUI.color = Color.white;
                }
                else if (kvp.Value is float f)
                {
                    EditorGUILayout.LabelField(f.ToString("F3")); // Clean float formatting
                }
                else if (kvp.Value is UnityEngine.Object obj)
                {
                    // Draw a nice un-editable object field for Transforms/GameObjects
                    EditorGUILayout.ObjectField(obj, typeof(UnityEngine.Object), true);
                }
                else
                {
                    EditorGUILayout.LabelField(kvp.Value?.ToString() ?? "NULL");
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}