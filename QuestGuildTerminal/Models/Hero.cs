// Models/Hero.cs
using System;
using System.Collections.Generic;

namespace QuestGuildTerminal
{
    public class Hero
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public string Class { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties (for Entity Framework - you can ignore if using raw SQL)
        public virtual ICollection<Quest> Quests { get; set; }
        public virtual ICollection<Achievement> Achievements { get; set; }

        // Default constructor
        public Hero()
        {
            Level = 1;
            Experience = 0;
            Class = "Adventurer";
            LastLoginDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            Quests = new List<Quest>();
            Achievements = new List<Achievement>();
        }

        // Constructor for registration
        public Hero(string username, string password, string email, string phone) : this()
        {
            Username = username;
            Password = password;
            Email = email;
            Phone = phone;
        }

        // Full constructor
        public Hero(string username, string password, string email, string phone, int level, int experience, string heroClass) : this()
        {
            Username = username;
            Password = password;
            Email = email;
            Phone = phone;
            Level = level;
            Experience = experience;
            Class = heroClass;
        }

        // Add this method to fix the VerifyPassword error
        public bool VerifyPassword(string password)
        {
            return Password == password; // Simple comparison - in production, use hashing!
        }
    }
}