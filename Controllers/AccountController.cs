using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureMailApp.Data;
using SecureMailApp.Models;
using SecureMailApp.Services;

namespace SecureMailApp.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly CryptoService _crypto;

    public AccountController(AppDbContext db, CryptoService crypto)
    {
        _db = db;
        _crypto = crypto;
    }

    
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _db.Users.AnyAsync(u => u.UserName == model.UserName || u.Email == model.Email))
        {
            ModelState.AddModelError("", "This username or email is already in use.");
            return View(model);
        }

        var (hash, salt) = _crypto.HashPassword(model.Password);
        var (pub, priv) = _crypto.GenerateRsaKeyPair();

        var user = new AppUser
        {
            UserName = model.UserName,
            Email = model.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            RsaPublicKeyXml = pub,
            RsaPrivateKeyXml = priv
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        TempData["RegistrationSuccess"] = "Registration completed. Please log in to continue.";
        return RedirectToAction("Login");
    }

    
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.UserName == model.Identifier || u.Email == model.Identifier);

        if (user == null || !_crypto.VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt))
        {
            ModelState.AddModelError("", "Invalid username/email or password.");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.UserName);

        return RedirectToAction("Inbox", "Messages");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
