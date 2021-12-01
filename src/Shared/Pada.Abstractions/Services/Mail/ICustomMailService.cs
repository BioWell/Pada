using System.Threading.Tasks;

namespace Pada.Abstractions.Services.Mail
{
    public interface ICustomMailService
    {
        Task SendAsync(CustomMailRequest request);
    }
}