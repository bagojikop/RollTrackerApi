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

namespace garmentBatch.Controllers
{
    public class PackSlipScanController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();

        // GET: api/PackSlipScan
        public IQueryable<PackSlipDetl> GetPackSlipDetls()
        {
            return db.PackSlipDetls;
        }

        // GET: api/PackSlipScan/5
        [ResponseType(typeof(PackSlipDetl))]
        public IHttpActionResult GetPackSlipDetl(long id)
        {
            PackSlipDetl packSlipDetl = db.PackSlipDetls.Where(w => w.slipdetlId == id).FirstOrDefault();
            if (packSlipDetl == null)
            {
                return NotFound();
            }

            return Ok(packSlipDetl);
        }

        // PUT: api/PackSlipScan/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putscan(long id, batch Batch)
        {

        var roll=db.PackSlipDetls.Where(c => c.SlipId == id && c.finyear == Batch.finYear
            && c.loomNo == Batch.loomNo && c.BatchNo == Batch.BatchNo && c.scanned == false).FirstOrDefault();

            if (roll == null)
            {
                return NotFound();
            }

            var b = new PackSlipDetl();
            b.SlipId =roll.SlipId;
            b.slipdetlId = roll.slipdetlId;
            b.BatchNo = roll.BatchNo;
            b.description = roll.description;
            b.finyear = roll.finyear;
            b.loomNo = roll.loomNo;
            b.mtr = roll.mtr;
            b.weight = roll.weight;
            b.scanned = true;

            db.Entry(roll).CurrentValues.SetValues(b);

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

        // POST: api/PackSlipScan
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

        // DELETE: api/PackSlipScan/5
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