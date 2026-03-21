using API;

using Infra.Extentions;
using Infra.Presistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddServices();       
builder.Services.AddOpenApi();
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
}
app.UseCors(policyName: "VercelPolicy");
app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<PresenceHub>("/hubs/presence");
app.MapGrpcService<PresenceGrpcService>();
app.MapGraphQL();
app.Run();
