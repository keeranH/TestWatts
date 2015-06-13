using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using CsvHelper;
using Econocom.Business.Service;
using Econocom.Model;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Traduction;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using Econocom.Model.ViewModel.Report;
using NLog;
using WebGrease.Css.Extensions;

namespace Econocom.Admin.Controllers.ServiceApi
{
    [System.Web.Http.Authorize]
    public class ServiceApiController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly BusinessService _service;
        public ServiceApiController()
        {
            _service = new BusinessService();
        }

        public List<Pays> GetPays()
        {
            try
            {
                return _service.GetPays();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomDePage"></param>
        /// <param name="cultureDeLangue"></param>
        /// <returns></returns>
        public List<ContenuModere> GetListeContenuPublier(string nomDePage, string cultureDeLangue)
        {
            try
            {
                return _service.ListeContenuPublier(nomDePage, cultureDeLangue);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lien"></param>
        /// <param name="nomDePage"></param>
        /// <param name="cultureDeLangue"></param>
        /// <returns></returns>
        public ContenuModereViewModel GetContenuPublier(string lien, string nomDePage, string cultureDeLangue)
        {
            try
            {
                return _service.GetContenuPublier(lien, nomDePage, cultureDeLangue);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Section> GetListeSection()
        {
            try
            {
                var totalPages = 0;
                return _service.ListeSection(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
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
                return _service.GetListeTypeObjet();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<dynamic> GetListeObjet(int id, int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                return _service.GetListeObjet(id, currentPage, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TypeRatio> GetListeTypeRatio()
        {
            try
            {
                var totalPages = 0;
                return _service.GetListeTypeRatio(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Langue> GetListeLangue()
        {
            try
            {
                var totalPages = 0;
                return _service.ListeLangues(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
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
                return _service.SauvegardeSection(sectionViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
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
                return _service.SauvegardePage(pageViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Modele> GetListeModele()
        {
            try
            {
                var totalPages = 0;
                return _service.ListeModele(0, 0, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
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
                return _service.GetClientParNom(nom);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
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
                return _service.GetContactParNomClient(nom);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
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
                return _service.MAJContact(contactsViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }


        public bool SetListeUtilisateur(ListeUtilisateurViewModel listeUtilisateurViewModel)
        {
            try
            {
                return _service.SetListeUtilisateur(listeUtilisateurViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
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
            try
            {
                return _service.GetTypeContactParLibelle(libelle);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Contact SetContact(Contact contact)
        {
            try
            {
                return _service.SetContact(contact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contenuModere"></param>
        /// <returns></returns>
        public ContenuModere MAJContenuModere(ContenuModere contenuModere)
        {
            try
            {
                return _service.MAJContenuModere(contenuModere);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Client GetClient(int id)
        {
            try
            {
                return _service.GetClientParId(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
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
                return _service.GetLangue(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
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
                return _service.GetPage(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }



        public Utilisateur GetUtilisateur(Utilisateur user)
        {
            try
            {
                return _service.GetUtilisateur(user);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Recupere un TypeObjet par identifiant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TypeObjet GetTypeObjet(int id)
        {
            try
            {
                return _service.GetTypeObjet(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
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
                return _service.GetTypeObjet(nom);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<dynamic> SauvegardeListe(Type type, List<object> list)
        {
            try
            {                                
                return _service.SauvegardeListe(type, list);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            } 
        }

        public List<dynamic> SupprimerListe(Type type, List<object> list)
        {
            try
            {
                return _service.SupprimerListe(type, list);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }
        
        public List<ContenuModere> GetListeContenuModere(int pageId)
        {
            try
            {
                return _service.GetListeContenuModere(pageId);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public bool SupprimerContenuModere(ContenuModere contenuModere)
        {
            try
            {
                return _service.SupprimerContenuModere(contenuModere);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public bool SupprimerPage(Page page)
        {
            try
            {
                return _service.SupprimerPage(page);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Section GetSection(int id)
        {
            try
            {
                return _service.GetSection(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }
        public ContenuMail GetContenuMail(int idTypeMail, int idLangue)
        {
            try
            {
                return _service.GetContenuMail(idTypeMail, idLangue);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<TypeMail> GetListeTypeMail()
        {
            try
            {
                return _service.GetListeTypeMail();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ContenuMail SetContenuMail(ContenuMail contenuMail)
        {
            try
            {
                return _service.SetContenuMail(contenuMail);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<Econocom.Model.ViewModel.ContactViewModel> GetListeContact()
        {
            try
            {
                return _service.GetListeContact();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }
        }



        public List<Section> GetChildren(int sectionId)
        {
            try
            {
                return _service.GetChildren(sectionId);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public bool SupprimerSection(Section section)
        {
            try
            {
                return _service.SupprimerSection(section);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Contact GetContactParEmail(string email)
        {
            try
            {
                return _service.GetContactParEmail(email);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public TypeMail GetTypeMail(string typeMail)
        {
            try
            {
                return _service.GetTypeMail(typeMail);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<Document> GetListDocument(int? page, int pageDimension, out int totalPages)
        {
            try
            {
                return _service.GetListeDocument(page, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }


        public bool SupprimerDocument(int id)
        {
            try
            {
                return _service.SupprimerDocument(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }


        public Document GetDocument(int docId)
        {
            try
            {
                return _service.GetDocument(docId);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public Langue GetLangueParIdPays(int idPays)
        {
            try
            {
                return _service.GetLangueParIdPays(idPays);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public Routage GetRoutageByLangueIdAndValeurActuelle(int langueId, string valeurActuelle)
        {
            try
            {
                return _service.GetRoutageByLangueIdAndValeurActuelle(langueId, valeurActuelle);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }            
        }

        public List<Equivalence> GetListeEquivalence(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                return _service.GetListeEquivalence(page, pageDimension, sort, sortdir, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                Logger.Error(e.StackTrace);
                return null;
            } 
        }

        public Equivalence GetEquivalence(int id)
        {            
            try
            {
                return _service.GetEquivalence(id);
            }
            catch (Exception e)
            {
                Logger.Error("equivalence id {0}", id);
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public Equivalence SetEquivalence(Equivalence equivalence)
        {
            try
            {
                return _service.SetEquivalence(equivalence);
            }
            catch (Exception e)
            {
                Logger.Error("equivalence libelle {0}", equivalence.LibelleEquivalence);
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public List<Politique> GetListePolitique(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                return _service.GetListePolitique(page, pageDimension, sort, sortdir, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public Politique GetPolitique(int id)
        {
            try
            {
                return _service.GetPolitique(id);
            }
            catch (Exception e)
            {
                Logger.Error("Politique id {0}", id);
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public Politique SetPolitique(Politique politique)
        {
            try
            {
                return _service.SetPolitique(politique);
            }
            catch (Exception e)
            {
                Logger.Error("Politique libelle {0}", politique.LibellePolitique);
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public List<Client> GetListeClientsSouscrits(DateTime dateDebut, DateTime dateFin, int? page, int pageDimension, out int totalPages)
        {
            try
            {
                return _service.GetListeClientsSouscrits(dateDebut, dateFin, page, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                totalPages = 0;
                return null;
            }
        }


        public ListeClientViewModel GetListeClients(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPage)
        {
            totalPage = 1;
            try
            {
                return _service.GetListeClients(id, search, page, sort, sortdir, pageDimension, out totalPage);
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
                return _service.GetRapportClient(clientId);
            }
            catch (Exception e)
            {
                Logger.Error("id={0},", clientId);
                throw e;
            }
        }

        public List<Client> GetListeSouscriptions(DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                return _service.GetListeSouscriptions(dateDebut, dateFin);
            }
            catch (Exception e)
            {
                Logger.Error("dateDebut={0}, dateFin={1} ", dateDebut, dateFin);
                throw e;
            }
        }

        public Politique SauvegardePolitique(Politique politique)
        {
            try
            {
                return _service.SauvegardePolitique(politique);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Politique GetDernierPolitique()
        {
            try
            {
                return _service.GetDernierPolitique();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Politique MAJPolitique(Politique politique)
        {
            try
            {
                return _service.MAJPolitique(politique);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Econocom.Model.Models.Benchmark.Utilisateur GetAdministrateur()
        {
            try
            {
                return _service.GetAdministrateur();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Econocom.Model.Models.Benchmark.Utilisateur GetCommercial()
        {
            try
            {
                return _service.GetCommercial();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Equivalence GetDernierEquivalence()
        {
            try
            {
                return _service.GetDernierEquivalence();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Equivalence SauvegardeEquivalence(Equivalence equivalence)
        {
            try
            {
                return _service.SauvegardeEquivalence(equivalence);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Equivalence MAJEquivalence(Equivalence equivalence)
        {
            try
            {
                return _service.MAJEquivalence(equivalence);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Adresse saveAdress(Adresse adresse)
        {
            try
            {
                return _service.saveAdress(adresse);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public DetailsClient saveDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                return _service.saveDetailsClient(detailsClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public bool CodeInterneExiste(string codeInterne)
        {
            try
            {
                return _service.CodeInterneExiste(codeInterne);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return false;
        }

        public Client saveClient(Client clientz)
        {
            try
            {
                return _service.SaveClient(clientz);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public TypeContact GetTypeContactByString(string typeContactLibelle)
        {
            try
            {
                return _service.GetTypeContactByString(typeContactLibelle);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public void SauvegardeContacte(Contact contact)
        {
            try
            {
                _service.SauvegardeContacte(contact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
        }

        public List<Tarif> GetListeTarif()
        {
            try
            {
                return _service.GetListOfTarifs();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public List<String> GetListeEmails()
        {
            try
            {
                return _service.GetListeEmails();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Pays GetPaysById(int paysId)
        {
            try
            {
                return _service.GetPaysById(paysId);
            }
            catch (Exception e)
            {
                Logger.Error("paysId={0} ", paysId);
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Tarif GetTarifById(int tarifId)
        {
            try
            {
                return _service.GetTarifById(tarifId);
            }
            catch (Exception e)
            {
                Logger.Error("tarifId={0} ", tarifId);
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Langue GetLangueByCode(string codeLangue)
        {
            try
            {
                return _service.GetLangueByCode(codeLangue);
            }
            catch (Exception e)
            {
                Logger.Error("codeLangue={0} ", codeLangue);
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public List<ContactViewModel> ListeContact(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPages)
        {
            try
            {
                return _service.ListeContact(id, search, page, sort, sortdir, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                totalPages = 0;
                return null;
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

        public List<AgeDevice> GetListeAgeDevice()
        {
            try
            {
                return _service.GetListeAgeDevice();
            }
            catch (Exception e)
            {
                LogguerErreur(e);               
                return null;
            }
        }

        public AgeDeviceTraduction GetAgeDeviceTraduction(int idAgeDevice, int idLangue)
        {
            try
            {
                return _service.GetAgeDeviceTraduction(idAgeDevice, idLangue);
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
                return _service.SetAgeDeviceTraduction(ageDeviceTraduction);
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
                return _service.GetListeTypeUsage();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public TypeUsageTraduction GetTypeUsageTraduction(int idTypeUsage, int idLangue)
        {
            try
            {
                return _service.GetTypeUsageTraduction(idTypeUsage, idLangue);
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
                return _service.SetTypeUsageTraduction(typeUsageTraduction);
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
                return _service.GetListeCategorieDevice();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public CategorieDeviceTraduction GetCategorieDeviceTraduction(int idCategorieDevice, int idLangue)
        {
            try
            {
                return _service.GetCategorieDeviceTraduction(idCategorieDevice, idLangue);
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
                return _service.SetCategorieDeviceTraduction(categorieDeviceTraduction);
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
                return _service.GetListeClasseDevice();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public ClasseDeviceTraduction GetClasseDeviceTraduction(int idClasseDevice, int idLangue)
        {
            try
            {
                return _service.GetClasseDeviceTraduction(idClasseDevice, idLangue);
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
                return _service.SetClasseDeviceTraduction(classeDeviceTraduction);
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
                return _service.GetListeEquivalence();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public EquivalenceTraduction GetEquivalenceTraduction(int idEquivalence, int idLangue)
        {
            try
            {
                return _service.GetEquivalenceTraduction(idEquivalence, idLangue);
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
                return _service.SetEquivalenceTraduction(equivalenceTraduction);
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
                return _service.GetListeFamilleDevice();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public FamilleDeviceTraduction GetFamilleDeviceTraduction(int idFamilleDevice, int idLangue)
        {
            try
            {
                return _service.GetFamilleDeviceTraduction(idFamilleDevice, idLangue);
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
                return _service.SetFamilleDeviceTraduction(familleDeviceTraduction);
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
                return _service.GetListeOrigine();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public OrigineTraduction GetOrigineTraduction(int idOrigine, int idLangue)
        {
            try
            {
                return _service.GetOrigineTraduction(idOrigine, idLangue);
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
                return _service.SetOrigineTraduction(origineTraduction);
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
                return _service.GetListePays();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public PaysTraduction GetPaysTraduction(int idPays, int idLangue)
        {
            try
            {
                return _service.GetPaysTraduction(idPays, idLangue);
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
                return _service.SetPaysTraduction(paysTraduction);
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
                return _service.GetListePolitique();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public PolitiqueTraduction GetPolitiqueTraduction(int idPolitique, int idLangue)
        {
            try
            {
                return _service.GetPolitiqueTraduction(idPolitique, idLangue);
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
                return _service.SetPolitiqueTraduction(politiqueTraduction);
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
                return _service.GetListeQuestion();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public QuestionTraduction GetQuestionTraduction(int idQuestion, int idLangue)
        {
            try
            {
                return _service.GetQuestionTraduction(idQuestion, idLangue);
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
                return _service.SetQuestionTraduction(questionTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<SecteurActivite> GetListeSecteurActivite()
        {
            try
            {
                return _service.GetListeSecteurActivite();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public SecteurActiviteTraduction GetSecteurActiviteTraduction(int idSecteurActivite, int idLangue)
        {
            try
            {
                return _service.GetSecteurActiviteTraduction(idSecteurActivite, idLangue);
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
                return _service.SetSecteurActiviteTraduction(secteurActiviteTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        //public List<Tarif> GetListeTarif()
        //{
        //    try
        //    {
        //        return _service.GetListeTarif();
        //    }
        //    catch (Exception e)
        //    {
        //        LogguerErreur(e);
        //        return null;
        //    }
        //}

        public TarifTraduction GetTarifTraduction(int idTarif, int idLangue)
        {
            try
            {
                return _service.GetTarifTraduction(idTarif, idLangue);
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
                return _service.SetTarifTraduction(tarifTraduction);
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
                return _service.GetListeTypeDevice();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public TypeDeviceTraduction GetTypeDeviceTraduction(int idTypeDevice, int idLangue)
        {
            try
            {
                return _service.GetTypeDeviceTraduction(idTypeDevice, idLangue);
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
                return _service.SetTypeDeviceTraduction(typeDeviceTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public TypeRatioTraduction GetTypeRatioTraduction(int idTypeRatio, int idLangue)
        {
            try
            {
                return _service.GetTypeRatioTraduction(idTypeRatio, idLangue);
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
                return _service.SetTypeRatioTraduction(typeRatioTraduction);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }
       

        public List<Devise> GetListeDevise(int? page, int pageDimension, string sort, string sortdir, out int totalPage)
        {
            totalPage = 1;
            try
            {
                return _service.ListeDevise(page, pageDimension, sort, sortdir, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("page {0}, pageDimension {1}, sort {2}, sortdir{3}", page, pageDimension, sort, sortdir);
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public Devise GetDernierDevise()
        {
            try
            {
                return _service.GetDernierDevise();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Devise SauvegardeDevise(Devise devise)
        {
            try
            {
                return _service.SauvegardeDevise(devise);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public Devise GetDevise(int id)
        {
            try
            {
                return _service.GetDevise(id);
            }
            catch (Exception e)
            {
                Logger.Error("devise id {0}", id);
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        public Devise MAJDevise(Devise devise)
        {
            try
            {
                return _service.MAJDevise(devise);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return null;
        }

        public DeviseTraduction SetDeviseTraduction(DeviseTraduction devise)
        {
            try
            {
                return _service.SetDeviseTraduction(devise);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        public List<Devise> GetListeDevise()
        {
            try
            {
                return _service.GetListeDevise();
            }
            catch (Exception e)
            {
                Logger.Error("GetListeDevise : ", e.StackTrace);
                throw e;
            }
        }

        public Model.Models.Traduction.DeviseTraduction GetDeviseTraduction(int idDeviseTraduction, int idLangue)
        {
            try
            {
                return _service.GetDeviseTraduction(idDeviseTraduction, idLangue);
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
                return _service.GetDevises();
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
                return _service.GetSecteurActivites();
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }
    }
    
}
