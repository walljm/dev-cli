using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Models;
using Zeroconf;

namespace CLI.Commands
{
    public class MdnsCommand : CommandBase, ICommand
    {
        public override string Name { get { return "mdns"; } }
        public override string[] Aliases { get { return Array.Empty<string>(); } }

        public MdnsCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var domains = await ZeroconfResolver.BrowseDomainsAsync();
            var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));
            foreach (var resp in responses)
            {
                ContextStack.WriteLine($" {resp.DisplayName}");
                foreach (var svc in resp.IPAddresses)
                {
                    ContextStack.WriteLine($"   IP: {svc}");
                }

                ContextStack.WriteLine();

                foreach (var svc in resp.Services)
                {
                    var lines = svc.Value.ToString().Split(Environment.NewLine, StringSplitOptions.None);
                    ContextStack.WriteLine($"   {lines.FirstOrDefault() ?? ""}");
                    foreach (var line in lines.Skip(1))
                    {
                        ContextStack.WriteLine($"     {line}");
                    }
                    ContextStack.WriteLine();
                }
            }
            return true;
        }

        public Help[] GetHelp()
        {
            return new Help[] {
                new Help
                {
                    Command = $"{this.Name}",
                    Description = new List<string>
                    {
                        $"Scan for mDNS (Bonjour) services on the network",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
