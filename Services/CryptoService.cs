using System.Security.Cryptography;
using System.Text;
using SecureMailApp.Models;

namespace SecureMailApp.Services;

public class CryptoService
{

    // Password Hash (Salt + SHA-256) -- Password Hashing
    public (string hash, string salt)
    HashPassword(string password)
    {
        // 16-byte (128-bit) random salt
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);

        // hash = SHA256( salt || password )
        byte[] hashBytes = ComputeSha256WithSalt(password,
        saltBytes);

        return (Convert.ToBase64String(hashBytes),
        Convert.ToBase64String(saltBytes));
    }

    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);
        byte[] computedHashBytes = ComputeSha256WithSalt(password, saltBytes);
        string computedHash = Convert.ToBase64String(computedHashBytes);


        var a = Convert.FromBase64String(computedHash);
        var b = Convert.FromBase64String(storedHash);

        return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
    }

    private static byte[] ComputeSha256WithSalt(string password, byte[] saltBytes)
    {
        byte[] pwdBytes = Encoding.UTF8.GetBytes(password);

        // concating salt + password
        byte[] input = new byte[saltBytes.Length + pwdBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, input, 0, saltBytes.Length);
        Buffer.BlockCopy(pwdBytes, 0, input, saltBytes.Length, pwdBytes.Length);

        return SHA256.HashData(input);
    }


    // RSA key pair generation

    public (string publicKeyXml, string privateKeyXml) GenerateRsaKeyPair()
    {
        using var rsa = RSA.Create(2048);
        string publicKey = rsa.ToXmlString(false);
        string privateKey = rsa.ToXmlString(true);
        return (publicKey, privateKey);
    }


    // Encrypt + Sign

    public (byte[] encryptedKey, byte[] bodyIv, byte[] bodyCipherText, byte[] subjectIv, byte[] encryptedSubject, byte[] hash, byte[] signature)
        EncryptAndSign(string subject, string plainText, string recipientPublicKeyXml, string senderPrivateKeyXml)
    {
        byte[] subjectBytes = Encoding.UTF8.GetBytes(subject ?? string.Empty);

        //Signing
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        // Computing Hash
        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Combine(subjectBytes, plainBytes));

        // 2) Generating random AES Key + IV -- Confidentiality
        byte[] key = RandomNumberGenerator.GetBytes(32); // 256-bit
        byte[] bodyIv = RandomNumberGenerator.GetBytes(16);  // 128-bit
        byte[] subjectIv = RandomNumberGenerator.GetBytes(16);  //128-bit
        // 3) Encrypting subject and message with AES
        byte[] cipherText = EncryptWithAes(plainBytes, key, bodyIv);
        byte[] encryptedSubject = EncryptWithAes(subjectBytes, key, subjectIv);
        byte[] encryptedKey;
        // 4) Encrypting AES Key with receiver's RSA public key
        using (var rsaRecipient = RSA.Create())
        {
            rsaRecipient.FromXmlString(recipientPublicKeyXml);
            encryptedKey = rsaRecipient.Encrypt(key, RSAEncryptionPadding.OaepSHA256);
        }

        byte[] signature;

        // Signing hash with sender's RSA private key -- Authentication
        using (var rsaSender = RSA.Create())
        {
            rsaSender.FromXmlString(senderPrivateKeyXml);
            signature = rsaSender.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        return (encryptedKey, bodyIv, cipherText, subjectIv, encryptedSubject, hash, signature);
    }


    // Decrypt + Verify

    public (string subject, string plainText, bool isHashValid, bool isSignatureValid)
        DecryptAndVerify(EmailMessage message, string recipientPrivateKeyXml, string senderPublicKeyXml)
    {
        byte[] key;
        using (var rsaRecipient = RSA.Create())
        {
            rsaRecipient.FromXmlString(recipientPrivateKeyXml);
            key = rsaRecipient.Decrypt(message.EncryptedSymmetricKey, RSAEncryptionPadding.OaepSHA256);
        }

        bool isLegacySubject = message.SubjectIv == null || message.SubjectIv.Length == 0;

        byte[] subjectBytes = isLegacySubject
            ? message.EncryptedSubject
            : DecryptWithAes(message.EncryptedSubject, key, message.SubjectIv!);

        byte[] plainBytes = DecryptWithAes(message.CipherText, key, message.SymmetricIv!);

        // Hash is computed from (subject + message) -- Integrity
        using var sha = SHA256.Create();
        byte[] computedHash = sha.ComputeHash(isLegacySubject ? plainBytes : Combine(subjectBytes, plainBytes));


        bool isHashValid = computedHash.SequenceEqual(message.Hash);

        bool isSignatureValid;

        // Receiver verifies signature using sender's public key -- VERIFY
        using (var rsaSender = RSA.Create())
        {
            rsaSender.FromXmlString(senderPublicKeyXml);
            isSignatureValid = rsaSender.VerifyHash(
                message.Hash,
                message.Signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }

        string subject = Encoding.UTF8.GetString(subjectBytes);
        string plainText = Encoding.UTF8.GetString(plainBytes);
        return (subject, plainText, isHashValid, isSignatureValid);
    }

    private static byte[] EncryptWithAes(byte[] plainBytes, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using var enc = aes.CreateEncryptor();
        return enc.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
    }

    private static byte[] DecryptWithAes(byte[] cipherBytes, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using var dec = aes.CreateDecryptor();
        return dec.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
    }

    private static byte[] Combine(byte[] subjectBytes, byte[] bodyBytes)
    {
        byte[] combined = new byte[subjectBytes.Length + bodyBytes.Length];
        Buffer.BlockCopy(subjectBytes, 0, combined, 0, subjectBytes.Length);
        Buffer.BlockCopy(bodyBytes, 0, combined, subjectBytes.Length, bodyBytes.Length);
        return combined;
    }
}
