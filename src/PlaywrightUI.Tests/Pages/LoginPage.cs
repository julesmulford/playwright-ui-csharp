using Allure.Net.Commons;
using Microsoft.Playwright;
using PlaywrightUI.Tests.Constants;
using PlaywrightUI.Tests.Models;
using Serilog;

namespace PlaywrightUI.Tests.Pages;

public sealed class LoginPage : BasePage
{
    public LoginPage(IPage page, ILogger logger) : base(page, logger) { }

    public async Task NavigateAsync()
    {
        Logger.Information("Navigating to login page");
        await NavigateToAsync(AppConstants.Urls.Login);
        await WaitForSelectorAsync(AppConstants.Selectors.UsernameInput);
    }

    public async Task LoginAsync(string username, string password)
    {
        Logger.Information("Logging in as: {Username}", username);
        await AllureApi.Step("Enter credentials and submit login form", async () =>
        {
            await FillAsync(AppConstants.Selectors.UsernameInput, username, "username");
            await FillAsync(AppConstants.Selectors.PasswordInput, password, "password");
            await ClickAsync(AppConstants.Selectors.SubmitButton, "submit");
        });
    }

    public async Task LoginAsAsync(TestUser user)
    {
        await LoginAsync(user.Username, user.Password);
    }

    public async Task<string> GetErrorMessageAsync()
    {
        await WaitForSelectorAsync(AppConstants.Selectors.LoginErrorMessage, AppConstants.Timeouts.ShortMs);
        return await GetTextAsync(AppConstants.Selectors.LoginErrorMessage);
    }

    public async Task<bool> IsErrorDisplayedAsync()
    {
        try
        {
            await WaitForSelectorAsync(AppConstants.Selectors.LoginErrorMessage, AppConstants.Timeouts.ShortMs);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task<bool> IsOnLoginPageAsync()
    {
        return Page.Url.Contains("/auth/login");
    }
}
