using System;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace PredScout
{
    public static class SystemSignature
    {
        public static string GenerateSystemSignature()
        {
            // Get identifiers
            string cpuId = GetHardwareId("Win32_Processor", "ProcessorId");
            string biosSerial = GetHardwareId("Win32_BIOS", "SerialNumber");
            string diskSerial = GetHardwareId("Win32_DiskDrive", "SerialNumber");
            string macAddress = GetHardwareId("Win32_NetworkAdapterConfiguration", "MACAddress", true);

            // Combine the values to form the signature
            string combined = $"{cpuId}-{biosSerial}-{diskSerial}-{macAddress}";

            // Hash the combined string to create a unique signature
            return HashString(combined);
        }

        private static string GetHardwareId(string wmiClass, string wmiProperty, bool onlyFirst = false)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT {wmiProperty} FROM {wmiClass}"))
                {
                    ManagementObjectCollection results = searcher.Get();
                    foreach (ManagementObject obj in results)
                    {
                        return obj[wmiProperty]?.ToString() ?? string.Empty;
                    }
                }
            }
            catch
            {
                // Handle exceptions if necessary
            }
            return string.Empty;
        }

        private static string HashString(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool ValidateSystemSignature(string oldSignature, string newSignature)
        {
            return CalculateLevenshteinDistance(oldSignature, newSignature) <= 10; // Allow a max of 10 character changes
        }

        private static int CalculateLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            int sourceLength = source.Length;
            int targetLength = target.Length;

            int[,] distance = new int[sourceLength + 1, targetLength + 1];

            for (int i = 0; i <= sourceLength; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetLength; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceLength; i++)
            {
                for (int j = 1; j <= targetLength; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceLength, targetLength];
        }
    }
}
