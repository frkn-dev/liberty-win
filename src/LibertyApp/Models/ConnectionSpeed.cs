using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LibertyApp.Models;

public class ConnectionSpeed
{
    private readonly NetworkInterface[] _networkInterfaces;
    private Dictionary<NetworkInterface, long> _lastReceivedBytes;
    private Dictionary<NetworkInterface, long> _lastSentBytes;

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

    public double CalculateDownloadSpeed()
    {
        long totalDownloadBytes = 0;
        foreach (var networkInterface in _networkInterfaces)
        {
            var newValue = networkInterface.GetIPv4Statistics().BytesReceived;
            var diff = newValue - _lastReceivedBytes[networkInterface];

            totalDownloadBytes += diff;

            _lastReceivedBytes[networkInterface] = newValue;
        }

        return totalDownloadBytes / 1024.0 / 1024.0 * 8.0;
    }


    public double CalculateUploadSpeed()
    {
        long totalUploadBytes = 0;
        foreach (var networkInterface in _networkInterfaces)
        {
            var newValue = networkInterface.GetIPv4Statistics().BytesSent;
            var diff = newValue - _lastSentBytes[networkInterface];

            totalUploadBytes += diff;

            _lastSentBytes[networkInterface] = newValue;
        }

        return totalUploadBytes / 1024.0 / 1024.0 * 8.0;
    }
}

