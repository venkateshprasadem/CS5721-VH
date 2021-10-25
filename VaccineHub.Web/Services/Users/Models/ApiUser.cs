namespace VaccineHub.Web.Services.Users.Models
{
    public class ApiUser
    {
        /// <summary>The unique identifier</summary>
        public string EmailId { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public UserType UserType { get; set; }
    }
}