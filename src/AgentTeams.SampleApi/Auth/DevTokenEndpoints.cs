using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AgentTeams.SampleApi.Auth;

public static class DevTokenEndpoints
{
    public static IEndpointRouteBuilder MapDevTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/v1/auth/dev-token",
            Ok<DevTokenResponse> (DevTokenRequest? request, ApiAuthOptions options) =>
            {
                var expiresAt = DateTimeOffset.UtcNow.AddHours(1);
                var scopes = request?.Scopes is { Count: > 0 }
                    ? request.Scopes
                    : [ApiAuthOptions.EmployeeWriteScope];

                var claims = new Dictionary<string, object>
                {
                    [JwtRegisteredClaimNames.Sub] = string.IsNullOrWhiteSpace(request?.Subject)
                        ? "agent"
                        : request.Subject,
                    [JwtRegisteredClaimNames.Name] = string.IsNullOrWhiteSpace(request?.Name)
                        ? "Agent"
                        : request.Name,
                    ["scope"] = string.Join(' ', scopes)
                };

                var descriptor = new SecurityTokenDescriptor
                {
                    Issuer = options.Issuer,
                    Audience = options.Audience,
                    Claims = claims,
                    Expires = expiresAt.UtcDateTime,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                        SecurityAlgorithms.HmacSha256)
                };

                var token = new JsonWebTokenHandler().CreateToken(descriptor);

                return TypedResults.Ok(new DevTokenResponse(
                    token,
                    "Bearer",
                    expiresAt));
            })
            .WithName("CreateDevToken")
            .WithSummary("Create a development-only bearer token");

        return endpoints;
    }
}

public sealed record DevTokenRequest(
    string? Subject,
    string? Name,
    IReadOnlyList<string>? Scopes);

public sealed record DevTokenResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt);
