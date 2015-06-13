using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Econocom.Helper;
using Econocom.Model.Enum;
using Econocom.Model.Interfaces;
using Econocom.Model.Models;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Common;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using Econocom.Resource;
using Model.Interfaces;
using System.Data.SqlClient;
using NLog;
using Econocom.Model.Models.Traduction;
using System.Text;

namespace Econocom.Data
{
    public class EconocomDataManager
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private IUnitOfWork _unitOfWork;

        public EconocomDataManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public EconocomDataManager()
        {
            _unitOfWork = new EconocomContext();
        }

        /// <summary>
        /// Recupere le contenu d'une page et une langue
        /// </summary>
        /// <param name="pageId">Identifiant de la page</param>
        /// <param name="languageId">Identifiant de la langue</param>
        /// <returns>Le contenu de la page et la langue</returns>
        public Model.Models.CMS.ContenuModere GetPublishedContent(int pageId, int languageId)
        {
            
            try
            {
                var publishedContentRepository = new Repository<ContenuModere>();
                
                var content =
                    publishedContentRepository.Filter(
                        publishedContent =>
                        publishedContent.Page.Id == pageId && publishedContent.Langue.Id == languageId);
                return content.Any() ? content.Single() : null;
            }
            catch (Exception e)
            {
                Logger.Error("page {0}, langue {1}", pageId, languageId);
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere l'adresse
        /// </summary>
        /// <param name="id">Identifiant de l'adresse</param>
        /// <returns></returns>
        public Adresse GetAdresseById(int id)
        {
            try
            {
                var adresseRepository = new Repository<Adresse>();
                var adresse = adresseRepository.Find(id);
                return adresse;
            }
            catch (Exception e)
            {
                Logger.Error("identifiant adresse {0}", id);
                LogguerErreur(e);
                throw;
            }

            return null;
        }

        /// <summary>
        /// Creer ou Modifier un client
        /// </summary>
        /// <param name="client">Le client a creer/mettre a jour</param>
        /// <returns>le nouveau client/client mis a jour</returns>
        public Client SetClient(Client client)
        {
            if (client == null)
            {
                Logger.Error("Client null");
                return null;
            }

            try
            {               
                var clientRepository = new Repository<Client>();

                if (ClientExists(client))
                {
                    client.DateModification = DateTime.Now;
                    var status = clientRepository.Update(client);
                    _unitOfWork.SaveChanges();
                    return client;
                }
                else
                {
                    client.Statut = (int)ClientStatut.Valide;
                    var newClient = clientRepository.Create(client);
                    _unitOfWork.SaveChanges();
                    return newClient;
                }
            }
            catch (Exception e)
            {
                Logger.Error("raison sociale {0}", client.RaisonSociale);
                LogguerErreur(e);
                throw;
            }
            return client;
        }

        /// <summary>
        /// Verifier si un client existe
        /// </summary>
        /// <param name="client">Le client</param>
        /// <returns>True si le client existe, sinon False</returns>
        private bool ClientExists(Client client)
        {
            try
            {
                if (client == null || client.DetailsClient==null)
                    return false;

                Logger.Debug("raison sociale {0}, code interne {1}", client.RaisonSociale, client.DetailsClient.CodeInterneClient);

                var clientRepository = new Repository<Client>();
                var existingClient =
                    clientRepository.Filter(
                        c =>
                        (c.Id != client.Id) &&
                        (c.Nom == (client.Nom) ||
                         c.DetailsClient.CodeInterneClient == (client.DetailsClient.CodeInterneClient)));
                if (existingClient.Any())
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Logger.Error("raison sociale {0}", client != null ? client.RaisonSociale : "");
                LogguerErreur(e);
                throw;
            }

            return false;
        }

        /// <summary>
        /// Recupere un client
        /// </summary>
        /// <param name="clientId">L'identifiant du client</param>
        /// <returns>Le client</returns>
        public Client GetClient(int clientId)
        {
            try
            {
                Logger.Debug("id client {0}", clientId);
                var clientRepository = new Repository<Client>();
                var query = clientRepository.Filter(c => c.Id == clientId).Include(c => c.SecteurActivite);
                if (query.Any())
                {
                    var client = query.Single();
                    return client;
                }                
            }
            catch (Exception e)
            {
                Logger.Error("id client {0}", clientId);
                LogguerErreur(e);
                throw;
            }

            return null;
        }

        /// <summary>
        /// Mis a jour d'un client
        /// </summary>
        /// <param name="client">Le client a mettre a jour</param>
        /// <returns>Le client mis a jour</returns>
        public Client UpdateClient(Client client)
        {
            if (client == null)
            {
                Logger.Error("Client null");
                return null;
            }

            try
            {                
                var clientRepository = new Repository<Client>();
                client.DateModification = DateTime.Now;
                clientRepository.Update(client);
                _unitOfWork.SaveChanges();
            }
            catch(Exception e)
            {
                Logger.Error("id {0}, raison sociale {1}", client.Id, client.RaisonSociale );
                LogguerErreur(e);
                throw;
            }
            return client;
        }

        //public Client ValidateClient(string status, Client c)
        //{
        //    IRepository<Client> clientRepository = new Repository<Client>();

        //    //var client = clientRepository.Find(c.Id);
        //    //client.Name = c.Name;
        //    //client.Address.Address1 = c.Address.Address1;
        //    //client.Address.Address2 = c.Address.Address2;
        //    //client.Address.PostalCode = c.Address.PostalCode;
        //    //client.ClientDetail.ClientCode = c.ClientDetail.ClientCode;
        //    //client.Address.Country = c.Address.Country;

        //    //client.Status = status;

        //    //foreach (var contact in client.Contacts)
        //    //{
        //    //    contact.VerificationCode = Guid.NewGuid().ToString();
        //    //}

        //    //clientRepository.Update(client);
        //    //_unitOfWork.SaveChanges();

        //    //return client;
        //    return null;
        //}

        /// <summary>
        /// Recupere la liste des clients entre 2 dates
        /// </summary>
        /// <param name="from">Date de debut</param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<Client> GetClients(DateTime from, DateTime to)
        {            
            try
            {
                var clientRepository = new Repository<Client>();
                var clients = clientRepository.Filter(c => (c.DateDebut >= from && c.DateDebut <= to));
                if (clients.Any())
                {
                    var liste = clients.ToList();
                    return liste;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Date {0} a date: {1}", from.ToShortDateString(), to.ToShortDateString());
                LogguerErreur(e);
                throw;
            }

            return null;
        }

        /// <summary>
        /// Recupere un contact
        /// </summary>
        /// <param name="email">Adresse email du contact</param>
        /// <param name="motpasse">Mot de passe du contact</param>
        /// <returns>Le contact</returns>
        public Contact GetContact(string email, string motpasse)
        {
            try
            {
                Logger.Debug("email {0}, mot de passe {1}", email, motpasse);
                var contactRepository = new Repository<Contact>();
                var contact =
                    contactRepository.Filter(c => c.Email == (email) && c.MotPasse == (motpasse) && c.DateFin == null && c.Client.Statut != 2).Include(c => c.Client);
                return contact.Any() ? contact.Single() : null;
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }           
        }

        /// <summary>
        /// Recupere un contact avec un adresse email
        /// </summary>
        /// <param name="email">L'adresse email du contact</param>
        /// <returns>Le contact</returns>
        public Contact GetContactByEmail(string email)
        {
            try
            {
                Logger.Debug("email {0}", email);
                var contactRepository = new Repository<Contact>();
                var contact = contactRepository.Filter(c => c.Email == email).AsNoTracking();
                return contact.Any() ? contact.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere un contact
        /// </summary>
        /// <param name="typeId">Le type de contact</param>
        /// <param name="clientId">L'identifiant du client</param>
        /// <returns>Le contact</returns>
        public Contact GetContactByTypeAndClient(int typeId, int clientId)
        {
            try
            {
                Logger.Debug("typeId {0}, id client {1}", typeId, clientId);
                var contactRepository = new Repository<Contact>();
                var contact = contactRepository.Filter(c => c.TypeContactId == typeId && c.ClientId == clientId);
                return contact.Any() ? contact.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des utilisateurs par client et type utilisateur
        /// </summary>
        /// <param name="clientId">L'identifiant du client</param>
        /// <param name="typeContactId">L'identifiant du type de contact</param>
        /// <returns>La liste des utilisateurs</returns>
        public List<Contact> GetUtilisateursByClientIdAndTypeContactId(int clientId, int typeContactId)
        {
            try
            {
                Logger.Debug("typeContactId {0}, id client {1}", typeContactId, clientId);
                IRepository<Contact> contactRepository = new Repository<Contact>();
                var listeUtilisateur = contactRepository.Filter(f => f.Client.Id == clientId && f.TypeContactId == typeContactId).ToList();
                return listeUtilisateur;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        ////
        //public Contact UpdateContactPassword(Contact contact)
        //{
        //    //contact.VerificationCode = " ";
        //    //var contactRepository = new Repository<Contact>();
        //    //contactRepository.Update(contact);
        //    return contact;
        //}

        /// <summary>
        /// Recupere un contact
        /// </summary>
        /// <param name="contactId">L'identifiant du contact</param>
        /// <returns>Le contact</returns>
        public Contact GetContactById(int contactId)
        {
            try
            {
                Logger.Debug("contactId {0}", contactId);
                var contactRepository = new Repository<Contact>();
                var contact = contactRepository.Find(contactId);
                return contact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Savegarde un contact
        /// </summary>
        /// <param name="contact">Le contact a sauvegarder</param>
        /// <returns>Le contact</returns>
        public Contact SetContact(Contact contact)
        {
            try
            {
                Logger.Debug("NomContact {0}, PrenomContact {1}", contact.NomContact, contact.PrenomContact);
                var contactRepository = new Repository<Contact>();
                var newContact = contactRepository.Create(contact);
                _unitOfWork.SaveChanges();
                return newContact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Mise a jour d'un contact
        /// </summary>
        /// <param name="contact">Le contact a mettre a jour</param>
        /// <returns>Le contact mis a jour</returns>
        public Contact UpdateContact(Contact contact)
        {
            try
            {
                Logger.Debug("NomContact {0}, PrenomContact {1}", contact.NomContact, contact.PrenomContact);
                var contactRepository = new Repository<Contact>();
                contact.DateModification = DateTime.Now;
                contactRepository.Update(contact);
                _unitOfWork.SaveChanges();
                return contact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }
        

        public DetailsClient SetClientDetail(DetailsClient clientDetail)
        {
            try
            {
                IRepository<DetailsClient> clientDetailRepository = new Repository<DetailsClient>();
                clientDetailRepository.Create(clientDetail);
                _unitOfWork.SaveChanges();
                return clientDetail;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public DetailsClient GetDetailsClient(int idClient)
        {
            try
            {
                var clientDetailRepository = new Repository<DetailsClient>();
                var detailsClient = clientDetailRepository.Find(idClient);
                _unitOfWork.SaveChanges();
                return detailsClient;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public DetailsClient SetDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                var clientDetailRepository = new Repository<DetailsClient>();
                clientDetailRepository.Create(detailsClient);
                _unitOfWork.SaveChanges();
                return detailsClient;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }



        public Contact GetContactByVerificationCode(string verificationCode)
        {
            try
            {
                IRepository<Contact> contactRepository = new Repository<Contact>();
                var contact = contactRepository.Filter(c => c.CodeVerification == (verificationCode)).AsNoTracking();
                return contact.Any() ? contact.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Question> GetAllQuestions()
        {
            try
            {
                IRepository<Question> questionRepository = new Repository<Question>();
                var questions = questionRepository.All().ToList();
                return questions;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public Contact UpdateVerificationCode(Contact contact)
        {
            try
            {
                var contactRepository = new Repository<Contact>(_unitOfWork);
                contact.DateModification = DateTime.Now;
                contactRepository.Update(contact);
                _unitOfWork.SaveChanges();
                return contact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }      

        public Question GetQuestion(int id)
        {
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                IRepository<Question> questionRepository = new Repository<Question>();
               
                var question = questionRepository.All()
                                                  .Select(
                                                      t =>
                                                      new
                                                          {
                                                              Question = t,
                                                              QuestionTraductions =
                                                          t.QuestionTraductions.Where(l => l.LangueId == langueId)
                                                          })
                                                  .AsEnumerable()
                                                  .Select(t => t.Question)
                                                  .Where(t => t.Id == id)
                                                  .ToList();

                if (question.Any())
                    return question.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }
       

        //public ClientDetail GetClientDetails(int clientId)
        //{
        //    var clientRepository = new Repository<Client>();
        //    var client = clientRepository.Filter(c => c.Id == clientId).AsNoTracking();
        //    if (client.Any())
        //    {
        //        return client.Single().ClientDetail;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public Pays GetPaysById(int paysId)
        {
            try
            {
                var paysRepository = new Repository<Pays>();
                var pays = paysRepository.Filter(p => p.Id == paysId).AsNoTracking();
                return pays.Any() ? pays.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public Client GetClientById(int clientId)
        {
            try
            {
                var clientRepository = new Repository<Client>();
                var client = clientRepository.Filter(c => c.Id == clientId).AsNoTracking();
                return client.Any() ? client.Single() : null;
                return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }

        }


        public Client GetClientByLoginName(String loginName)
        {
            try
            {
                var contactRepository = new Repository<Contact>();
                var contact = contactRepository.Filter(c => c.Email == loginName).AsNoTracking();
                return contact.Any() ? contact.Single().Client : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Pays> GetCountries()
        {
            try
            {
                IRepository<Pays> countryRepository = new Repository<Pays>();
                var listePays = countryRepository.All().ToList();
                return listePays;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

                   
        

        public Langue GetDefaultLanguage()
        {
            try
            {
                var languageRepository = new Repository<Langue>();
                var language = languageRepository.Filter(l => l.Defaut == true);
                if (language.Any())
                {
                    return language.Single();
                }
                else
                {                    
                    return null;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Langue> GetLanguages()
        {
            var languageRegpository = new Repository<Langue>();
            var languages = languageRegpository.Filter(l => l.Localise == true).ToList();
            return languages;
        }

        public Langue GetLangueByCode(String code)
        {

            var LangueRepository = new Repository<Langue>();
            var langue = LangueRepository.Filter(c => c.Code == code).AsNoTracking();
            return langue.Any() ? langue.Single() : null;
        }

        public List<ContenuModere> GetPublishedContentsForPage(string name, string languageCulture)
        {
            var publishedContentRepository = new Repository<ContenuModere>();
            var publishedContents =
                publishedContentRepository.Filter(
                    publishedContent =>
                    publishedContent.Page.Nom == name && publishedContent.Langue.Culture == languageCulture).ToList();
            return publishedContents;
        }
        
        public dynamic SetObject(dynamic dynamicObject)
        {
            dynamic obj = dynamicObject;
            Type objectType = obj.GetType();
            if (objectType.IsEquivalentTo(typeof (AgeDevice)))
            {
                var repository = new Repository<AgeDevice>();
                repository.Update(obj);
            }


            _unitOfWork.SaveChanges();
            return dynamicObject;
        }

        public AgeDevice GetDeviceAge(int id)
        {
            var repository = new Repository<AgeDevice>();
            return repository.Find(id);
        }

        public List<AgeDevice> GetDeviceAges()
        {
            var repository = new Repository<AgeDevice>();
            return repository.All().ToList();
        }

        #region REFERENCE

        public Pays SetPays(Pays Pays)
        {
            try
            {
                var repository = new Repository<Pays>();
                if (repository.All().Any(p => p.Id == (Pays.Id)))
                {
                    bool isModified = false;
                    var objetExistant = repository.All().First(p => p.Id == (Pays.Id));
                    if (objetExistant.LibellePays != Pays.LibellePays ||
                        objetExistant.CoefficientConversion != Pays.CoefficientConversion)
                        isModified = true;

                    objetExistant.LibellePays = Pays.LibellePays;
                    objetExistant.CoefficientConversion = Pays.CoefficientConversion;
                    objetExistant.DateDebut = Pays.DateDebut;
                    objetExistant.DateFin = Pays.DateFin;
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;
                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(Pays);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibellePays={1}", Pays.Id, Pays.LibellePays);
                LogguerErreur(e);
                return null;
            }
            return Pays;
        }

        public CategorieDevice SetCategorieDevice(CategorieDevice categorieDevice)
        {
            try
            {
                var repository = new Repository<CategorieDevice>();
                if (repository.All().Any(c => c.Id == categorieDevice.Id))
                {
                    var existingcategorieDevice = repository.All().First(c => c.Id == (categorieDevice.Id));

                    bool isModified = existingcategorieDevice.LibelleCategorieDevice != categorieDevice.LibelleCategorieDevice;                    

                    if (categorieDevice.FamilleDeviceId != null && categorieDevice.FamilleDeviceId > 0)
                        existingcategorieDevice.FamilleDeviceId = categorieDevice.FamilleDeviceId;

                    existingcategorieDevice.LibelleCategorieDevice = categorieDevice.LibelleCategorieDevice;
                    existingcategorieDevice.URL = categorieDevice.URL;

                    if (categorieDevice.DateDebut > default(DateTime))
                        existingcategorieDevice.DateDebut = categorieDevice.DateDebut;

                    if (categorieDevice.DateFin != null && categorieDevice.DateFin > default(DateTime) &&
                        categorieDevice.DateFin >= categorieDevice.DateDebut)
                        existingcategorieDevice.DateFin = categorieDevice.DateFin;

                    if (isModified)
                        existingcategorieDevice.DateModification = DateTime.Now;
                    repository.Update(existingcategorieDevice);
                }
                else
                {
                    repository.Create(categorieDevice);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleCategorieDevice={1}", categorieDevice.Id, categorieDevice.LibelleCategorieDevice);
                LogguerErreur(e);
                return null;
            }
            return categorieDevice;
        }

        public ClasseDevice SetClasseDevice(ClasseDevice classeDevice)
        {
            var repository = new Repository<ClasseDevice>();
            try
            {
                if (repository.All().Any(c => c.Id == classeDevice.Id)
                    )
                {
                    var isModified = false;
                    var existingClasseDevice = repository.All().First(c => c.Id == classeDevice.Id );

                    if (existingClasseDevice.LibelleClasseDevice != classeDevice.LibelleClasseDevice )
                        isModified = true;                   

                    if (classeDevice.SecteurActiviteId != null && classeDevice.SecteurActiviteId > 0)
                        existingClasseDevice.SecteurActiviteId = classeDevice.SecteurActiviteId;

                    existingClasseDevice.LibelleClasseDevice = classeDevice.LibelleClasseDevice;
                    existingClasseDevice.DateDebut = classeDevice.DateDebut;
                    existingClasseDevice.DateFin = classeDevice.DateFin;

                    if (isModified)
                        existingClasseDevice.DateModification = DateTime.Now;
                    repository.Update(existingClasseDevice);
                }
                else
                {
                    repository.Create(classeDevice);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleClasseDevice={1}", classeDevice.Id, classeDevice.LibelleClasseDevice);
                LogguerErreur(e);
                return null;
            }

            return classeDevice;
        }

        /// <summary>
        ///  Creer/Modifier un objet AgeDevice
        /// </summary>
        /// <param name="ageDevice">Objet a creer/modifier</param>
        /// <returns>L'objet AgeDevice</returns>
        public AgeDevice SetAgeDevice(AgeDevice ageDevice)
        {
            var repository = new Repository<AgeDevice>();
            try
            {
                if (repository.All().Any(c => c.Id == (ageDevice.Id)))
                {
                    var existingAgeDevice = repository.All().First(c => c.Id == (ageDevice.Id));
                    bool isModified = existingAgeDevice.LibelleAgeDevice != ageDevice.LibelleAgeDevice;                   

                    if (ageDevice.SecteurActiviteId != null && ageDevice.SecteurActiviteId > 0)
                        existingAgeDevice.SecteurActiviteId = ageDevice.SecteurActiviteId;

                    existingAgeDevice.LibelleAgeDevice = ageDevice.LibelleAgeDevice;

                    if (ageDevice.DateDebut > default(DateTime))
                        existingAgeDevice.DateDebut = ageDevice.DateDebut;

                    if (ageDevice.DateFin != null && ageDevice.DateFin > default(DateTime))
                        existingAgeDevice.DateFin = ageDevice.DateFin;

                   if(isModified)
                        existingAgeDevice.DateModification = DateTime.Now;
                    repository.Update(existingAgeDevice);
                }
                else
                {
                    repository.Create(ageDevice);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleAgeDevice={1}", ageDevice.Id, ageDevice.LibelleAgeDevice);
                LogguerErreur(e);
                return null;
            }
            return ageDevice;
        }
       

        public ConsoWattHeur SetConsoWattHeur(ConsoWattHeur consoWattHeur)
        {
            var repository = new Repository<ConsoWattHeur>();
            try
            {
                if (repository.All().Any(c => (c.ClasseDeviceId == consoWattHeur.ClasseDeviceId &&
                                               c.AgeDeviceId == consoWattHeur.AgeDeviceId &&
                                               c.TypeDeviceId == consoWattHeur.TypeDeviceId))
                    )
                {
                    var existingConsoWattHeur =
                        repository.All().First(c => (c.ClasseDeviceId == consoWattHeur.ClasseDeviceId &&
                                                     c.AgeDeviceId == consoWattHeur.AgeDeviceId &&
                                                     c.TypeDeviceId == consoWattHeur.TypeDeviceId));
                    var isModified = existingConsoWattHeur.Consommation != consoWattHeur.Consommation;

                    existingConsoWattHeur.Consommation = consoWattHeur.Consommation;
                    if (isModified)
                        existingConsoWattHeur.DateModification = DateTime.Now;
                    repository.Update(existingConsoWattHeur);
                }
                else
                {
                    consoWattHeur.DateDebut = DateTime.Now;
                    repository.Create(consoWattHeur);
                }
            }
            catch (Exception e)
            {
                Logger.Error("TypeDeviceId={0}, ClasseDeviceId={1}, AgeDeviceId={2}", consoWattHeur.TypeDeviceId, consoWattHeur.ClasseDeviceId, consoWattHeur.AgeDeviceId);
                LogguerErreur(e);
                return null;
            }

            return consoWattHeur;
        }

        /// <summary>
        /// Creer/Modifier un objet FamilleDevice
        /// </summary>
        /// <param name="familleDevice">L'objet FamilleDevice a creer/modifier</param>
        /// <returns>L'objet FamilleDevice</returns>
        public FamilleDevice SetFamilleDevice(FamilleDevice familleDevice)
        {
            var repository = new Repository<FamilleDevice>();
            try
            {
                if (repository.All().Any(c => c.Id == familleDevice.Id &&
                                              (familleDevice.SecteurActiviteId == null ||
                                               (familleDevice.SecteurActiviteId != null &&
                                                c.SecteurActiviteId == familleDevice.SecteurActiviteId))                                              
                    ))
                {
                    var existingFamilleDevice = repository.All().First(c => c.Id == familleDevice.Id &&
                                                                              (familleDevice.SecteurActiviteId == null ||
                                                                               (familleDevice.SecteurActiviteId != null &&
                                                                                c.SecteurActiviteId ==
                                                                                familleDevice.SecteurActiviteId))
                                                                              );
                    bool isModified = existingFamilleDevice.LibelleFamilleDevice != familleDevice.LibelleFamilleDevice ||                                      
                                      existingFamilleDevice.SecteurActiviteId != familleDevice.SecteurActiviteId;
                    

                    if (familleDevice.SecteurActiviteId != null && familleDevice.SecteurActiviteId > 0)
                        existingFamilleDevice.SecteurActiviteId = familleDevice.SecteurActiviteId;

                    existingFamilleDevice.LibelleFamilleDevice = familleDevice.LibelleFamilleDevice;
                    existingFamilleDevice.URL = familleDevice.URL;
                    existingFamilleDevice.DateDebut = familleDevice.DateDebut;
                    existingFamilleDevice.DateFin = familleDevice.DateFin;

                    if (isModified)
                        existingFamilleDevice.DateModification = DateTime.Now;

                    repository.Update(existingFamilleDevice);
                }
                else
                {
                    repository.Create(familleDevice);
                }
            }
            catch (Exception e)
            {
                Logger.Error("origine id={0}, libelle={1}", familleDevice.Id, familleDevice.LibelleFamilleDevice);
                LogguerErreur(e);
                return null;
            }
            return familleDevice;
        }

        /// <summary>
        /// Creer/Modifier
        /// </summary>
        /// <param name="origine"></param>
        /// <returns></returns>
        public Origine SetOrigine(Origine origine)
        {
            var repository = new Repository<Origine>();
            try
            {
                if (repository.All().Any(o => o.Id == origine.Id))
                {
                    var existingOrigine = repository.All().First(p => p.Id == (origine.Id));
                    var isModified = existingOrigine.LibelleOrigine != origine.LibelleOrigine;
                    
                    existingOrigine.LibelleOrigine = origine.LibelleOrigine;
                    
                    if (isModified)
                        existingOrigine.DateModification = DateTime.Now;

                    repository.Update(existingOrigine);
                }
                else
                {
                    repository.Create(origine);
                }
            }
            catch (Exception e)
            {
                Logger.Error("origine id={0}, libelle={1}", origine.Id, origine.LibelleOrigine);
                LogguerErreur(e);
                return null;
            }
            return origine;
        }

        public Politique SetPolitique(Politique politique)
        {
            var repository = new Repository<Politique>();

            try
            {
                if (repository.All().Any(p => p.Id == politique.Id))
                {
                    var existingPolitique = repository.All().First(p => p.Id == (politique.Id));
                    var isModified = existingPolitique.LibellePolitique != politique.LibellePolitique ||
                                     existingPolitique.Objectif != politique.Objectif ||
                                     existingPolitique.Device != politique.Device ||
                                     existingPolitique.Benefice != politique.Benefice;

                    existingPolitique.LibellePolitique = politique.LibellePolitique;
                    existingPolitique.Objectif = politique.Objectif;
                    existingPolitique.Device = politique.Device;
                    existingPolitique.Benefice = politique.Benefice;

                    if (isModified)
                        existingPolitique.DateModification = DateTime.Now;
                    repository.Update(existingPolitique);
                }
                else
                {
                    var maxObjet = repository.All()
                                             .OrderByDescending(x => x.Id)
                                             .Take(1)
                        ;
                    var maxId = 1;
                    if (maxObjet.Any())
                    {
                        var maxPolitique = maxObjet.Single();
                        maxId = maxPolitique.Id;
                        politique.Id = maxId + 1;
                    }
                    repository.Create(politique);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Politique id={0}, libelle={1}", politique.Id, politique.LibellePolitique);
                LogguerErreur(e);
                return null;
            }

            return politique;
        }

        /// <summary>
        /// Creer/Modifier Ratio
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public Ratio SetRatio(Ratio ratio)
        {
            var repository = new Repository<Ratio>();
            try
            {
                if (repository.All().Any(r => (r.TypeDeviceId == ratio.TypeDeviceId &&
                                               r.SecteurActiviteId == ratio.SecteurActiviteId))
                    )
                {
                    var existingRatio = repository.All().First(r => (r.TypeDeviceId == ratio.TypeDeviceId &&
                                                                     r.SecteurActiviteId == ratio.SecteurActiviteId));
                    var isModified = existingRatio.Valeur != ratio.Valeur||                   
                    existingRatio.SecteurActiviteId != ratio.SecteurActiviteId ||
                    existingRatio.TypeRatioId != ratio.TypeRatioId;

                    existingRatio.Valeur = ratio.Valeur;
                    existingRatio.DateDebut = ratio.DateDebut;
                    existingRatio.DateFin = ratio.DateFin;
                    existingRatio.SecteurActiviteId = ratio.SecteurActiviteId;
                    existingRatio.TypeRatioId = ratio.TypeRatioId;

                    if (isModified)
                        existingRatio.DateModification = DateTime.Now;

                    repository.Update(existingRatio);
                }
                else
                {
                    repository.Create(ratio);
                }
            }
            catch (Exception e)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}", ratio.SecteurActiviteId, ratio.TypeDeviceId);
                LogguerErreur(e);
                return null;
            }
            return ratio;
        }

        public SecteurActivite SetSecteurActivite(SecteurActivite secteurActivite)
        {
            try
            {
                var repository = new Repository<SecteurActivite>();
                if (repository.All().Any(s => s.Id == secteurActivite.Id))
                {
                    var objetExistant = repository.All().First(s => s.Id == (secteurActivite.Id));
                    var isModified = objetExistant.LibelleSecteurActivite != secteurActivite.LibelleSecteurActivite ||
                                     objetExistant.SegmentActivite != secteurActivite.SegmentActivite ||
                                     objetExistant.TypeActivite != secteurActivite.TypeActivite;

                    objetExistant.LibelleSecteurActivite = secteurActivite.LibelleSecteurActivite;
                    objetExistant.SegmentActivite = secteurActivite.SegmentActivite;
                    objetExistant.TypeActivite = secteurActivite.TypeActivite;
                    objetExistant.DateDebut = secteurActivite.DateDebut;
                    objetExistant.DateFin = secteurActivite.DateFin;
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;
                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(secteurActivite);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleSecteurActivite={1}", secteurActivite.Id, secteurActivite.LibelleSecteurActivite);
                LogguerErreur(e);
                return null;
            }
            return secteurActivite;
        }

        public TypeContact SetTypeContact(TypeContact typeContact)
        {
            try
            {
                var repository = new Repository<TypeContact>();
                if (repository.All().Any(p => p.Id == typeContact.Id))
                {
                    var existingPolitique = repository.All().First(p => p.Id == typeContact.Id);
                    var isModified = existingPolitique.LibelleTypeContact != typeContact.LibelleTypeContact;
                    existingPolitique.LibelleTypeContact = typeContact.LibelleTypeContact;

                    if (isModified)
                        existingPolitique.DateModification = DateTime.Now;
                    repository.Update(existingPolitique);
                }
                else
                {
                    repository.Create(typeContact);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeContact={1}", typeContact.Id, typeContact.LibelleTypeContact);
                LogguerErreur(e);
                return null;
            }
            return typeContact;
        }

        public TypeDevice SetTypeDevice(TypeDevice typeDevice)
        {
            try
            {
                var repository = new Repository<TypeDevice>();
                if (repository.All().Any(c => c.Id == typeDevice.Id))
                {
                    var existingTypeDevice = repository.All().First(c => c.Id == typeDevice.Id);
                    var isModified = existingTypeDevice.CategorieDeviceId != typeDevice.CategorieDeviceId ||
                                     existingTypeDevice.LibelleTypeDevice != typeDevice.LibelleTypeDevice;

                    existingTypeDevice.CategorieDeviceId = typeDevice.CategorieDeviceId;
                    existingTypeDevice.LibelleTypeDevice = typeDevice.LibelleTypeDevice;
                    existingTypeDevice.URL = typeDevice.URL;
                    existingTypeDevice.DateDebut = typeDevice.DateDebut;
                    existingTypeDevice.DateFin = typeDevice.DateFin;
                    
                    if (isModified)
                        existingTypeDevice.DateModification = DateTime.Now;

                    repository.Update(existingTypeDevice);
                }
                else
                {
                    repository.Create(typeDevice);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeDevice={1}", typeDevice.Id, typeDevice.LibelleTypeDevice);
                LogguerErreur(e);
                return null;
            }
            return typeDevice;
        }

        public TypeObjet SetTypeObjet(TypeObjet typeObjet)
        {
            try
            {
                var repository = new Repository<TypeObjet>();
                if (repository.All().Any(p => p.Id == typeObjet.Id))
                {
                    var objetExistant = repository.All().First(p => p.NomObjet == (typeObjet.NomObjet));
                    objetExistant.NomTable = typeObjet.NomTable;
                    objetExistant.NomObjet = typeObjet.NomObjet;
                    objetExistant.Namespace = typeObjet.Namespace;                                       
                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(typeObjet);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, NomObjet={1}", typeObjet.Id, typeObjet.NomObjet);
                LogguerErreur(e);
                return null;
            }
            return typeObjet;
        }

        public TypeUsage SetTypeUsage(TypeUsage typeUsage)
        {
            if (typeUsage == null)
                return null;

            try
            {
                var repository = new Repository<TypeUsage>();
                if (repository.All().Any(p => p.Id == (typeUsage.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeUsage.Id));
                    var isModified = objetExistant.LibelleTypeUsage != typeUsage.LibelleTypeUsage;

                    objetExistant.LibelleTypeUsage = typeUsage.LibelleTypeUsage;
                    objetExistant.DateDebut = typeUsage.DateDebut;
                    objetExistant.DateFin = typeUsage.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;
                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(typeUsage);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeUsage={1}", typeUsage.Id, typeUsage.LibelleTypeUsage);
                LogguerErreur(e);
                return null;
            }
            return typeUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ventilationClasseAgeDevice"></param>
        /// <returns></returns>
        public VentilationClasseAgeDevice SetVentilationClasseAgeDevice(
            VentilationClasseAgeDevice ventilationClasseAgeDevice)
        {
            try
            {
                var repository = new Repository<VentilationClasseAgeDevice>();
                if (repository.All().Any(c => (c.ClasseDeviceId == (ventilationClasseAgeDevice.ClasseDeviceId) &&
                                               c.AgeDeviceId == ventilationClasseAgeDevice.AgeDeviceId &&
                                               c.TypeDeviceId == ventilationClasseAgeDevice.TypeDeviceId &&
                                               c.SecteurActiviteId == ventilationClasseAgeDevice.SecteurActiviteId)
                    ))
                {
                    var existingVentilationClasseDevice = repository.All().First(
                        c => (c.ClasseDeviceId == ventilationClasseAgeDevice.ClasseDeviceId &&
                              c.AgeDeviceId == ventilationClasseAgeDevice.AgeDeviceId &&
                              c.TypeDeviceId == ventilationClasseAgeDevice.TypeDeviceId &&
                              c.SecteurActiviteId == ventilationClasseAgeDevice.SecteurActiviteId));
                    var isModified = existingVentilationClasseDevice.Coefficient != ventilationClasseAgeDevice.Coefficient;
                    existingVentilationClasseDevice.Coefficient = ventilationClasseAgeDevice.Coefficient;
                    existingVentilationClasseDevice.DateDebut = ventilationClasseAgeDevice.DateDebut;
                    existingVentilationClasseDevice.DateFin = ventilationClasseAgeDevice.DateFin;
                    if (isModified)
                        existingVentilationClasseDevice.DateModification = DateTime.Now;
                    repository.Update(existingVentilationClasseDevice);
                }
                else
                {
                    repository.Create(ventilationClasseAgeDevice);
                }
            }
            catch (Exception e)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}, AgeDeviceId={3}", ventilationClasseAgeDevice.SecteurActiviteId, ventilationClasseAgeDevice.TypeDeviceId, ventilationClasseAgeDevice.ClasseDeviceId, ventilationClasseAgeDevice.AgeDeviceId);
                LogguerErreur(e);               
                return null;
            }
            return ventilationClasseAgeDevice;
        }

        public VentilationClasseDevice SetVentilationClasseDevice(VentilationClasseDevice ventilationClasseDevice)
        {
            try
            {
                var repository = new Repository<VentilationClasseDevice>();
                if (repository.All().Any(c => c.ClasseDeviceId == (ventilationClasseDevice.ClasseDeviceId) &&
                                              (c.SecteurActiviteId == (ventilationClasseDevice.SecteurActiviteId)) &&
                                              (c.TypeDeviceId == (ventilationClasseDevice.TypeDeviceId)))
                    )
                {
                    var existingVentilationClasseDevice = repository.All().First(
                        c => c.ClasseDeviceId == (ventilationClasseDevice.ClasseDeviceId) &&
                             (c.SecteurActiviteId == (ventilationClasseDevice.SecteurActiviteId)) &&
                             (c.TypeDeviceId == (ventilationClasseDevice.TypeDeviceId))
                        );
                    var isModified = existingVentilationClasseDevice.Coefficient != ventilationClasseDevice.Coefficient;
                    
                    existingVentilationClasseDevice.Coefficient = ventilationClasseDevice.Coefficient;
                    existingVentilationClasseDevice.DateDebut = ventilationClasseDevice.DateDebut;
                    existingVentilationClasseDevice.DateFin = ventilationClasseDevice.DateFin;

                    if (isModified)
                        existingVentilationClasseDevice.DateModification = DateTime.Now;
                    repository.Update(existingVentilationClasseDevice);
                }
                else
                {
                    repository.Create(ventilationClasseDevice);
                }
            }
            catch (Exception e)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}", ventilationClasseDevice.SecteurActiviteId, ventilationClasseDevice.TypeDeviceId, ventilationClasseDevice.ClasseDeviceId);
                LogguerErreur(e);
                return null;
            }
            return ventilationClasseDevice;
        }

        #endregion

        /// <summary>
        /// Recupere la liste des pays
        /// </summary>
        /// <returns>la liste des pays</returns>
        public List<Pays> ListePays(int currentPage, int pageDimension, out int totalPages)
        {
            totalPages = 1;
            try
            {
                var repository = new Repository<Pays>(_unitOfWork);
                totalPages = repository.All().Count();
                if (pageDimension > 0 && currentPage>=0)
                {
                    totalPages = (int) Math.Ceiling((double)totalPages/pageDimension);
                    return repository.All().OrderBy(o=>o.LibellePays).Skip(currentPage*pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        /// <summary>
        /// Recupere la liste des origines
        /// </summary>
        /// <returns></returns>
        public List<Origine> ListeOrigines(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Origine>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();                         
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des TypeObjets
        /// </summary>
        /// <returns>la liste des TypeObjets</returns>
        public List<TypeObjet> ListeTypeObjet(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<TypeObjet>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Mis a jour du contenu
        /// </summary>
        /// <param name="editedPublishedContent"></param>
        /// <returns></returns>
        public EditPublishedContent UpdatePublishedContent(EditPublishedContent editedPublishedContent)
        {
            try
            {
                var publishedContentRepository = new Repository<ContenuModere>();

                List<ContenuModere> publishedContentList = editedPublishedContent.ContenuModeres;

                foreach (ContenuModere item in publishedContentList)
                {
                    ContenuModere publishedContent = publishedContentRepository.Find(item.Id);
                    publishedContent.Contenu = item.Contenu;
                    publishedContent.DateModification = DateTime.Now;
                    publishedContentRepository.Update(publishedContent);
                    
                    _unitOfWork.SaveChanges();
                }

                return editedPublishedContent;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des pages
        /// </summary>
        /// <returns></returns>
        public List<Page> GetPages()
        {
            try
            {
                var repository = new Repository<Page>();

                var liste = repository.All().ToList();

                return liste;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des adresses
        /// </summary>
        /// <returns></returns>
        public List<Adresse> ListeAdresse()
        {
            try
            {
                var repository = new Repository<Adresse>();

                var liste = repository.All().ToList();

                return liste;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des ConsoWattHeur
        /// </summary>
        /// <returns>la liste des ConsoWattHeur</returns>
        public List<ConsoWattHeur> ListeConsoWattHeur(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<ConsoWattHeur>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.TypeDeviceId).ThenBy(o=>o.ClasseDeviceId).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarder un client
        /// </summary>
        /// <param name="client">Le client a sauvegarder</param>
        /// <returns>Le nouveau client</returns>
        public Client SauvegardeClient(Client client)
        {
            if (client == null)
            {
                Logger.Debug("client null");
                return null;
            }

            try
            {
                Logger.Debug("client raison sociale {0}", client.RaisonSociale);
                client.Statut = (int)ClientStatut.EnCours;
                client.DateDebut = DateTime.Now;
                client.DateFin = DateTime.Now.AddYears(1);
                var clientRepository = new Repository<Client>();
                clientRepository.Create(client);
                _unitOfWork.SaveChanges();
                return client;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        ///  Recupere la liste des langues
        /// </summary>
        /// <returns>la liste des langues</returns>
        public List<Langue> ListeLangues(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Langue>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des contenus
        /// </summary>
        /// <param name="nomDePage"></param>
        /// <param name="cultureDeLangue"></param>
        /// <returns></returns>
        public List<ContenuModere> ListeContenuPublier(string nomDePage, string cultureDeLangue)
        {
            try
            {
                var repository = new Repository<ContenuModere>();
                var liste =
                    repository.Filter(
                        publishedContent =>
                        publishedContent.Page.Nom == nomDePage && publishedContent.Langue.Culture == cultureDeLangue)
                              .Include(o => o.Page)
                              .Include(o => o.CleModele)
                              .Include(o => o.Langue)
                              .ToList();
                return liste;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public ContenuModereViewModel GetContenuPublier(string lien, string nomDePage, string cultureDeLangue)
        {
            try
            {
                var repository = new Repository<ContenuModere>();
                if (
                    repository.Filter(
                        o => o.Page.Nom == nomDePage && o.Page.Lien == lien && (o.Langue.Culture == cultureDeLangue || o.Langue.Code == cultureDeLangue))
                              .Include(o => o.Page).Any())
                {
                    var resultat =
                        repository.Filter(
                            o => o.Page.Nom == nomDePage && o.Page.Lien == lien && (o.Langue.Culture == cultureDeLangue || o.Langue.Code==cultureDeLangue))
                                  .Include(o => o.Page).Single();
                    if (resultat != null)
                    {
                        var contenuModererViewModel = new ContenuModereViewModel();
                        contenuModererViewModel.Id = resultat.Id;
                        contenuModererViewModel.Contenu = resultat.Contenu;
                        contenuModererViewModel.DateDebut = resultat.DateDebut;
                        contenuModererViewModel.Page = resultat.Page;
                        contenuModererViewModel.PageId = resultat.PageId;
                        contenuModererViewModel.CleModeleId = resultat.CleModeleId;
                        contenuModererViewModel.LangueId = resultat.LangueId;

                        return contenuModererViewModel;
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public Client ActualiserClient(Client client)
        {
            try
            {
                client.Statut = (int)ClientStatut.Valide;
                var clientRepository = new Repository<Client>();
                client.DateModification = DateTime.Now;
                clientRepository.Update(client);
                _unitOfWork.SaveChanges();
                return client;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }


        public List<Page> ListePage(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Page>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarde nouveau contenu
        /// </summary>
        /// <param name="contenuModere">Nouveau contenu a sauevgarder</param>
        /// <returns>Le nouveau contenu</returns>
        public ContenuModere SauvegardeContenuModerer(ContenuModere contenuModere)
        {
            try
            {
                Logger.Debug("contenuModere LangueId {0} PageId {1}", contenuModere.LangueId, contenuModere.PageId);
                var repository = new Repository<ContenuModere>();
                repository.Create(contenuModere);
                _unitOfWork.SaveChanges();
                return contenuModere;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Actualiser un contenu
        /// </summary>
        /// <param name="contenu">Contenu a actualiser</param>
        /// <returns>Contenu actualise</returns>
        public ContenuModere MAJContenuModere(ContenuModere contenu)
        {
            try
            {
                Logger.Debug("contenuModere Id {0} LangueId {1} PageId {2}",contenu.Id, contenu.LangueId, contenu.PageId);

                var repository = new Repository<ContenuModere>();
                if (contenu != null)
                {
                    var contenuModere = repository.Find(contenu.Id);

                    if (contenuModere != null && contenuModere.LangueId == contenu.LangueId)
                    {
                        contenuModere.Contenu = contenu.Contenu;
                        contenuModere.DateModification = DateTime.Now;
                        repository.Update(contenuModere);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        var contenuModerer = new ContenuModere
                            {
                                Contenu = contenu.Contenu,
                                PageId = contenu.PageId,
                                DateDebut = DateTime.Now,
                                LangueId = contenu.LangueId
                            };
                        contenuModere = SauvegardeContenuModerer(contenuModerer);
                    }
                    return contenuModere;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }
        
        /// <summary>
        /// Recupere la liste des clients non moderer
        /// </summary>
        /// <param name="page">Une page dans la pagination</param>
        /// <param name="pageDimension">Le nombre de clients par page</param>
        /// <param name="totalPages">Le nombre total des pages</param>
        /// <returns></returns>
        public List<Client> ClientsNonModerer(int? page, int pageDimension, out int totalPages)
        {
            try
            {
                Logger.Debug("page {0}, pageDimension {1}", page, pageDimension);
                totalPages = 0;
                var repository = new Repository<Client>();
                var clients = repository.Filter(o => o.Statut == (int)ClientStatut.EnCours)
                                        .Include(o => o.Adresse)
                                        .Include(o => o.Adresse.Pays)
                                        .OrderBy(o => o.Id);

                totalPages = clients.Count();
                if (page.HasValue)
                {
                    clients = (IOrderedQueryable<Client>)clients.Skip((page.Value - 1) * pageDimension).Take(pageDimension);
                }
                else
                {
                    clients = clients.Take(pageDimension) as IOrderedQueryable<Client>;
                }
                return clients.ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }           
        }

        /// <summary>
        /// Recupere un client
        /// </summary>
        /// <param name="id">L'identifiant du client</param>
        /// <returns>Le client</returns>
        public Client GetClientParId(int id)
        {
            try
            {
                var repository = new Repository<Client>();
                var client = repository.Filter(c => c.Id == id)
                                       .Include(c => c.Adresse)
                                       .Include(c => c.Adresse.Pays)
                                       .Include(c => c.DetailsClient)
                                       .Include(c => c.DetailsClient.Tarif)
                                       .Include(c => c.DetailsClient.Devise)
                                       .Include(c => c.DetailsClient.Devise.DeviseTraductions)
                                       .Include(c => c.Contacts)
                                       .Include(c => c.SecteurActivite);
                return client.Any() ? client.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }            
        }

        /// <summary>
        /// Recupere la liste des pays
        /// </summary>
        /// <returns>la liste des pays</returns>
        public List<Pays> GetPays()
        {
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var repository = new Repository<Pays>();
                var pays = repository.All()
                              .Select(t => new { Pays = t, PaysTraductions = t.PaysTraductions.Where(l => l.LangueId == langueId) }).AsEnumerable().Select(t => t.Pays)
                              .ToList();

                return pays;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des sections
        /// </summary>
        /// <returns>la liste des sections</returns>
        public List<Section> ListeSection(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Section>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().Include(s => s.Parent)
                                     .Include(s => s.Sections)
                                     .Include(s => s.Pages).OrderBy(o=>o.Id).Skip(currentPage*pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All()
                                     .Include(s => s.Parent)
                                     .Include(s => s.Sections)
                                     .Include(s => s.Pages)
                                     .ToList();

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Actualiser un contact
        /// </summary>
        /// <param name="contact">Le contact a actualiser</param>
        /// <returns>Le contact actualise</returns>
        public Contact MAJContact(Contact contact)
        {
            try
            {
                var repository = new Repository<Contact>();
                contact.DateModification = DateTime.Now;
                repository.Update(contact);
                _unitOfWork.SaveChanges();
                return contact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// Recupere un contact
        /// </summary>
        /// <param name="verificationCode">Le code de verification du contact</param>
        /// <returns>Le contact</returns>
        public Contact GetContactParVerificationCode(string verificationCode)
        {
            try
            {
                var repository = new Repository<Contact>();
                var contact =
                    repository.Filter(c => c.CodeVerification == verificationCode)
                              .Include(c => c.Client)
                              .Include(c => c.Reponses)
                              .Single();
                return contact;
            }

            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des questions
        /// </summary>
        /// <returns>la liste des questions</returns>
        public List<Question> GetQuestions()
        {
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var repository = new Repository<Question>();
                var questions = repository.All()
                                .Select(t => new { Question = t, QuestionTraductions = t.QuestionTraductions.Where(l => l.LangueId == langueId) }).AsEnumerable().Select(t => t.Question)
                              .ToList();
                return questions;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Actualiser une Reponse
        /// </summary>
        /// <param name="reponse">Reponse a actualiser</param>
        /// <returns>Reponse actualisee</returns>
        public Reponse SetReponse(Reponse reponse)
        {
            try
            {
                Logger.Debug("reponse Id {0}", reponse.Id);
                var reponseRepository = new Repository<Reponse>();
                reponseRepository.Create(reponse);
                _unitOfWork.SaveChanges();
                return reponse;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionViewModel"></param>
        /// <returns></returns>
        public bool SauvegardeSection(SectionViewModel sectionViewModel)
        {
            try
            {
                var section = new Section();
                section.Nom = sectionViewModel.Nom;
                section.ParentId = sectionViewModel.IdParent == 0 ? (int?) null : sectionViewModel.IdParent;
                var sectionRepository = new Repository<Section>();
                sectionRepository.Create(section);
                _unitOfWork.SaveChanges();
                return true;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Section GetSection(int id)
        {
            try
            {
                var repository = new Repository<Section>();
                var section = repository.Filter(o => o.Id == id)
                                        .Include(s => s.Pages);
                return section.Any() ? section.Single() : null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageViewModel"></param>
        /// <returns></returns>
        public bool SauvegardePage(PageViewModel pageViewModel)
        {
            try
            {
                var page = new Page();
                page.Nom = pageViewModel.Nom;
                page.Actif = pageViewModel.Actif;
                page.Lien = pageViewModel.Lien;
                //page.ModeleNonModereId = pageViewModel.IdModele;
                page.SectionId = pageViewModel.SectionId;
                var repository = new Repository<Page>();
                repository.Create(page);
                _unitOfWork.SaveChanges();
                return true;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Modele> ListeModele(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Modele>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Tarif> GetListOfTarifs()
        {           
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var repository = new Repository<Tarif>();                

                var liste=
                    repository.All()
                              .Select(t => new { Tarif = t, TarifTraductions = t.TarifTraductions.Where(l => l.LangueId == langueId) }).AsEnumerable().Select(t => t.Tarif)
                              .ToList();
                
                return liste;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adresse"></param>
        /// <returns></returns>
        public Adresse MAJAdresse(Adresse adresse)
        {
            try
            {
                var repository = new Repository<Adresse>();               
                repository.Update(adresse);
                _unitOfWork.SaveChanges();
                return adresse;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Client> ClientsModerer()
        {
            try
            {

                var repository = new Repository<Client>();
                var liste = repository.Filter(c => c.Statut == (int)ClientStatut.Valide).Include(c => c.Adresse)
                                      .ToList();

                return liste;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        public Client GetClientParNom(string nom)
        {
            try
            {
                var repository = new Repository<Client>();
                var client = repository.Filter(o => o.Nom == nom).Include(o => o.Contacts.Select(c => c.TypeContact));
                return client.Any() ? client.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        public List<Contact> GetContactParNomClient(string nom)
        {
            try
            {
                var repository = new Repository<Contact>();
                List<Contact> contacts =
                    repository.Filter(o => o.Client.Nom == (nom)).Include(o => o.TypeContact).ToList();
                return contacts;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactsViewModel"></param>
        /// <returns></returns>
        public bool MAJContact(ContactsViewModel contactsViewModel)
        {
            try
            {
                var repository = new Repository<Contact>();

                List<Contact> contacts = contactsViewModel.Contacts;

                foreach (var item in contacts)
                {
                    Contact contact = repository.Find(item.Id);
                    contact.Email = item.Email;
                    contact.DateModification = DateTime.Now;
                    repository.Update(contact);
                    _unitOfWork.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public Utilisateur SetUtilisateur(Utilisateur utilisateur)
        {
            try
            {
                var repository = new Repository<Utilisateur>();
                utilisateur.DateModification = DateTime.Now;
                repository.Update(utilisateur);
                _unitOfWork.SaveChanges();
                return utilisateur;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Politique> ListePolitique(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Politique>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="politiqueId"></param>
        /// <returns></returns>
        public Politique GetPolitiqueParId(int politiqueId)
        {
            try
            {
                var repository = new Repository<Politique>();
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var politique =
                  repository.All()
                                    .Select(
                                        q =>
                                        new
                                        {
                                            Politique = q,
                                            PolitiqueTraductions =
                                        q.PolitiqueTraductions.Where(l => l.LangueId == langueId)
                                        }).AsEnumerable()
                                    .Select(t => t.Politique)
                                    .Where(t => t.Id == politiqueId);
              

                if (politique.Any())
                    return politique.FirstOrDefault();
                else
                    return null;
                
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libelle"></param>
        /// <returns></returns>
        public TypeContact GetTypeContactParLibelle(string libelle)
        {
            var repository = new Repository<TypeContact>();
            var typeContact = repository.Filter(o => o.LibelleTypeContact == libelle);
            return typeContact.Any() ? typeContact.Single() : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compteEvaluation"></param>
        /// <returns></returns>
        public CompteEvaluation SauvegardeEvaluation(CompteEvaluation compteEvaluation)
        {
            try
            {
                var sectionRepository = new Repository<CompteEvaluation>();
                compteEvaluation = sectionRepository.Create(compteEvaluation);
                _unitOfWork.SaveChanges();
                return compteEvaluation;

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SecteurActivite> ListeSecteurActivite(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {              
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var repository = new Repository<SecteurActivite>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All()
                        .Select(t => new { SecteurActivite = t, SecteurActiviteTraductions = t.SecteurActiviteTraductions.Where(l => l.LangueId == langueId) }).AsEnumerable().Select(t => t.SecteurActivite)
                        .ToList();               
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeObjet> GetListeTypeObjet()
        {
            try
            {
                var repository = new Repository<TypeObjet>();
                IQueryable<TypeObjet> listeTypeObjet = repository.All().Where(
                    x =>
                    !(x.NomTable.Equals("TypeObjet") || x.NomTable.Equals("Routage") ||
                       x.NomTable.Equals("CleModele") || x.NomTable.Equals("Section") ||
                       x.NomTable.Equals("ContenuModere") || x.NomTable.Equals("ContenuMail") ||
                       x.NomTable.Equals("Modele") || x.NomTable.Equals("Page") ||
                       x.NomTable.Equals("PolitiqueTraduction") ||
                       x.NomTable.Equals("EquivalenceTraduction")));             

                return listeTypeObjet.OrderBy(o => o.NomTable).ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Langue GetLangue(int id)
        {
            try
            {
                var repository = new Repository<Langue>();
                var langue = repository.Find(id);
                return langue;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiDeviceId"></param>
        /// <returns></returns>
        public ConfigSiDevice GetConfigSiDevice(int configSiDeviceId)
        {
            try
            {
                var repository = new Repository<ConfigSiDevice>();
                var configSiDevice = repository.Filter(o => o.Id == configSiDeviceId)
                                               .Include(o => o.ConfigSiConsommations);
                return configSiDevice.Any() ? configSiDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiDevice"></param>
        /// <returns></returns>
        public ConfigSiDevice SauveGarderConfigSiDevice(ConfigSiDevice configSiDevice)
        {
            try
            {
                var repository = new Repository<ConfigSiDevice>();
                repository.Create(configSiDevice);
                _unitOfWork.SaveChanges();
                return configSiDevice;

            }
            catch (Exception e)
            {
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiDevice"></param>
        /// <returns></returns>
        public ConfigSiDevice MAJConfigSiDevice(ConfigSiDevice configSiDevice)
        {
            try
            {
                var repository = new Repository<ConfigSiDevice>();
                Logger.Debug("MAJConfigSiDevice configSiDevice {0} quantite {1}", configSiDevice.Id, configSiDevice.Quantite);
                configSiDevice.DateModification = DateTime.Now;
                
               var statut = repository.Update(configSiDevice);
                //_unitOfWork.SaveChanges();
                return configSiDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        //public ConfigSiDevice MAJConfigSiDeviceChild(ConfigSiDevice configSiDevice)
        //{
        //    try
        //    {
        //        var repository = new Repository<ConfigSiDevice>(_unitOfWork);
        //        Logger.Debug("MAJConfigSiDevice configSiDevice {0} quantite {1}", configSiDevice.Id, configSiDevice.Quantite);
        //        configSiDevice.DateModification = DateTime.Now;

        //        var repositoryConso = new Repository<ConfigSiConsommation>(_unitOfWork);
        //        foreach (var conso in configSiDevice.ConfigSiConsommations)
        //        {
        //            repositoryConso.Update(conso);
        //        }
        //        var statut = repository.Update(configSiDevice);
        //        _unitOfWork.SaveChanges();
        //        return configSiDevice;
        //    }
        //    catch (Exception e)
        //    {
        //        LogguerErreur(e);
        //        throw;
        //    }
        //}
       
        public ConfigSi MAJConfigSiDetail(ConfigSi configSi)
        {
            try
            {
                var repositoryDevice = new Repository<ConfigSiDevice>(_unitOfWork);                
                var repositoryConso = new Repository<ConfigSiConsommation>(_unitOfWork);

                foreach (var configSiDevice in configSi.ConfigSiDevices)
                {
                    foreach (var conso in configSiDevice.ConfigSiConsommations)
                    {
                        repositoryConso.Update(conso);
                    }
                    repositoryDevice.Update(configSiDevice);
                }   
                            
                var repositoryConfigSi = new Repository<ConfigSi>(_unitOfWork);
                repositoryConfigSi.Update(configSi);
                _unitOfWork.SaveChanges();
                return configSi;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public int MAJConfigSiResultat(ConfigSi configSi, decimal coefficientConversion)
        {
            try
            {
                var repository = new Repository<ConfigSi>();
                var sb = new StringBuilder();
                sb.Append(" Update BENCHMARK.Resultat ");
                sb.Append(" Set CoefficientConversion=@CoefficientConversion, ConsommationCo2=((ConsommationWatt*@CoefficientConversion)/1000) ");
                sb.Append(" Where ConfigSiId=@ConfigSiId");

                var sql = sb.ToString();
                var result = repository.ExecuteCommand(sql, new SqlParameter("CoefficientConversion", coefficientConversion), new SqlParameter("ConfigSiId", configSi.Id));
                return result;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public int MAJConfigSiConsommation(ConfigSi configSi, decimal coefficientConversion)
        {
            try
            {
                var repository = new Repository<ConfigSi>();
                var sb = new StringBuilder();
                sb.Append(" Update BENCHMARK.ConfigSiConsommation ");
                sb.Append(" Set CoefficientConversion=@CoefficientConversion, ConsommationCo2=((ConsommationWatt*@CoefficientConversion)/1000) ");
                sb.Append(" Where ConfigSiDeviceId in (Select IdConfigSiDevice from BENCHMARK.ConfigSiDevice where ConfigSiId=@ConfigSiId)");

                var sql = sb.ToString();               
                var result =repository.ExecuteCommand(sql, new SqlParameter("CoefficientConversion",coefficientConversion ), new SqlParameter("ConfigSiId", configSi.Id));
                return result;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiConsommation"></param>
        /// <returns></returns>
        public ConfigSiConsommation SauveGarderConfigSiConsommation(ConfigSiConsommation configSiConsommation)
        {
            try
            {
                var repository = new Repository<ConfigSiConsommation>();
                repository.Create(configSiConsommation);
                _unitOfWork.SaveChanges();
                return configSiConsommation;

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CompteEvaluation GetCompteEvaluation(int id)
        {
            try
            {
                var repository = new Repository<CompteEvaluation>();
                var compteEvaluation = repository.Filter(o => o.Id == id)
                                                 .Include(o => o.SecteurActivite)
                                                 .Include(o => o.Pays)
                                                 .Include(o => o.Devise)
                                                 .Include(o => o.Devise.DeviseTraductions);
                return compteEvaluation.Any() ? compteEvaluation.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secteurActiviteId"></param>
        /// <returns></returns>
        public List<FamilleDevice> GetFamilleDeviceParSecteurActivite(int secteurActiviteId)
        {            
            try
            {
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager GetFamilleDeviceParSecteurActivite ");
                var langueId = 1;                
                var familleRepository = new Repository<FamilleDevice>();              
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var listefamilleDevice = familleRepository.All().Select(f => new { FamilleDevice = f, FamilleDeviceTraductions = f.FamilleDeviceTraductions.Where(ft => ft.LangueId == langueId) }).AsEnumerable().Select(r => r.FamilleDevice).ToList();
                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager GetFamilleDeviceParSecteurActivite ");
                return listefamilleDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="familleDeviceId"></param>
        /// <returns></returns>
        public List<CategorieDevice> GetCategorieDeviceParFamille(int familleDeviceId)
        {
            try
            {
                var langueId = 1;
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager GetCategorieDeviceParFamille ");
                var categorieRepository = new Repository<CategorieDevice>();
                
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var listeCategorieDevice = categorieRepository.All().Select(c => new { CategorieDevice = c, CategorieDeviceTraductions = c.CategorieDeviceTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.CategorieDevice).Where(c => c.FamilleDeviceId == familleDeviceId).ToList();

                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager GetCategorieDeviceParFamille ");
                return listeCategorieDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categorieDeviceId"></param>
        /// <param name="secteurActiviteId"></param>
        /// <returns></returns>
        public List<TypeDevice> GetTypeDeviceParCategorie(int categorieDeviceId, int secteurActiviteId)
        {
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var typeDeviceRepository = new Repository<TypeDevice>();

               
                var liste = typeDeviceRepository.All()
                                                   .Where(
                                                       c =>
                                                       c.CategorieDeviceId == categorieDeviceId &&
                                                       c.Ratios.Any(r => r.SecteurActiviteId == secteurActiviteId))
                                                   .Select(
                                                       t =>
                                                       new
                                                           {
                                                               TypeDevice = t,
                                                               TypeDeviceTraductions = t.TypeDeviceTraductions.Where(ft => ft.LangueId == langueId)
                                                           })
                                                   .ToList()
                                                   .Select(a => a.TypeDevice).ToList();

                return liste;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secteurActiviteId"></param>
        /// <param name="typeDeviceId"></param>
        /// <returns></returns>
        public Ratio GetRatio(int secteurActiviteId, int typeDeviceId)
        {
            try
            {
                var ratioRepository = new Repository<Ratio>();
                var ratio = ratioRepository.Filter(
                    r => (r.TypeDeviceId == typeDeviceId && r.SecteurActiviteId == secteurActiviteId))
                                           .Include(r => r.TypeRatio);
                return ratio.Any() ? ratio.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secteurActiviteId"></param>
        /// <param name="typeDeviceId"></param>
        /// <returns></returns>
        public List<VentilationClasseDevice> GetVentilationClasseDeviceParTypeDevice(int secteurActiviteId,
                                                                                     int typeDeviceId)
        {
            try
            {
                var ventilationClasseDeviceRepository = new Repository<VentilationClasseDevice>();
                var listeVentilationClasseDevice = ventilationClasseDeviceRepository.Filter(
                    v => (v.TypeDeviceId == typeDeviceId && v.SecteurActiviteId == secteurActiviteId)).ToList();
                return listeVentilationClasseDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secteurActiviteId"></param>
        /// <param name="typeDeviceId"></param>
        /// <returns></returns>
        public List<VentilationClasseAgeDevice> GetVentilationClasseAgeDeviceParTypeDevice(int secteurActiviteId,
                                                                                           int typeDeviceId)
        {
            try
            {
                var ventilationClasseAgeDeviceRepository = new Repository<VentilationClasseAgeDevice>();
                var listeVentilationClasseAgeDevice = ventilationClasseAgeDeviceRepository.Filter(
                    v => (v.TypeDeviceId == typeDeviceId && v.SecteurActiviteId == secteurActiviteId)).ToList();
                return listeVentilationClasseAgeDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDeviceId"></param>
        /// <returns></returns>
        public Usage GetUsageParTypeDevice(int typeDeviceId)
        {
            try
            {
                var repositoryUsageDevice = new Repository<UsageDevice>();
                var usageDevice =
                    repositoryUsageDevice.Filter(o => o.TypeDeviceId == typeDeviceId).Include(o => o.Usage).Single();

                var repositoryUsage = new Repository<Usage>();
                var usage = repositoryUsage.Filter(u => u.Id == usageDevice.UsageId).Include(u => u.TypeUsage).Single();

                return usage;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDeviceId"></param>
        /// <returns></returns>
        public List<ConsoWattHeur> GetConsoWattHeur(int typeDeviceId)
        {
            try
            {
                var repository = new Repository<ConsoWattHeur>();
                var listeConsoWattHeur = repository.Filter(o => o.TypeDeviceId == typeDeviceId).ToList();
                return listeConsoWattHeur;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secteurActiviteId"></param>
        /// <returns></returns>
        public ClasseDevice GetClasseDeviceParSecteurActiviteId(int secteurActiviteId)
        {
            var classeDeviceRepository = new Repository<ClasseDevice>();
            var classeDevice = classeDeviceRepository.Filter(c => c.SecteurActiviteId == secteurActiviteId);
            return classeDevice.Any() ? classeDevice.Single() : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiConsommation"></param>
        /// <returns></returns>
        public ConfigSiConsommation MAJConfigSiConsommation(ConfigSiConsommation configSiConsommation)
        {
            try
            {
                var repository = new Repository<ConfigSiConsommation>();
                configSiConsommation.DateModification = DateTime.Now;
                var statut = repository.Update(configSiConsommation);
                //_unitOfWork.SaveChanges();
                return configSiConsommation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiConsommation"></param>
        /// <returns></returns>
        public ConfigSiConsommation MAJConfigSiBatchConsommation(ConfigSiConsommation configSiConsommation)
        {
            try
            {
                var repository = new Repository<ConfigSiConsommation>();
                var consommation = repository.Find(configSiConsommation.Id);
                Logger.Debug("MAJConfigSiBatchConsommation consommation {0} quantite {1}",configSiConsommation.Id, configSiConsommation.Quantite);
                consommation.Quantite = configSiConsommation.Quantite;
                consommation.ConsommationCo2 = configSiConsommation.ConsommationCo2;
                consommation.ConsommationWatt = configSiConsommation.ConsommationWatt;
                consommation.DateModification = configSiConsommation.DateModification;
                var statut = repository.Update(consommation);
                //_unitOfWork.SaveChanges();
                return configSiConsommation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiDeviceId"></param>
        /// <param name="classeDeviceId"></param>
        /// <param name="ageDeviceId"></param>
        /// <returns></returns>
        public ConfigSiConsommation GetConfigSiConsommation(int configSiDeviceId, int classeDeviceId, int ageDeviceId)
        {
            var repository = new Repository<ConfigSiConsommation>();
            var configSiConsommation =
                repository.Filter(
                    o =>
                    o.ConfigSiDeviceId == configSiDeviceId && o.ClasseDeviceId == classeDeviceId &&
                    o.AgeDeviceId == ageDeviceId);
            return configSiConsommation.Any() ? configSiConsommation.Single() : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Page GetPage(int id)
        {
            try
            {
                var repository = new Repository<Page>();
                var page = repository.Filter(o => o.Id == id).Single();
                return page;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Methode pour ajouter un tarif
        /// </summary>
        /// <param name="tarif"></param>
        /// <returns>Le tarif ajoute/modifie</returns>
        public Tarif SetTarif(Tarif tarif)
        {
            try
            {
                var repository = new Repository<Tarif>();
                if (repository.All().Any(p => p.Id == (tarif.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (tarif.Id));
                    var isModified = objetExistant.LibelleTarif != tarif.LibelleTarif ||
                                     objetExistant.NbreMinDevice != tarif.NbreMinDevice ||
                                     objetExistant.NbreMaxDevice != tarif.NbreMaxDevice ||
                                     objetExistant.TarifAnnuel != tarif.TarifAnnuel;

                    objetExistant.LibelleTarif = tarif.LibelleTarif;
                    objetExistant.NbreMinDevice = tarif.NbreMinDevice;
                    objetExistant.NbreMaxDevice = tarif.NbreMaxDevice;
                    objetExistant.TarifAnnuel = tarif.TarifAnnuel;
                    objetExistant.DateFin = tarif.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;
                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(tarif);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return tarif;
        }

        /// <summary>setconfigsi
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public Question SetQuestion(Question question)
        {
            try
            {
                var repository = new Repository<Question>();
                if (repository.All().Any(p => p.Id == (question.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (question.Id));
                    var isModified = objetExistant.LibelleQuestion != question.LibelleQuestion;
                    objetExistant.LibelleQuestion = question.LibelleQuestion;
                    objetExistant.DateFin = question.DateFin;
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(question);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return question;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contenuMail"></param>
        /// <returns></returns>
        public ContenuMail SetContenuMail(ContenuMail contenuMail)
        {
            try
            {
                var repository = new Repository<ContenuMail>();
                if (repository.All().Any(p => (p.Id == contenuMail.Id && p.LangueId==contenuMail.LangueId) ))
                {
                    var objetExistant = repository.All().First(p => (p.Id == contenuMail.Id && p.LangueId == contenuMail.LangueId));
                    var isModified = objetExistant.Contenu != contenuMail.Contenu ||
                                     objetExistant.Sujet != contenuMail.Sujet;

                    objetExistant.Contenu = contenuMail.Contenu;
                    objetExistant.Sujet = contenuMail.Sujet;
                    objetExistant.DateFin = contenuMail.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    if (contenuMail.DateDebut == new DateTime())
                        contenuMail.DateDebut = DateTime.Now;
                    repository.Create(contenuMail);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return contenuMail;
        }

        /// <summary>
        /// Methode pour sauvegarder un objet ChangementAnneeCalendaire
        /// </summary>
        /// <param name="typeMail">l'objet a sauvegarder</param>
        /// <returns>l'objet sauvegardee</returns>
        public object SetTypeMail(TypeMail typeMail)
        {
            try
            {
                var repository = new Repository<TypeMail>();
                if (repository.All().Any(p => p.Id == (typeMail.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeMail.Id));
                    var isModified = objetExistant.LibelleTypeMail != typeMail.LibelleTypeMail;
                    objetExistant.LibelleTypeMail = typeMail.LibelleTypeMail;
                    objetExistant.DateFin = typeMail.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    if (typeMail.DateDebut == new DateTime())
                        typeMail.DateDebut = DateTime.Now;
                    repository.Create(typeMail);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return typeMail;
        }

        /// <summary>
        /// Methode pour sauvegarder un objet ChangementAnneeCalendaire
        /// </summary>
        /// <param name="changementAnneeCalendaire">l'objet a sauvegarder</param>
        /// <returns>l'objet sauvegardee</returns>
        public object SetChangementAnneeCalendaire(ChangementAnneeCalendaire changementAnneeCalendaire)
        {
            try
            {
                var repository = new Repository<ChangementAnneeCalendaire>();
                if (repository.All().Any(p => p.Id == (changementAnneeCalendaire.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (changementAnneeCalendaire.Id));
                    var isModified = objetExistant.AgeDeviceId != changementAnneeCalendaire.AgeDeviceId||
                    objetExistant.CoefficientSource != changementAnneeCalendaire.CoefficientSource||
                    objetExistant.CoefficientDestination != changementAnneeCalendaire.CoefficientDestination;

                    objetExistant.AgeDeviceId = changementAnneeCalendaire.AgeDeviceId;
                    objetExistant.CoefficientSource = changementAnneeCalendaire.CoefficientSource;
                    objetExistant.CoefficientDestination = changementAnneeCalendaire.CoefficientDestination;
                    objetExistant.DateFin = changementAnneeCalendaire.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    if (changementAnneeCalendaire.DateDebut == new DateTime())
                        changementAnneeCalendaire.DateDebut = DateTime.Now;
                    repository.Create(changementAnneeCalendaire);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return changementAnneeCalendaire;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeRatio"></param>
        /// <returns></returns>
        public TypeRatio SetTypeRatio(TypeRatio typeRatio)
        {
            try
            {
                var repository = new Repository<TypeRatio>();
                if (repository.All().Any(p => p.Id == (typeRatio.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeRatio.Id));
                    var isModified = objetExistant.LibelleTypeRatio != typeRatio.LibelleTypeRatio;
                    objetExistant.LibelleTypeRatio = typeRatio.LibelleTypeRatio;
                    objetExistant.DateFin = typeRatio.DateFin;
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    if (typeRatio.DateDebut == new DateTime())
                        typeRatio.DateDebut = DateTime.Now;
                    repository.Create(typeRatio);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return typeRatio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equivalence"></param>
        /// <returns></returns>
        public Equivalence SetEquivalence(Equivalence equivalence)
        {
            try
            {
                var repository = new Repository<Equivalence>();
                if (repository.All().Any(p => p.Id == (equivalence.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (equivalence.Id));
                    var isModified = objetExistant.LibelleEquivalence != equivalence.LibelleEquivalence ||
                                     objetExistant.Valeur != equivalence.Valeur ||
                                     objetExistant.Mesure != equivalence.Mesure ||
                                     objetExistant.Afficher != equivalence.Afficher;

                    objetExistant.LibelleEquivalence = equivalence.LibelleEquivalence;
                    objetExistant.Valeur = equivalence.Valeur;
                    objetExistant.Mesure = equivalence.Mesure;
                    objetExistant.Afficher = equivalence.Afficher;
                    if (equivalence.DateFin != null && equivalence.DateFin > default(DateTime))
                        objetExistant.DateFin = equivalence.DateFin;
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;
                    repository.Update(objetExistant);
                }
                else
                {
                    var maxObjet = repository.All()
                                 .OrderByDescending(x => x.Id)
                                 .Take(1)
                                 ;
                    var maxId = 1;
                    if (maxObjet.Any())
                    {
                        var maxEquivalence = maxObjet.Single();
                        maxId = maxEquivalence.Id;
                        equivalence.Id = maxId + 1;
                    }                  

                    if (equivalence.DateDebut == new DateTime())
                        equivalence.DateDebut = DateTime.Now;
                    repository.Create(equivalence);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return equivalence;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usage"></param>
        /// <returns></returns>
        public Usage SetUsage(Usage usage)
        {
            try
            {
                var repository = new Repository<Usage>();
                if (repository.All().Any(p => p.Id == (usage.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (usage.Id));
                    var isModified = objetExistant.NbrHeuresIntensifJour != usage.NbrHeuresIntensifJour||
                    objetExistant.NbrHeuresNonIntensifJour != usage.NbrHeuresNonIntensifJour||
                    objetExistant.NbrHeuresOffJour != usage.NbrHeuresOffJour||
                    objetExistant.NbrJourTravaille != usage.NbrJourTravaille||
                    objetExistant.PourcentageConsoExterne != usage.PourcentageConsoExterne||
                    objetExistant.TypeUsageId != usage.TypeUsageId||
                    objetExistant.UtiliseEnergieExterne != usage.UtiliseEnergieExterne||
                    objetExistant.CoeffModeOff != usage.CoeffModeOff||
                    objetExistant.CoeffNonIntensif != usage.CoeffNonIntensif||
                    objetExistant.CoeffReductionWattExterne != usage.CoeffReductionWattExterne;

                    objetExistant.NbrHeuresIntensifJour = usage.NbrHeuresIntensifJour;
                    objetExistant.NbrHeuresNonIntensifJour = usage.NbrHeuresNonIntensifJour;
                    objetExistant.NbrHeuresOffJour = usage.NbrHeuresOffJour;
                    objetExistant.NbrJourTravaille = usage.NbrJourTravaille;
                    objetExistant.PourcentageConsoExterne = usage.PourcentageConsoExterne;
                    objetExistant.TypeUsageId = usage.TypeUsageId;
                    objetExistant.UtiliseEnergieExterne = usage.UtiliseEnergieExterne;
                    objetExistant.CoeffModeOff = usage.CoeffModeOff;
                    objetExistant.CoeffNonIntensif = usage.CoeffNonIntensif;
                    objetExistant.CoeffReductionWattExterne = usage.CoeffReductionWattExterne;
                    objetExistant.DateFin = usage.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    if (usage.DateDebut == new DateTime())
                        usage.DateDebut = DateTime.Now;
                    repository.Create(usage);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return usage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usageDevice"></param>
        /// <returns></returns>
        public UsageDevice SetUsageDevice(UsageDevice usageDevice)
        {
            try
            {
                var repository = new Repository<UsageDevice>();
                if (repository.All().Any(p => p.Id == (usageDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (usageDevice.Id));
                    var isModified = objetExistant.UsageId != usageDevice.UsageId ||
                                     objetExistant.TypeDeviceId != usageDevice.TypeDeviceId;

                    objetExistant.UsageId = usageDevice.UsageId;
                    objetExistant.TypeDeviceId = usageDevice.TypeDeviceId;
                    objetExistant.DateFin = usageDevice.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    if (usageDevice.DateDebut == new DateTime())
                        usageDevice.DateDebut = DateTime.Now;
                    repository.Create(usageDevice);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return usageDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="langue"></param>
        /// <returns></returns>
        public Langue SetLangue(Langue langue)
        {
            try
            {
                var repository = new Repository<Langue>();
                if (repository.All().Any(p => p.Id == (langue.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (langue.Id));
                    var isModified = objetExistant.Code != langue.Code||
                    objetExistant.Culture != langue.Culture||
                    objetExistant.Nom != langue.Nom||
                    objetExistant.Defaut != langue.Defaut||
                    objetExistant.Pays != langue.Pays||
                    objetExistant.Localise != langue.Localise;

                    objetExistant.Code = langue.Code;
                    objetExistant.Culture = langue.Culture;
                    objetExistant.Nom = langue.Nom;
                    objetExistant.Defaut = langue.Defaut;
                    objetExistant.Pays = langue.Pays;
                    objetExistant.Localise = langue.Localise;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(langue);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return langue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public Section SetSection(Section section)
        {
            try
            {
                var repository = new Repository<Section>();
                if (repository.All().Any(p => p.Id == (section.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (section.Id));
                    var isModified = objetExistant.Nom != section.Nom ||
                                     objetExistant.ParentId != section.ParentId;

                    objetExistant.Nom = section.Nom;
                    objetExistant.ParentId = section.ParentId;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(section);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return section;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modele"></param>
        /// <returns></returns>
        public Modele SetModele(Modele modele)
        {
            try
            {
                var repository = new Repository<Modele>();
                if (repository.All().Any(p => p.Id == (modele.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (modele.Id));
                    var isModified = objetExistant.Nom != modele.Nom ||
                                     objetExistant.Lien != modele.Lien;

                    objetExistant.Nom = modele.Nom;
                    objetExistant.Lien = modele.Lien;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(modele);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return modele;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public Page SetPage(Page page)
        {
            try
            {
                var repository = new Repository<Page>();
                if (repository.All().Any(p => p.Id == (page.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (page.Id));
                    var isModified = objetExistant.Nom != page.Nom||
                    objetExistant.Lien != page.Lien||
                    objetExistant.SectionId != page.SectionId||
                    objetExistant.ModeleModereId != page.ModeleModereId||
                    objetExistant.Actif != page.Actif;

                    objetExistant.Nom = page.Nom;
                    objetExistant.Lien = page.Lien;
                    objetExistant.SectionId = page.SectionId;
                    objetExistant.ModeleModereId = page.ModeleModereId;
                    objetExistant.Actif = page.Actif;
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;
                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(page);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return page;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cleModele"></param>
        /// <returns></returns>
        public CleModele SetCleModele(CleModele cleModele)
        {
            try
            {
                var repository = new Repository<CleModele>();
                if (repository.All().Any(p => p.Id == (cleModele.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (cleModele.Id));
                    var isModified = objetExistant.Nom != cleModele.Nom ||
                                     objetExistant.ModeleId != cleModele.ModeleId;

                    objetExistant.Nom = cleModele.Nom;
                    objetExistant.ModeleId = cleModele.ModeleId;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(cleModele);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return cleModele;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contenuModere"></param>
        /// <returns></returns>
        public ContenuModere SetContenuModere(ContenuModere contenuModere)
        {
            try
            {
                var repository = new Repository<ContenuModere>();
                if (repository.All().Any(p => p.Id == (contenuModere.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (contenuModere.Id));
                    var isModified = objetExistant.Contenu != contenuModere.Contenu||
                    objetExistant.PageId != contenuModere.PageId||
                    objetExistant.CleModeleId != contenuModere.CleModeleId||
                    objetExistant.LangueId != contenuModere.LangueId;

                    objetExistant.Contenu = contenuModere.Contenu;
                    objetExistant.PageId = contenuModere.PageId;
                    objetExistant.CleModeleId = contenuModere.CleModeleId;
                    objetExistant.LangueId = contenuModere.LangueId;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(contenuModere);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return contenuModere;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routage"></param>
        /// <returns></returns>
        public Routage SetRoutage(Routage routage)
        {
            try
            {
                var repository = new Repository<Routage>();
                if (repository.All().Any(p => p.Id == (routage.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (routage.Id));
                    var isModified = objetExistant.ValeurActuelle != routage.ValeurActuelle ||
                                     objetExistant.ValeurTraduite != routage.ValeurTraduite ||
                                     objetExistant.PageId != routage.PageId ||
                                     objetExistant.Actif != routage.Actif ||
                                     objetExistant.LangueId != routage.LangueId;

                    objetExistant.ValeurActuelle = routage.ValeurActuelle;
                    objetExistant.ValeurTraduite = routage.ValeurTraduite;
                    objetExistant.PageId = routage.PageId;
                    objetExistant.Actif = routage.Actif;
                    objetExistant.LangueId = routage.LangueId;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(routage);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return routage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Routage> ListeDesRoutages()
        {
            try
            {
                var repository = new Repository<Routage>();
                var listeTypeObjet = repository.All().ToList();
                return listeTypeObjet;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="langueId"></param>
        /// <param name="valeurActuelle"></param>
        /// <returns></returns>
        public Routage GetRoutageByLangueIdAndValeurActuelle(int langueId, string valeurActuelle)
        {
            try
            {
                var repository = new Repository<Routage>();
                var response = repository.Filter(o => o.LangueId == langueId && o.ValeurActuelle == valeurActuelle);
                if (response.Any())
                    return response.Single();
                else
                {
                    var langue = GetLangueByCode("en");
                    response = repository.Filter(o => o.LangueId == langue.Id && o.ValeurActuelle == valeurActuelle);
                    if (response.Any())
                        return response.Single();
                    return null;
                }               
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public Reponse GetReponse(int contactId)
        {
            try
            {
                var repository = new Repository<Reponse>();
                var reponse = repository.Filter(o => o.ContactId == contactId).Single();
                return reponse;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Question> ListeQuestion(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Question>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Contact GetContactParEmail(string email)
        {
            try
            {
                var repository = new Repository<Contact>();
                var contact = repository.Filter(c => c.Email == email && c.DateFin == null && c.Client.Statut != 2).Include(c => c.Client)
                                        .Single();
                return contact;
            }

            catch (Exception e)
            {
                var s = e.StackTrace;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSi"></param>
        /// <returns></returns>
        public ConfigSi SauvegardeConfigSi(ConfigSi configSi)
        {
            try
            {
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager SauvegardeConfigSi ");
                var repository = new Repository<ConfigSi>();
                repository.Create(configSi);
                _unitOfWork.SaveChanges();
                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager SauvegardeConfigSi ");
                return configSi;

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiClient"></param>
        /// <returns></returns>
        public ConfigSiClient SauvegardeConfigSiClient(ConfigSiClient configSiClient)
        {
            try
            {
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager SauvegardeConfigSiClient ");
                var repository = new Repository<ConfigSiClient>();
                repository.Create(configSiClient);
                _unitOfWork.SaveChanges();
                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager SauvegardeConfigSiClient ");
                return configSiClient;

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiDevice"></param>
        /// <returns></returns>
        public ConfigSiDevice SauvegardeConfigSiDevice(ConfigSiDevice configSiDevice)
        {
            try
            {
                var repository = new Repository<ConfigSiDevice>();
                repository.Create(configSiDevice);
                _unitOfWork.SaveChanges();
                return configSiDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pays"></param>
        /// <returns></returns>
        public bool SupprimerPays(Pays pays)
        {
            var repository = new Repository<Pays>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (pays.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (pays.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibellePays={1}", pays.Id, pays.LibellePays);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibellePays={1}", pays.Id, pays.LibellePays);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Supprime un objet CategorieDevice
        /// </summary>
        /// <param name="categorieDevice">L'objet a supprimer</param>
        /// <returns>True si l'objet est supprime</returns>
        public bool SupprimerCategorieDevice(CategorieDevice categorieDevice)
        {
            var repository = new Repository<CategorieDevice>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (categorieDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (categorieDevice.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleCategorieDevice={1}", categorieDevice.Id, categorieDevice.LibelleCategorieDevice);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleCategorieDevice={1}", categorieDevice.Id, categorieDevice.LibelleCategorieDevice);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classeDevice"></param>
        /// <returns></returns>
        public bool SupprimerClasseDevice(ClasseDevice classeDevice)
        {
            var repository = new Repository<ClasseDevice>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (classeDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (classeDevice.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleClasseDevice={1}", classeDevice.Id, classeDevice.LibelleClasseDevice);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleClasseDevice={1}", classeDevice.Id, classeDevice.LibelleClasseDevice);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageDevice"></param>
        /// <returns></returns>
        public bool SupprimerAgeDevice(AgeDevice ageDevice)
        {
            var repository = new Repository<AgeDevice>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (ageDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (ageDevice.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleAgeDevice={1}", ageDevice.Id, ageDevice.LibelleAgeDevice);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleAgeDevice={1}", ageDevice.Id, ageDevice.LibelleAgeDevice);
                LogguerErreur(e);
                throw;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoWattHeur"></param>
        /// <returns></returns>
        public bool SupprimerConsoWattHeur(ConsoWattHeur consoWattHeur)
        {
            var repository = new Repository<ConsoWattHeur>();
            try
            {
                var statut = 0;
                if (
                    repository.All()
                              .Any(
                                  p =>
                                  p.TypeDeviceId == (consoWattHeur.TypeDeviceId) &&
                                  p.ClasseDeviceId == consoWattHeur.ClasseDeviceId &&
                                  p.AgeDeviceId == consoWattHeur.AgeDeviceId))
                {
                    var objetExistant = repository.All().First(p =>
                                                               p.TypeDeviceId == (consoWattHeur.TypeDeviceId) &&
                                                               p.ClasseDeviceId == consoWattHeur.ClasseDeviceId &&
                                                               p.AgeDeviceId == consoWattHeur.AgeDeviceId);
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("TypeDeviceId={0}, ClasseDeviceId={1}, AgeDeviceId={2}", consoWattHeur.TypeDeviceId, consoWattHeur.ClasseDeviceId, consoWattHeur.AgeDeviceId);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("TypeDeviceId={0}, ClasseDeviceId={1}, AgeDeviceId={2}", consoWattHeur.TypeDeviceId, consoWattHeur.ClasseDeviceId, consoWattHeur.AgeDeviceId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="familleDevice"></param>
        /// <returns></returns>
        public bool SupprimerFamilleDevice(FamilleDevice familleDevice)
        {
            var repository = new Repository<FamilleDevice>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (familleDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (familleDevice.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleFamilleDevice={1}", familleDevice.Id, familleDevice.LibelleFamilleDevice);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleFamilleDevice={1}", familleDevice.Id, familleDevice.LibelleFamilleDevice);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origine"></param>
        /// <returns></returns>
        public bool SupprimerOrigine(Origine origine)
        {
            var repository = new Repository<Origine>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (origine.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (origine.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleOrigine={1}", origine.Id, origine.LibelleOrigine);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleOrigine={1}", origine.Id, origine.LibelleOrigine);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="politique"></param>
        /// <returns></returns>
        public bool SupprimerPolitique(Politique politique)
        {
            var repository = new Repository<Politique>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (politique.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (politique.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibellePolitique={1}", politique.Id, politique.LibellePolitique);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibellePolitique={1}", politique.Id, politique.LibellePolitique);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public bool SupprimerRatio(Ratio ratio)
        {
            var repository = new Repository<Ratio>();
            try
            {
                var statut = 0;
                if (
                    repository.All()
                              .Any(
                                  p =>
                                  p.SecteurActiviteId == (ratio.SecteurActiviteId) &&
                                  p.TypeDeviceId == ratio.TypeDeviceId && p.TypeRatioId == ratio.TypeRatioId))
                {
                    var objetExistant =
                        repository.All()
                                  .First(
                                      p =>
                                      p.SecteurActiviteId == (ratio.SecteurActiviteId) &&
                                      p.TypeDeviceId == ratio.TypeDeviceId && p.TypeRatioId == ratio.TypeRatioId);
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}", ratio.SecteurActiviteId, ratio.TypeDeviceId);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}", ratio.SecteurActiviteId, ratio.TypeDeviceId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secteurActivite"></param>
        /// <returns></returns>
        public bool SupprimerSecteurActivite(SecteurActivite secteurActivite)
        {
            var repository = new Repository<SecteurActivite>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (secteurActivite.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (secteurActivite.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleSecteurActivite={1}", secteurActivite.Id, secteurActivite.LibelleSecteurActivite);
                LogguerErreur(dex);
                throw;
            } 
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleSecteurActivite={1}", secteurActivite.Id, secteurActivite.LibelleSecteurActivite);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeContact"></param>
        /// <returns></returns>
        public bool SupprimerTypeContact(TypeContact typeContact)
        {
            var repository = new Repository<TypeContact>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (typeContact.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeContact.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleTypeContact={1}", typeContact.Id, typeContact.LibelleTypeContact);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeContact={1}", typeContact.Id, typeContact.LibelleTypeContact);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDevice"></param>
        /// <returns></returns>
        public bool SupprimerTypeDevice(TypeDevice typeDevice)
        {
            var repository = new Repository<TypeDevice>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (typeDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeDevice.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleTypeDevice={1}", typeDevice.Id, typeDevice.LibelleTypeDevice);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeDevice={1}", typeDevice.Id, typeDevice.LibelleTypeDevice);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeObjet"></param>
        /// <returns></returns>
        public bool SupprimerTypeObjet(TypeObjet typeObjet)
        {
            var repository = new Repository<TypeObjet>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (typeObjet.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeObjet.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, NomObjet={1}", typeObjet.Id, typeObjet.NomObjet);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, NomObjet={1}", typeObjet.Id, typeObjet.NomObjet);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeUsage"></param>
        /// <returns></returns>
        public bool SupprimerTypeUsage(TypeUsage typeUsage)
        {
            var repository = new Repository<TypeUsage>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (typeUsage.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeUsage.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleTypeUsage={1}", typeUsage.Id, typeUsage.LibelleTypeUsage);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeUsage={1}", typeUsage.Id, typeUsage.LibelleTypeUsage);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ventilationClasseAgeDevice"></param>
        /// <returns></returns>
        public bool SupprimerVentilationClasseAgeDevice(VentilationClasseAgeDevice ventilationClasseAgeDevice)
        {
            var repository = new Repository<VentilationClasseAgeDevice>();
            try
            {
                var statut = 0;
                if (
                    repository.All()
                              .Any(
                                  p =>
                                  p.AgeDeviceId == (ventilationClasseAgeDevice.AgeDeviceId) &&
                                  p.ClasseDeviceId == ventilationClasseAgeDevice.ClasseDeviceId &&
                                  p.TypeDeviceId == ventilationClasseAgeDevice.TypeDeviceId))
                {
                    var objetExistant =
                        repository.All()
                                  .First(
                                      p =>
                                      p.AgeDeviceId == (ventilationClasseAgeDevice.AgeDeviceId) &&
                                      p.ClasseDeviceId == ventilationClasseAgeDevice.ClasseDeviceId &&
                                      p.TypeDeviceId == ventilationClasseAgeDevice.TypeDeviceId);
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}, AgeDeviceId={3} ", ventilationClasseAgeDevice.SecteurActiviteId,
                              ventilationClasseAgeDevice.TypeDeviceId, ventilationClasseAgeDevice.ClasseDeviceId,
                              ventilationClasseAgeDevice.AgeDeviceId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}, AgeDeviceId={3} ", ventilationClasseAgeDevice.SecteurActiviteId,
                             ventilationClasseAgeDevice.TypeDeviceId, ventilationClasseAgeDevice.ClasseDeviceId,
                             ventilationClasseAgeDevice.AgeDeviceId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ventilationClasseDevice"></param>
        /// <returns></returns>
        public bool SupprimerVentilationClasseDevice(VentilationClasseDevice ventilationClasseDevice)
        {
            var repository = new Repository<VentilationClasseDevice>();
            try
            {
                var statut = 0;
                if (
                    repository.All()
                              .Any(
                                  p =>
                                  p.TypeDeviceId == (ventilationClasseDevice.TypeDeviceId) &&
                                  p.SecteurActiviteId == ventilationClasseDevice.SecteurActiviteId))
                {
                    var objetExistant =
                        repository.All()
                                  .First(
                                      p =>
                                      p.TypeDeviceId == (ventilationClasseDevice.TypeDeviceId) &&
                                      p.SecteurActiviteId == ventilationClasseDevice.SecteurActiviteId);
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}", ventilationClasseDevice.SecteurActiviteId,
                               ventilationClasseDevice.TypeDeviceId, ventilationClasseDevice.ClasseDeviceId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("SecteurActiviteId={0}, TypeDeviceId={1}, ClasseDeviceId={2}", ventilationClasseDevice.SecteurActiviteId,
                              ventilationClasseDevice.TypeDeviceId, ventilationClasseDevice.ClasseDeviceId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tarif"></param>
        /// <returns></returns>
        public bool SupprimerTarif(Tarif tarif)
        {
            var repository = new Repository<Tarif>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (tarif.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (tarif.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleTarif={1}, TarifAnnuel={2}", tarif.Id, tarif.LibelleTarif, tarif.TarifAnnuel);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTarif={1}, TarifAnnuel={2}", tarif.Id, tarif.LibelleTarif, tarif.TarifAnnuel);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public bool SupprimerQuestion(Question question)
        {
            var repository = new Repository<Question>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (question.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (question.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleQuestion={1}", question.Id, question.LibelleQuestion);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleQuestion={1}", question.Id, question.LibelleQuestion);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contenuMail"></param>
        /// <returns></returns>
        public bool SupprimerContenuMail(ContenuMail contenuMail)
        {
            var repository = new Repository<ContenuMail>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (contenuMail.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (contenuMail.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LangueId={1}, TypeMailId={2}", contenuMail.Id, contenuMail.LangueId, contenuMail.TypeMailId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LangueId={1}, TypeMailId={2}", contenuMail.Id, contenuMail.LangueId, contenuMail.TypeMailId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeMail"></param>
        /// <returns></returns>
        public bool SupprimerTypeMail(TypeMail typeMail)
        {
            var repository = new Repository<TypeMail>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (typeMail.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeMail.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleTypeMail={1}", typeMail.Id, typeMail.LibelleTypeMail);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeMail={1}", typeMail.Id, typeMail.LibelleTypeMail);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changementAnneeCalendaire"></param>
        /// <returns></returns>
        public bool SupprimerChangementAnneeCalendaire(ChangementAnneeCalendaire changementAnneeCalendaire)
        {
            var repository = new Repository<ChangementAnneeCalendaire>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (changementAnneeCalendaire.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (changementAnneeCalendaire.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, AgeDeviceId={1}", changementAnneeCalendaire.Id, changementAnneeCalendaire.AgeDeviceId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, AgeDeviceId={1}", changementAnneeCalendaire.Id, changementAnneeCalendaire.AgeDeviceId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeRatio"></param>
        /// <returns></returns>
        public bool SupprimerTypeRatio(TypeRatio typeRatio)
        {
            var repository = new Repository<TypeRatio>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (typeRatio.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (typeRatio.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleTypeRatio={1}", typeRatio.Id, typeRatio.LibelleTypeRatio);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleTypeRatio={1}", typeRatio.Id, typeRatio.LibelleTypeRatio);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equivalence"></param>
        /// <returns></returns>
        public bool SupprimerEquivalence(Equivalence equivalence)
        {
            var repository = new Repository<Equivalence>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (equivalence.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (equivalence.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LibelleEquivalence={1}", equivalence.Id, equivalence.LibelleEquivalence);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LibelleEquivalence={1}", equivalence.Id, equivalence.LibelleEquivalence);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usage"></param>
        /// <returns></returns>
        public bool SupprimerUsage(Usage usage)
        {
            var repository = new Repository<Usage>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (usage.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (usage.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, TypeUsageId={1}", usage.Id, usage.TypeUsageId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, TypeUsageId={1}", usage.Id, usage.TypeUsageId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usageDevice"></param>
        /// <returns></returns>
        public bool SupprimerUsageDevice(UsageDevice usageDevice)
        {
            var repository = new Repository<UsageDevice>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (usageDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (usageDevice.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, TypeDeviceId={1}, UsageId={2}", usageDevice.Id, usageDevice.TypeDeviceId, usageDevice.UsageId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, TypeDeviceId={1}, UsageId={2}", usageDevice.Id, usageDevice.TypeDeviceId, usageDevice.UsageId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="langue"></param>
        /// <returns></returns>
        public bool SupprimerLangue(Langue langue)
        {
            var repository = new Repository<Langue>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (langue.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (langue.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, Code={1}", langue.Id, langue.Code);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, Code={1}", langue.Id, langue.Code);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool SupprimerSection(Section section)
        {
            var repository = new Repository<Section>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (section.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (section.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, Nom={1}", section.Id, section.Nom);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, Nom={1}", section.Id, section.Nom);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modele"></param>
        /// <returns></returns>
        public bool SupprimerModele(Modele modele)
        {
            var repository = new Repository<Modele>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (modele.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (modele.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, Nom={1}", modele.Id, modele.Nom);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, Nom={1}", modele.Id, modele.Nom);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool SupprimerPage(Page page)
        {
            var repository = new Repository<Page>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (page.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (page.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, Nom={1}", page.Id, page.Nom);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, Nom={1}", page.Id, page.Nom);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cleModele"></param>
        /// <returns></returns>
        public bool SupprimerCleModele(CleModele cleModele)
        {
            var repository = new Repository<CleModele>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (cleModele.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (cleModele.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, Nom={1}", cleModele.Id, cleModele.Nom);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, Nom={1}", cleModele.Id, cleModele.Nom);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contenuModere"></param>
        /// <returns></returns>
        public bool SupprimerContenuModere(ContenuModere contenuModere)
        {
            var repository = new Repository<ContenuModere>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (contenuModere.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (contenuModere.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LangueId={1}, PageId={2}", contenuModere.Id, contenuModere.LangueId, contenuModere.PageId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LangueId={1}, PageId={2}", contenuModere.Id, contenuModere.LangueId, contenuModere.PageId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routage"></param>
        /// <returns></returns>
        public bool SupprimerRoutage(Routage routage)
        {
            var repository = new Repository<Routage>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (routage.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (routage.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, LangueId={1}, PageId={2}", routage.Id, routage.LangueId, routage.PageId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, LangueId={1}, PageId={2}", routage.Id, routage.LangueId, routage.PageId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Creer/Modifier un objet EquivalenceTraduction
        /// </summary>
        /// <param name="equivalenceTraduction">L'objet EquivalenceTraduction a creer</param>
        /// <returns>L'objet EquivalenceTraduction cree</returns>
        public EquivalenceTraduction SetEquivalenceTraduction(EquivalenceTraduction equivalenceTraduction)
        {
            try
            {
                var repository = new Repository<EquivalenceTraduction>();
                if (
                    repository.All()
                              .Any(
                                  p =>
                                  (p.Id == equivalenceTraduction.Id ||
                                   (p.EquivalenceId == equivalenceTraduction.EquivalenceId &&
                                    p.LangueId == equivalenceTraduction.LangueId))))
                {
                    var objetExistant =
                        repository.All()
                                  .First(
                                      p =>
                                      (p.Id == equivalenceTraduction.Id ||
                                       (p.EquivalenceId == equivalenceTraduction.EquivalenceId &&
                                        p.LangueId == equivalenceTraduction.LangueId)));

                    var isModified = objetExistant.EquivalenceId != equivalenceTraduction.EquivalenceId||
                    objetExistant.LangueId != equivalenceTraduction.LangueId||
                    objetExistant.Valeur != equivalenceTraduction.Valeur||
                    objetExistant.Mesure != equivalenceTraduction.Mesure||
                    objetExistant.LibelleEquivalence != equivalenceTraduction.LibelleEquivalence;

                    objetExistant.Afficher = equivalenceTraduction.Afficher;
                    objetExistant.EquivalenceId = equivalenceTraduction.EquivalenceId;
                    objetExistant.LangueId = equivalenceTraduction.LangueId;
                    objetExistant.Valeur = equivalenceTraduction.Valeur;
                    objetExistant.Mesure = equivalenceTraduction.Mesure;
                    objetExistant.Afficher = equivalenceTraduction.Afficher;
                    objetExistant.DateFin = equivalenceTraduction.DateFin;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    objetExistant.LibelleEquivalence = equivalenceTraduction.LibelleEquivalence;
                    repository.Update(objetExistant);
                }
                else
                {
                    if (equivalenceTraduction.DateDebut == new DateTime())
                        equivalenceTraduction.DateDebut = DateTime.Now;
                    repository.Create(equivalenceTraduction);
                }

                return equivalenceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Creer/Modifier un objet PolitiqueReduction
        /// </summary>
        /// <param name="politiqueTraduction">L'objet PolitiqueReduction a creer</param>
        /// <returns>L'objet PolitiqueReduction cree</returns>
        public PolitiqueTraduction SetPolitiqueTraduction(PolitiqueTraduction politiqueTraduction)
        {
            try
            {
                var repository = new Repository<PolitiqueTraduction>();

                try
                {
                    if (repository.All().Any(p => p.Id == politiqueTraduction.Id))
                    {
                        var existingPolitique = repository.All().First(p => p.Id == (politiqueTraduction.Id));
                        var isModified = existingPolitique.PolitiqueId != politiqueTraduction.PolitiqueId ||
                                         existingPolitique.LangueId != politiqueTraduction.LangueId ||
                                         existingPolitique.LibellePolitique != politiqueTraduction.LibellePolitique ||
                                         existingPolitique.Device != politiqueTraduction.Device ||
                                         existingPolitique.Objectif != politiqueTraduction.Objectif ||
                                         existingPolitique.Benefice != politiqueTraduction.Benefice ||
                                         existingPolitique.LibellePolitique != politiqueTraduction.LibellePolitique;

                        existingPolitique.PolitiqueId = politiqueTraduction.PolitiqueId;
                        existingPolitique.LangueId = politiqueTraduction.LangueId;
                        existingPolitique.LibellePolitique = politiqueTraduction.LibellePolitique;
                        existingPolitique.Device = politiqueTraduction.Device;
                        existingPolitique.Objectif = politiqueTraduction.Objectif;
                        existingPolitique.Benefice = politiqueTraduction.Benefice;
                        existingPolitique.LibellePolitique = politiqueTraduction.LibellePolitique;

                        if (isModified)
                            existingPolitique.DateModification = DateTime.Now;

                        repository.Update(existingPolitique);
                    }
                    else
                    {
                        politiqueTraduction.DateDebut = DateTime.Now;
                        repository.Create(politiqueTraduction);
                    }
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

            return politiqueTraduction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="politiqueTraduction"></param>
        /// <returns></returns>
        public bool SupprimerPolitiqueTraduction(PolitiqueTraduction politiqueTraduction)
        {
            var repository = new Repository<PolitiqueTraduction>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (politiqueTraduction.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (politiqueTraduction.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, PolitiqueId={1}, LangueId={2}", politiqueTraduction.Id, politiqueTraduction.PolitiqueId, politiqueTraduction.LangueId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, PolitiqueId={1}, LangueId={2}", politiqueTraduction.Id, politiqueTraduction.PolitiqueId, politiqueTraduction.LangueId);
                LogguerErreur(e);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equivalenceTraduction"></param>
        /// <returns></returns>
        public bool SupprimerEquivalenceTraduction(EquivalenceTraduction equivalenceTraduction)
        {
            var repository = new Repository<EquivalenceTraduction>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (equivalenceTraduction.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (equivalenceTraduction.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, EquivalenceId={1}, LangueId={2}", equivalenceTraduction.Id, equivalenceTraduction.EquivalenceId, equivalenceTraduction.LangueId);
                LogguerErreur(dex);
                throw;
            }   
            catch (Exception e)
            {
                Logger.Error("Id={0}, EquivalenceId={1}, LangueId={2}", equivalenceTraduction.Id, equivalenceTraduction.EquivalenceId, equivalenceTraduction.LangueId);
                LogguerErreur(e);
                return false;
            }
        }


        /// <summary>
        /// Retourne un utilisateur existant avec le meme email et mot de passe
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Utilisateur GetUtilisateur(Utilisateur user)
        {
            var repository = new Repository<Utilisateur>();
            try
            {

                if (repository.All().Any(p => p.Email == (user.Email) && p.MotDePasse == user.MotDePasse))
                {
                    var objetExistant =
                        repository.All().First(p => p.Email == (user.Email) && p.MotDePasse == user.MotDePasse);
                    return objetExistant;
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.Error("user Id={0}", user.Id);
                LogguerErreur(e);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeRatio> ListeTypeRatio(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<TypeRatio>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCategorie"></param>
        /// <returns></returns>
        public CategorieDevice GetCategorieDevice(int idCategorie)
        {
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var repository = new Repository<CategorieDevice>();
               // var categorieDevice = repository.Filter(o => o.Id == idCategorie)
                 //                               .Include(o => o.FamilleDevice);

                var categorieDevice1 = repository.All().Select(c => new { CategorieDevice = c, CategorieDeviceTraduction = c.CategorieDeviceTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.CategorieDevice).Where(c=>c.Id==idCategorie).ToList();


                return categorieDevice1.Any() ? categorieDevice1.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TypeObjet GetTypeObjet(int id)
        {
            try
            {
                var repository = new Repository<TypeObjet>();

                var objet = repository.Find(o => o.Id == id);

                return objet;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nom"></param>
        /// <returns></returns>
        public TypeObjet GetTypeObjet(string nom)
        {
            try
            {
                var repository = new Repository<TypeObjet>();

                var objet = repository.Find(o => o.NomObjet == nom);

                return objet;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CategorieDevice> ListeCategorieDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CategorieDevice>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o=>o.Id).Skip(currentPage*pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ClasseDevice> ListeClasseDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<ClasseDevice>();
                
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();                    
                }
                else
                    return repository.All().Select(c => new { ClasseDevice = c, ClasseeDeviceTraduction = c.ClasseDeviceTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.ClasseDevice).ToList();

                //var listeCategorieDevice = repository.All().Select(c => new { ClasseDevice = c, ClasseeDeviceTraduction = c.ClasseDeviceTraductions.Where(ct => ct.LangueId == 1) }).AsEnumerable().Select(r => r.ClasseDevice).ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<AgeDevice> ListeAgeDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<AgeDevice>();
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();                    
                }
                else
                    return repository.All().Select(c => new { AgeDevice = c, ClasseeDeviceTraduction = c.AgeDeviceTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.AgeDevice).ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<FamilleDevice> ListeFamilleDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<FamilleDevice>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();                    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Ratio> ListeRatio(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Ratio>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.SecteurActiviteId).ThenBy(o => o.TypeDeviceId).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeContact> ListeTypeContact(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<TypeContact>();
                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeDevice> ListeTypeDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<TypeDevice>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeUsage> ListeTypeUsage(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<TypeUsage>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<VentilationClasseAgeDevice> ListeVentilationClasseAgeDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<VentilationClasseAgeDevice>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.SecteurActiviteId).ThenBy(o=>o.TypeDeviceId).ThenBy(o=>o.ClasseDeviceId).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<VentilationClasseDevice> ListeVentilationClasseDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<VentilationClasseDevice>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.SecteurActiviteId).ThenBy(o => o.TypeDeviceId).ThenBy(o => o.ClasseDeviceId).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Tarif> ListeTarif(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Tarif>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ContenuMail> ListeContenuMail(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<ContenuMail>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        //public ContenuMail GetContenuMail(string codeLangue, string libelleTypeMail)
        //{
        //    try
        //    {
        //        //Recuperer Id langue
        //        var langue = GetLangueByCode(codeLangue);

        //        var typeMail = GetTypeMail(libelleTypeMail);

        //        var repository = new Repository<ContenuMail>();
        //        var contenuMail = repository.Filter(o => o.LangueId == langue.Id && o.TypeMailId == typeMail.Id);

        //        return contenuMail.Any() ? contenuMail.Single() : null;
        //    }
        //    catch (Exception e)
        //    {
        //        LogguerErreur(e);
        //    }
        //    return null;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="libelleTypeMail"></param>
        /// <returns></returns>
        public TypeMail GetTypeMail(string libelleTypeMail)
        {
            try
            {
                var repository = new Repository<TypeMail>();
                var typeMail = repository.Filter(o => o.LibelleTypeMail == libelleTypeMail);

                return typeMail.Any() ? typeMail.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeMail> ListeTypeMail(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<TypeMail>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ChangementAnneeCalendaire> ListeChangementAnneeCalendaire(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<ChangementAnneeCalendaire>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Equivalence> ListeEquivalence(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Equivalence>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Usage> ListeUsage(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Usage>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<UsageDevice> ListeUsageDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<UsageDevice>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<EquivalenceTraduction> ListeEquivalenceTraduction(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<EquivalenceTraduction>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<PolitiqueTraduction> ListePolitiqueTraduction(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<PolitiqueTraduction>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CleModele> ListeCleModele(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CleModele>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ContenuModere> ListeContenuModere(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<ContenuModere>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Routage> ListeRoutage(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Routage>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();    
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<Devise> ListeDevise(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Devise>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devise"></param>
        /// <returns></returns>
        public Devise SetDevise(Devise devise)
        {
            try
            {
                var repository = new Repository<Devise>();
                if (repository.All().Any(p => p.Id == (devise.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (devise.Id));
                    var isModified = objetExistant.Libelle != devise.Libelle ||
                                     objetExistant.CodeDevise != devise.CodeDevise
                                    ;

                    objetExistant.Libelle = devise.Libelle;
                    objetExistant.CodeDevise = devise.CodeDevise;                   
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(devise);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return devise;
        }

        public Origine GetOrigineParLibelle(string Libelle)
        {
            try
            {
                var repository = new Repository<Origine>();

                var origine = repository.Filter(o => o.LibelleOrigine == Libelle);

                return origine.Any() ? origine.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idClient"></param>
        /// <returns></returns>
        public ConfigSiClient GetConfigSiClient(int idClient)
        {
            try
            {
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager GetConfigSiClient ");
                                     
                var origineActuel = GetOrigineParLibelle(OrigineEnum.Actuel.ToString());
                var repository = new Repository<ConfigSiClient>();
                var configSiClients = repository.Filter(o => o.ClientId == idClient && o.OrigineId == origineActuel.Id)
                                               .Include(o => o.ConfigSi)
                                               .Include(o => o.ConfigSi.ConfigSiDevices)
                                               .Include(o => o.ConfigSi.Resultats)
                                               .Include(o => o.Client)
                                               .Include("ConfigSi.ConfigSiDevices.ConfigSiConsommations")
                                               .ToList();
                //var result =  configSiClients.Any() ? configSiClients.OrderBy(o => o.DateDebut).First() : null;

                ConfigSiClient result = null;
                if (configSiClients.Any())
                {
                    if (configSiClients.Count.ToString().Equals("1"))
                    {
                        result = configSiClients.Single();
                    }
                    else
                    {
                        result = configSiClients.OrderBy(o => o.DateDebut).First();

                        var originePerso = GetOrigineParLibelle(OrigineEnum.Personnalisation.ToString());
                        configSiClients = configSiClients.OrderBy(o => o.DateDebut).ToList();
                        for (int i = 1; i < configSiClients.Count(); i++)
                        {
                            configSiClients[i].OrigineId = originePerso.Id;
                            repository.Update(configSiClients[i]);
                            _unitOfWork.SaveChanges();   
                        }

                    }
                }

                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager GetConfigSiClient ");

                return result;
                
                /*var configSiClient = repository.Filter(o => o.ClientId == idClient)
                                                .Include(o => o.ConfigSi)
                                                .Include(o => o.ConfigSi.ConfigSiDevices);
                return configSiClient.Any() ? configSiClient.Single() : null;*/
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiDeviceId"></param>
        /// <returns></returns>
        public List<ConfigSiConsommation> GetConfigSiConsommations(int configSiDeviceId)
        {
            try
            {
                var repository = new Repository<ConfigSiConsommation>();
                var configSiConsommations = repository.Filter(o => o.ConfigSiDeviceId == configSiDeviceId)
                                                      .Include(o => o.ClasseDevice)
                                                      .Include(o => o.AgeDevice)
                                                      .ToList();
                return configSiConsommations;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DetailsClient GetDetailsClientById(int id)
        {
            try
            {
                var repository = new Repository<DetailsClient>();

                var objet = repository.Find(o => o.Id == id);

                return objet;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<FamilleDevice> GetListeFamilleDevices()
        {
            try
            {
                var langueId = 1;

                var familleRepository = new Repository<FamilleDevice>();
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var listefamilleDevice = familleRepository.All().Select(f => new { FamilleDevice = f, FamilleDeviceTraductions = f.FamilleDeviceTraductions.Where(ft => ft.LangueId == langueId) }).AsEnumerable().Select(r => r.FamilleDevice).ToList();

                foreach (var familleDevice in listefamilleDevice)
                {
                    if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                        familleDevice.LibelleFamilleDevice = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                    else
                        familleDevice.LibelleFamilleDevice = familleDevice.LibelleFamilleDevice;
                }

                return listefamilleDevice;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public Simulation GetSimulation(int clientId)
        {
            var repositorySimulation = new Repository<Simulation>();
            var repositoryConfigSi = new Repository<ConfigSi>();
            var repositoryConfigSiDevice = new Repository<ConfigSiDevice>();
            var repositoryConfigSiConsommation = new Repository<ConfigSiConsommation>();
            var repositoryConfigSiClient = new Repository<ConfigSiClient>();

            try
            {
                var q = repositoryConfigSiClient.Filter(c => c.ClientId == clientId && c.DateFin == null)
                                                .Include("ConfigSi")
                                                .Include("ConfigSi.Simulations")
                                                .Include("Client")
                                                .Include("ConfigSi.ConfigSiDevices")
                                                .Include("ConfigSi.ConfigSiDevices.ConfigSiConsommations");
                var result = q.Where(o => o.ConfigSi.Simulations.Any(s => s.DateFin == null));
                var resultFinale = result.ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }

            return new Simulation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeContact"></param>
        /// <returns></returns>
        public TypeContact GetTypeContactByString(string typeContact)
        {
            try
            {
                var repository = new Repository<TypeContact>();

                var objet = repository.Find(o => o.LibelleTypeContact == typeContact);

                return objet;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adresse"></param>
        /// <returns></returns>
        public Adresse saveAdress(Adresse adresse)
        {
            try
            {
                var sectionRepository = new Repository<Adresse>();
                Adresse clientAdresse = sectionRepository.Create(adresse);
                _unitOfWork.SaveChanges();

                return clientAdresse;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detailsClient"></param>
        /// <returns></returns>
        public DetailsClient saveDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                var DetailsClientRepository = new Repository<DetailsClient>();
                DetailsClient dc = DetailsClientRepository.Create(detailsClient);
                _unitOfWork.SaveChanges();

                return dc;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public Client SaveClient(Client client)
        {
            try
            {
                using (var scope = new System.Transactions.TransactionScope())
                {
                    _unitOfWork = new EconocomContext();
                    var clientRepository = new Repository<Client>(_unitOfWork);
                    client = clientRepository.Create(client);
                    _unitOfWork.SaveChanges();
                    scope.Complete();
                }

                return client;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Contact SauvegardeContact(Contact contact)
        {
            try
            {
                var sectionRepository = new Repository<Contact>();
                contact = sectionRepository.Create(contact);
                _unitOfWork.SaveChanges();

                return contact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public List<ContenuModere> GetListeContenuModere(int pageId)
        {
            try
            {
                var repository = new Repository<ContenuModere>();
                var contenuModereListe = repository.Filter(o => o.PageId == pageId).ToList();
                return contenuModereListe;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adresse"></param>
        /// <returns></returns>
        public Adresse updateAdress(Adresse adresse)
        {
            try
            {
                var adresseRepository = new Repository<Adresse>();                
                adresseRepository.Update(adresse);
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return adresse;
        }

        public Client updateClient(Client client)
        {
            try
            {
                var societeRepository = new Repository<Client>();
                client.DateModification = DateTime.Now;
                societeRepository.Update(client);
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Contact updateContacte(Contact contact)
        {
            try
            {
                var contactRepository = new Repository<Contact>();
                contact.DateModification = DateTime.Now;
                contactRepository.Update(contact);
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idTypeMail"></param>
        /// <param name="idLangue"></param>
        /// <returns></returns>
        public ContenuMail GetContenuMail(int idTypeMail, int idLangue)
        {
            try
            {
                var repository = new Repository<ContenuMail>();
                var contenuModereListe = repository.Find(o => (o.TypeMailId == idTypeMail && o.LangueId == idLangue));
                return contenuModereListe;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeMail> GetListeTypeMail()
        {
            try
            {
                var repository = new Repository<TypeMail>();
                var contenuModereListe = repository.All().ToList();
                return contenuModereListe;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Contact> GetListeContact()
        {
            try
            {
                var repository = new Repository<Contact>();
                var listeContact = repository.All()
                                             .Include(o => o.Client)
                                             .Include(o => o.TypeContact)
                                             .Include(o => o.Reponses)
                                             .Include("Reponses.Question")
                                             .ToList();
               
                return listeContact;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public List<Section> GetChildren(int sectionId)
        {
            try
            {
                var repository = new Repository<Section>();
                var liste = repository.Filter(o => o.ParentId == sectionId)
                                      .Include(s => s.Parent)
                                      .Include(s => s.Sections)
                                      .Include(s => s.Pages)
                                      .ToList();
                return liste;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiConsommation"></param>
        /// <returns></returns>
        public ConfigSiConsommation SetConfigSiConsommation(ConfigSiConsommation configSiConsommation)
        {
            try
            {
                var configSiConsommationRepository = new Repository<ConfigSiConsommation>();
                configSiConsommationRepository.Create(configSiConsommation);
                _unitOfWork.SaveChanges();
                return configSiConsommation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tarifId"></param>
        /// <returns></returns>
        public Tarif GetTarifById(int tarifId)
        {

            var tarifRepository = new Repository<Tarif>();
            var tarif = tarifRepository.Filter(p => p.Id == tarifId).AsNoTracking();
            return tarif.Any() ? tarif.Single() : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultat"></param>
        /// <returns></returns>
        public Resultat SauvegardeResultat(Resultat resultat)
        {
            try
            {
                var repository = new Repository<Resultat>();
                repository.Create(resultat);
                _unitOfWork.SaveChanges();
                return resultat;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configSiId"></param>
        /// <returns></returns>
        public List<Resultat> GetResultats(int configSiId)
        {
            try
            {
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager GetResultats ");
                var repository = new Repository<Resultat>();
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                //var listeCategorieDevice = repository.All().Select(c => new {Resultat=c, TypeDeviceTraductions=c.ConfigSiDevice.TypeDevice.TypeDeviceTraductions.Where(td=>td.LangueId==langueId)}).AsEnumerable().Select(c=>c.Resultat).Where(c => c.ConfigSiId == configSiId).ToList();                 

                var liste = repository.Filter(o => o.ConfigSiId == configSiId)
                                      .Include(o => o.ConfigSiDevice)
                                      .Include(o => o.ConfigSiDevice.ConfigSiConsommations)
                                      .Include(o => o.ConfigSiDevice.TypeDevice)
                                      .ToList();

                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager GetResultats ");
                return liste;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public Document SauvegarderDocument(Document document)
        {
            try
            {
                var repository = new Repository<Document>();
                var doc = repository.Create(document);
                _unitOfWork.SaveChanges();
                return doc;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<Document> GetListeDocument(int? page, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Document>();
                var documents = repository.All()
                                      .OrderBy(o => o.Id);//.ToList();
                totalPages = documents.Count();

                if (page.HasValue)
                {
                    documents = (IOrderedQueryable<Document>)documents.Skip((page.Value - 1) * pageDimension).Take(pageDimension);
                }
                else
                {
                    documents = documents.Take(pageDimension) as IOrderedQueryable<Document>;
                }
                return documents.ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            totalPages = 0;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SupprimerDocument(int id)
        {
            var repository = new Repository<Document>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Document id={0}", id);
                LogguerErreur(dex);
                throw;
            }  
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CodeInterneExiste(string code)
        {
            var clientRepository = new Repository<Client>();
            var client = clientRepository.Filter(p => p.CodeInterneWattsGreen == code).AsNoTracking();
            return client.Any() ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ConfigSi GetConfigSi(int id)
        {
            try
            {
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager GetConfigSi ");

                var repository = new Repository<ConfigSi>();
                var configSi = repository.Find(id);
                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager GetConfigSi ");
                return configSi;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        public ConfigSi GetConfigSiDetails(int id)
        {
            try
            {
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager GetConfigSi ");

                var repository = new Repository<ConfigSi>();
                var configSi =
                    repository.Filter(o => o.Id == id)
                              .Include(o => o.Resultats)
                              .Include(o => o.ConfigSiDevices)
                              .Include("ConfigSiDevices.ConfigSiConsommations");
                              
               
                ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager GetConfigSi ");
                return configSi.FirstOrDefault();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// Actualiser un configSi
        /// </summary>
        /// <param name="configSi">Le ConfigSi a actualiser</param>
        /// <returns>Le ConfigSi actualise</returns>
        public ConfigSi MAJConfigSi(ConfigSi configSi)
        {
            if (configSi==null)
            {
                Logger.Debug("configSi null");
                return null;
            }

            try
            {
                Logger.Debug("configSi {0}", configSi.Id);
                var repository = new Repository<ConfigSi>();
                configSi.DateModification = DateTime.Now;
                var resultat = repository.Update(configSi);
               _unitOfWork.SaveChanges();
                return configSi;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere un document
        /// </summary>
        /// <param name="docId">L'identifiant du document</param>
        /// <returns>Le document</returns>
        public Document GetDocument(int docId)
        {
            try
            {
                Logger.Debug("document Id {0}", docId);
                var repository = new Repository<Document>();
                var document = repository.Find(o => o.Id == docId);
                return document;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des adresses emails
        /// </summary>
        /// <returns></returns>
        public List<string> GetListeEmails()
        {
            try
            {
                var repository = new Repository<Contact>();
                var contacts = repository.Filter(o => o.DateFin == null && o.Client.Statut != 2).ToList(); //Contact pas supprimé et client pas rejetté
                return contacts.Select(contact => contact.Email).ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reponse"></param>
        /// <returns></returns>
        public Reponse UpdateReponse(Reponse reponse)
        {
            if (reponse == null)
            {
                Logger.Debug("reponse null");
                return null;
            }

            try
            {
                Logger.Debug("reponse id {0}", reponse.Id);
                var repository = new Repository<Reponse>();
                var statut = repository.Update(reponse);
                Logger.Debug("Nombre de reponse id {0} mis a jour {1}", reponse.Id, statut);
                return reponse;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la langue pour un pays
        /// </summary>
        /// <param name="idPays">L'identifiant d pays</param>
        /// <returns>La langue du pays</returns>
        public Langue GetLangueParIdPays(int idPays)
        {
            try
            {
                Logger.Debug("idPays {0}", idPays);
                var repository = new Repository<Langue>();
                var langue = repository.Find(o => o.PaysId == idPays);
                return langue;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere un configSiDevice 
        /// </summary>
        /// <param name="libelle">Le libelle du configSiDevice</param>
        /// <returns>Le configSiDevice</returns>
        public List<ConfigSiDevice> GetConfigSiDeviceParLibelle(string libelle, int configSiId)
        {
            try
            {
                var repository = new Repository<ConfigSiDevice>();
                var configSiDevices = repository.Filter(o => o.LibelleDevice == libelle && o.ConfigSiId == configSiId).ToList();
                return configSiDevices.Any() ? configSiDevices : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere resultat pour un configSiDevice
        /// </summary>
        /// <param name="configSiDeviceId"></param>
        /// <returns></returns>
        public Resultat GetResultatParConfigSiDevice(int configSiDeviceId)
        {
            try
            {
                Logger.Debug("configSiDeviceId {0}", configSiDeviceId);
                var repository = new Repository<Resultat>();
                var resultat = repository.Find(o => o.ConfigSiDeviceId == configSiDeviceId);
                if (resultat == null)
                    Logger.Debug("resultat for configSiDeviceId {0} = null", configSiDeviceId);

                return resultat;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Mis a jour d'un resultat
        /// </summary>
        /// <param name="resultat"></param>
        /// <returns></returns>
        public Resultat MAJResultat(Resultat resultat)
        {
            if (resultat == null)
            {
                Logger.Debug("resultat null");
                return null;
            }

            try
            {
                Logger.Debug(" ConfigSiId={0} ConfigSiDeviceId={1}", resultat.ConfigSiId, resultat.ConfigSiDeviceId);
                var repository = new Repository<Resultat>();                       
                repository.Update(resultat);
                _unitOfWork.SaveChanges();
                return resultat;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Sauvegarde une simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public Simulation SauvegardeSimulation(Simulation simulation)
        {
            if (simulation == null)
            {
                Logger.Debug("simulation null");
                return null;
            }

            try
            {
                Logger.Debug("Libelle simulation {0}", simulation.LibelleSimulation);
                var repository = new Repository<Simulation>();
                simulation = repository.Create(simulation);
                _unitOfWork.SaveChanges();
                return simulation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        ///  Recupere le ConfigSiClient d'un client
        /// </summary>
        /// <param name="idClient">L'identifiant du client</param>
        /// <returns>Le ConfigSiClient</returns>
        public ConfigSiClient GetConfigSiPourClientAvecStatutModifie(int idClient)
        {           
            var repositoryConfigSiClient = new Repository<ConfigSiClient>();

            try
            {
                Logger.Debug("id client {0}", idClient);
                var configSiClients = repositoryConfigSiClient.Filter(c => c.ClientId == idClient && c.DateFin == null)
                                                              .Include(c => c.ConfigSi)
                                                              .Include(c => c.ConfigSi.ConfigSiDevices)
                                                              .Include(c => c.ConfigSi.Resultats)
                                                              .Include("ConfigSi.ConfigSiDevices.ConfigSiConsommations")
                                                              .ToList();

                var origine = GetOrigineParLibelle(OrigineEnum.Actuel.ToString());
                var configSiClient = configSiClients.Any()
                                         ? configSiClients.Where(o => o.OrigineId == origine.Id).OrderBy(o => o.DateDebut)
                                                          .First()
                                         : null;
                                         //? configSiClients.Where(o => o.OrigineId == origine.Id && o.ConfigSi.Statut == (int) ConfigSiStatut.Modifier)
                                                         
                return configSiClient;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }            
        }       

        /// <summary>
        /// Sauvegarder un rapport
        /// </summary>
        /// <param name="rapport">Rapport </param>
        /// <returns></returns>
        public Rapport SauvegarderRapport(Rapport rapport)
        {
            if (rapport == null)
            {
                Logger.Debug("Rapport null");
                return null;
            }

            try
            {
                Logger.Debug("LibelleRapport {0}", rapport.LibelleRapport);
                var repository = new Repository<Rapport>();
                rapport = repository.Create(rapport);
                _unitOfWork.SaveChanges();
                return rapport;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la simulation en cours pour un contact (administrateur)
        /// </summary>
        /// <param name="idContact">L'identifiant du contact</param>
        /// <returns></returns>
        public Simulation GetSimulationEnCours(int idContact)
        {
            try
            {
                Logger.Debug("idContact {0}", idContact);
                var debut = ConsommationHelper.GetStartTime("EconocomDataManager GetSimulationEnCours ");
                var repository = new Repository<Simulation>();
                var simulation =  repository.Filter(o => o.ContactId == idContact && o.Statut == (int) SimulationStatut.EnCours);
                if (simulation.Any())
                {
                    var lastSimulation = simulation.ToList().LastOrDefault();
                    if (lastSimulation != null)
                        Logger.Debug(lastSimulation.DateDebut);
                    var firstSimulation = simulation.ToList().FirstOrDefault();

                    if (firstSimulation != null)
                        Logger.Debug(firstSimulation.DateDebut);
                    ConsommationHelper.GetTimeDifference(debut, "EconocomDataManager GetSimulationEnCours ");
                    return lastSimulation ?? null;
                }
               
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public List<Simulation> GetSimulationsEnCours(int idContact)
        {
            try
            {
                Logger.Debug("idContact {0}", idContact);
                var repository = new Repository<Simulation>();
                var simulation = repository.Filter(o => o.ContactId == idContact && o.Statut == (int)SimulationStatut.EnCours);
                return simulation.ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }           
        }

        /// <summary>
        /// Recupere une config si
        /// </summary>
        /// <param name="idConfigSi">L'identifiant de la config si</param>
        /// <returns>La config si</returns>
        public ConfigSi GetConfigSiParId(int idConfigSi)
        {
            var repository = new Repository<ConfigSi>();

            try
            {
                Logger.Debug("idConfigSi {0}", idConfigSi);
                var configSi = repository.Filter(c => c.Id == idConfigSi && c.DateFin == null)
                                         .Include(c => c.ConfigSiDevices)
                                         .Include(c => c.Resultats)
                                         .Include("Resultats.ConfigSiDevice.TypeDevice")
                                         .Include("ConfigSiDevices.ConfigSiConsommations");

                return configSi.Any() ? configSi.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }
            return null;
        }

        /// <summary>
        /// Recupere la liste des rapports de personnalisation
        /// </summary>
        /// <param name="id">L'identifiant du rapport</param>
        /// <param name="search">Mot cle a rechercher</param>
        /// <param name="page">La page dans la pagination</param>
        /// <param name="sort">Le champs a filtrer</param>
        /// <param name="sortdir">Ascendant ou descendant</param>
        /// <param name="pageDimension">Le nombre total de rapport par page</param>
        /// <param name="totalPage">Le nombre total de page</param>
        /// <returns>La liste des rapports depPersonnalisation</returns>
        public List<Rapport> GetRapportPersonnalisation(int? id, string search, int? page, string sort, string sortdir,
                                                        int pageDimension, out int totalPage)
        {
            try
            {
                totalPage = 1;
                Logger.Debug("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}, pageDimension={5}", id, search,
                            page, sort, sortdir, pageDimension);
                var repository = new Repository<Rapport>();
                var rapport = repository.All().Where(o=>o.Contact.ClientId==id && o.Origine.LibelleOrigine == "Personnalisation");
                if (!string.IsNullOrEmpty(search))
                    rapport = rapport.Where(o => (o.LibelleRapport.ToLower().Contains(search.ToLower())));
                if (sortdir == null || sortdir.ToLowerInvariant() == "asc")
                {
                    if (sort == null) rapport = rapport.OrderBy(o => o.Id);
                    else if (sort.ToLowerInvariant() == "libellerapport") rapport = rapport.OrderBy(o => o.LibelleRapport);
                    else if (sort.ToLowerInvariant() == "datedebut") rapport = rapport.OrderBy(o => o.DateDebut);
                    else if (sort.ToLowerInvariant() == "heurecree") rapport = rapport.OrderBy(o => o.HeureCree);
                }
                else if (sortdir.ToLowerInvariant() == "desc")
                {
                    if (sort == null) rapport = rapport.OrderBy(o => o.Id);
                    else if (sort.ToLowerInvariant() == "libellerapport") rapport = rapport.OrderByDescending(o => o.LibelleRapport);
                    else if (sort.ToLowerInvariant() == "datedebut") rapport = rapport.OrderByDescending(o => o.DateDebut);
                    else if (sort.ToLowerInvariant() == "heurecree") rapport = rapport.OrderByDescending(o => o.HeureCree);
                }

                totalPage = rapport.Count();
                if (page.HasValue)
                {
                    rapport = rapport.Skip((page.Value - 1)*pageDimension).Take(pageDimension);
                }
                else
                {
                    rapport = rapport.Take(pageDimension);
                }

                var rapportListe = rapport.ToList();

                return rapportListe;
            }
            catch (Exception e)
            {               
                LogguerErreur(e);
                throw;
            }
            
        }

        /// <summary>
        /// Recupere la liste des rapports de simulation
        /// </summary>
        /// <param name="id">L'identifiant du rapport</param>
        /// <param name="search">Mot cle a rechercher</param>
        /// <param name="page">La page dans la pagination</param>
        /// <param name="sort">Le champs a filtrer</param>
        /// <param name="sortdir">Ascendant ou descendant</param>
        /// <param name="pageDimension">Le nombre total de rapport par page</param>
        /// <param name="totalPage">Le nombre total de page</param>
        /// <returns>La liste des rapports de simulation</returns>
        public List<Rapport> GetRapportSimulation(int? id, string search, int? page, string sort, string sortdir,
                                                  int pageDimension, out int totalPage)
        {
            try
            {
                totalPage = 1;
                Logger.Debug("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}, pageDimension={5}", id, search,
                            page, sort, sortdir, pageDimension);
                var repository = new Repository<Rapport>();
                var rapport = repository.All().Where(o => o.Contact.ClientId == id && o.Origine.LibelleOrigine == "Simulation");
                if (!string.IsNullOrEmpty(search))
                    rapport = rapport.Where(o => (o.LibelleRapport.ToLower().Contains(search.ToLower())));
                if (sortdir == null || sortdir.ToLowerInvariant() == "asc")
                {
                    if (sort == null) rapport = rapport.OrderBy(o => o.Id);
                    else if (sort.ToLowerInvariant() == "libellerapport") rapport = rapport.OrderBy(o => o.LibelleRapport);
                    else if (sort.ToLowerInvariant() == "datedebut") rapport = rapport.OrderBy(o => o.DateDebut);
                    else if (sort.ToLowerInvariant() == "heurecree") rapport = rapport.OrderBy(o => o.HeureCree);
                }
                else if (sortdir.ToLowerInvariant() == "desc")
                {
                    if (sort == null) rapport = rapport.OrderBy(o => o.Id);
                    else if (sort.ToLowerInvariant() == "libellerapport") rapport = rapport.OrderByDescending(o => o.LibelleRapport);
                    else if (sort.ToLowerInvariant() == "datedebut") rapport = rapport.OrderByDescending(o => o.DateDebut);
                    else if (sort.ToLowerInvariant() == "heurecree") rapport = rapport.OrderByDescending(o => o.HeureCree);
                }
                totalPage = rapport.Count();
                if (page.HasValue)
                {
                    rapport = rapport.Skip((page.Value - 1)*pageDimension).Take(pageDimension);
                }
                else
                {
                    rapport = rapport.Take(pageDimension);
                }
                var rapportListe = rapport.ToList();

                return rapportListe;
            }
            catch (Exception e)
            {               
                LogguerErreur(e);                
                throw;
            }           
        }

        /// <summary>
        /// Mis a jour d'une simulation
        /// </summary>
        /// <param name="simulation">La simulation a mettre a jour</param>
        /// <returns>La simulation mis a jour</returns>
        public Simulation MAJSimulation(Simulation simulation)
        {
            try
            {
                Logger.Debug("simulation id {0}, Libelle Simulation={1}", simulation.Id, simulation.LibelleSimulation);
                var repository = new Repository<Simulation>();
                simulation.DateModification = DateTime.Now;
                repository.Update(simulation);
                _unitOfWork.SaveChanges();
                return simulation;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return simulation;
        }

        /// <summary>
        /// Recupere la liste des equivalences
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageDimension"></param>
        /// <param name="sort"></param>
        /// <param name="sortdir"></param>
        /// <param name="totalPage"></param>
        /// <returns>la liste des equivalences</returns>
        public List<Equivalence> GetListeEquivalence(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                Logger.Debug("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                var repository = new Repository<Equivalence>();
                var listeEquivalence = repository.All().Where(o => o.DateFin == null);
                if ((sortdir == null || sortdir.ToLowerInvariant() == "asc") && sort != null)
                {
                    if (sort.ToLowerInvariant() == "libelleequivalence") listeEquivalence = listeEquivalence.OrderBy(o => o.LibelleEquivalence);
                    else if (sort.ToLowerInvariant() == "datedebut") listeEquivalence = listeEquivalence.OrderBy(o => o.DateDebut);
                }
                else if (sortdir != null && sortdir.ToLowerInvariant() == "desc" && sort != null)
                {
                    if (sort.ToLowerInvariant() == "libelleequivalence") listeEquivalence = listeEquivalence.OrderByDescending(o => o.LibelleEquivalence);
                    else if (sort.ToLowerInvariant() == "datedebut") listeEquivalence = listeEquivalence.OrderByDescending(o => o.DateDebut);
                }
                else
                {
                    listeEquivalence = listeEquivalence.OrderBy(o => o.Id);
                }

                if (listeEquivalence.Any())
                    totalPage = listeEquivalence.Count();

                if (page.HasValue)
                {
                    listeEquivalence = listeEquivalence.Skip((page.Value - 1) * pageDimension).Take(pageDimension);
                }
                else
                {
                    listeEquivalence = listeEquivalence.Skip(0).Take(pageDimension);
                }

                if (listeEquivalence.Any())
                {
                    var rapportListe = listeEquivalence.ToList();
                    return rapportListe;
                }
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere la liste des politiques
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageDimension"></param>
        /// <param name="sort"></param>
        /// <param name="sortdir"></param>
        /// <param name="totalPage"></param>
        /// <returns>la liste des politiques</returns>
        public List<Politique> GetListePolitique(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                Logger.Debug("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                var repository = new Repository<Politique>();
                var listePolitique = repository.All().Where(o => o.DateFin==null);
                if ((sortdir == null || sortdir.ToLowerInvariant() == "asc") && sort != null)
                {
                    if (sort.ToLowerInvariant() == "libellepolitique") listePolitique = listePolitique.OrderBy(o => o.LibellePolitique);
                    else if (sort.ToLowerInvariant() == "datedebut") listePolitique = listePolitique.OrderBy(o => o.DateDebut);
                }
                else if (sortdir != null && sortdir.ToLowerInvariant() == "desc" && sort != null)
                {
                    if (sort.ToLowerInvariant() == "libellepolitique") listePolitique = listePolitique.OrderByDescending(o => o.LibellePolitique);
                    else if (sort.ToLowerInvariant() == "datedebut") listePolitique = listePolitique.OrderByDescending(o => o.DateDebut);
                }
                else
                {
                    listePolitique = listePolitique.OrderBy(o => o.Id);
                }

                if (listePolitique.Any())
                    totalPage = listePolitique.Count();

                if (page.HasValue)
                {
                    listePolitique = listePolitique.Skip((page.Value - 1) * pageDimension).Take(pageDimension);
                }
                else
                {
                    listePolitique = listePolitique.Skip(0).Take(pageDimension);
                }

                if (listePolitique.Any())
                {
                    var rapportListe = listePolitique.ToList();
                    return rapportListe;
                }
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere une equivalence
        /// </summary>
        /// <param name="id">L'identifiant de l'equivalence</param>
        /// <returns>L'equivalence</returns>
        public Equivalence GetEquivalence(int id)
        {
            var repository = new Repository<Equivalence>();
            Logger.Debug("equivalence id={0}", id);
            try
            {
                var equivalence = repository.Filter(c => c.Id == id);
                return equivalence.Any() ? equivalence.Single() : null;
            }
            catch (Exception e)
            {               
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere une politiqe
        /// </summary>
        /// <param name="id">L'identifiant de la  politique</param>
        /// <returns>La politique</returns>
        public Politique GetPolitique(int id)
        {
            var repository = new Repository<Politique>();

            try
            {
                Logger.Debug("politique id={0}", id);
                var politique = repository.Filter(c => c.Id == id);
                return politique.Any() ? politique.Single() : null;
            }
            catch (Exception e)
            {               
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere le type device
        /// </summary>
        /// <param name="id">L'identifiant du type device</param>
        /// <returns>Le typedevice</returns>
        public TypeDevice GetTypeDeviceParId(int id)
        {
            var repository = new Repository<TypeDevice>();
            Logger.Debug("id type device {0}", id);
            try
            {
                var typeDevice = repository.Filter(o => o.Id == id)
                                         .Include(o => o.CategorieDevice)
                                         .Include(o => o.CategorieDevice.FamilleDevice);

                return typeDevice.Any() ? typeDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere la liste des equivalences
        /// </summary>
        /// <returns>la liste des equivalences</returns>
        public List<Equivalence> GetListeEquivalence()
        {
            try
            {
                var repository = new Repository<Equivalence>();

                var equivalences = repository.All().ToList();
                return equivalences;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere un rapport avec les informations sur la consommation
        /// </summary>
        /// <param name="rapportId">L'identifiant de la page</param>
        /// <returns>un rapport avec les informations sur la consommation</returns>
        public Rapport GetRapport(int rapportId)
        {
            var repository = new Repository<Rapport>();
            try
            {
                Logger.Debug("id Rapport {0}", rapportId);
                var rapport = repository.Filter(c => c.Id == rapportId)
                                        .Include("ConfigSi")
                                        .Include("ConfigSi.ConfigSiDevices")
                                        .Include("ConfigSi.ConfigSiDevices.TypeDevice")
                                        .Include("ConfigSi.ConfigSiDevices.TypeDevice.TypeDeviceTraductions")
                                        .Include("ConfigSi.ConfigSiDevices.TypeDevice.CategorieDevice")
                                        .Include("ConfigSi.ConfigSiDevices.TypeDevice.CategorieDevice.CategorieDeviceTraductions")
                                        .Include("ConfigSi.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice")
                                        .Include("ConfigSi.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice.FamilleDeviceTraductions")
                                        .Include("ConfigSi.ConfigSiDevices.ConfigSiConsommations")
                                        .OrderBy(o => o.ConfigSi.ConfigSiDevices
                                                       .OrderBy(d => d.TypeDevice.CategorieDeviceId)                                                       
                                                       .FirstOrDefault().TypeDeviceId);

                var result = rapport.Any() ? rapport.FirstOrDefault() : null;

                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                if (result != null)
                {
                    var configSiDevices = result.ConfigSi.ConfigSiDevices;

                    //Traduction configSiDevice
                    if (configSiDevices !=null)
                    {
                        foreach (var configSiDevice in configSiDevices)
                        {
                            var deviceTraductions = configSiDevice.TypeDevice.TypeDeviceTraductions.ToList();
                            var deviceTraduit = deviceTraductions.Find(o => o.LangueId == langueId);
                            if (configSiDevice.Source == (int)ConfigSiDeviceType.Benchmark)
                            {
                                configSiDevice.LibelleDevice = deviceTraduit != null ? deviceTraduit.LibelleTypeDevice : configSiDevice.LibelleDevice;
                            }

                            //Traduction categorieDevice
                            if (configSiDevice.TypeDevice.CategorieDevice != null)  
                            {
                                var categorieTraductions = configSiDevice.TypeDevice.CategorieDevice.CategorieDeviceTraductions.ToList();
                                var categorieTraduit = categorieTraductions.Find(o => o.LangueId == langueId);
                                configSiDevice.TypeDevice.CategorieDevice.LibelleCategorieDevice = categorieTraduit != null ? categorieTraduit.LibelleCategorieDevice : configSiDevice.TypeDevice.CategorieDevice.LibelleCategorieDevice;
                            
                                
                                //Traduction familleDevice
                                if (configSiDevice.TypeDevice.CategorieDevice != null)
                                {
                                    var familleTraductions = configSiDevice.TypeDevice.CategorieDevice.FamilleDevice.FamilleDeviceTraductions.ToList();
                                    var familleTraduit = familleTraductions.Find(o => o.LangueId == langueId);
                                    configSiDevice.TypeDevice.CategorieDevice.FamilleDevice.LibelleFamilleDevice = familleTraduit != null ? familleTraduit.LibelleFamilleDevice : configSiDevice.TypeDevice.CategorieDevice.FamilleDevice.LibelleFamilleDevice;
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {               
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere la liste des clients entre 2 dates, avec pagination
        /// </summary>
        /// <param name="dateDebut">Date debut du filtrage</param>
        /// <param name="dateFin">date fin du filtrage</param>
        /// <param name="page">Le numero de la page dans la pagination</param>
        /// <param name="pageDimension">Le nombre de clients par page</param>
        /// <param name="totalPages">Le nombre total de pages</param>
        /// <returns></returns>
        public List<Client> GetListeClientsSouscrits(DateTime dateDebut, DateTime dateFin, int? page, int pageDimension, out int totalPages)
        {
            try
            {
                totalPages = 0;
                Logger.Debug("page {0}, pageDimension {1}, dateDebut {2}, dateFin{3}", page, pageDimension, dateDebut.ToShortDateString(), dateFin.ToShortDateString());
                var repository = new Repository<Client>();

                var clients = repository.Filter(o => (o.DateDebut >= dateDebut && o.DateDebut <= dateFin))
                                        .Include("Adresse")
                                        .Include("Adresse.Pays")
                                        .Include("DetailsClient")
                                        .Include("DetailsClient.Tarif")
                                        .Include("Contacts")
                                        .Include("Contacts.TypeContact")
                                        .OrderBy(o=>o.Id);
                if (clients.Any())
                {
                    totalPages = clients.Count();
                    if (page.HasValue)
                    {
                        clients =
                            (IOrderedQueryable<Client>) clients.Skip((page.Value - 1)*pageDimension).Take(pageDimension);
                    }
                    else
                    {
                        clients = clients.Take(pageDimension) as IOrderedQueryable<Client>;
                    }

                    return clients.ToList();
                }
                
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
           
            return null;
        }

        /// <summary>
        /// Recupere un rapport
        /// </summary>
        /// <param name="id">L'identifiant du rapport</param>
        /// <returns>Le rapport</returns>
        public Rapport GetRapportParId(int id)
        {
            var repository = new Repository<Rapport>();
            try
            {
                Logger.Debug("id Rapport {0}", id);
                var rapport = repository.Find(r => r.Id == id);
                return rapport;
            }
            catch (Exception e)
            {               
                LogguerErreur(e);
                throw;
            }           
        }

        /// <summary>
        /// Recupere une simulation
        /// </summary>
        /// <param name="idRapport">L'identifiant du rapport de simulation</param>
        /// <returns>La simulation</returns>
        public Simulation GetSimulationParIdRapport(int idRapport)
        {
            var repository = new Repository<Simulation>();
            try
            {
                Logger.Debug("{0}", idRapport);
                var rapport = GetRapportParId(idRapport);
                
               var simulation = repository.Filter(c => c.ConfigSiSimuleId == rapport.ConfigSiId)
                                  .Include("ConfigSiSimule")
                                  .Include("ConfigSiSimule.ConfigSiDevices")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.TypeDeviceTraductions")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.CategorieDevice")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.CategorieDevice.CategorieDeviceTraductions")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice.FamilleDeviceTraductions")
                                  .Include("ConfigSiSimule.ConfigSiDevices.ConfigSiConsommations")
                                  .Include("ConfigSiSource")
                                  .Include("ConfigSiSource.ConfigSiDevices")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.TypeDeviceTraductions")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.CategorieDevice")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.CategorieDevice.CategorieDeviceTraductions")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice.FamilleDeviceTraductions")
                                  .Include("ConfigSiSource.ConfigSiDevices.ConfigSiConsommations")
                                  .OrderBy(o => o.ConfigSiSimule.ConfigSiDevices
                                                 .OrderBy(d => d.TypeDevice.CategorieDeviceId)
                                                 .FirstOrDefault().TypeDeviceId);
                
                var result = simulation.Any() ? simulation.FirstOrDefault() : null;


                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                if (result != null)
                {
                    var configSiDevicesSimule = result.ConfigSiSimule.ConfigSiDevices;
                    var configSiDevicesSource = result.ConfigSiSource.ConfigSiDevices;

                    //Traduction configSiDevicesSimule
                    if (configSiDevicesSimule != null)
                    {
                        foreach (var configSiDeviceSimule in configSiDevicesSimule)
                        {
                            var deviceSimuleTraductions = configSiDeviceSimule.TypeDevice.TypeDeviceTraductions.ToList();
                            var deviceSimuleTraduit = deviceSimuleTraductions.Find(o => o.LangueId == langueId);
                            if (configSiDeviceSimule.Source == (int)ConfigSiDeviceType.Benchmark)
                            {
                                configSiDeviceSimule.LibelleDevice = deviceSimuleTraduit != null ? deviceSimuleTraduit.LibelleTypeDevice : configSiDeviceSimule.LibelleDevice;
                            }

                            //Traduction categorieDevicesSimule
                            if (configSiDeviceSimule.TypeDevice.CategorieDevice != null)
                            {
                                var categorieSimuleTraductions = configSiDeviceSimule.TypeDevice.CategorieDevice.CategorieDeviceTraductions.ToList();
                                var categorieSimuleTraduit = categorieSimuleTraductions.Find(o => o.LangueId == langueId);
                                configSiDeviceSimule.TypeDevice.CategorieDevice.LibelleCategorieDevice = categorieSimuleTraduit != null ? categorieSimuleTraduit.LibelleCategorieDevice : configSiDeviceSimule.TypeDevice.CategorieDevice.LibelleCategorieDevice;


                                //Traduction familleDevicesSimule
                                if (configSiDeviceSimule.TypeDevice.CategorieDevice != null)
                                {
                                    var familleSimuleTraductions = configSiDeviceSimule.TypeDevice.CategorieDevice.FamilleDevice.FamilleDeviceTraductions.ToList();
                                    var familleSimuleTraduit = familleSimuleTraductions.Find(o => o.LangueId == langueId);
                                    configSiDeviceSimule.TypeDevice.CategorieDevice.FamilleDevice.LibelleFamilleDevice = familleSimuleTraduit != null ? familleSimuleTraduit.LibelleFamilleDevice : configSiDeviceSimule.TypeDevice.CategorieDevice.FamilleDevice.LibelleFamilleDevice;
                                }
                            }
                        }
                    }


                    //Traduction configSiDevicesSource
                    if (configSiDevicesSource != null)
                    {
                        foreach (var configSiDeviceSource in configSiDevicesSource)
                        {
                            var deviceSourceTraductions = configSiDeviceSource.TypeDevice.TypeDeviceTraductions.ToList();
                            var deviceSourceTraduit = deviceSourceTraductions.Find(o => o.LangueId == langueId);
                            if (configSiDeviceSource.Source == (int)ConfigSiDeviceType.Benchmark)
                            {
                                configSiDeviceSource.LibelleDevice = deviceSourceTraduit != null ? deviceSourceTraduit.LibelleTypeDevice : configSiDeviceSource.LibelleDevice;
                            }

                            //Traduction categorieDevicesSimule
                            if (configSiDeviceSource.TypeDevice.CategorieDevice != null)
                            {
                                var categorieSourceTraductions = configSiDeviceSource.TypeDevice.CategorieDevice.CategorieDeviceTraductions.ToList();
                                var categorieSourceTraduit = categorieSourceTraductions.Find(o => o.LangueId == langueId);
                                configSiDeviceSource.TypeDevice.CategorieDevice.LibelleCategorieDevice = categorieSourceTraduit != null ? categorieSourceTraduit.LibelleCategorieDevice : configSiDeviceSource.TypeDevice.CategorieDevice.LibelleCategorieDevice;


                                //Traduction familleDevicesSimule
                                if (configSiDeviceSource.TypeDevice.CategorieDevice != null)
                                {
                                    var familleSourceTraductions = configSiDeviceSource.TypeDevice.CategorieDevice.FamilleDevice.FamilleDeviceTraductions.ToList();
                                    var familleSourceTraduit = familleSourceTraductions.Find(o => o.LangueId == langueId);
                                    configSiDeviceSource.TypeDevice.CategorieDevice.FamilleDevice.LibelleFamilleDevice = familleSourceTraduit != null ? familleSourceTraduit.LibelleFamilleDevice : configSiDeviceSource.TypeDevice.CategorieDevice.FamilleDevice.LibelleFamilleDevice;
                                }
                            }
                        }
                    }
                }
                
                
                /*
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var liste = repository.All().Select(
                                            t =>
                                            new
                                            {
                                                Simulation = t,
                                                TypeDeviceTraductions =
                                            t.ConfigSiSource.ConfigSiDevices.Any(ft => ft.TypeDevice.TypeDeviceTraductions.Any(l => l.LangueId == langueId))
                                            }).Where(c => c.Simulation.ConfigSiSimuleId == rapport.ConfigSiId).Select(f => f.Simulation);

                var subQuery = liste.Include("ConfigSiSimule")
                                  .Include("ConfigSiSimule.ConfigSiDevices")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.CategorieDevice")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.TypeDeviceTraductions")
                                  .Include("ConfigSiSimule.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice")
                                  .Include("ConfigSiSimule.ConfigSiDevices.ConfigSiConsommations")
                                  .Include("ConfigSiSource")
                                  .Include("ConfigSiSource.ConfigSiDevices")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.CategorieDevice")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.TypeDeviceTraductions")
                                  .Include("ConfigSiSource.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice")
                                  .Include("ConfigSiSource.ConfigSiDevices.ConfigSiConsommations")
                                  .OrderBy(o => o.ConfigSiSimule.ConfigSiDevices
                                                 .OrderBy(d => d.TypeDevice.CategorieDeviceId)
                                                 .FirstOrDefault().TypeDeviceId);
                //var resultFinale = subQuery.ToList();
                var resultFinale = subQuery.Any() ? subQuery.FirstOrDefault() : null;


                return resultFinale;*/

                return result;
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des clients
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <param name="search">Mots cle a rechercher</param>
        /// <param name="page">No. de page dans la pagination</param>
        /// <param name="sort">Nom de la colonne a filtrer</param>
        /// <param name="sortdir">Ascendante ou descendante</param>
        /// <param name="pageDimension">Nombre total de clients par page</param>
        /// <param name="totalPage">Nombre total de page</param>
        /// <returns>Litse des clients</returns>
        public List<Client> GetListeClients(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPage)       
        {
            totalPage = 1;
            try
            {
                Logger.Debug("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                var repository = new Repository<Client>();
                var listeClient = repository.All()
                                        .Where(o => o.Statut != 2) //Client pas rejeté
                                        .Include("Adresse")
                                        .Include("Adresse.Pays")
                                        .Include("DetailsClient")
                                        .Include("DetailsClient.Tarif")
                                        .Include("ConfigSIClients");
                long idClient = 0;
                var isId = Int64.TryParse(search, out idClient);
                if (!string.IsNullOrEmpty(search))
                    listeClient = listeClient.Where(o => (o.RaisonSociale.ToLower().Contains(search.ToLower()) || o.Id==idClient || o.Groupe.ToLower().Contains(search.ToLower()) || o.CodeNAF.ToLower().Contains(search.ToLower()) || o.IdentificationNational.ToLower().Contains(search.ToLower())));

                if ((sortdir == null || sortdir.ToLowerInvariant() == "asc") && sort != null)
                {
                    if (sort.ToLowerInvariant() == "id") listeClient = listeClient.OrderBy(o => o.Id);
                    else if (sort.ToLowerInvariant() == "raisonsociale") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "adresse1") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "ville") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "codepostal") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "libellepays") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "codeinternewattsgreen") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "groupe") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "datedebut") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "datevalidation") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "libelletarif") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "clienteconocom") listeClient = listeClient.OrderBy(o => o.RaisonSociale);
                    else listeClient = listeClient.OrderBy(o => o.Id);
                }
                else if (sortdir != null && sortdir.ToLowerInvariant() == "desc" && sort != null)
                {
                    if (sort.ToLowerInvariant() == "id") listeClient = listeClient.OrderByDescending(o => o.Id);
                    else if (sort.ToLowerInvariant() == "raisonsociale") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "adresse1") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "ville") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "codepostal") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "libellepays") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "codeinternewattsgreen") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "groupe") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "datedebut") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "datevalidation") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "libelletarif") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else if (sort.ToLowerInvariant() == "clienteconocom") listeClient = listeClient.OrderByDescending(o => o.RaisonSociale);
                    else listeClient = listeClient.OrderByDescending(o => o.Id);
                }
                else
                {
                    listeClient = listeClient.OrderBy(o => o.Id);
                }

                if (listeClient.Any())
                    totalPage = listeClient.Count();

                if (page.HasValue)
                {
                    listeClient = listeClient.Skip((page.Value - 1) * pageDimension).Take(pageDimension);
                }
                else
                {
                    listeClient = listeClient.Skip(0).Take(pageDimension);
                }

                if (listeClient.Any())
                {
                    var rapportListe = listeClient.ToList();
                    return rapportListe;
                }
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere la configsiclient pour generer un rapport
        /// </summary>
        /// <param name="idClient">L'identifiant du client</param>
        /// <returns>La configsiclient</returns>
        public ConfigSiClient GetConfigSiClientRapport(int idClient)
        {
            try
            {
                Logger.Debug("idClient {0}", idClient);
                var repository = new Repository<ConfigSiClient>();
                var configSiClients = repository.Filter(o => o.ClientId == idClient)
                                                .Include(o => o.ConfigSi)
                                                .Include(o => o.ConfigSi.ConfigSiDevices)
                                                .Include(o => o.ConfigSi.Resultats)
                                                .Include(o => o.Client)
                                                .Include("ConfigSi.ConfigSiDevices.TypeDevice")
                                                .Include("ConfigSi.ConfigSiDevices.TypeDevice.CategorieDevice")
                                                .Include("ConfigSi.ConfigSiDevices.TypeDevice.CategorieDevice.FamilleDevice")
                                                .Include("ConfigSi.ConfigSiDevices.ConfigSiConsommations")
                                                .ToList();
                return configSiClients.Any() ? configSiClients.OrderBy(o => o.DateDebut).First() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere le rapport pour un client
        /// </summary>
        /// <param name="clientId">L'identifiant du client</param>
        /// <returns>La rapport du client</returns>
        public Rapport GetRapportClient(int clientId)
        {           
            try
            {
                Logger.Debug("{0}", clientId);
                var configSiClient = GetConfigSiClientRapport(clientId);
                if (configSiClient != null)
                {
                    var libelleRapport = clientId.ToString();
                    if (configSiClient.Client != null)
                        libelleRapport = String.Format(Traduction.LibelleRapportClient, configSiClient.Client.RaisonSociale);
                    var rapport = new Rapport {ConfigSi = configSiClient.ConfigSi, LibelleRapport = libelleRapport, DateDebut = DateTime.Now};

                    return rapport;
                }
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere la liste des souscriptions
        /// </summary>
        /// <param name="dateDebut">Date de debut pour le filtrage</param>
        /// <param name="dateFin">Date de fin pour le filtrage</param>
        /// <returns>la liste des souscriptions</returns>
        public List<Client> GetListeSouscriptions(DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                Logger.Debug("dateDebut={0}, dateFin={1} ", dateDebut, dateFin);
                var repository = new Repository<Client>();

                var clients = repository.Filter(o => (o.DateDebut >= dateDebut && o.DateDebut <= dateFin))
                                        .Include("Adresse")
                                        .Include("Adresse.Pays")
                                        .Include("DetailsClient")
                                        .Include("DetailsClient.Tarif")
                                        .Include("Contacts")
                                        .Include("Contacts.TypeContact");

                return clients.Any() ? clients.ToList() : null;
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }           
        }

        /// <summary>
        /// Sauvegarde une politique
        /// </summary>
        /// <param name="politique">La politique a sauvegarder</param>
        /// <returns>La nouvelle politique</returns>
        public Politique SauvegardePolitique(Politique politique)
        {
            try
            {
                Logger.Debug("politique {0}", politique.LibellePolitique);
                var repository = new Repository<Politique>();
                politique = repository.Create(politique);
                _unitOfWork.SaveChanges();
                return politique;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere l'utilisateur commercial
        /// </summary>
        /// <returns>L'utilisateur commercial</returns>
        public Utilisateur GetCommercial()
        {
            try
            {
                var repository = new Repository<Utilisateur>();
                var commercial = repository.Filter(o => o.Authorization == (int)TypeContactEnum.Commercial).FirstOrDefault();
                return commercial;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere l'administrater du site
        /// </summary>
        /// <returns>L'utilisateur administrateur</returns>
        public Utilisateur GetAdministrateur()
        {
            try
            {
                var repository = new Repository<Utilisateur>();
                var administrateur = repository.Filter(o => o.Authorization == (int) TypeContactEnum.Administrateur).FirstOrDefault();
                return administrateur;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere la derniere politique
        /// </summary>
        /// <returns>La derniere politique</returns>
        public Politique GetDernierPolitique()
        {
            try
            {
                var repository = new Repository<Politique>();

                var politiques = repository.All().ToList();
                return politiques.Last();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Mise a jour d'une politique
        /// </summary>
        /// <param name="politique">La politiqe a mettre a jour</param>
        /// <returns>La politiqe mise a jour</returns>
        public Politique MAJPolitique(Politique politique)
        {
            try
            {
                var repository = new Repository<Politique>();
                politique.DateModification = DateTime.Now;
                repository.Update(politique);
                _unitOfWork.SaveChanges();
                return politique;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere le dernier equivalence
        /// </summary>
        /// <returns>Le dernier equivalence</returns>
        public Equivalence GetDernierEquivalence()
        {
            try
            {
                var repository = new Repository<Equivalence>();

                var equivalences = repository.All().ToList();
                return equivalences.Last();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Sauvegarde un equivalence
        /// </summary>
        /// <param name="equivalence">L'equivalence a mettre a jour</param>
        /// <returns>L'equivalence mis a jour</returns>
        public Equivalence SauvegardeEquivalence(Equivalence equivalence)
        {
            try
            {
                Logger.Debug("Id {0}, LibelleEquivalence {1}", equivalence.Id, equivalence.LibelleEquivalence);
                var repository = new Repository<Equivalence>();
                equivalence = repository.Create(equivalence);
                _unitOfWork.SaveChanges();
                return equivalence;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Mis a jour d'un equivalence
        /// </summary>
        /// <param name="equivalence">L'eqivalence a mettre a jour</param>
        /// <returns>L'eqivalence mis a jour</returns>
        public Equivalence MAJEquivalence(Equivalence equivalence)
        {
            if (equivalence == null)
                return null;

            try
            {
                Logger.Debug("Id {0}, LibelleEquivalence {1}", equivalence.Id, equivalence.LibelleEquivalence);
                var repository = new Repository<Equivalence>();
                equivalence.DateModification = DateTime.Now;
                repository.Update(equivalence);
                _unitOfWork.SaveChanges();
                return equivalence;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere la liste des equivalencesOrigine
        /// </summary>
        /// <returns>La liste des equivalences</returns>
        public List<Equivalence> GetListeEquivalencePourAfficher()
        {
            try
            {
                var idLangue = 1;
                var langue = GetLangue();

                if (langue != null)
                {
                    idLangue = langue.Id;
                }

                var repository = new Repository<Equivalence>();
                var equivalences = repository.All()
                    .Select(t => new { Equivalence = t, EquivalenceTraduction = t.EquivalenceTraductions.Where(l => l.LangueId == idLangue) }).AsEnumerable().Select(t => t.Equivalence).Where(o => (o.Afficher == true && o.DateFin == null)).OrderByDescending(o => o.Id).Take(5);
                if (equivalences.Any())
                {
                    var liste = equivalences.ToList();
                    return liste;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere le nombre des clients valides
        /// </summary>
        /// <returns>le nombre des clients valides</returns>
        public int GetNombreClientsValide()
        {
            var nombre = 0;
            try
            {
                var listeClients = GetListeClientsValide(null, null);
                if (listeClients != null)
                    nombre = listeClients.Count;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return nombre;
        }

        /// <summary>
        /// Recupere la liste des clients valides avec pagination
        /// </summary>
        /// <param name="page">La page de la pagination</param>
        /// <param name="pageDimension">Le nombre des clients par page</param>
        /// <returns></returns>
        public List<Client> GetListeClientsValide(int? page, int? pageDimension)
        {           
            try
            {
                Logger.Debug("page {0}, pageDimension {1}", page, pageDimension);
                var repository = new Repository<Client>();
                var listeClient = repository.All().Where(o=>(o.Statut==(int)ClientStatut.Valide && o.SecteurActiviteId!=null) ).OrderBy(o => o.Id);
                Logger.Debug("liste 1");
                if (page.HasValue && page.Value>0 && pageDimension.HasValue && pageDimension.Value>0)
                {
                    listeClient = (IOrderedQueryable<Client>) listeClient.Skip((page.Value - 1) * pageDimension.Value).Take(pageDimension.Value);
                }
                else if (pageDimension.HasValue)
                {
                    listeClient = (IOrderedQueryable<Client>) listeClient.Skip(0).Take(pageDimension.Value);
                }
                Logger.Debug("liste 2");

                if (listeClient.Any())
                {
                    Logger.Debug("liste {0}", listeClient.Count());
                    var rapportListe = listeClient.ToList();
                    return rapportListe;
                }
            }
            catch (Exception e)
            {                
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Recupere la liste de ChangementAnneeCalendaire
        /// </summary>
        /// <returns>la liste de ChangementAnneeCalendaire</returns>
        public List<ChangementAnneeCalendaire> GetListeChangementAnneeCalendaire()
        {
            try
            {
                var repository = new Repository<ChangementAnneeCalendaire>();
                var listeChangementAnneeCalendaire = repository.All();

                if (listeChangementAnneeCalendaire.Any())
                {
                    var liste = listeChangementAnneeCalendaire.ToList();
                    return liste;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Mis a jour d'un configSI
        /// </summary>
        /// <param name="configSi"></param>
        /// <returns></returns>
        public ConfigSi SetConfigSi(ConfigSi configSi)
        {
            try
            {
                Logger.Debug("ConfigSi id={0}", configSi.Id);
                var repository = new Repository<ConfigSi>();
                configSi.DateModification = DateTime.Now;
                var resultat = repository.Update(configSi);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return configSi;
        }

        /// <summary>
        /// Mis a jour d'un ConfigSIClient
        /// </summary>
        /// <param name="configSiClient">Le ConfigSIClient</param>
        /// <returns>Le ConfigSIClient</returns>
        public ConfigSiClient SetConfigSiClient(ConfigSiClient configSiClient)
        {
            try
            {
                Logger.Debug("ConfigSiId {0}, id client {1}", configSiClient.ConfigSiId, configSiClient.ClientId);
                var repository = new Repository<ConfigSiClient>();
                var existing = repository.Find(o => o.Id == configSiClient.Id);
                existing.ConfigSi= configSiClient.ConfigSi;
                configSiClient.DateModification = DateTime.Now;
                var resultat = repository.Update(existing);
                Logger.Debug("Nombre de ConfigSIClient mis a  jour {0}", resultat);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return configSiClient;
        }

        /// <summary>
        /// Recupere la liste des contact avec pagination
        /// </summary>
        /// <param name="page">La page</param>
        /// <param name="pageDimension">Le nombre de contact par page</param>
        /// <param name="totalPages">Le nombre total des pages</param>
        /// <returns></returns>
        public List<Contact> ListeContact(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPages)
        {
            totalPages = 0;
            try
            {
                
                Logger.Debug("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                var repository = new Repository<Contact>();

                var listeContact = repository.All().Include(o => o.Client)
                                             .Include(o => o.TypeContact)
                                             .Include(o => o.Reponses)
                                             .Include("Reponses.Question");
                                             
                long idContact = 0;
                var isId = Int64.TryParse(search, out idContact);
                if (!string.IsNullOrEmpty(search))
                    listeContact = listeContact.Where(o => (o.NomContact.ToLower().Contains(search.ToLower()) || 
                                        o.Id == idContact || o.PrenomContact.ToLower().Contains(search.ToLower()) ||
                                        o.Client.CodeInterneWattsGreen.ToLower().Contains(search.ToLower()) || 
                                        o.Fonction.ToLower().Contains(search.ToLower()) || 
                                        o.Email.ToLower().Contains(search.ToLower()) || 
                                        o.TypeContact.LibelleTypeContact.ToLower().Contains(search.ToLower()) ));

                if ((sortdir == null || sortdir.ToLowerInvariant() == "asc") && sort != null)
                {
                    if (sort.ToLowerInvariant() == "id") listeContact = listeContact.OrderBy(o => o.Id);
                    else if (sort.ToLowerInvariant() == "codeinternewattsgreen") listeContact = listeContact.OrderBy(o => o.Client.CodeInterneWattsGreen);
                    else if (sort.ToLowerInvariant() == "profil") listeContact = listeContact.OrderBy(o => o.TypeContact.LibelleTypeContact);
                    else if (sort.ToLowerInvariant() == "email") listeContact = listeContact.OrderBy(o => o.Email);
                    else if (sort.ToLowerInvariant() == "nomcontact") listeContact = listeContact.OrderBy(o => o.NomContact);
                    else if (sort.ToLowerInvariant() == "prenomcontact") listeContact = listeContact.OrderBy(o => o.PrenomContact);
                    else if (sort.ToLowerInvariant() == "fonction") listeContact = listeContact.OrderBy(o => o.Fonction);
                    else if (sort.ToLowerInvariant() == "datepremiereconnexion") listeContact = listeContact.OrderBy(o => o.DateDebut);
                    else listeContact = listeContact.OrderBy(o => o.Id);
                }
                else if (sortdir != null && sortdir.ToLowerInvariant() == "desc" && sort != null)
                {
                    if (sort.ToLowerInvariant() == "id") listeContact = listeContact.OrderByDescending(o => o.Id);
                    else if (sort.ToLowerInvariant() == "codeinternewattsgreen") listeContact = listeContact.OrderByDescending(o => o.Client.CodeInterneWattsGreen);
                    else if (sort.ToLowerInvariant() == "profil") listeContact = listeContact.OrderByDescending(o => o.TypeContact.LibelleTypeContact);
                    else if (sort.ToLowerInvariant() == "email") listeContact = listeContact.OrderByDescending(o => o.Email);
                    else if (sort.ToLowerInvariant() == "nomcontact") listeContact = listeContact.OrderByDescending(o => o.NomContact);
                    else if (sort.ToLowerInvariant() == "prenomcontact") listeContact = listeContact.OrderByDescending(o => o.PrenomContact);
                    else if (sort.ToLowerInvariant() == "fonction") listeContact = listeContact.OrderByDescending(o => o.Fonction);
                    else if (sort.ToLowerInvariant() == "datepremiereconnexion") listeContact = listeContact.OrderByDescending(o => o.DateDebut);
                    else listeContact = listeContact.OrderByDescending(o => o.Id);
                }
                else
                {
                    listeContact = listeContact.OrderBy(o => o.Id);
                }
                
                
                if (listeContact.Any())
                {
                    totalPages = listeContact.Count();
                    if (page.HasValue)
                    {
                        listeContact =
                            (IOrderedQueryable<Contact>)
                            listeContact.Skip((page.Value - 1)*pageDimension).Take(pageDimension);
                    }
                    else
                    {
                        listeContact = listeContact.Take(pageDimension) as IOrderedQueryable<Contact>;
                    }

                    return listeContact!=null?listeContact.ToList():null;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
           
            return null;
        }

        public List<Politique> GetPolitiques()
        {
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var repository = new Repository<Politique>();
                var listePolitique = repository.All()
                    .Select(t => new { Politique = t, PolitiqueTraductions = t.PolitiqueTraductions.Where(l => l.LangueId == langueId) }).AsEnumerable().Select(t => t.Politique).Where(p => p.DateFin == null)
                              .ToList();
                return listePolitique;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
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

        public List<Devise> GetListeDevise()
        {
            try
            {
                var langueId = 1;
                var repository = new Repository<Devise>();

                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var listeDevises = repository.All().Select(d => new { Devise = d, DeviseTraductions = d.DeviseTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.Devise).Where(p => p.DateFin == null).ToList();
                
                return listeDevises;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public Contact GetDetailsContact(int idContact)
        {
            try
            {
                var repository = new Repository<Contact>();
                var client = repository.Filter(c => c.Id == idContact)
                                       .Include(c => c.Client)
                                       .Include(c => c.Client.DetailsClient)
                                       .Include(c => c.Client.DetailsClient.Devise)
                                       .Single();
                return client;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Model.Models.Traduction.AgeDeviceTraduction GetAgeDeviceTraduction(int idAgeDevice, int idLangue)
        {
            try
            {
                var repository = new Repository<AgeDeviceTraduction>();
                var ageDeviceTraduction = repository.All().SingleOrDefault(o => (o.AgeDeviceId == idAgeDevice && o.LangueId==idLangue));
                return ageDeviceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public AgeDeviceTraduction SetAgeDeviceTraduction(AgeDeviceTraduction ageDeviceTraduction)
        {
            try
            {
                if (ageDeviceTraduction != null)
                {
                    Logger.Debug("AgeDeviceTraduction  agedevice {0}, langue {1}", ageDeviceTraduction.AgeDeviceId,
                                ageDeviceTraduction.LangueId);
                    var repository = new Repository<AgeDeviceTraduction>();
                    var existing = repository.Find(o => (o.AgeDeviceId == ageDeviceTraduction.AgeDeviceId && o.LangueId==ageDeviceTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = ageDeviceTraduction;
                        existing.DateDebut = DateTime.Now;
                         existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleAgeDevice != ageDeviceTraduction.LibelleAgeDevice;
                        existing.LibelleAgeDevice = ageDeviceTraduction.LibelleAgeDevice;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }                    
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return ageDeviceTraduction;
        }

        public Model.Models.Traduction.ClasseDeviceTraduction GetClasseDeviceTraduction(int idClasseDevice, int idLangue)
        {
            try
            {
                var repository = new Repository<ClasseDeviceTraduction>();
                var classeDeviceTraduction = repository.All().SingleOrDefault(o => (o.ClasseDeviceId == idClasseDevice && o.LangueId == idLangue));
                return classeDeviceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public ClasseDeviceTraduction SetClasseDeviceTraduction(ClasseDeviceTraduction classeDeviceTraduction)
        {
            try
            {
                if (classeDeviceTraduction != null)
                {
                    Logger.Debug("ClasseDeviceTraduction  classeDevice {0}, langue {1}", classeDeviceTraduction.ClasseDeviceId,
                                classeDeviceTraduction.LangueId);
                    var repository = new Repository<ClasseDeviceTraduction>();
                    var existing = repository.Find(o => (o.ClasseDeviceId == classeDeviceTraduction.ClasseDeviceId && o.LangueId == classeDeviceTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = classeDeviceTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleClasseDevice != classeDeviceTraduction.LibelleClasseDevice; 
                        existing.LibelleClasseDevice = classeDeviceTraduction.LibelleClasseDevice;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return classeDeviceTraduction;
        }

        public TypeUsageTraduction GetTypeUsageTraduction(int idTypeUsage, int idLangue)
        {
            try
            {
                var repository = new Repository<TypeUsageTraduction>();
                var ageDeviceTraduction = repository.All().SingleOrDefault(o => (o.TypeUsageId == idTypeUsage && o.LangueId == idLangue));
                return ageDeviceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public TypeUsageTraduction SetTypeUsageTraduction(TypeUsageTraduction typeUsageTraduction)
        {
            try
            {
                if (typeUsageTraduction != null)
                {
                    Logger.Debug("typeUsageTraduction  typeUsage {0}, langue {1}", typeUsageTraduction.TypeUsageId,
                                typeUsageTraduction.LangueId);
                    var repository = new Repository<TypeUsageTraduction>();
                    var existing = repository.Find(o => (o.TypeUsageId == typeUsageTraduction.TypeUsageId && o.LangueId == typeUsageTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = typeUsageTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleTypeUsage != typeUsageTraduction.LibelleTypeUsage;
                        existing.LibelleTypeUsage = typeUsageTraduction.LibelleTypeUsage;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return typeUsageTraduction;
        }

        public Model.Models.Traduction.CategorieDeviceTraduction GetCategorieDeviceTraduction(int idCategorieDevice, int idLangue)
        {
            try
            {
                var repository = new Repository<CategorieDeviceTraduction>();
                var categorieDeviceTraduction = repository.All().SingleOrDefault(o => (o.CategorieDeviceId == idCategorieDevice && o.LangueId == idLangue));
                return categorieDeviceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public CategorieDeviceTraduction SetCategorieDeviceTraduction(CategorieDeviceTraduction categorieDeviceTraduction)
        {
            try
            {
                if (categorieDeviceTraduction != null)
                {
                    Logger.Debug("CategorieDeviceTraduction  categorieDevice {0}, langue {1}", categorieDeviceTraduction.CategorieDeviceId,
                                categorieDeviceTraduction.LangueId);
                    var repository = new Repository<CategorieDeviceTraduction>();
                    var existing = repository.Find(o => (o.CategorieDeviceId == categorieDeviceTraduction.CategorieDeviceId && o.LangueId == categorieDeviceTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = categorieDeviceTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleCategorieDevice != categorieDeviceTraduction.LibelleCategorieDevice;
                        existing.LibelleCategorieDevice = categorieDeviceTraduction.LibelleCategorieDevice;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return categorieDeviceTraduction;
        }

        public Model.Models.Traduction.EquivalenceTraduction GetEquivalenceTraduction(int idClasseDevice, int idLangue)
        {
            try
            {
                var repository = new Repository<EquivalenceTraduction>();
                var equivalenceTraduction = repository.All().SingleOrDefault(o => (o.EquivalenceId == idClasseDevice && o.LangueId == idLangue));
                return equivalenceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public FamilleDeviceTraduction GetFamilleDeviceTraduction(int idFamilleDevice, int idLangue)
        {
            try
            {
                var repository = new Repository<FamilleDeviceTraduction>();
                var familleDeviceTraduction = repository.All().SingleOrDefault(o => (o.FamilleDeviceId == idFamilleDevice && o.LangueId == idLangue));
                return familleDeviceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public FamilleDeviceTraduction SetFamilleDeviceTraduction(FamilleDeviceTraduction familleDeviceTraduction)
        {
            try
            {
                if (familleDeviceTraduction != null)
                {
                    Logger.Debug("FamilleDeviceTraduction  familleDevice {0}, langue {1}", familleDeviceTraduction.FamilleDeviceId,
                                familleDeviceTraduction.LangueId);
                    var repository = new Repository<FamilleDeviceTraduction>();
                    var existing = repository.Find(o => (o.FamilleDeviceId == familleDeviceTraduction.FamilleDeviceId && o.LangueId == familleDeviceTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = familleDeviceTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleFamilleDevice != familleDeviceTraduction.LibelleFamilleDevice;
                        existing.LibelleFamilleDevice = familleDeviceTraduction.LibelleFamilleDevice;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return familleDeviceTraduction;
        }

        public Model.Models.Traduction.OrigineTraduction GetOrigineTraduction(int idOrigine, int idLangue)
        {
            try
            {
                var repository = new Repository<OrigineTraduction>();
                var origineTraduction = repository.All().SingleOrDefault(o => (o.OrigineId == idOrigine && o.LangueId == idLangue));
                return origineTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public OrigineTraduction SetOrigineTraduction(OrigineTraduction origineTraduction)
        {
            try
            {
                if (origineTraduction != null)
                {
                    Logger.Debug("OrigineTraduction  agedevice {0}, langue {1}", origineTraduction.OrigineId,
                                origineTraduction.LangueId);
                    var repository = new Repository<OrigineTraduction>();
                    var existing = repository.Find(o => (o.OrigineId == origineTraduction.OrigineId && o.LangueId == origineTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = origineTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleOrigine != origineTraduction.LibelleOrigine;
                        existing.LibelleOrigine = origineTraduction.LibelleOrigine;

                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return origineTraduction;
        }

        public Model.Models.Traduction.PaysTraduction GetPaysTraduction(int idPays, int idLangue)
        {
            try
            {
                var repository = new Repository<PaysTraduction>();
                var paysTraduction = repository.All().SingleOrDefault(o => (o.PaysId == idPays && o.LangueId == idLangue));
                return paysTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public PaysTraduction SetPaysTraduction(PaysTraduction paysTraduction)
        {
            try
            {
                if (paysTraduction != null)
                {
                    Logger.Debug("PaysTraduction  agedevice {0}, langue {1}", paysTraduction.PaysId,
                                paysTraduction.LangueId);
                    var repository = new Repository<PaysTraduction>();
                    var existing = repository.Find(o => (o.PaysId == paysTraduction.PaysId && o.LangueId == paysTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = paysTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibellePays != paysTraduction.LibellePays;
                        existing.LibellePays = paysTraduction.LibellePays;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return paysTraduction;
        }

        public Model.Models.Traduction.PolitiqueTraduction GetPolitiqueTraduction(int idPolitique, int idLangue)
        {
            try
            {
                var repository = new Repository<PolitiqueTraduction>();
                var politiqueTraduction = repository.All().SingleOrDefault(o => (o.PolitiqueId == idPolitique && o.LangueId == idLangue));
                return politiqueTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }


        public QuestionTraduction GetQuestionTraduction(int idQuestion, int idLangue)
        {
            try
            {
                var repository = new Repository<QuestionTraduction>();
                var questionTraduction = repository.All().SingleOrDefault(o => (o.QuestionId == idQuestion && o.LangueId == idLangue));
                return questionTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public QuestionTraduction SetQuestionTraduction(QuestionTraduction questionTraduction)
        {
            try
            {
                if (questionTraduction != null)
                {
                    Logger.Debug("QuestionTraduction  agedevice {0}, langue {1}", questionTraduction.QuestionId,
                                questionTraduction.LangueId);
                    var repository = new Repository<QuestionTraduction>();
                    var existing = repository.Find(o => (o.QuestionId == questionTraduction.QuestionId && o.LangueId == questionTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = questionTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleQuestion != questionTraduction.LibelleQuestion;
                        existing.LibelleQuestion = questionTraduction.LibelleQuestion;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return questionTraduction;
        }

        public Model.Models.Traduction.SecteurActiviteTraduction GetSecteurActiviteTraduction(int idSecteurActivite, int idLangue)
        {
            try
            {
                var repository = new Repository<SecteurActiviteTraduction>();
                var secteurActiviteTraduction = repository.All().SingleOrDefault(o => (o.SecteurActiviteId == idSecteurActivite && o.LangueId == idLangue));
                return secteurActiviteTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public SecteurActiviteTraduction SetSecteurActiviteTraduction(SecteurActiviteTraduction secteurActiviteTraduction)
        {
            try
            {
                if (secteurActiviteTraduction != null)
                {
                    Logger.Debug("SecteurActiviteTraduction  agedevice {0}, langue {1}", secteurActiviteTraduction.SecteurActiviteId,
                                secteurActiviteTraduction.LangueId);
                    var repository = new Repository<SecteurActiviteTraduction>();
                    var existing = repository.Find(o => (o.SecteurActiviteId == secteurActiviteTraduction.SecteurActiviteId && o.LangueId == secteurActiviteTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = secteurActiviteTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleSecteurActivite != secteurActiviteTraduction.LibelleSecteurActivite;
                        existing.LibelleSecteurActivite = secteurActiviteTraduction.LibelleSecteurActivite;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return secteurActiviteTraduction;
        }

        public Model.Models.Traduction.TarifTraduction GetTarifTraduction(int idTarif, int idLangue)
        {
            try
            {
                var repository = new Repository<TarifTraduction>();
                var tarifTraduction = repository.All().SingleOrDefault(o => (o.TarifId == idTarif && o.LangueId == idLangue));
                return tarifTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public TarifTraduction SetTarifTraduction(TarifTraduction tarifTraduction)
        {
            try
            {
                if (tarifTraduction != null)
                {
                    Logger.Debug("TarifTraduction  agedevice {0}, langue {1}", tarifTraduction.TarifId,
                                tarifTraduction.LangueId);
                    var repository = new Repository<TarifTraduction>();
                    var existing = repository.Find(o => (o.TarifId == tarifTraduction.TarifId && o.LangueId == tarifTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = tarifTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleTarif != tarifTraduction.LibelleTarif;
                        existing.LibelleTarif = tarifTraduction.LibelleTarif;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return tarifTraduction;
        }

        public TypeDeviceTraduction GetTypeDeviceTraduction(int idTypeDevice, int idLangue)
        {
            try
            {
                var repository = new Repository<TypeDeviceTraduction>();
                var typeDeviceTraduction = repository.All().SingleOrDefault(o => (o.TypeDeviceId == idTypeDevice && o.LangueId == idLangue));
                return typeDeviceTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public TypeDeviceTraduction SetTypeDeviceTraduction(TypeDeviceTraduction typeDeviceTraduction)
        {
            try
            {
                if (typeDeviceTraduction != null)
                {
                    Logger.Debug("TypeDeviceTraduction  agedevice {0}, langue {1}", typeDeviceTraduction.TypeDeviceId,
                                typeDeviceTraduction.LangueId);
                    var repository = new Repository<TypeDeviceTraduction>();
                    var existing = repository.Find(o => (o.TypeDeviceId == typeDeviceTraduction.TypeDeviceId && o.LangueId == typeDeviceTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = typeDeviceTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleTypeDevice != typeDeviceTraduction.LibelleTypeDevice;
                        existing.LibelleTypeDevice = typeDeviceTraduction.LibelleTypeDevice;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return typeDeviceTraduction;
        }

        public TypeRatioTraduction GetTypeRatioTraduction(int idTypeRatio, int idLangue)
        {
            try
            {
                var repository = new Repository<TypeRatioTraduction>();
                var typeRatioTraduction = repository.All().SingleOrDefault(o => (o.TypeRatioId == idTypeRatio && o.LangueId == idLangue));
                return typeRatioTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public TypeRatioTraduction SetTypeRatioTraduction(TypeRatioTraduction typeRatioTraduction)
        {
            try
            {
                if (typeRatioTraduction != null)
                {
                    Logger.Debug("TypeRatioTraduction  agedevice {0}, langue {1}", typeRatioTraduction.TypeRatioId,
                                typeRatioTraduction.LangueId);
                    var repository = new Repository<TypeRatioTraduction>();
                    var existing = repository.Find(o => (o.TypeRatioId == typeRatioTraduction.TypeRatioId && o.LangueId == typeRatioTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = typeRatioTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.LibelleTypeRatio != typeRatioTraduction.LibelleTypeRatio;
                        existing.LibelleTypeRatio = typeRatioTraduction.LibelleTypeRatio;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return typeRatioTraduction;
        }
   

        /// <summary>
        /// Recupere la liste de devises
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageDimension"></param>
        /// <param name="sort"></param>
        /// <param name="sortdir"></param>
        /// <param name="totalPage"></param>
        /// <returns>la liste de devises</returns>
        public List<Devise> ListeDevise(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                Logger.Debug("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                var repository = new Repository<Devise>();
                var listeDevise = repository.All().Where(o => o.DateFin == null);
                if ((sortdir == null || sortdir.ToLowerInvariant() == "asc") && sort != null)
                {
                    if (sort.ToLowerInvariant() == "libelle") listeDevise = listeDevise.OrderBy(o => o.Libelle);
                    else if (sort.ToLowerInvariant() == "datedebut") listeDevise = listeDevise.OrderBy(o => o.DateDebut);
                }
                else if (sortdir != null && sortdir.ToLowerInvariant() == "desc" && sort != null)
                {
                    if (sort.ToLowerInvariant() == "libelle") listeDevise = listeDevise.OrderByDescending(o => o.Libelle);
                    else if (sort.ToLowerInvariant() == "datedebut") listeDevise = listeDevise.OrderByDescending(o => o.DateDebut);
                }
                else
                {
                    listeDevise = listeDevise.OrderBy(o => o.Id);
                }

                if (listeDevise.Any())
                    totalPage = listeDevise.Count();

                if (page.HasValue)
                {
                    listeDevise = listeDevise.Skip((page.Value - 1) * pageDimension).Take(pageDimension);
                }
                else
                {
                    listeDevise = listeDevise.Skip(0).Take(pageDimension);
                }

                if (listeDevise.Any())
                {
                    var rapportListe = listeDevise.ToList();
                    return rapportListe;
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }


        /// <summary>
        /// Recupere le dernier devise
        /// </summary>
        /// <returns>Le dernier devise</returns>
        public Devise GetDernierDevise()
        {
            try
            {
                var repository = new Repository<Devise>();

                var devises = repository.All().ToList();
                return devises.Last();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }
        
        /// <summary>
        /// Sauvegarde devise
        /// </summary>
        /// <param name="equivalence">Devise a mettre a jour</param>
        /// <returns>Devise mis a jour</returns>
        public Devise SauvegardeDevise(Devise devise)
        {
            try
            {
                Logger.Debug("Id {0}, Libelle {1}", devise.Id, devise.Libelle);
                var repository = new Repository<Devise>();
                devise = repository.Create(devise);
                _unitOfWork.SaveChanges();
                return devise;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// Recupere une devise
        /// </summary>
        /// <param name="id">L'identifiant de la devise</param>
        /// <returns>La devise</returns>
        public Devise GetDevise(int id)
        {
            var repository = new Repository<Devise>();
            Logger.Debug("devise id={0}", id);
            try
            {
                var devise = repository.Filter(c => c.Id == id);
                return devise.Any() ? devise.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Mis a jour d'une devise
        /// </summary>
        /// <param name="devise">La devise a mettre a jour</param>
        /// <returns>La devise mis a jour</returns>
        public Devise MAJDevise(Devise devise)
        {
            if (devise == null)
                return null;

            try
            {
                Logger.Debug("Id {0}, Libelle {1}", devise.Id, devise.Libelle);
                var repository = new Repository<Devise>();
                devise.DateModification = DateTime.Now;
                repository.Update(devise);
                _unitOfWork.SaveChanges();
                return devise;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }
        
        
        public Client GetClientParIdClient(int idClient)
        {
            try
            {
                var clientRepository = new Repository<Client>();
                var client = clientRepository.Filter(o => o.Id == idClient)
                                             .Include(o => o.Adresse)
                                             .Include(o => o.DetailsClient)
                                             .Include(o => o.Contacts);
                return client.Any() ? client.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public DetailsClient MAJDetailsClient(DetailsClient detailsClient)
        {
            if (detailsClient == null)
                return null;

            try
            {
                Logger.Debug("Id {0}", detailsClient.Id);
                var repository = new Repository<DetailsClient>();
                repository.Update(detailsClient);
                _unitOfWork.SaveChanges();
                return detailsClient;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Langue GetLangueParCulture(string culture)
        {
            var repository = new Repository<Langue>();
            Logger.Debug("culture={0}", culture);
            try
            {
                var langue = repository.Filter(o => o.Culture == culture);
                return langue.Any() ? langue.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        public FamilleDevice GetFamilleParId(int idFamille)
        {
            try
            {
                var familleRepository = new Repository<FamilleDevice>();
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var familleDevice = familleRepository.All().Select(f => new { FamilleDevice = f, FamilleDeviceTraduction = f.FamilleDeviceTraductions.Where(ft => ft.LangueId == langueId) }).AsEnumerable().Select(r => r.FamilleDevice).Where(c => c.Id == idFamille);

                return familleDevice.Any() ? familleDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public CategorieDevice GetCategorieParId(int idCategorie)
        {
            try
            {
                var categorieRepository = new Repository<CategorieDevice>();
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                var categorieDevice = categorieRepository.All().Select(c => new { CategorieDevice = c, CategorieDeviceTraduction = c.CategorieDeviceTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.CategorieDevice).Where(c => c.Id == idCategorie);

                return categorieDevice.Any() ? categorieDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Langue GetLangue()
        {
            try
            {
                var culture = System.Threading.Thread.CurrentThread;
                var langue = GetLangueByCode(culture.CurrentCulture.TwoLetterISOLanguageName);
                return langue;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
            }

            return null;
        }

        public DeviseTraduction SetDeviseTraduction(DeviseTraduction deviseTraduction)
        {
            try
            {
                if (deviseTraduction != null)
                {
                    Logger.Debug("deviseTraduction  DeviseId {0}, langue {1}", deviseTraduction.DeviseId,
                                deviseTraduction.LangueId);
                    var repository = new Repository<DeviseTraduction>();
                    var existing = repository.Find(o => (o.DeviseId == deviseTraduction.DeviseId && o.LangueId == deviseTraduction.LangueId));
                    if (existing == null)
                    {
                        existing = deviseTraduction;
                        existing.DateDebut = DateTime.Now;
                        existing = repository.Create(existing);
                    }
                    else
                    {
                        var isModified = existing.Libelle != deviseTraduction.Libelle;
                        existing.Libelle = deviseTraduction.Libelle;
                        if (isModified)
                            existing.DateModification = DateTime.Now;
                        var resultat = repository.Update(existing);
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return deviseTraduction;
        }

        public DeviseTraduction GetDeviseTraduction(int idDevise, int idLangue)
        {
            try
            {
                var repository = new Repository<DeviseTraduction>();
                var deviseTraduction = repository.All().SingleOrDefault(o => (o.DeviseId == idDevise && o.LangueId == idLangue));
                return deviseTraduction;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }


        public List<Devise> GetDevises(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<Devise>();
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().Select(c => new { Devise = c, ClasseeDeviceTraduction = c.DeviseTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.Devise).ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SecteurActivite> GetSecteurActivites(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<SecteurActivite>();
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().Select(c => new { SecteurActivite = c, ClasseeDeviceTraduction = c.SecteurActiviteTraductions.Where(ct => ct.LangueId == langueId) }).AsEnumerable().Select(r => r.SecteurActivite).ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public List<string> GetListeEmailPasValide(int clientId)
        {
            try
            {
                var repository = new Repository<Contact>();
                var contacts = repository.Filter(o => o.DateFin == null && o.Client.Statut != 2 && o.ClientId != clientId).ToList(); //Contact pas supprimé et client pas rejetté
                return contacts.Select(contact => contact.Email).ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Question GetQuestionParDefaut()
        {
            try
            {
                var langueId = 1;
                var langue = GetLangue();
                if (langue != null)
                    langueId = langue.Id;
                
                var repository = new Repository<Question>();

                var question = repository.All().Select( t => new { Question = t, QuestionTraductions = t.QuestionTraductions.Where(l => l.LangueId == langueId) })
                                               .AsEnumerable()
                                               .Select(t => t.Question)
                                               .ToList();

                if (question.Any())
                    return question.FirstOrDefault();
                else
                    return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Reponse SauvegardeReponse(Reponse reponse)
        {
            try
            {
                var repository = new Repository<Reponse>();
                repository.Create(reponse);
                _unitOfWork.SaveChanges();
                return reponse;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ConfigSiClient MAJConfigSiClient(int idConfigSiClient)
        {
            try
            {
                var configSiClientRepository = new Repository<ConfigSiClient>();
                var result = configSiClientRepository.Filter(o => o.Id == idConfigSiClient);

                if (result.Any())
                {
                    var configSiClient = result.Single();

                    var origine = GetOrigineParLibelle(OrigineEnum.Personnalisation.ToString());
                    configSiClient.OrigineId = origine.Id;

                    configSiClientRepository.Update(configSiClient);
                    _unitOfWork.SaveChanges();

                    return configSiClient;
                }
                return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public ClasseDevice GetClasseParId(int id)
        {
            var repository = new Repository<ClasseDevice>();
            Logger.Debug("id classe device {0}", id);
            try
            {
                var classeDevice = repository.Filter(o => o.Id == id);

                return classeDevice.Any() ? classeDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }


        public AgeDevice GetAgeParId(int id)
        {
            var repository = new Repository<AgeDevice>();
            Logger.Debug("id age device {0}", id);
            try
            {
                var ageDevice = repository.Filter(o => o.Id == id);

                return ageDevice.Any() ? ageDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public Usage GetUsageParTypeUsageId(int typeUsageId)
        {
            var repository = new Repository<Usage>();
            Logger.Debug("TypeUsageId {0}", typeUsageId);
            try
            {
                var usage = repository.Filter(o => o.TypeUsageId == typeUsageId);

                return usage.Any() ? usage.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public ConsoWattHeur GetConsoWattHeurParClasseAge(int idAge, int idClasse, int idTypeDevice)
        {
            var repository = new Repository<ConsoWattHeur>();
            try
            {
                var consoWattHeur = repository.Filter(o => o.AgeDeviceId == idAge && o.ClasseDeviceId == idClasse && o.TypeDeviceId == idTypeDevice);

                return consoWattHeur.Any() ? consoWattHeur.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public TypeUsage GetTypeUsage(int IdTypeUsage)
        {
            var repository = new Repository<TypeUsage>();
            try
            {
                var typeUsage = repository.Filter(o => o.Id == IdTypeUsage);

                return typeUsage.Any() ? typeUsage.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        //evolution import ECSChange
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<CorrespondanceSecteurActivite> ListeCorrespondanceSecteurActivite(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CorrespondanceSecteurActivite>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CorrespondanceSecteurActivite GetCorrespondanceSecteurActivite(int id)
        {
            var repository = new Repository<CorrespondanceSecteurActivite>();
            Logger.Debug("correspondanceSecteurActivite id={0}", id);
            try
            {
                var correspondanceSecteurActivite = repository.Filter(c => c.Id == id);
                return correspondanceSecteurActivite.Any() ? correspondanceSecteurActivite.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondanceSecteurActivite"></param>
        /// <returns></returns>
        public CorrespondanceSecteurActivite SetCorrespondanceSecteurActivite(CorrespondanceSecteurActivite correspondanceSecteurActivite)
        {
            try
            {
                var repository = new Repository<CorrespondanceSecteurActivite>();
                if (repository.All().Any(p => p.Id == (correspondanceSecteurActivite.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceSecteurActivite.Id));
                    var isModified = objetExistant.CodeActivite != correspondanceSecteurActivite.CodeActivite ||
                                     objetExistant.TypeSocieteId != correspondanceSecteurActivite.TypeSocieteId ||
                                     objetExistant.SecteurActiviteId != correspondanceSecteurActivite.SecteurActiviteId 
                                    ;

                    objetExistant.CodeActivite = correspondanceSecteurActivite.CodeActivite;
                    objetExistant.TypeSocieteId = correspondanceSecteurActivite.TypeSocieteId;
                    objetExistant.SecteurActiviteId = correspondanceSecteurActivite.SecteurActiviteId;                   
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    //if (langue.DateDebut == new DateTime())
                    //  langue.DateDebut = DateTime.Now;
                    repository.Create(correspondanceSecteurActivite);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return correspondanceSecteurActivite;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondanceSecteurActivite"></param>
        /// <returns></returns>
        public bool SupprimerCorrespondanceSecteurActivite(CorrespondanceSecteurActivite correspondanceSecteurActivite)
        {
            var repository = new Repository<CorrespondanceSecteurActivite>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (correspondanceSecteurActivite.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceSecteurActivite.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceSecteurActivite.Id, correspondanceSecteurActivite.TypeSocieteId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceSecteurActivite.Id, correspondanceSecteurActivite.TypeSocieteId);
                LogguerErreur(e);
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<CorrespondanceTypeDevice> ListeCorrespondanceTypeDevice(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CorrespondanceTypeDevice>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CorrespondanceTypeDevice GetCorrespondanceTypeDevice(int id)
        {
            var repository = new Repository<CorrespondanceTypeDevice>();
            Logger.Debug("CorrespondanceTypeDevice id={0}", id);
            try
            {
                var correspondanceTypeDevice = repository.Filter(c => c.Id == id);
                return correspondanceTypeDevice.Any() ? correspondanceTypeDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="correspondanceTypeDevice"></param>
       /// <returns></returns>
        public CorrespondanceTypeDevice SetCorrespondanceTypeDevice(CorrespondanceTypeDevice correspondanceTypeDevice)
        {
            try
            {
                var repository = new Repository<CorrespondanceTypeDevice>();
                if (repository.All().Any(p => p.Id == (correspondanceTypeDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceTypeDevice.Id));
                    var isModified = objetExistant.TypeDeviceId != correspondanceTypeDevice.TypeDeviceId ||
                                     objetExistant.TypeSocieteId != correspondanceTypeDevice.TypeSocieteId ||
                                     objetExistant.CodeCategorie != correspondanceTypeDevice.CodeCategorie ||
                                     objetExistant.CodeFamille != correspondanceTypeDevice.CodeFamille ||
                                     objetExistant.CodeSousFamille != correspondanceTypeDevice.CodeSousFamille 
                                    ;

                    objetExistant.TypeDeviceId = correspondanceTypeDevice.TypeDeviceId;
                    objetExistant.TypeSocieteId = correspondanceTypeDevice.TypeSocieteId;
                    objetExistant.CodeCategorie = correspondanceTypeDevice.CodeCategorie;
                    objetExistant.CodeFamille = correspondanceTypeDevice.CodeFamille;
                    objetExistant.CodeSousFamille = correspondanceTypeDevice.CodeSousFamille;
                    objetExistant.Caracteristique = correspondanceTypeDevice.Caracteristique;
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {                    
                    repository.Create(correspondanceTypeDevice);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return correspondanceTypeDevice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondanceTypeDevice"></param>
        /// <returns></returns>
        public bool SupprimerCorrespondanceTypeDevice(CorrespondanceTypeDevice correspondanceTypeDevice)
        {
            var repository = new Repository<CorrespondanceTypeDevice>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (correspondanceTypeDevice.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceTypeDevice.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceTypeDevice.Id, correspondanceTypeDevice.TypeSocieteId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceTypeDevice.Id, correspondanceTypeDevice.TypeSocieteId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<CorrespondanceProcesseur> ListeCorrespondanceProcesseur(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CorrespondanceProcesseur>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CorrespondanceProcesseur GetCorrespondanceProcesseur(int id)
        {
            var repository = new Repository<CorrespondanceProcesseur>();
            Logger.Debug("CorrespondanceProcesseur id={0}", id);
            try
            {
                var correspondanceProcesseur = repository.Filter(c => c.Id == id);
                return correspondanceProcesseur.Any() ? correspondanceProcesseur.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

       /// <summary>
       /// 
       /// </summary>
        /// <param name="correspondanceProcesseur"></param>
       /// <returns></returns>
        public CorrespondanceProcesseur SetCorrespondanceProcesseur(CorrespondanceProcesseur correspondanceProcesseur)
        {
            try
            {
                var repository = new Repository<CorrespondanceProcesseur>();
                if (repository.All().Any(p => p.Id == (correspondanceProcesseur.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceProcesseur.Id));
                    var isModified = objetExistant.CodeProcesseur != correspondanceProcesseur.CodeProcesseur ||
                                     objetExistant.TypeSocieteId != correspondanceProcesseur.TypeSocieteId ||
                                     objetExistant.ClasseDeviceId != correspondanceProcesseur.ClasseDeviceId  
                                    ;

                    objetExistant.CodeProcesseur = correspondanceProcesseur.CodeProcesseur;
                    objetExistant.TypeSocieteId = correspondanceProcesseur.TypeSocieteId;
                    objetExistant.ClasseDeviceId = correspondanceProcesseur.ClasseDeviceId;
                
                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(correspondanceProcesseur);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return correspondanceProcesseur;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondanceProcesseur"></param>
        /// <returns></returns>
        public bool SupprimerCorrespondanceProcesseur(CorrespondanceProcesseur correspondanceProcesseur)
        {
            var repository = new Repository<CorrespondanceProcesseur>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (correspondanceProcesseur.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceProcesseur.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceProcesseur.Id, correspondanceProcesseur.TypeSocieteId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceProcesseur.Id, correspondanceProcesseur.TypeSocieteId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<CorrespondanceVitesse> ListeCorrespondanceVitesse(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CorrespondanceVitesse>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CorrespondanceVitesse GetCorrespondanceVitesse(int id)
        {
            var repository = new Repository<CorrespondanceVitesse>();
            Logger.Debug("CorrespondanceVitesse id={0}", id);
            try
            {
                var correspondanceImprimante = repository.Filter(c => c.Id == id);
                return correspondanceImprimante.Any() ? correspondanceImprimante.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

       /// <summary>
       /// 
       /// </summary>
        /// <param name="correspondanceVitesse"></param>
       /// <returns></returns>
        public CorrespondanceVitesse SetCorrespondanceVitesse(CorrespondanceVitesse correspondanceVitesse)
        {
            try
            {
                var repository = new Repository<CorrespondanceVitesse>();
                if (repository.All().Any(p => p.Id == (correspondanceVitesse.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceVitesse.Id));
                    var isModified = objetExistant.Vitesse1 != correspondanceVitesse.Vitesse1 ||
                                    objetExistant.Vitesse2 != correspondanceVitesse.Vitesse2 ||
                                    objetExistant.Vitesse3 != correspondanceVitesse.Vitesse3 ||
                                    objetExistant.TypeDeviceId != correspondanceVitesse.TypeDeviceId ||
                                     objetExistant.TypeSocieteId != correspondanceVitesse.TypeSocieteId  
                                    ;

                    objetExistant.Vitesse1 = correspondanceVitesse.Vitesse1;
                    objetExistant.Vitesse2 = correspondanceVitesse.Vitesse2;
                    objetExistant.Vitesse3 = correspondanceVitesse.Vitesse3;
                    objetExistant.TypeDeviceId = correspondanceVitesse.TypeDeviceId; 
                    objetExistant.TypeSocieteId = correspondanceVitesse.TypeSocieteId;
                    objetExistant.Vitesse1ClasseDeviceId = correspondanceVitesse.Vitesse1ClasseDeviceId;
                    objetExistant.Vitesse2ClasseDeviceId = correspondanceVitesse.Vitesse2ClasseDeviceId;
                    objetExistant.Vitesse3ClasseDeviceId = correspondanceVitesse.Vitesse3ClasseDeviceId;
                
                    //if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(correspondanceVitesse);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return correspondanceVitesse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondanceVitesse"></param>
        /// <returns></returns>
        public bool SupprimerCorrespondanceVitesse(CorrespondanceVitesse correspondanceVitesse)
        {
            var repository = new Repository<CorrespondanceVitesse>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (correspondanceVitesse.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceVitesse.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceVitesse.Id, correspondanceVitesse.TypeSocieteId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceVitesse.Id, correspondanceVitesse.TypeSocieteId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<CorrespondanceTaille> ListeCorrespondanceTaille(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CorrespondanceTaille>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CorrespondanceTaille GetCorrespondanceTaille(int id)
        {
            var repository = new Repository<CorrespondanceTaille>();
            Logger.Debug("CorrespondanceTaille id={0}", id);
            try
            {
                var correspondanceEcran = repository.Filter(c => c.Id == id);
                return correspondanceEcran.Any() ? correspondanceEcran.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

       /// <summary>
       /// 
       /// </summary>
        /// <param name="correspondanceTaille"></param>
       /// <returns></returns>
        public CorrespondanceTaille SetCorrespondanceTaille(CorrespondanceTaille correspondanceTaille)
        {
            try
            {
                var repository = new Repository<CorrespondanceTaille>();
                if (repository.All().Any(p => p.Id == (correspondanceTaille.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceTaille.Id));
                    var isModified = objetExistant.Taille1 != correspondanceTaille.Taille1 ||
                                    objetExistant.Taille2 != correspondanceTaille.Taille2 ||
                                    objetExistant.Taille3 != correspondanceTaille.Taille3 ||
                                    objetExistant.TypeDeviceId != correspondanceTaille.TypeDeviceId ||
                                     objetExistant.TypeSocieteId != correspondanceTaille.TypeSocieteId ||
                                     objetExistant.Taille1ClasseDeviceId != correspondanceTaille.Taille1ClasseDeviceId || 
                                     objetExistant.Taille2ClasseDeviceId != correspondanceTaille.Taille2ClasseDeviceId ||
                                     objetExistant.Taille3ClasseDeviceId != correspondanceTaille.Taille3ClasseDeviceId 
                                    ;

                    objetExistant.Taille1 = correspondanceTaille.Taille1;
                    objetExistant.Taille2 = correspondanceTaille.Taille2;
                    objetExistant.Taille3 = correspondanceTaille.Taille3;
                    objetExistant.TypeDeviceId = correspondanceTaille.TypeDeviceId;
                    objetExistant.TypeSocieteId = correspondanceTaille.TypeSocieteId;
                    objetExistant.Taille1ClasseDeviceId = correspondanceTaille.Taille1ClasseDeviceId;
                    objetExistant.Taille2ClasseDeviceId = correspondanceTaille.Taille2ClasseDeviceId;
                    objetExistant.Taille3ClasseDeviceId = correspondanceTaille.Taille3ClasseDeviceId;
                
                    if (isModified)
                    //if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(correspondanceTaille);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return correspondanceTaille;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondanceTaille"></param>
        /// <returns></returns>
        public bool SupprimerCorrespondanceTaille(CorrespondanceTaille correspondanceTaille)
        {
            var repository = new Repository<CorrespondanceTaille>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (correspondanceTaille.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondanceTaille.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceTaille.Id, correspondanceTaille.TypeSocieteId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, type societe={1}", correspondanceTaille.Id, correspondanceTaille.TypeSocieteId);
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageDimension"></param>
        /// <param name="totalPages"></param>
        /// <returns></returns>
        public List<CorrespondancePuissance> ListeCorrespondancePuissance(int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                var repository = new Repository<CorrespondancePuissance>();

                totalPages = repository.All().Count();
                if (currentPage >= 0 && pageDimension > 0)
                {
                    totalPages = (int)Math.Ceiling((double)totalPages / pageDimension);
                    return repository.All().OrderBy(o => o.Id).Skip(currentPage * pageDimension).Take(pageDimension).ToList();
                }
                else
                    return repository.All().ToList();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CorrespondancePuissance GetCorrespondancePuissance(int id)
        {
            var repository = new Repository<CorrespondancePuissance>();
            Logger.Debug("CorrespondancePuissance id={0}", id);
            try
            {
                var correspondancePuissance = repository.Filter(c => c.Id == id);
                return correspondancePuissance.Any() ? correspondancePuissance.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondancePuissance"></param>
        /// <returns></returns>
        public CorrespondancePuissance SetCorrespondancePuissance(CorrespondancePuissance correspondancePuissance)
        {
            try
            {
                var repository = new Repository<CorrespondancePuissance>();
                if (repository.All().Any(p => p.Id == (correspondancePuissance.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondancePuissance.Id));
                    var isModified = objetExistant.Puissance1 != correspondancePuissance.Puissance1 ||
                                    objetExistant.Puissance2 != correspondancePuissance.Puissance2 ||
                                    objetExistant.Puissance3 != correspondancePuissance.Puissance3 ||
                                    objetExistant.TypeDeviceId != correspondancePuissance.TypeDeviceId ||
                                     objetExistant.TypeSocieteId != correspondancePuissance.TypeSocieteId ||
                                     objetExistant.Puissance1 != correspondancePuissance.Puissance1ClasseDeviceId ||
                                     objetExistant.Puissance2 != correspondancePuissance.Puissance2ClasseDeviceId ||
                                     objetExistant.Puissance3 != correspondancePuissance.Puissance3ClasseDeviceId
                                    ;

                    objetExistant.Puissance1 = correspondancePuissance.Puissance1;
                    objetExistant.Puissance2 = correspondancePuissance.Puissance2;
                    objetExistant.Puissance3 = correspondancePuissance.Puissance3;
                    objetExistant.TypeDeviceId = correspondancePuissance.TypeDeviceId;
                    objetExistant.TypeSocieteId = correspondancePuissance.TypeSocieteId;
                    objetExistant.Puissance1ClasseDeviceId = correspondancePuissance.Puissance1ClasseDeviceId;
                    objetExistant.Puissance2ClasseDeviceId = correspondancePuissance.Puissance2ClasseDeviceId;
                    objetExistant.Puissance3ClasseDeviceId = correspondancePuissance.Puissance3ClasseDeviceId;

                    if (isModified)
                        objetExistant.DateModification = DateTime.Now;

                    repository.Update(objetExistant);
                }
                else
                {
                    repository.Create(correspondancePuissance);
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return correspondancePuissance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="correspondancePuissance"></param>
        /// <returns></returns>
        public bool SupprimerCorrespondancePuissance(CorrespondancePuissance correspondancePuissance)
        {
            var repository = new Repository<CorrespondancePuissance>();
            try
            {
                var statut = 0;
                if (repository.All().Any(p => p.Id == (correspondancePuissance.Id)))
                {
                    var objetExistant = repository.All().First(p => p.Id == (correspondancePuissance.Id));
                    statut = repository.Delete(objetExistant);
                }
                return statut == 1;
            }
            catch (DataException dex)
            {
                Logger.Error("Id={0}, type societe={1}", correspondancePuissance.Id, correspondancePuissance.TypeSocieteId);
                LogguerErreur(dex);
                throw;
            }
            catch (Exception e)
            {
                Logger.Error("Id={0}, type societe={1}", correspondancePuissance.Id, correspondancePuissance.TypeSocieteId);
                LogguerErreur(e);
                throw;
            }
        }
        
        //fin evolution import ECSChange


        public Client GetCodeInterneClient(string codeInterneClient)
        {
            var detailsClientRepository = new Repository<DetailsClient>();
            var client = new Client();

            try
            {
                var detailsClient = detailsClientRepository.Filter(o => o.CodeInterneClient == codeInterneClient);
            
                if (detailsClient.Any())
                {
                    client = GetClientParIdDetailsClient(detailsClient.Single().Id);
                    return client;
                }

                return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }
        
        public Client GetClientParIdDetailsClient(int idDetailsClient)
        {
            var clientRepository = new Repository<Client>();
            try
            {
                var client = clientRepository.Filter(o => o.DetailsClientId == idDetailsClient)
                                             .Include(o => o.Adresse)
                                             .Include(o => o.DetailsClient);


                return client.Any() ? client.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }

        public SecteurActivite GetSecteurActiviteParId(int secteurActiviteId)
        {
            try
            {
                var secteurActiviteRepository = new Repository<SecteurActivite>();
                var secteurActivite = secteurActiviteRepository.Filter(s => s.Id == secteurActiviteId).AsNoTracking();
                return secteurActivite.Any() ? secteurActivite.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public CorrespondanceTypeDevice GetCorrespondanceTypeDevice(int typeSocieteId, string codeCategorie, string codeFamille, string codeSousFamille)
        {
            try
            {
                var correspondanceTypeDeviceRepository = new Repository<CorrespondanceTypeDevice>();
                var correspondanceTypeDevice =
                    correspondanceTypeDeviceRepository.Filter(c => c.TypeSocieteId == typeSocieteId && c.CodeCategorie == codeCategorie
                                                                && c.CodeFamille == codeFamille && c.CodeSousFamille == codeSousFamille);
                return correspondanceTypeDevice.Any() ? correspondanceTypeDevice.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public CorrespondanceProcesseur GetCorrespondanceProcesseur(int typeSocieteId, string codeProcesseur)
        {
            try
            {
                var correspondanceProcesseurRepository = new Repository<CorrespondanceProcesseur>();
                var correspondanceProcesseur = correspondanceProcesseurRepository.Filter(c => c.TypeSocieteId == typeSocieteId && c.CodeProcesseur == codeProcesseur);
                return correspondanceProcesseur.Any() ? correspondanceProcesseur.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public CorrespondanceVitesse GetCorrespondanceVitesse(int typeSocieteId, int typeDeviceId)
        {
            try
            {
                var correspondanceVitesseRepository = new Repository<CorrespondanceVitesse>();
                var correspondanceVitesse = correspondanceVitesseRepository.Filter(c => c.TypeSocieteId == typeSocieteId && c.TypeDeviceId == typeDeviceId);
                return correspondanceVitesse.Any() ? correspondanceVitesse.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public CorrespondanceTaille GetCorrespondanceTaille(int typeSocieteId, int typeDeviceId)
        {
            try
            {
                var correspondanceTailleRepository = new Repository<CorrespondanceTaille>();
                var correspondanceTaille = correspondanceTailleRepository.Filter(c => c.TypeSocieteId == typeSocieteId && c.TypeDeviceId == typeDeviceId);
                return correspondanceTaille.Any() ? correspondanceTaille.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public CorrespondancePuissance GetCorrespondancePuissance(int typeSocieteId, int typeDeviceId)
        {
            try
            {
                var correspondancePuissanceRepository = new Repository<CorrespondancePuissance>();
                var correspondancePuissance = correspondancePuissanceRepository.Filter(c => c.TypeSocieteId == typeSocieteId && c.TypeDeviceId == typeDeviceId);
                return correspondancePuissance.Any() ? correspondancePuissance.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public Contact GetContactClientParAuthorisation(int idClient, int authorisation)
        {
            try
            {
                var repository = new Repository<Contact>();
                var contact = repository.Filter(c => c.ClientId == idClient && c.Authorisations == authorisation);
                return contact.Any() ? contact.Single() : null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public Pays GetPaysParLibelle(string libelle, int langueId)
        {
            try
            {
                var repository = new Repository<PaysTraduction>();
                var paysTrad = repository.Filter(p => p.LibellePays.ToLower() == libelle && p.LangueId == langueId);

                var pays = new Pays();
                if (paysTrad.Any())
                {
                    pays = GetPaysById(paysTrad.Single().PaysId);
                    return pays;
                }
                return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public SecteurActivite GetSecteurActiviteParCode(string code)
        {
            try
            {
                var repository = new Repository<CorrespondanceSecteurActivite>();
                var correspondanceSecteurActivite = repository.Filter(c => c.CodeActivite == code);

                var secteurActivite = new SecteurActivite();
                if (correspondanceSecteurActivite.Any())
                {
                    secteurActivite = GetSecteurActiviteParId(correspondanceSecteurActivite.Single().SecteurActiviteId);
                    return secteurActivite;
                }
                return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public Devise GetDeviseParLibelle(string libelle, int langueId)
        {
            try
            {
                var repository = new Repository<DeviseTraduction>();
                var deviseTrad = repository.Filter(d => d.Libelle.ToLower() == libelle && d.LangueId == langueId);

                var devise = new Devise();
                if (deviseTrad.Any())
                {
                    devise = GetDevise(deviseTrad.Single().DeviseId);
                    return devise;
                }
                return null;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }


        public List<TypeDevice> GetTypeDevices()
        {
            try
            {
                var repository = new Repository<TypeDevice>();
                var typeDevices = repository.All().ToList();
                return typeDevices;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

    }
}

