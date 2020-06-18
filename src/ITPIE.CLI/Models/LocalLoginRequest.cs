using Newtonsoft.Json;

namespace ITPIE.CLI.Models
{
    public class LocalLoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}