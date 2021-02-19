using APIManagerLibrary.APIResponse;
using EasyManager.UserControls;
using EasyManagerDb;
using EasyManagerLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ComboBox = System.Windows.Controls.ComboBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Panel = System.Windows.Controls.Panel;
using TextBox = System.Windows.Controls.TextBox;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for Parametre.xaml
    /// </summary>
    public partial class Parametre : INotifyPropertyChanged, IAccessControl
    {
        private bool _applytva;
        private bool _restaureprog;
        private bool _bupprog;
        private bool _usersaveprog;
        public decimal Taux { get; set; }

        public bool RestaureProgr 
        { 
            get=>_restaureprog;
            set
            {
                if (_restaureprog == value)
                    return;
                _restaureprog = value;
                OnPropertyChanged();
            }
        }

        public bool BackupProgr
        {
            get => _bupprog;
            set
            {
                if (_bupprog == value)
                    return;
                _bupprog = value;
                OnPropertyChanged();
            }
        }

        public bool UserAppProgr
        {
            get => _usersaveprog;
            set
            {
                if (_usersaveprog == value)
                    return;
                _usersaveprog = value;
                OnPropertyChanged();
            }
        }

        private List<int> _checkedList = new List<int>();
        private Tuple<bool, List<RoleModule>> HasModuleList { get; set; }
        public List<Module> Modules { get; set; } = new List<Module>();
        public int SelectedRoleId { get; set; } = 0;
        public Home GetHome { get; set; }
        public bool IsRoleEdit { get; set; } = false;
        public string RoleLibelle { get; set; }
        ManagementEventWatcher watcher;
        ManagementEventWatcher watcherRemoved;
        public List<string> Languages { get { return GetLanguages(); } }

        /// <summary>
        /// List de modules selectionés
        /// </summary>
        public List<int> CheckedList
        {
            get { return _checkedList; }
            set { _checkedList = value; }
        }

        /// <summary>
        /// Liste des modules selectionés utilisé avant
        /// </summary>
        public List<int> AlreadySelectedModule { get; set; } = new List<int>();

        public string _roleText;
        public Parametre()
        {
            InitializeComponent();
            //CreateTable();
            //ManagerCompanyTable("Consigne", "Text", "");
            //CreateTableAppUserInfo();
            //CreateTableBackupServer();
            DataContext = this;
        }

        public Parametre(Home home)
        {
            InitializeComponent();
            GetHome = home;
            GetHome.GetParametre = this;
            //CreateTable();
            //ManagerCompanyTable("Consigne", "Text", "");
            //CreateTableAppUserInfo();
            //CreateTableBackupServer();
            DataContext = this;
        }

        public Parametre(Home home, bool roleedit, string rolelibelle)
        {
            InitializeComponent();
            GetHome = home;
            GetHome.GetParametre = this;
            IsRoleEdit = roleedit;
            RoleLibelle = rolelibelle;
            //CreateTable();
            //ManagerCompanyTable("Consigne", "Text", "");
            //CreateTableAppUserInfo();
            //CreateTableBackupServer();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ApplyTVA
        {
            get { return _applytva; }
            set { _applytva = value; OnPropertyChanged("ApplyTVA"); }
        }

        public string RoleText
        {
            get { return _roleText; }
            set
            {
                _roleText = value;
                OnPropertyChanged("RoleText");
            }
        }

        public Module GetModule { get => ParamModule(); set { } }

        private void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            OnlyNumber(sender as TextBox);
        }

        private void OnlyNumber(TextBox tb)
        {
            tb.Text = InfoChecker.NumericDecOnlyRegExp(tb.Text);
            tb.SelectionStart = tb.Text.Length;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!CanAccess(GetHome.DataContextt.ConnectedUser))
            {
                IsEnabled = false;
                MessageBox.Show(Properties.Resources.CanAccess, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                IsEnabled = true;
                var lsttva = DbManager.GetAll<Tva>();
                if (lsttva.Count > 0)
                {
                    var tva = lsttva[0];
                    TvaTaux.Text = $"{tva.Taux}";
                    ApplyTVA = tva.Apply;
                }

                var lstcompany = DbManager.GetAll<CompanyInfo>();
                if (lstcompany.Count > 0)
                {
                    var company = lstcompany[0];
                    txtcompanyName.Text = company.Nom;
                    txtcompanyContact.Text = company.Contact;
                    txtcompanyEmail.Text = company.Email;
                    txtconsigne.Text = company.Consigne;
                }

                FilledDropDown();
                SetCheckBox(ModuleList(), StackOne, StackTwo);

                if (IsRoleEdit)
                {
                    OpenRoleExpander();
                }

                GetAppLang();
                SetBackupInfo();
                SetOnlineBackupInfo();
                SetAppUserInfo();
                GetShopLogo();
            }
        }

        private void SetAppUserInfo()
        {
            var lstinfo = DbManager.GetAll<AppUserInfo>();
            if (lstinfo.Count > 0)
            {
                var info = lstinfo[0];
                txtappusername.Text = info.Nom;
                txtappusercontact.Text = info.Contact;
                txtappuseremail.Text = info.Email;
                txtappid.Text = info.Id.ToString();
            }
            else
            {
                var linfoserver = DbManager.GetAll<LicenceRegInfoServer>().FirstOrDefault();
                if (linfoserver == null)
                    return;
                txtappid.Text = linfoserver.AppKey.ToString();
            }
        }

        private void GetShopLogo()
        {
            List<ShopLogo> logos = DbManager.GetAll<ShopLogo>();
            if (logos.Count() == 0)
            {
                // there is not data in the table
                // set the default logo
                reclogo.Source = new BitmapImage(new Uri(InfoChecker.ShopLogoDefault()));
            }
            else
            {
                //get the last record
                var logo = logos.LastOrDefault();
                reclogo.Source = new BitmapImage(new Uri(InfoChecker.SetShopLogoPath(logo.Name)));
            }
        }

        private Module ParamModule()
        {
            return DbManager.GetByColumnName<Module>("Libelle", Properties.Resources.ModuleSettings)[0];
        }

        private void OpenRoleExpander()
        {
            Expander.IsExpanded = false;
            ExpanderOne.IsExpanded = false;
            ExpanderTwo.IsExpanded = true;
            ExpanderThree.IsExpanded = false;


            CbRoleList.SelectedItem = RoleLibelle;
        }



        //Expend 2
        private bool UpdateTva(Tva tva, int id)
        {
            return DbManager.UpDate(tva, id);
        }

        private bool SaveTva(Tva tva)
        {
            return DbManager.Save(tva);
        }

        private Tva GetTva()
        {

            var tva = new Tva
            {
                Taux = Convert.ToDecimal(TvaTaux.Text),
                Apply = ApplyTVA
            };
            return tva;
        }

        private void btnsavetva_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TvaTaux.Text))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SaveTVAToDatabase(GetTva()))
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool SaveTVAToDatabase(Tva tva)
        {
            var tvafromdb = DbManager.GetAll<Tva>();
            if (tvafromdb.Count > 0)
            {
                //Update
                return UpdateTva(tva, tvafromdb[0].Id);
            }
            else
            {
                //Insertion
                return SaveTva(tva);
            }
        }
        //End Expend 2


        //Expend
        private void txtcompanyContact_KeyUp(object sender, KeyEventArgs e)
        {
            txtcompanyContact.Text = InfoChecker.ContactRegExp(txtcompanyContact.Text);
            txtcompanyContact.SelectionStart = txtcompanyContact.Text.Length;

        }

        private bool SaveCompanyToDatabase(CompanyInfo company)
        {
            var companyfromdb = DbManager.GetAll<CompanyInfo>();
            if (companyfromdb.Count > 0)
            {
                //Update
                return UpdateCompany(company, companyfromdb[0].Id);
            }
            else
            {
                //Insertion
                return SaveCompany(company);
            }
        }

        private void txtcompanyEmail_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void SaveCompanyInfo()
        {
            string nom = txtcompanyName.Text.ToLower();
            string contact = txtcompanyContact.Text.ToLower();
            string mail = txtcompanyEmail.Text;

            if (InfoChecker.IsEmpty(nom) || InfoChecker.IsEmpty(contact) || InfoChecker.IsEmpty(mail))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (nom.Length <= 2 || contact.Length <= 2)
            {
                MessageBox.Show(Properties.Resources.ShortEnter, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (!InfoChecker.MailRegExp(txtcompanyEmail.Text))
            {
                MessageBox.Show(Properties.Resources.EmailError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (SaveCompanyToDatabase(GetCompany()))
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private CompanyInfo GetCompany()
        {
            var company = new CompanyInfo
            {
                Contact = txtcompanyContact.Text,
                Email = txtcompanyEmail.Text,
                Nom = txtcompanyName.Text,
                Consigne = txtconsigne.Text
            };

            return company;

        }

        private bool UpdateCompany(CompanyInfo company, int id)
        {
            return DbManager.UpDate(company, id);
        }

        private void btnCompany_Click(object sender, RoutedEventArgs e)
        {
            SaveCompanyInfo();
        }

        private bool SaveCompany(CompanyInfo company)
        {
            return DbManager.Save(company);
        }
        //End Expend





        //Expend 3
        private void FilledDropDown()
        {
            CbRoleList.ItemsSource = RoleList();
        }

        private List<string> RoleList()
        {
            List<string> rolelst = new List<string>();
            var lst = DbManager.GetAll<Role>();

            rolelst.Add(Properties.Resources.MakeSelection);
            foreach (var item in lst)
            {
                rolelst.Add(item.Libelle);
            }

            return rolelst;
        }

        private List<Module> ModuleList()
        {
            var rslt = DbManager.GetAll<Module>();
            return rslt;
        }

        private void SetCheckBox(List<Module> lst_rm, StackPanel pnl, StackPanel pnl2)
        {
            int cnt = 0;
            int splitpoint = lst_rm.Count % 2;
            if (splitpoint > 0)
            {
                SetImpairCheckBox(cnt, lst_rm, splitpoint, StackOne, StackTwo);
            }
            else
            {
                SetPairCheckBox(cnt, lst_rm, splitpoint, StackOne, StackTwo);
            }
        }

        /// <summary>
        /// Est appelé quand le nombre de modules est impaire
        /// </summary>
        /// <param name="cnt">Le nombre checkbox par colonne</param>
        /// <param name="lst_rm">La liste des modules</param>
        /// <param name="splitpoint">Le modulo du nombre de module par 2</param>
        /// <param name="pnl">Colonne un</param>
        /// <param name="pnl2">Colonne deux</param>
        private void SetImpairCheckBox(int cnt, List<Module> lst_rm, int splitpoint, Panel pnl, Panel pnl2)
        {
            cnt = (lst_rm.Count - splitpoint) / 2;
            for (int i = 0; i < cnt + splitpoint; i++)
            {
                ToggleControl tc = new ToggleControl
                {
                    Module = lst_rm[i]
                };
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl.Children.Add(tc);
            }

            for (int i = cnt + splitpoint; i < lst_rm.Count; i++)
            {
                ToggleControl tc = new ToggleControl
                {
                    Module = lst_rm[i]
                };
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl2.Children.Add(tc);
            }
        }

        /// <param name="pnl2">Colonne deux</param>
        private void SetPairCheckBox(int cnt, List<Module> lst_rm, int splitpoint, Panel pnl, Panel pnl2)
        {
            cnt = (lst_rm.Count - splitpoint) / 2;
            for (int i = 0; i < cnt + splitpoint; i++)
            {
                ToggleControl tc = new ToggleControl
                {
                    Module = lst_rm[i]
                };
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl.Children.Add(tc);
            }

            for (int i = cnt + splitpoint; i < lst_rm.Count; i++)
            {
                ToggleControl tc = new ToggleControl
                {
                    Module = lst_rm[i]
                };
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl2.Children.Add(tc);
            }
        }

        private void CbRoleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            var selectedid = combobox.SelectedIndex;
            RoleText = selectedid == 0 ? "Rôle" : combobox.SelectedItem as string;
            if (selectedid != 0)
                SelectedRoleId = DbManager.GetByColumnName<Role>("Libelle", RoleText)[0].Id;
            else
            {
                SelectedRoleId = 0;
                AlreadySelectedModule.Clear();
                CheckedList.Clear();
                StackOne.Children.Clear();
                StackTwo.Children.Clear();
                SetCheckBox(ModuleList(), StackOne, StackTwo);
                return;
            }
            AlreadySelectedModule.Clear();
            StackOne.Children.Clear();
            StackTwo.Children.Clear();
            CheckedList.Clear();
            HasModuleList = HasModules(SelectedRoleId);
            if (HasModuleList.Item1)
                foreach (var item in HasModuleList.Item2)
                    CheckedList.Add(item.ModuleId);

            if (selectedid != 0)
                GetModuleByRoleFromRoleModule(SelectedRoleId);
            SetCheckBox(ModuleList(), StackOne, StackTwo);
        }

        private void CheckTheCkeckBoxes(ToggleControl tc, List<Module> lst_rm, int i)
        {
            if (AlreadySelectedModule.Count > 0)
            {
                tc.State = AlreadySelectedModule.Contains(lst_rm[i].Id);
            }
            else
                tc.State = false;
        }

        public void GetModuleByRoleFromRoleModule(int roleid)
        {
            List<RoleModule> lstrm = DbManager.GetByColumnName<RoleModule>("RoleId", roleid.ToString());
            if (lstrm != null)
            {
                AlreadySelectedModule.Clear();
                foreach (var item in lstrm)
                {
                    AlreadySelectedModule.Add(item.ModuleId);
                }
            }
        }

        public bool CreateRoleModule()
        {
            List<bool> rslt = new List<bool>();
            foreach (var item in CheckedList)
            {
                RoleModule rm = new RoleModule
                {
                    ModuleId = item,
                    RoleId = SelectedRoleId
                };
                rslt.Add(DbManager.Save(rm));
            }

            return !rslt.Contains(false);
        }

        public bool CreateRModule()
        {
            if (HasModuleList.Item1)
            {
                //has module
                List<RoleModule> listnouveau = new List<RoleModule>();
                foreach (var item in CheckedList)
                {
                    RoleModule rm = new RoleModule { ModuleId = item, RoleId = SelectedRoleId };
                    listnouveau.Add(rm);
                }

                List<RoleModule> listexistant = HasModuleList.Item2;

                return RModuleManager(listnouveau, listexistant);
            }
            else
            {
                return CreateRoleModule();
            }
        }

        public bool DeleteRoleModule(List<RoleModule> list)
        {
            List<bool> rslt = new List<bool>();
            foreach (var item in list)
            {
                rslt.Add(DbManager.Delete<RoleModule>(item.Id));
            }

            return !rslt.Contains(false);
        }

        /// <summary>
        /// Retreive no duplicate data from the two list
        /// </summary>
        /// <param name="l"></param>
        /// <param name="l1"></param>
        /// <returns></returns>
        private List<RoleModule> NoDuplicateRoleModule(List<RoleModule> l, List<RoleModule> l1)
        {
            var list = l1.Concat(l).GroupBy(x => x.ModuleId).Where(x => x.Count() == 1).Select(x => x.FirstOrDefault())
                .ToList();
            return list;
        }

        private bool RModuleManager(List<RoleModule> listnew, List<RoleModule> listold)
        {
            if (listnew.Count > listold.Count)
            {
                //Ajout
                var toadd = NoDuplicateRoleModule(listnew, listold);
                return CreateRoleModule(toadd);
            }
            else if (listnew.Count < listold.Count)
            {
                //Suppression
                var todelete = NoDuplicateRoleModule(listnew, listold);
                return DeleteRoleModule(todelete);
            }
            else
            {
                //Modification 'update'
                List<bool> rslt = new List<bool>();
                var todeletetoadd = NoDuplicateRoleModule(listnew, listold);
                foreach (var item in todeletetoadd)
                {
                    if (listold.Contains(item))
                        rslt.Add(DbManager.Delete<RoleModule>(item.Id));
                    else if (listnew.Contains(item))
                        rslt.Add(DbManager.Save(item));
                }

                return !rslt.Contains(false);
            }
        }

        public bool CreateRoleModule(List<RoleModule> list)
        {
            List<bool> rslt = new List<bool>();
            foreach (var item in list)
            {
                rslt.Add(DbManager.Save(item));
            }

            return !rslt.Contains(false);
        }

        public Tuple<bool, List<RoleModule>> HasModules(int RoleId)
        {
            var lst = DbManager.GetByColumnName<RoleModule>("RoleId", RoleId.ToString());
            bool b;
            if (lst == null)
                b = false;
            else
                b = lst.Count > 0;
            return Tuple.Create(b, lst);
        }

        private void Save()
        {
            if (CreateRModule())
            {
                MessageBox.Show(Properties.Resources.RoleParametre, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                if (MessageBox.Show(Properties.Resources.RoleParametreErreur, Properties.Resources.MainTitle,
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Save();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRoleId == 0)
            {
                MessageBox.Show(Properties.Resources.SelectRole, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Save();
        }

        //End Expend 3



        //Expend 4
        private void btnSaveRole_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtrole.Text))
            {
                if (txtrole.Text.Length > 3)
                {
                    var role = new EasyManagerLibrary.Role(0, txtrole.Text);
                    //Check if the role already exist
                    if (!RoleExist(txtrole.Text))
                    {
                        //Role doen't exist
                        if (EasyManagerDb.DbManager.Save(role))
                        {
                            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                            Reset();
                        }
                        else
                        {
                            MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        //Role exist
                        MessageBox.Show(Properties.Resources.RoleExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ShortEnter, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool RoleExist(string rolelibelle)
        {
            var rol = EasyManagerDb.DbManager.GetByColumnName<EasyManagerLibrary.Role>("Libelle", rolelibelle);
            return rol.Count > 0;
        }

        private void Reset()
        {
            txtrole.Text = String.Empty;
        }

        //End Expend 4
        public bool CanAccess(Utilisateur utilisateur)
        {
            //UserRole
            var userrole = DbManager.GetByColumnName<Role>("Libelle", utilisateur.RoleLibelle)[0];
            //Role module
            var rolemodule = DbManager.GetByColumnName<RoleModule>("RoleId", userrole.Id.ToString());

            List<int> usermoduleid = new List<int>();
            foreach (var item in rolemodule)
                usermoduleid.Add(item.ModuleId);

            return usermoduleid.Contains(GetModule.Id);
        }

        private List<string> GetLanguages()
        {
            List<string> lst = new List<string>();
            foreach (var item in DbManager.GetAll<Language>())
            {
                lst.Add(item.Libelle);
            }
            return lst;
        }

        private void btnSavelang_Click(object sender, RoutedEventArgs e)
        {
            string selecteditem = CbLang.SelectedValue as string;
            if (string.IsNullOrWhiteSpace(selecteditem))
                return;
            var lang = DbManager.GetByColumnName<Language>("Libelle", selecteditem)[0].Code;
            if (InfoChecker.SetKeyValue("Lang", lang))
            {
                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                GetHome.IsLicencing = true;
                InfoChecker.RestartApp();
            }
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void GetAppLang()
        {
            string langcode = InfoChecker.KeyValue("Lang");
            var lang = DbManager.GetByColumnName<Language>("Code", langcode)[0].Libelle;
            CbLang.SelectedValue = lang;

        }

        private void Btnpath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog();

        }

        public void FolderBrowserDialog()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    BackupInfo backupinfo = new BackupInfo
                    {
                        Date = DateTime.UtcNow,
                        Dir = fbd.SelectedPath,
                        LastBackupDate = DateTime.UtcNow
                    };
                    //save to database
                    var savedid = DbManager.SaveData(backupinfo);
                    if (savedid > 0)
                    {
                        // load saved backup information
                        var backup = DbManager.GetById<BackupInfo>(savedid);
                        //set the selected directory path in the textblock
                        txtpath.Text = backup.Dir;
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }
            }
        }

        public void FileBrowserDialog()
        {
            using (var fbd = new OpenFileDialog())
            {
                fbd.Filter = "JPG(*.jpg)|*jpg|JPEG(*.jpeg)|*.jpeg|PNG(*.png)|*.png";
                fbd.Title = Properties.Resources.MainTitle;
                DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    //copy the selected image to the application file directory
                    reclogo.Source = null;

                    if (InfoChecker.SavePicture(fbd.FileName, out string fname))
                    {
                        //save the icon information to the database
                        ShopLogo shopLogo = new ShopLogo
                        {
                            Name = fname,
                            CreationDate = DateTime.UtcNow
                        };

                        if (!DbManager.Save(shopLogo))
                        {
                            MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }
            }
        }

        private void Btnbackup_Click(object sender, RoutedEventArgs e)
        {
            Backup();
        }

        private void Backup()
        {
            var lst = DbManager.GetAll<BackupInfo>();
            var result = 2;
            var backup = new BackupInfo();
            if (lst.Count > 0)
            {
                backup = lst.Last();
                result = InfoChecker.Backup(backup);
            }

            if (result == 1)
            {
                var query = $"UPDATE BackupInfo SET LastBackupDate='{InfoChecker.AjustDateWithTime(DateTime.UtcNow)}' WHERE Id={backup.Id}";
                if (DbManager.UpdateCustumQuery(query))
                {
                    txtlastbackup.Text = InfoChecker.AjustDateWithDMY(DateTime.UtcNow);
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    SetBackupInfo();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);


            }
            else if (result == 2)
            {
                FolderBrowserDialog();
                Backup();
            }
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void AutoBackup()
        {
            var lst = DbManager.GetAll<BackupInfo>();
            var backup = lst.Last();
            var result = InfoChecker.Backup(backup);
            if (result == 1)
            {
                var query = $"UPDATE BackupInfo SET LastBackupDate='{InfoChecker.AjustDateWithTime(DateTime.UtcNow)}' WHERE Id={backup.Id}";
                DbManager.UpdateCustumQuery(query);
            }
        }

        private void SetBackupInfo()
        {
            var lst = DbManager.GetAll<BackupInfo>();
            if (lst.Count == 0) return;
            var backup = lst.Last();

            if (Directory.Exists(backup.Dir))
            {
                txtpath.Text = backup.Dir;
                txtlastbackup.Text = InfoChecker.AjustDateWithDMY(backup.LastBackupDate);
            }
            else
            {
                txtpath.Text = "";
                txtlastbackup.Text = "";
            }

        }

        private void SetOnlineBackupInfo()
        {
            var lst = DbManager.GetAll<BackupInfoServer>();
            if (lst.Count == 0) return;
            var backup = lst.Last();

            txtonlinelastbackup.Text = InfoChecker.AjustDateWithDMY(backup.LastBackupDate);

        }

        private void Btnrestore_Click(object sender, RoutedEventArgs e)
        {
            var lst = DbManager.GetAll<BackupInfo>();
            var backup = lst.Last();
            if (InfoChecker.Restore(backup))
            {
                MessageBox.Show($"{Properties.Resources.Succes} {Properties.Resources.Restart}", Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                GetHome.IsLicencing = true;
                InfoChecker.RestartApp();
            }
            else
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Btnchangepassword_Click(object sender, RoutedEventArgs e)
        {
            PasseChange passeChange = new PasseChange(GetHome.DataContextt.ConnectedUser);
            passeChange.ShowDialog();
        }

        private void Btneditcompte_Click(object sender, RoutedEventArgs e)
        {
            Compte compte = new Compte(GetHome.DataContextt.ConnectedUser, false);
            compte.ShowDialog();
        }

        #region Drive Add/Remove Event watcher
        private void WatchDriveAdding()
        {
            watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType=2");
            watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
            watcher.Query = query;
            watcher.Start();
            //watcher.WaitForNextEvent();
        }

        private void WatchDriveRemoving()
        {
            watcherRemoved = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType=3");
            watcherRemoved.EventArrived += new EventArrivedEventHandler(watcher_EventArrived_Removed);
            watcherRemoved.Query = query;
            watcherRemoved.Start();
            ///watcher.WaitForNextEvent();
        }
        private BackupInfo SetBackupInfos()
        {
            var lst = DbManager.GetAll<BackupInfo>();
            var backup = lst.Last();

            return backup;
        }
        private void watcher_EventArrived_Removed(object sender, EventArrivedEventArgs e)
        {
            var r = e.NewEvent.Properties["DriveName"].Value.ToString();
            var b = SetBackupInfos();
            if (!Directory.Exists(b.Dir))
            {
                var drive = b.Dir.Split(':')[0];
                if ($"{drive}:" == r)
                {
                    _ = (MethodInvoker)delegate () { SetBackupInfo(); };

                }
            }
        }

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var r = e.NewEvent.Properties["DriveName"].Value.ToString();
            var b = SetBackupInfos();
            if (Directory.Exists(b.Dir))
            {
                var drive = b.Dir.Split(':')[0];
                if ($"{drive}:" == r)
                {
                    _ = (MethodInvoker)delegate () { SetBackupInfo(); };
                }
            }
        }
        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        private void btnlogo_Click(object sender, RoutedEventArgs e)
        {
            FileBrowserDialog();
            //set the icon
            GetShopLogo();
        }

        private void CreateTable()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS ShopLogo  (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Name TEXT NOT NULL," +
                    "CreationDate TEXT NOT NULL)";
            DbManager.CreateNewTable(query);

        }
        public void ManagerCompanyTable(string columnName, string typ, string defaultvalue)
        {
            //check if the column already exist the table
            if (!DbManager.CustumQueryCheckColumn<CompanyInfo>(columnName))
            {
                // the column is not in the table
                // so we have to create it
                DbManager.CreateNewColumn<CompanyInfo>(columnName, typ, defaultvalue);
            }

        }

        private async void btnsaveappuserinfo_Click(object sender, RoutedEventArgs e)
        {
            if (InfoChecker.IsConnected())
            {
                UserAppProgr = true;
                if (!string.IsNullOrWhiteSpace(txtappusername.Text) && txtappusername.Text.Length > 3)
                {
                    if (!string.IsNullOrWhiteSpace(txtappusercontact.Text) && txtappusercontact.Text.Length >= 8)
                    {
                        if (!string.IsNullOrWhiteSpace(txtappuseremail.Text))// todo email regex
                        {
                            // save appuserinfo
                            var appuserinfo = new AppUserInfo();
                            var key = DbManager.GetByColumnName<Settings>("Name", "AppKey").FirstOrDefault().Data;
                            appuserinfo.Id = key;
                            appuserinfo.Contact = txtappusercontact.Text;
                            appuserinfo.Email = txtappuseremail.Text;
                            appuserinfo.Nom = txtappusername.Text;

                            //check if is an update
                            var userinfo = DbManager.GetAll<AppUserInfo>();


                            if (userinfo.Count > 0)
                            {
                                // is an update
                                if (DbManager.UpDate(appuserinfo, userinfo[0].Id))
                                {
                                    //update userinfo to server
                                    if (InfoChecker.IsConnected())
                                    {
                                        await UpdateAppuserinfoServer(appuserinfo, userinfo[0].Id);
                                    }
                                    UserAppProgr = false;
                                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                                }

                                else
                                {
                                    UserAppProgr = false;
                                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                // is a save
                                if (DbManager.SaveWithId(appuserinfo))
                                {
                                    //save userinfo to server
                                    if (InfoChecker.IsConnected())
                                        await SaveAppuserinfoServer(appuserinfo);
                                    UserAppProgr = false;
                                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    UserAppProgr = false;
                                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }


                        }
                        else
                        {
                            UserAppProgr = false;
                            MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        UserAppProgr = false;
                        MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    UserAppProgr = false;
                    MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.NoConnection, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private async Task<bool> SaveAppuserinfoServer(AppUserInfo infoServer)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = "api/v1/AppUserInfoes";
            var rslt = await ApiResponse.Post(infoServer);

            return rslt.Item2 == System.Net.HttpStatusCode.OK;

        }

        private async Task<bool> UpdateAppuserinfoServer(AppUserInfo infoServer,string id)
        {
            ApiResponse.BaseUrl = InfoChecker.KeyValue("ApiPath");
            ApiResponse.Url = $"api/v1/AppUserInfoes/{id}";
            var rslt = await ApiResponse.Put(infoServer);

            return rslt.Item2 == System.Net.HttpStatusCode.OK;

        }

        private void CreateTableAppUserInfo()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS AppUserInfo (" +
                    "Id TEXT PRIMARY KEY," +
                    "Nom TEXT," +
                    "Contact TEXT," +
                    "Email TEXT)";
            DbManager.CreateNewTable(query);

        }

        private void CreateTableBackupServer()
        {
            //create table if not exist
            string query = "CREATE TABLE IF NOT EXISTS BackupInfoServer (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Dir TEXT," +
                    "Date TEXT," +
                    "LastBackupDate TEXT," +
                    "AppId TEXT," +
                    "ServerId TEXT)";
            DbManager.CreateNewTable(query);
        }

        private async void btnonlinebackup_Click(object sender, RoutedEventArgs e)
        {
            //check if appuserinfo is set
            var appuserinfo = DbManager.GetAll<AppUserInfo>();
            if (appuserinfo.Count() > 0)
            {
                //todo make the online backup
                //check if the user has internet
                if (InfoChecker.IsConnected())
                {
                    BackupProgr = true;
                    var key = DbManager.GetByColumnName<Settings>("Name", "AppKey").FirstOrDefault().Data;
                    //there are an existing backup?
                    var serverbackup = DbManager.GetAll<BackupInfoServer>();
                    if (serverbackup.Count > 0)
                    {
                        //update
                        var bup = serverbackup[serverbackup.Count - 1];

                        string baseurl = InfoChecker.KeyValue("ApiPath");
                        //backup file
                        if (InfoChecker.Backup())
                        {
                            string tempbackup = System.IO.Path.GetTempPath() + "EasyManager";
                            string path = System.IO.Path.Combine(tempbackup, "EasyManagerDb.EasyManager");

                            var byt = File.ReadAllBytes(path);
                            File.Delete(path);

                            if (await MakeOnlineBackupUpdate(bup.AppId, key, byt, baseurl))
                            {
                                //save backup info to local bd
                                string query = $"UPDATE BackupInfoServer SET LastBackupDate='{InfoChecker.AjustDateDb(DateTime.UtcNow)}' WHERE AppId='{key}'";
                                DbManager.UpdateCustumQuery(query);
                                BackupProgr = false;
                                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                            else
                            {
                                BackupProgr = false;
                                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            // Unenable to copy the db to the temp file
                            BackupProgr = false;
                            MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        //first backup
                        string baseurl = InfoChecker.KeyValue("ApiPath");
                        //backup file
                        if (InfoChecker.Backup())
                        {
                            string tempbackup = System.IO.Path.GetTempPath() + "EasyManager";
                            string path = System.IO.Path.Combine(tempbackup, "EasyManagerDb.EasyManager");

                            var byt = File.ReadAllBytes(path);
                            File.Delete(path);
                            if (await MakeOnlineBackupPost(key, byt, baseurl))
                            {
                                //save backup info to local bd
                                BackupInfoServer backupInfoServer = new BackupInfoServer
                                {
                                    AppId = key,
                                    Date = DateTime.UtcNow,
                                    Dir = path
                                };
                                backupInfoServer.LastBackupDate = backupInfoServer.Date;

                                DbManager.Save(backupInfoServer);
                                BackupProgr = false;
                                MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                BackupProgr = false;
                                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            // Unenable to copy the db to the temp file
                            BackupProgr = false;
                            MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }


                    }
                    
                    SetOnlineBackupInfo();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.NoConnection, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                
            }
        }

        private async void btnonlinerestore_Click(object sender, RoutedEventArgs e)
        {
            if (InfoChecker.IsConnected())
            {
                //check if appuserinfo is set
                var appuserinfo = DbManager.GetAll<AppUserInfo>();
                if (appuserinfo.Count() > 0)
                {
                    //todo make the online restauration
                    //download file from the server
                    //check if the user has internet
                    RestaureProgr = true;
                    var key = DbManager.GetByColumnName<Settings>("Name", "AppKey").FirstOrDefault().Data;
                    string baseurl = InfoChecker.KeyValue("ApiPath");

                    if (await GetOnlineBackupFileAsync(key, baseurl))
                    {
                        RestaureProgr = false;
                        MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                        GetHome.IsLicencing = true;
                        InfoChecker.RestartApp();
                    }
                    else
                    {
                        RestaureProgr = false;
                        MessageBox.Show(Properties.Resources.NoBackup, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
                MessageBox.Show(Properties.Resources.NoConnection, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private async Task<bool> MakeOnlineBackupUpdate(string id, string appid, byte[] filebytearray, string baseurl)
        {
            using (var multiPartStream = new MultipartFormDataContent())
            {
                multiPartStream.Add(new StringContent(id), "Id");
                multiPartStream.Add(new ByteArrayContent(filebytearray, 0, filebytearray.Length), "BackupFileName", "EasyManagerDb.EasyManager");
                multiPartStream.Add(new StringContent(appid), "AppUserInfoId");
                //api/v1/backups/update/181a4e1e-afd7-40c9-870f-9b18efde908e
                string requesturl = $"{baseurl}api/v1/backups/update/{id}";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, requesturl)
                {
                    Content = multiPartStream
                };

                //httpRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("multipart/form-data"));

                HttpClient httpClient = new HttpClient();
                using (HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest))
                {
                    return httpResponse.IsSuccessStatusCode;
                }
            }

        }

        private async Task<bool> MakeOnlineBackupPost(string appid, byte[] filebytearray, string baseurl)
        {
            using (var multiPartStream = new MultipartFormDataContent())
            {
                multiPartStream.Add(new ByteArrayContent(filebytearray, 0, filebytearray.Length), "BackupFileName", "EasyManagerDb.EasyManager");
                multiPartStream.Add(new StringContent(appid), "AppUserInfoId");
                string requesturl = $"{baseurl}/api/v1/backups/frm";
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, requesturl)
                {
                    Content = multiPartStream
                };

                HttpClient httpClient = new HttpClient();
                using (HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest))
                {
                    return httpResponse.IsSuccessStatusCode;
                }
            }

        }

        private bool GetOnlineBackupFile(string appid, string baseurl)
        {
            try
            {
                FileDownloader.Chemin = $"{baseurl}api/v1/Backups/Download/AppUser/{appid}";
                string dbpath = System.IO.Path.GetFullPath("EasyManagerDb.db");
                
                return FileDownloader.SaveDownload(dbpath);
            }
            catch
            {

                return false;
            }

        }
        private async Task<bool> GetOnlineBackupFileAsync(string appid, string baseurl)
        {
            return await Task.Run(() => GetOnlineBackupFile(appid,baseurl));

        }
    }
}
