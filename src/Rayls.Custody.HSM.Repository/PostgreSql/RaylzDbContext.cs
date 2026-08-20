using Microsoft.EntityFrameworkCore;
using Rayls.Custody.HSM.Service.Interface.Repositories.Model;

namespace Rayls.Custody.HSM.Repository.PostgreSql
{
    public class RaylzDbContext : DbContext
    {
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public RaylzDbContext()
        {                
        }

        public RaylzDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Token>(b => {
                b.HasKey(item => new {item.Id});
            });
            modelBuilder.Entity<Token>().ToTable("tokens");

            modelBuilder.Entity<Wallet>(b => {
                b.HasKey(item => new {item.Id});
            });
            modelBuilder.Entity<Wallet>().ToTable("wallets");

            modelBuilder.Entity<Transaction>(b => {
                b.HasKey(item => new {item.Id});
            });
            modelBuilder.Entity<Transaction>().ToTable("transactions");

            base.OnModelCreating(modelBuilder);
        }
    }
}
