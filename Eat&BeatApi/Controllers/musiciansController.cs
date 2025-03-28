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
using Eat_BeatApi.Models;
using Eat_BeatApi.Clases;

namespace Eat_BeatApi.Controllers
{
    public class musiciansController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/musicians
        public IHttpActionResult Getmusician()
        {
            db.Configuration.LazyLoadingEnabled = false;

            var musicians = db.musician
            .Include(m => m.user)
            .Include(m => m.classification)
            .Select(m => new
            {
                idUser = m.idUser,
                idRol = m.user.idRol,
                name = m.user.name,
                email = m.user.email,
                password = m.user.password,
                rating = db.perform
                    .Where(p => p.idMusician == m.idUser)
                    .Select(p => (double?)p.musicianRate)
                    .DefaultIfEmpty(0)
                    .Average(),
                longitude = m.longitude,
                latitude = m.latitude,
                description = m.description,
                multimedia = m.user.multimedia.Select(mm => new
                {
                    idMultimedia = mm.idMultimedia,
                    url = mm.url,
                    size = mm.size,
                    type = db.multimedia_type.Where(mt => mt.idMultimediaType == mm.idMultimediaType).Select(mt => mt.name).FirstOrDefault()
                }).ToList(),
                genre = m.genre.Select(g => g.name).ToList(),
                classification = m.classification.Select(c => c.name).ToList()
            }).ToList();

            return Ok(musicians);
        }


        // GET: api/musicians/5
        [ResponseType(typeof(musician))]
        public async Task<IHttpActionResult> Getmusician(int id)
        {
            IHttpActionResult result;

            var musician = await db.musician
            .Include(m => m.user)
            .Include(m => m.classification)
            .Select(m => new
            {
                idUser = m.idUser,
                idRol = m.user.idRol,
                name = m.user.name,
                email = m.user.email,
                password = m.user.password,
                rating = db.perform
                    .Where(p => p.idMusician == m.idUser)
                    .Select(p => (double?)p.musicianRate)
                    .DefaultIfEmpty(0)
                    .Average(),
                longitude = m.longitude,
                latitude = m.latitude,
                description = m.description,
                multimedia = m.user.multimedia.Select(mm => new
                {
                    idMultimedia = mm.idMultimedia,
                    url = mm.url,
                    size = mm.size,
                    type = db.multimedia_type.Where(mt => mt.idMultimediaType == mm.idMultimediaType).Select(mt => mt.name).FirstOrDefault()
                }).ToList(),
                genre = m.genre.Select(g => g.name).ToList(),
                classification = m.classification.Select(c => c.name).ToList()
            })
            .FirstOrDefaultAsync();

            if (musician == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(musician);
            }

            return result;
        }

        // PUT: api/musicians/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmusician(int id, musician musician)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != musician.idUser)
            {
                return BadRequest();
            }

            db.Entry(musician).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!musicianExists(id))
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

        // POST: api/musicians
        [ResponseType(typeof(musician))]
        public async Task<IHttpActionResult> Postmusician(musician musician)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.musician.Add(musician);

                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = musician.idUser }, musician);
                }
                catch (DbUpdateException)
                {
                    if (musicianExists(musician.idUser))
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

        // DELETE: api/musicians/5
        [ResponseType(typeof(musician))]
        public async Task<IHttpActionResult> Deletemusician(int id)
        {
            IHttpActionResult result;

            musician _musician = await db.musician.FindAsync(id);
            if (_musician == null)
            {
                result = NotFound();
            }
            else
            {
                try
                {
                    db.musician.Remove(_musician);
                    await db.SaveChangesAsync();
                    result = Ok(_musician);
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

        private bool musicianExists(int id)
        {
            return db.musician.Count(e => e.idUser == id) > 0;
        }
    }
}