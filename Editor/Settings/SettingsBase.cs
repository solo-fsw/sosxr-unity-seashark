namespace SOSXR.SeaShark
{
    public abstract class SettingsBase
    {
        public abstract void OnGUI();
    }

    public abstract class ProjectSettingsBase : SettingsBase
    {
    }

    public abstract class PreferencesBase : SettingsBase
    {
    }
}