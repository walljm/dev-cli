using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace CLI.Settings
{
    public class Storage
    {
        private readonly IDataProtector protector;

        private readonly string protectedFilePath = "protected.settings";
        private readonly string settingsFilePath = "public.settings";

        // the 'provider' parameter is provided by DI
        public Storage(IDataProtectionProvider provider)
        {
            var name = $"{Assembly.GetCallingAssembly().GetName().FullName}.{nameof(CommandLineInterface)}";
            this.protector = provider.CreateProtector(name);
        }

        public void StoreSettings(AppSettings obj)
        {
            if (File.Exists(this.protectedFilePath))
            {
                File.Delete(this.protectedFilePath); // delete the file first.
            }
            if (File.Exists(this.settingsFilePath))
            {
                File.Delete(this.settingsFilePath); // delete the file first.
            }

            File.WriteAllText(this.protectedFilePath, this.protector.Protect(JsonConvert.SerializeObject(obj.Protected)));
            File.WriteAllText(this.settingsFilePath, JsonConvert.SerializeObject(obj.Public));
        }

        public AppSettings RetrieveSettings()
        {
            var settings = new AppSettings();

            if (File.Exists(this.protectedFilePath))
            {
                var encValue = File.ReadAllText(this.protectedFilePath);
                settings.Protected = JsonConvert.DeserializeObject<ProtectedSettings>(this.protector.Unprotect(encValue));
            }
            if (File.Exists(this.settingsFilePath))
            {
                settings.Public = JsonConvert.DeserializeObject<PublicSettings>(File.ReadAllText(this.settingsFilePath));
            }

            return settings; // return defaults.
        }

        public void GetUsernameAndPassword(AppSettings settings, bool store = false)
        {
            // if you're storing the protected settings, then you can safely reset them here.
            settings.Protected = new ProtectedSettings();
            Console.WriteLine(" Please enter your ITPIE API credentials ");

            // get username and password.
            Console.Write(" Username: ");
            settings.Protected.ItpieUser = Console.ReadLine();

            Console.Write(" Password: ");
            settings.Protected.ItpiePass = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                settings.Protected.ItpiePass += key.KeyChar;
            }

            Console.WriteLine();
            if (!store)
            {
                Console.Write(" Write credentials to secure Storage? (y/n): ");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Y)
                    {
                        // store whatever settings were passed in.
                        this.StoreSettings(settings);
                    }

                    break;
                }
                
                Console.WriteLine();
            }
            else
            {
                this.StoreSettings(settings);
            }
        }

        public void GetItpieServerUrl(AppSettings settings)
        {
            // get username and password.
            Console.Write(" Please enter your ITPIE URL (e.g. https://itpie.com): ");
            settings.Public.ItpieServerUrl = Console.ReadLine();

            Console.Write(" Write to settings file? (y/n): ");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    // store whatever settings were passed in.
                    this.StoreSettings(settings);
                }

                break;
            }
            Console.WriteLine();
        }
    }
}
