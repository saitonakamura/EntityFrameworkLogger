using System;
using System.Linq;
using Autofac;
using EntityFrameworkLogger.Library;
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

            var artist = artists.First();
            var oldName = artist.Name;
            var newName = "New name" + new Random().Next(100);
            testController.UpdateArtistName(artist, newName);

            var loggerService = _ioc.Resolve<LoggerService>();

            var changes = loggerService.GetChangesByEntityId<Artist>(artist.ArtistId);
            Assert.AreEqual(1, changes.Count);

            var change = changes.First();
            Assert.AreEqual(EntityOperations.Modified, change.EntityOperation);

            var modifiedFieldName = "Name";
            Assert.IsTrue(change.ValueChanges.Select(x => x.FieldName).Contains(modifiedFieldName), "'Name' is not in the ValuesChanges");
            Assert.AreEqual(oldName, change.ValueChanges.Single(x => x.FieldName == modifiedFieldName).OldValue);
            Assert.AreEqual(newName, change.ValueChanges.Single(x => x.FieldName == modifiedFieldName).NewValue);
        }
    }
}
