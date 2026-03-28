using Microsoft.Playwright;
using PlaywrightUI.Tests.Config;
using Serilog;

namespace PlaywrightUI.Tests.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly ILogger Logger;
    protected readonly string BaseUrl;

    protected BasePage(IPage page, ILogger logger)
    {
        Page = page;
        Logger = logger;
        BaseUrl = TestConfiguration.Application.BaseUrl;
    }

    protected async Task NavigateToAsync(string path)
    {
        var url = $"{BaseUrl}{path}";
        Logger.Information("Navigating to: {Url}", url);
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });
    }

    protected async Task ClickAsync(string selector, string description = "")
    {
        Logger.Debug("Clicking: {Description} ({Selector})", description, selector);
        await Page.Locator(selector).First.ClickAsync();
    }

    protected async Task FillAsync(string selector, string value, string description = "")
    {
        Logger.Debug("Filling {Description}: '{Value}'", description, value);
        await Page.Locator(selector).First.FillAsync(value);
    }

    protected async Task<string> GetTextAsync(string selector)
    {
        return await Page.Locator(selector).First.InnerTextAsync();
    }

    protected async Task<bool> IsVisibleAsync(string selector)
    {
        return await Page.Locator(selector).First.IsVisibleAsync();
    }

    protected async Task WaitForSelectorAsync(string selector, int timeoutMs = 0)
    {
        var timeout = timeoutMs > 0 ? timeoutMs : (int)TestConfiguration.Browser.Timeout;
        await Page.Locator(selector).First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    protected async Task WaitForUrlAsync(string urlPattern)
    {
        await Page.WaitForURLAsync(new Regex(urlPattern), new PageWaitForURLOptions
        {
            Timeout = TestConfiguration.Browser.NavigationTimeout
        });
    }
}
