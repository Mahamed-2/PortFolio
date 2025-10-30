

 📖 Table of Contents
- [🌟 Overview]
- [⚡ Features]
- [🚀 Quick Start]
- [🏗️ Project Structure]
- [🎮 How to Use]
- [🔧 Configuration]


 🌟 Overview

Quest Guild Terminal is an epic console application where you become a heroic adventurer managing quests, battling deadlines, and receiving guidance from your trusty AI Guild Advisor! Built with C and Object-Oriented Programming principles.

> "Every great hero needs a trusty quest log!" ⚔️

 ⚡ Features

| Feature | Description | Status |
|---------|-------------|--------|
| 🏹 Hero Management | Create your hero profile with secure authentication | ✅ |
| 📜 Quest System | Add, complete, and track quests with deadlines | ✅ |
| 🤖 AI Guild Advisor | Get AI-generated quest descriptions and summaries | ✅ |
| 🔔 Smart Notifications | Deadline alerts and progress tracking | ✅ |
| 🏆 Achievement System | Earn badges and level up your hero | ✅ |
| 📧 Real Notifications | Email & SMS integration for 2FA | 🔄 |
| 💾 Data Persistence | SQLite database for saving progress | ✅ |

 🚀 Quick Start

 Prerequisites
- [.NET 9.0]
- Visual Studio Code or Visual Studio

 Installation
```bash
 Clone the repository
git clone https://github.com/yourusername/quest-guild-terminal.git

 Navigate to project
cd quest-guild-terminal

 Build the project
dotnet build

 Run the application
dotnet run
```

 First Time Setup
```
🏰 Welcome to the Quest Guild Terminal! 🏰
==========================================

1. Register New Hero
2. Login Hero  
3. Exit Guild

Enter your choice: 1
```

 🏗️ Project Structure

```
quest-guild-terminal/
├── 📄 Program.cs                  🎯 Application entry point
├── 📄 QuestGuildApp.cs            🎮 Main application controller
├── 📄 AppConfig.cs                ⚙️ Configuration settings
│
├── 📂 Models/                     🏛️ Data models
│   ├── Hero.cs                    🦸 Hero character data
│   ├── Quest.cs                   📜 Quest information
│   ├── Priority.cs                🎯 Quest priority levels
│   └── Achievement.cs             🏆 Achievement system
│
├── 📂 Services/                   🔧 Service implementations
│   ├── Authenticator.cs           🔐 User authentication
│   ├── NotificationService.cs     📢 Notifications
│   └── GuildAdvisorAI.cs          🤖 AI assistance
│
├── 📂 Managers/                   🎯 Business logic
│   └── QuestManager.cs            📋 Quest management
│
├── 📂 Data/                       🗄️ Data layer
│   └── QuestGuildContext.cs       💾 Database context
│
└── 📂 Utilities/                  🛠️ Helper classes
    └── MenuHelper.cs              🖥️ User interface helpers
```

 🎮 How to Use

 Becoming a Hero 🦸
1. Register your hero with a unique name and password
2. Verify your identity with 2FA code
3. Start your adventure!

 Managing Quests 📜
```
🎯 Hero's Quarters - Welcome, BraveHero!
1. Add New Quest
2. View All Quests  
3. Update/Complete Quest
4. Request Guild Advisor Help
5. Show Guild Report
6. Check Deadline Notifications
7. Logout
```

 AI Assistance 🤖
The Guild Advisor can:
- Generate epic quest descriptions
- Suggest quest priorities 
- Provide heroic summaries of your progress

 🔧 Configuration

 Environment Variables
Create a `.env` file or set environment variables:

```bash
 For AI Features (Optional)
GEMINI_API_KEY=your_gemini_api_key_here

 For Email Notifications (Optional)  
SENDGRID_API_KEY=your_sendgrid_key
SENDGRID_FROM_EMAIL=your@email.com

 For SMS Notifications (Optional)
TWILIO_ACCOUNT_SID=your_account_sid
TWILIO_AUTH_TOKEN=your_auth_token
TWILIO_PHONE_NUMBER=+1234567890
```

 Database Setup
The application uses SQLite - no setup required! A `questguild.db` file will be automatically created.

 📸 Screenshots

(Since I can't include actual images, here are ASCII representations)

 Main Menu
```
┌────────────────────────────────────────┐
│            🏰 QUEST GUILD TERMINAL     │
│                ==================      │
│                                        │
│          1. Register New Hero          │
│          2. Login Hero                 │
│          3. Exit Guild                 │
│                                        │
│          Enter your choice: _          │
└────────────────────────────────────────┘
```

 Hero Dashboard
```
┌────────────────────────────────────────┐
│   🦸 HERO'S QUARTERS - Welcome, Alex!  │
│                ==================      │
│                                        │
│   1. Add New Quest        ⚔️          │
│   2. View All Quests      📜          │
│   3. Update/Complete Quest ✅          │
│   4. Guild Advisor Help   🤖          │
│   5. Show Guild Report    📊          │
│   6. Check Notifications  🔔          │
│   7. Logout               🚪          │
│                                        │
│          Enter your choice: _          │
└────────────────────────────────────────┘
```

 Quest Display
```
┌────────────────────────────────────────┐
│           📜 YOUR QUEST JOURNAL        │
│                ==================      │
│                                        │
│  [1] Defeat the Dragon 🐉             │
│      Due: 2024-01-15 | Priority: High │
│      ⚠️ DEADLINE NEAR!                │
│                                        │
│  [2] Rescue the Princess 👸          │
│      Due: 2024-01-20 | Priority: Medium│
│                                        │
└────────────────────────────────────────┘
```

 🛠️ Development

 Building from Source
```bash
 Clone and build
git clone https://github.com/yourusername/quest-guild-terminal.git
cd quest-guild-terminal
dotnet build

 Run tests
dotnet test

 Create release build
dotnet publish -c Release
```

 Adding New Features
The project follows clean architecture principles:

1. Add new models in `Models/` folder
2. Create services in `Services/` folder  
3. Update main app in `QuestGuildApp.cs`
4. Test thoroughly before committing

 Code Style
- Use PascalCase for class names and methods
- Use camelCase for local variables
- Add XML comments for public methods
- Follow SOLID principles





</div>

---

Quest Guild Terminal - Organizing adventures, one quest at a time! 🏰📜
