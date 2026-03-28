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
[Category(AppConstants.Categories.Regression)]
[AllureSuite("Dashboard")]
[AllureFeature("Dashboard Validation")]
public sealed class DashboardTests : BrowserFixture
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
        await _loginPage.LoginAsAsync(TestDataFactory.AdminUser);
        await _dashboardPage.WaitForLoadAsync();
    }

    [Test]
    [AllureStory("Dashboard Heading")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task Dashboard_ShouldDisplayCorrectHeading()
    {
        var heading = await _dashboardPage.GetHeadingTextAsync();
        heading.Should().Be("Dashboard");
    }

    [Test]
    [AllureStory("Dashboard URL")]
    [AllureSeverity(SeverityLevel.minor)]
    public async Task Dashboard_ShouldHaveCorrectUrl()
    {
        Page.Url.Should().Contain(AppConstants.Urls.Dashboard);
    }

    [Test]
    [AllureStory("Logged In Username")]
    [AllureSeverity(SeverityLevel.minor)]
    public async Task Dashboard_ShouldShowLoggedInUsername()
    {
        var username = await _dashboardPage.GetLoggedInUsernameAsync();
        username.Should().NotBeNullOrEmpty("logged-in user's name should be visible in the topbar");
    }
}
