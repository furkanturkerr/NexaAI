using Microsoft.AspNetCore.Mvc;

namespace NexaAI.WebUI.Controllers;

public class DefaultController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}