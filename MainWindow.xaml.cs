using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace DroneSimulator;
public partial class MainWindow : Window
{
    readonly string[] aircraft = { "DJI Matrice 30", "DJI Matrice 30T", "DJI Matrice 300 RTK", "DJI Matrice 350 RTK", "DJI Matrice 4E", "DJI Mavic 3E", "DJI Mavic 3T" };
    readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };
    ConnectionClient? connection;
    int seconds, battery = 100, failures;
    long sequence;
    bool flying, gnssLost, sending;
    double latitude = 25.28, longitude = 51.53, originLatitude = 25.28, originLongitude = 51.53;
    public MainWindow()
    {
        InitializeComponent(); AircraftList.ItemsSource = aircraft; AircraftList.SelectedIndex = 0;
        timer.Tick += async (_, _) =>
        {
            if (flying) { seconds++; if (seconds % 8 == 0) battery = Math.Max(0, battery - 1); if (!gnssLost) { latitude = Math.Min(90, originLatitude + seconds * 1.4 / 111320); longitude = originLongitude; } if (battery == 0) flying = false; }
            UpdateDisplay(); var client = connection; if (client is null || sending) return; sending = true;
            try
            {
                await client.SendAsync(new(1, true, sequence++, DateTimeOffset.UtcNow.ToString("O"), AircraftList.SelectedItem?.ToString() ?? aircraft[0], new[] { longitude, latitude }, battery, flying ? Math.Min(60, seconds * 2) : 0, flying ? 1.4 : 0, flying, gnssLost));
                if (connection == client) { failures = 0; ConnectionStatus.Text = "Connected · simulated telemetry sent · view Simulation in Mission Operations"; }
            }
            catch (Exception error)
            {
                if (connection == client) { failures++; ConnectionStatus.Text = "Telemetry paused: " + Friendly(error);
                    if (failures >= 3 || error is HttpRequestException { StatusCode: HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden }) { connection = null; client.Dispose(); SetConnected(false); ConnectionStatus.Text += " Pair again to reconnect. Last position becomes stale after 15 seconds."; } }
            }
            finally { sending = false; }
        };
        timer.Start(); Closed += (_, _) => { timer.Stop(); connection?.Dispose(); connection = null; };
    }
    static string Friendly(Exception error) => error is TaskCanceledException ? "System did not respond within five seconds." : error is HttpRequestException { StatusCode: null } ? "Cannot reach the system. Check its address, network and HTTPS certificate." : error.Message;
    void SetConnected(bool value) { ConnectButton.IsEnabled = !value; DisconnectButton.IsEnabled = value; ServerAddress.IsEnabled = !value; PairingCode.IsEnabled = !value; }
    async void ConnectClicked(object sender, RoutedEventArgs e)
    {
        ConnectButton.IsEnabled = false; ConnectionClient? candidate = null;
        try { candidate = new(ServerAddress.Text); ConnectionStatus.Text = "Connecting…"; await candidate.ConnectAsync(PairingCode.Text); connection = candidate; candidate = null; sequence = 0; failures = 0; PairingCode.Clear(); SetConnected(true); ConnectionStatus.Text = "Connected · start a virtual flight or view idle telemetry in your system."; }
        catch (Exception error) { ConnectionStatus.Text = Friendly(error); SetConnected(false); }
        finally { candidate?.Dispose(); }
    }
    async void DisconnectClicked(object sender, RoutedEventArgs e)
    {
        var client = connection; connection = null; DisconnectButton.IsEnabled = false;
        try { if (client is not null) await client.DisconnectAsync(); ConnectionStatus.Text = "Disconnected · test aircraft cleared from the system."; }
        catch (Exception error) { ConnectionStatus.Text = "Disconnected locally. " + Friendly(error) + " Last position becomes stale after 15 seconds."; }
        finally { client?.Dispose(); SetConnected(false); }
    }
    void UpdateDisplay() { StatusText.Text = flying ? "Status: virtual flight running" : battery == 0 ? "Status: battery depleted (simulated)" : "Status: idle"; AltitudeText.Text = $"Altitude: {(flying ? Math.Min(60, seconds * 2) : 0)} m"; BatteryText.Text = $"Battery: {battery}%"; GnssText.Text = gnssLost ? "GNSS: LOST (injected fault)" : "GNSS: normal"; PositionText.Text = FormattableString.Invariant($"Position: {latitude:F5}, {longitude:F5} (simulated)"); }
    void AircraftChanged(object sender, SelectionChangedEventArgs e) { if (ModelText is null) return; ModelText.Text = AircraftList.SelectedItem?.ToString() ?? "Select an aircraft"; flying = false; seconds = 0; battery = 100; gnssLost = false; latitude = originLatitude; longitude = originLongitude; UpdateDisplay(); }
    void StartClicked(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(LatitudeInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) || !double.IsFinite(lat) || Math.Abs(lat) > 90 || !double.TryParse(LongitudeInput.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon) || !double.IsFinite(lon) || Math.Abs(lon) > 180) { ConnectionStatus.Text = "Enter valid test latitude (-90 to 90) and longitude (-180 to 180)."; return; }
        originLatitude = latitude = lat; originLongitude = longitude = lon; seconds = 0; battery = 100; gnssLost = false; flying = true; UpdateDisplay();
    }
    void StopClicked(object sender, RoutedEventArgs e) { flying = false; UpdateDisplay(); }
    void FaultClicked(object sender, RoutedEventArgs e) { gnssLost = !gnssLost; UpdateDisplay(); }
}
