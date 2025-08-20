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
    public DbSet<Otp> Otps { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }

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
        modelBuilder.Entity<Otp>().ToTable("Otp");
        modelBuilder.Entity<ChatRoom>().ToTable("ChatRoom");
        modelBuilder.Entity<ChatMessage>().ToTable("ChatMessage");
        modelBuilder.Entity<ChatParticipant>().ToTable("ChatParticipant");

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
            builder.HasOne(x => x.User)
                .WithMany(u => u.Inquiries)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Rental)
                .WithOne(x => x.Inquiry)
                .HasForeignKey<Rental>(x => x.InquiryId)
                .OnDelete(DeleteBehavior.NoAction);
        });

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
                    Email = "haitu@mtp.com",
                    Password = "123",
                    FullName = "User Hai Tu",
                    ProfilePicture = "https://res.cloudinary.com/di9xfkskd/image/upload/v1755622804/rentify_photos/rentify_2e0cdddd-946d-457b-9da7-cd7bf55e2eb1.jpg",
                    RoleId = "b8d237b8b6f849988d60c6c3c1d0a943",
                    IsVerify = true
                },
                new User
                {
                    Id = "1a3bcd12345678901234567890123456",
                    Email = "sontung@mtp.com",
                    Password = "123",
                    FullName = "Admin Son Tung",
                    ProfilePicture = "https://res.cloudinary.com/di9xfkskd/image/upload/v1755622706/rentify_photos/rentify_72ae6872-62f8-4836-a1d3-5998005ec4b6.jpg",
                    RoleId = "2e7b5a97e42e4e84a08ffbe0bc05d2ea",
                    IsVerify = true
                }
            );
        });

        #endregion
    }
}