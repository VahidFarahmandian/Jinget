using System.Net;

namespace Jinget.Core.Utilities.IP;

public static class IPUtility
{
    /// <summary>
    /// check whether the given ip address is in the given CIDR range
    /// </summary>   
    public static bool IsIPInRange(string ipAddress, string cidrRange)
    {
        if (string.IsNullOrEmpty(cidrRange) || !cidrRange.Contains('/')) return false;

        string[] parts = cidrRange.Split('/');
        if (parts.Length != 2) return false;

        string baseAddressStr = parts[0];
        if (!int.TryParse(parts[1], out int prefixLength)) return false;

        IPAddress? ip; // To store parsed IP address
        if (IPAddress.TryParse(baseAddressStr, out ip))
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) //IPv4
            {
                if (prefixLength < 0 || prefixLength > 32) return false;
            }
            else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) //IPv6
            {
                if (prefixLength < 0 || prefixLength > 128) return false;
            }
            else
            {
                return false; //Unknown IP format
            }

            if (!IPAddress.TryParse(ipAddress, out IPAddress targetAddress)) return false;

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) //IPv4
            {
                uint baseAddressInt = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
                uint targetAddressInt = BitConverter.ToUInt32(targetAddress.GetAddressBytes().Reverse().ToArray(), 0);
                uint mask = ~(uint.MaxValue >> prefixLength);
                return (baseAddressInt & mask) == (targetAddressInt & mask);
            }
            else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) //IPv6
            {
                //Handle IPv6
                byte[] baseAddressBytes = ip.GetAddressBytes();
                byte[] targetAddressBytes = targetAddress.GetAddressBytes();

                for (int i = 0; i < 16; i++)
                {
                    byte mask = (byte)(prefixLength > i * 8 ? (prefixLength > (i + 1) * 8 ? 0xFF : (0xFF << (8 - (prefixLength - i * 8)))) : 0);
                    if ((baseAddressBytes[i] & mask) != (targetAddressBytes[i] & mask))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false; //Unknown IP format
            }

        }
        else
        {
            return false; //Invalid IP format
        }
    }

}