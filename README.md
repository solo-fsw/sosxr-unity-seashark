# SOSXR SeaShark - C# Utilities Library

A comprehensive collection of C# extensions, utilities, and design pattern implementations for Unity development. SeaShark provides lightweight, reusable components for common gameplay, UI, and system architecture challenges.

## Table of Contents

- [Installation](#installation)
- [Quick Start](#quick-start)
- [Core Features](#core-features)
  - [Design Patterns](#design-patterns)
  - [Extension Methods](#extension-methods)
  - [Configuration System](#configuration-system)
  - [Pub/Sub System](#pubsub-system)
- [Advanced Features](#advanced-features)
  - [Fader](#fader)
  - [Fonts](#fonts)
  - [Interface Support](#interface-support)
  - [In Build](#in-build)
  - [Interfaces](#interfaces)
  - [Minimap](#minimap)
  - [Move (Work in Progress)](#move-work-in-progress)
  - [Object Cue](#object-cue)
  - [Query Strings](#query-strings)
  - [Video Player](#video-player)
- [Contributing](#contributing)
- [License](#license)

## Installation

Add this package to your Unity project via the Package Manager or by cloning the repository into your Assets folder.

## Quick Start

### Using Extension Methods

```csharp
// Safe component access
var rigidbody = gameObject.GetOrAdd<Rigidbody>();

// Vector manipulation
var newPos = transform.position.WithX(10f);

// String utilities
if (!playerName.IsNullOrWhiteSpace())
{
    Debug.Log(playerName);
}
```

### Using the Configuration System

```csharp
public class GameConfig : BaseConfigData
{
    [SerializeField] private int m_difficulty;
    public int Difficulty
    {
        get => m_difficulty;
        set => SetValue(ref m_difficulty, value, nameof(Difficulty));
    }
}

// In initialization code:
var config = ScriptableObject.CreateInstance<GameConfig>();
config.UpdateJsonOnValueChange = new List<string> { nameof(GameConfig.Difficulty) };
config.Initialize();
config.Subscribe(nameof(GameConfig.Difficulty), (newValue) => 
    Debug.Log($"Difficulty changed to {newValue}"));
```

### Using the Mediator Pattern

```csharp
// Subscribe to a medium
Mediator.Subscribe(new Medium("PlayerDamaged"), (medium) =>
{
    Debug.Log($"Player took damage: {medium.Data}");
});

// Publish an event
Mediator.Publish(new Medium("PlayerDamaged", damageAmount));
```

## Core Features

### Design Patterns

#### Command

The Command pattern in SeaShark encapsulates actions as discrete, self-contained units. This makes gameplay sequences, editor actions, and AI steps easier to reason about, test, and reuse.

SeaShark exposes two practical execution strategies: **CommandQueue** (FIFO) and **CommandStack** (LIFO). CommandQueue processes commands in the order they were issued, which is ideal for linear sequences, scripted events, and multi-step user input flows. CommandStack preserves a history of actions so the most recent command can be undone or reversed, which is perfect for undo/redo workflows.

Use cases typically involve modeling an action as an object (e.g., MoveCommand, AttackCommand, SpawnCommand) and routing them through a central executor or history manager. This keeps producers decoupled from executors and makes error handling, retries, and analytics straightforward.

**Related types and namespaces:** `SOSXR.SeaShark.CommandQueue`, `SOSXR.SeaShark.CommandStack`, `SOSXR.SeaShark.ICommand`. For decoupled sequencing, also explore `SOSXR.SeaShark.Mediator`-related classes.

#### Mediator

The Mediator pattern in SeaShark provides a decoupled message-passing backbone. Components communicate via a central hub rather than direct references, reducing coupling and simplifying extension.

Mediator, Medium, and MediatorRegistry form a lightweight pub/sub style system that lets components publish messages and subscribe to types they care about. This approach supports scalable, modular architectures where adding or swapping systems doesn't require rewiring existing components.

Why use a mediator: it makes behavior extensible, testable, and easier to reason about in large scenes. It also facilitates cross-system coordination without tight coupling between game objects.

**Usage patterns:** Register mediator roles with MediatorRegistry, publish messages through Medium, and subscribe to message types in interested components. Use a mediator instance to orchestrate coordinated actions without direct references.

**Related types and namespaces:** `SOSXR.SeaShark.Mediator.Mediator`, `SOSXR.SeaShark.Mediator.Medium`, `SOSXR.SeaShark.Mediator.MediatorRegistry`.

### Extension Methods

Extension methods are lightweight helpers attached to existing Unity types (GameObject, Vector, String, Color, etc.) to reduce boilerplate and improve readability. They live in the `SOSXR.SeaShark.Extensions` family of namespaces and are designed to feel natural in everyday Unity workflows.

Main categories you'll encounter: **GameObjectExtensions**, **VectorExtensions**, **StringExtensions**, **ColorExtensions**, **TransformExtensions**, and possibly additional domain-specific extension sets. The goal is to provide fluent, readable defaults for common tasks (e.g., safe component access, vector tweaking, string utilities, and color manipulations).

Why use extensions: they reduce repetitive boilerplate, improve code clarity, and enable fluent style patterns that align with gameplay logic and UI workflows.

**Common usage patterns:**
- `GetOrAddComponent<T>(gameObject)` ensures a component exists without boilerplate
- `Vector3.WithX(v, x)` / `WithY(v, y)` / `WithZ(v, z)` yield modified vectors in a single expression
- `string.ToTitleCase()` or `string.IsNullOrWhiteSpace()` for string utilities
- `color.WithAlpha(a)` produces a new color with adjusted transparency

**Related namespaces:** `SOSXR.SeaShark.Extensions`, `SOSXR.SeaShark.Extensions.GameObjectExtensions`, `SOSXR.SeaShark.Extensions.VectorExtensions`, `SOSXR.SeaShark.Extensions.StringExtensions`, `SOSXR.SeaShark.Extensions.ColorExtensions`.

### Configuration System

The BaseConfigData system provides automatic JSON persistence and reactive change notifications. Define configuration classes by inheriting from BaseConfigData and using the SetValue helper in property setters.

**Key features:**
- Automatic JSON serialization/deserialization
- Property change notifications (specific and global)
- Reflection-based field change detection in the editor
- Configurable auto-update on specific property changes

**Related types:** `SOSXR.SeaShark.BaseConfigData`, `SOSXR.SeaShark.HandleConfigData`.

### Pub/Sub System

The GlobalEvent system provides a garbage-free, disposal-based event mechanism for decoupled communication. Events are obtained via Get(), listeners are registered/unregistered, and the event fires when disposed.

**Key features:**
- Singleton-like pattern with InUse flag to prevent re-entrant firing
- Garbage-free event firing
- Automatic cleanup and reuse
- Thread-safe listener management

**Related types:** `SOSXR.SeaShark.Excessives.GlobalEvent<T>`.

## Advanced Features

### Fader

The Fader utility provides smooth fade transitions for UI and scene-related elements, typically by adjusting CanvasGroup alpha or material/color properties.

Use it to create polished transitions between UI panels, scenes, or gameplay states without duplicating fade logic across components.

**Why:** Consistent timing and easing across the project, better user experience, and a centralized place to tweak feel and duration.

**Basic usage patterns:** Fade a UI panel in by gradually increasing a CanvasGroup alpha from 0 to 1 over a duration; fade out when closing a panel; chain fades with content loading steps to produce seamless UX.

**Related types/namespaces:** `SOSXR.SeaShark.UI.Fader`, `SOSXR.SeaShark.UI.CanvasGroupFader`, `SOSXR.SeaShark.Utils.FaderUtilities`.

### Fonts

Font utilities surface and manage font assets (e.g., Maple Mono, LinBiolineum) used across UI text. They simplify loading, caching, and applying fonts to TMPro components.

**How to use:** Load a TMP_FontAsset via a helper (e.g., FontManager.LoadAsset or FontUtils.GetFontAsset("Maple Mono")) and assign it to TextMeshPro components at runtime for consistent typography across the app.

**Common scenarios:** Theming or accessibility changes that require font swaps, global font defaults for headings vs body text, and runtime font switching for localization or branding updates.

**Related namespaces:** `SOSXR.SeaShark.Fonts`, `SOSXR.SeaShark.Fonts.MapleMono`, `SOSXR.SeaShark.Fonts.LinBiolineum`.

### Interface Support

Interface support helps you expose and consume interfaces across Unity objects in a decoupled fashion. Attributes and small discovery helpers enable straightforward interface-based wiring without hard references.

**What's available:** A lightweight InterfaceAttribute (and helpers) to tag components that implement interfaces for runtime discovery and resolution. This makes it easier to wire services, controllers, and systems in large projects.

**How to implement:** Define a simple interface (e.g., IMyService) and implement it on a MonoBehaviour. Mark the component with the appropriate interface attribute to participate in discovery, then resolve via an InterfaceResolver or similar helper.

**Usage patterns:** Implement core lifecycle or service interfaces (IInitializable, IUpdatable, IConfigurable) to standardize behavior, then resolve implementations at runtime through the interface abstraction.

**Related resources:** InterfaceAttribute, `SOSXR.SeaShark.Interfaces`.

### In Build

In Build contains build-time utilities and scripts intended to influence the final build. They help you tailor production behavior without polluting editor-time code paths.

**Typical tasks:** Strip or replace debug utilities for production, apply platform-specific tweaks, and prune unused assets or test hooks to reduce build size.

**Why:** Keep a clean separation between editor-time tooling and runtime behavior, while ensuring predictable production builds.

**Usage patterns:** Create prebuild/postbuild scripts or use BuildConfig directives to enable/disable features depending on target platforms. Configure these utilities to run automatically during the build pipeline.

**Related resources:** `SOSXR.SeaShark.Build`, BuildTime utilities, BuildScripts.

### Interfaces

This section lists core interface contracts used across SOSXR components and when to implement them. Interfaces provide explicit, decoupled contracts that help with testing and modularity.

**Typical contracts** include generic lifecycle hooks and configurability markers (examples: IInitializable, IUpdatable, IConfigurable). Use them to enforce consistent behavior across components.

**When to implement:** When your class provides a service or capability that others should consume via a known interface, not a concrete type. This enables clean substitution and easier unit testing.

**How to implement:** Define a small interface and implement it on a MonoBehaviour or ScriptableObject. Expose resolution utilities to obtain a concrete implementation at runtime without tight coupling.

**Related resources:** InterfaceAttribute, interface discovery utilities.

### Minimap

The Minimap subsystem ties together a Cartographer with a minimap UI to visualize a simplified representation of the world. It decouples world data from the UI rendering, letting you adjust what appears on the map without altering gameplay mechanics.

Configure MapMaker entries to describe map regions, icons, visibility rules, and zoom behavior. MapMaker entries allow you to tailor map visuals for exploration, objectives, and player guidance.

**Basic setup steps:**
1. Add a Minimap controller to your UI
2. Wire in a Cartographer component
3. Create MapMaker entries for the areas you want on the map
4. Update or tune entries to reflect terrain, objectives, or faction colors as needed

**Related classes:** `SOSXR.SeaShark.Minimap.Cartographer`, `SOSXR.SeaShark.Minimap.MapMaker`, `SOSXR.SeaShark.Minimap.MapMakerEntry`, `SOSXR.SeaShark.UI.MinimapPanel`.

### Move (Work in Progress)

Move is a work-in-progress feature aimed at providing movement control and basic pathing for game entities. The current scope focuses on scaffolding movement commands and waypoint navigation; more advanced pathfinding, obstacle avoidance, and performance optimizations are forthcoming.

**Status and limitations:** As an evolving API, some method names and behaviors may change. It's best used with feature flags or guarded usage inside experimental branches.

**How to use (conceptual):** Attach a MoveController or MoveAgent to your unit, call StartMove(targetPosition) or feed a waypoint list, and listen for completion or interruption events to trigger subsequent actions.

**Related resources:** `SOSXR.SeaShark.Move`, MoveController, MoveAgent.

### Object Cue

Object Cue provides lightweight cues to visually mark objects in the scene. This is useful for tutorials, onboarding, tooltips, or gameplay feedback when an object is interactive or important.

**Use cases** include highlighting interactables when focused, signaling objectives or collectibles, and providing hover or targeting feedback in 3D space.

**Basic usage (conceptual):** Attach an ObjectCue component to a target, or invoke ObjectCue.Show(target, CueType.Highlight, duration). Hide or fade out after the cue completes or when the player moves away.

**Related resources:** `SOSXR.SeaShark.Cues.ObjectCue`, CueType enums, CuePalette.

### Query Strings

QueryURL and a simple builder help you assemble URLs with query parameters in a safe, consistent manner. This is particularly valuable for making REST API calls, search endpoints, or feature-flag driven URLs.

**Why:** Centralizes encoding, parameter ordering, and null-handling for query strings. Reduces boilerplate and makes testing easier by isolating URL-building logic.

**Basic usage pattern (conceptual):** Start with a base URL, then chain Add("key", value) calls for each parameter. Finalize with ToString() to obtain the full URL.

**Related types:** `SOSXR.SeaShark.Network.QueryURL`, `SOSXR.SeaShark.Network.QueryStringBuilder`.

### Video Player

Video player utilities wrap Unity's VideoPlayer integration to streamline common playback tasks. They provide helpers to load, play, pause, seek, and respond to completion events in a consistent way.

**Why:** Simplify in-game tutorials, cutscenes, and contextual video experiences. Centralized handling reduces boilerplate across multiple scenes and UI layers.

**Basic usage pattern (conceptual):** Invoke VideoPlayerManager.Play(videoClip) to start playback, and subscribe to completion callbacks to trigger next steps. Use Pause/Resume/Seek for precise playback control as part of UI or gameplay flow.

**Related resources:** `SOSXR.SeaShark.Video.Player`, VideoPlayerExtensions, VideoCueSystem.

## Contributing

Contributions are welcome! Please ensure that:
- All public APIs have XML documentation comments
- Complex logic includes inline comments explaining the "why"
- Code follows the existing style and conventions
- Tests are included for new features

## License

See LICENSE.md for details.
