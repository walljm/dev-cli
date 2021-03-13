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
        public override string[] Aliases { get { return Array.Empty<string>(); } }

        public PingCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
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

                    ContextStack.WriteLine($"Pinging {addresses.Count} IP Addresses");
                    Console.Write(" ");
                    Parallel.ForEach(addresses, ip =>
                    {
                        replies.Add(ping(ip));
                        Console.Write(".");
                    });

                    ContextStack.WriteLine();
                    ContextStack.WriteLine("  Successful pings...");
                    foreach (var r in replies.Where(v => v.Status == IPStatus.Success).OrderBy(v => v.Address.ToString().IpToInt()))
                    {
                        ContextStack.WriteLine($"   {r.Address}");
                    }

                    ContextStack.WriteLine();
                    ContextStack.WriteLine("  Failed pings...");
                    foreach (var r in replies.Where(v => v.Status != IPStatus.Success).OrderBy(v => v.Address.ToString().IpToInt()))
                    {
                        ContextStack.WriteLine($"   {r.Address} {r.Status}");
                    }
                    ContextStack.WriteLine();
                }
                else if (target.IsIP())
                {
                    var r = ping(target);
                    ContextStack.WriteLine();
                    ContextStack.WriteLine($"         Status: {r.Status}");
                    ContextStack.WriteLine($" RoundTrip Time: {r.RoundtripTime} ms");
                    ContextStack.WriteLine($"   Time to live: {r.Options.Ttl} ms");
                    ContextStack.WriteLine($"    Buffer size: {r.Buffer.Length}");
                    ContextStack.WriteLine();
                }
                else
                {
                    ContextStack.WriteLine(
                        "Error: you must provide a valid IP or Subnet to ping.  Multiple values separated by a space.");
                }
            }
            ContextStack.WriteLine();

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
            return new Help[] {
                new Help
                {
                    Command = this,
                    Description = new List<string>
                    {
                        $"Test subnet or ip for ping"
                    }
                }
            };
        }
    }
}
