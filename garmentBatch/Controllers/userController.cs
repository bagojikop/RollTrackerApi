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
using garmentBatch.Models;
using garmentBatch.classes;
using Newtonsoft.Json;

namespace garmentBatch.Controllers
{
    public class userController : ApiController
    {

        private GarmentBatchEntities db = new GarmentBatchEntities();

        // GET: api/user
        [HttpGet]
        public IHttpActionResult GrantCredentials(string username,string psw)
        {
            var result = new  result();

            string passworddecoded = dsserp.commans.password.EncryptPass(psw);

            var b = dsserp.commans.password.DecryptPass("K2dANW3lEHnyAuv+88G+Mg==");

            var user = db.localuser2.Where(c => c.username == username
            ).Include(c=>c.localuser).FirstOrDefault();

            if (user != null)
            {
                if (user.localuser.password == passworddecoded)
                {
                    user.localuser = null;

                    result.status = 1;
                    result.data = user;
                    result.message = "Success";
                }
                else
                {
                    result.status = 0;
                    result.data = null;
                    result.message = "Incorrect Password";
                }
            }
            else
            {
                result.status = 0;
                result.data = null;
                result.message = "Invalid Username";

            }
            var json = JsonConvert.SerializeObject( result,
              new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return Ok(JsonConvert.DeserializeObject(json));
        }

        public IQueryable<localuser2> Getlocaluser2()
        {
            return db.localuser2;
        }

        // GET: api/user/5
        [ResponseType(typeof(localuser2))]
        public IHttpActionResult Getlocaluser2(int id)
        {
            localuser2 localuser2 = db.localuser2.Find(id);
            if (localuser2 == null)
            {
                return NotFound();
            }

            return Ok(localuser2);
        }

        // PUT: api/user/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putlocaluser2(int id, localuser2 localuser2)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != localuser2.ID)
            {
                return BadRequest();
            }

            db.Entry(localuser2).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!localuser2Exists(id))
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

        // POST: api/user
        [ResponseType(typeof(localuser2))]
        public IHttpActionResult Postlocaluser2(localuser2 localuser2)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.localuser2.Add(localuser2);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = localuser2.ID }, localuser2);
        }

        // DELETE: api/user/5
        [ResponseType(typeof(localuser2))]
        public IHttpActionResult Deletelocaluser2(int id)
        {
            localuser2 localuser2 = db.localuser2.Find(id);
            if (localuser2 == null)
            {
                return NotFound();
            }

            db.localuser2.Remove(localuser2);
            db.SaveChanges();

            return Ok(localuser2);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool localuser2Exists(int id)
        {
            return db.localuser2.Count(e => e.ID == id) > 0;
        }
    }
}