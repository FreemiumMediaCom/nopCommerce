using System.Threading.Tasks;
using Autofac;
using FreemiumMedia.Nop.Plugin.Misc.Meetup.Services;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace FreemiumMedia.Nop.Plugin.Misc.Meetup.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public async virtual Task Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //register UPSService
            builder.RegisterType<MeetupService>().AsSelf().InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}