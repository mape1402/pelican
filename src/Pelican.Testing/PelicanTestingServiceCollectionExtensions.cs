using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Pelican.Testing
{
    /// <summary>
    /// Registers Pelican testing infrastructure in a dependency injection container.
    /// </summary>
    public static class PelicanTestingServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Pelican handlers and the test dispatcher using handlers discovered from the specified assemblies.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="handlerAssemblies">The assemblies to scan for handlers.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddPelicanTesting(this IServiceCollection services, params Assembly[] handlerAssemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddPelican(handlerAssemblies ?? Array.Empty<Assembly>());
            RegisterTestingServices(services);
            return services;
        }

        /// <summary>
        /// Registers Pelican testing services through the adapter-friendly contract.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="handlerAssemblies">The assemblies to scan for handlers.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddPelicanTestingAdapter(this IServiceCollection services, params Assembly[] handlerAssemblies)
            => services.AddPelicanTesting(handlerAssemblies);

        /// <summary>
        /// Replaces the handler for a request with response.
        /// </summary>
        public static IServiceCollection ReplacePelicanHandler<TRequest, TResponse, THandler>(this IServiceCollection services)
            where TRequest : IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.RemoveAll<IRequestHandler<TRequest, TResponse>>();
            services.AddTransient<IRequestHandler<TRequest, TResponse>, THandler>();
            return services;
        }

        /// <summary>
        /// Replaces the handler for a request without response.
        /// </summary>
        public static IServiceCollection ReplacePelicanHandler<TRequest, THandler>(this IServiceCollection services)
            where TRequest : IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.RemoveAll<IRequestHandler<TRequest>>();
            services.AddTransient<IRequestHandler<TRequest>, THandler>();
            return services;
        }

        private static void RegisterTestingServices(IServiceCollection services)
        {
            services.TryAddScoped<PelicanDispatchTrace>();
            services.TryAddScoped<PelicanTestingDispatcher>();
            services.TryAddScoped<IPelicanTesting>(sp => sp.GetRequiredService<PelicanTestingDispatcher>());
            services.TryAddScoped<IPelicanTestingAdapter>(sp => sp.GetRequiredService<PelicanTestingDispatcher>());
        }
    }
}
