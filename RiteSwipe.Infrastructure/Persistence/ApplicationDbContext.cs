using Microsoft.EntityFrameworkCore;
using RiteSwipe.Domain.Entities;
using RiteSwipe.Application.Common.Interfaces;

namespace RiteSwipe.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<UserSkill> UserSkills { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<Swipe> Swipes { get; set; } = null!;
    public DbSet<TaskApplication> TaskApplications { get; set; } = null!;
    public DbSet<EscrowPayment> EscrowPayments { get; set; } = null!;
    public DbSet<TaskReview> TaskReviews { get; set; } = null!;
    public DbSet<TaskDispute> TaskDisputes { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserSkills composite key
        modelBuilder.Entity<UserSkill>()
            .HasKey(us => new { us.UserId, us.SkillId });

        // Configure User relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserSkills)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.PostedTasks)
            .WithOne(t => t.PostedByUser)
            .HasForeignKey(t => t.PostedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Task relationships
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.RequiredSkill)
            .WithMany(s => s.Tasks)
            .HasForeignKey(t => t.SkillRequiredId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.EscrowPayment)
            .WithOne(e => e.Task)
            .HasForeignKey<EscrowPayment>(e => e.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure TaskReview relationships
        modelBuilder.Entity<TaskReview>()
            .HasOne(tr => tr.ReviewerUser)
            .WithMany(u => u.ReviewsGiven)
            .HasForeignKey(tr => tr.ReviewerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskReview>()
            .HasOne(tr => tr.ReviewedUser)
            .WithMany(u => u.ReviewsReceived)
            .HasForeignKey(tr => tr.ReviewedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Skill>()
            .HasIndex(s => s.SkillName)
            .IsUnique();

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => t.Status);

        modelBuilder.Entity<Swipe>()
            .HasIndex(s => new { s.UserId, s.TaskId })
            .IsUnique();

        modelBuilder.Entity<TaskApplication>()
            .HasIndex(ta => new { ta.TaskId, ta.WorkerId })
            .IsUnique();

        // Configure decimal precisions
        modelBuilder.Entity<TaskItem>()
            .Property(t => t.MinPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TaskItem>()
            .Property(t => t.MaxPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TaskItem>()
            .Property(t => t.CurrentRate)
            .HasPrecision(18, 2);

        modelBuilder.Entity<EscrowPayment>()
            .Property(e => e.AmountHeld)
            .HasPrecision(18, 2);
    }
}
