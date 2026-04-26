using API;
using API.graphqlAPis;

using Infra.Extentions;
using Infra.Presistence;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddGraphQLServer()
    .AddQueryType<GraphQLApis>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();
builder.Services.AddScoped<GraphQLApis>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelPolicy", policy =>
    {
        policy.WithOrigins("https://*.vercel.app", "http://localhost:4200")
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHttpsRedirection();
}
app.UseCors(policyName: "VercelPolicy");
app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database Migration Failed!");
    }
}
app.MapHub<PresenceHub>("/api/hubs/presence");
app.MapGrpcService<PresenceGrpcService>();
app.MapGraphQL();
await app.RunAsync();
