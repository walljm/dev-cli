using Newtonsoft.Json;

namespace ITPIE.CLI.Models
{
    public class Token
    {
        [JsonProperty("signed_token")]
        public string SignedToken { get; set; }
    }
}