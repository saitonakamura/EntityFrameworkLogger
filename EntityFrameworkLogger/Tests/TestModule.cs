using Autofac;
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

            builder.RegisterType<LoggerService>().InstancePerLifetimeScope();
            builder.RegisterType<TestController>();
        }
    }
}