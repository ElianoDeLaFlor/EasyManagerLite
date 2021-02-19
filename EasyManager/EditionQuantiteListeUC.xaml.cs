using EasyManager.MenuItems;
using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
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
using ClassLib = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for EditionQuantiteListeUC.xaml
    /// </summary>
    public partial class EditionQuantiteListeUC : UserControl
    {
        #region Variable declaration
        private List<QuantiteEdition> QuantiteEditions { get; set; }
        private List<DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();
        public List<QuantiteEdition> GetQuantiteEditions { get; set; }
        public QuantiteEdition QuantiteEdition { get; set; }
        private int RowIndex = 0;
        public bool IsFilter { get; set; }
        public Home GetHome { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public string SelectedColumn { get; set; } = "DateEdition";
        public CompanyInfo Company { get; set; }
        #endregion

        public EditionQuantiteListeUC()
        {
            InitializeComponent();
        }
        public EditionQuantiteListeUC(Home home)
        {
            InitializeComponent();
            GetHome = home;
            int day = (DateTime.Now.Day) - 1;
            int LeftDaysToTheEnd;
            if (DateTime.UtcNow.Month == 2)
            {
                //febrary
                //total day of the month => 28
                LeftDaysToTheEnd = 28 - (DateTime.Now.Day);
            }
            else
            {
                //total day of the month=>30
                LeftDaysToTheEnd = 30 - (DateTime.Now.Day);
            }

            DateDebut = InfoChecker.AjustDate(InfoChecker.DateArriere(day));
            DateFin = InfoChecker.AjustDate(InfoChecker.NextDate(DateTime.UtcNow, LeftDaysToTheEnd));



            var datacontext = new QuantiteEditionListeUCViewModel();
            datacontext.EditionQuantiteListeUC = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;

            ((QuantiteEditionListeUCViewModel)DataContext).DatePicker.Date = Convert.ToDateTime(DateDebut);
            ((QuantiteEditionListeUCViewModel)DataContext).DatePicker.DateF = Convert.ToDateTime(DateFin);


            Datagrid.Width = (GetHome.ActualWidth) - 50;
            GetHome.SizeChanged += GetHome_SizeChanged;
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Datagrid.Width = GetHome.ActualWidth - 50;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            var datagridcell = sender as DataGridCell;
            var chk = datagridcell.Content as CheckBox;
            //Checked

            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;
            delete.RowIndex = RowIndex;
            if (!DeleteList.Contains(delete, new DeleteRefComparer()))
                DeleteList.Add(delete);
            //ManageGroupAction();
        }

        private void OnUnChecked(object sender, RoutedEventArgs e)
        {
            DeleteRef delete = new DeleteRef();
            delete.CanBeDelete = true;
            delete.RowIndex = RowIndex;
            var d = (from del in DeleteList where del.RowIndex == delete.RowIndex select del).Single();
            int deleteindex = DeleteList.IndexOf(d);
            DeleteList.RemoveAt(deleteindex);
            //ManageGroupAction();
        }

        private void TxtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (IsFilter)
            {
                //Is a number?
                if (Converter.IsNumeric(TxtSearch.Text))
                {
                    Research();
                }
                else
                {
                    if (TxtSearch.Text.Length > 3)
                    {
                        Research();
                    }
                    else
                    {
                        ResetList();
                    }
                }
            }
        }

        private void Research()
        {
            var rslt = Search(TxtSearch.Text.ToLower());
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                QuantiteEditions = null;
                QuantiteEditions = rslt;
                SetData(QuantiteEditions);
            }
            else
            {
                //ResetList();
            }
        }

        private void Research(int k)
        {
            var rslt = SingleSearch(TxtSearch.Text.ToLower());
            if (rslt == null || rslt.Count == 0)
            {
                MessageBox.Show(Properties.Resources.EmptyResult, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ResetList();
                return;
            }

            QuantiteEditions = null;
            QuantiteEditions = (rslt);
            SetData(QuantiteEditions);
        }

        //private void OtoComplete(string[] str, TextBox tb)
        //{
            
        //    var source = new AutoCompleteStringCollection();
        //    source.AddRange(str);
        //    tb.AutoCompleteCustomSource = source;
        //    tb.AutoCompleteMode = AutoCompleteMode.Suggest;
        //    tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
        //}

        private void SetData(List<QuantiteEdition> quantiteEditions)
        {
            Datagrid.ItemsSource = quantiteEditions;
        }

        private List<QuantiteEdition> Search(string critere)
        {
            var rslt = from disc in GetQuantiteEditions where disc.Id.ToString().Contains(critere) || disc.ProduitNom.ToLower().Contains(critere) || disc.DateEdition.ToString().Contains(critere) || disc.UserName.ToLower().Contains(critere) || disc.Quantite.ToString().Contains(critere) || disc.PrixUnitaire.ToString().Contains(critere) select disc;
            return rslt.ToList();
        }

        private List<QuantiteEdition> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetQuantiteEditions where c.Id.ToString() == critere || c.ProduitNom.ToLower() == critere || c.DateEdition.ToString() == critere || c.UserName.ToLower().Contains(critere) || c.PrixUnitaire.ToString()==critere || c.Quantite.ToString()==critere select c;
                return rslt.ToList();
            }
            catch
            {
                return null;
            }
        }

        private void ResetList()
        {
            QuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
            SetData(QuantiteEditions);
        }

        private List<QuantiteEdition> GetAllQuantiteEditions(string datedebut, string datefin)
        {
            var rslt = DbManager.GetDataByDate<QuantiteEdition>(SelectedColumn, datedebut, datefin);
            var lst = new List<QuantiteEdition>();
            if(rslt==null)
                return lst;
            foreach (var item in rslt)
            {
                var user = DbManager.GetById<Utilisateur>(item.UtilisateurId);
                item.SetUser(user);
                var produit = DbManager.GetById<ClassLib.Produit>(item.ProduitId);
                item.SetProduit(produit);
                lst.Add(item);
            }
            return lst;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                ResetList();
                return;
            }

            Research(1);
        }

        private void ChkFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            IsFilter = false;
            btnSearch.IsEnabled = true;
        }

        private void ChkFilter_Checked(object sender, RoutedEventArgs e)
        {
            IsFilter = true;
            btnSearch.IsEnabled = false;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            QuantiteEdition = row.Item as QuantiteEdition;
            RowIndex = QuantiteEdition.Id;
            row.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var row = sender as DataGridRow;
            int rid = row.GetIndex();
            if (rid == 0)
                row.Background = new SolidColorBrush(Colors.White);
            else
            {
                var color = rid % 2 == 0 ? Colors.White : Colors.LightGreen;
                row.Background = new SolidColorBrush(color);
            }
        }

        private void OnRowSelected(object sender, RoutedEventArgs e)
        {
            //var row = sender as DataGridRow;
            //row.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void OnCellSellected(object sender, RoutedEventArgs e)
        {
            var cell = sender as DataGridCell;
            //cell.Background = "{DynamicResource SecondaryHueLightBrush}"
            //cell.Background= new SolidColorBrush(ColorZoneMode.Accent);
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            var pnl = sender as StackPanel;
            var btns = pnl.Children;
            foreach (var item in btns)
            {
                var btn = item as Button;
                btn.Visibility = Visibility.Visible;
            }
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            var pnl = sender as StackPanel;
            var btns = pnl.Children;
            foreach (var item in btns)
            {
                var btn = item as Button;
                btn.Visibility = Visibility.Hidden;
            }
        }

        private void btndelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.DeleteNotification, Properties.Resources.MainTitle,
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Delete(RowIndex))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    QuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
                    SetData(QuantiteEditions);
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Error);

            }
        }

        private bool Delete(int id)
        {
            var quantiteedition = DbManager.GetById<QuantiteEdition>(id);
            if (quantiteedition == null)
                return true;
            return DbManager.Delete<QuantiteEdition>(id);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title = Properties.Resources.EditionQuantite;
                GetHome.MainTitle.Text = Properties.Resources.EditionQuantite;
            }
            GetQuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
            QuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
            SetData(QuantiteEditions);
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;
        }

        private void QuantiteEditionList_Click(object sender, RoutedEventArgs e)
        {
            QuantiteEditions.Clear();
            QuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
            SetData(QuantiteEditions);
        }

        private void BtnDelFromList_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in DeleteList)
            {
                Delete(item.RowIndex);
            }
            MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Information);
            QuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
            SetData(QuantiteEditions);
        }

        private async void btnprint_Click(object sender, RoutedEventArgs e)
        {
            GetHome.DataContextt.Progress = Visibility.Visible;
            if (await GenApproListAsync(QuantiteEditions))
            {
                GetHome.DataContextt.Progress = Visibility.Hidden;
                showDocument = new ShowDocument(SaveLocation);
                showDocument.ShowDialog();
            }
            else
            {
                GetHome.DataContextt.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetCompany()
        {
            var company = DbManager.GetAll<CompanyInfo>();
            if (company.Count > 0)
                Company = company[0];
            else
                Company = null;

        }

        private async Task<bool> GenApproListAsync(List<QuantiteEdition> appro)
        {
            return await Task.Run(() => GenQuantiteEditionList(appro));
        }

        private bool GenQuantiteEditionList(List<QuantiteEdition> appro)
        {
            Office office = new Office();
            GetCompany();
            //Company info
            if (Company != null)
            {
                office.CompanyName = Company.Nom;
                office.CompanyTel = $"Tel:{Company.Contact}";
                office.CompanyEmail = $"Email:{Company.Email}";
            }
            else
            {
                office.CompanyName = "Easy manager";
                office.CompanyTel = "Tel:+228 99 34 12 11";
                office.CompanyEmail = "Email:delaflor@flor.com";
            }
            office.Code = Properties.Resources.Number;
            office.Utilisateur = Properties.Resources.User;
            office.Date = Properties.Resources.DateDebut;
            office.Produit = Properties.Resources.ProduitTitle;
            office.GetListAppro = Properties.Resources.ModuleEditonQuantiteListe;
            office.ListQuantiteEdition = appro;
            office.Quantite = Properties.Resources.Quantite;
            office.PrixUnitaire = Properties.Resources.Price;
            office.Periode = $"{Properties.Resources.FactureDU} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateDebut))}  {Properties.Resources.Au} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateFin))} ";
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.Approvisionnement, Properties.Resources.Approvisionnement);
            return office.GenListeAppro(System.IO.Path.GetFullPath("Files\\ListeAppro_Prototype.dotx"), SaveLocation);
        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = ((QuantiteEditionListeUCViewModel)DataContext).DatePicker.Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((QuantiteEditionListeUCViewModel)DataContext).DatePicker.Date = Calendar.SelectedDate.Value;

            //update list on date change
            DateDebut = InfoChecker.AjustDate(((QuantiteEditionListeUCViewModel)DataContext).DatePicker.Date);
            QuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
            GetQuantiteEditions = QuantiteEditions;
            SetData(QuantiteEditions);
        }

        public void CalendarDialogOpenedEventHandlerF(object sender, DialogOpenedEventArgs eventArgs)
        {
            CalendarF.SelectedDate = ((QuantiteEditionListeUCViewModel)DataContext).DatePicker.DateF;
        }

        public void CalendarDialogClosingEventHandlerF(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CalendarF.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((QuantiteEditionListeUCViewModel)DataContext).DatePicker.DateF = CalendarF.SelectedDate.Value;

            //update list on date change
            DateFin = InfoChecker.AjustDate(((QuantiteEditionListeUCViewModel)DataContext).DatePicker.DateF);
            QuantiteEditions = GetAllQuantiteEditions(DateDebut, DateFin);
            GetQuantiteEditions = QuantiteEditions;
            SetData(QuantiteEditions);
        }        
    }



    public class QuantiteEditionListeUCViewModel
    {
        public DatePickerViewModel DatePicker { get; set; }
        public EditionQuantiteListeUC EditionQuantiteListeUC { get; set; }
    }
}
