namespace PetaPoco.Core.Inflection
{
    /// <summary>
    /// Static inflection singleton helper.
    /// </summary>
    public static class Inflector
    {
        private static IInflector _inflector;

        /// <summary>
        /// Gets or sets the <see cref="IInflector"/> instance.
        /// Default is <see cref="EnglishInflector"/>.
        /// </summary>
        /// <remarks>
        /// Set to <see langword="null"/> to restore the default <see cref="EnglishInflector"/>.
        /// </remarks>
        /// <value>The currently set <see cref="IInflector"/> instance.</value>
        /// <seealso cref="EnglishInflector"/>
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
