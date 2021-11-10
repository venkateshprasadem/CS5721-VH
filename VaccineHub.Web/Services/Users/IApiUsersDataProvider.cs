using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VaccineHub.Web.Services.Users.Models;

namespace VaccineHub.Web.Services.Users
{
    public interface IApiUsersDataProvider
    {
        Task AddUserAsync(ApiUser apiUser, CancellationToken cancellationToken);
        Task<ApiUser> GetUserAsync(string emailId, CancellationToken cancellationToken);
        Task<IDictionary<string, ApiUser>> GetUsersAsync(CancellationToken cancellationToken);
        Task UpdateApiUserAsync(ApiUser apiUser, CancellationToken token);
    }
}