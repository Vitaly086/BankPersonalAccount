using BankAccountService.DbContext;
using BankAccountService.GrpsServices;
using BankAccountService.Services.Implementations;
using BankAccountService.Services.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigureKestrel(builder);
AddServices(builder.Services, builder.Configuration);

var app = builder.Build();
ConfigureGrpcServices(app);

app.Run();


void ConfigureKestrel(WebApplicationBuilder builder)
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5000, o => o.Protocols = HttpProtocols.Http2);
    });
}

void AddServices(IServiceCollection services, IConfiguration configuration)
{
    AddDbContext(services, configuration);
    services.AddGrpc();
    services.AddLogging();
    
    services.AddScoped<IAuthorizationService, AuthorizationService>();
    services.AddScoped<IClientAccountRepository, ClientAccountAccountRepository>();}

void AddDbContext(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<BankAccountContext>(o => o.UseNpgsql(connectionString));
}

void ConfigureGrpcServices(WebApplication app)
{
    app.MapGrpcService<AuthorizationGrpcService>();
    app.MapGrpcService<AccountDetailsGrpcService>();
}