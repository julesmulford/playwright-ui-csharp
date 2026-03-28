using PlaywrightUI.Tests.Models;

namespace PlaywrightUI.Tests.Data;

public sealed class EmployeeBuilder
{
    private string _firstName = "Test";
    private string _lastName = $"Employee{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    private string? _middleName;
    private string? _employeeId;

    public EmployeeBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public EmployeeBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public EmployeeBuilder WithMiddleName(string middleName)
    {
        _middleName = middleName;
        return this;
    }

    public EmployeeBuilder WithEmployeeId(string employeeId)
    {
        _employeeId = employeeId;
        return this;
    }

    public EmployeeBuilder WithUniqueSuffix()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpper();
        _lastName = $"Auto{suffix}";
        return this;
    }

    public Employee Build() => new()
    {
        FirstName = _firstName,
        LastName = _lastName,
        MiddleName = _middleName,
        EmployeeId = _employeeId
    };

    public static EmployeeBuilder Create() => new();
}
