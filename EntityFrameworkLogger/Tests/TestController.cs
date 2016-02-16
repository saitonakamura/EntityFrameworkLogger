using System.Collections.Generic;
using System.Linq;
using EntityFrameworkLogger.Library;
using EntityFrameworkLogger.Model;

namespace EntityFrameworkLogger.Tests
{
    public class TestController
    {
        private readonly IUnitOfWork _context;

        public TestController(IUnitOfWork context)
        {
            _context = context;
        }

        public IReadOnlyCollection<Artist> GetArtists()
        {
            return _context.Query<Artist>().ToList();
        }

        public void UpdateArtistName(Artist artist, string newName)
        {
            artist.Name = newName;

            _context.SaveChanges();
        }
    }
}