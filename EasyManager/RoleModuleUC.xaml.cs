using EasyManager.ClientClass;
using EasyManager.UserControls;
using EasyManagerDb;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for RoleModuleUC.xaml
    /// </summary>
    public partial class RoleModuleUC : UserControl, INotifyPropertyChanged
    {
        private List<int> _checkedList = new List<int>();
        private Tuple<bool, List<RoleModule>> HasModuleList { get; set; }
        public List<Module> Modules { get; set; } = new List<Module>();
        public int SelectedRoleId { get; set; } = 0;
        public Home GetHome { get; set; }

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

        public RoleModuleUC()
        {
            InitializeComponent();
            DataContext = this;
        }

        public RoleModuleUC(Home h)
        {
            InitializeComponent();
            DataContext = this;
            GetHome = h;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string str)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(str));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FilledDropDown();
            SetCheckBox(ModuleList(), StackOne, StackTwo);
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.ModuleRoleModule;
                GetHome.MainTitle.Text = Properties.Resources.ModuleRoleModule;
            }
        }

        private void TestData()
        {
            for (int i = 0; i < 15; i++)
            {
                ToggleControl uc = new ToggleControl();
                Module m = new Module() {Id = i + 1, Libelle = $"Module {i}"};
                uc.State = false;
                uc.Module = m;

                StackOne.Children.Add(uc);
            }

            for (int i = 0; i < 15; i++)
            {
                ToggleControl uc = new ToggleControl();
                Toggle t = new Toggle();
                Module m = new Module() {Id = i + 1, Libelle = $"Module {i}"};
                uc.State = false;
                uc.Module = m;

                StackTwo.Children.Add(uc);
            }
        }

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
                ToggleControl tc = new ToggleControl();
                tc.Module = lst_rm[i];
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl.Children.Add(tc);
            }

            for (int i = cnt + splitpoint; i < lst_rm.Count; i++)
            {
                ToggleControl tc = new ToggleControl();
                tc.Module = lst_rm[i];
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl2.Children.Add(tc);
            }
        }

        /// <summary>
        /// Est appelé quand le nombre de modules est paire
        /// </summary>
        /// <param name="cnt">Le nombre checkbox par colonne</param>
        /// <param name="lst_rm">La liste des modules</param>
        /// <param name="splitpoint">Le modulo du nombre de module par 2</param>
        /// <param name="pnl">Colonne un</param>
        /// <param name="pnl2">Colonne deux</param>
        private void SetPairCheckBox(int cnt, List<Module> lst_rm, int splitpoint, Panel pnl, Panel pnl2)
        {
            cnt = (lst_rm.Count - splitpoint) / 2;
            for (int i = 0; i < cnt + splitpoint; i++)
            {
                ToggleControl tc = new ToggleControl();
                tc.Module = lst_rm[i];
                CheckTheCkeckBoxes(tc, lst_rm, i);
                tc.SetCheckedList(ref _checkedList);
                pnl.Children.Add(tc);
            }

            for (int i = cnt + splitpoint; i < lst_rm.Count; i++)
            {
                ToggleControl tc = new ToggleControl();
                tc.Module = lst_rm[i];
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
                RoleModule rm = new RoleModule();
                rm.ModuleId = item;
                rm.RoleId = SelectedRoleId;
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
                List<RoleModule> listexistant = new List<RoleModule>();
                foreach (var item in CheckedList)
                {
                    RoleModule rm = new RoleModule {ModuleId = item, RoleId = SelectedRoleId};
                    listnouveau.Add(rm);
                }

                listexistant = HasModuleList.Item2;

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
    }
}
