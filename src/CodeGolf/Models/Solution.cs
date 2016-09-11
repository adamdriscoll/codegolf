using System;

namespace CodeGolf.Models
{
    public class Solution : CodeGolfDocument
    { 
        public string Content { get; set; }

        public int Length => Content.Length;

        public Language Language { get; set; }

        public Guid Problem { get; set; }

        public Guid Author { get; set; }

        public bool? Passing { get; set; }

        public override DocumentType Type => DocumentType.Solution;

        public int Votes { get; set; }
    }
}