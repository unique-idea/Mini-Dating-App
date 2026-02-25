using Microsoft.EntityFrameworkCore;
using Mini_Dating_App_BE.Data.Models;

namespace Mini_Dating_App_BE.Data
{
    public class MiniDatingAppDbContext : DbContext
    {
        public MiniDatingAppDbContext(DbContextOptions<MiniDatingAppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Availability> Availabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.UserId);

                modelBuilder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

                modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .UseCollation("NOCASE");

                entity.HasMany(e => e.LikesGiven)
                    .WithOne(l => l.Liker)
                    .HasForeignKey(l => l.LikerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.LikesReceived).WithOne(l => l.Liked)
                    .HasForeignKey(l => l.LikedId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Availabilities)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserLike>(entity =>
            {
                entity.ToTable("UserLike");
                entity.HasKey(e => e.UserLikeId);

                entity.HasOne(e => e.Liker)
                    .WithMany(u => u.LikesGiven)
                    .HasForeignKey(e => e.LikerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Liked)
                    .WithMany(u => u.LikesReceived)
                    .HasForeignKey(e => e.LikedId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Availability>(entity =>
            {
                entity.ToTable("Availability");

                entity.HasKey(a => a.AvailabilityId);

                entity.HasOne(a => a.User)
                    .WithMany(u => u.Availabilities)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.ToTable("Match");

                entity.HasKey(m => m.MatchId);

                entity.Property(e => e.Status)
                      .HasConversion<string>();

                entity.HasOne(m => m.UserA)
                      .WithMany()
                      .HasForeignKey(m => m.UserAId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.UserB)
                      .WithMany()
                      .HasForeignKey(m => m.UserBId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.ScheduledDate)
                      .WithOne(sc => sc.Match)
                      .HasForeignKey<ScheduledDate>(sc => sc.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScheduledDate>(entity =>
            {
                entity.ToTable("ScheduledDate");

                entity.HasKey(sc => sc.MatchId);

            });
        }
    }
}
