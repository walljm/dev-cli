using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CLI.Commands.Itpie.Models
{
    public class JobResponse
    {
        [Required]
        public string Name { get; set; }

        public int Id { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public IEnumerable<string> JobTypes { get; set; }

        public IEnumerable<CollectionJobTargetResponse> Targets { get; set; }
    }

    public class CollectionJobTargetResponse
    {
        public int Id { get; set; }
        public int CollectionJobId { get; set; }
        public string Protocol { get; set; }
        public IEnumerable<string> IncludedRanges { get; set; }
        public IEnumerable<string> ExcludedRanges { get; set; }
    }

    public class JobWithTargetRequest
    {
        [Required]
        public string Name { get; set; }

        public IEnumerable<string> JobTypes { get; set; }
        public string Protocol { get; set; }

        public string IncludedRanges { get; set; }
    }

    public class CredentialGroupRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; } = string.Empty;
        public IEnumerable<CredentialRequest> Credentials { get; set; } = new List<CredentialRequest>();

        public string IncludedRanges { get; set; } = string.Empty;

        public string ExcludedRanges { get; set; } = string.Empty;
    }

    public class CredentialRequest
    {
        [Required]
        public string Name { get; set; }

        public bool IsPrivileged { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }

        public string AuthenticationProtocol { get; set; } = "None";
        public string PrivacyProtocol { get; set; } = "None";
        public string SecurityLevel { get; set; } = "None";
        public string CommunityString { get; set; } = string.Empty;
        public string PrivacyPassphrase { get; set; } = string.Empty;
        public string AuthenticationPassphrase { get; set; } = string.Empty;
    }
}
