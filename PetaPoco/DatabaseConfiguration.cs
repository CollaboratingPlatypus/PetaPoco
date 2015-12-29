// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/27</date>

using System;
using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    ///     A helper class which enables fluent configuration.
    /// </summary>
    public class DatabaseConfiguration : IDatabaseBuildConfiguration, DatabaseConfiguration.IBuildConfigurationSettings, IHideObjectMethods
    {
        private readonly IDictionary<string, object> _settings = new Dictionary<string, object>();

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

        /// <summary>
        ///     Private constructor to force usage of static build method.
        /// </summary>
        private DatabaseConfiguration()
        {
        }

        /// <summary>
        ///     Starts a new PetaPoco build configuration.
        /// </summary>
        /// <returns>An instance of <see cref="IDatabaseBuildConfiguration" /> to form a fluent interface.</returns>
        public static IDatabaseBuildConfiguration Build()
        {
            return new DatabaseConfiguration();
        }

        void IBuildConfigurationSettings.SetSetting(string key, object value)
        {
            // Note: no argument checking because, pref, enduser unlikely and handled by RT/FW
            if (value != null)
                _settings[key] = value;
            else
                _settings.Remove(key);
        }

        void IBuildConfigurationSettings.TryGetSetting<T>(string key, Action<T> setSetting, Action onFail = null)
        {
            // Note: no argument checking because, pref, enduser unlikely and handled by RT/FW
            object setting;
            if (_settings.TryGetValue(key, out setting))
                setSetting((T) setting);
            else if (onFail != null)
                onFail();
        }
    }
}