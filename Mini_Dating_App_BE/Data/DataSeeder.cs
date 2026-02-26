using Mini_Dating_App_BE.Data.Models;

namespace Mini_Dating_App_BE.Data
{
    public static class DataSeeder
    {
        public static void Seed(MiniDatingAppDbContext context)
        {
            if (context.Users.Any()) return;

            var random = new Random(42);

            var users = new List<User>
        {
            new User { UserId = Guid.NewGuid(), Name = "Alice",  Age = 23, Gender = "Female", Bio = "Love hiking and coffee ☕",          Email = "alice@example.com"  },
            new User { UserId = Guid.NewGuid(), Name = "Bob",    Age = 25, Gender = "Male",   Bio = "Guitarist and foodie 🎸",            Email = "bob@example.com"    },
            new User { UserId = Guid.NewGuid(), Name = "Emma",   Age = 22, Gender = "Female", Bio = "Bookworm and traveler 📚",           Email = "emma@example.com"   },
            new User { UserId = Guid.NewGuid(), Name = "James",  Age = 27, Gender = "Male",   Bio = "Fitness enthusiast and chef 🏋️",    Email = "james@example.com"  },
            new User { UserId = Guid.NewGuid(), Name = "Sophie", Age = 24, Gender = "Female", Bio = "Artist and cat lover 🎨",            Email = "sophie@example.com" },
            new User { UserId = Guid.NewGuid(), Name = "Liam",   Age = 26, Gender = "Male",   Bio = "Gamer and movie buff 🎮",            Email = "liam@example.com"   },
            new User { UserId = Guid.NewGuid(), Name = "Mia",    Age = 21, Gender = "Female", Bio = "Dancer and music lover 💃",          Email = "mia@example.com"    },
            new User { UserId = Guid.NewGuid(), Name = "Noah",   Age = 28, Gender = "Male",   Bio = "Engineer who loves the outdoors 🏕️", Email = "noah@example.com"   },
        };

            context.Users.AddRange(users);
            context.SaveChanges();

            var tomorrow = DateTime.Today.AddDays(1);
            var availabilities = new List<Availability>();

            var timeSlots = new List<(TimeSpan start, TimeSpan end)>
        {
            (new TimeSpan(8, 0, 0),  new TimeSpan(11, 0, 0)),  
            (new TimeSpan(9, 0, 0),  new TimeSpan(12, 0, 0)), 
            (new TimeSpan(10, 0, 0), new TimeSpan(13, 0, 0)),  
            (new TimeSpan(13, 0, 0), new TimeSpan(16, 0, 0)), 
            (new TimeSpan(14, 0, 0), new TimeSpan(17, 0, 0)), 
            (new TimeSpan(17, 0, 0), new TimeSpan(20, 0, 0)), 
            (new TimeSpan(18, 0, 0), new TimeSpan(21, 0, 0)),
            (new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0)),  
            (new TimeSpan(14, 0, 0), new TimeSpan(19, 0, 0)), 
        };

            foreach (var user in users)
            {
                
                var numDays = random.Next(10, 16);
                var selectedDays = Enumerable.Range(0, 21)
                    .OrderBy(_ => random.Next())
                    .Take(numDays)
                    .OrderBy(d => d)
                    .ToList();

                foreach (var dayOffset in selectedDays)
                {
                    var date = tomorrow.AddDays(dayOffset);
                    var slot = timeSlots[random.Next(timeSlots.Count)];

                    availabilities.Add(new Availability
                    {
                        AvailabilityId = Guid.NewGuid(),
                        UserId = user.UserId,
                        Date = date,
                        StartTime = slot.start,
                        EndTime = slot.end
                    });
                }
            }

            context.Availabilities.AddRange(availabilities);
            context.SaveChanges();
        }
    }
}
