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
    /// Interaction logic for Categorie.xaml
    /// </summary>
    public partial class Categorie : Window
    {
        public EasyManagerLibrary.Categorie categories { get; set; }
        public int CategorieId { get; set; } = 0;
        public Categorie()
        {
            InitializeComponent();
        }

        public Categorie(int catid)
        {
            InitializeComponent();
            CategorieId = catid;
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string lblcontent = txtlabel.Text.ToLower();
            if (!Check(lblcontent))
            {
                MessageBox.Show(Properties.Resources.EmptyField, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (txtlabel.Text.Length <= 2)
            {
                MessageBox.Show(Properties.Resources.ShortEnter, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (CategorieId == 0)
            {
                if (LibelleExist(lblcontent))
                {
                    MessageBox.Show(Properties.Resources.LblExist, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            GetCategorie();
            if (CategorieId == 0)
            {
                //new categorie
                if (DbManager.Save(categories))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    Reset();
                }
                else
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //update
                if (DbManager.UpDate<EasyManagerLibrary.Categorie>(categories, CategorieId))
                {
                    MessageBox.Show(Properties.Resources.Succes, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    //Reset();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Error, Properties.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            

        }
        private void GetCategorie()
        {
            categories = new EasyManagerLibrary.Categorie();
            categories.Libelle = txtlabel.Text.ToLower();
            categories.Description = txtDescription.Text.Length<=3?txtlabel.Text:txtDescription.Text;
        }

        /// <summary>
        /// Vérifies libelle
        /// </summary>
        /// <returns></returns>
        private bool Check(string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        private bool LibelleExist(string str)
        {
            return DbManager.GetCategorieByLibelle(str) != null;
        }

        private void Reset()
        {
            txtlabel.Text = String.Empty;
            txtDescription.Text = String.Empty;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CategorieId != 0)
            {
                categories = DbManager.GetById<EasyManagerLibrary.Categorie>(CategorieId);
                txtlabel.Text = categories.Libelle;
                txtDescription.Text = categories.Description;
            }
        }
    }
}
