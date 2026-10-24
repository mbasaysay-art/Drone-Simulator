using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
namespace DroneSimulator;
public sealed record SimulationFrame(int protocolVersion, bool simulation, long sequence, string sentAt, string model, double[] position, int battery, double altitude, double speed, bool flying, bool gnssLost);
public sealed class ConnectionClient : IDisposable
{
    readonly HttpClient http;
    readonly SemaphoreSlim gate = new(1, 1);
    string? token;
    const string Route = "api/mission-operations/windows-simulator/";
    public ConnectionClient(string serverAddress, HttpMessageHandler? handler = null)
    {
        if (!Uri.TryCreate(serverAddress.Trim(), UriKind.Absolute, out var address) ||
            (address.Scheme != "https" && !(address.Scheme == "http" && address.IsLoopback)) ||
            !string.IsNullOrEmpty(address.UserInfo) || !string.IsNullOrEmpty(address.Query) || !string.IsNullOrEmpty(address.Fragment))
            throw new ArgumentException("Use your system’s HTTPS address, or HTTP localhost for a local test.");
        http = new HttpClient(handler ?? new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false })
        { BaseAddress = new Uri(address.AbsoluteUri.TrimEnd('/') + "/"), Timeout = TimeSpan.FromSeconds(5) };
    }
    async Task<JsonElement> RequestAsync(HttpMethod method, string route, object? body = null, bool authenticated = true)
    {
        using var csrfResponse = await http.GetAsync("api/auth/csrf-token");
        if (!csrfResponse.IsSuccessStatusCode) throw new HttpRequestException("Cannot obtain a connection security token.");
        var csrf = await csrfResponse.Content.ReadFromJsonAsync<JsonElement>();
        using var request = new HttpRequestMessage(method, Route + route);
        request.Headers.Add("x-csrf-token", csrf.GetProperty("csrfToken").GetString());
        if (authenticated) { if (token is null) throw new InvalidOperationException("Pair the simulator first."); request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); }
        if (body is not null) request.Content = JsonContent.Create(body);
        using var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var message = (int)response.StatusCode switch
            {
                401 => "Connection code or pairing expired. Generate a new code in the system.",
                403 => "Connection permission or browser sign-in ended. Check access and pair again.",
                404 => "This server does not have the Windows simulator connector installed.",
                409 => "Connector disabled, or telemetry rejected. Check the test-server configuration and pair again.",
                429 => "Too many connection requests. Wait before trying again.",
                _ => "System rejected the connection (HTTP " + (int)response.StatusCode + ")."
            };
            throw new HttpRequestException(message, null, response.StatusCode);
        }
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }
    public async Task ConnectAsync(string code)
    {
        await gate.WaitAsync();
        try { var result = await RequestAsync(HttpMethod.Post, "pair", new { code = code.Trim().ToLowerInvariant() }, false);
            if (result.GetProperty("protocolVersion").GetInt32() != 1 || !result.GetProperty("simulation").GetBoolean()) throw new InvalidOperationException("Server does not support this simulation protocol.");
            token = result.GetProperty("token").GetString() ?? throw new InvalidOperationException("No pairing token received."); }
        finally { gate.Release(); }
    }
    public async Task SendAsync(SimulationFrame frame) { await gate.WaitAsync(); try { await RequestAsync(HttpMethod.Post, "telemetry", frame); } finally { gate.Release(); } }
    public async Task DisconnectAsync() { await gate.WaitAsync(); try { if (token is not null) await RequestAsync(HttpMethod.Delete, "telemetry"); } finally { token = null; gate.Release(); } }
    public void Dispose() { token = null; http.Dispose(); }
}
