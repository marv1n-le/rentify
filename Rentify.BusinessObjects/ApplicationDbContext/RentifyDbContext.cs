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

        #region Seed User

        modelBuilder.Entity<Role>(options =>
        {
            options.HasData(
                new Role
                {
                    Id = "b8d237b8b6f849988d60c6c3c1d0a943",
                    Name = "User"
                },
                new Role
                {
                    Id = "2e7b5a97e42e4e84a08ffbe0bc05d2ea",
                    Name = "Admin"
                }
            );
        });

        modelBuilder.Entity<User>(options =>
        {
            options.HasData(
                new User
                {
                    Id = "f49aa51bbd304e77933e24bbed65b165",
                    Email = "user@gmail.com",
                    Password = "123",
                    FullName = "Người dùng 1",
                    RoleId = "b8d237b8b6f849988d60c6c3c1d0a943",
                },
                new User
                {
                    Id = "1a3bcd12345678901234567890123456",
                    Email = "admin@gmail.com",
                    Password = "123",
                    FullName = "Admin",
                    RoleId = "2e7b5a97e42e4e84a08ffbe0bc05d2ea"
                },
                new User
                {
                    Id = "29d72211a9f7480c9812d61ee17c92b9",
                    Email = "user2@gmail.com",
                    Password = "123",
                    FullName = "Người dùng 2",
                    RoleId = "b8d237b8b6f849988d60c6c3c1d0a943"
                }
            );
        });
        #endregion

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