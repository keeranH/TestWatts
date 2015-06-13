using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;
using Econocom.Model.Interfaces;
using Econocom.Model.Models;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Common;
using Econocom.Model.Models.Traduction;

namespace Econocom.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class EconocomContext : DbContext, IUnitOfWork
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #region CMS Table Mapping

            modelBuilder.Entity<Langue>().ToTable("Langue", "CMS");
            //modelBuilder.Entity<HistoriqueContenu>().ToTable("HistoriqueContenu", "CMS");
            modelBuilder.Entity<Page>().ToTable("Page", "CMS");
            //modelBuilder.Entity<HistoriqueModelePage>().ToTable("HistoriqueModelePage", "CMS");
            modelBuilder.Entity<ContenuModere>().ToTable("ContenuModere", "CMS");
            modelBuilder.Entity<Section>().ToTable("Section", "CMS");
            modelBuilder.Entity<Modele>().ToTable("Modele", "CMS");
            modelBuilder.Entity<CleModele>().ToTable("CleModele", "CMS");
            //modelBuilder.Entity<ContenuNonModere>().ToTable("ContenuNonModere", "CMS");
            modelBuilder.Entity<Routage>().ToTable("Routage", "CMS");
            modelBuilder.Entity<TypeObjet>().ToTable("TypeObjet", "CMS");
            #endregion

            #region CMS Association

            modelBuilder.Entity<Langue>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdLangue");



            modelBuilder.Entity<Page>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdPage");


            modelBuilder.Entity<ContenuModere>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdContenuModere");

            modelBuilder.Entity<Section>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdSection");
            modelBuilder.Entity<Section>().HasMany(o => o.Pages).WithRequired(o => o.Section);

            modelBuilder.Entity<Modele>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdModele");

            modelBuilder.Entity<CleModele>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdCleModele");

      

            modelBuilder.Entity<Routage>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdRoutage");

            modelBuilder.Entity<ConfigSi>()
                   .HasKey(o => o.Id)
                   .Property(o => o.Id)
                   .HasColumnName("IdConfigSi");

            modelBuilder.Entity<ConfigSiClient>()
                .HasKey(o => o.Id)
                .Property(o => o.Id)
                .HasColumnName("IdConfigSiClient");

            modelBuilder.Entity<ConfigSiDevice>()
                .HasKey(o => o.Id)
                .Property(o => o.Id)
                .HasColumnName("IdConfigSiDevice");
            modelBuilder.Entity<ConfigSiDevice>().Property(c => c.CoeffModeOff).HasPrecision(18, 4);
            modelBuilder.Entity<ConfigSiDevice>().Property(c => c.CoeffNonIntensif).HasPrecision(18, 4);
            modelBuilder.Entity<ConfigSiDevice>().Property(c => c.CoeffReductionWattExterne).HasPrecision(18, 4);
            modelBuilder.Entity<ConfigSiDevice>().Property(c => c.PourcentConsomExtern).HasPrecision(18, 4);


           modelBuilder.Entity<ConfigSiConsommation>()
             .HasKey(o => o.Id)
             .Property(o => o.Id)
             .HasColumnName("IdConfigSiConsommation");
           modelBuilder.Entity<ConfigSiConsommation>().Property(c => c.CoefficientConversion).HasPrecision(18, 4);
           modelBuilder.Entity<ConfigSiConsommation>().Property(c => c.ConsommationIntensif).HasPrecision(18, 4);
           modelBuilder.Entity<ConfigSiConsommation>().Property(c => c.ConsommationIntensifUnitaire).HasPrecision(18, 4);
           modelBuilder.Entity<ConfigSiConsommation>().Property(c => c.ConsommationNonIntensif).HasPrecision(18, 4);
           modelBuilder.Entity<ConfigSiConsommation>().Property(c => c.ConsommationCo2).HasPrecision(18, 4);
           modelBuilder.Entity<ConfigSiConsommation>().Property(c => c.ConsommationWatt).HasPrecision(18, 4);

           modelBuilder.Entity<Document>()
            .HasKey(o => o.Id)
            .Property(o => o.Id)
            .HasColumnName("IdDocument");

            #endregion

            #region Benchmark Table Mapping

            modelBuilder.Entity<Adresse>().ToTable("Adresse", "BENCHMARK");
            modelBuilder.Entity<AgeDevice>().ToTable("AgeDevice", "BENCHMARK");
            modelBuilder.Entity<CategorieDevice>().ToTable("CategorieDevice", "BENCHMARK");
            modelBuilder.Entity<ChangementAnneeCalendaire>().ToTable("ChangementAnneeCalendaire", "BENCHMARK");
            modelBuilder.Entity<ClasseDevice>().ToTable("ClasseDevice", "BENCHMARK");
            modelBuilder.Entity<Client>().ToTable("Client", "BENCHMARK");
            modelBuilder.Entity<ConsommationDeviceClient>().ToTable("ConsommationDeviceClient", "BENCHMARK");
            
            modelBuilder.Entity<ConsoWattHeur>().ToTable("ConsoWattHeur", "BENCHMARK");
            modelBuilder.Entity<Contact>().ToTable("Contact", "BENCHMARK");
            modelBuilder.Entity<ContenuMail>().ToTable("ContenuMail", "BENCHMARK");
            modelBuilder.Entity<DetailsClient>().ToTable("DetailsClient", "BENCHMARK");
            modelBuilder.Entity<DeviceClient>().ToTable("DeviceClient", "BENCHMARK");
            modelBuilder.Entity<Devise>().ToTable("Devise", "BENCHMARK");
            modelBuilder.Entity<Document>().ToTable("Document", "BENCHMARK");
            modelBuilder.Entity<Equivalence>().ToTable("Equivalence", "BENCHMARK");            
            modelBuilder.Entity<FamilleDevice>().ToTable("FamilleDevice", "BENCHMARK");           
            modelBuilder.Entity<Origine>().ToTable("Origine", "BENCHMARK");
            modelBuilder.Entity<Pays>().ToTable("Pays", "BENCHMARK");
            modelBuilder.Entity<Politique>().ToTable("Politique", "BENCHMARK");            
            modelBuilder.Entity<Question>().ToTable("Question", "BENCHMARK");
            modelBuilder.Entity<Ratio>().ToTable("Ratio", "BENCHMARK");
            modelBuilder.Entity<Rapport>().ToTable("Rapport", "BENCHMARK");           
            modelBuilder.Entity<ConfigSi>().ToTable("ConfigSi", "BENCHMARK");
            modelBuilder.Entity<ConfigSiClient>().ToTable("ConfigSiClient", "BENCHMARK");
            modelBuilder.Entity<ConfigSiConsommation>().ToTable("ConfigSiConsommation", "BENCHMARK");
            modelBuilder.Entity<ConfigSiDevice>().ToTable("ConfigSiDevice", "BENCHMARK");
            modelBuilder.Entity<Reponse>().ToTable("Reponse", "BENCHMARK");
            modelBuilder.Entity<SecteurActivite>().ToTable("SecteurActivite", "BENCHMARK");
            modelBuilder.Entity<Simulation>().ToTable("Simulation", "BENCHMARK");
            
            modelBuilder.Entity<Tarif>().ToTable("Tarif", "BENCHMARK");
            modelBuilder.Entity<TypeContact>().ToTable("TypeContact", "BENCHMARK");
            modelBuilder.Entity<TypeDevice>().ToTable("TypeDevice", "BENCHMARK");
            modelBuilder.Entity<TypeMail>().ToTable("TypeMail", "BENCHMARK");
            modelBuilder.Entity<TypeObjet>().ToTable("TypeObjet", "BENCHMARK");
            modelBuilder.Entity<TypeRatio>().ToTable("TypeRatio", "BENCHMARK");
            modelBuilder.Entity<TypeUsage>().ToTable("TypeUsage", "BENCHMARK");
            modelBuilder.Entity<Usage>().ToTable("Usage", "BENCHMARK");
            modelBuilder.Entity<UsageDevice>().ToTable("UsageDevice", "BENCHMARK");
            modelBuilder.Entity<VentilationClasseDevice>().ToTable("VentilationClasseDevice", "BENCHMARK");
            modelBuilder.Entity<VentilationClasseAgeDevice>().ToTable("VentilationClasseAgeDevice", "BENCHMARK");
            modelBuilder.Entity<CompteEvaluation>().ToTable("CompteEvaluation", "BENCHMARK");
            modelBuilder.Entity<Resultat>().ToTable("Resultat", "BENCHMARK");
            modelBuilder.Entity<UsageDevice>().ToTable("UsageDevice", "BENCHMARK");
            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateur", "BENCHMARK");

            modelBuilder.Entity<TypeClient>().ToTable("TypeClient", "BENCHMARK");
            modelBuilder.Entity<CorrespondanceSecteurActivite>().ToTable("CorrespondanceSecteurActivite", "BENCHMARK");
            modelBuilder.Entity<CorrespondanceTypeDevice>().ToTable("CorrespondanceTypeDevice", "BENCHMARK");
            modelBuilder.Entity<CorrespondanceProcesseur>().ToTable("CorrespondanceProcesseur", "BENCHMARK");
            modelBuilder.Entity<CorrespondanceVitesse>().ToTable("CorrespondanceVitesse", "BENCHMARK");
            modelBuilder.Entity<CorrespondanceTaille>().ToTable("CorrespondanceTaille", "BENCHMARK");
            modelBuilder.Entity<CorrespondancePuissance>().ToTable("CorrespondancePuissance", "BENCHMARK");
            #endregion

            #region Benchmark Association

            modelBuilder.Entity<Adresse>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdAdresse");

            modelBuilder.Entity<AgeDevice>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdAgeDevice")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<CategorieDevice>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdCategorieDevice")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<ChangementAnneeCalendaire>()
                        .HasKey(o => o.Id)                        
                        .Property(o => o.Id)
                        .HasColumnName("IdChangementAnneeCalendaire")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<ChangementAnneeCalendaire>().Property(c => c.CoefficientDestination).HasPrecision(18, 4);
            modelBuilder.Entity<ChangementAnneeCalendaire>().Property(c => c.CoefficientSource).HasPrecision(18, 4);

            modelBuilder.Entity<ClasseDevice>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdClasseDevice")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Client>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdClient");

            modelBuilder.Entity<ConsommationDeviceClient>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdConsommationDeviceClient");           

            modelBuilder.Entity<ConsoWattHeur>()
                        .HasKey(c => new {c.ClasseDeviceId, c.TypeDeviceId, c.AgeDeviceId}).Property(c=>c.Consommation).HasPrecision(18,4);                        

            modelBuilder.Entity<Contact>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdContact");

            modelBuilder.Entity<ContenuMail>()
                       .HasKey(o => o.Id)
                       .Property(o => o.Id)
                       .HasColumnName("IdContenuMail");

            modelBuilder.Entity<CompteEvaluation>()
                       .HasKey(o => o.Id)
                       .Property(o => o.Id)
                       .HasColumnName("IdCompteEvaluation");

            modelBuilder.Entity<DetailsClient>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdDetailClient");
            modelBuilder.Entity<DetailsClient>().Property(c => c.TauxEquipement).HasPrecision(18, 4);
            modelBuilder.Entity<DetailsClient>().Property(c => c.PrixKWH).HasPrecision(18, 4);
            modelBuilder.Entity<DetailsClient>().Property(c => c.CoefficientCO2).HasPrecision(18, 4);

            modelBuilder.Entity<DeviceClient>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdDeviceClient");

            modelBuilder.Entity<Devise>()
                       .HasKey(o => o.Id)
                       .Property(o => o.Id)
                       .HasColumnName("IdDevise");

            modelBuilder.Entity<Document>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdDocument");

            modelBuilder.Entity<Equivalence>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdEquivalence");
            

         

            modelBuilder.Entity<FamilleDevice>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdFamilleDevice")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);          

            modelBuilder.Entity<Origine>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdOrigine")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Pays>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdPays")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<Pays>().Property(c => c.CoefficientConversion).HasPrecision(18, 4);

            modelBuilder.Entity<Politique>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdPolitique")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

           
            modelBuilder.Entity<Question>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdQuestion");

            
            modelBuilder.Entity<Ratio>().HasKey(r => new {r.SecteurActiviteId, r.TypeDeviceId});
            modelBuilder.Entity<Ratio>().Property(c => c.Valeur).HasPrecision(18, 4);

            modelBuilder.Entity<Reponse>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdReponse");

            modelBuilder.Entity<SecteurActivite>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdSecteurActivite")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Simulation>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdSimulation");          

            modelBuilder.Entity<Tarif>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTarif");

            modelBuilder.Entity<TypeContact>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeContact")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<TypeDevice>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeDevice")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<TypeMail>()
                       .HasKey(o => o.Id)
                       .Property(o => o.Id)
                       .HasColumnName("IdTypeMail")
                       .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<TypeRatio>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeRatio")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<TypeObjet>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeObjet")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<TypeUsage>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeUsage")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Usage>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdUsage");
            modelBuilder.Entity<Usage>().Property(c => c.CoeffNonIntensif).HasPrecision(18, 4);
            modelBuilder.Entity<Usage>().Property(c => c.CoeffModeOff).HasPrecision(18, 4);
            modelBuilder.Entity<Usage>().Property(c => c.CoeffReductionWattExterne).HasPrecision(18, 4);
            modelBuilder.Entity<Usage>().Property(c => c.PourcentageConsoExterne).HasPrecision(18, 4);

            modelBuilder.Entity<VentilationClasseDevice>()
                        .HasKey(r => new {r.SecteurActiviteId, r.TypeDeviceId, r.ClasseDeviceId});
            modelBuilder.Entity<VentilationClasseDevice>().Property(c => c.Coefficient).HasPrecision(18, 4);

            modelBuilder.Entity<VentilationClasseAgeDevice>()
                        .HasKey(r => new { r.SecteurActiviteId, r.ClasseDeviceId, r.TypeDeviceId, r.AgeDeviceId });
            modelBuilder.Entity<VentilationClasseAgeDevice>().Property(c => c.Coefficient).HasPrecision(18, 4);

            modelBuilder.Entity<Resultat>()
                       .HasKey(o => o.Id)
                       .Property(o => o.Id)
                       .HasColumnName("IdResultat");
            modelBuilder.Entity<Resultat>().Property(c => c.CoefficientConversion).HasPrecision(18, 4);
            modelBuilder.Entity<Resultat>().Property(c => c.ConsommationCo2).HasPrecision(18, 4);
            modelBuilder.Entity<Resultat>().Property(c => c.ConsommationWatt).HasPrecision(18, 4);

            modelBuilder.Entity<UsageDevice>()
                       .HasKey(o => o.Id)
                       .Property(o => o.Id)
                       .HasColumnName("IdUsageDevice");

            modelBuilder.Entity<Utilisateur>()
                       .HasKey(o => o.Id)
                       .Property(o => o.Id)
                       .HasColumnName("IdUtilisateur");

            modelBuilder.Entity<Rapport>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdRapport");

            #endregion

            #region Traduction Mapping
            modelBuilder.Entity<AgeDeviceTraduction>().ToTable("AgeDeviceTraduction", "Traduction");
            modelBuilder.Entity<CategorieDeviceTraduction>().ToTable("CategorieDeviceTraduction", "Traduction");
            modelBuilder.Entity<ClasseDeviceTraduction>().ToTable("ClasseDeviceTraduction", "Traduction");
            modelBuilder.Entity<EquivalenceTraduction>().ToTable("EquivalenceTraduction", "Traduction");
            modelBuilder.Entity<FamilleDeviceTraduction>().ToTable("FamilleDeviceTraduction", "Traduction");
            modelBuilder.Entity<OrigineTraduction>().ToTable("OrigineTraduction", "Traduction");
            modelBuilder.Entity<PaysTraduction>().ToTable("PaysTraduction", "Traduction");
            modelBuilder.Entity<PolitiqueTraduction>().ToTable("PolitiqueTraduction", "Traduction");
            modelBuilder.Entity<QuestionTraduction>().ToTable("QuestionTraduction", "Traduction");
            modelBuilder.Entity<SecteurActiviteTraduction>().ToTable("SecteurActiviteTraduction", "Traduction");
            modelBuilder.Entity<TarifTraduction>().ToTable("TarifTraduction", "Traduction");
            modelBuilder.Entity<TypeDeviceTraduction>().ToTable("TypeDeviceTraduction", "Traduction");
            modelBuilder.Entity<TypeRatioTraduction>().ToTable("TypeRatioTraduction", "Traduction");
            modelBuilder.Entity<TypeUsageTraduction>().ToTable("TypeUsageTraduction", "Traduction");
            modelBuilder.Entity<DeviseTraduction>().ToTable("DeviseTraduction", "Traduction"); 
            #endregion

            modelBuilder.Entity<AgeDeviceTraduction>().HasKey(r => new { r.AgeDeviceId, r.LangueId });
            modelBuilder.Entity<AgeDeviceTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdAgeDeviceTraduction");
            modelBuilder.Entity<CategorieDeviceTraduction>().HasKey(r => new { r.CategorieDeviceId, r.LangueId });
            modelBuilder.Entity<CategorieDeviceTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdCategorieDeviceTraduction");
            modelBuilder.Entity<ClasseDeviceTraduction>().HasKey(r => new { r.ClasseDeviceId, r.LangueId });
            modelBuilder.Entity<ClasseDeviceTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdClasseDeviceTraduction");
            modelBuilder.Entity<EquivalenceTraduction>().HasKey(r => new { r.EquivalenceId, r.LangueId });
            modelBuilder.Entity<EquivalenceTraduction>()
                    .HasKey(o => o.Id)
                    .Property(o => o.Id)
                    .HasColumnName("IdEquivalenceTraduction");
            modelBuilder.Entity<FamilleDeviceTraduction>().HasKey(r => new { r.FamilleDeviceId, r.LangueId });
            modelBuilder.Entity<FamilleDeviceTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdFamilleDeviceTraduction");
            modelBuilder.Entity<OrigineTraduction>().HasKey(r => new { r.OrigineId, r.LangueId });
            modelBuilder.Entity<OrigineTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdOrigineTraduction");
            modelBuilder.Entity<PaysTraduction>().HasKey(r => new { r.PaysId, r.LangueId });
            modelBuilder.Entity<PaysTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdPaysTraduction");
            modelBuilder.Entity<PolitiqueTraduction>().HasKey(r => new { r.PolitiqueId, r.LangueId });
            modelBuilder.Entity<PolitiqueTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdPolitiqueTraduction");                     
            modelBuilder.Entity<QuestionTraduction>().HasKey(r => new { r.QuestionId, r.LangueId });
            modelBuilder.Entity<QuestionTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdQuestionTraduction");
            modelBuilder.Entity<SecteurActiviteTraduction>().HasKey(r => new { r.SecteurActiviteId, r.LangueId });
            modelBuilder.Entity<SecteurActiviteTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdSecteurActiviteTraduction");
            modelBuilder.Entity<TarifTraduction>().HasKey(r => new { r.TarifId, r.LangueId });
            modelBuilder.Entity<TarifTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTarifTraduction");
            modelBuilder.Entity<TypeDeviceTraduction>().HasKey(r => new { r.TypeDeviceId, r.LangueId });
            modelBuilder.Entity<TypeDeviceTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeDeviceTraduction");
            modelBuilder.Entity<TypeRatioTraduction>().HasKey(r => new { r.TypeRatioId, r.LangueId });
            modelBuilder.Entity<TypeRatioTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeRatioTraduction");
            modelBuilder.Entity<TypeUsageTraduction>().HasKey(r => new { r.TypeUsageId, r.LangueId });
            modelBuilder.Entity<TypeUsageTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdTypeUsageTraduction");
            modelBuilder.Entity<DeviseTraduction>().HasKey(r => new { r.DeviseId, r.LangueId });
            modelBuilder.Entity<DeviseTraduction>()
                        .HasKey(o => o.Id)
                        .Property(o => o.Id)
                        .HasColumnName("IdDeviseTraduction");

        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Adresse> Adresses { get; set; }
        public DbSet<AgeDevice> AgeDevices { get; set; }
        public DbSet<CategorieDevice> CategorieDevices { get; set; }
        public DbSet<ClasseDevice> ClasseDevices { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ConsommationDeviceClient> ConsommationDeviceClients { get; set; }
        
        public DbSet<ConsoWattHeur> ConsoWattHeurs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<DetailsClient> DetailsClients { get; set; }
        public DbSet<DeviceClient> DeviceClients { get; set; }
        public DbSet<Devise> Devises { get; set; }
        public DbSet<Equivalence> Equivalences { get; set; }
        public DbSet<FamilleDevice> FamilleDevices { get; set; }        
        public DbSet<Origine> Origines { get; set; }
        public DbSet<Pays> Pays { get; set; }
        public DbSet<Politique> Politiques { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Ratio> Ratios { get; set; }
        public DbSet<Reponse> Reponses { get; set; }
        public DbSet<Resultat> Resultats { get; set; }
        public DbSet<ConfigSi> ConfigSis { get; set; }
        public DbSet<ConfigSiClient> ConfigSiClients { get; set; }
        public DbSet<ConfigSiConsommation> ConfigSiConsommations { get; set; }
        public DbSet<ConfigSiDevice> ConfigSiDevices { get; set; }
        public DbSet<SecteurActivite> SecteurActivites { get; set; }
        public DbSet<Simulation> Simulations { get; set; }
        
        public DbSet<Tarif> Tarifs { get; set; }
        public DbSet<TypeContact> TypeContacts { get; set; }
        public DbSet<TypeDevice> TypeDevices { get; set; }
        public DbSet<TypeObjet> TypeObjets { get; set; }
        public DbSet<TypeUsage> TypeUsages { get; set; }
        public DbSet<Usage> Usages { get; set; }
        public DbSet<VentilationClasseAgeDevice> VentilationClasseAgeDevices { get; set; }
        public DbSet<VentilationClasseDevice> VentilationClasseDevices { get; set; }
        public DbSet<CompteEvaluation> Evaluations { get; set; }
        public DbSet<Document> Document { get; set; }

        public DbSet<Section> Sections { get; set; }
        public DbSet<Langue> Langues { get; set; }
        public DbSet<Modele> Modeles { get; set; }
        public DbSet<CleModele> CleModeles { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<ContenuModere> ContenuModeres { get; set; }        
        public DbSet<Routage> Routages { get; set; }
        

        public EconocomContext()
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 3600;
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.AutoDetectChangesEnabled = true;
        }

        public EconocomContext(string connectionString)
            : base(connectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 3600;
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.AutoDetectChangesEnabled = true;
        }

        public int SaveChanges()
        {
            return base.SaveChanges();
        }

    }
}
