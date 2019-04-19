using System;

namespace PetaPoco
{
    /// <summary>
    ///     Represents the build configuration settings contract.
    /// </summary>
    public interface IBuildConfigurationSettings
    {
        /// <summary>
        ///     Sets the setting against the specified key.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        void SetSetting(string key, object value);

        /// <summary>
        ///     Tries to get the setting and calls the <paramref name="getSetting" /> to set the value if found.
        /// </summary>
        /// <typeparam name="T">The setting type.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="getSetting">The get setting callback.</param>
        /// <param name="onFail">The on fail callback, called when no setting can be gotten.</param>
        void TryGetSetting<T>(string key, Action<T> getSetting, Action onFail = null);
    }
}