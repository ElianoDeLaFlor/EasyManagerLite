using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using EasyManager.MenuItems;
using Licence;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Connexion.xaml
    /// </summary>
    public partial class ConnexionUC : UserControl
    {
        private bool Display { get; set; } = true;
        private string langcode { get; set; } = "fr";
        public Home GetHome { get; set; }

        public Utilisateur GetUtilisateur { get; set; } = new Utilisateur();
        public ConnexionUC()
        {
            InitializeComponent();
        }

        public ConnexionUC(Home h)
        {
                InitializeComponent();
                GetHome = h;
        }



        private void HideInformation()
        {
            Task.Factory.StartNew(() =>
            {         
                Thread.Sleep(3500);
            }).ContinueWith(t =>
            {
                msg.IsActive = false;
                Display = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        private void Message(string str)
        {
            if (Display)
            {
                snackbar.Content = str;
                msg.IsActive = true;
                Display = false;
            }
            HideInformation();
        }
        
        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckFields())
            {
                //we're good
                GetUtilisateur = DbManager.GetUserByLogin(txtlogin.Text.ToLower());
                if (GetUtilisateur != null)
                {
                    if (IsPassCorrect(GetUtilisateur, txtpasse.Password))
                    {
                        if (IsOkPasseDate(GetUtilisateur))
                        {
                            if (GetUtilisateur.Deleted == true)
                            {
                                GetHome.MainSnackbar.MessageQueue.Enqueue(Properties.Resources.AccountDeleted);
                                return;
                            }
                            GetHome.MenuToggleButton.Visibility = Visibility.Visible;
                            var datacontext = GetHome.DataContextt;
                            datacontext.ConnectedUser = GetUtilisateur;
                            datacontext.IsConnected = Visibility.Visible;

                            //Get user role
                            var userrole = DbManager.GetByColumnName<Role>("Libelle", GetUtilisateur.RoleLibelle);
                            // Get modules that user can access
                            var lst = UserMenus(userrole[0].Id);

                            foreach (var item in lst)
                            {
                                //TableauDeBord|Categorie|Client|Compte|Discount|Produit|EditionQuantite|Vente|VenteCredit|ListeCatégorie|ListeClient|ListeUtilisateur|ListeDiscount|ListeProduit|ListeEditionQuantite|ListeVente|ListeVenteCredit||Role|AttributionRole
                                if (item == Properties.Resources.ModuleCategorie)
                                {
                                    MenuContent menu = new MenuContent("Hexagons", Properties.Resources.CategoryTitle, item, new CategorieUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleClient)
                                {
                                    MenuContent menu = new MenuContent("PersonTie", Properties.Resources.ClientTitle, item, new ClientUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleCompte)
                                {
                                    MenuContent menu = new MenuContent("AccountAdd", Properties.Resources.CompteTitle, item, new CompteUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleDiscount)
                                {
                                    MenuContent menu = new MenuContent("TrolleyMinus", Properties.Resources.Discount, item, new DiscountUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleProduit)
                                {
                                    MenuContent menu = new MenuContent("DatabasePlus", Properties.Resources.ProduitTitle, item, new ProduitUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleEditonQuantite)
                                {
                                    MenuContent menu = new MenuContent("DatabaseEdit", Properties.Resources.ModuleEditonQuantite, item, new EditionQuantiteUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleVente)
                                {
                                    MenuContent menu = new MenuContent("CashMultiple", Properties.Resources.ModuleVente, item, new VenteUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleVenteCredit)
                                {
                                    MenuContent menu = new MenuContent("CashRefund", Properties.Resources.VenteCreditTitle, item, new VenteCreditUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleListeCategorie)
                                {
                                    MenuContent menu = new MenuContent("Collage", Properties.Resources.ModuleListeRole, item, new CategorieListeUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleListClient)
                                {
                                    MenuContent menu = new MenuContent("AccountSupervisor", Properties.Resources.ModuleListClient, item, new ClientListeUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleDiscountList)
                                {
                                    MenuContent menu = new MenuContent("ViewSequential", Properties.Resources.ModuleDiscountList, item, new DiscountListUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleListeProduits)
                                {
                                    MenuContent menu = new MenuContent("AlphaPBox", Properties.Resources.ModuleListeProduits, item, new ProduitListUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleEditonQuantiteListe)
                                {
                                    MenuContent menu = new MenuContent("BusDoubleDecker", Properties.Resources.ModuleEditonQuantiteListe, item, new EditionQuantiteListeUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleListeVente)
                                {
                                    MenuContent menu = new MenuContent("CurrencyNgn", Properties.Resources.ModuleListeVente, item, new VenteListeUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleListeVenteCredit)
                                {
                                    MenuContent menu = new MenuContent("CreditCardRefund", Properties.Resources.ModuleListeVenteCredit, item, new VenteCreditListeUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleListeRole)
                                {
                                    MenuContent menu = new MenuContent("Cards", Properties.Resources.ModuleListeRole, item, new RoleListUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }
                                else if (item == Properties.Resources.ModuleListeUtilisateur)
                                {
                                    MenuContent menu = new MenuContent("AccountGroup", Properties.Resources.ModuleListeUtilisateur, item, new UserListUC(GetHome));
                                    datacontext.MenuContent = menu;
                                }

                            }
                            GetHome.DataContextt = datacontext;
                            GetHome.MenuItemsListBox.ItemsSource = null;

                            GetHome.MenuItemsListBox.ItemsSource = datacontext.MenuContents;
                            //GetHome.MenuItemsListBox.SelectedIndex = 0;
                            GetHome.DataContextt.MenuIndex = 0;
                        }
                        else
                            return;
                    }
                    else
                    {
                        GetHome.MainSnackbar.MessageQueue.Enqueue(Properties.Resources.WrongCredential);
                        btnPass4Get.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    GetHome.MainSnackbar.MessageQueue.Enqueue(Properties.Resources.WrongCredential);
                    btnPass4Get.Visibility = Visibility.Visible;
                }
            }
            else
            {
                //a field is empty
                GetHome.MainSnackbar.MessageQueue.Enqueue(Properties.Resources.EmptyField);
            }
        }

        private bool IsPassCorrect(Utilisateur utilisateur, string mdp)
        {
            return Equals(utilisateur.Password, InfoChecker.Encrypt(mdp));
        }

        private bool IsPassCorrecte(Utilisateur utilisateur, string mdp)
        {
            return Equals(utilisateur.Password, InfoChecker.Encrypt(mdp));
        }


        private bool IsOkPasseDate(Utilisateur utilisateur)
        {
            //On détermine le jour restant pour que le passe expire
            var LeftDays = InfoChecker.DateDiff(utilisateur.PassDate, DateTime.Now);
            //Si le jour restant est >0 et <=5 on notifie l'utilisateur
            if (LeftDays > 0 && LeftDays <= 5)
            {
                double LD;
                if (LeftDays > 0 && LeftDays < 1)
                    LD = 0;
                else
                    LD = LeftDays;
                string msg = $"{Properties.Resources.PassExpireSoon} {LD} {Properties.Resources.Day}";
                if (MessageBox.Show(msg, Properties.Resources.MainTitle, MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // On demande à l'utilisateur s'il veut changer de mot de passe
                    PasseChange passeChange=new PasseChange(utilisateur);
                    passeChange.ShowDialog();
                }

                return true;
            }
            else if(LeftDays==0)
            {
                //Si le jour restant est==0 on notifie l'utilisateur et on arrête le processus de connexion
                if(MessageBox.Show(Properties.Resources.PassExpire,Properties.Resources.MainTitle,MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
                {
                    // On demande à l'utilisateur de changer de mot de passe
                    PasseChange passeChange=new PasseChange(utilisateur);
                    passeChange.ShowDialog();
                }

                return false;
            }
            else
                return true;

        }

        private bool CheckFields()
        {
            return string.IsNullOrWhiteSpace(txtlogin.Text) || string.IsNullOrWhiteSpace(txtpasse.Password);
        }

        private void SnackbarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            msg.IsActive = false;
            Display = true;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ConnexionTitle;
                GetHome.MainTitle.Text = Properties.Resources.ConnexionTitle;
            }
            var datacontext = (MainWindowViewModel)GetHome.DataContext;
            if (langcode == GetAppLang())
            {
                //French
                if (Servante.IsFirstTime())
                {
                    if (!Servante.LangAlreadyInsert())
                        await SaveLangToDataBaseASync();
                    if (!Servante.ModuleAlreadyInsert())
                    {
                        await SaveModuleToDataBaseASync();
                    }
                    else
                    {
                        //update module => in to french
                        await UpdateModuleAsync();
                    }

                    if (!Servante.RoleAlreadyInsert())
                    {
                        await SaveRoleAsync();
                    }
                        ShowAccountForm();
                }
                else
                {
                    if (!Servante.ModuleIsInFrench())
                    {
                        //update module => in to french
                        await UpdateModuleAsync();
                    }
                }
            }
            else
            {
                //English
                if (Servante.IsFirstTime())
                {
                    if (!Servante.LangAlreadyInsert())
                        await SaveLangToDataBaseASync();
                    if (!Servante.ModuleAlreadyInsert())
                    {
                        await SaveModuleToDataBaseASync();
                    }
                    else
                    {
                        //update module => in to english
                        await UpdateModuleAsync();
                    }

                    if (!Servante.RoleAlreadyInsert())
                    {
                        await SaveRoleAsync();
                    }
                        ShowAccountForm();
                }
                else
                {
                    if (Servante.ModuleIsInFrench())
                    {
                        //update module => in to english
                        await UpdateModuleAsync();
                    }
                }
            }
            
            if (datacontext.ConnectedUser != null)
            {
                //User is already connected
                GetHome.MenuToggleButton.Visibility = Visibility.Visible;
                datacontext.IsConnected = Visibility.Visible;
                GetHome.DataContext = datacontext;
                GetHome.MainSnackbar.MessageQueue.Enqueue(Properties.Resources.AlreadyConnected);
                btnValidate.IsEnabled = false;
                btnPass4Get.Visibility = Visibility.Collapsed;
            }
            else
            {
                //User is not yet connected
                GetHome.MenuToggleButton.Visibility = Visibility.Hidden;
                datacontext.IsConnected = Visibility.Collapsed;
                GetHome.DataContext = datacontext;
            }
            

        }

        private void ShowAccountForm()
        {
            if (LicenceInfo.IsActivate())
            {
                MessageBox.Show(Properties.Resources.FirstTime, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Compte compte = new Compte(true)
                {
                    IsCreation = true
                };
                compte.ShowDialog();
            }
            
        }

        private void SaveModuleToDataBase()
        {
            var lstmodule = ListModule(Properties.Resources.ModuleList);
            foreach (var item in lstmodule)
            {
                DbManager.Save(item);
            }
        }

        private void SaveLangToDataBase()
        {
            Language langfr = new Language();
            langfr.Code = "fr";
            langfr.Libelle = "Français";

            Language langen = new Language();
            langen.Code = "en";
            langen.Libelle = "English";

            DbManager.Save(langfr);
            DbManager.Save(langen);


        }

        private void UpdateModule()
        {
            int count = 1;
            var lstmodule = ListModule(Properties.Resources.ModuleList);
            foreach (var item in lstmodule)
            {
                item.Id = count;
                Servante.UpdateModule(item);
                count++;
            }
        }

        private string GetAppLang()
        {
            return InfoChecker.KeyValue("Lang");
        }

        private Task UpdateModuleAsync()
        {
            return Task.Run(() => UpdateModule());
        }

        private Task SaveModuleToDataBaseASync()
        {
            return Task.Run(() => SaveModuleToDataBase());
        }

        private Task SaveLangToDataBaseASync()
        {
            return Task.Run(() => SaveLangToDataBase());
        }

        private void SaveRole()
        {
            var lstrole = ListRole(Properties.Resources.RoleList);
            foreach (var item in lstrole)
            {
                DbManager.Save(item);
            }
            //les modules depuis la base de données
            var modules = DbManager.GetAll<Module>();
            //les roles depuis la base de donnée
            var roles = DbManager.GetAll<Role>();
            //on assigne tous les modules au premier role admin
            foreach (var item in modules)
            {
                var rolemodule = new RoleModule(roles[0].Id, item.Id);
                DbManager.Save(rolemodule);
            }
        }

        private Task SaveRoleAsync()
        {
            return Task.Run(() => SaveRole());
        }

        /// <summary>
        /// Retourne la liste des modules depuis le fichier resources
        /// </summary>
        /// <param name="str">La chaine contenant les modules</param>
        /// <returns></returns>
        public List<Module> ListModule(string str)
        {
            string[] lst = str.Split(',');
            List<Module> lstmModules=new List<Module>();
            foreach (var item in lst)
            {
                Module m = new Module(0, item);
                lstmModules.Add(m);

            }

            return lstmModules;
        }

        public List<Role> ListRole(string str)
        {
            string[] lst = str.Split(',');
            List<Role> lstroles=new List<Role>();
            foreach (var item in lst)
            {
                Role m = new Role(0, item);
                lstroles.Add(m);

            }

            return lstroles;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //GetHome.MenuItemsListBox.SelectedIndex = 7;
        }

        private List<string> UserMenus(int roleid)
        {
            //get user allowed modules
            var modules = DbManager.GetByColumnName<RoleModule>("RoleId", roleid.ToString());
            //get menu that user can access
            List<string> menu = new List<string>();
            foreach (var item in modules)
            {
                menu.Add(DbManager.GetById<Module>(item.ModuleId).Libelle);
            }
            return menu;
        }

        private void Pass4Get_Click(object sender, RoutedEventArgs e)
        {
            PassForget passForget = new PassForget();
            passForget.ShowDialog();

        }
    }
}
