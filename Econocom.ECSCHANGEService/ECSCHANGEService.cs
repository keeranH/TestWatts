using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Service;


namespace Econocom.ECSCHANGEService
{
    public partial class ECSCHANGEService : ServiceBase
    {
        private FileSystemWatcher _ecschangeWatcher;
        private FileSystemWatcher _importationWatcher;

        readonly BusinessService _service = new BusinessService();
        private readonly ServiceUtility _util = new ServiceUtility();

        public ECSCHANGEService()
        {
            InitializeComponent();
            _ecschangeWatcher.Path += ConfigurationManager.AppSettings["LienInitial"];
            _importationWatcher.Path += ConfigurationManager.AppSettings["LienImportWatt"];
        }

        protected override void OnStart(string[] args)
        {

        }

        protected override void OnStop()
        {

        }

        /// <summary>
        /// L'événement est déclenché quand un fichier est crée ou déposé dans le répertoire
        /// </summary>
        private void ECSCHANGEWatcher_Created(object sender,FileSystemEventArgs e)
        {
            try
            {
                if (_util.VerifierExtensionFichier(e))
                {
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
                        Client client = _service.TraiterDonneesClient(_util.ECSChangeModel.InfoClientImportViewModel);

                        //si pas d'erreurs , conversion du fichier d'articles au format d'importation dans WATTS Green
                        //Création du fichier d'importation
                        if (client.Id != int.MinValue)
                        {
                            _util.ECSChangeModel = _service.TraiterDonneesEquipements(_util.ECSChangeModel, client);
                            _util.CreationFichierImportation();
                        }

                    }
                    else
                    {
                        _util.InsertionMessageDansLog(false);
                        _util.CreationFichierKo();               
                    }
                                                           
                }              
            }
            catch (Exception ex)
            {
               _util.LogguerErreur(ex);
                if (File.Exists(_util.LienEnTraitement))
                {
                    //Déplacer le fichier initial dans le répertoire Fichier traite KO
                    _util.DeplacerDansRepFichierTraiteKo(_util.LienEnTraitement, _util.NomFichier);
                }           
            }

        }

        /// <summary>
        ///  L'événement est déclenché quand le fichier d'importation est déposé dans le répertoire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportationWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                var doc = new DocumentUpload()
                {
                    DocumentName = e.Name,
                    Data = File.ReadAllBytes(e.FullPath)
                };

                //Lecture du fichier en entrée,Sauvegarde des equipements en bdd,Mise a jr client
                _util.ECSChangeModel = _service.UploadDoc(doc, string.Empty);

                if (_util.ECSChangeModel.ImportParcValide)
                {
                    //Insertion dans log
                    _util.InsertionMessageDansLog(true);
                }
                else
                {
                    _util.ImportationEchoue();
                }
                
            }
            catch (Exception ex)
            {
                _util.LogguerErreur(ex);
            }
       
        }

    }
}
