using System.Threading.Tasks;

namespace Pada.Abstractions.Messaging
{
    public interface INotificationEventDispatcher
    {
        Task DispatchAsync(params INotificationEvent[] events);
    }
}