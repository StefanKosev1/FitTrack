using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FitTrack.Web.Tests;

[TestClass]
public sealed class FitTrackWebTests
{
    private FitTrackWebApplicationFactory _factory = null!;

    [TestInitialize]
    public void Initialize()
    {
        _factory = new FitTrackWebApplicationFactory();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _factory.Dispose();
    }

    // Tests that the home page loads and shows the expected FitTrack content.
    [TestMethod]
    public async Task GetHomePage_ReturnsSuccessfulResponseWithFitTrackContent()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
        StringAssert.Contains(content, "FitTrack");
        StringAssert.Contains(content, "Join The Crew");
    }

    // Tests that anonymous users are redirected to login when opening the dashboard.
    [TestMethod]
    public async Task GetDashboard_WhenAnonymous_RedirectsToLogin()
    {
        using var client = CreateClientWithoutAutomaticRedirects();

        var response = await client.GetAsync("/Dashboard");

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        StringAssert.StartsWith(
            response.Headers.Location?.PathAndQuery,
            "/Account/Login?ReturnUrl=");
    }

    // Tests that registration signs in the user and allows access to the dashboard.
    [TestMethod]
    public async Task Register_WithValidDetails_AuthenticatesUserAndShowsDashboard()
    {
        using var client = CreateClientWithoutAutomaticRedirects();
        var email = $"integration-{Guid.NewGuid():N}@example.com";

        var registerPage = await client.GetAsync("/Account/Register");
        var registerHtml = await registerPage.Content.ReadAsStringAsync();
        var antiforgeryToken = GetAntiforgeryToken(registerHtml);

        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Input.FullName"] = "Integration Member",
            ["Input.Email"] = email,
            ["Input.Password"] = "Password123!",
            ["__RequestVerificationToken"] = antiforgeryToken
        });

        var registerResponse = await client.PostAsync("/Account/Register", form);

        Assert.AreEqual(HttpStatusCode.Redirect, registerResponse.StatusCode);
        Assert.AreEqual("/", registerResponse.Headers.Location?.OriginalString);

        var dashboardResponse = await client.GetAsync("/Dashboard");
        var dashboardHtml = await dashboardResponse.Content.ReadAsStringAsync();

        dashboardResponse.EnsureSuccessStatusCode();
        StringAssert.Contains(dashboardHtml, "Integration Member");
        StringAssert.Contains(dashboardHtml, "No QR Code Yet");
    }

    private HttpClient CreateClientWithoutAutomaticRedirects()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    private static string GetAntiforgeryToken(string html)
    {
        var match = Regex.Match(
            html,
            "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");

        Assert.IsTrue(match.Success, "The registration page did not contain an antiforgery token.");

        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }
}
