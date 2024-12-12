# Hunter vs Prey Agent

## Objective

The **Hunter vs Prey Agent** project demonstrates the integration of **Unity** and **Unity ML-Agents** to create a dynamic game environment. The goal is to simulate adaptive behavior in non-player characters (NPCs) using AI, focusing on training two agents (Hunter and Prey) in a competitive environment.

## Game Scenario

### Agents
- **Hunter**: Pursues the Prey and intercepts it to win.
- **Prey**: Evades the Hunter while collecting randomly spawned targets.

### Rules
- The **Hunter** earns points for catching the Prey and loses points for hitting walls.
- The **Prey** earns points for collecting targets but loses points for getting caught or colliding with walls.

### Environment
The game initializes agents at random positions and encourages diverse interaction scenarios.

## Methodology

1. **Random Initialization**: Both agents and targets spawn at random locations.
2. **Design and Inputs**: Heuristic-based design guides the initial behaviors.
3. **Reward System**:
   - Encourages goal-oriented actions.
   - Penalizes undesirable behaviors like wall collisions or inactivity.
4. **Training**:
   - Utilizes **Proximal Policy Optimization (PPO)**.
   - Employs multiple parallel environments for efficient training.

## Tools and Frameworks

- **Unity 3D**: For creating the game environment.
- **Unity ML-Agents**: Framework for training agents using reinforcement learning.
- **Python**: For managing training configurations and reinforcement learning algorithms.

## Training Details

- **Reinforcement Learning**: Based on PPO to balance exploration and exploitation.
- **Hyperparameters**:
  - Batch Size: 512
  - Buffer Size: 10,240
  - Learning Rate: Linear schedule
  - Epochs: 5
  - Maximum Steps: 5,000,000

## Results and Observations

- The **Hunter** consistently performed better due to a simpler task and refined strategy.
- The **Prey** faced challenges due to its complex objectives.
- Reward tuning played a significant role in shaping agent behavior.

## Challenges

- Computational overhead during training.
- Difficulty in achieving balance between agent objectives.
- Prey agent's strategy often led to undesired behaviors, like self-destructing when cornered.

## Conclusion

The project explores the capabilities of **Unity ML-Agents** in creating interactive and adaptive AI environments. It highlights the potential of reinforcement learning and identifies areas for further improvement in complex agent behavior.
