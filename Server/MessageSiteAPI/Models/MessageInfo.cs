using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageSiteAPI.Models
{
    [Table("MessageInfo")]
    public class MessageInfo
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Sender is required.")]
        [ForeignKey("SenderId")]
        public required IdentityUser Sender { get; set; }


        [Required(ErrorMessage = "Recipient is required.")]
        [ForeignKey("RecipientId")]
        public required IdentityUser Recipient { get; set; }


        [Required(ErrorMessage = "Message content is required.")]
        [StringLength(500, ErrorMessage = "Message content cannot exceed 500 characters.")]
        public required string Content { get; set; }


        [Required(ErrorMessage = "Timestamp is required.")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;


        [Required(ErrorMessage = "IsRead status is required.")]
        public bool IsRead { get; set; } = false;

    }
}
