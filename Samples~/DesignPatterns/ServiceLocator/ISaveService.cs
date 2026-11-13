namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Service interface for persistence. Implementations can target PlayerPrefs, JSON files, cloud saves, or encrypted platform storage.
    /// </summary>
    public interface ISaveService
    {
        /// <summary>
        ///     Saves an integer value for the provided key.
        /// </summary>
        /// <param name="key">The unique persistence key.</param>
        /// <param name="value">The value to store.</param>
        void SaveInt(string key, int value);


        /// <summary>
        ///     Loads an integer value for the provided key.
        /// </summary>
        /// <param name="key">The unique persistence key.</param>
        /// <param name="defaultValue">The value returned when no saved value exists.</param>
        /// <returns>The stored value or <paramref name="defaultValue"/> when absent.</returns>
        int LoadInt(string key, int defaultValue = 0);
    }
}
