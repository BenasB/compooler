using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Compooler.API.Options;

public class JwtBearerOptionsConfiguration(IConfiguration configuration)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private record JwtOptions(string Issuer, string Audience)
    {
        public static string SectionName = "Jwt";
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        var jwtOptions =
            configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Could not get the Jwt options");

        options.Authority = "https://securetoken.google.com/compooler-434805";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ValidateAudience = false,
            // ValidateIssuer = false,
            // ValidateIssuerSigningKey = false,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            // SignatureValidator = (token, _) => new JsonWebToken(token)
            NameClaimType = ClaimTypes.NameIdentifier
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            },
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }
}
