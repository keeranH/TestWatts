using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Data;
using Econocom.Model.Enum;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Helper.Email;
using NLog;

namespace Econocom.Admin.Controllers.GestionClient
{
    public class ModerationController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IUnitOfWork _unitOfWork = new EconocomContext();
       
        private readonly ServiceApiController _service;
        public static int PageSize = 10;

        public ModerationController()
        {
            _service = new ServiceApiController();
            PageSize = 10;
        }

        //
        // GET: /Moderation/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Souscription(int? page)
        {
            try
            {
                var apiModeration = new ModerationApiController();

                var pageDimension = 10;

                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;

                int totalPages = 1;
                var clients = apiModeration.GetClientNonModerer(page, pageDimension, out totalPages);
                ViewBag.Total = totalPages;

                return View(clients);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var apiModeration = new ModerationApiController();
                var client = apiModeration.GetClientParId(id);

                var contact_soucripteur = apiModeration.GetTypeContactByString("Souscripteur");
                var contact_administrateur = apiModeration.GetTypeContactByString("Administrateur");
                TypeContact contact_utilisateur = apiModeration.GetTypeContactByString("Utilisateur");
                List<Contact> listeUtilisateurs = apiModeration.GetUtilisateursByContactAndTypeContact(id, contact_utilisateur.Id);

                if (client == null)
                    return RedirectToAction("Index", "Error");

                var adress = apiModeration.GetAdresseById((int) client.AdresseId);
                Pays pays = apiModeration.getPaysById(adress.PaysId);
                var souscripteur = apiModeration.GetContactByTypeAndClient(contact_soucripteur.Id, id);
                var administrateur = apiModeration.GetContactByTypeAndClient(contact_administrateur.Id, id);
               
                var svm = new SouscriptionViewModel();
                SocieteSouscriptionViewModel ssvm=  new SocieteSouscriptionViewModel();

                SocieteSouscriptionViewModel societeSouscription = new SocieteSouscriptionViewModel();
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
                societeSouscription.ListPays = apiModeration.ListePays();
                societeSouscription.PaysId = adress.PaysId;
                societeSouscription.LibellePays = pays.LibellePays;
                societeSouscription.CodeInterneWattsGreen = client.CodeInterneWattsGreen;
                societeSouscription.ChiffresAffaires = client.DetailsClient.ChiffreAffaire;

                if (client.ClientEconocom == false)
                {
                    societeSouscription.LibelleClientEconocom = "Non";
                }
                else
                {
                    societeSouscription.LibelleClientEconocom = "Oui";
                }
                svm.SocieteInputSouscriptionViewModel = societeSouscription;

                ContactSouscriptionViewModel contactSouscription = new ContactSouscriptionViewModel();
                contactSouscription.Nom = souscripteur.NomContact;
                contactSouscription.Prenom = souscripteur.PrenomContact;
                contactSouscription.Email = souscripteur.Email;
                contactSouscription.Fonction = souscripteur.Fonction;
                contactSouscription.Telephone = souscripteur.NumeroPhone;
                contactSouscription.ClientId = souscripteur.ClientId;
                svm.SouscripteurInputSouscriptionViewModel = contactSouscription;

                if (administrateur != null)
                {
                    ContactSouscriptionViewModel adminSouscription = new ContactSouscriptionViewModel();
                    adminSouscription.Nom = administrateur.NomContact;
                    adminSouscription.Prenom = administrateur.PrenomContact;
                    adminSouscription.Email = administrateur.Email;
                    adminSouscription.Fonction = administrateur.Fonction;
                    adminSouscription.Telephone = administrateur.NumeroPhone;
                    adminSouscription.ClientId = administrateur.ClientId;
                    svm.AdministrationInputSouscriptionViewModel = adminSouscription;
                }
                else
                {
                    svm.EstSouscripteurAdmin = true;

                    ContactSouscriptionViewModel adminSouscription = new ContactSouscriptionViewModel();
                    adminSouscription.Nom = souscripteur.NomContact;
                    adminSouscription.Prenom = souscripteur.PrenomContact;
                    adminSouscription.Email = souscripteur.Email;
                    adminSouscription.Fonction = souscripteur.Fonction;
                    adminSouscription.Telephone = souscripteur.NumeroPhone;
                    adminSouscription.ClientId = souscripteur.ClientId;
                    svm.AdministrationInputSouscriptionViewModel = adminSouscription;
                }

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
                svm.UtilisateurInputSouscriptionViewModel = ecranUtilisateurs;


               

                return View(svm);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Error");
            }
        }

        /*
        [HttpPost]
        public ActionResult Edit(String btnstatus, RegistrationClientModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var btnStatus = btnstatus.Equals("Accept") ? "Accepté" : "Rejeté";
                    var apiModeration = new ModerationApiController();
                    apiModeration.ActualiserClient(model.ToClient());
                    return RedirectToAction("Souscription", "Moderation");
                }
                catch (Exception e)
                {
                    return RedirectToAction("Index", "Error");
                }
            }

            if (model == null)
                return RedirectToAction("Index", "Error");

            return View(model);
        }*/

        [HttpPost]
        public ActionResult SauvegarderSouscription(String btnStatus, SouscriptionViewModel svm)
        {
            try
            {
                var apiModeration = new ModerationApiController();

                string expression = @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$"; 
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


                var clientId = Convert.ToInt32(svm.SocieteInputSouscriptionViewModel.ClientId);
                var contact_administrateur = apiModeration.GetTypeContactByString("Administrateur");
                var administrateur = apiModeration.GetContactByTypeAndClient(contact_administrateur.Id, clientId);

                var listeEmailsSaisies = new List<String>();

                if (svm.SouscripteurInputSouscriptionViewModel.Email != null)
                {
                    listeEmailsSaisies.Add(svm.SouscripteurInputSouscriptionViewModel.Email);
                }

                if (administrateur != null)
                {
                    if (svm.AdministrationInputSouscriptionViewModel.Email != null)
                    {
                        listeEmailsSaisies.Add(svm.AdministrationInputSouscriptionViewModel.Email);
                    }
                }

                foreach (var utilisateur in svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs)
                {
                    if (utilisateur.Email != null)
                    {
                        listeEmailsSaisies.Add(utilisateur.Email);
                    }
                }

                var listeEmailsExistants = apiModeration.GetListeEmailPasValide(clientId);
                
                if (listeEmailsSaisies != null && listeEmailsSaisies.Count > 0)
                {
                    //verifier si mail souscripteur est valide
                    var listeMail1 = listeEmailsSaisies.FindAll(o => o.Equals(svm.SouscripteurInputSouscriptionViewModel.Email));
                    var listeMail2 = listeEmailsExistants.FindAll(o => o.Equals(svm.SouscripteurInputSouscriptionViewModel.Email));
                    
                    if (listeMail1.Count > 1)
                    {
                        ModelState.AddModelError("SouscripteurInputSouscriptionViewModel.Email", "ErrorIdentique");
                        Logger.Error("email souscripteur en doublons: " + svm.SouscripteurInputSouscriptionViewModel.Email);
                    }
                    else if (listeMail2.Count > 0)
                    {
                        ModelState.AddModelError("SouscripteurInputSouscriptionViewModel.Email", "ErrorExistant");
                        Logger.Error("email souscripteur existant : " + svm.SouscripteurInputSouscriptionViewModel.Email);
                    }

                    //verifier si mail administrateur est valide
                    var listeMail3 = listeEmailsSaisies.FindAll(o => o.Equals(svm.AdministrationInputSouscriptionViewModel.Email));
                    var listeMail4 = listeEmailsExistants.FindAll(o => o.Equals(svm.AdministrationInputSouscriptionViewModel.Email));
                    
                    if (listeMail3.Count > 1)
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
                    for (int i = 0; i < listeUtilisateurs.Count; i++)
                    {
                        var listeMail5 = listeEmailsSaisies.FindAll(o => o.Equals(svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email));
                        var listeMail6 = listeEmailsExistants.FindAll(o => o.Equals(svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email));

                        if (listeMail5.Count > 1)
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorIdentique");
                            Logger.Error("email utilisateur existant ou en doublons: " + svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email);
                        }
                        else if (listeMail6.Count > 0)
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorExistant");
                            Logger.Error("email utilisateur existant ou en doublons: " + svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email);
                        }
                    }
                }
                

                //Souscripteur et Administrateur meme personne
                if (administrateur == null)
                {
                    svm.EstSouscripteurAdmin = true;

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

                if (ModelState.IsValid)
                {
                    var contact_soucripteur = apiModeration.GetTypeContactByString("Souscripteur");
                    TypeContact contact_utilisateur = apiModeration.GetTypeContactByString("Utilisateur");

                    Client societe = apiModeration.GetClientById(clientId);
                    var adresse = apiModeration.GetAdresseById((int) societe.AdresseId);

                    try
                    {
                        societe.RaisonSociale = svm.SocieteInputSouscriptionViewModel.RaisonSociale;
                        societe.TVAIntraComm = svm.SocieteInputSouscriptionViewModel.Tva;
                        societe.Groupe = svm.SocieteInputSouscriptionViewModel.Groupe;
                        societe.IdentificationNational = svm.SocieteInputSouscriptionViewModel.IdentificationNational;
                        societe.ClientEconocom = svm.SocieteInputSouscriptionViewModel.ClientEconocom;
                        societe.ContactEconocom = svm.SocieteInputSouscriptionViewModel.VotreContacte;
                        societe.CodeInterneWattsGreen = svm.SocieteInputSouscriptionViewModel.CodeInterneWattsGreen;
                        societe.DateModification = DateTime.Today;

                        if (svm.SocieteInputSouscriptionViewModel.PaysId != null)
                            adresse.PaysId = (int) svm.SocieteInputSouscriptionViewModel.PaysId;
                        adresse.Adresse1 = svm.SocieteInputSouscriptionViewModel.Addresse;
                        adresse.CodePostal = svm.SocieteInputSouscriptionViewModel.CodePostal;
                        adresse.Ville = svm.SocieteInputSouscriptionViewModel.Ville;

                        apiModeration.updateClient(societe);
                        apiModeration.updateAdress(adresse);

                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    Contact souscripteur = apiModeration.GetContactByTypeAndClient(contact_soucripteur.Id, clientId);

                    try
                    {
                        souscripteur.ClientId = clientId;
                        souscripteur.NomContact = svm.SouscripteurInputSouscriptionViewModel.Nom;
                        souscripteur.PrenomContact = svm.SouscripteurInputSouscriptionViewModel.Prenom;
                        souscripteur.Fonction = svm.SouscripteurInputSouscriptionViewModel.Fonction;
                        souscripteur.NumeroPhone = svm.SouscripteurInputSouscriptionViewModel.Telephone;
                        souscripteur.Email = svm.SouscripteurInputSouscriptionViewModel.Email;


                        apiModeration.updateContacte(souscripteur);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    
                    if (administrateur != null)
                    {
                        try
                        {
                            administrateur.ClientId = clientId;
                            administrateur.NomContact = svm.AdministrationInputSouscriptionViewModel.Nom;
                            administrateur.PrenomContact = svm.AdministrationInputSouscriptionViewModel.Prenom;
                            administrateur.Fonction = svm.AdministrationInputSouscriptionViewModel.Fonction;
                            administrateur.NumeroPhone = svm.AdministrationInputSouscriptionViewModel.Telephone;
                            administrateur.Email = svm.AdministrationInputSouscriptionViewModel.Email;
                            
                            apiModeration.updateContacte(administrateur);

                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }

                    var utilisateurRepository = new Repository<Contact>();

                    List<Contact> maList = new List<Contact>();
                    maList = apiModeration.GetUtilisateursByContactAndTypeContact(clientId, contact_utilisateur.Id);

                    for (int i = 0; i < maList.Count; i++)
                    {
                        for (int j = 0; j < svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs.Count; j++)
                        {
                            if (maList[i].Id == svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].id)
                            {
                                maList[i].NomContact =
                                    svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].Nom;
                                maList[i].PrenomContact =
                                    svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].Prenom;
                                maList[i].Email = svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].Email;

                                utilisateurRepository.Update(maList[i]);

                                _unitOfWork.SaveChanges();
                            }
                        }
                    }

                    for (int j = maList.Count;
                         j < svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs.Count;
                         j++)
                    {
                        if (svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].Nom != null)
                        {
                            Contact utilisateur = new Contact();
                            utilisateur.NomContact = svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].Nom;
                            utilisateur.PrenomContact =
                                svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].Prenom;
                            utilisateur.Email = svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[j].Email;
                            utilisateur.ClientId = clientId;
                            utilisateur.TypeContactId = 3;

                            utilisateurRepository.Create(utilisateur);
                            _unitOfWork.SaveChanges();
                        }

                    }

                    if (btnStatus.Equals("Valider"))
                    {
                        //maj client - actif = true
                        try
                        {
                            Client client =
                                apiModeration.GetClientParId(
                                    Convert.ToInt32(svm.SocieteInputSouscriptionViewModel.ClientId));

                            client.Statut = (int)ClientStatut.Valide;
                            client.DateModification = DateTime.Now;
                            client.DateValidation = DateTime.Now;
                            var clientRepository = new Repository<Client>();
                            clientRepository.Update(client);
                            _unitOfWork.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                        //envoyer mail
                        try
                        {
                            EnvoyerEmailModeration(svm);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Erreur lors de l'envoi de mail apres moderation");
                        }

                        return RedirectToAction("Souscription");
                    }
                    else
                    {
                        svm.SocieteInputSouscriptionViewModel.ListPays = apiModeration.ListePays();
                        return View("Edit", svm);
                    }
                }
                else
                {
                    //Détecter les erreurs
                    foreach (var valeur in ModelState.Values)
                    {
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
                    svm.SocieteInputSouscriptionViewModel.ListPays = apiModeration.ListePays();
                    return View("Edit", svm);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool EnvoyerEmailModeration(SouscriptionViewModel souscriptionViewModel)
        {
            var apiModeration = new ModerationApiController();
            var emailList = new Dictionary<string, string>();

            try
            {
                // Ajouter l'adresse mail du souscripteur
                var contact = apiModeration.GetContactParEmail(souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Email);
                emailList.Add(souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Email, contact.CodeVerification);

                // Ajouter l'adresse mail de l'administrateur
                contact = apiModeration.GetContactParEmail(souscriptionViewModel.AdministrationInputSouscriptionViewModel.Email);
                if (!emailList.ContainsKey(souscriptionViewModel.AdministrationInputSouscriptionViewModel.Email))
                {
                    emailList.Add(souscriptionViewModel.AdministrationInputSouscriptionViewModel.Email, contact.CodeVerification);
                }

                // Ajouter les adresses mail des utilisateurs
                foreach (var utilisateur in souscriptionViewModel.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs)
                {
                    if (utilisateur.Email != null)
                    {
                        contact = apiModeration.GetContactParEmail(utilisateur.Email);
                        emailList.Add(utilisateur.Email, contact.CodeVerification);
                    }
                }

                var idPays = 1;
                
                try
                {
                    idPays = souscriptionViewModel.SocieteInputSouscriptionViewModel.PaysId ?? 2;
                    var langue = _service.GetLangueParIdPays(idPays);
                    var idLangue = langue == null ? 1 : langue.Id;
                    var codeLangue = langue == null ? "fr" : langue.Code;

                    // Recuperer le sujet et le contenu du mail
                    var typeMail = apiModeration.GetTypeMail("Validation souscription");
                    var contenuMail = apiModeration.GetContenuMail(typeMail.Id, idLangue); // Hard-coded

                    var routageHome = _service.GetRoutageByLangueIdAndValeurActuelle(idLangue, "Home");
                    if(routageHome==null)
                        Logger.Error("Pas de routage pour Home, idLangue="+idLangue);

                    var routageGestionMotDePasse = _service.GetRoutageByLangueIdAndValeurActuelle(idLangue, "GestionMotDePasse");
                    if (routageGestionMotDePasse == null)
                        Logger.Error("Pas de routage pour GestionMotDePasse, idLangue=" + idLangue);

                    if (routageHome != null && routageGestionMotDePasse != null)
                    {
                        var lien = "xxServerAddress/" + codeLangue + "/" + routageHome.ValeurTraduite + "/" +
                                   routageGestionMotDePasse.ValeurTraduite + "?email=";
                        // Envoyer Email
                        return EmailHelper.SendMailModeration(emailList, contenuMail, lien);
                    }
                    return false;
                }
                catch (Exception e)
                {
                    Logger.Error("Erreur lors de la recuperation du langue avec idPays = "+ idPays);
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
            return false;
        }

        public ActionResult ValidateClient(int id)
        {
             ModerationApiController moderationApi= new ModerationApiController();
             Client client = moderationApi.GetClientParId(id);
             try
             {
                 client.Statut = (int)ClientStatut.Valide;
                 client.DateModification = DateTime.Now;
                 var societeRepository = new Repository<Client>();
                 societeRepository.Update(client);
                 _unitOfWork.SaveChanges();
             }
             catch (Exception e)
             {
                 throw e;
             }

             //var clients = moderationApi.GetClientNonModerer();
             //return View("Souscription", clients);
             return RedirectToAction("Souscription");
            
         }

         public ActionResult RejectClient(int id)
         {
             ModerationApiController moderationApi = new ModerationApiController();
             Client client = moderationApi.GetClientParId(id);

             try
             {
                 client.Statut = (int)ClientStatut.Refuse;
                 client.DateModification = DateTime.Now;
                 var societeRepository = new Repository<Client>();
                 societeRepository.Update(client);
                 _unitOfWork.SaveChanges();
             }
             catch (Exception e)
             {
                 throw e;
             }

             //var clients = moderationApi.GetClientNonModerer();

             //return View("Souscription", clients);
             return RedirectToAction("Souscription");
         }
    }
}
