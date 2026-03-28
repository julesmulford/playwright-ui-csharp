namespace PlaywrightUI.Tests.Constants;

public static class AppConstants
{
    public static class Urls
    {
        public const string Login = "/web/index.php/auth/login";
        public const string Dashboard = "/web/index.php/dashboard/index";
        public const string EmployeeList = "/web/index.php/pim/viewEmployeeList";
        public const string AddEmployee = "/web/index.php/pim/addEmployee";
        public const string Logout = "/web/index.php/auth/logout";
    }

    public static class Selectors
    {
        public const string UsernameInput = "input[name=\"username\"]";
        public const string PasswordInput = "input[name=\"password\"]";
        public const string SubmitButton = "button[type=\"submit\"]";
        public const string LoginErrorMessage = ".oxd-alert-content-text";
        public const string SideNavItem = ".oxd-nav-item";
        public const string NavText = ".oxd-nav-text";
        public const string DashboardHeading = "h6.oxd-text--h6";
        public const string UserDropdown = ".oxd-userdropdown-tab";
        public const string LogoutLink = "a:has-text(\"Logout\")";
        public const string EmployeeSearchInput = "input[placeholder=\"Type for hints...\"]";
        public const string AddButton = "button:has-text(\"Add\")";
        public const string TableRows = ".oxd-table-body .oxd-table-row";
        public const string FirstNameInput = "input[name=\"firstName\"]";
        public const string LastNameInput = "input[name=\"lastName\"]";
        public const string SaveButton = "button[type=\"submit\"]:has-text(\"Save\")";
        public const string SuccessToast = ".oxd-toast--success";
        public const string DeleteConfirmButton = ".orangehrm-dialog-popup button:has-text(\"Yes, Delete\")";
        public const string TableDeleteButton = "button[title=\"Delete\"]";
        public const string TableEditButton = "button[title=\"Edit\"]";
        public const string SearchButton = "button[type=\"submit\"]:has-text(\"Search\")";
    }

    public static class Timeouts
    {
        public const int DefaultMs = 30000;
        public const int NavigationMs = 30000;
        public const int ShortMs = 5000;
        public const int ToastMs = 10000;
    }

    public static class Categories
    {
        public const string Smoke = "Smoke";
        public const string Regression = "Regression";
        public const string EndToEnd = "EndToEnd";
    }

    public static class TestMessages
    {
        public const string InvalidCredentials = "Invalid credentials";
        public const string RequiredField = "Required";
    }
}
