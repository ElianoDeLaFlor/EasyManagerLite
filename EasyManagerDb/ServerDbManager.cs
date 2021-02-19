using Dapper;
using Dapper.Contrib.Extensions;
using EasyManagerLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyManagerDb
{
    class ServerDbManager
    {
        public static List<T> GetAll<T>() where T : class, new()
        {
            T result = new T();
            // object type
            string TypeName = result.GetType().Name;

            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<T>($"select * from {TypeName}", new DynamicParameters());
                return r.ToList();
            };


        }

        public static List<T> GetAll<T>(string orderedbycomumn) where T : class, new()
        {
            T result = new T();
            // object type
            string TypeName = result.GetType().Name;

            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                //SELECT * FROM Notifications ORDER by Date DESC
                var r = idb.Query<T>($"SELECT * FROM {TypeName} ORDER BY {orderedbycomumn} DESC", new DynamicParameters());
                return r.ToList();
            };


        }

        public static T GetById<T>(int id) where T : class, new()
        {
            T result = new T();
            // object type
            string TypeName = result.GetType().Name;

            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var r = idb.Query<T>($"select * from {TypeName} where Id={id}", new DynamicParameters());
                    return r.Single();
                }
                catch
                {
                    return null;
                }
            };

        }

        public static List<T> GetByColumnName<T>(string column, string param) where T : class, new()
        {
            T result = new T();
            // object type
            string TypeName = result.GetType().Name;

            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var r = idb.Query<T>($"select * from {TypeName} where {column} ='{param}'", new DynamicParameters());
                    return r.ToList();
                }
                catch
                {
                    return new List<T>();
                }
            };


        }
        public static List<T> GetByColumnNameNot<T>(string column, string param) where T : class, new()
        {
            T result = new T();
            // object type
            string TypeName = result.GetType().Name;

            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var r = idb.Query<T>($"select * from {TypeName} where {column} !='{param}'", new DynamicParameters());
                    return r.ToList();
                }
                catch
                {
                    return new List<T>();
                }
            };


        }
        public static List<T> GetDataByDate<T>(string column, string datedebut, string datefin, bool canceled = false) where T : class, new()
        {
            T result = new T();
            // object type
            string TypeName = result.GetType().Name;

            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var r = idb.Query<T>($"SELECT * FROM {TypeName} WHERE Canceled='{canceled}' AND {column} BETWEEN '{datedebut}' AND '{datefin}'", new DynamicParameters());
                    return r.ToList();
                }
                catch
                {
                    return new List<T>();
                }
            };


        }

        public static List<T> GetDataByDate_<T>(string column, string datedebut, string datefin) where T : class, new()
        {
            T result = new T();
            // object type
            string TypeName = result.GetType().Name;

            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var r = idb.Query<T>($"SELECT * FROM {TypeName} WHERE {column} BETWEEN '{datedebut}' AND '{datefin}'", new DynamicParameters());
                    return r.ToList();
                }
                catch
                {
                    return new List<T>();
                }
            };


        }

        public static List<T> CustumQuery<T>(string query) where T : class, new()
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var r = idb.Query<T>(query, new DynamicParameters());
                    return r.ToList();
                }
                catch
                {
                    return new List<T>();
                }
            };
        }
        public static bool CustumQueryCheckColumn<T>(string cln) where T : class, new()
        {
            try
            {
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;

                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<T>($"select * from {TypeName} WHERE {cln}=NULL", new DynamicParameters());
                };
                return true;
            }
            catch
            {

                return false;
            }

        }

        public static bool CustumQueryCheckTable<T>() where T : class, new()
        {
            try
            {
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;

                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<T>($"SELECT * FROM {TypeName}", new DynamicParameters());
                };
                return true;
            }
            catch
            {

                return false;
            }

        }

        public static bool CreateNewColumn<T>(string cln, string type, string defaultvalue) where T : class, new()
        {
            try
            {
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;
                string query = $"ALTER TABLE {TypeName} ADD COLUMN {cln} {type} DEFAULT {defaultvalue}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    try
                    {
                        var r = idb.Query<bool>(query, new DynamicParameters());
                        return r.FirstOrDefault();
                    }
                    catch
                    {
                        return false;
                    }
                };
            }
            catch
            {
                return false;
            }
        }
        public static bool CreateNewTable(string query)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    try
                    {
                        var r = idb.Query<bool>(query, new DynamicParameters());
                        return r.FirstOrDefault();
                    }
                    catch
                    {
                        return false;
                    }
                };
            }
            catch
            {
                return false;
            }
        }

        public static bool UpdatePassword(int id, string newpassword, string newpassdate)
        {
            try
            {
                //UPDATE
                string query = $"UPDATE Utilisateur SET Password=\"{newpassword}\",PassDate=\"{newpassdate}\" WHERE Id={id}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Execute(query);
                    return true;
                };
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateCustumQuery(string query)
        {
            try
            {
                //UPDATE
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Execute(query);
                    return true;
                };
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateDiscountCanceled(int id, bool cancel)
        {
            try
            {
                //UPDATE
                int k = cancel == true ? 1 : 0;
                string query = $"UPDATE Discount SET Canceled='{k}' WHERE Id={id}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Execute(query);
                    return true;
                };
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<VenteCredit> GetVenteCreditSolde()
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<VenteCredit>($"select * from VenteCredit where MontantRestant= 0", new DynamicParameters());
                return r.ToList();
            };
        }

        public static List<VenteCredit> GetVenteCreditNonSolde()
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<VenteCredit>($"select * from VenteCredit where MontantRestant> 0", new DynamicParameters());
                return r.ToList();
            };
        }

        public static List<ProduitVendu> GetAllProductByVente(int VenteId)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<ProduitVendu>($"select * from ProduitVendu where VenteId={VenteId} ", new DynamicParameters());
                var lst = r.ToList();
                List<ProduitVendu> produitVendus = new List<ProduitVendu>();
                foreach (var item in lst)
                {
                    var rslt = idb.Query<Produit>($"select * from Produit where Id={item.ProduitId}", new DynamicParameters()).Single();
                    item.SetProduit(rslt);
                    produitVendus.Add(item);
                }
                return produitVendus;
            };
        }

        public static List<ProduitCredit> GetAllProductByCmd(int CmdId)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<ProduitCredit>($"select * from ProduitCredit where CommandeId={CmdId} ", new DynamicParameters());
                var lst = r.ToList();
                List<ProduitCredit> produitVendus = new List<ProduitCredit>();
                foreach (var item in lst)
                {
                    var rslt = idb.Query<Produit>($"select * from Produit where Id={item.ProduitId}", new DynamicParameters()).Single();
                    item.SetProduit(rslt);
                    produitVendus.Add(item);
                }
                return produitVendus;
            };
        }

        public static List<ProduitVendu> GetAllProductByVnt(int VenteId)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<ProduitVendu>($"select * from ProduitVendu where VenteId={VenteId} ", new DynamicParameters());
                var lst = r.ToList();
                List<ProduitVendu> produitVendus = new List<ProduitVendu>();
                foreach (var item in lst)
                {
                    var rslt = idb.Query<Produit>($"select * from Produit where Id={item.ProduitId}", new DynamicParameters()).Single();
                    item.SetProduit(rslt);
                    produitVendus.Add(item);
                }
                return produitVendus;
            };
        }

        public static List<ProduitCredit> GetProduitCreditSolde(int Cmdid)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<ProduitCredit>($"select * from ProduitCredit where QuantiteRestante= 0 and CommandeId={Cmdid}", new DynamicParameters());
                return r.ToList();
            };
        }

        public static List<ProduitCredit> GetProduitCreditNonSolde(int Cmdid)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<ProduitCredit>($"select * from ProduitCredit where QuantiteRestante> 0 and CommandeId={Cmdid}", new DynamicParameters());
                return r.ToList();
            };
        }

        public static List<ProduitCredit> GetProduitCredit(int Cmdid)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<ProduitCredit>($"select * from ProduitCredit where CommandeId={Cmdid}", new DynamicParameters());
                return r.ToList();
            };
        }

        /// <summary>
        /// récupère le produit de la commande basé sur son id et celui de la commande
        /// </summary>
        /// <param name="CmdId">VenteCredit Id</param>
        /// <param name="ProdId">Produit Id</param>
        /// <returns></returns>
        public static ProduitCredit GetProductByIdCmdId(int CmdId, int ProdId)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                var r = idb.Query<ProduitCredit>($"select * from ProduitCredit where CommandeId={CmdId} and ProduitId={ProdId} ", new DynamicParameters()).Single();
                return r;
            };
        }
        public static Notifications GetNotificationByType(string prodname, bool isappro)
        {
            using (IDbConnection idb = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var r = idb.Query<Notifications>($"select * from Notifications where ProduitNom='{prodname}' and IsApprovisionnement={isappro}", new DynamicParameters()).Single();
                    return r;
                }
                catch
                {
                    return null;
                }
            };
        }

        public static bool UpdatePCLeftQty(int CmdId, int ProdId, double QR)
        {
            try
            {
                //UPDATE Categorie SET Libelle="catone_" WHERE Id='1';
                string query = $"UPDATE ProduitCredit SET QuantiteRestante={QR} WHERE CommandeId={CmdId} and ProduitId={ProdId} ";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Execute(query);
                    return true;
                };
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static bool UpdateVCLeftCost(int CmdId, decimal leftcost)
        {
            try
            {
                //UPDATE Categorie SET Libelle="catone_" WHERE Id='1';
                string query = $"UPDATE VenteCredit SET MontantRestant={leftcost} WHERE Id={CmdId}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Execute(query);
                    return true;
                };
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static Utilisateur GetUserByNL(string n, string l)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Utilisateur>($"SELECT * FROM Utilisateur WHERE Nom='{n}' and Prenom='{l}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }


        }

        public static Utilisateur GetUserByLogin(string log)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Utilisateur>($"SELECT * FROM Utilisateur WHERE Login='{log}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }
        }

        public static Client GetClientByName(string log)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Client>($"SELECT * FROM Client WHERE Nom='{log}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }
        }

        /// <summary>
        /// Retourne la categorie par libellé
        /// </summary>
        /// <param name="lbl">Le libellé de la categorie</param>
        /// <returns></returns>
        public static Categorie GetCategorieByLibelle(string lbl)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Categorie>($"SELECT * FROM Categorie WHERE Libelle='{lbl}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }
        }

        public static Produit GetProduitByName(string lbl)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Produit>($"SELECT * FROM Produit WHERE Nom='{lbl}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }
        }

        public static Produit GetProduitById(int lbl)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Produit>($"SELECT * FROM Produit WHERE Id='{lbl}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }
        }

        public static Client GetClientById(int id)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Client>($"SELECT * FROM Client WHERE Id='{id}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }
        }

        public static Vente GetVenteById(int id)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Vente>($"SELECT * FROM Vente WHERE Id='{id}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {

                return null;
            }
        }

        public static List<Vente> GetVenteByDate(string dt, bool canceled = false)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    //select * from Utilisateur where Deleted='false' and (DATEPART(YYYY,PassDate)=2021 and DATEPART(MM,PassDate)=04 and DATEPART(DD,PassDate)=03);
                    var dat = Convert.ToDateTime(dt);
                    string query = $"SELECT * FROM Vente WHERE Canceled='{canceled}' AND (DATEPART(YYYY,Date)={dat.Year} and DATEPART(MM,Date)={dat.Month} and DATEPART(DD,Date)={dat.Day})";
                    var r = idb.Query<Vente>(query, new DynamicParameters());
                    return r.ToList();
                };
            }
            catch
            {
                return null;
            }
        }

        public static List<ProduitVendu> GetProduiVenduByProdIdVenteId(int ProdId, int VenteId)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<ProduitVendu>($"SELECT * FROM ProduitVendu WHERE ProduitId={ProdId} AND VenteId={VenteId}", new DynamicParameters());
                    return r.ToList();
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retourne le client par nom et prenom
        /// </summary>
        /// <param name="nom">Nom du client</param>
        /// <param name="prenom">Prénom du client</param>
        /// <returns></returns>
        public static Client GetClientByLastNameName(string nom, string prenom)
        {
            try
            {
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var r = idb.Query<Client>($"SELECT * FROM Client WHERE Nom='{nom}' AND Prenom='{prenom}'", new DynamicParameters()).Single();
                    return r;
                };
            }
            catch
            {
                return null;
            }
        }

        public static bool Save<T>(T t) where T : class, new()
        {
            try
            {
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;
                // get object properties
                var properties = result.GetType().GetProperties();
                Tuple<string, string> tuple = ColNames(properties);
                string query = $"INSERT INTO {TypeName} ({tuple.Item1}) values ({tuple.Item2})";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {

                    idb.Execute(query, t);
                    return true;
                };
            }
            catch
            {

                return false;
            }
        }

        public static bool SaveWithId<T>(T t) where T : class, new()
        {
            try
            {
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;
                // get object properties
                var properties = result.GetType().GetProperties();
                Tuple<string, string> tuple = ColNamesWithId(properties);
                string query = $"INSERT INTO {TypeName} ({tuple.Item1}) values ({tuple.Item2})";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {

                    idb.Execute(query, t);
                    return true;
                };
            }
            catch
            {

                return false;
            }
        }

        public static bool Save(VenteCredit cmd)
        {
            try
            {
                // object type
                string TypeName = cmd.GetType().Name;
                // get object properties
                var properties = cmd.GetType().GetProperties();
                Tuple<string, string> tuple = ColNames(properties);
                string query = $"INSERT INTO {TypeName} ({tuple.Item1}) values ({tuple.Item2})";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {

                    idb.Execute(query, cmd);
                    return true;
                };
            }
            catch
            {
                return false;
            }
        }

        public static int SaveData<T>(T t) where T : class, new()
        {
            try
            {
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;
                // get object properties
                var properties = result.GetType().GetProperties();
                Tuple<string, string> tuple = ColNames(properties);
                string query = $"INSERT INTO {TypeName} ({tuple.Item1}) values ({tuple.Item2});SELECT last_insert_rowid()";
                string queryid = $"SELECT last_insert_rowid()";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    var rslt = idb.Query<int>(query, t);
                    return rslt.First();
                };
            }
            catch
            {

                return 0;
            }
        }



        private static Tuple<string, string> ColNames(PropertyInfo[] p)
        {
            string rslt = "";
            string rsltP = "";
            int cnt = 1;
            foreach (var item in p)
            {
                var attrib = item.GetCustomAttribute<WriteAttribute>();
                if (item.Name == "Id")
                {
                    cnt++;
                    continue;
                }
                if (cnt == p.Count())
                {
                    if (attrib != null)
                        continue;
                    rslt += $"{item.Name}";
                    rsltP += $"@{item.Name}";
                }
                else
                {
                    if (attrib != null)
                        continue;

                    rslt += $"{item.Name}, ";
                    rsltP += $"@{item.Name}, ";
                }
                cnt++;
            }

            return new Tuple<string, string>(RemoveLastComa(rslt), RemoveLastComa(rsltP));
        }

        private static Tuple<string, string> ColNamesWithId(PropertyInfo[] p)
        {
            string rslt = "";
            string rsltP = "";
            int cnt = 1;
            foreach (var item in p)
            {
                var attrib = item.GetCustomAttribute<WriteAttribute>();
                if (cnt == p.Count())
                {
                    if (attrib != null)
                        continue;
                    rslt += $"{item.Name}";
                    rsltP += $"@{item.Name}";
                }
                else
                {
                    if (attrib != null)
                        continue;

                    rslt += $"{item.Name}, ";
                    rsltP += $"@{item.Name}, ";
                }
                cnt++;
            }

            return new Tuple<string, string>(RemoveLastComa(rslt), RemoveLastComa(rsltP));
        }

        public static string RemoveLastComa(string str)
        {
            try
            {
                //"azerty, "
                int length = str.Length;
                bool HasSpace;
                HasSpace = str.EndsWith(" ");
                str = HasSpace == true ? str.Remove(length - 1, 1) : str;
                length = str.Length;
                var index = str.ElementAt(length - 1);
                if (index == ',')
                {
                    return str.Remove(length - 1, 1);
                }
                return str;
            }
            catch (Exception)
            {
                return str;
            }

        }
        private static string UpdateQuery(PropertyInfo[] p, object obj)
        {
            string rslt = "";
            int cnt = 1;
            foreach (var item in p)
            {
                var attrib = item.GetCustomAttribute<WriteAttribute>();
                if (item.Name.ToLower() == "id")
                {
                    cnt++;

                    continue;
                }
                if (cnt == p.Count())
                {
                    if (attrib != null)
                        continue;
                    if (item.PropertyType.Name == "String")
                        rslt += $"{item.Name}='{item.GetValue(obj)}'";
                    else if (item.PropertyType.Name.ToLower() == "boolean")
                    {
                        int k = item.GetValue(obj).ToString() == "True" ? 1 : 0;
                        rslt += $"{item.Name}='{k}'";
                    }
                    else if (item.PropertyType.Name.ToLower() == "datetime")
                    {

                        string datetime = InfoChecker.AjustDateWithTime(GetDateFromString(item.GetValue(obj).ToString()));
                        rslt += $"{item.Name}='{datetime}'";
                    }
                    else
                        rslt += $"{item.Name}='{item.GetValue(obj)}'";
                }
                else
                {
                    if (attrib != null)
                        continue;
                    if (item.PropertyType.Name == "String")
                        rslt += $"{item.Name}='{item.GetValue(obj)}',";
                    else if (item.PropertyType.Name.ToLower() == "boolean")
                    {
                        int k = item.GetValue(obj).ToString() == "True" ? 1 : 0;
                        rslt += $"{item.Name}='{k}',";
                    }
                    else if (item.PropertyType.Name.ToLower() == "datetime")
                    {

                        string datetime = InfoChecker.AjustDateWithTime(GetDateFromString(item.GetValue(obj).ToString()));
                        rslt += $"{item.Name}='{datetime}',";
                    }

                    else
                    {
                        if (obj.GetType().Name == "Discount")
                        {
                            string str = "";
                            if (item.GetValue(obj) == null && item.Name != "ProduitNom")
                            {
                                str = "0";
                            }
                            else if (item.GetValue(obj) == null && item.Name == "ProduitNom")
                            {
                                str = "";
                            }
                            else
                                str = item.GetValue(obj).ToString();
                            rslt += $"{item.Name}='{str.Replace(',', '.')}',";

                        }
                        else
                        {
                            if (item.PropertyType == typeof(Nullable<DateTime>))
                            {
                                if (item.GetValue(obj) == null)
                                    rslt += $"{item.Name}=NULL,";
                                else
                                    rslt += $"{item.Name}='{(item.GetValue(obj).ToString()).Replace(',', '.')}',";
                            }
                            else
                            {
                                rslt += $"{item.Name}='{(item.GetValue(obj).ToString()).Replace(',', '.')}',";
                            }
                        }

                    }

                    cnt++;
                }
            }
            return RemoveLastComa(rslt);
        }

        private static DateTime GetDateFromString(string str)
        {
            string date = str.Split(' ')[0];
            string time = str.Split(' ')[1];

            var dat = date.Split('/');
            var tim = time.Split(':');

            return new DateTime(Convert.ToInt32(dat[2]), Convert.ToInt32(dat[1]), Convert.ToInt32(dat[0]), Convert.ToInt32(tim[0]), Convert.ToInt32(tim[1]), Convert.ToInt32(tim[2]));
        }

        public static bool UpDate<T>(T t, int id) where T : class, new()
        {
            try
            {
                //UPDATE Produit SET Nom='updatedname',Description='updated Description',	QuantiteTotale=123 WHERE Id=1;
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;
                // get object properties
                var properties = result.GetType().GetProperties();
                string param = UpdateQuery(properties, t);
                string query = $"UPDATE {TypeName} SET {param} WHERE Id={id}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    idb.Execute(query);
                    return true;
                };
            }
            catch
            {
                return false;
            }
        }

        public static bool UpDate<T>(T t, string id) where T : class, new()
        {
            try
            {
                //UPDATE Produit SET Nom='updatedname',Description='updated Description',	QuantiteTotale=123 WHERE Id=1;
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;
                // get object properties
                var properties = result.GetType().GetProperties();
                string param = UpdateQuery(properties, t);
                string query = $"UPDATE {TypeName} SET {param} WHERE Id='{id}'";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    idb.Execute(query);
                    return true;
                };
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete<T>(int id) where T : class, new()
        {
            try
            {
                T result = new T();
                // object type
                string TypeName = result.GetType().Name;
                string query = $"DELETE FROM {TypeName} WHERE Id={id}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    idb.Execute(query);
                    return true;
                };
            }
            catch
            {
                return false;
            }
        }
        public static bool DeleteRoleByLibelle(string role)
        {
            try
            {
                string query = $"DELETE FROM Role WHERE Libelle={role}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    idb.Execute(query);
                    return true;
                };
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteRoleModuleByRole(int roleid)
        {
            try
            {
                string query = $"DELETE FROM RoleModule WHERE RoleId={roleid}";
                using (IDbConnection idb = new SqlConnection(GetConnectionString()))
                {
                    idb.Execute(query);
                    return true;
                };
            }
            catch
            {
                return false;
            }
        }


        private static string GetConnectionString(string id = "EasyDbContext")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
