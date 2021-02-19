using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerDb
{
    public static class Servante
    {
        /// <summary>
        /// Mets à jour la somme restant au niveau de la vente à crédit
        /// </summary>
        /// <param name="VCId">Id VenteCredit</param>
        /// <param name="sommeregle">Somme à retranchée</param>
        /// <returns></returns>
        public static bool UpdateVenteCreditRemainingCost(int VCId, decimal sommeregle)
        {
            try
            {
                var ventecredit = DbManager.GetById<VenteCredit>(VCId);
                decimal left = ventecredit.MontantRestant - sommeregle;
                ventecredit.MontantRestant = left;
                
                return DbManager.UpDate(ventecredit, VCId);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Mets à jour la quantité restante au niveau de la ventecredit
        /// </summary>
        /// <param name="VCId">Id VenteCredit</param>
        /// <param name="prod">Produit</param>
        /// <param name="qtyselle">Quantité vendue</param>
        /// <returns></returns>
        public static bool UpdateProductRemainingQtyInCmd(int VCId, Produit prod, int qtyselle)
        {
            try
            {
                //produit à credit
                var pc = DbManager.GetProductByIdCmdId(VCId, prod.Id);
                double left = pc.QuantiteRestante - qtyselle;
                return DbManager.UpdatePCLeftQty(VCId, prod.Id, left);
            }
            catch
            {
                return false;
            }
        }

        public static List<VenteCredit> VenteCreditSolde()
        {
            try
            {
                return DbManager.GetVenteCreditSolde();
            }
            catch
            {
                return null;
            }
        }

        public static List<VenteCredit> VenteCreditNonSolde()
        {
            try
            {
                return DbManager.GetVenteCreditNonSolde();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Vérifie si c'est la première fois qu'on utilise l'appli
        /// </summary>
        /// <returns>retourne true si c'est la première fois dans le cas contraire false</returns>
        public static bool IsFirstTime()
        {
            var user = DbManager.GetAll<Utilisateur>();
            return user.Count == 0;
        }

        public static bool InsertLang()
        {
            Language langfr = new Language();
            langfr.Code = "fr";
            langfr.Libelle = "Français";

            Language langen = new Language();
            langen.Code = "en";
            langen.Libelle = "English";

            var rsltfr=DbManager.Save(langfr);
            var rslten=DbManager.Save(langen);

            return rslten && rsltfr;
        }

        public static bool ModuleIsInFrench()
        {
            if (ModuleAlreadyInsert())
            {
                string r= DbManager.GetById<Module>(1).Libelle;
                return r == "Catégorie";
            }
            return true;

        }

        public static bool UpdateModule(Module module)
        {
            string query = $"UPDATE Module SET Libelle='{module.Libelle}' WHERE Id={module.Id}";
            return DbManager.UpdateCustumQuery(query);
        }

        public static bool ModuleAlreadyInsert()
        {
            var modules = DbManager.GetAll<Module>();
            return modules.Count != 0;
        }

        public static bool LangAlreadyInsert()
        {
            var langs = DbManager.GetAll<Language>();
            return langs.Count != 0;
        }

        public static bool RoleAlreadyInsert()
        {
            var roles = DbManager.GetAll<Role>();
            return roles.Count != 0;
        }

        /// <summary>
        /// Check to know if the product has a discount
        /// </summary>
        /// <param name="prodId"> Product Id</param>
        /// <returns></returns>
        public static Tuple<bool,Discount,bool> ProductHasDiscount(int prodId)
        {
            try
            {
                //Récupère les informations sur le produit
                var Prod = DbManager.GetById<Produit>(prodId);
                //Vérifie si le produit a une reduction
                var reductionList = DbManager.GetByColumnName<Discount>("ProduitNom", Prod.Nom);
                var reduction = (from r in reductionList where r.Canceled == false select r).FirstOrDefault();
                //si oui on récupère les informations de réduction du produit
                if (reduction != null)
                {
                    return new Tuple<bool, Discount, bool>(true, reduction, reduction.IsValidForCredit);
                }
                //si non on vérifie si la categorie à laquelle appartient le produit à une réduction
                else
                {
                    var catdiscountlist = DbManager.GetByColumnName<Discount>("CategorieId", Prod.CategorieId.ToString());
                    var catdis = (from r in catdiscountlist where r.Canceled == false select r).FirstOrDefault();
                    //si oui on récupère les informations de réduction de la categorie
                    if (catdis != null)
                    {
                        return new Tuple<bool, Discount, bool>(true, catdis, catdis.IsValidForCredit);
                    }
                    //si non le produit n'a aucune réduction
                    return new Tuple<bool, Discount, bool>(false, null, false);
                }
            }
            catch
            {
                return new Tuple<bool, Discount, bool>(false, null, false);
            }
        }

        /// <summary>
        /// Check to know if the client has a discount
        /// </summary>
        /// <param name="clientId"> Client Id</param>
        /// <returns>
        /// return true if the client has a discount otherwise false
        /// return the discount
        /// return true if the discount is applyable to VenteCredit
        /// </returns>
        public static Tuple<bool, Discount,bool> ClientHasDiscount(int clientId)
        {
            try
            {
                //Récupere les informations du client
                var client = DbManager.GetClientById(clientId);
                //Vérifie si le client à une réduction
                var disclst = DbManager.GetByColumnName<Discount>("ClientId", clientId.ToString());
                var clientdisc = (from r in disclst where r.Canceled == false select r).FirstOrDefault();
                //Si oui on récupère les informations de la réduction
                if (clientdisc != null)
                    return new Tuple<bool, Discount, bool>(true, clientdisc, clientdisc.IsValidForCredit);
                //Si non le client n'a aucune réduction
                return new Tuple<bool, Discount, bool>(false, null, false);
            }
            catch
            {

                return new Tuple<bool, Discount, bool>(false, null, false);
            }
        }

        
    }
}
