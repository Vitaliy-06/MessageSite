using MessageSiteAPI.Models;
using MessageSiteAPI.Models.Dtos;

namespace MessageSiteAPI.Services.Interfaces
{
    public interface IMessageInfoService
    {
        Task<MessageInfo> CreateMessage(CreateMessageDto message);
        Task<IEnumerable<MessageInfo>> GetMessagesBySenderIdAndReceiverId(string senderId, string receiverId);
        Task<IEnumerable<MessageInfo>> GetMessagesBySenderEmailAndReceiverEmail(string senderEmail, string receiverEmail);

    }
}
