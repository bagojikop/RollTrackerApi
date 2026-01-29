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
    public class whOutRollController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();

        // GET: api/whOutRoll
        public IQueryable<PackSlipReqdetl> GetRolls(long id)
        {
            return db.PackSlipReqdetls.Where(c=>c.req_id==id);
        }

        // GET: api/whOutRoll/5
        [ResponseType(typeof(PackSlipReqdetl))]
        public IHttpActionResult GetPackSlipReqdetl(long id)
        {
            PackSlipReqdetl packSlipReqdetl = db.PackSlipReqdetls.Find(id);
            if (packSlipReqdetl == null)
            {
                return NotFound();
            }

            return Ok(packSlipReqdetl);
        }

        // PUT: api/whOutRoll/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPackSlipReqdetl(long id, PackSlipReqdetl packSlipReqdetl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != packSlipReqdetl.detl_id)
            {
                return BadRequest();
            }

            db.Entry(packSlipReqdetl).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackSlipReqdetlExists(id))
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

        // POST: api/whOutRoll
       
        public IHttpActionResult PostRoll(PackSlipReqdetl roll)
        {
            var mtrs = db.PackSlipReqdetls.Where(w => w.req_id == roll.req_id).Sum(s => (int?)s.mtr) ?? 0;
            byte grade = db.PackSlipReqheads.Where(w => w.req_id == roll.req_id).Select(s => s.grade_id).First();

            if (mtrs < roll.PackSlipReqhead.totalMtrs)
            {
              
                 var batch = db.Batches.Include(c=>c.PackSlipReqdetls).
                    Where(c => c.prodCode == roll.PackSlipReqhead.prodCode && c.QcCode == roll.PackSlipReqhead.QcCode
                && c.finYear == roll.finyear && c.loomNo == roll.loomNo && c.BatchNo == roll.BatchNo 
                && (c.PackSlipReqdetls.Sum(s => (int?)s.mtr) ?? 0) != c.Mtrs
                && (grade ==0 || c.GradeId == grade)
                  && (roll.PackSlipReqhead.status_id  == 0 || c.Rejected  == (roll.PackSlipReqhead.status_id==1 ?true :false))
                ).FirstOrDefault();

                if (batch != null)
                { 
                    roll.mtr = batch.Mtrs - batch.PackSlipReqdetls.Sum(s => (int?)s.mtr) ?? 0;
                    if (roll.mtr > 0)
                    {
                        roll.PackSlipReqhead = null;
                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        roll.PackSlipReqhead = null;

                        db.PackSlipReqdetls.Add(roll);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbUpdateException)
                        {
                            if (PackSlipReqdetlExists(roll.detl_id))
                            {
                                return Conflict();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    return Conflict();
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = roll.detl_id}, roll);
        }

        // DELETE: api/whOutRoll/5
        [ResponseType(typeof(PackSlipReqdetl))]
        public IHttpActionResult DeleteRoll(long id)
        {
            PackSlipReqdetl packSlipReqdetl = db.PackSlipReqdetls.Find(id);
            if (packSlipReqdetl == null)
            {
                return NotFound();
            }

            db.PackSlipReqdetls.Remove(packSlipReqdetl);
            db.SaveChanges();

            return Ok(packSlipReqdetl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PackSlipReqdetlExists(long id)
        {
            return db.PackSlipReqdetls.Count(e => e.detl_id == id) > 0;
        }
    }
}