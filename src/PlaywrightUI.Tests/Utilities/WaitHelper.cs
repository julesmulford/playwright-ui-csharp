using Microsoft.Playwright;

namespace PlaywrightUI.Tests.Utilities;

public static class WaitHelper
{
    public static async Task WaitForConditionAsync(
        Func<Task<bool>> condition,
        int timeoutMs = 10000,
        int pollingIntervalMs = 500,
        string description = "condition")
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (DateTime.UtcNow < deadline)
        {
            if (await condition())
                return;
            await Task.Delay(pollingIntervalMs);
        }
        throw new TimeoutException($"Condition '{description}' not met within {timeoutMs}ms.");
    }

    public static async Task WaitForNetworkIdleAsync(IPage page, int timeoutMs = 5000)
    {
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
        {
            Timeout = timeoutMs
        });
    }

    public static async Task WaitForElementToDisappearAsync(IPage page, string selector, int timeoutMs = 10000)
    {
        await page.Locator(selector).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = timeoutMs
        });
    }

    public static async Task RetryAsync(Func<Task> action, int maxRetries = 2, int delayMs = 1000)
    {
        for (int attempt = 1; attempt <= maxRetries + 1; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch when (attempt <= maxRetries)
            {
                await Task.Delay(delayMs);
            }
        }
    }
}
