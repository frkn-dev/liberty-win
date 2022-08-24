using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace LibertyApp.Models;

public class ConnectionSpeed : ObservableObject
{
    private readonly NetworkInterface[] _networkInterfaces;
    private readonly Dictionary<NetworkInterface, long> _lastReceivedBytes;
    private readonly Dictionary<NetworkInterface, long> _lastSentBytes;

    public double Download
    {
        get => _download;
        private set => SetProperty(ref _download, value);
    }
    private double _download;

    public double Upload
    {
        get => _upload;
        private set => SetProperty(ref _upload, value);
    }
    private double _upload;

    public ConnectionSpeed()
    {
        _networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        _lastReceivedBytes = new Dictionary<NetworkInterface, long>();
        _lastSentBytes = new Dictionary<NetworkInterface, long>();

        foreach (var networkInterface in _networkInterfaces)
        {
            var statistic = networkInterface.GetIPv4Statistics();
            _lastReceivedBytes.Add(networkInterface, statistic.BytesReceived);
            _lastSentBytes.Add(networkInterface, statistic.BytesSent);
        }
    }

    public void CalculateDownloadSpeed()
    {
        long totalDownloadBytes = 0;
        foreach (var networkInterface in _networkInterfaces)
        {
            var newValue = networkInterface.GetIPv4Statistics().BytesReceived;
            var diff = newValue - _lastReceivedBytes[networkInterface];

            totalDownloadBytes += diff;

            _lastReceivedBytes[networkInterface] = newValue;
        }

        Download = totalDownloadBytes / 1024.0 / 1024.0 * 8.0;
    }

    public void CalculateUploadSpeed()
    {
        long totalUploadBytes = 0;
        foreach (var networkInterface in _networkInterfaces)
        {
            var newValue = networkInterface.GetIPv4Statistics().BytesSent;
            var diff = newValue - _lastSentBytes[networkInterface];

            totalUploadBytes += diff;

            _lastSentBytes[networkInterface] = newValue;
        }

        Upload = totalUploadBytes / 1024.0 / 1024.0 * 8.0;
    }
}

