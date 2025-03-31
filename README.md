# Mine Rush

Mine Rush is a web-based betting game inspired by Stake's Mines game. It is built using **ASP.NET Core MVC 8.0, jQuery, AJAX, and SQL Server**. The game features a login/register system, wallet management, and a dynamic game board with real-time interactions.

## Features

### Authentication & User Management
- Users must **register and log in** using a **cookie-based authentication system**.
- Only logged-in users can access the game, wallet, and profile settings.
- **Logout** functionality clears the authentication cookies.

### Wallet System
- Players can **view their balance** and **deposit/withdraw funds**.
- The last **10 transactions** are displayed for tracking history.

### Game Mechanics
- **Betting & Start Game**
  - Players can start a game only if the **bet amount is within their wallet balance**.
  - Once the game starts, a **session and in-memory cache (_cache)** store game details:
    - **Bomb positions** (based on selected bomb count).
    - **Initial multiplier** (higher bomb count = higher rewards).
- **Gameplay**
  - A **5x5 grid (25 tiles)** is available for selection.
  - Clicking a tile triggers an **AJAX request** to check if it's a bomb or diamond.
  - If a **diamond** is found:
    - The tile updates dynamically.
    - Profit is updated without reloading.
  - If a **bomb** is found:
    - The session and cache are cleared.
    - UI is updated dynamically (buttons disabled, cashout replaced with start button, profit reset).
  - Users can **cash out at any time** before hitting a bomb.
- **Game State Persistence**
  - On page reload, an AJAX request checks for **existing sessions**.
  - If an active game exists, the **UI updates automatically** with the last game state.

### Technologies Used
- **ASP.NET Core MVC 8.0** (Backend & Controllers)
- **jQuery & AJAX** (Frontend interactivity)
- **SQL Server** (Database management)
- **Session & Caching** (Game state optimization)
- **Bootstrap** (UI & Styling)

## Installation & Setup

### Prerequisites
- **.NET 8.0 SDK**
- **SQL Server**
- **Visual Studio 2022 or VS Code**

### Steps to Run
1. **Clone the Repository**
   ```sh
   git clone https://github.com/your-username/mine-rush.git
   cd mine-rush
   ```
2. **Configure Database**
   - Open `appsettings.json` and update the **SQL Server connection string**.
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=MineRushDB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
3. **Apply Migrations & Seed Data**
   ```sh
   dotnet ef database update
   ```
4. **Run the Project**
   ```sh
   dotnet run
   ```
5. **Open in Browser**
   - Navigate to `http://localhost:5000/`

### Images  Start
## 1) Home Page
- Home Page (1)
![image](https://github.com/user-attachments/assets/4f378a11-9523-4779-8cf2-850115b5cbbb)

- Home Page (2)
![image](https://github.com/user-attachments/assets/fc8eb33b-f74c-470c-a14e-29f2b226fcc6)

- Home Page (3)
![image](https://github.com/user-attachments/assets/3936d194-7109-4a59-9170-720a6fe31def)

- Home Page (4)
![image](https://github.com/user-attachments/assets/c1fcd4c2-66d1-4079-bf71-73a6cc6b6935)

## 2) Rules Page
![image](https://github.com/user-attachments/assets/3cfee749-6b32-448a-bf51-6e38a627d8c9)

## 3) About Page
- About Page (1)
![image](https://github.com/user-attachments/assets/09b9d582-e8b8-4c07-9181-263c23414905)

- About Page (2)
![image](https://github.com/user-attachments/assets/4950a2d4-9a19-4b6d-b2b0-71181ca10619)

## 4) Contact Page
![image](https://github.com/user-attachments/assets/6e3c2f89-073b-4b90-b887-714db36d2239)

## 5) Register Page
![image](https://github.com/user-attachments/assets/e5a33c5b-9bdb-47a3-a481-45f5729262fe)

## 6) Register Page
![image](https://github.com/user-attachments/assets/6d4f5a60-951c-49f8-9b71-cd171066da20)

## 7) Home Page (After successful login)
![image](https://github.com/user-attachments/assets/cc1b3221-0d52-4c20-9f56-0b44b8d79ac2)

## 8) Edit Profile Page (Login Required to Access)
![image](https://github.com/user-attachments/assets/171eb5e8-643e-4a6e-a97d-8b952e7dcf49)

## 9) Wallet Page (Login Required to Access)
- Wallet Page (1 - Balance)
![image](https://github.com/user-attachments/assets/6ddb324c-2ce6-475e-9af0-d59d18eae990)

- Wallet Page (2 - Deposit | Withdraw ~ Dummy Wallet)
![image](https://github.com/user-attachments/assets/57981165-dad0-4134-bec5-19833c258096)

- Wallet Page (3 - Transactions Table)
![image](https://github.com/user-attachments/assets/66da4526-c636-404d-b157-0cd2df8e0f94)

## 9) Play Game Page (Login Required to Access)
- Play Game Page (1 - Game Not Started)
![image](https://github.com/user-attachments/assets/5f8973ee-311b-426c-91c9-0b3c8a763de9)

- Play Game Page (2 - Game Started)
![image](https://github.com/user-attachments/assets/c8d1f229-00b4-4f2b-8f11-e1a001c0ea8e)

- Play Game Page (3 - Profit Dynamically Updates after Diamonds Reveal)
![image](https://github.com/user-attachments/assets/84b3ddf8-ec94-4770-982d-d6ffa4df536a)

- Play Game Page (4a Scenario - Player does cashout | +Balance updates as well in Wallet Page)
![image](https://github.com/user-attachments/assets/e4ff9df5-ee8b-4176-a5c8-4b69dd0934c2)

- Play Game Page (4b Scenario - Bomb detected | Everything resets, -Balance updates as well in Wallet Page)
![image](https://github.com/user-attachments/assets/6facbc0f-d5e2-43ee-acaa-191a53190ac5)
![image](https://github.com/user-attachments/assets/155f8e54-62eb-4844-9441-785d46c46cf0)

### Images  End

## Lets Connect
- LinkedIn : https://www.linkedin.com/in/omberde/
- Email : omberde0@gmail.com

## MIT License
This project is licensed under the **MIT License**. Feel free to use, modify, and distribute it.

## Contribution
- Fork the repository.
- Create a new branch (`feature/new-feature`).
- Commit your changes.
- Open a Pull Request.

- ## PS
This was my first project to include Repository/Service layer pattern design and 
I implemented it without any guided video/source just plain impromtu implementations, 
so any improvements/guidance from experienced ones are really welcomed.
