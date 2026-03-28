using Allure.Net.Commons;
using Microsoft.Playwright;
using PlaywrightUI.Tests.Constants;
using Serilog;

namespace PlaywrightUI.Tests.Pages;

public sealed class DashboardPage : BasePage
{
    public DashboardPage(IPage page, ILogger logger) : base(page, logger) { }

    public async Task WaitForLoadAsync()
    {
        Logger.Information("Waiting for Dashboard to load");
        await Page.WaitForURLAsync(url => url.Contains("/dashboard/index"), new PageWaitForURLOptions
        {
            Timeout = AppConstants.Timeouts.NavigationMs
        });
        await WaitForSelectorAsync(AppConstants.Selectors.DashboardHeading);
    }

    public async Task<string> GetHeadingTextAsync()
    {
        return await GetTextAsync(AppConstants.Selectors.DashboardHeading);
    }

    public async Task<bool> IsLoadedAsync()
    {
        return Page.Url.Contains("/dashboard/index") &&
               await IsVisibleAsync(AppConstants.Selectors.DashboardHeading);
    }

    public async Task LogoutAsync()
    {
        Logger.Information("Logging out");
        await AllureApi.Step("Perform logout", async () =>
        {
            await ClickAsync(AppConstants.Selectors.UserDropdown, "user dropdown");
            await ClickAsync(AppConstants.Selectors.LogoutLink, "logout link");
            await Page.WaitForURLAsync(url => url.Contains("/auth/login"), new PageWaitForURLOptions
            {
                Timeout = AppConstants.Timeouts.NavigationMs
            });
        });
    }

    public async Task<string> GetLoggedInUsernameAsync()
    {
        return await GetTextAsync(".oxd-userdropdown-name");
    }
}
