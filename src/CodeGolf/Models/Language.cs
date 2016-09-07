using CodeGolf.Api.Models;

namespace CodeGolf.Models
{
    public class Language : CodeGolfDocument
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public bool SupportsValidation { get; set; }

        public override DocumentType Type => DocumentType.Language;
    }
}