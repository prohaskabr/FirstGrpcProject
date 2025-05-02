using Basics;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;
using System.Diagnostics;

namespace MvcClient.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FirstServiceDefinition.FirstServiceDefinitionClient client;

    public HomeController(ILogger<HomeController> logger, FirstServiceDefinition.FirstServiceDefinitionClient client)
    {
        _logger = logger;
        this.client = client;
    }

    public IActionResult Index()
    {
        var call = client.Unary(new Request { Content = "Hello from MVC" });
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
