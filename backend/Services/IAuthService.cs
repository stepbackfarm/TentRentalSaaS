using System.Threading.Tasks;
using TentRentalSaaS.Api.DTOs;

namespace TentRentalSaaS.Api.Services
{
    public interface IAuthService
    {
        Task RequestLoginLinkAsync(string email);
        Task<PortalDataDto?> VerifyLoginTokenAsync(string token);
    }
}
