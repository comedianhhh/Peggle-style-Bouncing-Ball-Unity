# Peggle-style Bouncing Ball 


### **1. DOTS Physics for Optimized Performance**

The project's physics simulation is built entirely on Unity's DOTS Physics package, as required by the assignment, and it deliberately avoids Unity's traditional physics system.[1] This choice was fundamental to achieving the project's goal of high performance and scalability.[2, 3]

*   **Bouncing and Force Application:** The ball's core bouncing behavior is handled natively by the DOTS Physics engine. When the ball is launched, an initial impulse is applied to its `PhysicsVelocity` component.[4] Unlike traditional `Rigidbody.AddForce()`, this direct manipulation of velocity is a core principle of DOTS Physics and ensures a precise and efficient simulation . All subsequent bounces off pegs, walls, and the moving bucket are a result of the physics simulation itself, where the engine calculates and applies forces to change the ball's `PhysicsVelocity` based on the collision .
*   **Efficient Collision Detection:** A dedicated `ICollisionEventsJob` is used to handle all collision detection.[5] This job is designed to run in parallel, which is critical for performance in a game with a large number of interacting entities.[6] To prevent unnecessary calculations, `CollisionFilter` components were implemented. These filters use bitmasks to define collision layers, ensuring that entities like the ball only check for collisions with relevant objects like pegs and walls, and ignore collisions with non-interactive elements or other pegs.[5, 7] This granular control over collision pairs is a major optimization strategy .
*   **Optimized Colliders:** For all physics objects (the ball, pegs, walls, and the moving bucket), primitive `PhysicsShape` components were used, such as `SphereCollider` and `BoxCollider` . These simple shapes are significantly more performant for collision detection than complex `MeshCollider` components, which require a much higher computational cost to process.[7]

### **2. Moving Bricks with ECS**

The dynamic movement of the special Infinity, M, and Z-shaped bricks is implemented using the Entity Component System (ECS), which is the core framework of DOTS.[6] This approach separates data from logic, allowing for highly efficient, parallelized processing of all moving entities.[6, 8]

*   **Data-Driven Movement:** The path for each moving brick is defined by a data-only component, such as `MovingBrickPathData`, which stores parameters like speed and a `PathType` enum (Infinity, M, or Z).[1] A separate `PathProgress` component stores the mutable state of each brick's journey along its path.[1]
*   **The `BrickMovementSystem`:** The game's logic for moving these bricks resides in a dedicated system named `BrickMovementSystem`.[1] This system uses an efficient query to find all entities that have a `MovingBrickTag` and the necessary path components. For each matching entity, the system calculates a new position based on the stored `PathType` and a `deltaTime` value.[1]
*   **Implementation Logic:**
    *   For the Infinity path, the system uses parametric equations (mathematical functions like sine and cosine) to calculate a continuous, looping figure-eight trajectory.[1]
    *   For the M and Z paths, the system calculates the brick's position by interpolating between predefined waypoints. When the brick reaches the end of its path, a flag in its `PathProgress` component is flipped, causing it to reverse direction and traverse the path backward.[1]
*   **Burst Compilation:** The entire `BrickMovementSystem` is tagged with the `` attribute.[1] The Burst Compiler translates the C# movement code into highly optimized native code, which allows the logic to be executed efficiently across multiple CPU cores . This ensures that even with a large number of moving entities, the game maintains a high frame rate and smooth performance.
