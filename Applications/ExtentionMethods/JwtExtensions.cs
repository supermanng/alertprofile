using System;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Applications.ExtentionMethods
{
    public static class JwtExtensions
    {



        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = Configuration.GetValue<bool>("Jwt:ValidateSigningKey"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Jwt:SecurityKey"))),
                    ValidateIssuer = Configuration.GetValue<bool>("Jwt:ValidateIssuer"),
                    ValidIssuer = Configuration.GetValue<string>("Jwt:Issuer"),
                    ValidateAudience = Configuration.GetValue<bool>("Jwt:ValidateAudience"),
                    ValidAudience = Configuration.GetValue<string>("Jwt:Audience"),
                    ValidateLifetime = Configuration.GetValue<bool>("Jwt:ValidateLifeTime"),
                    ClockSkew = TimeSpan.FromMinutes(Configuration.GetValue<int>("Jwt:DateToleranceMinutes")) //5 minute tolerance for the expiration date
                };
            });

            //services.AddPolicy("OnlyMobifinAccess", "AllowCoreBanking", "Mobifin");
            //services.AddPolicy("OnlyAgentAccess", "CustomerType", CustomerType.AGENT.ToString());
            return services;
        }
    }
}
