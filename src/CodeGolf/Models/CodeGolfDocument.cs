using System;
using Newtonsoft.Json;

namespace CodeGolf.Models
{
    public abstract class CodeGolfDocument
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        public abstract DocumentType Type { get; }

        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }
    }

    public enum DocumentType
    {
        Problem,
        Solution,
        Language,
        User,
        Version,
        Vote
    }
}