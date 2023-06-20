using System.Data;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IConnection
    {
        /// <summary>
        /// Gets or sets the connection reuse policy for the shared connection or <see cref="IDatabase"/> instance.
        /// </summary>
        /// <remarks>
        /// When set to <see langword="true"/> the first opened connection is kept alive until <see cref="CloseSharedConnection" /> is called or the <see cref="IDatabase" /> is disposed.
        /// </remarks>
        /// <seealso cref="OpenSharedConnection" />
        bool KeepConnectionAlive { get; set; }

        /// <summary>
        /// Gets the currently open shared connection, or <see langword="null"/> if there is no open connection.
        /// </summary>
        /// <seealso cref="KeepConnectionAlive" />
        /// <seealso cref="OpenSharedConnection" />
        /// <seealso cref="CloseSharedConnection" />
        IDbConnection Connection { get; }

        /// <summary>
        /// Opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="Database.OpenSharedConnection" /> and <see cref="Database.CloseSharedConnection" /> are reference counted and should be balanced.
        /// </remarks>
        /// <seealso cref="Database.Connection" />
        /// <seealso cref="Database.KeepConnectionAlive" />
        /// <seealso cref="Database.CloseSharedConnection" />
        void OpenSharedConnection();

#if !NET40
        /// <summary>
        /// The asyncronous version of <see cref="Database.OpenSharedConnection" />.
        /// </summary>
        Task OpenSharedConnectionAsync();
#endif

        /// <summary>
        /// Releases the shared connection.
        /// </summary>
        /// <remarks>
        /// Calls to <see cref="Database.OpenSharedConnection" /> and <see cref="Database.CloseSharedConnection" /> are reference counted and should be balanced.
        /// </remarks>
        /// <seealso cref="Database.Connection" />
        /// <seealso cref="Database.KeepConnectionAlive" />
        /// <seealso cref="Database.OpenSharedConnection" />
        void CloseSharedConnection();
    }
}
