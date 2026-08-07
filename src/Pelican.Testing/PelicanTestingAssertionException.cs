namespace Pelican.Testing
{
    /// <summary>
    /// Represents an assertion failure produced by Pelican testing helpers.
    /// </summary>
    public sealed class PelicanTestingAssertionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PelicanTestingAssertionException"/> class.
        /// </summary>
        /// <param name="message">The assertion failure message.</param>
        public PelicanTestingAssertionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PelicanTestingAssertionException"/> class.
        /// </summary>
        /// <param name="message">The assertion failure message.</param>
        /// <param name="innerException">The exception that caused this assertion failure.</param>
        public PelicanTestingAssertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
