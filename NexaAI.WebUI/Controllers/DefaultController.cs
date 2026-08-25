using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NexaAI.WebUI.Controllers;

[Authorize]
public class DefaultController : Controller
{
    public IActionResult Index(Guid? conversationId)
    {
        ViewBag.ConversationId = conversationId;

        return View();
    }
}