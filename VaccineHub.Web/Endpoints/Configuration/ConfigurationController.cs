using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineHub.Web.Services.Users;
using VaccineHub.Web.Services.Users.Models;

namespace VaccineHub.Web.Endpoints.Configuration
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public sealed class ConfigurationController : ControllerBase
    {
        private readonly IApiUsersDataProvider _apiUsersDataProvider;

        public ConfigurationController([NotNull] IApiUsersDataProvider apiUsersDataProvider)
        {
            _apiUsersDataProvider = apiUsersDataProvider ?? throw new ArgumentNullException(nameof(apiUsersDataProvider));
        }

        #region Create
 
        /// <summary>
        ///  Add Api User to Vaccine Hub Database
        /// </summary>
        [HttpPost]
        [Route("ApiUser")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AddUserAsync([FromBody] ApiUser user, CancellationToken token)
        {
            await _apiUsersDataProvider.AddUserAsync(user, token);

            return NoContent();
        }

        #endregion

        #region Read

        /// <summary>
        ///  Retrieve Api user from Vaccine Hub database
        /// </summary>
        [HttpGet]
        [Route("ApiUser('{emailId}')")]
        [ProducesResponseType(typeof(ApiUser), 200)]
        public async Task<IActionResult> GetUserAsync(string emailId, CancellationToken token)
        {
            var user = await _apiUsersDataProvider.GetUserAsync(emailId, token);

            return Ok(user);
        }

        /// <summary>
        ///  Retrieve all Api users from Vaccine Hub database
        /// </summary>
        [HttpGet]
        [Route("ApiUsers")]
        [ProducesResponseType(typeof(IDictionary<string, ApiUser>), 200)]
        public async Task<IActionResult> GetUsersAsync(CancellationToken token)
        {
            var users = await _apiUsersDataProvider.GetUsersAsync(token);

            return Ok(users ?? new Dictionary<string, ApiUser>());
        }

        #endregion

        #region Update

        /// <summary>
        ///  Update Api user from Vaccine Hub Database
        /// </summary>
        [HttpPut]
        [Route("ApiUser('{apiUserId}')")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateApiUserAsync(string apiUserId, [FromBody] ApiUser user, CancellationToken token)
        {
            try
            {
                await _apiUsersDataProvider.UpdateApiUserAsync(apiUserId, user, token);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        #endregion
    }
}