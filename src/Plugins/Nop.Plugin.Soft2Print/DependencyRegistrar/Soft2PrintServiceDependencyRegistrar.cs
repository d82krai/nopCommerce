using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.Soft2Print.DependencyRegistrar
{
    public class Soft2PrintServiceDependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<Services.SettingService>().As<Services.SettingService>().InstancePerLifetimeScope();
            builder.RegisterType<Services.PluginSettingService>().As<Services.PluginSettingService>().InstancePerLifetimeScope();
            builder.RegisterType<Services.ProductAttributeService>().As<Services.ProductAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<Services.Soft2PrintAPIService>().As<Services.Soft2PrintAPIService>().InstancePerLifetimeScope();
        }
        public int Order { get { return 1; } }
    }
}
