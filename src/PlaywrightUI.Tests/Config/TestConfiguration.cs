using Microsoft.Extensions.Configuration;

namespace PlaywrightUI.Tests.Config;

public static class TestConfiguration
{
    private static readonly IConfiguration Configuration;

    static TestConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "local";

        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"Config/appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables("TEST_")
            .Build();
    }

    public static BrowserConfig Browser =>
        Configuration.GetSection("Browser").Get<BrowserConfig>() ?? new BrowserConfig();

    public static ApplicationConfig Application =>
        Configuration.GetSection("Application").Get<ApplicationConfig>() ?? new ApplicationConfig();

    public static TestConfig Test =>
        Configuration.GetSection("Test").Get<TestConfig>() ?? new TestConfig();

    public static LoggingConfig Logging =>
        Configuration.GetSection("Logging").Get<LoggingConfig>() ?? new LoggingConfig();
}

public record BrowserConfig
{
    public string Type { get; init; } = "Chromium";
    public bool Headless { get; init; } = true;
    public int SlowMo { get; init; } = 0;
    public float Timeout { get; init; } = 30000;
    public float NavigationTimeout { get; init; } = 30000;
    public int ViewportWidth { get; init; } = 1920;
    public int ViewportHeight { get; init; } = 1080;
    public bool RecordVideo { get; init; } = false;
    public bool CaptureTrace { get; init; } = false;
}

public record ApplicationConfig
{
    public string BaseUrl { get; init; } = "https://opensource-demo.orangehrmlive.com";
    public string AdminUsername { get; init; } = "Admin";
    public string AdminPassword { get; init; } = "admin123";
}

public record TestConfig
{
    public int RetryCount { get; init; } = 2;
    public bool ScreenshotOnFailure { get; init; } = true;
    public bool VideoOnFailure { get; init; } = false;
    public bool TraceOnFailure { get; init; } = true;
}

public record LoggingConfig
{
    public string MinimumLevel { get; init; } = "Information";
    public string LogDirectory { get; init; } = "TestResults/Logs";
}
