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
using System.Threading.Tasks;

namespace garmentBatch.Controllers
{
    public class PackSlipController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();
        respayload rtn = new respayload();

        // GET: api/PackSlip
        [HttpGet]
        public async Task<IHttpActionResult> GetPackSlipheads()
        {
            try
            {
                rtn.data = await db.PackSlipheads.ToListAsync();
            }catch (Exception ex)
            {
                rtn.status_cd = 0;
                rtn.error.message = ex.Message;
            }

            return Ok(rtn);
        }

        // GET: api/PackSlip
        [HttpGet]
        public async Task<IHttpActionResult> GetReadyPackSlips()
        {
            try
            {
                rtn.data = await db.PackSlipheads.Include(i => i.PackSlipReqhead).
                    Where(c => c.PackSlipDetls.Any(w => w.scanned == false)).AsNoTracking()
                    .Include(c => c.Quality.product) .ToListAsync();
            }catch (Exception ex)
            {
                rtn.status_cd = 0;
                rtn.error.message = ex.Message;
            }

            return Ok(rtn);
        }

        // GET: api/PackSlip/5
        [HttpGet]
        public async Task<IHttpActionResult> GetPackSliphead(int id)
        {
            try
            {
                rtn.data =await db.PackSlipheads.Where(s => s.SlipId == id).SingleOrDefaultAsync();

            }catch(Exception ex)
            {
                rtn.status_cd = 0;
                rtn.error.message = ex.Message;
            }

            return Ok(rtn);
        }

        // GET: api/PackSlip/5
        [HttpGet]
        public async Task<IHttpActionResult> GetRolls(int id)
        {
            try
            {
                rtn.data=await db.PackSlipDetls.Where(w => w.SlipId == id && w.scanned == true).ToListAsync();
            }catch( Exception ex)
            {
                rtn.status_cd= 0;
                rtn.error.message = ex.Message;
            }
            return Ok(rtn);
        }

        // PUT: api/PackSlip/5
        [HttpPut]
        public IHttpActionResult PutScan(int id,batch data)
        {
            PackSlipDetl detl = db.PackSlipDetls.Where(c => c.SlipId == id && 
                                                       c.finyear == data.finYear && 
                                                       c.loomNo == data.loomNo 
                                                       && c.BatchNo == data.BatchNo 
                                                       && c.scanned == false).FirstOrDefault();
                detl.scanned = true;

            db.Entry(detl).State = EntityState.Modified;
 
                db.SaveChanges();
             
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/PackSlip/5
        [HttpPut]
        public IHttpActionResult removeScan(int slipdetlId )
        {
            PackSlipDetl detl = db.PackSlipDetls.Where(c => c.slipdetlId == slipdetlId ).FirstOrDefault();
            detl.scanned = false;

            db.Entry(detl).State = EntityState.Modified;


            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);


        }

        // PUT: api/PackSlip/5
        [HttpPut]
        public IHttpActionResult PutPackSliphead(int id, PackSliphead packSliphead)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != packSliphead.SlipId)
            {
                return BadRequest();
            }

            db.Entry(packSliphead).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackSlipheadExists(id))
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

        // POST: api/PackSlip
        [HttpPost]
        public IHttpActionResult PostPackSliphead(PackSliphead packSliphead)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PackSlipheads.Add(packSliphead);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = packSliphead.SlipId }, packSliphead);
        }

        // DELETE: api/PackSlip/5
        [HttpDelete]
        public IHttpActionResult DeletePackSliphead(int id)
        {
            PackSliphead packSliphead = db.PackSlipheads.Find(id);
            if (packSliphead == null)
            {
                return NotFound();
            }

            db.PackSlipheads.Remove(packSliphead);
            db.SaveChanges();

            return Ok(packSliphead);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PackSlipheadExists(int id)
        {
            return db.PackSlipheads.Count(e => e.SlipId == id) > 0;
        }
    }
}