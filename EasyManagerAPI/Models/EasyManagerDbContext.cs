using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyManagerAPI.Models
{
    public class EasyManagerDbContext:DbContext
    {
        public EasyManagerDbContext() : base("EasyManagerDb") { }

        public virtual DbSet<LicenceInformation> LicenceInformations { get; set; }
        public virtual DbSet<LienceRegInfoServer> LienceRegInfoServers { get; set; }
        public virtual DbSet<AppUserInfo> AppUserInfoes { get; set; }
        public virtual DbSet<Backup> Backups { get; set; }
        public virtual DbSet<AppVersion> AppVersions { get; set; }
    }
}