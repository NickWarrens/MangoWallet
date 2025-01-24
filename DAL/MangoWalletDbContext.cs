using Domain.Balances;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class MangoWalletDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<CurrencyBalance> CurrencyBalances { get; set; }

    public MangoWalletDbContext(DbContextOptions<MangoWalletDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Key)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<UserWallet>()
            .HasOne(w => w.User)
            .WithOne(u => u.UserWallet)
            .HasForeignKey<UserWallet>(w => w.UserId)
            .IsRequired();
        
        modelBuilder.Entity<CurrencyBalance>()
            .HasOne(cb => cb.UserWallet)
            .WithMany(w => w.CurrencyBalances)
            .HasForeignKey(cb => cb.UserWalletId)
            .IsRequired();

        modelBuilder.Entity<CurrencyBalance>()
            .Property(cb => cb.Balance)
            .IsRequired();
        
        modelBuilder.Entity<CurrencyBalance>()
            .Property(cb => cb.CurrencyType)
            .IsRequired();
    }

    public bool CreateDatabase(bool drop)
    {
        if (drop)
        {
            Database.EnsureDeleted();
        }
        bool databaseCreated = Database.EnsureCreated();
        return databaseCreated;
    }
    
    public bool IsDatabaseEmpty()
    {
        return !Users.Any() && !CurrencyBalances.Any() && !UserWallets.Any();
    }
}