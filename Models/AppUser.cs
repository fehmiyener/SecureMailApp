namespace SecureMailApp.Models;

public class AppUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;   

    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    public string RsaPublicKeyXml { get; set; } = null!;
    public string RsaPrivateKeyXml { get; set; } = null!;
}
