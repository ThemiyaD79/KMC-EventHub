using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using KNC.API.Models;

namespace KNC.API.Controllers
{
    public class KMCEventsController : ApiController
    {
        private EventDbContext db = new EventDbContext();
        public IQueryable<KMCEvent> GetKMCEvents(string search = null, string category = null)
        {
            var events = db.KMCEvents.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                events = events.Where(e => e.Title.Contains(search) || e.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(category))
            {
                events = events.Where(e => e.Category == category);
            }

            return events;
        }
        public IHttpActionResult GetKMCEvent(string id)
        {
            KMCEvent kMCEvent = db.KMCEvents.Find(id);
            if (kMCEvent == null) return NotFound();
            return Ok(kMCEvent);
        }
        public IHttpActionResult PutKMCEvent(string id, KMCEvent kMCEvent)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != kMCEvent.EventId) return BadRequest();

            db.Entry(kMCEvent).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KMCEventExists(id)) return NotFound();
                else throw;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        public IHttpActionResult PostKMCEvent(KMCEvent kMCEvent)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (KMCEventExists(kMCEvent.EventId)) return BadRequest("An event with this ID already exists.");

            db.KMCEvents.Add(kMCEvent);

            try { db.SaveChanges(); }
            catch (Exception ex) { return InternalServerError(ex); }

            return CreatedAtRoute("DefaultApi", new { id = kMCEvent.EventId }, kMCEvent);
        }
        public IHttpActionResult DeleteKMCEvent(string id)
        {
            KMCEvent kMCEvent = db.KMCEvents.Find(id);
            if (kMCEvent == null) return NotFound();

            db.KMCEvents.Remove(kMCEvent);
            db.SaveChanges();
            return Ok(kMCEvent);
        }
        [Route("api/KMCEvents/Register")]
        [HttpPost]
        public IHttpActionResult RegisterForEvent(EventRegistration registration)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            registration.RegistrationDate = DateTime.Now;
            db.EventRegistrations.Add(registration);
            db.SaveChanges();

            return Ok("Registration Successful");
        }
        [Route("api/KMCEvents/Registrations")]
        [HttpGet]
        public IHttpActionResult GetAllRegistrations()
        {
            var registrations = from r in db.EventRegistrations
                                join e in db.KMCEvents on r.EventId equals e.EventId
                                select new RegistrationDTO
           {
                         RegistrationId = r.RegistrationId,
                         EventId = r.EventId,
                         EventTitle = e.Title,
                         Name = r.Name,
                         Age = r.Age,
                         PhoneNumber = r.PhoneNumber,
                         RegistrationDate = r.RegistrationDate
            };

            return Ok(registrations.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        private bool KMCEventExists(string id)
        {
            return db.KMCEvents.Count(e => e.EventId == id) > 0;
        }
    }
}