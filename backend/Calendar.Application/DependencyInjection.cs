using Calendar.Application.CQRS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Calendar.Application;

public static class DependencyInjection
{
    extension(IServiceCollection  services)
    {
        public IServiceCollection AddApplication()
        {
            services
                .AddComponent();
        
            return services;
        }

        private IServiceCollection AddComponent()
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
            var assembly = typeof(DependencyInjection).Assembly;

            services.Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes
                    .AssignableToAny(
                        typeof(ICommandHandler<,>),
                        typeof(ICommandHandler<>),
                        typeof(IQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
        
            return services;
        }
    }
}