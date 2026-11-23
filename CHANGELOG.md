# Changelog

All notable changes to this project will be documented in this file.
The changelog format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [0.8.1] - 2026-11-23

Fixes from the audit of older SeaShark code (2026-11-17).

### Fixed

- `ChangeLayer` restored the wrong layer, and threw for layer 0: it stored a layer index in a `LayerMask` and took its logarithm. It now stores the index. `Change()` warns and does nothing when the mask does not have exactly one layer.
- `Mediator` logged two errors on every Play when the project has no `MediatorRegistry` in Resources, also when the Mediator is not used. It now logs one warning, the first time the Mediator is used.
- `transform.DestroyChildren()` did not compile (CS0121): it was declared in both `ExtensionMethods` and `TransformExtensions`. The `ExtensionMethods` copy is removed.
- `TweenSequence`: from the second loop on (and on the way back of a Yoyo loop), a later tween on the same property was applied before its start time and overwrote the earlier tween. Tweens are now only applied from their start time, and in start-time order, also when they are inserted out of order.
- `FrameRateLimiter` and the Design Pattern samples threw every frame with Active Input Handling set to Input System only. They now use the Input System when it is enabled, and the legacy Input Manager otherwise (samples: new `SampleKeyboard` helper).
- `AnimatorHelpers` set the Animator's avatar to null on Start. It now keeps the current avatar and logs a warning that building one is not implemented.
- The package declares Unity 6000.0, but used `FindObjectsByType` overloads without a sort mode, which exist from 6000.4. The new `SceneObjects.FindAll` uses those on 6000.4 and newer, and the sort-mode overloads before that.
- `CommandQueue.Clear()` now also clears the redo stack.
- `ExtensionMethods`: `GetChildrenWithoutParent` returned trailing nulls when children were inactive. `RoundCorrectly(double)` rounded through `float`; both overloads returned or threw on NaN, infinity and very large values (now returned unchanged). `GetPersistentCallersFromUnityEvent` threw for overloaded methods and missing targets. `GetRemainder` returned the dividend for negative dividends. `AddIfUnique` added `null` every time, and `AddRangeUnique(null)` added a `null`. `SortByName` threw on destroyed objects. `IsLastChild` threw on root objects. `PlayEnsured` ignored the pitch when it scheduled the temporary source's destroy.
- `BitWisE.SingleBinaryByteToBool` returned the bits in the opposite order of `BoolArrayToSingleBinaryByte`, so a round trip reversed them.
- `CryptoSymmetric`: an empty key divided by zero; it now throws `ArgumentException`. The docs now say it is obfuscation, not encryption.
- `ExtensionsE.Write`, `WriteLine` and the `Write...Elements` helpers wrote to `System.Console`, which Unity does not show. They log to the Unity Console.
- `MediatorRegistry.UpdateData` updated the published `Medium`, not the stored entry.
- `TextMeshProSetText`: a clear scheduled by an earlier `SetText` hid newer text.
- `MatchAudioSpeedToTimeSpeed` threw on a destroyed `AudioSource`.
- `QueryURL.BuildQueryURL` wrote numbers with the current culture (`1,5` on nl-NL), threw for `null` parameter names or source, and added a second `?` to a URL that already has a query.
- `LoadQueryURL` waited forever for the WebView, with a warning every 0.1 s. It now logs once and stops after `m_readyTimeout` seconds (default 30; 0 waits forever).
- `StringExtensions.RichRotation` and `RichSpace` wrote the number with the current culture, which broke the TextMeshPro tag on nl-NL.
- `TaskExtensions.Forget` lost the original stack trace when it rethrew.
- `FileHelpers`: unsupported platforms returned an error sentence as the path; they now return an empty string and log a warning. `GetArborXRPath` has a Windows branch.
- `WaitFor.Seconds`: the cache grew without limit; it is cleared at 100 durations. Zero and negative durations return `null`.
- `Move`: `SmoothMoveTowards` and `SmoothBounceBackAndForth` were frame-rate dependent; they use `Move.SmoothingFactor` now. `SmoothBounceBackAndForth` with both points equal (both zero by default) pulled the object to the world origin; it now does nothing.
- `QualityLevel` never applied the device-specific level, and `DeviceDependentUnityEvents` only logged "Subscribe to Events from here". Both now react to `CurrentDevice.Changed`. `DeviceDependentUnityEvents` fires once per device, also when it runs on Start and on the event.
- `EasyIK` threw every frame without a Target, with `NumberOfJoints` 0, or with a joint that has no child. It now logs an error and disables itself.
- `DrawGizmo` logged a warning on every repaint for a non-uniform scale. It now warns once.
- `SetMainCameraAsCanvasWorldCam`, `LineRendererController`, `DeleteColliders` and `ResetXROrigin` resolved their references only in `OnValidate`, so they were null in builds and after `AddComponent` at runtime. `SetMainCameraAsCanvasWorldCam` could also disable itself from `OnValidate` in Edit mode.
- `Singleton`, `PersistentSingleton` and `RegulatorSingleton` created a new GameObject when `Instance` was read during shutdown.
- `TagSelectorAttribute.cs` had a leftover `#define SOSXR_EDITORTOOLS_INSTALLED`.

### Added

- `SceneObjects.FindAll<T>(FindObjectsInactive)` and `SceneObjects.FindAll(Type, FindObjectsInactive)`.
- `CurrentDevice.Changed` event and `CurrentDevice.Set(Device)`.
- `ChangeLayer.LayerFromMask(int)`, `FrameRateLimiter.FrameRateForFunctionKey(int)`, `Move.SmoothingFactor(float, float)`.
- `LoadQueryURL.m_readyTimeout`.
- EditMode tests: `OlderCodeFixTests`.

### Changed

- `ExtensionMethods.DestroyChildren(Transform)` is removed; use `TransformExtensions.DestroyChildren`.
- `FileHelpers` returns an empty string on unsupported platforms.
- `ExtensionsE.Write` and `WriteLine` log to the Unity Console.

### Not changed

- `ChangeLayer`, `AudioSourcePlayer`, `SimpleToneGenerator` and `ToneExporter` stay in the global namespace: moving them would break code that uses them.

## [0.8.0] - 2026-11-17

### Added

- `BuildInfoToUI.FormatBuildInfo(string line, DateTime now)` (public static).
- EditMode tests for `BuildInfoToUI` and `CommandStack.Clear`.

### Fixed

- Overzealous Gizmo initialization in DrawGizmo
- Unity 6.6 deprecation warnings (UAC0009): `AdditionalUnityEvent` (`OnlyInDevelopmentBuilds`, `OnlyInProductionBuilds`) and `InProductionBuild` now check `Debug.isDebugBuild` at runtime instead of the `DEVELOPMENT_BUILD` symbol. Unity removes that symbol in 6.8. Behaviour is unchanged and works on every supported Unity version.
- Unity 6.6 serialization analyzer warning (UAC1001) on `MoveEditor.vector3OptionsProp`: marked `[NonSerialized]`.
- `BuildInfoToUI` read the wrong CSV columns and threw a `FormatException` every 10 seconds. It now reads the EditorSpice build-info layout (ProductionBuild, SemVer, bundle code, date, time) and shows "Invalid build information format." for a bad line instead of throwing.
- The Poke Deformation sample path in `package.json` pointed to a folder that does not exist (`Samples/Poke Deformation`). It is now `Samples/PokeDeformation`.
- `CommandStack.Clear()` now also clears the redo history.
- `Cartographer.DestroyMap()` now destroys only the minimap sprites it created, not every object with the minimap tag.
- `PlatformDependentSpawner` warns instead of throwing when a prefab or the device reference is not assigned. It no longer spawns twice when called again before the spawn delay ends.
- `AudioSourcePlayer` finds its `AudioSource` in `Awake` (it was only set in the Editor) and warns when no clips are assigned.

### Changed

- `AdditionalUnityEvent.ShouldFire` delegates to new internal `MatchesBuildContext(...)`, a pure function that EditMode tests can call. Runtime assembly gets `InternalsVisibleTo("SOSXR.SeaShark.Tests")` (`Runtime/AssemblyInfo.cs`).
- `UIManager` moved from `UI/` to `Runtime/UI/`, so it is compiled into the `SOSXR.SeaShark` assembly when the package is installed through the Package Manager. The file GUID is unchanged.
- Android Runtime Permissions sample: the class `GetRuntimePermission` is renamed to `GetRuntimePermissions` to match its file name. Scenes that used the old component must add it again.
- `BuildInfoToUI` labels the build type as "ProductionBuild".

### Tests

- `Tests/EditMode/BuildContextTests.cs` covers every `BuildTriggerType` in Editor, development-player and production-player contexts.

## [0.7.2] - 2026-11-10

### Added

- `[Info]` now works on non-serialized and private fields. The help box is drawn below the default Inspector, above any `[Button]` methods, and prefixed with the field name. Classes with their own `[CustomEditor]` do not use `SOSXRBaseEditor` and do not show these boxes.
- `Editor/General/SerializedPropertyUtility.cs`, a shared helper used by `HidingDrawer` and `ShowIf`.
- `Tests/EditMode/AttributeDrawerTests.cs` (with `AttributeDrawerTestTargets.cs`) covers the attribute drawer fixes in this release.

### Changed

- Attributes now apply to a List or array as a whole, not to each element: `HorizontalLine`, `ReadOnlyGroupBegin`, `ReadOnlyGroupEnd`, `ReadOnly`, `DisableEditing`, `ShowIf`, `EnableIf`, `HideIf` (and its variants), `Required`, `LabelSearch`, and `Info`.
- `ReadOnly` and `DisableEditing` make the whole List read-only, with no resize or reorder. `ShowIf` and `HideIf` hide the whole List, header included. `EnableIf` shows one toggle for the List. `HorizontalLine` and `ReadOnlyGroup` decorators now draw on List fields.
- Behaviour change: `[Required]` on a List or array now means the List must not be empty. It used to check each element.

### Fixed

- `[Info]` on Lists and arrays now draws one help box above the whole collection instead of being ignored.
- `[ShowIf]` finds its condition field on base classes (including private fields) and inside nested serializable classes and List elements.
- `[Required]` flags an empty List or array and draws the whole List under the warning.
- `[LabelSearch]` array mode works. Search and foldout state is kept per object and field, so the first frame is no longer blank.
- `[Preview]` has the correct height on first draw. It no longer throws `NullReferenceException` when a Renderer has no material.
- Multi-object editing: `Angle`, `BoxRange`, `Time`, `TagSelector`, `Password`, and `LabelSearch` only write on a real edit and show mixed values. Selecting several objects no longer copies one object's value to all.
- `[Password]` draws non-string fields normally instead of hiding them.
- `[Optional]` label colour is readable in the light Editor theme.
- Removed an unused `using UnityEditor.PackageManager;` from the `InfoBoxExample` sample that broke player builds.

## [0.7.1]

### Fixed

- Dependency issue with DesignPatterns sample
- Dependency issue in PokeDeformation sample

## [0.7.0]

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

## [0.4.0]

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

## [0.3.0] - 2025-07-22

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

## [0.2.1] 2025-04-02

> ### Package Numbering Change
>
> #### Package will now be numbered starting with 0, to better reflect the current status in development (see the official semver information [here](https://semver.org/#spec-item-4))
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
