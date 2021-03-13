using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Threading.Tasks;
using CLI.Commands.Itpie.Models;
using CLI.Models;
using Newtonsoft.Json;

namespace CLI.Commands
{
    public class ItpieCommand : CommandBase, ICommand
    {
        private readonly HttpClient client;

        public override string Name { get { return "itpie"; } }
        public override string[] Aliases { get { return Array.Empty<string>(); } }

        public ItpieCommand(ContextStack stack)
        {
            this.stack = stack;

            if (this.stack.AppSettings.Public.AcceptAllCerts)
            {
                this.client = new HttpClient(new SocketsHttpHandler
                {
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        RemoteCertificateValidationCallback =
                            new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) =>
                            {
                                return true;
                            })
                    }
                });
            }
            else
            {
                this.client = new HttpClient();
            }

            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> Run(string cmd)
        {
            if (string.IsNullOrEmpty(this.stack.AppSettings.Protected?.ItpiePass) ||
                string.IsNullOrEmpty(this.stack.AppSettings.Protected?.ItpieUser))
            {
                this.stack.Storage.GetUsernameAndPassword(this.stack.AppSettings);
            }

            if (string.IsNullOrEmpty(this.stack.AppSettings.Public?.ItpieServerUrl))
            {
                this.stack.Storage.GetItpieServerUrl(this.stack.AppSettings);
            }

            var did_login = await this.DoLogin(this.stack.AppSettings.Protected.ItpieUser, this.stack.AppSettings.Protected.ItpiePass);
            if (!did_login)
            {
                return false;
            }

            this.initItpieContext(this.stack);
            return true;
        }

        private ContextStack initItpieContext(ContextStack stack)
        {
            var commands = new List<ICommand>
            {
                new HelpCommand(stack),
                new PipeCommand(stack),
                new GrepCommand(),
                new PingCommand(stack),
                new TestCommand(stack),

                new Itpie.SetupCommand(stack, this.client)
            };
            var context = new Context(this.Name, commands);
            stack.AddContext(context);
            return stack;
        }

        private class LocalLoginRequest
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }
        }

        private class Token
        {
            [JsonProperty("signed_token")]
            public string SignedToken { get; set; }
        }

        public async Task<bool> DoLogin(string user, string pass)
        {
            try
            {
                var response = await this.client.PostAsJsonAsync($"{this.stack.AppSettings.Public.IptieApiUrl}/authentication/login",
                    new LocalLoginRequest
                    {
                        Email = user,
                        Password = pass
                    });

                if (!response.IsSuccessStatusCode)
                {
                    var c = await response.Content.ReadAsStringAsync();
                    var d = JsonConvert.DeserializeObject<ProblemDetails>(c);
                    ContextStack.WriteLine();
                    ContextStack.WriteLine("Unable to login.");
                    foreach (var ext in d.Extensions.Where(kvp => kvp.Key == "errors"))
                    {
                        var dict = ((Newtonsoft.Json.Linq.JObject)ext.Value).ToObject<Dictionary<string, string[]>>();
                        foreach (var msg in dict.SelectMany(o => o.Value))
                        {
                            ContextStack.WriteLine(msg);
                        }
                    }

                    ContextStack.WriteLine();
                    ContextStack.WriteStart("Do you want to enter new Credentials? (y/n)");
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Y)
                        {
                            ContextStack.WriteLine();
                            this.stack.Storage.GetUsernameAndPassword(this.stack.AppSettings);
                            await this.DoLogin(this.stack.AppSettings.Protected.ItpieUser, this.stack.AppSettings.Protected.ItpieUser);
                        }

                        break;
                    }
                    ContextStack.WriteLine();

                    return false;
                }
                var token = await response.Content.ReadAsAsync<Token>();
                this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.SignedToken);
                return true;
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException is System.Security.Authentication.AuthenticationException)
                {
                    ContextStack.WriteLine($"ERROR: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        public Help[] GetHelp()
        {
            return new Help[] {
                new Help
                {
                    Command = this,
                    Description = new List<string>
                    {
                        $"A set of commands for interacting with an ITPIE server",
                        "",
                        "Commands:",
                        " > setup"
                    }
                }
            };
        }
    }
}
