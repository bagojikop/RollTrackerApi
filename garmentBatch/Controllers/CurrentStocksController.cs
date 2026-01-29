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
using System.Web.UI.WebControls;
using garmentBatch.Models;

namespace garmentBatch.Controllers
{
    public class CurrentStocksController : ApiController
    {
        private GarmentBatchEntities db = new GarmentBatchEntities();
        respayload rtn = new respayload();
        // GET: api/CurrentStocks


        [HttpPost]
        public async Task<IHttpActionResult> GetCurrentStocks([FromBody] QueryStringParameters page)
        {
            try
            {
                var filter = new EntityFrameworkFilter<CurrentStock>();


                var data = filter.Filter(db.CurrentStocks,page.keys);

                rtn.data = await data.OrderBy(o => o.finyear).Skip((page.PageNumber - 1) * page.PageSize)
                           .Take(page.PageSize).ToListAsync();
                
                if (page.PageNumber == 1)
                    rtn.PageDetails = PageDetail<CurrentStock>.ToPagedList(data, page.PageNumber, page.PageSize);
            }
            catch (Exception ex)
            {
                rtn.status_cd = 0;
                rtn.error.exception = ex;
            }


            return Ok(rtn);
        }

        // GET: api/CurrentStocks/5
        [ResponseType(typeof(CurrentStock))]
        public   IHttpActionResult GetCurrentStock(string finyear, string loomNo, int BatchNo)
        {
            CurrentStock currentStock = db.CurrentStocks.Where(c => c.finyear == finyear && c.loomNo == loomNo && c.BatchNo == BatchNo).FirstOrDefault();

            if (currentStock == null)
            {
                return NotFound();
            }

            return Ok(currentStock);
        }

        // PUT: api/CurrentStocks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCurrentStock(string finyear, string loomNo, int BatchNo, CurrentStock currentStock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (finyear != currentStock.finyear || loomNo != currentStock.loomNo || BatchNo ==currentStock.BatchNo )
            {
                return BadRequest();
            }

            db.Entry(currentStock).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrentStockExists(finyear ,loomNo ,BatchNo ))
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

        // POST: api/CurrentStocks
        [ResponseType(typeof(CurrentStock))]
        public async Task<IHttpActionResult> PostCurrentStock(CurrentStock currentStock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CurrentStocks.Add(currentStock);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CurrentStockExists(currentStock.finyear,currentStock.loomNo ,currentStock.BatchNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = currentStock.finyear }, currentStock);
        }

        // DELETE: api/CurrentStocks/5
        [ResponseType(typeof(CurrentStock))]
        public async Task<IHttpActionResult> DeleteCurrentStock(string finyear,string loomNo,int BatchNo)
        {
            CurrentStock currentStock = db.CurrentStocks.Where(c => c.finyear == finyear && c.loomNo == loomNo && c.BatchNo == BatchNo).FirstOrDefault();
            if (currentStock == null)
            {
                return NotFound();
            }

            db.CurrentStocks.Remove(currentStock);
            await db.SaveChangesAsync();

            return Ok(currentStock);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CurrentStockExists(string finyear, string loomNo, int BatchNo)
        {
            return db.CurrentStocks.Count(e => e.finyear == finyear && e.loomNo ==loomNo && e.BatchNo ==BatchNo ) > 0;
        }
    }
}