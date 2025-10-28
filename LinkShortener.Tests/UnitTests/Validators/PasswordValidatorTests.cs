using LinkShortener.Application.Common.Validators;
using Xunit;

namespace LinkShortener.Tests.UnitTests.Validators
{
    public class PasswordValidatorTests
    {
        [Theory]
        [InlineData("ValidPassword1!")]
        [InlineData("MySecureP@ssw0rd")]
        [InlineData("AnotherValid123!@#")]
        public void Validate_ValidPassword_ReturnsTrue(string password)
        {
            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate(password);

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void Validate_EmptyPassword_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate("");

            // Assert
            Assert.False(isValid);
            Assert.Equal("Password cannot be empty", errorMessage);
        }

        [Fact]
        public void Validate_TooShort_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate("Short1!");

            // Assert
            Assert.False(isValid);
            Assert.Contains("at least 8 characters", errorMessage);
        }

        [Fact]
        public void Validate_NoUpperCase_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate("lowercase123!");

            // Assert
            Assert.False(isValid);
            Assert.Contains("uppercase letter", errorMessage);
        }

        [Fact]
        public void Validate_NoLowerCase_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate("UPPERCASE123!");

            // Assert
            Assert.False(isValid);
            Assert.Contains("lowercase letter", errorMessage);
        }

        [Fact]
        public void Validate_NoDigit_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate("NoDigitsHere!");

            // Assert
            Assert.False(isValid);
            Assert.Contains("digit", errorMessage);
        }

        [Fact]
        public void Validate_NoSpecialChar_ReturnsFalse()
        {
            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate("NoSpecial123");

            // Assert
            Assert.False(isValid);
            Assert.Contains("special character", errorMessage);
        }

        [Fact]
        public void Validate_TooLong_ReturnsFalse()
        {
            // Arrange
            var longPassword = new string('a', 129) + "A1!";

            // Act
            var (isValid, errorMessage) = PasswordValidator.Validate(longPassword);

            // Assert
            Assert.False(isValid);
            Assert.Contains("cannot exceed 128 characters", errorMessage);
        }
    }
}
