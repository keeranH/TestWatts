using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Econocom.Model.ViewModel;
using NLog;

namespace FichierImportService
{
    public class ServiceUtility
    {
        #region static variables

        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public static readonly string ConfigInitial = ConfigurationManager.AppSettings["LienInitial"];
        public static readonly string ConfigEnTraitement = ConfigurationManager.AppSettings["LienTraitement"];
        public static readonly string ConfigImportWatt = ConfigurationManager.AppSettings["LienImportWatt"];
        public static readonly string ConfigFichierTraiteOk = ConfigurationManager.AppSettings["LienFichierTraiteOk"];
        public static readonly string ConfigFichierTraiteKo = ConfigurationManager.AppSettings["LienFichierTraiteKo"];
        public static readonly string ConfigFichiers = ConfigurationManager.AppSettings["LienFichiers"];
        public static readonly string ConfigBdd = ConfigurationManager.ConnectionStrings["EconocomContext"].ConnectionString;

        #endregion

        #region Private members

        /// <summary>
        /// Nom du fichier traité
        /// </summary>
        private string _nomFichier = "";

        /// <summary>
        /// Nom initial du fichier traité
        /// </summary>
        private string _nomInitialFichier = "";

        /// <summary>
        /// Chemin initial ou on a deposé le fichier
        /// </summary>
        private string _lienInitial = "";

        /// <summary>
        /// Chemin traitement fichier
        /// </summary>
        private string _lienEnTraitement = "";

        /// <summary>
        /// Chemin import Watt 
        /// </summary>
        private string _lienImportWatt = "";

        /// <summary>
        /// Chemin Fichier traite OK
        /// </summary>
        private string _lienFichierTraiteOk = "";

        /// <summary>
        /// Chemin Fichier traite KO
        /// </summary>
        private string _lienFichierTraiteKo = "";

        /// <summary>
        /// Timestamp
        /// </summary>
        private string _timeStamp = "";

        /// <summary>
        /// Lien ou les nouveaux fichiers sont créés
        /// </summary>
        private string _lienNouveauFichier = "";

        /// <summary>
        /// nouveau fichier d'importation
        /// </summary>
        private string _nomNouveauFichierimportation = "";

        /// <summary>
        /// ECSchange model
        /// </summary>
        private ImportECSchangeViewModel _ecsChangeModel;

        /// <summary>
        /// ECSchange model Equipements traitées
        /// </summary>
        private ImportECSchangeViewModel _ecsChangeModelMaj;

        private string _message = "";

        #endregion

        #region Getters setters

        public string NomInitialFichier
        {
            get { return _nomInitialFichier; }
            set { _nomInitialFichier = value; }
        }

        public string NomFichier
        {
            get { return _nomFichier; }
            set { _nomFichier = value; }
        }

        public string LienInitial
        {
            get { return _lienInitial; }
            set { _lienInitial = value; }
        }

        public string LienEnTraitement
        {
            get { return _lienEnTraitement; }
            set { _lienEnTraitement = value; }
        }

        public string LienImportWatt
        {
            get { return _lienImportWatt; }
            set { _lienImportWatt = value; }
        }

        public string LienFichierTraiteOk
        {
            get { return _lienFichierTraiteOk; }
            set { _lienFichierTraiteOk = value; }
        }

        public string LienFichierTraiteKo
        {
            get { return _lienFichierTraiteKo; }
            set { _lienFichierTraiteKo = value; }
        }

        public string TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public string LienNouveauFichier
        {
            get { return _lienNouveauFichier; }
            set { _lienNouveauFichier = value; }
        }

        public string NomNouveauFichierImportation
        {
            get { return _nomNouveauFichierimportation; }
            set { _nomNouveauFichierimportation = value; }
        }

        public ImportECSchangeViewModel ECSChangeModel
        {
            get { return _ecsChangeModel; }
            set { _ecsChangeModel = value; }
        }

        public ImportECSchangeViewModel ECSChangeModelMaj
        {
            get { return _ecsChangeModelMaj; }
            set { _ecsChangeModelMaj = value; }
        }

        public static bool FlagInProgress { get; set; }

        #endregion

        #region Déplacer les fichiers

        /// <summary>
        /// Déplacer le fichier dans le répertoire EN_TRAITEMENT
        /// </summary>
        public void DeplacerDansRepEnTraitement()
        {
            LienEnTraitement = ConfigEnTraitement + NomFichier;

            if (File.Exists(LienInitial))
            {
                File.Move(LienInitial, LienEnTraitement);
                _message = "Le fichier {0} a été déplacé dans le répertoire EN_TRAITEMENT";
                Logger.Info(_message, NomFichier);
            }
        }


        /// <summary>
        /// Déplacer le fichier dans le répertoire FICHIER TRAITE OK
        /// </summary>
        public void DeplacerDansRepFichierTraiteOk(string lien, string fileName)
        {
            LienFichierTraiteOk = ConfigFichierTraiteOk + fileName;

            if (File.Exists(lien))
            {
                File.Move(lien, LienFichierTraiteOk);
                _message = "Le fichier {0} a été déplacé dans le répertoire FICHIER_TRAITE_OK";
                Logger.Info(_message, fileName);
            }
        }

        /// <summary>
        /// Déplacer le fichier dans le répertoire FICHIER TRAITE KO
        /// </summary>
        public void DeplacerDansRepFichierTraiteKo(string lien, string fileName)
        {
            LienFichierTraiteKo = ConfigFichierTraiteKo + fileName;

            if (File.Exists(lien))
            {
                File.Move(lien, LienFichierTraiteKo);
                _message = "Le fichier {0} a été déplacé dans le répertoire FICHIER_TRAITE_KO";
                Logger.Info(_message, fileName);
            }
        }

        #endregion

        /// <summary>
        /// Logger les erreurs dans le fichier log
        /// </summary>
        /// <param name="e"></param>
        public void LogguerErreur(Exception e)
        {
            try
            {
                if (e == null)
                {
                    Logger.Error("Exception null");
                }
                else
                {
                    if (e.InnerException != null)
                    {
                        Logger.Error(e.InnerException.InnerException != null
                            ? e.InnerException.InnerException.Message
                            : e.InnerException.Message);
                    }
                    else
                    {
                        Logger.Error(e.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Ajouter Timestamp avec le nom du fichier
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string AppendTimeStamp(string fileName)
        {
            try
            {
                TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                return string.Concat(
                    Path.GetFileNameWithoutExtension(fileName),
                    TimeStamp,
                    Path.GetExtension(fileName)
                    );

            }
            catch (Exception ex)
            {
                LogguerErreur(ex);
                throw;
            }
        }

        /// <summary>
        /// Verifier l'extension du fichier
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool VerifierExtensionFichier(FileSystemEventArgs e)
        {
            try
            {
                LienInitial = e.FullPath;
                NomInitialFichier = e.Name;

                string extensionFichier = Path.GetExtension(LienInitial);

                if (extensionFichier != null && extensionFichier.Equals(".csv"))
                {
                    NomFichier = AppendTimeStamp(e.Name);
                    return true;
                }

                Logger.Info("L'extension de fichier " + e.Name + " n'est pas appropriée");

                DeplacerDansRepFichierTraiteKo(e.FullPath, e.Name);

                return false;

            }
            catch (Exception ex)
            {
                LogguerErreur(ex);
                throw;
            }

        }

        /// <summary>
        /// Création fichiers client et equipements
        /// </summary>
        public void CreationFichierImportation()
        {
            try
            {
                NomNouveauFichierImportation = "ImportationWatts" + TimeStamp + ".csv";

                String lienTemporaire = ConfigEnTraitement + NomNouveauFichierImportation;

                //Création fichier d'importation
                using (
                    var fs =
                        new FileStream(lienTemporaire, FileMode.Create))
                {
                    using (var streamWriter = new StreamWriter(fs, Encoding.UTF8))
                    {
                        var csvWriter = new CsvWriter(streamWriter);
                        csvWriter.Configuration.HasHeaderRecord = false;
                        csvWriter.Configuration.Delimiter = ";";
                        csvWriter.Configuration.SkipEmptyRecords = false;
                        csvWriter.Configuration.Encoding = Encoding.UTF8;

                        csvWriter.WriteRecords(ECSChangeModel.ListeImportClientViewModel);

                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                LienImportWatt = ConfigImportWatt + NomNouveauFichierImportation;
                if (File.Exists(lienTemporaire))
                {
                    File.Move(lienTemporaire, LienImportWatt);
                }
            }
            catch (Exception ex)
            {
                LogguerErreur(ex);
                throw;
            }
        }

        /// <summary>
        /// Insertion des messages dans le fichier log
        /// </summary>
        /// <param name="flagSuccès"></param>
        public void InsertionMessageDansLog(bool flagSuccès)
        {
            try
            {
                if (flagSuccès)
                {
                    //Déplacer le fichier initial/fichier d'importation dans le répertoire Fichier traite OK
                    DeplacerDansRepFichierTraiteOk(LienEnTraitement, NomFichier);
                    DeplacerDansRepFichierTraiteOk(LienImportWatt, NomNouveauFichierImportation);

                    var builder = new StringBuilder();
                    builder.Append(" Statut: ").Append("Traitement réussi pour le fichier ").Append(NomFichier);
                    builder.Append(" Identifiant:").Append(ECSChangeModel.InfoClientImportViewModel.IdClient);
                    builder.Append(" Nom Societe :").Append(ECSChangeModel.InfoClientImportViewModel.RaisonSociale);
                    builder.Append(" Nombre de lignes d’équipement total: ").Append(ECSChangeModel.ListeImportClientViewModel.Count);
                    builder.Append(" Poids Total: ").Append(ECSChangeModel.PoidsTotal);

                    Logger.Info(builder.ToString());
                }
                else
                {
                    var builder = new StringBuilder();
                    builder.Append(" Statut: ").Append("Traitement echoué pour le fichier ").Append(NomFichier);
                    builder.Append(" Identifiant:").Append(ECSChangeModel.InfoClientImportViewModel.IdClient);
                    builder.Append(" Nom Societe :").Append(ECSChangeModel.InfoClientImportViewModel.RaisonSociale);
                    builder.Append(" Nombre de lignes d’équipement total: ").Append(ECSChangeModel.ListeEquipementImportViewModel.Count);

                    int count = ECSChangeModel.ListeEquipementImportViewModel.Count(x => !string.IsNullOrEmpty(x.Erreur));
                    if (count > 0)
                    {
                        builder.Append(" Nombre de lignes d’équipement en erreur: ").Append(count);
                    }
                  

                    Logger.Info(builder.ToString());
                }

            }
            catch (Exception ex)
            {
                LogguerErreur(ex);
                throw;
            }
        }

        /// <summary>
        /// Génération d'un fichier identique comme le fichier en entrée mais avec une colonne raison de l’erreur ajoutée
        /// </summary>
        public void CreationFichierKo()
        {
            try
            {

                if (File.Exists(LienEnTraitement))
                {
                    File.Delete(LienEnTraitement);
                }
                
                using (
              var fs =
                  new FileStream(ConfigFichierTraiteKo + NomFichier,
                      FileMode.Create))
                {
                    using (var streamWriter = new StreamWriter(fs, Encoding.UTF8))
                    {
                        var csvWriter = new CsvWriter(streamWriter);
                        csvWriter.Configuration.HasHeaderRecord = false;
                        csvWriter.Configuration.Delimiter = ";";
                        csvWriter.Configuration.Encoding = Encoding.UTF8;

                        csvWriter.WriteRecord(ECSChangeModel.InfoClientImportViewModel);

                        InsererDonnees(ECSChangeModel, csvWriter, "KO");

                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                _message = "Le fichier {0} a été déplacé dans le répertoire FICHIER TRAITE KO";
                Logger.Info(_message, NomFichier);
            }
            catch (Exception ex)
            {
                LogguerErreur(ex);
                throw;
            }
        }

        public void InsererDonnees(ImportECSchangeViewModel importViewModel, CsvWriter csvWriter, string statut)
        {
            try
            {
                foreach (var record in ECSChangeModel.ListeEquipementImportViewModel)
                {
                    csvWriter.WriteField(record.CodeCategorie);
                    csvWriter.WriteField(record.LibelleCategorie);
                    csvWriter.WriteField(record.CodeFamille);
                    csvWriter.WriteField(record.LibelleFamille);
                    csvWriter.WriteField(record.CodeSousFamille);
                    csvWriter.WriteField(record.LibelleSousFamille);
                    csvWriter.WriteField(record.RefECS);
                    csvWriter.WriteField(record.LibelleRefECS);
                    csvWriter.WriteField(record.LibelleMarque);
                    csvWriter.WriteField(record.LibelleGamme);
                    csvWriter.WriteField(record.LibelleModele);
                    csvWriter.WriteField(record.NbProcesseurs);
                    csvWriter.WriteField(record.CodeProcesseur);
                    csvWriter.WriteField(record.LibelleCodeProcesseur);
                    csvWriter.WriteField(record.LibelleTypeProcesseur);
                    csvWriter.WriteField(record.Frequence);
                    csvWriter.WriteField(record.UniteFrequence);
                    csvWriter.WriteField(record.LibelleUniteFrequence);
                    csvWriter.WriteField(record.Memoire);
                    csvWriter.WriteField(record.UniteMemoire);
                    csvWriter.WriteField(record.LibelleUniteMemoire);
                    csvWriter.WriteField(record.Taille);
                    csvWriter.WriteField(record.UniteTaille);
                    csvWriter.WriteField(record.Vitesse);
                    csvWriter.WriteField(record.UniteVitesse);
                    csvWriter.WriteField(record.LibelleVitesse);
                    csvWriter.WriteField(record.Couleur);
                    csvWriter.WriteField(record.RectoVerso);
                    csvWriter.WriteField(record.NormeGraphique);
                    csvWriter.WriteField(record.CodeResolution);
                    csvWriter.WriteField(record.LibelleResolution);
                    csvWriter.WriteField(record.Poids);
                    csvWriter.WriteField(record.UnitePoids);
                    csvWriter.WriteField(record.LibelleUnitePoids);
                    csvWriter.WriteField(record.CodeTechnologie);
                    csvWriter.WriteField(record.NombreVoies);
                    csvWriter.WriteField(record.Puissance);
                    csvWriter.WriteField(record.UnitePuissance);
                    csvWriter.WriteField(record.DateFacture.ToString("dd/MM/yyyy"));
                    csvWriter.WriteField(record.Quantite);
                    if(statut.Equals("KO"))
                        csvWriter.WriteField(record.Erreur);
                    csvWriter.WriteField(record.Caracteristique);

                    csvWriter.NextRecord();
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Génération d'un fichier identique comme le fichier en entrée mais avec une colonne caractéristique ajoutée
        /// </summary>
        public void CreationFichierOk()
        {
            try
            {

                if (File.Exists(LienEnTraitement))
                {
                    File.Delete(LienEnTraitement);
                }

                using (
              var fs =
                  new FileStream(ConfigFichierTraiteOk + NomFichier,
                      FileMode.Create))
                {
                    using (var streamWriter = new StreamWriter(fs, Encoding.UTF8))
                    {
                        var csvWriter = new CsvWriter(streamWriter);
                        csvWriter.Configuration.HasHeaderRecord = false;
                        csvWriter.Configuration.Delimiter = ";";
                        csvWriter.Configuration.Encoding = Encoding.UTF8;

                        csvWriter.WriteRecord(ECSChangeModel.InfoClientImportViewModel);

                        InsererDonnees(ECSChangeModel, csvWriter, "OK");

                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogguerErreur(ex);
                throw;
            }
        }

        /// <summary>
        /// Importation des données a echoué
        /// </summary>
        public void ImportationEchoue()
        {
            DeplacerDansRepFichierTraiteKo(LienEnTraitement, NomFichier);
            DeplacerDansRepFichierTraiteKo(LienImportWatt, NomNouveauFichierImportation);

            Logger.Info("Importation des données dans WATTS Green a echoué");
        }


    }

}
