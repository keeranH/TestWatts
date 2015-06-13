using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Econocom.Helper.Authorisation;
using Econocom.Helper.Email;
using Econocom.Model.Enum;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Web4.Controllers.ApiControllers;
using NLog;
using TypeClient = Econocom.Model.Enum.TypeClient;

namespace Econocom.Web4.Controllers
{
    public class MesInformationsController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ServiceApiController service;

        public MesInformationsController()
        {
           this.InitLanguageDropDown();
           service = new ServiceApiController();
        }

        //
        // GET: /MesInformations/
        public ActionResult Index()
        {
            try
            {
                var mesInformationsViewModel = new MesInformationsViewModel();
                var societeViewModel = new SocieteSouscriptionViewModel();
                var entrepriseViewModel = new EvaluationEntrepriseViewModel();
                var utilisateurViewModel = new UtilisateurSouscriptionViewModel();

                mesInformationsViewModel.SocieteSouscriptionViewModel = societeViewModel;
                mesInformationsViewModel.DetailsEntrepriseViewModel = entrepriseViewModel;
                mesInformationsViewModel.UtilisateurSouscriptionViewModel = utilisateurViewModel;

                var listePays = service.ListePays();
                societeViewModel.ListPays = listePays;

                //Liste déroulante pour choix de secteur d'activité
                var secteurActivites = service.GetListeSecteurActivite();
                entrepriseViewModel.ListeSecteurActivite = secteurActivites;

                //Liste déroulante pour choix de devise
                var devises = service.GetListeDevise();
                entrepriseViewModel.ListeDevise = devises;

                var contactEmail = User.Identity.Name;
                var contact = new Contact();
                if (contactEmail != string.Empty)
                {
                    contact = service.GetContactParEmail(contactEmail);
                }
                else
                {
                    Logger.Info("Redirection page d'accueil");
                    return RedirectToAction("Index", "Home");
                }

                var client = service.GetClientParId(contact.ClientId);
                mesInformationsViewModel = ConstruireMesInformations(client, mesInformationsViewModel);

                mesInformationsViewModel.TypeClient = client.TypeClient;

                var autorisationHelper = new AuthorisationHelper();

                //vérifier si utilisateur est un admin ou non
                if (autorisationHelper.EstAdministrateur(contact) || autorisationHelper.EstSouscripteurAdmin(contact))
                {
                    ViewBag.TypeContact = "Admin";
                    Session.Add("typeContact", "Admin");
                    Logger.Info("TypeContact => Admin");
                }

                ViewBag.MsgModification = Session["MsgModification"];
                Session.Remove("MsgModification");

                ViewBag.TypeContact = Session["typeContact"];

                return View(mesInformationsViewModel);
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        public MesInformationsViewModel ConstruireMesInformations(Client client, MesInformationsViewModel mesInformationsViewModel)
        {
            try
            {
                var societeViewModel = mesInformationsViewModel.SocieteSouscriptionViewModel;
                var entrepriseViewModel = mesInformationsViewModel.DetailsEntrepriseViewModel;
                var utilisateurViewModel = mesInformationsViewModel.UtilisateurSouscriptionViewModel;

                //Details societe
                societeViewModel.RaisonSociale = client.RaisonSociale;
                societeViewModel.Groupe = client.Groupe;
                societeViewModel.Addresse = client.Adresse.Adresse1;
                societeViewModel.PaysId = client.Adresse.PaysId;
                societeViewModel.CodePostal = client.Adresse.CodePostal;
                societeViewModel.Ville = client.Adresse.Ville;
                societeViewModel.ChiffresAffaires = client.DetailsClient.ChiffreAffaire;

                //Details entreprise
                entrepriseViewModel.SecteurActiviteId = client.SecteurActiviteId;
                entrepriseViewModel.Effectifs = client.DetailsClient.Effectif;
                entrepriseViewModel.NombreSites = client.DetailsClient.NombreSite;
                entrepriseViewModel.TauxEquipement = client.DetailsClient.TauxEquipement;
                entrepriseViewModel.PrixMoyenKwatt = client.DetailsClient.PrixKWH;
                entrepriseViewModel.DeviseId = client.DetailsClient.DeviseId;
                entrepriseViewModel.CoefficientCO2 = client.DetailsClient.CoefficientCO2;

                var authorisationHelper = new AuthorisationHelper();
                var nouvelleListeUtilisateurs = new List<Utilisateurs>();

                //Admin
                var admin = client.Contacts.Where(o => o.Authorisations == (int) TypeContactEnum.Administrateur);
                var admin1 = admin.Any() ? admin.Single() : client.Contacts.Single(o => o.Authorisations == (int)TypeContactEnum.SouscripteurAdmin);
                var utilisateurAdmin = new Utilisateurs { id = admin1.Id, Nom = admin1.NomContact, Prenom = admin1.PrenomContact, Email = admin1.Email };
                nouvelleListeUtilisateurs.Add(utilisateurAdmin);
                
                //Souscripteur
                var souscripteur = client.Contacts.Where(o => o.Authorisations == (int)TypeContactEnum.Souscripteur && o.DateFin == null);
                var souscripteur1 = souscripteur.Any() ? souscripteur.Single() : null;
                if (souscripteur1 != null)
                {
                    var utilisateurSouscripteur = new Utilisateurs
                        {
                            id = souscripteur1.Id,
                            Nom = souscripteur1.NomContact,
                            Prenom = souscripteur1.PrenomContact,
                            Email = souscripteur1.Email
                        };
                    nouvelleListeUtilisateurs.Add(utilisateurSouscripteur);
                }

                //Autres Utilisateurs
                foreach (var contact in client.Contacts.Where(o => o.Authorisations == (int)TypeContactEnum.Utilisateur && o.DateFin == null))
                {
                    var utilisateur = new Utilisateurs();
                    utilisateur.id = contact.Id;
                    utilisateur.Nom = contact.NomContact;
                    utilisateur.Prenom = contact.PrenomContact;
                    utilisateur.Email = contact.Email;
                    nouvelleListeUtilisateurs.Add(utilisateur);
                }

                if (nouvelleListeUtilisateurs.Count < 7)
                {
                    var difference = 7 - nouvelleListeUtilisateurs.Count;
                    for (int i = 0; i < difference; i++)
                    {
                        var utilisateur = new Utilisateurs();
                        nouvelleListeUtilisateurs.Add(utilisateur);
                    }
                }

                utilisateurViewModel.ListeUtilisateurs = nouvelleListeUtilisateurs;

                return mesInformationsViewModel;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw e;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(MesInformationsViewModel mesInformationsViewModel)
        {
            bool recalculer = false;

            try
            {
                var contactEmail = User.Identity.Name;
                var contact = new Contact();
                if (contactEmail != string.Empty)
                {
                    contact = service.GetContactParEmail(contactEmail);
                }
                else
                {
                    Logger.Info("Redirection page d'accueil");
                    return RedirectToAction("Index", "Home");
                }

                var client = service.GetClientParIdClient(contact.ClientId);

                mesInformationsViewModel.TypeClient = client.TypeClient;

                if (client.TypeClient != (int)TypeClient.Econocom)
                {
                    ModelState["DetailsEntrepriseViewModel.TauxEquipement"].Errors.Clear();
                    ModelState["DetailsEntrepriseViewModel.NombreSites"].Errors.Clear();
                }
                        
                if (mesInformationsViewModel.SocieteSouscriptionViewModel.PaysId == null)
                {
                    ModelState.AddModelError("SocieteSouscriptionViewModel.PaysId", "ErrorRequired");
                }

                if (mesInformationsViewModel.DetailsEntrepriseViewModel.SecteurActiviteId == null)
                {
                    ModelState.AddModelError("DetailsEntrepriseViewModel.SecteurActiviteId", "ErrorRequired");
                }

                if (mesInformationsViewModel.DetailsEntrepriseViewModel.DeviseId == null)
                {
                    ModelState.AddModelError("DetailsEntrepriseViewModel.DeviseId", "ErrorRequired");
                }

                string expression = @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$";
               
                var listeUtilisateurs = mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs;
                for (int i = 0; i < listeUtilisateurs.Count; i++)
                {
                    if ((listeUtilisateurs[i].Nom == null || listeUtilisateurs[i].Nom.Equals("")) &&
                        (listeUtilisateurs[i].Prenom == null || listeUtilisateurs[i].Prenom.Equals("")) &&
                        (listeUtilisateurs[i].Email == null || listeUtilisateurs[i].Email.Equals("")))
                    {
                        if (listeUtilisateurs[i].id != null)
                        {
                            /*
                            ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Nom", "ErrorRequired");
                            ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Prenom", "ErrorRequired");
                            ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorRequired");
                            */
                        }
                    }
                    else
                    {
                        if (listeUtilisateurs[i].Email != null && !listeUtilisateurs[i].Email.Equals(""))
                        {
                            Match match = Regex.Match(listeUtilisateurs[i].Email, expression, RegexOptions.IgnoreCase);
                            if (!match.Success)
                                ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorMail");
                        }
                        if (listeUtilisateurs[i].Nom == null || listeUtilisateurs[i].Nom.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Nom", "ErrorRequired");
                        }
                        if (listeUtilisateurs[i].Prenom == null || listeUtilisateurs[i].Prenom.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Prenom", "ErrorRequired");
                        }
                        if (listeUtilisateurs[i].Email == null || listeUtilisateurs[i].Email.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorRequired");
                        }
                    }
                }


                var listeEmailsSaisies = new List<String>();
                
                for (int i = 1; i < listeUtilisateurs.Count; i++)
                {
                    if (listeUtilisateurs[i].Email != null)
                    {
                        listeEmailsSaisies.Add(listeUtilisateurs[i].Email);
                    }
                }

                var listeEmailsExistants = service.GetListeEmails();


                if (listeEmailsSaisies != null && listeEmailsSaisies.Count > 0)
                {
                    
                    //verifier si mails utilisateurs sont valides
                    for (int i = 1; i < listeUtilisateurs.Count(); i++)
                    {
                        var listeMail5 = listeEmailsSaisies.FindAll(o => o.Equals(mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs[i].Email));
                        var listeMail6 = listeEmailsExistants.FindAll(o => o.Equals(mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs[i].Email));
                        
                        if (listeUtilisateurs[i].Email != null && listeUtilisateurs[i].Email.Equals(listeUtilisateurs[0].Email))
                        {
                            ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorIdentique");
                        }

                        if (listeUtilisateurs[i].id == null)
                        {
                            if (listeMail5.Count() > 1)
                            {
                                ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorIdentique");
                                Logger.Error("email utilisateur existant ou en doublons: " +mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs[i].Email);
                            }
                            else if (listeMail6.Count > 0)
                            {
                                ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorExistant");
                                Logger.Error("email utilisateur existant ou en doublons: " +mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs[i].Email);
                            }
                        }
                        else
                        {
                            if (listeMail5.Count() > 1)
                            {
                                ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorIdentique");
                                Logger.Error("email utilisateur existant ou en doublons: " + mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs[i].Email);
                            }
                            else if (listeMail6.Count > 1)
                            {
                                ModelState.AddModelError("UtilisateurSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorExistant");
                                Logger.Error("email utilisateur existant ou en doublons: " + mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs[i].Email);
                            }
                        }
                    }
                }

                //Vérifier si le modèle est valide
                if (ModelState.IsValid)
                {
                    try
                    {
                        var societeViewModel = mesInformationsViewModel.SocieteSouscriptionViewModel;
                        var entrepriseViewModel = mesInformationsViewModel.DetailsEntrepriseViewModel;
                        var utilisateurViewModel = mesInformationsViewModel.UtilisateurSouscriptionViewModel;
                       
                        //MAJ Adresse
                        var adresse = client.Adresse;
                        adresse.Adresse1 = societeViewModel.Addresse;
                        adresse.PaysId = societeViewModel.PaysId.Value;
                        adresse.CodePostal = societeViewModel.CodePostal;
                        adresse.Ville = societeViewModel.Ville;
                        adresse = service.MAJAdresse(adresse);

                        //MAJ DetailsClient
                        var detailsClient = client.DetailsClient;
                        detailsClient.ChiffreAffaire = societeViewModel.ChiffresAffaires;
                        detailsClient.Effectif = entrepriseViewModel.Effectifs;
                        detailsClient.NombreSite = entrepriseViewModel.NombreSites;
                        detailsClient.TauxEquipement = entrepriseViewModel.TauxEquipement;
                        if (detailsClient.PrixKWH != entrepriseViewModel.PrixMoyenKwatt)
                        {
                            detailsClient.PrixKWH = entrepriseViewModel.PrixMoyenKwatt;
                            recalculer = true;
                        }
                        detailsClient.DeviseId = entrepriseViewModel.DeviseId;
                        if (detailsClient.CoefficientCO2 != entrepriseViewModel.CoefficientCO2)
                        {
                            detailsClient.CoefficientCO2 = entrepriseViewModel.CoefficientCO2;
                            recalculer = true;
                        }
                        detailsClient = service.MAJDetailsClient(detailsClient);

                        //MAJ ConfigSiConsommation

                        //MAJ Resultat

                        //MAJ Utilisateurs
                        var contacts = client.Contacts;
                        TypeContact contact_utilisateur = service.GetTypeContactByString("Utilisateur");
                        var contactsReinitialisation = new List<Contact>();

                        foreach (var utilisateur in utilisateurViewModel.ListeUtilisateurs)
                        {
                            //MAJ utilisateur existant
                            if (utilisateur.id != null)
                            {
                                var contact1 = contacts.Single(o => o.Id == utilisateur.id);

                                if ((utilisateur.Nom == null || utilisateur.Nom.Equals("")) &&
                                    (utilisateur.Prenom == null || utilisateur.Prenom.Equals("")) &&
                                    (utilisateur.Email == null || utilisateur.Email.Equals("")))
                                {
                                    //MAJ date de fin
                                    //var supprime = service.SupprimeContact(utilisateur.id.Value);
                                    contact1.DateFin = DateTime.Now;
                                    contact1 = service.UpdateContact(contact1);

                                    utilisateur.id = null;
                                }
                                else
                                {
                                    contact1.NomContact = utilisateur.Nom;
                                    contact1.PrenomContact = utilisateur.Prenom;
                                    contact1.Email = utilisateur.Email;

                                    contact1 = service.UpdateContact(contact1);

                                    if (utilisateur.GenereMDP && utilisateur.Email != null)
                                    {
                                        contactsReinitialisation.Add(contact1);
                                    }
                                }
                            }
                            else
                            {
                                //Creation nouveau utilisateur
                                if (utilisateur.Nom != null)
                                {
                                    var nouveauUtilisateur = new Contact()
                                    {
                                        NomContact = utilisateur.Nom,
                                        PrenomContact = utilisateur.Prenom,
                                        Email = utilisateur.Email,
                                        TypeContactId = contact_utilisateur.Id,
                                        CodeVerification = Guid.NewGuid().ToString(),
                                        ClientId = client.Id,
                                        Authorisations = (int) TypeContactEnum.Utilisateur
                                    };

                                    nouveauUtilisateur = service.SetContact(nouveauUtilisateur);
                                    contactsReinitialisation.Add(nouveauUtilisateur);
                                    
                                    utilisateur.id = nouveauUtilisateur.Id;
                                }
                            }
                        }
                        ReInitialisationMotDePasse(contactsReinitialisation);
                        
                        //MAJ Client
                        client.RaisonSociale = societeViewModel.RaisonSociale;
                        client.Groupe = societeViewModel.Groupe;
                        client.SecteurActiviteId = entrepriseViewModel.SecteurActiviteId;
                        client = service.MAJClient(client);

                        Session.Add("MsgModification", Resource.Traduction.MsgModification);

                        if (recalculer)
                        {
                            //reset current session: Simulation or Personnalisation
                            var resultat = RecalculerConsommation(entrepriseViewModel.CoefficientCO2);
                            resultat = MajResultat(entrepriseViewModel.CoefficientCO2);

                            //reset Simulation or Personnalisation
                            resultat = MajConsommation(entrepriseViewModel.CoefficientCO2);
                        }

                        /*
                        //Reload page

                        //Liste déroulante pour choix de pays
                        mesInformationsViewModel.SocieteSouscriptionViewModel.ListPays = service.ListePays();

                        //Liste déroulante pour choix de secteur d'activité
                        mesInformationsViewModel.DetailsEntrepriseViewModel.ListeSecteurActivite = service.GetListeSecteurActivite();

                        //Liste déroulante pour choix de devise
                        mesInformationsViewModel.DetailsEntrepriseViewModel.ListeDevise = service.GetListeDevise();

                        //Generer MDP -> FAUX
                        foreach (var utilisateur in mesInformationsViewModel.UtilisateurSouscriptionViewModel.ListeUtilisateurs)
                        {
                            utilisateur.GenereMDP = false;
                        }*/

                        ViewBag.TypeContact = Session["typeContact"];

                        //return View(mesInformationsViewModel);

                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        LogguerErreur(e);
                        throw e;
                    }
                }
                else
                {
                    //Détecter les erreurs
                    foreach (var valeur in ModelState.Values)
                    {
                        Logger.Error(valeur.Value.AttemptedValue + ", " + valeur.Value.AttemptedValue + "," + valeur.Errors);
                        foreach (var erreur in valeur.Errors)
                        {
                            Logger.Error(erreur.ErrorMessage + ", " + erreur.Exception);

                            //Vérification champs obligatoire
                            if (erreur.ErrorMessage.Equals("ErrorRequired"))
                            {
                                ViewBag.Erreur = "ErrorRequired";
                            }

                            else if (erreur.ErrorMessage.Equals("ErrorRange"))
                            {
                                ViewBag.Erreur = "ErrorRange";
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

                            else
                            {
                                ViewBag.Erreur = "ErrorFormat";
                            }
                        }
                    }
                }

                //Liste déroulante pour choix de pays
                mesInformationsViewModel.SocieteSouscriptionViewModel.ListPays = service.ListePays(); 

                //Liste déroulante pour choix de secteur d'activité
                mesInformationsViewModel.DetailsEntrepriseViewModel.ListeSecteurActivite = service.GetListeSecteurActivite(); 

                //Liste déroulante pour choix de devise
                mesInformationsViewModel.DetailsEntrepriseViewModel.ListeDevise = service.GetListeDevise(); 

                ViewBag.TypeContact = Session["typeContact"];

                return View(mesInformationsViewModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ReInitialisationMotDePasse(List<Contact> listeContact)
        {
            try
            {
                foreach (var contact in listeContact)
                {
                    //Creer nouveau code de verification
                    contact.CodeVerification = Guid.NewGuid().ToString();
                    service.UpdateContact(contact);

                    var emailList = new Dictionary<string, string>();
                    emailList.Add(contact.Email, contact.CodeVerification);
                   
                    var currentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                    
                    // Recuperer le sujet et le contenu du mail
                    var typeMail = service.GetTypeMail("Reinitialisation");

                    //Get langue
                    var langue = service.GetLangueParCulture(currentCulture);

                    var contenuMail = new ContenuMail();
                    var lien = "";

                    if (langue != null)
                    {
                        contenuMail = service.GetContenuMail(typeMail.Id, langue.Id);
                        lien = "xxServerAddress/" + langue.Code + "/Home/GestionMotDePasse?email=";
                    }
                    else
                    {
                        contenuMail = service.GetContenuMail(typeMail.Id, 1); // Hard-coded
                        lien = "xxServerAddress/" + "fr" + "/Home/GestionMotDePasse?email=";   
                    }

                    // Statut
                    if (!EmailHelper.SendMailModeration(emailList, contenuMail, lien))
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        private bool MajConsommation(decimal coefficientCo2)
        {
            var typeUtilisateur = Session["userType"].ToString();           
            var contactEmail = User.Identity.Name;
            var id = 0;
            if (contactEmail != string.Empty)
            {
                var contact = service.GetContactParEmail(contactEmail);
                switch (typeUtilisateur)
                {
                    case "Payant":
                        return service.MajSimulationClient(contact.Id, coefficientCo2);                       
                    case "Simulation":
                        return service.MajConfigClient(contact.ClientId, coefficientCo2);  
                        break;
                }
            }

            return false;
        }

        private bool RecalculerConsommation(decimal coefficientCo2)
        {
            var resultatCalculateur = (CalculateurViewModel) Session["resultatCalculateur"];
            resultatCalculateur.CoefficientConversion = coefficientCo2;
            if (resultatCalculateur != null)
            {
                if (resultatCalculateur.FamilleDeviceViewModels != null)
                {
                    foreach (var famille in resultatCalculateur.FamilleDeviceViewModels)
                    {
                        foreach (var categorie in famille.CategorieDevices)
                        {
                            foreach (var typeDevice in categorie.TypeDevices)
                            {
                                foreach (var consommation in typeDevice.ConfigSiDevice.ConfigSiConsommations)
                                {
                                    consommation.CoefficientConversion = coefficientCo2;
                                    consommation.ConsommationCo2 = (consommation.ConsommationWatt*coefficientCo2) / 1000;
                                }
                                typeDevice.ConsoCo2 = (typeDevice.ConsoWatt*coefficientCo2) / 1000;
                            }
                            categorie.ConsoCo2 = (categorie.ConsoWatt*coefficientCo2) / 1000;
                        }
                        famille.ConsoCo2 = (famille.ConsoWatt*coefficientCo2) / 1000;
                    }
                    resultatCalculateur.TotalConsoCo2 = (resultatCalculateur.TotalConsoWatt*coefficientCo2) / 1000;

                    service.MajConsommation(resultatCalculateur.ConfigSi, coefficientCo2);                    
                    Session["resultatCalculateur"] = resultatCalculateur;
                    return true;
                }
            }

            return false;
        }

        private bool MajResultat(decimal coefficientCo2)
        {
            var resultatCalculateur = (CalculateurViewModel)Session["resultatCalculateur"];
            if (resultatCalculateur != null)
            {
                if (resultatCalculateur.ConfigSi != null)
                {
                    return service.MajResultat(resultatCalculateur.ConfigSi, coefficientCo2);
                }
            }

            return false;
        }
       
    }
}
