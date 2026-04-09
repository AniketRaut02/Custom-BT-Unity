# Custom Physics Engine for Unity

A lightweight, component-based physics engine built from scratch in C# to bypass Unity's built-in PhysX. This project demonstrates custom integration, collision detection, and resolution logic designed for high-performance or specialized 2D/3D interactions.

## 🚀 Features
* **Custom Integrator:** Implementation of Verlet and Euler integration for predictable motion.
* **Collision Detection:** Support for AABB (Axis-Aligned Bounding Box), Sphere-to-Sphere, and Raycast intersections.
* **Material Properties:** Adjustable friction, restitution (bounciness), and mass constants per object.
* **Non-Kinematic Solver:** Operates independently of Unity’s standard Physics loop for total control over the simulation.

## 🛠️ Technical Implementation
The engine calculates forces using:
* **F = ma** for acceleration derivation.
* Impulse-based collision resolution to handle energy transfer between bodies.
* Spatial partitioning (Grid-based) to optimize collision checks.

## 📂 Project Structure
* `/Core`: The main physics solver and integration logic.
* `/Colliders`: Geometric collision primitives and intersection math.
* `/Utils`: Vector math extensions and gravity constants.

## 🎮 Getting Started
1. Clone the repository into your Unity `Assets` folder.
2. Attach the `CustomPhysicsBody` component to any GameObject.
3. Use the `PhysicsWorld` manager to globalize gravity and simulation steps.
