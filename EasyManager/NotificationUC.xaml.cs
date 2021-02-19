using EasyManagerDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for NotificationUC.xaml
    /// </summary>
    public partial class NotificationUC : UserControl
    {
        public string ProductName { get; set; }
        public string Messages { get; set; }
        public double LeftQuanity { get; set; }
        public DateTime Date { get; set; }
        public string StringDate { get; set; }
        public int NotificationId { get; set; }
        public string Colors { get; set; }
        public NotificationUI NotificationUI { get; set; }
        public NotificationUC()
        {
            InitializeComponent();
            DataContext = this;
        }
        public NotificationUC(NotificationUI notificationUI)
        {
            InitializeComponent();
            NotificationUI = notificationUI;
            DataContext = this;
        }

        private bool Delete(int id)
        {
            return DbManager.Delete<EasyManagerLibrary.Notifications>(id);
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (Delete(NotificationId))
                NotificationUI.Reload();
        }
    }
}
