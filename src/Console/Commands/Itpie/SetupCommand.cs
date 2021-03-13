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
        public override string[] Aliases { get { return Array.Empty<string>(); } }

        public SetupCommand(ContextStack stack, HttpClient client)
        {
            this.client = client;
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var args = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1);
            var job = args.Take(1).FirstOrDefault() ?? "all";
            var runNow = args.Skip(1).FirstOrDefault() == "-run";

            switch (job)
            {
                case "all":
                {
                    var files = Directory.GetFiles(AppContext.BaseDirectory, "*.ips");
                    foreach (var file in files)
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        await this.doSetup(name, runNow);
                    }
                    break;
                }
                default:
                {
                    await this.doSetup(job, runNow);
                    break;
                }
            }
            return true;
        }

        private async Task doSetup(string name, bool runNow = false)
        {
            var file = Path.Combine(AppContext.BaseDirectory, $"{name}.ips");
            string ips;
            if (File.Exists(file))
            {
                ips = File.ReadAllText(file);
            }
            else
            {
                ContextStack.WriteError($"Unable to locate file: {file}");
                return;
            }

            // create the job
            var suffix = $"{DateTime.Now.Ticks}";
            var job = new JobWithTargetRequest
            {
                Name = $"{name}-{suffix}",
                JobTypes = new[] { "ARP", "MAC", "Device", "Route" },
                Protocol = "Ssh",
                IncludedRanges = ips
            };
            var jobResult = await this.client.PostAsJsonAsync($"{this.stack.AppSettings.Public.IptieApiUrl}/collection/jobs/target", job);
            var jobResponse = await jobResult.Content.ReadAsAsync<JobResponse>();

            var (user, pass) = getCredentials(name);
            var creds = new CredentialGroupRequest
            {
                Name = $"{name}-{suffix}",
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

            await this.client.PostAsJsonAsync($"{this.stack.AppSettings.Public.IptieApiUrl}/collection/credential-groups", creds);

            if (runNow)
            {
                await this.client.PostAsJsonAsync($"{this.stack.AppSettings.Public.IptieApiUrl}/collection/jobs/{jobResponse.Id}/schedules/trigger", jobResponse.Id);
            }
        }

        private static (string, string) getCredentials(string action)
        {
            ContextStack.WriteLine($"Please enter credentials for the {action} job");

            ContextStack.WriteStart("Username: ");
            var user = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Enter)
                    break;
                user += key.KeyChar;
            }

            Console.WriteLine();

            ContextStack.WriteStart("Password: ");
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
                    Command = this,
                    Description = new List<string>
                    {
                        $"Setup jobs and credentials for ITPIE",
                        "",
                        "Options:",
                        "  -run : executes the job as soon as it has been created.",
                        "",
                        "Examples:",
                        " > setup corp",
                        " > setup sdev -run"
                    }
                }
            };
        }
    }
}
