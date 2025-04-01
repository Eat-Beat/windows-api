using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Eat_BeatApi.Models;

namespace Eat_BeatApi.Controllers
{
    public class multimediasController : ApiController
    {
        private Entities db = new Entities();

        [HttpGet]
        [Route("api/multimedias/{id}")]
        public IHttpActionResult GetRestaurantChat(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var multimedias = db.multimedia
                .Where(m => m.idUser == id)
                .Select(m => new
                {
                    m.idMultimedia,
                    m.url,
                    m.size,
                    type = db.multimedia_type.Where(mt => mt.idMultimediaType == m.idMultimediaType).Select(mt => mt.name).FirstOrDefault()
                })
                .ToList();

            if (!multimedias.Any())
            {
                return NotFound();
            }

            return Ok(multimedias);
        }

        // PUT: api/multimedias/1
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/multimedias/{idUser}")]
        public IHttpActionResult Putmultimedia(int idUser, multimedia multimedia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar el registro con idUser y siempre idMultimedia = 1
            var existingMultimedia = db.multimedia.FirstOrDefault(m => m.idUser == idUser && m.idMultimedia == 1);

            if (existingMultimedia == null)
            {
                return NotFound();
            }

            // Actualizar los valores con los datos recibidos
            existingMultimedia.idMultimediaType = multimedia.idMultimediaType;
            existingMultimedia.url = multimedia.url;
            existingMultimedia.size = multimedia.size;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(); // Si hay conflictos con la concurrencia
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/multimedias
        [ResponseType(typeof(multimedia))]
        public IHttpActionResult Postmultimedia(multimedia multimedia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar el último idMultimedia para este usuario
            var lastMultimedia = db.multimedia
                                   .Where(m => m.idUser == multimedia.idUser)
                                   .OrderByDescending(m => m.idMultimedia)
                                   .FirstOrDefault();

            multimedia.idMultimedia = (lastMultimedia != null) ? lastMultimedia.idMultimedia + 1 : 1;

            db.multimedia.Add(multimedia);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return Conflict();
            }

            return CreatedAtRoute("DefaultApi", new { idUser = multimedia.idUser, idMultimedia = multimedia.idMultimedia }, multimedia);
        }

        // DELETE: api/multimedias/1/5
        [ResponseType(typeof(multimedia))]
        [HttpDelete]
        [Route("api/multimedias/{idUser}/{idMultimedia}")]
        public IHttpActionResult Deletemultimedia(int idUser, int idMultimedia)
        {
            multimedia multimedia = db.multimedia.FirstOrDefault(m => m.idUser == idUser && m.idMultimedia == idMultimedia);

            if (multimedia == null)
            {
                return NotFound();
            }

            db.multimedia.Remove(multimedia);
            db.SaveChanges();

            return Ok(multimedia);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool multimediaExists(int id)
        {
            return db.multimedia.Count(e => e.idUser == id) > 0;
        }
    }
}