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
        return View();
    }
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> StartGame(string betAmount, int minesCount)
    {
        var startGameResponse = await _playerService.StartGameSession(HttpContext, betAmount, minesCount);
        return Json(startGameResponse);
    }
    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GetActiveGame()
    {
        var activeGameStatus = await _playerService.GetActiveGameSession(HttpContext);
        return Json(activeGameStatus);
    }
    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> GameTileClick(int tileClickedPosition)
    {
        var tileClickResponse = await _playerService.TileClickUpdateSession(HttpContext, tileClickedPosition);
        return Json(tileClickResponse);
    }
    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> CashoutGame()
    {
        var endGameResponse = await _playerService.CashoutGameSession(HttpContext);
        return Json(endGameResponse);
    }

    [HttpPost]
    public IActionResult ClearSessionOnly()
    {
        /* 
            fetch('/Game/ClearSession', { method: 'POST' })
            .then(response => response.json())
            .then(data => {
            if (data.success) {
                alert("Session cleared!");
            }
            })
            .catch(error => console.error('Error:', error));
        */
        HttpContext.Session.Clear(); // Clears session data but keeps authentication (explicit browser testing)
        return Json(new { success = true });
    }

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
    // [HttpPost("Game/Edit-Profile")] // IMPORTANT use this for explicitly setting
    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> ProfileEdit([FromBody] PlayerViewModel playerVm)
    {
        bool isPlayerEdited = await _playerService.EditPlayerAsync(HttpContext, playerVm);
        if (isPlayerEdited == true)
        {
            return Json(new { success = true, message = "Profile edited successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Email/Username already esists." });
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
