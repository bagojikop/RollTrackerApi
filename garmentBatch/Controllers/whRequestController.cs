using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using garmentBatch.classes;
using garmentBatch.Models;
using Microsoft.Ajax.Utilities;
using static garmentBatch.Controllers.whRequestController;

namespace garmentBatch.Controllers
{
    public class whRequestController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();
        respayload rtn = new respayload();


        [HttpGet]
        // GET: api/whRequest
        public IHttpActionResult GetRequests()
        {
            rtn.data= db.PackSlipReqheads.Include(c => c.product).Include(i=>i.sales001_01).Where(c => c.accept == false && (c.isOrderComplete == false || c.isOrderComplete == null)).ToList();

            return Ok(rtn.data);
        }
         
        public IHttpActionResult getSumSelectedmtrs(int req_id)
        {
            var mtrs = db.PackSlipReqdetls.Where(w => w.req_id == req_id).Sum(s => (int?)s.mtr);
            mtrs = mtrs ?? 0;
            return Ok(mtrs);

        }

        // GET: api/whRequest/5
        [ResponseType(typeof(PackSlipReqhead))]
        public IHttpActionResult GetPackSlipReqhead(int id)
        {
            PackSlipReqhead packSlipReqhead = db.PackSlipReqheads.Find(id);
            if (packSlipReqhead == null)
            {
                return NotFound();
            }

            return Ok(packSlipReqhead);
        }

        // PUT: api/whRequest/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPackSlipReqhead(int id, PackSlipReqhead packSlipReqhead)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != packSlipReqhead.req_id)
            {
                return BadRequest();
            }

            db.Entry(packSlipReqhead).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackSlipReqheadExists(id))
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

        // POST: api/whRequest
        [ResponseType(typeof(PackSlipReqhead))]
        public IHttpActionResult PostPackSlipReqhead(PackSlipReqhead packSlipReqhead)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PackSlipReqheads.Add(packSlipReqhead);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PackSlipReqheadExists(packSlipReqhead.req_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = packSlipReqhead.req_id }, packSlipReqhead);
        }

        // DELETE: api/whRequest/5
        [ResponseType(typeof(PackSlipReqhead))]
        public IHttpActionResult DeletePackSlipReqhead(int id)
        {
            PackSlipReqhead packSlipReqhead = db.PackSlipReqheads.Find(id);
            if (packSlipReqhead == null)
            {
                return NotFound();
            }

            db.PackSlipReqheads.Remove(packSlipReqhead);
            db.SaveChanges();

            return Ok(packSlipReqhead);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PackSlipReqheadExists(int id)
        {
            return db.PackSlipReqheads.Count(e => e.req_id == id) > 0;
        }
    }
}