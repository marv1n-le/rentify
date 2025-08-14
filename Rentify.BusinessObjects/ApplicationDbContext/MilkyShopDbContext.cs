using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;

namespace Rentify.BusinessObjects.ApplicationDbContext;

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
    public DbSet<Category> Categories { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<RentalItem> RentalItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Role>().ToTable("Role");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Feedback>().ToTable("Feedback");
        modelBuilder.Entity<Item>().ToTable("Item");
        modelBuilder.Entity<Post>().ToTable("Post");
        modelBuilder.Entity<Comment>().ToTable("Comment");
        modelBuilder.Entity<Rental>().ToTable("Rental");
        modelBuilder.Entity<RentalItem>().ToTable("RentalItem");

        modelBuilder.Entity<RentalItem>(entity =>
        {
            entity.HasKey(ri => new { ri.RentalId, ri.ItemId });
            entity.HasOne(ri => ri.Rental)
                .WithMany(r => r.RentalItems)
                .HasForeignKey(ri => ri.RentalId);
            entity.HasOne(ri => ri.Item)
                .WithMany(i => i.RentalItems)
                .HasForeignKey(ri => ri.ItemId);
        });
    }

}