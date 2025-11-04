// Models/Quest.cs (ensure it has proper ID handling)
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuestGuildTerminal
{
    public class Quest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public Priority Priority { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? CompletedDate { get; set; }

        // Foreign key (for database version)
        public int HeroId { get; set; }

        // Navigation property (for database version)
        [ForeignKey("HeroId")]
        public virtual Hero Hero { get; set; }
        public bool RequiresGameCompletion { get; set; }
        public string RequiredGame { get; set; } = "Tetris";
        public int RequiredGameLevel { get; set; } = 3;
        public bool IsGameCompleted { get; set; } = false;

        // Constructor for simple usage (without heroId)
        public Quest(string title, string description, DateTime dueDate, Priority priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            HeroId = 0; // Default value for non-database usage
        }

        // Constructor for database usage (with heroId)
        public Quest(string title, string description, DateTime dueDate, Priority priority, int heroId)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            HeroId = heroId;
        }

        public bool IsNearDeadline()
        {
            return !IsCompleted && (DueDate - DateTime.Now).TotalHours <= 24;
        }

        public void MarkComplete()
    {
        if (RequiresGameCompletion && !IsGameCompleted)
        {
            throw new InvalidOperationException("Must complete game challenge first!");
        }
        
        IsCompleted = true;
        CompletedDate = DateTime.Now;
    }
    
    public void MarkGameCompleted()
    {
        IsGameCompleted = true;
        // Don't auto-complete quest, let them decide when to submit
    }

        public override string ToString()
        {
            var status = IsCompleted ? "✅ COMPLETED" : "⚔️ ACTIVE";
            var deadlineWarning = IsNearDeadline() && !IsCompleted ? " ⚠️ DEADLINE NEAR!" : "";
            return $"[{Id}] {Title} | Due: {DueDate:yyyy-MM-dd} | Priority: {Priority} | {status}{deadlineWarning}\n   Description: {Description}";
        }
    }
}