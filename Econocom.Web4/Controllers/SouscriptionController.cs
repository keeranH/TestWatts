using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Econocom.Data;
using Econocom.Model.Enum;
using Econocom.Model.ViewModel;
using Econocom.Web4.Controllers.ApiControllers;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Interfaces;
using NLog;
using Omu.ValueInjecter;
using Econocom.Helper.Email;
using Rotativa;
using System.Web.Security;

namespace Econocom.Web4.Controllers
{
    public class SouscriptionController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IUnitOfWork _unitOfWork = new EconocomContext();

        private ServiceApiController service;

        public String Test = "";

        public SouscriptionController()
         {
             this.InitLanguageDropDown();
            service = new ServiceApiController();
         }

        //
        // GET: /Souscription/

        public ActionResult Index()
        {
            ViewBag.subscribeType = "start";
           
            SouscriptionViewModel svm= new SouscriptionViewModel();
            SocieteSouscriptionViewModel societeModel=  new SocieteSouscriptionViewModel();
            ContactSouscriptionViewModel souscripteurModel = new ContactSouscriptionViewModel();

            svm.SocieteInputSouscriptionViewModel = societeModel;
            svm.SouscripteurInputSouscriptionViewModel = souscripteurModel;
            //svm.EstSouscripteurAdmin = "False";
            svm.EstSouscripteurAdmin = false;

            var listePays = service.ListePays();            
            societeModel.ListPays = listePays;
            
            return View(svm);
        }

        public ActionResult Index_two()
        {
            var apiTarif= new TarifApiController();
            
           var listeTarifs = apiTarif.GetListOfTarifs();


           return View("Index2", listeTarifs);
        }

        public ActionResult index_three()
        {
            Session.Remove("subscribeSession");
            Session.Remove("subscribeClientId");
            Session.Remove("tarifSession");
            Session.Remove("detailsClientId");
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Validation()
        {
            //SouscriptionViewModel svm = Recapitulatif();
            //return View(svm);
            var succes = CreerClient();
            if (succes)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index_two", "Souscription");
            }
           
        }
       
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        public ActionResult index_two(int? tarifId)
        {
            try
            {
                Session.Add("tarifSession", tarifId);               
            }
            catch (Exception e)
            {
                throw;
            }
            return View("Validation");
        }


        public ActionResult ConditionsGenerals()
        {
            base.InitierContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");

            var contenu = base.GetContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");
            return View("ConditionsGenerals", contenu);

            //var contenu = base.GetContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");
            //return new RazorPDF.PdfResult(contenu, "ConditionsGenerals");
        }

        public bool CreerClient()
        {
            Logger.Info("Sauvegarde d'un nouveau client: Debut");
            try
            {
                var souscriptionSession = (SouscriptionViewModel)Session["subscribeSession"];
                var tarifId = Session["tarifSession"];

                var clientModel = souscriptionSession.SocieteInputSouscriptionViewModel;
                var souscripteur = souscriptionSession.SouscripteurInputSouscriptionViewModel;
                var administrateur = souscriptionSession.AdministrationInputSouscriptionViewModel;
                var listeUtilisateurs = souscriptionSession.UtilisateurInputSouscriptionViewModel;

                var client = new Client();
                AdresseApiController ApiAdress = new AdresseApiController();
                               
                try
                {
                    ClientApiController ApiClient = new ClientApiController();

                    // SAVING ADRESSE
                    var adresse = new Adresse()
                    {
                        Adresse1 = clientModel.Addresse,
                        CodePostal = clientModel.CodePostal,
                        Ville = clientModel.Ville,
                        PaysId = (int)clientModel.PaysId

                    };

                    var pays = service.GetPaysById((int)clientModel.PaysId);
                   
                    var detailsClient = new DetailsClient()
                    {
                        ChiffreAffaire = clientModel.ChiffresAffaires,
                        TarifId = (int)tarifId,
                    };

                    if (pays != null && pays.CoefficientConversion.HasValue)
                        detailsClient.CoefficientCO2 = pays.CoefficientConversion.Value;

                    // Générer le code interne. On génére un autre code si un code est déjà associé à un client
                    var codeInterne = "";
                    do
                    {
                        codeInterne = Security.HashHelper.GenererCodeInterne();
                    } while (ApiClient.CodeInterneExiste(codeInterne));

                    ContactApiController contactApi = new ContactApiController();
                    TypeContact contact_souscripteur = contactApi.GetTypeContactByString(TypeContactEnum.Souscripteur.ToString());
                    TypeContact contact_administrateur = contactApi.GetTypeContactByString(TypeContactEnum.Administrateur.ToString());
                    TypeContact contact_utilisateur = contactApi.GetTypeContactByString(TypeContactEnum.Utilisateur.ToString());
                   
                    var nouveauClient = new Client
                    {
                        Adresse = adresse,
                        RaisonSociale = clientModel.RaisonSociale,
                        IdentificationNational = clientModel.IdentificationNational,
                        Groupe = clientModel.Groupe,
                        TVAIntraComm = clientModel.Tva,
                        ContactEconocom = clientModel.VotreContacte,
                        ClientEconocom = clientModel.ClientEconocom,
                        DetailsClient = detailsClient,
                        DateDebut = DateTime.Now,
                        CodeInterneWattsGreen = codeInterne
                    };

                    if (souscripteur.Email.Equals(administrateur.Email))
                    {
                        var contact = new Contact()
                        {
                            NomContact = souscripteur.Nom,
                            PrenomContact = souscripteur.Prenom,
                            NumeroPhone = souscripteur.Telephone,
                            Email = souscripteur.Email,
                            Fonction = souscripteur.Fonction,
                            TypeContactId = contact_souscripteur.Id,
                            CodeVerification = Guid.NewGuid().ToString(),
                            Authorisations = (int)TypeContactEnum.SouscripteurAdmin

                        };
                        nouveauClient.Contacts.Add(contact);
                    }
                    else
                    {
                        var contact = new Contact()
                            {
                                NomContact = souscripteur.Nom,
                                PrenomContact = souscripteur.Prenom,
                                NumeroPhone = souscripteur.Telephone,
                                Email = souscripteur.Email,
                                Fonction = souscripteur.Fonction,
                                TypeContactId = contact_souscripteur.Id,
                                CodeVerification = Guid.NewGuid().ToString(),
                                Authorisations = (int) TypeContactEnum.Souscripteur

                            };
                        nouveauClient.Contacts.Add(contact);

                        var admin = new Contact()
                        {
                            NomContact = administrateur.Nom,
                            PrenomContact = administrateur.Prenom,
                            NumeroPhone = administrateur.Telephone,
                            Email = administrateur.Email,
                            Fonction = administrateur.Fonction,
                            TypeContactId = contact_administrateur.Id,
                            CodeVerification = Guid.NewGuid().ToString(),
                            Authorisations = (int)TypeContactEnum.Administrateur
                        };
                        nouveauClient.Contacts.Add(admin);
                    }
                    
                   foreach (Utilisateurs t in listeUtilisateurs.ListeUtilisateurs)
                    {
                        if (t.Nom != null)
                        {
                            Contact utilisateur = new Contact()
                            {
                                NomContact = t.Nom,
                                PrenomContact = t.Prenom,
                                Email = t.Email,
                                TypeContactId = contact_utilisateur.Id,
                                CodeVerification = Guid.NewGuid().ToString(),
                                Authorisations = (int)TypeContactEnum.Utilisateur

                            };

                            nouveauClient.Contacts.Add(utilisateur);
                        }
                    }
                    
                    client = ApiClient.saveClient(nouveauClient);
                    Logger.Info("Fin sauvegarde d'un nouveau client {0} id {1}: OK", client.RaisonSociale, client.Id);

                    // Envoyer Email
                    bool email = EnvoyerEmailEnregistrement(souscriptionSession);
                    bool mailWebmaster = EnvoyerEmailSouscriptionAuWebmaster(souscripteur.Nom, clientModel.RaisonSociale);

                    Session.Add("subscribeClientId", client.Id);

                    return email;
                }
                catch (Exception e)
                {
                    Logger.Info("Fin sauvegarde d'un nouveau client {0}: KO", client.RaisonSociale);
                    LogguerErreur(e);                    
                }                              
            }
            catch (Exception e)
            {
                Logger.Error("Sauvegarde d'un nouveau client: Erreur " + e.StackTrace);
            }
            return false;
        }

        public ActionResult AcceptConditions()
        {
            try
            {
                var succes = CreerClient();
                if (succes)
                {
                    return RedirectToAction("Validation", "Souscription");
                }
                else
                {
                    return View("ConditionsGenerals");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return View("ConditionsGenerals");
            }

        }
        
         public ActionResult RefuserConditions()
         {

             Session.Remove("subscribeSession");
             Session.Remove("subscribeClientId");
             Session.Remove("tarifSession");
             Session.Remove("detailsClientId");

             return RedirectToAction("Index", "Home");

         }

         public ActionResult ConditionsGeneralsPartial()
         {
             try
             {
                 //base.InitierContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");
                 //String ggg = base.InitierContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");
                 //return new PartialViewAsPdf();

                 var contenu = base.GetContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");                
                 return new RazorPDF.PdfResult(contenu, "ConditionsGeneralsPartial");

                
             }
             catch (Exception e)
             {
                 throw e;
             }
         }


        private bool EnvoyerEmailEnregistrement(SouscriptionViewModel souscriptionViewModel)
        {
            var emailList = new List<string>();

            try
            {
                // Ajouter l'adresse mail du souscripteur
                emailList.Add(souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Email);

                var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                var langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);
                if (langueChoisi == null)
                {
                    langueChoisi = service.GetLangueByCode("fr");
                }

                // Recuperer le sujet et le contenu du mail
                var typeMail = service.GetTypeMail("Demande de souscription");
                var contenuMail = service.GetContenuMail(typeMail.Id, langueChoisi.Id);

                // Pays
                var pays = new Pays();
                if (souscriptionViewModel.SocieteInputSouscriptionViewModel.PaysId != null)
                {
                    pays = service.GetPaysById(souscriptionViewModel.SocieteInputSouscriptionViewModel.PaysId??0);
                    souscriptionViewModel.SocieteInputSouscriptionViewModel.Pays = pays;
                }

                // Tarif
                var tarifId = Session["tarifSession"];
                var tarif = service.GetTarifById(Convert.ToInt32(tarifId));
                tarif.LibelleTarif = Resource.Traduction.ResourceManager.GetString("Tarif" + tarif.Id) ?? tarif.LibelleTarif;
                
                // Envoyer Email
                return EmailHelper.SendRegistrationMailToContacts(emailList, contenuMail, souscriptionViewModel, tarif);
            }
            catch (Exception e) 
            {
                Logger.Error("Erreur lors de l'envoi de mail");
                return false;
            }
        }


        public bool EnvoyerEmailSouscriptionAuWebmaster(string souscripteur, string societe)
        {
            try
            {                
                var LangueChoisi = (Langue)Session["LangueChoisi"];
                if (LangueChoisi == null)
                {
                    LangueChoisi = service.GetLangueByCode("fr");
                }

                // Récupérer le sujet et le contenu du mail
                var typeMail = service.GetTypeMail("Alerte nouvelle souscription");
                var contenuMail = service.GetContenuMail(typeMail.Id, LangueChoisi.Id);

                // Récupérer l'adresse mail du webmaster
                var administrateur = service.GetAdministrateur();

                var webMasterEmail = "";
                if (administrateur != null)
                {
                    webMasterEmail = administrateur.Email;
                }

                // Envoyer Email
                return EmailHelper.EnvoyerMailSouscriptionAuWebmaster(webMasterEmail, contenuMail, souscripteur, societe);
            }
            catch (Exception e)
            {
                Logger.Error("Erreur lors de l'envoi de mail");
                return false;
            }
        }

        public SouscriptionViewModel Recapitulatif()
        {

            var sessionClientId = (int)Session["subscribeClientId"];
            var clientId = sessionClientId;

            var apiClient = new ClientApiController();
            var client = apiClient.GetClientById(clientId);


            var apiAdress = new AdresseApiController();
            var adress = apiAdress.GetAdresseById((int) client.AdresseId);

            var apiContact = new ContactApiController();

            TypeContact contact_souscripteur = apiContact.GetTypeContactByString("Souscripteur");
            TypeContact contact_administrateur = apiContact.GetTypeContactByString("Administrateur");
            TypeContact contact_utilisateur = apiContact.GetTypeContactByString("Utilisateur");

            var souscripteur = apiContact.GetContactByTypeAndClient(contact_souscripteur.Id, clientId);
            var administrateur = apiContact.GetContactByTypeAndClient(contact_administrateur.Id, clientId);
           
            List<Contact> listeUtilisateurs= new List<Contact>();
            listeUtilisateurs= apiContact.GetUtilisateursByContactAndTypeContact(clientId, contact_utilisateur.Id);

            var apiPays = new PaysApiController();
            Pays pays = apiPays.getPaysById(adress.PaysId);
            
            SouscriptionViewModel svm = new SouscriptionViewModel();
            SocieteSouscriptionViewModel societeSouscription= new SocieteSouscriptionViewModel();

            societeSouscription.ClientId = client.Id;
            societeSouscription.RaisonSociale = client.RaisonSociale;
            societeSouscription.Groupe = client.Groupe;
            societeSouscription.Tva = client.TVAIntraComm;
            societeSouscription.VotreContacte = client.ContactEconocom;
            societeSouscription.IdentificationNational = client.IdentificationNational;
            societeSouscription.ClientEconocom = client.ClientEconocom;
            societeSouscription.Addresse = adress.Adresse1;
            societeSouscription.Ville = adress.Ville;
            societeSouscription.CodePostal = adress.CodePostal;

            var listePays = service.ListePays();
            foreach (var p in listePays)
            {
                p.LibellePays = Resource.Traduction.ResourceManager.GetString("Pays" + p.Id) ?? p.LibellePays;
            }
            societeSouscription.ListPays = listePays;

            societeSouscription.PaysId = adress.PaysId;
            societeSouscription.LibellePays = Resource.Traduction.ResourceManager.GetString("Pays" + pays.Id) ?? pays.LibellePays;
            if (client.ClientEconocom == false)
            {
                societeSouscription.LibelleClientEconocom = Resource.Traduction.Non;
            }
            else
            {
                societeSouscription.LibelleClientEconocom = Resource.Traduction.Oui;
            }
            
            svm.SocieteSouscriptionViewModel = societeSouscription;

            ContactSouscriptionViewModel contactSouscription = new ContactSouscriptionViewModel();
            contactSouscription.Nom = souscripteur.NomContact;
            contactSouscription.Prenom = souscripteur.PrenomContact;
            contactSouscription.Email = souscripteur.Email;
            contactSouscription.Fonction = souscripteur.Fonction;
            contactSouscription.Telephone = souscripteur.NumeroPhone;
            contactSouscription.ClientId = souscripteur.ClientId;
            svm.SouscripteurSouscriptionViewModel = contactSouscription;

            ContactSouscriptionViewModel adminSouscription = new ContactSouscriptionViewModel();
            adminSouscription.Nom = administrateur.NomContact;
            adminSouscription.Prenom = administrateur.PrenomContact;
            adminSouscription.Email = administrateur.Email;
            adminSouscription.Fonction = administrateur.Fonction;
            adminSouscription.Telephone = administrateur.NumeroPhone;
            adminSouscription.ClientId = administrateur.ClientId;
            svm.AdministrationSouscriptionViewModel = adminSouscription;
            
            List<Utilisateurs> maList = new List<Utilisateurs>();

            for (int i = 0; i < listeUtilisateurs.Count; i++)
            {
                Utilisateurs utilisateurSouscription = new Utilisateurs();

                utilisateurSouscription.Nom = listeUtilisateurs[i].NomContact;
                utilisateurSouscription.Prenom = listeUtilisateurs[i].PrenomContact;
                utilisateurSouscription.Email = listeUtilisateurs[i].Email;
                utilisateurSouscription.ClientId = listeUtilisateurs[i].ClientId;
                utilisateurSouscription.id = listeUtilisateurs[i].Id;
                maList.Add(utilisateurSouscription);
            }

            for (int k = listeUtilisateurs.Count; k < 5; k++)
            {
                Utilisateurs utilisateurSouscription = new Utilisateurs();
                maList.Add(utilisateurSouscription);
            }
            
            UtilisateurSouscriptionViewModel ecranUtilisateurs = new UtilisateurSouscriptionViewModel();
            ecranUtilisateurs.ListeUtilisateurs = maList;
            svm.UtilisateurSouscriptionViewModel = ecranUtilisateurs;
            
            return svm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SouscriptionInput(SouscriptionViewModel svm)
        {
            try
            {
                if (svm.SocieteInputSouscriptionViewModel.PaysId == null)
                {
                    ModelState.AddModelError("SocieteInputSouscriptionViewModel.PaysId", "ErrorRequired");
                }

                string expression = @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$";
                //@"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" + @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" + @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
                var listeUtilisateurs = svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs;
                for (int i = 0; i < listeUtilisateurs.Count; i++)
                {
                    if ((listeUtilisateurs[i].Nom == null || listeUtilisateurs[i].Nom.Equals("")) &&
                        (listeUtilisateurs[i].Prenom == null || listeUtilisateurs[i].Prenom.Equals("")) &&
                        (listeUtilisateurs[i].Email == null || listeUtilisateurs[i].Email.Equals("")))
                    {
                        
                    }
                    else
                    {
                        if (listeUtilisateurs[i].Email != null && !listeUtilisateurs[i].Email.Equals(""))
                        {
                            Match match = Regex.Match(listeUtilisateurs[i].Email, expression, RegexOptions.IgnoreCase);
                            if (!match.Success)
                                ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorMail");
                        }
                        if (listeUtilisateurs[i].Nom == null || listeUtilisateurs[i].Nom.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Nom", "ErrorRequired");
                        }
                        if (listeUtilisateurs[i].Prenom == null || listeUtilisateurs[i].Prenom.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Prenom", "ErrorRequired");
                        }
                        if (listeUtilisateurs[i].Email == null || listeUtilisateurs[i].Email.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorRequired");
                        }
                    }
                }

                var listeEmailsSaisies = new List<String>();

                if (svm.SouscripteurInputSouscriptionViewModel.Email != null)
                {
                    listeEmailsSaisies.Add(svm.SouscripteurInputSouscriptionViewModel.Email);
                }

                if (svm.AdministrationInputSouscriptionViewModel.Email != null)
                {
                    listeEmailsSaisies.Add(svm.AdministrationInputSouscriptionViewModel.Email);
                }

                foreach (var utilisateur in svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs)
                {
                    if (utilisateur.Email != null)
                    {
                        listeEmailsSaisies.Add(utilisateur.Email);
                    }
                }

                ContactApiController contactApi = new ContactApiController();
                var listeEmailsExistants = contactApi.GetListeEmails();


                if (listeEmailsSaisies != null && listeEmailsSaisies.Count > 0)
                {
                    //verifier si mail souscripteur est valide
                    var listeMail1 = listeEmailsSaisies.FindAll(o => o.Equals(svm.SouscripteurInputSouscriptionViewModel.Email));
                    var listeMail2 = listeEmailsExistants.FindAll(o => o.Equals(svm.SouscripteurInputSouscriptionViewModel.Email));

                    if (listeMail1.Count() > 1 && !svm.EstSouscripteurAdmin)
                    {
                        ModelState.AddModelError("SouscripteurInputSouscriptionViewModel.Email", "ErrorIdentique");
                        Logger.Error("email souscripteur en doublons: " +svm.SouscripteurInputSouscriptionViewModel.Email);
                    }
                    else if (listeMail2.Count > 0)
                    {
                        ModelState.AddModelError("SouscripteurInputSouscriptionViewModel.Email", "ErrorExistant");
                        Logger.Error("email souscripteur existant : " + svm.SouscripteurInputSouscriptionViewModel.Email);
                    }

                    //verifier si mail administrateur est valide
                    var listeMail3 = listeEmailsSaisies.FindAll(o => o.Equals(svm.AdministrationInputSouscriptionViewModel.Email));
                    var listeMail4 = listeEmailsExistants.FindAll(o => o.Equals(svm.AdministrationInputSouscriptionViewModel.Email));

                    if (listeMail3.Count() > 1 && !svm.EstSouscripteurAdmin)
                    {
                        ModelState.AddModelError("AdministrationInputSouscriptionViewModel.Email", "ErrorIdentique");
                        Logger.Error("email adminstrateur existant ou en doublons: " + svm.AdministrationInputSouscriptionViewModel.Email);
                    }
                    else if (listeMail4.Count > 0)
                    {
                        ModelState.AddModelError("AdministrationInputSouscriptionViewModel.Email", "ErrorExistant");
                        Logger.Error("email adminstrateur existant ou en doublons: " + svm.AdministrationInputSouscriptionViewModel.Email);
                    }

                    //verifier si mails utilisateurs sont valides
                    for (int i = 0; i < listeUtilisateurs.Count(); i++)
                    {
                        var listeMail5 = listeEmailsSaisies.FindAll(o => o.Equals(svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email));
                        var listeMail6 = listeEmailsExistants.FindAll(o => o.Equals(svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email));

                        if (listeMail5.Count() > 1)
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorIdentique");
                            Logger.Error("email utilisateur existant ou en doublons: " + svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email);
                        }
                        else if(listeMail6.Count > 0)
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorExistant");
                            Logger.Error("email utilisateur existant ou en doublons: " + svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email);
                        }
                    }
                }

                //Souscripteur et Administrateur meme personne
                if (svm.EstSouscripteurAdmin == true)
                {
                    svm.AdministrationInputSouscriptionViewModel.Nom = svm.SouscripteurInputSouscriptionViewModel.Nom;
                    ModelState["AdministrationInputSouscriptionViewModel.Nom"].Errors.Clear();

                    svm.AdministrationInputSouscriptionViewModel.Prenom = svm.SouscripteurInputSouscriptionViewModel.Prenom;
                    ModelState["AdministrationInputSouscriptionViewModel.Prenom"].Errors.Clear();

                    svm.AdministrationInputSouscriptionViewModel.Fonction = svm.SouscripteurInputSouscriptionViewModel.Fonction;
                    ModelState["AdministrationInputSouscriptionViewModel.Fonction"].Errors.Clear();

                    svm.AdministrationInputSouscriptionViewModel.Email = svm.SouscripteurInputSouscriptionViewModel.Email;
                    ModelState["AdministrationInputSouscriptionViewModel.Email"].Errors.Clear();

                    svm.AdministrationInputSouscriptionViewModel.Telephone = svm.SouscripteurInputSouscriptionViewModel.Telephone;
                    ModelState["AdministrationInputSouscriptionViewModel.Telephone"].Errors.Clear();
                }

                if (!svm.Accepter)
                {
                    ModelState.AddModelError("Accepter", "ErrorRequired");
                    ViewBag.Accepter = "ErrorRequired";
                }

                if (ModelState.IsValid)
                {
                    Session.Add("subscribeSession", svm);
                    return RedirectToAction("Index_two", "Souscription");
                }
                else
                {
                    //Détecter les erreurs
                    foreach (var valeur in ModelState.Values)
                    {
                        Logger.Error(valeur.Value.AttemptedValue + ", " + valeur.Value.AttemptedValue +","+ valeur.Errors);
                        foreach (var erreur in valeur.Errors)
                        {
                            Logger.Error(erreur.ErrorMessage + ", " + erreur.Exception);
                            
                            //Vérification champs obligatoire
                            if (erreur.ErrorMessage.Equals("ErrorRequired"))
                            {
                                ViewBag.Erreur = "ErrorRequired";
                            }

                            //Vérification format de l'email  
                            else if (erreur.ErrorMessage.Equals("ErrorMail"))
                            {
                                ViewBag.Erreur = "ErrorMail";
                            }

                            //Vérification adresses emails identiques
                            else if (erreur.ErrorMessage.Equals("ErrorIdentique"))
                            {
                                ViewBag.Erreur = "ErrorIdentique";
                            }

                            //Vérification adresses emails déjà référencées dans wattsgreen
                            else if (erreur.ErrorMessage.Equals("ErrorExistant"))
                            {
                                ViewBag.Erreur = "ErrorExistant";
                            }

                        }
                    }

                    ViewBag.subscribeType = "start";

                    var listePays = service.ListePays();
                    svm.SocieteInputSouscriptionViewModel.ListPays = listePays;

                    //svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs = (List<Utilisateurs>)Session["listeUtilisateurs"];
                    svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs = listeUtilisateurs;

                    return View("Index", svm);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Erreur donnees souscription " + e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Simulateur()
        {
            var evm = new EvaluationEntrepriseViewModel();

           // var evaluationEntrepriseViewModel = new EvaluationEntrepriseViewModel();

            //Liste déroulante pour choix de secteur d'activité
            var secteurActivites = service.GetListeSecteurActivite();
            evm.ListeSecteurActivite = secteurActivites;

            //Liste déroulante pour choix de devise
            var devises = service.GetListeDevise();
            evm.ListeDevise = devises;
            
            return View(evm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Simulateur(EvaluationEntrepriseViewModel evm)
        {
            try
            {
                var clientId = (int) Session["subscribeClientId"];
                var detailsClientId = 0;

                if (Session["detailsClientId"] != null)
                {
                    detailsClientId = (int) Session["detailsClientId"];
                }

                if (evm.SecteurActiviteId == null)
                {
                    ModelState.AddModelError("SecteurActiviteId", "ErrorRequired");
                }

                //if (evm.DeviseId == null)
                //{
                //    ModelState.AddModelError("DeviseId", "ErrorRequired");
                //}

                if (ModelState.IsValid)
                {
                    ClientApiController ApiClient = new ClientApiController();

                    Client client = ApiClient.GetClientById(clientId);
                    client.SecteurActiviteId = evm.SecteurActiviteId;
                    var clientRepository = new Repository<Client>();
                    clientRepository.Update(client);
                    _unitOfWork.SaveChanges();

                    DetailsClient detailClient = ApiClient.GetDetailsClientById(detailsClientId);

                    detailClient.Effectif = evm.Effectifs;
                    detailClient.TauxEquipement = evm.TauxEquipement;
                    detailClient.NombreSite = evm.NombreSites;
                    detailClient.PrixKWH = evm.PrixMoyenKwatt;
                    detailClient.DeviseId = evm.DeviseId;

                   //var pays = service.GetPaysById(client.Adresse.PaysId);
                   // if (pays != null && pays.CoefficientConversion.HasValue)
                   //     detailClient.CoefficientCO2 = pays.CoefficientConversion.Value;

                    var detailsClientRepository = new Repository<DetailsClient>();
                    detailsClientRepository.Update(detailClient);
                    _unitOfWork.SaveChanges();

                    Session.Add("userType", "Payant");
                    Session.Add("idClient", client.Id);
                    return RedirectToRoute(new {controller = "Consommation", action = "Index", id = client.Id});
                }
                else
                {
                    //Détecter les erreurs
                    foreach (var valeur in ModelState.Values)
                    {
                        foreach (var erreur in valeur.Errors)
                        {
                            //Vérification champs obligatoire
                            if (erreur.ErrorMessage.Equals("ErrorRequired"))
                            {
                                ViewBag.Erreur = "ErrorRequired";
                            }

                            //Vérification intervalle de valeurs et format de l'email  
                            else if (erreur.ErrorMessage.Equals("ErrorRange") || erreur.ErrorMessage.Equals("ErrorMail"))
                            {
                                ViewBag.Erreur = "ErrorRange";
                            }

                            else
                            {
                                ViewBag.Erreur = "ErrorFormat";
                            }

                        }
                    }

                    var secteurActivites = service.GetListeSecteurActivite();
                    //foreach (var secteur in secteurActivites)
                    //{
                    //    secteur.LibelleSecteurActivite = Resource.Traduction.ResourceManager.GetString("SecteurActivite" + secteur.Id) ?? secteur.LibelleSecteurActivite;
                    //}
                    evm.ListeSecteurActivite = secteurActivites;

                    //Liste déroulante pour choix de devise
                    var devises = service.GetListeDevise();
                    evm.ListeDevise = devises;

                    return View(evm);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult Souscription_Pdf()
        {
            try
            {
                var souscriptionSession = (SouscriptionViewModel)Session["subscribeSession"];

                var listePays = service.ListePays();
                foreach (var pays in listePays)
                {
                    pays.LibellePays = Resource.Traduction.ResourceManager.GetString("Pays" + pays.Id) ?? pays.LibellePays;
                }
                souscriptionSession.SocieteInputSouscriptionViewModel.ListPays = listePays;

                souscriptionSession.SocieteInputSouscriptionViewModel.LibelleClientEconocom = souscriptionSession.SocieteInputSouscriptionViewModel.ClientEconocom == true ? Resource.Traduction.Oui : Resource.Traduction.Non;
                
                var p = service.GetPaysById(souscriptionSession.SocieteInputSouscriptionViewModel.PaysId.Value);
                souscriptionSession.SocieteInputSouscriptionViewModel.LibellePays = Resource.Traduction.ResourceManager.GetString("Pays" + p.Id) ?? p.LibellePays;
                //return new PartialViewAsPdf("Souscription_Pdf", souscriptionSession);
                
                
                return new RazorPDF.PdfResult(souscriptionSession, "Souscription_Pdf");
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
        }


        public FileResult TermesEtConditionsPartial()
        {
            try
            {
                string path = Server.MapPath("~/uploads/tec.pdf");
                string filename = Path.GetFileName(path);
                Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + filename+"\"");
                return File(path, "application/pdf");

               // return File("~/uploads/tec.pdf", System.Net.Mime.MediaTypeNames.Application.Pdf, "tec.pdf");
                //return File("~/uploads/tec.pdf", "application/pdf");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
