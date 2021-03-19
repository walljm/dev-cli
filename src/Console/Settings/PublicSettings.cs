using System;

namespace CLI.Settings
{
    public class PublicSettings
    {
        public string DefaultPrompt { get; set; } = "sdev";
        public string ItpieServerUrl { get; set; }
        public string IptieApiUrl { get { return $"{this.ItpieServerUrl}/api"; } }
        public bool AcceptAllCerts { get; set; } = false;
        public string ItpieProjectPath = @"c:\Projects\vae\operations";

        public void PrintSettings()
        {
            Console.WriteLine($"  {nameof(this.ItpieProjectPath).SpaceByCamelCase(),20}: {this.ItpieProjectPath}");
            Console.WriteLine($"  {nameof(this.ItpieServerUrl).SpaceByCamelCase(),20}: {this.ItpieServerUrl}");
            Console.WriteLine($"  {nameof(this.IptieApiUrl).SpaceByCamelCase(),20}: {this.IptieApiUrl}");
            Console.WriteLine($"  {nameof(this.AcceptAllCerts).SpaceByCamelCase(),20}: {this.AcceptAllCerts}");
        }

        public void UpdateWithCommandLineSettings(CommandLineSettings cls)
        {
            // only update the values if they aren't null.
            // we assume that the default value will be null.  this is how we
            //  determine if something was passed in via commandline or if
            //  nothing was passed in, or if the values are coming from settings
            //  saved in the settings file.
            this.ItpieServerUrl = cls.ItpieApiUrl ?? this.ItpieServerUrl;
            this.AcceptAllCerts = cls.AcceptAllCerts ?? this.AcceptAllCerts;
            this.ItpieProjectPath = cls.ItpieProjectPath ?? this.ItpieProjectPath;
            this.DefaultPrompt = cls.DefaultPrompt ?? this.DefaultPrompt;
        }
    }
}
