using Autofac;
using Autofac.Extras.DynamicProxy2;
using EntityFrameworkLogger.Library;
using EntityFrameworkLogger.Model;

namespace EntityFrameworkLogger.Tests
{
    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(x => new EntityFrameworkLoggerContext()).InstancePerLifetimeScope();

            builder.RegisterType<LoggerStorage>().InstancePerLifetimeScope();
            builder.RegisterType<LoggerService>();
            builder.RegisterType<LoggerServiceInterceptor>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().EnableInterfaceInterceptors().InterceptedBy(typeof(LoggerServiceInterceptor)).InstancePerLifetimeScope();

            builder.RegisterType<TestController>();
        }
    }
}