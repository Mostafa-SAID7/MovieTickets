using Microsoft.EntityFrameworkCore;
using MovieTickets.Models;

namespace MovieTickets.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }
        public DbSet<MovieImg> MovieImgs { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Keys
            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<MovieCategory>()
                .HasKey(mc => new { mc.MovieId, mc.CategoryId });

            // Relations: MovieActor
            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(ma => ma.ActorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations: MovieCategory
            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieCategories)
                .HasForeignKey(mc => mc.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Category)
                .WithMany(c => c.MovieCategories)
                .HasForeignKey(mc => mc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation: MovieImg
            modelBuilder.Entity<MovieImg>()
                .HasOne(mi => mi.Movie)
                .WithMany(m => m.MovieImgs)
                .HasForeignKey(mi => mi.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation: Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Movie)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =============================
            // 🔹 Seed Data
            // =============================

            modelBuilder.Entity<Cinema>().HasData(
                new Cinema { Id = 1, Name = "Cinema Cairo", Description = "Main cinema in Cairo", Address = "123 Main St" },
                new Cinema { Id = 2, Name = "Alex Cinema", Description = "Cinema in Alexandria", Address = "123 Main St" },
                new Cinema { Id = 3, Name = "Luxor Cinema", Description = "Cinema in Luxor", Address = "123 Main St" }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action" },
                new Category { Id = 2, Name = "Comedy" },
                new Category { Id = 3, Name = "Drama" },
                new Category { Id = 4, Name = "Horror" },
                new Category { Id = 5, Name = "Sci-Fi" }
            );

            // Seed Movies (CinemaId + CategoryId موجودين خلاص فوق 👌)
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Title = "Avengers: Endgame",
                    Description = "Superheroes unite to battle Thanos.",
                    Price = 100,
                    StartDate = new DateTime(2019, 4, 26),
                    EndDate = new DateTime(2025, 12, 31),

                    CategoryId = 1,
                    CinemaId = 1
                },
                new Movie
                {
                    Id = 2,
                    Title = "The Mask",
                    Description = "A man discovers a magical mask.",
                    StartDate = new DateTime(1994, 7, 29),
                    CategoryId = 2,
                    CinemaId = 2
                },
                new Movie
                {
                    Id = 3,
                    Title = "The Shawshank Redemption",
                    Description = "Two imprisoned men bond over a number of years.",
                    StartDate = new DateTime(1994, 9, 23),
                    CategoryId = 3,
                    CinemaId = 3
                },
                new Movie
                {
                    Id = 4,
                    Title = "The Conjuring",
                    Description = "Paranormal investigators help a family.",
                    StartDate = new DateTime(2013, 7, 19),
                    CategoryId = 4,
                    CinemaId = 1
                },
                new Movie
                {
                    Id = 5,
                    Title = "Inception",
                    Description = "A thief enters people's dreams to steal secrets.",
                    StartDate = new DateTime(2010, 7, 16),
                    CategoryId = 5,
                    CinemaId = 2
                }
            );
            // Seed Actors
            modelBuilder.Entity<Actor>().HasData(
                new Actor { Id = 1, FirstName = "Robert", LastName = "Downey Jr." },
                new Actor { Id = 2, FirstName = "Chris", LastName = "Evans" },
                new Actor { Id = 3, FirstName = "Scarlett", LastName = "Johansson" },
                new Actor { Id = 4, FirstName = "Leonardo", LastName = "DiCaprio" },
                new Actor { Id = 5, FirstName = "Tom", LastName = "Hanks" }
            );
    

        }
    }
}
