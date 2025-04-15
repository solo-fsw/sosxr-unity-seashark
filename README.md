# SeaShark

**Our very own C# enhancements library**

- By: Maarten R. Struijk Wilbrink
- For: Leiden University SOSXR
- Fully open source: Feel free to add to, or modify, anything you see fit.

## Attribution

A huge thanks goes to two channels who are creating incredible content:
- [Warped Imagination](https://www.youtube.com/@WarpedImagination)
- [git-amend](https://www.youtube.com/@git-amend) on YouTube and also his [repo](https://github.com/adammyhre/Unity-Utils)
  
A lot of the good work shown in this repo is either a direct copy of their work, or a heavily 'inspired' version of it.

Quite a few of the components here are __not__ by my design. They're all Open Source, and where possible I tried to attribute to the original author. If you see something that you believe is yours, please let me know, and I'll be happy to add you to the list of contributors.

### Distinction between SeaShark and EditorSpice

When using [EditorSpice](https://github.com/solo-fsw/sosxr-unity-editorspice) in your project, none of your actual 'game-code' should be affected. These are tools to make the Editor behave in (marginally) more useful ways, but will not / should not be embedded in your actual program. You should be able to delete the entire EditorSpice folder / package without affecting your game in any way. The Spice aims for zero errors, zero null references, and zero warnings.ß

[SeaShark](https://github.com/solo-fsw/sosxr-unity-seashark) however is a library of patterns and attributes that you can use in your project. They're designed to be embedded in your project, and to be used by your code. Similarly to EditorSpice they're designed to make your project better, and to make your life easier. However, you cannot use SeaShark and then delete it without creating tons of pretty red lines in your console.

## Installation

1. Open the Unity project you want to install this package in.
2. Open the Package Manager window.
3. Click on the `+` button and select `Add package from git URL...`.
4. Paste the URL of this repo into the text field and press `Add`. Make sure it ends with `.git`.


# Additional Unity Events

# Button (for Methods)

# Config Data

1. Create a ScriptableObject which holds your data by deriving from `BaseConfigData`. See `DemoConfigData` as an example.
    1. You should have only one (derived) `ConfigData` in use.
2. Create a new instance of your justly created asset by right-clicking in the Project window and selecting the same path as you set in the `[CreateAssetMenu(...)` section of the ScriptableObject (e.g. `Create > SOSXR > Config Data > Demo Config Data`).
3. Create a JSON file by hitting the `CreateNewConfigJsonFile` button. See the console to find out where it has been created. You can hit the `Reveal in Finder` button too.
4. To edit the data:
    1. Either edit the `.json` directly, and hit `LoadConfigFromJson` when you're done
    2. Edit the fields in the Inspector, and hit `UpdateConfigJson`
5. If you are editing the JSON directly, hit `LoadConfigFromJson` once you've made changes in the JSON.

## Best practices

Use a `[SerializeField] private` backing-field with a `public` property for your data. Have the `SetValue` method linked in the `set` portion like so:

```csharp
[SerializeField] private string m_baseURL = "https://youtu.be/xvFZjo5PgG0?si=F3cJFXtwofUAeA";
public string BaseURL
{
    get => m_baseURL;
    set => SetValue(ref m_baseURL, value, nameof(BaseURL));
}
```

## Responding to Data (changes) in the Config

Use any of the `ConfigXXXToUnityEvent` classes to pipe through any of the data from the `ConfigData` to a UnityEvent. This will then pass along that info to whatever component you like.

## Writing to JSON

You can list which variable changes will trigger a JSON update:

``` json
"m_updateJsonOnSpecificValueChanged": [
   "QueryStringURL",
   "PPN"
],
```

In this case the JSON gets rewrit when QueryStringURL changes or the PPN changes.

In the Editor you can use the checkboxes to add or remove variables that should trigger this update.

(These functions leverage the [PubSub](#pubsub-) system mentioned below)

----

While in the Editor, the OnValidate should pick up on any changes to the ScriptableObject, if you name your fields and properties correctly (see below). If it doesn't work, you have to hit the `UpdateConfigJson` button to save your changes to disk.

The DemoConfigData's OnValidate and the corresponding auto-storing of the JSON to disk when changing values in the Inspector works only when:
- Your `[SerializeField] private ...` backing-field is named starting with "m_" and in camelCase (e.g. `m_likeThis`)
- Your corresponding `public` property is named the exact same name, but without te "m_", and in PascalCase (e.g.
  `LikeThis`)
  This issue doesn't exist when writing directly to the `public` property.

## PubSub

Register that you want to update the Json when a value changes. This way, any time you change any of the properties of the (derived) `ConfigData` class, those changes will automatically get stored into the JSON on disk. See `DemoConfigData` for more examples.

### (Un)subscribe to specific value chance

``` csharp
private void OnEnable()
{
    configData.Subscribe(nameof(configData.ShowDebug), _ => RespondToNotification());
}

private void RespondToNotification()
{
    Debug.LogFormat("ShowDebug changed to {0}", configData.ShowDebug);
}

private void OnDisable()
{
    configData.Unsubscribe(nameof(configData.ShowDebug), _ => RespondToNotification());
}
```

### (Un)subscribe to any value change

```csharp
private void OnEnable()
{
    configData.SubscribeToAny(OnAnyValueChanged);
}

private void OnAnyValueChanged(string propertyName, object newValue)
{
    Debug.Log($"{propertyName} changed to: {newValue}");
}

private void OnDisable()
{
    configData.UnsubscribeFromAny(OnAnyValueChanged);
}
```

A similar thing is done in the `OnValidate` on the `BaseConfigData` class: after any change (in the Inspector for example) to any field that's in the `m_updateJsonOnSpecificValueChanged` list, the `HandleConfigData.UpdateConfigJson(this);` kicks in and will save your changes to disk (but see [Bonus](#bonus)).

# Design Patterns

## Command

## Mediator

# Extension Methods

# Fader

# Fonts
## LinBiolineum
## Maple Mono
> By subframe7536

Downloaded from [GitHub](https://github.com/subframe7536/maple-font?tab=readme-ov-file#install)

Imported as a [TMPro](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html) font asset using this [tutorial](https://www.youtube.com/watch?v=EV4wFb78FFs).

### Usage in Unity

Use the 'Maple Mono' font asset in any text field. It will automatically grab the italic font if you hit the 'I' in the textmeshpro component.
If you'd rather always use the italic version, you can drag that one into the TMPro field too, but this shouldn't be necessary.

# Interface Support
By [TheDudeFromCI](https://github.com/TheDudeFromCI/Unity-Interface-Support/?tab=readme-ov-file).

# General Attributes

## BoxRangeAttribute

The BoxRangeDrawer is a custom property drawer for Unity that provides a user-friendly way to input and restrict values for fields marked with `[BoxRange]`. It supports int, float, Vector2, Vector3, and Vector3Int types, displaying a slider in the Unity Inspector to enforce that values remain within a specified range. This tool enhances the editor experience by visually representing the allowed range for properties, ensuring that values stay within defined bounds without additional validation logic. The drawer is especially useful for developers who need to constrain numeric or vector inputs directly within the Unity editor.

## DecimalAttribute

The DecimalsDrawer is a custom property drawer for Unity that rounds float properties to a specified number of decimal places in the Unity Inspector, based on the DecimalsAttribute. Mark a value with `[Decimal]` It displays the allowed precision and ensures that the values entered are automatically rounded to the defined decimal precision, using MidpointRounding.AwayFromZero. Currently, it supports rounding for float properties and provides placeholder messages for unsupported types like Vector2 and Vector3. This drawer simplifies the process of managing decimal precision in float values directly within the Unity editor.

MidpointRounding.AwayFromZero is used to ensure that values are rounded to the nearest even number when they fall exactly between two integers. This rounding method is commonly used in financial applications and is the default behavior for the Math.Round method in C#.

## DisableEditingAttribute

The DisableEditingPropertyDrawer is a custom property drawer for Unity that disables editing of fields marked with `[DisableEditing]`. When applied, it renders the property as read-only, preventing users from modifying its value while still displaying it in the Inspector. This drawer is useful for situations where a property needs to be visible but should not be editable, such as when the value is controlled by other systems or scripts.

## HelpAttribute (John Earnshaw, reblGreen Software Limited)

## HideIf Attribute (based on BasteRainGames)

## Horizontal Line Attribute (by Warped Imagination)

## InfoAttribute (by Warped Imagination)

The InfoDrawer is a custom property drawer for Unity that displays informational text in the Unity Inspector for fields marked with `[Info]`. When applied, it shows a help box with a message specified by the attribute, without rendering the actual property field. This drawer is useful for providing additional context or instructions to developers or designers using the Inspector. Set the MessageType (None, Info, Warning, Error) if so desired, otherwise it defaults to None.

## Optional

To let future people know that the variable is optional, and that you don't necessarily need to set it. It has options to mark the field as "Will Find", "Will Get", and "Will Add" as well. These indicate that they may be blank in the Inspector right now, but that you will (in your own code) will get the values later, either through a Find somewhere in the scene, a GetComponent-type action, or an AddComponent.


## PreviewDrawer

The PreviewDrawer is a custom property drawer for Unity that allows fields marked with `[Preview]` to display a visual preview of certain object types in the Unity Inspector. It supports previews for Texture, Material, Sprite, and GameObject types, displaying the associated texture or material on the Inspector when these objects are assigned to the field. The drawer automatically adjusts the height of the preview based on the attribute's specified height, making it useful for providing immediate visual feedback for assets like textures or materials without the need to open them separately.

## ReadOnlyAttribute (BeginReadOnlyGroup, EndReadOnlyGroup)

The ReadOnlyDrawer, BeginReadOnlyGroupDrawer, and EndReadOnlyGroupDrawer are custom property drawers for Unity that allow you to mark individual properties or groups of properties as read-only in the Inspector. The ReadOnlyDrawer disables a single property marked with `[ReadOnly]`, ensuring that the field remains visible but uneditable. The `[BeginReadOnlyGroup]` and `[EndReadOnlyGroup]` are used to define a block of properties that are rendered as read-only within a group, disabling editing for all properties between these two markers. This is useful when you want to prevent users from modifying certain properties while still allowing them to view the values.

## Required Attribute (from Warped Imagination)

## Suffix Attribute

## TagSelectorAttribute (by Dylan Engelman and Brecht Lecluyse)

The TagSelectorPropertyDrawer is a custom property drawer for Unity that allows you to select tags from a dropdown menu for fields marked with `[TagSelector]`. If the UseDefaultTagFieldDrawer option is enabled, it uses Unity's default tag selector. Otherwise, it generates a custom tag list, including a "<NoTag>" option, allowing the user to assign or clear tags from the property. This drawer simplifies tag selection by offering a dropdown of all available tags, ensuring that string properties can only be assigned valid tag values, which is especially useful in managing tagging systems within Unity projects.

## TimeAttribute

The TimeAttribute is a custom property drawer for Unity that formats an integer value representing time (in seconds)into a human-readable format, such as hours, minutes, and seconds, for fields marked with the TimeAttribute. If the DisplayHours option in the TimeAttribute is enabled, the time is shown in a hh:mm:ss format; otherwise, it is displayed in a mm:ss format. It displays both the raw integer input field and the formatted time underneath it. If the property is not an integer, it displays an error message. This drawer is useful for managing and visualizing time-based properties in the Unity Inspector.

# In Build

## In Production Build

## Destroy in Build

# Interfaces

## InterfaceAttribute

# Move (Work in Progress)

# Object Cue

# Query Strings

# Simple Helpers

Some MonoBehaviours and Static classes that give some useful functionality. Notable inclusions are:

- DDOL
- FindGameObjectsWithTag
- SetMainCameraAsCanvasWorldCamera

# Video Player
