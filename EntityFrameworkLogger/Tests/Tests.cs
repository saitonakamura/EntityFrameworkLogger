using System.Collections.Generic;
using System.Linq;
using Autofac;
using EntityFrameworkLogger.Model;
using NUnit.Framework;

namespace EntityFrameworkLogger.Tests
{
    [TestFixture]
    public class Tests
    {
        private IContainer _ioc;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<TestModule>();
            _ioc = builder.Build();
        }

        [Test]
        public void Test1()
        {
            var testController = _ioc.Resolve<TestController>();

            var artists = testController.GetArtists();

            Assert.IsNotEmpty(artists);
        }
    }

    public class TestController
    {
        private readonly EntityFrameworkLoggerContext _context;

        public TestController(EntityFrameworkLoggerContext context)
        {
            _context = context;
        }

        public IEnumerable<Artist> GetArtists()
        {
            return _context.Artist.ToList();
        }
    }

    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(x => new EntityFrameworkLoggerContext());

            builder.RegisterType<TestController>();
        }
    }
}
