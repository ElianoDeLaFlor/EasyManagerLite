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
using EasyManagerAPI.Models;

namespace EasyManagerAPI.Controllers
{
    [RoutePrefix("api/v1/LicenceInformation")]
    public class LicenceInformationController : ApiController
    {
        private EasyManagerDbContext db = new EasyManagerDbContext();

        // GET: api/v1/LicenceInformation
        [HttpGet]
        [Route("")]
        public List<LicenceInformation> GetLicenceInformations()
        {
            return db.LicenceInformations.ToList();
        }

        // GET: api/v1/LicenceInformation/5
        [ResponseType(typeof(LicenceInformation))]
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetLicenceInformation(int id)
        {
            LicenceInformation licenceInformation = await db.LicenceInformations.FindAsync(id);
            if (licenceInformation == null)
            {
                return NotFound();
            }

            return Ok(licenceInformation);
        }

        // GET: api/v1/LicenceInformation/appkey/5
        [ResponseType(typeof(LicenceInformation))]
        [HttpGet]
        [Route("Appkey/{appkey}")]
        public async Task<IHttpActionResult> GetLicenceInformationByAppKey(Guid appkey)
        {
            List<LicenceInformation> licenceInformation = await (from item in db.LicenceInformations where item.AppKey==appkey select item).ToListAsync();
            return Ok(licenceInformation);
        }

        public async Task<LicenceInformation> GetLastestLicenceInformationByAppKey(Guid appkey)
        {
            List<LicenceInformation> licenceInformation = await (from item in db.LicenceInformations orderby item.Id descending where item.AppKey == appkey select item).ToListAsync();


            return licenceInformation.FirstOrDefault();
        }

        // GET: api/v1/LicenceInformation/Code/5
        [ResponseType(typeof(LicenceInformation))]
        [HttpPost]
        [Route("Code")]
        public async Task<IHttpActionResult> GetLicenceInformationByCode(LicenceCode licenceCode)
        {
            LicenceInformation licenceInformation = await (from item in db.LicenceInformations where item.Code == licenceCode.Code select item).SingleOrDefaultAsync();
            if (licenceInformation == null)
            {
                return NotFound();
            }

            return Ok(licenceInformation);
        }

        // PUT: api/v1/LicenceInformation/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutLicenceInformation(int id, LicenceInformation licenceInformation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != licenceInformation.Id)
            {
                return BadRequest();
            }

            db.Entry(licenceInformation).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LicenceInformationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(licenceInformation);
        }

        // POST: api/v1/LicenceInformation
        [ResponseType(typeof(LicenceInformation))]
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostLicenceInformation(LicenceInformation licenceInformation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LicenceInformations.Add(licenceInformation);
            await db.SaveChangesAsync();

            return Ok(licenceInformation);
        }

        // DELETE: api/v1/LicenceInformation/5
        [ResponseType(typeof(LicenceInformation))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteLicenceInformation(int id)
        {
            LicenceInformation licenceInformation = await db.LicenceInformations.FindAsync(id);
            if (licenceInformation == null)
            {
                return NotFound();
            }

            db.LicenceInformations.Remove(licenceInformation);
            await db.SaveChangesAsync();

            return Ok(licenceInformation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LicenceInformationExists(int id)
        {
            return db.LicenceInformations.Count(e => e.Id == id) > 0;
        }
    }
}