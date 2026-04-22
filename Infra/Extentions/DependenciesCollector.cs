using System.Text;

using Application;
using Application.Contracts;
using Application.Contracts.IService;

using Domain.Entities;

using FluentValidation;

using Infra.Contracts_Imp;
using Infra.Presistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Quartz;
namespace Infra.Extentions;
public static class DependenciesCollector
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        #region Cloudinary
        var CloudinaryName = Environment.GetEnvironmentVariable("CloudinaryName");
        var CloudinaryApiKey = Environment.GetEnvironmentVariable("CloudinaryApiKey");
        var CloudinaryApiSecret = Environment.GetEnvironmentVariable("CloudinaryApiSecret");
        var account = new CloudinaryDotNet.Account(
        CloudinaryName,
        CloudinaryApiKey,
        CloudinaryApiSecret
        );
        services.AddTransient<CloudinaryDotNet.Cloudinary>(_ => new CloudinaryDotNet.Cloudinary(account));
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        #endregion

        var issuer = Environment.GetEnvironmentVariable("SaasJWTIssuer");
        var audience = Environment.GetEnvironmentVariable("SaasJWTAudience");
        var jwtKey = Environment.GetEnvironmentVariable("SaasJwtKey");
        var assembly = typeof(IApplicationHandlerMarker).Assembly;
        var DatabaseConfig = Environment.GetEnvironmentVariable("TalkRealConfig");
        services.AddDbContext<ApplicationDbContext>
         (options =>
         {
             options.UseNpgsql(DatabaseConfig);
         });
        services.AddScoped<ITalkRealServices, TalkRealServices>();
        services.AddSignalR();
        services.AddGrpc();
        services.AddScoped<IUnitofWork, UnitOfWork>();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }, typeof(AutoMapperProfile).Assembly);

        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        #region Mediator
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(assembly);
        #endregion

        #region Auth
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
         .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/presence"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            });

        #endregion

        #region Quartz outbox
        services.AddQuartz(q =>
{
    var jobKey = new JobKey("ProcessOutboxMessagesJob");
    q.AddJob<ProcessOutboxMessagesJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ProcessOutboxMessagesJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(5)
            .RepeatForever()));
});
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        #endregion

        return services;
    }
}

