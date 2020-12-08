using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CLI
{
    public static class Extensions
    {
        private static readonly Regex ipRegex = new Regex(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", RegexOptions.Compiled);
        private static readonly Regex subnetRegex = new Regex(@"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\/\d+", RegexOptions.Compiled);

        /// <summary>
        /// Changes a "CamelCasedString" to "Camel Cased String"
        /// </summary>
        /// <param name="camelCasedString"></param>
        /// <returns></returns>
        public static string SpaceByCamelCase(this string camelCasedString)
        {
            return System.Text.RegularExpressions.Regex.Replace(camelCasedString,
                "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }

        public static bool IsIP(this string str)
        {
            return ipRegex.IsMatch(str);
        }

        public static bool IsSubnet(this string str)
        {
            return subnetRegex.IsMatch(str);
        }

        public static uint DeriveLowNetworkAddress(this string cidr_address)
        {
            // Our low network address is simply a normalized string representation of the network
            // address using 32 ascii hex chars This string and the complementary HiNetworkAddress are
            // both used for ordinal string compare for contained ip's or subnets
            var ipaddress = "";
            byte prefix_length = 0;

            // This string based function is IpV4 and IpV6 compliant It will apply the mask bits of a
            // CIDR notation to the IPAddress to get the lowest (i.e. network) address
            if (cidr_address.Contains("/"))
            {
                ipaddress = cidr_address.ParseToIndexOf("/").Trim();
                prefix_length = (byte)Convert.ToInt32(cidr_address.ParseAfterIndexOf_PlusIndexLength("/").Trim());
            }

            if (ipaddress.IsIP() && prefix_length >= 0 && prefix_length <= 32)
            {
                return DeriveNetworkAddress(cidr_address);
            }

            throw new Exception("Somethign went wrong :(");
        }

        public static uint DeriveHiNetworkAddress(this string cidr_address)
        {
            // Our hi network address is simply a normalized string representation of the last ip
            // address in a network using 32 ascii hex chars This string and the complementary
            // LowNetworkAddress are both used for ordinal string compare for contained ip's or subnets
            var ipaddress = "";
            byte prefix_length = 0;

            // This string based function is IpV4 and IpV6 compliant It will apply the mask bits of a
            // CIDR notation to the IPAddress to get the lowest (i.e. network) address
            if (cidr_address.Contains("/"))
            {
                ipaddress = cidr_address.ParseToIndexOf("/").Trim();
                prefix_length = (byte)Convert.ToInt32(cidr_address.ParseAfterIndexOf_PlusIndexLength("/").Trim());
            }

            if (ipaddress.IsIP() && prefix_length >= 0 && prefix_length <= 32)
            {
                // our ipv4 hi network address is simply a normalized string representation of the
                // broadcast address
                var host_bits = (byte)(32 - prefix_length);
                return ipaddress.IpToInt() | (uint)(Math.Pow(2, host_bits) - 1);
            }

            throw new Exception("Somethign went wrong :(");
        }

        public static uint DeriveNetworkAddress(this string cidr_address)
        {
            var ipaddress = "";
            byte prefix_length = 0;

            // This string based function is IpV4 and IpV6 compliant It will apply the mask bits of a
            // CIDR notation to the IPAddress to get the lowest (i.e. network) address
            if (cidr_address.Contains("/"))
            {
                ipaddress = cidr_address.ParseToIndexOf("/").Trim();
                var cidr = cidr_address.ParseAfterIndexOf_PlusIndexLength("/").Trim();
                if (cidr != string.Empty && cidr != null)
                {
                    prefix_length = (byte)Convert.ToInt32(cidr);
                }
            }

            // we must delineate ipv4 from ipv6 because they are string representations
            if (ipaddress.IsIP()) // ipv4 method
            {
                //use bit-wise &
                uint net_int = ipaddress.IpToInt();
                var j = 31;
                uint bitmask_int = 0;

                for (int i = prefix_length; i > 0; i--)
                {
                    bitmask_int += (uint)Math.Pow(2, j);
                    j--;
                }

                return net_int & bitmask_int;
            }

            throw new Exception("Somethign went wrong :("); // we should never get here
        }

        public static string IntToIPv4(this uint i)
        {
            var ipV4_octets = new string[4];
            var period = @".";

            for (var j = 3; j >= 0; j--)
            {
                ipV4_octets[j] = ((byte)(i & 0xFF)).ToString();
                i >>= 8;
            }

            return string.Join(period, ipV4_octets);
        }

        public static uint IpToInt(this string ip)
        {
            uint output = 0;
            if (!ip.IsIP())
                return output;
            var octets = ip.Split('.');

            for (var x = 0; x < octets.Length; ++x)
            {
                var octet = (uint)Convert.ToInt32(octets[x]);
                octet = octet << 8 * (4 - 1 - x);
                output += octet;
            }

            return output;
        }

        /// <summary>
        /// Parses to index of the char provided inside of the '("[char]")'.
        /// <para>EXAMPLE:</para>
        /// <para>string bacon = "I love bacon and tomatoes!";</para>
        /// <para>string tomatoes = bacon.ParseToIndexOf("bacon");</para>
        /// <para>yields => string tomatoes= "I love";</para>
        ///</summary>
        /// <param name="s">The string/char.</param>
        /// <param name="idx">The index.</param>
        /// <returns></returns>
        public static string ParseToIndexOf(this string s, params string[] idx)
        {
            return ParseToIndexOf(s, false, idx);
        }

        /// <summary>
        /// Parses to index of the char provided inside of the '("[char]")'.
        /// <para>EXAMPLE:</para>
        /// <para>string bacon = "I love bacon and tomatoes!";</para>
        /// <para>string tomatoes = bacon.ParseToIndexOf("bacon");</para>
        /// <para>yields => string tomatoes= "I love";</para>
        ///</summary>
        /// <param name="s">The string/char.</param>
        /// <param name="case_insensitive">Boolean indicating weather to ignore case or not</param>
        /// <param name="idx">The index.</param>
        /// <returns></returns>
        public static string ParseToIndexOf(this string s, bool case_insensitive, params string[] idx)
        {
            if (case_insensitive)
            {
                var i = s.ToLower();
                var lowered = idx.Select(c => c.ToLower()).ToArray();
                var pos = -1;
                foreach (var val in lowered)
                {
                    var x = i.IndexOf(val, StringComparison.Ordinal);
                    if (x > -1 && (x < pos || pos == -1))
                        pos = x;
                }

                if (pos > -1)
                    return s.Substring(0, pos);
                else
                    return s;
            }
            else
                return s.Split(idx, StringSplitOptions.None)[0]; // this guarantees that it doesn't fail if the idx is missing.
        }

        /// <summary>
        /// Parses after what is declared in '("[this]")' to the next index of the previously declared
        /// character or characters.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="idx">The idx.</param>
        /// <returns></returns>
        public static string ParseAfterIndexOf_PlusIndexLength(this string s, params string[] idx)
        {
            return ParseAfterIndexOf_PlusIndexLength(s, false, idx);
        }

        /// <summary>
        /// Parses after what is matched by the regular expresssion.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="reg">Regular expression to match</param>
        /// <returns></returns>
        public static string ParseAfterIndexOf_PlusIndexLength(this string s, Regex reg)
        {
            var m = reg.Match(s);
            return s.Substring(m.Index + m.Length);
        }

        /// <summary>
        /// Parses after what is declared in '("[this]")' to the next index of the previously declared
        /// character or characters.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="case_insensitive">
        /// bool operator indicating if the string should be found regardless of case
        /// </param>
        /// <param name="idx">The idx.</param>
        /// <returns></returns>
        public static string ParseAfterIndexOf_PlusIndexLength(this string s, bool case_insensitive, params string[] idx)
        {
            if (s.Length == 0)
                return s;

            var t = s;
            if (case_insensitive)
                t = s.ToLower();

            foreach (var item in idx)
            {
                var i = item;
                if (case_insensitive)
                    i = item.ToLower();

                if (t.IndexOf(i, StringComparison.Ordinal) > -1)
                {
                    return s.Substring(t.IndexOf(i, StringComparison.Ordinal) + i.Length);
                }
            }

            return s; // default to the whole string if none succeeded, as this is what would happen anyway.
        }
    }
}
