using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;

namespace Rentify.BusinessObjects.ApplicationDbContext;

public class RentifyDbContext : DbContext
{
    public RentifyDbContext()
    {

    }

    public RentifyDbContext(DbContextOptions<RentifyDbContext> options) : base(options)
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
    public DbSet<Inquiry> Inquiries { get; set; }

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
        modelBuilder.Entity<Inquiry>().ToTable("Inquiry");

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

        modelBuilder.Entity<Item>(options =>
        {
            options.HasOne(i => i.Post)
                .WithOne(i => i.Item)
                .HasForeignKey<Post>(p => p.ItemId);
        });

        modelBuilder.Entity<Inquiry>(builder =>
        {
            builder.HasOne(x => x.Post)
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(u => u.Inquiries)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Rental)
                .WithMany() // không khai báo collection trong Rental
                .HasForeignKey(x => x.RentalId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}