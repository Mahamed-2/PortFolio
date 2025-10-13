// Models/Achievement.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuestGuildTerminal
{
    public class HeroAchievement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int HeroId { get; set; }

        [ForeignKey("HeroId")]
        public virtual Hero Hero { get; set; }

        [Required]
        public AchievementType AchievementType { get; set; }

        [Required]
        public DateTime EarnedDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string Description { get; set; }
    }

    public enum AchievementType
    {
        FirstQuest,
        QuestMaster,
        DeadlineCrusher,
        HeroicPerseverance,
        GuildLegend
    }
}