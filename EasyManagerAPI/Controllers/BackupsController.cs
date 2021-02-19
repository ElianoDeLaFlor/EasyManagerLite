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
    [RoutePrefix("api/v1/Backups")]
    public class BackupsController : ApiController
    {
        private EasyManagerDbContext db = new EasyManagerDbContext();

        // GET: api/v1/Backups
        [Route("")]
        [HttpGet]
        public List<Backup> GetBackups()
        {
            return db.Backups.ToList();
        }

        // GET: api/v1/Backups/5
        [ResponseType(typeof(Backup))]
        [Route("{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBackup(Guid id)
        {
            Backup backup = await db.Backups.FindAsync(id);
            if (backup == null)
            {
                return NotFound();
            }

            return Ok(backup);
        }

        private async Task<Backup> GetBackupById(Guid id)
        {
            Backup welcomImage = await db.Backups.FindAsync(id);
            if (welcomImage == null)
            {
                return null;
            }

            return (welcomImage);
        }

        private async Task<Backup> GetBackupByAppUserId(Guid id)
        {
            var bup = await (from item in db.Backups where item.AppUserInfoId == id select item).ToListAsync();
            if (bup.Count == 0)
            {
                return null;
            }

            return (bup.LastOrDefault());
        }

        // PUT: api/v1/Backups/5
        [ResponseType(typeof(void))]
        [Route("{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutBackup(Guid id, Backup backup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != backup.Id)
            {
                return BadRequest();
            }

            db.Entry(backup).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BackupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(backup);
        }

        // PUT: api/v1/Backups/update/00000000-0000-0000-0000-000000000000
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("update/{id}")]
        public async Task<IHttpActionResult> PutBackupfrm(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                string location;
                //load welcomimage from database
                var bup = await GetBackupByAppUserId(id);
                if (bup == null)
                {
                    return await PostBackupFrm();
                }
                //Check if the request contains multipart/form-data
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return BadRequest();
                }
                string loc = HttpContext.Current.Server.MapPath("~/BackupFiles");
                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                try
                {
                    //read the form data
                    await Request.Content.ReadAsMultipartAsync(provider);
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    //Show all they key-value pairs
                    foreach (var key in provider.FormData.AllKeys)
                    {
                        foreach (var val in provider.FormData.GetValues(key))
                        {
                            //Add key value pair to the dictionary
                            dic.Add(key, val);
                        }
                    }



                    //Gets files from the provider and save them to a folder.
                    foreach (var item in provider.FileData)
                    {
                        var key = item.Headers.ContentDisposition.Name.Trim('"');
                        var value = item.Headers.ContentDisposition.FileName.Trim('"');
                        if (key == "BackupFileName" && !string.IsNullOrEmpty(value))
                        {
                            //delete the existing file
                            Servante.Delete(HttpContext.Current.Server.MapPath($"~/BackupFiles/{bup.BackupFileName}"));
                            var localfilename = item.LocalFileName;
                            var savelocation = Servante.FileSavePath(loc, value, out string saveloc);
                            location = saveloc;
                            File.Move(localfilename, savelocation);
                            bup.BackupFileName = location;
                        }
                        else
                        {
                            Servante.Delete(item.LocalFileName);
                            dic.Add(key, value);
                        }
                    }

                    if (dic.ContainsKey("AppUserInfoId"))
                        bup.AppUserInfoId = new Guid(dic["AppUserInfoId"]);
                    db.Entry(bup).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return Ok(bup);
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }

            }
        }

        // POST: api/v1/Backups
        [ResponseType(typeof(Backup))]
        public async Task<IHttpActionResult> PostBackup(Backup backup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Backups.Add(backup);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BackupExists(backup.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(backup);
        }

        [ResponseType(typeof(Backup))]
        [HttpPost]
        [Route("frm")]
        // POST: api/v1/Backups/frm
        public async Task<IHttpActionResult> PostBackupFrm()
        {
            //Check if the request contains multipart/form-data
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest();
            }
            Servante.CheckPath(HttpContext.Current.Server.MapPath("~/BackupFiles"));
            string loc = HttpContext.Current.Server.MapPath("~/BackupFiles");
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            try
            {
                //read the form data
                await Request.Content.ReadAsMultipartAsync(provider);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                //Show all they key-value pairs
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        //Add key value pair to the dictionary
                        dic.Add(key, val);
                    }
                }
                Backup bup = new Backup();
                //Gets files from the provider and save them to a folder.
                foreach (var item in provider.FileData)
                {

                    var key = item.Headers.ContentDisposition.Name.Trim('"');
                    var value = item.Headers.ContentDisposition.FileName.Trim('"');
                    if (key == "BackupFileName" && !string.IsNullOrEmpty(value))
                    {
                        var localfilename = item.LocalFileName;
                        var savelocation = Servante.FileSavePath(loc, value, out string saveloc);
                        dic.Add(key, saveloc);
                        File.Move(localfilename, savelocation);
                        bup.BackupFileName = saveloc;
                    }
                    else
                    {
                        dic.Add(key, value);
                    }

                }

                if (dic.ContainsKey("AppUserInfoId"))
                    bup.AppUserInfoId = new Guid(dic["AppUserInfoId"]);
                
                    bup.Id = Guid.NewGuid();
                db.Backups.Add(bup);
                await db.SaveChangesAsync();
                return Ok(bup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/v1/Backups/5
        [ResponseType(typeof(Backup))]
        [Route("{id}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteBackup(Guid id)
        {
            Backup backup = await db.Backups.FindAsync(id);
            if (backup == null)
            {
                return NotFound();
            }

            db.Backups.Remove(backup);
            await db.SaveChangesAsync();

            return Ok(backup);
        }

        //GET:api/v1/Backups/Download/5
        /// <summary>
        /// Downloads file
        /// </summary>
        /// <param nom="path"></param>
        [HttpGet]
        [Route("Download/{id}")]
        public async Task<HttpResponseMessage> DownloadById(Guid id)
        {
            var wimg = await GetBackupById(id);
            if (wimg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            string path = HttpContext.Current.Server.MapPath($"~/BackupFiles/{wimg.BackupFileName}");
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

        //GET:api/v1/Backups/Download/5
        /// <summary>
        /// Downloads file
        /// </summary>
        /// <param nom="path"></param>
        [HttpGet]
        [Route("Download/AppUser/{id}")]
        public async Task<HttpResponseMessage> DownloadByAppUserId(Guid id)
        {
            var wimg = await GetBackupByAppUserId(id);
            if (wimg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            string path = HttpContext.Current.Server.MapPath($"~/BackupFiles/{wimg.BackupFileName}");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BackupExists(Guid id)
        {
            return db.Backups.Count(e => e.Id == id) > 0;
        }
    }
}