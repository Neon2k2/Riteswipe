using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiteSwipe.Application.Common.Interfaces;
using RiteSwipe.Application.Services;
using RiteSwipe.Infrastructure.Persistence;
using RiteSwipe.Infrastructure.Services;

namespace RiteSwipe.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
            ));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Register application services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEscrowService, EscrowService>();
        services.AddScoped<ISkillService, SkillService>();

        // Register identity and authentication services
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<JwtService>();

        return services;
    }
}
