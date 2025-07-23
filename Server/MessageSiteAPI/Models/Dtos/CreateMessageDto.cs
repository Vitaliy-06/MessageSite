namespace MessageSiteAPI.Models.Dtos
{
    public class CreateMessageDto
    {
        public string SenderId { get; set; }
        public required string ReceiverEmail { get; set; }
        public required string Content { get; set; }
        public required bool isRead { get; set; } = false;
    }
}
