using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class Notifications
    {
        public event EventHandler<NotificationEventArgs> NoticationEvent;
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ProduitNom { get; set; }
        public string Message { get; set; }
        public string Couleur { get; set; }
        public bool IsApprovisionnement { get; set; }
        
        private bool _save;

        private double _produitQuantiteRestante;
        public double ProduitQuantiteRestante 
        {
            get { return _produitQuantiteRestante; }
            set 
            {
                _produitQuantiteRestante = value;
                NoticationEvent?.Invoke(this, new NotificationEventArgs(this));
            }
        }

        [Write(false)]
        public bool Save
        {
            get { return _save; }
            set { _save = value; NoticationEvent?.Invoke(this, new NotificationEventArgs(this)); }
        }
    }
}
