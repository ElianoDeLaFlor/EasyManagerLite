using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        public EasyManagerLibrary.Client client { get; set; }
        public int ClientId { get; set; }
        public bool IsUpdate { get; set; }
        public Client()
        {
            InitializeComponent();
            ManagerTable("ClientType", "INTEGER", "0");
        }

        public Client(int id)
        {
            InitializeComponent();
            ClientId = id;
            IsUpdate = true;
            ManagerTable("ClientType", "INTEGER", "0");
        }

        public void ManagerTable(string columnName, string typ, string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<EasyManagerLibrary.Client>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<EasyManagerLibrary.Client>(columnName, typ, defaultvalue);
            }

        }

        private void Client_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string nom = txtlabel.Text.ToLower();
            string prenom = txtprenom.Text.ToLower();
            string contact = txtcontact.Text;

            if (InfoChecker.IsEmpty(nom) || InfoChecker.IsEmpty(prenom))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (nom.Length <= 2 || prenom.Length <= 2)
            {
                MessageBox.Show(Properties.Resources.ShortEnter, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (!IsUpdate)
                if (IsClientExist(nom, prenom))
                {
                    MessageBox.Show(Properties.Resources.UserExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

            if (!InfoChecker.IsEmpty(contact))
            {
                if (contact.Length < 8 || contact.Length > 8)
                {
                    MessageBox.Show(Properties.Resources.ContactError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            GetClient(nom, prenom, contact);
            if(IsUpdate)
            {
                client.Id = ClientId;
                // Update
                if (DbManager.UpDate<EasyManagerLibrary.Client>(client,ClientId))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    Reset();
                    Close();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //Insertion
                if (DbManager.Save(client))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    Reset();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void GetClient(string nom, string prenom, string contact)
        {
            client = new EasyManagerLibrary.Client();
            client.Nom = nom;
            client.Prenom = prenom;
            client.Contact = contact;
            client.ClientType = cbclienttype.SelectedIndex == 0 ? ClientType.ClientSimple : ClientType.ClientGrossiste;
        }

        private bool IsClientExist(string nom, string prenom)
        {
            return DbManager.GetClientByLastNameName(nom, prenom) != null;
        }

        private void Reset()
        {
            txtlabel.Text = string.Empty;
            txtprenom.Text = string.Empty;
            txtcontact.Text = string.Empty;
            cbclienttype.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void txtcontact_KeyUp(object sender, KeyEventArgs e)
        {
            txtcontact.Text = InfoChecker.ContactRegExp(txtcontact.Text);
            txtcontact.SelectionStart = txtcontact.Text.Length;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsUpdate)
            {
                Titre.Text = Properties.Resources.EditionClient;
                Fill(GetClient(ClientId));
            }
            else
                Titre.Text = Properties.Resources.ClientTitle;

        }

        private void PerformClick(Button btn)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(btn);
            IInvokeProvider invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider.Invoke();
        }

        private void Messagee_Box(string msg, string actiontitle, SolidColorBrush colorBrush)
        {
            MsgContent.Content = msg;
            MsgContent.Foreground = colorBrush;
            MsgContent.FontSize = 20;
            btnOk.Content = actiontitle;
            PerformClick(btnmsgbox);
        }

        private EasyManagerLibrary.Client GetClient(int id)
        {
            var client = DbManager.GetById<EasyManagerLibrary.Client>(id);
            return client;
        }


        private void Fill(EasyManagerLibrary.Client client)
        {
            txtlabel.Text = client.Nom;
            txtprenom.Text = client.Prenom;
            txtcontact.Text = client.Contact;
            cbclienttype.SelectedIndex = client.ClientType == ClientType.ClientSimple?0:1;
            
        }
    }
}
