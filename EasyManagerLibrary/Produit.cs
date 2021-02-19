using CsvHelper.Configuration.Attributes;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerLibrary
{
    public class Produit
    {
        public event EventHandler<AlertQuantityEventArgs> AlertQuantityEvent;
        [Index(0)]
        public int Id { get; set; }
        [Index(1)]
        public string Nom { get; set; }
        [Ignore]
        public string Description { get; set; }
        [Index(2)]
        [Name("Quantité totale")]
        public double QuantiteTotale { get; set; }

        [Index(3)]
        [Name("Prix unitaire")]
        public decimal PrixUnitaire { get; set; }
        [Ignore]
        public int CategorieId { get; set; }
        [Index(7)]
        [Name("Quantité alerte")]
        public double QuantiteAlerte { get; set; }
        [Ignore]
        public int SupplierId { get; set; }
        [Index(4)]
        [Name("Prix grossiste")]
        public decimal PrixGrossiste { get; set; }

        private double _quantiterestante;

        private string CategorieNom;
        private Supplier _supplier;

        [Index(6)]
        [Name("Quantité restante")]
        public double QuantiteRestante 
        { 
            get 
            { 
                return _quantiterestante; 
            } 
            set 
            { 
                _quantiterestante = value; 
                AlertQuantityEvent?.Invoke(this, new AlertQuantityEventArgs(this));
            } 
        }

        [Write(false)]
        [Index(8)]
        [Name("Catégorie")]
        public string GetCategorieNom => CategorieNom;

        [Write(false)]
        [Ignore]
        public Supplier GetSupplier => _supplier;
        [Index(5)]
        [Write(false)]
        [Name("Quantité Vendue")]
        public double QuantiteVendue => QuantiteTotale-QuantiteRestante;

        public void SetCategorieNom(string nom)
        {
            CategorieNom = nom;
        }

        public void SetSupplier(Supplier supplier)
        {
            _supplier = supplier;
        }
    }

    //ToDo Enregistrer l'utilisasteur qui a enregistré le produit
}
