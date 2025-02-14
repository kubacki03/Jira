﻿using System.Security.Cryptography;
using System.Text;

public class Hash
{
    private static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public static string HashPassword(string password)
    {
        string salt = GenerateSalt(); // Generowanie soli
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = sha256.ComputeHash(saltedPasswordBytes);
            return salt + ":" + Convert.ToBase64String(hashBytes); // Przechowywanie soli i hasha
        }
    }

    public static bool VerifyPassword(string inputPassword, string storedHash)
    {
        // Rozdzielamy sól i hash
        string[] parts = storedHash.Split(':');
        if (parts.Length != 2) return false; // Nieprawidłowy format

        string salt = parts[0]; // Pobranie soli
        string correctHash = parts[1]; // Pobranie hasha

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(inputPassword + salt);
            byte[] hashBytes = sha256.ComputeHash(saltedPasswordBytes);
            string hashedInput = Convert.ToBase64String(hashBytes);
            return hashedInput == correctHash; // Sprawdzanie poprawności hasła
        }
    }
}
