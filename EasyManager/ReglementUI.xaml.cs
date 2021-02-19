using EasyManagerDb;
using EasyManagerLibrary;
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
using System.Windows.Shapes;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for ReglementUI.xaml
    /// </summary>
    public partial class ReglementUI : Window
    {
        public List<string> Commande { get; set; } = new List<string>();
        public int UtilisateurId { get; set; }
        public int VenteCreditId { get; set; }
        public ReglementUI()
        {
            InitializeComponent();
        }
        
        public ReglementUI(int userid)
        {
            InitializeComponent();
            UtilisateurId = userid;
        }

        private void btnsave_Click(object sender, RoutedEventArgs e)
        {
            if (CbCmdList.SelectedIndex <= 0)
                MessageBox.Show(Properties.Resources.RegSelectionError, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                if (Convert.ToDecimal(txtmontant.Text) > 0)
                {
                    //save
                    if (SaveReglement(GetReglement()))
                    {
                        // vente credit
                        var vc = DbManager.GetById<VenteCredit>(VenteCreditId);
                        decimal lef=vc.Montant- Convert.ToDecimal(txtmontant.Text);
                        DbManager.UpdateVCLeftCost(vc.Id, lef);
                        MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                        Reset();
                    }
                    else
                        MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    MessageBox.Show(Properties.Resources.Error,Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool SaveReglement(Reglement reg)
        {
            return DbManager.Save(reg);
        }

        private void GetCommandeList()
        {
            Commande.Add(Properties.Resources.MakeSelection);
            var rslt = DbManager.GetAll<VenteCredit>();
            foreach (var item in rslt)
            {
                Commande.Add(InfoChecker.FormatIdent(item.Id));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            container.MinWidth = ActualWidth - 500;
            GetCommandeList();
            FilledDropDownCmd();
        }

        private void FilledDropDownCmd()
        {
            if (CbCmdList.ItemsSource != Commande)
                CbCmdList.ItemsSource = Commande;
        }

        private void txtmontant_KeyUp(object sender, KeyEventArgs e)
        {
            txtmontant.Text = InfoChecker.NumericDecOnlyRegExp(txtmontant.Text);
            txtmontant.SelectionStart = txtmontant.Text.Length;
        }

        private Reglement GetReglement()
        {
            var reg = new Reglement();
            reg.Date = DateTime.Now;
            reg.Montant = Convert.ToDecimal(txtmontant.Text);
            reg.UtilisateurId = UtilisateurId;
            reg.VenteCreditId = VenteCreditId;
            return reg;
        }

        private void CbCmdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            string sel = cb.SelectedValue as string;
            if (sel != Properties.Resources.MakeSelection)
                VenteCreditId = Convert.ToInt32(sel);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size= ActualWidth - 500;
            if (size > 0)
                container.MinWidth = size; 
        }

        private void Reset()
        {
            txtmontant.Clear();
            CbCmdList.SelectedIndex = 0;
        }

    }
}
