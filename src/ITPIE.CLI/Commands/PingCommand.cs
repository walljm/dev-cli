using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using CLI.Models;

#pragma warning disable 1998

namespace CLI.Commands
{
    public class PingCommand : CommandBase, ICommand
    {
        public override string Name { get { return "ping"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public PingCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;
            var args = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1);

            foreach (var target in args.Select(v => v.Trim()))
            {
                if (target.IsSubnet())
                {
                    var h = target.DeriveHiNetworkAddress();
                    var l = target.DeriveLowNetworkAddress();
                    var replies = new ConcurrentBag<PingReply>();
                    var addresses = new List<string>();
                    for (var i = l; i <= h; i++)
                    {
                        addresses.Add(i.IntToIPv4());
                    }

                    this.stack.WriteLine($"Pinging {addresses.Count} IP Addresses");
                    Console.Write(" ");
                    Parallel.ForEach(addresses, ip =>
                    {
                        replies.Add(ping(ip));
                        Console.Write(".");
                    });

                    this.stack.WriteLine();
                    this.stack.WriteLine("  Successful pings...");
                    foreach (var r in replies.Where(v => v.Status == IPStatus.Success).OrderBy(v => v.Address.ToString().IpToInt()))
                    {
                        this.stack.WriteLine($"   {r.Address}");
                    }

                    this.stack.WriteLine();
                    this.stack.WriteLine("  Failed pings...");
                    foreach (var r in replies.Where(v => v.Status != IPStatus.Success).OrderBy(v => v.Address.ToString().IpToInt()))
                    {
                        this.stack.WriteLine($"   {r.Address} {r.Status}");
                    }
                }
                else if (target.IsIP())
                {
                    var r = ping(target);
                    this.stack.WriteLine();
                    this.stack.WriteLine($"         Status: {r.Status}");
                    this.stack.WriteLine($" RoundTrip Time: {r.RoundtripTime} ms");
                    this.stack.WriteLine($"   Time to live: {r.Options.Ttl} ms");
                    this.stack.WriteLine($"    Buffer size: {r.Buffer.Length}");
                    this.stack.WriteLine();
                }
                else
                {
                    this.stack.WriteLine(
                        "Error: you must provide a valid IP or Subnet to ping.  Multiple values separated by a space.");
                }
            }

            return true;
        }

        private static PingReply ping(string target)
        {
            string data = "a quick brown fox jumped over the lazy dog";

            Ping pingSender = new Ping();
            PingOptions options = new PingOptions
            {
                DontFragment = true
            };

            byte[] buffer = Encoding.ASCII.GetBytes(data);

            var reply = pingSender.Send(target, 1000 * 2, buffer, options);
            return reply;
        }

        public Help[] GetHelp()
        {
            return new Help[0];
        }
    }
}
