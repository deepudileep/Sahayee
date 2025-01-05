namespace Sahayee.Helper
{
    public static class CommonHelper
    {
        public static string GenerateRandomPassword(int length)
        {
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";

            // Combine all character sets
            string allChars = lowerCase + upperCase + digits + specialChars;
            Random random = new Random();

            // Ensure the password has at least one character from each category
            char[] password = new char[length];
            password[0] = lowerCase[random.Next(lowerCase.Length)];
            password[1] = upperCase[random.Next(upperCase.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = specialChars[random.Next(specialChars.Length)];

            // Fill the rest of the password with random characters from all sets
            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the password to ensure randomness
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }
    }
}
