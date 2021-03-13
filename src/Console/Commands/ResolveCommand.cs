using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using CLI.Models;
using DnsClient;

namespace CLI.Commands
{
    public class ResolveCommand : CommandBase, ICommand
    {
        private readonly IPAddress[] nameServers;
        private readonly LookupClient client;

        public override string Name { get { return "resolve"; } }
        public override string[] Aliases { get { return new string[] { "dns" }; } }

        public ResolveCommand(ContextStack stack)
        {
            this.stack = stack;
            var ns = new List<IPAddress>();

            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                var adapterProperties = adapter.GetIPProperties();
                ns.AddRange(adapterProperties.DnsAddresses.Where(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork));
            }

            ns.Add(IPAddress.Parse("8.8.8.8"));
            ns.Add(IPAddress.Parse("1.1.1.1"));
            this.nameServers = ns.ToArray();
            this.client = new LookupClient(this.nameServers);
        }

        public Task<bool> Run(string cmd)
        {
            var args = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
            if (args.Count == 0)
            {
                ContextStack.WriteLine(
                    "Error: you must provide a valid IP to lookup.  Multiple values separated by a space.");
                return Task.FromResult(false);
            }

            foreach (var host in args)
            {
                if (IPAddress.TryParse(host, out var ip))
                {
                    this.lookupIp(ip);
                }
                else
                {
                    this.lookupDns(host);
                }
            }

            return Task.FromResult(true);
        }

        private void lookupIp(IPAddress ip)
        {
            foreach (var ns in this.client.NameServers)
            {
                ContextStack.WriteLine($"Name Server: {ns}");
            }
            ContextStack.WriteLine();

            var result = this.client.QueryReverse(ip);
            if (result.Answers.Count == 0)
            {
                return;
            }

            this.WriteResponse(result.Answers);
            ContextStack.WriteLine();
        }

        private void lookupDns(string host)
        {
            foreach (var ns in this.client.NameServers)
            {
                ContextStack.WriteLine($"Name Server: {ns}");
            }
            ContextStack.WriteLine();

            foreach (QueryType t in Enum.GetValues(typeof(QueryType)))
            {
                var result = this.client.Query(host, t);
                if (result.Answers.Count == 0)
                {
                    continue;
                }

                this.WriteResponse(result.Answers);
                ContextStack.WriteLine();
            }
        }

        private class Column
        {
            public PropertyInfo Info { get; set; }
            public string Name { get; set; }
            public int Width { get; set; }
        }

        private void WriteResponse<T>(IEnumerable<T> c)
        {
            if (!c.Any())
            {
                return;
            }

            var groups = c.GroupBy(k => k.GetType());

            foreach (var group in groups)
            {
                var t = group.Key;
                ContextStack.WriteLine($" Record Type: {t.Name}");
                var fArray = t
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(propInfo =>
                    {
                        return new Column
                        {
                            Info = propInfo,
                            Name = propInfo.Name.SpaceByCamelCase(),
                            Width = 0
                        };
                    })
                    .ToList();

                foreach (var prop in fArray)
                {
                    prop.Width = prop.Width < prop.Name.Length ? prop.Name.Length : prop.Width;
                    foreach (var d in group)
                    {
                        var v = prop.Info.GetValue(d)?.ToString() ?? "";
                        prop.Width = prop.Width < v.Length ? v.Length : prop.Width;
                    }
                }

                // column headers.
                foreach (var prop in fArray)
                {
                    Console.Write($"  {prop.Name.PadRight(prop.Width + 2)}");
                }
                ContextStack.WriteLine();

                // column divider
                foreach (var prop in fArray)
                {
                    Console.Write($"  {string.Empty.PadRight(prop.Width, '-')}  ");
                }
                ContextStack.WriteLine();

                // data
                foreach (var d in group)
                {
                    foreach (var prop in fArray)
                    {
                        var v = prop.Info.GetValue(d)?.ToString() ?? "";
                        Console.Write($"  {v.PadRight(prop.Width + 2)}");
                    }
                    ContextStack.WriteLine();
                }
                ContextStack.WriteLine();
            }
        }

        public Help[] GetHelp()
        {
            return new Help[] {
                new Help
                {
                    Command = $"{this.Name}",
                    Description = new List<string>
                    {
                        $"Resolve ip or hostname to its opposite",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
