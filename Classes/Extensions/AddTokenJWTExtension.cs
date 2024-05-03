using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SocialNetwork.Extensions
{
    public static class AddTokenJWTExtension
    {
        public static IServiceCollection AddJWTAuthorization(this IServiceCollection services,
            IWebHostEnvironment currentEnvironment,
            string issuer,
            string audience,
            string encryptingKey,
            string signingKey
            )
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer((options) =>
               {
                   options.RequireHttpsMetadata = true;
                   options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuer = issuer,
                       ValidateAudience = true,
                       ValidAudience = audience,
                       ValidateLifetime = true,
                       TokenDecryptionKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(encryptingKey)),
                       IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(signingKey)),
                       ValidateIssuerSigningKey = true,                       
                   };

                   if (currentEnvironment.EnvironmentName == "Test")
                   {
                       options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                   }



               });


            return services;
        }
    }
}
