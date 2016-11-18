using System;
using Newtonsoft.Json;

namespace CodeGolf.Models
{
    public class SolutionComment 
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        public string Comment { get; set; }

        public User Commentor { get; set; }

        public Guid Solution { get; set; }
    }
}
