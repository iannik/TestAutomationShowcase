using NUnit.Framework;
using System.Net;

namespace TestAutomationShowcase.Core;

/// <summary>
/// Provides logging functionality for API test messages, requests, responses, and test steps.
/// </summary>
public class ApiTestLogger
{
    public void Log(string message) => TestContext.Out.WriteLine($"[{Timestamp}] {message}");

    public void LogRequest(HttpMethod method, string uri, string? body = null)
    {
        TestContext.Out.WriteLine($"[{Timestamp}] --> REQUEST: {method} {uri}");
        if (body is not null)
            TestContext.Out.WriteLine($"    Body: {body}");
    }

    public void LogResponse(HttpStatusCode statusCode, string? body = null)
    {
        TestContext.Out.WriteLine($"[{Timestamp}] <-- RESPONSE: {(int)statusCode} {statusCode}");
        if (body is not null)
            TestContext.Out.WriteLine($"    Body: {body}");
    }

    public void LogStep(string stepDescription) => TestContext.Out.WriteLine($"[{Timestamp}] [STEP] {stepDescription}");

    private static string Timestamp => DateTime.Now.ToString("HH:mm:ss.fff");
}
