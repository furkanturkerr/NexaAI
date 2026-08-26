using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NexaAI.WebApi.Hubs;

[Authorize]
public class ChatHub : Hub
{
}