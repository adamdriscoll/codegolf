using System;

namespace CodeGolf.Models
{
    public class DocumentVersion : CodeGolfDocument
    {
        public Version Version { get; set; }
        public override DocumentType Type => DocumentType.Version;
    }
}
