using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Soft2Print.DependencyRegistrar
{
    public class ContextDependencyRegistrar : IDependencyRegistrar
    {
        private const string websessionContext = "nop_object_context_s2p_webSession";
        private const string projectAttributesContext = "nop_object_context_s2p_projectAttributes";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {

            builder.RegisterType<Data.Repositories.WebSessionRepository>().As<Data.Repositories.IWebSessionRepository>().InstancePerLifetimeScope();
            builder.RegisterType<Data.Repositories.ProjectAttributeRepository>().As<Data.Repositories.IProjectAttributeRepository>().InstancePerLifetimeScope();
            //data context
            builder.RegisterPluginDataContext<Data.DALContext>(websessionContext);
            //override required repository with our custom context
            builder.RegisterType<EfRepository<Data.Entities.S2P_WebSession>>().As<IRepository<Data.Entities.S2P_WebSession>>().WithParameter(ResolvedParameter.ForNamed<IDbContext>(websessionContext)).InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<Data.Entities.S2P_ProjectAttributes>>().As<IRepository<Data.Entities.S2P_ProjectAttributes>>().WithParameter(ResolvedParameter.ForNamed<IDbContext>(websessionContext)).InstancePerLifetimeScope();


        }

        public int Order
        {
            get { return 1; }
        }
    }
}
