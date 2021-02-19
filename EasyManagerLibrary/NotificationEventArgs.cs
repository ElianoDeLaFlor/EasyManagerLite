using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class NotificationEventArgs:EventArgs
    {
        public Notifications Notification { get; private set; }

        public NotificationEventArgs(Notifications notification)
        {
            Notification = notification;
        }
    }
}
