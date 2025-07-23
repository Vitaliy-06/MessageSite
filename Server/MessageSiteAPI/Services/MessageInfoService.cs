using MessageSiteAPI.Models;
using MessageSiteAPI.Models.Dtos;
using MessageSiteAPI.Repositories.Interfaces;
using MessageSiteAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MessageSiteAPI.Services
{
    public class MessageInfoService : IMessageInfoService
    {
        private readonly IMessageInfoRepository _messageInfoRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public MessageInfoService(IMessageInfoRepository messageInfoRepository, UserManager<IdentityUser> userManager)
        {
            _messageInfoRepository = messageInfoRepository;
            _userManager = userManager;
        }

        public async Task<MessageInfo> CreateMessage(CreateMessageDto message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null");
            }

            var sender = await _userManager.FindByIdAsync(message.SenderId.ToString());
            var receiver = await _userManager.FindByEmailAsync(message.ReceiverEmail.ToString());
            if (sender == null || receiver == null)
            {
                throw new ArgumentException("Sender or receiver not found");
            }

            var newMessageInfo = new MessageInfo
            {
                Sender = sender,
                Recipient = receiver,
                Content = message.Content,
                Timestamp = DateTime.UtcNow,
                IsRead = message.isRead
            };

            var res = await _messageInfoRepository.AddAsync(newMessageInfo);

            if (!res)
            {
                throw new Exception("Failed to create message");
            }

            return newMessageInfo;
        }

        public async Task<IEnumerable<MessageInfo>> GetMessagesBySenderIdAndReceiverId(string senderId, string receiverId)
        {
            if (senderId == null || receiverId == null)
            {
                throw new ArgumentNullException("SenderId or ReceiverId cannot be null");
            }

            return await _messageInfoRepository.GetAllAsync(
                m => (m.Sender.Id == senderId && m.Recipient.Id == receiverId) ||
                (m.Sender.Id == receiverId && m.Recipient.Id == senderId),
                includeProperties: "Sender,Recipient");
        }

        public async Task<IEnumerable<MessageInfo>> GetMessagesBySenderEmailAndReceiverEmail(string senderEmail, string receiverEmail)
        {
            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(receiverEmail))
            {
                throw new ArgumentNullException("SenderEmail or ReceiverEmail cannot be null or empty");
            }

            var sender = await _userManager.FindByEmailAsync(senderEmail);
            var receiver = await _userManager.FindByEmailAsync(receiverEmail);

            if (sender == null || receiver == null)
            {
                throw new ArgumentException("Sender or receiver not found");
            }

            return await _messageInfoRepository.GetAllAsync(
                m => (m.Sender.Id == sender.Id && m.Recipient.Id == receiver.Id) ||
                     (m.Sender.Id == receiver.Id && m.Recipient.Id == sender.Id),
                includeProperties: "Sender,Recipient");
        }
    }
}
