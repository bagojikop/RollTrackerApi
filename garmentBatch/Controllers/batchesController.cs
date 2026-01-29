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
using System.Data.Entity.Core.Objects;

namespace garmentBatch.Controllers
{
    public class batchesController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();

        // GET: api/batches
        public IHttpActionResult GetUpdatePndBatches()
        {
            var batches = db.Batches.Where(c => (c.status == 0 || c.status == 1) && c.transtype == 0)
                                .Select(c => new batchItems
                                {
                                    finYear = c.finYear,
                                    BatchNo = c.BatchNo,
                                    loomNo = c.loomNo,
                                    BatchCreated = c.BatchCreated,
                                    status = c.status,
                                    statusname = c.status == 1 ? "Processing" : "Pending"
                                }).ToList();

            return Ok(batches);
        }
        public IHttpActionResult GetUpdatedBatches(DateTime from ,DateTime to)
        {
            var batches = db.Batches.Include(c=>c.PackSlipDetls.Select(s=>s.PackSliphead)).Include(c=>c.mst014).Include(c=>c.Quality.product)
                .Include(c=>c.Inspector)
               .Where(c => c.transtype == 0 && EntityFunctions.TruncateTime(c.inspectedDate) >= from.Date && EntityFunctions.TruncateTime(c.inspectedDate) <= to.Date)
                      .Select(c => new batchItems
                      {
                          finYear = c.finYear,
                          BatchNo = c.BatchNo,
                          loomNo = c.loomNo,
                          BatchCreated = c.BatchCreated,
                          inspectedby = c.Inspector.inspectorName,
                          inspectedDate = c.inspectedDate,
                          product = c.Quality.product.prodName,
                          QcCode = c.QcCode,
                          godown = c.mst014.L_Name,
                          stackNo = c.stacksId,
                          slipNo = c.PackSlipDetls.FirstOrDefault().PackSliphead.SlipNo,
                          slipDate = c.PackSlipDetls.FirstOrDefault().PackSliphead.SlipDate,
                          custname = c.PackSlipDetls.FirstOrDefault().PackSliphead.mst011.Acc_name,
                          location = c.PackSlipDetls.FirstOrDefault().PackSliphead.mst011.mst011_01.mst006.City_name,
                          mtrs = c.Mtrs,
                          weight = c.Weight


                      }).OrderBy(c => c.loomNo).ToList();

            return Ok(batches);
        }


        // GET: api/batches/5
        [HttpGet]
        public IHttpActionResult GetBatch(string finyear,string loomno,int batchno)
        
        {
          var batch=  db.Batches.Include(s=>s.Quality.product).Where(c =>  c.finYear == finyear && c.loomNo == loomno && c.BatchNo == batchno
           ).FirstOrDefault();

          
         

            if (batch == null)
            {
                return NotFound();
            }

            return Ok(batch);
        }

        // PUT: api/batches/5
        [HttpPut]
        public IHttpActionResult Put(string finyear, string loomno, int batchno, batch batch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (finyear != batch.finYear || loomno!=batch.loomNo|| batchno!=batch.BatchNo)
            {
                return BadRequest();
            }

            db.Entry(batch).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            
            catch (DbUpdateConcurrencyException)
            {
                if (!BatchExists(batch.finYear, batch.loomNo, batch.BatchNo))
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

        // POST: api/batches
        [HttpPost]
        public IHttpActionResult Post(Batch batch)
        {

           var finYear = batch.BatchCreated.Month >= 4 ? batch.BatchCreated.Year.ToString() + (batch.BatchCreated.Year + 1).ToString() : (batch.BatchCreated.Year - 1).ToString() + batch.BatchCreated.Year.ToString();

            var loomNo = batch.loomNo;

            var batchNo = db.Batches.Where(c => c.finYear == finYear && c.loomNo == loomNo).Max(c => (int?)c.BatchNo);

            batch.BatchNo = (batchNo == null ? 1 : ((int)batchNo) + 1);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Batches.Add(batch);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (BatchExists(batch.finYear,batch.loomNo,batch.BatchNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = batch.finYear }, batch);
        }

        // DELETE: api/batches/5
        [HttpDelete]
        public IHttpActionResult Delete(string finyear, string loomno, int batchno)
        {
            var batch = db.Batches.Where(c => c.finYear == finyear && c.loomNo == loomno && c.BatchNo == batchno
              ).FirstOrDefault();

            if (batch == null)
            {
                return NotFound();
            }

            db.Batches.Remove(batch);
            db.SaveChanges();

            return Ok(batch);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BatchExists(string finyear, string loomno, int batchno)
        {
            return db.Batches.Count(e => e.finYear == finyear && e.loomNo==loomno&& e.BatchNo==batchno) > 0;
        }
    }
}