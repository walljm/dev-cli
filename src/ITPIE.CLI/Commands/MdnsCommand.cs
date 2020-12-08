using System;
using System.Linq;
using System.Threading.Tasks;
using CLI.Models;
using Zeroconf;

namespace CLI.Commands
{
    public class MdnsCommand : CommandBase, ICommand
    {
        public override string Name { get { return "mdns"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public MdnsCommand(ContextStack stack)
        {
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;

            var domains = await ZeroconfResolver.BrowseDomainsAsync();
            var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));
            foreach (var resp in responses)
            {
                this.stack.WriteLine($" {resp.DisplayName}");
                foreach (var svc in resp.IPAddresses)
                {
                    this.stack.WriteLine($"   IP: {svc}");
                }

                this.stack.WriteLine();

                foreach (var svc in resp.Services)
                {
                    var lines = svc.Value.ToString().Split(Environment.NewLine, StringSplitOptions.None);
                    this.stack.WriteLine($"   {lines.FirstOrDefault() ?? ""}");
                    foreach (var line in lines.Skip(1))
                    {
                        this.stack.WriteLine($"     {line}");
                    }
                    this.stack.WriteLine();
                }
            }
            return true;
        }

        public Help[] GetHelp()
        {
            return new Help[0];
        }
    }
}
