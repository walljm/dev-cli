using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace CLI.Settings
{
    public class Storage
    {
        private readonly IDataProtector protector;

        private readonly string protectedFilePath;
        private readonly string settingsFilePath;

        private static readonly HashSet<string> yesAnswers = new() { "Y", "y", "yes" };

        // the 'provider' parameter is provided by DI
        public Storage()
        {
            this.protectedFilePath = Path.Combine(AppContext.BaseDirectory, "protected.settings");
            this.settingsFilePath = Path.Combine(AppContext.BaseDirectory, "public.settings");
            var name = $"{Assembly.GetExecutingAssembly().GetName().Name}.{nameof(CommandLineInterface)}";
            var destFolder = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), name);

            // Instantiate the data protection system at this folder
            var dataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(destFolder));
            this.protector = dataProtectionProvider.CreateProtector(name);
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
            else
            {
                settings.Protected = new ProtectedSettings();
            }

            if (File.Exists(this.settingsFilePath))
            {
                settings.Public = JsonConvert.DeserializeObject<PublicSettings>(File.ReadAllText(this.settingsFilePath));
            }
            else
            {
                settings.Public = new PublicSettings();
            }

            return settings; // return defaults.
        }

        public void GetUsernameAndPassword(AppSettings settings, bool prompt = true)
        {
            // if you're storing the protected settings, then you can safely reset them here.
            settings.Protected = new ProtectedSettings();
            ContextStack.WriteLine("Please enter your ITPIE API credentials ");

            // get username and password.
            ContextStack.WriteStart("Username: ");
            settings.Protected.ItpieUser = Console.ReadLine();

            ContextStack.WriteStart("Password: ");
            settings.Protected.ItpiePass = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                settings.Protected.ItpiePass += key.KeyChar;
            }

            ContextStack.WriteLine();
            if (prompt)
            {
                this.promptToStoreSettings(settings);
            }
        }

        public void GetItpieServerUrl(AppSettings settings)
        {
            // get username and password.
            ContextStack.WriteStart("Please enter your ITPIE URL (e.g. https://itpie.com): ");
            settings.Public.ItpieServerUrl = Console.ReadLine();

            this.promptToStoreSettings(settings);
        }

        public void promptToStoreSettings(AppSettings settings)
        {
            ContextStack.WriteStart("Write to settings file? (y/n): ");
            var key = Console.ReadLine();
            if (yesAnswers.Contains(key.Trim()))
            {
                // store whatever settings were passed in.
                this.StoreSettings(settings);
            }
            ContextStack.WriteLine();
        }
    }
}
