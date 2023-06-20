using System;

namespace PetaPoco
{
    /// <summary>
    /// Represents the build configuration settings contract.
    /// </summary>
    public interface IBuildConfigurationSettings
    {
        /// <summary>
        /// Sets the setting against the specified key.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        void SetSetting(string key, object value);

        /// <summary>
        /// Attempts to locate a setting of type <typeparamref name="T" /> using the provided <paramref name="key" />, calling the <paramref name="getSetting" /> callback with the setting's value if found.
        /// </summary>
        /// <typeparam name="T">The setting type.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="getSetting">The get setting callback.</param>
        /// <param name="onFail">The on fail callback, if set, that is called if the setting cannot be found.</param>
        void TryGetSetting<T>(string key, Action<T> getSetting, Action onFail = null);
    }
}
