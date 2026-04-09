DarkPixel Behavior Tree Framework - Official Documentation
Table of Contents
Introduction

Quick Start & Setup Guide
2.1. Creating the AI Assets
2.2. Setting Up the Scene Agent
2.3. Using the Visual Editor

The Blackboard System
3.1. Blackboard Asset vs. Runtime
3.2. Injecting Variables via Code

Script & API Reference
4.1. Core Components
4.2. Runtime Logic

Creating Custom Nodes
5.1. Writing the Runtime Script
5.2. Writing the Node Asset & UI
5.3. Adding the Node to the Search Menu

Live Debugging & Tools

1. Introduction
Welcome to the DarkPixel Behavior Tree Framework. This package provides a robust, visual, and highly performant node-based Artificial Intelligence system for Unity. It utilizes a strict Asset-to-Runtime cloning architecture to ensure multiple agents can share the same logic trees without overwriting shared memory, making it safe and scalable for any project size.

2. Quick Start & Setup Guide
This step-by-step tutorial will guide you through setting up your first intelligent agent in a scene.

2.1. Creating the AI Assets
Before an AI can act, it needs a brain (Behavior Tree) and a memory (Blackboard).

Right-click in your Project window.

Navigate to Create > BT > Behavior Tree and name it Demo_Tree.

Right-click again and navigate to Create > BT > Blackboard and name it Demo_Blackboard.

Select Demo_Blackboard in the Inspector and add a new Key (e.g., a Float named "Speed" set to 3.0).

2.2. Setting Up the Scene Agent
Create a 3D character (e.g., a Capsule) in your Unity Scene.

Select the character and click Add Component in the Inspector.

Add the BehaviorTreeAgent script to the character.

Drag your Demo_Tree asset into the Tree Asset slot.

Drag your Demo_Blackboard asset into the Blackboard Asset slot.

2.3. Using the Visual Editor
From the top Unity menu bar, select Tools > BT > Behavior Tree Editor.

Select your Demo_Tree asset in the Project window to load it into the graph.

Press Spacebar or right-click inside the grid to open the node search window.

Add a Selector node, connect it to the Root, and add a Debug Log node as a child of the Selector.

Press Play in the Unity Editor. Your character will now execute the tree logic automatically.

3. The Blackboard System
The Blackboard is a shared memory dictionary that allows different nodes to communicate (e.g., a "Check Target" node saving a player reference for a "Move To" node).

3.1. Blackboard Asset vs. Runtime
Blackboard Asset: A ScriptableObject where you define default, starting variables in the editor.

Runtime Blackboard: When the game starts, the BehaviorTreeAgent clones the Asset into a live memory dictionary. This ensures that if 10 enemies use the same Blackboard Asset, they each get their own unique memory at runtime.

3.2. Injecting Variables via Code
You will often need to pass dynamic scene data (like the Player's Transform) into the Behavior Tree. To do this, create a new C# script that inherits from BehaviorTreeAgent and override the setup method.

C#
using UnityEngine;
using DarkPixelGD.BehaviourTree;

public class CustomEnemyAI : BehaviorTreeAgent
{
    public Transform playerTarget;

    protected override void SetupBlackboard(Blackboard bb)
    {
        // Inject the player reference into the AI's memory before the tree starts
        bb.Set("player", playerTarget);
    }
}
4. Script & API Reference
All scripts are safely contained within the DarkPixelGD.BehaviourTree namespace to prevent conflicts with your project.

4.1. Core Components
BehaviorTreeAgent: The primary MonoBehaviour attached to GameObjects. It handles cloning the tree, initializing the Blackboard, and ticking the root node every frame.

BehaviorTree: The ScriptableObject that stores the visual graph data and node connections.

BlackboardAsset: The ScriptableObject storing default variable configurations.

BTNode: The base ScriptableObject for all nodes. It handles the data saving and custom Inspector UI drawing.

4.2. Runtime Logic
BTNodeRuntime: The standard C# class that executes logic during Play Mode. It returns a BTStatus.

BTStatus: An enum representing the state of a node (Success, Failure, Running).

Blackboard: The runtime dictionary. Contains methods like Set<T>(string key, T value) and Get<T>(string key).

5. Creating Custom Nodes
The framework is highly extensible. To create a new behavior, you must create a Runtime script (for logic) and a Node script (for data/UI).

5.1. Writing the Runtime Script
Create a script that inherits from BTNodeRuntime and override the OnUpdate method.

C#
using UnityEngine;
using DarkPixelGD.BehaviourTree;

public class CustomHealRuntime : BTNodeRuntime
{
    private int healAmount;

    public CustomHealRuntime(int amount, Blackboard bb, GameObject owner) : base(bb, owner)
    {
        healAmount = amount;
    }

    protected override BTStatus OnUpdate()
    {
        Debug.Log($"{owner.name} healed for {healAmount}!");
        return BTStatus.Success; 
    }
}
5.2. Writing the Node Asset & UI
Create a script that inherits from BTNode (or ActionNode, DecoratorNode).

C#
using UnityEngine;
using UnityEngine.UIElements;
using DarkPixelGD.BehaviourTree;

public class CustomHealNode : BTNode
{
    public int amountToHeal = 10;

    public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
    {
        return new CustomHealRuntime(amountToHeal, bb, owner);
    }

    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();
        // UI Toolkit logic to draw the inspector fields goes here
        return container;
    }
}
5.3. Adding the Node to the Search Menu
To make your custom node appear in the visual graph:

Open BTSearchWindow.cs.

Locate the CreateSearchTree method.

Add a new SearchTreeEntry passing your node's type into the userData field.

6. Live Debugging & Tools
The framework includes dedicated tools to help you monitor AI behavior during Play Mode.

Live Graph Execution: While the game is running, open the Behavior Tree Editor and click on any Agent in the scene Hierarchy. The graph will display live execution states, highlighting active nodes in yellow (Running) or green (Success).

Blackboard Debug Window: Navigate to Tools > BT > Blackboard Debug. During Play Mode, click on an Agent in the scene to view its live memory dictionary. This window updates variables (like cooldown floats or bools) in real-time as the tree executes.