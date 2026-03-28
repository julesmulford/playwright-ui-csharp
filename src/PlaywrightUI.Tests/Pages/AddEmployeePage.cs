using Allure.Net.Commons;
using Microsoft.Playwright;
using PlaywrightUI.Tests.Constants;
using PlaywrightUI.Tests.Models;
using Serilog;

namespace PlaywrightUI.Tests.Pages;

public sealed class AddEmployeePage : BasePage
{
    public AddEmployeePage(IPage page, ILogger logger) : base(page, logger) { }

    public async Task WaitForLoadAsync()
    {
        await Page.WaitForURLAsync(url => url.Contains("/pim/addEmployee"), new PageWaitForURLOptions
        {
            Timeout = AppConstants.Timeouts.NavigationMs
        });
        await WaitForSelectorAsync(AppConstants.Selectors.FirstNameInput);
    }

    public async Task FillEmployeeDetailsAsync(Employee employee)
    {
        Logger.Information("Filling employee details: {FullName}", employee.FullName);
        await AllureApi.Step($"Fill employee form for '{employee.FullName}'", async () =>
        {
            await FillAsync(AppConstants.Selectors.FirstNameInput, employee.FirstName, "first name");
            await FillAsync(AppConstants.Selectors.LastNameInput, employee.LastName, "last name");
            if (!string.IsNullOrWhiteSpace(employee.MiddleName))
            {
                await FillAsync("input[name=\"middleName\"]", employee.MiddleName, "middle name");
            }
        });
    }

    public async Task SaveAsync()
    {
        Logger.Information("Saving employee form");
        await AllureApi.Step("Save employee", async () =>
        {
            await ClickAsync(AppConstants.Selectors.SaveButton, "save");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        });
    }

    public async Task<bool> IsSuccessToastVisibleAsync()
    {
        try
        {
            await WaitForSelectorAsync(AppConstants.Selectors.SuccessToast, AppConstants.Timeouts.ToastMs);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task FillAndSaveAsync(Employee employee)
    {
        await WaitForLoadAsync();
        await FillEmployeeDetailsAsync(employee);
        await SaveAsync();
    }

    public async Task<string?> GetEmployeeIdAsync()
    {
        var idInput = Page.Locator(".employee-id input");
        if (await idInput.IsVisibleAsync())
            return await idInput.InputValueAsync();
        return null;
    }
}
