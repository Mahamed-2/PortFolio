// Models/Hero.cs (Updated for Database)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace QuestGuildTerminal
{
    public class Hero
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // Will be hashed

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public DateTime LastLoginDate { get; set; } = DateTime.Now;
        public int TotalQuestsCompleted { get; set; } = 0;
        public int CurrentLevel { get; set; } = 1;
        public int ExperiencePoints { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
        public virtual ICollection<HeroAchievement> Achievements { get; set; } = new List<HeroAchievement>();

        public Hero() { }

        public Hero(string username, string password, string email, string phone)
        {
            if (!IsValidUsername(username))
                throw new ArgumentException("Username must be 3-20 characters long and contain only letters and numbers");
            
            if (!IsValidPassword(password))
                throw new ArgumentException("Password must be at least 6 characters with 1 digit, 1 uppercase letter, and 1 special character");

            Username = username;
            Password = HashPassword(password); // Now hashed!
            Email = email;
            Phone = phone;
        }

        private bool IsValidUsername(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && 
                   username.Length >= 3 && 
                   username.Length <= 20 &&
                   Regex.IsMatch(username, @"^[a-zA-Z0-9]+$");
        }

        private bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Length >= 6 &&
                   Regex.IsMatch(password, @"[0-9]") &&
                   Regex.IsMatch(password, @"[A-Z]") &&
                   Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
        }

        private string HashPassword(string password)
        {
            // In a real app, use proper hashing like BCrypt
            // For now, we'll use a simple hash for demonstration
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password + "QuestGuildSalt");
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password)
        {
            return HashPassword(password) == Password;
        }

        public void AddExperience(int points)
        {
            ExperiencePoints += points;
            // Level up every 1000 experience points
            var newLevel = (ExperiencePoints / 1000) + 1;
            if (newLevel > CurrentLevel)
            {
                CurrentLevel = newLevel;
                Console.WriteLine($"🎉 Level Up! You are now Level {CurrentLevel}!");
            }
        }

        public override string ToString()
        {
            return $"Hero: {Username} | Level: {CurrentLevel} | Quests Completed: {TotalQuestsCompleted}";
        }
    }
}