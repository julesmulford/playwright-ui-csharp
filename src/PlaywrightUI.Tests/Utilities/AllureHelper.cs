using Allure.Net.Commons;
using Microsoft.Playwright;

namespace PlaywrightUI.Tests.Utilities;

public static class AllureHelper
{
    public static async Task AttachScreenshotAsync(IPage page, string name = "screenshot")
    {
        try
        {
            var screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions
            {
                FullPage = false,
                Type = ScreenshotType.Png
            });
            AllureApi.AddAttachment(name, "image/png", screenshotBytes);
        }
        catch (Exception ex)
        {
            Log(ex, "Failed to capture screenshot");
        }
    }

    public static void AttachText(string name, string content, string mimeType = "text/plain")
    {
        AllureApi.AddAttachment(name, mimeType, System.Text.Encoding.UTF8.GetBytes(content));
    }

    public static void AttachFile(string filePath, string name, string mimeType = "application/octet-stream")
    {
        if (!File.Exists(filePath)) return;
        var bytes = File.ReadAllBytes(filePath);
        AllureApi.AddAttachment(name, mimeType, bytes, Path.GetExtension(filePath));
    }

    public static async Task AttachHtmlAsync(IPage page, string name = "page-html")
    {
        try
        {
            var html = await page.ContentAsync();
            AllureApi.AddAttachment(name, "text/html", System.Text.Encoding.UTF8.GetBytes(html));
        }
        catch (Exception ex)
        {
            Log(ex, "Failed to capture page HTML");
        }
    }

    private static void Log(Exception ex, string message) =>
        Console.Error.WriteLine($"[AllureHelper] {message}: {ex.Message}");
}
