// Models/Achievement.cs
namespace QuestGuildTerminal
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EarnedDate { get; set; }
        public int HeroId { get; set; }
        public virtual Hero Hero { get; set; }

        public Achievement()
        {
            EarnedDate = DateTime.Now;
        }

        public Achievement(string name, string description, int heroId) : this()
        {
            Name = name;
            Description = description;
            HeroId = heroId;
        }
    }
}