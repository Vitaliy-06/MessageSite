using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessageSiteAPI.Models.Dtos
{
    public class RegLogUserDto
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string email { get; set; }

        public required string password { get; set; }
    }
}
