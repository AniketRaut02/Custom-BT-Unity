# Modular Behavior Tree System

A high-performance, node-based Behavior Tree (BT) framework designed for modular AI logic in Unity. This system allows for complex NPC decision-making through a clean, extensible C# architecture.

## 🧠 Core Architecture
The system utilizes a base `Node` class with three primary states:
* `SUCCESS`: The node completed its task.
* `FAILURE`: The node failed or conditions were not met.
* `RUNNING`: The node is still executing (e.g., a "MoveTo" action).

## 🧩 Included Nodes
* **Composites:** Sequence (AND logic), Selector (OR/Priority logic).
* **Decorators:** Inverter, Repeater, Succeeder.
* **Leaf Nodes:** Basic actions like Wait, MoveTo, and DebugLog.

## ➕ Adding Your Own Node
The system is designed for easy expansion. To create a custom Action or Decorator:

1. **Inherit from the Node class:** Create a new C# script and inherit from `BTNode`.
2. **Override the Evaluate method:** This is where your custom AI logic lives.
3. **Return a State:** You must return a `NodeState` so the parent knows how to proceed.

### Example Code:
```csharp
public class MyCustomAction : BTNode
{
    public override NodeState Evaluate()
    {
        // Your Logic (e.g., Check health or distance)
        if (targetInRange)
        {
            _state = NodeState.SUCCESS;
            return _state;
        }
        
        _state = NodeState.RUNNING;
        return _state;
    }
}
