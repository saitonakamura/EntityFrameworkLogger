using System.Collections.Generic;
using System.Linq;
using EntityFrameworkLogger.Library;
using EntityFrameworkLogger.Model;

namespace EntityFrameworkLogger.Tests
{
    public class TestController
    {
        private readonly EntityFrameworkLoggerContext _context;
        private readonly LoggerService _loggerService;

        public TestController(EntityFrameworkLoggerContext context, LoggerService loggerService)
        {
            _context = context;
            _loggerService = loggerService;
        }

        public IReadOnlyCollection<Artist> GetArtists()
        {
            return _context.Artist.ToList();
        }

        public void UpdateArtistName(Artist artist, string newName)
        {
            artist.Name = newName;

            _loggerService.LogChanges();
            _context.SaveChanges();
        }
    }
}