using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Web.Services.Users.Models;

namespace VaccineHub.Web.Services.Users
{
    public sealed class ApiUsersDataProvider : IApiUsersDataProvider
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        public ApiUsersDataProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ApiUser> GetUserAsync(string emailId, CancellationToken cancellationToken)
        {
            var users = await GetUsersAsync(cancellationToken);

            return users.TryGetValue(emailId, out var value)
                ? value
                : null;
        }

        public async Task<IDictionary<string, ApiUser>> GetUsersAsync(CancellationToken cancellationToken)
        {
            return await GetApiUsers(cancellationToken);
        }

        private async Task<IDictionary<string, ApiUser>> GetApiUsers(CancellationToken cancellationToken)
        {
            List<Persistence.Entities.ApiUser> list;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

                list = await dbContext.ApiUsers
                    .ToListAsync(cancellationToken);
            }

            return list.ToDictionary(
                apiUser => apiUser.EmailId,
                Mapper.Map<ApiUser>);
        }

        public async Task AddUserAsync(ApiUser apiUser, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var existingUser = await dbContext.ApiUsers.FindAsync(new object[] {apiUser.EmailId}, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Api User already exists");
            }

            var dbApiUser = Mapper.Map<Persistence.Entities.ApiUser>(apiUser);
            dbContext.ApiUsers.Add(dbApiUser);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateApiUserAsync(ApiUser apiUserToUpdate, CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            var user = await dbContext.ApiUsers.FindAsync(new object[] {apiUserToUpdate.EmailId}, token);

            if (user == null)
            {
                throw new Exception("apiUserId not found");
            }

            var dbApiUserToUpdate = Mapper.Map<ApiUser>(apiUserToUpdate);

            user.IsActive = dbApiUserToUpdate.IsActive;
            user.Password = dbApiUserToUpdate.Password;

            await dbContext.SaveChangesAsync(token);
        }

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<ApiUser, Persistence.Entities.ApiUser>()
                    .ForMember(dest => dest.EmailId, opt => opt.MapFrom(src => src.EmailId))
                    .ReverseMap();
            });
        }
    }
}