# Changelog

All notable changes to this project will be documented in this file.
The changelog format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [Unreleased]

### Added

- `Tests/EditMode/RegressionTests.cs` with regression tests for the bugs fixed in this release (`MathE.Clamp`, `MathE.ReMap`, `Transform.Reset`, `CommandStack` redo order, `Mediator`, and others).
- Animation Rigging and Poke Deformation samples are listed in `package.json`.

### Changed (breaking)

- `MathE.Clamp` parameter order is now `(value, min, max)` (was `(value, max, min)`).
- `TransformExtensions.Reset` / `GameObject.ResetTransformation` reset the local position (was world position).
- `SimpleToneGenerator.PlayForDuration(float)` takes seconds.
- `LoadQueryURL` now serializes `Object` references (`m_webView`, `m_configData`) and casts them to `ILoadQueryURL` / `IHaveQueryData`. Reassign them in the Inspector.
- `AdditionalUnityEvent`: a trigger during a pending delay no longer cancels it; a non-matching build filter no longer logs an error.
- Sample `QueryURL` class renamed to `SampleQueryURL`.
- Samples now live in `Samples~/` instead of `Samples/`, so Unity does not import them with the package. Import them from the Package Manager.
- Runtime and Editor asmdefs no longer reference `SOSXR.EnhancedLogger`. Added dependency `com.unity.xr.core-utils`.

### Fixed

- Player builds: unguarded `using UnityEditor;` in `Helpers` and `EasyIKVisuals`.
- Components that only resolved references in `OnValidate` (`VideoPlayerManager`, `ImageFader`, `SimpleToneGenerator`) now also do so in `Awake`.
- `BaseConfigData` deleted `config.json` in `OnDestroy`; lambda unsubscribe leaks in `BaseConfigData`, `KeyStrokeManager`, `UIManager`, `BuildQueryURL` sample.
- `MathE.ReMap`, physical constants, PID derivative, `FindAncestorByTag`, `ToVector3Int`, `DestroyChildrenImmediately`, `IsOdd` for negatives, `ExtensionsE.IsNull` for destroyed objects.
- `EasyIK` pole constraint with fewer than 3 joints, `TweenRunner` stalling on a throwing tween, `CommandStack` redo order, `GlobalEvent` in-use guard, `RegulatorSingleton` ties, `Mediator` without registry and with domain reload disabled, `Move` path/orbit/bounce glitches, `Cartographer` duplicates, `Whiteboard`/`VideoPlayerManager` texture leaks, `DOLocalRotate` long-way spins.
- Editor: multi-object editing re-enabled; button parameters use invariant culture and stable keys; property drawers fixed for lists, multi-edit and Undo.
- Samples with optional dependencies compile only when those packages are installed. Optional sample code (Mediator Timeline, CopyTransform, Animation Rigging, Poke Deformation) is guarded by `#if SOSXR_HAS_*` symbols set in the sample asmdef `versionDefines`. Animation Rigging and Poke Deformation are skipped entirely until their packages are present (`defineConstraints`).
- `CommandStack` redo is last-in-first-out, so the most recently undone command is redone first.
- `QueryConfigData.QueryStringVariables` is initialised to an empty list instead of `null`.
- `Mediator` static state is reset when Play starts, so it no longer carries over between play sessions.
- `GlobalEvent.Get()` reuses one instance instead of allocating a new one per call. The in-use flag is released even if a listener throws.
- `TimeController` skips unassigned Input Action references instead of throwing.

## [0.4.0] - In Progress

### Added

- Minimap

### Removed

- Removed SettingProvider (will now exclusively live in EditorSpice). You can register a new one from SeaShark (or wherever) if you want though, see Samples.

### Changed

- ButtonAttribute now works with values you set for parameters
- ButtonAttribute has option for space and/or horizontal line above.
- QuitApplication.Quit now also works in the editor

### Fixed

- Optimised the SOSXR Base Editor
- Reduced runtime allocations in `AdditionalUnityEvent` by replacing `.tag` reads and LINQ tag matching with `CompareTag` loops.
- Reduced coroutine and logging allocations in `AudioSourcePlayer` by caching the fixed wait and removing interpolated logger strings for value types.
- Removed LINQ allocations from `ColliderInteractionBase` target collider discovery.
- Added XML documentation and inline optimization notes for audited runtime components.

## [0.3.0] - 22-07-2025

### Added

- Dependency on [Enhanced Logger](https://github.com/solo-fsw/sosxr-unity-enhancedlogger)
- [Whiteboard](https://github.com/solo-fsw/sosxr-unity-whiteboard)
- [Keyboard](https://github.com/solo-fsw/sosxr-unity-keyboard)
- [Unity Interface Support](https://github.com/TheDudeFromCI/Unity-Interface-Support/)
- [Maple Mono font](https://github.com/subframe7536/maple-font)
- [EasyIK](https://github.com/joaen/EasyIK)
- Input Actions (in Samples/Input/)
- Optional attribute
- Android Permission Requester (which depends on [UnityAndroidRuntimePermissions](https://github.com/yasirkula/UnityAndroidRuntimePermissions))
- [SirHall - Excessives](https://github.com/SirHall/Excessives?tab=readme-ov-file)
- [drawers from: anchan828](https://github.com/anchan828/property-drawer-collection)
- [Geometry Painter](https://gist.github.com/phosphoer/8cccb00e20d9892af1438a795779bee0)
- [Book Page Curl](https://github.com/Dandarawy/UnityBookPageCurl?tab=LGPL-3.0-1-ov-file) (Samples due to more restrictive GPL3 license)
- Option to set the TMP Default font asset
- Easy way to create new Settings in either Project Settings, or Preferences.

### Fixed

- Unity minimum supported version
- Fixed windows finding home folder
- AdditionalUnityEvents not firing
- SOSXRBaseEditor now allows for `[ContextMenu(...)]` to draw buttons.

### Changes

- Moving files around
- Icons
- Readme
- Namespaces simplified: SOSXR.SeaShark for most, unless editor, then SOSXR.SeaShark.Editor
- SOSXRBaseEditor now allows for a Tooltip to be displayed.

## [0.2.1] 02-04-2025

> ### Package Numbering Change
> #### Package will now be numbered starting with 0, to better reflect the current status in development (see the official semver information [here](https://semver.org/#spec-item-4)).
>
> If any issues arise when updating from previous (and higher numbered versions), please delete the old version before updating to this version.

### Added

- Patterns
    - Command
    - Mediator
- Video Player
- [Config Data 2.3.2](https://github.com/solo-fsw/sosxr-unity-configdata)
- HorizontalLine attribute
- Button for Methods
-

### Fixed

- Adjusted namespaces

### Changed

- All to Shark logo
- InfoTextAttribute updated and now named InfoAttribute

## [2.0.0] - 2025-02-17

### Added

- [Object Cue v2.1.0](https://github.com/solo-fsw/sosxr-unity-objectcue)
- Attributes from [Editor Tools 2.2.0](https://github.com/solo-fsw/sosxr-unity-editortools)
- DrawGizmo from [Editor Tools 2.2.0](https://github.com/solo-fsw/sosxr-unity-editortools)
- [Fader 2.0.0](https://github.com/solo-fsw/sosxr-unity-fader)

## [1.0.0] - 2025-02-17

### Added

- [Additional Unity Events v2.0.1](https://github.com/solo-fsw/sosxr-unity-additionalunityevents)
- [Extension Methods v2.0.1](https://github.com/solo-fsw/sosxr-unity-extensionmethods)
- [SimpleHelpers v2.0.1](https://github.com/solo-fsw/sosxr-unity-simplehelpers)
- [Device Dependent Helper v2.0.0](https://github.com/solo-fsw/sosxr-unity-devicedependenthelpers)
