using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineHub.Web.Services.Users;

namespace VaccineHub.Web.Endpoints.ApiUser
{
    public sealed class ApiUserController : ControllerBase
    {
        private readonly IApiUsersDataProvider _apiUsersDataProvider;

        public ApiUserController([NotNull] IApiUsersDataProvider apiUsersDataProvider)
        {
            _apiUsersDataProvider = apiUsersDataProvider ?? throw new ArgumentNullException(nameof(apiUsersDataProvider));
        }

        #region Create
 
        /// <summary>
        ///  Add Api User to Vaccine Hub Database
        /// </summary>
        [HttpPost]
        [Authorize]
        [Route("ApiUser")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AddUserAsync([FromBody] Services.Users.Models.ApiUser user, CancellationToken token)
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
        [Authorize(Roles = "Admin")]
        [Route("ApiUser('{emailId}')")]
        [ProducesResponseType(typeof(Services.Users.Models.ApiUser), 200)]
        public async Task<IActionResult> GetUserAsync(string emailId, CancellationToken token)
        {
            var user = await _apiUsersDataProvider.GetUserAsync(emailId, token);

            return Ok(user);
        }

        /// <summary>
        ///  Retrieve all Api users from Vaccine Hub database
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("ApiUsers")]
        [ProducesResponseType(typeof(IDictionary<string, Services.Users.Models.ApiUser>), 200)]
        public async Task<IActionResult> GetUsersAsync(CancellationToken token)
        {
            var users = await _apiUsersDataProvider.GetUsersAsync(token);

            return Ok(users ?? new Dictionary<string, Services.Users.Models.ApiUser>());
        }

        #endregion

        #region Update

        /// <summary>
        ///  Update Api user from Vaccine Hub Database
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("ApiUser)")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateApiUserAsync([FromBody] Services.Users.Models.ApiUser user, CancellationToken token)
        {
            try
            {
                await _apiUsersDataProvider.UpdateApiUserAsync(user, token);

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