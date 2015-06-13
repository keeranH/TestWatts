using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel;
using Econocom.Web4.Controllers.ApiControllers;
using RazorPDF;
using Rotativa;

namespace Econocom.Web4.Controllers
{
    /// <summary>
    /// Controller pour CompteEvaluation
    /// </summary>
    public class EvaluationController : BaseController
    {
        private ServiceApiController service;
        private Langue langueChoisi;

         public EvaluationController()
         {
             this.InitLanguageDropDown();
            service = new ServiceApiController();
         }

         /// <summary>
         /// Afficher la page pour la creation d'un compte d'évaluation
         /// </summary>
         /// <returns></returns>
         public ActionResult Index()
         {
             var evaluationViewModel = new EvaluationViewModel();

             try
             {
                 //Récupérer le contenu modéré pour cette page
                 var culture = Econocom.Helper.Url.UrlHelper.GetCultureFromUrl(Request.Path);
                 langueChoisi = service.GetLangueByCode(culture.TwoLetterISOLanguageName);

                 if (langueChoisi != null)
                 {
                     base.InitierContenu("Evaluation/Index", "Index", langueChoisi.Culture);
                 }
                 else
                 {

                     base.InitierContenu("Evaluation/Index", "Index", "fr-FR");
                 }

                 //Liste déroulante pour choix de pays
                 var pays = service.ListePays();
                 evaluationViewModel.ListePays = pays;

                 //Liste déroulante pour choix de secteur d'activité
                 var secteurActivites = service.GetListeSecteurActivite();

                 evaluationViewModel.ListeSecteurActivite = secteurActivites;

                 //Liste déroulante pour choix de devise
                 var devises = service.GetListeDevise();
                 evaluationViewModel.ListeDevise = devises;
             }
             catch (Exception e)
             {
                 return base.Erreur();
             }
             return View(evaluationViewModel);
         }

         /// <summary>
         /// Methode pour valider les détails du compte d'évaluation et rediriger sur la page TermesEtConditions
         /// </summary>
         /// <param name="evaluationViewModel"></param>
         /// <returns></returns>
         /// <exception cref="Exception"></exception>
         [HttpPost]
         [ValidateAntiForgeryToken]
         [ValidateInput(false)]
         public ActionResult Index(EvaluationViewModel evaluationViewModel)
         {
             try
             {
                 //Liste déroulante pour choix de pays
                 var pays = service.ListePays();                 
                 evaluationViewModel.ListePays = pays;

                 //Liste déroulante pour choix de secteur d'activité
                 var secteurActivites = service.GetListeSecteurActivite();
                 evaluationViewModel.ListeSecteurActivite = secteurActivites;

                 //Liste déroulante pour choix de devise
                 var devises = service.GetListeDevise();
                 evaluationViewModel.ListeDevise = devises;

                 if (evaluationViewModel.PaysId == null)
                 {
                     ModelState.AddModelError("PaysId", "ErrorRequired");
                 }

                 if (evaluationViewModel.SecteurActiviteId == null)
                 {
                     ModelState.AddModelError("SecteurActiviteId", "ErrorRequired");
                 }

                 if (!evaluationViewModel.Accepter)
                 {
                     ModelState.AddModelError("Accepter", "ErrorRequired");
                     ViewBag.Accepter = "ErrorRequired";
                 }
                 //Vérifier si le modèle est valide
                 if (ModelState.IsValid)
                 {
                     try
                     {
                         //Garder les détails du compte d'évaluation dans une session
                         //Session.Add("detailsEvaluation", evaluationViewModel);                         
                         //Redirection sur la page TermesEtConditions
                         //var contenu = base.GetContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");                        
                         //return View("TermesEtConditions",contenu);

                         var compteEvaluation = new CompteEvaluation();

                         compteEvaluation.RaisonSociale = evaluationViewModel.RaisonSociale;
                         compteEvaluation.Groupe = evaluationViewModel.Groupe;
                         compteEvaluation.PaysId = evaluationViewModel.PaysId;
                         compteEvaluation.Nom = evaluationViewModel.Nom;
                         compteEvaluation.Prenom = evaluationViewModel.Prenom;
                         compteEvaluation.Fonction = evaluationViewModel.Fonction;
                         compteEvaluation.Email = evaluationViewModel.Email;
                         compteEvaluation.NumeroPhone = evaluationViewModel.NumeroPhone;

                         compteEvaluation.SecteurActiviteId = evaluationViewModel.SecteurActiviteId;
                         compteEvaluation.Effectif = evaluationViewModel.Effectifs;
                         compteEvaluation.TauxEquipement = evaluationViewModel.TauxEquipement;
                         compteEvaluation.NombreSites = evaluationViewModel.NombreSites;
                         compteEvaluation.PrixMoyenKwatt = evaluationViewModel.PrixMoyenKwatt;
                         compteEvaluation.DeviseId = evaluationViewModel.DeviseId;

                         compteEvaluation = service.SauvegardeEvaluation(compteEvaluation);
                         if (compteEvaluation.Id == 0)
                         {
                             return View("Index", evaluationViewModel);
                         }
                         Session.Remove("detailsEvaluation");
                         Session.Add("userType", "Demo");
                         Session.Add("idClient", compteEvaluation.Id);
                         return
                             RedirectToRoute(
                                 new {controller = "Consommation", action = "Index", id = compteEvaluation.Id});
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
                     //Récupérer le contenu modéré pour cette page                   
                     base.InitierContenu("Evaluation/Index", "Index", "fr-FR");

                     //Recharger la même page avec les messages d'erreur 
                     return View(evaluationViewModel);
                 }
             }
             catch (Exception e)
             {
                 return base.Erreur();
             }
         }

         /// <summary>
         /// Afficher la page TermesEtConditions
         /// </summary>
         /// <returns></returns>
         public ActionResult TermesEtConditions()
         {
             try
             {
                 //Récupérer le contenu modéré pour cette page
                 //base.InitierContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");
                 var contenu = base.GetContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");                        
                 return View("TermesEtConditions",contenu);
             }
             catch (Exception)
             {
                 return base.Erreur();
             }
             return View();
         }

         /// <summary>
         /// Sauvegarder les données pour créer un compte d'évaluation
         /// </summary>
         /// <param name="id">Si id = 1, Accepter. Si id = 0, Rejeter</param>
         /// <returns></returns>
         public ActionResult Conditions(int id)
         {
            try
            {
                var evaluationViewModel = (EvaluationViewModel)Session["detailsEvaluation"];
                var compteEvaluation = new CompteEvaluation();

                if (id == 1)
                {
                    compteEvaluation.RaisonSociale = evaluationViewModel.RaisonSociale;
                    compteEvaluation.Groupe = evaluationViewModel.Groupe;
                    compteEvaluation.PaysId = evaluationViewModel.PaysId;
                    compteEvaluation.Nom = evaluationViewModel.Nom;
                    compteEvaluation.Prenom = evaluationViewModel.Prenom;
                    compteEvaluation.Fonction = evaluationViewModel.Fonction;
                    compteEvaluation.Email = evaluationViewModel.Email;
                    compteEvaluation.NumeroPhone = evaluationViewModel.NumeroPhone;

                    compteEvaluation.SecteurActiviteId = evaluationViewModel.SecteurActiviteId;
                    compteEvaluation.Effectif = evaluationViewModel.Effectifs;
                    compteEvaluation.TauxEquipement = evaluationViewModel.TauxEquipement;
                    compteEvaluation.NombreSites = evaluationViewModel.NombreSites;
                    compteEvaluation.PrixMoyenKwatt = evaluationViewModel.PrixMoyenKwatt;
                    compteEvaluation.DeviseId = evaluationViewModel.DeviseId;

                    compteEvaluation = service.SauvegardeEvaluation(compteEvaluation);
                    if (compteEvaluation.Id == 0)
                    {
                        return View("Index", evaluationViewModel);
                    }
                    Session.Remove("detailsEvaluation");
                }
                else
                {
                    Session.Remove("detailsEvaluation");
                    return RedirectToRoute(new { controller = "Home", action = "Index" });

                }
                Session.Add("userType", "Demo");
                Session.Add("idClient", compteEvaluation.Id);
                return RedirectToRoute(new { controller = "Consommation", action = "Index", id = compteEvaluation.Id });
            }
            catch (Exception e)
            {
                return base.Erreur();
            }
             
         }

         public ActionResult TermesEtConditionsPartial()
         {
             try
             {
                 var contenu = base.GetContenu("Evaluation/TermesEtConditions", "TermesEtConditions", "fr-FR");                 
                 //return new PartialViewAsPdf();
                 return new RazorPDF.PdfResult(contenu, "TermesEtConditionsPartial");
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

         public ActionResult PDF()
         {
             try
             {
                 return new ActionAsPdf("TermesEtConditionsPartial") { FileName = "TermesEtConditions.pdf" };
             }
             catch (Exception e)
             {
                 throw e;
             }
         }
         public ActionResult Info()
         {
             try
             {
                 base.InitierContenu("Evaluation/Info", "Info", "fr-FR");
                 return View();
             }
             catch (Exception e)
             {
                 throw e;
             }
         }

    }
}
