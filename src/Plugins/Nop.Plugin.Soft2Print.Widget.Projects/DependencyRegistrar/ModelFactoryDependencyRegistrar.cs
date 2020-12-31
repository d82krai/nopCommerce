using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.Soft2Print.Widget.Projects.DependencyRegistrar
{
    public class ModelFactoryDependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ModelFactory.ProjectModelFactory>().As<ModelFactory.ProjectModelFactory>().InstancePerLifetimeScope();
        }
        public int Order
        {
            get { return 99; }
        }
    }
}
