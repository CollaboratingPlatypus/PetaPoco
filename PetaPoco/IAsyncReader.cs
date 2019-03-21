using System;
using System.Threading.Tasks;

namespace PetaPoco
{
    public interface IAsyncReader<out T> : IDisposable
    {
        Task<bool> ReadAsync();
        
        T Poco { get; }
    }
}