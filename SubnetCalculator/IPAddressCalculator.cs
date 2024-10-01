using System.Net;
using System.Numerics;

namespace SubnetCalculator
{
    public static class IPAddressCalculator
    {
        public static IPAddress GetSubnetMask(int cidr)
        {
            uint mask = cidr == 0 ? 0 : uint.MaxValue << (32 - cidr);
            return new IPAddress(BitConverter.GetBytes(mask).Reverse().ToArray());
        }

        public static int GetCIDRFromSubnetMask(IPAddress subnetMask)
        {
            byte[] bytes = subnetMask.GetAddressBytes();
            int cidr = 0;

            foreach (byte b in bytes)
            {
                cidr += CountBits(b);
            }

            return cidr;
        }

        private static int CountBits(byte b)
        {
            int count = 0;
            while (b != 0)
            {
                count += b & 1;
                b >>= 1;
            }
            return count;
        }

        public static IPAddress GetNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();

            byte[] networkBytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes);
        }

        public static IPAddress GetBroadcastAddress(IPAddress ipAddress, IPAddress subnetMask)
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();

            byte[] broadcastBytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                broadcastBytes[i] = (byte)(ipBytes[i] | (~maskBytes[i]));
            }

            return new IPAddress(broadcastBytes);
        }

        public static IPAddress GetFirstHostAddress(IPAddress networkAddress)
        {
            byte[] bytes = networkAddress.GetAddressBytes();
            bytes[3] = (byte)(bytes[3] + 1);
            return new IPAddress(bytes);
        }

        public static IPAddress GetLastHostAddress(IPAddress broadcastAddress)
        {
            byte[] bytes = broadcastAddress.GetAddressBytes();
            bytes[3] = (byte)(bytes[3] - 1);
            return new IPAddress(bytes);
        }

        public static BigInteger GetNumberOfHosts(int cidr)
        {
            return BigInteger.Pow(2, 32 - cidr) - 2;
        }

        public static IPAddress GetWildcardMask(IPAddress subnetMask)
        {
            byte[] maskBytes = subnetMask.GetAddressBytes();
            byte[] wildcardBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                wildcardBytes[i] = (byte)(~maskBytes[i]);
            }

            return new IPAddress(wildcardBytes);
        }

        public static string GetBinaryRepresentation(IPAddress ipAddress, IPAddress subnetMask)
        {
            string ipBinary = string.Join(".", ipAddress.GetAddressBytes().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
            string maskBinary = string.Join(".", subnetMask.GetAddressBytes().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

            return $"IP Address:   {ipAddress}   {ipBinary}\n" +
                   $"Subnet Mask:  {subnetMask}   {maskBinary}";
        }
    }
}
