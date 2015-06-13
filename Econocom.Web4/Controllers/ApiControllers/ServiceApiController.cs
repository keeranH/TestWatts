using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Helper;
using Econocom.Model;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Service;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using NLog;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class ServiceApiController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private BusinessService service;
        public ServiceApiController()
        {
            service = new BusinessService();
        }       

        public Boolean SauvegardeClient(Client client)
        {
            try
            {
                if (client != null)
                {
                    return service.SauvegardeClient(client);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return false;
        }
        
        public List<Pays> ListePays()
        {
            try
            {
                return service.GetPays();
            }
            catch(Exception)
            {
                throw;
            }
            return null;
        }
        
        public List<Langue> GetListeLangue()
        {
            try
            {
                var totalPages = 0;
                return service.ListeLangues(0, 0, out totalPages);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<ConsoWattHeur> ListeConsoWattHeur()
        {
            try
            {
                var totalPages = 0;
                return service.ListeConsoWattHeur(0, 0, out totalPages);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Contact GetContact(Contact contact)
        {
            try
            {
                if (contact.Email != null && contact.MotPasse != null)
                {
                    var contactExistant = service.GetContact(contact.Email, contact.MotPasse);
                    return contactExistant;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Client SetClient(Client client)
        {
            try
            {
                if (client == null)
                {
                    throw new Exception("SetClientErreur",new Exception("Objet client vide"));
                }
                else
                {
                    if (client.Adresse == null)
                        throw new Exception("SetClientErreur", new Exception("Adresse vide"));
                  
                    if (client.Contacts == null || client.Contacts.Count == 0)
                        throw new Exception("SetClientErreur", new Exception("Contacts vide"));

                    return service.SetClient(client);
                }
            }           
            catch (Exception e)
            {
                throw new Exception("SetClientErreur", new Exception("ErreurSauvegardeClient"));
            }
        }

        public List<ContenuModere> GetListeContenuPublier(string nomDePage, string cultureDeLangue)
        {
            try
            {
                return service.ListeContenuPublier(nomDePage, cultureDeLangue);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public ContenuModereViewModel GetContenuPublier(string lien, string nomDePage, string cultureDeLangue)
        {
            try
            {
                return service.GetContenuPublier(lien, nomDePage, cultureDeLangue);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public bool MajClient(Client client, String btnStatus)
        {
            try
            {
                if (client != null)
                {
                    if (btnStatus.Equals("Accepté"))
                        return service.ActualiserClient(client);
                    else
                        return service.ActualiserClient(client);//need to confirm what to do...no status field on client
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return false;
        }

        public List<Page> ListePage()
        {
            try
            {
                var totalPages = 0;
                return service.ListePage(0,0,out totalPages);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public ContenuModere MAJContenuModere(ContenuModere contenuModere)
        {
            try
            {
                return service.MAJContenuModere(contenuModere);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }
        
        public Client GetClient(int id)
        {
            try
            {
                return service.GetClientParId(id);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<Pays> GetListePays()
        {
            try
            {
                return service.GetPays();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<Section> GetListeSection()
        {
            try
            {
                var totalPages = 0;
                return service.ListeSection(0,0,out totalPages);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Contact GetContactParCodeVerification(string verificationCode)
        {
            try
            {
                return service.GetContactParVerificationCode(verificationCode);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<Question> GetListeQuestion()
        {
            try
            {
                return service.GetQuestions();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Question GetQuestion(int id)
        {
            try
            {
                return service.GetQuestionParId(id);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Contact MajContact(Contact contact)
        {
            try
            {
                return service.MAJContact(contact);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Contact VerifierCompte(RegisterViewModel enregistrementModel)
        {
            try
            {
                return service.VerificationCompte(enregistrementModel);
            }
            catch(Exception)
            {
                throw;
            }
            return null;
        }

        public bool SauvegardeSection(SectionViewModel sectionViewModel)
        {
            try
            {
                return service.SauvegardeSection(sectionViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool SauvegardePage(PageViewModel pageViewModel)
        {
            try
            {
                return service.SauvegardePage(pageViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public List<Modele> GetListeModele()
        {
            try
            {
                var totalPages = 0;
                return service.ListeModele(0,0,out totalPages);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<Client> GetClientsModerer()
        {
            try
            {
                return service.ClientsModerer();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Client GetClientParNom(string nom)
        {
            try
            {
                return service.GetClientParNom(nom);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<Contact> GetContactParNomClient(string nom)
        {
            try
            {
                return service.GetContactParNomClient(nom);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Econocom.Model.Models.Benchmark.Utilisateur GetAdministrateur()
        {
            try
            {
                return service.GetAdministrateur();
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
                return service.GetCommercial();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Client GetClientById(int clientId)
        {
            try
            {
                return service.GetClientById(clientId);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public bool MAJContact(ContactsViewModel contactsViewModel)
        {
            try
            {
                return service.MAJContact(contactsViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public List<Politique> GetListePolitiques()
        {
            try
            {
                return service.GetListePolitiques();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Politique GetPolitiqueParId(int politiqueId)
        {
            try
            {
                return service.GetPolitiqueParId(politiqueId);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Contact SetContact(Contact contact)
        {
            try
            {
                return service.SetContact(contact);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public TypeContact GetTypeContactParLibelle(string libelle)
        {
            try
            {
                return service.GetTypeContactParLibelle(libelle);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public CompteEvaluation SauvegardeEvaluation(CompteEvaluation compteEvaluation)
        {
            try
            {
                return service.SauvegardeEvaluation(compteEvaluation);
                
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<SecteurActivite> GetListeSecteurActivite()
        {
            try
            {
                return service.GetListeSecteurActivite();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<TypeObjet> GetListeTypeObjet()
        {
            try
            {
                return service.GetListeTypeObjet();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Langue GetLangue(int id)
        {
            try
            {
                return service.GetLangue(id);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Langue GetLangueByCode(String code)
        {
            try
            {
                return service.GetLangueByCode(code);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }


        public Page GetPage(int id)
        {
            return service.GetPage(id);
        }

        public Reponse GetReponse(int contactId)
        {
            try
            {
                return service.GetReponse(contactId);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Contact GetContactParEmail(string email)
        {
            try
            {
                return service.GetContactParEmail(email);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public List<Routage> ListeDesRoutages()
        {
            try
            {
                return service.ListeDesRoutages();
            }
            catch (Exception)
            {
                throw;
                return null;
            }
        }

        public Routage GetRoutageByLangueIdAndValeurActuelle(int langueId, string valeurActuelle)
        {
            try
            {
                return service. GetRoutageByLangueIdAndValeurActuelle(langueId, valeurActuelle);
            }
            catch (Exception)
            {
                throw;
                return null;
            }
            return null;
        }

        public List<Simulation> GetSimulations(int clientId)
        {
            try
            {
                return service.GetSimulations(clientId);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }


        public Contact UpdateContact(Contact contact)
        {
            try
            {
                return service.UpdateContact(contact);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Reponse SetReponse(Reponse reponse)
        {
            try
            {
                return service.SetReponse(reponse);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public ContenuMail GetContenuMail(string codeLangue, string typeMail)
        //{
        //    try
        //    {
        //        return service.GetContenuMail(codeLangue, typeMail);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return null;
        //}

        public ContenuMail GetContenuMail(int idTypeMail, int idLangue)
        {
            try
            {
                return service.GetContenuMail(idTypeMail, idLangue);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public TypeMail GetTypeMail(string libelleTypeMail)
        {
            try
            {
                return service.GetTypeMail(libelleTypeMail);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Pays GetPaysById(int paysId)
        {
            try
            {
                return service.GetPaysById(paysId);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Tarif GetTarifById(int tarifId)
        {
            try
            {
                return service.GetTarifById(tarifId);
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }



        public Reponse UpdateReponse(Reponse reponse)
        {
            try
            {
                return service.UpdateReponse(reponse);
            }
            catch (Exception e)
            {
                
                throw e;
            }
        }

        public Simulation SauvegardeSimulation(Simulation simulation)
        {
            try
            {
                return service.SauvegardeSimulation(simulation);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public ConfigSiClient GetDernierConfigSiPourClient(int idClient)
        {
            try
            {
                return service.GetDernierConfigSiPourClient(idClient);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Simulation CreerNouvelleSimulation(int idClient, int idContact, SimulationViewModel simulationViewModel)
        {
            try
            {
                var timeStart = ConsommationHelper.GetStartTime("CreerNouvelleSimulation  start");
                var simulation =  service.CreerNouvelleSimulation(idClient, idContact, simulationViewModel);
                var timeDiff = ConsommationHelper.GetTimeDifference(timeStart, " CreerNouvelleSimulation end");
                return simulation;
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
                return service.GetSimulationEnCours(idContact);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public bool GenererRapportSimulation(int idClient, int idContact)
        {
            try
            {
                return service.GenererRapportSimulation(idClient, idContact);
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        public List<Econocom.Model.Models.Benchmark.Rapport> GetRapportPersonnalisation(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPage)
        {
            try
            {
                return service.GetRapportPersonnalisation(id, search, page, sort, sortdir, pageDimension, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}, pageDimension={5}", id, search, page, sort, sortdir, pageDimension);
                throw e;
            }
        }

        public Econocom.Model.Models.Benchmark.Rapport GetRapport(int rapportId)
        {
            try
            {
                return service.GetRapport(rapportId);
            }
            catch (Exception e)
            {
                Logger.Error("id={0},", rapportId);
                throw e;
            }
        }
        public List<Econocom.Model.Models.Benchmark.Rapport> GetRapportSimulation(int? id, string search, int? page, string sort, string sortdir, int pageDimension, out int totalPage)
        {
            try
            {
                return service.GetRapportSimulation(id, search, page, sort, sortdir, pageDimension, out totalPage);
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}, pageDimension={5}", id, search, page, sort, sortdir, pageDimension);
                throw e;
            }
        }

        public Simulation GetSimulationParIdRapport(int idRapport)
        {
            try
            {
                return service.GetSimulationParIdRapport(idRapport);
            }
            catch (Exception e)
            {
                Logger.Error("id={0},", idRapport);
                throw e;
            }
        }

        public Simulation MAJSimulation(Simulation simulation)
        {
            try
            {
                return service.MAJSimulation(simulation);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public List<Devise> GetListeDevise()
        {
            try
            {
                return service.GetListeDevise();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public Contact GetDetailsContact(int idContact)
        {
            try
            {
                return service.GetDetailsContact(idContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public Client GetClientParId(int idClient)
        {
            try
            {
                return service.GetClientParId(idClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public Client GetClientParIdClient(int idClient)
        {
            try
            {
                return service.GetClientParIdClient(idClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public Client MAJClient(Client client)
        {
            try
            {
                return service.updateClient(client);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public Adresse MAJAdresse(Adresse adresse)
        {
            try
            {
                return service.updateAdress(adresse);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public DetailsClient MAJDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                return service.MAJDetailsClient(detailsClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public TypeContact GetTypeContactByString(String typeContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetTypeContactByString(typeContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public List<String> GetListeEmails()
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetListeEmails();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public Langue GetLangueParCulture(string culture)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetLangueParCulture(culture);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }


        public Question GetQuestionParDefaut()
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetQuestionParDefaut();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public Reponse SauvegardeReponse(Reponse reponse)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.SauvegardeReponse(reponse);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public bool MajResultat(ConfigSi configSi, decimal coefficientCo2)
        {
            try
            {
                var result = service.MAJResultat(configSi, coefficientCo2);
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public CalculateurViewModel MajConsommation(CalculateurViewModel calculateurViewModel)
        {
            try
            {
                calculateurViewModel = service.ReCalculerEtMAJConfigSiConsommation(calculateurViewModel);
            }
            catch (Exception)
            {
                throw;
            }
            return calculateurViewModel;
        }

        public bool MajConsommation(ConfigSi configSi, decimal coefficientCo2)
        {
            try
            {
                var result = service.MAJConsommation(configSi, coefficientCo2);             
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool MajSimulationClient(int idContact, decimal coefficientCo2)
        {
            try
            {
                var result = service.MAJSimulationClient(idContact, coefficientCo2);
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public bool MajConfigClient(int idClient, decimal coefficientCo2)
        {
            try
            {
                var result = service.MAJConfigClient(idClient, coefficientCo2);
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
    }
}
