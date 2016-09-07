using System;

namespace CodeGolf.Models
{
    public class DocumentVersion : CodeGolfDocument
    {
        public string Version { get; set; }
        public override DocumentType Type => DocumentType.Version;
    }
}
