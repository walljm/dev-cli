﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using ITPIE.CLI.Models;

namespace ITPIE.CLI.Commands
{
    public class FindCommand : ICommand
    {
        public string Name { get { return "find"; } }

        private string[] aliases { get { return new[] { "show", "display" }; } }
        private readonly HttpClient client;
        private readonly Stack<Context> stack;

        public FindCommand(Stack<Context> stack, HttpClient client)
        {
            this.client = client;
            this.stack = stack;
        }

        public async Task<bool> Run(string cmd)
        {
            var terms = cmd.Split(' ');
            if (terms.Length == 1)
            {
                Console.WriteLine($"  You must supply a sub command.");
                HelpCommand.WriteHelp(new List<ICommand> { this }, false, false);
                return false;
            }

            var sub = terms[1];
            var obj = terms.Length < 3 ? "*" : terms[2];  // default to wildcard
            var opt = terms.Length < 4 ? string.Empty : terms[3]; // default to empty.

            switch (sub)
            {
                case "device":
                    await this.HandleDevice(obj);
                    break;

                case "arp":
                    await this.HandleArp(obj, opt);
                    break;

                case "interface":
                    await this.HandleInterface(obj);
                    break;

                case "neighbor":
                    await this.HandleNeighbor(obj);
                    break;

                case "stp":
                    await this.HandleStp(obj, opt);
                    break;

                case "vlan":
                    await this.HandleVlan(obj, opt);
                    break;

                case "vrf":
                    await this.HandleVrf(obj, opt);
                    break;

                case "route":
                    await this.HandleRoute(obj, opt);
                    break;

                default:
                    Console.WriteLine("  That sub command is not supported.");
                    HelpCommand.WriteHelp(new List<ICommand> { this }, false, false);
                    break;
            }
            return true;
        }

        public async Task HandleDevice(string obj)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            var url = $"{itpieUrl}/device?term={obj}";
            await this.HandleResponse<DeviceSearchResult>(url);
        }

        public async Task HandleArp(string obj, string opt)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            var url = $"{itpieUrl}/arp?term={obj}";
            if (opt == "history")
            {
                url = $"{itpieUrl}/arp/history?term={obj}";
            }
            await this.HandleResponse<ArpSearchResult>(url);
        }

        public async Task HandleInterface(string obj)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            var url = $"{itpieUrl}/interface?term={obj}";
            await this.HandleResponse<InterfaceSearchResult>(url);
        }

        public async Task HandleNeighbor(string obj)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            var url = $"{itpieUrl}/neighbor?term={obj}";
            await this.HandleResponse<NeighborSearchResult>(url);
        }

        public async Task HandleStp(string obj, string opt)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            if (opt == "interface")
            {
                var url = $"{itpieUrl}/arp/interface?term={obj}";
                await this.HandleResponse<StpInterfaceSearchResult>(url);
            }
            else
            {
                var url = $"{itpieUrl}/stp?term={obj}";

                await this.HandleResponse<StpSearchResult>(url);
            }
        }

        public async Task HandleVlan(string obj, string opt)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            if (opt == "interface")
            {
                var url = $"{itpieUrl}/vlan/interface?term={obj}";
                await this.HandleResponse<VlanSearchInterfaceResult>(url);
            }
            else if (opt == "detail")
            {
                var url = $"{itpieUrl}/vlan/detail?term={obj}";
                await this.HandleResponse<VlanSearchDetailResult>(url);
            }
            else
            {
                var url = $"{itpieUrl}/vlan?term={obj}";
                await this.HandleResponse<VlanSearchResult>(url);
            }
        }

        public async Task HandleVrf(string obj, string opt)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            var url = $"{itpieUrl}/vrf?term={obj}";
            if (opt == "loopback")
            {
                url = $"{itpieUrl}/vrf/loopback?term={obj}";
            }

            await this.HandleResponse<VrfSearchResult>(url);
        }

        public async Task HandleRoute(string obj, string opt)
        {
            var itpieUrl = $"{this.stack.Peek().Variables[Constants.ItpieUrl]}/search";
            var url = $"{itpieUrl}/route?term={obj}&option={opt}";

            if (opt == "vrf")
            {
                url = $"{itpieUrl}/route/vrf?term={obj}&option={opt}";
            }

            await this.HandleResponse<RouteSearchResult>(url);
        }

        private class Column
        {
            public int Width { get; set; }
            public string Name { get; set; }
            public PropertyInfo Info { get; set; }
            public Formatters Formatter { get; set; }
            public int DisplayIndex { get; set; }

            public string GetValue(object o)
            {
                if (this.Formatter == Formatters.Interval)
                {
                    var v = this.Info.GetValue(o);
                    if (v == null)
                        return "null";
                    return new TimeSpan(((long)v * 10000000)).ToString();
                }
                else
                {
                    return this.Info.GetValue(o)?.ToString() ?? "null";
                }
            }
        }

        private async Task HandleResponse<T>(string url)
        {
            var r = await this.client.GetAsync(url);

            if (r.IsSuccessStatusCode)
            {
                var c = await r.Content.ReadAsAsync<PagedResponse<T>>();
                WriteResponse(c);
                return;
            }

            if (r.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var lc = this.stack.Peek().GetCommand<LoginCommand>();
                var u = lc.GetEnvUser();
                var p = lc.GetEnvPass();
                if (u != null && p != null)
                {
                    var did_login = await lc.Run(lc.Name);
                    if (!did_login)
                    {
                        Console.WriteLine("The current user is not authorized to login to the provided ITPIE api.");
                        return;
                    }
                }
                await this.HandleResponse<T>(url); // try again now that you have logged back into the server.
                return;
            }

            var err = await r.Content.ReadAsStringAsync();
            Console.WriteLine($"An Error occurred while speaking to the api: {r.StatusCode} {r.ReasonPhrase}\n{err}");
        }

        private static void WriteResponse<T>(PagedResponse<T> c)
        {
            var fArray = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(propInfo =>
                {
                    var attribute = propInfo.GetCustomAttributes<ColumnDisplayAttribute>().FirstOrDefault();
                    return new Column
                    {
                        Info = propInfo,
                        Name = attribute?.Name ?? propInfo.Name.SpaceByCamelCase(),
                        DisplayIndex = attribute?.DisplayIndex ?? int.MaxValue,
                        Formatter = attribute?.Formatter ?? Formatters.None,
                        Width = 0
                    };
                })
                .OrderBy(info => info.DisplayIndex)
                .ToList();

            foreach (var prop in fArray)
            {
                prop.Width = prop.Width < prop.Name.Length ? prop.Name.Length : prop.Width;
                foreach (var d in c.Items)
                {
                    var v = prop.GetValue(d);
                    prop.Width = prop.Width < v.Length ? v.Length : prop.Width;
                }
            }

            // column headers.
            foreach (var prop in fArray)
            {
                Console.Write(prop.Name.PadRight(prop.Width + 2));
            }
            Console.WriteLine();

            // column divider
            foreach (var prop in fArray)
            {
                Console.Write($"{string.Empty.PadRight(prop.Width, '-')}  ");
            }
            Console.WriteLine();

            // data
            foreach (var d in c.Items)
            {
                foreach (var prop in fArray)
                {
                    var v = prop.GetValue(d);
                    Console.Write(v.PadRight(prop.Width + 2));
                }
                Console.WriteLine();
            }
        }

        public bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name) || this.aliases.Any(c => cmd.StartsWith(c));
        }

        public Help[] GetHelp()
        {
            return new Help[]{
                new Help
                {
                    Command = $"find",
                    Description = new List<string>
                    {
                        "Intelligently search for specific types of information on your network.",
                        "",
                        "Aliases:",
                        "  show | display",
                        "",
                        "Sub Commands:",
                        "  find device <ip|hostname|network>",
                        "",
                        "  find interface <desc>",
                        "",
                        "  find neighbor <ip|hostname>",
                        "  find neighbor <ip|hostname> detail",
                        "",
                        "  find vlan <id|name>",
                        "  find vlan <id|name> detail",
                        "  find vlan <vlanid|ip|hostname> interface",
                        "",
                        "  find stp <vlanid|mstid|bridgeaddress|ip|hostname>",
                        "  find stp <vlanid|mstid|bridgeaddress|ip|hostname> detail",
                        "  find stp <vlanid|mstid|bridgeaddress|ip|hostname> interface",
                        "",
                        "  find arp <ip|network>",
                        "  find arp <ip|network> history",
                        "",
                        "  find route <ip|prefix>",
                        "  find route <ip|prefix> all",
                        "  find route <ip|prefix> exact",
                        "  find route <ip|prefix> vrf <routing-instance|vrf>",
                        "",
                        "  find vrf <name|ip|network> loopback",
                        "  find vrf <name|ip|network>",
                    }
                }
            };
        }
    }
}
