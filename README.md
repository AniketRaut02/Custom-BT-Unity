# DarkPixel Behavior Tree Framework

A professional, node-based Artificial Intelligence framework for Unity. Build, visualize, and debug complex character behaviors without writing boilerplate code. 

Built from the ground up using Unity's modern `Experimental.GraphView` and `UI Toolkit`, this framework provides a clean, AAA-style visual editor, a robust shared memory system (Blackboard), and live runtime debugging tools.

---

## 🌟 Key Features
* **Visual Node Editor:** A highly responsive GraphView editor with panning, zooming, comment groups, and right-click search menus.
* **Safe Runtime Architecture:** Uses a strict Asset-to-Runtime cloning system. 100 enemies can share the exact same Behavior Tree asset without overwriting each other's active memory or states.
* **Dynamic Blackboard:** A robust shared memory system. Define default variables in the editor, and inject dynamic scene data (like the Player Transform) via code at runtime.
* **Live Real-Time Debugging:** Watch the visual graph light up with execution states (Running, Success, Failure) during Play Mode, and inspect live memory values via the custom Blackboard Debugger.
* **Highly Extensible:** Creating custom Action or Condition nodes requires just a few lines of C#, and they automatically generate their own clean UI in the editor.

---

## 🚀 Getting Started

### 1. Creating the AI Assets
1. Right-click in your Project window.
2. Navigate to **Create > BT > Behavior Tree** and name it `EnemyTree`.
3. Right-click again and navigate to **Create > BT > Blackboard** and name it `EnemyBlackboard`.
4. Open the Blackboard asset and add any default variables you need (e.g., a Float named `MoveSpeed` set to `3.0`).

### 2. Setting Up an Agent
To make a GameObject use a Behavior Tree, attach the `BehaviorTreeAgent` script to it.

1. Add the **`BehaviorTreeAgent`** component to your character.
2. Drag your `EnemyTree` asset into the **Tree Asset** slot.
3. Drag your `EnemyBlackboard` asset into the **Blackboard Asset** slot.

### 3. Injecting Dynamic Data (The Blackboard)
Usually, your AI needs to know about scene-specific data, like the Player's transform. Create a small custom script inheriting from `BehaviorTreeAgent` to inject this data before the tree starts running.

```csharp
using UnityEngine;
using DarkPixelGD.BehaviourTree;

public class EnemyAI : BehaviorTreeAgent
{
    public Transform playerTarget;

    protected override void SetupBlackboard(Blackboard bb)
    {
        // This injects the player reference into the AI's memory at startup!
        bb.Set("player", playerTarget);
    }
}
```

---

## 🛠️ Editor Tools & Debugging

### The Behavior Tree Editor
To open the visual graph:

1. Navigate to **Tools > BT > Behavior Tree Editor**.
2. Select a Behavior Tree asset in your project folder to edit it.
3. Press **Spacebar** or **Right-click** inside the grid to add new nodes.
4. Hold **Shift** to select multiple nodes, then right-click → **Group Selected Nodes** to create a comment box.

---

### Live Graph Debugging
During Play Mode:

- Select an AI agent in the **Hierarchy**
- The editor switches to the **runtime clone automatically**
- Nodes highlight in real-time:
  - 🟡 **Yellow** → Running  
  - 🟢 **Green** → Success  
  - 🔴 **Red** → Failure  

---

### The Blackboard Debug Window
Never guess what your AI is thinking!

1. Navigate to **Tools > BT > Blackboard Debug**
2. Enter Play Mode
3. Select an AI agent in the scene

➡️ Instantly view all active Blackboard variables updating in real-time.

---

## 🧩 Creating Custom Nodes

The framework uses a **split architecture**:

- **BTNode (ScriptableObject):** Stores configuration + Editor UI  
- **BTNodeRuntime (C# Class):** Executes runtime logic  

---

### Example: Debug Log Node

#### 1. Node Code (DebugLogNode.cs)

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using DarkPixelGD.BehaviourTree;

public class DebugLogNode : BTNode
{
    public string message = "Hello World!";

    public override BTNodeRuntime CreateRuntime(Blackboard bb, GameObject owner)
    {
        return new DebugLogRuntime(message, bb, owner);
    }

    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();
        container.style.paddingLeft = 6;
        container.style.paddingTop = 4;
        
        var textField = new TextField("Log Message") { value = message };
        
        textField.RegisterValueChangedCallback(evt => 
        { 
            message = evt.newValue; 
            EditorUtility.SetDirty(this); 
        });

        container.Add(textField);
        return container;
    }
}
```

---

#### 2. Runtime Logic

```csharp
public class DebugLogRuntime : BTNodeRuntime
{
    private string message;

    public DebugLogRuntime(string msg, Blackboard bb, GameObject owner) : base(bb, owner)
    {
        this.message = msg;
    }

    protected override BTStatus OnUpdate()
    {
        Debug.Log($"[{owner.name}] says: {message}");
        return BTStatus.Success;
    }
}
```

---

#### 3. Registering in Search Window

Open **BTSearchWindow.cs** → `CreateSearchTree()`:

```csharp
tree.Add(new SearchTreeGroupEntry(new GUIContent("Action"), 1));

tree.Add(new SearchTreeEntry(new GUIContent("Debug Log"))
{
    level = 2,
    userData = typeof(DebugLogNode)
});
```

---

## 🏗️ Included Core Nodes

### Composites
- Selector  
- Sequence  

### Decorators
- Condition (Type-aware with Abort types)  

### Actions
- MoveTo  
- FaceTarget  
- GetNextWaypoint  
- PlayAnimation  
- Wait  
- CheckDistance  
