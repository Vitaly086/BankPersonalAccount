using System.Text;
using BankAccountGateway;
using BankAccountGateway.Middlewares;
using BankAccountService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

AddServices(builder.Services);
AddGrpcClients(builder.Services, builder.Configuration);
ConfigureAuthentication(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureApplication(app);

app.Run();


void AddServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddLogging();

    services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });
}

void AddGrpcClients(IServiceCollection services, IConfiguration configuration)
{
    var grpcServer = configuration.GetValue<string>("GrpcServer");
    services.AddGrpcClient<AuthorizationGrpc.AuthorizationGrpcClient>(options =>
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };
        options.Address = new Uri(grpcServer);
        options.ChannelOptionsActions.Add(o => o.HttpHandler = httpClientHandler);
    });
    services.AddGrpcClient<AccountDetailsGrpc.AccountDetailsGrpcClient>(options =>
    {
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };
        options.Address = new Uri(grpcServer);
        options.ChannelOptionsActions.Add(channelOptions => { channelOptions.HttpHandler = httpClientHandler; });
    });
}


void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtKey = configuration.GetSection("JwtToken").Value;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateAudience = false,
                ValidateIssuer = false
            };
        });
    services.AddAuthorization();
}

void ConfigureApplication(WebApplication app)
{
    app.UseCustomExceptionHandler();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}