using EasyManager.MenuItems;
using EasyManagerDb;
using EasyManagerLibrary;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for DiscountListUC.xaml
    /// </summary>
    public partial class DiscountListUC : UserControl
    {
        #region Variable declaration
        private List<Discount> Discounts { get; set; }
        private List<DeleteRef> DeleteList { get; set; } = new List<DeleteRef>();
        public List<Discount> GetDiscounts { get; set; }
        public Discount Discount { get; set; }
        private int RowIndex = 0;
        public bool IsFilter { get; set; }
        public Home GetHome { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string SaveLocation { get; set; }
        public ShowDocument showDocument { get; set; }
        public List<string> ColumnList { get; set; }
        public string SelectedColumn { get; set; }
        #endregion
        public DiscountListUC()
        {
            InitializeComponent();
        }
        public DiscountListUC(Home h)
        {
            InitializeComponent();
            GetHome = h;

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



            var datacontext = new DiscountListeUCViewModel();
            datacontext.DiscountListUC = this;
            datacontext.DatePicker = new DatePickerViewModel();
            DataContext = datacontext;

            ((DiscountListeUCViewModel)DataContext).DatePicker.Date = Convert.ToDateTime(DateDebut);
            ((DiscountListeUCViewModel)DataContext).DatePicker.DateF = Convert.ToDateTime(DateFin);

            
            Datagrid.Width = (GetHome.ActualWidth) - 30;
            GetHome.SizeChanged += GetHome_SizeChanged;
        }

        private void GetHome_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Datagrid.Width = (GetHome.ActualWidth) - 30;
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

        private void ManageGroupAction()
        {
            if (DeleteList.Count > 1)
                GroupAction.Visibility = Visibility.Visible;
            else
                GroupAction.Visibility = Visibility.Hidden;
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
            var rslt = Search(TxtSearch.Text);
            if (rslt == null)
                ResetList();
            if (rslt.Count > 0)
            {
                Discounts = null;
                Discounts = rslt;
                SetData(Discounts);
            }
            else
            {
                //ResetList();
            }
        }

        private void Research(int k)
        {
            var rslt = SingleSearch(TxtSearch.Text);
            if (rslt == null || rslt.Count == 0)
            {
                MessageBox.Show(Properties.Resources.EmptyResult, Properties.Resources.MainTitle, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ResetList();
                return;
            }

            Discounts = null;
            Discounts = (rslt);
            SetData(Discounts);
        }

        private void SetData(List<Discount> discount)
        {
            Datagrid.ItemsSource = discount;
        }

        private List<Discount> Search(string critere)
        {
            var rslt = from disc in GetDiscounts where disc.Id.ToString().Contains(critere) || disc.Taux.ToString().Contains(critere) || disc.DateDebut.ToString().Contains(critere) || disc.DateFin.ToString().Contains(critere) select disc;
            return rslt.ToList();
        }

        private List<Discount> SingleSearch(string critere)
        {
            try
            {
                var rslt = from c in GetDiscounts where c.Id.ToString() == critere || c.Tau == critere || c.DateDebut.ToString() == critere || c.DateFin.ToString() == critere select c;
                return rslt.ToList();
            }
            catch
            {
                return null;
            }
        }

        private void ResetList()
        {
            Discounts = GetAllDiscounts(DateDebut,DateFin);
            SetData(Discounts);
        }

        private List<Discount> GetAllDiscounts()
        {
            var rslt = DbManager.GetAll<Discount>();
            var lst = new List<Discount>();
            foreach (var item in rslt)
            {
                item.CancelStatus = item.Canceled == true ? Properties.Resources.Canceled : Properties.Resources.InProgress;
                item.ForCredit = item.IsValidForCredit == true ? Properties.Resources.VenteACredit : "-";
                if (item.CategorieId.HasValue && item.CategorieId.Value > 0)
                    item.SetCat(DbManager.GetById<ClassL.Categorie>(item.CategorieId.Value));
                else if (item.ClientId.HasValue && item.ClientId.Value > 0)
                    item.SetClient(DbManager.GetById<ClassL.Client>(item.ClientId.Value));
                var user = DbManager.GetById<Utilisateur>(item.UserId);
                item.SetUtilisateur(user);
                lst.Add(item);
            }
            return lst;
        }

        private List<Discount> GetAllDiscounts(string datedebut, string datefin, bool canceled=false)
        {
            var rslt = DbManager.GetDataByDate<Discount>(SelectedColumn, datedebut, datefin,canceled);
            var lst = new List<Discount>();
            foreach (var item in rslt)
            {
                item.CancelStatus = item.Canceled == true ? Properties.Resources.Canceled : Properties.Resources.InProgress;
                item.ForCredit = item.IsValidForCredit == true ? Properties.Resources.VenteACredit : "-";
                if (item.CategorieId.HasValue && item.CategorieId.Value > 0)
                    item.SetCat(DbManager.GetById<ClassL.Categorie>(item.CategorieId.Value));
                else if (item.ClientId.HasValue && item.ClientId.Value > 0)
                    item.SetClient(DbManager.GetById<ClassL.Client>(item.ClientId.Value));
                var user = DbManager.GetById<Utilisateur>(item.UserId);
                item.SetUtilisateur(user);
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

            Research();
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
            Discount = row.Item as Discount;
            RowIndex = Discount.Id;
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
                    Discounts = GetAllDiscounts(DateDebut,DateFin);
                    SetData(Discounts);
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK,
                        MessageBoxImage.Error);

            }
        }

        private bool Delete(int id)
        {
            var lst = DbManager.GetById<Discount>(id);
            if (lst == null)
                return true;
            return DbManager.Delete<Discount>(id);
        }

        private void btnINFO_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnedit_Click(object sender, RoutedEventArgs e)
        {
            //Obtenir le discount à modifier
            DiscountEdit discountEdit = new DiscountEdit(RowIndex);
            //Affichage
            discountEdit.ShowDialog();
            //retour à la liste avec mise à jour
            Discounts = GetAllDiscounts(DateDebut,DateFin);
            SetData(Discounts);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetHome != null)
            {
                GetHome.Title= Properties.Resources.DiscountList;
                GetHome.MainTitle.Text = Properties.Resources.DiscountList;
            }
            SetColumnList();
            ColumnType.ItemsSource = ColumnList;
            GetDiscounts = GetAllDiscounts(DateDebut, DateFin);
            Discounts = GetAllDiscounts(DateDebut, DateFin);
            btnSearch.IsEnabled = true;
            ChkFilter.IsChecked = false;

        }

        private void Btncanceldiscount_Click(object sender, RoutedEventArgs e)
        {
            var discount = DbManager.GetById<Discount>(RowIndex);
            //var b = discount.Canceled;
            discount.Canceled = true; //b==true?false:true;
            if(DbManager.UpDate(discount, RowIndex))
            {
                Discounts = GetAllDiscounts(DateDebut,DateFin);
                SetData(Discounts);
            }
                
        }

        private void WDiscountList_Click(object sender, RoutedEventArgs e)
        {
            Discounts.Clear();
            Discounts = GetDiscountsByCritere("Canceled", "0");
            SetData(Discounts);
        }

        private List<Discount> GetDiscountsByCritere(string column,string param)
        {
            var rslt = DbManager.GetByColumnName<Discount>(column, param);
            var lst = new List<Discount>();
            foreach (var item in rslt)
            {
                item.CancelStatus = item.Canceled == true ? Properties.Resources.Canceled : Properties.Resources.InProgress;
                item.ForCredit = item.IsValidForCredit == true ? Properties.Resources.VenteACredit : "-";
                if (item.CategorieId.HasValue && item.CategorieId.Value > 0)
                    item.SetCat(DbManager.GetById<ClassL.Categorie>(item.CategorieId.Value));
                else if (item.ClientId.HasValue && item.ClientId.Value > 0)
                    item.SetClient(DbManager.GetById<ClassL.Client>(item.ClientId.Value));
                lst.Add(item);
            }
            return lst;
        }

        private void CDiscountList_Click(object sender, RoutedEventArgs e)
        {
            Discounts.Clear();
            var lst = GetAllDiscounts(DateDebut, DateFin,true);

            Discounts = (from rslt in lst where rslt.Canceled == true select rslt).ToList();
            SetData(Discounts);
        }

        private void DiscountCredit_Click(object sender, RoutedEventArgs e)
        {
            Discounts.Clear();
            var lst= GetAllDiscounts(DateDebut, DateFin);

            Discounts = (from rslt in lst where rslt.IsValidForCredit == true select rslt).ToList();
            SetData(Discounts);
        }

        private void DiscountList_Click(object sender, RoutedEventArgs e)
        {
            Discounts.Clear();
            Discounts = GetAllDiscounts(DateDebut,DateFin);
            SetData(Discounts);
        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = ((DiscountListeUCViewModel)DataContext).DatePicker.Date;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((DiscountListeUCViewModel)DataContext).DatePicker.Date = Calendar.SelectedDate.Value;

            //update list on date change
            DateDebut = InfoChecker.AjustDate(((DiscountListeUCViewModel)DataContext).DatePicker.Date);
            Discounts = GetAllDiscounts(DateDebut, DateFin);
            GetDiscounts = Discounts;
            SetData(Discounts);
        }

        public void CalendarDialogOpenedEventHandlerF(object sender, DialogOpenedEventArgs eventArgs)
        {
            CalendarF.SelectedDate = ((DiscountListeUCViewModel)DataContext).DatePicker.DateF;
        }

        public void CalendarDialogClosingEventHandlerF(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!CalendarF.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            ((DiscountListeUCViewModel)DataContext).DatePicker.DateF = CalendarF.SelectedDate.Value;

            //update list on date change
            DateFin = InfoChecker.AjustDate(((DiscountListeUCViewModel)DataContext).DatePicker.DateF);
            Discounts = GetAllDiscounts(DateDebut, DateFin);
            GetDiscounts = Discounts;
            SetData(Discounts);
        }

        private void ColumnType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var col = sender as ComboBox;
            var index = col.SelectedIndex;
            if (index == 0)
                SelectedColumn = "DateDebut";
            else
                SelectedColumn = "DateFin";
            
            Discounts = GetAllDiscounts(DateDebut, DateFin);
            GetDiscounts = Discounts;
            SetData(Discounts);
        }

        private void SetColumnList()
        {
            ColumnList = new List<string>();
            ColumnList.Add(Properties.Resources.DateDebut);
            ColumnList.Add(Properties.Resources.DateFin);
        }

        private bool GenDiscountList(List<Discount> discount)
        {
            Office office = new Office();
            office.CompanyName = "DeLaFlor Corporation";
            office.CompanyTel = "Tel:+228 99 34 12 11";
            office.CompanyEmail = "Email:delaflor@flor.com";
            office.Code = Properties.Resources.Number;
            office.Utilisateur = Properties.Resources.User;
            office.Date = Properties.Resources.DateDebut;
            office.DateFin = Properties.Resources.DateFin;
            office.ListDiscount = discount;
            office.Annuler = Properties.Resources.CancelActive;
            office.Appliquer = Properties.Resources.Applicable;
            office.Categorie = Properties.Resources.Category;
            office.Client = Properties.Resources.Client;
            office.Discount = Properties.Resources.Discount;
            office.Taux = Properties.Resources.Taux;
            office.Produit = Properties.Resources.ProduitTitle;
            office.GetListDiscount = Properties.Resources.DiscountList;
            office.Periode = $"{Properties.Resources.FactureDU} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateDebut))}  {Properties.Resources.Au} {InfoChecker.AjustDateWithDMY(Convert.ToDateTime(DateFin))} ";
            SaveLocation = InfoChecker.SaveLoc(Properties.Resources.DiscountFolder, Properties.Resources.DiscountFolder);
            return office.GenListeDiscount(System.IO.Path.GetFullPath("Files\\ListeDiscount_Prototype.dotx"), SaveLocation);
        }

        private async void btnprint_Click(object sender, RoutedEventArgs e)
        {
            if (Discounts == null || Discounts.Count == 0)
            {
                MessageBox.Show(Properties.Resources.EmptyList, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            GetHome.DataContextt.Progress = Visibility.Visible;
            
            if (await PrintAsync(Discounts))
            {
                 showDocument = new ShowDocument(SaveLocation);
                GetHome.DataContextt.Progress = Visibility.Hidden;
                showDocument.ShowDialog();           
            }
            else
            {
                GetHome.DataContextt.Progress = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Print(List<Discount> lst)
        {
            return GenDiscountList(lst);
        }

        private async Task<bool> PrintAsync(List<Discount> lst)
        {
            return await Task.Run(() => Print(lst));
        }
    }

    public class DiscountListeUCViewModel
    {
        public DatePickerViewModel DatePicker { get; set; }
        public DiscountListUC DiscountListUC { get; set; }
    }
}
