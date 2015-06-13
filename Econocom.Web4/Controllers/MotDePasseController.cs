using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Econocom.Helper.Email;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Web4.Controllers.ApiControllers;
using System.Net.Mail;

namespace Econocom.Web4.Controllers
{
    /// <summary>
    /// Controller pour CompteEvaluation
    /// </summary>
    public class MotDePasseController : BaseController
    {
        private ServiceApiController service;
        
        public MotDePasseController()
        {
            this.InitLanguageDropDown();
            service = new ServiceApiController();
        }

        /// <summary>
        /// Afficher la page pour la modification / réinitialisation du mot de passe
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var motDePasseModifierViewModel = new MotDePasseModifierViewModel();
            var motDePasseOublieViewModel = new MotDePasseOublieViewModel();
            
            try
            {
                if (Session["MdpDonneesSaisies_Modifier"] != null)
                    motDePasseModifierViewModel = (MotDePasseModifierViewModel)Session["MdpDonneesSaisies_Modifier"];

                var contactEmail = User.Identity.Name;
                if (contactEmail != string.Empty)
                {
                    var contact = service.GetContactParEmail(contactEmail);

                    motDePasseModifierViewModel.Contact = contact;
                    motDePasseModifierViewModel.IdContact = contact.Id;

                    // Detail Reponse
                    var reponse = new Reponse();
                    reponse = service.GetReponse(contact.Id);
                    motDePasseModifierViewModel.Reponse = reponse;

                    // Detail Question
                    var question = service.GetQuestionParDefaut();
                    if (reponse != null)
                    {
                        question = service.GetQuestion(reponse.QuestionId);
                    }
                    motDePasseModifierViewModel.Question = question;
                    motDePasseModifierViewModel.IdQuestion = question.Id;

                    Session.Remove("MdpDonneesSaisies_Modifier"); 
                    return View(motDePasseModifierViewModel);
                }
                else
                {
                    if (Session["MdpDonneesSaisies_Oublie"] != null)
                        motDePasseOublieViewModel = (MotDePasseOublieViewModel)Session["MdpDonneesSaisies_Oublie"];

                    // Liste des questions
                    var questions = service.GetListeQuestion();                    
                    motDePasseOublieViewModel.Questions = questions;

                    Session.Remove("MdpDonneesSaisies_Oublie"); 
                    return View(motDePasseOublieViewModel);
                }
            }
            catch (Exception e)
            {
                return base.Erreur();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [OutputCache(Duration = 0)]
        public ActionResult ModifierMdp(MotDePasseModifierViewModel motDePasseViewModel)
        {
            try
            {
                var contactEmail = User.Identity.Name;
                var contact = service.GetContactParEmail(contactEmail);

                Session.Add("MdpDonneesSaisies_Modifier", motDePasseViewModel);

                // Détail Reponse
                var reponse = service.GetReponse(contact.Id);

                if (reponse == null)
                {
                    reponse = new Reponse();
                    reponse.Valeur = motDePasseViewModel.ReponseSaisie;
                    reponse.ContactId = motDePasseViewModel.IdContact;
                    reponse.QuestionId = motDePasseViewModel.IdQuestion;
                    reponse = service.SauvegardeReponse(reponse);
                }

                // Detail Question (Afficher la question en cas d'erreur)
                var question = service.GetQuestion(reponse.QuestionId);
                question.LibelleQuestion = Resource.Traduction.ResourceManager.GetString("Question" + question.Id) ?? question.LibelleQuestion;
                motDePasseViewModel.Question = question;

                //Vérifier si le modèle est valide
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Vérifier si la reponse est correcte
                        if (reponse.Valeur == motDePasseViewModel.ReponseSaisie)
                        {
                            // Vérifier l'ancient password
                            if (Security.HashHelper.HashPassWordCompare(contact.MotPasse, motDePasseViewModel.AncientMotDePasse))
                            {
                                // Vérifier si les champs Nouveau mot de passe et Confirmer mot de passe sont identiques
                                if (motDePasseViewModel.NouveauMotDePasse == motDePasseViewModel.ConfirmerMotDePasse)
                                {
                                    // Modifier Mot de Passe
                                    var mdp = motDePasseViewModel.NouveauMotDePasse;
                                    mdp = Security.HashHelper.HashPassWord(mdp);

                                    contact.MotPasse = mdp;
                                    service.UpdateContact(contact);
                                    ViewBag.ErrorMessage = Resource.Traduction.MdpModifie;
                                    motDePasseViewModel = new MotDePasseModifierViewModel();
                                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                    Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                                    Response.Cache.SetNoStore();
                                }
                                else
                                {
                                    ViewBag.ErrorMessage = Resource.Traduction.ConfirmationMdpIncorrrect;
                                }
                            }
                            else
                            {
                                ViewBag.ErrorMessage = Resource.Traduction.AncientMdpIncorrect;
                            }
                        }
                        else
                        {
                            ViewBag.ErrorMessage = Resource.Traduction.ReponseIncorrecte;
                        }
                    } 
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    //Détecter les erreurs sur les champs obligatoires
                    foreach (var valeur in ModelState.Values)
                    {
                        foreach (var erreur in valeur.Errors)
                        {
                            //Vérification champs obligatoires
                            if (erreur.ErrorMessage.Equals("ErrorRequired"))
                            {
                                ViewBag.ErrorMessage = Resource.Traduction.ErreurChampObligatoire;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return base.Erreur();
            }
            return PartialView("MenuPartial/_MotDePasseModifierPartial", motDePasseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnvoyerMdp(MotDePasseOublieViewModel motDePasseOublieViewModel)
        {
            try
            {
                Session.Add("MdpDonneesSaisies_Oublie", motDePasseOublieViewModel);

                // Liste des questions
                var questions = service.GetListeQuestion();                
                motDePasseOublieViewModel.Questions = questions;

                //Vérifier si le modèle est valide
                if (ModelState.IsValid)
                {
                    try
                    {
                        var contact = service.GetContactParEmail(motDePasseOublieViewModel.EmailSaisi);

                        if (contact != null)
                        {
                            // Détail Reponse
                            var reponse = new Reponse();
                            reponse = service.GetReponse(contact.Id);
                            contact.CodeVerification = Guid.NewGuid().ToString();
                            service.UpdateContact(contact);
                            // Verifier la reponse de la question secrete
                            if (reponse.Valeur == motDePasseOublieViewModel.ReponseSaisie)
                            {
                                var emailList = new Dictionary<string, string>();

                                emailList.Add(contact.Email, contact.CodeVerification);
                                //var LangueChoisi = (Langue)Session["LangueChoisi"];
                                var codeLangue = "fr";

                                // Recuperer le sujet et le contenu du mail
                                var typeMail = service.GetTypeMail("Reinitialisation");
                                var contenuMail = service.GetContenuMail(typeMail.Id, 1);       // Hard-coded

                                var lien = "xxServerAddress/" + codeLangue + "/Home/GestionMotDePasse?email=";
                               
                                // Envoyer email
                               // MailMessage message = new MailMessage();
                                //message = EmailHelper.BuildMessage("", contact.Email, Resource.Home.Home.EmailContenu + contact.MotPasse, "", 
                                    //Resource.Home.Home.EmailSujet);

                                // Statut 
                                var statutMsg = "";
                                if (!EmailHelper.SendMailModeration(emailList, contenuMail, lien))
                                    statutMsg = "Erreur sur l'envoi mail";
                                else
                                    statutMsg = Resource.Traduction.MdpEnvoye + contact.Email;
                                
                                ViewBag.ErrorMessage = statutMsg;
                            }
                            else
                            {
                                // Mauvaise réponse
                                ViewBag.ErrorMessage = Resource.Traduction.ReponseIncorrecte;
                            }
                        }
                        else
                        {
                            // Contact n'existe pas
                            ViewBag.ErrorMessage = Resource.Traduction.ContactNonExistant;
                        }
                    }
                    catch (Exception e)
                    {
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
                                ViewBag.ErrorMessage = Resource.Traduction.ErreurChampObligatoire;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return base.Erreur();
            }
            return PartialView("MenuPartial/_MotDePasseOubliePartial", motDePasseOublieViewModel);
        }
    }
}
