using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MinesGame.Models;
using MinesGame.Service;

namespace MinesGame.Controllers;

public class GameController : Controller
{
    private readonly ILogger<GameController> _logger;
    private readonly IGameService _gameService;

    public GameController(IGameService gameService ,ILogger<GameController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    [HttpGet]
    [Route("[controller]/Home")]
    public IActionResult HomePage()
    {
        return View();
    }

    [Route("[controller]/PlayMines")]
    public IActionResult PlayGamePage()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
