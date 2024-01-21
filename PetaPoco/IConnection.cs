using System.Data;
using System.Threading.Tasks;

namespace PetaPoco
{
    /// <summary>
    /// Defines methods and properties for managing database connections. This includes opening and closing shared connections, and
    /// accessing the currently open connection.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Gets or sets the connection reuse policy for the shared connection or <see cref="IDatabase"/> instance.
        /// </summary>
        /// <remarks>
        /// When set to <see langword="true"/> the first opened connection is kept alive until <see cref="CloseSharedConnection"/> is called
        /// or the <see cref="IDatabase"/> is disposed.
        /// </remarks>
        bool KeepConnectionAlive { get; set; }

        /// <summary>
        /// Gets the currently open shared connection, or <see langword="null"/> if there is no open connection.
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="OpenSharedConnection"/> and <see cref="CloseSharedConnection"/> are reference counted and must be balanced.
        /// </remarks>
        void OpenSharedConnection();

#if ASYNC
        /// <summary>
        /// Asynchronously opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="OpenSharedConnectionAsync()"/> and <see cref="CloseSharedConnection"/> are reference counted and must be
        /// balanced.
        /// </remarks>
        /// <returns>
		/// A task that represents the asynchronous operation.
		/// </returns>
        Task OpenSharedConnectionAsync();

        // TODO: Missing overload: `Task OpenSharedConnectionAsync(CancellationToken)`
#endif

        /// <summary>
        /// Releases the shared connection.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="OpenSharedConnection"/> and <see cref="CloseSharedConnection"/> are reference counted and must be balanced.
        /// </remarks>
        void CloseSharedConnection();
    }
}
