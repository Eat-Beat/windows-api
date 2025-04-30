using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Eat_BeatApi.Clases;
using Eat_BeatApi.Models;

namespace Eat_BeatApi.Controllers
{
    public class restaurantsController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/restaurants
        public IHttpActionResult Getrestaurant()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var restaurants = db.restaurant
                .Include(r => r.user)
                .Select(r => new
                {
                    idUser = r.user.idUser,
                    idRol = r.user.idRol,
                    name = r.user.name,
                    email = r.user.email,
                    password = r.user.password,
                    rating = db.perform
                        .Where(p => p.idRestaurant == r.idUser)
                        .Select(p => (double?)p.restaurantRate)
                        .DefaultIfEmpty(0) 
                        .Average(),
                    address = r.address,
                    addressNum = r.addressNum,
                    zipCode = r.zipCode,
                    multimedia = r.user.multimedia
                        .Where(mm => mm.idMultimedia == 1)
                        .Select(mm => new
                        {
                            idMultimedia = mm.idMultimedia,
                            url = mm.url,
                            size = mm.size,
                            type = db.multimedia_type.Where(mt => mt.idMultimediaType == mm.idMultimediaType).Select(mt => mt.name).FirstOrDefault()
                        }).FirstOrDefault(),
                })
                .ToList();

            return Ok(restaurants);
        }

        // GET: api/restaurants/5
        [ResponseType(typeof(restaurant))]
        public async Task<IHttpActionResult> Getrestaurant(int id)
        {
            IHttpActionResult result;

            var restaurant = await db.restaurant
            .Include(r => r.user) 
            .Where(r => r.idUser == id) 
            .Select(r => new
            {
                idUser = r.user.idUser,
                idRol = r.user.idRol,
                name = r.user.name,
                email = r.user.email,
                password = r.user.password, 
                rating = db.perform
                    .Where(p => p.idRestaurant == r.idUser)
                    .Select(p => (double?)p.restaurantRate)
                    .DefaultIfEmpty(0)
                    .Average(),
                address = r.address,
                addressNum = r.addressNum,
                zipCode = r.zipCode
            })
            .FirstOrDefaultAsync(); 

            if (restaurant == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(restaurant);
            }

            return result;
        }

        // PUT: api/restaurants/5
        [HttpPut]
        [Route("api/restaurants/{id}")]
        public async Task<IHttpActionResult> PutRestaurant(int id, restaurant restaurant)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != restaurant.idUser) return BadRequest();

            db.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!db.restaurant.Any(r => r.idUser == id))
                    return NotFound();
                else
                    throw;
            }
        }

        // POST: api/restaurants
        [ResponseType(typeof(restaurant))]
        [HttpPost]
        [Route("api/restaurants")]
        public async Task<IHttpActionResult> PostRestaurant(restaurant restaurant)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            db.restaurant.Add(restaurant);
            try
            {
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { id = restaurant.idUser }, restaurant);
            }
            catch (DbUpdateException)
            {
                if (db.restaurant.Any(r => r.idUser == restaurant.idUser))
                    return Conflict();
                else
                    throw;
            }
        }

        // DELETE: api/restaurants/5
        [HttpDelete]
        [Route("api/restaurants/{id}")]
        public async Task<IHttpActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await db.restaurant.FindAsync(id);
            if (restaurant == null) return NotFound();

            db.restaurant.Remove(restaurant);
            await db.SaveChangesAsync();

            return Ok(restaurant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool restaurantExists(int id)
        {
            return db.restaurant.Count(e => e.idUser == id) > 0;
        }
    }
}