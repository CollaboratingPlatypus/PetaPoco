using System;

namespace PetaPoco
{
    /// <summary>
    /// Represents the build configuration settings contract.
    /// </summary>
    public interface IBuildConfigurationSettings
    {
        /// <summary>
        /// Sets a setting with a specified key that can be used for future retrieval of the setting's value.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        void SetSetting(string key, object value);

        /// <summary>
        /// Attempts to locate a setting of type <typeparamref name="T"/> using the specified <paramref name="key"/>, and invokes <paramref
        /// name="onGetAction"/> with the setting's value if found.
        /// </summary>
        /// <typeparam name="T">The type of the setting's value object.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="onGetAction">The action to invoke with the setting's value when the setting is found.</param>
        /// <param name="onFailAction">An optional action to invoke if the setting cannot be found.</param>
        void TryGetSetting<T>(string key, Action<T> onGetAction, Action onFailAction = null);
    }
}
