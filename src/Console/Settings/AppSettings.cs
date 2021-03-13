namespace CLI.Settings
{
    public class AppSettings
    {
        public ProtectedSettings Protected { get; set; }
        public PublicSettings Public { get; set; }

        public bool IsProtectedValid()
        {
            return this.Protected != null &&
                   this.Protected.ItpiePass != null &&
                   this.Protected.ItpieUser != null;
        }
        
        public bool IsPublicValid()
        {
            return this.Public != null &&
                   this.Public.ItpieServerUrl!= null;
        }

        public const string InvalidProtectedMessage = "A username and password must be populated.  Please run the application with the -s|--store option to securely store the credentials.";
        public const string InvalidPublicMessage = "An ITPIE Url must be provided either by using the -i|--itpie option or from a previously stored value in the public.settings file.";
    }
}
