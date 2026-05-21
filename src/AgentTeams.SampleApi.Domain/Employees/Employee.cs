namespace AgentTeams.SampleApi.Domain.Employees;

public sealed class Employee
{
    private Employee()
    {
    }

    public Guid Id { get; private set; }

    public string EmployeeCode { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Department { get; private set; } = string.Empty;

    public string JobTitle { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static EmployeeDomainResult<Employee> Create(
        string employeeCode,
        string fullName,
        string email,
        string department,
        string jobTitle,
        DateTimeOffset now)
    {
        var errors = ValidateFields(fullName, email, department, jobTitle);

        if (string.IsNullOrWhiteSpace(employeeCode))
        {
            errors["employeeCode"] = ["Employee code is required."];
        }
        else if (employeeCode.Trim().Length > 32)
        {
            errors["employeeCode"] = ["Employee code must be 32 characters or fewer."];
        }

        if (errors.Count > 0)
        {
            return EmployeeDomainResult<Employee>.Invalid(errors);
        }

        return EmployeeDomainResult<Employee>.Valid(new Employee
        {
            Id = Guid.NewGuid(),
            EmployeeCode = employeeCode.Trim(),
            FullName = fullName.Trim(),
            Email = email.Trim(),
            Department = department.Trim(),
            JobTitle = jobTitle.Trim(),
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        });
    }

    public static Employee Rehydrate(
        Guid id,
        string employeeCode,
        string fullName,
        string email,
        string department,
        string jobTitle,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        return new Employee
        {
            Id = id,
            EmployeeCode = employeeCode,
            FullName = fullName,
            Email = email,
            Department = department,
            JobTitle = jobTitle,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public EmployeeDomainResult<Employee> Update(
        string fullName,
        string email,
        string department,
        string jobTitle,
        bool isActive,
        DateTimeOffset now)
    {
        var errors = ValidateFields(fullName, email, department, jobTitle);
        if (errors.Count > 0)
        {
            return EmployeeDomainResult<Employee>.Invalid(errors);
        }

        FullName = fullName.Trim();
        Email = email.Trim();
        Department = department.Trim();
        JobTitle = jobTitle.Trim();
        IsActive = isActive;
        UpdatedAt = now;

        return EmployeeDomainResult<Employee>.Valid(this);
    }

    private static Dictionary<string, string[]> ValidateFields(
        string fullName,
        string email,
        string department,
        string jobTitle)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(fullName))
        {
            errors["fullName"] = ["Full name is required."];
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            errors["email"] = ["A valid email is required."];
        }

        if (string.IsNullOrWhiteSpace(department))
        {
            errors["department"] = ["Department is required."];
        }

        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            errors["jobTitle"] = ["Job title is required."];
        }

        return errors;
    }
}
