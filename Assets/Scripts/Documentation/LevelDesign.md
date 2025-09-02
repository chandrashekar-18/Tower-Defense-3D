# Level Design Guide

## Overview
This guide explains how to create and edit levels for Tower Defence 3D using the built-in Level Editor tool.

## Opening the Level Editor
1. In Unity Editor, go to `Tools > Tower Defense > Level Editor`
2. The Level Editor window will appear with three main sections:
   - Level Properties
   - Grid Editor
   - Wave Editor

## Level Properties
- **Level Name**: Give your level a unique name
- **Level Number**: Set the level's order in the game progression
- **Starting Currency**: Set how much currency players start with
- **Grid Size**: Adjust the width and height of the level grid (5-30)

## Grid Editor

### Paint Tools
Use these tools to design your level layout:
- ⬜ Empty: Clear terrain for tower placement
- 🛤️ Path: Mark the path enemies will follow
- 🟢 Spawn: Enemy spawn points (at least one required)
- 🔴 Exit: Level exit points (at least one required)
- ⬛ Obstacle: Blocked areas where towers cannot be placed

### Quick Actions
- **Clear Grid**: Reset the entire grid layout
- **Random Path**: Generate a procedural path with customizable settings
  - Path Complexity: Controls path windings (1-5)
  - Allow Diagonals: Enable/disable diagonal path segments
  - Min Path Length: Set minimum path length

### Best Practices
1. Ensure there's a clear path from spawn to exit
2. Create strategic tower placement spots
3. Use obstacles to create choke points
4. Balance path length with difficulty
5. Include multiple tower placement options

## Wave Editor

### Wave Configuration
- **Add Wave**: Create a new enemy wave
- **Random Waves**: Generate procedural waves with increasing difficulty
- **Enemy Groups**: Configure groups within each wave:
  - Enemy Type: Select enemy variant
  - Count: Number of enemies in group
  - Spawn Delay: Time between enemy spawns
  - Delay Between Groups: Pause between groups

### Wave Design Tips
1. Start with easier enemies in early waves
2. Gradually increase difficulty
3. Mix different enemy types for variety
4. Balance spawn delays for rhythm
5. Create breaks between waves for tower building

## Saving and Testing
1. Click "Save Level" to store your level
2. Test your level in Play mode
3. Adjust difficulty based on playtesting
4. Ensure all paths are reachable
5. Verify tower placement spots are balanced

## Common Pitfalls to Avoid
- Don't make paths too short or direct
- Avoid creating impossible choke points
- Ensure sufficient tower placement options
- Don't overwhelm players in early waves
- Test all enemy spawn points