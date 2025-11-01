using BankDL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace BankDL.DataAccess
{
	public class BankDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public DbSet<AccountEntity> Accounts { get; set; }

        public async Task BeginTransactionAsync()
        {
            await Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await Database.RollbackTransactionAsync();
        }

        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<UserEntity>(e =>
            {
                e.HasKey(u => u.Id);

                e.HasOne(u => u.Account)
                    .WithOne(acc => acc.User)
                    .HasForeignKey<AccountEntity>(acc => acc.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<AccountEntity>(e =>
            {
                e.HasKey(acc => acc.Id);

                e.HasIndex(acc => acc.UserId)
                    .IsUnique();

                e.Property(p => p.AccountNumber).HasValueGenerator<OrderIdValueGenerator>();
            });

        }
    }
}

public class OrderIdValueGenerator : ValueGenerator<int>
{
    private int _current = 10000;

    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry)
        => Interlocked.Increment(ref _current);
}
