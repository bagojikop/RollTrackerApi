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
using garmentBatch.Models;

namespace garmentBatch.Controllers
{
    public class packSlipImageController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();
 
        // GET: api/packSlip_002/5
        [ResponseType(typeof(packSlip_002))]
        public async Task<IHttpActionResult> Get(int id)
        {
            packSlip_002 packSlip_002 = await db.packSlip_002.FindAsync(id);
            if (packSlip_002 == null)
            {
                return NotFound();
            }

            return Ok(packSlip_002);
        }

      
        // POST: api/packSlip_002
        [ResponseType(typeof(packSlip_002))]
        public async Task<IHttpActionResult> PostImage(packSlip_002 packSlip_002)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           var existImage = await db.packSlip_002.FindAsync(packSlip_002.SlipId);
            if (existImage != null)
            {
                db.packSlip_002.Remove(existImage);
            }

            db.packSlip_002.Add(packSlip_002);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (packSlip_002Exists(packSlip_002.SlipId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = packSlip_002.SlipId }, packSlip_002);
        }



        


        // PUT: api/whOutRoll/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPackSlip(long id, packSlip_002 packSlipReqdetl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != packSlipReqdetl.SlipId)
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
                if (!packSlip_002Exists(packSlipReqdetl.SlipId))
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


        private bool packSlip_002Exists(int id)
        {
            return db.packSlip_002.Count(e => e.SlipId == id) > 0;
        }
    }
}