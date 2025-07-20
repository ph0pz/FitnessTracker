namespace FitnessTracker.API.Helpers
{
    public static class PasswordHasher // Make it static as it doesn't need state
    {
        // Hashes a password using BCrypt
        public static string HashPassword(string password)
        {
            // Generates a salt and hashes the password with a work factor of 12
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        // Verifies a password against a stored hash
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
