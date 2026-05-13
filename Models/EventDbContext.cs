using System.Data.Entity;

namespace KNC.API.Models
{
    public class EventDbContext : DbContext
    {
        public EventDbContext() : base("name=EventDbContext")
        {
            Database.SetInitializer<EventDbContext>(null);
        }

        public DbSet<KMCEvent> KMCEvents { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
    }
}