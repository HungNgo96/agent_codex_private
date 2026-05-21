namespace AgentTeams.SampleApi.Application.Employees;

public enum EmployeeResultStatus
{
    Success,
    ValidationFailed,
    NotFound,
    Conflict
}

public sealed record EmployeeResult<T>
{
    private EmployeeResult(
        EmployeeResultStatus status,
        T? value,
        IReadOnlyDictionary<string, string[]> validationErrors,
        string? code,
        string? message)
    {
        Status = status;
        Value = value;
        ValidationErrors = validationErrors;
        Code = code;
        Message = message;
    }

    public EmployeeResultStatus Status { get; }

    public T? Value { get; }

    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

    public string? Code { get; }

    public string? Message { get; }

    public static EmployeeResult<T> Success(T value)
    {
        return new EmployeeResult<T>(
            EmployeeResultStatus.Success,
            value,
            new Dictionary<string, string[]>(),
            null,
            null);
    }

    public static EmployeeResult<T> ValidationFailed(IReadOnlyDictionary<string, string[]> validationErrors)
    {
        return new EmployeeResult<T>(
            EmployeeResultStatus.ValidationFailed,
            default,
            validationErrors,
            null,
            null);
    }

    public static EmployeeResult<T> NotFound()
    {
        return new EmployeeResult<T>(
            EmployeeResultStatus.NotFound,
            default,
            new Dictionary<string, string[]>(),
            null,
            null);
    }

    public static EmployeeResult<T> Conflict(string code, string message)
    {
        return new EmployeeResult<T>(
            EmployeeResultStatus.Conflict,
            default,
            new Dictionary<string, string[]>(),
            code,
            message);
    }
}

public sealed record EmployeeResult
{
    private EmployeeResult(EmployeeResultStatus status)
    {
        Status = status;
    }

    public EmployeeResultStatus Status { get; }

    public static EmployeeResult Success()
    {
        return new EmployeeResult(EmployeeResultStatus.Success);
    }

    public static EmployeeResult NotFound()
    {
        return new EmployeeResult(EmployeeResultStatus.NotFound);
    }
}
