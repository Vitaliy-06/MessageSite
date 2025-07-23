using MessageSiteAPI.Hubs;
using MessageSiteAPI.Models.Dtos;
using MessageSiteAPI.Services;
using MessageSiteAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MessageSiteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageInfoService _messageInfoService;
        private readonly IAuthService _authService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageInfoService messageInfoService, IAuthService authService, IHubContext<ChatHub> hubContext)
        {
            _messageInfoService = messageInfoService;
            _authService = authService;
            _hubContext = hubContext;
        }


        [HttpPost("create-message")]
        [Authorize]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto message)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _authService.GetUserByEmailAsync(userEmail);

                message.SenderId = user.Id;

                var result = await _messageInfoService.CreateMessage(message);

                var data = new
                {
                    content = result.Content,
                    sender = new { result.Sender.Id, result.Sender.Email },
                    receiver = new { result.Recipient.Id, result.Recipient.Email },
                    timestamp = result.Timestamp,
                    isRead = result.IsRead
                };

                var emails = new[] { result.Sender.Email, result.Recipient.Email };
                Array.Sort(emails);
                var room = $"{emails[0]}-{emails[1]}";
                Console.WriteLine($"Room: {room}");
                await _hubContext.Clients.Group(room)
                    .SendAsync("ReceiveMessage", data);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    message = "Message created successfully",
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Error: " + ex.Message });
            }
        }

        [HttpGet("get-messages")]
        [Authorize]
        public async Task<IActionResult> GetMessages([FromQuery] string receiverEmail)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _authService.GetUserByEmailAsync(userEmail);

            var messages = await _messageInfoService.GetMessagesBySenderEmailAndReceiverEmail(user.Email, receiverEmail);

            return StatusCode(StatusCodes.Status200OK, new
            {
                message = "Messages retrieved successfully",
                data = messages.Select(m => new
                {
                    m.Content,
                    Sender = new { m.Sender.Id, m.Sender.Email },
                    Recipient = new { m.Recipient.Id, m.Recipient.Email },
                    m.Timestamp,
                    m.IsRead
                })
            }
            );
        }
    }
}

