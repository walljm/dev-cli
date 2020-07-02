using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class LoginCommand : ICommand
    {
        public string Name { get { return "login"; } }
        private readonly Stack<Context> stack;
        private HttpClient client;

        public LoginCommand(Stack<Context> stack, bool handleAllCerts = false)
        {
            this.stack = stack;
            this.HandleAcceptAllCertificates(handleAllCerts);
        }

        public async Task<bool> Run(string cmd)
        {
            var user = this.GetUser();
            var pass = this.GetPass();

            var did_login = await this.DoLogin(user, pass);
            if (!did_login)
            {
                return false;
            }

            this.InitAuthorizedContext();

            return true;
        }

        public Context InitAuthorizedContext()
        {
            var findContext = new Context
            {
                Prompt = "itpie#",
                Commands = new List<ICommand>
                {
                    new HelpCommand(this.stack),
                    new PipeCommand(this.stack),
                    new GrepCommand(),
                    new FindCommand(this.stack, this.client),
                    new SetCommand(this.stack),
                    new AboutCommand(this.stack)
                },
                Variables = this.stack.Peek().Variables // transfer the variables.
            };
            this.stack.Push(findContext);
            Console.WriteLine();
            return findContext;
        }

        public string GetPass()
        {
            var pass = string.Empty;
            var env_pass = this.GetEnvPass();
            if (env_pass != null)
            {
                pass = env_pass;
            }
            else if (this.stack.Peek().Variables.ContainsKey(Constants.Pass))
            {
                pass = this.stack.Peek().Variables[Constants.Pass].ToString();
            }
            else
            {
                Console.Write("Password: ");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    pass += key.KeyChar;
                }
            }

            this.stack.Peek().Variables[Constants.Pass] = pass;
            return pass;
        }

        public string GetEnvPass()
        {
            return Environment.GetEnvironmentVariable(Constants.EnvironmentPasswordVarName);
        }

        public string GetUser()
        {
            var env_user = this.GetEnvUser();
            string user;
            if (env_user != null)
            {
                user = env_user;
            }
            else if (this.stack.Peek().Variables.ContainsKey(Constants.User))
            {
                user = this.stack.Peek().Variables[Constants.User].ToString();
            }
            else
            {
                Console.Write("Username: ");
                user = Console.ReadLine();
            }

            this.stack.Peek().Variables[Constants.User] = user;
            return user;
        }

        public string GetEnvUser()
        {
            return Environment.GetEnvironmentVariable(Constants.EnvironmentUsernameVarName);
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

        public async Task<bool> DoLogin(string user, string pass)
        {
            var ctx = this.stack.Peek();
            var itpieUrl = ctx.Variables[Constants.ItpieUrl].ToString().TrimEnd('/');
            try
            {
                var response = await this.client.PostAsJsonAsync($"{itpieUrl}/authentication/login",
                    new LocalLoginRequest
                    {
                        Email = user,
                        Password = pass
                    });

                if (!response.IsSuccessStatusCode)
                {
                    var c = await response.Content.ReadAsStringAsync();
                    var d = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(c);
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
                    Console.WriteLine($"To accept invalid certificates, set the {Constants.AcceptAllCertificatesCommand} variable to 'true'");
                }
                return false;
            }
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name);
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help
                {
                    Command = $"login",
                    Description = new List<string>
                    {
                        "Authenticates the user with the itpie server."
                    }
                }
            };
        }
    }
}
