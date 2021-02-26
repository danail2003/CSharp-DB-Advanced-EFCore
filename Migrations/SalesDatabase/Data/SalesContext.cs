namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data.Models;

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
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Product>()
                .HasKey(x => x.ProductId);

            modelBuilder
                .Entity<Product>()
                .Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Product>()
                .Property(x => x.Quantity)
                .IsRequired(true);

            modelBuilder
                .Entity<Product>()
                .Property(x => x.Price)
                .IsRequired();

            modelBuilder
                .Entity<Product>()
                .Property(x => x.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No description");

            modelBuilder
                .Entity<Product>()
                .HasMany(x => x.Sales)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);

            modelBuilder
                .Entity<Customer>()
                .HasKey(x => x.CustomerId);

            modelBuilder
                .Entity<Customer>()
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Customer>()
                .Property(x => x.Email)
                .HasMaxLength(80)
                .IsRequired(true);

            modelBuilder
                .Entity<Customer>()
                .Property(x => x.CreditCardNumber)
                .IsRequired();

            modelBuilder
                .Entity<Customer>()
                .HasMany(x => x.Sales)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder
                .Entity<Store>()
                .HasKey(x => x.StoreId);

            modelBuilder
                .Entity<Store>()
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode();

            modelBuilder
                .Entity<Store>()
                .HasMany(x => x.Sales)
                .WithOne(x => x.Store)
                .HasForeignKey(x => x.StoreId);

            modelBuilder
                .Entity<Sale>()
                .HasKey(x => x.SaleId);

            modelBuilder
                .Entity<Sale>()
                .Property(x => x.Date)
                .IsRequired(true)
                .HasColumnType("DATETIME2")
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
