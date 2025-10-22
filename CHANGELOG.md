# Changelog

All notable changes to this project will be documented in this file.
The changelog format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [0.4.0] - In Progress

### Added

### Removed

- Removed SettingProvider (will now exclusively live in EditorSpice). You can register a new one from SeaShark (or wherever) if you want though, see Samples.

### Changed

- ButtonAttribute now works with values you set for parameters
- ButtonAttribute has option for space and/or horizontal line above.

### Fixed

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

