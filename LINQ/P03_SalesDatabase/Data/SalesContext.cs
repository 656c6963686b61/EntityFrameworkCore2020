namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;
    using static DataSettings;
    public class SalesContext : DbContext
    {
        public SalesContext()
        {
            
        }

        public SalesContext(DbContextOptions options)
            :base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DefaultConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(x => x.Name)
                .IsUnicode();

            modelBuilder.Entity<Customer>()
                .Property(x => x.Name)
                .IsUnicode();
            modelBuilder.Entity<Customer>()
                .Property(x => x.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Store>()
                .Property(x => x.Name)
                .IsUnicode();
        }
    }
}
