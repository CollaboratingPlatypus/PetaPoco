using System.Data;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IConnection
    {
        /// <summary>
        ///     When set to true the first opened connection is kept alive until <see cref="CloseSharedConnection" />
        ///     or <see cref="Dispose" /> is called.
        /// </summary>
        /// <seealso cref="OpenSharedConnection" />
        bool KeepConnectionAlive { get; set; }

        /// <summary>
        ///     Provides access to the currently open shared connection.
        /// </summary>
        /// <returns>
        ///     The currently open connection, or <c>Null</c>.
        /// </returns>
        /// <seealso cref="OpenSharedConnection" />
        /// <seealso cref="CloseSharedConnection" />
        /// <seealso cref="KeepConnectionAlive" />
        IDbConnection Connection { get; }

        /// <summary>
        ///     Opens a connection that will be used for all subsequent queries.
        /// </summary>
        /// <remarks>
        ///     Calls to <see cref="Database.OpenSharedConnection" />/<see cref="Database.CloseSharedConnection" /> are reference
        ///     counted and should be balanced
        /// </remarks>
        /// <seealso cref="Database.Connection" />
        /// <seealso cref="Database.CloseSharedConnection" />
        /// <seealso cref="Database.KeepConnectionAlive" />
        void OpenSharedConnection();

#if !NET40
        /// <summary>
        ///     The async version of <see cref="Database.OpenSharedConnection" />.
        /// </summary>
        Task OpenSharedConnectionAsync();
#endif

        /// <summary>
        ///     Releases the shared connection.
        /// </summary>
        /// <remarks>
        ///     Calls to <see cref="Database.OpenSharedConnection" />/<see cref="Database.CloseSharedConnection" /> are reference
        ///     counted and should be balanced
        /// </remarks>
        /// <seealso cref="Database.Connection" />
        /// <seealso cref="Database.OpenSharedConnection" />
        /// <seealso cref="Database.KeepConnectionAlive" />
        void CloseSharedConnection();
    }
}