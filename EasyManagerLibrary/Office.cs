using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyManagerLibrary
{
    public class Office
    {
        #region Variables declaration
        _Application wordApp;
        _Document wordDoc;
        List<Shape> shapeColl;
        Dictionary<string, Field> FieldDic;
        public VenteCredit GetVenteCredit { get; set; }
        public RapportProduit GetRapportProduit { get; set; }
        public List<RapportProduit> GetRapportProduits { get; set; }
        public Vente GetVente { get; set; }
        public bool IsRecall { get; set; } = false;
        public List<Vente> ListVente { get; set; }
        public List<VenteCredit> ListVenteCredit { get; set; }
        public List<Produit> GetProduits { get; set; }
        public List<QuantiteEdition> ListQuantiteEdition { get; set; }
        public List<Discount> ListDiscount { get; set; }
        public List<Client> GetClientLists { get; set; }
        public List<OperationCaisse> GetOperationCaisses { get; set; }

        public string CompanyName { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyEmail { get; set; }
        public Client GetClient { get; set; }
        public string CompanyInfo { get; set; }
        public string FactureNO { get; set; }
        public string FactureDU { get; set; }
        public string TypeFacture { get; set; }//output=Facture
        public string Arrete { get; set; }//output=Arrêté la présente Facture à la somme de:
        public string Client { get; set; }
        public string Total { get; set; }
        public string Date { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Quantite { get; set; }
        public string PrixUnitaire { get; set; }
        public string Montant { get; set; }
        public string Discount { get; set; }
        public string ListProduct { get; set; }
        public string ListeVente { get; set; }
        public string ListeVenteCredit { get; set; }
        public string Produit { get; set; }
        public string QuantiteVendue { get; set; }
        public string QuantiteCreditPayer { get; set; }
        public string MontantCreditPayer { get; set; }
        public string QuantiteCredit { get; set; }
        public string MontantCredit { get; set; }
        public string QuantiteRestante { get; set; }
        public string MontantRestante { get; set; }
        public string Categorie { get; set; }
        public string Solde { get; set; }
        public string CommandeClient { get; set; }
        public string Rembourssement { get; set; }
        public string ResteAPayer { get; set; }
        public string Utilisateur { get; set; }
        public string Periode { get; set; }
        public string At { get; set; }
        public decimal Somme { get; set; }
        public string DateFin { get; set; }
        public string Annuler { get; set; }
        public string Taux { get; set; }
        public string Appliquer { get; set; }
        public decimal SommeRestant { get; set; }
        public string TotalReste { get; set; }
        public string WordNotInstalled { get; set; }
        public string Operation { get; set; }
        public List<ProduitVendu> ProduitVendus { get; set; }
        public List<ProduitCredit> ProduitCommandes { get; set; }
        public string GetListDiscount { get; set; }
        public string GetListAppro { get; set; }
        public string tva { get; set; }
        public string tauxtva { get; set; }
        public decimal SommeTva { get; set; }
        public decimal SommeTtc { get; set; }
        public string TotalTTC { get; set; }
        public string MontantTotalCredit { get; set; }
        public Tva GetTva { get; set; }
        public string ListeProduitVendu { get; set; }
        public string NomClient { get; set; }
        public string ClientList { get; set; }
        public string OperationCaisseListe { get; set; }
        public string ClientFirstName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientContact { get; set; }
        public decimal SortieToale { get; set; }
        public string Depense { get; set; }
        public string Recette { get; set; }
        public string SommeEnCaisse { get; set; }
        public decimal DepenseTotal { get; set; }
        public decimal RecetteTotal { get; set; }
        public decimal SommeEnCaisseTotal { get; set; }
        public string DiscountValue { get; set; }
        public string LogoPath { get; set; }
        public string Consigne { get; set; }
        public bool IsCommand { get; set; }
        #endregion

        public Office() { }

        public Office(VenteCredit cmd)
        {
            GetVenteCredit = cmd;
        }

        public Office(Vente vente)
        {
            GetVente = vente;
        }

        private void FieldDico(_Document wordDoc)
        {
            FieldDic = new Dictionary<string, Field>();
            foreach (Field field in wordDoc.Fields)
            {
                Range range = field.Code;//get the range
                string str = range.Text;//get text in the range
                if (str.StartsWith(" MERGEFIELD"))
                {
                    int num1 = str.IndexOf("\\");
                    int num2 = str.Length - num1;
                    FieldDic.Add(str.Substring(11, num1 - 11).Trim(), field);
                }
            }
        }

        private void ManageTableVente(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            var s = tbl.get_Style();
            object obj1 = Missing.Value;
            int nbrrow = ProduitVendus.Count - 1;
            //Table header
            SetTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ProduitVendus[0].GetProduit().Nom.ToString();
            tbl.Cell(2, 2).Range.Text = ProduitVendus[0].Quantite.ToString();
            tbl.Cell(2, 3).Range.Text = (ProduitVendus[0].PrixUnitaire).ToString();
            tbl.Cell(2, 4).Range.Text = (ProduitVendus[0].Montant).ToString();
            tbl.Cell(2, 5).Range.Text = ProduitVendus[0].GetDiscount;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ProduitVendus[i + 1].GetProduit().Nom.ToString();
                tbl.Cell(i + 3, 2).Range.Text = ProduitVendus[i + 1].Quantite.ToString();
                tbl.Cell(i + 3, 3).Range.Text = (ProduitVendus[i + 1].PrixUnitaire).ToString();
                tbl.Cell(i + 3, 4).Range.Text = (ProduitVendus[i + 1].Montant).ToString();
                tbl.Cell(i + 3, 5).Range.Text = ProduitVendus[i + 1].GetDiscount;
            }

        }

        private void SetTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Designation;
            tbl.Cell(1, 2).Range.Text = Quantite;
            tbl.Cell(1, 3).Range.Text = PrixUnitaire;
            tbl.Cell(1, 4).Range.Text = Montant;
            tbl.Cell(1, 5).Range.Text = Discount;
        }
        private void SetRapportTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Produit;
            tbl.Cell(1, 2).Range.Text = QuantiteVendue;
            tbl.Cell(1, 3).Range.Text = Montant;
            tbl.Cell(1, 4).Range.Text = QuantiteCreditPayer;
            tbl.Cell(1, 5).Range.Text = MontantCreditPayer;
            tbl.Cell(1, 6).Range.Text = QuantiteCredit;
            tbl.Cell(1, 7).Range.Text = MontantTotalCredit;
        }
        private void SetListeProduitTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Code;
            tbl.Cell(1, 2).Range.Text = Produit;
            tbl.Cell(1, 3).Range.Text = Quantite;
            tbl.Cell(1, 4).Range.Text = QuantiteVendue;
            tbl.Cell(1, 5).Range.Text = QuantiteRestante;
            tbl.Cell(1, 6).Range.Text = PrixUnitaire;
            tbl.Cell(1, 7).Range.Text = Categorie;
        }

        private void SetListeClientTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Code;
            tbl.Cell(1, 2).Range.Text = ClientLastName;
            tbl.Cell(1, 3).Range.Text = ClientFirstName;
            tbl.Cell(1, 4).Range.Text = ClientContact;
        }

        private void SetListeOperationCaisseTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Operation;
            tbl.Cell(1, 2).Range.Text = Montant;
            tbl.Cell(1, 3).Range.Text = Utilisateur;
            tbl.Cell(1, 4).Range.Text = Date;
        }
        private void SetListeOperationSortieTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Operation;
            tbl.Cell(1, 2).Range.Text = Montant;
        }

        private void SetListeVenteTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Code;
            tbl.Cell(1, 2).Range.Text = CommandeClient;
            tbl.Cell(1, 3).Range.Text = Utilisateur;
            tbl.Cell(1, 4).Range.Text = Date;
            tbl.Cell(1, 5).Range.Text = Montant;
        }

        private void SetListeProduitCreditTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Code;
            tbl.Cell(1, 2).Range.Text = Produit;
            tbl.Cell(1, 3).Range.Text = Quantite;
            tbl.Cell(1, 4).Range.Text = PrixUnitaire;
            tbl.Cell(1, 5).Range.Text = Montant;
            tbl.Cell(1, 6).Range.Text = QuantiteRestante;
            tbl.Cell(1, 7).Range.Text = ResteAPayer;
            tbl.Cell(1, 8).Range.Text = Discount;
        }

        private void SetListeProduitVenduTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Code;
            tbl.Cell(1, 2).Range.Text = Produit;
            tbl.Cell(1, 3).Range.Text = Quantite;
            tbl.Cell(1, 4).Range.Text = PrixUnitaire;
            tbl.Cell(1, 5).Range.Text = Montant;
            tbl.Cell(1, 6).Range.Text = Discount;
        }

        private void SetListeVenteCreditTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Code;
            tbl.Cell(1, 2).Range.Text = Client;
            tbl.Cell(1, 3).Range.Text = Utilisateur;
            tbl.Cell(1, 4).Range.Text = Date;
            tbl.Cell(1, 5).Range.Text = Montant;
            tbl.Cell(1, 6).Range.Text = MontantRestante;
            tbl.Cell(1, 7).Range.Text = Solde;
        }

        private void SetListeDiscountTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Taux;
            tbl.Cell(1, 2).Range.Text = Date;
            tbl.Cell(1, 3).Range.Text = DateFin;
            tbl.Cell(1, 4).Range.Text = Produit;
            tbl.Cell(1, 5).Range.Text = Categorie;
            tbl.Cell(1, 6).Range.Text = Client;
            tbl.Cell(1, 7).Range.Text = Annuler;
            tbl.Cell(1, 8).Range.Text = Appliquer;
            tbl.Cell(1, 9).Range.Text = Utilisateur;
        }

        private void SetListeApproTableHeader(Table tbl)
        {
            tbl.Cell(1, 1).Range.Text = Code;
            tbl.Cell(1, 2).Range.Text = Produit;
            tbl.Cell(1, 3).Range.Text = Date;
            tbl.Cell(1, 4).Range.Text = Utilisateur;
            tbl.Cell(1, 5).Range.Text = Quantite;
            tbl.Cell(1, 6).Range.Text = PrixUnitaire;
        }

        private void ManageTableCommande(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = ProduitCommandes.Count - 1;
            //Table header
            SetTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ProduitCommandes[0].GetProduit().Nom.ToString();
            tbl.Cell(2, 2).Range.Text = ProduitCommandes[0].Quantite.ToString();
            tbl.Cell(2, 3).Range.Text = (ProduitCommandes[0].PrixUnitaire).ToString();
            tbl.Cell(2, 4).Range.Text = (ProduitCommandes[0].Montant).ToString();
            tbl.Cell(2, 5).Range.Text = ProduitCommandes[0].GetDiscount;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ProduitCommandes[i + 1].GetProduit().Nom.ToString();
                tbl.Cell(i + 3, 2).Range.Text = ProduitCommandes[i + 1].Quantite.ToString();
                tbl.Cell(i + 3, 3).Range.Text = (ProduitCommandes[i + 1].PrixUnitaire).ToString();
                tbl.Cell(i + 3, 4).Range.Text = (ProduitCommandes[i + 1].Montant).ToString();
                tbl.Cell(i + 3, 5).Range.Text = ProduitCommandes[i + 1].GetDiscount;
            }

        }

        private void ManageTableListeProduit(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = GetProduits.Count - 1;
            //Table header
            SetListeProduitTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = GetProduits[0].Id.ToString();
            tbl.Cell(2, 2).Range.Text = GetProduits[0].Nom;
            tbl.Cell(2, 3).Range.Text = GetProduits[0].QuantiteTotale.ToString();
            tbl.Cell(2, 4).Range.Text = GetProduits[0].QuantiteVendue.ToString();
            tbl.Cell(2, 5).Range.Text = GetProduits[0].QuantiteRestante.ToString();
            tbl.Cell(2, 6).Range.Text = (GetProduits[0].PrixUnitaire).ToString();
            tbl.Cell(2, 7).Range.Text = GetProduits[0].GetCategorieNom;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = GetProduits[i + 1].Id.ToString();
                tbl.Cell(i + 3, 2).Range.Text = GetProduits[i + 1].Nom;
                tbl.Cell(i + 3, 3).Range.Text = GetProduits[i + 1].QuantiteTotale.ToString();
                tbl.Cell(i + 3, 4).Range.Text = GetProduits[i + 1].QuantiteVendue.ToString();
                tbl.Cell(i + 3, 5).Range.Text = GetProduits[i + 1].QuantiteRestante.ToString();
                tbl.Cell(i + 3, 6).Range.Text = (GetProduits[i + 1].PrixUnitaire).ToString();
                tbl.Cell(i + 3, 7).Range.Text = GetProduits[i + 1].GetCategorieNom;
            }

        }

        private void ManageTableListeClient(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = GetClientLists.Count - 1;
            //Table header
            SetListeClientTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = GetClientLists[0].Id.ToString();
            tbl.Cell(2, 2).Range.Text = GetClientLists[0].Prenom;
            tbl.Cell(2, 3).Range.Text = GetClientLists[0].Nom;
            tbl.Cell(2, 4).Range.Text = GetClientLists[0].Contact;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = GetClientLists[i + 1].Id.ToString();
                tbl.Cell(i + 3, 2).Range.Text = GetClientLists[i + 1].Prenom;
                tbl.Cell(i + 3, 3).Range.Text = GetClientLists[i + 1].Nom;
                tbl.Cell(i + 3, 4).Range.Text = GetClientLists[i + 1].Contact;
            }

        }

        private void ManageTableListeOperationCaisse(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = GetOperationCaisses.Count - 1;
            //Table header
            SetListeOperationCaisseTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = GetOperationCaisses[0].GetOperation().Libelle;
            tbl.Cell(2, 2).Range.Text = (GetOperationCaisses[0].Montant).ToString();
            tbl.Cell(2, 3).Range.Text = GetOperationCaisses[0].NomUtilisateur;
            tbl.Cell(2, 4).Range.Text = InfoChecker.AjustDateWithDMY(GetOperationCaisses[0].Date);

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = GetOperationCaisses[i + 1].GetOperation().Libelle;
                tbl.Cell(i + 3, 2).Range.Text = (GetOperationCaisses[i + 1].Montant).ToString();
                tbl.Cell(i + 3, 3).Range.Text = GetOperationCaisses[i + 1].NomUtilisateur;
                tbl.Cell(i + 3, 4).Range.Text = InfoChecker.AjustDateWithDMY(GetOperationCaisses[i + 1].Date);
            }

        }

        private void ManageTableListeOperationSortie(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = GetOperationCaisses.Count - 1;
            //Table header
            SetListeOperationSortieTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = GetOperationCaisses[0].GetOperation().Libelle;
            tbl.Cell(2, 2).Range.Text = InfoChecker.CurrencyFormat(GetOperationCaisses[0].Montant);

            //check if is the last row to add the sum row
            if(nbrrow==0)
            {
                tbl.Rows.Add(ref obj1);
                tbl.Rows.Add(ref obj1);
                tbl.Cell(3, 1).Merge(tbl.Cell(3, 2));
                tbl.Cell(4, 1).Range.Text = Total;
                tbl.Cell(4, 2).Range.Text = InfoChecker.CurrencyFormat(SortieToale);
            }

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = GetOperationCaisses[i + 1].GetOperation().Libelle;
                tbl.Cell(i + 3, 2).Range.Text = (GetOperationCaisses[i + 1].Montant).ToString();

                //check if is the last row to add the sum row
                if (nbrrow == i+1)
                {
                    i++;
                    tbl.Rows.Add(ref obj1);
                    tbl.Rows.Add(ref obj1);
                    tbl.Cell(i + 3, 1).Merge(tbl.Cell(i + 3, 2));
                    tbl.Cell(i + 4, 1).Range.Text = Total;
                    tbl.Cell(i + 4, 2).Range.Text = InfoChecker.CurrencyFormat(SortieToale);
                }
            }

        }

        private void ManageCaisseResume(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = GetOperationCaisses.Count - 1;
            //Table header
            SetListeOperationSortieTableHeader(tbl);
            //row 1
            if(GetOperationCaisses.Count()>0)
                tbl.Cell(2, 1).Range.Text = GetOperationCaisses[0].GetOperation().Libelle;
            else
                tbl.Cell(2, 1).Range.Text = "";

            if(GetOperationCaisses.Count()>0)
                tbl.Cell(2, 2).Range.Text = InfoChecker.CurrencyFormat(GetOperationCaisses[0].Montant);
            else
                tbl.Cell(2, 2).Range.Text = InfoChecker.CurrencyFormat(0);

            //check if is the last row to add the sum row
            if (nbrrow == 0)
            {
                tbl.Rows.Add(ref obj1);
                tbl.Rows.Add(ref obj1);
                tbl.Rows.Add(ref obj1);
                tbl.Rows.Add(ref obj1);

                tbl.Cell(3, 1).Merge(tbl.Cell(3, 2));

                tbl.Cell(4, 1).Range.Text = Depense;
                tbl.Cell(4, 2).Range.Text = InfoChecker.CurrencyFormat(DepenseTotal);

                tbl.Cell(5, 1).Range.Text = Recette;
                tbl.Cell(5, 2).Range.Text = InfoChecker.CurrencyFormat(RecetteTotal);
                
                tbl.Cell(6, 1).Range.Text = SommeEnCaisse;
                tbl.Cell(6, 2).Range.Text = InfoChecker.CurrencyFormat(SommeEnCaisseTotal);
            }

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = GetOperationCaisses[i + 1].GetOperation().Libelle;
                tbl.Cell(i + 3, 2).Range.Text = (GetOperationCaisses[i + 1].Montant).ToString();

                //check if is the last row to add the sum row
                if (nbrrow == i + 1)
                {
                    i++;
                    tbl.Rows.Add(ref obj1);
                    tbl.Rows.Add(ref obj1);
                    tbl.Rows.Add(ref obj1);
                    tbl.Rows.Add(ref obj1);

                    tbl.Cell(i + 3, 1).Merge(tbl.Cell(i + 3, 2));

                    tbl.Cell(i + 4, 1).Range.Text = Depense;
                    tbl.Cell(i + 4, 2).Range.Text = InfoChecker.CurrencyFormat(DepenseTotal);

                    tbl.Cell(i + 5, 1).Range.Text = Recette;
                    tbl.Cell(i + 5, 2).Range.Text = InfoChecker.CurrencyFormat(RecetteTotal);
                    
                    tbl.Cell(i + 6, 1).Range.Text = SommeEnCaisse;
                    tbl.Cell(i + 6, 2).Range.Text = InfoChecker.CurrencyFormat(SommeEnCaisseTotal);
                }
            }

        }

        private void ManageTableRapport(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = GetRapportProduits.Count - 1;
            //Table header
            SetRapportTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = GetRapportProduits[0].Name;
            tbl.Cell(2, 2).Range.Text = GetRapportProduits[0].QuantiteVendue;
            tbl.Cell(2, 3).Range.Text = GetRapportProduits[0].Montant;
            tbl.Cell(2, 4).Range.Text = GetRapportProduits[0].QuantiteCreditPayer;
            tbl.Cell(2, 5).Range.Text = GetRapportProduits[0].MontantCreditPayer;
            tbl.Cell(2, 6).Range.Text = GetRapportProduits[0].QuaniteCredit;
            tbl.Cell(2, 7).Range.Text = GetRapportProduits[0].MontantCredit;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = GetRapportProduits[i + 1].Name;
                tbl.Cell(i + 3, 2).Range.Text = GetRapportProduits[i + 1].QuantiteVendue;
                tbl.Cell(i + 3, 3).Range.Text = GetRapportProduits[i + 1].Montant;
                tbl.Cell(i + 3, 4).Range.Text = GetRapportProduits[i + 1].QuantiteCreditPayer;
                tbl.Cell(i + 3, 5).Range.Text = GetRapportProduits[i + 1].MontantCreditPayer;
                tbl.Cell(i + 3, 6).Range.Text = GetRapportProduits[i + 1].QuaniteCredit;
                tbl.Cell(i + 3, 7).Range.Text = GetRapportProduits[i + 1].MontantCredit;
            }

        }

        private void ManageTableListeVente(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = ListVente.Count - 1;
            //Table header
            SetListeVenteTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ListVente[0].Id.ToString();
            tbl.Cell(2, 2).Range.Text = ListVente[0].CommandeClient;
            tbl.Cell(2, 3).Range.Text = ListVente[0].UserName;
            tbl.Cell(2, 4).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListVente[0].Date);
            tbl.Cell(2, 5).Range.Text = (ListVente[0].Montant).ToString();

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ListVente[i + 1].Id.ToString();
                tbl.Cell(i + 3, 2).Range.Text = ListVente[i + 1].CommandeClient;
                tbl.Cell(i + 3, 3).Range.Text = ListVente[i + 1].UserName;
                tbl.Cell(i + 3, 4).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListVente[i + 1].Date);
                tbl.Cell(i + 3, 5).Range.Text = (ListVente[i + 1].Montant).ToString();
            }

        }
        private void ManageTableListeProduitCommande(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = ProduitCommandes.Count - 1;
            //Table header
            SetListeProduitCreditTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ProduitCommandes[0].Id.ToString();
            tbl.Cell(2, 2).Range.Text = ProduitCommandes[0].ProductName;
            tbl.Cell(2, 3).Range.Text = ProduitCommandes[0].Quantite.ToString();
            tbl.Cell(2, 4).Range.Text = (ProduitCommandes[0].PrixUnitaire).ToString();
            tbl.Cell(2, 5).Range.Text = (ProduitCommandes[0].Montant).ToString();
            tbl.Cell(2, 6).Range.Text = ProduitCommandes[0].QuantiteRestante.ToString();
            tbl.Cell(2, 7).Range.Text = (ProduitCommandes[0].SommeRestante).ToString();
            tbl.Cell(2, 8).Range.Text = ProduitCommandes[0].GetDiscount;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ProduitCommandes[i + 1].Id.ToString();
                tbl.Cell(i + 3, 2).Range.Text = ProduitCommandes[i + 1].ProductName;
                tbl.Cell(i + 3, 3).Range.Text = ProduitCommandes[i + 1].Quantite.ToString();
                tbl.Cell(i + 3, 4).Range.Text = (ProduitCommandes[i + 1].PrixUnitaire).ToString();
                tbl.Cell(i + 3, 5).Range.Text = (ProduitCommandes[i + 1].Montant).ToString();
                tbl.Cell(i + 3, 6).Range.Text = ProduitCommandes[i + 1].QuantiteRestante.ToString();
                tbl.Cell(i + 3, 7).Range.Text = (ProduitCommandes[i + 1].SommeRestante).ToString();
                tbl.Cell(i + 3, 8).Range.Text = ProduitCommandes[i + 1].GetDiscount;
            }

        }

        private void ManageTableListeProduitVendu(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = ProduitVendus.Count - 1;
            //Table header
            SetListeProduitVenduTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ProduitVendus[0].Id.ToString();
            tbl.Cell(2, 2).Range.Text = ProduitVendus[0].ProduitNom;
            tbl.Cell(2, 3).Range.Text = ProduitVendus[0].Quantite.ToString();
            tbl.Cell(2, 4).Range.Text = (ProduitVendus[0].PrixUnitaire).ToString();
            tbl.Cell(2, 5).Range.Text = (ProduitVendus[0].Montant).ToString();
            tbl.Cell(2, 6).Range.Text = ProduitVendus[0].GetDiscount;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ProduitVendus[i + 1].Id.ToString();
                tbl.Cell(i + 3, 2).Range.Text = ProduitVendus[i + 1].ProduitNom;
                tbl.Cell(i + 3, 3).Range.Text = ProduitVendus[i + 1].Quantite.ToString();
                tbl.Cell(i + 3, 4).Range.Text = (ProduitVendus[i + 1].PrixUnitaire).ToString();
                tbl.Cell(i + 3, 5).Range.Text = (ProduitVendus[i + 1].Montant).ToString();
                tbl.Cell(i + 3, 6).Range.Text = ProduitVendus[i + 1].GetDiscount;
            }

        }

        private void ManageTableListeVenteCredit(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = ListVenteCredit.Count - 1;
            //Table header
            SetListeVenteCreditTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ListVenteCredit[0].Id.ToString();
            tbl.Cell(2, 2).Range.Text = ListVenteCredit[0].NomClient;
            tbl.Cell(2, 3).Range.Text = ListVenteCredit[0].UserName;
            tbl.Cell(2, 4).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListVenteCredit[0].Date);
            tbl.Cell(2, 5).Range.Text = (ListVenteCredit[0].Montant).ToString();
            tbl.Cell(2, 6).Range.Text = (ListVenteCredit[0].MontantRestant).ToString();
            tbl.Cell(2, 7).Range.Text = ListVenteCredit[0].GetSolde();

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ListVenteCredit[i + 1].Id.ToString();
                tbl.Cell(i + 3, 2).Range.Text = ListVenteCredit[i + 1].NomClient;
                tbl.Cell(i + 3, 3).Range.Text = ListVenteCredit[i + 1].UserName;
                tbl.Cell(i + 3, 4).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListVenteCredit[i + 1].Date);
                tbl.Cell(i + 3, 5).Range.Text = (ListVenteCredit[i + 1].Montant).ToString();
                tbl.Cell(i + 3, 6).Range.Text = (ListVenteCredit[i + 1].MontantRestant).ToString();
                tbl.Cell(i + 3, 7).Range.Text = ListVenteCredit[i + 1].GetSolde();
            }

        }

        private void ManageTableListeDiscount(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = ListDiscount.Count - 1;
            //Table header
            SetListeDiscountTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ListDiscount[0].Tau;
            tbl.Cell(2, 2).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListDiscount[0].DateDebut);
            tbl.Cell(2, 3).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListDiscount[0].DateFin);
            tbl.Cell(2, 4).Range.Text = ListDiscount[0].Produit;
            tbl.Cell(2, 5).Range.Text = ListDiscount[0].NomCat;
            tbl.Cell(2, 6).Range.Text = ListDiscount[0].NomClient;
            tbl.Cell(2, 7).Range.Text = ListDiscount[0].Cancel;
            tbl.Cell(2, 8).Range.Text = ListDiscount[0].Credit;
            tbl.Cell(2, 9).Range.Text = ListDiscount[0].NomUtilisateur;

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ListDiscount[i + 1].Tau;
                tbl.Cell(i + 3, 2).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListDiscount[i + 1].DateDebut);
                tbl.Cell(i + 3, 3).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListDiscount[i + 1].DateFin);
                tbl.Cell(i + 3, 4).Range.Text = ListDiscount[i + 1].Produit;
                tbl.Cell(i + 3, 5).Range.Text = ListDiscount[i + 1].NomCat;
                tbl.Cell(i + 3, 6).Range.Text = ListDiscount[i + 1].NomClient;
                tbl.Cell(i + 3, 7).Range.Text = ListDiscount[i + 1].Cancel;
                tbl.Cell(i + 3, 8).Range.Text = ListDiscount[i + 1].Credit;
                tbl.Cell(i + 3, 9).Range.Text = ListDiscount[i + 1].NomUtilisateur;
            }

        }

        private void ManageTableListeAppro(_Document wordDoc)
        {
            var tbl = wordDoc.Tables[1];
            object obj1 = Missing.Value;
            int nbrrow = ListQuantiteEdition.Count - 1;
            //Table header
            SetListeApproTableHeader(tbl);
            //row 1
            tbl.Cell(2, 1).Range.Text = ListQuantiteEdition[0].Id.ToString();
            tbl.Cell(2, 2).Range.Text = ListQuantiteEdition[0].ProduitNom;
            tbl.Cell(2, 3).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListQuantiteEdition[0].DateEdition);
            tbl.Cell(2, 4).Range.Text = ListQuantiteEdition[0].UserName;
            tbl.Cell(2, 5).Range.Text = ListQuantiteEdition[0].Quantite.ToString();
            tbl.Cell(2, 6).Range.Text = (ListQuantiteEdition[0].PrixUnitaire).ToString();

            for (int i = 0; i < nbrrow; i++)
            {
                // row 1+
                tbl.Rows.Add(ref obj1);
                tbl.Cell(i + 3, 1).Range.Text = ListQuantiteEdition[i + 1].Id.ToString();
                tbl.Cell(i + 3, 2).Range.Text = ListQuantiteEdition[i + 1].ProduitNom;
                tbl.Cell(i + 3, 3).Range.Text = InfoChecker.AjustDateWithTimeDMYT(ListQuantiteEdition[i + 1].DateEdition);
                tbl.Cell(i + 3, 4).Range.Text = ListQuantiteEdition[i + 1].UserName;
                tbl.Cell(i + 3, 5).Range.Text = ListQuantiteEdition[i + 1].Quantite.ToString();
                tbl.Cell(i + 3, 6).Range.Text = (ListQuantiteEdition[i + 1].PrixUnitaire).ToString();                
            }

        }

        private void BillWritter(_Document wordDoc)
        {
            shapeColl = new List<Shape>();
            foreach (Shape shape in wordDoc.Shapes)
            {
                if ((shape.Name.Contains("Text Box")) || (shape.Name.Contains("Zone de texte")))
                {
                    // shape.TextFrame.TextRange.Text = shape.ID.ToString();
                    shapeColl.Add(shape);
                }
            }
        }

        private void Writter(int id, string text)
        {
            foreach (Shape shape in shapeColl)
            {
                if (shape.ID == id)
                    shape.TextFrame.TextRange.Text = text;
            }
        }

        private void SetPicture(int id, string path)
        {
            foreach (Shape shape in shapeColl)
            {
                if (shape.ID == id)
                    shape.Fill.UserPicture(path);
            }
        }

        private void WritterTest()
        {
            foreach (Shape shape in shapeColl)
            {
                shape.TextFrame.TextRange.Text = shape.ID.ToString();
            }
        }

        private string DateFacture(char k = 'F')
        {
            if (k == 'F')
                return $"{GetVente.Date.ToShortDateString()} {At} {GetVente.Date.ToShortTimeString()}";
            else
                return $"{GetVenteCredit.Date.ToShortDateString()} {At} {GetVenteCredit.Date.ToShortTimeString()}";
        }

        private string ClientName()
        {
            return GetClient == null ? "--" : $"{GetClient.Prenom} {GetClient.Nom}";
        }

        private void SetFactureTableHeader()
        {
            //N°
            Writter(18, FactureNO);
            //Du
            Writter(19, FactureDU);
            //Client
            Writter(20, Client);
        }

        private void SetFactureTableHeaderNew(string facnumn,string facdate)
        {
            //N°
            Writter(13, TypeFacture);
            //Du
            Writter(18, facnumn);
            //Client
            Writter(19, facdate);
        }

        private void FillCompanyInfo()
        {
            // Nom de la société
            Writter(4, CompanyName);
            //Contact de la socité
            Writter(5, CompanyTel);
            //Email de la société
            Writter(26, CompanyEmail);
        }
        private void FillCompanyInfoNew()
        {
            // Nom de la société
            Writter(12, CompanyName);
            //Contact de la socité
            Writter(17, CompanyTel);
            //Email de la société
            Writter(16, CompanyEmail);
            //Consigne
            Writter(5, Consigne);
        }

        private void FillFields(string filetype, string arret, char k = 'F')
        {
            FillCompanyInfo();
            //Type de fichie (facture/commande)
            Writter(7, filetype);

            if (k != 'F')
            {
                //Numero de facture/commande
                Writter(8, InfoChecker.FormatIdent(GetVenteCredit.Id));
                //Date de la facture/commande
                Writter(9, DateFacture('C'));
            }
            else
            {
                //Numero de facture/commande
                Writter(8, InfoChecker.FormatIdent(GetVente.Id));
                //Date de la facture/commande
                Writter(9, DateFacture());
            }

            //Nom client
            Writter(10, ClientName());
            //TotalHT
            Writter(11, Total);
            if (SommeTva != 0)
            {
                //TVA
                Writter(17, tva);
                //Taux tva
                Writter(24, tauxtva);
                //Cout tva
                Writter(21, InfoChecker.CurrencyFormat(SommeTva));
            }
            //TotalTTC
            Writter(22, TotalTTC);
            //Somme TotalTTC
            Writter(23, InfoChecker.CurrencyFormat(SommeTtc));
            //Somme total
            if (k != 'F')
                Writter(12, InfoChecker.CurrencyFormat(GetVenteCredit.Montant));
            else
                Writter(12, InfoChecker.CurrencyFormat(GetVente.Montant));
            //Type de fichie (facture/commande)
            Writter(13, arret);
            if (GetVenteCredit != null && k == 'F')
            {
                //Rembourssement
                Writter(1, Rembourssement);
                // N° Commande
                Writter(2, InfoChecker.FormatIdent(GetVenteCredit.Id));
                //Montant total credit
                Writter(3, MontantTotalCredit);
                //Somme taltal credit
                Writter(6, InfoChecker.CurrencyFormat(GetVenteCredit.Montant));
                //Reste à payer
                Writter(15, ResteAPayer);
                //Somme restante
                Writter(16, InfoChecker.CurrencyFormat(GetVenteCredit.MontantRestant));
            }

            //Montant de la facture/commande en lettre
            //if (k != 'F')
            //    NumberToLetter.Chiffre = SommeTtc.ToString();
            //else
                NumberToLetter.Chiffre = AjusteSommeFormat(SommeTtc);
            Writter(14, NumberToLetter.Result());
        }

        private void FillFieldsNew(string filetype, string arret, char k = 'F')
        {
            FillCompanyInfoNew();

            if (k != 'F')
            {
                SetFactureTableHeaderNew(InfoChecker.FormatIdent(GetVenteCredit.Id), $"DATE:{DateFacture('C')}");
            }
            else
            {
                SetFactureTableHeaderNew(InfoChecker.FormatIdent(GetVente.Id), $"DATE:{DateFacture()}");
            }

            //Nom client
            Writter(27, $"{Client} {ClientName()}");
            
            decimal disc = 0;
            if(decimal.TryParse(DiscountValue,out disc))
            {
                if (disc > 0)
                {

                }
                else
                {
                    disc = 0;
                }
            }
            else
            {
                disc = 0;
            }
            if (SommeTva != 0 && disc!=0)
            {
                //TVA + Reduction
                Writter(24,$"{tva} {tauxtva} | {Discount} {InfoChecker.CurrencyFormat(disc)}");
                //TotalHT
                Writter(11, Total);
            }
            else if(SommeTva == 0 && disc != 0)
            {
                //Reduction
                Writter(24, $"{Discount} {InfoChecker.CurrencyFormat(disc)}");
                //TotalHT
                Writter(11, Total);
            }
            else if(SommeTva != 0 && disc == 0)
            {
                //TVA
                Writter(24, $"{tva} {tauxtva}");
                //TotalHT
                Writter(11, Total);
            }
            else
            {
                //
                //Writter(24, $"{tva} {tauxtva} | {Discount} {InfoChecker.CurrencyFormat(disc)}");
            }
            
            //TotalTTC
            Writter(25, $"{TotalTTC}: {InfoChecker.CurrencyFormat(SommeTtc)}");
            //TotalTTC
            Writter(1, $"{TotalTTC}: {InfoChecker.CurrencyFormat(SommeTtc)}");
            //Somme total hors tax
            if (k != 'F')
            {
                if (SommeTva != 0 || disc != 0)
                {
                    var somme = GetVenteCredit.Montant + disc;
                    Writter(23, $"{Total} {InfoChecker.CurrencyFormat(somme)}");

                }
            }
            else
            {
                if (SommeTva != 0 || disc != 0)
                {
                    var somme = GetVente.Montant + disc;
                    Writter(23, $"{Total} {InfoChecker.CurrencyFormat(somme)}");

                }
            }

            if (IsCommand)//GetVenteCredit != null && k == 'F'
            {
                //Rembourssement
                Writter(2, $"{Rembourssement} {InfoChecker.FormatIdent(GetVenteCredit.Id)}");
                
                //Montant total credit
                Writter(3, $"{MontantTotalCredit} {InfoChecker.CurrencyFormat(GetVenteCredit.Montant)}");
                
                //Reste à payer
                Writter(4, $"{ResteAPayer} {InfoChecker.CurrencyFormat(GetVenteCredit.MontantRestant)}");
            }
        }

        private string AjusteSommeFormat(decimal somme)
        {
            string str = somme.ToString();
            if (str.Contains("."))
            {
                var rslt = str.Replace(".", ",");
                return rslt;
            }
            else
                return str;
        }

        public bool GenFacture(string recutemplete, string savepath, char k = 'F')
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = recutemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    //WritterTest();
                    SetFactureTableHeader();



                    if (k != 'F')
                    {
                        ManageTableCommande(wordDoc);
                        FillFields(TypeFacture, Arrete, 'J');
                    }
                    else
                    {
                        ManageTableVente(wordDoc);
                        FillFields(TypeFacture, Arrete);
                    }
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }

        }

        public bool GenFactureNew(string recutemplete, string savepath, char k = 'F')
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = recutemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    //WritterTest();
                    //SetFactureTableHeader();
                    SetPicture(8, LogoPath);


                    if (k != 'F')
                    {
                        ManageTableCommande(wordDoc);
                        FillFieldsNew(TypeFacture, Arrete, 'J');
                    }
                    else
                    {
                        ManageTableVente(wordDoc);
                        FillFieldsNew(TypeFacture, Arrete);
                    }
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }

        }

        public bool GenListeProduit(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    //WritterTest();
                    ManageTableListeProduit(wordDoc);
                    // Nom de la société
                    Writter(4, CompanyName);
                    //Contact de la socité
                    Writter(5, CompanyTel);
                    //Email de la société
                    Writter(1, CompanyEmail);
                    //Type de fichie 
                    Writter(7, ListProduct);
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                    
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeClient(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeClient(wordDoc);
                    // Nom de la société
                    Writter(4, CompanyName);
                    //Contact de la socité
                    Writter(5, CompanyTel);
                    //Email de la société
                    Writter(1, CompanyEmail);
                    //Type de fichie 
                    Writter(7, ClientList);
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeOperationCaisse(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeOperationCaisse(wordDoc);
                    // Nom de la société
                    Writter(4, CompanyName);
                    //Contact de la socité
                    Writter(5, CompanyTel);
                    //Email de la société
                    Writter(1, CompanyEmail);
                    //Periode du rapport
                    Writter(2, Periode);
                    //Type de fichie 
                    Writter(7, OperationCaisseListe);
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeFluxOperation(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeOperationSortie(wordDoc);
                    // Nom de la société
                    Writter(4, CompanyName);
                    //Contact de la socité
                    Writter(5, CompanyTel);
                    //Email de la société
                    Writter(1, CompanyEmail);
                    //Type de fichie 
                    Writter(7, OperationCaisseListe);
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenCaisseResume(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageCaisseResume(wordDoc);
                    // Nom de la société
                    Writter(4, CompanyName);
                    //Contact de la socité
                    Writter(5, CompanyTel);
                    //Email de la société
                    Writter(1, CompanyEmail);
                    //Type de fichie 
                    Writter(7, OperationCaisseListe);
                    //Periode du rapport
                    Writter(3, Periode);
                    
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenRapports(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableRapport(wordDoc);
                    // Nom de la société
                    Writter(4, CompanyName);
                    //Contact de la socité
                    Writter(5, CompanyTel);
                    //Email de la société
                    Writter(1, CompanyEmail);
                    //Type de fichie 
                    Writter(7, ListProduct);//Rapport
                    //Periode du rapport
                    Writter(2, Periode);
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeVente(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeVente(wordDoc);
                    FillCompanyInfo();
                    //Type de fichie 
                    Writter(7, ListeVente);
                    //Periode
                    Writter(1, Periode);
                    //Contact
                    Writter(9, CompanyTel);
                    //Email
                    Writter(10, CompanyEmail);
                    //Total
                    Writter(3, Total);
                    //Somme
                    Writter(6, InfoChecker.CurrencyFormat(Somme));
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeVenteCredit(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeVenteCredit(wordDoc);
                    FillCompanyInfo();
                    //Type de fichie 
                    Writter(7, ListeVenteCredit);
                    //Periode
                    Writter(1, Periode);
                    //Contact
                    Writter(9, CompanyTel);
                    //Email
                    Writter(10, CompanyEmail);
                    //Total
                    Writter(3, $"{Montant} {Total.ToLower()}");
                    //Somme
                    Writter(6, InfoChecker.CurrencyFormat(Somme));
                    //Total montant reste
                    Writter(2, $"{Total} {MontantRestante.ToLower()}");
                    //montant restant
                    Writter(5, InfoChecker.CurrencyFormat(SommeRestant));
                    //text Total soldé
                    Writter(11, $"{Total} {Solde.ToLower()}");
                    //montant soldé
                    Writter(12, InfoChecker.CurrencyFormat(Somme - SommeRestant));
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeDiscount(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeDiscount(wordDoc);
                    FillCompanyInfo();
                    //Type de fichie 
                    Writter(7, GetListDiscount);
                    //Periode
                    Writter(1, Periode);
                    //Contact
                    Writter(9, CompanyTel);
                    //Email
                    Writter(10, CompanyEmail);
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeAppro(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeAppro(wordDoc);
                    //Type de fichie 
                    Writter(7, GetListAppro);
                    //Company Name
                    Writter(4, CompanyName);
                    //Contact
                    Writter(5, CompanyTel);
                    //Email
                    Writter(1, CompanyEmail);
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeProduitCredit(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeProduitCommande(wordDoc);
                    //Title
                    Writter(7, ListeProduitVendu);
                    //Company 
                    Writter(4, CompanyName);
                    //Client name
                    Writter(1, NomClient);
                    //Contact
                    Writter(9, CompanyTel);
                    //Email
                    Writter(10, CompanyEmail);
                    //Total
                    Writter(3, Total);
                    //Somme
                    Writter(6, InfoChecker.CurrencyFormat(Somme));
                    if (SommeTva != 0)
                    {
                        //TVA
                        Writter(2, tva);
                        //Taux tva
                        Writter(11, tauxtva);
                        //Cout tva
                        Writter(5, InfoChecker.CurrencyFormat(SommeTva));
                    }
                    //TotalTTC
                    Writter(12, TotalTTC);
                    //Somme TotalTTC
                    Writter(13, InfoChecker.CurrencyFormat(SommeTtc));
                    //Sommerestante
                    Writter(14, TotalReste);
                    //Somme restante
                    Writter(15, InfoChecker.CurrencyFormat(SommeRestant));
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }

        public bool GenListeProduitVendu(string listetemplete, string savepath)
        {
            try
            {
                wordApp = (_Application)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("000209FF-0000-0000-C000-000000000046")));
                if (wordApp == null)
                    return false;
                try
                {
                    object obj1 = Missing.Value;
                    object Template = listetemplete;
                    object savpath = savepath;
                    wordDoc = wordApp.Documents.Add(ref Template, ref obj1, ref obj1, ref obj1);
                    BillWritter(wordDoc);
                    ManageTableListeProduitVendu(wordDoc);
                    //Title
                    Writter(7, ListeProduitVendu);
                    //Company 
                    Writter(4, CompanyName);
                    //Client name
                    Writter(1, NomClient);
                    //Contact
                    Writter(9, CompanyTel);
                    //Email
                    Writter(10, CompanyEmail);
                    //Total
                    Writter(3, Total);
                    //Somme
                    Writter(6, InfoChecker.CurrencyFormat(Somme));
                    if (SommeTva != 0)
                    {
                        //TVA
                        Writter(2, tva);
                        //Taux tva
                        Writter(11, tauxtva);
                        //Cout tva
                        Writter(5, InfoChecker.CurrencyFormat(SommeTva));
                    }
                    //TotalTTC
                    Writter(12, TotalTTC);
                    //Somme TotalTTC
                    Writter(13, InfoChecker.CurrencyFormat(SommeTtc));
                    //Date
                    Writter(8, $"{InfoChecker.AjustDateWithDMY(DateTime.UtcNow)}");
                    wordDoc.SaveAs2(ref savpath, WdSaveFormat.wdFormatPDF, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1, ref obj1);
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    object obj = Missing.Value;
                    wordApp.Quit(false, ref obj, ref obj);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(WordNotInstalled);
                return false;
            }
        }
    }
}
