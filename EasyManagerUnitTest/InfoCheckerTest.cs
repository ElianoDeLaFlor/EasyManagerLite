using System;
using EasyManagerLibrary;
using EasyManagerDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace EasyManagerUnitTest
{
    [TestClass]
    public class InfoCheckerTest
    {
        [TestMethod]
        public void FormatIdent()
        {
            // Arrange
            int id = 121;
            string expected = "0000000121";
            // Actual
            string actual = InfoChecker.FormatIdent(id);
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsMonthOldTest()
        {
            //Arrange
            var date = new DateTime(2019, 10, 2);
            //bool expected = true;
            //Actual
            bool actual = InfoChecker.IsMonthOld(date);
            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void InsertTest()
        {
            VenteCredit cmd = new VenteCredit
            {
                ClientId = 1,
                Date = DateTime.Now,
                Montant = 12345,
                MontantRestant = 12345,
                UtilisateurId = 1
            };

            Client client;
            client = DbManager.GetById<Client>(2);
            cmd.SetClient(client);
            var u = DbManager.GetById<Utilisateur>(1);
            cmd.SetUser(u);

            bool actual = DbManager.Save(cmd);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void RemoveLastComaTest()
        {
            //Arrange
            string str = "Libelle, Description,";
            string expected = "Libelle, Description";

            //Actual
            string Actual = DbManager.RemoveLastComa(str);

            //Assert
            Assert.AreEqual(expected, Actual);

        }


        [TestMethod]
        public void UpdateCommandRemainingCostTest()
        {
            //Arrange
            bool expected = true;
            //Actual
            var actual = Servante.UpdateVenteCreditRemainingCost(1, 1);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DateArriereTest()
        {
            //Arrange
            int param = 3;
            DateTime expected = new DateTime(2019, 12, 07);
            //Actual
            var actual = InfoChecker.DateArriere(param);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LasteDatesTest()
        {
            //Arrange
            int param = 3;
            List<DateTime> expected = new List<DateTime>() { new DateTime(2019, 12, 09), new DateTime(2019, 12, 08), new DateTime(2019, 12, 07) };
            //Actual
            var actual = InfoChecker.LasteDates(param);
            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void VenteByDateTest()
        {
            //Arrange
            int expected = 4;
            //Actual
            int actual = DbManager.GetVenteByDate(InfoChecker.AjustDate(new DateTime(2019, 11, 28))).Count;
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetProduiVenduByProdIdVenteIdTest()
        {
            //Arrange
            int expected = 1;
            //Actual
            int actual = DbManager.GetProduiVenduByProdIdVenteId(2, 1).Count;
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test()
        {
            //Arrange
            string expected = "10/12/2019";
            //Actual
            string actual = DateTime.Now.ToShortDateString();
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetModuleByIdTest()
        {
            //Arrange
            Module exepted = new Module() { Id = 2, Libelle = "ModuleTwo" };
            //Actual
            var a = DbManager.GetById<Module>(2);
            var actual = a;
            //Assert
            Assert.AreEqual(exepted.Libelle, actual.Libelle);
        }

        [TestMethod]
        public void DateDiffTest()
        {
            //Arrange
            DateTime dt = new DateTime(2019, 12, 13, 12, 30, 00);
            DateTime dt2 = new DateTime(2019, 12, 14, 00, 30, 00);
            double expected = 0.5;
            //Actual
            var actual = InfoChecker.DateDiff(dt, dt2);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NexDateTest()
        {
            //Arrange
            DateTime d = new DateTime(2019, 12, 01);
            DateTime expeted = new DateTime(2019, 12, 05);
            int added = 4;
            //Actual
            var actual = InfoChecker.NextDate(d, added);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void IsPassOkTest()
        {
            //Arrange
            string pass = "azerty";
            bool expeted = false;
            //Actual
            var actual = InfoChecker.IsPassOk(pass);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void IsWithinDateTest()
        {
            //Arrange
            DateTime debut = new DateTime(2020, 01, 02, 08, 30, 45);
            DateTime fin = new DateTime(2020, 01, 08, 15, 45, 45);
            DateTime tocheck = new DateTime(2020, 01, 05, 15, 45, 45);
            bool expeted = true;
            //Actual
            var actual = InfoChecker.IsWithinDate(debut, fin, tocheck);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void HasExpireTest()
        {
            //Arrange
            DateTime fin = new DateTime(2020, 01, 11, 15, 45, 45);
            DateTime tocheck = new DateTime(2020, 01, 12, 15, 45, 45);
            bool expeted = true;
            //Actual
            var actual = InfoChecker.HasExpire(fin, tocheck);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void AjustDateWithTimeTest()
        {
            //Arrange
            DateTime date = new DateTime(2020,01,10,12,17,44);
            string expeted = "2020-01-10 12:17:44";
            //Actual
            var actual = InfoChecker.AjustDateWithTime(date);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void AjustDateWithTimeDMYTTest()
        {
            //Arrange
            DateTime date = new DateTime(2020, 01, 10, 12, 17, 44);
            string expeted = "10-01-2020 12:17:44";
            //Actual
            var actual = InfoChecker.AjustDateWithTimeDMYT(date);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void ConvertTODecimalTest()
        {
            //Arrange
            decimal expeted = 12.3M;
            //Actual
            var actual = InfoChecker.ConvertTODecimal("12.3");
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void EmailParternTest()
        {
            //Arrange
            string str = "elian2o@gmail.com";
            bool expeted = true;
            //Actual
            var actual = InfoChecker.MailRegExp(str);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void CheckColunm()
        {
            //Arrange
            string str = "ValueDiscount";
            bool expeted = false;
            //Actual
            var actual = DbManager.CustumQueryCheckColumn<Vente>(str);
            //Assert
            Assert.AreEqual(expeted, actual);
        }

        [TestMethod]
        public void WriteCsvTest()
        {
            //Arrange
            List<Language> langs = new List<Language>
            {
                new Language{Code="fr",Libelle="francais",Id=1},
                new Language{Code="en",Libelle="Anglais",Id=2},
                new Language{Code="br",Libelle="belgique",Id=3},
                new Language{Code="tg",Libelle="togo",Id=4}
            };
            bool expeted = true;
            //Actual
            var actual = CSVManager.WriteDatas<Language>("Lang.csv", langs);
            //Assert
            Assert.AreEqual(expeted, actual);
        }



    }
}
