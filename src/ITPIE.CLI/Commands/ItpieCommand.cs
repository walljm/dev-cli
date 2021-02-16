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

#pragma warning disable 1998

namespace CLI.Commands
{
    public class ItpieCommand : CommandBase, ICommand
    {
        private HttpClient client;
        private string itpieApiUrl = "http://localhost:5000/api";
        private const string prompt = "itpie";
        private const string projectPath = @"c:\Projects\vae\operations";
        private const string projectPathEnvName = @"c:\Projects\vae\operations";

        public override string Name { get { return "itpie"; } }
        public override string[] Aliases { get { return new string[] { }; } }

        public ItpieCommand(ContextStack stack)
        {
            this.stack = stack;
            this.HandleAcceptAllCertificates(true);
        }

        public async Task<bool> Run(string cmd)
        {
            if (this.stack.Peek().HasEnvVariable(Constants.EnvironmentItpieUrlVarName))
            {
                this.itpieApiUrl = this.stack.Peek().GetEnvVariable(Constants.EnvironmentItpieUrlVarName);
            }

            var user = this.GetUser();
            var pass = this.GetPass();

            var did_login = await this.DoLogin(user, pass);
            if (!did_login)
            {
                return false;
            }

            this.initItpieContext(this.stack);

            this.stack.Peek().SetEnvVariable(Constants.EnvironmentPasswordVarName, pass);
            this.stack.Peek().SetEnvVariable(Constants.EnvironmentUsernameVarName, user);
            this.stack.Peek().SetEnvVariable(Constants.EnvironmentItpieUrlVarName, this.itpieApiUrl);
            return true;
        }

        private ContextStack initItpieContext(ContextStack stack)
        {
            var context = new Context
            {
                Prompt = $"{prompt}",
                Commands = Context.GetDefaultCommands(stack).Where(c => c.Name != this.Name).ToList()
            };
            context.Commands.Add(new Itpie.SetupCommand(stack, this.client));
            context.SetEnvVariable(projectPathEnvName, projectPath);
            stack.Push(context);
            Console.WriteLine();

            return stack;
        }

        public string GetPass()
        {
            var pass = string.Empty;
            if (this.stack.Peek().HasEnvVariable(Constants.EnvironmentPasswordVarName))
            {
                pass = this.stack.Peek().GetEnvVariable(Constants.EnvironmentPasswordVarName);
            }
            else
            {
                this.stack.WriteStart("Password: ");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    pass += key.KeyChar;
                }
            }

            return pass;
        }

        public string GetUser()
        {
            string user;
            if (this.stack.Peek().HasEnvVariable(Constants.EnvironmentUsernameVarName))
            {
                user = this.stack.Peek().GetEnvVariable(Constants.EnvironmentUsernameVarName);
            }
            else
            {
                this.stack.WriteStart("Username: ");
                user = Console.ReadLine();
            }

            return user;
        }

        public void HandleAcceptAllCertificates(bool acceptAllCerts)
        {
            if (acceptAllCerts)
            {
                var handler = new SocketsHttpHandler
                {
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        RemoteCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) =>
                        {
                            return true;
                        })
                    }
                };
                this.client = new HttpClient(handler);
                this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            else
            {
                this.client = new HttpClient();
                this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
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
            var ctx = this.stack.Peek();
            try
            {
                var response = await this.client.PostAsJsonAsync($"{this.itpieApiUrl}/authentication/login",
                    new LocalLoginRequest
                    {
                        Email = user,
                        Password = pass
                    });

                if (!response.IsSuccessStatusCode)
                {
                    var c = await response.Content.ReadAsStringAsync();
                    var d = JsonConvert.DeserializeObject<ProblemDetails>(c);
                    Console.WriteLine();
                    Console.WriteLine("Unable to login.");
                    foreach (var ext in d.Extensions.Where(kvp => kvp.Key == "errors"))
                    {
                        var dict = ((Newtonsoft.Json.Linq.JObject)ext.Value).ToObject<Dictionary<string, string[]>>();
                        foreach (var msg in dict.SelectMany(o => o.Value))
                        {
                            Console.WriteLine(msg);
                        }
                    }
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
                    Console.WriteLine($"ERROR: {ex.InnerException.Message}");
                }
                return false;
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
                        $"A set of commands for interacting with itpie",
                        "",
                        "Aliases:",
                        $"  {string.Join(" | ", this.Aliases)}",
                    }
                }
            };
        }
    }
}
