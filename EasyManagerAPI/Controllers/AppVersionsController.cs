using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using EasyManagerAPI.Models;

namespace EasyManagerAPI.Controllers
{
    [RoutePrefix("api/v1/AppVersions")]
    public class AppVersionsController : ApiController
    {
        private EasyManagerDbContext db = new EasyManagerDbContext();

        // GET: api/v1/AppVersions
        [HttpGet]
        [Route("")]
        public List<AppVersion> GetAppVersions()
        {
            return db.AppVersions.ToList();
        }

        // GET: api/v1/AppVersions/5
        [ResponseType(typeof(AppVersion))]
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetAppVersion(int id)
        {
            AppVersion appVersion = await db.AppVersions.FindAsync(id);
            if (appVersion == null)
            {
                return NotFound();
            }

            return Ok(appVersion);
        }

        public async Task<AppVersion> GetLastestAppVersion()
        {
            AppVersion appVersion = await (from item in db.AppVersions orderby item.Id descending select item).FirstOrDefaultAsync();
            
            return appVersion;
        }

        // PUT: api/v1/AppVersions/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> PutAppVersion(int id, AppVersion appVersion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != appVersion.Id)
            {
                return BadRequest();
            }

            db.Entry(appVersion).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppVersionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(appVersion);
        }

        // POST: api/v1/AppVersions
        [ResponseType(typeof(AppVersion))]
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostAppVersion(AppVersion appVersion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            appVersion.CreationDate = DateTime.UtcNow;
            db.AppVersions.Add(appVersion);
            await db.SaveChangesAsync();

            return Ok(appVersion);
        }

        // POST: api/v1/AppVersions
        [ResponseType(typeof(AppVersion))]
        [HttpPost]
        [Route("CheckUpdate/{appkey}")]
        public async Task<IHttpActionResult> IsthereAnUpdate(Guid appkey,AppVersion appVersion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var lastversion = await GetLastestAppVersion();
            LicenceInformationController licenceInformationController = new LicenceInformationController();
            //var licenceinfo = await licenceInformationController.GetLastestLicenceInformationByAppKey(appkey);
            var rslt = Servante.CompareAppVersion(lastversion, appVersion)==1;
            

            return Ok(rslt);
        }

        //GET:api/v1/AppVersions/Download
        [HttpGet]
        [Route("Download")]
        public async Task<HttpResponseMessage> DownloadUpdate()
        {
            string path = HttpContext.Current.Server.MapPath($"~/UpdateFile/EasyManagerSetup.msi");
            FileInfo fi = new FileInfo(path);
            if (!File.Exists(path))
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            var dataStream = new MemoryStream(Servante.DownloadImage(path));
            HttpResponseMessage HttpRM = new HttpResponseMessage();
            HttpRM.Content = new StreamContent(dataStream);
            HttpRM.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            HttpRM.Content.Headers.ContentDisposition.FileName = fi.Name;
            HttpRM.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            return await Task.FromResult(HttpRM);
        }

        // DELETE: api/v1/AppVersions/5
        [ResponseType(typeof(AppVersion))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAppVersion(int id)
        {
            AppVersion appVersion = await db.AppVersions.FindAsync(id);
            if (appVersion == null)
            {
                return NotFound();
            }

            db.AppVersions.Remove(appVersion);
            await db.SaveChangesAsync();

            return Ok(appVersion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AppVersionExists(int id)
        {
            return db.AppVersions.Count(e => e.Id == id) > 0;
        }
    }
}