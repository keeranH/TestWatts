using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using CsvHelper;
using Econocom.Business.Events.Email;
using Econocom.Calculateur;
using Econocom.Helper;
using Econocom.Helper.ChangementAnnee;
using Econocom.Helper.File;
using Econocom.Model.Enum;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Service;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using Econocom.Model.ViewModel.Report;
using NLog;
using Omu.ValueInjecter;
using Security;
using Econocom.Data;
using Page = Econocom.Model.Models.CMS.Page;
using Econocom.Model.Models.Traduction;
using Econocom.Model.ViewModel.CsvMap;

namespace Econocom.Business.Service
{
    public class BusinessService
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string link = "~/Images/Demo/Family/";
        private readonly EconocomDataManager _econocomDataManager;
        private IUnitOfWork _unitOfWork;
        /// <summary>
        /// 
        /// </summary>
        public BusinessService()
        {
            _unitOfWork = new EconocomContext();
            _econocomDataManager = new EconocomDataManager(_unitOfWork);
        }

        public BusinessService(string connectionString)
        {
            _unitOfWork = new EconocomContext(connectionString);
            _econocomDataManager = new EconocomDataManager(_unitOfWork);
        }

        public BusinessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _econocomDataManager = new EconocomDataManager(unitOfWork);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="econocomDataManager"></param>
        public BusinessService(EconocomDataManager econocomDataManager)
        {
            _econocomDataManager = econocomDataManager;
        }


        /// <summary>
        /// Récupérer contenu modéré par page et langue
        /// </summary>
        /// <param name="pageId">Identifiant de la page</param>
        /// <param name="languageId">Identifiant de la langue</param>
        /// <returns>Objet ContenuModere</returns>
        public ContenuModere GetPublishedContent(int pageId, int languageId)
        {
            try
            {
                var resultat = _econocomDataManager.GetPublishedContent(pageId, languageId);
                if (resultat != null)
                    Logger.Debug("GetPublishedContent entrée: pageId= {0}, languageId= {1}, sortie : PublishedContentId= {2}",
                                pageId, languageId, resultat.Id);
                else
                    Logger.Debug(
                        "GetPublishedContent entrée: pageId= {0}, languageId= {1}, sortie : PublishedContentId=null",
                        pageId, languageId);

                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("GetPublishedContent entrée: pageId= {0}, languageId= {1}, sortie=>erreur : {2}", pageId, languageId, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarder un client
        /// </summary>
        /// <param name="client">Objet client</param>
        /// <returns>Objet Client </returns>
        public Client SetClient(Client client)
        {
            try
            {
                return _econocomDataManager.SetClient(client);
            }
            catch (Exception e)
            {
                Logger.Error("SetClient entrée: client, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer un client par son identifiant
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <returns>Objet Client</returns>
        public Client GetClient(int clientId)
        {
            try
            {
                return _econocomDataManager.GetClient(clientId);
            }
            catch (Exception e)
            {
                Logger.Error("GetClient entrée: clientId= {0}, sortie=>erreur : {1}", clientId, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer un pays par identifiant
        /// </summary>
        /// <param name="paysId">Identifiant du pays</param>
        /// <returns>Objet PAys</returns>
        public Pays GetPaysById(int paysId)
        {
            try
            {
                return _econocomDataManager.GetPaysById(paysId);
            }
            catch (Exception e)
            {
                Logger.Error("GetPaysById entrée: paysId= {0}, sortie=>erreur : {1}", paysId, e.StackTrace);
                throw;
            }

        }

        /// <summary>
        /// Mise à jour d'un client
        /// </summary>
        /// <param name="client">Objet client</param>
        /// <returns>Objet Client</returns>
        public Client UpdateClient(Client client)
        {
            try
            {
                return _econocomDataManager.UpdateClient(client);
            }
            catch (Exception e)
            {
                Logger.Error("UpdateClient entrée: client, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Validation client
        /// </summary>
        /// <param name="client">Objet client</param>
        /// <returns>Objet Client</returns>
        public Client ValidateClient(Client client)
        {
            try
            {
                if (client != null)
                {
                    if (string.IsNullOrEmpty(client.Nom))
                    {
                        throw new FaultException("Nome", new FaultCode("Nom est vide"));
                    }
                    if (string.IsNullOrEmpty(client.Nom))
                    {
                        throw new FaultException("Nome", new FaultCode("Nom est vide"));
                    }
                    if (string.IsNullOrEmpty(client.CodeNAF))
                    {
                        throw new FaultException("CodeNAF", new FaultCode("CodeNAF est vide"));
                    }
                    if (string.IsNullOrEmpty(client.RaisonSociale))
                    {
                        throw new FaultException("RaisonSociale", new FaultCode("RaisonSociale est vide"));
                    }
                    if (string.IsNullOrEmpty(client.TVAIntraComm))
                    {
                        throw new FaultException("TVAIntraComm", new FaultCode("TVAIntraComm est vide"));
                    }
                    if (client.Adresse != null)
                    {
                        if (string.IsNullOrEmpty(client.Adresse.Adresse1))
                        {
                            throw new FaultException("Adresse.Adresse1", new FaultCode("Adresse1 est vide"));
                        }
                        if (string.IsNullOrEmpty(client.Adresse.Adresse2))
                        {
                            throw new FaultException("Adresse.Adresse2", new FaultCode("Adresse2 est vide"));
                        }
                        if (string.IsNullOrEmpty(client.Adresse.Adresse3))
                        {
                            throw new FaultException("Adresse.Adresse3", new FaultCode("Adresse3 est vide"));
                        }
                        if (string.IsNullOrEmpty(client.Adresse.CodePostal))
                        {
                            throw new FaultException("Adresse.CODEPOSTAL", new FaultCode("Codepostal est vide"));
                        }
                        if (string.IsNullOrEmpty(client.Adresse.Region))
                        {
                            throw new FaultException("Adresse.Region", new FaultCode("Region est vide"));
                        }
                        if (string.IsNullOrEmpty(client.Adresse.Ville))
                        {
                            throw new FaultException("Adresse.Ville", new FaultCode("Ville est vide"));
                        }
                    }
                    else
                    {
                        throw new FaultException("Adresse", new FaultCode("Adresse est vide"));
                    }

                    if (client.Contacts != null)
                    {
                        foreach (Contact contact in client.Contacts)
                        {
                            if (!string.IsNullOrEmpty(contact.Email))
                            {
                                var contactExist = _econocomDataManager.GetContactByEmail(contact.Email);
                                if (contactExist != null)
                                {
                                    throw new FaultException("", new FaultCode("duplicate email"));
                                }
                            }
                        }
                    }
                }
                return client;
            }
            catch (Exception e)
            {
                Logger.Error("ValidateClient entrée: client, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer la liste des clients entre deux dates
        /// </summary>
        /// <param name="from">Date de début</param>
        /// <param name="to">Date de fin</param>
        /// <returns>Liste de clients</returns>
        public List<Client> GetClients(DateTime from, DateTime to)
        {
            try
            {
                return _econocomDataManager.GetClients(from, to);
            }
            catch (Exception e)
            {
                Logger.Error("GetClients entrée: from= {0}, to= {1}, sortie=>erreur : {2}", from, to, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer adresse par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'adresse</param>
        /// <returns>Objet Adresse</returns>
        public Adresse GetAdresseById(int id)
        {
            try
            {
                return _econocomDataManager.GetAdresseById(id);
            }
            catch (Exception e)
            {
                Logger.Error("GetAdresseById entrée: id= {0}, sortie=>erreur : {1}", id, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré un contact par son nom de connexion et mot de passe
        /// </summary>
        /// <param name="loginName">Nom de connexion</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Objet Contact</returns>
        public Contact GetContact(string loginName, string password)
        {
            try
            {
                var hashPassword = HashHelper.HashPassWord(password);
                return _econocomDataManager.GetContact(loginName, hashPassword);
            }
            catch (Exception e)
            {
                Logger.Error("GetContact entrée: loginName= {0}, password= {1}, sortie=>erreur : {2}", loginName, password, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré un contact par son adresse mail
        /// </summary>
        /// <param name="email">Adresse mail</param>
        /// <returns>Objet Contact</returns>
        public Contact GetContactByEmail(string email)
        {
            try
            {
                return _econocomDataManager.GetContactByEmail(email);
            }
            catch (Exception e)
            {
                Logger.Error("GetContactByEmail entrée: email= {0}, sortie=>erreur : {1}", email, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré un contact par type et identifiant client
        /// </summary>
        /// <param name="typeId">Identifiant du type contact</param>
        /// <param name="clientId">Identifiant du client</param>
        /// <returns>Objet Contact</returns>
        public Contact GetContactByTypeAndClient(int typeId, int clientId)
        {
            try
            {
                return _econocomDataManager.GetContactByTypeAndClient(typeId, clientId);
            }
            catch (Exception e)
            {
                Logger.Error("GetContactByTypeAndClient entrée: typeId= {0}, clientId= {1}, sortie=>erreur : {2}", typeId, clientId, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré liste utilisateurs pour client
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <param name="typeContactId">Identifiant du type contact</param>
        /// <returns>Liste de contacts</returns>
        public List<Contact> GetUtilisateursByClientIdAndTypeContactId(int clientId, int typeContactId)
        {
            try
            {
                return _econocomDataManager.GetUtilisateursByClientIdAndTypeContactId(clientId, typeContactId);
            }
            catch (Exception e)
            {
                Logger.Error("GetUtilisateursByClientIdAndTypeContactId entrée: clientId= {0}, typeContactId= {1}, sortie=>erreur : {2}", clientId, typeContactId, e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Mise à jour du mot de passe d'un contact
        /// </summary>
        /// <param name="contact">Le contact</param>
        /// <returns>Objet Contact</returns>
        public Contact UpdateContactPassword(Contact contact)
        {
            try
            {
                //return _econocomDataManager.UpdateContactPassword(contact);
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("UpdateContactPassword entrée: contact, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré un contact par son identifiant
        /// </summary>
        /// <param name="contactId">Indentifiant du contact</param>
        /// <returns>Objet Contact</returns>
        public Contact GetContactById(int contactId)
        {
            try
            {
                return _econocomDataManager.GetContactById(contactId);
            }
            catch (Exception e)
            {
                Logger.Error("GetContactById entrée: contactId= {0}, sortie=>erreur : {1}", contactId, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarder un contact
        /// </summary>
        /// <param name="contact">Le contact</param>
        /// <returns>Objet Contact</returns>
        public Contact SetContact(Contact contact)
        {
            try
            {
                return _econocomDataManager.SetContact(contact);
            }
            catch (Exception e)
            {
                Logger.Error("SetContact entrée: contact, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Mise à jour d'un contact 
        /// </summary>
        /// <param name="contact">Le contact</param>
        /// <returns>Objet Contact</returns>
        public Contact UpdateContact(Contact contact)
        {
            try
            {
                return _econocomDataManager.UpdateContact(contact);
            }
            catch (Exception e)
            {
                Logger.Error("UpdateContact entrée: contact, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Récupéré un contact le code de vérification
        /// </summary>
        /// <param name="verificationCode">Le code de vérification</param>
        /// <returns>Objet Contact</returns>
        public Contact GetContactByVerificationCode(string verificationCode)
        {
            try
            {
                return _econocomDataManager.GetContactByVerificationCode(verificationCode);
            }
            catch (Exception e)
            {
                Logger.Error("GetContactByVerificationCode entrée: verificationCode= {0}, sortie=>erreur : {1}", verificationCode, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré liste de questions
        /// </summary>
        /// <returns>Liste de questions</returns>
        public List<Question> GetAllQuestions()
        {
            try
            {
                return _econocomDataManager.GetAllQuestions();
            }
            catch (Exception e)
            {
                Logger.Error("GetAllQuestions : sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Mise à jour du code de vérification d'un contact
        /// </summary>
        /// <param name="contact">Objet contact</param>
        /// <returns>Objet Contact</returns>
        public Contact UpdateVerificationCode(Contact contact)
        {
            try
            {
                return _econocomDataManager.UpdateVerificationCode(contact);
            }
            catch (Exception e)
            {
                Logger.Error("UpdateVerificationCode entrée: contact, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Envoyer le mail de réinitialisation du mot de passe
        /// </summary>
        /// <param name="contact">Objet contact</param>
        /// <returns>vrai / faux</returns>
        public bool SendResetPasswordMail(Contact contact)
        {
            try
            {
                EmailHelper.SendResetPasswordMail(contact);
            }
            catch (Exception e)
            {
                Logger.Error("SendResetPasswordMail entrée: contact, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return true;
        }

        /// <summary>
        /// Récupéré une question par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de la question</param>
        /// <returns>Objet Question</returns>
        public Question GetQuestion(int id)
        {
            try
            {
                return _econocomDataManager.GetQuestion(id);
            }
            catch (Exception e)
            {
                Logger.Error("GetQuestion entrée: id= {0}, sortie=>erreur : {1}", id, e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Récupéré les détails d'un client
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <returns>Objet DetailsClient</returns>
        public DetailsClient GetDetailsClient(int clientId)
        {
            try
            {
                return _econocomDataManager.GetDetailsClient(clientId);
            }
            catch (Exception e)
            {
                Logger.Error("GetDetailsClient entrée: clientId= {0}, sortie=>erreur : {1}", clientId, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré liste de pays
        /// </summary>
        /// <returns>Liste de pays</returns>
        public List<Pays> GetCountries()
        {
            try
            {
                return _econocomDataManager.GetCountries();
            }
            catch (Exception e)
            {
                Logger.Error("GetAllQuestions : sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré un client par son identifiant
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <returns>Objet Client</returns>
        public Client GetClientById(int clientId)
        {
            try
            {
                return _econocomDataManager.GetClientById(clientId);
            }
            catch (Exception e)
            {
                Logger.Error("GetClientById entrée: clientId= {0}, sortie=>erreur : {1}", clientId, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré les détails d'un client pour rapport
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <param name="reportType">Type rapport</param>
        /// <returns>Objet</returns>
        public object GetReportClientData(int clientId, string reportType)
        {
            try
            {
                //return _econocomDataManager.GetReportClientData(clientId, reportType);
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("GetReportClientData entrée: clientId= {0}, reportType= {1}, sortie=>erreur : {2}", clientId, reportType, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré un client par son nom de connexion
        /// </summary>
        /// <param name="loginName">Nom de connexion</param>
        /// <returns>Objet Client</returns>
        public Client GetClientByLoginName(string loginName)
        {
            try
            {
                return _econocomDataManager.GetClientByLoginName(loginName);
            }
            catch (Exception e)
            {
                Logger.Error("GetClientByLoginName entrée: loginName= {0}, sortie=>erreur : {1}", loginName, e.StackTrace);
                throw e;
            }
        }



        /// <summary>
        /// Récupéré la langue par défaut
        /// </summary>
        /// <returns>Objet Langue</returns>
        public Langue GetDefaultLanguage()
        {
            try
            {
                return _econocomDataManager.GetDefaultLanguage();
            }
            catch (Exception e)
            {
                Logger.Error("GetDefaultLanguage : sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré la liste de langues
        /// </summary>
        /// <returns>Liste de langues</returns>
        public List<Langue> GetLanguages()
        {
            try
            {
                return _econocomDataManager.GetLanguages();
            }
            catch (Exception e)
            {
                Logger.Error("GetLanguages : sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré langue par code
        /// </summary>
        /// <param name="code">code de la langue</param>
        /// <returns>Objet Langue</returns>
        public Langue GetLangueByCode(String code)
        {
            try
            {
                return _econocomDataManager.GetLangueByCode(code);
            }
            catch (Exception e)
            {
                Logger.Error("GetLangueByCode entrée: code= {0}, sortie=>erreur : {1}", code, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré liste contenu pour une page par le nom de la page et culture de langue
        /// </summary>
        /// <param name="name">Nom de page</param>
        /// <param name="languageCulture">Culture de langue</param>
        /// <returns>Liste de Contenu modéré</returns>
        public List<ContenuModere> GetPublishedContentsForPage(string name, string languageCulture)
        {
            try
            {
                return _econocomDataManager.GetPublishedContentsForPage(name, languageCulture);
            }
            catch (Exception e)
            {
                Logger.Error("GetPublishedContentsForPage entrée: name= {0}, languageCulture= {1}, sortie=>erreur : {2}", name, languageCulture, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarder objet dynamique
        /// </summary>
        /// <param name="dynamicObject">objet dynamique</param>
        /// <returns>Objet dynamique</returns>
        public dynamic SetObject(dynamic dynamicObject)
        {
            try
            {
                return _econocomDataManager.SetObject(dynamicObject);
            }
            catch (Exception e)
            {
                Logger.Error("SetObject entrée: dynamicObject, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré un objet AgeDevice par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'ageDevice</param>
        /// <returns>Objet AgeDevice</returns>
        public AgeDevice GetDeviceAge(int id)
        {
            try
            {
                return _econocomDataManager.GetDeviceAge(id);
            }
            catch (Exception e)
            {
                Logger.Error("GetDeviceAge entrée: id= {0}, sortie=>erreur : {1}", id, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupéré la liste d'objets AgeDevice
        /// </summary>
        /// <returns>Liste age devices</returns>
        public List<AgeDevice> GetDeviceAges()
        {
            try
            {
                return _econocomDataManager.GetDeviceAges();
            }
            catch (Exception e)
            {
                Logger.Error("GetDeviceAges, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarder les données d'un fichier
        /// </summary>
        /// <param name="upload">L'objet contenant les données</param>
        /// <returns>Liste objets</returns>
        public List<object> SauvegardeDonneeDuFichier(Econocom.Model.Models.Service.DocumentUpload upload)
        {
            try
            {
                var stream = new MemoryStream(upload.Data);
                var reader = new CsvReader(new StreamReader(stream, Encoding.UTF8));
                reader.Configuration.Delimiter = ";";
                //var nomTable = FileHelper.GetTypeObjet(upload.DocumentName);                
                var modeleFichier = FileHelper.GetFileModel(upload.DocumentName);
                if (modeleFichier != null)
                {
                    var nomTable = modeleFichier.NomTable;

                    var resultat = new List<object>();
                    if (modeleFichier.Operation == "SUPPRESSION")
                        resultat = SupprimerDonneesReference(nomTable, reader);
                    else if (modeleFichier.Operation == "CREATION")
                        resultat = SauvegardeDonneesReference(nomTable, reader);
                    return resultat;
                }
            }
            catch (Exception e)
            {
                Logger.Error("SauvegardeDonneeDuFichier entrée: upload, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return null;
        }

        public void LogguerListe(Type type, List<object> listeObjets)
        {
            try
            {
                if (type == typeof(Pays))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Pays>().ToList();
                        if (liste.Any())
                            liste.ForEach(
                                x => Logger.Debug("Pays: Id={0}, LibellePays={1}", x.Id, x.LibellePays));
                    }
                }
                else if (type == typeof(CategorieDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<CategorieDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(
                                x =>
                               Logger.Debug("CategorieDevice: Id={0}, LibelleCategorieDevice={1}", x.Id,
                                             x.LibelleCategorieDevice));
                    }
                }
                else if (type == typeof(ClasseDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<ClasseDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(
                                x =>
                               Logger.Debug("ClasseDevice: Id={0}, LibelleClasseDevice={1}", x.Id,
                                             x.LibelleClasseDevice));

                    }
                }
                else if (type == typeof(AgeDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<AgeDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(
                                x =>
                               Logger.Debug("AgeDevice: Id={0}, LibelleAgeDevice={1}", x.Id,
                                             x.LibelleAgeDevice));
                    }
                }
                else if (type == typeof(ConsoWattHeur))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<ConsoWattHeur>().ToList();
                        if (liste.Any())
                            liste.ForEach(
                                x =>
                                Logger.Error(
                                    "ConsoWattHeur: TypeDeviceId={0}, ClasseDeviceId={1}, AgeDeviceId={2}",
                                    x.TypeDeviceId, x.ClasseDeviceId, x.AgeDeviceId));
                    }
                }
                else if (type == typeof(FamilleDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<FamilleDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("FamilleDevice: Id={0}, LibelleFamilleDevice={1}", x.Id, x.LibelleFamilleDevice));
                    }
                }
                else if (type == typeof(Origine))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Origine>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Origine: Id={0}, LibelleOrigine={1}", x.Id, x.LibelleOrigine));
                    }
                }
                else if (type == typeof(Politique))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Politique>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Politique: Id={0}, LibellePolitique={1}", x.Id, x.LibellePolitique));
                    }
                }
                else if (type == typeof(Ratio))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Ratio>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Ratio: SecteurActiviteId={0}, TypeDeviceId={1}", x.SecteurActiviteId, x.TypeDeviceId));
                    }
                }
                else if (type == typeof(SecteurActivite))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<SecteurActivite>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("SecteurActivite: Id={0}, LibelleSecteurActivite={1}", x.Id, x.LibelleSecteurActivite));
                    }
                }
                else if (type == typeof(TypeContact))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<TypeContact>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("TypeContact: Id={0}, LibelleTypeContact={1}", x.Id, x.LibelleTypeContact));
                    }
                }
                else if (type == typeof(TypeDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<TypeDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("TypeDevice: Id={0}, LibelleTypeDevice={1}", x.Id, x.LibelleTypeDevice));
                    }
                }
                else if (type == typeof(TypeObjet))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<TypeObjet>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("TypeObjet: Id={0}, NomObjet={1}", x.Id, x.NomObjet));
                    }
                }
                else if (type == typeof(TypeUsage))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<TypeUsage>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("TypeUsage: Id={0}, LibelleTypeUsage={1}", x.Id, x.LibelleTypeUsage));
                    }
                }
                else if (type == typeof(VentilationClasseAgeDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<VentilationClasseAgeDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("VentilationClasseAgeDevice: SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}, AgeDeviceId={3}", x.SecteurActiviteId, x.TypeDeviceId, x.ClasseDeviceId, x.AgeDeviceId));
                    }
                }
                else if (type == typeof(VentilationClasseDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<VentilationClasseDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("VentilationClasseDevice: SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}", x.SecteurActiviteId, x.TypeDeviceId, x.ClasseDeviceId));
                    }
                }
                else if (type == typeof(Tarif))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Tarif>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Tarif: Id={0}, LibelleTarif={1}", x.Id, x.LibelleTarif));
                    }
                }
                else if (type == typeof(Question))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Question>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Question: Id={0}, LibelleQuestion={1}", x.Id, x.LibelleQuestion));
                    }
                }
                else if (type == typeof(ContenuMail))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<ContenuMail>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("ContenuMail: Id={0}, TypeMailId={1}, LangueId={2}", x.Id, x.TypeMailId, x.LangueId));
                    }
                }
                else if (type == typeof(TypeMail))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<TypeMail>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("TypeMail: Id={0}, LibelleTypeMail={1}", x.Id, x.LibelleTypeMail));
                    }
                }
                else if (type == typeof(ChangementAnneeCalendaire))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<ChangementAnneeCalendaire>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("ChangementAnneeCalendaire: Id={0}, AgeDeviceId={1}", x.Id, x.AgeDeviceId));
                    }
                }
                else if (type == typeof(TypeRatio))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<TypeRatio>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("TypeRatio: Id={0}, LibelleTypeRatio={1}", x.Id, x.LibelleTypeRatio));
                    }
                }
                else if (type == typeof(Equivalence))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Equivalence>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Equivalence: Id={0}, LibelleEquivalence={1}", x.Id, x.LibelleEquivalence));
                    }
                }
                else if (type == typeof(Usage))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Usage>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Usage: Id={0}, TypeUsageId={1}", x.Id, x.TypeUsageId));
                    }
                }
                else if (type == typeof(UsageDevice))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<UsageDevice>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("UsageDevice: Id={0}, TypeDeviceId={1}, UsageId={2}", x.Id, x.TypeDeviceId, x.UsageId));
                    }
                }
                else if (type == typeof(EquivalenceTraduction))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<EquivalenceTraduction>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("EquivalenceTraduction: Id={0}, EquivalenceId={1}, LangueId={2}", x.Id, x.EquivalenceId, x.LangueId));
                    }
                }
                else if (type == typeof(PolitiqueTraduction))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<PolitiqueTraduction>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("PolitiqueTraduction: Id={0}, PolitiqueId={1}, LangueId={2}", x.Id, x.PolitiqueId, x.LangueId));
                    }
                }
                else if (type == typeof(Langue))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Langue>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Langue: Id={0}, PaysId={1}, Nom={2}", x.Id, x.PaysId, x.Nom));
                    }
                }
                else if (type == typeof(Section))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Section>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Section: Id={0}, Nom={1}", x.Id, x.Nom));
                    }
                }
                else if (type == typeof(Modele))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Modele>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Modele: Id={0}, Nom={1}", x.Id, x.Nom));
                    }
                }
                else if (type == typeof(Page))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Page>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Page: Id={0}, Nom={1}", x.Id, x.Nom));
                    }
                }
                else if (type == typeof(CleModele))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<CleModele>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("CleModele: Id={0}, ModeleId={1}", x.Id, x.ModeleId));
                    }
                }
                else if (type == typeof(ContenuModere))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<ContenuModere>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("ContenuModere: Id={0}, PageId={1}, LangueId={2}", x.Id, x.PageId, x.LangueId));
                    }
                }
                else if (type == typeof(Routage))
                {
                    if (listeObjets.Any())
                    {
                        var liste = listeObjets.Cast<Routage>().ToList();
                        if (liste.Any())
                            liste.ForEach(x => Logger.Debug("Routage: Id={0}, PageId={1}, LangueId={2}", x.Id, x.PageId, x.LangueId));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("VerifierDoublons entrée: type, listeObjets , sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        public Type GetMap(Type type)
        {
            try
            {
                if (type == typeof(Pays))
                {
                    return typeof(PaysMap);
                }
                else if (type == typeof(CategorieDevice))
                {
                    return typeof(CategorieDeviceMap);
                }
                else if (type == typeof(ClasseDevice))
                {
                    return typeof(ClasseDeviceMap);
                }
                else if (type == typeof(AgeDevice))
                {
                    return typeof(AgeDeviceMap);
                }
                else if (type == typeof(ConsoWattHeur))
                {
                    return typeof(ConsoWattHeurMap);
                }
                else if (type == typeof(FamilleDevice))
                {
                    return typeof(FamilleDeviceMap);
                }
                else if (type == typeof(Origine))
                {
                    return typeof(OrigineMap);
                }
                else if (type == typeof(Politique))
                {
                    return typeof(PolitiqueMap);
                }
                else if (type == typeof(Ratio))
                {
                    return typeof(RatioMap);
                }
                else if (type == typeof(SecteurActivite))
                {
                    return typeof(SecteurActiviteMap);
                }
                else if (type == typeof(TypeContact))
                {
                    return typeof(TypeContactMap);
                }
                else if (type == typeof(TypeDevice))
                {
                    return typeof(TypeDeviceMap);
                }
                else if (type == typeof(TypeObjet))
                {
                    return typeof(TypeObjetMap);
                }
                else if (type == typeof(TypeUsage))
                {
                    return typeof(TypeUsageMap);
                }
                else if (type == typeof(VentilationClasseAgeDevice))
                {
                    return typeof(VentilationClasseAgeDeviceMap);
                }
                else if (type == typeof(VentilationClasseDevice))
                {
                    return typeof(VentilationClasseDeviceMap);
                }
                else if (type == typeof(Tarif))
                {
                    return typeof(TarifMap);
                }
                else if (type == typeof(Question))
                {
                    return typeof(QuestionMap);
                }
                else if (type == typeof(ContenuMail))
                {
                    return typeof(ContenuMailMap);
                }
                else if (type == typeof(TypeMail))
                {
                    return typeof(TypeMailMap);
                }
                else if (type == typeof(ChangementAnneeCalendaire))
                {
                    return typeof(ChangementAnneeCalendaireMap);
                }
                else if (type == typeof(TypeRatio))
                {
                    return typeof(TypeRatioMap);
                }
                else if (type == typeof(Equivalence))
                {
                    return typeof(EquivalenceMap);
                }
                else if (type == typeof(Usage))
                {
                    return typeof(UsageMap);
                }
                else if (type == typeof(UsageDevice))
                {
                    return typeof(UsageDeviceMap);
                }
                else if (type == typeof(EquivalenceTraduction))
                {
                    return typeof(EquivalenceTraductionMap);
                }
                else if (type == typeof(PolitiqueTraduction))
                {
                    return typeof(PolitiqueTraductionMap);
                }
                else if (type == typeof(Langue))
                {
                    return typeof(LangueMap);
                }
                else if (type == typeof(Devise))
                {
                    return typeof(DeviseMap);
                }
                else if (type == typeof(CorrespondanceSecteurActivite))
                {
                    return typeof(CorrespondanceSecteurActiviteMap);
                }
                else if (type == typeof(CorrespondanceTypeDevice))
                {
                    return typeof(CorrespondanceTypeDeviceMap);
                }
                else if (type == typeof(CorrespondanceProcesseur))
                {
                    return typeof(CorrespondanceProcesseurMap);
                }
                else if (type == typeof(CorrespondanceVitesse))
                {
                    return typeof(CorrespondanceVitesseMap);
                }
                else if (type == typeof(CorrespondanceTaille))
                {
                    return typeof(CorrespondanceTailleMap);
                }
                else if (type == typeof(CorrespondancePuissance))
                {
                    return typeof(CorrespondancePuissanceMap);
                }

            }
            catch (Exception e)
            {
                Logger.Error("GetMap  =>erreur : {0}", e.StackTrace);
                throw;
            }
            return null;
        }


        public List<object> VerifierDoublons(Type type, List<object> listeObjets)
        {
            var doublons = new List<object>() as IEnumerable<object>;
            try
            {
                if (type == typeof(Pays))
                {
                    if (listeObjets.Count > 0)
                    {
                        doublons =
                           listeObjets.Cast<Pays>().Where(
                               a => listeObjets.Cast<Pays>().Except(new List<Pays> { a }).Any(x => x.Id == a.Id && x.LibellePays == a.LibellePays));
                    }
                }
                else if (type == typeof(CategorieDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        doublons =
                            listeObjets.Cast<CategorieDevice>().Where(
                                a =>
                                listeObjets.Cast<CategorieDevice>()
                                           .Except(new List<CategorieDevice> { a })
                                           .Any(
                                               x => x.Id == a.Id && x.LibelleCategorieDevice == a.LibelleCategorieDevice));
                    }
                }
                else if (type == typeof(ClasseDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        doublons =
                            listeObjets.Cast<ClasseDevice>().Where(
                                a =>
                                listeObjets.Cast<ClasseDevice>()
                                           .Except(new List<ClasseDevice> { a })
                                           .Any(x => x.Id == a.Id && x.LibelleClasseDevice == a.LibelleClasseDevice));
                    }
                }
                else if (type == typeof(AgeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        doublons =
                            listeObjets.Cast<AgeDevice>().Where(
                                a =>
                                listeObjets.Cast<AgeDevice>()
                                           .Except(new List<AgeDevice> { a })
                                           .Any(x => x.Id == a.Id && x.LibelleAgeDevice == a.LibelleAgeDevice));
                    }
                }
                else if (type == typeof(ConsoWattHeur))
                {
                    if (listeObjets.Count > 0)
                    {
                        doublons =
                            listeObjets.Cast<ConsoWattHeur>().Where(
                                a =>
                                listeObjets.Cast<ConsoWattHeur>()
                                           .Except(new List<ConsoWattHeur> { a })
                                           .Any(
                                               x =>
                                               x.TypeDeviceId == a.TypeDeviceId && x.ClasseDeviceId == a.ClasseDeviceId &&
                                               x.AgeDeviceId == a.AgeDeviceId));
                    }
                }
                else if (type == typeof(FamilleDevice))
                {
                    doublons =
                        listeObjets.Cast<FamilleDevice>().Where(
                            a =>
                            listeObjets.Cast<FamilleDevice>()
                                       .Except(new List<FamilleDevice> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleFamilleDevice == a.LibelleFamilleDevice));
                }
                else if (type == typeof(Origine))
                {
                    doublons =
                        listeObjets.Cast<Origine>().Where(
                            a =>
                            listeObjets.Cast<Origine>()
                                       .Except(new List<Origine> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleOrigine == a.LibelleOrigine));
                }
                else if (type == typeof(Politique))
                {
                    doublons =
                        listeObjets.Cast<Politique>().Where(
                            a =>
                            listeObjets.Cast<Politique>()
                                       .Except(new List<Politique> { a })
                                       .Any(x => x.Id == a.Id && x.LibellePolitique == a.LibellePolitique));
                }
                else if (type == typeof(Ratio))
                {
                    doublons =
                        listeObjets.Cast<Ratio>().Where(
                            a =>
                            listeObjets.Cast<Ratio>()
                                       .Except(new List<Ratio> { a })
                                       .Any(
                                           x =>
                                           x.SecteurActiviteId == a.SecteurActiviteId &&
                                           x.TypeDeviceId == a.TypeDeviceId));
                }
                else if (type == typeof(SecteurActivite))
                {
                    doublons =
                        listeObjets.Cast<SecteurActivite>().Where(
                            a =>
                            listeObjets.Cast<SecteurActivite>()
                                       .Except(new List<SecteurActivite> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleSecteurActivite == a.LibelleSecteurActivite));
                }
                else if (type == typeof(TypeContact))
                {
                    doublons =
                        listeObjets.Cast<TypeContact>().Where(
                            a =>
                            listeObjets.Cast<TypeContact>()
                                       .Except(new List<TypeContact> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleTypeContact == a.LibelleTypeContact));
                }
                else if (type == typeof(TypeDevice))
                {
                    doublons =
                        listeObjets.Cast<TypeDevice>().Where(
                            a =>
                            listeObjets.Cast<TypeDevice>()
                                       .Except(new List<TypeDevice> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleTypeDevice == a.LibelleTypeDevice));
                }
                else if (type == typeof(TypeObjet))
                {
                    doublons =
                        listeObjets.Cast<TypeObjet>().Where(
                            a =>
                            listeObjets.Cast<TypeObjet>()
                                       .Except(new List<TypeObjet> { a })
                                       .Any(x => x.Id == a.Id && x.NomObjet == a.NomObjet));
                }
                else if (type == typeof(TypeUsage))
                {
                    doublons =
                        listeObjets.Cast<TypeUsage>().Where(
                            a =>
                            listeObjets.Cast<TypeUsage>()
                                       .Except(new List<TypeUsage> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleTypeUsage == a.LibelleTypeUsage));
                }
                else if (type == typeof(VentilationClasseAgeDevice))
                {
                    doublons =
                        listeObjets.Cast<VentilationClasseAgeDevice>().Where(
                            a =>
                            listeObjets.Cast<VentilationClasseAgeDevice>()
                                       .Except(new List<VentilationClasseAgeDevice> { a })
                                       .Any(
                                           x =>
                                           (x.SecteurActiviteId == a.SecteurActiviteId &&
                                            x.TypeDeviceId == a.TypeDeviceId
                                            && x.ClasseDeviceId == a.ClasseDeviceId && x.AgeDeviceId == a.AgeDeviceId)));
                }
                else if (type == typeof(VentilationClasseDevice))
                {
                    doublons =
                        listeObjets.Cast<VentilationClasseDevice>().Where(
                            a =>
                            listeObjets.Cast<VentilationClasseDevice>()
                                       .Except(new List<VentilationClasseDevice> { a })
                                       .Any(
                                           x =>
                                           x.SecteurActiviteId == a.SecteurActiviteId &&
                                           x.TypeDeviceId == a.TypeDeviceId
                                           && x.ClasseDeviceId == a.ClasseDeviceId));
                }
                else if (type == typeof(Tarif))
                {
                    doublons =
                        listeObjets.Cast<Tarif>().Where(
                            a =>
                            listeObjets.Cast<Tarif>()
                                       .Except(new List<Tarif> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleTarif == a.LibelleTarif));
                }
                else if (type == typeof(Question))
                {
                    doublons =
                        listeObjets.Cast<Question>().Where(
                            a =>
                            listeObjets.Cast<Question>()
                                       .Except(new List<Question> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleQuestion == a.LibelleQuestion));
                }
                else if (type == typeof(ContenuMail))
                {
                    doublons =
                        listeObjets.Cast<ContenuMail>().Where(
                            a =>
                            listeObjets.Cast<ContenuMail>()
                                       .Except(new List<ContenuMail> { a })
                                       .Any(
                                           x => x.Id == a.Id && x.TypeMailId == a.TypeMailId && x.LangueId == a.LangueId));
                }
                else if (type == typeof(TypeMail))
                {
                    doublons =
                        listeObjets.Cast<TypeMail>().Where(
                            a =>
                            listeObjets.Cast<TypeMail>()
                                       .Except(new List<TypeMail> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleTypeMail == a.LibelleTypeMail));
                }
                else if (type == typeof(ChangementAnneeCalendaire))
                {
                    doublons =
                        listeObjets.Cast<ChangementAnneeCalendaire>().Where(
                            a =>
                            listeObjets.Cast<ChangementAnneeCalendaire>()
                                       .Except(new List<ChangementAnneeCalendaire> { a })
                                       .Any(x => x.Id == a.Id && x.AgeDeviceId == a.AgeDeviceId));
                }
                else if (type == typeof(TypeRatio))
                {
                    doublons =
                        listeObjets.Cast<TypeRatio>().Where(
                            a =>
                            listeObjets.Cast<TypeRatio>()
                                       .Except(new List<TypeRatio> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleTypeRatio == a.LibelleTypeRatio));
                }
                else if (type == typeof(Equivalence))
                {
                    doublons =
                        listeObjets.Cast<Equivalence>().Where(
                            a =>
                            listeObjets.Cast<Equivalence>()
                                       .Except(new List<Equivalence> { a })
                                       .Any(x => x.Id == a.Id && x.LibelleEquivalence == a.LibelleEquivalence));
                }
                else if (type == typeof(Usage))
                {
                    doublons =
                        listeObjets.Cast<Usage>().Where(
                            a =>
                            listeObjets.Cast<Usage>()
                                       .Except(new List<Usage> { a })
                                       .Any(x => x.Id == a.Id && x.TypeUsageId == a.TypeUsageId));
                }
                else if (type == typeof(UsageDevice))
                {
                    doublons =
                        listeObjets.Cast<UsageDevice>().Where(
                            a =>
                            listeObjets.Cast<UsageDevice>()
                                       .Except(new List<UsageDevice> { a })
                                       .Any(
                                           x =>
                                           x.Id == a.Id && x.TypeDeviceId == a.TypeDeviceId && x.UsageId == a.UsageId));
                }
                else if (type == typeof(EquivalenceTraduction))
                {
                    doublons =
                        listeObjets.Cast<EquivalenceTraduction>().Where(
                            a =>
                            listeObjets.Cast<EquivalenceTraduction>()
                                       .Except(new List<EquivalenceTraduction> { a })
                                       .Any(
                                           x =>
                                           x.Id == a.Id && x.EquivalenceId == a.EquivalenceId &&
                                           x.LangueId == a.LangueId));
                }
                else if (type == typeof(PolitiqueTraduction))
                {
                    doublons =
                        listeObjets.Cast<PolitiqueTraduction>().Where(
                            a =>
                            listeObjets.Cast<PolitiqueTraduction>()
                                       .Except(new List<PolitiqueTraduction> { a })
                                       .Any(
                                           x =>
                                           x.Id == a.Id && x.PolitiqueId == a.PolitiqueId && x.LangueId == a.LangueId));
                }
                else if (type == typeof(Langue))
                {
                    doublons =
                        listeObjets.Cast<Langue>().Where(
                            a =>
                            listeObjets.Cast<Langue>()
                                       .Except(new List<Langue> { a })
                                       .Any(x => x.Id == a.Id && x.PaysId == a.PaysId));
                }
                else if (type == typeof(Section))
                {
                    doublons =
                        listeObjets.Cast<Section>().Where(
                            a =>
                            listeObjets.Cast<Section>()
                                       .Except(new List<Section> { a })
                                       .Any(x => x.Id == a.Id && x.Nom == a.Nom));
                }
                else if (type == typeof(Modele))
                {
                    doublons =
                        listeObjets.Cast<Modele>().Where(
                            a =>
                            listeObjets.Cast<Modele>()
                                       .Except(new List<Modele> { a })
                                       .Any(x => x.Id == a.Id && x.Nom == a.Nom));
                }
                else if (type == typeof(Page))
                {
                    doublons =
                        listeObjets.Cast<Page>().Where(
                            a =>
                            listeObjets.Cast<Page>().Except(new List<Page> { a }).Any(x => x.Id == a.Id && x.Nom == a.Nom));
                }
                else if (type == typeof(CleModele))
                {
                    doublons =
                        listeObjets.Cast<CleModele>().Where(
                            a =>
                            listeObjets.Cast<CleModele>()
                                       .Except(new List<CleModele> { a })
                                       .Any(x => x.Id == a.Id && x.ModeleId == a.ModeleId));

                }
                else if (type == typeof(ContenuModere))
                {
                    doublons =
                        listeObjets.Cast<ContenuModere>().Where(
                            a =>
                            listeObjets.Cast<ContenuModere>()
                                       .Except(new List<ContenuModere> { a })
                                       .Any(x => x.Id == a.Id && x.PageId == a.PageId && x.LangueId == a.LangueId));
                }
                else if (type == typeof(Routage))
                {
                    doublons =
                        listeObjets.Cast<Routage>().Where(
                            a =>
                            listeObjets.Cast<Routage>()
                                       .Except(new List<Routage> { a })
                                       .Any(x => x.Id == a.Id && x.PageId == a.PageId && x.LangueId == a.LangueId));
                }

                if (doublons.Any())
                {
                    var liste = doublons.ToList();
                    Logger.Debug("Objets en doublons DEBUT: ");
                    LogguerListe(type, liste);
                    Logger.Debug("Objets en doublons FIN: ");
                }
            }
            catch (Exception e)
            {
                Logger.Error("VerifierDoublons entrée: type, listeObjets , sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return doublons.ToList();
        }
        /// <summary>
        /// Sauvegarder une liste d'objets
        /// </summary>
        /// <param name="type">Le type d'objet</param>
        /// <param name="listeObjets">La liste des objets</param>
        /// <returns>Liste objets</returns>
        public List<object> SauvegardeListe(Type type, List<object> listeObjets)
        {
            var objetsEnErreur = new List<object>();
            try
            {
                var objet = new object();


                if (type == typeof(Pays))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Pays>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var pays in listeObjets.Cast<Pays>())
                        {
                            if (pays.Id == 0)
                            {
                                ++maxId;
                                pays.Id = maxId;
                            }
                            objet = _econocomDataManager.SetPays(pays);
                            if (objet == null)
                                objetsEnErreur.Add(pays);
                        }
                    }
                }
                else if (type == typeof(CategorieDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CategorieDevice>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var categorieDevice in listeObjets.Cast<CategorieDevice>())
                        {
                            if (categorieDevice.Id == 0)
                            {
                                ++maxId;
                                categorieDevice.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCategorieDevice(categorieDevice);
                            if (objet == null)
                                objetsEnErreur.Add(categorieDevice);
                        }
                    }
                }
                else if (type == typeof(ClasseDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<ClasseDevice>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var classeDevice in listeObjets.Cast<ClasseDevice>())
                        {
                            if (classeDevice.Id == 0)
                            {
                                ++maxId;
                                classeDevice.Id = maxId;
                            }
                            objet = _econocomDataManager.SetClasseDevice(classeDevice);
                            if (objet == null)
                                objetsEnErreur.Add(classeDevice);
                        }
                    }
                }
                else if (type == typeof(AgeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<AgeDevice>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var ageDevice in listeObjets.Cast<AgeDevice>())
                        {
                            if (ageDevice.Id == 0)
                            {
                                ++maxId;
                                ageDevice.Id = maxId;
                            }
                            objet = _econocomDataManager.SetAgeDevice(ageDevice);
                            if (objet == null)
                                objetsEnErreur.Add(ageDevice);
                        }
                    }
                }
                else if (type == typeof(ConsoWattHeur))
                {
                    if (listeObjets.Count > 0)
                    {
                        //var maxObjet = listeObjets.Cast<ConsoWattHeur>().Where(o => o.DateFin == null).OrderByDescending(x => x.Id).Take(1).Single();
                        //var maxId = maxObjet.Id;

                        foreach (var consoWattHeur in listeObjets.Cast<ConsoWattHeur>())
                        {
                            objet = _econocomDataManager.SetConsoWattHeur(consoWattHeur);
                            if (objet == null)
                                objetsEnErreur.Add(consoWattHeur);
                        }
                    }
                }
                else if (type == typeof(FamilleDevice))
                {
                    var maxObjet =
                        listeObjets.Cast<FamilleDevice>()
                                   .Where(o => o.DateFin == null)
                                   .OrderByDescending(x => x.Id)
                                   .Take(1)
                                   .Single();
                    var maxId = maxObjet.Id;
                    if (listeObjets.Count > 0)
                    {
                        foreach (var familleDevice in listeObjets.Cast<FamilleDevice>())
                        {
                            if (familleDevice.Id == 0)
                            {
                                ++maxId;
                                familleDevice.Id = maxId;
                            }
                            objet = _econocomDataManager.SetFamilleDevice(familleDevice);
                            if (objet == null)
                                objetsEnErreur.Add(familleDevice);
                        }
                    }
                }
                else if (type == typeof(Origine))
                {
                    var maxObjet =
                        listeObjets.Cast<Origine>()
                                   .Where(o => o.DateFin == null)
                                   .OrderByDescending(x => x.Id)
                                   .Take(1)
                                   .Single();
                    var maxId = maxObjet.Id;
                    if (listeObjets.Count > 0)
                    {
                        foreach (var origine in listeObjets.Cast<Origine>())
                        {
                            if (origine.Id == 0)
                            {
                                ++maxId;
                                origine.Id = maxId;
                            }
                            objet = _econocomDataManager.SetOrigine(origine);
                            if (objet == null)
                                objetsEnErreur.Add(origine);
                        }
                    }
                }
                else if (type == typeof(Politique))
                {
                    var maxObjet =
                        listeObjets.Cast<Politique>()
                                   .Where(o => o.DateFin == null)
                                   .OrderByDescending(x => x.Id)
                                   .Take(1)
                                   .Single();
                    var maxId = maxObjet.Id;
                    if (listeObjets.Count > 0)
                    {
                        foreach (var politique in listeObjets.Cast<Politique>())
                        {
                            if (politique.Id == 0)
                            {
                                ++maxId;
                                politique.Id = maxId;
                            }
                            objet = _econocomDataManager.SetPolitique(politique);
                            if (objet == null)
                                objetsEnErreur.Add(politique);
                        }
                    }
                }
                else if (type == typeof(Ratio))
                {
                    //var maxObjet = listeObjets.Cast<Ratio>().Where(o => o.DateFin == null).OrderByDescending(x => x.Id).Take(1).Single();
                    //var maxId = maxObjet.Id;
                    if (listeObjets.Count > 0)
                    {
                        foreach (var ratio in listeObjets.Cast<Ratio>())
                        {
                            objet = _econocomDataManager.SetRatio(ratio);
                            if (objet == null)
                                objetsEnErreur.Add(ratio);
                        }
                    }
                }
                else if (type == typeof(SecteurActivite))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<SecteurActivite>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;
                        foreach (var secteurActivite in listeObjets.Cast<SecteurActivite>())
                        {
                            if (secteurActivite.Id == 0)
                            {
                                ++maxId;
                                secteurActivite.Id = maxId;
                            }
                            objet = _econocomDataManager.SetSecteurActivite(secteurActivite);
                            if (objet == null)
                                objetsEnErreur.Add(secteurActivite);
                        }
                    }
                }
                else if (type == typeof(TypeContact))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<TypeContact>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var typeContact in listeObjets.Cast<TypeContact>())
                        {
                            if (typeContact.Id == 0)
                            {
                                ++maxId;
                                typeContact.Id = maxId;
                            }
                            objet = _econocomDataManager.SetTypeContact(typeContact);
                            if (objet == null)
                                objetsEnErreur.Add(typeContact);
                        }
                    }
                }
                else if (type == typeof(TypeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<TypeDevice>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var typeDevice in listeObjets.Cast<TypeDevice>())
                        {
                            if (typeDevice.Id == 0)
                            {
                                ++maxId;
                                typeDevice.Id = maxId;
                            }
                            objet = _econocomDataManager.SetTypeDevice(typeDevice);
                            if (objet == null)
                                objetsEnErreur.Add(typeDevice);
                        }
                    }
                }
                else if (type == typeof(TypeObjet))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet = listeObjets.Cast<TypeObjet>().OrderByDescending(x => x.Id).Take(1).Single();
                        var maxId = maxObjet.Id;

                        foreach (var typeObjet in listeObjets.Cast<TypeObjet>())
                        {
                            if (typeObjet.Id == 0)
                            {
                                ++maxId;
                                typeObjet.Id = maxId;
                            }
                            objet = _econocomDataManager.SetTypeObjet(typeObjet);
                            if (objet == null)
                                objetsEnErreur.Add(typeObjet);
                        }
                    }
                }
                else if (type == typeof(TypeUsage))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<TypeUsage>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var typeUsage in listeObjets.Cast<TypeUsage>())
                        {
                            if (typeUsage.Id == 0)
                            {
                                ++maxId;
                                typeUsage.Id = maxId;
                            }
                            objet = _econocomDataManager.SetTypeUsage(typeUsage);
                            if (objet == null)
                                objetsEnErreur.Add(typeUsage);
                        }
                    }
                }
                else if (type == typeof(VentilationClasseAgeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var ventilationClasseAgeDevice in listeObjets.Cast<VentilationClasseAgeDevice>())
                        {
                            objet = _econocomDataManager.SetVentilationClasseAgeDevice(ventilationClasseAgeDevice);
                            if (objet == null)
                                objetsEnErreur.Add(ventilationClasseAgeDevice);
                        }
                    }
                }
                else if (type == typeof(VentilationClasseDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var ventilationClasseDevice in listeObjets.Cast<VentilationClasseDevice>())
                        {
                            objet = _econocomDataManager.SetVentilationClasseDevice(ventilationClasseDevice);
                            if (objet == null)
                                objetsEnErreur.Add(ventilationClasseDevice);
                        }
                    }
                }
                else if (type == typeof(Tarif))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Tarif>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var tarif in listeObjets.Cast<Tarif>())
                        {
                            if (tarif.Id == 0)
                            {
                                ++maxId;
                                tarif.Id = maxId;
                            }
                            objet = _econocomDataManager.SetTarif(tarif);
                            if (objet == null)
                                objetsEnErreur.Add(tarif);
                        }
                    }
                }
                else if (type == typeof(Question))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Question>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var question in listeObjets.Cast<Question>())
                        {
                            if (question.Id == 0)
                            {
                                ++maxId;
                                question.Id = maxId;
                            }
                            objet = _econocomDataManager.SetQuestion(question);
                            if (objet == null)
                                objetsEnErreur.Add(question);
                        }
                    }
                }
                else if (type == typeof(ContenuMail))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<ContenuMail>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var contenuMail in listeObjets.Cast<ContenuMail>())
                        {
                            if (contenuMail.Id == 0)
                            {
                                ++maxId;
                                contenuMail.Id = maxId;
                            }
                            objet = _econocomDataManager.SetContenuMail(contenuMail);
                            if (objet == null)
                                objetsEnErreur.Add(contenuMail);
                        }
                    }
                }
                else if (type == typeof(TypeMail))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<TypeMail>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var typeMail in listeObjets.Cast<TypeMail>())
                        {
                            if (typeMail.Id == 0)
                            {
                                ++maxId;
                                typeMail.Id = maxId;
                            }
                            objet = _econocomDataManager.SetTypeMail(typeMail);
                            if (objet == null)
                                objetsEnErreur.Add(typeMail);
                        }
                    }
                }
                else if (type == typeof(ChangementAnneeCalendaire))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<ChangementAnneeCalendaire>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var changementAnneeCalendaire in listeObjets.Cast<ChangementAnneeCalendaire>())
                        {
                            if (changementAnneeCalendaire.Id == 0)
                            {
                                ++maxId;
                                changementAnneeCalendaire.Id = maxId;
                            }
                            objet = _econocomDataManager.SetChangementAnneeCalendaire(changementAnneeCalendaire);
                            if (objet == null)
                                objetsEnErreur.Add(changementAnneeCalendaire);
                        }
                    }
                }
                else if (type == typeof(TypeRatio))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<TypeRatio>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var typeRatio in listeObjets.Cast<TypeRatio>())
                        {
                            if (typeRatio.Id == 0)
                            {
                                ++maxId;
                                typeRatio.Id = maxId;
                            }
                            objet = _econocomDataManager.SetTypeRatio(typeRatio);
                            if (objet == null)
                                objetsEnErreur.Add(typeRatio);
                        }
                    }
                }
                else if (type == typeof(Equivalence))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Equivalence>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var equivalence in listeObjets.Cast<Equivalence>())
                        {
                            if (equivalence.Id == 0)
                            {
                                ++maxId;
                                equivalence.Id = maxId;
                            }
                            objet = _econocomDataManager.SetEquivalence(equivalence);
                            if (objet == null)
                                objetsEnErreur.Add(equivalence);
                        }
                    }
                }
                else if (type == typeof(Usage))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Usage>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var usage in listeObjets.Cast<Usage>())
                        {
                            if (usage.Id == 0)
                            {
                                ++maxId;
                                usage.Id = maxId;
                            }
                            objet = _econocomDataManager.SetUsage(usage);
                            if (objet == null)
                                objetsEnErreur.Add(usage);
                        }
                    }
                }
                else if (type == typeof(UsageDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<UsageDevice>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var usageDevice in listeObjets.Cast<UsageDevice>())
                        {
                            if (usageDevice.Id == 0)
                            {
                                ++maxId;
                                usageDevice.Id = maxId;
                            }
                            objet = _econocomDataManager.SetUsageDevice(usageDevice);
                            if (objet == null)
                                objetsEnErreur.Add(usageDevice);
                        }
                    }
                }
                else if (type == typeof(EquivalenceTraduction))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<EquivalenceTraduction>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var equivalenceTraduction in listeObjets.Cast<EquivalenceTraduction>())
                        {
                            if (equivalenceTraduction.Id == 0)
                            {
                                ++maxId;
                                equivalenceTraduction.Id = maxId;
                            }

                            objet = _econocomDataManager.SetEquivalenceTraduction(equivalenceTraduction);
                            if (objet == null)
                                objetsEnErreur.Add(equivalenceTraduction);
                        }
                    }
                }
                else if (type == typeof(PolitiqueTraduction))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<PolitiqueTraduction>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var politiqueTraduction in listeObjets.Cast<PolitiqueTraduction>())
                        {
                            if (politiqueTraduction.Id == 0)
                            {
                                ++maxId;
                                politiqueTraduction.Id = maxId;
                            }
                            objet = _econocomDataManager.SetPolitiqueTraduction(politiqueTraduction);
                            if (objet == null)
                                objetsEnErreur.Add(politiqueTraduction);
                        }
                    }
                }
                else if (type == typeof(Langue))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Langue>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var langue in listeObjets.Cast<Langue>())
                        {
                            if (langue.Id == 0)
                            {
                                ++maxId;
                                langue.Id = maxId;
                            }
                            objet = _econocomDataManager.SetLangue(langue);
                            if (objet == null)
                                objetsEnErreur.Add(langue);
                        }
                    }
                }
                else if (type == typeof(Section))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Section>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var section in listeObjets.Cast<Section>())
                        {
                            if (section.Id == 0)
                            {
                                ++maxId;
                                section.Id = maxId;
                            }
                            objet = _econocomDataManager.SetSection(section);
                            if (objet == null)
                                objetsEnErreur.Add(section);
                        }
                    }
                }
                else if (type == typeof(Modele))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Modele>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var modele in listeObjets.Cast<Modele>())
                        {
                            if (modele.Id == 0)
                            {
                                ++maxId;
                                modele.Id = maxId;
                            }
                            objet = _econocomDataManager.SetModele(modele);
                            if (objet == null)
                                objetsEnErreur.Add(modele);
                        }
                    }
                }
                else if (type == typeof(Page))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Page>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var page in listeObjets.Cast<Page>())
                        {
                            if (page.Id == 0)
                            {
                                ++maxId;
                                page.Id = maxId;
                            }
                            objet = _econocomDataManager.SetPage(page);
                            if (objet == null)
                                objetsEnErreur.Add(page);
                        }
                    }
                }
                else if (type == typeof(CleModele))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CleModele>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var cleModele in listeObjets.Cast<CleModele>())
                        {
                            if (cleModele.Id == 0)
                            {
                                ++maxId;
                                cleModele.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCleModele(cleModele);
                            if (objet == null)
                                objetsEnErreur.Add(cleModele);
                        }
                    }
                }
                else if (type == typeof(ContenuModere))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<ContenuModere>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var contenuModere in listeObjets.Cast<ContenuModere>())
                        {
                            if (contenuModere.Id == 0)
                            {
                                ++maxId;
                                contenuModere.Id = maxId;
                            }
                            objet = _econocomDataManager.SetContenuModere(contenuModere);
                            if (objet == null)
                                objetsEnErreur.Add(contenuModere);
                        }
                    }
                }
                else if (type == typeof(Routage))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Routage>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var routage in listeObjets.Cast<Routage>())
                        {
                            if (routage.Id == 0)
                            {
                                ++maxId;
                                routage.Id = maxId;
                            }
                            objet = _econocomDataManager.SetRoutage(routage);
                            if (objet == null)
                                objetsEnErreur.Add(routage);
                        }
                    }
                }
                else if (type == typeof(Devise))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<Devise>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var devise in listeObjets.Cast<Devise>())
                        {
                            if (devise.Id == 0)
                            {
                                ++maxId;
                                devise.Id = maxId;
                            }
                            objet = _econocomDataManager.SetDevise(devise);
                            if (objet == null)
                                objetsEnErreur.Add(devise);
                        }
                    }
                }
                else if (type == typeof(CorrespondanceSecteurActivite))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CorrespondanceSecteurActivite>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var correspondanceSecteurActivite in listeObjets.Cast<CorrespondanceSecteurActivite>())
                        {
                            if (correspondanceSecteurActivite.Id == 0)
                            {
                                ++maxId;
                                correspondanceSecteurActivite.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCorrespondanceSecteurActivite(correspondanceSecteurActivite);
                            if (objet == null)
                                objetsEnErreur.Add(correspondanceSecteurActivite);
                        }
                    }
                }
                else if (type == typeof(CorrespondanceTypeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CorrespondanceTypeDevice>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var correspondanceTypeDevice in listeObjets.Cast<CorrespondanceTypeDevice>())
                        {
                            if (correspondanceTypeDevice.Id == 0)
                            {
                                ++maxId;
                                correspondanceTypeDevice.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCorrespondanceTypeDevice(correspondanceTypeDevice);
                            if (objet == null)
                                objetsEnErreur.Add(correspondanceTypeDevice);
                        }
                    }
                }
                else if (type == typeof(CorrespondanceProcesseur))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CorrespondanceProcesseur>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var correspondanceProcesseur in listeObjets.Cast<CorrespondanceProcesseur>())
                        {
                            if (correspondanceProcesseur.Id == 0)
                            {
                                ++maxId;
                                correspondanceProcesseur.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCorrespondanceProcesseur(correspondanceProcesseur);
                            if (objet == null)
                                objetsEnErreur.Add(correspondanceProcesseur);
                        }
                    }
                }
                else if (type == typeof(CorrespondanceTaille))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CorrespondanceTaille>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var correspondanceTaille in listeObjets.Cast<CorrespondanceTaille>())
                        {
                            if (correspondanceTaille.Id == 0)
                            {
                                ++maxId;
                                correspondanceTaille.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCorrespondanceTaille(correspondanceTaille);
                            if (objet == null)
                                objetsEnErreur.Add(correspondanceTaille);
                        }
                    }
                }
                else if (type == typeof(CorrespondanceVitesse))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CorrespondanceVitesse>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var correspondanceVitesse in listeObjets.Cast<CorrespondanceVitesse>())
                        {
                            if (correspondanceVitesse.Id == 0)
                            {
                                ++maxId;
                                correspondanceVitesse.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCorrespondanceVitesse(correspondanceVitesse);
                            if (objet == null)
                                objetsEnErreur.Add(correspondanceVitesse);
                        }
                    }
                }
                else if (type == typeof(CorrespondancePuissance))
                {
                    if (listeObjets.Count > 0)
                    {
                        var maxObjet =
                            listeObjets.Cast<CorrespondancePuissance>()
                                       .Where(o => o.DateFin == null)
                                       .OrderByDescending(x => x.Id)
                                       .Take(1)
                                       .Single();
                        var maxId = maxObjet.Id;

                        foreach (var correspondancePuissance in listeObjets.Cast<CorrespondancePuissance>())
                        {
                            if (correspondancePuissance.Id == 0)
                            {
                                ++maxId;
                                correspondancePuissance.Id = maxId;
                            }
                            objet = _econocomDataManager.SetCorrespondancePuissance(correspondancePuissance);
                            if (objet == null)
                                objetsEnErreur.Add(correspondancePuissance);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("SauvegardeListe entrée: type, listeObjets , sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return objetsEnErreur;
        }


        /// <summary>
        /// Sauvegarder les données du fichier csv
        /// </summary>
        /// <param name="nomTable">Nom de la table de la BDD, exemple "TypeObjet"</param>
        /// <param name="reader">Les donnees du fichier csv</param>
        /// <returns>Une liste des objets en erreurs dans le fichier</returns>
        private List<object> SauvegardeDonneesReference(string nomTable, CsvReader reader)
        {
            List<object> objetsEnErreur = null;
            try
            {
                var typeObjetRepository = new Repository<TypeObjet>();
                var listeTypeObjet = _econocomDataManager.GetListeTypeObjet();
                if (listeTypeObjet.AsQueryable().Any(t => t.NomTable.Equals(nomTable)))
                {
                    var objetType = typeObjetRepository.All().First(t => t.NomTable.Equals(nomTable));
                    var typeObjetNamespace = objetType.NomObjet + "," + objetType.Namespace;
                    var type = Type.GetType(typeObjetNamespace);

                    reader.Configuration.RegisterClassMap(GetMap(type));
                    reader.Configuration.Delimiter = ";";
                    var listeObjets = reader.GetRecords(type).ToList();

                    objetsEnErreur = VerifierDoublons(type, listeObjets);
                    if (objetsEnErreur == null || objetsEnErreur.Count == 0)
                    {
                        objetsEnErreur = SauvegardeListe(type, listeObjets);
                        if (objetsEnErreur != null && objetsEnErreur.Count > 0)
                        {
                            Logger.Debug("Objets en erreur: Debut");
                            LogguerListe(type, objetsEnErreur);
                            Logger.Debug("Objets en erreur: Fin");
                        }
                    }
                }
                else
                {
                    //var currentObjectType = typeof (TypeObjet).Name;
                    if (typeof(TypeObjet).Name.Equals(nomTable))
                    {
                        var type = typeof(TypeObjet);
                        var listeObjets = reader.GetRecords(type).ToList();
                        objetsEnErreur = VerifierDoublons(type, listeObjets);
                        if (objetsEnErreur == null || objetsEnErreur.Count == 0)
                            objetsEnErreur = SauvegardeListe(type, listeObjets);
                        if (objetsEnErreur != null && objetsEnErreur.Count > 0)
                        {
                            Logger.Debug("Objets en erreur: Debut");
                            LogguerListe(type, objetsEnErreur);
                            Logger.Debug("Objets en erreur: Fin");
                        }
                    }
                    else
                    {
                        Logger.Error("SauvegardeDonneesReference: La table {0} n'existe pas, le nom du fichier est invalide", nomTable);
                    }
                }
                return objetsEnErreur;
            }
            catch (Exception e)
            {
                Logger.Error("SauvegardeDonneesReference entrée: nomTable= {0}, reader , sortie=>erreur : {1}", nomTable, e.StackTrace);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Methode pour sauvegarder les données d'un fichier csv
        /// </summary>
        /// <param name="stream">Le fichier</param>
        /// <param name="fileName">Le nom du fichier</param>
        /// <returns>Une liste des lignes du fichier qui sont en erreur</returns>
        public List<object> SauvegardeDonneeDuFichier(Stream stream, string fileName)
        {
            var objetsEnErreur = new List<object>();
            try
            {
                var reader = new CsvReader(new StreamReader(stream));
                var typeObjetRepository = new Repository<TypeObjet>();
                var nomTable = FileHelper.GetTypeObjet(fileName);
                objetsEnErreur = SauvegardeDonneesReference(nomTable, reader);
                return objetsEnErreur;
            }
            catch (Exception e)
            {
                Logger.Error("SauvegardeDonneeDuFichier entrée: stream, fileName= {0}, sortie=>erreur : {1}", fileName, e.StackTrace);
                throw;
            }
            return objetsEnErreur;
        }

        /// <summary>
        /// Supprime les donnees de reference
        /// </summary>
        /// <param name="nomTable">Nom de la table</param>
        /// <param name="reader">Objet CsvReader</param>
        /// <returns>Liste Objets</returns>
        private List<object> SupprimerDonneesReference(string nomTable, CsvReader reader)
        {
            var objetsEnErreur = new List<object>();
            try
            {
                var typeObjetRepository = new Repository<TypeObjet>();
                var listeTypeObjet = _econocomDataManager.GetListeTypeObjet();
                if (listeTypeObjet.AsQueryable().Any(t => t.NomTable.Equals(nomTable)))
                {
                    var objetType = typeObjetRepository.All().First(t => t.NomTable.Equals(nomTable));
                    var typeObjetNamespace = objetType.NomObjet + "," + objetType.Namespace;
                    var type = Type.GetType(typeObjetNamespace);
                    var listeObjets = reader.GetRecords(type).ToList();
                    objetsEnErreur = SupprimerListe(type, listeObjets);
                }
                else
                {
                    if (typeof(TypeObjet).Name.Equals(nomTable))
                    {
                        var type = typeof(TypeObjet);
                        var listeObjets = reader.GetRecords(type).ToList();
                        objetsEnErreur = SupprimerListe(type, listeObjets);
                    }
                }
                return objetsEnErreur;
            }
            catch (Exception e)
            {
                Logger.Error("SupprimerDonneesReference entrée: nomTable= {0}, reader, sortie=>erreur : {1}", nomTable, e.StackTrace);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Supprimer liste d'objets
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="listeObjets">Liste d'objets</param>
        /// <returns>Liste objets</returns>
        public List<object> SupprimerListe(Type type, List<object> listeObjets)
        {
            var objetsEnErreur = new List<object>();
            try
            {
                bool resultat = false;

                if (type == typeof(Pays))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var pays in listeObjets.Cast<Pays>())
                        {
                            resultat = _econocomDataManager.SupprimerPays(pays);
                            if (!resultat)
                                objetsEnErreur.Add(pays);
                        }
                    }
                }
                else if (type == typeof(CategorieDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var categorieDevice in listeObjets.Cast<CategorieDevice>())
                        {
                            resultat = _econocomDataManager.SupprimerCategorieDevice(categorieDevice);
                            if (!resultat)
                                objetsEnErreur.Add(categorieDevice);
                        }
                    }
                }
                else if (type == typeof(ClasseDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var classeDevice in listeObjets.Cast<ClasseDevice>())
                        {
                            resultat = _econocomDataManager.SupprimerClasseDevice(classeDevice);
                            if (!resultat)
                                objetsEnErreur.Add(classeDevice);
                        }
                    }
                }
                else if (type == typeof(AgeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var ageDevice in listeObjets.Cast<AgeDevice>())
                        {
                            resultat = _econocomDataManager.SupprimerAgeDevice(ageDevice);
                            if (!resultat)
                                objetsEnErreur.Add(ageDevice);
                        }
                    }
                }
                else if (type == typeof(ConsoWattHeur))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var consoWattHeur in listeObjets.Cast<ConsoWattHeur>())
                        {
                            resultat = _econocomDataManager.SupprimerConsoWattHeur(consoWattHeur);
                            if (!resultat)
                                objetsEnErreur.Add(consoWattHeur);
                        }
                    }
                }
                else if (type == typeof(FamilleDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var familleDevice in listeObjets.Cast<FamilleDevice>())
                        {
                            resultat = _econocomDataManager.SupprimerFamilleDevice(familleDevice);
                            if (!resultat)
                                objetsEnErreur.Add(familleDevice);
                        }
                    }
                }
                else if (type == typeof(Origine))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var origine in listeObjets.Cast<Origine>())
                        {
                            resultat = _econocomDataManager.SupprimerOrigine(origine);
                            if (!resultat)
                                objetsEnErreur.Add(origine);
                        }
                    }
                }
                else if (type == typeof(Politique))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var politique in listeObjets.Cast<Politique>())
                        {
                            resultat = _econocomDataManager.SupprimerPolitique(politique);
                            if (!resultat)
                                objetsEnErreur.Add(politique);
                        }
                    }
                }
                else if (type == typeof(PolitiqueTraduction))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var politiqueTraduction in listeObjets.Cast<PolitiqueTraduction>())
                        {
                            resultat = _econocomDataManager.SupprimerPolitiqueTraduction(politiqueTraduction);
                            if (!resultat)
                                objetsEnErreur.Add(politiqueTraduction);
                        }
                    }
                }
                else if (type == typeof(Ratio))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var ratio in listeObjets.Cast<Ratio>())
                        {
                            resultat = _econocomDataManager.SupprimerRatio(ratio);
                            if (!resultat)
                                objetsEnErreur.Add(ratio);
                        }
                    }
                }
                else if (type == typeof(SecteurActivite))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var secteurActivite in listeObjets.Cast<SecteurActivite>())
                        {
                            resultat = _econocomDataManager.SupprimerSecteurActivite(secteurActivite);
                            if (!resultat)
                                objetsEnErreur.Add(secteurActivite);
                        }
                    }
                }
                else if (type == typeof(TypeContact))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var typeContact in listeObjets.Cast<TypeContact>())
                        {
                            resultat = _econocomDataManager.SupprimerTypeContact(typeContact);
                            if (!resultat)
                                objetsEnErreur.Add(typeContact);
                        }
                    }
                }
                else if (type == typeof(TypeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var typeDevice in listeObjets.Cast<TypeDevice>())
                        {
                            resultat = _econocomDataManager.SupprimerTypeDevice(typeDevice);
                            if (!resultat)
                                objetsEnErreur.Add(typeDevice);
                        }
                    }
                }
                else if (type == typeof(TypeObjet))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var typeObjet in listeObjets.Cast<TypeObjet>())
                        {
                            resultat = _econocomDataManager.SupprimerTypeObjet(typeObjet);
                            if (!resultat)
                                objetsEnErreur.Add(typeObjet);
                        }
                    }
                }
                else if (type == typeof(TypeUsage))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var typeUsage in listeObjets.Cast<TypeUsage>())
                        {
                            resultat = _econocomDataManager.SupprimerTypeUsage(typeUsage);
                            if (!resultat)
                                objetsEnErreur.Add(typeUsage);
                        }
                    }
                }
                else if (type == typeof(VentilationClasseAgeDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var ventilationClasseAgeDevice in listeObjets.Cast<VentilationClasseAgeDevice>())
                        {
                            resultat =
                                _econocomDataManager.SupprimerVentilationClasseAgeDevice(ventilationClasseAgeDevice);
                            if (!resultat)
                                objetsEnErreur.Add(ventilationClasseAgeDevice);
                        }
                    }
                }
                else if (type == typeof(VentilationClasseDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var ventilationClasseDevice in listeObjets.Cast<VentilationClasseDevice>())
                        {
                            resultat = _econocomDataManager.SupprimerVentilationClasseDevice(ventilationClasseDevice);
                            if (!resultat)
                                objetsEnErreur.Add(ventilationClasseDevice);
                        }
                    }
                }
                else if (type == typeof(Tarif))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var tarif in listeObjets.Cast<Tarif>())
                        {
                            resultat = _econocomDataManager.SupprimerTarif(tarif);
                            if (!resultat)
                                objetsEnErreur.Add(tarif);
                        }
                    }
                }
                else if (type == typeof(Question))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var question in listeObjets.Cast<Question>())
                        {
                            resultat = _econocomDataManager.SupprimerQuestion(question);
                            if (!resultat)
                                objetsEnErreur.Add(question);
                        }
                    }
                }
                else if (type == typeof(ContenuMail))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var contenuMail in listeObjets.Cast<ContenuMail>())
                        {
                            resultat = _econocomDataManager.SupprimerContenuMail(contenuMail);
                            if (!resultat)
                                objetsEnErreur.Add(contenuMail);
                        }
                    }
                }
                else if (type == typeof(TypeMail))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var typeMail in listeObjets.Cast<TypeMail>())
                        {
                            resultat = _econocomDataManager.SupprimerTypeMail(typeMail);
                            if (!resultat)
                                objetsEnErreur.Add(typeMail);
                        }
                    }
                }
                else if (type == typeof(ChangementAnneeCalendaire))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var changementAnneeCalendaire in listeObjets.Cast<ChangementAnneeCalendaire>())
                        {
                            resultat = _econocomDataManager.SupprimerChangementAnneeCalendaire(changementAnneeCalendaire);
                            if (!resultat)
                                objetsEnErreur.Add(changementAnneeCalendaire);
                        }
                    }
                }
                else if (type == typeof(TypeRatio))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var typeRatio in listeObjets.Cast<TypeRatio>())
                        {
                            resultat = _econocomDataManager.SupprimerTypeRatio(typeRatio);
                            if (!resultat)
                                objetsEnErreur.Add(typeRatio);
                        }
                    }
                }
                else if (type == typeof(Equivalence))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var equivalence in listeObjets.Cast<Equivalence>())
                        {
                            resultat = _econocomDataManager.SupprimerEquivalence(equivalence);
                            if (!resultat)
                                objetsEnErreur.Add(equivalence);
                        }
                    }
                }
                else if (type == typeof(EquivalenceTraduction))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var equivalenceTraduction in listeObjets.Cast<EquivalenceTraduction>())
                        {
                            resultat = _econocomDataManager.SupprimerEquivalenceTraduction(equivalenceTraduction);
                            if (!resultat)
                                objetsEnErreur.Add(equivalenceTraduction);
                        }
                    }
                }
                else if (type == typeof(Usage))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var usage in listeObjets.Cast<Usage>())
                        {
                            resultat = _econocomDataManager.SupprimerUsage(usage);
                            if (!resultat)
                                objetsEnErreur.Add(usage);
                        }
                    }
                }
                else if (type == typeof(UsageDevice))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var usageDevice in listeObjets.Cast<UsageDevice>())
                        {
                            resultat = _econocomDataManager.SupprimerUsageDevice(usageDevice);
                            if (!resultat)
                                objetsEnErreur.Add(usageDevice);
                        }
                    }
                }
                else if (type == typeof(Langue))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var langue in listeObjets.Cast<Langue>())
                        {
                            resultat = _econocomDataManager.SupprimerLangue(langue);
                            if (!resultat)
                                objetsEnErreur.Add(langue);
                        }
                    }
                }
                else if (type == typeof(Section))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var section in listeObjets.Cast<Section>())
                        {
                            resultat = _econocomDataManager.SupprimerSection(section);
                            if (!resultat)
                                objetsEnErreur.Add(section);
                        }
                    }
                }
                else if (type == typeof(Modele))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var modele in listeObjets.Cast<Modele>())
                        {
                            resultat = _econocomDataManager.SupprimerModele(modele);
                            if (!resultat)
                                objetsEnErreur.Add(modele);
                        }
                    }
                }
                else if (type == typeof(Page))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var page in listeObjets.Cast<Page>())
                        {
                            resultat = _econocomDataManager.SupprimerPage(page);
                            if (!resultat)
                                objetsEnErreur.Add(page);
                        }
                    }
                }
                else if (type == typeof(CleModele))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var cleModele in listeObjets.Cast<CleModele>())
                        {
                            resultat = _econocomDataManager.SupprimerCleModele(cleModele);
                            if (!resultat)
                                objetsEnErreur.Add(cleModele);
                        }
                    }
                }
                else if (type == typeof(ContenuModere))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var contenuModere in listeObjets.Cast<ContenuModere>())
                        {
                            resultat = _econocomDataManager.SupprimerContenuModere(contenuModere);
                            if (!resultat)
                                objetsEnErreur.Add(contenuModere);
                        }
                    }
                }
                else if (type == typeof(Routage))
                {
                    if (listeObjets.Count > 0)
                    {
                        foreach (var routage in listeObjets.Cast<Routage>())
                        {
                            resultat = _econocomDataManager.SupprimerRoutage(routage);
                            if (!resultat)
                                objetsEnErreur.Add(routage);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("SupprimerListe entrée: type, listeObjets, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return objetsEnErreur;
        }

        /// <summary>
        /// Récupérer la liste de pays
        /// </summary>
        /// <returns>Liste de pays</returns>
        public List<Pays> ListePays(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var liste = _econocomDataManager.ListePays(currentPage, pageDimension, out totalPages);
                return liste;
            }
            catch (Exception e)
            {
                Logger.Error("ListePays, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }
            return null;
        }

        /// <summary>
        /// Récupérer la liste d'origines
        /// </summary>
        /// <returns>Liste des origines</returns>
        public List<Origine> ListeOrigines(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.ListeOrigines(currentPage, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error("ListeOrigines, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Mettre à jour contenu publié
        /// </summary>
        /// <param name="editedPublishedContent">Objet EditedPublishedContent</param>
        /// <returns>Objet EditPublishedContent </returns>
        public EditPublishedContent UpdatePublishedContent(EditPublishedContent editedPublishedContent)
        {
            try
            {
                return _econocomDataManager.UpdatePublishedContent(editedPublishedContent);
            }
            catch (Exception e)
            {
                Logger.Error("UpdatePublishedContent entrée: editedPublishedContent, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer la liste de Pages
        /// </summary>
        /// <returns>Liste de pages</returns>
        public List<Page> GetPages()
        {
            try
            {
                return _econocomDataManager.GetPages();
            }
            catch (Exception e)
            {
                Logger.Error("GetPages, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Récupérer la liste de TypeObjets
        /// </summary>
        /// <returns>Liste TypeObjets</returns>
        public List<TypeObjet> ListeTypeObjet(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.ListeTypeObjet(currentPage, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error("GetPages, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Récupérer la liste d'adresses
        /// </summary>
        /// <returns>Liste Adresse</returns>
        public List<Adresse> ListeAdresse()
        {
            try
            {
                return _econocomDataManager.ListeAdresse();
            }
            catch (Exception e)
            {
                Logger.Error("ListeAdresse, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Récupérer la liste de ConsoWattHeur
        /// </summary>
        /// <returns>Liste ConsoWattHeurs</returns>
        public List<ConsoWattHeur> ListeConsoWattHeur(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.ListeConsoWattHeur(currentPage, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error("ListeConsoWattHeur, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarder ConsoWattHeur
        /// </summary>
        /// <param name="consowattheur">Objet consowattheur</param>
        /// <returns>Objet UploadResult</returns>
        public UploadResult SetConsoWattHeur(ConsoWattHeur consowattheur)
        {
            try
            {
                //_econocomDataManager.SetConsoWattHeur(consowattheur);
                var result = new UploadResult();
                result.Status = true;
                result.Error = "Cannot save";
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("SetConsoWattHeur entrée: consowattheur, sortie=>erreur : {0}", e.StackTrace);
                var result = new UploadResult();
                result.Status = false;
                result.Error = "Cannot save";
                return result;
            }
        }

        /// <summary>
        /// Sauvegarder un client
        /// </summary>
        /// <param name="client">Objet Client</param>
        /// <returns>Vrai / Faux</returns>
        public bool SauvegardeClient(Client client)
        {
            try
            {
                if (client != null)
                {
                    client = ValidateClient(client);
                    _econocomDataManager.SauvegardeClient(client);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error("SauvegardeClient entrée: client, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return false;
        }

        /// <summary>
        /// Récupérer la liste de Langues
        /// </summary>
        /// <returns>Liste de langues</returns>
        public List<Langue> ListeLangues(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.ListeLangues(currentPage, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error("ListeLangues, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer la liste de Contenu Modéré par le nom de la page et la culture de la langue
        /// </summary>
        /// <param name="nomDePage">Nom de la page</param>
        /// <param name="cultureDeLangue">Culture de la langue</param>
        /// <returns>Liste de Contenu Modéré</returns>
        public List<ContenuModere> ListeContenuPublier(string nomDePage, string cultureDeLangue)
        {
            try
            {
                var resultat = _econocomDataManager.ListeContenuPublier(nomDePage, cultureDeLangue);
                if (resultat != null)
                    Logger.Debug("ListeContenuPublier entrée: nomDePage= {0}, cultureDeLangue= {1}, sortie : ListeContenuPublier=pas nulle",
                                 nomDePage, cultureDeLangue);
                else
                    Logger.Debug("GetPublishedContent entrée: nomDePage= {0}, cultureDeLangue= {1}, sortie : ListeContenuPublier=nulle",
                                 nomDePage, cultureDeLangue);

                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("ListeContenuPublier entrée: nomDePage= {0}, cultureDeLangue= {1}, sortie=>erreur : {2}", e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Récupérer le Contenu publié par le lien de la page, le nom de la page et la culture de la langue
        /// </summary>
        /// <param name="lien">Le lien de la page</param>
        /// <param name="nomDePage">Le nom de la page</param>
        /// <param name="cultureDeLangue">La culture de la langue</param>
        /// <returns>Objet ContenuModereViewModel</returns>
        public ContenuModereViewModel GetContenuPublier(string lien, string nomDePage, string cultureDeLangue)
        {
            try
            {
                var resultat = _econocomDataManager.GetContenuPublier(lien, nomDePage, cultureDeLangue);
                if (resultat != null)
                    Logger.Debug("GetPublishedContent entrée: nomDePage= {0}, sortie : IdContenuPublier= {2}",
                                nomDePage, resultat.Id);
                else
                    Logger.Debug("GetPublishedContent entrée: nomDePage= {0}, sortie : ContenuPublier=nulle",
                                nomDePage);

                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("GetPublishedContent entrée: nomDePage= {0}, sortie=>erreur : {2}", e.StackTrace);
                throw;
            }
        }


        /// <summary>
        /// Mettre à jour un client
        /// </summary>
        /// <param name="client">Objet client</param>
        /// <returns>Vrai / Faux</returns>
        public bool ActualiserClient(Client client)
        {
            try
            {
                if (client != null)
                {
                    foreach (Contact contact in client.Contacts)
                    {
                        var guid = System.Guid.NewGuid();
                        contact.CodeVerification = guid.ToString();
                        _econocomDataManager.MAJContact(contact);

                    }
                    _econocomDataManager.MAJAdresse(client.Adresse);
                    _econocomDataManager.ActualiserClient(client);
                    //envoi mail??
                    EmailHelper.SendRegistrationMailToContacts(client.Contacts);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error("ActualiserClient entrée: client= {0}, sortie=>erreur : {2}", e.StackTrace);
                throw;
            }
            return false;
        }


        /// <summary>
        /// Récupérer la liste de Pages
        /// </summary>
        /// <returns>Liste de pages</returns>
        public List<Page> ListePage(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var resultat = _econocomDataManager.ListePage(currentPage, pageDimension, out totalPages);
                if (resultat != null)
                    Logger.Debug("ListePage : pas nulle");
                else
                    Logger.Debug("ListePage : nulle");

                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("ListePage: sortie=>erreur : {0}", e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Mettre à jour un contenu modéré
        /// </summary>
        /// <param name="contenuModere">Objet contenuModere</param>
        /// <returns>Objet Contenu Modéré</returns>
        public ContenuModere MAJContenuModere(ContenuModere contenuModere)
        {
            try
            {
                var resultat = _econocomDataManager.MAJContenuModere(contenuModere);
                if (resultat != null)
                    Logger.Debug("MAJContenuModere : pas nulle");
                else
                    Logger.Debug("MAJContenuModere : nulle");

                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("MAJContenuModere: entrée: contenuModere, sortie=>erreur : {1}", e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Récupérer la liste de clients non modérés
        /// </summary>
        /// <param name="page">Page actuelle</param>
        /// <param name="pageDimension">Nombre de clients à afficher par page</param>
        /// <param name="totalPages">Nombre total de pages</param>
        /// <returns>Liste de clients</returns>
        public List<Client> ClientsNonModerer(int? page, int pageDimension, out int totalPages)
        {
            try
            {
                var resultat = _econocomDataManager.ClientsNonModerer(page, pageDimension, out totalPages);
                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("ClientsNonModerer entrée: page= {0}, pageDimension= {1}, totalPages= {2}, sortie=>erreur :  {3}", page, pageDimension, e.StackTrace);
                totalPages = 0;
                return null;
            }
        }

        /// <summary>
        ///  Récupérer un client par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <returns>Objet Client</returns>
        public Client GetClientParId(int id)
        {
            try
            {
                var client = _econocomDataManager.GetClientParId(id);

                if (client != null)
                {
                    if (client.DetailsClient != null)
                    {
                        var devise = client.DetailsClient.Devise;

                        var idLangue = 1;
                        var langue = _econocomDataManager.GetLangue();
                        if (langue != null)
                        {
                            idLangue = langue.Id;
                        }
                        if (devise != null)
                        {
                            if (devise.DeviseTraductions != null)
                            {
                                var deviseTraductions = devise.DeviseTraductions.ToList();
                                var deviseTraduit = deviseTraductions.Find(o => o.LangueId == idLangue);
                                devise.Libelle = deviseTraduit != null ? deviseTraduit.Libelle : devise.Libelle;
                            }
                        }
                    }
                }

                return client;
            }
            catch (Exception e)
            {
                Logger.Error("GetClientParId entrée: id= {0}, sortie=>erreur : {1}", id, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer la liste de pays
        /// </summary>
        /// <returns>Liste de pays</returns>
        public List<Pays> GetPays()
        {
            try
            {
                var listePays = _econocomDataManager.GetPays();
                if (listePays != null)
                {
                    foreach (var pays in listePays)
                    {
                        if (pays.PaysTraductions != null && pays.PaysTraductions.ElementAt(0) != null)
                            pays.LibellePays = pays.PaysTraductions.ElementAt(0).LibellePays;
                        else
                            pays.LibellePays = pays.LibellePays;
                    }
                }

                return listePays;
            }
            catch (Exception e)
            {
                Logger.Error("GetPays, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Récupérer la liste de sections
        /// </summary>
        /// <returns>Liste de sections</returns>
        public List<Section> ListeSection(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var resultat = _econocomDataManager.ListeSection(currentPage, pageDimension, out totalPages);
                if (resultat != null)
                    Logger.Debug("ListeSection : pas nulle");
                else
                    Logger.Debug("ListeSection : nulle");

                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("ListeSection: sortie=>erreur : {0}", e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Récupérer contact par code de vérification
        /// </summary>
        /// <param name="verificationCode">Le code de vérification</param>
        /// <returns>Objet Contact</returns>
        public Contact GetContactParVerificationCode(string verificationCode)
        {
            try
            {
                var contact = _econocomDataManager.GetContactParVerificationCode(verificationCode);
                return contact;
            }
            catch (Exception e)
            {
                Logger.Error("GetContactParVerificationCode entrée: verificationCode= {0}, sortie=>erreur : {1}", verificationCode, e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Récupérer la liste de questions 
        /// </summary>
        /// <returns>Liste de questions</returns>
        public List<Question> GetQuestions()
        {
            try
            {
                var listeQuestions = _econocomDataManager.GetQuestions();
                foreach (var question in listeQuestions)
                {
                    if (question.QuestionTraductions != null && question.QuestionTraductions.ElementAt(0) != null)
                        question.LibelleQuestion = question.QuestionTraductions.ElementAt(0).LibelleQuestion;
                    else
                        question.LibelleQuestion = question.LibelleQuestion;
                }
                return listeQuestions;
            }
            catch (Exception e)
            {
                Logger.Error("GetQuestions, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Récupérer question par identifiant
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Objet Question</returns>
        public Question GetQuestionParId(int id)
        {
            try
            {
                var question = _econocomDataManager.GetQuestion(id);
                if (question != null)
                {
                    if (question.QuestionTraductions != null && question.QuestionTraductions.ElementAt(0) != null)
                    {
                        question.LibelleQuestion = question.QuestionTraductions.ElementAt(0).LibelleQuestion;
                    }
                }
                return question;
            }
            catch (Exception e)
            {
                Logger.Error("GetQuestionParId entrée: id= {0}, sortie=>erreur : {1}", id, e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Mettre à jour Contact
        /// </summary>
        /// <param name="contact">Objet contact</param>
        /// <returns>Objet Contact</returns>
        public Contact MAJContact(Contact contact)
        {
            try
            {
                if (contact != null)
                {
                    var contactMAJ = _econocomDataManager.MAJContact(contact);
                    return contactMAJ;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                Logger.Error("MAJContact entrée: contact, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Vérification de la création d'un compte
        /// </summary>
        /// <param name="enregistrementModel">Objet RegisterViewModel</param>
        /// <returns>Objet Contact</returns>
        public Contact VerificationCompte(RegisterViewModel enregistrementModel)
        {
            try
            {
                if (enregistrementModel != null)
                {
                    var existingContact = _econocomDataManager.GetContactByVerificationCode(enregistrementModel.Id.ToString());
                    if (existingContact != null)
                    {
                        existingContact.MotPasse = HashHelper.HashPassWord(enregistrementModel.Password);
                        var question = _econocomDataManager.GetQuestion(enregistrementModel.QuestionId);
                        var reponse = new Reponse();
                        reponse.QuestionId = question.Id;
                        reponse.Valeur = enregistrementModel.Answer;
                        var reponseSauvegarder = _econocomDataManager.SetReponse(reponse);
                        existingContact.CodeVerification = null;
                        var contact = _econocomDataManager.MAJContact(existingContact);
                        return contact;
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("VerificationCompte entrée: enregistrementModel, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        //Not used ??
        public Client ValidateClient(string status, Client client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sauvegarde une Section
        /// </summary>
        /// <param name="sectionViewModel">Object sectionViewModel</param>
        /// <returns>Vrai / Faux</returns>
        public bool SauvegardeSection(SectionViewModel sectionViewModel)
        {
            try
            {
                if (sectionViewModel != null)
                {
                    _econocomDataManager.SauvegardeSection(sectionViewModel);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error("SauvegardeSection entrée: sectionViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return false;
        }

        /// <summary>
        /// Sauvegarde une page
        /// </summary>
        /// <param name="pageViewModel">Objet PageViewModel</param>
        /// <returns>Vrai / Faux</returns>
        public bool SauvegardePage(PageViewModel pageViewModel)
        {
            try
            {
                if (pageViewModel != null)
                {
                    _econocomDataManager.SauvegardePage(pageViewModel);
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error("SauvegardePage entrée: pageViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return false;
        }

        /// <summary>
        /// Récupérer la liste de tarifs
        /// </summary>
        /// <returns>Liste de tarifs</returns>
        public List<Tarif> GetListOfTarifs()
        {
            try
            {
                return _econocomDataManager.GetListOfTarifs();
            }
            catch (Exception e)
            {
                Logger.Error("GetListOfTarifs, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Récupérer la liste de modèles
        /// </summary>
        /// <returns>Liste de modèles</returns>
        public List<Modele> ListeModele(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.ListeModele(currentPage, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error("ListeModele, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Récupérer la liste de clients modérés
        /// </summary>
        /// <returns>Liste de clients</returns>
        public List<Client> ClientsModerer()
        {
            try
            {
                var resultat = _econocomDataManager.ClientsModerer();
                return resultat;
            }
            catch (Exception e)
            {
                Logger.Error("ClientsModerer, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }
        }

        public CalculateurViewModel GetCalculateurViewModel(List<FamilleDevice> listefamilleDevice, Client client, ConfigSi configSi)
        {
            var calculateurViewModel = new CalculateurViewModel();
            var listeFamilleDeviceViewModel = new List<FamilleDeviceViewModel>();
            try
            {
                var secteurActiviteId = client.SecteurActiviteId.Value;

                foreach (FamilleDevice familleDevice in listefamilleDevice)
                {
                    //Conversion familleDevice -> familleDeviceViewModel
                    var familleDeviceViewModel = new FamilleDeviceViewModel();
                    familleDeviceViewModel = FamilleDeviceViewModel(client.Id, familleDevice, familleDeviceViewModel);

                    //Obtenir liste CategorieDevice pour chaque famille
                    List<CategorieDevice> listeCategorieDevice = _econocomDataManager.GetCategorieDeviceParFamille(familleDevice.Id);

                    var listeCategorieDeviceViewModel = new List<CategorieDeviceViewModel>();

                    foreach (CategorieDevice categorieDevice in listeCategorieDevice)
                    {
                        //Conversion categorieDevice -> categorieDeviceViewModel
                        var categorieDeviceViewModel = new CategorieDeviceViewModel();
                        categorieDeviceViewModel = CategorieDeviceViewModel(client.Id, categorieDevice, categorieDeviceViewModel);

                        //Obtenir liste TypeDevice pour chaque categorie
                        List<TypeDevice> listeTypeDevice = _econocomDataManager.GetTypeDeviceParCategorie(categorieDevice.Id, secteurActiviteId);

                        var listeTypeDeviceViewModel = new List<TypeDeviceViewModel>();

                        foreach (TypeDevice typeDevice in listeTypeDevice)
                        {
                            try
                            {
                                //Conversion typeDevice -> typeDeviceViewModel
                                var typeDeviceViewModel = new TypeDeviceViewModel();
                                typeDeviceViewModel = TypeDeviceViewModel(typeDevice, typeDeviceViewModel);

                                // Obtenir ratio pour chaque TypeDevice
                                Ratio ratio = _econocomDataManager.GetRatio(secteurActiviteId, typeDevice.Id);
                                var ratioViewModel = RatioViewModel(ratio);

                                // Obtenir typeRatio
                                TypeRatio typeRatio = ratio.TypeRatio;
                                var typeRatioViewModel = TypeRatioViewModel(typeRatio);
                                ratioViewModel.TypeRatioViewModel = typeRatioViewModel;
                                ratioViewModel.TypeRatioId = typeRatio.Id;

                                // Obtenir Ventilation Classe Device pour chaque TypeDevice
                                var ventilationClasseDeviceViewModels = VentilationClasseDeviceViewModels(secteurActiviteId, typeDevice);

                                // Obtenir Ventilation Classe Age Device pour chaque TypeDevice
                                var ventilationClasseAgeDeviceViewModels = VentilationClasseAgeDeviceViewModels(secteurActiviteId, typeDevice);

                                // Obtenir Usage pour chaque TypeDevice
                                Usage usage = _econocomDataManager.GetUsageParTypeDevice(typeDevice.Id);

                                var usageViewModel = DetaillesUsageViewModel(usage);

                                // Obtenir typeUsage
                                TypeUsage typeUsage = usage.TypeUsage;
                                var typeUsageViewModel = TypeUsageViewModel(typeUsage);

                                usageViewModel.TypeUsage = typeUsageViewModel;

                                // Obtenir ConsoWattHeur pour chaque TypeDevice
                                var consoWattHeurViewModels = ConsoWattHeurViewModels(typeDevice);

                                typeDeviceViewModel.Ratio = ratioViewModel;
                                typeDeviceViewModel.VentilationClasseDevices = ventilationClasseDeviceViewModels;
                                typeDeviceViewModel.VentilationClasseAgeDevices = ventilationClasseAgeDeviceViewModels;
                                typeDeviceViewModel.Usage = usageViewModel;
                                typeDeviceViewModel.ConsoWattHeurs = consoWattHeurViewModels;

                                //Creation ConfigSiDevice
                                var configSiDevice = ConfigSiDevice(configSi, typeDevice, usage);
                                typeDeviceViewModel.ConfigSiDevice = configSiDevice;

                                // if (typeUtilisateur == "Payant")
                                //{
                                configSiDevice = _econocomDataManager.SauvegardeConfigSiDevice(configSiDevice);
                                if (configSiDevice != null)
                                {
                                    Logger.Debug("IdConfigSiDevice sauvegardé = " + configSiDevice.Id);
                                    typeDeviceViewModel.ConfigSiDevice = configSiDevice;
                                }
                                else
                                {
                                    Logger.Debug("ConfigSiDevice nulle");
                                }
                                // }
                                listeTypeDeviceViewModel.Add(typeDeviceViewModel);
                            }
                            catch (Exception e)
                            {
                                LogguerErreur(e);
                            }
                        }
                        categorieDeviceViewModel.TypeDevices = listeTypeDeviceViewModel;
                        listeCategorieDeviceViewModel.Add(categorieDeviceViewModel);
                    }
                    familleDeviceViewModel.CategorieDevices = listeCategorieDeviceViewModel;
                    listeFamilleDeviceViewModel.Add(familleDeviceViewModel);
                }
                calculateurViewModel.FamilleDeviceViewModels = listeFamilleDeviceViewModel;

                calculateurViewModel.Effectif = client.DetailsClient.Effectif;
                calculateurViewModel.PrixKWatt = client.DetailsClient.PrixKWH;
                calculateurViewModel.CoefficientConversion = (client.DetailsClient != null && client.DetailsClient.CoefficientCO2 != 0) ? client.DetailsClient.CoefficientCO2 : client.Adresse.Pays.CoefficientConversion; //client.Adresse.Pays.CoefficientConversion;
                calculateurViewModel.TauxEquipement = client.DetailsClient.TauxEquipement;
                calculateurViewModel.SuperficieDataCenter = client.DetailsClient.SuperficieDataCenter;
                calculateurViewModel.NombreSite = client.DetailsClient.NombreSite;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }



            return calculateurViewModel;

        }

        public CalculateurViewModel GetCalculateurViewModelPourDemo()
        {
            return null;
        }

        public CalculateurViewModel GetCalculateurViewModelPourSouscription()
        {
            return null;
        }

        /// <summary>
        /// Construire l'objet pour alimenter le calculateur et mettre à jour les tables nécéssaires après la calculation
        /// </summary>
        /// <param name="idClient">Identifiant du client</param>
        /// <param name="typeUtilisateur">Type d'utilisateur : demo / payant / simulation</param>
        /// <returns>Objet CalculateurViewModel</returns>
        public CalculateurViewModel GetResultatCalculateur(int idClient, string typeUtilisateur)
        {
            var debut = ConsommationHelper.GetStartTime("BusinessService GetResultatCalculateur ");
            Logger.Debug("idClient = " + idClient);
            Logger.Debug("idClient = " + typeUtilisateur);

            var calculateurViewModel = new CalculateurViewModel();
            ConfigSiClient configClient = null;

            //Creation ConfigSi
            var configSi = new ConfigSi();
            configSi.LibelleConfigSi = "ConfigSi" + idClient;
            configSi.DateDebut = DateTime.Now;
            configSi.Statut = (int)ConfigSiStatut.Creer;
            calculateurViewModel.ConfigSi = configSi;

            //Creation ConfigSiClient
            var configSiClient = new ConfigSiClient();
            configSiClient.ClientId = Convert.ToInt32(idClient);
            configSiClient.DateDebut = DateTime.Now;
            calculateurViewModel.ConfigSiClient = configSiClient;

            var compteEvaluation = new CompteEvaluation();
            var client = new Client();
            var listefamilleDevice = new List<FamilleDevice>();
            int secteurActiviteId = 0;

            if (typeUtilisateur == "Demo")
            {
                compteEvaluation = _econocomDataManager.GetCompteEvaluation(idClient);

                //client = _econocomDataManager.GetClientParId(Convert.ToInt32(idClient));

                if (compteEvaluation != null)
                {
                    Logger.Debug("idCompteEvaluation = " + compteEvaluation.Id);

                    //Obtenir liste FamilleDevice par SecteurActivite du client non-payant
                    listefamilleDevice = _econocomDataManager.GetFamilleDeviceParSecteurActivite(compteEvaluation.SecteurActivite.Id);
                    secteurActiviteId = compteEvaluation.SecteurActivite.Id;
                }
                else
                {
                    Logger.Error("CompteEvaluation nulle");
                }
            }
            else if (typeUtilisateur == "Payant")
            {
                client = _econocomDataManager.GetClientParId(Convert.ToInt32(idClient));
                configClient = _econocomDataManager.GetConfigSiClient(idClient);
                secteurActiviteId = client.SecteurActivite.Id;

                //Pour client connectant pour la premiere fois
                if (configClient == null)
                {
                    var debutconfigclient = ConsommationHelper.GetStartTime("Client connectant pour la premiere fois");
                    //Logger.Debug("Client connectant pour la premiere fois");
                    configSi = _econocomDataManager.SauvegardeConfigSi(configSi);
                    configSiClient.ConfigSiId = configSi.Id;

                    var origine = _econocomDataManager.GetOrigineParLibelle(OrigineEnum.Actuel.ToString());
                    configSiClient.OrigineId = origine.Id;

                    configSiClient = _econocomDataManager.SauvegardeConfigSiClient(configSiClient);
                    configSiClient.ConfigSi = configSi;

                    //Obtenir liste FamilleDevice par SecteurActivite du client payant
                    listefamilleDevice = _econocomDataManager.GetFamilleDeviceParSecteurActivite(client.SecteurActivite.Id);
                    var finconfigclient = ConsommationHelper.GetTimeDifference(debutconfigclient, "Client connectant pour la premiere fois");
                }
                else
                {
                    //Logger.Debug("Client connectant pour les autres fois");
                    var debutconfigexistant = ConsommationHelper.GetStartTime("Client connectant pour les autres fois");
                    //Pour client connectant pour les autres fois
                    calculateurViewModel = GetDetailsConfigPourClientExistant(configClient, client, typeUtilisateur, null);
                    var finconfigexistant = ConsommationHelper.GetTimeDifference(debutconfigexistant, "Client connectant pour les autres fois");
                }
            }

            #region ConfigClientNull
            if (configClient == null)
            {
                //Construire FamilleDevice Liste
                calculateurViewModel = ConstruireFamilleDeviceListe(calculateurViewModel, listefamilleDevice, idClient,
                                                                    secteurActiviteId, typeUtilisateur);

                //Pour client demo
                if (typeUtilisateur == "Demo")
                {
                    calculateurViewModel.Effectif = compteEvaluation.Effectif;
                    calculateurViewModel.PrixKWatt = compteEvaluation.PrixMoyenKwatt;
                    calculateurViewModel.CoefficientConversion = compteEvaluation.Pays.CoefficientConversion;
                    calculateurViewModel.TauxEquipement = compteEvaluation.TauxEquipement;
                    calculateurViewModel.SuperficieDataCenter = null;
                    calculateurViewModel.NombreSite = compteEvaluation.NombreSites;
                }
                //Pour client payant
                else
                {
                    calculateurViewModel.Effectif = client.DetailsClient.Effectif;
                    calculateurViewModel.PrixKWatt = client.DetailsClient.PrixKWH;
                    calculateurViewModel.CoefficientConversion = client.Adresse.Pays.CoefficientConversion;
                    calculateurViewModel.TauxEquipement = client.DetailsClient.TauxEquipement;
                    calculateurViewModel.SuperficieDataCenter = client.DetailsClient.SuperficieDataCenter;
                    calculateurViewModel.NombreSite = client.DetailsClient.NombreSite;
                }

                var calculator = new Calculator();
                var debut2 = ConsommationHelper.GetStartTime("CalculerTotalConsommation start");
                calculateurViewModel = calculator.CalculerTotalConsommation(calculateurViewModel);
                var fin2 = ConsommationHelper.GetTimeDifference(debut2, "CalculerTotalConsommation end");

                if (typeUtilisateur == "Payant")
                {
                    //Update configSiDevice + sauvegarde configSiConsommation for device 
                    var debut3 = ConsommationHelper.GetStartTime("FamilleDeviceViewModels start");
                    foreach (var familleDevice in calculateurViewModel.FamilleDeviceViewModels)
                    {
                        foreach (var categorieDevice in familleDevice.CategorieDevices)
                        {
                            foreach (var typeDevice in categorieDevice.TypeDevices)
                            {
                                var configSiDevice = _econocomDataManager.GetConfigSiDevice(typeDevice.ConfigSiDevice.Id);
                                configSiDevice.Quantite = typeDevice.ConfigSiDevice.Quantite;
                                var updatedConfigSiDevice = _econocomDataManager.MAJConfigSiDevice(configSiDevice);

                                decimal? consommationWatt = 0;
                                decimal? consommationCO2 = 0;
                                foreach (var configSiConsommation in typeDevice.ConfigSiDevice.ConfigSiConsommations)
                                {
                                    try
                                    {
                                        var x = _econocomDataManager.SauveGarderConfigSiConsommation(configSiConsommation);

                                        consommationWatt = configSiConsommation.ConsommationWatt + consommationWatt;
                                        consommationCO2 = configSiConsommation.ConsommationCo2 + consommationCO2;
                                    }
                                    catch (Exception e)
                                    {
                                        LogguerErreur(e);
                                    }
                                    Logger.Debug("ConfigSiConsommations sauvegardés pour configSiDevice = " + updatedConfigSiDevice.Id);
                                }

                                //sauvegarde resultat pour chaque configSiDevice
                                try
                                {
                                    var resultat = new Resultat();
                                    resultat.ConsommationWatt = consommationWatt;
                                    resultat.ConsommationCo2 = consommationCO2;
                                    resultat.CoefficientConversion = client.Adresse.Pays.CoefficientConversion;
                                    resultat.ConfigSiId = configSi.Id;
                                    resultat.ConfigSiDeviceId = updatedConfigSiDevice.Id;
                                    resultat = _econocomDataManager.SauvegardeResultat(resultat);
                                }
                                catch (Exception e)
                                {
                                    LogguerErreur(e);
                                }
                                Logger.Debug("Resultat sauvegardé pour configSiDevice = " + updatedConfigSiDevice.Id);
                            }
                        }
                    }
                    var fin3 = ConsommationHelper.GetTimeDifference(debut3, "FamilleDeviceViewModels end");
                }
            }
            #endregion
            //Logger.Debug("GetResultatCalculateur : End");
            var fin = ConsommationHelper.GetTimeDifference(debut, "BusinessService GetResultatCalculateur");
            return calculateurViewModel;
        }


        public CalculateurViewModel ConstruireFamilleDeviceListe(CalculateurViewModel calculateurViewModel, List<FamilleDevice> listefamilleDevice, int idClient, int secteurActiviteId, string typeUtilisateur)
        {
            var listeFamilleDeviceViewModel = new List<FamilleDeviceViewModel>();
            try
            {
                var debut1 = ConsommationHelper.GetStartTime("listefamilleDevice start");
                foreach (FamilleDevice familleDevice in listefamilleDevice)
                {
                    //Conversion familleDevice -> familleDeviceViewModel
                    var familleDeviceViewModel = new FamilleDeviceViewModel();
                    familleDeviceViewModel = FamilleDeviceViewModel(idClient, familleDevice, familleDeviceViewModel);

                    //Obtenir liste CategorieDevice pour chaque famille
                    List<CategorieDevice> listeCategorieDevice = _econocomDataManager.GetCategorieDeviceParFamille(familleDevice.Id);

                    var listeCategorieDeviceViewModel = new List<CategorieDeviceViewModel>();

                    foreach (CategorieDevice categorieDevice in listeCategorieDevice)
                    {
                        //Conversion categorieDevice -> categorieDeviceViewModel
                        var categorieDeviceViewModel = new CategorieDeviceViewModel();
                        categorieDeviceViewModel = CategorieDeviceViewModel(idClient, categorieDevice, categorieDeviceViewModel);

                        //Obtenir liste TypeDevice pour chaque categorie
                        List<TypeDevice> listeTypeDevice = _econocomDataManager.GetTypeDeviceParCategorie(categorieDevice.Id, secteurActiviteId);

                        var listeTypeDeviceViewModel = new List<TypeDeviceViewModel>();

                        foreach (TypeDevice typeDevice in listeTypeDevice)
                        {
                            try
                            {
                                //Conversion typeDevice -> typeDeviceViewModel
                                var typeDeviceViewModel = new TypeDeviceViewModel();
                                typeDeviceViewModel = TypeDeviceViewModel(typeDevice, typeDeviceViewModel);

                                if (typeUtilisateur != "Import")
                                {
                                    // Obtenir ratio pour chaque TypeDevice
                                    Ratio ratio = _econocomDataManager.GetRatio(secteurActiviteId, typeDevice.Id);
                                    var ratioViewModel = RatioViewModel(ratio);

                                    // Obtenir typeRatio
                                    TypeRatio typeRatio = ratio.TypeRatio;
                                    var typeRatioViewModel = TypeRatioViewModel(typeRatio);
                                    ratioViewModel.TypeRatioViewModel = typeRatioViewModel;
                                    ratioViewModel.TypeRatioId = typeRatio.Id;

                                    typeDeviceViewModel.Ratio = ratioViewModel;
                                }

                                // Obtenir Ventilation Classe Device pour chaque TypeDevice
                                var ventilationClasseDeviceViewModels = VentilationClasseDeviceViewModels(secteurActiviteId, typeDevice);

                                // Obtenir Ventilation Classe Age Device pour chaque TypeDevice
                                var ventilationClasseAgeDeviceViewModels = VentilationClasseAgeDeviceViewModels(secteurActiviteId, typeDevice);

                                // Obtenir Usage pour chaque TypeDevice
                                Usage usage = _econocomDataManager.GetUsageParTypeDevice(typeDevice.Id);

                                var usageViewModel = DetaillesUsageViewModel(usage);

                                // Obtenir typeUsage
                                TypeUsage typeUsage = usage.TypeUsage;
                                var typeUsageViewModel = TypeUsageViewModel(typeUsage);

                                usageViewModel.TypeUsage = typeUsageViewModel;

                                // Obtenir ConsoWattHeur pour chaque TypeDevice
                                var consoWattHeurViewModels = ConsoWattHeurViewModels(typeDevice);

                                typeDeviceViewModel.VentilationClasseDevices = ventilationClasseDeviceViewModels;
                                typeDeviceViewModel.VentilationClasseAgeDevices = ventilationClasseAgeDeviceViewModels;
                                typeDeviceViewModel.Usage = usageViewModel;
                                typeDeviceViewModel.ConsoWattHeurs = consoWattHeurViewModels;

                                //Creation ConfigSiDevice
                                var configSiDevice = ConfigSiDevice(calculateurViewModel.ConfigSi, typeDevice, usage);
                                typeDeviceViewModel.ConfigSiDevice = configSiDevice;

                                if (typeUtilisateur == "Payant")
                                {
                                    configSiDevice = _econocomDataManager.SauvegardeConfigSiDevice(configSiDevice);
                                    if (configSiDevice != null)
                                    {
                                        Logger.Debug("IdConfigSiDevice sauvegardé = " + configSiDevice.Id);
                                        typeDeviceViewModel.ConfigSiDevice = configSiDevice;
                                    }
                                    else
                                    {
                                        Logger.Debug("ConfigSiDevice nulle");
                                    }
                                }
                                else if (typeUtilisateur == "Demo" || typeUtilisateur == "Import")
                                {
                                    typeDeviceViewModel.IsModified = true;
                                }

                                listeTypeDeviceViewModel.Add(typeDeviceViewModel);
                            }
                            catch (Exception e)
                            {
                                LogguerErreur(e);
                            }
                        }
                        categorieDeviceViewModel.TypeDevices = listeTypeDeviceViewModel;
                        listeCategorieDeviceViewModel.Add(categorieDeviceViewModel);
                    }
                    familleDeviceViewModel.CategorieDevices = listeCategorieDeviceViewModel;
                    listeFamilleDeviceViewModel.Add(familleDeviceViewModel);
                }
                var fin1 = ConsommationHelper.GetTimeDifference(debut1, "listefamilleDevice end");
                calculateurViewModel.FamilleDeviceViewModels = listeFamilleDeviceViewModel;

                return calculateurViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        /// <summary>
        /// Construire un objet TypeUsageViewModel à partir d'un objet TypeUsage
        /// </summary>
        /// <param name="typeUsage">Objet typeUsage</param>
        /// <returns>Objet TypeUsageViewModel</returns>
        private static TypeUsageViewModel TypeUsageViewModel(TypeUsage typeUsage)
        {
            try
            {
                object typeUsageObj = Activator.CreateInstance(typeof(TypeUsageViewModel));
                typeUsageObj.InjectFrom(typeUsage);
                var typeUsageViewModel = (TypeUsageViewModel)typeUsageObj;
                return typeUsageViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("TypeUsageViewModel entrée: typeUsage, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire un objet DetaillesUsageViewModel à partir d'un objet Usage
        /// </summary>
        /// <param name="usage">Objet usage</param>
        /// <returns></returns>
        private static DetaillesUsageViewModel DetaillesUsageViewModel(Usage usage)
        {
            try
            {
                object usageObj = Activator.CreateInstance(typeof(DetaillesUsageViewModel));
                usageObj.InjectFrom(usage);
                var usageViewModel = (DetaillesUsageViewModel)usageObj;
                return usageViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("DetaillesUsageViewModel entrée: usage, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire un objet TypeRatioViewModel à partir d'un objet TypeRatio
        /// </summary>
        /// <param name="typeRatio">Objet typeRatio</param>
        /// <returns>Objet TypeRatioViewModel</returns>
        private static TypeRatioViewModel TypeRatioViewModel(TypeRatio typeRatio)
        {
            try
            {
                object typeRatioObj = Activator.CreateInstance(typeof(TypeRatioViewModel));
                typeRatioObj.InjectFrom(typeRatio);
                var typeRatioViewModel = (TypeRatioViewModel)typeRatioObj;

                if (typeRatio.TypeRatioTraductions != null && typeRatio.TypeRatioTraductions.ElementAt(0) != null)
                    typeRatioViewModel.LibelleTypeRatio = typeRatio.TypeRatioTraductions.ElementAt(0).LibelleTypeRatio;

                return typeRatioViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("TypeRatioViewModel entrée: typeRatio, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire un objet RatioViewModel à partir d'un objet Ratio
        /// </summary>
        /// <param name="ratio">Objet ratio</param>
        /// <returns>Objet RatioViewModel</returns>
        private static RatioViewModel RatioViewModel(Ratio ratio)
        {
            try
            {
                object ratioObj = Activator.CreateInstance(typeof(RatioViewModel));
                ratioObj.InjectFrom(ratio);
                var ratioViewModel = (RatioViewModel)ratioObj;
                return ratioViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("RatioViewModel entrée: ratio, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire un objet TypeDeviceViewModel à partir d'un objet typeDevice et typeDeviceViewModel
        /// </summary>
        /// <param name="typeDevice">Objet typeDevice</param>
        /// <param name="typeDeviceViewModel">Objet typeDeviceViewModel</param>
        /// <returns>Objet TypeDeviceViewModel</returns>
        private static TypeDeviceViewModel TypeDeviceViewModel(TypeDevice typeDevice, TypeDeviceViewModel typeDeviceViewModel)
        {
            try
            {
                object typeDeviceObj = Activator.CreateInstance(typeof(TypeDeviceViewModel));
                typeDeviceObj.InjectFrom(typeDevice);
                typeDeviceViewModel = (TypeDeviceViewModel)typeDeviceObj;
                typeDeviceViewModel.Id = typeDevice.Id;
                typeDeviceViewModel.URL = typeDevice.URL;

                if (typeDevice.TypeDeviceTraductions != null && typeDevice.TypeDeviceTraductions.ElementAt(0) != null)
                    typeDeviceViewModel.LibelleTypeDevice =
                        typeDevice.TypeDeviceTraductions.ElementAt(0).LibelleTypeDevice;

                return typeDeviceViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("TypeDeviceViewModel entrée: typeDevice, typeDeviceViewModel, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire un objet CategorieDeviceViewModel à partir d'un objet CategorieDevice et CategorieDeviceViewModel
        /// </summary>
        /// <param name="idClient">Identifiant du client</param>
        /// <param name="categorieDevice">Objet categorieDevice</param>
        /// <param name="categorieDeviceViewModel">Objet categorieDeviceViewModel</param>
        /// <returns>Objet CategorieDeviceViewModel</returns>
        private static CategorieDeviceViewModel CategorieDeviceViewModel(int idClient, CategorieDevice categorieDevice, CategorieDeviceViewModel categorieDeviceViewModel)
        {
            try
            {
                object categorieDeviceObj = Activator.CreateInstance(typeof(CategorieDeviceViewModel));
                categorieDeviceObj.InjectFrom(categorieDevice);
                categorieDeviceViewModel = (CategorieDeviceViewModel)categorieDeviceObj;
                categorieDeviceViewModel.Id = categorieDevice.Id;
                categorieDeviceViewModel.ClientId = idClient;
                categorieDeviceViewModel.URL = categorieDevice.URL;

                if (categorieDevice.CategorieDeviceTraductions != null &&
                    categorieDevice.CategorieDeviceTraductions.ElementAt(0) != null)
                    categorieDeviceViewModel.LibelleCategorieDevice =
                        categorieDevice.CategorieDeviceTraductions.ElementAt(0).LibelleCategorieDevice;

                return categorieDeviceViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("CategorieDeviceViewModel entrée: idClient= {0}, categorieDevice, categorieDeviceViewModel, sortie=>erreur : {1}", idClient, e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire un objet FamilleDeviceViewModel à partir d'un objet FamilleDevice et FamilleDeviceViewModel
        /// </summary>
        /// <param name="idClient">Identifiant du client</param>
        /// <param name="familleDevice">Objet familleDevice</param>
        /// <param name="familleDeviceViewModel">Objet familleDeviceViewModel</param>
        /// <returns>Objet CategorieDeviceViewModel</returns>
        private static FamilleDeviceViewModel FamilleDeviceViewModel(int idClient, FamilleDevice familleDevice, FamilleDeviceViewModel familleDeviceViewModel)
        {
            try
            {
                object familleDeviceObj = Activator.CreateInstance(typeof(FamilleDeviceViewModel));
                familleDeviceObj.InjectFrom(familleDevice);
                familleDeviceViewModel = (FamilleDeviceViewModel)familleDeviceObj;
                familleDeviceViewModel.Id = familleDevice.Id;
                familleDeviceViewModel.ClientId = idClient;
                familleDeviceViewModel.URL = familleDevice.URL;
                if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                    familleDeviceViewModel.LibelleFamilleDevice = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                return familleDeviceViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("CategorieDeviceViewModel entrée: idClient= {0}, familleDevice, familleDeviceViewModel, sortie=>erreur : {1}", idClient, e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire un objet ConfigSiDevice à partir d'un objet ConfigSi, TypeDevice et Usage
        /// </summary>
        /// <param name="configSi">Objet ConfigSi</param>
        /// <param name="typeDevice">Objet typeDevice</param>
        /// <param name="usage">Objet usage</param>
        /// <returns>Objet ConfigSiDevice</returns>
        private static ConfigSiDevice ConfigSiDevice(ConfigSi configSi, TypeDevice typeDevice, Usage usage)
        {
            try
            {
                var configSiDevice = new ConfigSiDevice();
                configSiDevice.ConfigSiId = configSi.Id;
                configSiDevice.TypeDeviceId = typeDevice.Id;
                configSiDevice.TypeUsageId = usage.TypeUsageId;

                if (typeDevice.TypeDeviceTraductions != null && typeDevice.TypeDeviceTraductions.ElementAt(0) != null)
                    configSiDevice.LibelleDevice = typeDevice.TypeDeviceTraductions.ElementAt(0).LibelleTypeDevice;
                else
                    configSiDevice.LibelleDevice = typeDevice.LibelleTypeDevice;

                configSiDevice.NbrJourTravaille = usage.NbrJourTravaille;
                configSiDevice.NbrHeuresIntensifJour = usage.NbrHeuresIntensifJour;
                configSiDevice.NbrHeuresOffJour = usage.NbrHeuresOffJour;
                configSiDevice.UtiliseEnergieExterne = usage.UtiliseEnergieExterne;
                configSiDevice.PourcentConsomExtern = usage.PourcentageConsoExterne.HasValue ? usage.PourcentageConsoExterne.Value : 0;
                configSiDevice.CoeffReductionWattExterne = usage.CoeffReductionWattExterne.HasValue ? usage.CoeffReductionWattExterne.Value : 0;
                configSiDevice.NbrHeuresNonIntensifJour = usage.NbrHeuresNonIntensifJour;
                configSiDevice.CoeffNonIntensif = usage.CoeffNonIntensif.HasValue ? usage.CoeffNonIntensif.Value : 0;
                configSiDevice.CoeffModeOff = usage.CoeffModeOff;
                configSiDevice.DateDebut = DateTime.Now;
                configSiDevice.Source = (int)ConfigSiDeviceType.Benchmark;
                return configSiDevice;
            }
            catch (Exception e)
            {
                Logger.Error("ConfigSiDevice entrée: configSi, typeDevice, usage, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire une liste de ConsoWattHeurViewModel à partir d'un TypeDevice
        /// </summary>
        /// <param name="typeDevice">Objet typeDevice</param>
        /// <returns>Liste de ConsoWattHeurViewModel</returns>
        private List<ConsoWattHeurViewModel> ConsoWattHeurViewModels(TypeDevice typeDevice)
        {
            try
            {
                List<ConsoWattHeur> listeConsoWattHeur = _econocomDataManager.GetConsoWattHeur(typeDevice.Id);

                var consoWattHeurViewModels = new List<ConsoWattHeurViewModel>();

                foreach (var consoWattHeur in listeConsoWattHeur)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    object consoWattHeurObj = Activator.CreateInstance(typeof(ConsoWattHeurViewModel));
                    consoWattHeurObj.InjectFrom(consoWattHeur);
                    var consoWattHeurViewModel = (ConsoWattHeurViewModel)consoWattHeurObj;
                    consoWattHeurViewModels.Add(consoWattHeurViewModel);
                }
                return consoWattHeurViewModels;
            }
            catch (Exception e)
            {
                Logger.Error("ConsoWattHeurViewModels entrée: typeDevice, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Construire une liste de VentilationClasseDeviceViewModel à partir d'un TypeDevice
        /// </summary>
        /// <param name="secteurActiviteId">Identifiant d'une secteur d'activité</param>
        /// <param name="typeDevice">Objet typeDevice</param>
        /// <returns>Liste de ConsoWattHeurViewModel</returns>
        private List<VentilationClasseDeviceViewModel> VentilationClasseDeviceViewModels(int secteurActiviteId, TypeDevice typeDevice)
        {
            try
            {
                List<VentilationClasseDevice> listeVentilationClasseDevice = _econocomDataManager.GetVentilationClasseDeviceParTypeDevice(secteurActiviteId, typeDevice.Id);

                var ventilationClasseDeviceViewModels = new List<VentilationClasseDeviceViewModel>();

                foreach (var ventilationClasseDevice in listeVentilationClasseDevice)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    object ventilationClasseDeviceObj = Activator.CreateInstance(typeof(VentilationClasseDeviceViewModel));
                    ventilationClasseDeviceObj.InjectFrom(ventilationClasseDevice);
                    var ventilationClasseDeviceViewModel = (VentilationClasseDeviceViewModel)ventilationClasseDeviceObj;
                    ventilationClasseDeviceViewModels.Add(ventilationClasseDeviceViewModel);
                }
                return ventilationClasseDeviceViewModels;
            }
            catch (Exception e)
            {
                Logger.Error("VentilationClasseDeviceViewModels entrée: secteurActiviteId= {0}, typeDevice, sortie=>erreur : {1}", secteurActiviteId, e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Construire une liste de VentilationClasseAgeDeviceViewModel à partir d'un TypeDevice
        /// </summary>
        /// <param name="secteurActiviteId">Identifiant d'une secteur d'activité</param>
        /// <param name="typeDevice">Objet typeDevice</param>
        /// <returns>Liste de VentilationClasseAgeDeviceViewModel</returns>
        private List<VentilationClasseAgeDeviceViewModel> VentilationClasseAgeDeviceViewModels(int secteurActiviteId, TypeDevice typeDevice)
        {
            try
            {
                List<VentilationClasseAgeDevice> listeVentilationClasseAgeDevice = _econocomDataManager.GetVentilationClasseAgeDeviceParTypeDevice(secteurActiviteId, typeDevice.Id);

                var ventilationClasseAgeDeviceViewModels = new List<VentilationClasseAgeDeviceViewModel>();

                foreach (var ventilationClasseAgeDevice in listeVentilationClasseAgeDevice)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    object ventilationClasseAgeDeviceObj = Activator.CreateInstance(typeof(VentilationClasseAgeDeviceViewModel));
                    ventilationClasseAgeDeviceObj.InjectFrom(ventilationClasseAgeDevice);
                    var ventilationClasseAgeDeviceViewModel = (VentilationClasseAgeDeviceViewModel)ventilationClasseAgeDeviceObj;
                    ventilationClasseAgeDeviceViewModels.Add(ventilationClasseAgeDeviceViewModel);
                }
                return ventilationClasseAgeDeviceViewModels;
            }
            catch (Exception e)
            {
                Logger.Error("VentilationClasseAgeDeviceViewModels entrée: secteurActiviteId= {0}, typeDevice, sortie=>erreur : {1}", secteurActiviteId, e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Récupérer les détails config pour un client existant
        /// </summary>
        /// <param name="client">Objet Client</param>
        /// <param name="typeUtilisateur">TypeUtilisateur : demo / payant / simulation</param>
        /// <param name="idContact">Identifiant du contact</param>
        /// <returns>Objet CalculateurViewModel</returns>
        public CalculateurViewModel GetDetailsConfigPourClientExistant(ConfigSiClient configSiClient, Client client, string typeUtilisateur, int? idContact)
        {
            try
            {
                //Logger.Debug("GetDetailsConfigPourClientExistant : Start ");
                var debut = ConsommationHelper.GetStartTime("GetDetailsConfigPourClientExistant");
                if (client != null)
                {
                    Logger.Debug("IdClient = " + client.Id);
                }
                else
                {
                    Logger.Debug("Client nulle ");
                }
                Logger.Debug("typeUtilisateur = " + typeUtilisateur);
                Logger.Debug("idContact = " + typeUtilisateur);

                var calculateurViewModel = new CalculateurViewModel();
                var idClient = client.Id;

                int secteurActiviteId = client.SecteurActiviteId.HasValue ? client.SecteurActiviteId.Value : 1;
                Logger.Debug("secteurActiviteId = " + secteurActiviteId);

                //var configSiClient = new ConfigSiClient();
                var configSi = new ConfigSi();

                if (typeUtilisateur.Equals("Payant"))
                {
                    //Get ConfigSiClient
                    // configSiClient =  _econocomDataManager.GetConfigSiClient(idClient);
                    if (configSiClient == null)
                    {
                        configSiClient = _econocomDataManager.GetConfigSiClient(idClient);
                        Logger.Debug("configSiClient nulle ");
                    }
                    calculateurViewModel.ConfigSiClient = configSiClient;

                    //Get ConfigSi
                    configSi = configSiClient.ConfigSi;
                    if (configSi == null)
                    {
                        Logger.Debug("configSi nulle ");
                    }
                    calculateurViewModel.ConfigSi = configSi;
                }
                else
                {
                    //get simulation en cours pour client
                    var simulation = _econocomDataManager.GetSimulationEnCours(idContact.Value);
                    if (simulation == null)
                    {
                        Logger.Debug("Pas de simulation en cours");
                    }
                    else //get configSi
                        configSi = _econocomDataManager.GetConfigSi(simulation.ConfigSiSimuleId.Value);

                    if (configSi == null)
                    {
                        Logger.Debug("configSi nulle ");
                    }
                    calculateurViewModel.ConfigSi = configSi;
                }

                //Get Resultat
                var listeResultats = _econocomDataManager.GetResultats(configSi.Id);
                if (listeResultats == null)
                {
                    Logger.Debug("listeResultats nulle ");
                }

                //Obtenir liste FamilleDevice par SecteurActivite du client payant
                var listefamilleDevice = _econocomDataManager.GetFamilleDeviceParSecteurActivite(client.SecteurActivite.Id);
                var listeFamilleDeviceViewModel = new List<FamilleDeviceViewModel>();
                var debutlistefamilleDevice = ConsommationHelper.GetStartTime("GetDetailsConfigPourClientExistant listefamilleDevice: Start");
                var isDeviceModified = false;
                //var isCategorieModified = false;
                // var isFamilleModified = false;
                foreach (FamilleDevice familleDevice in listefamilleDevice)
                {
                    //isFamilleModified = false;
                    //Conversion familleDevice -> familleDeviceViewModel
                    var familleDeviceViewModel = new FamilleDeviceViewModel();
                    familleDeviceViewModel = FamilleDeviceViewModel(idClient, familleDevice, familleDeviceViewModel);

                    //Obtenir liste CategorieDevice pour chaque famille
                    List<CategorieDevice> listeCategorieDevice = _econocomDataManager.GetCategorieDeviceParFamille(familleDevice.Id);

                    var listeCategorieDeviceViewModel = new List<CategorieDeviceViewModel>();

                    foreach (CategorieDevice categorieDevice in listeCategorieDevice)
                    {
                        //isCategorieModified = false;
                        //Conversion categorieDevice -> categorieDeviceViewModel
                        var categorieDeviceViewModel = new CategorieDeviceViewModel();
                        categorieDeviceViewModel = CategorieDeviceViewModel(idClient, categorieDevice, categorieDeviceViewModel);

                        //Construire liste TypeDevice d'apres configSiDevice liste
                        var listeTypeDeviceViewModel = new List<TypeDeviceViewModel>();

                        if (listeResultats.Any())
                        {
                            foreach (var resultat in listeResultats)
                            {
                                var configSiDevice = resultat.ConfigSiDevice;
                                //Logger.Debug("IdConfigSiDevice = " + configSiDevice.Id);

                                if (configSiDevice.TypeDevice.CategorieDeviceId == categorieDevice.Id)
                                {
                                    //Conversion typeDevice -> typeDeviceViewModel
                                    var typeDeviceViewModel = new TypeDeviceViewModel();
                                    typeDeviceViewModel = TypeDeviceViewModel(configSiDevice, typeDeviceViewModel);

                                    // Obtenir ratio pour chaque TypeDevice
                                    Ratio ratio = _econocomDataManager.GetRatio(secteurActiviteId, configSiDevice.TypeDeviceId);
                                    var ratioViewModel = RatioViewModel(ratio);

                                    // Obtenir typeRatio
                                    TypeRatio typeRatio = ratio.TypeRatio;
                                    var typeRatioViewModel = TypeRatioViewModel(typeRatio);
                                    ratioViewModel.TypeRatioViewModel = typeRatioViewModel;

                                    // Obtenir Ventilation Classe Device pour chaque TypeDevice
                                    var ventilationClasseDeviceViewModels = VentilationClasseDeviceViewModels(secteurActiviteId, configSiDevice);

                                    // Obtenir Ventilation Classe Age Device pour chaque TypeDevice
                                    var ventilationClasseAgeDeviceViewModels = VentilationClasseAgeDeviceViewModels(secteurActiviteId, configSiDevice);

                                    // Obtenir Usage pour chaque TypeDevice
                                    var usageViewModel = DetaillesUsageViewModel(configSiDevice);

                                    // Obtenir typeUsage
                                    Usage usage = _econocomDataManager.GetUsageParTypeDevice(configSiDevice.TypeDeviceId);
                                    TypeUsage typeUsage = usage.TypeUsage;
                                    var typeUsageViewModel = TypeUsageViewModel(typeUsage);
                                    usageViewModel.TypeUsage = typeUsageViewModel;                                   
                                    // Obtenir ConsoWattHeur pour chaque TypeDevice
                                    var consoWattHeurViewModels = ConsoWattHeurViewModels(configSiDevice);

                                    typeDeviceViewModel.Ratio = ratioViewModel;
                                    typeDeviceViewModel.VentilationClasseDevices = ventilationClasseDeviceViewModels;
                                    typeDeviceViewModel.VentilationClasseAgeDevices = ventilationClasseAgeDeviceViewModels;
                                    typeDeviceViewModel.Usage = usageViewModel;
                                    typeDeviceViewModel.ConsoWattHeurs = consoWattHeurViewModels;
                                    
                                    //ConfigSiDevice
                                    typeDeviceViewModel.ConfigSiDevice = configSiDevice;
                                    foreach (var configSiConsommation in typeDeviceViewModel.ConfigSiDevice.ConfigSiConsommations)
                                    {
                                        if (configSiConsommation.ConsommationWatt.HasValue)
                                            typeDeviceViewModel.ConsoWatt += configSiConsommation.ConsommationWatt.Value;
                                        if (configSiConsommation.ConsommationCo2.HasValue)
                                            typeDeviceViewModel.ConsoCo2 += configSiConsommation.ConsommationCo2.Value;
                                    }
                                    isDeviceModified = configSiDevice.ConfigSiConsommations.Any(c => c.IsModified == true);
                                    if (isDeviceModified)
                                    {
                                        typeDeviceViewModel.IsModified = true;
                                        categorieDeviceViewModel.IsModified = true;
                                        familleDeviceViewModel.IsModified = true;
                                    }

                                    listeTypeDeviceViewModel.Add(typeDeviceViewModel);
                                }
                            }
                        }
                        foreach (var typeDevice in listeTypeDeviceViewModel)
                        {
                            categorieDeviceViewModel.ConsoWatt += typeDevice.ConsoWatt;
                            categorieDeviceViewModel.ConsoCo2 += typeDevice.ConsoCo2;
                        }
                        categorieDeviceViewModel.TypeDevices = listeTypeDeviceViewModel;
                        listeCategorieDeviceViewModel.Add(categorieDeviceViewModel);
                    }

                    foreach (var categorieDevice in listeCategorieDeviceViewModel)
                    {
                        familleDeviceViewModel.ConsoWatt += categorieDevice.ConsoWatt;
                        familleDeviceViewModel.ConsoCo2 += categorieDevice.ConsoCo2;
                    }
                    familleDeviceViewModel.CategorieDevices = listeCategorieDeviceViewModel;
                    listeFamilleDeviceViewModel.Add(familleDeviceViewModel);
                }
                ConsommationHelper.GetTimeDifference(debutlistefamilleDevice, "GetDetailsConfigPourClientExistant listefamilleDevice: end");
                calculateurViewModel.FamilleDeviceViewModels = listeFamilleDeviceViewModel;

                calculateurViewModel.Effectif = client.DetailsClient.Effectif;
                calculateurViewModel.PrixKWatt = client.DetailsClient.PrixKWH;
                //calculateurViewModel.CoefficientConversion = client.Adresse.Pays.CoefficientConversion;
                calculateurViewModel.CoefficientConversion = (client.DetailsClient != null && client.DetailsClient.CoefficientCO2 != 0) ? client.DetailsClient.CoefficientCO2 : client.Adresse.Pays.CoefficientConversion; 
                calculateurViewModel.TauxEquipement = client.DetailsClient.TauxEquipement;
                calculateurViewModel.SuperficieDataCenter = client.DetailsClient.SuperficieDataCenter;
                calculateurViewModel.NombreSite = client.DetailsClient.NombreSite;

                //Logger.Debug("GetDetailsConfigPourClientExistant : End ");
                ConsommationHelper.GetTimeDifference(debut, "GetDetailsConfigPourClientExistant : End");
                return calculateurViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("GetDetailsConfigPourClientExistant entrée: client, typeUtilisateur= {0}, idContact= {1}, sortie=>erreur : {1}", typeUtilisateur, idContact, e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Mettre à jour Resultat
        /// </summary>
        /// <param name="calculateurViewModel">Objet calculateurViewModel</param>
        /// <returns>vrai / faux</returns>
        public bool MAJResultat(CalculateurViewModel calculateurViewModel)
        {
            try
            {
                Logger.Debug("MAJResultat : Start ");
                foreach (var familleDevice in calculateurViewModel.FamilleDeviceViewModels)
                {
                    if (familleDevice.IsModified)
                    {
                        foreach (var categorieDevice in familleDevice.CategorieDevices)
                        {
                            if (categorieDevice.IsModified)
                            {
                                foreach (var typeDevice in categorieDevice.TypeDevices)
                                {
                                    if (typeDevice.IsModified)
                                    {
                                        decimal? consommationWatt = 0;
                                        decimal? consommationCO2 = 0;

                                        foreach (
                                            var configSiConsommation in typeDevice.ConfigSiDevice.ConfigSiConsommations
                                            )
                                        {
                                            consommationWatt = configSiConsommation.ConsommationWatt + consommationWatt;
                                            consommationCO2 = configSiConsommation.ConsommationCo2 + consommationCO2;
                                        }

                                        var resultat =
                                            _econocomDataManager.GetResultatParConfigSiDevice(
                                                typeDevice.ConfigSiDevice.Id);
                                        resultat.ConsommationWatt = consommationWatt;
                                        Logger.Debug("ConsommationWatt = " + consommationWatt);

                                        resultat.ConsommationCo2 = consommationCO2;
                                        Logger.Debug("ConsommationCo2 = " + consommationCO2);

                                        resultat = _econocomDataManager.MAJResultat(resultat);

                                        typeDevice.IsModified = false;
                                        Logger.Debug("IdResulatat MAJ = " + resultat.Id);
                                    }
                                }
                                categorieDevice.IsModified = false;
                            }
                        }
                        familleDevice.IsModified = false;
                    }
                }
                Logger.Debug("MAJResultat : End");
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("MAJResultat entrée: calculateurViewModel, sortie=>erreur : {0}", e.StackTrace);
                return false;
            }
        }

        public bool MAJResultat(ConfigSi configSi, decimal coefficientCo2)
        {
            try
            {
                Logger.Debug("MAJResultat : Start ");
                if (configSi != null)
                {
                    var result = _econocomDataManager.MAJConfigSiResultat(configSi, coefficientCo2);
                    return result == 1;
                }
                Logger.Debug("MAJResultat : End");
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("MAJResultat entrée: calculateurViewModel, sortie=>erreur : {0}", e.StackTrace);
                return false;
            }
        }

        public bool MAJConsommation(ConfigSi configSi, decimal coefficientCo2)
        {
            try
            {
                Logger.Debug("MAJConsommation : Start ");
                if (configSi != null)
                {
                    var result = _econocomDataManager.MAJConfigSiConsommation(configSi, coefficientCo2);
                    return result == 1;
                }
                Logger.Debug("MAJConsommation : End");
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("MAJConsommation, sortie=>erreur : {0}", e.StackTrace);
                return false;
            }
        }

        public bool MAJConfigClient(int idClient, decimal coefficientCo2)
        {
            try
            {
                var configSiClientActuel = _econocomDataManager.GetConfigSiClient(idClient);
                if (configSiClientActuel != null)
                {
                    var resultat = MAJResultat(configSiClientActuel.ConfigSi, coefficientCo2);
                    resultat = MAJConsommation(configSiClientActuel.ConfigSi, coefficientCo2);
                    return resultat;
                }
            }
            catch (Exception e)
            {
                Logger.Error("MAJResultat entrée: calculateurViewModel, sortie=>erreur : {0}", e.StackTrace);
            }
            return false;
        }

        public bool MAJSimulationClient(int idContact, decimal coefficientCo2)
        {
            try
            {
                var simulation = GetSimulationEnCours(idContact);
                if (simulation != null && simulation.ConfigSiSimuleId.HasValue)
                {
                    var configSi = _econocomDataManager.GetConfigSiDetails(simulation.ConfigSiSimuleId.Value);
                    if (configSi != null)
                    {
                        var resultat = MAJResultat(configSi, coefficientCo2);
                        resultat = MAJConsommation(configSi, coefficientCo2);
                    }
                }

                if (simulation != null && simulation.ConfigSiSourceId.HasValue)
                {
                    var configSi = _econocomDataManager.GetConfigSiDetails(simulation.ConfigSiSourceId.Value);
                    if (configSi != null)
                    {
                        var resultat = MAJResultat(configSi, coefficientCo2);
                        resultat = MAJConsommation(configSi, coefficientCo2);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("MAJResultat entrée: calculateurViewModel, sortie=>erreur : {0}", e.StackTrace);
            }
            return false;
        }
        /// <summary>
        /// Re-Calculer les différentes quantités, consommations et mettre à jour ConfigSiConsommation
        /// </summary>
        /// <param name="calculateurViewModel">Objet CalculateurViewModel</param>
        /// <returns>Objet CalculateurViewModel</returns>
        public CalculateurViewModel ReCalculerEtMAJConfigSiConsommation(CalculateurViewModel calculateurViewModel)
        {
            var debut = ConsommationHelper.GetStartTime("ReCalculerEtMAJConfigSiConsommation start");
            try
            {
                //Logger.Error("ReCalculerEtMAJConfigSiConsommation : Start ");                
                var calculator = new Calculator();
                var debut1 = ConsommationHelper.GetStartTime("CalculerTotalConsommation start:");
                calculateurViewModel = calculator.CalculerTotalConsommation(calculateurViewModel);
                ConsommationHelper.GetTimeDifference(debut1, "CalculerTotalConsommation end:");

                //MAJ configSiConsommations 
                var debut2 = ConsommationHelper.GetStartTime("ReCalculerEtMAJConfigSiConsommation FamilleDeviceViewModels start:");
                foreach (var familleDevice in calculateurViewModel.FamilleDeviceViewModels)
                {
                    foreach (var categorieDevice in familleDevice.CategorieDevices)
                    {
                        foreach (var typeDevice in categorieDevice.TypeDevices)
                        {
                            foreach (var configSiConsommation in typeDevice.ConfigSiDevice.ConfigSiConsommations)
                            {
                                try
                                {
                                    if (configSiConsommation.IsModified)
                                    {
                                        configSiConsommation.IsModified = false;
                                        var updated = _econocomDataManager.MAJConfigSiConsommation(configSiConsommation);
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogguerErreur(e);
                                }

                            }
                        }
                    }
                }
                ConsommationHelper.GetTimeDifference(debut2, "ReCalculerEtMAJConfigSiConsommation FamilleDeviceViewModels end:");
            }
            catch (Exception e)
            {
                Logger.Error("ReCalculerEtMAJConfigSiConsommation entrée: calculateurViewModel, sortie=>erreur : {0}", e.StackTrace);
                return null;
            }
            var fin = ConsommationHelper.GetTimeDifference(debut, "ReCalculerEtMAJConfigSiConsommation : End");
            //Logger.Debug("ReCalculerEtMAJConfigSiConsommation : End ");
            return calculateurViewModel;
        }

        /// <summary>
        /// Calculer total consommation
        /// </summary>
        /// <param name="calculateurViewModel">Objet CalculateurViewModel</param>
        /// <returns>Objet CalculateurViewModel</returns>
        public CalculateurViewModel CalculerTotalConsommation(CalculateurViewModel calculateurViewModel)
        {
            var debut = ConsommationHelper.GetStartTime("BusinessService, CalculerTotalConsommation start:");
            try
            {
                //Logger.Error("CalculerTotalConsommation : Start ");

                var familleDeviceListe = new List<FamilleDeviceViewModel>();
                decimal? consommationWatt = 0;
                decimal? consommationCo2 = 0;

                //Calculer total consommation pour chaque deviceFamille
                foreach (var deviceFamille in calculateurViewModel.FamilleDeviceViewModels)
                {
                    var familleDeviceViewModel = CalculerTotalConsommationParFamille(deviceFamille);
                    familleDeviceListe.Add(familleDeviceViewModel);
                }

                //Calculer total consommation watt & co2 pour toutes les deviceFamilles
                foreach (var famille in familleDeviceListe)
                {
                    consommationWatt = consommationWatt + famille.ConsoWatt;
                    consommationCo2 = consommationCo2 + famille.ConsoCo2;
                }

                calculateurViewModel.TotalConsoWatt = consommationWatt;
                Logger.Debug("TotalConsoWatt = " + consommationWatt);

                calculateurViewModel.TotalConsoCo2 = consommationCo2;
                Logger.Debug("TotalConsoCo2 = " + consommationWatt);

                var calculator = new Calculator();
                foreach (var deviceFamille in calculateurViewModel.FamilleDeviceViewModels)
                {
                    deviceFamille.Pourcentage = calculator.CalculerPourcentageConsoAnnuelle(deviceFamille.ConsoWatt ?? 0, calculateurViewModel.TotalConsoWatt ?? 0);
                    deviceFamille.Prix = deviceFamille.ConsoWatt.Value * calculateurViewModel.PrixKWatt;
                }

                //calculer prix
                calculateurViewModel = CalculerPrix(calculateurViewModel);

                //Logger.Error("CalculerTotalConsommation : End ");
                ConsommationHelper.GetTimeDifference(debut, "BusinessService, CalculerTotalConsommation");
                return calculateurViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("CalculerTotalConsommation entrée: calculateurViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }
        }


        /// <summary>
        /// Calculer cout kwt
        /// </summary>
        /// <param name="calculateurViewModel"></param>
        /// <returns></returns>
        public CalculateurViewModel CalculerPrix(CalculateurViewModel calculateurViewModel)
        {
            try
            {
                Logger.Debug("CalculerPrix : Start ");

                calculateurViewModel.TotalPrix = CalulationPrix(calculateurViewModel.PrixKWatt, calculateurViewModel.TotalConsoWatt);

                foreach (var famille in calculateurViewModel.FamilleDeviceViewModels)
                {
                    famille.Prix = CalulationPrix(calculateurViewModel.PrixKWatt, famille.ConsoWatt);

                    foreach (var categorie in famille.CategorieDevices)
                    {
                        categorie.Prix = CalulationPrix(calculateurViewModel.PrixKWatt, categorie.ConsoWatt);

                        foreach (var device in categorie.TypeDevices)
                        {
                            device.Prix = CalulationPrix(calculateurViewModel.PrixKWatt, device.ConsoWatt) ?? 0;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("CalculerPrix entrée: calculateurViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }

            Logger.Debug("CalculerPrix : End ");
            return calculateurViewModel;
        }


        /// <summary>
        /// Calculation cout total 
        /// </summary>
        /// <param name="prixKwatt">Prix par kWt</param>
        /// <param name="consoWatt">Valeur en kWt</param>
        /// <returns>Prix total</returns>
        public decimal? CalulationPrix(decimal prixKwatt, decimal? consoWatt)
        {
            try
            {
                var prix = prixKwatt * consoWatt;
                return prix;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Calculer total consommation par famille
        /// </summary>
        /// <param name="familleDeviceViewModel">Objet familleDeviceViewModel</param>
        /// <returns>Objet FamilleDeviceViewModel</returns>
        public FamilleDeviceViewModel CalculerTotalConsommationParFamille(FamilleDeviceViewModel familleDeviceViewModel)
        {
            try
            {
                Logger.Debug("CalculerTotalConsommationParFamille : Start ");

                var categorieDeviceListe = new List<CategorieDeviceViewModel>();
                decimal? consommationWatt = 0;
                decimal? consommationCo2 = 0;

                //Calculer total consommation pour chaque categorieDevice
                foreach (var categorieDevice in familleDeviceViewModel.CategorieDevices)
                {
                    var categorieDeviceViewModel = CalculerTotalConsommationParCategorie(categorieDevice);
                    categorieDeviceListe.Add(categorieDeviceViewModel);
                }

                //Calculer total consommation watt & co2 pour toutes les categorieDevices
                foreach (var categorie in categorieDeviceListe)
                {
                    consommationWatt = consommationWatt + categorie.ConsoWatt;
                    consommationCo2 = consommationCo2 + categorie.ConsoCo2;
                }

                familleDeviceViewModel.CategorieDevices = categorieDeviceListe;
                familleDeviceViewModel.ConsoWatt = consommationWatt;
                Logger.Debug("ConsoWatt = " + consommationWatt);

                familleDeviceViewModel.ConsoCo2 = consommationCo2;
                Logger.Debug("ConsoCo2 = " + consommationCo2);

                var calculator = new Calculator();
                foreach (var categorieDevice in familleDeviceViewModel.CategorieDevices)
                {
                    categorieDevice.Pourcentage = calculator.CalculerPourcentageConsoAnnuelle(categorieDevice.ConsoWatt ?? 0, familleDeviceViewModel.ConsoWatt ?? 0);
                }
            }
            catch (Exception e)
            {
                Logger.Error("CalculerTotalConsommationParFamille entrée: familleDeviceViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }

            Logger.Debug("CalculerTotalConsommationParFamille : End ");
            return familleDeviceViewModel;
        }

        /// <summary>
        /// Calculer total consommation par categorie
        /// </summary>
        /// <param name="categorieDeviceViewModel">Objet categorieDeviceViewModel</param>
        /// <returns>Objet CategorieDeviceViewModel</returns>
        public CategorieDeviceViewModel CalculerTotalConsommationParCategorie(CategorieDeviceViewModel categorieDeviceViewModel)
        {
            try
            {
                Logger.Debug("CalculerTotalConsommationParCategorie : Start ");

                var typeDeviceListe = new List<TypeDeviceViewModel>();
                decimal? consommationWatt = 0;
                decimal? consommationCo2 = 0;

                //Calculer total consommation pour chaque typeDevice
                foreach (var typeDevice in categorieDeviceViewModel.TypeDevices)
                {
                    var typeDeviceViewModel = CalculerTotalConsommationParTypeDevice(typeDevice);
                    typeDeviceListe.Add(typeDeviceViewModel);
                }

                //Calculer total consommation watt & co2 pour toutes les typeDevices
                foreach (var device in typeDeviceListe)
                {
                    consommationWatt = consommationWatt + device.ConsoWatt;
                    consommationCo2 = consommationCo2 + device.ConsoCo2;
                }

                categorieDeviceViewModel.TypeDevices = typeDeviceListe;
                categorieDeviceViewModel.ConsoWatt = consommationWatt;
                Logger.Debug("ConsoWatt = " + consommationWatt);

                categorieDeviceViewModel.ConsoCo2 = consommationCo2;
                Logger.Debug("ConsoCo2 = " + consommationCo2);
            }
            catch (Exception e)
            {
                Logger.Error("CalculerTotalConsommationParCategorie entrée: categorieDeviceViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }

            Logger.Debug("CalculerTotalConsommationParCategorie : End ");
            return categorieDeviceViewModel;
        }

        /// <summary>
        /// Calculer total consommation par type device
        /// </summary>
        /// <param name="typeDeviceViewModel">Objet typeDeviceViewModel</param>
        /// <returns>Objet TypeDeviceViewModel</returns>
        public TypeDeviceViewModel CalculerTotalConsommationParTypeDevice(TypeDeviceViewModel typeDeviceViewModel)
        {
            try
            {
                Logger.Debug("CalculerTotalConsommationParTypeDevice : Start ");

                decimal? consommationWatt = 0;
                decimal? consommationCo2 = 0;

                //Get resultat par configSiDeviceId
                var resultat = _econocomDataManager.GetResultatParConfigSiDevice(typeDeviceViewModel.ConfigSiDevice.Id);
                consommationWatt = consommationWatt + (resultat.ConsommationWatt.HasValue ? resultat.ConsommationWatt.Value : 0);
                consommationCo2 = consommationCo2 + (resultat.ConsommationCo2.HasValue ? resultat.ConsommationCo2.Value : 0);
                typeDeviceViewModel.ConsoWatt = consommationWatt.Value;
                typeDeviceViewModel.ConsoCo2 = consommationCo2.Value;
            }
            catch (Exception e)
            {
                Logger.Error("CalculerTotalConsommationParTypeDevice entrée: typeDeviceViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw e;
            }

            Logger.Debug("CalculerTotalConsommationParTypeDevice : End ");
            return typeDeviceViewModel;
        }

        /// <summary>
        /// Construire un objet TypeDeviceViewModel à partir d'un ConfigSiDevice et TypeDeviceViewModel
        /// </summary>
        /// <param name="configSiDevice">Objet configSiDevice</param>
        /// <param name="typeDeviceViewModel">Objet typeDeviceViewModel</param>
        /// <returns>Objet TypeDeviceViewModel</returns>
        private static TypeDeviceViewModel TypeDeviceViewModel(ConfigSiDevice configSiDevice, TypeDeviceViewModel typeDeviceViewModel)
        {
            try
            {
                object typeDeviceObj = Activator.CreateInstance(typeof(TypeDeviceViewModel));
                typeDeviceObj.InjectFrom(configSiDevice);
                typeDeviceViewModel = (TypeDeviceViewModel)typeDeviceObj;
                typeDeviceViewModel.Id = configSiDevice.TypeDeviceId;
                typeDeviceViewModel.LibelleTypeDevice = configSiDevice.LibelleDevice;
                typeDeviceViewModel.URL = configSiDevice.TypeDevice.URL;

                if (configSiDevice.Source == (int)ConfigSiDeviceType.Benchmark)
                {
                    if (configSiDevice.TypeDevice.TypeDeviceTraductions != null &&
                        configSiDevice.TypeDevice.TypeDeviceTraductions.ElementAt(0) != null)
                    {
                        typeDeviceViewModel.LibelleTypeDevice =
                            configSiDevice.TypeDevice.TypeDeviceTraductions.ElementAt(0).LibelleTypeDevice;                        
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("TypeDeviceViewModel entrée: configSiDevice, typeDeviceViewModel, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return typeDeviceViewModel;
        }

        /// <summary>
        /// Construire une liste de ConsoWattHeurViewModel à partir d'un ConfigSiDevice
        /// </summary>
        /// <param name="configSiDevice">Objet configSiDevice</param>
        /// <returns>Liste de ConsoWattHeurViewModel</returns>
        private List<ConsoWattHeurViewModel> ConsoWattHeurViewModels(ConfigSiDevice configSiDevice)
        {
            var consoWattHeurViewModels = new List<ConsoWattHeurViewModel>();
            try
            {
                List<ConsoWattHeur> listeConsoWattHeur = _econocomDataManager.GetConsoWattHeur(configSiDevice.TypeDeviceId);

                foreach (var consoWattHeur in listeConsoWattHeur)
                {
                    object consoWattHeurObj = Activator.CreateInstance(typeof(ConsoWattHeurViewModel));
                    consoWattHeurObj.InjectFrom(consoWattHeur);
                    var consoWattHeurViewModel = (ConsoWattHeurViewModel)consoWattHeurObj;
                    consoWattHeurViewModels.Add(consoWattHeurViewModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ConsoWattHeurViewModels entrée: configSiDevice, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return consoWattHeurViewModels;
        }

        /// <summary>
        /// Construire un objet DetaillesUsageViewModel à partir d'un ConfigSiDevice
        /// </summary>
        /// <param name="configSiDevice">Objet configSiDevice</param>
        /// <returns>Objet DetaillesUsageViewModel</returns>
        private static DetaillesUsageViewModel DetaillesUsageViewModel(ConfigSiDevice configSiDevice)
        {
            try
            {
                object usageObj = Activator.CreateInstance(typeof(DetaillesUsageViewModel));
                usageObj.InjectFrom(configSiDevice);
                var usageViewModel = (DetaillesUsageViewModel)usageObj;
                usageViewModel.PourcentageConsoExterne = configSiDevice.PourcentConsomExtern;
                return usageViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("DetaillesUsageViewModel entrée: configSiDevice, sortie=>erreur : {0}", e.StackTrace);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Construire une liste de VentilationClasseAgeDeviceViewModel à partir d'un ConfigSiDevice
        /// </summary>
        /// <param name="secteurActiviteId">Identifiant de secteur activité</param>
        /// <param name="configSiDevice">Objet configSiDevice</param>
        /// <returns>Liste VentilationClasseAgeDeviceViewModel</returns>
        private List<VentilationClasseAgeDeviceViewModel> VentilationClasseAgeDeviceViewModels(int secteurActiviteId, ConfigSiDevice configSiDevice)
        {
            var ventilationClasseAgeDeviceViewModels = new List<VentilationClasseAgeDeviceViewModel>();

            try
            {
                List<VentilationClasseAgeDevice> listeVentilationClasseAgeDevice = _econocomDataManager.GetVentilationClasseAgeDeviceParTypeDevice(secteurActiviteId, configSiDevice.TypeDeviceId);

                foreach (var ventilationClasseAgeDevice in listeVentilationClasseAgeDevice)
                {
                    object ventilationClasseAgeDeviceObj = Activator.CreateInstance(typeof(VentilationClasseAgeDeviceViewModel));
                    ventilationClasseAgeDeviceObj.InjectFrom(ventilationClasseAgeDevice);
                    var ventilationClasseAgeDeviceViewModel = (VentilationClasseAgeDeviceViewModel)ventilationClasseAgeDeviceObj;
                    ventilationClasseAgeDeviceViewModels.Add(ventilationClasseAgeDeviceViewModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error("VentilationClasseAgeDeviceViewModels entrée: secteurActiviteId= {0}, configSiDevice, sortie=>erreur : {1}", secteurActiviteId, e.StackTrace);
                throw;
            }
            return ventilationClasseAgeDeviceViewModels;
        }

        /// <summary>
        /// Construire une liste de VentilationClasseDeviceViewModel à partir d'un ConfigSiDevice
        /// </summary>
        /// <param name="secteurActiviteId">Identifiant de secteur activité</param>
        /// <param name="configSiDevice">Objet configSiDevice</param>
        /// <returns>Liste VentilationClasseDeviceViewModel</returns>
        private List<VentilationClasseDeviceViewModel> VentilationClasseDeviceViewModels(int secteurActiviteId, ConfigSiDevice configSiDevice)
        {
            var ventilationClasseDeviceViewModels = new List<VentilationClasseDeviceViewModel>();
            try
            {
                List<VentilationClasseDevice> listeVentilationClasseDevice = _econocomDataManager.GetVentilationClasseDeviceParTypeDevice(secteurActiviteId, configSiDevice.TypeDeviceId);

                foreach (var ventilationClasseDevice in listeVentilationClasseDevice)
                {
                    object ventilationClasseDeviceObj = Activator.CreateInstance(typeof(VentilationClasseDeviceViewModel));
                    ventilationClasseDeviceObj.InjectFrom(ventilationClasseDevice);
                    var ventilationClasseDeviceViewModel = (VentilationClasseDeviceViewModel)ventilationClasseDeviceObj;
                    ventilationClasseDeviceViewModels.Add(ventilationClasseDeviceViewModel);
                }
            }
            catch (Exception e)
            {
                Logger.Error("VentilationClasseDeviceViewModels entrée: secteurActiviteId= {0}, configSiDevice, sortie=>erreur : {1}", secteurActiviteId, e.StackTrace);
                throw;
            }
            return ventilationClasseDeviceViewModels;
        }

        /// <summary>
        /// Construire modèle pour affichage au niveau famille
        /// </summary>
        /// <param name="calculateurViewModel">Objet calculateurViewModel</param>
        /// <param name="idClient">Identifiant client</param>
        /// <param name="typeUtilisateur">typeUtilisateur : demo / payant / simulation</param>
        /// <returns>Objet ConsommationViewModel</returns>
        public ConsommationViewModel GetDetailsFamille(CalculateurViewModel calculateurViewModel, int idClient, string typeUtilisateur)
        {
            try
            {
                var consommationViewModel = new ConsommationViewModel();
                var consommationHelper = new ConsommationHelper();
                var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                //consommationViewModel.DateModifier = calculateurViewModel.ConfigSi.DateModification == null ? calculateurViewModel.ConfigSi.DateDebut.ToString("dd/MM/yyyy") : calculateurViewModel.ConfigSi.DateModification.Value.ToString("dd/MM/yyyy");
                consommationViewModel.DateModifier = DateTime.Now.ToString("dd.MM.yyyy");
                consommationViewModel.TotalConsoWatt = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(calculateurViewModel.TotalConsoWatt.Value)), "kWh");
                consommationViewModel.TotalConsoCo2 = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(calculateurViewModel.TotalConsoCo2.Value)), "teq-CO2");
                consommationViewModel.TotalPrix = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(calculateurViewModel.TotalPrix.Value)), "");
               
                var detailConsommations = new List<DetailConsommationViewModel>();

                foreach (var famille in calculateurViewModel.FamilleDeviceViewModels)
                {
                    var detailConsommation = new DetailConsommationViewModel();

                    detailConsommation.ConsoCo2 = new Consommation(null, Math.Round(famille.ConsoCo2.Value).ToString(), "teq-CO2");
                    detailConsommation.ConsoWatt = new Consommation(null, Math.Round(famille.ConsoWatt.Value).ToString(), "kWh");
                    detailConsommation.Prix = new Consommation(null, Math.Round(famille.Prix.Value).ToString(), "");
                    detailConsommation.IdFamille = famille.Id.ToString();
                    detailConsommation.IdClient = idClient.ToString();
                    detailConsommation.Libeller = famille.LibelleFamilleDevice;
                    detailConsommation.URL = famille.URL;

                    if (typeUtilisateur == "Demo")
                    {
                        detailConsommation.LienImage = "~/Content/images/Consommation/Demo/Famille/" + detailConsommation.IdFamille + ".png";
                    }
                    else if (typeUtilisateur == "Payant")
                    {
                        detailConsommation.LienImage = "~/Content/images/Consommation/Payant/Famille/" + detailConsommation.IdFamille + ".png";
                    }
                    else if (typeUtilisateur == "Simulation")
                    {
                        detailConsommation.LienImage = "~/Content/images/Consommation/Simulation/Famille/" + detailConsommation.IdFamille + ".png";
                    }

                    //detailConsommation.LongueurGrapheWattTotale = consommationHelper.CalculerLongeurGrapheTotal(Convert.ToDecimal(famille.ConsoWatt));
                    //detailConsommation.LongueurGrapheWattMilieu = consommationHelper.CalculerLongeurGrapheMilieu(Convert.ToDecimal(famille.ConsoWatt));
                    //detailConsommation.LongueurGrapheCO2Totale = consommationHelper.CalculerLongeurGrapheTotal(Convert.ToDecimal(famille.ConsoCo2));
                    //detailConsommation.LongueurGrapheCO2Milieu = consommationHelper.CalculerLongeurGrapheMilieu(Convert.ToDecimal(famille.ConsoCo2));
                    detailConsommation.Pourcentage = new Consommation(null, (Math.Round(famille.Pourcentage)).ToString(), "%");

                    var consommationViewModel1 = GetDetailsCategorieParFamille(idClient, famille, typeUtilisateur);
                    consommationViewModel1.DateModifier = consommationViewModel.DateModifier;
                    detailConsommation.SubObjet = consommationViewModel1;

                    detailConsommations.Add(detailConsommation);
                }
                detailConsommations = consommationHelper.CalculerLongeurGraphe(detailConsommations);
                consommationViewModel.DetailConsommations = detailConsommations;
                return consommationViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("GetDetailsFamille entrée: calculateurViewModel, idClient= {0}, typeUtilisateur= {1}, sortie=>erreur : {1}", idClient, typeUtilisateur, e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Construire modèle pour affichage au niveau catégorie
        /// </summary>
        /// <param name="idClient">Identifiant du client</param>
        /// <param name="familleDevice">Objet FamilleDeviceViewModel</param>
        /// <param name="typeUtilisateur">typeUtilisateur : demo / payant/ simulation</param>
        /// <returns>Objet ConsommationViewModel</returns>
        public ConsommationViewModel GetDetailsCategorieParFamille(int idClient, FamilleDeviceViewModel familleDevice, string typeUtilisateur)
        {
            try
            {
                var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                var consommationViewModel = new ConsommationViewModel();
                var consommationHelper = new ConsommationHelper();

                consommationViewModel.TotalConsoWatt = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(familleDevice.ConsoWatt.Value)), "kWh");
                consommationViewModel.TotalConsoCo2 = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(familleDevice.ConsoCo2.Value)), "teq-CO2");
                consommationViewModel.TotalPrix = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(familleDevice.Prix.Value)), "");
                consommationViewModel.URL = familleDevice.URL;
                var detailConsommations = new List<DetailConsommationViewModel>();

                foreach (var categorieDevice in familleDevice.CategorieDevices)
                {
                    var detailConsommation1 = new DetailConsommationViewModel();

                    detailConsommation1.ConsoCo2 = new Consommation(null, Math.Round(categorieDevice.ConsoCo2.Value).ToString(), "teq-CO2");
                    detailConsommation1.ConsoWatt = new Consommation(null, Math.Round(categorieDevice.ConsoWatt.Value).ToString(), "kWh");
                    detailConsommation1.Prix = new Consommation(null, Math.Round(categorieDevice.Prix.Value).ToString(), "");
                    detailConsommation1.IdCategorie = categorieDevice.Id.ToString();
                    detailConsommation1.IdClient = categorieDevice.ClientId.ToString();
                    detailConsommation1.Libeller = categorieDevice.LibelleCategorieDevice;
                    detailConsommation1.URL = categorieDevice.URL;
                    if (typeUtilisateur == "Demo")
                    {
                        detailConsommation1.LienImage = "~/Content/images/Consommation/Demo/Categorie/" + detailConsommation1.IdCategorie + ".png";
                    }
                    else if (typeUtilisateur == "Payant")
                    {
                        detailConsommation1.LienImage = "~/Content/images/Consommation/Payant/Categorie/" + detailConsommation1.IdCategorie + ".png";
                    }
                    else if (typeUtilisateur == "Simulation")
                    {
                        detailConsommation1.LienImage = "~/Content/images/Consommation/Simulation/Categorie/" + detailConsommation1.IdCategorie + ".png";
                    }

                    //detailConsommation1.LongueurGrapheWattTotale = consommationHelper.CalculerLongeurGrapheTotal(Convert.ToDecimal(categorieDevice.ConsoWatt));
                    //detailConsommation1.LongueurGrapheWattMilieu = consommationHelper.CalculerLongeurGrapheMilieu(Convert.ToDecimal(categorieDevice.ConsoWatt));
                    //detailConsommation1.LongueurGrapheCO2Totale = consommationHelper.CalculerLongeurGrapheTotal(Convert.ToDecimal(categorieDevice.ConsoCo2));
                    //detailConsommation1.LongueurGrapheCO2Milieu = consommationHelper.CalculerLongeurGrapheMilieu(Convert.ToDecimal(categorieDevice.ConsoCo2));
                    detailConsommation1.Pourcentage = new Consommation(null, (Math.Round(categorieDevice.Pourcentage)).ToString(), "%");
                    detailConsommations.Add(detailConsommation1);
                }
                detailConsommations = consommationHelper.CalculerLongeurGraphe(detailConsommations);
                consommationViewModel.DetailConsommations = detailConsommations;
                return consommationViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("GetDetailsCategorieParFamille entrée: idClient= {0}, familleDevice, typeUtilisateur= {1}, sortie=>erreur : {1}", idClient, typeUtilisateur, e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Construire modèle pour affichage au niveau type device
        /// </summary>
        /// <param name="calculateurViewModel">Objet calculateurViewModel</param>
        /// <param name="idClient">Identifiant client</param>
        /// <param name="idCategorie">Identifiant catégorie</param>
        /// <param name="typeUtilisateur">typeUtilisateur : demo / payant / simulation</param>
        /// <returns>Objet ConsommationViewModel</returns>
        public ConsommationViewModel GetDetailsDevice(CalculateurViewModel calculateurViewModel, string idClient, string idCategorie, string typeUtilisateur)
        {
            try
            {
                var consommationViewModel = new ConsommationViewModel();
                var consommationHelper = new ConsommationHelper();

                var detailConsommations = new List<DetailConsommationViewModel>();
                var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
                foreach (var familleDevice in calculateurViewModel.FamilleDeviceViewModels)
                {
                    foreach (var categorieDevice in familleDevice.CategorieDevices)
                    {
                        if (categorieDevice.Id == int.Parse(idCategorie) && categorieDevice.TypeDevices.Count > 0)
                        {
                            foreach (var typeDevice in categorieDevice.TypeDevices)
                            {
                                //consommationViewModel.DateModifier = calculateurViewModel.ConfigSi.DateModification == null ? calculateurViewModel.ConfigSi.DateDebut.ToString("dd/MM/yyyy") : calculateurViewModel.ConfigSi.DateModification.Value.ToString("dd/MM/yyyy");
                                consommationViewModel.DateModifier = DateTime.Now.ToString("dd.MM.yyyy");
                                consommationViewModel.TotalConsoWatt = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(categorieDevice.ConsoWatt.Value)), "kWh");
                                consommationViewModel.TotalConsoCo2 = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(categorieDevice.ConsoCo2.Value)), "teq-CO2");
                                consommationViewModel.TotalPrix = new Consommation(null, ConsommationHelper.FormatterValeur(Math.Round(categorieDevice.Prix.Value)), "");
                                consommationViewModel.URL = categorieDevice.URL;

                                var detailConsommation = new DetailConsommationViewModel();

                                detailConsommation.ConsoCo2 = new Consommation(null, Math.Round(typeDevice.ConsoCo2).ToString(), "teq-CO2");
                                detailConsommation.ConsoWatt = new Consommation(null, Math.Round(typeDevice.ConsoWatt).ToString(), "kWh");
                                detailConsommation.Prix = new Consommation(null, Math.Round(typeDevice.Prix).ToString(), "");
                                detailConsommation.IdTypeDevice = typeDevice.Id.ToString();
                                detailConsommation.IdClient = idClient;
                                detailConsommation.IdCategorie = idCategorie;
                                detailConsommation.Libeller = typeDevice.LibelleTypeDevice;
                                detailConsommation.Source = typeDevice.ConfigSiDevice.Source;
                                detailConsommation.URL = typeDevice.URL;
                                if (typeUtilisateur == "Demo")
                                {
                                    detailConsommation.LienImage = "~/Content/images/Consommation/Demo/Categorie/" + detailConsommation.IdCategorie + ".png";
                                }
                                else if (typeUtilisateur == "Payant")
                                {
                                    detailConsommation.LienImage = "~/Content/images/Consommation/Payant/Categorie/" + detailConsommation.IdCategorie + ".png";
                                }
                                else if (typeUtilisateur == "Simulation")
                                {
                                    detailConsommation.LienImage = "~/Content/images/Consommation/Simulation/Categorie/" + detailConsommation.IdCategorie + ".png";
                                }
                                detailConsommations.Add(detailConsommation);
                            }
                        }
                    }
                }
                detailConsommations = consommationHelper.CalculerLargeurGraphe(detailConsommations);
                consommationViewModel.DetailConsommations = detailConsommations;
                return consommationViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("GetDetailsDevice entrée: calculateurViewModel, idClient= {0}, idCategorie= {1}, typeUtilisateur= {2}, sortie=>erreur : {1}", idClient, idCategorie, typeUtilisateur, e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Construire modèle pour affichage au niveau modification quantité device
        /// </summary>
        /// <param name="idCategorie">Identifiant catégorie</param>
        /// <param name="resultatCalculateur">Objet CalculateurViewModel</param>
        /// <param name="idClient">Identifiant client</param>
        /// <param name="typeUtilisateur">typeUtilisateur : demo / payant / simulation</param>
        /// <returns></returns>
        public ClientDeviceTypeViewModel GetTypeDevicesClient(int idCategorie, CalculateurViewModel resultatCalculateur, int idClient, string typeUtilisateur)
        {
            try
            {
                var consommationHelper = new ConsommationHelper();
                var clientDeviceTypeViewModel = new ClientDeviceTypeViewModel();

                clientDeviceTypeViewModel.IdClient = idClient;
                clientDeviceTypeViewModel.IdCategorie = idCategorie;
                
                var categorie = GetCategorieDevice(idCategorie);
                clientDeviceTypeViewModel.IdFamille = Convert.ToInt32(categorie.FamilleDeviceId);

                if (typeUtilisateur == "Payant" || typeUtilisateur == "Simulation")
                {
                    clientDeviceTypeViewModel.ConfigSiId = resultatCalculateur.ConfigSi.Id;
                }

                var classes = ListeClasseDevice();
                var ages = ListeAgeDevice();

                var configSiDeviceViewModelListe = new List<ConfigSiDeviceViewModel>();
                foreach (var familleDevice in resultatCalculateur.FamilleDeviceViewModels)
                {
                    foreach (var categorieDevice in familleDevice.CategorieDevices)
                    {
                        foreach (var typeDevice in categorieDevice.TypeDevices)
                        {
                            if (typeUtilisateur == "Payant" || typeUtilisateur == "Simulation")
                            {
                                typeDevice.ConfigSiDevice = _econocomDataManager.GetConfigSiDevice(typeDevice.ConfigSiDevice.Id);
                            }

                            if (categorieDevice.Id == idCategorie)
                            {
                                clientDeviceTypeViewModel.TotalConsoWatt = categorieDevice.ConsoWatt.HasValue ? categorieDevice.ConsoWatt.Value : 0;
                                clientDeviceTypeViewModel.TotalConsoCo2 = categorieDevice.ConsoCo2.HasValue ? categorieDevice.ConsoCo2.Value : 0;
                                clientDeviceTypeViewModel.TotalPrix = categorieDevice.Prix.HasValue ? categorieDevice.Prix.Value : 0;
                                clientDeviceTypeViewModel.URL = categorieDevice.URL;

                                var configSiDeviceViewModel = new ConfigSiDeviceViewModel();
                                if (typeUtilisateur == "Payant" || typeUtilisateur == "Simulation")
                                {
                                    configSiDeviceViewModel.Id = typeDevice.ConfigSiDevice.Id;
                                }
                                configSiDeviceViewModel.TypeDeviceId = Convert.ToInt32(typeDevice.Id);
                                configSiDeviceViewModel.LibelleDevice = typeDevice.LibelleTypeDevice;
                                configSiDeviceViewModel.Quantite = typeDevice.ConfigSiDevice.Quantite;
                                configSiDeviceViewModel.EstNouveau = false;
                                configSiDeviceViewModel.Source = typeDevice.ConfigSiDevice.Source;
                                configSiDeviceViewModel.TypeUsageId = typeDevice.ConfigSiDevice.TypeUsageId;
                                configSiDeviceViewModel.Guid = Guid.NewGuid();
                                configSiDeviceViewModel.URL = typeDevice.URL;

                                if (typeUtilisateur == "Payant" || typeUtilisateur == "Simulation")
                                {
                                    //ConfigSiDeviceAttributeViewModel
                                    var configSiDeviceAttributeViewModel = new ConfigSiDeviceAttributeViewModel();

                                    configSiDeviceAttributeViewModel.IdDevice = Convert.ToInt32(typeDevice.Id);
                                    configSiDeviceAttributeViewModel.IdCategorie = Convert.ToInt32(typeDevice.CategorieDeviceId);
                                    configSiDeviceAttributeViewModel.NbrJourTravaille = typeDevice.ConfigSiDevice.NbrJourTravaille;
                                    configSiDeviceAttributeViewModel.NbrHeuresIntensifJour = typeDevice.ConfigSiDevice.NbrHeuresIntensifJour;
                                    configSiDeviceAttributeViewModel.NbrHeuresOffJour = typeDevice.ConfigSiDevice.NbrHeuresOffJour;
                                    configSiDeviceAttributeViewModel.UtiliseEnergieExterne = typeDevice.ConfigSiDevice.UtiliseEnergieExterne;
                                    configSiDeviceAttributeViewModel.PourcentConsomExtern = typeDevice.ConfigSiDevice.PourcentConsomExtern;
                                    configSiDeviceAttributeViewModel.CoeffAbbatement = typeDevice.ConfigSiDevice.CoeffReductionWattExterne;
                                    configSiDeviceAttributeViewModel.NbrHeuresNonIntensifJour = typeDevice.ConfigSiDevice.NbrHeuresNonIntensifJour;
                                    configSiDeviceAttributeViewModel.CoeffNonIntensif = typeDevice.ConfigSiDevice.CoeffNonIntensif;
                                    configSiDeviceAttributeViewModel.CoeffModeOff = Convert.ToDecimal(typeDevice.ConfigSiDevice.CoeffModeOff);
                                    configSiDeviceAttributeViewModel.Quantite = typeDevice.ConfigSiDevice.Quantite;
                                    configSiDeviceAttributeViewModel.DateDebut = typeDevice.ConfigSiDevice.DateDebut;
                                    configSiDeviceAttributeViewModel.LibelleDevice = typeDevice.LibelleTypeDevice;
                                    configSiDeviceAttributeViewModel.EstNouveau = false;
                                    configSiDeviceAttributeViewModel.TypeUsageId = typeDevice.ConfigSiDevice.TypeUsageId;
                                    configSiDeviceAttributeViewModel.URL = typeDevice.URL;

                                    var configSiConsommation = new ConfigSiConsommationViewModel();
                                    configSiConsommation.IdDevice = Convert.ToInt32(typeDevice.Id);
                                    configSiConsommation.ConfigSiDeviceId = typeDevice.ConfigSiDevice.Id;
                                    configSiDeviceAttributeViewModel.ConfigSiConsommationViewModel = configSiConsommation;

                                    var gammeDeviceList = new List<GammesDeviceViewModel>();
                                    foreach (var classe in classes)
                                    {
                                        var gammeDevice = new GammesDeviceViewModel();
                                        gammeDevice.IdDevice = Convert.ToInt32(typeDevice.Id);
                                        gammeDevice.IdClasse = classe.Id;

                                        if (classe.ClasseDeviceTraductions != null &&
                                            classe.ClasseDeviceTraductions.ElementAt(0) != null)
                                            gammeDevice.Libeller =
                                                classe.ClasseDeviceTraductions.ElementAt(0).LibelleClasseDevice;
                                        else
                                            gammeDevice.Libeller = classe.LibelleClasseDevice;

                                        gammeDevice.isModified = false;
                                        gammeDevice.LibelleDevice = configSiDeviceViewModel.LibelleDevice;
                                        gammeDevice.Guid = configSiDeviceViewModel.Guid;

                                        var ageDeviceList = new List<AgeDeviceViewModel>();

                                        foreach (var age in ages)
                                        {
                                            var configConso = typeDevice.ConfigSiDevice.ConfigSiConsommations.Find(o => (o.ClasseDeviceId == classe.Id && o.AgeDeviceId == age.Id));

                                            var ageDevice = new AgeDeviceViewModel();
                                            if (familleDevice.SecteurActiviteId.HasValue)
                                                ageDevice.SecteurActiviteId = familleDevice.SecteurActiviteId.Value;

                                            if (familleDevice.ClientId.HasValue)
                                                ageDevice.ClientId = familleDevice.ClientId.Value;

                                            ageDevice.IdDevice = Convert.ToInt32(typeDevice.Id);
                                            ageDevice.IdAge = age.Id;//configConso.AgeDeviceId;
                                            ageDevice.IdClasse = classe.Id;//configConso.ClasseDeviceId;
                                            if (age.AgeDeviceTraductions != null &&
                                                age.AgeDeviceTraductions.ElementAt(0) != null)
                                                ageDevice.LibelleAgeDevice = age.AgeDeviceTraductions.ElementAt(0).LibelleAgeDevice;
                                            else
                                                ageDevice.LibelleAgeDevice = age.LibelleAgeDevice;

                                            ageDevice.Quantite = configConso != null ? configConso.Quantite : 0;
                                            ageDevice.IsModified = false;
                                            ageDevice.DateDebut = age.DateDebut;
                                            ageDevice.DateModification = age.DateModification;
                                            ageDevice.LibelleDevice = configSiDeviceViewModel.LibelleDevice;
                                            ageDevice.UniqueGuid = configSiDeviceViewModel.Guid;
                                            ageDeviceList.Add(ageDevice);
                                        }
                                        gammeDevice.AgeDeviceViewModels = ageDeviceList;
                                        gammeDevice.Quantiter = Convert.ToInt32(consommationHelper.CalculerTotalQuantite(ageDeviceList));
                                        gammeDeviceList.Add(gammeDevice);
                                    }

                                    var classeDeviceList = new List<ClasseDeviceViewModel>();
                                    foreach (var classe in classes)
                                    {
                                        var classeDevice = new ClasseDeviceViewModel();
                                        classeDevice.Id = classe.Id;
                                        classeDevice.LibelleClasseDevice = classe.LibelleClasseDevice;
                                        classeDevice.DateDebut = classe.DateDebut;
                                        classeDevice.DateModification = classe.DateModification;

                                        var ageDeviceConsoList = new List<AgeDeviceConsoViewModel>();
                                        foreach (var age in ages)
                                        {
                                            var configConso = typeDevice.ConfigSiDevice.ConfigSiConsommations.Find(o => (o.ClasseDeviceId == classe.Id && o.AgeDeviceId == age.Id));
                                            var ageDeviceConso = new AgeDeviceConsoViewModel();
                                            ageDeviceConso.AgeDeviceId = age.Id;
                                            ageDeviceConso.IdConfigConsommation = configConso != null ? configConso.Id : 0;
                                            ageDeviceConso.LibelleAge = age.LibelleAgeDevice;
                                            //ageDeviceConso.ConsommationIntensif = configConso != null ? Convert.ToDecimal(configConso.ConsommationIntensifUnitaire * (1 - (typeDevice.ConfigSiDevice.CoeffNonIntensif + typeDevice.ConfigSiDevice.CoeffModeOff))) : 0;
                                            ageDeviceConso.ConsommationIntensif = configConso != null ? Convert.ToDecimal(configConso.ConsommationIntensifUnitaire) : 0;
                                            ageDeviceConso.ConsommationNonIntensif = configConso != null ? configConso.ConsommationIntensifUnitaire * typeDevice.ConfigSiDevice.CoeffNonIntensif : 0;
                                            ageDeviceConso.ConsommationModeOff = configConso != null ? Convert.ToDecimal(configConso.ConsommationIntensifUnitaire * typeDevice.ConfigSiDevice.CoeffModeOff) : 0;

                                            ageDeviceConsoList.Add(ageDeviceConso);
                                        }
                                        classeDevice.AgeDeviceConsoViewModels = ageDeviceConsoList;
                                        classeDeviceList.Add(classeDevice);
                                    }

                                    configSiDeviceAttributeViewModel.ConfigSiConsommationViewModel.ClasseDeviceViewModel = classeDeviceList;
                                    configSiDeviceViewModel.detailsDeviceType = configSiDeviceAttributeViewModel;
                                    configSiDeviceViewModel.Gammes = gammeDeviceList;
                                }
                                configSiDeviceViewModelListe.Add(configSiDeviceViewModel);
                            }
                        }
                    }
                }
                clientDeviceTypeViewModel.ConfigSiDevice = configSiDeviceViewModelListe;
                return clientDeviceTypeViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("GetTypeDevicesClient entrée: calculateurViewModel, idCategorie= {0}, resultatCalculateur, idClient= {1}, typeUtilisateur= {2}, sortie=>erreur : {1}", idCategorie, idClient, typeUtilisateur, e.StackTrace);
                throw e;
            }
        }

        public ClientDeviceTypeViewModel GetClientBenchmark(int clientId)
        {
            throw new NotImplementedException();
        }

        public Client GetClientParNom(string nom)
        {
            return _econocomDataManager.GetClientParNom(nom);
        }

        public List<Contact> GetContactParNomClient(string nom)
        {
            return _econocomDataManager.GetContactParNomClient(nom);
        }

        public bool MAJContact(ContactsViewModel contactViewModel)
        {
            return _econocomDataManager.MAJContact(contactViewModel);
        }

        public bool SetListeUtilisateur(ListeUtilisateurViewModel listeUtilisateurViewModel)
        {
            try
            {
                _econocomDataManager.SetUtilisateur(listeUtilisateurViewModel.Administrateur);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            try
            {
                _econocomDataManager.SetUtilisateur(listeUtilisateurViewModel.Commercial);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return true;
        }

        public List<Politique> GetListePolitiques()
        {
            var listePolitiques = _econocomDataManager.GetPolitiques();
            foreach (var politique in listePolitiques)
            {
                if (politique.PolitiqueTraductions != null && politique.PolitiqueTraductions.ElementAt(0) != null)
                    politique.LibellePolitique = politique.PolitiqueTraductions.ElementAt(0).LibellePolitique;
                else
                    politique.LibellePolitique = politique.LibellePolitique;
            }
            return listePolitiques;
        }

        public Politique GetPolitiqueParId(int politiqueId)
        {
            var politique = _econocomDataManager.GetPolitiqueParId(politiqueId);
            if (politique.PolitiqueTraductions != null && politique.PolitiqueTraductions.ElementAt(0) != null)
            {
                politique.LibellePolitique = politique.PolitiqueTraductions.ElementAt(0).LibellePolitique;
                politique.Device = politique.PolitiqueTraductions.ElementAt(0).Device;
                politique.Benefice = politique.PolitiqueTraductions.ElementAt(0).Benefice;
                politique.Objectif = politique.PolitiqueTraductions.ElementAt(0).Objectif;
                politique.PolitiqueTraductions = null;
            }

            return politique;
        }

        public TypeContact GetTypeContactParLibelle(string libelle)
        {
            return _econocomDataManager.GetTypeContactParLibelle(libelle);
        }

        public CompteEvaluation SauvegardeEvaluation(CompteEvaluation compteEvaluation)
        {
            return _econocomDataManager.SauvegardeEvaluation(compteEvaluation);
        }

        public List<SecteurActivite> GetListeSecteurActivite()
        {
            try
            {
                int totalPages = 0;
                var listeSecteurActivites = _econocomDataManager.ListeSecteurActivite(0, 0, out totalPages);
                foreach (var secteurActivite in listeSecteurActivites)
                {
                    if (secteurActivite.SecteurActiviteTraductions != null && secteurActivite.SecteurActiviteTraductions.ElementAt(0) != null)
                        secteurActivite.LibelleSecteurActivite = secteurActivite.SecteurActiviteTraductions.ElementAt(0).LibelleSecteurActivite;
                    else
                        secteurActivite.LibelleSecteurActivite = secteurActivite.LibelleSecteurActivite;
                }
                return listeSecteurActivites;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }

        }

        public List<TypeObjet> GetListeTypeObjet()
        {
            try
            {
                var typeObjetListe = _econocomDataManager.GetListeTypeObjet();
                return typeObjetListe;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public Langue GetLangue(int id)
        {
            try
            {
                return _econocomDataManager.GetLangue(id);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ConfigSiDevice GetConfigSiDevice(int configSiDeviceId)
        {
            try
            {
                return _econocomDataManager.GetConfigSiDevice(configSiDeviceId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public ConfigSiDevice SauveGarderConfigSiDevice(ConfigSiDevice configSiDevice)
        {
            try
            {
                return _econocomDataManager.SauveGarderConfigSiDevice(configSiDevice);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public ConfigSiDevice MAJConfigSiDevice(ConfigSiDevice configSiDevice)
        {
            try
            {
                return _econocomDataManager.MAJConfigSiDevice(configSiDevice);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public ConfigSiConsommation SauveGarderConfigSiConsommation(ConfigSiConsommation configSiConsommation)
        {
            return _econocomDataManager.SauveGarderConfigSiConsommation(configSiConsommation);
        }

        public ConfigSiConsommation MAJConfigSiConsommation(ConfigSiConsommation configSiConsommation)
        {
            return _econocomDataManager.MAJConfigSiConsommation(configSiConsommation);
        }

        public ConfigSiConsommation GetConfigSiConsommation(int configSiDeviceId, int classeDeviceId, int ageDeviceId)
        {
            return _econocomDataManager.GetConfigSiConsommation(configSiDeviceId, classeDeviceId, ageDeviceId);
        }

        public Page GetPage(int id)
        {
            return _econocomDataManager.GetPage(id);
        }

        public List<Routage> ListeDesRoutages()
        {
            return _econocomDataManager.ListeDesRoutages();
        }

        public Reponse GetReponse(int contactId)
        {
            return _econocomDataManager.GetReponse(contactId);
        }

        public List<Question> GetListeQuestion(int currentPage, int pageDimension, out int totalPages)
        {
            return _econocomDataManager.ListeQuestion(currentPage, pageDimension, out totalPages);
        }

        public Contact GetContactParEmail(string email)
        {
            try
            {
                var contact = _econocomDataManager.GetContactParEmail(email);
                return contact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public Routage GetRoutageByLangueIdAndValeurActuelle(int langueId, string valeurActuelle)
        {
            try
            {
                return _econocomDataManager.GetRoutageByLangueIdAndValeurActuelle(langueId, valeurActuelle);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public Utilisateur GetUtilisateur(Utilisateur user)
        {
            return _econocomDataManager.GetUtilisateur(user);
        }

        /*public List<TypeRatio> ListeTypeRatio()
        {
            //return _econocomDataManager.ListeTypeRatio();
        }*/

        /// <summary>
        /// Recupere la liste des TypeRatio
        /// </summary>
        /// <returns></returns>
        public List<TypeRatio> GetListeTypeRatio(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.ListeTypeRatio(currentPage, pageDimension, out  totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste d'un TypeObjet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetListeObjet(int id, int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var objetType = GetTypeObjet(id);
                var typeObjetNamespace = objetType.NomObjet + "," + objetType.Namespace;
                var type = Type.GetType(typeObjetNamespace);
                IEnumerable<dynamic> liste = new List<dynamic>();
                totalPages = 1;
                currentPage = currentPage - 1;
                try
                {
                    var objet = new object();

                    if (type == typeof(Pays))
                    {
                        liste = _econocomDataManager.ListePays(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CategorieDevice))
                    {
                        liste = _econocomDataManager.ListeCategorieDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(ClasseDevice))
                    {
                        liste = _econocomDataManager.ListeClasseDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(AgeDevice))
                    {
                        liste = _econocomDataManager.ListeAgeDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(ConsoWattHeur))
                    {
                        liste = _econocomDataManager.ListeConsoWattHeur(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(FamilleDevice))
                    {
                        liste = _econocomDataManager.ListeFamilleDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Origine))
                    {
                        liste = _econocomDataManager.ListeOrigines(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Politique))
                    {
                        liste = _econocomDataManager.ListePolitique(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Ratio))
                    {
                        liste = _econocomDataManager.ListeRatio(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(SecteurActivite))
                    {
                        liste = _econocomDataManager.ListeSecteurActivite(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(TypeContact))
                    {
                        liste = _econocomDataManager.ListeTypeContact(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(TypeDevice))
                    {
                        liste = _econocomDataManager.ListeTypeDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(TypeObjet))
                    {
                        liste = _econocomDataManager.ListeTypeObjet(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(TypeUsage))
                    {
                        liste = _econocomDataManager.ListeTypeUsage(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(VentilationClasseAgeDevice))
                    {
                        liste = _econocomDataManager.ListeVentilationClasseAgeDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(VentilationClasseDevice))
                    {
                        liste = _econocomDataManager.ListeVentilationClasseDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Tarif))
                    {
                        liste = _econocomDataManager.ListeTarif(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Question))
                    {
                        liste = _econocomDataManager.ListeQuestion(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(ContenuMail))
                    {
                        liste = _econocomDataManager.ListeContenuMail(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(TypeMail))
                    {
                        liste = _econocomDataManager.ListeTypeMail(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(ChangementAnneeCalendaire))
                    {
                        liste = _econocomDataManager.ListeChangementAnneeCalendaire(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(TypeRatio))
                    {
                        liste = _econocomDataManager.ListeTypeRatio(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Equivalence))
                    {
                        liste = _econocomDataManager.ListeEquivalence(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Usage))
                    {
                        liste = _econocomDataManager.ListeUsage(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(UsageDevice))
                    {
                        liste = _econocomDataManager.ListeUsageDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(EquivalenceTraduction))
                    {
                        liste = _econocomDataManager.ListeEquivalenceTraduction(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(PolitiqueTraduction))
                    {
                        liste = _econocomDataManager.ListePolitiqueTraduction(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Langue))
                    {
                        liste = _econocomDataManager.ListeLangues(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Section))
                    {
                        liste = _econocomDataManager.ListeSection(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Modele))
                    {
                        liste = _econocomDataManager.ListeModele(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Page))
                    {
                        liste = _econocomDataManager.ListePage(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CleModele))
                    {
                        liste = _econocomDataManager.ListeCleModele(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(ContenuModere))
                    {
                        liste = _econocomDataManager.ListeContenuModere(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Routage))
                    {
                        liste = _econocomDataManager.ListeRoutage(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(Devise))
                    {
                        liste = _econocomDataManager.ListeDevise(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CorrespondanceSecteurActivite))
                    {
                        liste = _econocomDataManager.ListeCorrespondanceSecteurActivite(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CorrespondanceTypeDevice))
                    {
                        liste = _econocomDataManager.ListeCorrespondanceTypeDevice(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CorrespondanceProcesseur))
                    {
                        liste = _econocomDataManager.ListeCorrespondanceProcesseur(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CorrespondanceVitesse))
                    {
                        liste = _econocomDataManager.ListeCorrespondanceVitesse(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CorrespondanceTaille))
                    {
                        liste = _econocomDataManager.ListeCorrespondanceTaille(currentPage, pageDimension, out totalPages);
                    }
                    else if (type == typeof(CorrespondancePuissance))
                    {
                        liste = _econocomDataManager.ListeCorrespondancePuissance(currentPage, pageDimension, out totalPages);
                    }
                    return liste;
                }
                catch (Exception e)
                {
                    LogguerErreur(e);
                    throw;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }


        /// <summary>
        /// Recupere un TypeObjet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TypeObjet GetTypeObjet(int id)
        {
            try
            {
                return _econocomDataManager.GetTypeObjet(id);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere un TypeObjet par nom
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        public TypeObjet GetTypeObjet(string nom)
        {
            try
            {
                return _econocomDataManager.GetTypeObjet(nom);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public CategorieDevice GetCategorieDevice(int idCategorie)
        {
            try
            {
                return _econocomDataManager.GetCategorieDevice(idCategorie);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }


        public List<Simulation> GetSimulations(int clientId)
        {
            try
            {
                //var simulation = _econocomDataManager.GetSimulations(clientId);
                //return simulation;
                return null;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public DetailsClient GetDetailsClientById(int id)
        {
            try
            {
                var detailsClient = _econocomDataManager.GetDetailsClientById(id);
                return detailsClient;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public Simulation GetSimulationEnCours(int idContact)
        {
            try
            {
                return _econocomDataManager.GetSimulationEnCours(idContact);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<ClasseDevice> ListeClasseDevice()
        {
            try
            {
                var totalPages = 0;
                var listeClasseDevice = _econocomDataManager.ListeClasseDevice(0, 0, out totalPages);
                foreach (var classe in listeClasseDevice)
                {
                    if (classe.ClasseDeviceTraductions != null && classe.ClasseDeviceTraductions.ElementAt(0) != null)
                        classe.LibelleClasseDevice = classe.ClasseDeviceTraductions.ElementAt(0).LibelleClasseDevice;
                    else
                        classe.LibelleClasseDevice = classe.LibelleClasseDevice;
                }
                return listeClasseDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public List<AgeDevice> ListeAgeDevice()
        {
            try
            {
                var totalPages = 0;
                var listeAgeDevice = _econocomDataManager.ListeAgeDevice(0, 0, out totalPages);
                foreach (var age in listeAgeDevice)
                {
                    if (age.AgeDeviceTraductions != null && age.AgeDeviceTraductions.ElementAt(0) != null)
                        age.LibelleAgeDevice = age.AgeDeviceTraductions.ElementAt(0).LibelleAgeDevice;
                    else
                        age.LibelleAgeDevice = age.LibelleAgeDevice;
                }
                return listeAgeDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public Reponse SetReponse(Reponse reponse)
        {
            try
            {
                return _econocomDataManager.SetReponse(reponse);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public TypeContact GetTypeContactByString(string typeContact)
        {
            try
            {
                return _econocomDataManager.GetTypeContactByString(typeContact);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Adresse saveAdress(Adresse adresse)
        {
            try
            {
                return _econocomDataManager.saveAdress(adresse);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public DetailsClient saveDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                return _econocomDataManager.saveDetailsClient(detailsClient);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Client SaveClient(Client client)
        {
            try
            {
                client = _econocomDataManager.SaveClient(client);
                return client;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Contact SauvegardeContacte(Contact contact)
        {
            try
            {
                return _econocomDataManager.SauvegardeContact(contact);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<ContenuModere> GetListeContenuModere(int pageId)
        {
            try
            {
                return _econocomDataManager.GetListeContenuModere(pageId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public bool SupprimerContenuModere(ContenuModere contenuModere)
        {
            try
            {
                return _econocomDataManager.SupprimerContenuModere(contenuModere);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public bool SupprimerPage(Page page)
        {
            try
            {
                return _econocomDataManager.SupprimerPage(page);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Adresse updateAdress(Adresse adresse)
        {
            try
            {
                return _econocomDataManager.updateAdress(adresse);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Client updateClient(Client client)
        {
            try
            {
                return _econocomDataManager.updateClient(client);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Contact updateContacte(Contact contact)
        {
            try
            {
                return _econocomDataManager.updateContacte(contact);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Section GetSection(int id)
        {
            try
            {
                return _econocomDataManager.GetSection(id);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ContenuMail GetContenuMail(int idTypeMail, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetContenuMail(idTypeMail, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<TypeMail> GetListeTypeMail()
        {
            try
            {
                return _econocomDataManager.GetListeTypeMail();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ContenuMail SetContenuMail(ContenuMail contenuMail)
        {
            try
            {
                return _econocomDataManager.SetContenuMail(contenuMail);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public TypeMail GetTypeMail(string libelleTypeMail)
        {
            try
            {
                return _econocomDataManager.GetTypeMail(libelleTypeMail);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }



        public List<Econocom.Model.ViewModel.ContactViewModel> GetListeContact()
        {
            try
            {
                var liste = _econocomDataManager.GetListeContact();
                var listeContact = new List<Econocom.Model.ViewModel.ContactViewModel>();
                if (liste != null)
                {
                    foreach (var contact in liste)
                    {
                        var newContact = new Econocom.Model.ViewModel.ContactViewModel();
                        newContact.InjectFrom(contact);
                        if (contact.Reponses != null && contact.Reponses.Any())
                        {
                            newContact.Reponse = contact.Reponses.ElementAt(0).Valeur;
                            newContact.Question = contact.Reponses.ElementAt(0).Question.LibelleQuestion;
                        }
                        newContact.Profil = contact.TypeContact.LibelleTypeContact;
                        newContact.CodeInterneWattsGreen = contact.Client.CodeInterneWattsGreen;
                        newContact.DatePremiereConnexion = contact.DateDebut;
                        listeContact.Add(newContact);
                    }
                }
                return listeContact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public List<Section> GetChildren(int sectionId)
        {
            try
            {
                return _econocomDataManager.GetChildren(sectionId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public bool SupprimerSection(Section section)
        {
            try
            {
                return _econocomDataManager.SupprimerSection(section);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Tarif GetTarifById(int tarifId)
        {
            try
            {
                return _econocomDataManager.GetTarifById(tarifId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Document SauvegardeDocument(Document document)
        {
            try
            {
                return _econocomDataManager.SauvegarderDocument(document);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<Document> GetListeDocument(int? page, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.GetListeDocument(page, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public bool SupprimerDocument(int id)
        {
            try
            {
                return _econocomDataManager.SupprimerDocument(id);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public bool CodeInterneExiste(string code)
        {
            try
            {
                return _econocomDataManager.CodeInterneExiste(code);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Resultat GetResultatDevice(int configSiDeviceId)
        {
            try
            {
                return _econocomDataManager.GetResultatParConfigSiDevice(configSiDeviceId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Resultat SauvegarderResultat(Resultat resultat)
        {
            try
            {
                return _econocomDataManager.SauvegardeResultat(resultat);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ConfigSi GetConfigSi(int id)
        {
            try
            {
                return _econocomDataManager.GetConfigSi(id);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ConfigSi MAJConfigSi(ConfigSi configSi)
        {
            try
            {
                return _econocomDataManager.MAJConfigSi(configSi);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }


        public Document GetDocument(int docId)
        {
            try
            {
                return _econocomDataManager.GetDocument(docId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<string> GetListeEmails()
        {
            try
            {
                return _econocomDataManager.GetListeEmails();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Reponse UpdateReponse(Reponse reponse)
        {
            try
            {
                return _econocomDataManager.UpdateReponse(reponse);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Langue GetLangueParIdPays(int idPays)
        {
            try
            {
                return _econocomDataManager.GetLangueParIdPays(idPays);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<ConfigSiDevice> GetConfigSiDeviceParLibelle(string libelle, int configSiId)
        {
            try
            {
                return _econocomDataManager.GetConfigSiDeviceParLibelle(libelle, configSiId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Simulation SauvegardeSimulation(Simulation simulation)
        {
            try
            {
                return _econocomDataManager.SauvegardeSimulation(simulation);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ConfigSiClient GetDernierConfigSiPourClient(int idClient)
        {
            try
            {
                return _econocomDataManager.GetConfigSiClient(idClient);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ConfigSiClient GetConfigSiPourClientAvecStatutModifie(int idClient)
        {
            try
            {
                return _econocomDataManager.GetConfigSiPourClientAvecStatutModifie(idClient);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public bool GenererRapportPersonnalisation(int idClient, int idContact)
        {
            try
            {
                var configSi = _econocomDataManager.GetConfigSiPourClientAvecStatutModifie(idClient).ConfigSi;

                //create new configSi same as current configSi
                var origine = _econocomDataManager.GetOrigineParLibelle(OrigineEnum.Rapport.ToString());
                var configSiNouveau = CreerCopyConfigSiExistant(configSi, idClient, origine);

                /*
                object configSiObj = Activator.CreateInstance(typeof(ConfigSi));
                configSi.InjectFrom(configSi);                
                var configSiNouveau = (ConfigSi)configSiObj;
                configSiNouveau.Statut = (int)ConfigSiStatut.Creer;
                configSiNouveau.Resultats = configSi.Resultats;
                configSiNouveau.ConfigSiClients = configSi.ConfigSiClients;
                configSiNouveau.ConfigSiDevices = configSi.ConfigSiDevices;
                configSiNouveau = _econocomDataManager.SauvegardeConfigSi(configSiNouveau);*/

                //creer rapport
                var rapport = new Rapport();
                rapport.ContactId = idContact;
                rapport.OrigineId = 2;
                rapport.ConfigSiId = configSiNouveau.Id;
                rapport.DateDebut = DateTime.Now;
                rapport.HeureCree = DateTime.Now.TimeOfDay;
                rapport.LibelleRapport = "Rapport_Personnalisation_" + idClient + "_" + DateTime.Now;
                rapport = _econocomDataManager.SauvegarderRapport(rapport);

                //set current configSi to generer & MAJ configSi
                var configSiModifie = _econocomDataManager.GetConfigSiPourClientAvecStatutModifie(idClient).ConfigSi;
                configSiModifie.Statut = (int)ConfigSiStatut.Generer;
                configSiModifie = _econocomDataManager.MAJConfigSi(configSiModifie);

                return true;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return false;
            }
        }

        public ConfigSi CreerCopyConfigSiExistant(ConfigSi configSi, int idClient, Origine origine)
        {
            try
            {
                //creer configSi
                var configSiNouveau = new ConfigSi();
                configSiNouveau.LibelleConfigSi = "configSi" + idClient;
                configSiNouveau.Statut = (int)ConfigSiStatut.Creer;
                configSiNouveau.DateDebut = DateTime.Now;
                configSiNouveau = _econocomDataManager.SauvegardeConfigSi(configSiNouveau);

                //creer configSiClient
                var configSiClientNouveau = new ConfigSiClient();
                configSiClientNouveau.ConfigSiId = configSiNouveau.Id;
                configSiClientNouveau.ClientId = idClient;
                configSiClientNouveau.DateDebut = DateTime.Now;
                configSiClientNouveau.OrigineId = origine.Id;
                configSiClientNouveau = _econocomDataManager.SauvegardeConfigSiClient(configSiClientNouveau);

                //creer configSiDevices 
                foreach (var configSiDevice in configSi.ConfigSiDevices)
                {
                    object configSiDeviceObj = Activator.CreateInstance(typeof(ConfigSiDevice));
                    configSiDeviceObj.InjectFrom(configSiDevice);
                    var configSiDeviceNouveau = (ConfigSiDevice)configSiDeviceObj;
                    configSiDeviceNouveau.ConfigSi = null;
                    configSiDeviceNouveau.ConfigSiId = configSiNouveau.Id;

                    //configSiConsommations - set configSiDevice to null before save
                    var configSiConsommations = configSiDevice.ConfigSiConsommations;
                    if (configSiConsommations != null && configSiConsommations.Count > 0)
                    {
                        foreach (var configSiConsommation in configSiConsommations)
                        {
                            configSiConsommation.ConfigSiDevice = null;
                        }
                    }

                    configSiDeviceNouveau.ConfigSiConsommations = configSiDevice.ConfigSiConsommations;
                    configSiDeviceNouveau = _econocomDataManager.SauveGarderConfigSiDevice(configSiDeviceNouveau);


                    var resultat = configSi.Resultats.Single(x => x.ConfigSiDeviceId == configSiDevice.Id);
                    object resultatObj = Activator.CreateInstance(typeof(Resultat));
                    resultatObj.InjectFrom(resultat);
                    var resultatNouveau = (Resultat)resultatObj;
                    resultatNouveau.ConfigSi = null;
                    resultatNouveau.ConfigSiId = configSiNouveau.Id;
                    resultatNouveau.ConfigSiDevice = null;
                    resultatNouveau.ConfigSiDeviceId = configSiDeviceNouveau.Id;
                    resultatNouveau = _econocomDataManager.SauvegardeResultat(resultatNouveau);

                }
                return configSiNouveau;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public bool TerminerSimulationEnCours(int idClient, int idContact)
        {
            try
            {
                var simulations = _econocomDataManager.GetSimulationsEnCours(idContact);

                if (simulations != null)
                {
                    foreach (var simulation in simulations)
                    {
                        simulation.Statut = (int)SimulationStatut.Terminer;
                        var updatedSimulation = _econocomDataManager.MAJSimulation(simulation);
                    }
                }
                else
                {
                    Logger.Debug("Aucune simulation a terminee avant la creation d'une nouvelle simulation");
                }
                return true;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return false;
            }
        }

        public Simulation CreerNouvelleSimulation(int idClient, int idContact, SimulationViewModel simulationViewModel)
        {
            try
            {
                var timeStart = ConsommationHelper.GetStartTime("CreerNouvelleSimulation");
                var terminer = TerminerSimulationEnCours(idClient, idContact);

                if (terminer)
                {
                    var configSi = _econocomDataManager.GetConfigSiClient(idClient).ConfigSi;

                    var origine = _econocomDataManager.GetOrigineParLibelle(OrigineEnum.Simulation.ToString());

                    //creer un nouveau configSi identique au configSi actuel : configSi source
                    var configSiSource = CreerCopyConfigSiExistant(configSi, idClient, origine);

                    //creer un nouveau configSi identique au configSi actuel : configSi simule
                    var configSiNouveau = CreerCopyConfigSiExistant(configSi, idClient, origine);

                    //creer simulation
                    var simulation = new Simulation();
                    simulation.PolitiqueId = simulationViewModel.PolitiqueId;
                    simulation.ContactId = idContact;
                    simulation.OrigineId = 3;
                    simulation.ConfigSiSourceId = configSiSource.Id;
                    simulation.ConfigSiSimuleId = configSiNouveau.Id;
                    simulation.DateDebut = DateTime.Now;
                    simulation.LibelleSimulation = simulationViewModel.LibelleSimulation;
                    simulation.Statut = (int)SimulationStatut.EnCours;

                    //sauvegarde simulation
                    simulation = _econocomDataManager.SauvegardeSimulation(simulation);
                    var timeDifff = ConsommationHelper.GetTimeDifference(timeStart, "CreerNouvelleSimulation end");
                    return simulation;
                }
                else
                {
                    Logger.Debug("Echec pour terminer la simulation en cours");
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
            return null;
        }


        public bool GenererRapportSimulation(int idClient, int idContact)
        {

            try
            {
                //get simulation en cours
                var simulation = _econocomDataManager.GetSimulationEnCours(idContact);

                var configSi = _econocomDataManager.GetConfigSi(simulation.ConfigSiSimuleId.Value);

                //create new configSi same as current configSi
                //var configSiNouveau = CreerCopyConfigSiExistant(configSi, idClient);

                //creer rapport
                var rapport = new Rapport();
                rapport.ContactId = idContact;
                rapport.OrigineId = 3;
                rapport.ConfigSiId = configSi.Id;
                rapport.DateDebut = DateTime.Now;
                rapport.HeureCree = DateTime.Now.TimeOfDay;
                rapport.LibelleRapport = simulation.LibelleSimulation;
                rapport = _econocomDataManager.SauvegarderRapport(rapport);

                //set current configSi to generer & MAJ configSi
                var configSiModifie = _econocomDataManager.GetConfigSi(simulation.ConfigSiSimuleId.Value);
                configSiModifie.Statut = (int)ConfigSiStatut.Generer;
                configSiModifie = _econocomDataManager.MAJConfigSi(configSiModifie);

                //MAJ simulation
                simulation.Statut = (int)SimulationStatut.Terminer;
                simulation = _econocomDataManager.MAJSimulation(simulation);

                return true;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return false;
            }
        }


        public List<Rapport> GetRapportPersonnalisation(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPage)
        {
            try
            {
                var rapports = _econocomDataManager.GetRapportPersonnalisation(id, search, page, sort, sortdir, pageDimension, out totalPage);
                return rapports;
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}, pageDimension={5}", id, search, page, sort, sortdir, pageDimension);
                LogguerErreur(e);
                totalPage = 1;
                return null;
            }
            return null;
        }

        public Rapport GetRapport(int rapportId)
        {
            try
            {
                var rapport = _econocomDataManager.GetRapport(rapportId);
                return rapport;
            }
            catch (Exception e)
            {
                Logger.Error("id={0}", rapportId);
                LogguerErreur(e);
                return null;
            }
            return null;
        }

        public List<Rapport> GetRapportSimulation(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPage)
        {
            try
            {
                var rapports = _econocomDataManager.GetRapportSimulation(id, search, page, sort, sortdir, pageDimension, out totalPage);
                return rapports;
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}, pageDimension={5}", id, search, page, sort, sortdir, pageDimension);
                LogguerErreur(e);
                totalPage = 1;
                return null;
            }
            return null;
        }

        public List<Equivalence> GetListeEquivalence(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                return _econocomDataManager.GetListeEquivalence(page, pageDimension, sort, sortdir, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                LogguerErreur(e);
                return null;
            }
        }

        public List<Politique> GetListePolitique(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                return _econocomDataManager.GetListePolitique(page, pageDimension, sort, sortdir, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                LogguerErreur(e);
                return null;
            }
        }

        public Equivalence GetEquivalence(int id)
        {
            try
            {
                return _econocomDataManager.GetEquivalence(id);
            }
            catch (Exception e)
            {
                Logger.Error("equivalence id {0}", id);
                LogguerErreur(e);
                return null;
            }
        }

        public Equivalence SetEquivalence(Equivalence equivalence)
        {
            try
            {
                return _econocomDataManager.SetEquivalence(equivalence);
            }
            catch (Exception e)
            {
                Logger.Error("equivalence libelle {0}", equivalence.LibelleEquivalence);
                LogguerErreur(e);
                return null;
            }
        }

        public Politique GetPolitique(int id)
        {
            try
            {
                return _econocomDataManager.GetPolitique(id);
            }
            catch (Exception e)
            {
                Logger.Error("Politique id {0}", id);
                LogguerErreur(e);
                return null;
            }
        }

        public Politique SetPolitique(Politique politique)
        {
            try
            {
                return _econocomDataManager.SetPolitique(politique);
            }
            catch (Exception e)
            {
                Logger.Error("politique libelle {0}", politique.LibellePolitique);
                LogguerErreur(e);
                return null;
            }
        }

        public ResultatSimulationViewModel GetResultatSimulation(int idContact)
        {
            Logger.Debug("GetResultatSimulation : Start");
            Logger.Debug("IdContact = " + idContact);
            try
            {
                var resultatSimulation = new ResultatSimulationViewModel();
                var simulationHelper = new SimulationHelper();

                //Get Simulation
                var simulation = _econocomDataManager.GetSimulationEnCours(idContact);
                if (simulation != null)
                {
                    Logger.Debug("Simulation Id = " + simulation.Id);
                    resultatSimulation.DateModifier = simulation.DateModification != null ? simulation.DateModification.Value.ToString("dd.MM.yyyy") : simulation.DateDebut.ToString("dd.MM.yyyy");
                    resultatSimulation.NomSimulation = simulation.LibelleSimulation;
                }
                else
                {
                    Logger.Debug("Simulation est nulle ");
                }

                //Get liste famille
                var listeFamilleDevices = _econocomDataManager.GetListeFamilleDevices();

                var listeDetailsSimulation = new List<DetailResultatSimulationViewModel>();
                foreach (var famille in listeFamilleDevices)
                {
                    var detailSimulation = new DetailResultatSimulationViewModel();
                    detailSimulation.IdFamille = famille.Id;
                    detailSimulation.LibelleFamille = famille.LibelleFamilleDevice;
                    listeDetailsSimulation.Add(detailSimulation);
                }

                //get configSiSource
                var configSiSource = _econocomDataManager.GetConfigSiParId(simulation.ConfigSiSourceId.Value);
                if (configSiSource != null)
                {
                    Logger.Debug("configSiSource Id = " + configSiSource.Id);
                    listeDetailsSimulation = CalculerConsoParFamillePourConfigSi(configSiSource, listeDetailsSimulation, "Source");
                }
                else
                {
                    Logger.Debug("configSiSource est nulle ");
                }


                //get configSiSimule
                var configSiSimule = _econocomDataManager.GetConfigSiParId(simulation.ConfigSiSimuleId.Value);
                if (configSiSimule != null)
                {
                    Logger.Debug("configSiSimule Id = " + configSiSimule.Id);
                    listeDetailsSimulation = CalculerConsoParFamillePourConfigSi(configSiSimule, listeDetailsSimulation, "Simule");
                }
                else
                {
                    Logger.Debug("configSiSimule est nulle ");
                }


                //comparer conso source et simule
                Logger.Debug("Comparer conso source et simule");
                listeDetailsSimulation = simulationHelper.ComparerConsoSourceEtSimule(listeDetailsSimulation);

                //calculer total conso source
                Logger.Debug("calculer total conso simule");
                decimal totalConsoWattSource = simulationHelper.CalculerTotalConsoSource(listeDetailsSimulation);

                resultatSimulation.TotalConsoWattSource = totalConsoWattSource;
                resultatSimulation.TotalConsoWattSimule = simulationHelper.CalculerTotalConsoWattSimule(listeDetailsSimulation);
                resultatSimulation.TotalConsoCO2Simule = simulationHelper.CalculerTotalConsoCo2Simule(listeDetailsSimulation);

                resultatSimulation.DetailsSimulation = listeDetailsSimulation;

                //get prix kwt du client
                var contact = _econocomDataManager.GetDetailsContact(idContact);
                var prixKwatt = contact.Client.DetailsClient.PrixKWH;
                var devise = contact.Client.DetailsClient.Devise;

                var codeDevise = devise != null ? devise.CodeDevise : " ";
                listeDetailsSimulation = simulationHelper.CalculerPrixSimuler(listeDetailsSimulation, prixKwatt, codeDevise);
                resultatSimulation.TotalPrixSimule = (resultatSimulation.TotalConsoWattSimule * prixKwatt);

                //calculer pourcentage
                Logger.Debug("calculer pourcentage");
                resultatSimulation = simulationHelper.CalculerPourcentage(resultatSimulation);

                //calculer longeur graphes
                Logger.Debug("calculer longeur graphes");
                resultatSimulation = simulationHelper.CalculerLongeurGraphes(resultatSimulation);

                var listeEquivalence = _econocomDataManager.GetListeEquivalencePourAfficher();

                foreach (var equivalence in listeEquivalence)
                {
                    if (equivalence.EquivalenceTraductions != null && equivalence.EquivalenceTraductions.ElementAt(0) != null)
                        equivalence.LibelleEquivalence = equivalence.EquivalenceTraductions.ElementAt(0).LibelleEquivalence;
                    else
                        equivalence.LibelleEquivalence = equivalence.LibelleEquivalence;
                }
                Logger.Debug("listeEquivalence size = " + listeEquivalence.Count);

                //calculer equivalence
                Logger.Debug("calculer equivalence");
                resultatSimulation = simulationHelper.CalculerEquivalence(listeEquivalence, resultatSimulation);

                Logger.Debug("GetResultatSimulation : End");
                return resultatSimulation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public List<DetailResultatSimulationViewModel> CalculerConsoParFamillePourConfigSi(ConfigSi configSi, List<DetailResultatSimulationViewModel> listeDetailSimulation, string typeConfig)
        {
            Logger.Debug("CalculerConsoParFamillePourConfigSi : Start");

            decimal totalConsoWattFamille1 = 0;
            decimal totalConsoWattFamille2 = 0;
            decimal totalConsoWattFamille3 = 0;
            decimal totalConsoWattFamille4 = 0;

            decimal totalConsoCo2Famille1 = 0;
            decimal totalConsoCo2Famille2 = 0;
            decimal totalConsoCo2Famille3 = 0;
            decimal totalConsoCo2Famille4 = 0;

            try
            {
                foreach (var resultat in configSi.Resultats)
                {
                    //get typeDevcie
                    var typeDevice = _econocomDataManager.GetTypeDeviceParId(resultat.ConfigSiDevice.TypeDeviceId);
                    Logger.Debug("TypeDevice Id = " + typeDevice.Id);

                    var familleId = typeDevice.CategorieDevice.FamilleDeviceId;
                    Logger.Debug("Famille Id = " + familleId);

                    switch (familleId)
                    {
                        case 1:
                            {
                                totalConsoWattFamille1 = totalConsoWattFamille1 + (resultat.ConsommationWatt.HasValue ? resultat.ConsommationWatt.Value : 0);
                                Logger.Debug("totalConsoWattFamille1 = " + totalConsoWattFamille1);
                                totalConsoCo2Famille1 = totalConsoCo2Famille1 + (resultat.ConsommationCo2.HasValue ? resultat.ConsommationCo2.Value : 0);
                                Logger.Debug("totalConsoCo2Famille1 = " + totalConsoCo2Famille1);
                            }
                            break;
                        case 2:
                            {
                                totalConsoWattFamille2 = totalConsoWattFamille2 + (resultat.ConsommationWatt.HasValue ? resultat.ConsommationWatt.Value : 0);
                                Logger.Debug("totalConsoWattFamille2 = " + totalConsoWattFamille2);
                                totalConsoCo2Famille2 = totalConsoCo2Famille2 + (resultat.ConsommationCo2.HasValue ? resultat.ConsommationCo2.Value : 0);
                                Logger.Debug("totalConsoCo2Famille2 = " + totalConsoCo2Famille2);
                            }
                            break;
                        case 3:
                            {
                                totalConsoWattFamille3 = totalConsoWattFamille3 + (resultat.ConsommationWatt.HasValue ? resultat.ConsommationWatt.Value : 0);
                                Logger.Debug("totalConsoWattFamille3 = " + totalConsoWattFamille3);
                                totalConsoCo2Famille3 = totalConsoCo2Famille3 + (resultat.ConsommationCo2.HasValue ? resultat.ConsommationCo2.Value : 0);
                                Logger.Debug("totalConsoCo2Famille3 = " + totalConsoCo2Famille3);
                            }
                            break;
                        case 4:
                            {
                                totalConsoWattFamille4 = totalConsoWattFamille4 + (resultat.ConsommationWatt.HasValue ? resultat.ConsommationWatt.Value : 0);
                                Logger.Debug("totalConsoWattFamille4 = " + totalConsoWattFamille4);
                                totalConsoCo2Famille4 = totalConsoCo2Famille4 + (resultat.ConsommationCo2.HasValue ? resultat.ConsommationCo2.Value : 0);
                                Logger.Debug("totalConsoCo2Famille4 = " + totalConsoCo2Famille4);
                            }
                            break;
                    }
                }

                if (typeConfig.Equals("Source"))
                {
                    listeDetailSimulation.Find(o => o.IdFamille == 1).consoWattSource = totalConsoWattFamille1;
                    listeDetailSimulation.Find(o => o.IdFamille == 2).consoWattSource = totalConsoWattFamille2;
                    listeDetailSimulation.Find(o => o.IdFamille == 3).consoWattSource = totalConsoWattFamille3;
                    listeDetailSimulation.Find(o => o.IdFamille == 4).consoWattSource = totalConsoWattFamille4;
                    listeDetailSimulation.Find(o => o.IdFamille == 1).consoCo2Source = totalConsoCo2Famille1;
                    listeDetailSimulation.Find(o => o.IdFamille == 2).consoCo2Source = totalConsoCo2Famille2;
                    listeDetailSimulation.Find(o => o.IdFamille == 3).consoCo2Source = totalConsoCo2Famille3;
                    listeDetailSimulation.Find(o => o.IdFamille == 4).consoCo2Source = totalConsoCo2Famille4;
                    Logger.Debug("typeConfig = source");
                }
                else
                {
                    listeDetailSimulation.Find(o => o.IdFamille == 1).consoWattSimule = totalConsoWattFamille1;
                    listeDetailSimulation.Find(o => o.IdFamille == 2).consoWattSimule = totalConsoWattFamille2;
                    listeDetailSimulation.Find(o => o.IdFamille == 3).consoWattSimule = totalConsoWattFamille3;
                    listeDetailSimulation.Find(o => o.IdFamille == 4).consoWattSimule = totalConsoWattFamille4;
                    listeDetailSimulation.Find(o => o.IdFamille == 1).consoCo2Simule = totalConsoCo2Famille1;
                    listeDetailSimulation.Find(o => o.IdFamille == 2).consoCo2Simule = totalConsoCo2Famille2;
                    listeDetailSimulation.Find(o => o.IdFamille == 3).consoCo2Simule = totalConsoCo2Famille3;
                    listeDetailSimulation.Find(o => o.IdFamille == 4).consoCo2Simule = totalConsoCo2Famille4;
                    Logger.Debug("typeConfig = simule");
                }

                Logger.Debug("CalculerConsoParFamillePourConfigSi : End");
                return listeDetailSimulation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public Simulation MAJSimulation(Simulation simulation)
        {
            try
            {
                return _econocomDataManager.MAJSimulation(simulation);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<Client> GetListeClientsSouscrits(DateTime dateDebut, DateTime dateFin, int? page, int pageDimension, out int totalPages)
        {
            try
            {
                return _econocomDataManager.GetListeClientsSouscrits(dateDebut, dateFin, page, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }


        public Simulation GetSimulationParIdRapport(int idRapport)
        {
            try
            {
                return _econocomDataManager.GetSimulationParIdRapport(idRapport);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ListeClientViewModel GetListeClients(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPage)
        {
            var listeClient = new List<ClientViewModel>();
            totalPage = 1;
            try
            {
                var liste = _econocomDataManager.GetListeClients(id, search, page, sort, sortdir, pageDimension, out totalPage);
                if (liste != null)
                {
                    foreach (var client in liste)
                    {
                        try
                        {
                            var clientViewModel = new ClientViewModel();

                            if (client.Adresse != null)
                                clientViewModel.InjectFrom(client.Adresse);
                            if (client.DetailsClient != null && client.DetailsClient.Tarif != null)
                                clientViewModel.InjectFrom(client.DetailsClient.Tarif);

                            clientViewModel.ClientEconocom = Resource.Traduction.ResourceManager.GetString(client.ClientEconocom.ToString().ToUpper());
                            clientViewModel.Adresse1 = client.Adresse.Adresse1;
                            clientViewModel.Ville = client.Adresse.Ville;
                            clientViewModel.CodePostal = client.Adresse.CodePostal;
                            clientViewModel.LibellePays = client.Adresse.Pays.LibellePays;
                            clientViewModel.ConfigSi = (client.ConfigSIClients != null && client.ConfigSIClients.Count > 0);

                            //we inject from client last to get the correct Id value (client Id)
                            //if we inject from client first, the Id value is overridden by other entities with their Id value.
                            clientViewModel.InjectFrom(client);
                            listeClient.Add(clientViewModel);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("id={0}", client.Id);
                            LogguerErreur(e);
                        }
                    }

                    var listeClientViewModel = new ListeClientViewModel();
                    listeClientViewModel.Clients = listeClient;
                    listeClientViewModel.Total = totalPage;
                    return listeClientViewModel;
                }
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}, pageDimension={5}", id, search, page, sort, sortdir, pageDimension);
            }
            return null;
        }

        public Rapport GetRapportClient(int clientId)
        {
            try
            {
                var rapport = _econocomDataManager.GetRapportClient(clientId);
                return rapport;
            }
            catch (Exception e)
            {
                Logger.Error("client id={0}", clientId);
                LogguerErreur(e);
            }
            return null;
        }

        public List<Client> GetListeSouscriptions(DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                return _econocomDataManager.GetListeSouscriptions(dateDebut, dateFin);
            }
            catch (Exception e)
            {
                Logger.Error("dateDebut={0}, dateFin={1} ", dateDebut, dateFin);
                LogguerErreur(e);
            }
            return null;
        }

        public Politique SauvegardePolitique(Politique politique)
        {
            try
            {
                return _econocomDataManager.SauvegardePolitique(politique);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Utilisateur GetCommercial()
        {
            try
            {
                return _econocomDataManager.GetCommercial();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Utilisateur GetAdministrateur()
        {
            try
            {
                return _econocomDataManager.GetAdministrateur();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Politique GetDernierPolitique()
        {
            try
            {
                return _econocomDataManager.GetDernierPolitique();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Politique MAJPolitique(Politique politique)
        {
            try
            {
                return _econocomDataManager.MAJPolitique(politique);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Equivalence GetDernierEquivalence()
        {
            try
            {
                return _econocomDataManager.GetDernierEquivalence();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Equivalence SauvegardeEquivalence(Equivalence equivalence)
        {
            try
            {
                return _econocomDataManager.SauvegardeEquivalence(equivalence);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }


        public Equivalence MAJEquivalence(Equivalence equivalence)
        {
            try
            {
                return _econocomDataManager.MAJEquivalence(equivalence);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public bool MajConfigSiPourNouvelleAnnee(int batchSize)
        {
            try
            {
                Logger.Debug("MajConfigSiPourNouvelleAnnee batchsize {0}", batchSize);
                var recalculerQuantitePourNouvelleAnnee = RecalculerQuantitePourNouvelleAnnee(batchSize);

                if (recalculerQuantitePourNouvelleAnnee)
                {
                    recalculerQuantitePourNouvelleAnnee = RecalculerConsommationPourNouvelleAnnee(batchSize);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return false;
            }
            return true;
        }

        public bool SetConfigSiDevice(ConfigSi configSi)
        {
            try
            {
                if (configSi != null && configSi.ConfigSiDevices != null)
                {
                    for (int i = 0; i < configSi.ConfigSiDevices.Count; i++)
                    {
                        var configSiDevice = configSi.ConfigSiDevices.ElementAt(i);
                        var maj = _econocomDataManager.MAJConfigSiDevice(configSiDevice);
                        foreach (var consommation in configSiDevice.ConfigSiConsommations)
                        {
                            var majConso = _econocomDataManager.MAJConfigSiBatchConsommation(consommation);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                Logger.Error("configsi {0}", configSi.Id);
                return false;
            }
            return true;
        }

        //public bool SetConfigSiConsommation(ConfigSi configSi)
        //{
        //    try
        //    {
        //        if (configSi != null && configSi.ConfigSiDevices!=null)
        //        {
        //            for (int i = 0; i < configSi.ConfigSiDevices.Count; i++)
        //            {
        //                var configSiDevice = configSi.ConfigSiDevices.ElementAt(i);
        //                if (configSiDevice != null && configSiDevice.ConfigSiConsommations.Count > 0)
        //                {
        //                    for (int j = 0; j < configSiDevice.ConfigSiConsommations.Count; j++)
        //                    {
        //                        if (configSiDevice.ConfigSiConsommations[j] != null)
        //                        {
        //                            configSiDevice.ConfigSiConsommations[j].DateModification = DateTime.Now;
        //                            _econocomDataManager.MAJConfigSiBatchConsommation(configSiDevice.ConfigSiConsommations[j]);
        //                        }

        //                    }
        //                    _econocomDataManager.MAJConfigSiDevice(configSiDevice);
        //                }
        //            }                    
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //       LogguerErreur(e);
        //        Logger.Error("configsi {0}", configSi.Id);
        //        return false;
        //    }
        //    return true;
        //}

        public bool SetConfigSi(ConfigSi configSi)
        {
            try
            {
                if (configSi != null)
                {
                    configSi.DateModification = DateTime.Now;
                    _econocomDataManager.SetConfigSi(configSi);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                Logger.Error("configsi {0}", configSi.Id);
                return false;
            }
            return true;
        }

        public bool SetConfigSiClient(ConfigSiClient configSiClient)
        {
            try
            {
                if (configSiClient != null)
                {
                    configSiClient.DateModification = DateTime.Now;
                    configSiClient.ConfigSi.DateModification = DateTime.Now;
                    foreach (var configSiDevice in configSiClient.ConfigSi.ConfigSiDevices)
                    {
                        configSiDevice.DateModification = DateTime.Now;
                        foreach (var configSiConsommation in configSiDevice.ConfigSiConsommations)
                        {
                            configSiConsommation.DateModification = DateTime.Now;
                        }
                    }
                    _econocomDataManager.SetConfigSiClient(configSiClient);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                Logger.Error("configsi {0}", configSiClient.Id);
                return false;
            }
            return true;
        }


        public bool RecalculerQuantitePourNouvelleAnnee(int batchSize)
        {
            try
            {
                if (batchSize < 1)
                {
                    Logger.Debug("RecalculerQuantitePourNouvelleAnnee batchSize {0} ", batchSize);
                    return false;
                }

                var nombreClients = _econocomDataManager.GetNombreClientsValide();
                Logger.Debug("RecalculerQuantitePourNouvelleAnnee  nombreClients {0}", nombreClients);
                if (nombreClients < 1)
                {
                    Logger.Debug("RecalculerQuantitePourNouvelleAnnee: Pas de client valide");
                    return false;
                }

                var listeChangementAnneeCalendaire = _econocomDataManager.GetListeChangementAnneeCalendaire();
                if (listeChangementAnneeCalendaire != null)
                {
                    var pages = (double)nombreClients / batchSize;
                    var totalPages = (int)Math.Ceiling(pages);
                    Logger.Debug("RecalculerQuantite pages {0} totalPages {1}", pages, totalPages);

                    for (int i = 0; i < totalPages; ++i)
                    {
                        Logger.Debug("RecalculerQuantite page {0} of totalPages {1} ..", i + 1, totalPages);
                        var listeClients = _econocomDataManager.GetListeClientsValide(i + 1, batchSize);

                        if (listeClients != null)
                        {
                            foreach (var client in listeClients)
                            {
                                var configSiClient = _econocomDataManager.GetConfigSiClient(client.Id);
                                if (configSiClient != null && configSiClient.ConfigSiId.HasValue)
                                {
                                    var configSi = configSiClient.ConfigSi;// _econocomDataManager.GetConfigSi(configSiClient.ConfigSiId.Value);
                                    //configSi.ConfigSiDevices = configSiClient.ConfigSi.ConfigSiDevices;
                                    try
                                    {
                                        var helper = new ChangementAnneeHelper();
                                        Logger.Debug("RecalculerQuantite  configsi {0} client {1}", configSi.Id, configSiClient.ClientId);
                                        configSi = helper.RecalculerQuantite(configSi, listeChangementAnneeCalendaire);
                                        //configSiClient.ConfigSi = configSi;
                                        Logger.Debug("Mise a jour configsi {0} pour client {1}", configSiClient.ConfigSiId, configSiClient.ClientId);
                                        SetConfigSiDevice(configSi);
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("configsi {0}, client {1} ", configSiClient.ConfigSiId, configSiClient.ClientId);
                                        LogguerErreur(e);
                                    }

                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return false;
            }
            return true;
        }

        public bool RecalculerConsommationPourNouvelleAnnee(int batchSize)
        {
            try
            {
                if (batchSize < 1)
                {
                    Logger.Debug("RecalculerConsommationPourNouvelleAnnee batchSize {0} ", batchSize);
                    return false;
                }

                var nombreClients = _econocomDataManager.GetNombreClientsValide();
                Logger.Debug("RecalculerConsommationPourNouvelleAnnee  nombreClients {0}", nombreClients);
                if (nombreClients < 1)
                {
                    Logger.Debug("RecalculerConsommationPourNouvelleAnnee: Pas de client valide");
                    return false;
                }

                if (nombreClients > 0)
                {
                    var pages = (double)nombreClients / batchSize;
                    var totalPages = (int)Math.Ceiling(pages);
                    Logger.Debug("RecalculerConsommationPourNouvelleAnnee pages {0} totalPages {1}", pages, totalPages);
                    for (int i = 0; i < totalPages; ++i)
                    {
                        Logger.Debug("RecalculerConsommationPourNouvelleAnnee page {0} / totalPages {1}", i + 1, totalPages);
                        var listeClients = _econocomDataManager.GetListeClientsValide(i + 1, batchSize);
                        if (listeClients != null)
                        {
                            foreach (var client in listeClients)
                            {
                                var resultatCalculateur = GetResultatCalculateur(client.Id, "Payant");
                                resultatCalculateur = ReCalculerEtMAJConfigSiConsommation(resultatCalculateur);
                                var resultat = MAJResultat(resultatCalculateur);
                                Logger.Debug("RecalculerConsommationPourNouvelleAnnee resultat {0} client {1}", resultat, client.Id);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return false;
            }
            return true;
        }

        public List<Econocom.Model.ViewModel.ContactViewModel> ListeContact(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPages)
        {
            try
            {
                var liste = _econocomDataManager.ListeContact(id, search, page, sort, sortdir, pageDimension, out totalPages);
                var listeContact = new List<Econocom.Model.ViewModel.ContactViewModel>();
                if (liste != null)
                {
                    foreach (var contact in liste)
                    {
                        var newContact = new Econocom.Model.ViewModel.ContactViewModel();
                        newContact.InjectFrom(contact);
                        if (contact.Reponses != null && contact.Reponses.Any())
                        {
                            newContact.Reponse = contact.Reponses.ElementAt(0).Valeur;
                            newContact.Question = contact.Reponses.ElementAt(0).Question.LibelleQuestion;
                        }
                        newContact.Profil = contact.TypeContact.LibelleTypeContact;
                        newContact.CodeInterneWattsGreen = contact.Client.CodeInterneWattsGreen;
                        newContact.DatePremiereConnexion = contact.DateDebut;
                        listeContact.Add(newContact);
                    }
                }
                return listeContact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            totalPages = 0;
            return null;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        private void LogguerErreur(Exception e)
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
                        if (e.InnerException.InnerException != null)
                        {
                            Logger.Error(e.InnerException.InnerException.Message);
                        }
                        else
                        {
                            Logger.Error(e.InnerException.Message);
                        }
                    }
                    else
                    {
                        Logger.Error(e.Message);
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
        /// Récupéré liste de devises
        /// </summary>
        /// <returns>Liste de devises</returns>
        public List<Devise> GetListeDevise()
        {
            try
            {
                var listeDevise = _econocomDataManager.GetListeDevise();
                foreach (var devise in listeDevise)
                {
                    if (devise.DeviseTraductions != null && devise.DeviseTraductions.ElementAt(0) != null)
                        devise.Libelle = devise.DeviseTraductions.ElementAt(0).Libelle;
                    else
                        devise.Libelle = devise.Libelle;
                }
                return listeDevise;
            }
            catch (Exception e)
            {
                Logger.Error("GetListeDevise : ", e.StackTrace);
                throw e;
            }
        }



        /// <summary>
        /// Récupérer compte evaluation par un idClient
        /// </summary>
        /// <param name="idClient">Identifiant du client</param>
        /// <returns>Objet CompteEvaluation</returns>
        public CompteEvaluation GetCompteEvaluation(int idClient)
        {
            try
            {
                var compteEvaluation = _econocomDataManager.GetCompteEvaluation(idClient);

                var devise = compteEvaluation.Devise;

                var idLangue = 1;
                var langue = _econocomDataManager.GetLangue();
                if (langue != null)
                {
                    idLangue = langue.Id;
                }

                if (devise != null)
                {
                    if (devise.DeviseTraductions != null)
                    {
                        var deviseTraductions = devise.DeviseTraductions.ToList();
                        var deviseTraduit = deviseTraductions.Find(o => o.LangueId == idLangue);
                        devise.Libelle = deviseTraduit != null ? deviseTraduit.Libelle : devise.Libelle;
                    }
                }

                return compteEvaluation;
            }
            catch (Exception e)
            {
                Logger.Error("GetCompteEvaluation : ", e.StackTrace);
                throw e;
            }
        }


        /// <summary>
        /// Récupérer details contact par idContact
        /// </summary>
        /// <param name="idContact">Identifiant du contact</param>
        /// <returns>Objet Contact</returns>
        /// <exception cref="Exception"></exception>
        public Contact GetDetailsContact(int idContact)
        {
            try
            {
                return _econocomDataManager.GetDetailsContact(idContact);
            }
            catch (Exception e)
            {
                Logger.Error("GetDetailsContact : ", e.StackTrace);
                throw e;
            }
        }

        public List<AgeDevice> GetListeAgeDevice()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeAgeDevice(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.AgeDeviceTraduction GetAgeDeviceTraduction(int idAgeDevice, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetAgeDeviceTraduction(idAgeDevice, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public AgeDeviceTraduction SetAgeDeviceTraduction(AgeDeviceTraduction ageDeviceTraduction)
        {
            try
            {
                return _econocomDataManager.SetAgeDeviceTraduction(ageDeviceTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<TypeUsage> GetListeTypeUsage()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeTypeUsage(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.TypeUsageTraduction GetTypeUsageTraduction(int idTypeUsage, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetTypeUsageTraduction(idTypeUsage, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public TypeUsageTraduction SetTypeUsageTraduction(TypeUsageTraduction typeUsageTraduction)
        {
            try
            {
                return _econocomDataManager.SetTypeUsageTraduction(typeUsageTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<CategorieDevice> GetListeCategorieDevice()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeCategorieDevice(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.CategorieDeviceTraduction GetCategorieDeviceTraduction(int idAgeDevice, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetCategorieDeviceTraduction(idAgeDevice, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public CategorieDeviceTraduction SetCategorieDeviceTraduction(CategorieDeviceTraduction categorieDeviceTraduction)
        {
            try
            {
                return _econocomDataManager.SetCategorieDeviceTraduction(categorieDeviceTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<ClasseDevice> GetListeClasseDevice()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeClasseDevice(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.ClasseDeviceTraduction GetClasseDeviceTraduction(int idClasseDevice, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetClasseDeviceTraduction(idClasseDevice, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public ClasseDeviceTraduction SetClasseDeviceTraduction(ClasseDeviceTraduction classeDeviceTraduction)
        {
            try
            {
                return _econocomDataManager.SetClasseDeviceTraduction(classeDeviceTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Equivalence> GetListeEquivalence()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeEquivalence(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.EquivalenceTraduction GetEquivalenceTraduction(int idEquivalence, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetEquivalenceTraduction(idEquivalence, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public EquivalenceTraduction SetEquivalenceTraduction(EquivalenceTraduction equivalenceTraduction)
        {
            try
            {
                return _econocomDataManager.SetEquivalenceTraduction(equivalenceTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<FamilleDevice> GetListeFamilleDevice()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeFamilleDevice(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.FamilleDeviceTraduction GetFamilleDeviceTraduction(int idFamilleDevice, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetFamilleDeviceTraduction(idFamilleDevice, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public FamilleDeviceTraduction SetFamilleDeviceTraduction(FamilleDeviceTraduction familleDeviceTraduction)
        {
            try
            {
                return _econocomDataManager.SetFamilleDeviceTraduction(familleDeviceTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Origine> GetListeOrigine()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeOrigines(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.OrigineTraduction GetOrigineTraduction(int idOrigine, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetOrigineTraduction(idOrigine, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public OrigineTraduction SetOrigineTraduction(OrigineTraduction origineTraduction)
        {
            try
            {
                return _econocomDataManager.SetOrigineTraduction(origineTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Pays> GetListePays()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListePays(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.PaysTraduction GetPaysTraduction(int idPays, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetPaysTraduction(idPays, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public PaysTraduction SetPaysTraduction(PaysTraduction paysTraduction)
        {
            try
            {
                return _econocomDataManager.SetPaysTraduction(paysTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Politique> GetListePolitique()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListePolitique(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.PolitiqueTraduction GetPolitiqueTraduction(int idPolitique, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetPolitiqueTraduction(idPolitique, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public PolitiqueTraduction SetPolitiqueTraduction(PolitiqueTraduction politiqueTraduction)
        {
            try
            {
                return _econocomDataManager.SetPolitiqueTraduction(politiqueTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Question> GetListeQuestion()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeQuestion(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.QuestionTraduction GetQuestionTraduction(int idQuestion, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetQuestionTraduction(idQuestion, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public QuestionTraduction SetQuestionTraduction(QuestionTraduction questionTraduction)
        {
            try
            {
                return _econocomDataManager.SetQuestionTraduction(questionTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.SecteurActiviteTraduction GetSecteurActiviteTraduction(int idSecteurActivite, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetSecteurActiviteTraduction(idSecteurActivite, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public SecteurActiviteTraduction SetSecteurActiviteTraduction(SecteurActiviteTraduction secteurActiviteTraduction)
        {
            try
            {
                return _econocomDataManager.SetSecteurActiviteTraduction(secteurActiviteTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Tarif> GetListeTarif()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeTarif(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.TarifTraduction GetTarifTraduction(int idTarif, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetTarifTraduction(idTarif, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public TarifTraduction SetTarifTraduction(TarifTraduction tarifTraduction)
        {
            try
            {
                return _econocomDataManager.SetTarifTraduction(tarifTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<TypeDevice> GetListeTypeDevice()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeTypeDevice(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.TypeDeviceTraduction GetTypeDeviceTraduction(int idTypeDevice, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetTypeDeviceTraduction(idTypeDevice, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public TypeDeviceTraduction SetTypeDeviceTraduction(TypeDeviceTraduction typeDeviceTraduction)
        {
            try
            {
                return _econocomDataManager.SetTypeDeviceTraduction(typeDeviceTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<TypeRatio> GetListeTypeRatio()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.ListeTypeRatio(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.TypeRatioTraduction GetTypeRatioTraduction(int idTypeRatio, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetTypeRatioTraduction(idTypeRatio, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public TypeRatioTraduction SetTypeRatioTraduction(TypeRatioTraduction typeRatioTraduction)
        {
            try
            {
                return _econocomDataManager.SetTypeRatioTraduction(typeRatioTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }



        public List<Devise> ListeDevise(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                return _econocomDataManager.ListeDevise(page, pageDimension, sort, sortdir, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                LogguerErreur(e);
                return null;
            }
        }


        public Devise GetDernierDevise()
        {
            try
            {
                return _econocomDataManager.GetDernierDevise();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Devise SauvegardeDevise(Devise devise)
        {
            try
            {
                return _econocomDataManager.SauvegardeDevise(devise);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Devise GetDevise(int id)
        {
            try
            {
                return _econocomDataManager.GetDevise(id);
            }
            catch (Exception e)
            {
                Logger.Error("devise id {0}", id);
                LogguerErreur(e);
                return null;
            }
        }

        public Devise MAJDevise(Devise devise)
        {
            try
            {
                return _econocomDataManager.MAJDevise(devise);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Client GetClientParIdClient(int idClient)
        {
            try
            {
                return _econocomDataManager.GetClientParIdClient(idClient);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public DetailsClient MAJDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                return _econocomDataManager.MAJDetailsClient(detailsClient);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Langue GetLangueParCulture(string culture)
        {
            try
            {
                return _econocomDataManager.GetLangueParCulture(culture);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public FamilleDevice GetFamilleParId(int idFamille)
        {
            try
            {
                return _econocomDataManager.GetFamilleParId(idFamille);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public CategorieDevice GetCategorieParId(int idCategorie)
        {
            try
            {
                return _econocomDataManager.GetCategorieParId(idCategorie);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Model.Models.Traduction.DeviseTraduction GetDeviseTraduction(int idDeviseTraduction, int idLangue)
        {
            try
            {
                return _econocomDataManager.GetDeviseTraduction(idDeviseTraduction, idLangue);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public DeviseTraduction SetDeviseTraduction(DeviseTraduction deviseTraduction)
        {
            try
            {
                return _econocomDataManager.SetDeviseTraduction(deviseTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Devise> GetDevises()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.GetDevises(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<SecteurActivite> GetSecteurActivites()
        {
            try
            {
                int totalPages = 0;
                return _econocomDataManager.GetSecteurActivites(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<string> GetListeEmailPasValide(int clientId)
        {
            try
            {
                return _econocomDataManager.GetListeEmailPasValide(clientId);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Question GetQuestionParDefaut()
        {
            try
            {
                var question = _econocomDataManager.GetQuestionParDefaut();

                if (question != null)
                {
                    if (question.QuestionTraductions != null && question.QuestionTraductions.ElementAt(0) != null)
                    {
                        question.LibelleQuestion = question.QuestionTraductions.ElementAt(0).LibelleQuestion;
                    }
                }

                return question;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Reponse SauvegardeReponse(Reponse reponse)
        {
            try
            {
                return _econocomDataManager.SauvegardeReponse(reponse);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        #region Import Parc

        public ImportECSchangeViewModel UploadDoc(DocumentUpload document, string typeClient)
        {
            var debut = ConsommationHelper.GetStartTime("UploadDoc start");
            var importViewModel = new ImportECSchangeViewModel();
            var infoClient = new InfoClientImportViewModel();
            var importDonneesClientListe = new List<ImportClientViewModel>();

            var equipement = new ImportClientViewModel();

            try
            {
                var stream = new MemoryStream(document.Data);
                var reader = new CsvReader(new StreamReader(stream, Encoding.UTF8));
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                decimal poidsTotal = 0;
                
                //List<ImportClientViewModel> importDonneesClientListe = reader.GetRecords<ImportClientViewModel>().ToList();
                while (reader.Read())
                {
                    var infoEquipement = ConstruireImportClientViewModel(reader, typeClient);

                    if(infoEquipement.Caracteristique != (int)CorrespondanceTypeDeviceCaracteristique.NonApplicable)
                        importDonneesClientListe.Add(infoEquipement);

                    if (typeClient != "WattsGreen" && infoEquipement.Caracteristique != (int)CorrespondanceTypeDeviceCaracteristique.NonApplicable)
                        poidsTotal = Convert.ToDecimal(poidsTotal + (infoEquipement.Poids * infoEquipement.QteDevice));

                    equipement = infoEquipement;
                }

                //Si caracteristique de tous les devices = 5
                if (importDonneesClientListe.Count == 0)
                {
                    if (equipement.IdClient != 0)
                    {
                        var client = _econocomDataManager.GetClientParId(equipement.IdClient);

                        infoClient.IdClient = equipement.IdClient.ToString();
                        infoClient.RaisonSociale = client.RaisonSociale;
                        importViewModel.InfoClientImportViewModel = infoClient;
                    }
                }

                importViewModel.ListeImportClientViewModel = importDonneesClientListe;

                importViewModel.PoidsTotal = poidsTotal;
                
                TransactionOptions transactionOptions = new TransactionOptions()
                    {
                        IsolationLevel = IsolationLevel.Serializable, // Use whatever level you require
                        Timeout = TransactionManager.MaximumTimeout
                    };

                using (var scope = new System.Transactions.TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    var start = DateTime.Now;
                    Logger.Info("{0} {1} ", "Transaction starts :", start.ToString());

                    var importDonnee = importDonneesClientListe.FirstOrDefault();

                    if (importDonnee != null)
                    {
                        var idClient = importDonneesClientListe.Any() ? importDonnee.IdClient : 0;
                        Logger.Debug("IdClient => " + idClient);

                        //check if idClient is valid
                        var client = _econocomDataManager.GetClientParId(idClient);

                        if (client == null)
                        {
                            Logger.Error("Client " + idClient + " n'existe pas");
                            importViewModel.ImportParcValide = false;
                            return importViewModel;
                        }
                        else if (client.SecteurActiviteId == null)
                        {
                            Logger.Error("Client " + idClient + " n'a pas encore finaliser sa souscription");
                            importViewModel.ImportParcValide = false;
                            return importViewModel;
                        }

                        infoClient.IdClient = client.Id.ToString();
                        infoClient.RaisonSociale = client.RaisonSociale;
                        importViewModel.InfoClientImportViewModel = infoClient;

                        //Validation données
                        foreach (var donneesClient in importDonneesClientListe)
                        {
                            if (donneesClient.CoefficientConversion == null)
                            {
                                donneesClient.CoefficientConversion = (client.DetailsClient != null && client.DetailsClient.CoefficientCO2 != 0) ? client.DetailsClient.CoefficientCO2 : client.Adresse.Pays.CoefficientConversion;
                            }

                            var valider = ValiderDonnees(donneesClient);

                            if (!valider)
                            {
                                importViewModel.ImportParcValide = false;
                                return importViewModel;
                            }
                        }

                        //get configsiclient actuel pour client et set to inactif
                        var configSiClientActuel = _econocomDataManager.GetConfigSiClient(client.Id);
                        if (configSiClientActuel != null)
                            Logger.Debug("configSiClientActuel : " + configSiClientActuel.Id);
                        else
                            Logger.Error("configSiClientActuel nulle");

                        configSiClientActuel = _econocomDataManager.MAJConfigSiClient(configSiClientActuel.Id);

                        //Création config benchmark
                        var calculateurViewModel = CreationDonneesBenchmark(client);

                        //Faire appel au calculateur
                        var calculator = new Calculator();
                        var debut2 = ConsommationHelper.GetStartTime("Calculateur : CalculerTotalConsommation start");
                        calculateurViewModel = calculator.CalculerTotalConsommation(calculateurViewModel);
                        var fin2 = ConsommationHelper.GetTimeDifference(debut2, "Calculateur : CalculerTotalConsommation end");

                        //Sort donnees client
                        if (typeClient != "WattsGreen")
                            importDonneesClientListe = SortDonnees(importDonneesClientListe);

                        foreach (var donneesClient in importDonneesClientListe)
                        {
                            //Créer configSiDevice + ConfigSiConsommation ou MAJ
                            CreerOuMAJConfig(donneesClient, calculateurViewModel, client);
                        }

                        //Faire appel au calculateur
                        var debut3 = ConsommationHelper.GetStartTime("Calculateur : CalculerTotalConsommation start");
                        calculateurViewModel = calculator.CalculerTotalConsommation(calculateurViewModel);
                        var fin3 = ConsommationHelper.GetTimeDifference(debut3, "Calculateur : CalculerTotalConsommation end");

                        //construire objet pour enregistrer
                        var configSiClient = ConstruireConfigSiClientPourEnregistrer(calculateurViewModel);

                        //sauvegarde configSiClient
                        configSiClient = _econocomDataManager.SauvegardeConfigSiClient(configSiClient);

                        var configSiDeviceListe = configSiClient.ConfigSi.ConfigSiDevices;
                        
                        //Creation Resultat
                        foreach (var configSiDevice in configSiDeviceListe)
                        {
                            var resultat = new Resultat();
                            resultat.CoefficientConversion = (client.DetailsClient != null && client.DetailsClient.CoefficientCO2 != 0) ? client.DetailsClient.CoefficientCO2 : client.Adresse.Pays.CoefficientConversion;
                            resultat.ConfigSiId = configSiDevice.ConfigSiId;
                            resultat.ConfigSiDeviceId = configSiDevice.Id;
                            resultat.ConsommationCo2 = configSiDevice.ConfigSiConsommations.Aggregate(0, (current, configSiConsommation) => (int)(configSiConsommation.ConsommationCo2 + current));
                            resultat.ConsommationWatt = configSiDevice.ConfigSiConsommations.Aggregate(0, (current, configSiConsommation) => (int)(configSiConsommation.ConsommationWatt + current));

                            resultat = _econocomDataManager.SauvegardeResultat(resultat);
                        }
                    }

                    var end = DateTime.Now;
                    Logger.Info("{0} {1} ", "Transaction ends: ", end.ToString());

                    var difference = end - start;
                    Logger.Info("{0} {1}", "Transaction time taken: ", difference.Duration().ToString());

                    scope.Complete();
                }

                var fin = ConsommationHelper.GetTimeDifference(debut, "Upload Doc end");

                importViewModel.ImportParcValide = true;
                return importViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public ImportClientViewModel ConstruireImportClientViewModel(CsvReader reader, string typeClient)
        {
            try
            {
                var importClientViewModel = new ImportClientViewModel();

                if (reader.GetField(0) != "")
                {
                    importClientViewModel.IdClient = Convert.ToInt32(reader.GetField(0));
                }

                if (reader.GetField(1) != "")
                {
                    importClientViewModel.TypeDevice = Convert.ToInt32(reader.GetField(1));
                }
                
                importClientViewModel.LibelleDevice = reader.GetField(2);

                if (reader.GetField(3) != "")
                {
                    importClientViewModel.TypeUsageId = Convert.ToInt32(reader.GetField(3));
                }

                if (reader.GetField(4) != "")
                {
                    importClientViewModel.NbrJourTravaille = Convert.ToDecimal(reader.GetField(4));
                }

                if (reader.GetField(5) != "")
                {
                    importClientViewModel.NbrHeuresIntensifJour = Convert.ToDecimal(reader.GetField(5));
                }

                if (reader.GetField(6) != "")
                {
                    importClientViewModel.NbrHeuresNonIntensifJour = Convert.ToDecimal(reader.GetField(6));
                }

                if (reader.GetField(7) != "")
                {
                    importClientViewModel.NbrHeuresOffJour = Convert.ToDecimal(reader.GetField(7));
                }

                if (reader.GetField(8) != "")
                {
                    importClientViewModel.UtiliseEnergieExterne = Convert.ToBoolean(reader.GetField(8));
                }

                if (reader.GetField(9) != "")
                {
                    importClientViewModel.CoeffNonIntensif = Convert.ToDecimal(reader.GetField(9));
                }

                if (reader.GetField(10) != "")
                {
                    importClientViewModel.CoeffModeOff = Convert.ToDecimal(reader.GetField(10));
                }

                if (reader.GetField(11) != "")
                {
                    importClientViewModel.CoeffReductionWattExterne = Convert.ToDecimal(reader.GetField(11));
                }

                if (reader.GetField(12) != "")
                {
                    importClientViewModel.PourcentageConsoExterne = Convert.ToDecimal(reader.GetField(12));
                }

                if (reader.GetField(13) != "")
                {
                    importClientViewModel.ClasseDevice = Convert.ToInt32(reader.GetField(13));
                }

                if (reader.GetField(14) != "")
                {
                    importClientViewModel.AgeDevice = Convert.ToInt32(reader.GetField(14));
                }

                if (reader.GetField(15) != "")
                {
                    importClientViewModel.QteDevice = Convert.ToInt32(reader.GetField(15));
                }

                if (reader.GetField(16) != "")
                {
                    importClientViewModel.ConsommationIntensif = Convert.ToDecimal(reader.GetField(16));
                }

                if (reader.GetField(17) != "")
                {
                    importClientViewModel.ConsommationNonIntensif = Convert.ToDecimal(reader.GetField(17));
                }

                if (reader.GetField(18) != "")
                {
                    importClientViewModel.ConsommationWatt = Convert.ToDecimal(reader.GetField(18));
                }

                if (reader.GetField(19) != "")
                {
                    importClientViewModel.ConsommationCo2 = Convert.ToDecimal(reader.GetField(19));
                }

                if (reader.GetField(20) != "")
                {
                    importClientViewModel.CoefficientConversion = Convert.ToDecimal(reader.GetField(20));
                }

                if (reader.GetField(21) != "")
                {
                    importClientViewModel.ConsommationIntensifUnitaire = Convert.ToDecimal(reader.GetField(21));
                }

                if (typeClient != "WattsGreen")
                {
                    if (reader.GetField(22) != "")
                    {
                        importClientViewModel.Poids = Convert.ToDecimal(reader.GetField(22));
                    }

                    if (reader.GetField(23) != "")
                    {
                        importClientViewModel.NewPV = Convert.ToDecimal(reader.GetField(23));
                    }

                    if (reader.GetField(24) != "")
                    {
                        importClientViewModel.Caracteristique = Convert.ToInt32(reader.GetField(24));
                    }
                }

                return importClientViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public List<ImportClientViewModel> SortDonnees(List<ImportClientViewModel> importDonneesClientListe)
        {
            var importListe = new List<ImportClientViewModel>();

            try
            {
                var typeDevices = _econocomDataManager.GetTypeDevices();
                var classes = ListeClasseDevice();
                var ages = ListeAgeDevice();

                foreach (var typeDevice in typeDevices)
                {
                    foreach (var classe in classes)
                    {
                        foreach (var age in ages)
                        {
                            // find all records in list with same typedeviceid, classedeviceid, agedeviceid
                            var liste = importDonneesClientListe.FindAll(o => o.TypeDevice == typeDevice.Id && o.ClasseDevice == classe.Id && o.AgeDevice == age.Id);

                            var totale = 0;

                            if (liste.Any())
                            {
                                //sum up all their quantities
                                totale = liste.Aggregate(0, (current, x) => x.QteDevice + current);

                                //create single record
                                var donneeImport = new ImportClientViewModel();
                                donneeImport.TypeDevice = typeDevice.Id;
                                donneeImport.ClasseDevice = classe.Id;
                                donneeImport.AgeDevice = age.Id;
                                donneeImport.QteDevice = totale;
                                donneeImport.LibelleDevice = "";
                                donneeImport.IdClient = importDonneesClientListe.FirstOrDefault().IdClient;
                                importListe.Add(donneeImport);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }

            return importListe;
        }


        public CalculateurViewModel CreerOuMAJConfig(ImportClientViewModel donneesClient, CalculateurViewModel calculateurViewModel, Client client)
        {
            var debut = ConsommationHelper.GetStartTime("CreerOuMAJConfig start");

            try
            {
                foreach (var familleDevice in calculateurViewModel.FamilleDeviceViewModels)
                {
                    foreach (var categorieDevice in familleDevice.CategorieDevices)
                    {
                        foreach (var typeDevice in categorieDevice.TypeDevices)
                        {
                            if (donneesClient.TypeDevice == typeDevice.Id)
                            {
                                //Si libellé device nulle -> maj copy config benchmark
                                if (donneesClient.LibelleDevice == "")
                                {
                                    var configSiDevice = typeDevice.ConfigSiDevice;

                                    var typeDeviceTrad = _econocomDataManager.GetTypeDeviceTraduction(donneesClient.TypeDevice, 1); //to be checked
                                    donneesClient.LibelleDevice = typeDeviceTrad.LibelleTypeDevice;

                                    var usage = new Usage();
                                    if (donneesClient.TypeUsageId != null)
                                    {
                                        //Récupérer les données d'usage
                                        usage = _econocomDataManager.GetUsageParTypeUsageId((int)donneesClient.TypeUsageId);
                                    }
                                    else
                                    {
                                        usage = null;
                                    }

                                    NomaliseConfigSiDevice(configSiDevice, donneesClient, usage);

                                    foreach (var configSiConsommation in typeDevice.ConfigSiDevice.ConfigSiConsommations)
                                    {
                                        if (donneesClient.ClasseDevice == configSiConsommation.ClasseDeviceId && donneesClient.AgeDevice == configSiConsommation.AgeDeviceId)
                                        {
                                            NormaliseConfigSiConsommation(configSiConsommation, donneesClient);
                                        }
                                    }

                                    typeDevice.ConfigSiDevice.Quantite = typeDevice.ConfigSiDevice.ConfigSiConsommations.Aggregate(0, (current, configSiConsommation) => configSiConsommation.Quantite + current);

                                    return calculateurViewModel;
                                }
                                else
                                {
                                    //Get typeDevice traduction
                                    var typeDeviceTrad = _econocomDataManager.GetTypeDeviceTraduction(donneesClient.TypeDevice, 1);

                                    var libelleDevice = typeDeviceTrad.LibelleTypeDevice + "_" + donneesClient.LibelleDevice;

                                    //Vérifier s'il y a un device existant avec ce libellé
                                    foreach (var device in categorieDevice.TypeDevices)
                                    {
                                        if (donneesClient.TypeDevice == device.Id && libelleDevice == device.ConfigSiDevice.LibelleDevice)
                                        {
                                            //MAJ configsiconsommation
                                            foreach (var configSiConso in device.ConfigSiDevice.ConfigSiConsommations)
                                            {
                                                if (configSiConso.ClasseDeviceId == donneesClient.ClasseDevice && configSiConso.AgeDeviceId == donneesClient.AgeDevice)
                                                {
                                                    NormaliseConfigSiConsommation(configSiConso, donneesClient);
                                                }
                                            }
                                            device.ConfigSiDevice.Quantite = device.ConfigSiDevice.ConfigSiConsommations.Aggregate(0, (current, configSiConsommation) => configSiConsommation.Quantite + current);

                                            return calculateurViewModel;
                                        }
                                    }

                                    //Device avec ce libellé n'existe pas, créer nouveau
                                    var configSiDevice = CreationConfigSiDevice(donneesClient, calculateurViewModel);
                                    calculateurViewModel = ConstruireCalculateurViewModel(calculateurViewModel, client, configSiDevice);
                                    return calculateurViewModel;
                                }
                            }
                        }
                    }
                }

                return calculateurViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public CalculateurViewModel CreationDonneesBenchmark(Client client)
        {
            try
            {
                var debut1 = ConsommationHelper.GetStartTime("CreationDonneesBenchmark start");

                var calculateurViewModel = new CalculateurViewModel();

                //Creation ConfigSi
                var configSi = new ConfigSi();
                configSi.LibelleConfigSi = "ConfigSi" + client.Id;
                configSi.DateDebut = DateTime.Now;
                configSi.Statut = (int)ConfigSiStatut.Creer;
                calculateurViewModel.ConfigSi = configSi;

                //Creation ConfigSiClient
                var configSiClient = new ConfigSiClient();
                configSiClient.ClientId = client.Id;
                configSiClient.DateDebut = DateTime.Now;

                var origine = _econocomDataManager.GetOrigineParLibelle(OrigineEnum.Actuel.ToString());
                configSiClient.OrigineId = origine.Id;

                configSiClient.ConfigSi = configSi;
                calculateurViewModel.ConfigSiClient = configSiClient;

                //Obtenir liste FamilleDevice par SecteurActivite du client payant
                var listefamilleDevice = _econocomDataManager.GetFamilleDeviceParSecteurActivite(client.SecteurActivite.Id);

                //Construire FamilleDevice Liste
                calculateurViewModel = ConstruireFamilleDeviceListe(calculateurViewModel, listefamilleDevice, client.Id, client.SecteurActiviteId.Value, "Import");

                calculateurViewModel.Effectif = client.DetailsClient.Effectif;
                calculateurViewModel.PrixKWatt = client.DetailsClient.PrixKWH;
                calculateurViewModel.CoefficientConversion = (client.DetailsClient != null && client.DetailsClient.CoefficientCO2 != 0) ? client.DetailsClient.CoefficientCO2 : client.Adresse.Pays.CoefficientConversion;
                calculateurViewModel.TauxEquipement = client.DetailsClient.TauxEquipement;
                calculateurViewModel.SuperficieDataCenter = client.DetailsClient.SuperficieDataCenter;
                calculateurViewModel.NombreSite = client.DetailsClient.NombreSite;

                var fin1 = ConsommationHelper.GetTimeDifference(debut1, "CreationDonneesBenchmark end");

                return calculateurViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
            return null;
        }
        

        public bool ValiderDonnees(ImportClientViewModel donneesClient)
        {
            try
            {
                //Vérifier si IdTypeDevice existe
                var typeDevice = _econocomDataManager.GetTypeDeviceParId(donneesClient.TypeDevice);
                Logger.Debug("ValiderDonnees : TypeDeviceId => " + donneesClient.TypeDevice);

                if (typeDevice == null)
                {
                    Logger.Error("TypeDevice " + donneesClient.TypeDevice + " n'existe pas");
                }

                //Vérifier si IdClasse existe
                var classe = _econocomDataManager.GetClasseParId(donneesClient.ClasseDevice);
                Logger.Debug("ValiderDonnees : ClasseDeviceId => " + donneesClient.ClasseDevice);

                if (classe == null)
                {
                    Logger.Error("Classe " + donneesClient.ClasseDevice + " n'existe pas");
                }

                //Vérifier si IdAge existe
                var age = _econocomDataManager.GetAgeParId(donneesClient.AgeDevice);
                Logger.Debug("ValiderDonnees : AgeDeviceId => " + donneesClient.AgeDevice);

                if (age == null)
                {
                    Logger.Error("Age " + donneesClient.AgeDevice + " n'existe pas");
                }

                if (typeDevice != null && classe != null && age != null)
                {
                    //Si IdTypeUsage est renseigné, vérifier s'il existe
                    if (donneesClient.TypeUsageId != null)
                    {
                        var typeUsage = _econocomDataManager.GetTypeUsage(donneesClient.TypeUsageId.Value);
                    }

                    return true;
                }

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
            return false;
        }


        public ConfigSiDevice NomaliseConfigSiDevice(ConfigSiDevice configSiDevice, ImportClientViewModel donneesClient, Usage usage)
        {
            try
            {
                if (donneesClient.NbrJourTravaille != null)
                {
                    configSiDevice.NbrJourTravaille = donneesClient.NbrJourTravaille.Value;
                }
                else if (usage != null && usage.NbrJourTravaille != null)
                {
                    configSiDevice.NbrJourTravaille = usage.NbrJourTravaille;
                }

                if (donneesClient.NbrHeuresIntensifJour != null)
                {
                    configSiDevice.NbrHeuresIntensifJour = donneesClient.NbrHeuresIntensifJour.Value;
                }
                else if (usage != null && usage.NbrHeuresIntensifJour != null)
                {
                    configSiDevice.NbrHeuresIntensifJour = usage.NbrHeuresIntensifJour;
                }

                if (donneesClient.NbrHeuresNonIntensifJour != null)
                {
                    configSiDevice.NbrHeuresNonIntensifJour = donneesClient.NbrHeuresNonIntensifJour.Value;
                }
                else if (usage != null && usage.NbrHeuresNonIntensifJour != null)
                {
                    configSiDevice.NbrHeuresNonIntensifJour = usage.NbrHeuresNonIntensifJour;
                }

                if (donneesClient.NbrHeuresOffJour != null)
                {
                    configSiDevice.NbrHeuresOffJour = donneesClient.NbrHeuresOffJour.Value;
                }
                else if (usage != null && usage.NbrHeuresOffJour != null)
                {
                    configSiDevice.NbrHeuresOffJour = usage.NbrHeuresOffJour;
                }

                if (donneesClient.UtiliseEnergieExterne != null)
                {
                    configSiDevice.UtiliseEnergieExterne = donneesClient.UtiliseEnergieExterne.Value;
                }
                else if (usage != null && usage.UtiliseEnergieExterne != null)
                {
                    configSiDevice.UtiliseEnergieExterne = usage.UtiliseEnergieExterne;
                }

                if (donneesClient.CoeffNonIntensif != null)
                {
                    configSiDevice.CoeffNonIntensif = donneesClient.CoeffNonIntensif.Value;
                }
                else if (usage != null && usage.CoeffNonIntensif != null)
                {
                    configSiDevice.CoeffNonIntensif = usage.CoeffNonIntensif.Value;
                }

                if (donneesClient.CoeffModeOff != null)
                {
                    configSiDevice.CoeffModeOff = donneesClient.CoeffModeOff;
                }
                else if (usage != null && usage.CoeffModeOff != null)
                {
                    configSiDevice.CoeffModeOff = usage.CoeffModeOff;
                }

                if (donneesClient.CoeffReductionWattExterne != null)
                {
                    configSiDevice.CoeffReductionWattExterne = donneesClient.CoeffReductionWattExterne.Value;
                }
                else if (usage != null && usage.CoeffReductionWattExterne != null)
                {
                    configSiDevice.CoeffReductionWattExterne = usage.CoeffReductionWattExterne.Value;
                }

                if (donneesClient.PourcentageConsoExterne != null)
                {
                    configSiDevice.PourcentConsomExtern = donneesClient.PourcentageConsoExterne.Value;
                }
                else if (usage != null && usage.PourcentageConsoExterne != null)
                {
                    configSiDevice.PourcentConsomExtern = usage.PourcentageConsoExterne.Value;
                }

                configSiDevice.DateModification = DateTime.Now;

                return configSiDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }
        

        public ConfigSiConsommation NormaliseConfigSiConsommation(ConfigSiConsommation configSiConsommation, ImportClientViewModel donneesClient)
        {
            try
            {
                if (donneesClient.ConsommationIntensif != null)
                {
                    configSiConsommation.ConsommationIntensif = donneesClient.ConsommationIntensif.Value;
                }

                if (donneesClient.ConsommationNonIntensif != null)
                {
                    configSiConsommation.ConsommationNonIntensif = donneesClient.ConsommationNonIntensif.Value;
                }

                if (donneesClient.ConsommationIntensifUnitaire != null)
                {
                    configSiConsommation.ConsommationIntensifUnitaire = donneesClient.ConsommationIntensifUnitaire.Value;
                }

                configSiConsommation.Quantite = donneesClient.QteDevice;

                if (donneesClient.ConsommationWatt != null)
                {
                    configSiConsommation.ConsommationWatt = donneesClient.ConsommationWatt;
                }

                if (donneesClient.ConsommationCo2 != null)
                {
                    configSiConsommation.ConsommationCo2 = donneesClient.ConsommationCo2;
                }

                configSiConsommation.IsModified = true;

                return configSiConsommation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public ConfigSiDevice CreationConfigSiDevice(ImportClientViewModel donneesClient, CalculateurViewModel calculateurViewModel)
        {
            try
            {
                //create new
                var usage = new Usage();

                var configSiDevice = new ConfigSiDevice();

                if (donneesClient.TypeUsageId != null)
                {
                    //Récupérer les données d'usage par IdTypeUsage
                    usage = _econocomDataManager.GetUsageParTypeUsageId((int)donneesClient.TypeUsageId);

                    NomaliseConfigSiDevice(configSiDevice, donneesClient, usage);
                    configSiDevice.TypeUsageId = donneesClient.TypeUsageId;
                }
                else
                {
                    //Get usage benchmark
                    var usageBenchmark = _econocomDataManager.GetUsageParTypeDevice(donneesClient.TypeDevice);

                    //Remplacer les valeurs non renseignées par les valeurs benchmark
                    NomaliseConfigSiDevice(configSiDevice, donneesClient, usageBenchmark);
                }

                configSiDevice.TypeDeviceId = donneesClient.TypeDevice;
                configSiDevice.Source = 1;
                configSiDevice.DateDebut = DateTime.Now;
                configSiDevice.DateFin = null;
                configSiDevice.DateModification = null;

                //Get typeDevice traduction
                var typeDevice = _econocomDataManager.GetTypeDeviceTraduction(donneesClient.TypeDevice, 1);
                configSiDevice.LibelleDevice = typeDevice.LibelleTypeDevice + "_" + donneesClient.LibelleDevice;

                var configSiConsommations = CreationConfigSiConsommations(donneesClient, configSiDevice);
                configSiDevice.ConfigSiConsommations = configSiConsommations;

                configSiDevice.Quantite = configSiConsommations.Aggregate(0, (current, configSiConsommation) => configSiConsommation.Quantite + current);

                return configSiDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
            return null;
        }


        public List<ConfigSiConsommation> CreationConfigSiConsommations(ImportClientViewModel donneesClient, ConfigSiDevice configSiDevice)
        {
            var configSiConsommations = new List<ConfigSiConsommation>();
            try
            {
                var classes = ListeClasseDevice();
                var ages = ListeAgeDevice();

                foreach (var classe in classes)
                {
                    foreach (var ageDevice in ages)
                    {
                        var configSiConsommation = new ConfigSiConsommation();
                        configSiConsommation.AgeDeviceId = ageDevice.Id;
                        configSiConsommation.ClasseDeviceId = classe.Id;

                        var consowattHeur = _econocomDataManager.GetConsoWattHeurParClasseAge(ageDevice.Id, classe.Id, configSiDevice.TypeDeviceId);

                        if (donneesClient.AgeDevice == ageDevice.Id && donneesClient.ClasseDevice == classe.Id)
                        {
                            configSiConsommation.ConsommationIntensif = donneesClient.ConsommationIntensif != null ? donneesClient.ConsommationIntensif.Value : 0;
                            configSiConsommation.ConsommationNonIntensif = donneesClient.ConsommationNonIntensif != null ? donneesClient.ConsommationNonIntensif.Value : 0;
                            configSiConsommation.ConsommationIntensifUnitaire = donneesClient.ConsommationIntensifUnitaire != null ? donneesClient.ConsommationIntensifUnitaire.Value : consowattHeur.Consommation;
                            configSiConsommation.Quantite = donneesClient.QteDevice;
                        }
                        else
                        {
                            configSiConsommation.ConsommationIntensif = 0;
                            configSiConsommation.ConsommationNonIntensif = 0;
                            configSiConsommation.ConsommationIntensifUnitaire = consowattHeur.Consommation;
                            configSiConsommation.Quantite = 0;
                            configSiConsommation.IsModified = true;
                        }
                        configSiConsommation.DateModification = DateTime.Now;

                        configSiConsommations.Add(configSiConsommation);
                    }
                }
                return configSiConsommations;
            }
            catch (Exception e)
            {
                throw e;
            }
            return null;
        }


        public CalculateurViewModel ConstruireCalculateurViewModel(CalculateurViewModel calculateurViewModel, Client client, ConfigSiDevice configSiDevice)
        {
            try
            {
                var secteurActiviteId = client.SecteurActiviteId;
                var listeTypeDeviceViewModel = new List<TypeDeviceViewModel>();

                foreach (var familleDevice in calculateurViewModel.FamilleDeviceViewModels)
                {
                    foreach (var categorieDevice in familleDevice.CategorieDevices)
                    {
                        var categorieDeviceId = _econocomDataManager.GetTypeDeviceParId(configSiDevice.TypeDeviceId).CategorieDeviceId;

                        if (categorieDeviceId == categorieDevice.Id)
                        {
                            listeTypeDeviceViewModel = categorieDevice.TypeDevices;

                            //Conversion typeDevice -> typeDeviceViewModel
                            var typeDeviceViewModel = new TypeDeviceViewModel();
                            typeDeviceViewModel = TypeDeviceViewModel(configSiDevice, typeDeviceViewModel);

                            // Obtenir ratio pour chaque TypeDevice
                            Ratio ratio = _econocomDataManager.GetRatio((int)secteurActiviteId, configSiDevice.TypeDeviceId);
                            var ratioViewModel = RatioViewModel(ratio);

                            // Obtenir typeRatio
                            TypeRatio typeRatio = ratio.TypeRatio;
                            var typeRatioViewModel = TypeRatioViewModel(typeRatio);
                            ratioViewModel.TypeRatioViewModel = typeRatioViewModel;

                            // Obtenir Ventilation Classe Device pour chaque TypeDevice
                            var ventilationClasseDeviceViewModels = VentilationClasseDeviceViewModels((int)secteurActiviteId, configSiDevice);

                            // Obtenir Ventilation Classe Age Device pour chaque TypeDevice
                            var ventilationClasseAgeDeviceViewModels = VentilationClasseAgeDeviceViewModels((int)secteurActiviteId, configSiDevice);

                            // Obtenir Usage pour chaque TypeDevice
                            var usageViewModel = DetaillesUsageViewModel(configSiDevice);

                            // Obtenir typeUsage
                            Usage usage = _econocomDataManager.GetUsageParTypeDevice(configSiDevice.TypeDeviceId);
                            TypeUsage typeUsage = usage.TypeUsage;
                            var typeUsageViewModel = TypeUsageViewModel(typeUsage);
                            usageViewModel.TypeUsage = typeUsageViewModel;

                            // Obtenir ConsoWattHeur pour chaque TypeDevice
                            var consoWattHeurViewModels = ConsoWattHeurViewModels(configSiDevice);

                            typeDeviceViewModel.Ratio = ratioViewModel;
                            typeDeviceViewModel.VentilationClasseDevices = ventilationClasseDeviceViewModels;
                            typeDeviceViewModel.VentilationClasseAgeDevices = ventilationClasseAgeDeviceViewModels;
                            typeDeviceViewModel.Usage = usageViewModel;
                            typeDeviceViewModel.ConsoWattHeurs = consoWattHeurViewModels;
                            typeDeviceViewModel.IsModified = true;

                            //ConfigSiDevice
                            typeDeviceViewModel.ConfigSiDevice = configSiDevice;
                            listeTypeDeviceViewModel.Add(typeDeviceViewModel);

                            categorieDevice.TypeDevices = listeTypeDeviceViewModel;
                        }
                    }
                }
                return calculateurViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public ConfigSiClient ConstruireConfigSiClientPourEnregistrer(CalculateurViewModel calculateurViewModel)
        {
            try
            {
                var configSiDeviceListe = (from familleDevice in calculateurViewModel.FamilleDeviceViewModels from categorieDevice in familleDevice.CategorieDevices from typeDevice in categorieDevice.TypeDevices select typeDevice.ConfigSiDevice).ToList();

                calculateurViewModel.ConfigSiClient.ConfigSi.ConfigSiDevices = configSiDeviceListe;

                return calculateurViewModel.ConfigSiClient;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
            return null;
        }


        public ConfigSiClient MAJConfigSiClient(int idConfigSiClient)
        {
            try
            {
                return _econocomDataManager.MAJConfigSiClient(idConfigSiClient);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        #endregion


        #region Import Client ECSchange

        public ImportECSchangeViewModel UploadDocECSchange(DocumentUpload document)
        {
            Logger.Info("Default TimeOut: " +TransactionManager.DefaultTimeout);
            Logger.Info("Maximum TimeOut: " + TransactionManager.MaximumTimeout);

            var debut = ConsommationHelper.GetStartTime("UploadDocECSchange start");

            var importECSchangeViewModel = new ImportECSchangeViewModel();
            var listeEquipement = new List<EquipementImportViewModel>();

            try
            {
                var stream = new MemoryStream(document.Data);
                var reader = new CsvReader(new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")));
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                //Lire première ligne - client info
                reader.Read();
                var infoClient = ConstruireInfoClientViewModel(reader);
                
                //Validation des données client
                var erreur1 = ValiderDonneesClientImport(infoClient);
                if(erreur1.Length > 1)
                    erreur1 = erreur1.Remove(erreur1.Length - 1);
                infoClient.Erreur = erreur1;
                if (!erreur1.Equals(""))
                {
                    importECSchangeViewModel.InfoClientEnErreur = true;
                }

                //Lire les autres ligne - info équipement
                while (reader.Read())
                {
                    var infoEquipement = ConstruireEquipementInfoViewModel(reader);
                    var erreur2 = ValiderDonneesEquipementImport(infoEquipement);
                    infoEquipement = ValidationDonnesCorrespondance(infoEquipement, infoClient.TypeSociete);
                    infoEquipement.Erreur = erreur2 + infoEquipement.Erreur;
                    infoEquipement.Erreur = infoEquipement.Erreur.Length > 1 ? infoEquipement.Erreur.Remove(infoEquipement.Erreur.Length - 1) : infoEquipement.Erreur;
                    if (!infoEquipement.Erreur.Equals(""))
                    {
                        importECSchangeViewModel.InfoEquipementEnErreur = true;
                    }

                    listeEquipement.Add(infoEquipement);
                }
                
                importECSchangeViewModel.InfoClientImportViewModel = infoClient;
                importECSchangeViewModel.ListeEquipementImportViewModel = listeEquipement;

                var fin = ConsommationHelper.GetTimeDifference(debut, "UploadDocECSchange end");

                return importECSchangeViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }
        

        public InfoClientImportViewModel ConstruireInfoClientViewModel(CsvReader reader)
        {
            try
            {
                var infoClientViewModel = new InfoClientImportViewModel();

                infoClientViewModel.IdClient = reader.GetField(0);
                infoClientViewModel.RaisonSociale = reader.GetField(1);

                if (reader.GetField(2) != "")
                {
                    infoClientViewModel.TypeSociete = Convert.ToInt32(reader.GetField(2));
                }

                infoClientViewModel.Groupe = reader.GetField(3);
                infoClientViewModel.Adresse = reader.GetField(4);
                infoClientViewModel.CodePostal = reader.GetField(5);
                infoClientViewModel.Ville = reader.GetField(6);
                infoClientViewModel.CodePays = reader.GetField(7);

                if (reader.GetField(8) != "")
                {
                    infoClientViewModel.ChiffreAffaire = Convert.ToInt32(reader.GetField(8));
                }
                
                infoClientViewModel.CodeSecteurActivite = reader.GetField(9);
                
                if (reader.GetField(10) != "")
                {
                    infoClientViewModel.NombreSites = Convert.ToInt32(reader.GetField(10));
                }
                
                if (reader.GetField(11) != "")
                {
                    infoClientViewModel.PrixMoyenKWH = Convert.ToDecimal(reader.GetField(11));
                }

                if (reader.GetField(12) != "")
                {
                    infoClientViewModel.CoefficientCO2 = Convert.ToDecimal(reader.GetField(12));
                }

                if (reader.GetField(13) != "")
                {
                    infoClientViewModel.Effectifs = Convert.ToInt32(reader.GetField(13));
                }

                if (reader.GetField(14) != "")
                {
                    infoClientViewModel.TauxEquipement = Convert.ToDecimal(reader.GetField(14));
                }

                infoClientViewModel.CodeDevise = reader.GetField(15);
                
                if (reader.GetField(16) != "")
                {
                    infoClientViewModel.NomAdministrateur = reader.GetField(16);
                }

                if (reader.GetField(17) != "")
                {
                    infoClientViewModel.PrenomAdministrateur = reader.GetField(17);
                }

                if (reader.GetField(18) != "")
                {
                    infoClientViewModel.AdresseMailAdministrateur = reader.GetField(18);
                }

                if (reader.GetField(19) != "")
                {
                    infoClientViewModel.CodeLangue = reader.GetField(19);
                }

                return infoClientViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public EquipementImportViewModel ConstruireEquipementInfoViewModel(CsvReader reader)
        {
            try
            {
                var infoEquipementViewModel = new EquipementImportViewModel();

                infoEquipementViewModel.CodeCategorie = reader.GetField(0);
                infoEquipementViewModel.LibelleCategorie = reader.GetField(1);
                infoEquipementViewModel.CodeFamille = reader.GetField(2);
                infoEquipementViewModel.LibelleFamille = reader.GetField(3);
                infoEquipementViewModel.CodeSousFamille = reader.GetField(4);
                infoEquipementViewModel.LibelleSousFamille = reader.GetField(5);
                infoEquipementViewModel.RefECS = reader.GetField(6);
                infoEquipementViewModel.LibelleRefECS = reader.GetField(7);
                infoEquipementViewModel.LibelleMarque = reader.GetField(8);
                infoEquipementViewModel.LibelleGamme = reader.GetField(9);
                infoEquipementViewModel.LibelleModele = reader.GetField(10);
                infoEquipementViewModel.NbProcesseurs = reader.GetField(11);
                infoEquipementViewModel.CodeProcesseur = reader.GetField(12);
                infoEquipementViewModel.LibelleCodeProcesseur = reader.GetField(13);
                infoEquipementViewModel.LibelleTypeProcesseur = reader.GetField(14);
                infoEquipementViewModel.Frequence = reader.GetField(15);
                infoEquipementViewModel.UniteFrequence = reader.GetField(16);
                infoEquipementViewModel.LibelleUniteFrequence = reader.GetField(17);
                infoEquipementViewModel.Memoire = reader.GetField(18);
                infoEquipementViewModel.UniteMemoire = reader.GetField(19);
                infoEquipementViewModel.LibelleUniteMemoire = reader.GetField(20);

                if (reader.GetField(21) != "")
                {
                    infoEquipementViewModel.Taille = Convert.ToDecimal(reader.GetField(21));
                }
               
                infoEquipementViewModel.UniteTaille = reader.GetField(22);
                
                if (reader.GetField(23) != "")
                {
                    infoEquipementViewModel.Vitesse = Convert.ToDecimal(reader.GetField(23));
                }
                
                infoEquipementViewModel.UniteVitesse = reader.GetField(24);
                infoEquipementViewModel.LibelleVitesse = reader.GetField(25);
                infoEquipementViewModel.Couleur = reader.GetField(26);
                infoEquipementViewModel.RectoVerso = reader.GetField(27);
                infoEquipementViewModel.NormeGraphique = reader.GetField(28);
                infoEquipementViewModel.CodeResolution = reader.GetField(29);
                infoEquipementViewModel.LibelleResolution = reader.GetField(30);
                
                if (reader.GetField(31) != "")
                {
                    infoEquipementViewModel.Poids = Convert.ToDecimal(reader.GetField(31));
                }

                infoEquipementViewModel.UnitePoids = reader.GetField(32);
                infoEquipementViewModel.LibelleUnitePoids = reader.GetField(33);
                infoEquipementViewModel.CodeTechnologie = reader.GetField(34);
                infoEquipementViewModel.NombreVoies = reader.GetField(35);

                if (reader.GetField(36) != "")
                {
                    infoEquipementViewModel.Puissance = Convert.ToDecimal(reader.GetField(36));
                }

                infoEquipementViewModel.UnitePuissance = reader.GetField(37);

                if (reader.GetField(38) != "")
                {
                    infoEquipementViewModel.DateFacture = DateTime.ParseExact(reader.GetField(38), "dd/MM/yyyy", CultureInfo.CurrentCulture);
                    //infoEquipementViewModel.DateFacture = DateTime.ParseExact(reader.GetField(38), "d/M/yyyy", CultureInfo.CurrentCulture);
                    //infoEquipementViewModel.DateFacture = Convert.ToDateTime(reader.GetField(38));
                }
                
                if (reader.GetField(39) != "")
                {
                    infoEquipementViewModel.Quantite = Convert.ToInt32(reader.GetField(39));
                }

                return infoEquipementViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public string ValiderDonneesClientImport(InfoClientImportViewModel infoClient)
        {
            var strErreur = "";
            
            try
            {
                var codeInterneClient = infoClient.IdClient; 

                //Vérifier si client existe déjà
                var client = _econocomDataManager.GetCodeInterneClient(codeInterneClient);
                
                //Vérifier si Raison Sociale est valide
                if (string.IsNullOrEmpty(infoClient.RaisonSociale))
                {
                    if (client != null)
                    {
                        infoClient.RaisonSociale = client.RaisonSociale;
                    }
                    else
                    {
                        var erreur = "Raison Sociale est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Type Société est valide
                if (infoClient.TypeSociete != 1 && infoClient.TypeSociete != 2)
                {
                    var erreur = "Type Société n'est pas valide ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                //Vérifier si Groupe est valide
                if (string.IsNullOrEmpty(infoClient.Groupe))
                {
                    if (client != null)
                    {
                        infoClient.Groupe = client.Groupe;
                    }
                }

                //Vérifier si Adresse est valide
                if (string.IsNullOrEmpty(infoClient.Adresse))
                {
                    if (client != null)
                    {
                        infoClient.Adresse = client.Adresse.Adresse1;
                    }
                    else
                    {
                        var erreur = "Adresse est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Code Postal est valide
                if (string.IsNullOrEmpty(infoClient.CodePostal))
                {
                    if (client != null)
                    {
                        infoClient.CodePostal = client.Adresse.CodePostal;
                    }
                    else
                    {
                        var erreur = "Code Postal est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Ville est valide
                if (string.IsNullOrEmpty(infoClient.Ville))
                {
                    if (client != null)
                    {
                        infoClient.Ville = client.Adresse.Ville;
                    }
                    else
                    {
                        var erreur = "Ville est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Pays est valide
                if (string.IsNullOrEmpty(infoClient.CodePays))
                {
                    if (client != null)
                    {
                        var pays = _econocomDataManager.GetPaysTraduction(client.Adresse.PaysId, 1);
                        infoClient.CodePays = pays.LibellePays;
                    }
                    else
                    {
                        var erreur = "Pays est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }
                else
                {
                    var pays = _econocomDataManager.GetPaysParLibelle(infoClient.CodePays.ToLower(), 1);

                    if (pays == null)
                    {
                        var erreur = "Pays avec libellé " + infoClient.CodePays + " n'existe pas ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si secteur activité est valide
                if (string.IsNullOrEmpty(infoClient.CodeSecteurActivite))
                {
                    if (client != null)
                    {
                        var secteurActivite = _econocomDataManager.GetCorrespondanceSecteurActivite(Convert.ToInt32(client.SecteurActiviteId));
                        infoClient.CodeSecteurActivite = secteurActivite.CodeActivite;
                    }
                    else
                    {
                        var erreur = "Secteur activité est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }
                else
                {
                    var secteurActivite = _econocomDataManager.GetSecteurActiviteParCode(infoClient.CodeSecteurActivite);

                    if (secteurActivite == null)
                    {
                        var erreur = " Secteur Activité avec le code " + infoClient.CodeSecteurActivite + " n'existe pas ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Nombre Sites est valide
                if (infoClient.NombreSites != null && infoClient.NombreSites < 0)
                {
                    {
                        var erreur = "Nombre sites doit être > 0 ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }
                
                //Vérifier si Prix Moyen Kwh est valide
                if (infoClient.PrixMoyenKWH != null && infoClient.PrixMoyenKWH < 0)
                {
                    {
                        var erreur = "Prix Moyen Kwh doit être > 0 ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si CoefficientCO2 est valide
                if (infoClient.CoefficientCO2 < 0 || infoClient.CoefficientCO2 > 1)
                {
                    {
                        var erreur = "Coefficient CO2 doit être entre 0 et 1 ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Effectifs est valide
                if (infoClient.Effectifs <= 0)
                {
                    if (client != null)
                    {
                        infoClient.Effectifs = client.DetailsClient.Effectif;
                    }
                    else
                    {
                        var erreur = "Effectifs doit être > 0 ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Taux Equipement est valide
                if (infoClient.TauxEquipement != null && infoClient.TauxEquipement < 0)
                {
                    {
                        var erreur = "Taux Equipement doit être > 0 ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Devise est valide
                if (infoClient.CodeDevise != "")
                {
                    var devise = _econocomDataManager.GetDeviseParLibelle(infoClient.CodeDevise, 1);

                    if (devise == null)
                    {
                        var erreur = "Devise avec libellé " + infoClient.CodeDevise + " n'existe pas ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }
                else
                {
                    if (client != null)
                    {
                        var devise = _econocomDataManager.GetDeviseTraduction(Convert.ToInt32(client.DetailsClient.DeviseId), 1);
                        infoClient.CodeDevise = devise.Libelle;
                    }
                    else
                    {
                        //Si devise n'est pas renseigné, definir devise à Euro(€)
                        infoClient.CodeDevise = "Euro";
                    }
                }
                
                //Récupérer le contact admin / souscripteur admin pour le client 
                var contactAdmin = new Contact();
                if (client != null)
                {
                    contactAdmin = _econocomDataManager.GetContactClientParAuthorisation(client.Id, (int) TypeContactEnum.Administrateur) ??
                        _econocomDataManager.GetContactClientParAuthorisation(client.Id, (int) TypeContactEnum.SouscripteurAdmin);
                }

                //Vérifier si Nom Administrateur est valide
                if (string.IsNullOrEmpty(infoClient.NomAdministrateur))
                {
                    if (client != null)
                    {
                        infoClient.NomAdministrateur = contactAdmin.NomContact;
                    }
                    else
                    {
                        var erreur = "Nom Administrateur est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si Prénom Administrateur est valide
                if (string.IsNullOrEmpty(infoClient.PrenomAdministrateur))
                {
                    if (client != null)
                    {
                        infoClient.PrenomAdministrateur = contactAdmin.NomContact;
                    }
                    else
                    {
                        var erreur = "Prénom Administrateur est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                //Vérifier si adresse mail de l'administrateur est valide
                if (string.IsNullOrEmpty(infoClient.AdresseMailAdministrateur))
                {
                    if (client != null)
                    {
                        infoClient.AdresseMailAdministrateur = contactAdmin.Email;
                    }
                    else
                    {
                        var erreur = "Adresse mail de l'administrateur est nulle ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }
                else
                {
                    //Vérifier si format adresse mail est correct
                    string expression =
                        @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$";

                    Match match = Regex.Match(infoClient.AdresseMailAdministrateur, expression,
                        RegexOptions.IgnoreCase);
                    if (!match.Success)
                    {
                        var erreur = "Format de l'adresse mail " + infoClient.AdresseMailAdministrateur +
                                        " est incorrect ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }

                    //Vérifier pour un nouveau client ou si c'est un client existant et l'adresse mail de son admin n'est pas le meme que infoClient.AdresseMailAdministrateur 
                    if (client == null || (infoClient.AdresseMailAdministrateur != contactAdmin.Email))
                    {
                        //Vérifier si un contact avec ce mail adresse existe déjà dans WattsGreen
                        var contact = _econocomDataManager.GetContactByEmail(infoClient.AdresseMailAdministrateur);

                        if (contact != null)
                        {
                            var erreur = " Un utilisateur de Watts Green avec l'adresse email " +
                                         infoClient.AdresseMailAdministrateur + " existe déjà ";
                            Logger.Error(erreur);
                            strErreur = strErreur + erreur + ";";
                        }
                    }
                }

                //Vérifier si Code Langue est valide
                if (string.IsNullOrEmpty(infoClient.CodeLangue))
                {
                    var erreur = "Code langue est nulle ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }
                else
                {
                    var langue = _econocomDataManager.GetLangueByCode(infoClient.CodeLangue);

                    if (langue == null)
                    {
                        var erreur = "Langue avec code " + infoClient.CodeLangue + " n'existe pas ";
                        Logger.Error(erreur);
                        strErreur = strErreur + erreur + ";";
                    }
                }

                return strErreur;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public string ValiderDonneesEquipementImport(EquipementImportViewModel infoEquipement)
        {
            var strErreur = "";

            try
            {
                //Vérifier si Code Categorie est valide
                if (string.IsNullOrEmpty(infoEquipement.CodeCategorie))
                {
                    var erreur = "Code Categorie est nulle ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                //Vérifier si Code Famille est valide
                if (string.IsNullOrEmpty(infoEquipement.CodeFamille))
                {
                    var erreur = "Code Famille est nulle ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                //Vérifier si Code Sous Famille est valide
                if (string.IsNullOrEmpty(infoEquipement.CodeSousFamille))
                {
                    var erreur = "Code Sous Famille est nulle ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }
                
                //Vérifier si Taille est valide
                if (infoEquipement.Taille < 0)
                {
                    var erreur = "Taille n'est pas valide ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }
                
                //Vérifier si Vitesse est valide
                if (infoEquipement.Vitesse < 0)
                {
                    var erreur = "Vitesse n'est pas valide ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                //Vérifier si Poids est valide
                if (infoEquipement.Poids < 0)
                {
                    var erreur = "Poids n'est pas valide ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                //Vérifier si Puissance est valide
                if (infoEquipement.Puissance < 0)
                {
                    var erreur = "Puissance n'est pas valide ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                //Vérifier si Date Facture est valide
                if (infoEquipement.DateFacture == DateTime.MinValue)
                {
                    var erreur = "Date Facture est nulle ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                //Vérifier si Poids est valide
                if (infoEquipement.Quantite < 0)
                {
                    var erreur = "Quantité n'est pas valide ";
                    Logger.Error(erreur);
                    strErreur = strErreur + erreur + ";";
                }

                return strErreur;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public EquipementImportViewModel ValidationDonnesCorrespondance(EquipementImportViewModel infoEquipement, int typeSocieteId)
        {
            var strErreur = "";
            var typeDevice = new TypeDevice();

            try
            {
                //Rechercher type device dans table correspondance type device
                var correspondanceTypeDevice = _econocomDataManager.GetCorrespondanceTypeDevice(typeSocieteId, infoEquipement.CodeCategorie, infoEquipement.CodeFamille, infoEquipement.CodeSousFamille);

                if (correspondanceTypeDevice == null)
                {
                    strErreur = strErreur + "Type Device not found ;";
                    infoEquipement.Erreur = strErreur;
                }
                else
                {
                    infoEquipement.Caracteristique = correspondanceTypeDevice.Caracteristique;

                    if (correspondanceTypeDevice.Caracteristique != (int)CorrespondanceTypeDeviceCaracteristique.NonApplicable)
                    {
                        //Récupérer type device watts green
                        typeDevice = _econocomDataManager.GetTypeDeviceParId(Convert.ToInt32(correspondanceTypeDevice.TypeDeviceId));
                        infoEquipement.TypeDeviceId = typeDevice.Id;

                        //Determiner Classe Age
                        infoEquipement = DeterminerClasseEquipement(infoEquipement, typeSocieteId, correspondanceTypeDevice, typeDevice.Id);
                    }
                }

                return infoEquipement;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public EquipementImportViewModel DeterminerClasseEquipement(EquipementImportViewModel infoEquipement, int typeSocieteId, CorrespondanceTypeDevice correspondanceTypeDevice, int typeDeviceId)
        {
            var strErreur = "";
            var classeDeviceId = 0;
            var ageDevice = new AgeDevice();

            try
            {
                //Vérifier si caracteristique type device = recherche sur processeur
                if (correspondanceTypeDevice.Caracteristique.Equals((int)CorrespondanceTypeDeviceCaracteristique.RechercheSurProcesseur))
                {
                    if (string.IsNullOrEmpty(infoEquipement.CodeProcesseur))
                    {
                        strErreur = strErreur + "Code Processeur est nulle - RechercheSurProcesseur pas possible ;";
                    }
                    else
                    {
                        //Recherche du code processeur dans la table de correspondance Processeur
                        var correspondanceProcesseur = _econocomDataManager.GetCorrespondanceProcesseur(typeSocieteId,
                            infoEquipement.CodeProcesseur);

                        if (correspondanceProcesseur == null)
                        {
                            strErreur = strErreur + "Pocessor not found ;";
                        }
                        else
                        {
                            classeDeviceId = correspondanceProcesseur.ClasseDeviceId;
                        }
                    }
                }

                //Vérifier si caracteristique type device = recherche sur vitesse
                if (correspondanceTypeDevice.Caracteristique.Equals((int)CorrespondanceTypeDeviceCaracteristique.RechercheSurVitesse))
                {
                    //Recherche type device dans la table de correspondance Vitesse
                    var correspondanceVitesse = _econocomDataManager.GetCorrespondanceVitesse(typeSocieteId, typeDeviceId);

                    if (correspondanceVitesse == null)
                    {
                        strErreur = strErreur + "Vitesse not found ;";
                    }
                    else
                    {
                        if (infoEquipement.Vitesse <= correspondanceVitesse.Vitesse1)
                        {
                            classeDeviceId = correspondanceVitesse.Vitesse1ClasseDeviceId;
                        }

                        else if (infoEquipement.Vitesse > correspondanceVitesse.Vitesse1 && infoEquipement.Vitesse <= correspondanceVitesse.Vitesse2)
                        {
                            classeDeviceId = correspondanceVitesse.Vitesse2ClasseDeviceId;
                        }

                        else if (infoEquipement.Vitesse > correspondanceVitesse.Vitesse2 && infoEquipement.Vitesse <= correspondanceVitesse.Vitesse3)
                        {
                            classeDeviceId = correspondanceVitesse.Vitesse3ClasseDeviceId;
                        }

                        else if (classeDeviceId == 0)
                        {
                            strErreur = strErreur + "Aucune valeur de vitesse ne correspond à " + infoEquipement.Vitesse + " pour TypeSociéteId " + typeSocieteId + " et TypeDeviceId " +typeDeviceId + ";";   
                        }
                    }
                }


                //Vérifier si caracteristique type device = recherche sur taille
                if (correspondanceTypeDevice.Caracteristique.Equals((int)CorrespondanceTypeDeviceCaracteristique.RechercheSurTaille))
                {
                    //Recherche type device dans la table de correspondance taille
                    var correspondanceTaille = _econocomDataManager.GetCorrespondanceTaille(typeSocieteId, typeDeviceId);

                    if (correspondanceTaille == null)
                    {
                        strErreur = strErreur + "Taille not found ";
                    }
                    else
                    {
                        if (infoEquipement.Taille <= correspondanceTaille.Taille1)
                        {
                            classeDeviceId = correspondanceTaille.Taille1ClasseDeviceId;
                        }

                        else if (infoEquipement.Taille > correspondanceTaille.Taille1 && infoEquipement.Taille <= correspondanceTaille.Taille2)
                        {
                            classeDeviceId = correspondanceTaille.Taille2ClasseDeviceId;
                        }

                        else if (infoEquipement.Taille > correspondanceTaille.Taille2 && infoEquipement.Taille <= correspondanceTaille.Taille3)
                        {
                            classeDeviceId = correspondanceTaille.Taille3ClasseDeviceId;
                        }

                        else if (classeDeviceId == 0)
                        {
                            strErreur = strErreur + "Aucune valeur de taille ne correspond à " + infoEquipement.Taille + " pour TypeSociéteId " + typeSocieteId + " et TypeDeviceId " + typeDeviceId + ";";
                        }
                    }
                }


                //Vérifier si caracteristique type device = recherche sur puissance
                if (correspondanceTypeDevice.Caracteristique.Equals((int)CorrespondanceTypeDeviceCaracteristique.RechercheSurPuissance))
                {
                    //Recherche type device dans la table de correspondance puissance
                    var correspondancePuissance = _econocomDataManager.GetCorrespondancePuissance(typeSocieteId, typeDeviceId);

                    if (correspondancePuissance == null)
                    {
                        strErreur = strErreur + "Puissance not found ";
                    }
                    else
                    {
                        if (infoEquipement.Puissance <= correspondancePuissance.Puissance1)
                        {
                            classeDeviceId = correspondancePuissance.Puissance1ClasseDeviceId;
                        }

                        else if (infoEquipement.Puissance > correspondancePuissance.Puissance1 && infoEquipement.Puissance <= correspondancePuissance.Puissance2)
                        {
                            classeDeviceId = correspondancePuissance.Puissance2ClasseDeviceId;
                        }

                        else if (infoEquipement.Puissance > correspondancePuissance.Puissance2 && infoEquipement.Puissance <= correspondancePuissance.Puissance3)
                        {
                            classeDeviceId = correspondancePuissance.Puissance3ClasseDeviceId;
                        }

                        else if (classeDeviceId == 0)
                        {
                            strErreur = strErreur + "Aucune valeur de puissance ne correspond à " + infoEquipement.Puissance + " pour TypeSociéteId " + typeSocieteId + " et TypeDeviceId " + typeDeviceId + ";";
                        }
                    }
                }

                //Vérifier si caracteristique type device = non qualifié
                if (correspondanceTypeDevice.Caracteristique.Equals((int)CorrespondanceTypeDeviceCaracteristique.NonQualifier))
                {
                    //Pour type device "non qualifié, définir classeDevice = classe moyen de gamme"
                    classeDeviceId = 2;
                }

                //Déterminer age de l'équipement
                ageDevice = DeterminerAgeEquipement(infoEquipement);

                infoEquipement.ClasseDeviceId = classeDeviceId;
                infoEquipement.AgeDeviceId = ageDevice.Id;
                infoEquipement.Erreur = strErreur;

                return infoEquipement;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public AgeDevice DeterminerAgeEquipement(EquipementImportViewModel infoEquipement)
        {
            var ageDevice = new AgeDevice();

            try
            {
                //Formule : Date du jour de traitement – Date facture du fichier origine
                var nombreDeJours = (DateTime.Now - infoEquipement.DateFacture).Days;

                //Si nombre de jrs < 365 (Moins d'un an)
                if (nombreDeJours < 365)
                    ageDevice = _econocomDataManager.GetAgeParId(1);

                //Si nombre de jrs entre 365 et 1095 (Entre 1 et 3 ans)
                if (nombreDeJours >= 365 && nombreDeJours < 1095)
                    ageDevice = _econocomDataManager.GetAgeParId(2);

                //Si nombre de jrs entre 1095 et 1825 (Entre 3 et 5 ans)
                if (nombreDeJours >= 1095 && nombreDeJours < 1825)
                    ageDevice = _econocomDataManager.GetAgeParId(3);

                //Si nombre de jrs >= 1825 (Plus de 5 ans)
                if (nombreDeJours >= 1825)
                    ageDevice = _econocomDataManager.GetAgeParId(4);

                return ageDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public ImportECSchangeViewModel TraiterDonneesClient(ImportECSchangeViewModel importEcSchange)
        {
            try
            {
                var infoClient = importEcSchange.InfoClientImportViewModel;

                var codeInterneClient = infoClient.IdClient;

                //Vérifier si client existe déjà
                var client = _econocomDataManager.GetCodeInterneClient(codeInterneClient.ToString());

                var langue = _econocomDataManager.GetLangueByCode(infoClient.CodeLangue);
                
                using (var scope = new System.Transactions.TransactionScope())
                {

                    if (client != null)
                    {
                        //MAJ client existant
                        client = MAJClient(infoClient, client);

                        importEcSchange.ClientExistant = true;
                    }
                    else
                    {
                        //Création client + contact
                        client = CreationClient(infoClient);

                        //Creation ConfigSi
                        var configSi = new ConfigSi();
                        configSi.LibelleConfigSi = "ConfigSi" + client.Id;
                        configSi.DateDebut = DateTime.Now;
                        configSi.Statut = (int) ConfigSiStatut.Creer;

                        //Creation ConfigSiClient
                        var configSiClient = new ConfigSiClient();
                        configSiClient.ClientId = Convert.ToInt32(client.Id);
                        configSiClient.DateDebut = DateTime.Now;

                        configSi = _econocomDataManager.SauvegardeConfigSi(configSi);
                        configSiClient.ConfigSiId = configSi.Id;

                        var origine = _econocomDataManager.GetOrigineParLibelle(OrigineEnum.Actuel.ToString());
                        configSiClient.OrigineId = origine.Id;

                        configSiClient = _econocomDataManager.SauvegardeConfigSiClient(configSiClient);
                        configSiClient.ConfigSi = configSi;
                    }

                    scope.Complete();
                }

                importEcSchange.Client = client;
                return importEcSchange;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public Client MAJClient(InfoClientImportViewModel infoClient, Client client)
        {
            try
            {
                //Get secteurActivite, pays, devise
                var secteurActivite = _econocomDataManager.GetSecteurActiviteParCode(infoClient.CodeSecteurActivite);
                var pays = _econocomDataManager.GetPaysParLibelle(infoClient.CodePays, 1);
                var devise = _econocomDataManager.GetDeviseParLibelle(infoClient.CodeDevise, 1);

                //Client
                client.RaisonSociale = infoClient.RaisonSociale;
                client.Groupe = infoClient.Groupe;
                client.TypeClient = infoClient.TypeSociete;
                client.SecteurActiviteId = secteurActivite.Id;
                client.DateModification = DateTime.Now;
                client = _econocomDataManager.UpdateClient(client);
                
                //Adresse
                client.Adresse.Adresse1 = infoClient.Adresse;
                client.Adresse.CodePostal = infoClient.CodePostal;
                client.Adresse.Ville = infoClient.Ville;
                client.Adresse.PaysId = pays.Id;
                client.Adresse = _econocomDataManager.updateAdress(client.Adresse);
                
                //Details Client
                client.DetailsClient.CodeInterneClient = infoClient.IdClient;
                client.DetailsClient.PrixKWH = infoClient.PrixMoyenKWH != null ? Convert.ToDecimal(infoClient.PrixMoyenKWH) : 0;
                client.DetailsClient.DeviseId = devise.Id;
                client.DetailsClient.CoefficientCO2 = infoClient.CoefficientCO2 != 0 ? infoClient.CoefficientCO2 : Convert.ToDecimal(pays.CoefficientConversion); //infoClient.CoefficientCO2;
                client.DetailsClient.TauxEquipement = infoClient.TauxEquipement != null ? Convert.ToDecimal(infoClient.TauxEquipement) : 0;
                client.DetailsClient.ChiffreAffaire = infoClient.ChiffreAffaire != null ? Convert.ToInt32(infoClient.ChiffreAffaire) : 0;
                client.DetailsClient.NombreSite = infoClient.NombreSites != null ? Convert.ToInt32(infoClient.NombreSites) : 0;
                client.DetailsClient.Effectif = infoClient.Effectifs;
                client.DetailsClient = _econocomDataManager.MAJDetailsClient(client.DetailsClient);
                
                //Récupérer le contact admin / souscripteur admin pour le client 
                var contactAdmin = _econocomDataManager.GetContactClientParAuthorisation(client.Id, (int)TypeContactEnum.Administrateur) ??
                        _econocomDataManager.GetContactClientParAuthorisation(client.Id, (int)TypeContactEnum.SouscripteurAdmin);

                //Si c'est un nouveau admin
                if (contactAdmin.Email != infoClient.AdresseMailAdministrateur || string.IsNullOrEmpty(contactAdmin.MotPasse))
                {
                    client.Contacts = new List<Contact> {contactAdmin};
                    contactAdmin.CodeVerification = Guid.NewGuid().ToString();
                    contactAdmin.MotPasse = "";
                }

                //MAJ Contact admin uniquement
                contactAdmin.NomContact = infoClient.NomAdministrateur;
                contactAdmin.PrenomContact = infoClient.PrenomAdministrateur;
                contactAdmin.Email = infoClient.AdresseMailAdministrateur;
                contactAdmin = _econocomDataManager.MAJContact(contactAdmin);
                
                return client;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public Client CreationClient(InfoClientImportViewModel infoClient)
        {
            try
            {
                //Get secteurActivite, pays, devise
                var secteurActivite = _econocomDataManager.GetSecteurActiviteParCode(infoClient.CodeSecteurActivite);
                var pays = _econocomDataManager.GetPaysParLibelle(infoClient.CodePays, 1);
                var devise = _econocomDataManager.GetDeviseParLibelle(infoClient.CodeDevise, 1);

                // Générer le code interne. On génére un autre code si un code est déjà associé à un client
                var codeInterne = "";
                do
                {
                    codeInterne = Security.HashHelper.GenererCodeInterne();
                } while (CodeInterneExiste(codeInterne));
                
                //Créer Client
                var nouveauClient = new Client
                {
                    Adresse = new Adresse()
                    {
                        Adresse1 = infoClient.Adresse,
                        CodePostal = infoClient.CodePostal,
                        Ville = infoClient.Ville,
                        PaysId = pays.Id
                    },
                    RaisonSociale = infoClient.RaisonSociale,
                    Groupe = infoClient.Groupe,
                    TypeClient = infoClient.TypeSociete,
                    SecteurActiviteId = secteurActivite.Id,
                    Statut = (int)ClientStatut.Valide,
                    DateValidation = DateTime.Now,

                    DetailsClient = new DetailsClient()
                    {
                        CodeInterneClient = infoClient.IdClient,
                        PrixKWH = infoClient.PrixMoyenKWH != null ? Convert.ToDecimal(infoClient.PrixMoyenKWH) : 0,
                        DeviseId = devise.Id,
                        CoefficientCO2 = infoClient.CoefficientCO2 != 0 ? infoClient.CoefficientCO2 : Convert.ToDecimal(pays.CoefficientConversion), 
                        TauxEquipement = infoClient.TauxEquipement != null ? Convert.ToDecimal(infoClient.TauxEquipement) : 0,
                        ChiffreAffaire = infoClient.ChiffreAffaire != null ? Convert.ToInt32(infoClient.ChiffreAffaire) : 0,
                        NombreSite = infoClient.NombreSites != null ? Convert.ToInt32(infoClient.NombreSites) : 0,
                        Effectif = infoClient.Effectifs
                    },
                    DateDebut = DateTime.Now,
                    CodeInterneWattsGreen = codeInterne
                };

                //Créer Contact
                var contact = new Contact()
                {
                    NomContact = infoClient.NomAdministrateur,
                    PrenomContact = infoClient.PrenomAdministrateur,
                    Email = infoClient.AdresseMailAdministrateur,
                    TypeContactId = GetTypeContactByString(TypeContactEnum.Administrateur.ToString()).Id,
                    CodeVerification = Guid.NewGuid().ToString(),
                    Authorisations = (int)TypeContactEnum.SouscripteurAdmin
                };

                nouveauClient.Contacts.Add(contact);

                return SaveClient(nouveauClient);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public bool EnvoyerEmail(ImportECSchangeViewModel importEcSchange)
        {
            try
            {
                var infoClient = importEcSchange.InfoClientImportViewModel;
                var client = importEcSchange.Client;

                var langue = GetLangueByCode(infoClient.CodeLangue);

                if (client.Contacts.Count > 0)
                {
                    EnvoyerEmailEnregistrement(client, langue);
                }

                if (!importEcSchange.ClientExistant)
                {
                    //Envoyer email de souscription au webmaster
                    EnvoyerEmailSouscriptionAuWebmaster(infoClient.NomAdministrateur, infoClient.RaisonSociale, langue);
                }

                return true;
            }
            catch(Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public bool EnvoyerEmailEnregistrement(Client client, Langue langue)
        {
            var emailList = new Dictionary<string, string>();
            var noContract = "";

            try
            {
                var contacts = client.Contacts;

                var codeInterne = client.DetailsClient.CodeInterneClient;

                if(codeInterne.Length >= 10)
                    noContract = codeInterne.Substring(codeInterne.Length - 10);
                

                // Ajouter l'adresse mail du souscripteur
                foreach (var contact in contacts)
                {
                    emailList.Add(contact.Email, contact.CodeVerification);
                }
                
                // Récupérer le sujet et le contenu du mail
                var typeMail = GetTypeMail("Validation Flow");
                var contenuMail = GetContenuMail(typeMail.Id, langue.Id);

                var routageHome = GetRoutageByLangueIdAndValeurActuelle(langue.Id, "Home");
                if (routageHome == null)
                    Logger.Error("Pas de routage pour Home, idLangue=" + langue.Id);

                var routageGestionMotDePasse = GetRoutageByLangueIdAndValeurActuelle(langue.Id, "GestionMotDePasse");
                if (routageGestionMotDePasse == null)
                    Logger.Error("Pas de routage pour GestionMotDePasse, idLangue=" + langue.Id);

                if (routageHome != null && routageGestionMotDePasse != null)
                {
                    var lien = "xxServerAddress/" + langue.Code + "/" + routageHome.ValeurTraduite + "/" +
                               routageGestionMotDePasse.ValeurTraduite + "?email=";
                    
                    // Envoyer Email
                    return Econocom.Helper.Email.EmailHelper.SendMailValidationFlow(emailList, contenuMail, lien, noContract);
                }

                return false;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        
        public bool EnvoyerEmailSouscriptionAuWebmaster(string souscripteur, string societe, Langue langue)
        {
            try
            {
                // Récupérer le sujet et le contenu du mail
                var typeMail = GetTypeMail("Alerte nouvelle souscription");
                var contenuMail = GetContenuMail(typeMail.Id, langue.Id);

                // Récupérer l'adresse mail du webmaster
                var administrateur = GetAdministrateur();

                var webMasterEmail = "";
                if (administrateur != null)
                {
                    webMasterEmail = administrateur.Email;
                }

                // Envoyer Email
                return Econocom.Helper.Email.EmailHelper.EnvoyerMailSouscriptionAuWebmaster(webMasterEmail, contenuMail, souscripteur, societe);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }


        public ImportECSchangeViewModel TraiterDonneesEquipements(ImportECSchangeViewModel importECSchangeViewModel, Client client)
        {
            var listeImportClient = new List<ImportClientViewModel>();
            var infoClient = importECSchangeViewModel.InfoClientImportViewModel;
            var listeEquipementImportViewModel = importECSchangeViewModel.ListeEquipementImportViewModel;
            
            try
            {
                foreach (var equipement in listeEquipementImportViewModel)
                {
                    var importClient = new ImportClientViewModel();

                    importClient.IdClient = client.Id;
                    importClient.TypeDevice = equipement.TypeDeviceId;
                    importClient.ClasseDevice = equipement.ClasseDeviceId;
                    importClient.AgeDevice = equipement.AgeDeviceId;
                    importClient.QteDevice = equipement.Quantite;
                    importClient.Poids = equipement.Poids;
                    importClient.CoefficientConversion = infoClient.CoefficientCO2;
                    importClient.Caracteristique = equipement.Caracteristique;

                    listeImportClient.Add(importClient);
                }

                importECSchangeViewModel.ListeImportClientViewModel = listeImportClient;

                return importECSchangeViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        } 

        #endregion

    }

}
