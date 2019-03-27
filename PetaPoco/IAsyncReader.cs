using System;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IAsyncReader<out T> : IDisposable
    {
        T Poco { get; }

        Task<bool> ReadAsync();
    }
}