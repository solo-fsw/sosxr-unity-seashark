using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     <b>What:</b> PlayerPrefs-backed implementation of <see cref="ISaveService"/>.
    ///     <b>Why:</b> Provides a simple persistence backend for sample scenarios without requiring file I/O setup.
    ///     <b>How:</b> Delegates integer reads/writes to <see cref="PlayerPrefs"/> and immediately flushes saves to storage.
    /// </summary>
    public class PlayerPrefsSaveService : ISaveService
    {
        /// <summary>
        ///     Saves an integer value to PlayerPrefs using the provided key.
        /// </summary>
        /// <param name="key">The unique persistence key.</param>
        /// <param name="value">The value to store.</param>
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }


        /// <summary>
        ///     Loads an integer value from PlayerPrefs using the provided key.
        /// </summary>
        /// <param name="key">The unique persistence key.</param>
        /// <param name="defaultValue">The value returned when no saved value exists.</param>
        /// <returns>The stored value or <paramref name="defaultValue"/> when absent.</returns>
        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }
}
