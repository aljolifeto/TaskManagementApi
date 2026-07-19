using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Models;

namespace TaskManagementApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasKey(category => category.Id);

        modelBuilder.Entity<Category>()
            .Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<TaskItem>()
            .HasKey(task => task.Id);

        modelBuilder.Entity<TaskItem>()
            .Property(task => task.Title)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<TaskItem>()
            .HasOne(task => task.Category)
            .WithMany(category => category.Tasks)
            .HasForeignKey(task => task.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}