namespace AgentTeams.SampleApi.Application.Employees.Ports;

public interface IEmployeeClock
{
    DateTimeOffset UtcNow { get; }
}
