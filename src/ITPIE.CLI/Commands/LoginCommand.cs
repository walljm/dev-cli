﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class LoginCommand : ICommand
    {
        public string Name { get { return "login"; } }
        private readonly Stack<Context> stack;
        private readonly HttpClient client;

        public LoginCommand(Stack<Context> stack, HttpClient client)
        {
            this.stack = stack;
            this.client = client;
        }

        public async Task<bool> Run(string cmd)
        {
            string user = this.GetUser();
            string pass = this.GetPass();

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
            var env_pass = GetEnvPass();
            if (env_pass != null)
            {
                pass = env_pass;
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

            return pass;
        }

        public string GetEnvPass()
        {
            return Environment.GetEnvironmentVariable(Constants.EnvironmentPasswordVarName);
        }

        public string GetUser()
        {
            var env_user = GetEnvUser();
            if (env_user != null)
            {
                return env_user;
            }
            else
            {
                Console.Write("Username: ");
                return Console.ReadLine();
            }
        }

        public string GetEnvUser()
        {
            return Environment.GetEnvironmentVariable(Constants.EnvironmentUsernameVarName);
        }

        public async Task<bool> DoLogin(string user, string pass)
        {
            var ctx = this.stack.Peek();
            var itpieUrl = ctx.Variables[Constants.ItpieUrl].ToString().TrimEnd('/');
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

            ctx.Variables[Constants.User] = user;
            ctx.Variables[Constants.Pass] = pass;

            return true;
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
