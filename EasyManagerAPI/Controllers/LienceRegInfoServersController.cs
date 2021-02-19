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
    [RoutePrefix("api/v1/LienceRegInfoServers")]
    public class LienceRegInfoServersController : ApiController
    {
        private EasyManagerDbContext db = new EasyManagerDbContext();

        // GET: api/v1/LienceRegInfoServers
        [HttpGet]
        [Route("")]
        public List<LienceRegInfoServer> GetLienceRegInfoServers()
        {
            return db.LienceRegInfoServers.ToList();
        }

        // GET: api/v1/LienceRegInfoServers/5
        [ResponseType(typeof(LienceRegInfoServer))]
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetLienceRegInfoServer(int id)
        {
            LienceRegInfoServer lienceRegInfoServer = await db.LienceRegInfoServers.FindAsync(id);
            if (lienceRegInfoServer == null)
            {
                return NotFound();
            }

            return Ok(lienceRegInfoServer);
        }

        // GET: api/v1/LienceRegInfoServers/appkey/5
        [ResponseType(typeof(LienceRegInfoServer))]
        [HttpGet]
        [Route("Appkey/{appkey}")]
        public async Task<IHttpActionResult> GetLienceRegInfoServer(Guid appkey)
        {
            LienceRegInfoServer lienceRegInfoServer = await (from item in db.LienceRegInfoServers where item.AppKey == appkey select item).SingleOrDefaultAsync();
            if (lienceRegInfoServer == null)
            {
                return NotFound();
            }

            return Ok(lienceRegInfoServer);
        }

        // PUT: api/v1/LienceRegInfoServers/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutLienceRegInfoServer(int id, LienceRegInfoServer lienceRegInfoServer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lienceRegInfoServer.Id)
            {
                return BadRequest();
            }

            db.Entry(lienceRegInfoServer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LienceRegInfoServerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(lienceRegInfoServer);
        }

        // POST: api/v1/LienceRegInfoServers
        [ResponseType(typeof(LienceRegInfoServer))]
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostLienceRegInfoServer(LienceRegInfoServer lienceRegInfoServer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LienceRegInfoServers.Add(lienceRegInfoServer);
            await db.SaveChangesAsync();

            return Ok(lienceRegInfoServer);
        }

        // DELETE: api/v1/LienceRegInfoServers/5
        [ResponseType(typeof(LienceRegInfoServer))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteLienceRegInfoServer(int id)
        {
            LienceRegInfoServer lienceRegInfoServer = await db.LienceRegInfoServers.FindAsync(id);
            if (lienceRegInfoServer == null)
            {
                return NotFound();
            }

            db.LienceRegInfoServers.Remove(lienceRegInfoServer);
            await db.SaveChangesAsync();

            return Ok(lienceRegInfoServer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LienceRegInfoServerExists(int id)
        {
            return db.LienceRegInfoServers.Count(e => e.Id == id) > 0;
        }
    }
}