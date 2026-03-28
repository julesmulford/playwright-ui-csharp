namespace PlaywrightUI.Tests.Models;

public record TestUser
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
