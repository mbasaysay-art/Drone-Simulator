using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DroneSimulator;
static void Check(bool value, string message) { if (!value) throw new Exception(message); }
foreach (var address in new[] { "http://example.com", "file:///test", "https://user:password@example.com", "https://example.com?token=x" })
{ try { using var invalid = new ConnectionClient(address); throw new Exception("Unsafe address accepted"); } catch (ArgumentException) { } }
var handler = new FixtureHandler();
using (var client = new ConnectionClient("https://test.invalid/", handler))
{
 await client.ConnectAsync("00112233445566778899aabbccddeeff");
 await client.SendAsync(new(1, true, 0, DateTimeOffset.UtcNow.ToString("O"), "M350", new[] {51.53,25.28}, 90, 20, 1.4, true, false));
 await client.DisconnectAsync();
 Check(handler.PostedFrame, "Frame not sent"); Check(handler.Disconnected, "Disconnect not sent");
}
var url=Environment.GetEnvironmentVariable("CONNECTOR_E2E_URL");
if (!string.IsNullOrEmpty(url))
{
 using var actual = new ConnectionClient(url);
 await actual.ConnectAsync(Environment.GetEnvironmentVariable("CONNECTOR_E2E_CODE") ?? "");
 await actual.SendAsync(new(1,true,0,DateTimeOffset.UtcNow.ToString("O"),"CSharp integration test",new[] {51.53,25.28},90,20,1.4,true,false));
 await Task.Delay(250);
 await actual.SendAsync(new(1,true,1,DateTimeOffset.UtcNow.ToString("O"),"CSharp integration test",new[] {52.0,26.0},89,22,1.4,true,true));
 // Leave the temporary frame for the API test to verify account isolation and revocation.
}
Console.WriteLine("Connector client checks passed.");
sealed class FixtureHandler : HttpMessageHandler
{
 public bool PostedFrame, Disconnected;
 protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
 {
  var path=request.RequestUri!.AbsolutePath;
  if(path=="/api/auth/csrf-token") { if(request.Headers.Authorization is not null) throw new Exception("Scoped token leaked to CSRF endpoint"); return Reply(new {csrfToken="fixture-csrf"}); }
  if(!request.Headers.TryGetValues("x-csrf-token",out var values)||values.Single()!="fixture-csrf")throw new Exception("Missing CSRF protection");
  if(path.EndsWith("/pair")) { if(request.Headers.Authorization is not null)throw new Exception("Pair request has credentials");return Reply(new {token="fixture-token",simulation=true,protocolVersion=1}); }
  if(request.Headers.Authorization?.Parameter!="fixture-token")throw new Exception("Missing scoped bearer token");
  if(request.Method==HttpMethod.Post) { var frame=await request.Content!.ReadFromJsonAsync<JsonElement>(cancellationToken:cancellationToken);if(!frame.GetProperty("simulation").GetBoolean()||frame.GetProperty("position")[0].GetDouble()!=51.53)throw new Exception("Frame schema mismatch");PostedFrame=true; }
  if(request.Method==HttpMethod.Delete)Disconnected=true;
  return Reply(new {accepted=true});
 }
 static HttpResponseMessage Reply(object value)=>new(HttpStatusCode.OK) {Content=JsonContent.Create(value)};
}
