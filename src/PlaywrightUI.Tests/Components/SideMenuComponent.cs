using Allure.Net.Commons;
using Microsoft.Playwright;
using Serilog;

namespace PlaywrightUI.Tests.Components;

public sealed class SideMenuComponent
{
    private readonly IPage _page;
    private readonly ILogger _logger;

    public SideMenuComponent(IPage page, ILogger logger)
    {
        _page = page;
        _logger = logger;
    }

    private static readonly IReadOnlyDictionary<string, string> MenuItems = new Dictionary<string, string>
    {
        ["Admin"] = "/web/index.php/admin/viewSystemUsers",
        ["PIM"] = "/web/index.php/pim/viewEmployeeList",
        ["Leave"] = "/web/index.php/leave/viewLeaveList",
        ["Time"] = "/web/index.php/time/viewEmployeeTimesheet",
        ["Recruitment"] = "/web/index.php/recruitment/viewRecruitmentModule",
        ["Performance"] = "/web/index.php/performance/searchPerformanceReview",
        ["Dashboard"] = "/web/index.php/dashboard/index",
        ["Directory"] = "/web/index.php/directory/viewDirectory",
        ["Buzz"] = "/web/index.php/buzz/viewBuzz"
    };

    public async Task NavigateToAsync(string menuItemName)
    {
        _logger.Information("Navigating to menu item: {MenuItem}", menuItemName);
        await AllureApi.Step($"Navigate via side menu to '{menuItemName}'", async () =>
        {
            var navItem = _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = menuItemName }).First;
            await navItem.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        });
    }

    public async Task<bool> IsMenuItemVisibleAsync(string menuItemName)
    {
        return await _page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = menuItemName }).First.IsVisibleAsync();
    }

    public async Task<List<string>> GetAllMenuItemNamesAsync()
    {
        var navItems = _page.Locator(".oxd-nav-text");
        var count = await navItems.CountAsync();
        var names = new List<string>();
        for (int i = 0; i < count; i++)
        {
            names.Add(await navItems.Nth(i).InnerTextAsync());
        }
        return names;
    }

    public async Task<bool> IsMenuItemActiveAsync(string menuItemName)
    {
        var navItem = _page.Locator($".oxd-nav-item:has(.oxd-nav-text:text-is(\"{menuItemName}\"))");
        var classAttr = await navItem.First.GetAttributeAsync("class") ?? string.Empty;
        return classAttr.Contains("active", StringComparison.OrdinalIgnoreCase);
    }
}
