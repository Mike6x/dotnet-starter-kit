using FSH.Modules.Identity.Data;
using FSH.Modules.Identity.Features.v1.Users;
using FSH.Modules.Identity.Services;
using Microsoft.Extensions.Options;

namespace Identity.Tests.Services;

/// <summary>
/// Tests for PasswordExpiryService - handles password expiry logic.
/// </summary>
public sealed class PasswordExpiryServiceTests
{
    private static PasswordExpiryService CreateService(PasswordPolicyOptions options)
    {
        return new PasswordExpiryService(Options.Create(options));
    }

    private static FshUser CreateUser(DateTime lastPasswordChangeDate)
    {
        return new FshUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "testuser",
            LastPasswordChangeDate = lastPasswordChangeDate
        };
    }

    #region IsPasswordExpired Tests

    [Fact]
    public void IsPasswordExpired_Should_ReturnFalse_When_EnforcePasswordExpiryIsFalse()
    {
        // Arrange
        var options = new PasswordPolicyOptions { EnforcePasswordExpiry = false };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-1000)); // Very old password

        // Act
        var result = service.IsPasswordExpired(user);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPasswordExpired_Should_ReturnTrue_When_PasswordExceedsExpiryDays()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-91)); // Password changed 91 days ago

        // Act
        var result = service.IsPasswordExpired(user);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsPasswordExpired_Should_ReturnFalse_When_PasswordWithinExpiryDays()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-89)); // Password changed 89 days ago

        // Act
        var result = service.IsPasswordExpired(user);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPasswordExpired_Should_ReturnFalse_When_PasswordChangedToday()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow);

        // Act
        var result = service.IsPasswordExpired(user);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPasswordExpired_Should_ReturnTrue_When_ExactlyOnExpiryBoundary()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        // Password changed exactly 90 days and 1 second ago (just past expiry)
        var user = CreateUser(DateTime.UtcNow.AddDays(-90).AddSeconds(-1));

        // Act
        var result = service.IsPasswordExpired(user);

        // Assert
        result.ShouldBeTrue();
    }

    #endregion

    #region GetDaysUntilExpiry Tests

    [Fact]
    public void GetDaysUntilExpiry_Should_ReturnMaxValue_When_EnforcePasswordExpiryIsFalse()
    {
        // Arrange
        var options = new PasswordPolicyOptions { EnforcePasswordExpiry = false };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-1000));

        // Act
        var result = service.GetDaysUntilExpiry(user);

        // Assert
        result.ShouldBe(int.MaxValue);
    }

    [Fact]
    public void GetDaysUntilExpiry_Should_ReturnPositiveDays_When_PasswordNotExpired()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-80)); // 80 days ago

        // Act
        var result = service.GetDaysUntilExpiry(user);

        // Assert - TotalDays truncates, so could be 9 or 10 depending on time of day
        result.ShouldBeInRange(9, 10);
    }

    [Fact]
    public void GetDaysUntilExpiry_Should_ReturnNegativeDays_When_PasswordExpired()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-100)); // 100 days ago

        // Act
        var result = service.GetDaysUntilExpiry(user);

        // Assert
        result.ShouldBeLessThan(0); // Expired 10 days ago
    }

    [Fact]
    public void GetDaysUntilExpiry_Should_ReturnExpiryDays_When_PasswordJustChanged()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow);

        // Act
        var result = service.GetDaysUntilExpiry(user);

        // Assert - TotalDays truncates, so could be 89 or 90 depending on time of day
        result.ShouldBeInRange(89, 90);
    }

    #endregion

    #region IsPasswordExpiringWithinWarningPeriod Tests

    [Fact]
    public void IsPasswordExpiringWithinWarningPeriod_Should_ReturnFalse_When_EnforcePasswordExpiryIsFalse()
    {
        // Arrange
        var options = new PasswordPolicyOptions { EnforcePasswordExpiry = false };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-85));

        // Act
        var result = service.IsPasswordExpiringWithinWarningPeriod(user);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsPasswordExpiringWithinWarningPeriod_Should_ReturnTrue_When_WithinWarningDays()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90,
            PasswordExpiryWarningDays = 14
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-80)); // 10 days until expiry

        // Act
        var result = service.IsPasswordExpiringWithinWarningPeriod(user);

        // Assert
        result.ShouldBeTrue(); // 10 days <= 14 warning days
    }

    [Fact]
    public void IsPasswordExpiringWithinWarningPeriod_Should_ReturnFalse_When_OutsideWarningDays()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90,
            PasswordExpiryWarningDays = 14
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-70)); // 20 days until expiry

        // Act
        var result = service.IsPasswordExpiringWithinWarningPeriod(user);

        // Assert
        result.ShouldBeFalse(); // 20 days > 14 warning days
    }

    [Fact]
    public void IsPasswordExpiringWithinWarningPeriod_Should_ReturnFalse_When_AlreadyExpired()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90,
            PasswordExpiryWarningDays = 14
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-100)); // Already expired

        // Act
        var result = service.IsPasswordExpiringWithinWarningPeriod(user);

        // Assert
        result.ShouldBeFalse(); // Already expired, not "expiring soon"
    }

    [Fact]
    public void IsPasswordExpiringWithinWarningPeriod_Should_ReturnTrue_When_ExpiringToday()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90,
            PasswordExpiryWarningDays = 14
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-90)); // Expiring today (0 days)

        // Act
        var result = service.IsPasswordExpiringWithinWarningPeriod(user);

        // Assert
        result.ShouldBeTrue(); // 0 days is within warning period
    }

    #endregion

    #region GetPasswordExpiryStatus Tests

    [Fact]
    public void GetPasswordExpiryStatus_Should_ReturnExpiredStatus_When_PasswordExpired()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90,
            PasswordExpiryWarningDays = 14
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-100));

        // Act
        var result = service.GetPasswordExpiryStatus(user);

        // Assert
        result.IsExpired.ShouldBeTrue();
        result.IsExpiringWithinWarningPeriod.ShouldBeFalse();
        result.DaysUntilExpiry.ShouldBeLessThan(0);
        result.ExpiryDate.ShouldNotBeNull();
        result.Status.ShouldBe("Expired");
    }

    [Fact]
    public void GetPasswordExpiryStatus_Should_ReturnExpiringSoonStatus_When_WithinWarningPeriod()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90,
            PasswordExpiryWarningDays = 14
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-80)); // ~10 days until expiry

        // Act
        var result = service.GetPasswordExpiryStatus(user);

        // Assert
        result.IsExpired.ShouldBeFalse();
        result.IsExpiringWithinWarningPeriod.ShouldBeTrue();
        result.DaysUntilExpiry.ShouldBeInRange(9, 10); // TotalDays truncates
        result.ExpiryDate.ShouldNotBeNull();
        result.Status.ShouldBe("Expiring Soon");
    }

    [Fact]
    public void GetPasswordExpiryStatus_Should_ReturnValidStatus_When_PasswordValid()
    {
        // Arrange
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90,
            PasswordExpiryWarningDays = 14
        };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-30)); // ~60 days until expiry

        // Act
        var result = service.GetPasswordExpiryStatus(user);

        // Assert
        result.IsExpired.ShouldBeFalse();
        result.IsExpiringWithinWarningPeriod.ShouldBeFalse();
        result.DaysUntilExpiry.ShouldBeInRange(59, 60); // TotalDays truncates
        result.ExpiryDate.ShouldNotBeNull();
        result.Status.ShouldBe("Valid");
    }

    [Fact]
    public void GetPasswordExpiryStatus_Should_ReturnNullExpiryDate_When_ExpiryNotEnforced()
    {
        // Arrange
        var options = new PasswordPolicyOptions { EnforcePasswordExpiry = false };
        var service = CreateService(options);
        var user = CreateUser(DateTime.UtcNow.AddDays(-30));

        // Act
        var result = service.GetPasswordExpiryStatus(user);

        // Assert
        result.IsExpired.ShouldBeFalse();
        result.IsExpiringWithinWarningPeriod.ShouldBeFalse();
        result.DaysUntilExpiry.ShouldBe(int.MaxValue);
        result.ExpiryDate.ShouldBeNull();
        result.Status.ShouldBe("Valid");
    }

    [Fact]
    public void GetPasswordExpiryStatus_Should_CalculateCorrectExpiryDate()
    {
        // Arrange
        var lastChange = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var options = new PasswordPolicyOptions
        {
            EnforcePasswordExpiry = true,
            PasswordExpiryDays = 90
        };
        var service = CreateService(options);
        var user = CreateUser(lastChange);

        // Act
        var result = service.GetPasswordExpiryStatus(user);

        // Assert
        result.ExpiryDate.ShouldBe(lastChange.AddDays(90));
    }

    #endregion

    #region UpdateLastPasswordChangeDate Tests

    [Fact]
    public void UpdateLastPasswordChangeDate_Should_SetToCurrentUtcTime()
    {
        // Arrange
        var options = new PasswordPolicyOptions();
        var service = CreateService(options);
        var oldDate = DateTime.UtcNow.AddDays(-100);
        var user = CreateUser(oldDate);

        // Act
        var beforeUpdate = DateTime.UtcNow;
        service.UpdateLastPasswordChangeDate(user);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        user.LastPasswordChangeDate.ShouldBeGreaterThanOrEqualTo(beforeUpdate);
        user.LastPasswordChangeDate.ShouldBeLessThanOrEqualTo(afterUpdate);
    }

    #endregion
}
