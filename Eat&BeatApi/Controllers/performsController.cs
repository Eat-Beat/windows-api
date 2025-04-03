using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Eat_BeatApi.Models;

namespace Eat_BeatApi.Controllers
{
    public class performsController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/performs
        public IHttpActionResult Getperform()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var performs = db.perform
                .Select(p => new
                {
                    idPerform = p.idPerform,
                    idMusician = p.idMusician,
                    idRestaurant = p.idRestaurant,
                    dateTime = p.dateTime,
                    budget = p.budget,
                    musicianRate = p.musicianRate,
                    restaurantRate = p.restaurantRate
                })
                .ToList();

            return Ok(performs);
        }

        [HttpGet]
        [Route("api/performs/musician/{id}")]
        public IHttpActionResult GetPerformsByIdMusician(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var performs = db.perform
                .Where(p => p.idMusician == id)
                .Select(p => new
                {
                    idPerform = p.idPerform,
                    idMusician = p.idMusician,
                    idRestaurant = p.idRestaurant,
                    dateTime = p.dateTime,
                    budget = p.budget,
                    musicianRate = p.musicianRate,
                    restaurantRate = p.restaurantRate
                })
                .ToList();

            return Ok(performs);
        }

        [HttpGet]
        [Route("api/performs/profile/musician/{id}")]
        public IHttpActionResult GetPerformsProfileByIdMusician(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var performs = db.perform
                .Include(p => p.restaurant)
                .Include(p => p.restaurant.user)
                .Where(p => p.idMusician == id)
                .Select(p => new
                {
                    name = p.restaurant.user.name,
                    address = p.restaurant.address,
                    addressNum = p.restaurant.addressNum,
                    zipCode = p.restaurant.zipCode,
                    dateTime = p.dateTime,
                    rate = p.musicianRate
                })
                .ToList();

            return Ok(performs);
        }

        [HttpGet]
        [Route("api/performs/restaurant/{id}")]
        public IHttpActionResult GetPerformsByIdRestaurant(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var performs = db.perform
                .Where(p => p.idRestaurant == id)
                .Select(p => new
                {
                    idPerform = p.idPerform,
                    idMusician = p.idMusician,
                    idRestaurant = p.idRestaurant,
                    dateTime = p.dateTime,
                    budget = p.budget,
                    musicianRate = p.musicianRate,
                    restaurantRate = p.restaurantRate
                })
                .ToList();

            return Ok(performs);
        }

        [HttpGet]
        [Route("api/performs/profile/musician/{id}")]
        public IHttpActionResult GetPerformsProfileByIdRestaurant(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var performs = db.perform
                .Include(p => p.restaurant)
                .Include(p => p.restaurant.user)
                .Where(p => p.idMusician == id)
                .Select(p => new
                {
                    name = p.restaurant.user.name,
                    address = p.restaurant.address,
                    addressNum = p.restaurant.addressNum,
                    zipCode = p.restaurant.zipCode,
                    dateTime = p.dateTime,
                    rate = p.restaurantRate
                })
                .ToList();

            return Ok(performs);
        }

        // GET: api/performs/5
        [ResponseType(typeof(perform))]
        public async Task<IHttpActionResult> Getperform(int id)
        {
            perform perform = await db.perform.FindAsync(id);
            if (perform == null)
            {
                return NotFound();
            }

            return Ok(perform);
        }

        // PUT: api/performs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putperform(int id, perform perform)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != perform.idPerform)
            {
                return BadRequest();
            }

            db.Entry(perform).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!performExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/performs
        [ResponseType(typeof(perform))]
        public async Task<IHttpActionResult> Postperform(perform perform)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.perform.Add(perform);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = perform.idPerform }, perform);
        }

        // DELETE: api/performs/5
        [ResponseType(typeof(perform))]
        public async Task<IHttpActionResult> Deleteperform(int id)
        {
            perform perform = await db.perform.FindAsync(id);
            if (perform == null)
            {
                return NotFound();
            }

            db.perform.Remove(perform);
            await db.SaveChangesAsync();

            return Ok(perform);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool performExists(int id)
        {
            return db.perform.Count(e => e.idPerform == id) > 0;
        }
    }
}