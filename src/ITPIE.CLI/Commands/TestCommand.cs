using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CLI.Models;

#pragma warning disable 1998

namespace CLI.Commands
{
    public class TestCommand : CommandBase, ICommand
    {
        private object lockObject = "lock";

        public override string Name { get { return "test"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public TestCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;
            var args = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
            var verbose = args.Any(v => v == "-v");
            var telnet = args.Any(v => v == "-t");

            foreach (var target in args.Where(v => !v.StartsWith("-")).Select(v => v.Trim()))
            {
                if (target.IsSubnet())
                {
                    var h = target.DeriveHiNetworkAddress();
                    var l = target.DeriveLowNetworkAddress();

                    this.stack.WriteLine($"Testing {h - l} IP Addresses");
                    var replies = testMany(l, h);

                    printResults("SSH Open and Pingable", replies
                        .Where(v => v.PingResult.Status == IPStatus.Success && v.SshResult == PortStatus.Open)
                        .OrderBy(v => v.Address.ToString().IpToInt())
                        .ToList());

                    printResults("SSH Open but Ping Failed", replies
                        .Where(v => v.PingResult.Status != IPStatus.Success && v.SshResult == PortStatus.Open)
                        .OrderBy(v => v.Address.ToString().IpToInt())
                        .ToList());

                    if (telnet)
                    {
                        printResults("Telnet Open", replies
                            .Where(v => v.TelnetResult == PortStatus.Open)
                            .OrderBy(v => v.Address.ToString().IpToInt())
                            .ToList());
                    }

                    printResults("Ping Success but SSH Failed", replies
                        .Where(v => v.PingResult.Status == IPStatus.Success && v.SshResult == PortStatus.Closed)
                        .OrderBy(v => v.Address.ToString().IpToInt())
                        .ToList());

                    if (verbose)
                    {
                        printResults("Closed", replies
                            .Where(v => v.PingResult.Status != IPStatus.Success && v.SshResult == PortStatus.Closed)
                            .OrderBy(v => v.Address.ToString().IpToInt())
                            .ToList());
                    }
                }
                else if (target.IsIP())
                {
                    var r = testPing(target);
                    var s = testPort(target, 22);
                    this.stack.WriteLine();
                    this.stack.WriteLine($"     SSH Status: {s}");
                    this.stack.WriteLine($"    Ping Status: {r.Status}");
                    if (r.Status == IPStatus.Success)
                    {
                        this.stack.WriteLine($" RoundTrip Time: {r.RoundtripTime} ms");
                        this.stack.WriteLine($"   Time to live: {r.Options.Ttl} ms");
                        this.stack.WriteLine($"    Buffer size: {r.Buffer.Length}");
                    }

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

        private void printResults(string msg, IEnumerable<TestResult> results)
        {
            if (results.Any())
            {
                this.stack.WriteLine();
                this.stack.WriteLine(msg);
                this.stack.WriteLine("-------------------------------------------------------------------");

                foreach (var r in results)
                {
                    if (r.PingResult.Status != IPStatus.Success && r.SshResult == PortStatus.Closed)
                    {
                        this.stack.WriteLine($" {r.Address} {r.PingResult.Status}");
                    }
                    else
                    {
                        this.stack.WriteLine($" {r.Address}");
                    }
                }

                this.stack.WriteLine();
            }
        }

        private ConcurrentBag<TestResult> testMany(uint l, uint h)
        {
            var replies = new ConcurrentBag<TestResult>();
            var addresses = new List<string>();
            for (var i = l; i <= h; i++)
            {
                addresses.Add(i.IntToIPv4());
            }

            Console.Write(" ");
            Parallel.ForEach(addresses, ip =>
            {
                replies.Add(new TestResult
                {
                    Address = ip,
                    SshResult = testPort(ip, 22),
                    TelnetResult = testPort(ip, 23),
                    PingResult = testPing(ip)
                });

                lock (this.lockObject)
                {
                    if (Console.CursorLeft == Console.BufferWidth-1)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
            });
            return replies;
        }

        private static PortStatus testPort(string target, int port)
        {
            using var tcpClient = new TcpClient();
            var result = tcpClient.BeginConnect(target, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
            if (!success)
            {
                return PortStatus.Closed;
            }
            tcpClient.EndConnect(result);
            return PortStatus.Open;
        }

        private static PingReply testPing(string target)
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

    internal class TestResult
    {
        public string Address { get; set; }
        public PingReply PingResult { get; set; }
        public PortStatus SshResult { get; set; }
        public PortStatus TelnetResult { get; set; }
    }

    internal enum PortStatus
    {
        Open,
        Closed
    }
}
