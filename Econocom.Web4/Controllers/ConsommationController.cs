using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Econocom.Calculateur;
using Econocom.Helper.Authorisation;
using Econocom.Model.Enum;
using Econocom.Model.ViewModel;
using Econocom.Resource;
using NLog;
using Omu.ValueInjecter;
using Econocom.Web4.Controllers.ApiControllers;

using Econocom.Model.Models.Benchmark;
using Econocom.Helper;
using Rotativa;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Econocom.Web4.Controllers
{
    public class ConsommationController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string sessionName = "CurrentCategorie";
        private ConsommationApiController service;
        Calculator calculateur = new Calculator();

        public ConsommationController()
        {
            this.InitLanguageDropDown();
            service = new ConsommationApiController();
            ViewBag.consommation = "consommation";           
        }

        
        /// <summary>
        /// Affichage au niveau famille 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            var timeStart = ConsommationHelper.GetStartTime("Consommation index start");
            Logger.Debug("Consommation Index entrée: client id={0} ", id);          
            try
            {
                var detailsFamille = new ConsommationViewModel();
                var resultatCalculateur = new CalculateurViewModel();
                var autorisationHelper = new AuthorisationHelper();
                var client = new Client();
                
                var typeUtilisateur = "";
                var idClient = 0;
                var idContact = 0;
                var contact = new Contact();
                var contactEmail = User.Identity.Name;

                if (id == null && contactEmail != string.Empty)
                {
                    contact = service.GetContactParEmail(contactEmail);
                    id = contact.ClientId;
                }
                else if(id == null && contactEmail == string.Empty)
                {
                    return RedirectToLogin();
                }

                if (Session["userType"] == null && contactEmail == string.Empty)
                {
                    Logger.Debug("Session : usertype => nulle");
                    return RedirectToLogin();
                }
                else
                {
                    //récupérer les valeurs de la session
                    if (Session["userType"] != null)
                    {
                        typeUtilisateur = (string) Session["userType"];
                        Logger.Debug("Session : usertype => {0}", typeUtilisateur);

                        if (Session["idClient"] != null)
                        {
                            idClient = (int) Session["idClient"];
                            Logger.Debug("Session : idClient => {0}", idClient);
                        }
                        if (Session["idContact"] != null)
                        {
                            idContact = (int)Session["idContact"];
                            Logger.Debug("Session : idContact => {0}", idContact);
                        }
                    }
                    else
                    {
                        Logger.Debug("Reinitialiser session : usertype à Payant");
                        typeUtilisateur = "Payant";
                        Session.Add("typeUtilisateur", typeUtilisateur);
                    }

                    if (!typeUtilisateur.Equals("Demo"))
                    {
                        contact = service.GetContactParEmail(contactEmail);
                        if (contact == null)
                            return RedirectToLogin();

                         Logger.Debug("Authenticated User email: ", contact.Email);

                        if (Session["idClient"] == null || Session["idContact"] == null)
                        {
                            Logger.Debug("Session : idClient, idContact => nulle");
                           
                            idClient = contact.ClientId;
                            Session.Add("idClient", contact.ClientId);
                            Logger.Debug("Authenticated User, idClient => {0}", idClient);

                            idContact = contact.Id;
                            Session.Add("idContact", contact.Id);
                            Logger.Debug("Authenticated User, idContact => {0}", idContact);
                        }

                        //Récupérer un client par son id
                        client = service.GetClientParId((int)id);
                        if (client != null)
                        {
                            Logger.Info("Client récupéré : {0}", client.Id);
                            Session.Add("typeClient", client.TypeClient == (int)Econocom.Model.Enum.TypeClient.Econocom ? Econocom.Model.Enum.TypeClient.Econocom : Econocom.Model.Enum.TypeClient.ECSchange);

                            var codeInterne = client.DetailsClient.CodeInterneClient;
                            var numeroContrat = codeInterne;
                            if (codeInterne != null && codeInterne.Length >= 10)
                                numeroContrat = codeInterne.Substring(codeInterne.Length - 10);
                            Session.Add("numeroContrat", numeroContrat);
                        }
                        else
                        {
                            Logger.Info("Client nulle");
                        }
                        
                        //vérifier si utilisateur est un admin ou non
                        if (autorisationHelper.EstAdministrateur(contact) || autorisationHelper.EstSouscripteurAdmin(contact))
                        {
                            ViewBag.TypeContact = "Admin";
                            Session.Add("typeContact", "Admin");
                            Logger.Debug("TypeContact => Admin");
                        }
                        else
                        {
                            ViewBag.TypeContact = "Utilisateur";
                            Session.Add("typeContact", "Utilisateur");
                            Logger.Debug("TypeContact => Utilisateur");
                        }
                    }
                }

                var layout = "";

                //vérifier l'idClient de l'url est modifié
                if (idClient != id)
                {
                    Logger.Debug("Essayer d'accéder un autre compte que le votre => Redirection page d'accueil");
                    return RedirectToRoute(new {controller = "Home", action = "Index"});
                }
                else
                {
                    Logger.Debug("Type Utilisateur => {0}", typeUtilisateur);
                    switch (typeUtilisateur)
                    {
                        case "Demo":
                            {
                                //Calculer les différentes consommations pour un client Demo
                                resultatCalculateur = service.GetResultatCalculateur((int)id, "Demo");
                                if (resultatCalculateur != null)
                                    Logger.Debug("ResultatCalculateur récupéré pour User Demo {0}", id);

                                //construire modèle pour affichage
                                detailsFamille = service.GetDetailsFamille(resultatCalculateur, (int)id, "Demo");
                                if (detailsFamille != null)
                                    Logger.Debug("DetailsFamille récupéré pour User Demo {0}", id);
                               
                                ViewBag.UserType = "Demo";
                                Session.Add("userType", "Demo");
                                layout = "~/Views/Shared/Maquette/_Layout_Evaluation.cshtml";
                            }
                            break;
                        case "Payant":
                            {
                                //Calculer les différentes consommations pour un client payant
                                var startTime = ConsommationHelper.GetStartTime("Consommationcontroller, service.GetResultatCalculateur");
                                resultatCalculateur = service.GetResultatCalculateur((int)id, "Payant");
                                ConsommationHelper.GetTimeDifference(startTime,"Consommationcontroller, service.GetResultatCalculateur");                                

                                if (resultatCalculateur != null)                                   
                                {
                                    //Pour client existant
                                    if (resultatCalculateur.TotalConsoCo2 == null || resultatCalculateur.TotalConsoWatt == null)
                                    {
                                        var startCalculerTotalConsommation = ConsommationHelper.GetStartTime("Consommationcontroller, CalculerTotalConsommation");
                                        resultatCalculateur = service.CalculerTotalConsommation(resultatCalculateur);
                                        ConsommationHelper.GetTimeDifference(startCalculerTotalConsommation, "Consommationcontroller, CalculerTotalConsommation");
                                    }

                                    //Construire modèle pour affichage
                                    Logger.Debug("Consommationcontroller, GetDetailsFamille start");
                                    var startGetDetailsFamille = ConsommationHelper.GetStartTime("Consommationcontroller, GetDetailsFamille");
                                    detailsFamille = service.GetDetailsFamille(resultatCalculateur, (int) id, "Payant");
                                    ConsommationHelper.GetTimeDifference(startGetDetailsFamille,"Consommationcontroller, GetDetailsFamille");                                    

                                    //Vérification pour affichage bouton 'Généner Rapport'
                                    Logger.Debug("Consommationcontroller, GetConfigSiClient start");
                                    var startGetConfigSiClient = ConsommationHelper.GetStartTime("Consommationcontroller, GetConfigSiClient");
                                    var configClient = resultatCalculateur.ConfigSiClient;//service.GetConfigSiClient(idClient);
                                    if (configClient != null)
                                    {
                                        var configSi = configClient.ConfigSi;
                                        Session.Add("genererRapport", configSi.Statut == (int) ConfigSiStatut.Modifier ? "Vrai" : "Faux");
                                    }
                                    else
                                    {
                                        Logger.Debug("configClient nulle =>  genererRapport : Faux");
                                        Session.Add("genererRapport", "Faux");
                                    }
                                    ConsommationHelper.GetTimeDifference(startGetConfigSiClient, "Consommationcontroller, GetConfigSiClient");   
                                    
                                    ViewBag.UserType = "Payant";
                                    Session.Add("userType", "Payant");
                                    layout = "~/Views/Shared/Maquette/_Layout_Client.cshtml";
                                }
                            }
                            break;
                        case "Simulation":
                            {
                                resultatCalculateur = service.GetDetailsConfigPourClientExistant(client, "Simulation", idContact);
                                if (resultatCalculateur != null)
                                {
                                    Logger.Debug("ResultatCalculateur récupéré pour client existant ");

                                    if (resultatCalculateur.TotalConsoCo2 == null ||
                                        resultatCalculateur.TotalConsoWatt == null)
                                    {
                                        Logger.Debug("TotalConso null => Recalculation pour client existant ");
                                        resultatCalculateur = service.CalculerTotalConsommation(resultatCalculateur);
                                    }

                                    //Construire modèle pour affichage
                                    detailsFamille = service.GetDetailsFamille(resultatCalculateur, (int)id,"Simulation");
                                    if (detailsFamille != null)
                                        Logger.Debug("DetailsFamille récupéré pour client");
                                }

                                //Vérification pour affichage bouton 'Généner Rapport'
                                var simulation = service.GetSimulationEnCours(idContact);
                                if (simulation != null)
                                {
                                    var configSi = service.GetConfigSi(simulation.ConfigSiSimuleId.Value);
                                    Session.Add("genererRapport", configSi.Statut == (int) ConfigSiStatut.Modifier ? "Vrai" : "Faux");
                                    Session.Add("NomSimulation", simulation.LibelleSimulation);
                                    SetSimulationViewBag();
                                }
                                else
                                {
                                    Logger.Debug("simulation nulle =>  genererRapport : Faux");
                                    Session.Add("genererRapport", "Faux");
                                }

                                ViewBag.UserType = "Simulation";
                                Session.Add("userType", "Simulation");
                                layout = "~/Views/Shared/Maquette/_Layout_Simulation.cshtml";
                            }
                            break;
                    }

                    //Ajouter des valeurs dans la session
                    Session.Add("resultatCalculateur", resultatCalculateur);
                    Session.Add("detailsFamille", detailsFamille);
                    Session.Add("idClient", id);
                    Session.Add("layout", layout);

                    //Retirer les valeurs pour modification qty device de la session
                    //Session.Remove(sessionName);

                    ViewBag.GenererRapport = Session["genererRapport"];
                    ViewBag.HeureModifier = detailsFamille.HeureModifier;
                    ViewBag.TypeObjet = "Famille";
                    ConsommationHelper.GetTimeDifference(timeStart,"consommation index end");
                    return View("index", layout, detailsFamille);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        
        /// <summary>
        /// Redirection dans l'espace Ma Personnalisation de l'espace Ma Simulation
        /// </summary>
        /// <returns></returns>
        public ActionResult MaPersonnalisation()
        {
            Logger.Debug("MaPersonnalisation entrée: -");
            try
            {
                var typeUtilisateur = "";
                var idClient = 0;

                if (Session["userType"] != null && Session["idClient"] != null)
                {
                    typeUtilisateur = (string) Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    idClient = (int) Session["idClient"];
                    Logger.Debug("Session : idClient => {0}", idClient);
                }
                else
                {
                    Logger.Debug("Session : userType & idClient : nulle => Redirection login");
                    return RedirectToLogin();
                }

                //MAJ de la valeur de 'usertype' a payant quand on clique sur "Ma Personnalisation"
                if (typeUtilisateur.Equals("Simulation"))
                {
                    //Retirer les valeurs pour modification qty device de la session
                    Session.Remove(sessionName);

                    Session.Add("userType", "Payant");
                }

                Logger.Debug("MaPersonnalisation : end");
                return RedirectToAction("Index", "Consommation", new {id = idClient});
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }
        
        /// <summary>
        /// Affichage page categorie ou typeDevice
        /// </summary>
        /// <param name="idClient"></param>
        /// <param name="idFamille"></param>
        /// <param name="idCategorie"></param>
        /// <returns></returns>
        public ActionResult Naviguer(int idClient, int? idFamille,int? idCategorie)
        {
            Logger.Debug("Naviguer entrée: idClient={0}, idFamille={1}, idCategorie={2}", idClient, idFamille, idCategorie);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                SetSimulationViewBag();
                //récupérer les valeurs de la session
                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();
                var layout = "";

                if (Session["userType"] != null && Session["detailsFamille"] != null && Session["layout"] != null)
                {
                    typeUtilisateur = (string) Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");

                    layout = Session["layout"].ToString();
                    Logger.Debug("Session : layout => {0}", layout);
                }
                else
                {
                    Logger.Debug("Session : userType & detailsFamille & layout : nulle => Redirection Consommation/Index");
                    return RedirectToAction("Index", "Consommation", new { id = idClient });
                }
                
                var detailConsommations = (IEnumerable<DetailConsommationViewModel>) detailsFamille.DetailConsommations;

                ViewBag.detailConsommations = detailConsommations;
                ViewBag.GenererRapport = Session["genererRapport"];
                ViewBag.TypeContact = Session["typeContact"];
                ViewBag.UserType = typeUtilisateur;
                ViewBag.TypeClient = Session["typeClient"];
                ViewBag.NumeroContrat = Session["numeroContrat"];

                if (idClient != null)
                {
                    ViewBag.IdClient = idClient;
                    if (idFamille != null) //naviguer à page categorie
                    {
                        Logger.Debug("Naviguer à la page catégorie");
                        var detailsCategorie = new ConsommationViewModel();

                        foreach (var detailConsommation in detailConsommations)
                        {
                            if (detailConsommation.IdFamille == idFamille.ToString())
                            {
                                detailsCategorie = detailConsommation.SubObjet;
                                ViewBag.CurrentFamilleId = detailConsommation.IdFamille;
                                Session.Add("currentFamilleId", detailConsommation.IdFamille);
                                Logger.Debug("Session : currentFamilleId => {0}", detailConsommation.IdFamille);
                            }
                        }

                        ViewBag.DateModifier = detailsCategorie.DateModifier;
                        ViewBag.HeureModifier = detailsCategorie.HeureModifier;
                        ViewBag.IdParent = idFamille;
                        ViewBag.IdFamille = idFamille;
                        ViewBag.TypeObjet = "Categorie";

                        if (Session["typeAffichage"] == null)
                        {
                            Logger.Debug("Session : typeAffichage => nulle, reinitialiser à ShowWatt");
                            Session.Add("typeAffichage", "ShowWatt");   
                        }
                        ViewBag.TypeAffichage = Session["typeAffichage"];

                        return View("Categorie", layout, detailsCategorie);
                    }
                    else if (idCategorie != null) //naviguer a page device
                    {
                        Logger.Debug("Naviguer à la page device");

                        var resultatCalculateur = (CalculateurViewModel) Session["resultatCalculateur"];
                        var detailsDevice = service.GetDetailsDevice(resultatCalculateur, idClient.ToString(),
                                                                     idCategorie.ToString(), typeUtilisateur.ToString());

                        var categorie = service.GetCategorieDevice(Convert.ToInt32(idCategorie));

                        if (typeUtilisateur.Equals("Demo"))
                        {
                            ViewBag.CurrentCategorieImage = "~/Content/images/Consommation/Demo/Categorie/" +
                                                            idCategorie + ".png";
                            Session.Add("currentCategorieImage",
                                        "~/Content/images/Consommation/Demo/Categorie/" + idCategorie + ".png");
                        }
                        else if (typeUtilisateur.Equals("Payant"))
                        {
                            ViewBag.CurrentCategorieImage = "~/Content/images/Consommation/Payant/Categorie/" +
                                                            idCategorie + ".png";
                            Session.Add("currentCategorieImage",
                                        "~/Content/images/Consommation/Payant/Categorie/" + idCategorie + ".png");
                        }
                        else if (typeUtilisateur.Equals("Simulation"))
                        {
                            ViewBag.CurrentCategorieImage = "~/Content/images/Consommation/Simulation/Categorie/" +
                                                            idCategorie + ".png";
                            Session.Add("currentCategorieImage",
                                        "~/Content/images/Consommation/Simulation/Categorie/" + idCategorie + ".png");
                        }

                        ViewBag.CurrentCategorieId = idCategorie;
                        Session.Add("currentCategorieId", idCategorie);
                        Logger.Debug("Session : currentCategorieId => {0}", idCategorie);

                        ViewBag.IdFamille = categorie.FamilleDeviceId;
                        Session.Add("idFamille", categorie.FamilleDeviceId);
                        Logger.Debug("Session : idFamille => {0}", categorie.FamilleDeviceId);

                        ViewBag.DateModifier = detailsDevice.DateModifier;
                        ViewBag.HeureModifier = detailsDevice.HeureModifier;
                        ViewBag.IdParent = idCategorie;
                        ViewBag.TypeObjet = "Device";

                        if (Session["typeAffichage"] == null)
                        {
                            Logger.Debug("Session : typeAffichage => nulle, reinitialiser à ShowWatt");
                            Session.Add("typeAffichage", "ShowWatt");
                        }
                        ViewBag.TypeAffichage = Session["typeAffichage"];

                        return View("TypeDevice", layout, detailsDevice);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Consommation", new { id = idClient });
                    }
                    
                    Logger.Debug("Naviguer : End");
                }
                
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return base.Erreur();
        }

        /// <summary>
        /// Menu vertical au niveau categorie - render partial view categorie
        /// </summary>
        /// <param name="idFamille"></param>
        /// <param name="idClient"></param>
        /// <returns></returns>
        public ActionResult ProfilEnDetail(int idFamille, int idClient)
        {
            Logger.Debug("ProfilEnDetail entrée: idFamille={0}, idClient={1} ", idFamille, idFamille);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                SetSimulationViewBag();

                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();

                if (Session["userType"] != null && Session["detailsFamille"] != null)
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");
                }
                else
                {
                    Logger.Debug("Session : userType & detailsFamille : nulle => Redirection Consommation/Index");
                    return RedirectToAction("Index", "Consommation", new { id = idClient });
                }

                var detailConsommations = (IEnumerable<DetailConsommationViewModel>) detailsFamille.DetailConsommations;

                ViewBag.detailConsommations = detailConsommations;
                ViewBag.UserType = typeUtilisateur;
                ViewBag.IdParent = idFamille;
                ViewBag.DateModifier = detailsFamille.DateModifier;
                ViewBag.HeureModifier = detailsFamille.HeureModifier;
                ViewBag.GenererRapport = Session["genererRapport"];
                ViewBag.TypeContact = Session["typeContact"];
                ViewBag.TypeClient = Session["typeClient"];
                ViewBag.NumeroContrat = Session["numeroContrat"];

                if (Session["typeAffichage"] == null)
                {
                    Session.Add("typeAffichage", "ShowWatt");
                }
                var typeAffichage = Session["typeAffichage"];
                ViewBag.TypeAffichage = typeAffichage;

                var detailsCategorie = new ConsommationViewModel();

                foreach (var detailConsommation in detailConsommations)
                {
                    if (detailConsommation.IdFamille == idFamille.ToString())
                    {
                        detailsCategorie = detailConsommation.SubObjet;
                        ViewBag.CurrentFamilleId = detailConsommation.IdFamille;
                        Session.Add("currentFamilleId", detailConsommation.IdFamille);
                    }
                }

                var familleDevice = service.GetFamilleParId(idFamille);
                if (familleDevice != null)
                {
                    if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                    }
                    else
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.LibelleFamilleDevice;
                    }
                }

                var detaileObjet = detailsCategorie;

                var consoModels = new List<ConsoViewModel>();
                foreach (DetailConsommationViewModel detailConsommationViewModel in detaileObjet.DetailConsommations)
                {
                    var consoModel = new ConsoViewModel();
                    consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                    consoModel.IdClient = detailConsommationViewModel.IdClient;
                    consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                    consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                    consoModel.LienImage = detailConsommationViewModel.LienImage;

                    if (typeAffichage.Equals("ShowWatt"))
                    {
                        consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                        consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;
                        consoModel.Consommation = detailConsommationViewModel.ConsoWatt;
                        ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_kwh.png";
                        ViewBag.TotalConsoEnergetique = detaileObjet.TotalConsoWatt.Valeur + " " +detaileObjet.TotalConsoWatt.Measure;
                    }
                    else if (typeAffichage.Equals("ShowCO2"))
                    {
                        consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheCO2Totale;
                        consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheCO2Milieu;
                        consoModel.Consommation = detailConsommationViewModel.ConsoCo2;
                        ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_CO2.png";
                        ViewBag.TotalConsoEnergetique = detaileObjet.TotalConsoCo2.Valeur + " " +
                                                        detaileObjet.TotalConsoCo2.Measure;
                    }
                    else
                    {
                        consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                        consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;
                        
                        var devise = typeUtilisateur.Equals("Demo") ? service.GetCompteEvaluation(idClient).Devise : service.GetClientParId(idClient).DetailsClient.Devise;

                        var codeDevise = devise != null ? devise.CodeDevise : " ";
                        var conso = new Consommation(null, detailConsommationViewModel.Prix.Valeur, codeDevise);
                        consoModel.Consommation = conso;

                        ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_cout.png";

                        ViewBag.TotalConsoEnergetique = detaileObjet.TotalPrix.Valeur + " " + codeDevise;
                    
                    }
                    consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                    consoModel.Libeller = detailConsommationViewModel.Libeller;
                    consoModels.Add(consoModel);
                }
                ViewBag.IdParent = idFamille;
                ViewBag.TypeObjet = "Categorie";

                Logger.Debug("ProfilEnDetail : End");

                return PartialView("MenuPartial/_ConsommationDetailsPartial", consoModels);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return base.Erreur();
        }

        /// <summary>
        /// Menu vertical au niveau typeDevice - render partial view typeDevice
        /// </summary>
        /// <param name="idClient"></param>
        /// <param name="idCategorie"></param>
        /// <returns></returns>
        public ActionResult TypeDevice(int idClient, int idCategorie)
        {
            Logger.Debug("TypeDevice entrée: idClient={0}, idCategorie={1} ", idClient, idCategorie);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {               
                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();
                var resultatCalculateur = new CalculateurViewModel();

                if (Session["userType"] != null && Session["resultatCalculateur"] != null && Session["detailsFamille"] != null )
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    resultatCalculateur = (CalculateurViewModel)Session["resultatCalculateur"];
                    Logger.Debug("Session : resultatCalculateur pas nulle");

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");
                }
                else
                {
                    Logger.Debug("Session : userType, resultatCalculateur, detailsFamille : nulle => Redirection Consommation/Index");
                    return RedirectToAction("Index", "Consommation", new { id = idClient });
                }

                var detailsDevice = service.GetDetailsDevice(resultatCalculateur, idClient.ToString(),idCategorie.ToString(), typeUtilisateur.ToString());
                var detailConsommations = (IEnumerable<DetailConsommationViewModel>) detailsFamille.DetailConsommations;
                
                ViewBag.detailConsommations = detailConsommations;
                ViewBag.UserType = typeUtilisateur;
                ViewBag.IdParent = idCategorie;
                ViewBag.DateModifier = detailsDevice.DateModifier;
                ViewBag.HeureModifier = detailsDevice.HeureModifier;
                ViewBag.TypeObjet = "Device";
                ViewBag.GenererRapport = Session["genererRapport"];
                ViewBag.TypeContact = Session["typeContact"];
                ViewBag.TypeClient = Session["typeClient"];
                ViewBag.NumeroContrat = Session["numeroContrat"];
                
                if (Session["typeAffichage"] == null)
                {
                    Session.Add("typeAffichage", "ShowWatt");
                }
                ViewBag.TypeAffichage = Session["typeAffichage"];
                
                var categorie = service.GetCategorieDevice(Convert.ToInt32(idCategorie));
                
                if (typeUtilisateur.Equals("Demo"))
                {
                    ViewBag.CurrentCategorieImage = "~/Content/images/Consommation/Demo/Categorie/" + idCategorie + ".png";
                    Session.Add("currentCategorieImage", "~/Content/images/Consommation/Demo/Categorie/" + idCategorie + ".png");
                }
                else if (typeUtilisateur.Equals("Payant"))
                {
                    ViewBag.CurrentCategorieImage = "~/Content/images/Consommation/Payant/Categorie/" + idCategorie + ".png";
                    Session.Add("currentCategorieImage", "~/Content/images/Consommation/Payant/Categorie/" + idCategorie + ".png");
                }
                else if (typeUtilisateur.Equals("Simulation"))
                {
                    SetSimulationViewBag();
                    ViewBag.CurrentCategorieImage = "~/Content/images/Consommation/Simulation/Categorie/" + idCategorie + ".png";
                    Session.Add("currentCategorieImage", "~/Content/images/Consommation/Simulation/Categorie/" + idCategorie + ".png");
                }

                
                ViewBag.CurrentFamilleId = categorie.FamilleDeviceId;
                
                var familleDevice = service.GetFamilleParId(categorie.FamilleDeviceId.Value);
                if (familleDevice != null)
                {
                    if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                    }
                    else
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.LibelleFamilleDevice;
                    }
                }

                ViewBag.CurrentCategorieId = idCategorie;

                var categorieDevice = service.GetCategorieParId(idCategorie);
                if (categorieDevice != null)
                {
                    if (categorieDevice.CategorieDeviceTraductions != null && categorieDevice.CategorieDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentCategorieLibelle = categorieDevice.CategorieDeviceTraductions.ElementAt(0).LibelleCategorieDevice;
                    }
                    else
                    {
                        ViewBag.CurrentCategorieLibelle = categorieDevice.LibelleCategorieDevice;
                    }
                }

                ViewBag.IdFamille = categorie.FamilleDeviceId;
                
                Session.Add("currentCategorieId", idCategorie);
                Session.Add("currentFamilleId", categorie.FamilleDeviceId);
                var typeAffichage = Session["typeAffichage"].ToString();

                if (typeAffichage.Equals("ShowWatt"))
                {
                    detailsDevice.DetailConsommations =
                        detailsDevice.DetailConsommations.Where(o => Convert.ToDecimal(o.ConsoWatt.Valeur) > 0)
                                     .OrderByDescending(o => Convert.ToDecimal(o.ConsoWatt.Valeur))
                                     .ToList();
                }
                else if (typeAffichage.Equals("ShowCO2"))
                {
                    detailsDevice.DetailConsommations =
                        detailsDevice.DetailConsommations.Where(o => Convert.ToDecimal(o.ConsoCo2.Valeur) > 0)
                                     .OrderByDescending(o => Convert.ToDecimal(o.ConsoCo2.Valeur))
                                     .ToList();
                }
                else
                {
                    detailsDevice.DetailConsommations =
                        detailsDevice.DetailConsommations.Where(o => Convert.ToDecimal(o.Prix.Valeur) > 0)
                                     .OrderByDescending(o => Convert.ToDecimal(o.Prix.Valeur))
                                     .ToList();
                }
                
                var devise = typeUtilisateur.Equals("Demo") ? service.GetCompteEvaluation(idClient).Devise : service.GetClientParId(idClient).DetailsClient.Devise;

                var consoModels = new List<ConsoViewModel>();
                foreach (DetailConsommationViewModel detailConsommationViewModel in detailsDevice.DetailConsommations)
                {
                    var consoModel = new ConsoViewModel();
                    consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                    consoModel.IdClient = detailConsommationViewModel.IdClient;
                    consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                    consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                    consoModel.LienImage = detailConsommationViewModel.LienImage;

                    if (typeAffichage.Equals("ShowWatt"))
                    {
                        consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                        consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;
                        consoModel.Consommation = detailConsommationViewModel.ConsoWatt;
                        ViewBag.TotalConsoEnergetique = detailsDevice.TotalConsoWatt.Valeur + " " + detailsDevice.TotalConsoWatt.Measure;
                    }
                    else if (typeAffichage.Equals("ShowCO2"))
                    {
                        consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheCO2Totale;
                        consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheCO2Milieu;
                        consoModel.Consommation = detailConsommationViewModel.ConsoCo2;
                        ViewBag.TotalConsoEnergetique = detailsDevice.TotalConsoCo2.Valeur + " " + detailsDevice.TotalConsoCo2.Measure;
                    }
                    else
                    {
                        consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                        consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;

                        var codeDevise = devise != null ? devise.CodeDevise : " ";
                        var conso = new Consommation(null, detailConsommationViewModel.Prix.Valeur, codeDevise);
                        consoModel.Consommation = conso;

                        ViewBag.TotalConsoEnergetique = detailsDevice.TotalPrix.Valeur + " " + codeDevise;
                       
                    }
                    consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                    consoModel.Libeller = detailConsommationViewModel.Libeller;
                    consoModel.Source = detailConsommationViewModel.Source;
                    consoModels.Add(consoModel);
                }

                if (detailsDevice.DetailConsommations.Count == 0)
                {
                    if (typeAffichage.Equals("ShowWatt"))
                    {
                        ViewBag.TotalConsoEnergetique = "0 kWh";
                    }
                    else if (typeAffichage.Equals("ShowCO2"))
                    {
                        ViewBag.TotalConsoEnergetique = "0 teq-CO2";
                    }
                    else
                    {
                        var codeDevise = devise != null ? devise.CodeDevise : " ";
                        ViewBag.TotalConsoEnergetique = "0 " + codeDevise;
                    }
                }

                Logger.Debug("TypeDevice : End");
                return PartialView("MenuPartial/_DeviceDetailsPartial", consoModels);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }
    
        /// <summary>
        /// Affichage de toutes les quantités en kWh
        /// </summary>
        /// <param name="typeObjet"></param>
        /// <param name="idParent"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0)] 
        public ActionResult ShowWatt(string typeObjet, string idParent)
        {
            Logger.Debug("ShowWatt entrée: typeObjet={0}, idParent={1}", typeObjet, idParent);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                ConsommationViewModel detaileObjet;

                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();
                
                if (Session["userType"] != null && Session["detailsFamille"] != null)
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");
                }
                else
                {
                    Logger.Debug("Session : userType & detailsFamille : nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        var idClient = contact.ClientId;
                        return RedirectToAction("Index", "Consommation", new {id = idClient});
                    }
                    return RedirectToLogin();
                }

                var genererRapport = Session["genererRapport"];
                var typeContact = Session["typeContact"];

                var detailConsommations = (IEnumerable<DetailConsommationViewModel>) detailsFamille.DetailConsommations;
                ViewBag.detailConsommations = detailConsommations;

                ViewBag.UserType = typeUtilisateur;
                ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_kwh.png";
                ViewBag.TypeAffichage = "ShowWatt";
                Session.Add("typeAffichage", "ShowWatt");
                ViewBag.GenererRapport = genererRapport;
                ViewBag.TypeContact = typeContact;               
                SetSimulationViewBag();
                ViewBag.TypeClient = Session["typeClient"];
                ViewBag.NumeroContrat = Session["numeroContrat"];

                var idFamille = Session["currentFamilleId"] != null ? int.Parse(Session["currentFamilleId"].ToString()) : 1;
                var familleDevice = service.GetFamilleParId(idFamille);
                if (familleDevice != null)
                {
                    if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                    }
                    else
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.LibelleFamilleDevice;
                    }
                }

                switch (typeObjet)
                {
                    case ("Famille"):
                        detaileObjet = detailsFamille;
                        ViewBag.TypeObjet = "Famille";
                        ViewBag.DateModifier = detailsFamille.DateModifier;
                        ViewBag.HeureModifier = detailsFamille.HeureModifier;
                        ViewBag.TotalConsoEnergetique = detailsFamille.TotalConsoWatt.Valeur + " " + detailsFamille.TotalConsoWatt.Measure;
                        ViewBag.WikiLink = ConfigurationManager.AppSettings["WikiHome"];
                        break;

                    case ("Categorie"):
                        ViewBag.IdParent = idParent;
                        ViewBag.TypeObjet = "Categorie";
                        ViewBag.CurrentFamilleId = Session["currentFamilleId"] ?? 1;

                        var detailsCategorie = new ConsommationViewModel();

                        foreach (var detailConsommation in detailConsommations)
                        {
                            if (detailConsommation.IdFamille == idParent.ToString())
                            {
                                detailsCategorie = detailConsommation.SubObjet;
                            }
                        }
                        detaileObjet = detailsCategorie;
                        ViewBag.DateModifier = detaileObjet.DateModifier;
                        ViewBag.HeureModifier = detaileObjet.HeureModifier;
                        ViewBag.TotalConsoEnergetique = detaileObjet.TotalConsoWatt.Valeur + " " + detaileObjet.TotalConsoWatt.Measure;
                        break;

                    case ("Device"):
                        ViewBag.TypeObjet = "Device";
                        var idClient = Session["idClient"];
                        ViewBag.IdParent = idParent;

                        var resultatCalculateur = new CalculateurViewModel();
                        if (Session["resultatCalculateur"] != null)
                        {
                            resultatCalculateur = (CalculateurViewModel) Session["resultatCalculateur"];
                            Logger.Debug("Session : resultatCalculateur pas nulle");
                        }
                        else
                        {
                            Logger.Debug("Session : resultatCalculateur : nulle => Redirection Consommation/Index");
                            return RedirectToAction("Index", "Consommation", new { id = idClient });
                        }
                        var detailsDevice = service.GetDetailsDevice(resultatCalculateur, idClient.ToString(),idParent.ToString(), typeUtilisateur.ToString());
                        
                        detaileObjet = detailsDevice;
                        detaileObjet.DetailConsommations = detaileObjet.DetailConsommations.Where(o => Convert.ToDecimal(o.ConsoWatt.Valeur) > 0).OrderByDescending(o => Convert.ToDecimal(o.ConsoWatt.Valeur)).ToList();
                        
                        ViewBag.DateModifier = detaileObjet.DateModifier;
                        ViewBag.HeureModifier = detaileObjet.HeureModifier;
                        
                        ViewBag.CurrentCategorieId = Session["currentCategorieId"] ?? 1;

                        var idCategorie = Session["currentCategorieId"] != null ? int.Parse(Session["currentCategorieId"].ToString()) : 1;
                        var categorieDevice = service.GetCategorieParId(idCategorie);
                        if (categorieDevice != null)
                        {
                            if (categorieDevice.CategorieDeviceTraductions != null && categorieDevice.CategorieDeviceTraductions.ElementAt(0) != null)
                            {
                                ViewBag.CurrentCategorieLibelle = categorieDevice.CategorieDeviceTraductions.ElementAt(0).LibelleCategorieDevice;
                            }
                            else
                            {
                                ViewBag.CurrentCategorieLibelle = categorieDevice.LibelleCategorieDevice;
                            }
                        }

                        ViewBag.CurrentCategorieImage = Session["currentCategorieImage"] ?? "~/Content/images/Consommation/" + typeUtilisateur + "/Categorie/1.png";
                        ViewBag.CurrentFamilleId = Session["currentFamilleId"] ?? 1;
                        ViewBag.IdFamille = Session["idFamille"] ?? 1;

                        if (detaileObjet.TotalConsoWatt != null)
                        {
                            ViewBag.TotalConsoEnergetique = detaileObjet.TotalConsoWatt.Valeur + " " +detaileObjet.TotalConsoWatt.Measure;
                        }
                        else
                        {
                            ViewBag.TotalConsoEnergetique = "0 kWh";
                        }
                        break;

                    default:
                        detaileObjet = new ConsommationViewModel();
                        break;
                }

                var consoModels = new List<ConsoViewModel>();
                if (detaileObjet.DetailConsommations!=null)
                {
                    foreach (DetailConsommationViewModel detailConsommationViewModel in detaileObjet.DetailConsommations)
                    {
                        var consoModel = new ConsoViewModel();
                        consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                        consoModel.IdClient = detailConsommationViewModel.IdClient;
                        consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                        consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                        consoModel.LienImage = detailConsommationViewModel.LienImage;
                        consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                        consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;
                        consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                        consoModel.Libeller = detailConsommationViewModel.Libeller;
                        consoModel.Consommation = detailConsommationViewModel.ConsoWatt;
                        consoModel.Source = detailConsommationViewModel.Source;
                        consoModels.Add(consoModel);
                    }
                }
                Logger.Debug("ShowWatt : End");

                if (typeObjet.Equals("Device"))
                    return PartialView("MenuPartial/_DeviceDetailsPartial", consoModels);

                ViewBag.TypeObjet = typeObjet;
                return PartialView("MenuPartial/_ConsommationDetailsPartial", consoModels);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }


        /// <summary>
        /// Affichage de toutes les quantités en teq-CO2
        /// </summary>
        /// <param name="typeObjet"></param>
        /// <param name="idParent"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0)] 
        public ActionResult ShowCO2(string typeObjet, string idParent)
        {
            Logger.Debug("ShowCO2 entrée: typeObjet={0}, idParent={1}", typeObjet, idParent);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                ConsommationViewModel detaileObjet;

                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();

                SetSimulationViewBag();
                if (Session["userType"] != null && Session["detailsFamille"] != null)
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");
                }
                else
                {
                    Logger.Debug("Session : userType & detailsFamille : nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        var idClient = contact.ClientId;
                        return RedirectToAction("Index", "Consommation", new { id = idClient });
                    }
                    return RedirectToLogin();
                }

                var genererRapport = Session["genererRapport"];
                var typeContact = Session["typeContact"];

                var detailConsommations = (IEnumerable<DetailConsommationViewModel>) detailsFamille.DetailConsommations;
                ViewBag.detailConsommations = detailConsommations;

                ViewBag.UserType = typeUtilisateur;
                ViewBag.DateModifier = detailsFamille.DateModifier;
                ViewBag.HeureModifier = detailsFamille.HeureModifier;
                ViewBag.TotalConsoEnergetique = detailsFamille.TotalConsoCo2.Valeur + " " + detailsFamille.TotalConsoCo2.Measure;
                ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_CO2.png";
                ViewBag.TypeAffichage = "ShowCO2";
                Session.Add("typeAffichage", "ShowCO2");
                ViewBag.GenererRapport = genererRapport;
                ViewBag.TypeContact = typeContact;
                ViewBag.TypeClient = Session["typeClient"];
                ViewBag.NumeroContrat = Session["numeroContrat"];

                var idFamille = Session["currentFamilleId"] != null ? int.Parse(Session["currentFamilleId"].ToString()) : 1;
                var familleDevice = service.GetFamilleParId(idFamille);
                if (familleDevice != null)
                {
                    if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                    }
                    else
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.LibelleFamilleDevice;
                    }
                }

                switch (typeObjet)
                {
                    case ("Famille"):
                        detaileObjet = detailsFamille;
                        break;

                    case ("Categorie"):
                        ViewBag.IdParent = idParent;
                        ViewBag.TypeObjet = "Categorie";
                        ViewBag.CurrentFamilleId = Session["currentFamilleId"] ?? 1;                       
                        var detailsCategorie = new ConsommationViewModel();

                        foreach (var detailConsommation in detailConsommations)
                        {
                            if (detailConsommation.IdFamille == idParent.ToString())
                            {
                                detailsCategorie = detailConsommation.SubObjet;
                            }
                        }
                        detaileObjet = detailsCategorie;
                        ViewBag.DateModifier = detaileObjet.DateModifier;
                        ViewBag.HeureModifier = detaileObjet.HeureModifier;
                        ViewBag.TotalConsoEnergetique = detaileObjet.TotalConsoCo2.Valeur + " " + detaileObjet.TotalConsoCo2.Measure;
                        break;
                        
                    case ("Device"):
                        ViewBag.TypeObjet = "Device";
                        ViewBag.IdParent = idParent;
                        var idClient = Session["idClient"];

                        var resultatCalculateur = new CalculateurViewModel();
                        if (Session["resultatCalculateur"] != null)
                        {
                            resultatCalculateur = (CalculateurViewModel)Session["resultatCalculateur"];
                            Logger.Debug("Session : resultatCalculateur pas nulle");
                        }
                        else
                        {
                            Logger.Debug("Session : resultatCalculateur : nulle => Redirection Consommation/Index");
                            return RedirectToAction("Index", "Consommation", new { id = idClient });
                        }
                        var detailsDevice = service.GetDetailsDevice(resultatCalculateur, idClient.ToString(), idParent.ToString(), typeUtilisateur.ToString());
                       
                        detaileObjet = detailsDevice;
                        detaileObjet.DetailConsommations = detaileObjet.DetailConsommations.Where(o => Convert.ToDecimal(o.ConsoCo2.Valeur) > 0).OrderByDescending(o => Convert.ToDecimal(o.ConsoCo2.Valeur)).ToList();

                        ViewBag.DateModifier = detaileObjet.DateModifier;
                        ViewBag.HeureModifier = detaileObjet.HeureModifier;
                        ViewBag.CurrentCategorieImage = Session["currentCategorieImage"] ?? "~/Content/images/Consommation/" + typeUtilisateur + "/Categorie/1.png";
                        ViewBag.CurrentCategorieId = Session["currentCategorieId"] ?? 1;
                        
                        var idCategorie = Session["currentCategorieId"] != null ? int.Parse(Session["currentCategorieId"].ToString()) : 1;
                        var categorieDevice = service.GetCategorieParId(idCategorie);
                        if (categorieDevice != null)
                        {
                            if (categorieDevice.CategorieDeviceTraductions != null && categorieDevice.CategorieDeviceTraductions.ElementAt(0) != null)
                            {
                                ViewBag.CurrentCategorieLibelle = categorieDevice.CategorieDeviceTraductions.ElementAt(0).LibelleCategorieDevice;
                            }
                            else
                            {
                                ViewBag.CurrentCategorieLibelle = categorieDevice.LibelleCategorieDevice;
                            }
                        }

                        ViewBag.IdFamille = Session["idFamille"] ?? 1;
                        if (detaileObjet.TotalConsoCo2 != null)
                        {
                            ViewBag.TotalConsoEnergetique = detaileObjet.TotalConsoCo2.Valeur + " " + detaileObjet.TotalConsoCo2.Measure;
                        }
                        else
                        {
                            ViewBag.TotalConsoEnergetique = "0 teq-CO2";
                        }
                        break;

                    default:
                        detaileObjet = new ConsommationViewModel();
                        break;
                }

                var consoModels = new List<ConsoViewModel>();
                foreach (DetailConsommationViewModel detailConsommationViewModel in detaileObjet.DetailConsommations)
                {
                    var consoModel = new ConsoViewModel();

                    consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                    consoModel.IdClient = detailConsommationViewModel.IdClient;
                    consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                    consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                    consoModel.LienImage = detailConsommationViewModel.LienImage;
                    consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheCO2Totale;
                    consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheCO2Milieu;
                    consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                    consoModel.Libeller = detailConsommationViewModel.Libeller;
                    consoModel.Consommation = detailConsommationViewModel.ConsoCo2;
                    consoModel.Source = detailConsommationViewModel.Source;
                    consoModels.Add(consoModel);
                }

                Logger.Debug("ShowCO2 : End");

                if (typeObjet.Equals("Device"))
                    return PartialView("MenuPartial/_DeviceDetailsPartial", consoModels);

                ViewBag.TypeObjet = typeObjet;
                return PartialView("MenuPartial/_ConsommationDetailsPartial", consoModels);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        /// <summary>
        /// Affichage de toutes les quantités par cout
        /// </summary>
        /// <param name="typeObjet"></param>
        /// <param name="idParent"></param>
        /// <returns></returns>
        [OutputCache(Duration = 0)] 
        public ActionResult ShowPrix(string typeObjet, string idParent)
        {
            Logger.Debug("ShowPrix entrée: typeObjet={0}, idParent={1}", typeObjet, idParent);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                ConsommationViewModel detaileObjet;

                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();

                SetSimulationViewBag();
                if (Session["userType"] != null && Session["detailsFamille"] != null)
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");
                }
                else
                {
                    Logger.Debug("Session : userType & detailsFamille : nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        var clientId = contact.ClientId;
                        return RedirectToAction("Index", "Consommation", new { id = clientId });
                    }
                    return RedirectToLogin();
                }

                var genererRapport = Session["genererRapport"];
                var typeContact = Session["typeContact"];

                var detailConsommations = (IEnumerable<DetailConsommationViewModel>)detailsFamille.DetailConsommations;
                ViewBag.detailConsommations = detailConsommations;

                ViewBag.UserType = typeUtilisateur;
                ViewBag.DateModifier = detailsFamille.DateModifier;
                ViewBag.HeureModifier = detailsFamille.HeureModifier;
                ViewBag.TypeAffichage = "ShowPrix";
                Session.Add("typeAffichage", "ShowPrix");
                ViewBag.GenererRapport = genererRapport;
                ViewBag.TypeContact = typeContact;
                ViewBag.TypeClient = Session["typeClient"];
                ViewBag.NumeroContrat = Session["numeroContrat"];

                var idClient = (int) Session["idClient"];
                var devise = typeUtilisateur.Equals("Demo") ? service.GetCompteEvaluation(idClient).Devise : service.GetClientParId(idClient).DetailsClient.Devise;
                

                //ViewBag.LienImage = devise.UrlImage ?? "~/Content/images/others/consommation/Graph/v_cout.png"; 
                ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_cout.png";

                var codeDevise = devise != null ? devise.CodeDevise : " ";
                if (detailsFamille.TotalPrix != null)
                {
                    ViewBag.TotalConsoEnergetique = detailsFamille.TotalPrix.Valeur + " " + codeDevise;
                }
                else
                {
                    ViewBag.TotalConsoEnergetique = "0 " + codeDevise;
                }

                var idFamille = Session["currentFamilleId"] != null ? int.Parse(Session["currentFamilleId"].ToString()) : 1;
                var familleDevice = service.GetFamilleParId(idFamille);
                if (familleDevice != null)
                {
                    if (familleDevice.FamilleDeviceTraductions != null &&familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                    }
                    else
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.LibelleFamilleDevice;
                    }
                }

                switch (typeObjet)
                {
                    case ("Famille"):
                        detaileObjet = detailsFamille;
                        break;

                    case ("Categorie"):
                        ViewBag.IdParent = idParent;
                        ViewBag.TypeObjet = "Categorie";
                        ViewBag.CurrentFamilleId = Session["currentFamilleId"] ?? 1;

                        var detailsCategorie = new ConsommationViewModel();

                        foreach (var detailConsommation in detailConsommations)
                        {
                            if (detailConsommation.IdFamille == idParent.ToString())
                            {
                                detailsCategorie = detailConsommation.SubObjet;
                            }
                        }
                        detaileObjet = detailsCategorie;
                        ViewBag.DateModifier = detaileObjet.DateModifier;
                        ViewBag.HeureModifier = detaileObjet.HeureModifier;

                        ViewBag.TotalConsoEnergetique = detaileObjet.TotalPrix.Valeur + " " + codeDevise;
                       
                        break;

                    case ("Device"):
                        ViewBag.TypeObjet = "Device";
                        ViewBag.IdParent = idParent;

                        var resultatCalculateur = new CalculateurViewModel();
                        if (Session["resultatCalculateur"] != null)
                        {
                            resultatCalculateur = (CalculateurViewModel)Session["resultatCalculateur"];
                            Logger.Debug("Session : resultatCalculateur pas nulle");
                        }
                        else
                        {
                            Logger.Debug("Session : resultatCalculateur : nulle => Redirection Consommation/Index");
                            return RedirectToAction("Index", "Consommation", new { id = idClient });
                        }
                        var detailsDevice = service.GetDetailsDevice(resultatCalculateur, idClient.ToString(), idParent.ToString(), typeUtilisateur.ToString());

                        detaileObjet = detailsDevice;
                        detaileObjet.DetailConsommations = detaileObjet.DetailConsommations.Where(o => Convert.ToDecimal(o.Prix.Valeur) > 0).OrderByDescending(o => Convert.ToDecimal(o.Prix.Valeur)).ToList();

                        ViewBag.DateModifier = detaileObjet.DateModifier;
                        ViewBag.HeureModifier = detaileObjet.HeureModifier;
                        ViewBag.CurrentCategorieImage = Session["currentCategorieImage"] ?? "~/Content/images/Consommation/" + typeUtilisateur + "/Categorie/1.png";
                        ViewBag.CurrentCategorieId = Session["currentCategorieId"] ?? 1;
                        
                         var idCategorie = Session["currentCategorieId"] != null ? int.Parse(Session["currentCategorieId"].ToString()) : 1;
                        var categorieDevice = service.GetCategorieParId(idCategorie);
                        if (categorieDevice != null)
                        {
                            if (categorieDevice.CategorieDeviceTraductions != null && categorieDevice.CategorieDeviceTraductions.ElementAt(0) != null)
                            {
                                ViewBag.CurrentCategorieLibelle = categorieDevice.CategorieDeviceTraductions.ElementAt(0).LibelleCategorieDevice;
                            }
                            else
                            {
                                ViewBag.CurrentCategorieLibelle = categorieDevice.LibelleCategorieDevice;
                            }
                        }

                        ViewBag.IdFamille = Session["idFamille"] ?? 1;
                        if (detaileObjet.TotalPrix != null)
                        {
                            ViewBag.TotalConsoEnergetique = detaileObjet.TotalPrix.Valeur + " " + codeDevise;
                        }
                        else
                        {
                            ViewBag.TotalConsoEnergetique = "0 " + codeDevise;
                        }
                        break;

                    default:
                        detaileObjet = new ConsommationViewModel();
                        break;
                }

                var consoModels = new List<ConsoViewModel>();
                foreach (DetailConsommationViewModel detailConsommationViewModel in detaileObjet.DetailConsommations)
                {
                    var consoModel = new ConsoViewModel();

                    consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                    consoModel.IdClient = detailConsommationViewModel.IdClient;
                    consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                    consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                    consoModel.LienImage = detailConsommationViewModel.LienImage;
                    consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheCO2Totale;
                    consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheCO2Milieu;
                    consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                    consoModel.Libeller = detailConsommationViewModel.Libeller;

                    consoModel.Consommation = devise != null ? new Consommation(null, detailConsommationViewModel.Prix.Valeur, devise.CodeDevise) : new Consommation(null, detailConsommationViewModel.Prix.Valeur, "");
                   consoModel.Source = detailConsommationViewModel.Source;
                    consoModels.Add(consoModel);
                }

                Logger.Debug("ShowPrix : End");

                if (typeObjet.Equals("Device"))
                    return PartialView("MenuPartial/_DeviceDetailsPartial", consoModels);

                ViewBag.TypeObjet = typeObjet;
                return PartialView("MenuPartial/_ConsommationDetailsPartial", consoModels);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        /// <summary>
        /// Affichage au niveau modification quantité devices
        /// </summary>
        /// <param name="idCategorie"></param>
        /// <returns></returns>
        public ActionResult ModificationQuantiterDevice(int? idCategorie)
        {
            Logger.Debug("ModificationQuantiterDevice entrée: idCategorie={0}", idCategorie);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();
                var resultatCalculateur = new CalculateurViewModel();
                var layout = "";
                var idClient = 0;

                SetSimulationViewBag();
                if (Session["idclient"] != null)
                {
                    idClient = (int) Session["idclient"];
                    Logger.Debug("Session : idClient => {0}", idClient);
                }
                else
                {
                    Logger.Debug("Session : idClient : nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        idClient = contact.ClientId;
                    }
                }

                if (Session["userType"] != null && Session["resultatCalculateur"] != null && Session["detailsFamille"] != null && Session["layout"] != null)
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    resultatCalculateur = (CalculateurViewModel)Session["resultatCalculateur"];
                    Logger.Debug("Session : resultatCalculateur pas nulle");

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");

                    layout = (string)Session["layout"];
                    Logger.Debug("Session : layout => {0}", layout);
                }
                else
                {
                    Logger.Debug("Session : userType, resultatCalculateur, detailsFamilles, layout : nulle => Redirection Consommation/Index");
                    return RedirectToAction("Index", "Consommation", new { id = idClient });
                }
                
                ViewBag.IdParent = idCategorie;
                ViewBag.UserType = typeUtilisateur;
                ViewBag.TypeContact = Session["typeContact"];
                ViewBag.TypeAffichage = Session["typeAffichage"] ?? "ShowWatt";
                ViewBag.Modifier = true;
                ClientDeviceTypeViewModel typeDevicesClient = null;
                if (Session[sessionName] != null)
                {
                    Logger.Debug("Session : sessionName pas nulle");
                    typeDevicesClient = (ClientDeviceTypeViewModel) Session[sessionName];
                    if (typeDevicesClient.IdCategorie != idCategorie)
                    {
                        Logger.Debug("Changement catégorie, récupérer typeDevicesClient de la bdd");
                        typeDevicesClient = service.GetTypeDevicesClient(idCategorie.Value, resultatCalculateur, idClient, typeUtilisateur);                       
                        Session.Add(sessionName, typeDevicesClient);
                    }
                }
                else
                {
                    Logger.Debug("Session : sessionName nulle, récupérer typeDevicesClient de la bdd");
                    typeDevicesClient = service.GetTypeDevicesClient(idCategorie.Value, resultatCalculateur, idClient, typeUtilisateur);
                    Session.Add(sessionName, typeDevicesClient);
                }
              

                Logger.Debug("ModificationQuantiterDevice : End");

                return View("ModificationQuantiterDevice", layout, typeDevicesClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }


        /// <summary>
        /// Menu vertical au niveau modification quantité device - render partial view
        /// </summary>
        /// <param name="idCategorie"></param>
        /// <param name="typeAffichage"></param>
        /// <returns></returns>
        public ActionResult AfficherQuantiteDevice(int? idCategorie, string typeAffichage)
        {
            Logger.Debug("AfficherQuantiteDevice entrée: idCategorie={0}, typeAffichage={1}", idCategorie, typeAffichage);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                var typeUtilisateur = "";
                var detailsFamille = new ConsommationViewModel();
                var resultatCalculateur = new CalculateurViewModel();
                var idClient = 0;

                SetSimulationViewBag();
                if (Session["idclient"] != null)
                {
                    idClient = (int)Session["idclient"];
                    Logger.Debug("Session : idClient => {0}", idClient);
                }
                else
                {
                    Logger.Debug("Session : idClient : nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        idClient = contact.ClientId;
                    }
                }

                if (Session["userType"] != null && Session["resultatCalculateur"] != null && Session["detailsFamille"] != null)
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    resultatCalculateur = (CalculateurViewModel)Session["resultatCalculateur"];
                    Logger.Debug("Session : resultatCalculateur pas nulle");

                    detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                    Logger.Debug("Session : detailsFamille pas nulle");
                }
                else
                {
                    Logger.Debug("Session : userType, resultatCalculateur, detailsFamilles : nulle => Redirection Consommation/Index");
                    return RedirectToAction("Index", "Consommation", new { id = idClient });
                }
                var detailConsommations = (IEnumerable<DetailConsommationViewModel>)detailsFamille.DetailConsommations;
                
                ViewBag.TypeAffichage = typeAffichage;
                ViewBag.DateModifier = detailsFamille.DateModifier;
                ViewBag.detailConsommations = detailConsommations;
                ViewBag.IdParent = idCategorie;
                ViewBag.UserType = typeUtilisateur;
                ViewBag.TypeContact = Session["typeContact"];
                ViewBag.ErreurTarif = Session["MsgErreurTarif"];
                ViewBag.ModificationQty = 1;
                Session.Remove("MsgErreurTarif");
                ViewBag.ErreurQuantiteNegative = Session["ErreurQuantiteNegative"];
                Session.Remove("ErreurQuantiteNegative");
                ViewBag.TypeClient = Session["typeClient"];
                ViewBag.NumeroContrat = Session["numeroContrat"];

                var categorie = service.GetCategorieDevice(Convert.ToInt32(idCategorie));

                ViewBag.CurrentFamilleId = categorie.FamilleDeviceId;
                Session.Add("currentFamilleId", categorie.FamilleDeviceId);

                var familleDevice = service.GetFamilleParId(categorie.FamilleDeviceId.Value);
                if (familleDevice != null)
                {
                    if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                    }
                    else
                    {
                        ViewBag.CurrentFamilleLibelle = familleDevice.LibelleFamilleDevice;
                    }
                }

                ViewBag.CurrentCategorieId = idCategorie;
                Session.Add("currentCategorieId", idCategorie);

                var categorieDevice = service.GetCategorieParId(idCategorie.Value);
                if (categorieDevice != null)
                {
                    if (categorieDevice.CategorieDeviceTraductions != null && categorieDevice.CategorieDeviceTraductions.ElementAt(0) != null)
                    {
                        ViewBag.CurrentCategorieLibelle = categorieDevice.CategorieDeviceTraductions.ElementAt(0).LibelleCategorieDevice;
                    }
                    else
                    {
                        ViewBag.CurrentCategorieLibelle = categorieDevice.LibelleCategorieDevice;
                    }
                }

                ViewBag.IdFamille = categorie.FamilleDeviceId;

                if (Session["userType"].ToString().Equals("Demo"))
                {
                    ViewBag.ImageBig = "~/Content/images/Consommation/Demo/Categorie/" + idCategorie + "_Big.png";
                }
                else if (Session["userType"].ToString().Equals("Payant"))
                {
                    ViewBag.ImageBig = "~/Content/images/Consommation/Payant/Categorie/" + idCategorie + "_Big.png";
                }
                else if (Session["userType"].ToString().Equals("Simulation"))
                {
                    ViewBag.ImageBig = "~/Content/images/Consommation/Simulation/Categorie/" + idCategorie + "_Big.png";
                }
                
                ClientDeviceTypeViewModel typeDevicesClient = null;
                if (Session[sessionName] != null)
                {
                    Logger.Debug("Session : sessionName pas nulle");
                    typeDevicesClient = (ClientDeviceTypeViewModel)Session[sessionName];
                    if (typeDevicesClient.IdCategorie != idCategorie)
                    {
                        Logger.Debug("Changement catégorie, récupérer typeDevicesClient de la bdd");
                        typeDevicesClient = service.GetTypeDevicesClient(idCategorie.Value, resultatCalculateur, idClient, typeUtilisateur);
                    }
                }
                else
                {
                    Logger.Debug("Session : sessionName nulle, récupérer typeDevicesClient de la bdd");
                    typeDevicesClient = service.GetTypeDevicesClient(idCategorie.Value, resultatCalculateur, idClient, typeUtilisateur);
                    Session.Add(sessionName, typeDevicesClient);
                }
                ViewBag.WikiLink = typeDevicesClient.URL;
                typeDevicesClient = UpdateClientDeviceTypeViewModel(typeDevicesClient);
                Session.Add(sessionName, typeDevicesClient);
                var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

                var devise = typeUtilisateur.Equals("Demo") ? service.GetCompteEvaluation(idClient).Devise : service.GetClientParId(idClient).DetailsClient.Devise;

                if (typeAffichage.Equals("ShowWatt"))
                {
                    if (typeDevicesClient != null && typeDevicesClient.TotalConsoWatt != null)
                        ViewBag.TotalConsoEnergetique = ConsommationHelper.FormatterValeur(typeDevicesClient.TotalConsoWatt.Value) + " " + "kWh";
                    else
                    {
                        ViewBag.TotalConsoEnergetique =  "0 " + "kWh";
                    }
                }
                else if (typeAffichage.Equals("ShowCO2"))
                {
                    if (typeDevicesClient != null && typeDevicesClient.TotalConsoCo2 != null)
                        ViewBag.TotalConsoEnergetique = ConsommationHelper.FormatterValeur(typeDevicesClient.TotalConsoCo2.Value) + " " + "teq-CO2";
                    else
                    {
                        ViewBag.TotalConsoEnergetique = "0 " + "teq-CO2";
                    }
                }
                else
                {
                    var codeDevise = devise != null ? devise.CodeDevise : " ";
                    
                    if (typeDevicesClient != null && typeDevicesClient.TotalPrix != null)

                        ViewBag.TotalConsoEnergetique = ConsommationHelper.FormatterValeur(typeDevicesClient.TotalPrix.Value) + " " + codeDevise;
                    else
                    {
                        ViewBag.TotalConsoEnergetique = "0 " + codeDevise;
                    }

                }
                Session["typeAffichage"] = typeAffichage;
                Logger.Debug("AfficherQuantiteDevice : End");

                return PartialView("MenuPartial/_ModificationQuantiterDevicePartial", typeDevicesClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        
        /// <summary>
        /// MAJ des quantités modifiées dans la session
        /// </summary>
        /// <param name="idDevice"></param>
        /// <param name="libelleDevice"></param>
        /// <param name="quantiter"></param>
        /// <param name="idClasse"></param>
        /// <param name="idAge"></param>
        /// <param name="objetType"></param>
        /// <returns></returns>
        public string SetQuantity(string guid, int? quantiter, int? idClasse, int? idAge, int objetType)
        {
            Logger.Debug("SetQuantity entrée: guid={0}, quantiter={1}, idClasse={2}, idAge={3}, objetType={4}", guid, quantiter, idClasse, idAge, objetType);

            try
            {
                var estModifie = false;
                var clientDeviceTypeViewModel = new ClientDeviceTypeViewModel();

                if (Session[sessionName] != null)
                {
                    Logger.Debug("Session : sessionName pas nulle");
                    clientDeviceTypeViewModel = (ClientDeviceTypeViewModel) Session[sessionName];
                }
                else
                {
                    Logger.Debug("Session : sessionName : nulle => Redirection Consommation/AfficherQuantiteDevice : idCategorie = 1, typeAffichage = ShowWatt");
                    RedirectToAction("AfficherQuantiteDevice", "Consommation", new { idCategorie = 1, typeAffichage = "ShowWatt"});
                }

                //Modification quantite device
                if (objetType == 1)
                {
                    Logger.Debug("Modification quantité device");
                    foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        if (typeDevice.Guid.ToString().Equals(guid))
                        {
                            if (typeDevice.Quantite != quantiter)
                            {
                                typeDevice.Quantite = quantiter.HasValue ? quantiter.Value : 0;
                                typeDevice.IsModified = true;
                                estModifie = true;
                                typeDevice.EstRacineModifie = true;
                                clientDeviceTypeViewModel.EstDeviceModifie = true;
                                //Update DB
                                /*var configSiDevice = service.GetConfigSiDevice(Convert.ToInt32(idDevice));
                                configSiDevice.Quantite = Convert.ToInt32(quantiter);
                                service.MAJConfigSiDevice(configSiDevice);*/

                                break;
                            }
                        }
                    }
                }

                //Modification quantite classe
                else if (objetType == 2)
                {
                    Logger.Debug("Modification quantité classe");
                    foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        if (typeDevice.Guid.ToString().Equals(guid))
                        {
                            foreach (var gamme in typeDevice.Gammes)
                            {
                                if (gamme.IdClasse == idClasse)
                                {
                                    if (gamme.Quantiter != quantiter)
                                    {
                                        gamme.Quantiter =  quantiter.HasValue ? quantiter.Value : 0;
                                        gamme.isModified = true;
                                        typeDevice.IsModified = true;
                                        estModifie = true;
                                        typeDevice.EstParentModifie = true;
                                        clientDeviceTypeViewModel.EstDeviceModifie = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                //Modification quantite age
                else if (objetType == 3)
                {
                    Logger.Debug("Modification quantité age");
                    foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        if (typeDevice.Guid.ToString().Equals(guid))
                        {
                            foreach (var gamme in typeDevice.Gammes)
                            {
                                if (gamme.IdClasse == idClasse)
                                {
                                    foreach (var age in gamme.AgeDeviceViewModels)
                                    {
                                        if (age.IdAge == idAge)
                                        {
                                            if (age.Quantite != quantiter)
                                            {
                                                age.Quantite = quantiter.HasValue ? quantiter.Value : 0;
                                                typeDevice.IsModified = true;
                                                estModifie = true;
                                                gamme.EstEnfantModifie = true;
                                                clientDeviceTypeViewModel.EstDeviceModifie = true;
                                                //Update DB
                                                /*var configSiConsommation =service.GetConfigSiConsommation(Convert.ToInt32(idDevice),Convert.ToInt32(idClasse),Convert.ToInt32(idAge));
                                                configSiConsommation.Quantite = Convert.ToInt32(quantiter);
                                                service.MAJConfigSiConsommation(configSiConsommation);*/

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                clientDeviceTypeViewModel = UpdateClientDeviceTypeViewModel(clientDeviceTypeViewModel);
                Session.Add(sessionName, clientDeviceTypeViewModel);

                Logger.Debug("SetQuantity : End");

                if (estModifie)
                {
                    //Logger.Debug("Libelle Device : {0}", libelleDevice);
                    //var libelle = libelleDevice.Replace(" ", "").Replace("/", "").Replace("'", "").Replace("(", "").Replace(")", "");
                    return "true";
                }
                else
                {
                    Logger.Debug("Device pas modifié : {0}", guid);
                    return "false";
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return "";
            }
        }

        /// <summary>
        /// MAJ des listes configSiDevicesZeroListe(Qty du device = 0) et configSiDevicesNonZeroListe(Qty du device > 0)
        /// </summary>
        /// <param name="clientDeviceTypeViewModel"></param>
        /// <returns></returns>
        public ClientDeviceTypeViewModel UpdateClientDeviceTypeViewModel(ClientDeviceTypeViewModel clientDeviceTypeViewModel)
        {
            try
            {
                Logger.Debug("UpdateClientDeviceTypeViewModel entrée: clientDeviceTypeViewModel");

                var configSiDevicesNonZeroListe = new List<ConfigSiDeviceViewModel>();
                var configSiDevicesZeroListe = new List<ConfigSiDeviceViewModel>();
                foreach (var configSiDeviceViewModel in clientDeviceTypeViewModel.ConfigSiDevice)
                {
                    //Affichage libellé
                    configSiDeviceViewModel.LibelleAffiche = ConsommationHelper.FormatterLibelleDevice(configSiDeviceViewModel.LibelleDevice, 40);

                    if (configSiDeviceViewModel.Quantite == 0)
                    {
                        configSiDevicesZeroListe.Add(configSiDeviceViewModel);
                    }
                    else
                    {
                        configSiDevicesNonZeroListe.Add(configSiDeviceViewModel);
                    }
                }

                clientDeviceTypeViewModel.ConfigSiDevicesNonZero = configSiDevicesNonZeroListe.OrderBy(o => o.LibelleDevice);
                clientDeviceTypeViewModel.ConfigSiDevicesZero = configSiDevicesZeroListe.OrderBy(o => o.LibelleDevice);

                Logger.Debug("UpdateClientDeviceTypeViewModel : End");

                return clientDeviceTypeViewModel;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }
        }
        
        /// <summary>
        /// Affichage popup ModificationAttributs
        /// </summary>
        /// <param name="idDevice"></param>
        /// <param name="libelleDevice"></param>
        /// <returns></returns>
        public ActionResult ModificationAttributes(Guid guid)
        {
            Logger.Debug("UpdateClientDeviceTypeViewModel entrée: guid={0}", guid);

            ViewBag.TypeContact = Session["typeContact"];
            ViewBag.UserType = Session["userType"];
            ViewBag.TypeClient = Session["typeClient"];

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                ViewBag.MsgErreur = Session["ErreurLibelleDevice"];
                Session.Remove("ErreurLibelleDevice");

                var detailsDeviceType = new ConfigSiDeviceAttributeViewModel();
                var clientDeviceTypeViewModel = new ClientDeviceTypeViewModel();
                    
                if (Session[sessionName] != null)
                {
                    Logger.Debug("Session : sessionName pas nulle");
                    clientDeviceTypeViewModel = (ClientDeviceTypeViewModel) Session[sessionName];
                }
                else
                {
                    Logger.Debug("Session : sessionName : nulle => Redirection Consommation/AfficherQuantiteDevice : idCategorie = 1, typeAffichage = ShowWatt");
                    return RedirectToAction("AfficherQuantiteDevice", "Consommation", new { idCategorie = 1, typeAffichage = "ShowWatt"});
                }

                foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                {
                    if (typeDevice.Guid.ToString().Equals(guid.ToString()))
                    {
                        detailsDeviceType = typeDevice.detailsDeviceType;
                        if (detailsDeviceType == null)
                            detailsDeviceType = new ConfigSiDeviceAttributeViewModel();

                        //detailsDeviceType.IdDevice = (int) idDevice;
                        detailsDeviceType.Id = typeDevice.Id;
                        detailsDeviceType.Guid = typeDevice.Guid;
                        ViewBag.SourceDevice = typeDevice.Source;
                        ViewBag.WikiLink = typeDevice.URL;
                        break;
                    }
                }

                Logger.Debug("ModificationAttributes : End");

                return View(detailsDeviceType);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }   

        /// <summary>
        /// MAJ des modifications faites dans la popup ModificationAttributs dans la session
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ModificationAttributes(ConfigSiDeviceAttributeViewModel o)
        {
            Logger.Debug("ModificationAttributes entrée: ConfigSiDeviceAttributeViewModel");

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            var clientDeviceTypeViewModel = new ClientDeviceTypeViewModel();
            if (Session[sessionName] != null)
            {
                Logger.Debug("Session : sessionName pas nulle");
                clientDeviceTypeViewModel = (ClientDeviceTypeViewModel)Session[sessionName];
            }
            else
            {
                Logger.Debug("Session : sessionName : nulle => Redirection Consommation/AfficherQuantiteDevice : idCategorie = 1, typeAffichage = ShowWatt");
                return RedirectToAction("AfficherQuantiteDevice", "Consommation", new { idCategorie = 1, typeAffichage = "ShowWatt" });
            }
            
            try
            {
                ViewBag.UserType = Session["userType"];

                if (o.LibelleDevice == null)
                {
                    ModelState.AddModelError("o.LibelleDevice", "Erreur");
                    Session.Add("ErreurLibelleDevice", Resource.Traduction.MsgErreurLibelleObligatoire);
                }
                /*else if (o.LibelleDevice.Length > 100)
                {
                    ModelState.AddModelError("o.LibelleDevice", "Erreur");
                    Session.Add("ErreurLibelleDevice", Resource.Traduction.MsgErreurLongeurLibelleDevice);
                }*/
                else
                {
                    int idClient = 0;

                    if (Session["idClient"] != null)
                    {
                        idClient = (int)Session["idClient"];
                        Logger.Debug("Session : idClient => {0}", idClient);
                    }
                    else
                    {
                        var contactEmail = User.Identity.Name;
                        if (contactEmail != string.Empty)
                        {
                            var contact = service.GetContactParEmail(contactEmail);
                            idClient = contact.ClientId;
                            Logger.Debug("idClient => {0}", idClient);
                        }
                    }

                    var configSi = service.GetConfigSiClient(idClient).ConfigSi;

                    var configSiDevice = new ConfigSiDevice();
                    if (o.Id != 0)
                    {
                        configSiDevice = service.GetConfigSiDevice((int) o.Id);
                    }
                    else
                    {
                        configSiDevice.LibelleDevice = o.LibelleDevice;
                    }

                    if (!o.LibelleDevice.Equals(configSiDevice.LibelleDevice))
                    {
                        var device = service.GetConfigSiDeviceParLibelle(o.LibelleDevice, configSi.Id);
                        var device1 = clientDeviceTypeViewModel.ConfigSiDevice.FindAll(x => x.LibelleDevice.Equals(o.LibelleDevice));

                        if (device != null || device1.Any())
                        {
                            Logger.Debug("Validation : Device avec ce libellé existe déjà");

                            ModelState.AddModelError("o.LibelleDevice", "Erreur");
                            Session.Add("ErreurLibelleDevice", Resource.Traduction.MsgErreurLibelleExistant);
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        if (typeDevice.Guid.ToString().Equals(o.Guid.ToString()))
                        {
                            typeDevice.LibelleDevice = o.LibelleDevice;
                            typeDevice.detailsDeviceType = o;
                            typeDevice.IsModified = true;
                            break;
                        }
                    }
                    clientDeviceTypeViewModel = UpdateClientDeviceTypeViewModel(clientDeviceTypeViewModel);
                    Session.Add(sessionName, clientDeviceTypeViewModel);
                    Session.Add("ModificationQtyDevice", clientDeviceTypeViewModel);

                    Logger.Debug("ModificationAttributes : End");

                    return
                        Json(
                            new
                                {
                                    Url =
                                Url.Action("ModificationQuantiterDevice", "Consommation",
                                           new {idCategorie = clientDeviceTypeViewModel.IdCategorie})
                                });
                }
                else
                {
                    return View(o);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        /// <summary>
        /// Affichage popup CreationNouveauDevice
        /// </summary>
        /// <param name="idDevice"></param>
        /// <param name="libelleDevice"></param>
        /// <returns></returns>
        public ActionResult CreationConfigSiDevice(int? idDevice, string libelleDevice)
        {
            Logger.Debug("CreationConfigSiDevice entrée: idDevice={0}, libelleDevice={1}", idDevice, libelleDevice);

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                ViewBag.MsgErreur = Session["ErreurLibelleDevice"];
                Session.Remove("ErreurLibelleDevice");

                var detailsDeviceType = new ConfigSiDeviceAttributeViewModel();
                if (idDevice != null)
                {
                    var clientDeviceTypeViewModel = new ClientDeviceTypeViewModel();
                    if (Session[sessionName] != null)
                    {
                        Logger.Debug("Session : sessionName pas nulle");
                        clientDeviceTypeViewModel = (ClientDeviceTypeViewModel)Session[sessionName];
                    }
                    else
                    {
                        Logger.Debug("Session : sessionName : nulle => Redirection Consommation/AfficherQuantiteDevice : idCategorie = 1, typeAffichage = ShowWatt");
                        return RedirectToAction("AfficherQuantiteDevice", "Consommation", new { idCategorie = 1, typeAffichage = "ShowWatt" });
                    }

                    foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        if (typeDevice.TypeDeviceId.Equals(idDevice) && typeDevice.LibelleDevice.Equals(libelleDevice))
                        {
                            detailsDeviceType = typeDevice.detailsDeviceType;
                            if (detailsDeviceType == null)
                                detailsDeviceType = new ConfigSiDeviceAttributeViewModel();

                            detailsDeviceType.IdDevice = (int) idDevice;
                            break;
                        }
                    }
                }
                Session.Add("LibelleBenchmarkDevice", libelleDevice);

                Logger.Debug("CreationConfigSiDevice : End");

                return View(detailsDeviceType);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        /// <summary>
        /// MAJ des modifications faites dans la popup CreationNouveauDevice dans la session
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreationConfigSiDevice(ConfigSiDeviceAttributeViewModel o)
        {
            Logger.Debug("CreationConfigSiDevice entrée: ConfigSiDeviceAttributeViewModel");

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            var libelleDevice = Session["LibelleBenchmarkDevice"].ToString();
            Session.Remove("LibelleBenchmarkDevice");

            var clientDeviceTypeViewModel = new ClientDeviceTypeViewModel();
            if (Session[sessionName] != null)
            {
                Logger.Debug("Session : sessionName pas nulle");
                clientDeviceTypeViewModel = (ClientDeviceTypeViewModel)Session[sessionName];
            }
            else
            {
                Logger.Debug("Session : sessionName : nulle => Redirection Consommation/AfficherQuantiteDevice : idCategorie = 1, typeAffichage = ShowWatt");
                return RedirectToAction("AfficherQuantiteDevice", "Consommation", new { idCategorie = 1, typeAffichage = "ShowWatt" });
            }

            try
            {
                if (o.LibelleDevice == null)
                {
                    ModelState.AddModelError("o.LibelleDevice", "Erreur");
                    Session.Add("ErreurLibelleDevice", Resource.Traduction.MsgErreurLibelleObligatoire);
                }
                /*else if(o.LibelleDevice.Length > 100)
                {
                    ModelState.AddModelError("o.LibelleDevice", "Erreur");
                    Session.Add("ErreurLibelleDevice", Resource.Traduction.MsgErreurLongeurLibelleDevice);
                }*/
                else
                {
                    int idClient = 0;

                    if (Session["idClient"] != null)
                    {
                        idClient = (int)Session["idClient"];
                        Logger.Debug("Session : idClient => {0}", idClient);
                    }
                    else
                    {
                        var contactEmail = User.Identity.Name;
                        if (contactEmail != string.Empty)
                        {
                            var contact = service.GetContactParEmail(contactEmail);
                            idClient = contact.ClientId;
                            Logger.Debug("idClient => {0}", idClient);
                        }
                    }

                    var configSi = service.GetConfigSiClient(idClient).ConfigSi;

                    var device = service.GetConfigSiDeviceParLibelle(o.LibelleDevice, configSi.Id);
                    var device1 = clientDeviceTypeViewModel.ConfigSiDevice.FindAll(x => x.LibelleDevice.Equals(o.LibelleDevice));
                    if (device != null || device1.Any())
                    {
                        Logger.Debug("Validation : Device avec ce libellé existe déjà");

                        ModelState.AddModelError("o.LibelleDevice", "Erreur");
                        Session.Add("ErreurLibelleDevice", Resource.Traduction.MsgErreurLibelleExistant);
                    }
                }

                if (ModelState.IsValid)
                {
                    foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        if (typeDevice.TypeDeviceId.Equals(o.IdDevice) && typeDevice.LibelleDevice.Equals(libelleDevice))
                        {
                            //create new configSiDeviceViewModel
                            IValueInjecter injecter = new ValueInjecter();
                            object configSiDeviceObj = Activator.CreateInstance(typeof(ConfigSiDeviceViewModel));
                            configSiDeviceObj.InjectFrom(o);

                            var configDevice = (ConfigSiDeviceViewModel)configSiDeviceObj;
                            configDevice.EstNouveau = true;
                            configDevice.TypeDeviceId = typeDevice.TypeDeviceId;
                            configDevice.LibelleDevice = o.LibelleDevice;
                            configDevice.Quantite = typeDevice.Quantite;
                            configDevice.Source = (int)ConfigSiDeviceType.Client;
                            configDevice.Guid = Guid.NewGuid();
                            //configDevice.TypeUsageId = typeDevice.TypeUsageId;
                            var classes = service.ListeClasseDevice();
                            var ages = service.ListeAgeDevice();

                            var gammeDeviceList = new List<GammesDeviceViewModel>();
                            foreach (var classe in classes)
                            {
                                var gammeDevice = new GammesDeviceViewModel();
                                gammeDevice.IdDevice = typeDevice.TypeDeviceId;
                                gammeDevice.IdClasse = classe.Id;
                                gammeDevice.Libeller = classe.LibelleClasseDevice;
                                gammeDevice.isModified = false;
                                gammeDevice.LibelleDevice = o.LibelleDevice;
                                gammeDevice.Guid = configDevice.Guid;

                                var ageDeviceList = new List<AgeDeviceViewModel>();

                                var gamme = typeDevice.Gammes.Find(x => x.IdClasse == classe.Id);
                                foreach (var age in ages)
                                {
                                    var age1 = gamme.AgeDeviceViewModels.Find(x => x.IdAge == age.Id);

                                    var ageDevice = new AgeDeviceViewModel();
                                    ageDevice.IdDevice = typeDevice.TypeDeviceId;
                                    ageDevice.IdAge = age.Id;
                                    ageDevice.IdClasse = classe.Id;
                                    ageDevice.LibelleAgeDevice = age.LibelleAgeDevice;
                                    ageDevice.Quantite = age1.Quantite;
                                    ageDevice.IsModified = false;
                                    ageDevice.DateDebut = age.DateDebut;
                                    ageDevice.LibelleDevice =  o.LibelleDevice;
                                    ageDevice.UniqueGuid = configDevice.Guid;
                                    ageDeviceList.Add(ageDevice);
                                }
                                gammeDevice.AgeDeviceViewModels = ageDeviceList;
                                gammeDevice.Quantiter = gamme.Quantiter;
                                gammeDeviceList.Add(gammeDevice);
                            }

                            configDevice.Gammes = gammeDeviceList;

                            var detailsDeviceType = new ConfigSiDeviceAttributeViewModel();
                            detailsDeviceType.NbrJourTravaille = o.NbrJourTravaille;
                            detailsDeviceType.NbrHeuresIntensifJour = o.NbrHeuresIntensifJour;
                            detailsDeviceType.NbrHeuresOffJour = o.NbrHeuresOffJour;
                            detailsDeviceType.PourcentConsomExtern = o.PourcentConsomExtern;
                            detailsDeviceType.CoeffAbbatement = o.CoeffAbbatement;
                            detailsDeviceType.NbrHeuresNonIntensifJour = o.NbrHeuresNonIntensifJour;
                            detailsDeviceType.CoeffNonIntensif = o.CoeffNonIntensif;
                            detailsDeviceType.CoeffModeOff = o.CoeffModeOff;
                            detailsDeviceType.IdDevice = o.IdDevice;
                            detailsDeviceType.IdCategorie = o.IdCategorie;
                            detailsDeviceType.LibelleDevice = o.LibelleDevice;
                            configDevice.detailsDeviceType = detailsDeviceType;

                            configDevice.detailsDeviceType.ConfigSiConsommationViewModel = o.ConfigSiConsommationViewModel;

                            var configSiDevices = new List<ConfigSiDeviceViewModel>();
                            configSiDevices.AddRange(clientDeviceTypeViewModel.ConfigSiDevice);
                            configSiDevices.Add(configDevice);

                            clientDeviceTypeViewModel.ConfigSiDevice = configSiDevices;
                            break;
                        }
                    }
                    clientDeviceTypeViewModel = UpdateClientDeviceTypeViewModel(clientDeviceTypeViewModel);
                    Session.Add(sessionName, clientDeviceTypeViewModel);
                    Session.Add("ModificationQtyDevice", clientDeviceTypeViewModel);

                    Logger.Debug("CreationConfigSiDevice : End");

                    return Json(new {Url = Url.Action("ModificationQuantiterDevice", "Consommation", new { idCategorie = clientDeviceTypeViewModel.IdCategorie })});
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
            return RedirectToAction("CreationConfigSiDevice", new { idDevice = o.IdDevice, libelleDevice = libelleDevice });
        }

        /// <summary>
        /// Sauvegarder toutes les modifications faites sur la page ModificationQtyDevice dans la base
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public ActionResult SauvegarderAttributes(int? idFamille, int? idCategorie, int? modificationQty)
        {
            var debutSauvegarderAttributes =
                ConsommationHelper.GetStartTime("ConsommationController, SauvegarderAttributes");
            var idClient = (int) Session["idclient"];
            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            var typeUtilisateur = "";
            if (Session["userType"] != null)
            {
                typeUtilisateur = Session["userType"].ToString();
                Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);
            }
            else
            {
                if (Session["idclient"] != null)
                {
                    Logger.Debug("Session : idClient => {0}", idClient);
                }
                else
                {
                    Logger.Debug("Session : idClient => nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        idClient = contact.ClientId;
                    }
                }
                return RedirectToAction("Index", "Consommation", new {id = idClient});
            }

            var clientDeviceTypeViewModel = new ClientDeviceTypeViewModel();
            if (Session[sessionName] != null)
            {
                Logger.Debug("Session : sessionName pas nulle");
                clientDeviceTypeViewModel = (ClientDeviceTypeViewModel) Session[sessionName];

                var client = service.GetClientParId(clientDeviceTypeViewModel.IdClient);

                //Vérification quantité -ve
                var redirectToAction = VerifierQuantiteDevice(clientDeviceTypeViewModel);
                if (redirectToAction != null) return redirectToAction;

                clientDeviceTypeViewModel = TraiterDevice(clientDeviceTypeViewModel, client);

                TraiterConsommation(typeUtilisateur, client, ref clientDeviceTypeViewModel);

                ConsommationHelper.GetTimeDifference(debutSauvegarderAttributes,
                                                     "ConsommationController, SauvegarderAttributes");                                          
            }
           
            if(modificationQty.HasValue && modificationQty.Value==1)
                return RedirectToAction("ModificationQuantiterDevice", "Consommation", new { idCategorie = idCategorie }); 
            else if (idCategorie.HasValue)
                return RedirectToAction("Naviguer", new {idClient = idClient, idCategorie = idCategorie});
            else if (idFamille.HasValue)
                return RedirectToAction("Naviguer", new {idClient = idClient, idFamille = idFamille});
            else
                return RedirectToAction("Naviguer", new {idClient = idClient});

        }

        private bool TraiterConsommation(string typeUtilisateur, Client client, ref ClientDeviceTypeViewModel clientDeviceTypeViewModel)
        {
            var debutTraiterConsommation = ConsommationHelper.GetStartTime("ConsommationController, TraiterConsommation");
            int idContact = 0;

            if (Session["idContact"] != null)
            {
                idContact = (int) Session["idContact"];
                Logger.Debug("Session : idContact => {0}", idContact);
            }
            else
            {
                Logger.Debug("Session : idContact => nulle");
                var contactEmail = User.Identity.Name;
                if (contactEmail != string.Empty)
                {
                    Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                    var contact = service.GetContactParEmail(contactEmail);
                    idContact = contact.Id;
                }
            }

            var resultatCalculateur = new CalculateurViewModel();
            //Recalculer quantite et maj des valeurs dans les sessions 
            try
            {
                Logger.Debug("Recalculer quantité et maj des valeurs dans les sessions");

                //Pour un client payant (personnalisation)
                if (typeUtilisateur.Equals("Payant"))
                {
                    resultatCalculateur = service.GetResultatCalculateur(clientDeviceTypeViewModel.IdClient, "Payant");
                    if (resultatCalculateur == null)
                        Logger.Debug("resultatCalculateur => nulle");
                }
                else //Simulation
                {
                    var simulation = service.GetSimulationEnCours(idContact);
                    resultatCalculateur = service.GetDetailsConfigPourClientExistant(client, "Simulation", idContact);
                    if (resultatCalculateur == null)
                        Logger.Debug("resultatCalculateur => nulle");

                    //MAJ simulation
                    simulation.DateModification = DateTime.Now;
                    simulation = service.MAJSimulation(simulation);
                    Logger.Debug("Simulation MAJ id : {0}", simulation.Id);
                }

                //Recalculation et MAJ configSiConsommation
                Logger.Debug("Recalculer quantité et MAJ configSiConsommation");
                resultatCalculateur = service.ReCalculerEtMAJConfigSiConsommation(resultatCalculateur);
                if (resultatCalculateur == null)
                    Logger.Debug("resultatCalculateur => nulle");

                //MAJ resultat
                var majResultat = service.MAJResultat(resultatCalculateur);
                Session.Add("resultatCalculateur", resultatCalculateur);

                //Construire modèle pour affichage famille & catégorie
                var detailsFamille = service.GetDetailsFamille(resultatCalculateur, clientDeviceTypeViewModel.IdClient,
                                                               typeUtilisateur);
                Session.Add("detailsFamille", detailsFamille);

                //Construire modèle pour affichage typeDevice
                clientDeviceTypeViewModel = service.GetTypeDevicesClient(clientDeviceTypeViewModel.IdCategorie,
                                                                         resultatCalculateur, clientDeviceTypeViewModel.IdClient,
                                                                         typeUtilisateur);
                if (clientDeviceTypeViewModel == null)
                    Logger.Debug("clientDeviceTypeViewModel => nulle");

                //devices séparés en 2 listes différentes (qty=0 & qty!=0)
                clientDeviceTypeViewModel = UpdateClientDeviceTypeViewModel(clientDeviceTypeViewModel);
                Session.Add(sessionName, clientDeviceTypeViewModel);
                ConsommationHelper.GetTimeDifference(debutTraiterConsommation, "ConsommationController, TraiterConsommation");
            }
            catch (Exception e)
            {
               LogguerErreur(e);
               return false;
            }
            
            return true;
        }

        private ClientDeviceTypeViewModel TraiterDevice(ClientDeviceTypeViewModel clientDeviceTypeViewModel, Client client)
        {
            var debutTraiterDevice = ConsommationHelper.GetStartTime("ConsommationController, TraiterDevice");
            var consommationApiController = new ConsommationApiController();           
           
            try
            {
                if (clientDeviceTypeViewModel != null)
                {
                    //Vérification des quantités et recalculer si besoin
                    clientDeviceTypeViewModel = VerifierQuantite(clientDeviceTypeViewModel);
                    var configUpdated = false;

                    foreach (var typeDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        //Vérifier si le device a été modifié ou c'est un nouveau
                        if (typeDevice.IsModified || typeDevice.EstNouveau)
                        {
                            Logger.Debug("Device est modifié ou est nouveau");

                            IValueInjecter injecter = new ValueInjecter();
                            object configSiDevice = Activator.CreateInstance(typeof (ConfigSiDevice));
                            configSiDevice.InjectFrom(typeDevice.detailsDeviceType);
                            configSiDevice.InjectFrom(clientDeviceTypeViewModel);
                            configSiDevice.InjectFrom(typeDevice);

                            var configDevice = (ConfigSiDevice) configSiDevice;
                            configDevice.DateDebut = DateTime.Now;
                            configDevice.DateFin = null;
                            configDevice.DateModification = null;
                            configDevice.CoeffReductionWattExterne = typeDevice.detailsDeviceType.CoeffAbbatement;
                            //configDevice.TypeUsageId= typeDevice.detailsDeviceType.
                            //sauvegarder device et faire une entrée pour le device dans la table resultat pour un nouveau device
                            if (typeDevice.EstNouveau)
                            {
                                Logger.Debug("Device est nouveau");

                                //sauvegarder configSiDevice
                                configDevice = consommationApiController.SauveGarderConfigSiDevice(configDevice);
                                typeDevice.Id = configDevice.Id;
                                //typeDevice.EstNouveau = false;
                                Logger.Debug("ConfigSiDevice sauvegardé id : {0}", configDevice.Id);

                                //sauvegarder resultat
                                var resultat = new Resultat();
                                resultat.CoefficientConversion = client.Adresse.Pays.CoefficientConversion;
                                resultat.ConfigSiId = configDevice.ConfigSiId;
                                resultat.ConfigSiDeviceId = configDevice.Id;
                                resultat = consommationApiController.SauvegarderResultat(resultat);
                                Logger.Debug("Resultat sauvegardé id : {0}", resultat.Id);
                            }
                            else //MAJ configSiDevice pour un device existant
                            {
                                configDevice = consommationApiController.MAJConfigSiDevice(configDevice);
                                Logger.Debug("ConfigSiDevice MAJ id : {0}", configDevice.Id);
                            }

                            //MAJ configSi 
                            if (!configUpdated)
                            {
                                var configSi = consommationApiController.GetConfigSi(configDevice.ConfigSiId.Value);
                                configSi.DateModification = DateTime.Now;
                                configSi.Statut = (int) ConfigSiStatut.Modifier;
                                configSi = consommationApiController.MAJConfigSi(configSi);
                                Logger.Debug("configSi MAJ id : {0}", configSi.Id);
                                Session.Add("genererRapport", "Vrai");
                                configUpdated = true;
                            }
                            
                            
                            //Création configSiConsommations pour un nouveau device ou MAJ configSiConsommations pour un device existant
                            foreach (
                                var classe in typeDevice.detailsDeviceType.ConfigSiConsommationViewModel.ClasseDeviceViewModel)
                            {
                                var gamme = typeDevice.Gammes.Find(x => x.IdClasse == classe.Id);
                                
                                foreach (var ageConso in classe.AgeDeviceConsoViewModels)
                                {
                                    var configSiConsommation = new ConfigSiConsommation();
                                    configSiConsommation.AgeDeviceId = ageConso.AgeDeviceId;
                                    configSiConsommation.ClasseDeviceId = Convert.ToInt32(classe.Id);
                                    configSiConsommation.ConfigSiDeviceId = configDevice.Id;
                                    configSiConsommation.ConsommationIntensif = ageConso.ConsommationIntensif;
                                    configSiConsommation.ConsommationNonIntensif = ageConso.ConsommationNonIntensif;                                   
                                   // configSiConsommation.ConsommationIntensifUnitaire = ageConso.ConsommationIntensif;
                                    configSiConsommation.DateModification = DateTime.Now;

                                    
                                   
                                    var age = gamme.AgeDeviceViewModels.Find(o => o.IdAge == ageConso.AgeDeviceId);
                                    configSiConsommation.Quantite = age.Quantite;

                                    //MAJ configSiConsommations pour un device existant
                                    if (typeDevice.IsModified || gamme.isModified || age.IsModified)
                                    {
                                        var consommation = consommationApiController.GetConfigSiConsommation(configDevice.Id,
                                                                                              classe.Id.Value,
                                                                                              ageConso.AgeDeviceId);
                                        if (consommation != null)
                                        {
                                            configSiConsommation.Id = consommation.Id;
                                            configSiConsommation.IsModified = true;
                                            configSiConsommation.ConsommationIntensifUnitaire = consommation.ConsommationIntensifUnitaire;
                                            configSiConsommation = consommationApiController.MAJConfigSiConsommation(configSiConsommation);
                                            Logger.Debug("ConfigSiConsommation MAJ id : {0}", configSiConsommation.Id);                                           
                                        }
                                    }

                                    //Création configSiConsommations pour un nouveau device
                                    if(typeDevice.EstNouveau) 
                                    {
                                        configSiConsommation.IsModified = true;//we set it true so that, after calculateur, we know we should update
                                        configSiConsommation.ConsommationIntensifUnitaire = ageConso.ConsommationIntensif;
                                        typeDevice.IsModified = true;  
                                        configSiConsommation = consommationApiController.SauveGarderConfigSiConsommation(configSiConsommation);                                                                              
                                        Logger.Debug("ConfigSiConsommation sauvegardé id : {0}", configSiConsommation.Id);
                                    }
                                }

                                // mise a jour resultat device
                                //var total = typeDevice.detailsDeviceType.ConfigSiConsommationViewModel.ClasseDeviceViewModel
                                //    .Sum(t => (t.AgeDeviceConsoViewModels.Sum(a => (int?)a.ConsommationModeOff + (int?)a.ConsommationIntensif + (int?)a.ConsommationNonIntensif)))??0;

                                //var resultatDevice = consommationApiController.GetResultatDevice(configDevice);
                                //resultatDevice.ConsommationWatt = total;
                                //resultatDevice.ConsommationCo2 = total*resultatDevice.CoefficientConversion;
                                //consommationApiController.SauvegarderResultat(resultatDevice);
                            }

                        }

                        typeDevice.EstNouveau = false;
                    }
                }
                ConsommationHelper.GetTimeDifference(debutTraiterDevice,"ConsommationController, TraiterDevice");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                LogguerErreur(e);
            }
            return clientDeviceTypeViewModel;
        }

        private ActionResult VerifierQuantiteDevice(ClientDeviceTypeViewModel clientDeviceTypeViewModel)
        {
            var debutVerifierQuantiteDevice = ConsommationHelper.GetStartTime("ConsommationController, VerifierQuantiteDevice");
            foreach (var device in clientDeviceTypeViewModel.ConfigSiDevice)
            {
                if (device.IsModified)
                {
                    //Vérification quantité device
                    if (device.Quantite < 0)
                    {
                        Session.Add("ErreurQuantiteNegative", Resource.Traduction.MsgErreurQtyNegative);
                        return RedirectToAction("ModificationQuantiterDevice",
                                                new {idCategorie = clientDeviceTypeViewModel.IdCategorie});
                    }
                    else
                    {
                        //Vérification quantité par classe
                        foreach (var gamme in device.Gammes)
                        {
                            if (gamme.Quantiter < 0)
                            {
                                Session.Add("ErreurQuantiteNegative", Resource.Traduction.MsgErreurQtyNegative);
                                return RedirectToAction("ModificationQuantiterDevice",
                                                        new {idCategorie = clientDeviceTypeViewModel.IdCategorie});
                            }
                            else
                            {
                                //Vérification quantité par age
                                if (gamme.AgeDeviceViewModels.Any(age => age.Quantite < 0))
                                {
                                    Session.Add("ErreurQuantiteNegative", Resource.Traduction.MsgErreurQtyNegative);
                                    return RedirectToAction("ModificationQuantiterDevice",
                                                            new
                                                                {
                                                                    idCategorie =
                                                                clientDeviceTypeViewModel.IdCategorie
                                                                });
                                }
                            }
                        }
                    }
                }
            }
            ConsommationHelper.GetTimeDifference(debutVerifierQuantiteDevice, "ConsommationController, VerifierQuantiteDevice");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool VerificationTarif(Client client, ClientDeviceTypeViewModel clientDeviceTypeViewModel)
        {
            try
            {
                var tarif = client.DetailsClient.Tarif;

                var configSiDevices = clientDeviceTypeViewModel.ConfigSiDevice;
                var quantiteDevices = configSiDevices.Aggregate(0, (current, configSiDevice) => configSiDevice.Quantite + current);
                //var configSiDevices = service.GetConfigSiClient(client.Id).ConfigSi.ConfigSiDevices;
                //var quantiteDevices = configSiDevices.Aggregate(0, (current, configSiDevice) => configSiDevice.Quantite + current);

                if (tarif.NbreMaxDevice >= quantiteDevices)
                {
                    Session.Remove("MsgErreurTarif");
                    return true;
                }
                else
                {
                    Session.Add("MsgErreurTarif", Resource.Traduction.MsgErreurNombreDevices);
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Affichage de la page Resultat Simulation - Main Page
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ActionResult ResultatSimulation()
        {
            Logger.Debug("ResultatSimulation entrée: - ");

            if (RedirectUtilisateurInactif())
            {
                return RedirectToLogin();
            }

            try
            {
                var idContact = 0;
                
                if (Session["idContact"] != null)
                {
                    idContact = (int)Session["idContact"];
                    Logger.Debug("Session : idContact => {0}", idContact);
                }
                else
                {
                    Logger.Debug("Session : idContact => nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        idContact = contact.Id;
                    }
                }
                var resultatCalculateurSource = (CalculateurViewModel)Session["resultatCalculateur"];
                var resultatSimulation = service.GetResultatSimulation(idContact);
                Session.Add("resultatSimulation", resultatSimulation);
                if (resultatSimulation != null)
                    Session.Add("NomSimulation", resultatSimulation.NomSimulation);

                var typeUtilisateur = "";
                var layout = "";

                if (Session["userType"] != null && Session["detailsFamille"] != null)
                {
                    typeUtilisateur = (string) Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                    layout = (string)Session["layout"];
                    Logger.Debug("Session : layout => {0}", layout);
                }
                else
                {
                    typeUtilisateur = "Simulation";
                    Logger.Debug("Initialiser typeUtilisateur à simulation");

                    layout = "~/Views/Shared/Maquette/_Layout_Client.cshtml";
                    Logger.Debug("Initialiser layout à ~/Views/Shared/Maquette/_Layout_Client.cshtml");
                }

                ViewBag.UserType = typeUtilisateur;
                ViewBag.TypeObjet = "Famille";
                ViewBag.TypeAffichage = Session["typeAffichage"] ?? "ShowWatt";
                ViewBag.TypeContact = Session["typeContact"];

                Logger.Debug("ResultatSimulation : End");

                return View("ResultatSimulation", layout, resultatSimulation);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        /// <summary>
        /// Affichage de la page ResultatSimulation - Partial Page
        /// </summary>
        /// <param name="typeAffichage"></param>
        /// <returns></returns>
        public ActionResult AfficherResultatSimulation(string typeAffichage)
        {
            Logger.Debug("AfficherResultatSimulation entrée: typeAffichage={0} ", typeAffichage);

            try
            {
                var resultatSimulation = new ResultatSimulationViewModel();
                if (Session["resultatSimulation"] != null)
                {
                    resultatSimulation = (ResultatSimulationViewModel) Session["resultatSimulation"];
                    Logger.Debug("Session : resultatSimulation pas nulle ");
                }
                else
                {
                    Logger.Debug("Session : resultatSimulation : nulle => Redirection Consommation/ResultatSimulation");
                    return RedirectToAction("ResultatSimulation");
                }

                var detailsSimulation = (IEnumerable<DetailResultatSimulationViewModel>)resultatSimulation.DetailsSimulation;
                ViewBag.detailsSimulation = detailsSimulation;

                ViewBag.TypeAffichage = typeAffichage;
                ViewBag.TypeObjet = "SimulationFamille";
                ViewBag.DateModifier = resultatSimulation.DateModifier;
                ViewBag.TypeContact = Session["typeContact"];
                ViewBag.UserType = Session["userType"];
                SetSimulationViewBag();
                
                var idContact = 0;

                if (Session["idContact"] != null)
                {
                    idContact = (int)Session["idContact"];
                    Logger.Debug("Session : idContact => {0}", idContact);
                }
                else
                {
                    Logger.Debug("Session : idContact => nulle");
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        Logger.Debug("Récupérer l'identifiant de l'utilisateur connecté");
                        var contact = service.GetContactParEmail(contactEmail);
                        idContact = contact.Id;
                    }
                }

                var contact1 = service.GetDetailsContact(idContact);

                var consoModels = new List<ConsoViewModel>();
                foreach (var detailSimulation in detailsSimulation)
                {
                    var consoModel = new ConsoViewModel();
                    
                    consoModel.IdFamille = detailSimulation.IdFamille.ToString();
                    consoModel.LienImage = "~/Content/images/Consommation/Simulation/Famille/" + detailSimulation.IdFamille + ".png";
                    
                    if (typeAffichage.Equals("ShowWatt"))
                    {
                        var longeurGraphTotale = detailSimulation.LongueurGrapheWattSourceTotale >
                                                 detailSimulation.LongueurGrapheWattSimuleTotale
                                                     ? detailSimulation.LongueurGrapheWattSourceTotale
                                                     : detailSimulation.LongueurGrapheWattSimuleTotale;

                        consoModel.LongueurGrapheTotale = longeurGraphTotale;
                        consoModel.LongueurGrapheMilieu = detailSimulation.LongueurGrapheWattSourceMilieu;
                        consoModel.LongueurGrapheSimuleTotale = detailSimulation.LongueurGrapheWattSimuleTotale;
                        consoModel.LongueurGrapheSimuleMilieu = detailSimulation.LongueurGrapheWattSimuleMilieu;
                        consoModel.Consommation = new Consommation("", detailSimulation.ConsoWatt.Valeur, detailSimulation.ConsoWatt.Measure);
                        consoModel.consoTotale = detailSimulation.consoWattSource > detailSimulation.consoWattSimule
                                                     ? detailSimulation.consoWattSource
                                                     : detailSimulation.consoWattSimule;
                        ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_kwh.png";
                        ViewBag.TotalConsoEnergetique = ConsommationHelper.FormatterValeur(resultatSimulation.TotalConsoWattSimule) + " " + "kWh";
                    }
                    else if (typeAffichage.Equals("ShowCO2"))
                    {
                        var longeurGraphTotale = detailSimulation.LongueurGrapheCO2SourceTotale >
                                                 detailSimulation.LongueurGrapheCO2SimuleTotale
                                                     ? detailSimulation.LongueurGrapheCO2SourceTotale
                                                     : detailSimulation.LongueurGrapheCO2SimuleTotale;

                        consoModel.LongueurGrapheTotale = longeurGraphTotale;
                        consoModel.LongueurGrapheMilieu = detailSimulation.LongueurGrapheCO2SourceMilieu;
                        consoModel.LongueurGrapheSimuleTotale = detailSimulation.LongueurGrapheCO2SimuleTotale;
                        consoModel.LongueurGrapheSimuleMilieu = detailSimulation.LongueurGrapheCO2SimuleMilieu;
                        //consoModel.Consommation = detailSimulation.ConsoCo2;
                        consoModel.Consommation = new Consommation("", detailSimulation.ConsoCo2.Valeur,
                                                                   detailSimulation.ConsoCo2.Measure);
                        consoModel.consoTotale = detailSimulation.consoCo2Source > detailSimulation.consoCo2Simule
                                                     ? detailSimulation.consoCo2Source
                                                     : detailSimulation.consoCo2Simule;
                        ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_CO2.png";
                        ViewBag.TotalConsoEnergetique = ConsommationHelper.FormatterValeur(resultatSimulation.TotalConsoCO2Simule) + " " + "teq-CO2";
                    }
                    else
                    {
                        var longeurGraphTotale = detailSimulation.LongueurGrapheWattSourceTotale >
                                                detailSimulation.LongueurGrapheWattSimuleTotale
                                                    ? detailSimulation.LongueurGrapheWattSourceTotale
                                                    : detailSimulation.LongueurGrapheWattSimuleTotale;

                        consoModel.LongueurGrapheTotale = longeurGraphTotale;
                        consoModel.LongueurGrapheMilieu = detailSimulation.LongueurGrapheWattSourceMilieu;
                        consoModel.LongueurGrapheSimuleTotale = detailSimulation.LongueurGrapheWattSimuleTotale;
                        consoModel.LongueurGrapheSimuleMilieu = detailSimulation.LongueurGrapheWattSimuleMilieu;
                        consoModel.Consommation = new Consommation("", detailSimulation.Prix.Valeur, detailSimulation.Prix.Measure);
                        consoModel.consoTotale = detailSimulation.consoWattSource > detailSimulation.consoWattSimule
                                                     ? detailSimulation.consoWattSource
                                                     : detailSimulation.consoWattSimule;
                        
                        //ViewBag.LienImage = contact1.Client.DetailsClient.Devise.UrlImage ?? "~/Content/images/others/consommation/Graph/v_cout.png";
                        ViewBag.LienImage = "~/Content/images/others/consommation/Graph/v_cout.png";
                        ViewBag.TotalConsoEnergetique = ConsommationHelper.FormatterValeur(resultatSimulation.TotalPrixSimule) + " " + detailSimulation.Prix.Measure;
                    }

                    consoModel.Pourcentage = detailSimulation.Pourcentage;
                    consoModel.Libeller = detailSimulation.LibelleFamille;

                    consoModel.Equivalences = detailSimulation.Equivalences;

                    /*
                    consoModel.equivalenceLibelle1 = Resource.Traduction.ResourceManager.GetString("Equivalence" + detailSimulation.IdEquivalence1) ?? detailSimulation.equivalenceLibelle1;
                    consoModel.equivalenceLibelle2 = Resource.Traduction.ResourceManager.GetString("Equivalence" + detailSimulation.IdEquivalence2) ?? detailSimulation.equivalenceLibelle2;
                    consoModel.equivalenceLibelle3 = Resource.Traduction.ResourceManager.GetString("Equivalence" + detailSimulation.IdEquivalence3) ?? detailSimulation.equivalenceLibelle3;
                    consoModel.equivalenceLibelle4 = Resource.Traduction.ResourceManager.GetString("Equivalence" + detailSimulation.IdEquivalence4) ?? detailSimulation.equivalenceLibelle4;
                    consoModel.equivalenceLibelle5 = Resource.Traduction.ResourceManager.GetString("Equivalence" + detailSimulation.IdEquivalence5) ?? detailSimulation.equivalenceLibelle5;
                   
                    consoModel.qty1 = detailSimulation.qty1;
                    consoModel.qty2 = detailSimulation.qty2;
                    consoModel.qty3 = detailSimulation.qty3;
                    consoModel.qty4 = detailSimulation.qty4;
                    consoModel.qty5 = detailSimulation.qty5;
                    
                    consoModel.lienImage1 = detailSimulation.lienImage1;
                    consoModel.lienImage2 = detailSimulation.lienImage2;
                    consoModel.lienImage3 = detailSimulation.lienImage3;
                    consoModel.lienImage4 = detailSimulation.lienImage4;
                    consoModel.lienImage5 = detailSimulation.lienImage5;*/

                    consoModels.Add(consoModel);
                }

                Logger.Debug("AfficherResultatSimulation : End");

                return PartialView("MenuPartial/_ResultatSimulationPartial", consoModels);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        /// <summary>
        /// Vérifier si les quantités de saisie sont ventilées correctement
        /// </summary>
        /// <param name="clientDeviceTypeViewModel"></param>
        /// <returns></returns>
        public ClientDeviceTypeViewModel VerifierQuantite(ClientDeviceTypeViewModel clientDeviceTypeViewModel)
        {
            try
            {
                if (clientDeviceTypeViewModel.EstDeviceModifie)
                {
                    foreach (var configSiDevice in clientDeviceTypeViewModel.ConfigSiDevice)
                    {
                        var resultatCalculateur = new CalculateurViewModel();
                        if (Session["resultatCalculateur"] != null)
                        {
                            resultatCalculateur = (CalculateurViewModel) Session["resultatCalculateur"];
                        }
                        else
                        {
                            RedirectToAction("AfficherQuantiteDevice", "Consommation", new {idCategorie = 1, typeAffichage = "ShowWatt"});
                        }

                        var ventilationClasseDevices = new List<VentilationClasseDeviceViewModel>();
                        var ventilationClasseAgeDevices = new List<VentilationClasseAgeDeviceViewModel>();
                        foreach (var famille in resultatCalculateur.FamilleDeviceViewModels)
                        {
                            foreach (var categorie in famille.CategorieDevices)
                            {
                                foreach (var typeDevice in categorie.TypeDevices)
                                {
                                    if (typeDevice.Id == configSiDevice.TypeDeviceId)
                                    {
                                        ventilationClasseDevices = typeDevice.VentilationClasseDevices;
                                        ventilationClasseAgeDevices = typeDevice.VentilationClasseAgeDevices;
                                    }
                                }
                            }
                        }

                        //racine modifie
                        if (configSiDevice.EstRacineModifie && configSiDevice.IsModified)
                        {
                            //parent modifie
                            if (configSiDevice.EstParentModifie)
                            {
                                //enfant modifie / pas modifie
                                configSiDevice.Gammes = VerifierQuantiteEnfant(configSiDevice.Gammes, ventilationClasseAgeDevices);
                                configSiDevice.Quantite = CalculerQuantiteDevice(configSiDevice.Gammes);
                            }
                            else //parent pas modifie
                            {
                                var estEnfantModifie = false;
                                foreach (var gamme in configSiDevice.Gammes)
                                {
                                    //enfant modifie
                                    if (gamme.EstEnfantModifie)
                                    {
                                        gamme.Quantiter = CalculerQuantiteClasse(gamme.AgeDeviceViewModels);
                                        estEnfantModifie = true;
                                    }
                                }
                                if (estEnfantModifie)
                                {
                                    configSiDevice.Quantite = CalculerQuantiteDevice(configSiDevice.Gammes);
                                }

                                //enfant pas modifie
                                if (!estEnfantModifie)
                                {
                                    configSiDevice.Gammes = CalculerNombreDeviceParClasse(configSiDevice, ventilationClasseDevices, ventilationClasseAgeDevices);
                                }
                            }
                        }
                        
                        //racine pas modifie
                        if (!configSiDevice.EstRacineModifie && configSiDevice.IsModified)
                        {
                            //parent modifie
                            if (configSiDevice.EstParentModifie)
                            {
                                //enfant modifie / pas modifie
                                configSiDevice.Gammes = VerifierQuantiteEnfant(configSiDevice.Gammes, ventilationClasseAgeDevices);
                                configSiDevice.Quantite = CalculerQuantiteDevice(configSiDevice.Gammes);

                            }
                            else //parent pas modifie
                            {
                                //enfant modifie / pas modifie
                                configSiDevice.Gammes = VerifierQuantiteEnfant(configSiDevice.Gammes, ventilationClasseAgeDevices);
                                configSiDevice.Quantite = CalculerQuantiteDevice(configSiDevice.Gammes);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }
            return clientDeviceTypeViewModel;
        }


        /// <summary>
        /// Calculer qty totale pour une classe
        /// </summary>
        /// <param name="ages"></param>
        /// <returns></returns>
        public int CalculerQuantiteClasse(List<AgeDeviceViewModel> ages)
        {
            try
            {
                return ages.Aggregate(0, (current, age) => age.Quantite + current);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return 0;
            }
        }

        /// <summary>
        /// Calculer qty totale totale pour une device
        /// </summary>
        /// <param name="gammes"></param>
        /// <returns></returns>
        public int CalculerQuantiteDevice(List<GammesDeviceViewModel> gammes)
        {
            try
            {
                return gammes.Aggregate(0, (current, gamme) => gamme.Quantiter + current);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return 0;
            }
        }
        
        /// <summary>
        /// Vérifier si les ages sont ventilés correctement
        /// </summary>
        /// <param name="gammes"></param>
        /// <param name="ventilationClasseAgeDevices"></param>
        /// <returns></returns>
        public List<GammesDeviceViewModel> VerifierQuantiteEnfant(List<GammesDeviceViewModel> gammes, List<VentilationClasseAgeDeviceViewModel> ventilationClasseAgeDevices)
        {
            //var estEnfantModifie = false;

            try
            {
                foreach (var gamme in gammes)
                {
                    //enfant modifie
                    if (gamme.EstEnfantModifie)
                    {
                        gamme.Quantiter = CalculerQuantiteClasse(gamme.AgeDeviceViewModels);
                        //estEnfantModifie = true;
                    }
                    else if (gamme.isModified)
                    {
                        gamme.AgeDeviceViewModels = CalculerNombreDeviceParAge(gamme, ventilationClasseAgeDevices);
                    }
                }

                ////enfant pas modifie
                //if (!estEnfantModifie)
                //{
                //    foreach (var gamme in gammes)
                //    {
                //        if (gamme.isModified)
                //        {
                //            gamme.AgeDeviceViewModels = CalculerNombreDeviceParAge(gamme, ventilationClasseAgeDevices);
                //        }
                //    }
                //}
                return gammes;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Calculer nombre devices par classe
        /// </summary>
        /// <param name="configSiDevice"></param>
        /// <param name="ventilationClasseDevices"></param>
        /// <param name="ventilationClasseAgeDevices"></param>
        /// <returns></returns>
        public List<GammesDeviceViewModel> CalculerNombreDeviceParClasse(ConfigSiDeviceViewModel configSiDevice, List<VentilationClasseDeviceViewModel> ventilationClasseDevices, List<VentilationClasseAgeDeviceViewModel> ventilationClasseAgeDevices)
        {           
            var nbDeviceClasseList = new List<decimal>();
            int count;
            decimal nbDeviceClasse;
            int nombreDeviceAge;

            try
            {
                count = 0;
                nbDeviceClasseList.Clear();

                foreach (var classeDevice in ventilationClasseDevices)
                {
                    if (classeDevice.TypeDeviceId == configSiDevice.TypeDeviceId)
                    {
                        nbDeviceClasse = calculateur.CalculerNombreDevice(classeDevice.Coefficient, configSiDevice.Quantite);
                        nbDeviceClasseList.Add(nbDeviceClasse);
                    }
                }

                nbDeviceClasseList = calculateur.ArrondirDecimal(nbDeviceClasseList);

                Logger.Debug("liste from calculator arrondi");
                foreach (var val in nbDeviceClasseList)
                {
                    Logger.Debug(val);
                }
                for (int i = 0; i < configSiDevice.Gammes.Count(); i++)
                {
                    configSiDevice.Gammes[i].Quantiter = Convert.ToInt32(nbDeviceClasseList[i]);
                    Logger.Debug("{0} = {1}", configSiDevice.Gammes[i].Libeller, configSiDevice.Gammes[i].Quantiter);
                    configSiDevice.Gammes[i].AgeDeviceViewModels = CalculerNombreDeviceParAge(configSiDevice.Gammes[i], ventilationClasseAgeDevices);
                }

                return configSiDevice.Gammes;
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return null;
            }
        }

        /// <summary>
        /// Calculer nombre devices par age
        /// </summary>
        /// <param name="gamme"></param>
        /// <param name="ventilationClasseAgeDevices"></param>
        /// <returns></returns>
        public List<AgeDeviceViewModel> CalculerNombreDeviceParAge(GammesDeviceViewModel gamme, List<VentilationClasseAgeDeviceViewModel> ventilationClasseAgeDevices)
        {
            var nbDeviceAgeList = new List<decimal>();
            int count;
            decimal nbDeviceAge;
            int nombreDeviceAge;

            try
            {
                count = 0;
                nbDeviceAgeList.Clear();

                foreach (var ageDevice in ventilationClasseAgeDevices)
                {
                    if (ageDevice.ClasseDeviceId == gamme.IdClasse)
                    {
                        nbDeviceAge = calculateur.CalculerNombreDevice(ageDevice.Coefficient, gamme.Quantiter);
                        nbDeviceAgeList.Add(nbDeviceAge);
                    }
                }

                nbDeviceAgeList = calculateur.ArrondirDecimal(nbDeviceAgeList);

                for (int i = 0; i < gamme.AgeDeviceViewModels.Count(); i++)
                {
                    gamme.AgeDeviceViewModels[i].Quantite = Convert.ToInt32(nbDeviceAgeList[i]);
                }
                return gamme.AgeDeviceViewModels;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return null;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool RedirectUtilisateurInactif()
        {                        
            if (Session["userType"] == null)
            {
                System.Web.HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Redirection à la page d'accueil si la session est nulle
        /// </summary>
        /// <returns></returns>
        public ActionResult RedirectToLogin()
        {
            Logger.Debug("Redirection page d'accueil, session null");
            var currentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            Session.Clear();  // This may not be needed -- but can't hurt
            Session.Abandon();

            // Clear authentication cookie
            HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            rFormsCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(rFormsCookie);

            // Clear session cookie 
            HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            rSessionCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(rSessionCookie);
            if (Request.IsAjaxRequest())
                return JavaScript("window.location = '" + Url.Action("Index", "Home", new {lang = currentLanguage}) + "'");
            else
                return RedirectToAction("Index", "Home", new {lang = currentLanguage});
        }

        /// <summary>
        /// Générer rapport personnalisation
        /// </summary>
        /// <param name="idFamille"></param>
        /// <param name="idCategorie"></param>
        /// <returns></returns>
        public ActionResult GenererRapportPersonnalisation(int? idFamille, int? idCategorie, int? modificationQty)
        {
            try
            {
                var idContact = 0;
                var idClient = 0;
                if (Session["idContact"] != null && Session["idClient"] != null)
                {
                    idContact = (int)Session["idContact"];
                    idClient = (int)Session["idClient"];
                }
                else
                {
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        var contact = service.GetContactParEmail(contactEmail);
                        idContact = contact.Id;
                        idClient = contact.ClientId;
                    }
                }

                //get configSi for client where statut = moderer
                var genererRapport = service.GenererRapportPersonnalisation(idClient, idContact);
                Session.Add("genererRapport", "Faux");

                if (idCategorie != null)
                {
                    idFamille = null;
                }

                if (modificationQty == 1)
                {
                    return RedirectToAction("ModificationQuantiterDevice", "Consommation",
                                            new {idCategorie = idCategorie});
                }
                else
                {
                    return RedirectToAction("Naviguer", "Consommation",
                                           new { idClient = idClient, idFamille = idFamille, idCategorie = idCategorie });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        public void SetSimulationViewBag()
        {
            if (Session["NomSimulation"] != null)
            {
                ViewBag.NomSimulation = Session["NomSimulation"].ToString();
            }
        }

        public void ConsoFamille_Pdf()
        {
            var adresseServeur = ConfigurationManager.AppSettings["ServerAddress"];
            var pdfConsoFamille = new PdfConsoViewModel();

            //récupérer les valeurs de la session
            var typeUtilisateur = "";
            var detailsFamille = new ConsommationViewModel();
            var idClient = 0;
            if (Session["userType"] != null && Session["detailsFamille"] != null && Session["layout"] != null && Session["idClient"] != null)
            {
                typeUtilisateur = (string)Session["userType"];
                Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                Logger.Debug("Session : detailsFamille pas nulle");

                idClient = (int) Session["idClient"];
                Logger.Debug("Session : idClient pas nulle");
            }
            else
            {
                Logger.Debug("Session : userType & detailsFamille & layout : nulle => Redirection Consommation/Index");
                //return RedirectToAction("Index", "Consommation");
            }

            if (Session["typeAffichage"] == null)
            {
                Session.Add("typeAffichage", "ShowWatt");
            }
            else
            {
                pdfConsoFamille.TypeAffichage = Session["typeAffichage"].ToString();
            }

            pdfConsoFamille.LienSeparateurVertical = adresseServeur + "/Content/images/Consommation/separateur_vertical1.png";
            pdfConsoFamille.TypeUtilisateur = typeUtilisateur;
            pdfConsoFamille.LienImageSignePourcent = adresseServeur + "/Content/images/Consommation/pourcent.png";

            switch (typeUtilisateur)
            {
                case "Demo":
                    pdfConsoFamille.LienGrapheBas = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_bas_bleu.png";
                    pdfConsoFamille.LienGrapheMilieu = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_milieu_bleu.png";
                    pdfConsoFamille.LienGrapheHaut = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_haut_bleu.png";
                    break;
                case "Payant":
                    pdfConsoFamille.LienGrapheBas = adresseServeur + "/Content/images/Consommation/Payant/Graphe/graph_bas_vert.png";
                    pdfConsoFamille.LienGrapheMilieu = adresseServeur + "/Content/images/Consommation/Payant/Graphe/graph_milieu_vert.png";
                    pdfConsoFamille.LienGrapheHaut = adresseServeur + "/Content/images/Consommation/Payant/Graphe/graph_haut_vert.png";
                    break;
                default:
                    pdfConsoFamille.LienGrapheBas = adresseServeur + "/Content/images/Consommation/Simulation/Graphe/graph_bas_vert2.png";
                    pdfConsoFamille.LienGrapheMilieu = adresseServeur + "/Content/images/Consommation/Simulation/Graphe/graph_milieu_vert2.png";
                    pdfConsoFamille.LienGrapheHaut = adresseServeur + "/Content/images/Consommation/Simulation/Graphe/graph_haut_vert2.png";
                    break;
            }

            var devise = typeUtilisateur.Equals("Demo") ? service.GetCompteEvaluation(idClient).Devise : service.GetClientParId(idClient).DetailsClient.Devise;

            if (pdfConsoFamille.TypeAffichage == "ShowWatt")
            {
                pdfConsoFamille.TotalConso = detailsFamille.TotalConsoWatt.ValeurAfficher + " " + detailsFamille.TotalConsoWatt.Measure;
                pdfConsoFamille.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_kwh.png";
                pdfConsoFamille.ConsoAnnuelleTexte = Traduction.ConsoAnuelleKwh;
                pdfConsoFamille.TotalConsoTexte = Traduction.ConsoEnergetique;
            }
            else if (pdfConsoFamille.TypeAffichage == "ShowCO2")
            {
                pdfConsoFamille.TotalConso = detailsFamille.TotalConsoCo2.ValeurAfficher + " " + detailsFamille.TotalConsoCo2.Measure;
                pdfConsoFamille.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_CO2.png";
                pdfConsoFamille.ConsoAnnuelleTexte = Traduction.ConsoAnuelleCo2;
                pdfConsoFamille.TotalConsoTexte = Traduction.ConsoEnergetique;
            }
            else
            {
                var codeDevise = devise != null ? devise.CodeDevise : " ";
                pdfConsoFamille.TotalConso = detailsFamille.TotalPrix.ValeurAfficher + " " + codeDevise;
                pdfConsoFamille.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_cout.png";
                pdfConsoFamille.ConsoAnnuelleTexte = Traduction.CoutEnergetique;
                pdfConsoFamille.TotalConsoTexte = Traduction.CoutEnergetique;
            }

            pdfConsoFamille.LienScale = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_fond1.png";
            pdfConsoFamille.LienImagePourcentage = adresseServeur + "/Content/images/Consommation/Demo/Graphe/v_pourcentage.png";

            var detailConsommations = (IEnumerable<DetailConsommationViewModel>)detailsFamille.DetailConsommations;

            var consoModels = new List<ConsoViewModel>();
            foreach (DetailConsommationViewModel detailConsommationViewModel in detailConsommations)
            {
                var consoModel = new ConsoViewModel();
                consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                consoModel.IdClient = detailConsommationViewModel.IdClient;
                consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                consoModel.LienImage = detailConsommationViewModel.LienImage;
                consoModel.LienImage = consoModel.LienImage.Replace("~", adresseServeur);
                consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;
                consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                consoModel.Libeller = detailConsommationViewModel.Libeller;
                consoModel.Source = detailConsommationViewModel.Source;

                switch (pdfConsoFamille.TypeAffichage)
                {
                    case "ShowWatt":
                        consoModel.Consommation = detailConsommationViewModel.ConsoWatt;
                        break;
                    case "ShowCO2":
                        consoModel.Consommation = detailConsommationViewModel.ConsoCo2;
                        break;
                    default:
                        var codeDevise = devise != null ? devise.CodeDevise : " ";
                        var conso = new Consommation(null, detailConsommationViewModel.Prix.Valeur, codeDevise);
                        consoModel.Consommation = conso;
                        break;
                }

                consoModels.Add(consoModel);
            }

            pdfConsoFamille.ConsoViewModels = consoModels;

            GenererPdfFamille(pdfConsoFamille, detailsFamille);

            //return new PartialViewAsPdf("ConsoFamille_Pdf", pdfConsoFamille);
            //return View("ConsoFamille_Pdf", pdfConsoFamille);
        }


        public void GenererPdfFamille(PdfConsoViewModel pdfConsoFamille, ConsommationViewModel detailsFamille)
        {
            var adresseServeur = ConfigurationManager.AppSettings["ServerAddress"];
            var doc = new iTextSharp.text.Document();
            //doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

            try
            {
                HttpContext.Response.ContentType = "application/pdf";
                HttpContext.Response.AddHeader("content-disposition", "inline;filename=ConsoFamille_Pdf.pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                PdfWriter.GetInstance(doc, HttpContext.Response.OutputStream);

                doc.Open();

                var tableHeader = new PdfPTable(2);
                tableHeader.SetTotalWidth(new float[] { 500, 500 });
                tableHeader.TotalWidth = 800f;

                Font arial = FontFactory.GetFont("Arial", 20, Color.BLACK);
                var cellHeader1 = new PdfPCell(new Phrase(pdfConsoFamille.TotalConsoTexte, arial));
                cellHeader1.Border = 0;
                cellHeader1.HorizontalAlignment = 0;
                tableHeader.AddCell(new PdfPCell(cellHeader1));

                var cellHeader3 = new PdfPCell(new Phrase(pdfConsoFamille.TotalConso + " " + Traduction.ParAn, arial));
                cellHeader3.Border = 0;
                cellHeader3.HorizontalAlignment = 2;
                tableHeader.AddCell(new PdfPCell(cellHeader3));

                var cellHeader4 = new PdfPCell(new Phrase(pdfConsoFamille.FamilleLibelleActuel, arial));
                cellHeader4.Colspan = 2;
                cellHeader4.BorderWidthTop = 0;
                cellHeader4.BorderWidthBottom = 1;
                cellHeader4.BorderWidthLeft = 0;
                cellHeader4.BorderWidthRight = 0;
                cellHeader4.HorizontalAlignment = 0;
                cellHeader4.PaddingBottom = 4;
                tableHeader.AddCell(new PdfPCell(cellHeader4));

                doc.Add(tableHeader);

                doc.Add(new Paragraph("\n"));

                var table = new PdfPTable(2);
                table.SetTotalWidth(new float[] { 100, 400 });
                table.TotalWidth = 500f;
                table.DefaultCell.BorderWidth = 1;


                Image png1 = Image.GetInstance(adresseServeur + "/Content/images/Consommation/Payant/Famille/1.png");
                var cell1 = new PdfPCell(png1);
                cell1.Colspan = 2;
                cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(new PdfPCell(cell1));

                var cell2 = new PdfPCell(new Phrase("Collaborateur"));
                cell2.Colspan = 2;
                cell2.HorizontalAlignment = 1;
                table.AddCell(new PdfPCell(cell2));

                Image png2 = Image.GetInstance(adresseServeur + "/Content/images/Consommation/ScaleVertical.png");
                var cell3 = new PdfPCell(png2);
                cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(new PdfPCell(cell3));

                Image png3 = Image.GetInstance(adresseServeur + "/Content/images/Consommation/GrapheVertical_vert.png");
                var cell4 = new PdfPCell(png3);
                cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                cell4.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(new PdfPCell(cell4));

                Font arial12 = FontFactory.GetFont("Arial", 12, Color.LIGHT_GRAY);
                var cell5 = new PdfPCell(new Phrase("Consommation annuelle en kwh", arial12));
                cell5.Colspan = 2;
                cell5.HorizontalAlignment = 1;
                table.AddCell(new PdfPCell(cell5));
            
                var cell6 = new PdfPCell();
                cell6.Colspan = 2;
                cell6.AddElement(new Phrase("58 235kWh"));
                Image png4 = Image.GetInstance(adresseServeur + "/Content/images/others/consommation/Graph/v_kwh.png");
                cell6.AddElement(png4);
                cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                cell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(new PdfPCell(cell6));

                doc.Add(table);

                doc.Add(png4);
            }
            catch (Exception ex)
            {
                //Log error;
            }
            finally
            {
                doc.Close();
                HttpContext.Response.Write(doc);
                HttpContext.Response.End();
            }
        }

        public ActionResult ConsoCategorie_Pdf()
        {

            var adresseServeur = ConfigurationManager.AppSettings["ServerAddress"];
            var pdfConsoCategorie = new PdfConsoViewModel();

            //récupérer les valeurs de la session
            var typeUtilisateur = "";
            var detailsFamille = new ConsommationViewModel();
            var idClient = 0;
            if (Session["userType"] != null && Session["detailsFamille"] != null && Session["layout"] != null && Session["idClient"] != null)
            {
                typeUtilisateur = (string)Session["userType"];
                Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                detailsFamille = (ConsommationViewModel)Session["detailsFamille"];
                Logger.Debug("Session : detailsFamille pas nulle");

                idClient = (int)Session["idClient"];
                Logger.Debug("Session : idClient pas nulle");
            }
            else
            {
                Logger.Debug("Session : userType & detailsFamille & layout : nulle => Redirection Consommation/Index");
                return RedirectToAction("Index", "Consommation");
            }

            if (Session["typeAffichage"] == null)
            {
                Session.Add("typeAffichage", "ShowWatt");
            }
            else
            {
                pdfConsoCategorie.TypeAffichage = Session["typeAffichage"].ToString();
            }

            var idFamille = Session["currentFamilleId"] != null ? int.Parse(Session["currentFamilleId"].ToString()) : 1;
            var familleDevice = service.GetFamilleParId(idFamille);
            if (familleDevice != null)
            {
                if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                {
                    pdfConsoCategorie.FamilleLibelleActuel = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                }
                else
                {
                    pdfConsoCategorie.FamilleLibelleActuel = familleDevice.LibelleFamilleDevice;
                }
            }

            var detailsCategorie = new ConsommationViewModel();
            foreach (var detailConsommation in detailsFamille.DetailConsommations)
            {
                if (detailConsommation.IdFamille == idFamille.ToString())
                {
                    detailsCategorie = detailConsommation.SubObjet;
                }
            }

            pdfConsoCategorie.LienSeparateurVertical = adresseServeur + "/Content/images/Consommation/separateur_vertical1.png";
            pdfConsoCategorie.TypeUtilisateur = typeUtilisateur;
            pdfConsoCategorie.LienImageSignePourcent = adresseServeur + "/Content/images/Consommation/pourcent.png";

            switch (typeUtilisateur)
            {
                case "Demo":
                    pdfConsoCategorie.LienGrapheBas = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_bas_bleu.png";
                    pdfConsoCategorie.LienGrapheMilieu = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_milieu_bleu.png";
                    pdfConsoCategorie.LienGrapheHaut = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_haut_bleu.png";
                    break;
                case "Payant":
                    pdfConsoCategorie.LienGrapheBas = adresseServeur + "/Content/images/Consommation/Payant/Graphe/graph_bas_vert.png";
                    pdfConsoCategorie.LienGrapheMilieu = adresseServeur + "/Content/images/Consommation/Payant/Graphe/graph_milieu_vert.png";
                    pdfConsoCategorie.LienGrapheHaut = adresseServeur + "/Content/images/Consommation/Payant/Graphe/graph_haut_vert.png";
                    break;
                default:
                    pdfConsoCategorie.LienGrapheBas = adresseServeur + "/Content/images/Consommation/Simulation/Graphe/graph_bas_vert2.png";
                    pdfConsoCategorie.LienGrapheMilieu = adresseServeur + "/Content/images/Consommation/Simulation/Graphe/graph_milieu_vert2.png";
                    pdfConsoCategorie.LienGrapheHaut = adresseServeur + "/Content/images/Consommation/Simulation/Graphe/graph_haut_vert2.png";
                    break;
            }

            var devise = typeUtilisateur.Equals("Demo") ? service.GetCompteEvaluation(idClient).Devise : service.GetClientParId(idClient).DetailsClient.Devise;

            if (pdfConsoCategorie.TypeAffichage == "ShowWatt")
            {
                pdfConsoCategorie.TotalConso = detailsFamille.TotalConsoWatt.ValeurAfficher + " " + detailsFamille.TotalConsoWatt.Measure;
                pdfConsoCategorie.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_kwh.png";
                pdfConsoCategorie.ConsoAnnuelleTexte = Traduction.ConsoAnuelleKwh;
                pdfConsoCategorie.TotalConsoTexte = Traduction.ConsoEnergetique;
            }
            else if (pdfConsoCategorie.TypeAffichage == "ShowCO2")
            {
                pdfConsoCategorie.TotalConso = detailsFamille.TotalConsoCo2.ValeurAfficher + " " + detailsFamille.TotalConsoCo2.Measure;
                pdfConsoCategorie.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_CO2.png";
                pdfConsoCategorie.ConsoAnnuelleTexte = Traduction.ConsoAnuelleCo2;
                pdfConsoCategorie.TotalConsoTexte = Traduction.ConsoEnergetique;
            }
            else
            {
                var codeDevise = devise != null ? devise.CodeDevise : " ";
                pdfConsoCategorie.TotalConso = detailsFamille.TotalPrix.ValeurAfficher + " " + codeDevise;
                pdfConsoCategorie.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_cout.png";
                pdfConsoCategorie.ConsoAnnuelleTexte = Traduction.CoutEnergetique;
                pdfConsoCategorie.TotalConsoTexte = Traduction.CoutEnergetique;
            }

            pdfConsoCategorie.LienScale = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_fond1.png";
            pdfConsoCategorie.LienImagePourcentage = adresseServeur + "/Content/images/Consommation/Demo/Graphe/v_pourcentage.png";

            var detailConsommations = (IEnumerable<DetailConsommationViewModel>)detailsCategorie.DetailConsommations;

            var consoModels = new List<ConsoViewModel>();
            foreach (DetailConsommationViewModel detailConsommationViewModel in detailConsommations)
            {
                var consoModel = new ConsoViewModel();
                consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                consoModel.IdClient = detailConsommationViewModel.IdClient;
                consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                consoModel.LienImage = detailConsommationViewModel.LienImage;
                consoModel.LienImage = consoModel.LienImage.Replace("~", adresseServeur);
                consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;
                consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                consoModel.Libeller = detailConsommationViewModel.Libeller;
                consoModel.Source = detailConsommationViewModel.Source;

                switch (pdfConsoCategorie.TypeAffichage)
                {
                    case "ShowWatt":
                        consoModel.Consommation = detailConsommationViewModel.ConsoWatt;
                        break;
                    case "ShowCO2":
                        consoModel.Consommation = detailConsommationViewModel.ConsoCo2;
                        break;
                    default:
                        var codeDevise = devise != null ? devise.CodeDevise : " ";
                        var conso = new Consommation(null, detailConsommationViewModel.Prix.Valeur, codeDevise);
                        consoModel.Consommation = conso;
                        break;
                }

                consoModels.Add(consoModel);
            }

            pdfConsoCategorie.ConsoViewModels = consoModels;

            return new PartialViewAsPdf("ConsoCategorie_Pdf", pdfConsoCategorie);
            //return View("ConsoCategorie_Pdf", pdfConsoFamille);
        }


        public void ConsoDevice_Pdf()
        {
            
            var adresseServeur = ConfigurationManager.AppSettings["ServerAddress"];
            var pdfConsoFamille = new PdfConsoViewModel();

            //récupérer les valeurs de la session
            var typeUtilisateur = "";
            var resultatCalculateur = new CalculateurViewModel();
            var idClient = 0;


            if (Session["userType"] != null && Session["resultatCalculateur"] != null && Session["idClient"] != null)
            {
                typeUtilisateur = (string)Session["userType"];
                Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);

                resultatCalculateur = (CalculateurViewModel)Session["resultatCalculateur"];
                Logger.Debug("Session : resultatCalculateur pas nulle");

                idClient = (int)Session["idClient"];
                Logger.Debug("Session : idClient pas nulle");
            }
            else
            {
                Logger.Debug("Session : userType & detailsFamille & layout : nulle => Redirection Consommation/Index");
            }

            if (Session["typeAffichage"] == null)
            {
                Session.Add("typeAffichage", "ShowWatt");
            }
            else
            {
                pdfConsoFamille.TypeAffichage = Session["typeAffichage"].ToString();
            }

           var idFamille = Session["currentFamilleId"] != null ? int.Parse(Session["currentFamilleId"].ToString()) : 1;
            var familleDevice = service.GetFamilleParId(idFamille);
            if (familleDevice != null)
            {
                if (familleDevice.FamilleDeviceTraductions != null && familleDevice.FamilleDeviceTraductions.ElementAt(0) != null)
                {
                    pdfConsoFamille.FamilleLibelleActuel = familleDevice.FamilleDeviceTraductions.ElementAt(0).LibelleFamilleDevice;
                }
                else
                {
                    pdfConsoFamille.FamilleLibelleActuel = familleDevice.LibelleFamilleDevice;
                }
            }

            var idCategorie = Session["currentCategorieId"] != null ? int.Parse(Session["currentCategorieId"].ToString()) : 1;
            var categorieDevice = service.GetCategorieParId(idCategorie);
            if (categorieDevice != null)
            {
                if (categorieDevice.CategorieDeviceTraductions != null && categorieDevice.CategorieDeviceTraductions.ElementAt(0) != null)
                {
                    pdfConsoFamille.CategorieLibelleActuel = categorieDevice.CategorieDeviceTraductions.ElementAt(0).LibelleCategorieDevice;
                }
                else
                {
                    pdfConsoFamille.CategorieLibelleActuel = categorieDevice.LibelleCategorieDevice;
                }
            }

            if (typeUtilisateur.Equals("Demo"))
            {
                pdfConsoFamille.LienImageCategorieActuel = "~/Content/images/Consommation/Demo/Categorie/" + idCategorie + ".png";
            }
            else if (typeUtilisateur.Equals("Payant"))
            {
                pdfConsoFamille.LienImageCategorieActuel = "~/Content/images/Consommation/Payant/Categorie/" + idCategorie + ".png";
            }
            else if (typeUtilisateur.Equals("Simulation"))
            {
                pdfConsoFamille.LienImageCategorieActuel = "~/Content/images/Consommation/Simulation/Categorie/" + idCategorie + ".png";
            }
            
            var detailsDevice = service.GetDetailsDevice(resultatCalculateur, idClient.ToString(), idCategorie.ToString(), typeUtilisateur);

            pdfConsoFamille.LienSeparateurVertical = adresseServeur + "/Content/images/Consommation/separateur_vertical1.png";
            pdfConsoFamille.TypeUtilisateur = typeUtilisateur;

           
            var devise = typeUtilisateur.Equals("Demo") ? service.GetCompteEvaluation(idClient).Devise : service.GetClientParId(idClient).DetailsClient.Devise;

            switch (pdfConsoFamille.TypeAffichage)
            {
                case "ShowWatt":
                    pdfConsoFamille.TotalConso = detailsDevice.TotalConsoWatt.ValeurAfficher + " " + detailsDevice.TotalConsoWatt.Measure;
                    pdfConsoFamille.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_kwh.png";
                    pdfConsoFamille.ConsoAnnuelleTexte = Traduction.ConsoAnuelleKwh;
                    pdfConsoFamille.TotalConsoTexte = Traduction.ConsoEnergetique;
                    detailsDevice.DetailConsommations = detailsDevice.DetailConsommations.Where(o => Convert.ToDecimal(o.ConsoWatt.Valeur) > 0)
                                                                     .OrderByDescending(o => Convert.ToDecimal(o.ConsoWatt.Valeur))
                                                                     .ToList();
                    break;
                case "ShowCO2":
                    detailsDevice.DetailConsommations = detailsDevice.DetailConsommations.Where(o => Convert.ToDecimal(o.ConsoCo2.Valeur) > 0)
                                                                     .OrderByDescending(o => Convert.ToDecimal(o.ConsoCo2.Valeur))
                                                                     .ToList();
                    pdfConsoFamille.TotalConso = detailsDevice.TotalConsoCo2.ValeurAfficher + " " + detailsDevice.TotalConsoCo2.Measure;
                    pdfConsoFamille.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_CO2.png";
                    pdfConsoFamille.ConsoAnnuelleTexte = HttpUtility.HtmlEncode(Traduction.ConsoAnuelleCo2);
                    pdfConsoFamille.TotalConsoTexte = HttpUtility.HtmlEncode(Traduction.ConsoEnergetique);
                    break;
                default:
                    detailsDevice.DetailConsommations = detailsDevice.DetailConsommations.Where(o => Convert.ToDecimal(o.Prix.Valeur) > 0)
                                                                     .OrderByDescending(o => Convert.ToDecimal(o.Prix.Valeur))
                                                                     .ToList();

                    var codeDevise = devise != null ? devise.CodeDevise : " ";
                    pdfConsoFamille.TotalConso = detailsDevice.TotalPrix.ValeurAfficher + " " + codeDevise;
                    pdfConsoFamille.LienImageTypeAffichage = adresseServeur + "/Content/images/others/consommation/Graph/v_cout.png";
                    pdfConsoFamille.ConsoAnnuelleTexte = "coût annuel";
                    pdfConsoFamille.TotalConsoTexte = HttpUtility.HtmlEncode(Traduction.CoutEnergetique);
                    break;
            }

            pdfConsoFamille.LienScale = adresseServeur + "/Content/images/Consommation/Demo/Graphe/graph_fond1.png";
            pdfConsoFamille.LienImagePourcentage = adresseServeur + "/Content/images/Consommation/Demo/Graphe/v_pourcentage.png";

            
            var consoModels = new List<ConsoViewModel>();
            foreach (DetailConsommationViewModel detailConsommationViewModel in detailsDevice.DetailConsommations)
            {
                var consoModel = new ConsoViewModel();
                consoModel.IdFamille = detailConsommationViewModel.IdFamille;
                consoModel.IdClient = detailConsommationViewModel.IdClient;
                consoModel.IdCategorie = detailConsommationViewModel.IdCategorie;
                consoModel.IdTypeDevice = detailConsommationViewModel.IdTypeDevice;
                consoModel.LienImage = detailConsommationViewModel.LienImage;
                consoModel.LienImage = consoModel.LienImage.Replace("~", adresseServeur);
                consoModel.LongueurGrapheTotale = detailConsommationViewModel.LongueurGrapheWattTotale;
                consoModel.LongueurGrapheMilieu = detailConsommationViewModel.LongueurGrapheWattMilieu;
                consoModel.Pourcentage = detailConsommationViewModel.Pourcentage;
                consoModel.Libeller = HttpUtility.HtmlEncode(detailConsommationViewModel.Libeller);
                consoModel.Source = detailConsommationViewModel.Source;

                switch (pdfConsoFamille.TypeAffichage)
                {
                    case "ShowWatt":
                        consoModel.Consommation = detailConsommationViewModel.ConsoWatt;
                        break;
                    case "ShowCO2":
                        consoModel.Consommation = detailConsommationViewModel.ConsoCo2;
                        break;
                    default:
                        var codeDevise = devise != null ? devise.CodeDevise : " ";
                        var conso = new Consommation(null, detailConsommationViewModel.Prix.Valeur, codeDevise);
                        consoModel.Consommation = conso;
                        break;
                }

                consoModels.Add(consoModel);
            }

            pdfConsoFamille.ConsoViewModels = consoModels;

            if (detailsDevice.DetailConsommations.Count == 0)
            {
                if (pdfConsoFamille.TypeAffichage.Equals("ShowWatt"))
                {
                    ViewBag.TotalConsoEnergetique = "0 kWh";
                }
                else if (pdfConsoFamille.TypeAffichage.Equals("ShowCO2"))
                {
                    ViewBag.TotalConsoEnergetique = "0 teq-CO2";
                }
                else
                {
                    var codeDevise = devise != null ? devise.CodeDevise : " ";
                    ViewBag.TotalConsoEnergetique = "0 " + codeDevise;
                }
            }

            GenererPdfDevice(pdfConsoFamille, detailsDevice, idCategorie);
            //return new ViewAsPdf("ConsoDevice_Pdf", pdfConsoFamille);


        }

        public void GenererPdfDevice(PdfConsoViewModel pdfConsoFamille, ConsommationViewModel detailsDevice, int idCategorie)
        {
            var adresseServeur = ConfigurationManager.AppSettings["ServerAddress"];
            var doc = new iTextSharp.text.Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

            try
            {
                HttpContext.Response.ContentType = "application/pdf";
                HttpContext.Response.AddHeader("content-disposition", "inline;filename=ConsoDevice_Pdf.pdf");
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                PdfWriter.GetInstance(doc, HttpContext.Response.OutputStream);

                doc.Open();

                var tableHeader = new PdfPTable(2);
                tableHeader.SetTotalWidth(new float[] { 500, 500 });
                tableHeader.TotalWidth = 800f;

                Font arial = FontFactory.GetFont("Arial", 20, Color.BLACK);
                var cellHeader1 = new PdfPCell(new Phrase(pdfConsoFamille.TotalConsoTexte, arial));
                cellHeader1.Border = 0;
                cellHeader1.HorizontalAlignment = 0;
                tableHeader.AddCell(new PdfPCell(cellHeader1));

                var cellHeader3 = new PdfPCell(new Phrase(pdfConsoFamille.TotalConso + " " + Traduction.ParAn, arial));
                cellHeader3.Border = 0;
                cellHeader3.HorizontalAlignment = 2;
                tableHeader.AddCell(new PdfPCell(cellHeader3));

                var cellHeader4 = new PdfPCell(new Phrase(pdfConsoFamille.FamilleLibelleActuel + "  .  " + pdfConsoFamille.CategorieLibelleActuel, arial));
                cellHeader4.Colspan = 2;
                cellHeader4.BorderWidthTop = 0;
                cellHeader4.BorderWidthBottom = 1;
                cellHeader4.BorderWidthLeft = 0;
                cellHeader4.BorderWidthRight = 0;
                cellHeader4.HorizontalAlignment = 0;
                cellHeader4.PaddingBottom = 4;
                tableHeader.AddCell(new PdfPCell(cellHeader4));

                doc.Add(tableHeader);

                doc.Add(new Paragraph("\n"));

                var table = new PdfPTable(2);
                table.SetTotalWidth(new float[] { 300, 700 });
                table.TotalWidth = 800f;
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.DefaultCell.BorderWidth = 0;

                Image png1 = Image.GetInstance(adresseServeur + "/Content/images/Consommation/Payant/Categorie/" + idCategorie + ".png");
                var cell1 = new PdfPCell(png1);
                cell1.Colspan = 2;
                cell1.Border = 0;
                cell1.HorizontalAlignment = 1;
                table.AddCell(new PdfPCell(cell1));

                var cell2 = new PdfPCell(new Phrase(pdfConsoFamille.CategorieLibelleActuel));
                cell2.Colspan = 2;
                cell2.Border = 0;
                cell2.HorizontalAlignment = 1;
                table.AddCell(new PdfPCell(cell2));

                foreach (DetailConsommationViewModel detailConsommationViewModel in detailsDevice.DetailConsommations)
                {
                    var cell3 = new PdfPCell(new Phrase(detailConsommationViewModel.Libeller));
                    cell3.HorizontalAlignment = Element.ALIGN_BASELINE;
                    cell3.Border = 0;
                    table.AddCell(cell3);

                    Image png2 = Image.GetInstance(adresseServeur + "/Content/images/Consommation/greenBak2.png");
                    png2.ScaleAbsoluteWidth(detailConsommationViewModel.LongueurGrapheWattMilieu);
                    var cell4 = new PdfPCell(png2);
                    cell4.HorizontalAlignment = Element.ALIGN_BASELINE;
                    cell4.Border = 0;
                    table.AddCell(new PdfPCell(cell4));
                }

                var cell5 = new PdfPCell(new Phrase("%"));
                cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell5.Border = 0;
                table.AddCell(cell5);

                Image png3 = Image.GetInstance(adresseServeur + "/Content/images/Consommation/scaleHorizontal2.png");
                var cell6 = new PdfPCell(png3);
                cell6.HorizontalAlignment = Element.ALIGN_BASELINE;
                cell6.Border = 0;
                table.AddCell(new PdfPCell(cell6));

                doc.Add(table);

            }
            catch (Exception ex)
            {
                //Log error;
            }
            finally
            {
                doc.Close();
                HttpContext.Response.Write(doc);
                HttpContext.Response.End();
            }
        }
    }
}
