using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace SOSXR.SeaShark
{
    public static class Network
    {
        /// <summary>
        ///     Returns this device's local (LAN) IPv4 address, e.g. "192.168.1.23".
        ///     Returns null when no usable network connection is found.
        /// </summary>
        public static string GetLocalIPAddress()
        {
            // Preferred: ask the OS which interface it would use to reach the outside world.
            // A UDP 'connect' only sets the route; no packets are sent.
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect("8.8.8.8", 65530);

                if (socket.LocalEndPoint is IPEndPoint endPoint && !IPAddress.IsLoopback(endPoint.Address))
                {
                    return endPoint.Address.ToString();
                }
            }
            catch (SocketException)
            {
                // No route (e.g. offline). Fall through to interface scan.
            }

            // Fallback: scan active, non-loopback interfaces. Prefer ones with a gateway.
            try
            {
                var candidates = NetworkInterface.GetAllNetworkInterfaces().Where(ni => ni.OperationalStatus == OperationalStatus.Up).Where(ni => ni.NetworkInterfaceType is not NetworkInterfaceType.Loopback and not NetworkInterfaceType.Tunnel).Select(ni => ni.GetIPProperties()).OrderByDescending(props => props.GatewayAddresses.Any()).SelectMany(props => props.UnicastAddresses).Select(ua => ua.Address).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip));

                IPAddress address = candidates.FirstOrDefault();

                if (address != null)
                {
                    return address.ToString();
                }
            }
            catch (NetworkInformationException e)
            {
                Debug.LogWarning($"Network: could not read network interfaces: {e.Message}");
            }

            return null;
        }
    }
}
