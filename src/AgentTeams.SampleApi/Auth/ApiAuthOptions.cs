namespace AgentTeams.SampleApi.Auth;

public sealed class ApiAuthOptions
{
    public const string SectionName = "Auth";
    public const string EmployeeWriterPolicy = "EmployeeWriter";
    public const string EmployeeWriteScope = "employees.write";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SigningKey { get; init; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
        {
            throw new InvalidOperationException("Auth:Issuer must be configured.");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException("Auth:Audience must be configured.");
        }

        if (string.IsNullOrWhiteSpace(SigningKey) || SigningKey.Length < 32)
        {
            throw new InvalidOperationException("Auth:SigningKey must be configured with at least 32 characters.");
        }
    }
}
