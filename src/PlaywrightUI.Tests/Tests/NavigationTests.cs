using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using FluentAssertions;
using NUnit.Framework;
using PlaywrightUI.Tests.Components;
using PlaywrightUI.Tests.Constants;
using PlaywrightUI.Tests.Data;
using PlaywrightUI.Tests.Fixtures;
using PlaywrightUI.Tests.Pages;

namespace PlaywrightUI.Tests.Tests;

[TestFixture]
[Category(AppConstants.Categories.Smoke)]
[AllureSuite("Navigation")]
[AllureFeature("Side Menu Navigation")]
public sealed class NavigationTests : BrowserFixture
{
    private LoginPage _loginPage = null!;
    private DashboardPage _dashboardPage = null!;
    private SideMenuComponent _sideMenu = null!;

    [SetUp]
    public new async Task SetUpAsync()
    {
        await base.SetUpAsync();
        _loginPage = new LoginPage(Page, Logger);
        _dashboardPage = new DashboardPage(Page, Logger);
        _sideMenu = new SideMenuComponent(Page, Logger);

        await _loginPage.NavigateAsync();
        await _loginPage.LoginAsAsync(TestDataFactory.AdminUser);
        await _dashboardPage.WaitForLoadAsync();
    }

    [Test]
    [AllureStory("Navigate to PIM")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task SideMenu_NavigateToPIM_ShouldLoadEmployeeList()
    {
        await _sideMenu.NavigateToAsync("PIM");
        Page.Url.Should().Contain("/pim/viewEmployeeList");
    }

    [Test]
    [AllureStory("Navigate to Admin")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task SideMenu_NavigateToAdmin_ShouldLoadAdminPage()
    {
        await _sideMenu.NavigateToAsync("Admin");
        Page.Url.Should().Contain("/admin/");
    }

    [Test]
    [AllureStory("Side Menu Visibility")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task SideMenu_AllCoreMenuItems_ShouldBeVisible()
    {
        var expectedItems = new[] { "Admin", "PIM", "Leave", "Time", "Recruitment", "Dashboard" };
        foreach (var item in expectedItems)
        {
            (await _sideMenu.IsMenuItemVisibleAsync(item))
                .Should().BeTrue($"menu item '{item}' should be visible in the side navigation");
        }
    }

    [Test]
    [AllureStory("Navigate to Directory")]
    [AllureSeverity(SeverityLevel.minor)]
    public async Task SideMenu_NavigateToDirectory_ShouldLoadDirectoryPage()
    {
        await _sideMenu.NavigateToAsync("Directory");
        Page.Url.Should().Contain("/directory/");
    }
}
