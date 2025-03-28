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
    public class chatsController : ApiController
    {
        private Entities db = new Entities();

        [HttpGet]
        [Route("api/chats/restaurant/{id}")]
        public IHttpActionResult GetRestaurantChat(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var chats = db.chat
                .Where(c => c.idRestaurant == id)
                .Select(c => new
                {
                    c.idRestaurant,
                    c.idMusician,
                    c.idSender,
                    c.isMultimedia,
                    c.message,
                    c.timestamp
                })
                .ToList();

            if (!chats.Any())
            {
                return NotFound();
            }

            return Ok(chats);
        }

        [HttpGet]
        [Route("api/chats/musician/{id}")]
        public IHttpActionResult GetMusicianChat(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var chats = db.chat
                .Where(c => c.idMusician == id)
                .Select(c => new
                {
                    c.idRestaurant,
                    c.idMusician,
                    c.idSender,
                    c.isMultimedia,
                    c.message,
                    c.timestamp
                })
                .ToList();

            if (!chats.Any())
            {
                return NotFound();
            }

            return Ok(chats);
        }

        // POST: api/chats
        [ResponseType(typeof(chat))]
        public IHttpActionResult Postchat([FromBody] chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que los IDs existan en la base de datos antes de insertar
            bool restaurantExists = db.restaurant.Any(r => r.idUser == chat.idRestaurant);
            bool musicianExists = db.musician.Any(m => m.idUser == chat.idMusician);
            bool senderExists = db.user.Any(u => u.idUser == chat.idSender);

            if (!restaurantExists)
            {
                return BadRequest("El idRestaurant no existe.");
            }
            if (!musicianExists)
            {
                return BadRequest("El idMusician no existe.");
            }
            if (!senderExists)
            {
                return BadRequest("El idSender no existe.");
            }

            // Agregar el chat a la base de datos
            db.chat.Add(chat);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return InternalServerError(ex);
            }

            return CreatedAtRoute("DefaultApi", new { id = chat.idRestaurant }, chat);
        }
    }
}