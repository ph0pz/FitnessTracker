# 🏋️‍♂️ Fitness Tracker Web App

A full-stack fitness tracking web application that helps users monitor their meals, macros, and body stats — with AI-powered macro estimation using GPT.

This project demonstrates full-stack skills using Angular 19, ASP.NET Core Web API, and OpenAI GPT integration.

---

## ✅ Completed Features (For Presentation)

### 1. 🔐 User Authentication
- User registration and login
- JWT-based token authentication
- Profile page to set and edit macro goals (calories, protein, carbs, fat)

---

### 2. 📊 Dashboard Overview
- Daily macro summary: **consumed vs target**
- Macro progress chart with **visual trend tracking**

---

### 3. 🍱 Meal Tracker
- Add meals manually with macro values (calories, protein, carbs, fat)
- View meal history by **selected date**
- **Auto-calculate calories** from protein/carbs/fat
- Save **frequent meals** for faster entry

---

### 4. 🧠 GPT Macro Estimation
- Input example: _“I ate chicken curry and rice”_
- Uses **OpenAI GPT API** to return estimated:
  - Calories
  - Protein
  - Carbs
  - Fat
- Auto-fills meal form with GPT-suggested macros

---

### 5. ⚖️ Progress Tracking
- Record bodyweight **daily**
- View **weight progress chart** using Chart.js
- Support for tracking **body fat %**

---

## 🛠 Tech Stack

| Layer       | Technology                               |
|-------------|-------------------------------------------|
| Frontend    | Angular 19, Angular Material              |
| Backend     | ASP.NET Core Web API (.NET 8)             |
| Database    | SQL Server                                |
| Charts      | Chart.js                                  |
| Auth        | JWT (ASP.NET Identity)                    |
| AI API      | OpenAI GPT API                            |

---

## 🚀 Setup Instructions

### Frontend
```bash
cd FitnessTrackerApp
npm install
ng serve
```

### Backend
```bash
cd FitnessTracker.API
dotnet restore
dotnet ef database update
dotnet run
```

---



## 👨‍💻 Author

Developed by **Ponpithak Rodkean**  
Built for demo and interview to showcase real-world full-stack capabilities.

---
