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
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -----------------------
            // Composite keys
            // -----------------------
            modelBuilder.Entity<MovieActor>().HasKey(ma => new { ma.MovieId, ma.ActorId });
            modelBuilder.Entity<MovieCategory>().HasKey(mc => new { mc.MovieId, mc.CategoryId });

            // -----------------------
            // Movie <-> MovieActor (M:N)
            // -----------------------
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

            // -----------------------
            // Movie <-> MovieCategory (M:N)
            // -----------------------
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

            // -----------------------
            // MovieImgs -> Movie (1-M)
            // -----------------------
            modelBuilder.Entity<MovieImg>()
                .HasOne(mi => mi.Movie)
                .WithMany(m => m.MovieImgs)
                .HasForeignKey(mi => mi.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Showtimes -> Movie (1-M)
            // -----------------------
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes) // ensure Movie has ICollection<Showtime> Showtimes
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Hall <-> Showtime (1-M)
            // -----------------------
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Hall)
                .WithMany(h => h.Showtimes)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Hall -> Seats (1-M)
            // -----------------------
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Hall)
                .WithMany(h => h.Seats)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Restrict);

            // enforce unique seat number per hall (e.g., HallId + SeatNumber)
            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.HallId, s.SeatNumber })
                .IsUnique();

            // -----------------------
            // Booking relations
            // - Booking -> Movie (optional if you use Showtime instead)
            // - Booking -> User
            // - Booking -> Showtime (if you added ShowtimeId to Booking model)
            // -----------------------
            // Booking -> Movie (if Booking has MovieId)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Movie)
                .WithMany(m => m.Bookings) // ensure Movie has ICollection<Booking> Bookings if you use this
                .HasForeignKey(b => b.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> User
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings) // ensure User has ICollection<Booking> Bookings
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // If you changed Booking to reference a Showtime, you can enable:
            modelBuilder.Entity<Booking>()
       .HasOne(b => b.Showtime).WithMany(s => s.Bookings).HasForeignKey(b => b.ShowtimeId)
       .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Tickets -> Booking (1-M)
            // -----------------------
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade); // when a booking removed, delete tickets

            // Ticket -> Seat (optional): if you link ticket to a seat
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany() // Seat may have Booking reference or not; adjust as necessary
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // -----------------------
            // Payment <-> Booking (1-1)
            // If Booking has Payment navigation: Booking.Payment
            // -----------------------
            // If you want one-to-one:
            modelBuilder.Entity<Payment>()
       .HasOne(p => p.Booking).WithMany() // or .WithOne(b => b.Payment) if 1:1
       .HasForeignKey(p => p.BookingId)
       .OnDelete(DeleteBehavior.Restrict);

            // If you prefer one-to-many (multiple payments per booking), use WithMany and change model accordingly.

            // -----------------------
            // Precision & column types
            // -----------------------
            modelBuilder.Entity<Movie>()
                .Property(m => m.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Showtime>()
                .Property(s => s.TicketPrice)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Offer>()
                .Property(o => o.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            // -----------------------
            // Useful Indexes
            // -----------------------
            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.Title);

            modelBuilder.Entity<Showtime>()
                .HasIndex(s => s.ShowDateTime);

            modelBuilder.Entity<Offer>()
                .HasIndex(o => new { o.IsActive, o.ExpiryDate });

            modelBuilder.Entity<Promotion>()
                .HasIndex(p => new { p.IsActive, p.ExpiryDate });

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.Date);

            // -----------------------
            // Default values / value generation (optional)
            // -----------------------
            // Example: CreatedAt on Blog / Job
            modelBuilder.Entity<Blog>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Job>()
                .Property(j => j.PostedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();
            // precision / indexes etc...
            modelBuilder.Entity<Movie>().Property(m => m.Price).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Ticket>().Property(t => t.Price).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Booking>().Property(b => b.TotalPrice).HasColumnType("decimal(10,2)");
            // -----------------------
            // Concurrency tokens (if any model uses [Timestamp] RowVersion)
            // -----------------------
            // If your Movie model has RowVersion byte[] property:
            // modelBuilder.Entity<Movie>().Property(m => m.RowVersion).IsRowVersion();

            // -----------------------
            // Seed data (optional)
            // -----------------------
            // Keep seed data small and idempotent. You can add seed here if necessary.
        }
    }
}
