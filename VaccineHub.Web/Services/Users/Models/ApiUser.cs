using JetBrains.Annotations;
using VaccineHub.Web.Services.Users.Types;

namespace VaccineHub.Web.Services.Users.Models
{
    public class ApiUser
    {
        /// <summary>The unique identifier</summary>
        public string EmailId { get; [UsedImplicitly] set; }
        public string Password { get; [UsedImplicitly] set; }
        public bool IsActive { get; [UsedImplicitly] set; }
        public UserType? UserType { get; [UsedImplicitly] set; }
    }
}