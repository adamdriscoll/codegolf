namespace CodeGolf.Models
{
    public class User : CodeGolfDocument
    {
        public string Identity { get; set; }
        public string Authentication { get; set; }
        public int Score { get; set; }
        public override DocumentType Type => DocumentType.User;
    }
}
