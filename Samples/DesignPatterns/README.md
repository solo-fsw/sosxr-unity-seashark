# Unity Design Patterns Library

This library provides a collection of essential design patterns implemented for Unity. It serves as a practical resource for researchers and educators building psychological experiments, cognitive tasks, and educational tools using standard Unity APIs, MonoBehaviours, and ScriptableObjects.

## Table of Contents
1. [Singleton](#1-singleton)
2. [Command](#2-command)
3. [Observer](#3-observer)
4. [State Machine](#4-state-machine)
5. [Strategy](#5-strategy)
6. [Factory](#6-factory)
7. [Object Pool](#7-object-pool)
8. [Service Locator](#8-service-locator)
9. [Decorator](#9-decorator)
10. [Mediator](#10-mediator)

## Getting Started

### Namespace Structure
- Most patterns reside in the `SOSXR.TimelineExtensions.DesignPatterns` namespace.
- The **Mediator** pattern is located in the `SOSXR.TimelineExtensions` namespace.

### Assembly Definition
The library uses the `SOSXR.TimelineExtensions.DesignPatterns` assembly definition.

### Dependencies
This library depends on **SeaShark** (`SOSXR.SeaShark`) for specific utility classes:
- **Singleton**: Uses `PersistentSingleton<T>` and `Singleton<T>`.
- **Command**: Uses the `ICommand` interface and `CommandStack`.
- **Mediator**: Uses the `Mediator` and `Medium` messaging systems.

---

## 1. Singleton
**Description:** Ensures a class has only one instance and provides a global point of access to it.

- **When to use:** Global coordinators like `ExperimentSession` or `AudioManager` that must persist across scenes or be accessed from any script during an experiment.
- **Key Files:** `ExperimentSession.cs`, `AudioManager.cs`
- **How it works:** `ExperimentSession` inherits SeaShark's `PersistentSingleton<T>` to manage session-level state (Setup, Running, Paused, Completed), participant metadata, and session numbers. It survives scene loads across experiment phases and provides a static `Instance` property for global access.

## 2. Command
**Description:** Encapsulates a request as an object, allowing you to parameterize clients with different requests and support undoable operations.

- **When to use:** Experiment protocol editors where researchers build trial sequences and need undo/redo functionality for protocol changes.
- **Key Files:** `ProtocolEditorExample.cs`, `AddTrialCommand.cs`
- **How it works:** `AddTrialCommand` implements `ICommand` (from SeaShark) to handle adding trial conditions to a protocol list. The `ProtocolEditorExample` uses a `CommandStack` to manage protocol edits, allowing researchers to add trials (A), delete selected trials (D), and undo/redo changes (Ctrl+Z / Ctrl+Y).

## 3. Observer
**Description:** Defines a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically.

- **When to use:** Decoupling experiment components — data loggers, adaptive difficulty controllers, live accuracy displays, and block transition managers can all subscribe independently to response events.
- **Key Files:** `GameEvent.cs`, `GameEventGeneric.cs`, `GameEventListener.cs`, `ResponseTrackerExample.cs`
- **How it works:** This implementation uses ScriptableObject-based event channels. `GameEvent` acts as the subject that listeners (`GameEventListener`) subscribe to. When `ResponseTrackerExample` records a participant response, it raises events (`OnResponseRecorded`, `OnAccuracyUpdated`, `OnBlockCompleted`) without knowing which systems are listening.

## 4. State Machine
**Description:** Allows an object to alter its behavior when its internal state changes, appearing as if the object changed its class.

- **When to use:** Experiment flow control (consent → instructions → practice → trials → debrief), task phase management, or adaptive procedure states.
- **Key Files:** `IState.cs`, `StateMachine.cs`, `ExperimentFlowExample.cs`
- **How it works:** The `StateMachine` component delegates `Update` and `FixedUpdate` calls to the currently active `IState`. `ExperimentFlowExample` transitions through IRB-standard phases — `ConsentState`, `InstructionState`, `PracticeState`, `MainTrialState`, and `DebriefState` — keeping the logic for each phase isolated and extensible.

## 5. Strategy
**Description:** Defines a family of algorithms, encapsulates each one, and makes them interchangeable. Strategy lets the algorithm vary independently from clients that use it.

- **When to use:** Swapping adaptive procedures (staircase methods), response scoring rules, or feedback strategies per experimental condition.
- **Key Files:**
  - Adaptive: `IAdaptiveStrategy.cs`, `AdaptiveStrategies.cs`, `AdaptiveController.cs`
  - Scoring: `IScoringStrategy.cs`, `ScoringStrategies.cs`, `ScoringExample.cs`
- **How it works:** `AdaptiveController` manages psychophysical threshold estimation by switching between `FixedDifficultyStrategy`, `OneUpOneDownStaircase`, and `TwoDownOneUpStaircase`. `ScoringExample` demonstrates swappable response evaluation with `AccuracyOnlyScoring`, `SpeedAccuracyScoring`, `InverseEfficiencyScoring`, and `DeadlineScoring`. The trial runner calls the strategy interface without knowing which algorithm is active.

## 6. Factory
**Description:** Provides an interface for creating objects in a superclass, but allows subclasses to alter the type of objects that will be created.

- **When to use:** Generating trial sequences from condition tables, creating stimulus configurations, or producing counterbalanced trial blocks from ScriptableObject definitions.
- **Key Files:** `IFactory.cs`, `TrialFactory.cs`, `TrialDefinition.cs`, `TrialSequenceExample.cs`
- **How it works:** `TrialFactory` is a ScriptableObject that implements `IFactory<GameObject>`. It stores condition parameters (condition label, stimulus duration, inter-trial interval, difficulty level). When `Create()` is called, it instantiates a trial prefab and configures a `TrialDefinition` component. `TrialSequenceExample` picks factories to produce balanced, randomized blocks — adding a new condition requires only a new TrialFactory asset.

## 7. Object Pool
**Description:** Manages a cache of objects that are kept ready for use rather than being created and destroyed on demand.

- **When to use:** High-frequency stimulus presentation (RSVP streams, visual search arrays) where GC spikes could distort precise timing measurements.
- **Key Files:** `GameObjectPool.cs`, `PooledObject.cs`, `StimulusPoolExample.cs`
- **How it works:** `GameObjectPool` maintains a queue of inactive objects. `StimulusPoolExample` requests stimulus display objects from the pool for rapid serial visual presentation. After the configured display duration, stimuli are returned to the pool and deactivated for reuse — no allocation or GC pressure during critical experimental blocks.

## 8. Service Locator
**Description:** Provides a global point of access to a service without coupling the user to the concrete implementation.

- **When to use:** Accessing cross-cutting experiment services like timing, audio, logging, or input devices where you might want to swap implementations (e.g., `NullAudioService` for silent testing or classroom demonstrations).
- **Key Files:** `ServiceLocator.cs`, `IAudioService.cs`, `ISaveService.cs`, `NullAudioService.cs`, `UnityAudioService.cs`, `PlayerPrefsSaveService.cs`, `ResearchServicesExample.cs`
- **How it works:** The static `ServiceLocator` class acts as a registry. Services are registered by their interface (e.g., `IAudioService`). `ResearchServicesExample` demonstrates toggling between real audio and a null service, so the same experiment protocol runs on hardware-equipped lab machines and basic laptops in classrooms — without changing a single line of task code.

## 9. Decorator
**Description:** Attaches additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality.

- **When to use:** Composing stimulus presentation pipelines — adding backward masks, noise overlays, timing jitter, or contrast filters to a base stimulus for perception and psychophysics experiments.
- **Key Files:** `IStimulus.cs`, `BaseStimulus.cs`, `StimulusDecorator.cs`, `StimulusModifiers.cs`, `StimulusBuilderExample.cs`
- **How it works:** `StimulusDecorator` wraps an `IStimulus` instance. Concrete decorators like `BackwardMaskDecorator`, `NoiseOverlayDecorator`, `TimingJitterDecorator`, and `ContrastFilterDecorator` modify duration, intensity, or presentation behavior — forming composable chains (e.g., *Gabor Patch + Noise + Mask + Jitter*) without modifying the base stimulus.

## 10. Mediator
**Description:** Defines an object that encapsulates how a set of objects interact. Mediator promotes loose coupling by keeping objects from referring to each other explicitly.

- **When to use:** Timeline integrations where clips need to broadcast experiment events, or complex multi-system coordination where stimulus presentation, response collection, logging, and device synchronization should remain decoupled.
- **Key Files:** `MediatorBehaviour.cs`, `MediatorClip.cs`, `MediatorMixer.cs`, `MediatorTrack.cs`
- **How it works:** This implementation integrates the Mediator pattern with Unity Timeline. `MediatorBehaviour` (used in Timeline clips) publishes messages through the SeaShark `Mediator` system when a clip starts, plays, or ends. External systems subscribe to these channels to react to Timeline events without being directly linked to the PlayableGraph.
