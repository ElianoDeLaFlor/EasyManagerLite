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
using ClassL = EasyManagerLibrary;

namespace EasyManager
{
    /// <summary>
    /// Interaction logic for CaisseItem.xaml
    /// </summary>
    public partial class CaisseItem : UserControl
    {
        public ClassL.OperationCaisse GetOperationCaisse { get; set; }
        public string libelle { get; set; }
        public decimal montant { get; set; }
        public string Libelle { get { return GetOperation(GetOperationCaisse); }}
        public decimal Montant { get=>GetMontant(GetOperationCaisse); }
        public CaisseItem()
        {
            InitializeComponent();
            DataContext = this;
        }
        private string GetOperation(ClassL.OperationCaisse operationCaisse)
        {
            if (operationCaisse == null)
                return libelle;
            return DbManager.GetById<ClassL.Operation>(operationCaisse.OperationId).Libelle;
        }

        private decimal GetMontant(ClassL.OperationCaisse operationCaisse)
        {
            if (operationCaisse == null)
                return montant;
            return operationCaisse.Montant;
        }
    }
}
