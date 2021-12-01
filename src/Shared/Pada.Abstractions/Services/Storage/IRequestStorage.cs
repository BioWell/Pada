using System.Threading.Tasks;

namespace Pada.Abstractions.Services.Storage
{
    /// <summary>
    /// short term request session for request - 5 second
    /// </summary>
    public interface IRequestStorage
    {
        Task Set<T>(string key, T value);
        Task<T> Get<T>(string key);
    }
}