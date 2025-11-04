using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace Gemona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    /// <summary>
    /// Verifica o status geral da API e suas dependências
    /// </summary>
    /// <returns>Status detalhado de saúde da aplicação</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var stopwatch = Stopwatch.StartNew();
        var healthReport = await _healthCheckService.CheckHealthAsync();
        stopwatch.Stop();

        var response = new
        {
            status = healthReport.Status.ToString(),
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            version = "1.0.0",
            totalDuration = $"{stopwatch.ElapsedMilliseconds}ms",
            checks = healthReport.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description ?? "N/A",
                duration = $"{entry.Value.Duration.TotalMilliseconds}ms",
                tags = entry.Value.Tags,
                data = entry.Value.Data.Count > 0 ? entry.Value.Data : null,
                exception = entry.Value.Exception?.Message
            })
        };

        var statusCode = healthReport.Status switch
        {
            HealthStatus.Healthy => StatusCodes.Status200OK,
            HealthStatus.Degraded => StatusCodes.Status200OK,
            HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

        _logger.LogInformation(
            "Health check executado: Status={Status}, Duração={Duration}ms",
            healthReport.Status,
            stopwatch.ElapsedMilliseconds
        );

        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Verifica se a API está pronta para receber requisições (readiness probe)
    /// </summary>
    /// <returns>Status de prontidão da API</returns>
    [HttpGet("ready")]
    [AllowAnonymous]
    public async Task<IActionResult> Ready()
    {
        var healthReport = await _healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("db")
        );

        if (healthReport.Status == HealthStatus.Healthy)
        {
            return Ok(new
            {
                status = "Ready",
                message = "API está pronta para receber requisições",
                timestamp = DateTime.UtcNow
            });
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new
        {
            status = "Not Ready",
            message = "API não está pronta. Verificar dependências.",
            timestamp = DateTime.UtcNow,
            details = healthReport.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                error = e.Value.Exception?.Message
            })
        });
    }

    /// <summary>
    /// Verifica se a API está viva (liveness probe)
    /// </summary>
    /// <returns>Status básico de vida da API</returns>
    [HttpGet("live")]
    [AllowAnonymous]
    public IActionResult Live()
    {
        return Ok(new
        {
            status = "Alive",
            message = "API está respondendo",
            timestamp = DateTime.UtcNow,
            uptime = GetUptime()
        });
    }

    /// <summary>
    /// Retorna informações detalhadas sobre o sistema (apenas para admins)
    /// </summary>
    /// <returns>Informações do sistema</returns>
    [HttpGet("info")]
    [Authorize(Roles = "Admin")]
    public IActionResult Info()
    {
        var process = Process.GetCurrentProcess();

        return Ok(new
        {
            application = new
            {
                name = "Gemona API",
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                framework = Environment.Version.ToString(),
                uptime = GetUptime()
            },
            system = new
            {
                operatingSystem = Environment.OSVersion.ToString(),
                machineName = Environment.MachineName,
                processorCount = Environment.ProcessorCount,
                is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                is64BitProcess = Environment.Is64BitProcess
            },
            process = new
            {
                id = process.Id,
                name = process.ProcessName,
                startTime = process.StartTime,
                workingSet = $"{process.WorkingSet64 / 1024 / 1024} MB",
                privateMemory = $"{process.PrivateMemorySize64 / 1024 / 1024} MB",
                threads = process.Threads.Count
            },
            memory = new
            {
                totalMemory = $"{GC.GetTotalMemory(false) / 1024 / 1024} MB",
                gcGen0Collections = GC.CollectionCount(0),
                gcGen1Collections = GC.CollectionCount(1),
                gcGen2Collections = GC.CollectionCount(2)
            },
            timestamp = DateTime.UtcNow
        });
    }

    private static string GetUptime()
    {
        var uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
    }
}
