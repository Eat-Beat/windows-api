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

        // GET: api/chats
        public IQueryable<chat> Getchat()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.chat;
        }

        // GET: api/chats/5
        [ResponseType(typeof(chat))]
        public IHttpActionResult Getchat(int id)
        {
            chat chat = db.chat.Find(id);
            if (chat == null)
            {
                return NotFound();
            }

            return Ok(chat);
        }

        // PUT: api/chats/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putchat(int id, chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != chat.idRestaurant)
            {
                return BadRequest();
            }

            db.Entry(chat).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!chatExists(id))
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

        // POST: api/chats
        [ResponseType(typeof(chat))]
        public IHttpActionResult Postchat(chat chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.chat.Add(chat);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (chatExists(chat.idRestaurant))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = chat.idRestaurant }, chat);
        }

        // DELETE: api/chats/5
        [ResponseType(typeof(chat))]
        public IHttpActionResult Deletechat(int id)
        {
            chat chat = db.chat.Find(id);
            if (chat == null)
            {
                return NotFound();
            }

            db.chat.Remove(chat);
            db.SaveChanges();

            return Ok(chat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool chatExists(int id)
        {
            return db.chat.Count(e => e.idRestaurant == id) > 0;
        }
    }
}