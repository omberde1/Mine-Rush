using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinesGame.Models;
using MinesGame.Service;
using MinesGame.ViewModels;

namespace MinesGame.Controllers;

public class GameController : Controller
{
    private readonly IPlayerService _playerService;

    public GameController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    /* ---------- REGISTER | LOGIN ---------- */
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Home", "Game");
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] PlayerViewModel player)
    {
        bool isServerRunning = await _playerService.IsSqlServerAvailableAsync();
        if (isServerRunning == false)
        {
            return StatusCode(503, new { message = "Server down!" });
        }
        else
        {
            if (await _playerService.RegisterPlayerAsync(player) == true)
            {
                return Json(new { success = true, message = "Account created!" });
            }
            else
            {
                return Json(new { success = false, message = "Username/Email already exists" });
            }
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Home", "Game");
        }
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(string username_email, string password)
    {
        bool isServerRunning = await _playerService.IsSqlServerAvailableAsync();
        if (isServerRunning == false)
        {
            return Json(new { success = false, message = "Server down!" });
        }
        else
        {
            bool isSignedIn = await _playerService.LoginPlayerAsync(HttpContext, username_email, password);

            if (isSignedIn == true)
            {
                return Json(new { success = true, message = "Login successful!" });
            }
            else
            {
                return Json(new { success = false, message = "Invalid username or password!" });
            }
        }
    }

    [HttpGet]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Home", "Game");
    }

    /* ---------- NAV BAR ---------- */

    [HttpGet]
    public IActionResult Home()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Player")]
    public IActionResult PlayGame()
    {
        // var getGameStatus = await _playerService.GetGameSession(HttpContext);
        // return View(getGameStatus);
        return View();
    }
    // [HttpPost]
    // [Authorize(Roles = "Player")]
    // public async Task<IActionResult> StartGame(int betAmount, int minesCount)
    // {
    //     var isGameActive = await _playerService.StartGameSession(HttpContext, betAmount, minesCount);
    //     return View(isGameActive);
    // }
    // [HttpPost]
    // [Authorize(Roles = "Player")]
    // public async Task<IActionResult> TileClick([FromBody] int tilePosition)
    // {
    //     var isGameActive = await _playerService.TileClickUpdateSession(HttpContext, tilePosition);
    //     return View(isGameActive);
    // }
    // [HttpPost]
    // [Authorize(Roles = "Player")]
    // public async Task<IActionResult> EndGame()
    // {
    //     var isGameActive = await _playerService.EndGameSession(HttpContext);
    //     return View(isGameActive);
    // }

    [HttpGet]
    public IActionResult Rules()
    {
        return View();
    }

    [HttpGet]
    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    /* ---------- PLAYER ACTION BAR ---------- */

    [HttpGet]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> Profile()
    {
        var getDummyPlayer = await _playerService.CreateDummyPlayer(HttpContext);
        return View(getDummyPlayer);
    }
    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> Profile([FromBody] PlayerViewModel playerVm)
    {
        bool isPlayerEdited = await _playerService.EditPlayerAsync(HttpContext,playerVm);
        if(isPlayerEdited == true)
        {
            return Json(new {success=true, message="Profile edited successfully."});
        }
        else
        {
            return Json(new {success=false, message="Email/Username already esists."});
        }
    }

    [HttpGet]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> Wallet()
    {
        var getDummyPlayerWallet = await _playerService.CreateDummyPlayerWallet(HttpContext);
        return View(getDummyPlayerWallet);
    }
    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> WalletDeposit(string amount)
    {
        var addMoneyResponse = await _playerService.AddMoneyToWallet(HttpContext, amount);
        return Json(addMoneyResponse);
    }
    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> WalletWithdraw(string amount)
    {
        var addMoneyResponse = await _playerService.RemoveMoneyFromWallet(HttpContext, amount);
        return Json(addMoneyResponse);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
