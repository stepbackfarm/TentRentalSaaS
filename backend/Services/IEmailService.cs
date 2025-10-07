using System.Threading.Tasks;

namespace TentRentalSaaS.Api.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
