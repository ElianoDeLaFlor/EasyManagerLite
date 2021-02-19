use EasyManagerDb;

CREATE TABLE "BackupInfo" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Dir"	varchar(120),
	"Date"	DateTime,
	"LastBackupDate"	TEXT
);

CREATE TABLE "AppUserInfo" (
	"Id"	varchar(100),
	"Nom"	varchar(120),
	"Contact"	varchar(120),
	"Email"	varchar(150),
	PRIMARY KEY("Id")
);

CREATE TABLE "BackupInfoServer" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Dir"	varchar(120),
	"Date"	DateTime,
	"LastBackupDate"	varchar(120),
	"AppId"	varchar(120)
);

CREATE TABLE "Caisse" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"OperationCaisseId"	INTEGER,
	"Montant"	REAL
);

CREATE TABLE "Categorie" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Libelle"	varchar(120),
	"Description"	varchar(120)
);

CREATE TABLE "Client" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Nom"	varchar(120),
	"Prenom"	varchar(120),
	"Contact"	varchar(120),
	"ClientType"	INTEGER DEFAULT 0
);

CREATE TABLE "CompanyInfo" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Nom"	varchar(120),
	"Contact"	varchar(120),
	"Email"	varchar(120),
	"Consigne"	varchar(120)
);

CREATE TABLE "Discount" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Taux"	REAL,
	"DateDebut"	DateTime,
	"DateFin"	DateTime,
	"ProduitNom"	varchar(120),
	"CategorieId"	INTEGER,
	"ClientId"	INTEGER,
	"Canceled"	INTEGER,
	"IsValidForCredit"	INTEGER,
	"UserId"	INTEGER
);

CREATE TABLE "Language" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Libelle"	varchar(120),
	"Code"	varchar(120)
);

CREATE TABLE "LicenceInformation" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Name"	varchar(120),
	"PaymentDate"	varchar(120),
	"PaymentMethod"	varchar(120),
	"Duration"	varchar(120),
	"StartDate"	DateTime,
	"EndDate"	DateTime,
	"HasExpired"	INTEGER,
	"Code"	varchar(120),
	"TypeLicence"	INTEGER DEFAULT 0
);

CREATE TABLE "LicenceRegInfoServer" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Start"	DateTime,
	"End"	DateTime,
	"Version"	varchar(120),
	"EasyId"	INTEGER,
	"Type"	INTEGER DEFAULT 0,
	"AppKey"	varchar(120),
	"Status"	INTEGER
);

CREATE TABLE "Module" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Libelle"	varchar(120)
);

CREATE TABLE "Notifications" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Date"	DateTime,
	"ProduitNom"	varchar(120),
	"ProduitQuantiteRestante"	REAL,
	"Message"	varchar(120),
	"Couleur"	varchar(120),
	"IsApprovisionnement"	INTEGER
);

CREATE TABLE "Operation" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Libelle"	varchar(120),
	"TypeOperation"	INTEGER
);

CREATE TABLE "OperationCaisse" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"UtilisateurId"	INTEGER,
	"Date"	DateTime,
	"OperationId"	INTEGER,
	"Montant"	REAL
);

CREATE TABLE "Produit" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Nom"	varchar(120),
	"Description"	varchar(120),
	"QuantiteTotale"	REAL,
	"PrixUnitaire"	REAL,
	"CategorieId"	INTEGER,
	"QuantiteAlerte"	REAL,
	"QuantiteRestante"	REAL,
	"SupplierId"	INTEGER,
	"PrixGrossiste"	REAL DEFAULT 0
);

CREATE TABLE "ProduitCredit" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"ProduitId"	INTEGER,
	"Quantite"	REAL,
	"CommandeId"	INTEGER,
	"PrixUnitaire"	REAL,
	"Montant"	REAL,
	"QuantiteRestante"	REAL,
	"Discount"	REAL
);

CREATE TABLE "ProduitVendu" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"ProduitId"	INTEGER,
	"Quantite"	REAL,
	"VenteId"	INTEGER,
	"PrixUnitaire"	REAL,
	"Montant"	REAL,
	"Discount"	REAL
);

CREATE TABLE "QuantiteEdition" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"ProduitId"	INTEGER,
	"DateEdition"	varchar(120),
	"UtilisateurId"	INTEGER,
	"Quantite"	INTEGER,
	"PrixUnitaire"	INTEGER
);

CREATE TABLE "Reglement" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Date"	varchar(120),
	"VenteCreditId"	INTEGER,
	"UtilisateurId"	INTEGER,
	"Montant"	REAL
);

CREATE TABLE "Role" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Libelle"	varchar(120)
);

CREATE TABLE "RoleModule" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"RoleId"	INTEGER,
	"ModuleId"	INTEGER
);

CREATE TABLE "Settings" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Name"	varchar(120) NOT NULL,
	"Data"	varchar(120) NOT NULL,
	"CreationDate"	DateTime NOT NULL
);

CREATE TABLE "ShopLogo" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Name"	varchar(120) NOT NULL,
	"CreationDate"	DateTime NOT NULL
);

CREATE TABLE "Supplier" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Nom"	varchar(120),
	"Contact"	varchar(120),
	"Email"	varchar(120)
);

CREATE TABLE "TVA" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Taux"	REAL,
	"Apply"	INTEGER
);

CREATE TABLE "UserRole" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"UtilisateurId"	INTEGER,
	"RoleId"	INTEGER
);

CREATE TABLE "Utilisateur" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"Nom"	varchar(120),
	"Prenom"	varchar(120),
	"Password"	varchar(120),
	"Login"	varchar(120),
	"RoleLibelle"	varchar(120),
	"PassDate"	DateTime,
	"Deleted"	INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE "Vente" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"CommandeId"	INTEGER,
	"Montant"	REAL,
	"Date"	varchar(120),
	"UtilisateurId"	INTEGER,
	"ClientId"	INTEGER,
	"ValueDiscount"	REAL DEFAULT 0,
	"Canceled"	INTEGER DEFAULT 0,
	"CanceledDate"	DateTime DEFAULT NULL
);

CREATE TABLE "VenteCredit" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"ClientId"	INTEGER,
	"Date"	varchar(120),
	"UtilisateurId"	INTEGER,
	"Montant"	REAL,
	"MontantRestant"	REAL,
	"ValueDiscount"	REAL DEFAULT 0,
	"Canceled"	INTEGER DEFAULT 0,
	"CanceledDate"	DateTime DEFAULT NULL
);

CREATE TABLE "VersionInfo" (
	"Id"	INTEGER PRIMARY KEY Identity(1,1),
	"VersionNumber"	varchar(120),
	"UpdateTime"	varchar(120)
);