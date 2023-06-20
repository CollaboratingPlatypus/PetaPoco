using System;
using System.Threading.Tasks;

namespace PetaPoco
{
    /// <summary>
    /// Defines methods for asynchronously reading data as POCO objects from a data source.
    /// </summary>
    /// <typeparam name="T">The type of POCO object to read from the data source.</typeparam>
    public interface IAsyncReader<out T> : IDisposable
    {
        /// <summary>
        /// Gets the current POCO object of type <typeparamref name="T"/> that the reader is positioned at.
        /// </summary>
        T Poco { get; }

        /// <summary>
        /// Asynchronously reads the next row from the data source.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation. The task result contains <see langword="true"/> if more records exist; otherwise, <see langword="false"/>.</returns>
        Task<bool> ReadAsync();
    }
}
