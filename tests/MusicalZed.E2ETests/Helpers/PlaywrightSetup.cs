namespace MusicalZed.E2ETests.Helpers;

using Microsoft.Playwright;

public static class PlaywrightSetup
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:5002";
}
