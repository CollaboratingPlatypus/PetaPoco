// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/10</date>

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
        ///     Tries to get the setting and calls the <paramref name="setSetting" /> to set the value if found.
        /// </summary>
        /// <typeparam name="T">The setting type.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="setSetting">The set setting callback.</param>
        /// <param name="onFail">The on fail callback, called when no setting can be set.</param>
        void TryGetSetting<T>(string key, Action<T> setSetting, Action onFail = null);
    }
}