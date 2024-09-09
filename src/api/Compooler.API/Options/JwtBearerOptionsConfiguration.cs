using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Compooler.API.Options;

public class JwtBearerOptionsConfiguration(
    IConfiguration configuration,
    IWebHostEnvironment environment
) : IConfigureNamedOptions<JwtBearerOptions>
{
    private record JwtOptions(string Authority, string Audience)
    {
        public static string SectionName = "Jwt";
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.MapInboundClaims = false;

        if (environment.IsDevelopment())
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateActor = false,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = false,
                ValidateTokenReplay = false,
                SignatureValidator = (token, _) => new JsonWebToken(token)
            };
        }
        else
        {
            var jwtOptions =
                configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("Could not get the Jwt options");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = jwtOptions.Audience
            };

            options.Authority = jwtOptions.Authority;
        }

        options.TokenValidationParameters.NameClaimType = "user_id";
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }
}
