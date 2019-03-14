using System;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IEnumerableAsyncAdapter<T> : IDisposable
    {
        bool HasRecords { get; }

        Task<T> Pull();
    }
}