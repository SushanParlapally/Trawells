using BCrypt.Net;
using System;

namespace TravelDesk.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }
            if (string.IsNullOrWhiteSpace(hash))
            {
                throw new ArgumentException("Hash cannot be null or empty", nameof(hash));
            }
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}