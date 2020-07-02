using System;
using System.Collections.Generic;
using System.Linq;
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
            bool stop = false;
            Parser.Default
                .ParseArguments<CommandLineOptions>(args)
                .WithParsed(options =>
                {
                    opts = options;
                })
                .WithNotParsed(errors =>
                {
                    stop = errors.Any(e => e.StopsProcessing);
                });

            if (stop || opts == null)
            {
                return;
            }

            var stack = initStack(opts);

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
                var env_as_json = Environment.GetEnvironmentVariable(Constants.EnvironmentOutputAsJson);
                if (env_as_json == true.ToString() || opts.OutputAsJson)
                {
                    authorizedContext.Variables[Constants.EnvironmentOutputAsJson] = true;
                }

                await authorizedContext.HandleCommand(opts.Command);

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

            // if the user and pass environment vars are set, just log the user in already.
            var lc = stack.Peek().GetCommand<LoginCommand>();
            var u = lc.GetEnvUser();
            var p = lc.GetEnvPass();
            if (u != null && p != null)
            {
                await lc.Run(lc.Name);
            }

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
                stack.Peek().Variables[Constants.ItpieUrl] = $"{opts.ItpieUrl}/api";
            }
            else if (env_itpie_url != null)
            {
                stack.Peek().Variables[Constants.ItpieUrl] = $"{env_itpie_url}/api";
            }
            else
            {
                Console.WriteLine("You must provide an ITPIE API url.  You can avoid this prompt by either");
                Console.WriteLine($" setting the {Constants.EnvironmentItpieUrlVarName} environment variable");
                Console.WriteLine("  or by using the commandline flag: -i or --itpieUrl");
                Console.Write("Please type the url of the ITPIE API (e.g https://youritpieserver.com/): ");
                var url = Console.ReadLine();
                stack.Peek().Variables[Constants.ItpieUrl] = $"{url}/api";
            }
            return true;
        }

        private static Stack<Context> initStack(CommandLineOptions opts)
        {
            var handleAllCerts = false;
            if (opts.AcceptAllCertificates)
            {
                handleAllCerts = opts.AcceptAllCertificates;
            }
            var env_accept_all_certificates = Environment.GetEnvironmentVariable(Constants.EnvironmentAcceptAllCertificates);
            if (env_accept_all_certificates != null)
            {
                handleAllCerts = env_accept_all_certificates == true.ToString();
            }

            var stack = new Stack<Context>();
            var loginContext = new Context
            {
                Prompt = "itpie>",
                Commands = new List<ICommand>
                {
                    new HelpCommand(stack),
                    new LoginCommand(stack, handleAllCerts),
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
