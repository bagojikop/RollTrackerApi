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
using System.Data.Entity.Core.Objects;

namespace garmentBatch.Controllers
{
    public class SetCreationsController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();

        // GET: api/SetCreations
        public IQueryable<SetCreation> GetSets(DateTime from,DateTime to)
        {
            return db.SetCreations.Where(w=>EntityFunctions.TruncateTime(w.createdDate) >= from.Date && EntityFunctions.TruncateTime(w.createdDate) <= to.Date);
        }

        // GET: api/SetCreations/5
        [ResponseType(typeof(SetCreation))]
        public IHttpActionResult GetSet(string id)
        {
            SetCreation setCreation = db.SetCreations.Find(id);
            if (setCreation == null)
            {
                return NotFound();
            }

            return Ok(setCreation);
        }

        // PUT: api/SetCreations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Put(string id, SetCreation set)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != set.setNo)
            {
                return BadRequest();
            }

            db.Entry(set).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SetCreationExists(id))
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

        // POST: api/SetCreations
        [ResponseType(typeof(SetCreation))]
        public IHttpActionResult Post(SetCreation set)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SetCreations.Add(set);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SetCreationExists(set.setNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = set.setNo }, set);
        }

        // DELETE: api/SetCreations/5
        [ResponseType(typeof(SetCreation))]
        public IHttpActionResult Deleten(string id)
        {
            SetCreation setCreation = db.SetCreations.Find(id);
            if (setCreation == null)
            {
                return NotFound();
            }

            db.SetCreations.Remove(setCreation);
            db.SaveChanges();

            return Ok(setCreation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SetCreationExists(string id)
        {
            return db.SetCreations.Count(e => e.setNo == id) > 0;
        }
    }
}