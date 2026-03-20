## VGLog

EN

A personal video game library tracker built with Blazor Server and ASP.NET Core. Log your games, track your hours, rate your experiences.

---

## 📸 Screenshots

> <img width="1909" height="919" alt="VGLogDashboard" src="https://github.com/user-attachments/assets/c5fffc18-c4f2-48fb-8d13-ac18ac0947b3" />

> <img width="1914" height="912" alt="immagine" src="https://github.com/user-attachments/assets/eff5b0b9-73a6-406b-a06b-925058625904" />

---

## Features

- Dashboard — personalized overview for logged-in users, public view for guests
- Game Library — browse, add, edit and delete games from your personal collection
- Game Status — track every game as *Playing*, *Completed*, *To Play* or *Dropped*
- Statistics — completion rate, average personal rating, most played game, recently completed
- Profile — personal user page with display name and account info
- Authentication — secure register/login with Remember Me support

---

## 🛠️ Tech Stack

 - Blazor Server - Frontend + UI logic 
 - ASP.NET Core - Backend & routing 
 - Entity Framework Core - ORM & database migrations 
 - ASP.NET Identity - Authentication & user management 
 - Radzen - UI component library 
 - SQLite - Database 

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)

### Installation

```bash
# Clone the repository
git clone https://github.com/yourusername/vglog.git
cd vglog

# Run the app
dotnet run
```

The app will be available at `https://localhost:5001`

---

## 🧠 What I Learned

- Implementing the **Service Layer pattern** to separate business logic from UI components
- Using **ASP.NET Identity** for authentication with custom user claims
- Configuring **EF Core relationships** (One-to-Many, Many-to-Many) via Fluent API
- Combining **Blazor Server** with traditional MVC controllers for auth flows
- Managing **database migrations** and understanding when EF Core detects model changes
- Using **DTOs** to expose only the necessary data to the frontend
- Applying **SOLID principles** — especially dependency on interfaces over concrete classes
- Protecting the application with **rate limiting**, **CSRF tokens**, 
  **secure cookie policies** and a general approach to securing the code

