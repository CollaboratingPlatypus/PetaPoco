using System;
using System.Threading.Tasks;

namespace PetaPoco
{
    /// <summary>
    /// Specifies a set of methods for asynchronously reading data as POCO objects from a data source.
    /// </summary>
    /// <typeparam name="T">The POCO type representing a single result record.</typeparam>
    public interface IAsyncReader<out T> : IDisposable
    {
        /// <summary>
        /// Gets the current POCO object of type <typeparamref name="T"/> that the reader is positioned at.
        /// </summary>
        T Poco { get; }

        /// <summary>
        /// Asynchronously reads the next row from the data source.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if more records exist, otherwise <see langword="false"/>.</returns>
        Task<bool> ReadAsync();

        // TODO: Missing overload: `Task<bool> ReadAsync(CancellationToken)`
    }
}
