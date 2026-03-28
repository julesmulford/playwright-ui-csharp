using PlaywrightUI.Tests.Config;
using PlaywrightUI.Tests.Models;

namespace PlaywrightUI.Tests.Data;

public static class TestDataFactory
{
    public static TestUser AdminUser => new()
    {
        Username = TestConfiguration.Application.AdminUsername,
        Password = TestConfiguration.Application.AdminPassword
    };

    public static TestUser InvalidUser => new()
    {
        Username = "invalid_user_xyz",
        Password = "wrong_password_xyz"
    };

    public static TestUser EmptyCredentials => new()
    {
        Username = string.Empty,
        Password = string.Empty
    };

    public static Employee NewEmployee() =>
        EmployeeBuilder.Create().WithUniqueSuffix().Build();

    public static Employee EmployeeWithFullName(string firstName, string lastName) =>
        EmployeeBuilder.Create()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .Build();
}
