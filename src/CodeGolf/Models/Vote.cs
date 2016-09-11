using System;

namespace CodeGolf.Models
{
    public class Vote : CodeGolfDocument
    {
        public Guid Item { get; set; }
        public DocumentType ItemType { get; set; }

        public Guid Voter { get; set; }

        public int Value { get; set; }
        public override DocumentType Type => DocumentType.Vote;
    }
}
