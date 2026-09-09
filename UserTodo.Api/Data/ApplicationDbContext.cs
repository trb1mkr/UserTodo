using Microsoft.EntityFrameworkCore;
using UserTodo.Api.Models;

namespace UserTodo.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasMany(e => e.Todos)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description)
                .HasColumnName("Todo")
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Completed)
                .IsRequired();

            entity.Property(e => e.UserId)
                .IsRequired();
        });
    }
}
