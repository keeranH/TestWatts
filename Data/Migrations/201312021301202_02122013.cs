namespace Econocom.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _02122013 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BENCHMARK.Adresse",
                c => new
                    {
                        IdAdresse = c.Int(nullable: false, identity: true),
                        PaysId = c.Int(nullable: false),
                        Adresse1 = c.String(),
                        Adresse2 = c.String(),
                        Adresse3 = c.String(),
                        CodePostal = c.String(),
                        Region = c.String(),
                        Ville = c.String(),
                    })
                .PrimaryKey(t => t.IdAdresse)
                .ForeignKey("BENCHMARK.Pays", t => t.PaysId, cascadeDelete: true)
                .Index(t => t.PaysId);
            
            CreateTable(
                "BENCHMARK.Pays",
                c => new
                    {
                        IdPays = c.Int(nullable: false),
                        LibellePays = c.String(),
                        CoefficientConversion = c.Decimal(precision: 18, scale: 4),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdPays);
            
            CreateTable(
                "BENCHMARK.Support",
                c => new
                    {
                        IdSupport = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Prenom = c.String(),
                        Entreprise = c.String(),
                        Role = c.String(),
                        Ville = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Sujet = c.String(),
                        Contenu = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        PaysId = c.Int(),
                    })
                .PrimaryKey(t => t.IdSupport)
                .ForeignKey("BENCHMARK.Pays", t => t.PaysId)
                .Index(t => t.PaysId);
            
            CreateTable(
                "BENCHMARK.AgeDevice",
                c => new
                    {
                        IdAgeDevice = c.Int(nullable: false),
                        SecteurActiviteId = c.Int(),
                        ClientId = c.Int(),
                        LibelleAgeDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdAgeDevice)
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId)
                .Index(t => t.SecteurActiviteId);
            
            CreateTable(
                "BENCHMARK.SecteurActivite",
                c => new
                    {
                        IdSecteurActivite = c.Int(nullable: false),
                        LibelleSecteurActivite = c.String(),
                        SegmentActivite = c.String(),
                        TypeActivite = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdSecteurActivite);
            
            CreateTable(
                "BENCHMARK.ClasseDevice",
                c => new
                    {
                        IdClasseDevice = c.Int(nullable: false),
                        SecteurActiviteId = c.Int(),
                        ClientId = c.Int(),
                        LibelleClasseDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdClasseDevice)
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId)
                .Index(t => t.SecteurActiviteId);
            
            CreateTable(
                "BENCHMARK.ConsoWattHeur",
                c => new
                    {
                        ClasseDeviceId = c.Int(nullable: false),
                        TypeDeviceId = c.Int(nullable: false),
                        AgeDeviceId = c.Int(nullable: false),
                        Consommation = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.ClasseDeviceId, t.TypeDeviceId, t.AgeDeviceId })
                .ForeignKey("BENCHMARK.AgeDevice", t => t.AgeDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.ClasseDevice", t => t.ClasseDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId, cascadeDelete: true)
                .Index(t => t.AgeDeviceId)
                .Index(t => t.ClasseDeviceId)
                .Index(t => t.TypeDeviceId);
            
            CreateTable(
                "BENCHMARK.TypeDevice",
                c => new
                    {
                        IdTypeDevice = c.Int(nullable: false),
                        CategorieDeviceId = c.Int(),
                        LibelleTypeDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeDevice)
                .ForeignKey("BENCHMARK.CategorieDevice", t => t.CategorieDeviceId)
                .Index(t => t.CategorieDeviceId);
            
            CreateTable(
                "BENCHMARK.CategorieDevice",
                c => new
                    {
                        IdCategorieDevice = c.Int(nullable: false),
                        FamilleDeviceId = c.Int(),
                        ClientId = c.Int(),
                        LibelleCategorieDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdCategorieDevice)
                .ForeignKey("BENCHMARK.FamilleDevice", t => t.FamilleDeviceId)
                .Index(t => t.FamilleDeviceId);
            
            CreateTable(
                "BENCHMARK.FamilleDevice",
                c => new
                    {
                        IdFamilleDevice = c.Int(nullable: false),
                        SecteurActiviteId = c.Int(),
                        ClientId = c.Int(),
                        LibelleFamilleDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdFamilleDevice)
                .ForeignKey("BENCHMARK.Client", t => t.ClientId)
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId)
                .Index(t => t.ClientId)
                .Index(t => t.SecteurActiviteId);
            
            CreateTable(
                "BENCHMARK.Client",
                c => new
                    {
                        IdClient = c.Int(nullable: false, identity: true),
                        AdresseId = c.Int(),
                        SecteurActiviteId = c.Int(),
                        DetailsClientId = c.Int(),
                        RaisonSociale = c.String(),
                        IdentificationNational = c.String(),
                        Groupe = c.String(),
                        TVAIntraComm = c.String(),
                        CodeNAF = c.String(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                        DateValidation = c.DateTime(),
                        Statut = c.Int(nullable: false),
                        Nom = c.String(),
                        ContactEconocom = c.String(),
                        ClientEconocom = c.Boolean(nullable: false),
                        CodeInterneWattsGreen = c.String(),
                    })
                .PrimaryKey(t => t.IdClient)
                .ForeignKey("BENCHMARK.Adresse", t => t.AdresseId)
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId)
                .ForeignKey("BENCHMARK.DetailsClient", t => t.DetailsClientId)
                .Index(t => t.AdresseId)
                .Index(t => t.SecteurActiviteId)
                .Index(t => t.DetailsClientId);
            
            CreateTable(
                "BENCHMARK.Contact",
                c => new
                    {
                        IdContact = c.Int(nullable: false, identity: true),
                        TypeContactId = c.Int(),
                        ClientId = c.Int(nullable: false),
                        NomContact = c.String(),
                        PrenomContact = c.String(),
                        Fonction = c.String(),
                        NumeroPhone = c.String(),
                        NumeroGSM = c.String(),
                        Email = c.String(),
                        MotPasse = c.String(),
                        Authorisations = c.Int(),
                        CodeVerification = c.String(),
                        DateDebut = c.DateTime(),
                        DateModification = c.DateTime(),
                        DateFin = c.DateTime(),
                        Valider = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdContact)
                .ForeignKey("BENCHMARK.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.TypeContact", t => t.TypeContactId)
                .Index(t => t.ClientId)
                .Index(t => t.TypeContactId);
            
            CreateTable(
                "BENCHMARK.TypeContact",
                c => new
                    {
                        IdTypeContact = c.Int(nullable: false),
                        LibelleTypeContact = c.String(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeContact);
            
            CreateTable(
                "BENCHMARK.Simulation",
                c => new
                    {
                        IdSimulation = c.Int(nullable: false, identity: true),
                        PolitiqueId = c.Int(),
                        ContactId = c.Int(),
                        OrigineId = c.Int(),
                        ConfigSiSourceId = c.Int(),
                        ConfigSiSimuleId = c.Int(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                        LibelleSimulation = c.String(),
                        Statut = c.Int(nullable: false),
                        ConfigSi_Id = c.Int(),
                    })
                .PrimaryKey(t => t.IdSimulation)
                .ForeignKey("BENCHMARK.Contact", t => t.ContactId)
                .ForeignKey("BENCHMARK.Origine", t => t.OrigineId)
                .ForeignKey("BENCHMARK.Politique", t => t.PolitiqueId)
                .ForeignKey("BENCHMARK.ConfigSi", t => t.ConfigSi_Id)
                .ForeignKey("BENCHMARK.ConfigSi", t => t.ConfigSiSourceId)
                .ForeignKey("BENCHMARK.ConfigSi", t => t.ConfigSiSimuleId)
                .Index(t => t.ContactId)
                .Index(t => t.OrigineId)
                .Index(t => t.PolitiqueId)
                .Index(t => t.ConfigSi_Id)
                .Index(t => t.ConfigSiSourceId)
                .Index(t => t.ConfigSiSimuleId);
            
            CreateTable(
                "BENCHMARK.Origine",
                c => new
                    {
                        IdOrigine = c.Int(nullable: false),
                        LibelleOrigine = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdOrigine);
            
            CreateTable(
                "BENCHMARK.DeviceClient",
                c => new
                    {
                        IdDeviceClient = c.Int(nullable: false, identity: true),
                        ClasseDeviceId = c.Int(),
                        AgeDeviceId = c.Int(),
                        TypeDeviceId = c.Int(),
                        ClientId = c.Int(),
                        OrigineId = c.Int(),
                        Quantite = c.Int(nullable: false),
                        DateDebut = c.Int(nullable: false),
                        DateFin = c.Int(),
                    })
                .PrimaryKey(t => t.IdDeviceClient)
                .ForeignKey("BENCHMARK.AgeDevice", t => t.AgeDeviceId)
                .ForeignKey("BENCHMARK.ClasseDevice", t => t.ClasseDeviceId)
                .ForeignKey("BENCHMARK.Client", t => t.ClientId)
                .ForeignKey("BENCHMARK.Origine", t => t.OrigineId)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId)
                .Index(t => t.AgeDeviceId)
                .Index(t => t.ClasseDeviceId)
                .Index(t => t.ClientId)
                .Index(t => t.OrigineId)
                .Index(t => t.TypeDeviceId);
            
            CreateTable(
                "BENCHMARK.Politique",
                c => new
                    {
                        IdPolitique = c.Int(nullable: false),
                        LibellePolitique = c.String(),
                        Device = c.String(),
                        Objectif = c.String(),
                        Benefice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdPolitique);
            
            CreateTable(
                "BENCHMARK.ConfigSi",
                c => new
                    {
                        IdConfigSi = c.Int(nullable: false, identity: true),
                        LibelleConfigSi = c.String(),
                        DateModification = c.DateTime(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        Statut = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdConfigSi);
            
            CreateTable(
                "BENCHMARK.Resultat",
                c => new
                    {
                        IdResultat = c.Int(nullable: false, identity: true),
                        ConsommationWatt = c.Decimal(precision: 18, scale: 4),
                        ConsommationCo2 = c.Decimal(precision: 18, scale: 4),
                        CoefficientConversion = c.Decimal(precision: 18, scale: 4),
                        ConfigSiId = c.Int(),
                        ConfigSiDeviceId = c.Int(),
                    })
                .PrimaryKey(t => t.IdResultat)
                .ForeignKey("BENCHMARK.ConfigSi", t => t.ConfigSiId)
                .ForeignKey("BENCHMARK.ConfigSiDevice", t => t.ConfigSiDeviceId)
                .Index(t => t.ConfigSiId)
                .Index(t => t.ConfigSiDeviceId);
            
            CreateTable(
                "BENCHMARK.ConfigSiDevice",
                c => new
                    {
                        IdConfigSiDevice = c.Int(nullable: false, identity: true),
                        NbrJourTravaille = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NbrHeuresIntensifJour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NbrHeuresOffJour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtiliseEnergieExterne = c.Boolean(nullable: false),
                        PourcentConsomExtern = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CoeffReductionWattExterne = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NbrHeuresNonIntensifJour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CoeffNonIntensif = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CoeffModeOff = c.Decimal(precision: 18, scale: 4),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        Quantite = c.Int(nullable: false),
                        LibelleDevice = c.String(),
                        DateModification = c.DateTime(),
                        Source = c.Int(nullable: false),
                        TypeDeviceId = c.Int(nullable: false),
                        ConfigSiId = c.Int(),
                        TypeUsageId = c.Int(),
                    })
                .PrimaryKey(t => t.IdConfigSiDevice)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.ConfigSi", t => t.ConfigSiId)
                .ForeignKey("BENCHMARK.TypeUsage", t => t.TypeUsageId)
                .Index(t => t.TypeDeviceId)
                .Index(t => t.ConfigSiId)
                .Index(t => t.TypeUsageId);
            
            CreateTable(
                "BENCHMARK.TypeUsage",
                c => new
                    {
                        IdTypeUsage = c.Int(nullable: false),
                        LibelleTypeUsage = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeUsage);
            
            CreateTable(
                "BENCHMARK.ConfigSiConsommation",
                c => new
                    {
                        IdConfigSiConsommation = c.Int(nullable: false, identity: true),
                        ConfigSiDeviceId = c.Int(),
                        ClasseDeviceId = c.Int(nullable: false),
                        AgeDeviceId = c.Int(nullable: false),
                        ConsommationIntensif = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ConsommationNonIntensif = c.Decimal(nullable: false, precision: 18, scale: 4),
                        Quantite = c.Int(nullable: false),
                        DateModification = c.DateTime(nullable: false),
                        ConsommationWatt = c.Decimal(precision: 18, scale: 4),
                        ConsommationCo2 = c.Decimal(precision: 18, scale: 4),
                        CoefficientConversion = c.Decimal(precision: 18, scale: 4),
                        ConsommationIntensifUnitaire = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.IdConfigSiConsommation)
                .ForeignKey("BENCHMARK.ClasseDevice", t => t.ClasseDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.AgeDevice", t => t.AgeDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.ConfigSiDevice", t => t.ConfigSiDeviceId)
                .Index(t => t.ClasseDeviceId)
                .Index(t => t.AgeDeviceId)
                .Index(t => t.ConfigSiDeviceId);
            
            CreateTable(
                "BENCHMARK.ConfigSiClient",
                c => new
                    {
                        IdConfigSiClient = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(),
                        ConfigSiId = c.Int(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdConfigSiClient)
                .ForeignKey("BENCHMARK.Client", t => t.ClientId)
                .ForeignKey("BENCHMARK.ConfigSi", t => t.ConfigSiId)
                .Index(t => t.ClientId)
                .Index(t => t.ConfigSiId);
            
            CreateTable(
                "BENCHMARK.Reponse",
                c => new
                    {
                        IdReponse = c.Int(nullable: false, identity: true),
                        Valeur = c.String(),
                        QuestionId = c.Int(nullable: false),
                        ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdReponse)
                .ForeignKey("BENCHMARK.Question", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.Contact", t => t.ContactId, cascadeDelete: true)
                .Index(t => t.QuestionId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "BENCHMARK.Question",
                c => new
                    {
                        IdQuestion = c.Int(nullable: false, identity: true),
                        LibelleQuestion = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdQuestion);
            
            CreateTable(
                "BENCHMARK.DetailsClient",
                c => new
                    {
                        IdDetailClient = c.Int(nullable: false, identity: true),
                        TauxEquipement = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ChiffreAffaire = c.Int(),
                        NombreSite = c.Int(nullable: false),
                        PrixKWH = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SuperficieDataCenter = c.Int(),
                        SuperficieTotale = c.Int(),
                        CodeInterneClient = c.String(),
                        Effectif = c.Int(nullable: false),
                        TarifId = c.Int(),
                        DeviseId = c.Int(),
                    })
                .PrimaryKey(t => t.IdDetailClient)
                .ForeignKey("BENCHMARK.Tarif", t => t.TarifId)
                .ForeignKey("BENCHMARK.Devise", t => t.DeviseId)
                .Index(t => t.TarifId)
                .Index(t => t.DeviseId);
            
            CreateTable(
                "BENCHMARK.Tarif",
                c => new
                    {
                        IdTarif = c.Int(nullable: false, identity: true),
                        LibelleTarif = c.String(),
                        NbreMinDevice = c.Int(nullable: false),
                        NbreMaxDevice = c.Int(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                        TarifAnnuel = c.Int(),
                    })
                .PrimaryKey(t => t.IdTarif);
            
            CreateTable(
                "BENCHMARK.Devise",
                c => new
                    {
                        IdDevise = c.Int(nullable: false, identity: true),
                        CodeDevise = c.String(),
                        Libelle = c.String(),
                        UrlImage = c.String(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdDevise);
            
            CreateTable(
                "BENCHMARK.ConsommationDeviceClient",
                c => new
                    {
                        IdConsommationDeviceClient = c.Int(nullable: false, identity: true),
                        ClasseDeviceId = c.Int(nullable: false),
                        AgeDeviceId = c.Int(nullable: false),
                        TypeDeviceId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        Consommation = c.Int(nullable: false),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IdConsommationDeviceClient)
                .ForeignKey("BENCHMARK.AgeDevice", t => t.AgeDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.ClasseDevice", t => t.ClasseDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.Client", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId, cascadeDelete: true)
                .Index(t => t.AgeDeviceId)
                .Index(t => t.ClasseDeviceId)
                .Index(t => t.ClientId)
                .Index(t => t.TypeDeviceId);
            
            CreateTable(
                "BENCHMARK.Ratio",
                c => new
                    {
                        SecteurActiviteId = c.Int(nullable: false),
                        TypeDeviceId = c.Int(nullable: false),
                        TypeRatioId = c.Int(nullable: false),
                        Valeur = c.Decimal(precision: 18, scale: 4),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.SecteurActiviteId, t.TypeDeviceId })
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.TypeRatio", t => t.TypeRatioId, cascadeDelete: true)
                .Index(t => t.SecteurActiviteId)
                .Index(t => t.TypeDeviceId)
                .Index(t => t.TypeRatioId);
            
            CreateTable(
                "BENCHMARK.TypeRatio",
                c => new
                    {
                        IdTypeRatio = c.Int(nullable: false),
                        LibelleTypeRatio = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeRatio);
            
            CreateTable(
                "BENCHMARK.VentilationClasseAgeDevice",
                c => new
                    {
                        SecteurActiviteId = c.Int(nullable: false),
                        ClasseDeviceId = c.Int(nullable: false),
                        TypeDeviceId = c.Int(nullable: false),
                        AgeDeviceId = c.Int(nullable: false),
                        Coefficient = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.SecteurActiviteId, t.ClasseDeviceId, t.TypeDeviceId, t.AgeDeviceId })
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.AgeDevice", t => t.AgeDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.ClasseDevice", t => t.ClasseDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId, cascadeDelete: true)
                .Index(t => t.SecteurActiviteId)
                .Index(t => t.AgeDeviceId)
                .Index(t => t.ClasseDeviceId)
                .Index(t => t.TypeDeviceId);
            
            CreateTable(
                "BENCHMARK.VentilationClasseDevice",
                c => new
                    {
                        SecteurActiviteId = c.Int(nullable: false),
                        TypeDeviceId = c.Int(nullable: false),
                        ClasseDeviceId = c.Int(nullable: false),
                        Coefficient = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.SecteurActiviteId, t.TypeDeviceId, t.ClasseDeviceId })
                .ForeignKey("BENCHMARK.ClasseDevice", t => t.ClasseDeviceId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId, cascadeDelete: true)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId, cascadeDelete: true)
                .Index(t => t.ClasseDeviceId)
                .Index(t => t.SecteurActiviteId)
                .Index(t => t.TypeDeviceId);
            
            CreateTable(
                "BENCHMARK.ConsommationType",
                c => new
                    {
                        IdConsommationType = c.Int(nullable: false, identity: true),
                        LibelleConsommationType = c.String(),
                        ValeurConso = c.Int(nullable: false),
                        Afficher = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdConsommationType);
            
            CreateTable(
                "BENCHMARK.Equivalence",
                c => new
                    {
                        IdEquivalence = c.Int(nullable: false, identity: true),
                        LibelleEquivalence = c.String(),
                        Valeur = c.Double(nullable: false),
                        Mesure = c.String(),
                        Afficher = c.Boolean(nullable: false),
                        LienImage = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdEquivalence);
            
            CreateTable(
                "BENCHMARK.TypeObjet",
                c => new
                    {
                        IdTypeObjet = c.Int(nullable: false),
                        NomTable = c.String(),
                        NomObjet = c.String(),
                        Namespace = c.String(),
                    })
                .PrimaryKey(t => t.IdTypeObjet);
            
            CreateTable(
                "BENCHMARK.Usage",
                c => new
                    {
                        IdUsage = c.Int(nullable: false, identity: true),
                        TypeUsageId = c.Int(nullable: false),
                        NbrJourTravaille = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NbrHeuresIntensifJour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NbrHeuresNonIntensifJour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NbrHeuresOffJour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtiliseEnergieExterne = c.Boolean(nullable: false),
                        CoeffNonIntensif = c.Decimal(precision: 18, scale: 4),
                        CoeffModeOff = c.Decimal(precision: 18, scale: 4),
                        CoeffReductionWattExterne = c.Decimal(precision: 18, scale: 4),
                        PourcentageConsoExterne = c.Decimal(precision: 18, scale: 4),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdUsage)
                .ForeignKey("BENCHMARK.TypeUsage", t => t.TypeUsageId, cascadeDelete: true)
                .Index(t => t.TypeUsageId);
            
            CreateTable(
                "BENCHMARK.CompteEvaluation",
                c => new
                    {
                        IdCompteEvaluation = c.Int(nullable: false, identity: true),
                        RaisonSociale = c.String(),
                        Groupe = c.String(),
                        Nom = c.String(),
                        Prenom = c.String(),
                        Fonction = c.String(),
                        Email = c.String(),
                        NumeroPhone = c.String(),
                        Effectif = c.Int(nullable: false),
                        TauxEquipement = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NombreSites = c.Int(nullable: false),
                        PrixMoyenKwatt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaysId = c.Int(),
                        SecteurActiviteId = c.Int(),
                        DeviseId = c.Int(),
                    })
                .PrimaryKey(t => t.IdCompteEvaluation)
                .ForeignKey("BENCHMARK.Pays", t => t.PaysId)
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId)
                .ForeignKey("BENCHMARK.Devise", t => t.DeviseId)
                .Index(t => t.PaysId)
                .Index(t => t.SecteurActiviteId)
                .Index(t => t.DeviseId);
            
            CreateTable(
                "BENCHMARK.Document",
                c => new
                    {
                        IdDocument = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        DateCreation = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                        Repertoire = c.String(),
                    })
                .PrimaryKey(t => t.IdDocument);
            
            CreateTable(
                "BENCHMARK.ChangementAnneeCalendaire",
                c => new
                    {
                        IdChangementAnneeCalendaire = c.Int(nullable: false),
                        AgeDeviceId = c.Int(nullable: false),
                        CoefficientSource = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CoefficientDestination = c.Decimal(nullable: false, precision: 18, scale: 4),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                        Expression = c.String(),
                        Variable = c.String(),
                    })
                .PrimaryKey(t => t.IdChangementAnneeCalendaire);
            
            CreateTable(
                "BENCHMARK.ContenuMail",
                c => new
                    {
                        IdContenuMail = c.Int(nullable: false, identity: true),
                        TypeMailId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        Contenu = c.String(),
                        Sujet = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdContenuMail)
                .ForeignKey("BENCHMARK.TypeMail", t => t.TypeMailId, cascadeDelete: true)
                .Index(t => t.TypeMailId);
            
            CreateTable(
                "BENCHMARK.TypeMail",
                c => new
                    {
                        IdTypeMail = c.Int(nullable: false),
                        LibelleTypeMail = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeMail);
            
            CreateTable(
                "BENCHMARK.Rapport",
                c => new
                    {
                        IdRapport = c.Int(nullable: false, identity: true),
                        ContactId = c.Int(),
                        OrigineId = c.Int(),
                        ConfigSiId = c.Int(),
                        DateDebut = c.DateTime(nullable: false),
                        HeureCree = c.Time(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                        LibelleRapport = c.String(),
                    })
                .PrimaryKey(t => t.IdRapport)
                .ForeignKey("BENCHMARK.Contact", t => t.ContactId)
                .ForeignKey("BENCHMARK.Origine", t => t.OrigineId)
                .ForeignKey("BENCHMARK.ConfigSi", t => t.ConfigSiId)
                .Index(t => t.ContactId)
                .Index(t => t.OrigineId)
                .Index(t => t.ConfigSiId);
            
            CreateTable(
                "BENCHMARK.UsageDevice",
                c => new
                    {
                        IdUsageDevice = c.Int(nullable: false, identity: true),
                        UsageId = c.Int(),
                        TypeDeviceId = c.Int(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdUsageDevice)
                .ForeignKey("BENCHMARK.Usage", t => t.UsageId)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId)
                .Index(t => t.UsageId)
                .Index(t => t.TypeDeviceId);
            
            CreateTable(
                "BENCHMARK.Utilisateur",
                c => new
                    {
                        IdUtilisateur = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        MotDePasse = c.String(),
                        Authorization = c.Int(nullable: false),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdUtilisateur);
            
            CreateTable(
                "CMS.Langue",
                c => new
                    {
                        IdLangue = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Nom = c.String(),
                        Culture = c.String(),
                        Defaut = c.Boolean(nullable: false),
                        PaysId = c.Int(nullable: false),
                        Localise = c.Boolean(nullable: false),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                        LienImage = c.String(),
                    })
                .PrimaryKey(t => t.IdLangue)
                .ForeignKey("BENCHMARK.Pays", t => t.PaysId, cascadeDelete: true)
                .Index(t => t.PaysId);
            
            CreateTable(
                "CMS.ContenuModere",
                c => new
                    {
                        IdContenuModere = c.Int(nullable: false, identity: true),
                        Contenu = c.String(),
                        PageId = c.Int(),
                        CleModeleId = c.Int(),
                        LangueId = c.Int(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdContenuModere)
                .ForeignKey("CMS.Page", t => t.PageId)
                .ForeignKey("CMS.CleModele", t => t.CleModeleId)
                .ForeignKey("CMS.Langue", t => t.LangueId)
                .Index(t => t.PageId)
                .Index(t => t.CleModeleId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "CMS.CleModele",
                c => new
                    {
                        IdCleModele = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        ModeleId = c.Int(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdCleModele)
                .ForeignKey("CMS.Modele", t => t.ModeleId)
                .Index(t => t.ModeleId);
            
            CreateTable(
                "CMS.Modele",
                c => new
                    {
                        IdModele = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Lien = c.String(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdModele);
            
            CreateTable(
                "CMS.Page",
                c => new
                    {
                        IdPage = c.Int(nullable: false, identity: true),
                        Lien = c.String(),
                        Actif = c.Boolean(nullable: false),
                        Nom = c.String(),
                        SectionId = c.Int(nullable: false),
                        ModeleModereId = c.Int(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdPage)
                .ForeignKey("CMS.Modele", t => t.ModeleModereId)
                .ForeignKey("CMS.Section", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.ModeleModereId)
                .Index(t => t.SectionId);
            
            CreateTable(
                "CMS.Section",
                c => new
                    {
                        IdSection = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        ParentId = c.Int(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdSection)
                .ForeignKey("CMS.Section", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "CMS.Routage",
                c => new
                    {
                        IdRoutage = c.Int(nullable: false, identity: true),
                        ValeurActuelle = c.String(),
                        ValeurTraduite = c.String(),
                        Actif = c.Boolean(nullable: false),
                        PageId = c.Int(),
                        LangueId = c.Int(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdRoutage)
                .ForeignKey("CMS.Langue", t => t.LangueId)
                .ForeignKey("CMS.Page", t => t.PageId)
                .Index(t => t.LangueId)
                .Index(t => t.PageId);
            
            CreateTable(
                "Traduction.PaysTraduction",
                c => new
                    {
                        IdPaysTraduction = c.Int(nullable: false, identity: true),
                        PaysId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibellePays = c.String(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdPaysTraduction)
                .ForeignKey("BENCHMARK.Pays", t => t.PaysId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.PaysId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.OrigineTraduction",
                c => new
                    {
                        IdOrigineTraduction = c.Int(nullable: false, identity: true),
                        OrigineId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleOrigine = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdOrigineTraduction)
                .ForeignKey("BENCHMARK.Origine", t => t.OrigineId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.OrigineId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.PolitiqueTraduction",
                c => new
                    {
                        IdPolitiqueTraduction = c.Int(nullable: false, identity: true),
                        PolitiqueId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibellePolitique = c.String(),
                        Device = c.String(),
                        Objectif = c.String(),
                        Benefice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdPolitiqueTraduction)
                .ForeignKey("BENCHMARK.Politique", t => t.PolitiqueId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.PolitiqueId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.TypeUsageTraduction",
                c => new
                    {
                        IdTypeUsageTraduction = c.Int(nullable: false, identity: true),
                        TypeUsageId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleTypeUsage = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeUsageTraduction)
                .ForeignKey("BENCHMARK.TypeUsage", t => t.TypeUsageId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.TypeUsageId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.QuestionTraduction",
                c => new
                    {
                        IdQuestionTraduction = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleQuestion = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdQuestionTraduction)
                .ForeignKey("BENCHMARK.Question", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.QuestionId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.TarifTraduction",
                c => new
                    {
                        IdTarifTraduction = c.Int(nullable: false, identity: true),
                        TarifId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleTarif = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTarifTraduction)
                .ForeignKey("BENCHMARK.Tarif", t => t.TarifId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.TarifId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.FamilleDeviceTraduction",
                c => new
                    {
                        IdFamilleDeviceTraduction = c.Int(nullable: false, identity: true),
                        FamilleDeviceId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleFamilleDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdFamilleDeviceTraduction)
                .ForeignKey("BENCHMARK.FamilleDevice", t => t.FamilleDeviceId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.FamilleDeviceId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.CategorieDeviceTraduction",
                c => new
                    {
                        IdCategorieDeviceTraduction = c.Int(nullable: false, identity: true),
                        CategorieDeviceId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleCategorieDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdCategorieDeviceTraduction)
                .ForeignKey("BENCHMARK.CategorieDevice", t => t.CategorieDeviceId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.CategorieDeviceId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.TypeRatioTraduction",
                c => new
                    {
                        IdTypeRatioTraduction = c.Int(nullable: false, identity: true),
                        TypeRatioId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleTypeRatio = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeRatioTraduction)
                .ForeignKey("BENCHMARK.TypeRatio", t => t.TypeRatioId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.TypeRatioId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.TypeDeviceTraduction",
                c => new
                    {
                        IdTypeDeviceTraduction = c.Int(nullable: false, identity: true),
                        TypeDeviceId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleTypeDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdTypeDeviceTraduction)
                .ForeignKey("BENCHMARK.TypeDevice", t => t.TypeDeviceId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.TypeDeviceId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.ClasseDeviceTraduction",
                c => new
                    {
                        IdClasseDeviceTraduction = c.Int(nullable: false, identity: true),
                        ClasseDeviceId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleClasseDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdClasseDeviceTraduction)
                .ForeignKey("BENCHMARK.ClasseDevice", t => t.ClasseDeviceId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.ClasseDeviceId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.SecteurActiviteTraduction",
                c => new
                    {
                        IdSecteurActiviteTraduction = c.Int(nullable: false, identity: true),
                        SecteurActiviteId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleSecteurActivite = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdSecteurActiviteTraduction)
                .ForeignKey("BENCHMARK.SecteurActivite", t => t.SecteurActiviteId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.SecteurActiviteId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.AgeDeviceTraduction",
                c => new
                    {
                        IdAgeDeviceTraduction = c.Int(nullable: false, identity: true),
                        AgeDeviceId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleAgeDevice = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdAgeDeviceTraduction)
                .ForeignKey("BENCHMARK.AgeDevice", t => t.AgeDeviceId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.AgeDeviceId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.EquivalenceTraduction",
                c => new
                    {
                        IdEquivalenceTraduction = c.Int(nullable: false, identity: true),
                        EquivalenceId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        LibelleEquivalence = c.String(),
                        Valeur = c.Int(nullable: false),
                        Mesure = c.String(),
                        Afficher = c.Boolean(nullable: false),
                        LienImage = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdEquivalenceTraduction)
                .ForeignKey("BENCHMARK.Equivalence", t => t.EquivalenceId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.EquivalenceId)
                .Index(t => t.LangueId);
            
            CreateTable(
                "Traduction.DeviseTraduction",
                c => new
                    {
                        IdDeviseTraduction = c.Int(nullable: false, identity: true),
                        DeviseId = c.Int(nullable: false),
                        LangueId = c.Int(nullable: false),
                        Libelle = c.String(),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DateModification = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdDeviseTraduction)
                .ForeignKey("BENCHMARK.Devise", t => t.DeviseId, cascadeDelete: true)
                .ForeignKey("CMS.Langue", t => t.LangueId, cascadeDelete: true)
                .Index(t => t.DeviseId)
                .Index(t => t.LangueId);
            
        }
        
        public override void Down()
        {
            DropIndex("Traduction.DeviseTraduction", new[] { "LangueId" });
            DropIndex("Traduction.DeviseTraduction", new[] { "DeviseId" });
            DropIndex("Traduction.EquivalenceTraduction", new[] { "LangueId" });
            DropIndex("Traduction.EquivalenceTraduction", new[] { "EquivalenceId" });
            DropIndex("Traduction.AgeDeviceTraduction", new[] { "LangueId" });
            DropIndex("Traduction.AgeDeviceTraduction", new[] { "AgeDeviceId" });
            DropIndex("Traduction.SecteurActiviteTraduction", new[] { "LangueId" });
            DropIndex("Traduction.SecteurActiviteTraduction", new[] { "SecteurActiviteId" });
            DropIndex("Traduction.ClasseDeviceTraduction", new[] { "LangueId" });
            DropIndex("Traduction.ClasseDeviceTraduction", new[] { "ClasseDeviceId" });
            DropIndex("Traduction.TypeDeviceTraduction", new[] { "LangueId" });
            DropIndex("Traduction.TypeDeviceTraduction", new[] { "TypeDeviceId" });
            DropIndex("Traduction.TypeRatioTraduction", new[] { "LangueId" });
            DropIndex("Traduction.TypeRatioTraduction", new[] { "TypeRatioId" });
            DropIndex("Traduction.CategorieDeviceTraduction", new[] { "LangueId" });
            DropIndex("Traduction.CategorieDeviceTraduction", new[] { "CategorieDeviceId" });
            DropIndex("Traduction.FamilleDeviceTraduction", new[] { "LangueId" });
            DropIndex("Traduction.FamilleDeviceTraduction", new[] { "FamilleDeviceId" });
            DropIndex("Traduction.TarifTraduction", new[] { "LangueId" });
            DropIndex("Traduction.TarifTraduction", new[] { "TarifId" });
            DropIndex("Traduction.QuestionTraduction", new[] { "LangueId" });
            DropIndex("Traduction.QuestionTraduction", new[] { "QuestionId" });
            DropIndex("Traduction.TypeUsageTraduction", new[] { "LangueId" });
            DropIndex("Traduction.TypeUsageTraduction", new[] { "TypeUsageId" });
            DropIndex("Traduction.PolitiqueTraduction", new[] { "LangueId" });
            DropIndex("Traduction.PolitiqueTraduction", new[] { "PolitiqueId" });
            DropIndex("Traduction.OrigineTraduction", new[] { "LangueId" });
            DropIndex("Traduction.OrigineTraduction", new[] { "OrigineId" });
            DropIndex("Traduction.PaysTraduction", new[] { "LangueId" });
            DropIndex("Traduction.PaysTraduction", new[] { "PaysId" });
            DropIndex("CMS.Routage", new[] { "PageId" });
            DropIndex("CMS.Routage", new[] { "LangueId" });
            DropIndex("CMS.Section", new[] { "ParentId" });
            DropIndex("CMS.Page", new[] { "SectionId" });
            DropIndex("CMS.Page", new[] { "ModeleModereId" });
            DropIndex("CMS.CleModele", new[] { "ModeleId" });
            DropIndex("CMS.ContenuModere", new[] { "LangueId" });
            DropIndex("CMS.ContenuModere", new[] { "CleModeleId" });
            DropIndex("CMS.ContenuModere", new[] { "PageId" });
            DropIndex("CMS.Langue", new[] { "PaysId" });
            DropIndex("BENCHMARK.UsageDevice", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.UsageDevice", new[] { "UsageId" });
            DropIndex("BENCHMARK.Rapport", new[] { "ConfigSiId" });
            DropIndex("BENCHMARK.Rapport", new[] { "OrigineId" });
            DropIndex("BENCHMARK.Rapport", new[] { "ContactId" });
            DropIndex("BENCHMARK.ContenuMail", new[] { "TypeMailId" });
            DropIndex("BENCHMARK.CompteEvaluation", new[] { "DeviseId" });
            DropIndex("BENCHMARK.CompteEvaluation", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.CompteEvaluation", new[] { "PaysId" });
            DropIndex("BENCHMARK.Usage", new[] { "TypeUsageId" });
            DropIndex("BENCHMARK.VentilationClasseDevice", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.VentilationClasseDevice", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.VentilationClasseDevice", new[] { "ClasseDeviceId" });
            DropIndex("BENCHMARK.VentilationClasseAgeDevice", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.VentilationClasseAgeDevice", new[] { "ClasseDeviceId" });
            DropIndex("BENCHMARK.VentilationClasseAgeDevice", new[] { "AgeDeviceId" });
            DropIndex("BENCHMARK.VentilationClasseAgeDevice", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.Ratio", new[] { "TypeRatioId" });
            DropIndex("BENCHMARK.Ratio", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.Ratio", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.ConsommationDeviceClient", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.ConsommationDeviceClient", new[] { "ClientId" });
            DropIndex("BENCHMARK.ConsommationDeviceClient", new[] { "ClasseDeviceId" });
            DropIndex("BENCHMARK.ConsommationDeviceClient", new[] { "AgeDeviceId" });
            DropIndex("BENCHMARK.DetailsClient", new[] { "DeviseId" });
            DropIndex("BENCHMARK.DetailsClient", new[] { "TarifId" });
            DropIndex("BENCHMARK.Reponse", new[] { "ContactId" });
            DropIndex("BENCHMARK.Reponse", new[] { "QuestionId" });
            DropIndex("BENCHMARK.ConfigSiClient", new[] { "ConfigSiId" });
            DropIndex("BENCHMARK.ConfigSiClient", new[] { "ClientId" });
            DropIndex("BENCHMARK.ConfigSiConsommation", new[] { "ConfigSiDeviceId" });
            DropIndex("BENCHMARK.ConfigSiConsommation", new[] { "AgeDeviceId" });
            DropIndex("BENCHMARK.ConfigSiConsommation", new[] { "ClasseDeviceId" });
            DropIndex("BENCHMARK.ConfigSiDevice", new[] { "TypeUsageId" });
            DropIndex("BENCHMARK.ConfigSiDevice", new[] { "ConfigSiId" });
            DropIndex("BENCHMARK.ConfigSiDevice", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.Resultat", new[] { "ConfigSiDeviceId" });
            DropIndex("BENCHMARK.Resultat", new[] { "ConfigSiId" });
            DropIndex("BENCHMARK.DeviceClient", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.DeviceClient", new[] { "OrigineId" });
            DropIndex("BENCHMARK.DeviceClient", new[] { "ClientId" });
            DropIndex("BENCHMARK.DeviceClient", new[] { "ClasseDeviceId" });
            DropIndex("BENCHMARK.DeviceClient", new[] { "AgeDeviceId" });
            DropIndex("BENCHMARK.Simulation", new[] { "ConfigSiSimuleId" });
            DropIndex("BENCHMARK.Simulation", new[] { "ConfigSiSourceId" });
            DropIndex("BENCHMARK.Simulation", new[] { "ConfigSi_Id" });
            DropIndex("BENCHMARK.Simulation", new[] { "PolitiqueId" });
            DropIndex("BENCHMARK.Simulation", new[] { "OrigineId" });
            DropIndex("BENCHMARK.Simulation", new[] { "ContactId" });
            DropIndex("BENCHMARK.Contact", new[] { "TypeContactId" });
            DropIndex("BENCHMARK.Contact", new[] { "ClientId" });
            DropIndex("BENCHMARK.Client", new[] { "DetailsClientId" });
            DropIndex("BENCHMARK.Client", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.Client", new[] { "AdresseId" });
            DropIndex("BENCHMARK.FamilleDevice", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.FamilleDevice", new[] { "ClientId" });
            DropIndex("BENCHMARK.CategorieDevice", new[] { "FamilleDeviceId" });
            DropIndex("BENCHMARK.TypeDevice", new[] { "CategorieDeviceId" });
            DropIndex("BENCHMARK.ConsoWattHeur", new[] { "TypeDeviceId" });
            DropIndex("BENCHMARK.ConsoWattHeur", new[] { "ClasseDeviceId" });
            DropIndex("BENCHMARK.ConsoWattHeur", new[] { "AgeDeviceId" });
            DropIndex("BENCHMARK.ClasseDevice", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.AgeDevice", new[] { "SecteurActiviteId" });
            DropIndex("BENCHMARK.Support", new[] { "PaysId" });
            DropIndex("BENCHMARK.Adresse", new[] { "PaysId" });
            DropForeignKey("Traduction.DeviseTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.DeviseTraduction", "DeviseId", "BENCHMARK.Devise");
            DropForeignKey("Traduction.EquivalenceTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.EquivalenceTraduction", "EquivalenceId", "BENCHMARK.Equivalence");
            DropForeignKey("Traduction.AgeDeviceTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.AgeDeviceTraduction", "AgeDeviceId", "BENCHMARK.AgeDevice");
            DropForeignKey("Traduction.SecteurActiviteTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.SecteurActiviteTraduction", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("Traduction.ClasseDeviceTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.ClasseDeviceTraduction", "ClasseDeviceId", "BENCHMARK.ClasseDevice");
            DropForeignKey("Traduction.TypeDeviceTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.TypeDeviceTraduction", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("Traduction.TypeRatioTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.TypeRatioTraduction", "TypeRatioId", "BENCHMARK.TypeRatio");
            DropForeignKey("Traduction.CategorieDeviceTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.CategorieDeviceTraduction", "CategorieDeviceId", "BENCHMARK.CategorieDevice");
            DropForeignKey("Traduction.FamilleDeviceTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.FamilleDeviceTraduction", "FamilleDeviceId", "BENCHMARK.FamilleDevice");
            DropForeignKey("Traduction.TarifTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.TarifTraduction", "TarifId", "BENCHMARK.Tarif");
            DropForeignKey("Traduction.QuestionTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.QuestionTraduction", "QuestionId", "BENCHMARK.Question");
            DropForeignKey("Traduction.TypeUsageTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.TypeUsageTraduction", "TypeUsageId", "BENCHMARK.TypeUsage");
            DropForeignKey("Traduction.PolitiqueTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.PolitiqueTraduction", "PolitiqueId", "BENCHMARK.Politique");
            DropForeignKey("Traduction.OrigineTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.OrigineTraduction", "OrigineId", "BENCHMARK.Origine");
            DropForeignKey("Traduction.PaysTraduction", "LangueId", "CMS.Langue");
            DropForeignKey("Traduction.PaysTraduction", "PaysId", "BENCHMARK.Pays");
            DropForeignKey("CMS.Routage", "PageId", "CMS.Page");
            DropForeignKey("CMS.Routage", "LangueId", "CMS.Langue");
            DropForeignKey("CMS.Section", "ParentId", "CMS.Section");
            DropForeignKey("CMS.Page", "SectionId", "CMS.Section");
            DropForeignKey("CMS.Page", "ModeleModereId", "CMS.Modele");
            DropForeignKey("CMS.CleModele", "ModeleId", "CMS.Modele");
            DropForeignKey("CMS.ContenuModere", "LangueId", "CMS.Langue");
            DropForeignKey("CMS.ContenuModere", "CleModeleId", "CMS.CleModele");
            DropForeignKey("CMS.ContenuModere", "PageId", "CMS.Page");
            DropForeignKey("CMS.Langue", "PaysId", "BENCHMARK.Pays");
            DropForeignKey("BENCHMARK.UsageDevice", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.UsageDevice", "UsageId", "BENCHMARK.Usage");
            DropForeignKey("BENCHMARK.Rapport", "ConfigSiId", "BENCHMARK.ConfigSi");
            DropForeignKey("BENCHMARK.Rapport", "OrigineId", "BENCHMARK.Origine");
            DropForeignKey("BENCHMARK.Rapport", "ContactId", "BENCHMARK.Contact");
            DropForeignKey("BENCHMARK.ContenuMail", "TypeMailId", "BENCHMARK.TypeMail");
            DropForeignKey("BENCHMARK.CompteEvaluation", "DeviseId", "BENCHMARK.Devise");
            DropForeignKey("BENCHMARK.CompteEvaluation", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.CompteEvaluation", "PaysId", "BENCHMARK.Pays");
            DropForeignKey("BENCHMARK.Usage", "TypeUsageId", "BENCHMARK.TypeUsage");
            DropForeignKey("BENCHMARK.VentilationClasseDevice", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.VentilationClasseDevice", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.VentilationClasseDevice", "ClasseDeviceId", "BENCHMARK.ClasseDevice");
            DropForeignKey("BENCHMARK.VentilationClasseAgeDevice", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.VentilationClasseAgeDevice", "ClasseDeviceId", "BENCHMARK.ClasseDevice");
            DropForeignKey("BENCHMARK.VentilationClasseAgeDevice", "AgeDeviceId", "BENCHMARK.AgeDevice");
            DropForeignKey("BENCHMARK.VentilationClasseAgeDevice", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.Ratio", "TypeRatioId", "BENCHMARK.TypeRatio");
            DropForeignKey("BENCHMARK.Ratio", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.Ratio", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.ConsommationDeviceClient", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.ConsommationDeviceClient", "ClientId", "BENCHMARK.Client");
            DropForeignKey("BENCHMARK.ConsommationDeviceClient", "ClasseDeviceId", "BENCHMARK.ClasseDevice");
            DropForeignKey("BENCHMARK.ConsommationDeviceClient", "AgeDeviceId", "BENCHMARK.AgeDevice");
            DropForeignKey("BENCHMARK.DetailsClient", "DeviseId", "BENCHMARK.Devise");
            DropForeignKey("BENCHMARK.DetailsClient", "TarifId", "BENCHMARK.Tarif");
            DropForeignKey("BENCHMARK.Reponse", "ContactId", "BENCHMARK.Contact");
            DropForeignKey("BENCHMARK.Reponse", "QuestionId", "BENCHMARK.Question");
            DropForeignKey("BENCHMARK.ConfigSiClient", "ConfigSiId", "BENCHMARK.ConfigSi");
            DropForeignKey("BENCHMARK.ConfigSiClient", "ClientId", "BENCHMARK.Client");
            DropForeignKey("BENCHMARK.ConfigSiConsommation", "ConfigSiDeviceId", "BENCHMARK.ConfigSiDevice");
            DropForeignKey("BENCHMARK.ConfigSiConsommation", "AgeDeviceId", "BENCHMARK.AgeDevice");
            DropForeignKey("BENCHMARK.ConfigSiConsommation", "ClasseDeviceId", "BENCHMARK.ClasseDevice");
            DropForeignKey("BENCHMARK.ConfigSiDevice", "TypeUsageId", "BENCHMARK.TypeUsage");
            DropForeignKey("BENCHMARK.ConfigSiDevice", "ConfigSiId", "BENCHMARK.ConfigSi");
            DropForeignKey("BENCHMARK.ConfigSiDevice", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.Resultat", "ConfigSiDeviceId", "BENCHMARK.ConfigSiDevice");
            DropForeignKey("BENCHMARK.Resultat", "ConfigSiId", "BENCHMARK.ConfigSi");
            DropForeignKey("BENCHMARK.DeviceClient", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.DeviceClient", "OrigineId", "BENCHMARK.Origine");
            DropForeignKey("BENCHMARK.DeviceClient", "ClientId", "BENCHMARK.Client");
            DropForeignKey("BENCHMARK.DeviceClient", "ClasseDeviceId", "BENCHMARK.ClasseDevice");
            DropForeignKey("BENCHMARK.DeviceClient", "AgeDeviceId", "BENCHMARK.AgeDevice");
            DropForeignKey("BENCHMARK.Simulation", "ConfigSiSimuleId", "BENCHMARK.ConfigSi");
            DropForeignKey("BENCHMARK.Simulation", "ConfigSiSourceId", "BENCHMARK.ConfigSi");
            DropForeignKey("BENCHMARK.Simulation", "ConfigSi_Id", "BENCHMARK.ConfigSi");
            DropForeignKey("BENCHMARK.Simulation", "PolitiqueId", "BENCHMARK.Politique");
            DropForeignKey("BENCHMARK.Simulation", "OrigineId", "BENCHMARK.Origine");
            DropForeignKey("BENCHMARK.Simulation", "ContactId", "BENCHMARK.Contact");
            DropForeignKey("BENCHMARK.Contact", "TypeContactId", "BENCHMARK.TypeContact");
            DropForeignKey("BENCHMARK.Contact", "ClientId", "BENCHMARK.Client");
            DropForeignKey("BENCHMARK.Client", "DetailsClientId", "BENCHMARK.DetailsClient");
            DropForeignKey("BENCHMARK.Client", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.Client", "AdresseId", "BENCHMARK.Adresse");
            DropForeignKey("BENCHMARK.FamilleDevice", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.FamilleDevice", "ClientId", "BENCHMARK.Client");
            DropForeignKey("BENCHMARK.CategorieDevice", "FamilleDeviceId", "BENCHMARK.FamilleDevice");
            DropForeignKey("BENCHMARK.TypeDevice", "CategorieDeviceId", "BENCHMARK.CategorieDevice");
            DropForeignKey("BENCHMARK.ConsoWattHeur", "TypeDeviceId", "BENCHMARK.TypeDevice");
            DropForeignKey("BENCHMARK.ConsoWattHeur", "ClasseDeviceId", "BENCHMARK.ClasseDevice");
            DropForeignKey("BENCHMARK.ConsoWattHeur", "AgeDeviceId", "BENCHMARK.AgeDevice");
            DropForeignKey("BENCHMARK.ClasseDevice", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.AgeDevice", "SecteurActiviteId", "BENCHMARK.SecteurActivite");
            DropForeignKey("BENCHMARK.Support", "PaysId", "BENCHMARK.Pays");
            DropForeignKey("BENCHMARK.Adresse", "PaysId", "BENCHMARK.Pays");
            DropTable("Traduction.DeviseTraduction");
            DropTable("Traduction.EquivalenceTraduction");
            DropTable("Traduction.AgeDeviceTraduction");
            DropTable("Traduction.SecteurActiviteTraduction");
            DropTable("Traduction.ClasseDeviceTraduction");
            DropTable("Traduction.TypeDeviceTraduction");
            DropTable("Traduction.TypeRatioTraduction");
            DropTable("Traduction.CategorieDeviceTraduction");
            DropTable("Traduction.FamilleDeviceTraduction");
            DropTable("Traduction.TarifTraduction");
            DropTable("Traduction.QuestionTraduction");
            DropTable("Traduction.TypeUsageTraduction");
            DropTable("Traduction.PolitiqueTraduction");
            DropTable("Traduction.OrigineTraduction");
            DropTable("Traduction.PaysTraduction");
            DropTable("CMS.Routage");
            DropTable("CMS.Section");
            DropTable("CMS.Page");
            DropTable("CMS.Modele");
            DropTable("CMS.CleModele");
            DropTable("CMS.ContenuModere");
            DropTable("CMS.Langue");
            DropTable("BENCHMARK.Utilisateur");
            DropTable("BENCHMARK.UsageDevice");
            DropTable("BENCHMARK.Rapport");
            DropTable("BENCHMARK.TypeMail");
            DropTable("BENCHMARK.ContenuMail");
            DropTable("BENCHMARK.ChangementAnneeCalendaire");
            DropTable("BENCHMARK.Document");
            DropTable("BENCHMARK.CompteEvaluation");
            DropTable("BENCHMARK.Usage");
            DropTable("BENCHMARK.TypeObjet");
            DropTable("BENCHMARK.Equivalence");
            DropTable("BENCHMARK.ConsommationType");
            DropTable("BENCHMARK.VentilationClasseDevice");
            DropTable("BENCHMARK.VentilationClasseAgeDevice");
            DropTable("BENCHMARK.TypeRatio");
            DropTable("BENCHMARK.Ratio");
            DropTable("BENCHMARK.ConsommationDeviceClient");
            DropTable("BENCHMARK.Devise");
            DropTable("BENCHMARK.Tarif");
            DropTable("BENCHMARK.DetailsClient");
            DropTable("BENCHMARK.Question");
            DropTable("BENCHMARK.Reponse");
            DropTable("BENCHMARK.ConfigSiClient");
            DropTable("BENCHMARK.ConfigSiConsommation");
            DropTable("BENCHMARK.TypeUsage");
            DropTable("BENCHMARK.ConfigSiDevice");
            DropTable("BENCHMARK.Resultat");
            DropTable("BENCHMARK.ConfigSi");
            DropTable("BENCHMARK.Politique");
            DropTable("BENCHMARK.DeviceClient");
            DropTable("BENCHMARK.Origine");
            DropTable("BENCHMARK.Simulation");
            DropTable("BENCHMARK.TypeContact");
            DropTable("BENCHMARK.Contact");
            DropTable("BENCHMARK.Client");
            DropTable("BENCHMARK.FamilleDevice");
            DropTable("BENCHMARK.CategorieDevice");
            DropTable("BENCHMARK.TypeDevice");
            DropTable("BENCHMARK.ConsoWattHeur");
            DropTable("BENCHMARK.ClasseDevice");
            DropTable("BENCHMARK.SecteurActivite");
            DropTable("BENCHMARK.AgeDevice");
            DropTable("BENCHMARK.Support");
            DropTable("BENCHMARK.Pays");
            DropTable("BENCHMARK.Adresse");
        }
    }
}
