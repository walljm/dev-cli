using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CLI.Commands.Itpie.Models;
using CLI.Models;

namespace CLI.Commands.Itpie
{
    public class SetupCommand : CommandBase, ICommand
    {
        private readonly HttpClient client;

        public override string Name { get { return "setup"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public SetupCommand(ContextStack stack, HttpClient client)
        {
            this.client = client;
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var ctx = this.stack.Current;
            var args = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1);
            var name = args.Take(1).FirstOrDefault() ?? "all";

            var itpieUrl = ctx.GetEnvVariable(Constants.EnvironmentItpieUrlVarName);
            switch (name)
            {
                case "sdev":
                {
                    await this.doSetup(name, itpieUrl);
                    break;
                }
                case "corp":
                {
                    await this.doSetup(name, itpieUrl);
                    break;
                }
                // setup all ips.
                default:
                {
                    await this.doSetup("sdev", itpieUrl);
                    await this.doSetup("corp", itpieUrl);
                    break;
                }
            }
            return true;
        }

        private async Task doSetup(string name, string itpieUrl)
        {
            var ips = string.Empty;
            if (File.Exists($"{name}.ips"))
            {
                ips = File.ReadAllText($"{name}.ips");
            }

            // create the job
            var job = new JobWithTargetRequest
            {
                Name = $"{name}-{Guid.NewGuid()}",
                JobTypes = new[] { "ARP", "MAC", "Device", "Route" },
                Protocol = "Ssh",
                IncludedRanges = ips
            };
            await this.client.PostAsJsonAsync($"{itpieUrl}/collection/jobs/target", job);

            var (user, pass) = this.GetCredentials(name);
            var creds = new CredentialGroupRequest
            {
                Name = $"{name}-{Guid.NewGuid()}",
                IncludedRanges = ips,
                Credentials = new List<CredentialRequest>
                {
                    new CredentialRequest
                    {
                        Name = user,
                        IsPrivileged = false,
                        Type = "SSH",
                        Username = user,
                        Password = pass
                    }
                }
            };

            await this.client.PostAsJsonAsync($"{itpieUrl}/collection/credential-groups", creds);
        }

        public (string, string) GetCredentials(string action)
        {
            this.stack.WriteLine($"Please enter the credentials for the {action} job");

            this.stack.WriteStart("Username: ");
            var user = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Enter)
                    break;
                user += key.KeyChar;
            }

            Console.WriteLine();

            this.stack.WriteStart("Password: ");
            var pass = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                pass += key.KeyChar;
            }

            Console.WriteLine();

            return (user, pass);
        }

        public Help[] GetHelp()
        {
            return new Help[] {
                new Help
                {
                    Command = $"{this.Name}",
                    Description = new List<string>
                    {
                        $"Test subnet or ip for ping",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
