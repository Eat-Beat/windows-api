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
                    rating = db.perform.Where(p => p.idRestaurant == r.idUser).Average(p => p.restaurantRate),
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
                rating = db.perform.Where(p => p.idRestaurant == r.idUser).Average(p => p.restaurantRate), 
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
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putrestaurant(int id, restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurant.idUser)
            {
                return BadRequest();
            }

            db.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!restaurantExists(id))
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

        // POST: api/restaurants
        [ResponseType(typeof(restaurant))]
        public async Task<IHttpActionResult> Postrestaurant(restaurant restaurant)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.restaurant.Add(restaurant);

                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = restaurant.idUser }, restaurant);
                }
                catch (DbUpdateException)
                {
                    if (restaurantExists(restaurant.idUser))
                    {
                        result = Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return result;
        }

        // DELETE: api/restaurants/5
        [ResponseType(typeof(restaurant))]
        public async Task<IHttpActionResult> Deleterestaurant(int id)
        {
            IHttpActionResult result;

            restaurant restaurant = await db.restaurant.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    db.restaurant.Remove(restaurant);
                    await db.SaveChangesAsync();
                    result = Ok(restaurant);
                }
                catch (DbUpdateException ex)
                {
                    String missatge = "";
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    missatge = Utilitat.MissatgeError(sqlException);
                    result = BadRequest(missatge);
                }
            }

            return result;
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