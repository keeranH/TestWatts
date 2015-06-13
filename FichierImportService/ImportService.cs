using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Service;
using NLog;

namespace FichierImportService
{
    public partial class ImportService : ServiceBase
    {
        ITaskScheduler scheduler;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        readonly string LienInitial = ServiceUtility.ConfigInitial;
        readonly string LiemImportWatt = ServiceUtility.ConfigImportWatt;
        private static readonly ConnectionStringSettings connection = ConfigurationManager.ConnectionStrings["EconocomContext"];

        private readonly BusinessService _service;
        private readonly ServiceUtility _util ;
        private readonly ImportJob _job;

        public ImportService()
        {
            try
            {
                Logger.Info("Initalising components in importservice..start");
                InitializeComponent();
                _ecschangeWatcher.Path = LienInitial;
                _importationWatcher.Path = LiemImportWatt;
                Logger.Info("connection string " + connection.ConnectionString);
                _service = new BusinessService(connection.ConnectionString);
                _util = new ServiceUtility();
                _job = new ImportJob();
                Logger.Info("Initalising components in importservice...end");
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString);
            }
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Importservice..starts");
            scheduler=new TaskScheduler();
            scheduler.Run();
            Logger.Info("Importservice..ends");
        }

        protected override void OnStop()
        {
            Logger.Info("Importservice..stopped");
            if (scheduler != null)
            {
                scheduler.Stop();
            }
            Logger.Info("Importservice..stopped ends");
        }

        /// <summary>
        /// L'événement est déclenché quand un fichier est crée ou déposé dans le répertoire
        /// </summary>
        private void _ecschangeWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                if (_util.VerifierExtensionFichier(e))
                {
                    ServiceUtility.FlagInProgress = true;

                    //Déplacer le fichier dans le répertoire En_Traitement
                    _util.DeplacerDansRepEnTraitement();

                    var doc = new DocumentUpload()
                    {
                        DocumentName = _util.NomFichier,
                        Data = File.ReadAllBytes(_util.LienEnTraitement)
                    };

                    //Traitement du fichier commence
                    _util.ECSChangeModel = _service.UploadDocECSchange(doc);

                    //Si pas d'erreurs 
                    if (!_util.ECSChangeModel.InfoClientEnErreur && !_util.ECSChangeModel.InfoEquipementEnErreur)
                    {
                        //: Identifiant société trouvé ou ajouté dans WattsGreen(Création client ou récuperation identifiant)
                        _util.ECSChangeModel = _service.TraiterDonneesClient(_util.ECSChangeModel);

                        //si pas d'erreurs , conversion du fichier d'articles au format d'importation dans WATTS Green
                        //Création du fichier d'importation
                        if (_util.ECSChangeModel.Client.Id != int.MinValue)
                        {
                            _util.ECSChangeModel = _service.TraiterDonneesEquipements(_util.ECSChangeModel, _util.ECSChangeModel.Client);
                            //_util.CreationFichierOk();
                            _util.CreationFichierImportation();
                        }
                    }
                    else
                    { 
                         ServiceUtility.FlagInProgress = false;
                        _util.InsertionMessageDansLog(false);
                        _util.CreationFichierKo();
                        _job.MoveFiles();
                    }

                }
            }
            catch (Exception ex)
            {
                ServiceUtility.FlagInProgress = false;
                _util.LogguerErreur(ex);
                if (File.Exists(_util.LienEnTraitement))
                {
                    //Déplacer le fichier initial dans le répertoire Fichier traite KO
                    _util.DeplacerDansRepFichierTraiteKo(_util.LienEnTraitement, _util.NomFichier);
                }

                _job.MoveFiles();
            }           
        }

        /// <summary>
        ///  L'événement est déclenché quand le fichier d'importation est déposé dans le répertoire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _importationWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {          
            try
            {
                var doc = new DocumentUpload()
                {
                    DocumentName = e.Name,
                    Data = File.ReadAllBytes(e.FullPath)
                };

                //Lecture du fichier en entrée,Sauvegarde des equipements en bdd,Mise a jr client
                _util.ECSChangeModelMaj = _service.UploadDoc(doc, string.Empty);

                if (_util.ECSChangeModelMaj.ImportParcValide)
                {
                    //Insertion dans log
                    _util.InsertionMessageDansLog(true);

                    //Envoyer mail aux contacts / webmaster
                    _service.EnvoyerEmail(_util.ECSChangeModel);

                    _util.CreationFichierOk();
                   
                    ServiceUtility.FlagInProgress = false;                  
                }
                else
                {
                    ServiceUtility.FlagInProgress = false;
                    _util.ImportationEchoue();
                }

                _job.MoveFiles();
            }
            catch (Exception ex)
            {
                ServiceUtility.FlagInProgress = false;
                _util.LogguerErreur(ex);
                if (File.Exists(_util.LienEnTraitement))
                {
                    //Déplacer le fichier initial dans le répertoire Fichier traite KO
                    _util.DeplacerDansRepFichierTraiteKo(_util.LienEnTraitement, _util.NomFichier);
                }
                _job.MoveFiles();
            }         
        }
    }
}
