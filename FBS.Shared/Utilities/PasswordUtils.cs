namespace FBS.Shared.Utilities
{
    using System.Text;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    public static class PasswordUtils
    {
        public static string HashPassword(this string password)
        {
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>(
                    new OptionsWrapper<PasswordHasherOptions>(
                        new PasswordHasherOptions()
                        {
                            CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
                        }));

            string hashPassword = passwordHasher.HashPassword(string.Empty, password);
            return hashPassword;
        }

        public static string GeneratePassword(int length)
        {
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:'\",.<>?/";

            string allChars = lowerCase + upperCase + numbers + specialChars;
            StringBuilder password = new StringBuilder();
            Random random = new Random();

            password.Append(lowerCase[random.Next(lowerCase.Length)]);
            password.Append(upperCase[random.Next(upperCase.Length)]);
            password.Append(numbers[random.Next(numbers.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            return ShuffleString(password.ToString());
        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            PasswordHasher<string> passwordHasher = new PasswordHasher<string>(
                    new OptionsWrapper<PasswordHasherOptions>(
                        new PasswordHasherOptions()
                        {
                            CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
                        }));

            var verificationResult = passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            bool result = verificationResult == PasswordVerificationResult.Success;
            return result;
        }

        public static string ShuffleString(string str)
        {
            char[] array = str.ToCharArray();
            Random random = new Random();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }

            return new string(array);
        }
    }
}
