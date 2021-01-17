using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Web.Factories;

namespace Nop.Plugin.Soft2Print.DependencyRegistrar
{
    public class OverwriteDependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //ModelFactory
            builder.RegisterType<ModelFactory.CatalogModelFactory>().As<ICatalogModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ModelFactory.ProductModelFactory>().As<IProductModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ModelFactory.ShoppingCartModelFactory>().As<IShoppingCartModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ModelFactory.CommonModelFactory>().As<ICommonModelFactory>().InstancePerLifetimeScope();

            //Services
            builder.RegisterType<Services.Overwrite_ProductAttributeParser>().As<IProductAttributeParser>().InstancePerLifetimeScope();
            builder.RegisterType<Services.Overwrite_ShoppingCartService>().As<IShoppingCartService>().InstancePerLifetimeScope();
        }
        public int Order
        {
            get { return 99; }
        }
    }
}
