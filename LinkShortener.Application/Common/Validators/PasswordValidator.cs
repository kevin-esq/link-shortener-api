namespace LinkShortener.Application.Common.Validators
{
    public static class PasswordValidator
    {
        private const int MinLength = 8;
        private const int MaxLength = 128;

        public static (bool IsValid, string? ErrorMessage) Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty");

            if (password.Length < MinLength)
                return (false, $"Password must be at least {MinLength} characters long");

            if (password.Length > MaxLength)
                return (false, $"Password cannot exceed {MaxLength} characters");

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            if (!hasUpperCase)
                return (false, "Password must contain at least one uppercase letter");

            if (!hasLowerCase)
                return (false, "Password must contain at least one lowercase letter");

            if (!hasDigit)
                return (false, "Password must contain at least one digit");

            if (!hasSpecialChar)
                return (false, "Password must contain at least one special character");

            return (true, null);
        }
    }
}
