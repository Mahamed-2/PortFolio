// Data/QuestGuildContext.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestGuildTerminal.Data
{
    public class QuestGuildContext : DbContext
    {
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<HeroAchievement> HeroAchievements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use LocalDB for development
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=QuestGuildTerminal;Trusted_Connection=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints
            modelBuilder.Entity<Hero>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.HasIndex(h => h.Username).IsUnique();
                entity.HasIndex(h => h.Email).IsUnique();
                
                entity.HasMany(h => h.Quests)
                      .WithOne(q => q.Hero)
                      .HasForeignKey(q => q.HeroId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(h => h.Achievements)
                      .WithOne(a => a.Hero)
                      .HasForeignKey(a => a.HeroId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Quest>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.HasIndex(q => new { q.HeroId, q.Title });
                
                entity.Property(q => q.Priority)
                      .HasConversion<string>();
            });

            modelBuilder.Entity<HeroAchievement>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.HasIndex(a => new { a.HeroId, a.AchievementType }).IsUnique();
            });
        }
    }
}