using System;

namespace EasyManagerLibrary
{
    public class BackupInfo
    {
        public int Id { get; set; }
        public string Dir { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastBackupDate { get; set; }
    }
}