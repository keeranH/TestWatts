using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Econocom.Model;
using Econocom.Model.Enum;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Web4.Controllers.ApiControllers;
using NLog;

namespace Econocom.Web4.Controllers
{
    public class SimulationController : BaseController
    {
        private ServiceApiController service;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SimulationController()
        {
            this.InitLanguageDropDown();
            service = new ServiceApiController();
        }


        /// <summary>
        /// Afficher page création nouvelle simulation
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
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

                //vérifier s'il y a une simulation en cours
                var simulation = service.GetSimulationEnCours(idContact);

                if (simulation == null)
                {
                    //Afficher message d'erreur s'il en est
                    ViewBag.ErreurLibelleSimulation = Session["ErreurLibelleSimulation"];
                    ViewBag.ErreurChoixPolitique = Session["ErreurChoixPolitique"];
                    Session.Remove("ErreurLibelleSimulation");
                    Session.Remove("ErreurChoixPolitique");

                    //récupérer liste politiques
                    var politiques = service.GetListePolitiques();
                    var simulationViewModel = new SimulationViewModel();                   

                    simulationViewModel.Politiques = politiques;
                    ViewBag.TypeContact = "Admin";
                    return View(simulationViewModel);
                }
                else
                {
                    Session.Add("userType", "Simulation");
                    return RedirectToRoute(new {controller = "Consommation", action = "Index", id = idClient});
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                return base.Erreur();
            }
        }

        /// <summary>
        /// Redirection dans l'espace Mes Simulation de l'espace Ma Personnalisation
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifierSimulation()
        {
            Logger.Debug("MaSimulation entrée: -");
            try
            {
                var typeUtilisateur = "";
                
                if (Session["userType"] != null && Session["idClient"] != null)
                {
                    typeUtilisateur = (string)Session["userType"];
                    Logger.Debug("Session : typeUtilisateur => {0}", typeUtilisateur);
                }
                else
                {
                    Logger.Debug("Session : userType & idClient : nulle => Redirection login");
                    return RedirectToLogin();
                }

                //MAJ de la valeur de 'usertype' a payant quand on clique sur "Ma Personnalisation"
                if (typeUtilisateur.Equals("Payant"))
                {
                    //Retirer les valeurs pour modification qty device de la session
                    Session.Remove("CurrentCategorie");
                }

                Logger.Debug("MaSimulation : end");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }
        

        /// <summary>
        /// Sauvegarder une nouvelle simulation et redirection sur la page affichage consommation famille
        /// </summary>
        /// <param name="simulationViewModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(SimulationViewModel simulationViewModel)
        {
            try
            {
                if (simulationViewModel.PolitiqueId == null)
                {
                    ModelState.AddModelError("simulationViewModel.PolitiqueId", "ErreurChoixPolitique");
                    ViewBag.ErreurChoixPolitique = Resource.Traduction.MsgErreurChoixPolitique;
                }

                if (string.IsNullOrEmpty(simulationViewModel.LibelleSimulation))
                {
                    ModelState.AddModelError("simulationViewModel.LibelleSimulation", "ErreurLibelleSimulation");
                    ViewBag.ErreurLibelleSimulation = Resource.Traduction.MsgErreurLibelleSimulation;
                }

                //vérifier si l'état de modèle est valable
                if (ModelState.IsValid)
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

                    //créer nouveau simulation
                    var simulation = service.CreerNouvelleSimulation(idClient, idContact, simulationViewModel);
                    
                    //Redirection sur la page d'affichage conso par famille
                    Session.Add("userType", "Simulation");
                    return RedirectToRoute(new {controller = "Consommation", action = "Index", id = idClient});
                }
                else
                {
                    if (simulationViewModel.Politiques == null)
                    {
                        simulationViewModel.Politiques = service.GetListePolitiques();
                    }
                    ViewBag.TypeContact = "Admin";
                    return View(simulationViewModel);
                }

            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

      
        /// <summary>
        /// Afficher pop up detail politique
        /// </summary>
        /// <param name="politiqueId"></param>
        /// <returns></returns>
        public ActionResult DetailPolitique(int politiqueId)
        {
            try
            {
                //récupérer détail d'une politique et l'afficher 
                var politique = service.GetPolitiqueParId(politiqueId);                
                return Json(new {value = politique}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }


        /// <summary>
        /// Générer rapport simulation
        /// </summary>
        /// <param name="idFamille"></param>
        /// <param name="idCategorie"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ActionResult GenererRapportSimulation(int? idFamille, int? idCategorie)
        {
            try
            {
                //récupérer configSi pour client où statut = moderer
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

                //générer rapport simulation
                var genererRapport = service.GenererRapportSimulation(idClient, idContact);
                Session.Add("genererRapport", "Faux");

                //return RedirectToAction("Index");
                return RedirectToRoute(new { controller = "Rapport", action = "Simulations", id = idClient });
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }


        /// <summary>
        /// Terminer une simulation en cours et créé une nouvelle simulation
        /// </summary>
        /// <returns></returns>
        public ActionResult CreationNouvelleSimulation()
        {
            try
            {
                var idContact = 0;
                if (Session["idContact"] != null)
                {
                    idContact = (int)Session["idContact"];
                }
                else
                {
                    var contactEmail = User.Identity.Name;
                    if (contactEmail != string.Empty)
                    {
                        var contact = service.GetContactParEmail(contactEmail);
                        idContact = contact.Id;
                    }
                }

                //vérifier s'il y a une simulation en cours
                var simulation = service.GetSimulationEnCours(idContact);

                //définir l'état de la simulation à terminer
                if (simulation != null)
                {
                    simulation.Statut = (int) SimulationStatut.Terminer;

                    //MAJ simulation
                    simulation = service.MAJSimulation(simulation);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
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
                return JavaScript("window.location = '" + Url.Action("Index", "Home", new { lang = currentLanguage }) + "'");
            else
                return RedirectToAction("Index", "Home", new { lang = currentLanguage });
        }
    }
}
