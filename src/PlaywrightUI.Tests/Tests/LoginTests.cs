using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using FluentAssertions;
using NUnit.Framework;
using PlaywrightUI.Tests.Constants;
using PlaywrightUI.Tests.Data;
using PlaywrightUI.Tests.Fixtures;
using PlaywrightUI.Tests.Pages;

namespace PlaywrightUI.Tests.Tests;

[TestFixture]
[Category(AppConstants.Categories.Smoke)]
[AllureSuite("Authentication")]
[AllureFeature("Login")]
public sealed class LoginTests : BrowserFixture
{
    private LoginPage _loginPage = null!;
    private DashboardPage _dashboardPage = null!;

    [SetUp]
    public new async Task SetUpAsync()
    {
        await base.SetUpAsync();
        _loginPage = new LoginPage(Page, Logger);
        _dashboardPage = new DashboardPage(Page, Logger);
        await _loginPage.NavigateAsync();
    }

    [Test]
    [AllureStory("Valid Login")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Admin user with valid credentials should be redirected to the Dashboard")]
    public async Task Login_WithValidCredentials_ShouldReachDashboard()
    {
        await _loginPage.LoginAsAsync(TestDataFactory.AdminUser);
        await _dashboardPage.WaitForLoadAsync();

        (await _dashboardPage.IsLoadedAsync())
            .Should().BeTrue("dashboard should be visible after valid login");

        (await _dashboardPage.GetHeadingTextAsync())
            .Should().Be("Dashboard");
    }

    [Test]
    [Category(AppConstants.Categories.Regression)]
    [AllureStory("Invalid Login")]
    [AllureSeverity(SeverityLevel.critical)]
    [Retry(2)]
    [Description("Invalid credentials should display the error message and stay on login page")]
    public async Task Login_WithInvalidCredentials_ShouldShowError()
    {
        await _loginPage.LoginAsAsync(TestDataFactory.InvalidUser);

        (await _loginPage.IsErrorDisplayedAsync())
            .Should().BeTrue("error message should be shown for invalid credentials");

        var errorText = await _loginPage.GetErrorMessageAsync();
        errorText.Should().Contain(AppConstants.TestMessages.InvalidCredentials);
    }

    [Test]
    [Category(AppConstants.Categories.Regression)]
    [AllureStory("Empty Credentials")]
    [AllureSeverity(SeverityLevel.normal)]
    [Description("Submitting empty credentials should display validation errors")]
    public async Task Login_WithEmptyCredentials_ShouldShowValidationErrors()
    {
        await _loginPage.LoginAsAsync(TestDataFactory.EmptyCredentials);

        var errorVisible = await _loginPage.IsErrorDisplayedAsync();
        var url = Page.Url;

        errorVisible.Should().BeTrue("validation error should appear for empty credentials");
        url.Should().Contain("/auth/login", "user should remain on login page");
    }

    [Test]
    [Category(AppConstants.Categories.Smoke)]
    [AllureStory("Logout")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Logged-in user should be able to log out and be returned to the login page")]
    public async Task Logout_AfterValidLogin_ShouldReturnToLoginPage()
    {
        await _loginPage.LoginAsAsync(TestDataFactory.AdminUser);
        await _dashboardPage.WaitForLoadAsync();
        await _dashboardPage.LogoutAsync();

        Page.Url.Should().Contain("/auth/login", "user should be on the login page after logout");
        (await _loginPage.IsOnLoginPageAsync()).Should().BeTrue();
    }

    [Test]
    [Category(AppConstants.Categories.Regression)]
    [AllureStory("Invalid Login - Wrong Password")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task Login_WithCorrectUsernameWrongPassword_ShouldShowError()
    {
        await _loginPage.LoginAsync(TestDataFactory.AdminUser.Username, "wrong_password_999");

        (await _loginPage.IsErrorDisplayedAsync())
            .Should().BeTrue("error message should be shown for wrong password");
    }
}
