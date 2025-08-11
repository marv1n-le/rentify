using Microsoft.EntityFrameworkCore;
using MilkyShop.BusinessObjects.Entities;

namespace MilkyShop.BusinessObjects.ApplicationDbContext;

public class MilkyShopDbContext : DbContext
{
    public MilkyShopDbContext()
    {
        
    }
    
    public MilkyShopDbContext(DbContextOptions<MilkyShopDbContext> options) : base(options)
    {
        
    }
    
    //DbSet
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<DiscountBatch> DiscountBatches { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Role>().ToTable("Role");
        modelBuilder.Entity<Address>().ToTable("Address");
        modelBuilder.Entity<Brand>().ToTable("Brand");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Discount>().ToTable("Discount");
        modelBuilder.Entity<Feedback>().ToTable("Feedback");
        modelBuilder.Entity<Order>().ToTable("Order");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<DiscountBatch>().ToTable("DiscountBatch");
        modelBuilder.Entity<Payment>().ToTable("Payment");
    }
        
}