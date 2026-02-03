using System.ComponentModel.DataAnnotations.Schema;

namespace SecureMailApp.Models;

public class EmailMessage
{
    public int Id { get; set; }

    public int SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;

    public int ReceiverId { get; set; }
    public AppUser Receiver { get; set; } = null!;

    [Column("Subject")]
    public byte[] EncryptedSubject { get; set; } = null!;
    public byte[] SubjectIv { get; set; } = null!;

    public byte[] EncryptedSymmetricKey { get; set; } = null!;
    public byte[] SymmetricIv { get; set; } = null!;
    public byte[] CipherText { get; set; } = null!;

    public byte[] Hash { get; set; } = null!;
    public byte[] Signature { get; set; } = null!;

    [NotMapped]
    public string DecryptedSubject { get; set; } = string.Empty;

    [NotMapped]
    public string DecryptedBody { get; set; } = string.Empty;

    [NotMapped]
    public bool IsHashValid { get; set; }

    [NotMapped]
    public bool IsSignatureValid { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now; 

}
