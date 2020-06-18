using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CommandLine;
using ITPIE.CLI.Commands;

namespace ITPIE.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            CommandLineOptions opts = null;
            Parser.Default
                .ParseArguments<CommandLineOptions>(args)
                .WithParsed(options =>
                {
                    opts = options;
                });

            var stack = initStack();
            if (!setItpieUrl(stack, opts))
            {
                return; // you can't do anything without an itpie api url to work against.
            }

            if (opts != null && opts.Command != null)
            {
                // handle login.
                var login = stack.Peek().GetCommand<LoginCommand>();

                var user = string.Empty;
                if (opts.User != null) // prefer the commandline.
                {
                    user = opts.User;
                }
                else
                {
                    // no commandline user?  then get the user the normal way.
                    user = login.GetUser();
                }

                var pass = login.GetPass();
                var did_login = await login.DoLogin(user, pass);
                if (!did_login)
                    return;

                // you survived login, now run the command.
                var authorizedContext = login.InitAuthorizedContext(); // initialize the logged in context.
                await authorizedContext.HandleCommand(opts.Command);

                return;
            }

            // if the user and pass environment vars are set, just log the user in already.
            var lc = stack.Peek().GetCommand<LoginCommand>();
            var u = lc.GetEnvUser();
            var p = lc.GetEnvPass();
            if (u != null && p != null)
            {
                var did_login = await lc.Run(lc.Name);
                if (!did_login)
                    return;
            }
            // say hello to the user.
            var welcome = new List<string>
            {
                "Welcome to the ITPIE CLI!",
            };
            foreach (var str in welcome)
            {
                Console.WriteLine(str);
            }
            var about = stack.Peek().GetCommand<AboutCommand>();
            await about.Run(about.Name);
            Console.WriteLine();

            // enter the interactive loop.
            while (true)
            {
                var context = stack.Peek();
                context.WritePrompt();

                var cmd = Console.ReadLine();
                if (cmd == "exit") // handle a universal exit command.
                {
                    return;
                }

                await context.HandleCommand(cmd);
            }
        }

        private static bool setItpieUrl(Stack<Context> stack, CommandLineOptions opts)
        {
            var env_itpie_url = Environment.GetEnvironmentVariable(Constants.EnvironmentItpieUrlVarName);

            if (opts.ItpieUrl != null)
            {
                stack.Peek().Variables[Constants.ItpieUrl] = opts.ItpieUrl;
            }
            else if (env_itpie_url != null)
            {
                stack.Peek().Variables[Constants.ItpieUrl] = env_itpie_url;
            }
            else
            {
                Console.WriteLine($"You must provide an ITPIE API url.  You can either set the {Constants.EnvironmentItpieUrlVarName} " +
                    $"environment variable or use the commandline flag: -i or --itpieUrl");
                return false;
            }
            return true;
        }

        private static Stack<Context> initStack()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var stack = new Stack<Context>();
            var loginContext = new Context
            {
                Prompt = "itpie>",
                Commands = new List<ICommand>
                {
                    new HelpCommand(stack),
                    new LoginCommand(stack, client),
                    new SetCommand(stack),
                    new AboutCommand(stack)
                },
                Variables = new Dictionary<string, object>()
            };
            stack.Push(loginContext);
            return stack;
        }
    }
}
