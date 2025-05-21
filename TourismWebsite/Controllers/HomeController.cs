using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TourismWebsite.Models;
using TourismWebsite.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TourismWebsite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TourismContext _context;

    public HomeController(ILogger<HomeController> logger, TourismContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var featuredDestinations = await _context.Destinations
                                            .OrderBy(d => d.Id)
                                            .Take(4)
                                            .ToListAsync();
        
        return View(featuredDestinations);
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
