# 💘 Mini Dating App

A Web-App simulator a mobile-first dating application built with **React + Vite** (Frontend) and **.NET 10** (Backend), featuring real-time match notifications via SignalR, availability-based date scheduling, and a clean phone-frame UI.

**Live Demo:**
- Frontend: [https://minidating-7kvxdtupp-unique-idea1s-projects.vercel.app?_vercel_share=aKTzLZjoCE9dEy46fuMn0qoQkwucrm75](https://minidating-7kvxdtupp-unique-idea1s-projects.vercel.app?_vercel_share=aKTzLZjoCE9dEy46fuMn0qoQkwucrm75)
- Backend API: [https://mini-dating-app-axvl.onrender.com/swagger/index.html](https://mini-dating-app-axvl.onrender.com/swagger/index.html)
- Video Demo (Local + Happy Case): 
---

## 📁 System Organization

```
Mini_Dating_App/
├── Mini_Dating_App_BE/        # .NET 10 Web API
│   ├── Controllers/           # API endpoints
│   ├── Services/              # Business logic
│   │   ├── Implements/        # Concrete service classes
│   │   └── Interfaces/        # Service contracts
│   ├── Repositories/          # Data access layer (Unit of Work pattern)
│   ├── Data/
│   │   ├── Models/            # EF Core entities
│   │   └── MiniDatingAppDbContext.cs
│   ├── Hubs/                  # SignalR hub
│   ├── Middleware/            # Global exception handler
│   └── Dockerfile
│
└── Mini_Dating_App_FE/        # React + Vite
    ├── src/
    │   ├── pages/             # MainPage, ProfilesPage, MatchesPage
    │   ├── components/        # PhoneLayout, Navbar, TopBar, AvailabilityOverlay
    │   ├── api/               # Axios client with JWT interceptor
    │   └── services/          # SignalR service
    ├── nginx.conf             # Nginx config for Docker
    └── Dockerfile
```

### Architecture Pattern

- **Frontend:** Component-based React with page-level state management
- **Backend:** Clean architecture with Repository + Unit of Work pattern
- **Real-time:** SignalR with user-based group routing
- **Auth:** JWT Bearer tokens stored in localStorage

---

## 💾 Data Storage

| Layer | Technology | Purpose |
|-------|-----------|---------|
| Database | SQLite (via Entity Framework Core) | Persistent storage for users, likes, matches, availability, scheduled date |
| Frontend session | localStorage | JWT token + current user info |
| Real-time state | React useState | Match list, UI state, notifications |

### Database Schema

```
User          → has many UserLike (as liker and liked)
User          → has many Availability
UserLike      → triggers Match creation when mutual
Match         → has one ScheduledDate (when slot found)
Match         → belongs to two Users (UserA, UserB)
```

### Seeded Data

On first launch with an empty database, the system automatically seeds 8 demo users (Alice, Bob, Emma, James, Sophie, Liam, Mia, Noah) each with 10–15 random availability slots spread across the next 21 days, designed to produce overlapping time slots for demo purposes.

---

## 💕 Match Logic

### How Matching Works

1. User A browses profiles and clicks ❤️ **Like** on User B
2. System records a `UserLike` entry (`LikerId = A`, `LikedId = B`)
3. System checks if a **mutual like** exists (B already liked A)
4. If mutual → a `Match` is created with status `Matched`
5. Both users are **instantly notified** via SignalR

### Match Status Flow

```
Matched → Pending → Scheduled
                 ↘ NoSlotFound → (update availability) → Scheduled
```

| Status | Meaning |
|--------|---------|
| `Matched` | Both users liked each other, neither has requested a date yet |
| `Pending` | One user clicked "Let's Date", waiting for the other to confirm |
| `NoSlotFound` | Both confirmed but no overlapping free slot was found |
| `Scheduled` | Common slot found, date is confirmed |

### Scheduling Request

- Either user can click **"Let's Date"** to confirm intent
- When both users have confirmed → system runs the slot-finding algorithm
- `UserConfirmed` flag tracks whether the current user has already confirmed
- UI adapts: "Waiting for partner..." vs "It's your turn!"

---

## 🕐 Overlapping Slot Algorithm

The algorithm finds the earliest common free time slot between two users, respecting existing scheduled dates.

### Step-by-step Logic

```
Input: curAva (current user's availability), othAva (other user's availability)

1. Pre-load existing scheduled matches for BOTH users (to check conflicts)
2. Build a dictionary: othAvaByDate = { "2026-02-25": Availability, ... }
3. For each day in curAva:
   a. Check if othAva has the same day → skip if not
   b. Check if current user already has a scheduled match that day AND time overlaps → skip
   c. Check if other user already has a scheduled match that day AND time overlaps → skip
   d. Calculate overlap:
      potentialStart = MAX(curStart, othStart)
      potentialEnd   = MIN(curEnd,   othEnd)
   e. If overlap < 30 minutes → skip
   f. Otherwise → assign ScheduledDate and return true
4. If no slot found → return false → status becomes NoSlotFound
```

### Conflict Detection

```csharp
var hasConflict =
    curBookedMatches.Any(x =>
        x.ScheduledDate.Date == date &&
        x.ScheduledDate.StartTime < potentialEndTime &&
        x.ScheduledDate.EndTime   > potentialStartTime)
    ||
    othBookedMatches.Any(x =>
        x.ScheduledDate.Date == date &&
        x.ScheduledDate.StartTime < potentialEndTime &&
        x.ScheduledDate.EndTime   > potentialStartTime);
```

This uses the standard **interval overlap formula**: `A.start < B.end && B.start < A.end`.

### Complexity

- Time: **O(C × M)** where C = availability days (max 21), M = existing scheduled matches
- In practice nearly O(1) since both values are small and bounded
- Optimized: `othAvaByDate` dictionary makes per-day lookup O(1)

### Date Handling

- Availability starts from **tomorrow** (never today) to avoid edge cases near midnight
- All dates use **local date construction** (not `toISOString()`) to prevent UTC timezone shifting

---

## 🔔 Real-time Notifications (SignalR)

- Each user joins their own SignalR group on login: `JoinUserGroup(userId)`
- Backend notifies relevant users on every match event:
  - `matchCreated` → both users when a mutual like creates a match
  - `matchUpdated` → both users when match status changes (Pending, Scheduled, NoSlotFound)
- Frontend listens in `MatchesPage` and updates state without full page refresh
- A **pulsing pink dot** appears on the Matches navbar icon when an update arrives while the user is on another page

---

## 🚀 Deployment

| Service | Platform | URL |
|---------|---------|-----|
| Frontend | Vercel | Auto-deploys on push to `main` |
| Backend | Render (Docker) | Auto-deploys on push to `main` |
| Database | SQLite (ephemeral on Render) | Re-seeded on each redeploy |

---

## 🔧 If I Had More Time

- **Profile photos** — currently using emoji avatars; real image upload with cloud storage (Cloudinary/S3) would make profiles feel much more real
- **Push notifications** — native browser push notifications so users get alerted even when the app is closed, not just via SignalR while the tab is open
- **Persistent database** — migrate from SQLite to PostgreSQL on Render to prevent data loss on redeploy
- **Pagination** on profile browsing — currently loads all profiles at once; lazy loading would be needed at scale
- **Filter character** - Allows users to filter various characteristics of other people's profiles that they want to view.
---

## 💡 Proposed New Features

### 1. 📍 Location-based Matching
Allow users to set their location and filter matches by distance. When a date is scheduled, the system could suggest nearby cafes or restaurants as a meeting point. This makes the scheduling step more complete and reduces the friction of "where should we meet?".

### 2. 💬 In-app Chat for Matched Users
Once two users match, open a simple real-time chat channel between them (using the existing SignalR infrastructure). Currently matched users have no way to communicate before their date — a chat would let them confirm plans, reschedule, or just get to know each other better.

### 3. ⭐ Post-date Rating & Feedback
After a scheduled date has passed, prompt both users to rate the experience. This creates a feedback loop that helps surface better matches, and could power a recommendation engine over time that learns which types of profiles a user tends to enjoy meeting.

---

## 🛠 Tech Stack

**Frontend**
- React 18 + Vite
- Bootstrap (utility classes)
- Axios (HTTP client)
- @microsoft/signalr (real-time)
- React Router v6

**Backend**
- .NET 10 Web API
- Entity Framework Core + SQLite
- AutoMapper
- SignalR
- JWT Authentication

**DevOps**
- Docker + docker-compose (local)
- Vercel (FE hosting)
- Render (BE hosting)
- GitHub (CI/CD trigger)
