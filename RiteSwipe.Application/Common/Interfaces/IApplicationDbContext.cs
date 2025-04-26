using Microsoft.EntityFrameworkCore;
using RiteSwipe.Domain.Entities;

namespace RiteSwipe.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Skill> Skills { get; }
    DbSet<UserSkill> UserSkills { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<Swipe> Swipes { get; }
    DbSet<TaskApplication> TaskApplications { get; }
    DbSet<EscrowPayment> EscrowPayments { get; }
    DbSet<TaskReview> TaskReviews { get; }
    DbSet<TaskDispute> TaskDisputes { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
