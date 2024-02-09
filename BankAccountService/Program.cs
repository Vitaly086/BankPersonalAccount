using BankAccountService.DbContext;
using BankAccountService.GrpsServices;
using BankAccountService.Services.Implementations;
using BankAccountService.Services.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000, o => o.Protocols =
        HttpProtocols.Http2);
});


var services = builder.Services;

services.AddDbContext<BankAccountContext>(o =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    o.UseNpgsql(connectionString);
});
services.AddGrpc();

services.AddScoped<IAuthorizationService, AuthorizationService>();
services.AddScoped<IClientAccountRepository, ClientAccountAccountRepository>();


var app = builder.Build();

app.MapGrpcService<AuthorizationGrpcService>();
app.MapGrpcService<AccountDetailsGrpcService>();

app.Run();