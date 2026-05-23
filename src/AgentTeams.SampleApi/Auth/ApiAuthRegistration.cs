using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AgentTeams.SampleApi.Auth;

public static class ApiAuthRegistration
{
    public static IServiceCollection AddApiAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authOptions = configuration
            .GetSection(ApiAuthOptions.SectionName)
            .Get<ApiAuthOptions>() ?? new ApiAuthOptions();

        authOptions.Validate();

        services.AddSingleton(authOptions);

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(authOptions.SigningKey));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(ApiAuthOptions.EmployeeWriterPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.FindAll("scope")
                        .SelectMany(claim => claim.Value.Split(
                            ' ',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        .Contains(ApiAuthOptions.EmployeeWriteScope, StringComparer.Ordinal));
            });
        });

        return services;
    }
}
