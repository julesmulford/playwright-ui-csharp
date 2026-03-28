using Allure.Net.Commons;
using Microsoft.Playwright;
using PlaywrightUI.Tests.Constants;
using Serilog;

namespace PlaywrightUI.Tests.Pages;

public sealed class EmployeeListPage : BasePage
{
    public EmployeeListPage(IPage page, ILogger logger) : base(page, logger) { }

    public async Task NavigateAsync()
    {
        Logger.Information("Navigating to Employee List");
        await NavigateToAsync(AppConstants.Urls.EmployeeList);
        await WaitForSelectorAsync(".oxd-table");
    }

    public async Task SearchByNameAsync(string name)
    {
        Logger.Information("Searching for employee: {Name}", name);
        await AllureApi.Step($"Search for employee '{name}'", async () =>
        {
            await FillAsync(AppConstants.Selectors.EmployeeSearchInput, name, "employee name search");
            await ClickAsync(AppConstants.Selectors.SearchButton, "search");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        });
    }

    public async Task ClickAddEmployeeAsync()
    {
        Logger.Information("Clicking Add Employee button");
        await ClickAsync(AppConstants.Selectors.AddButton, "add employee");
        await Page.WaitForURLAsync(url => url.Contains("/pim/addEmployee"), new PageWaitForURLOptions
        {
            Timeout = AppConstants.Timeouts.NavigationMs
        });
    }

    public async Task<int> GetRowCountAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        return await Page.Locator(AppConstants.Selectors.TableRows).CountAsync();
    }

    public async Task<bool> IsEmployeeInListAsync(string employeeName)
    {
        var rows = Page.Locator(AppConstants.Selectors.TableRows);
        var count = await rows.CountAsync();
        for (int i = 0; i < count; i++)
        {
            var text = await rows.Nth(i).InnerTextAsync();
            if (text.Contains(employeeName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public async Task DeleteEmployeeByNameAsync(string employeeName)
    {
        Logger.Information("Deleting employee: {Name}", employeeName);
        await AllureApi.Step($"Delete employee '{employeeName}'", async () =>
        {
            var rows = Page.Locator(AppConstants.Selectors.TableRows);
            var count = await rows.CountAsync();
            for (int i = 0; i < count; i++)
            {
                var row = rows.Nth(i);
                var text = await row.InnerTextAsync();
                if (text.Contains(employeeName, StringComparison.OrdinalIgnoreCase))
                {
                    await row.Locator(AppConstants.Selectors.TableDeleteButton).ClickAsync();
                    await WaitForSelectorAsync(AppConstants.Selectors.DeleteConfirmButton);
                    await ClickAsync(AppConstants.Selectors.DeleteConfirmButton, "confirm delete");
                    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    return;
                }
            }
            throw new InvalidOperationException($"Employee '{employeeName}' not found in list for deletion.");
        });
    }

    public async Task ClickEditForEmployeeAsync(string employeeName)
    {
        var rows = Page.Locator(AppConstants.Selectors.TableRows);
        var count = await rows.CountAsync();
        for (int i = 0; i < count; i++)
        {
            var row = rows.Nth(i);
            var text = await row.InnerTextAsync();
            if (text.Contains(employeeName, StringComparison.OrdinalIgnoreCase))
            {
                await row.Locator(AppConstants.Selectors.TableEditButton).ClickAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return;
            }
        }
        throw new InvalidOperationException($"Employee '{employeeName}' not found in list for editing.");
    }
}
