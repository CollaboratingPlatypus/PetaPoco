namespace PetaPoco.Core.Inflection
{
    /// <summary>
    ///     Static inflection helper
    /// </summary>
    public static class Inflector
    {
        private static IInflector _inflector;

        /// <summary>
        ///     Gets or sets the <see cref="IInflector" /> instacne.
        /// </summary>
        /// <param name="value">
        ///     The inflector to set as the default instance, or null to restore the default
        ///     <see cref="EnglishInflector" />.
        /// </param>
        /// <remarks>
        ///     By default the <see cref="EnglishInflector" /> instance used.
        /// </remarks>
        /// <returns>
        ///     The currently set <see cref="IInflector" /> instance.
        /// </returns>
        public static IInflector Instance
        {
            get { return _inflector; }
            set { _inflector = value ?? new EnglishInflector(); }
        }

        static Inflector()
        {
            _inflector = new EnglishInflector();
        }
    }
}