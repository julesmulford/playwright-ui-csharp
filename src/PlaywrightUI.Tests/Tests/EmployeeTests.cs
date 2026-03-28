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
[AllureSuite("Employee Management")]
[AllureFeature("Employee CRUD")]
public sealed class EmployeeTests : BrowserFixture
{
    private LoginPage _loginPage = null!;
    private DashboardPage _dashboardPage = null!;
    private EmployeeListPage _employeeListPage = null!;
    private AddEmployeePage _addEmployeePage = null!;

    [SetUp]
    public new async Task SetUpAsync()
    {
        await base.SetUpAsync();
        _loginPage = new LoginPage(Page, Logger);
        _dashboardPage = new DashboardPage(Page, Logger);
        _employeeListPage = new EmployeeListPage(Page, Logger);
        _addEmployeePage = new AddEmployeePage(Page, Logger);

        await _loginPage.NavigateAsync();
        await _loginPage.LoginAsAsync(TestDataFactory.AdminUser);
        await _dashboardPage.WaitForLoadAsync();
    }

    [Test]
    [AllureStory("Create Employee")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Admin should be able to create a new employee via the PIM module")]
    public async Task Employee_Create_ShouldSucceedAndAppearInList()
    {
        var employee = TestDataFactory.NewEmployee();
        Logger.Information("Creating employee: {FullName}", employee.FullName);

        await _employeeListPage.NavigateAsync();
        await _employeeListPage.ClickAddEmployeeAsync();
        await _addEmployeePage.FillAndSaveAsync(employee);

        await _employeeListPage.NavigateAsync();
        await _employeeListPage.SearchByNameAsync(employee.LastName);

        (await _employeeListPage.IsEmployeeInListAsync(employee.LastName))
            .Should().BeTrue($"newly created employee '{employee.FullName}' should appear in search results");
    }

    [Test]
    [AllureStory("Delete Employee")]
    [AllureSeverity(SeverityLevel.critical)]
    [Description("Admin should be able to delete an employee and they should no longer appear in the list")]
    public async Task Employee_Delete_ShouldRemoveFromList()
    {
        var employee = TestDataFactory.NewEmployee();

        // Create first
        await _employeeListPage.NavigateAsync();
        await _employeeListPage.ClickAddEmployeeAsync();
        await _addEmployeePage.FillAndSaveAsync(employee);

        // Delete
        await _employeeListPage.NavigateAsync();
        await _employeeListPage.SearchByNameAsync(employee.LastName);
        await _employeeListPage.DeleteEmployeeByNameAsync(employee.LastName);

        // Verify
        await _employeeListPage.SearchByNameAsync(employee.LastName);
        (await _employeeListPage.IsEmployeeInListAsync(employee.LastName))
            .Should().BeFalse($"employee '{employee.FullName}' should not appear after deletion");
    }

    [Test]
    [AllureStory("Search Employees")]
    [AllureSeverity(SeverityLevel.normal)]
    public async Task Employee_Search_ShouldFilterResults()
    {
        await _employeeListPage.NavigateAsync();
        var initialCount = await _employeeListPage.GetRowCountAsync();

        // Search for "Admin" which should find the admin user record
        await _employeeListPage.SearchByNameAsync("Admin");
        var filteredCount = await _employeeListPage.GetRowCountAsync();

        filteredCount.Should().BeLessOrEqualTo(initialCount,
            "search should narrow results or keep same count");
    }

    [Test]
    [AllureStory("Employee List Loads")]
    [AllureSeverity(SeverityLevel.normal)]
    [Category(AppConstants.Categories.Smoke)]
    public async Task EmployeeList_ShouldLoadWithRecords()
    {
        await _employeeListPage.NavigateAsync();
        var count = await _employeeListPage.GetRowCountAsync();
        count.Should().BeGreaterThan(0, "the employee list should contain at least one record");
    }

    [Test]
    [AllureStory("Add Employee Form Validation")]
    [AllureSeverity(SeverityLevel.normal)]
    [Category(AppConstants.Categories.Regression)]
    public async Task AddEmployee_WithEmptyFirstName_ShouldShowValidationError()
    {
        await _employeeListPage.NavigateAsync();
        await _employeeListPage.ClickAddEmployeeAsync();
        await _addEmployeePage.WaitForLoadAsync();

        // Submit without filling required fields
        await _addEmployeePage.SaveAsync();

        var errorVisible = await Page.Locator(".oxd-input-field-error-message").First.IsVisibleAsync();
        errorVisible.Should().BeTrue("form validation errors should appear when required fields are empty");
    }
}
