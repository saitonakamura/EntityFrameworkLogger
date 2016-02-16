namespace EntityFrameworkLogger.Model
{
    public class Track
    {
        public int TrackId { get; set; }
        public string Name { get; set; }
        public virtual Album Album { get; set; }
        public string Composer { get; set; }
    }
}