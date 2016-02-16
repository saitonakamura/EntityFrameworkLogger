using System.Collections.Generic;

namespace EntityFrameworkLogger.Model
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}