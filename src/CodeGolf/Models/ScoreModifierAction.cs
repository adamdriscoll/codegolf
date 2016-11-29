using System;

namespace CodeGolf.Models
{
    public class ScoreModifierAction : CodeGolfDocument
    {
        public int Value { get; set; }

        public User User { get; set; }

        public Guid Modifier { get; set; }

        public ModifierType ModifierType { get; set; }

        public override DocumentType Type { get; }
    }

    public enum ModifierType
    {
        Problem,
        Solution,
        Vote
    }
}
