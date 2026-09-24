using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TnaboExtApp.Models;

namespace TnaboExtApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TnaboExtApp.Models.Phase> Phase { get; set; } = default!;
        public DbSet<TnaboExtApp.Models.Street> Street { get; set; } = default!;
        public DbSet<TnaboExtApp.Models.House> House { get; set; } = default!;
        public DbSet<TnaboExtApp.Models.HouseStatus> HouseStatus { get; set; } = default!;
        public DbSet<TnaboExtApp.Models.HouseImage> HouseImage { get; set; } = default!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<MonthlyIncome> MonthlyIncome { get; set; } = null!;
    }
}
