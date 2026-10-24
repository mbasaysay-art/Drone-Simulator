using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace DroneSimulator;
public partial class MainWindow : Window
{
    readonly string[] aircraft = { "DJI Matrice 30", "DJI Matrice 30T", "DJI Matrice 300 RTK", "DJI Matrice 350 RTK", "DJI Matrice 4E", "DJI Mavic 3E", "DJI Mavic 3T" };
    readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };
    int seconds = 0, battery = 100;
    bool flying = false, gnssLost = false;
    public MainWindow()
    {
        InitializeComponent(); AircraftList.ItemsSource = aircraft; AircraftList.SelectedIndex = 0;
        timer.Tick += (_, _) => { if (!flying) return; seconds++; if(seconds % 8 == 0) battery = Math.Max(0, battery - 1); AltitudeText.Text = $"Altitude: {Math.Min(60, seconds * 2)} m"; BatteryText.Text = $"Battery: {battery}%"; PositionText.Text = $"Position: local simulated displacement {seconds * 1.4:F1} m"; if (battery == 0) { flying = false; StatusText.Text = "Status: battery depleted (simulated)"; } };
        timer.Start();
    }
    void AircraftChanged(object sender, SelectionChangedEventArgs e) { ModelText.Text = AircraftList.SelectedItem?.ToString() ?? "Select an aircraft"; flying = false; seconds = 0; battery = 100; gnssLost = false; StatusText.Text = "Status: idle"; AltitudeText.Text = "Altitude: 0 m"; BatteryText.Text = "Battery: 100%"; GnssText.Text = "GNSS: normal"; PositionText.Text = "Position: simulated origin"; }
    void StartClicked(object sender, RoutedEventArgs e) { flying = true; StatusText.Text = "Status: virtual flight running"; }
    void FaultClicked(object sender, RoutedEventArgs e) { gnssLost = !gnssLost; GnssText.Text = gnssLost ? "GNSS: LOST (injected fault)" : "GNSS: normal"; }
}
