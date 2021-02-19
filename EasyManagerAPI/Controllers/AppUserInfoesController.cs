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
    [RoutePrefix("api/v1/AppUserInfoes")]
    public class AppUserInfoesController : ApiController
    {
        private EasyManagerDbContext db = new EasyManagerDbContext();

        // GET: api/v1/AppUserInfoes
        [HttpGet]
        [Route("")]
        public List<AppUserInfo> GetAppUserInfoes()
        {
            return db.AppUserInfoes.ToList();
        }

        // GET: api/v1/AppUserInfoes/5
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(AppUserInfo))]
        public async Task<IHttpActionResult> GetAppUserInfo(Guid id)
        {
            AppUserInfo appUserInfo = await db.AppUserInfoes.FindAsync(id);
            if (appUserInfo == null)
            {
                return NotFound();
            }

            return Ok(appUserInfo);
        }

        // PUT: api/v1/AppUserInfoes/5
        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAppUserInfo(Guid id, AppUserInfo appUserInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != appUserInfo.Id)
            {
                return BadRequest();
            }

            db.Entry(appUserInfo).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppUserInfoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(appUserInfo);
        }

        // POST: api/v1/AppUserInfoes
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(AppUserInfo))]
        public async Task<IHttpActionResult> PostAppUserInfo(AppUserInfo appUserInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AppUserInfoes.Add(appUserInfo);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AppUserInfoExists(appUserInfo.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(appUserInfo);
        }

        // DELETE: api/v1/AppUserInfoes/5
        [HttpDelete]
        [Route("{id}")]
        [ResponseType(typeof(AppUserInfo))]
        public async Task<IHttpActionResult> DeleteAppUserInfo(Guid id)
        {
            AppUserInfo appUserInfo = await db.AppUserInfoes.FindAsync(id);
            if (appUserInfo == null)
            {
                return NotFound();
            }

            db.AppUserInfoes.Remove(appUserInfo);
            await db.SaveChangesAsync();

            return Ok(appUserInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AppUserInfoExists(Guid id)
        {
            return db.AppUserInfoes.Count(e => e.Id == id) > 0;
        }
    }
}