using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers;

public class BlogController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}