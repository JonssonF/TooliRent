using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TooliRent.Domain.Entities;
using InfraAppUser = TooliRent.Infrastructure.Identity.AppUser;

namespace TooliRent.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<InfraAppUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Tool> Tools => Set<Tool>();
        public DbSet<ToolCategory> Categories => Set<ToolCategory>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingItem> BookingItems => Set<BookingItem>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<LoanItem> LoanItems => Set<LoanItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<BookingItem>().HasKey(x => new { x.BookingId, x.ToolId });
            b.Entity<LoanItem>().HasKey(x => new { x.LoanId, x.ToolId });

            b.Entity<Tool>()
                .Property(t => t.PricePerDay)
                .HasPrecision(18, 2);

            b.Entity<Tool>()
                .Property(t => t.PricePerHour)
                .HasPrecision(18, 2);

            b.Entity<Tool>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
                

            b.Entity<Booking>()
                .HasMany(x => x.Items)
                .WithOne(i => i.Booking!)
                .HasForeignKey(i => i.BookingId);

            b.Entity<Loan>()
                .HasMany(x => x.Items)
                .WithOne(i => i.Loan!)
                .HasForeignKey(i => i.LoanId);
        }
    }
}
