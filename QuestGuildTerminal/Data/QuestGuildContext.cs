// Data/QuestGuildContext.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace QuestGuildTerminal.Data
{
    public class QuestGuildContext : DbContext
    {
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Achievement> Achievements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=questguild.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships if needed
            modelBuilder.Entity<Quest>()
                .HasOne(q => q.Hero)
                .WithMany(h => h.Quests)
                .HasForeignKey(q => q.HeroId);

            modelBuilder.Entity<Achievement>()
                .HasOne(a => a.Hero)
                .WithMany(h => h.Achievements)
                .HasForeignKey(a => a.HeroId);
        }
    }
}