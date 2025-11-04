using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Gemona.Infrastructure.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Gemona.API.Middlewares;
using Serilog;

// Configuração inicial do Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Gemona.API")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/gemona-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("Iniciando aplicação Gemona API");

var builder = WebApplication.CreateBuilder(args);

// Adiciona Serilog ao host
builder.Host.UseSerilog();

// Configuração HTTPS
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 5269;
});

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
                "https://gemona.com.br",
                "https://www.gemona.com.br",
                "https://app.gemona.com.br"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:4200",
                "http://localhost:5173",
                "http://localhost:8080"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

// Configuração FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Gemona.Application.Validators.Categoria.CreateCategoriaRequestValidator>();

builder.Services.AddEndpointsApiExplorer();

// Configuração Swagger com autenticação JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gemona API",
        Version = "v1",
        Description = "API para gerenciamento de serviços e estabelecimentos",
        Contact = new OpenApiContact
        {
            Name = "Gemona Team",
            Email = "contato@gemona.com.br"
        }
    });

    // Configuração de segurança JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

// Configuração JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Em produção, mudar para true
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    // Configuração de eventos para debugging (opcional)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                success = false,
                message = "Não autorizado. Token inválido ou ausente."
            });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware de tratamento global de exceções (deve ser o primeiro)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gemona API V1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Gemona API - Documentação";
    });
    
    // CORS permissivo em desenvolvimento
    app.UseCors("Development");
}
else
{
    // CORS restritivo em produção
    app.UseCors("Production");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}