using Pelican.Mediator.Internals;
using Pelican.Mediator;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Registers Mediator into service container.
    /// - Scans assemblies to find all request handlers.
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        /// Registers handlers and mediator types from the specified assemblies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Collection of assemblies for scanning.</param>
        public static void AddPelican(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped<IMediator, Mediator>();
            services.AddScoped<IPipelineManagement, PipelineManagement>();
            services.AddScoped<IHandlerFactory, HandlerFactory>();

            services.RegisterHandlers(assemblies);
        }

        private static void RegisterHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            var registration = (Type genericInterfaceType) =>
            {
                var handlerTypes = assemblies.SelectMany(assemby => assemby.GetTypes()
                    .Where(x => !x.IsAbstract)
                    .Select(x => new
                    {
                        ImplementationType = x,
                        ImplementedInterfaceType = x.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType)
                    })
                    .Where(x => x.ImplementedInterfaceType != null));

                foreach (var handlerType in handlerTypes)
                    services.AddScoped(handlerType.ImplementedInterfaceType, handlerType.ImplementationType);
            };

            registration(typeof(IRequestHandler<>));
            registration(typeof(IRequestHandler<,>));
        }
    }
}

