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

namespace garmentBatch.Controllers
{
    public class dispatchController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();

        // GET: api/dispatch
        public IQueryable<PackSliphead> getdispateches()
        {
            return db.PackSlipDetls.Where(w => w.scanned == false).Select(c => c.PackSliphead).Distinct();

        }

        // GET: api/dispatch/5
        [ResponseType(typeof(PackSlipDetl))]
        public IHttpActionResult GetPackSlipDetl(long id)
        {
            PackSlipDetl packSlipDetl = db.PackSlipDetls.Find(id);
            if (packSlipDetl == null)
            {
                return NotFound();
            }

            return Ok(packSlipDetl);
        }

        // PUT: api/dispatch/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRoll(long id, PackSlipDetl packSlipDetl)
        {
            var roll = db.PackSlipDetls.Where(c => c.slipdetlId == id && c.scanned == false);
            if (roll != null) 
            packSlipDetl.scanned = false;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != packSlipDetl.slipdetlId)
            {
                return BadRequest();
            }

            db.Entry(packSlipDetl).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackSlipDetlExists(id))
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

        // POST: api/dispatch
        [ResponseType(typeof(PackSlipDetl))]
        public IHttpActionResult PostPackSlipDetl(PackSlipDetl packSlipDetl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PackSlipDetls.Add(packSlipDetl);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = packSlipDetl.slipdetlId }, packSlipDetl);
        }

        // DELETE: api/dispatch/5
        [ResponseType(typeof(PackSlipDetl))]
        public IHttpActionResult DeletePackSlipDetl(long id)
        {
            PackSlipDetl packSlipDetl = db.PackSlipDetls.Find(id);
            if (packSlipDetl == null)
            {
                return NotFound();
            }

            db.PackSlipDetls.Remove(packSlipDetl);
            db.SaveChanges();

            return Ok(packSlipDetl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PackSlipDetlExists(long id)
        {
            return db.PackSlipDetls.Count(e => e.slipdetlId == id) > 0;
        }
    }
}