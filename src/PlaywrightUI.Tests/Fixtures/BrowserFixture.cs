using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightUI.Tests.Config;
using PlaywrightUI.Tests.Utilities;
using Serilog;
using Serilog.Context;

namespace PlaywrightUI.Tests.Fixtures;

[AllureNUnit]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class BrowserFixture
{
    protected IPlaywright PlaywrightInstance { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected ILogger Logger { get; private set; } = null!;

    private string _correlationId = string.Empty;
    private string? _videoPath;
    private string? _tracePath;

    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        var logDir = TestConfiguration.Logging.LogDirectory;
        Directory.CreateDirectory(logDir);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(ParseLogLevel(TestConfiguration.Logging.MinimumLevel))
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                Path.Combine(logDir, "test-.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        PlaywrightInstance = await Playwright.CreateAsync();
        var config = TestConfiguration.Browser;

        Browser = config.Type.ToLowerInvariant() switch
        {
            "firefox" => await PlaywrightInstance.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = config.Headless,
                SlowMo = config.SlowMo
            }),
            "webkit" => await PlaywrightInstance.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = config.Headless,
                SlowMo = config.SlowMo
            }),
            _ => await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = config.Headless,
                SlowMo = config.SlowMo
            })
        };
    }

    [SetUp]
    public async Task SetUpAsync()
    {
        _correlationId = Guid.NewGuid().ToString("N")[..12];
        Logger = Log.ForContext("CorrelationId", _correlationId);

        using var _ = LogContext.PushProperty("CorrelationId", _correlationId);
        Logger.Information("Starting test: {TestName}", TestContext.CurrentContext.Test.FullName);

        var config = TestConfiguration.Browser;
        var testConfig = TestConfiguration.Test;

        var contextOptions = new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = config.ViewportWidth,
                Height = config.ViewportHeight
            },
            IgnoreHTTPSErrors = true
        };

        if (testConfig.VideoOnFailure || config.RecordVideo)
        {
            var videoDir = Path.Combine("TestResults", "Videos", _correlationId);
            Directory.CreateDirectory(videoDir);
            contextOptions.RecordVideoDir = videoDir;
            contextOptions.RecordVideoSize = new RecordVideoSize { Width = config.ViewportWidth, Height = config.ViewportHeight };
        }

        Context = await Browser.NewContextAsync(contextOptions);
        Context.SetDefaultTimeout(config.Timeout);
        Context.SetDefaultNavigationTimeout(config.NavigationTimeout);

        if (testConfig.TraceOnFailure || config.CaptureTrace)
        {
            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true,
                Title = TestContext.CurrentContext.Test.FullName
            });
        }

        Page = await Context.NewPageAsync();

        Page.Console += (_, msg) => Logger.Debug("[Browser Console] {Type}: {Text}", msg.Type, msg.Text);
        Page.PageError += (_, error) => Logger.Error("[Browser Error] {Error}", error);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;
        var testName = TestContext.CurrentContext.Test.FullName;
        var config = TestConfiguration.Test;

        using var _ = LogContext.PushProperty("CorrelationId", _correlationId);

        if (outcome == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            Logger.Warning("Test FAILED: {TestName}", testName);

            if (config.ScreenshotOnFailure)
            {
                await AllureHelper.AttachScreenshotAsync(Page, $"failure_{_correlationId}");
            }
        }
        else
        {
            Logger.Information("Test PASSED: {TestName}", testName);
        }

        if (config.TraceOnFailure || TestConfiguration.Browser.CaptureTrace)
        {
            _tracePath = Path.Combine("TestResults", "Traces", $"{_correlationId}.zip");
            Directory.CreateDirectory(Path.GetDirectoryName(_tracePath)!);
            await Context.Tracing.StopAsync(new TracingStopOptions { Path = _tracePath });

            if (outcome == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                AllureHelper.AttachFile(_tracePath, "trace.zip", "application/zip");
            }
        }

        await Page.CloseAsync();
        await Context.CloseAsync();

        if (config.VideoOnFailure)
        {
            var video = Page.Video;
            if (video is not null)
            {
                _videoPath = await video.PathAsync();
                if (outcome == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    AllureHelper.AttachFile(_videoPath, "video.webm", "video/webm");
                }
            }
        }
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        await Browser.CloseAsync();
        PlaywrightInstance.Dispose();
        await Log.CloseAndFlushAsync();
    }

    private static Serilog.Events.LogEventLevel ParseLogLevel(string level) =>
        level.ToLowerInvariant() switch
        {
            "debug" => Serilog.Events.LogEventLevel.Debug,
            "warning" or "warn" => Serilog.Events.LogEventLevel.Warning,
            "error" => Serilog.Events.LogEventLevel.Error,
            _ => Serilog.Events.LogEventLevel.Information
        };
}
