using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureMailApp.Data;
using SecureMailApp.Models;
using SecureMailApp.Services;

namespace SecureMailApp.Controllers;

public class MessagesController : Controller
{
    private readonly AppDbContext _db;
    private readonly CryptoService _crypto;

    public MessagesController(AppDbContext db, CryptoService crypto)
    {
        _db = db;
        _crypto = crypto;
    }

    private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

    public async Task<IActionResult> Inbox()
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");

        var userId = CurrentUserId.Value;
        var currentUser = await _db.Users.FindAsync(userId);
        if (currentUser == null) return RedirectToAction("Login", "Account");

        var messages = await _db.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.ReceiverId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        foreach (var message in messages)
        {
            var result = _crypto.DecryptAndVerify(
                message,
                currentUser.RsaPrivateKeyXml,
                message.Sender.RsaPublicKeyXml
            );

            message.DecryptedSubject = result.subject;
            message.DecryptedBody = result.plainText;
            message.IsHashValid = result.isHashValid;
            message.IsSignatureValid = result.isSignatureValid;
        }

        return View(messages);
    }

    [HttpGet]
    public IActionResult Compose()
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Compose(string recipientEmail, string subject, string content)
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");

        var sender = await _db.Users.FindAsync(CurrentUserId.Value);
        if (sender == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var recipient = await _db.Users.FirstOrDefaultAsync(u => u.Email == recipientEmail);
        if (recipient == null)
        {
            ViewBag.Error = "Reciever email not found.";
            return View();
        }

        var (encKey, iv, cipher, subjectIv, encryptedSubject, hash, sig) = _crypto.EncryptAndSign(
            subject ?? string.Empty,
            content,
            recipient.RsaPublicKeyXml,
            sender.RsaPrivateKeyXml
        );

        var msg = new EmailMessage
        {
            SenderId = sender.Id,
            ReceiverId = recipient.Id,
            EncryptedSubject = encryptedSubject,
            SubjectIv = subjectIv,
            EncryptedSymmetricKey = encKey,
            SymmetricIv = iv,
            CipherText = cipher,
            Hash = hash,
            Signature = sig
        };

        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();

        return RedirectToAction("Inbox");
    }


    public async Task<IActionResult> Details(int id)
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");

        var msg = await _db.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (msg == null) return NotFound();
        if (msg.ReceiverId != CurrentUserId) return Forbid();

        var result = _crypto.DecryptAndVerify(
            msg,
            msg.Receiver.RsaPrivateKeyXml,
            msg.Sender.RsaPublicKeyXml
        );

        msg.DecryptedSubject = result.subject;
        msg.DecryptedBody = result.plainText;
        msg.IsHashValid = result.isHashValid;
        msg.IsSignatureValid = result.isSignatureValid;

        return View(msg);
    }
}
