using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Eat_BeatApi.Models;

namespace Eat_BeatApi.Controllers
{
    public class usersController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/users
        public IHttpActionResult Getuser()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var users = db.user
                .Select(u => new
                {
                    idUser = u.idUser,
                    idRol = u.idRol,
                    name = u.name,
                    email = u.email,
                    password = u.password
                })
                .ToList();

            return Ok(users);
        }

        // GET: api/users/5
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Getuser(int id)
        {
            IHttpActionResult result;

            var user = await db.user
                .Where(u => u.idUser == id)
                .Select(u => new
                {
                    idUser = u.idUser,
                    idRol = u.idRol,
                    name = u.name,
                    email = u.email,
                    password = u.password
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(user);
            }

            return result;
        }

        // PUT: api/users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putuser(int id, user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.idUser)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userExists(id))
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

        // POST: api/users
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Postuser(user user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.user.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.idUser }, user);
        }

        // DELETE: api/users/5
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Deleteuser(int id)
        {
            user user = await db.user.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.user.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool userExists(int id)
        {
            return db.user.Count(e => e.idUser == id) > 0;
        }
    }
}