# Roadmap Repo

- By: Maarten R. Struijk Wilbrink
- For: Leiden University SOSXR
- Fully open source: Feel free to add to, or modify, anything you see fit.

## Project Vision
- SeaShark provides a lean, modular set of C# utilities and design-pattern implementations for Unity.
- Core philosophy: lightweight, reusable, and decoupled components that minimize dependencies and maximize composability.

## Current Status
- Current version: 0.5.0 (stable core)
- Stability: production-ready core utilities; some areas are experimental and may evolve
- Production-ready: UI utilities, gameplay scaffolding, and common design patterns
- Experimental: networking helpers, ML integration points, VR/AR considerations

## Planned Features
- High Priority (next 1-2 releases):
  - Advanced pathfinding and obstacle avoidance
  - Enhanced minimap features (fog of war, dynamic markers)
  - Performance optimizations for large scenes
- Medium Priority (future releases):
  - Additional design pattern implementations
  - Extended UI utilities
  - Networking helpers
- Low Priority (long-term):
  - VR/AR support considerations
  - Advanced physics utilities
  - Machine learning integration points

## Known Issues
- Some utilities assume Unity API compatibility with recent LTS releases; verify against your Unity version.
- Pathfinding features may require scene/navmesh configuration for optimal results.
- Minimap fog of war and dynamic markers may need explicit data sources in complex scenes.
- Editor-only helpers and sample dependencies may not be included in builds; follow sample README for setup.

## Contributing
- How to contribute: open issues and pull requests on the GitHub repository; follow the project guidelines.
- Code standards: follow Unity/C# conventions; use XML documentation for public APIs; prefer lightweight, decoupled components.
- Testing requirements: build and run the provided Samples; perform basic smoke tests for pathfinding and UI utilities.

## Timeline
- 2026-04: v0.5.0 release – core utilities stabilized; core UI and gameplay scaffolding complete.
- 2026-07: v0.6.0 – advanced pathfinding, minimap enhancements, and performance tuning for large scenes.
- 2026-12: v1.0.0 – stable feature set; VR/AR and ML integration groundwork.

(End of file)
