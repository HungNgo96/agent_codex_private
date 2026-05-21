namespace AgentTeams.SampleApi.Domain.Employees;

public sealed record EmployeeDomainResult<T>
{
    private EmployeeDomainResult(T? value, IReadOnlyDictionary<string, string[]> validationErrors)
    {
        Value = value;
        ValidationErrors = validationErrors;
    }

    public T? Value { get; }

    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

    public bool IsValid => ValidationErrors.Count == 0;

    public static EmployeeDomainResult<T> Valid(T value)
    {
        return new EmployeeDomainResult<T>(value, new Dictionary<string, string[]>());
    }

    public static EmployeeDomainResult<T> Invalid(IReadOnlyDictionary<string, string[]> validationErrors)
    {
        return new EmployeeDomainResult<T>(default, validationErrors);
    }
}
