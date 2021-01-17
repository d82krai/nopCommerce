using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.Soft2Print.Widget.Projects.DependencyRegistrar
{
    public class Soft2PrintServiceDependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<Services.PluginSettingService>().As<Services.PluginSettingService>().InstancePerLifetimeScope();
        }
        public int Order { get { return 1; } }
    }
}
