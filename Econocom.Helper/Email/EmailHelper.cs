using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Helper.Email
{
    public class EmailHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static string mailServer = ConfigurationManager.AppSettings["MailServer"];
        static string mailPort = ConfigurationManager.AppSettings["MailPort"];
        static string mailUserName = ConfigurationManager.AppSettings["MailUserName"];
        static string mailUserPassword = ConfigurationManager.AppSettings["MailUserPassword"];
        static string AccountVerificationUrl = ConfigurationManager.AppSettings["AccountVerificationUrl"];
        static string EmailSender = ConfigurationManager.AppSettings["MailSender"];
        static string ServerAddress = ConfigurationManager.AppSettings["ServerAddress"];

        /// <summary>
        /// Envoyer un email aux contacts apres la souscription
        /// </summary>
        /// <param name="emailListe"></param>
        /// <param name="contenuMail"></param>
        /// <param name="souscriptionViewModel"></param>
        /// <param name="tarif"></param>
        /// <returns></returns>
        public static bool SendRegistrationMailToContacts(List<string> emailListe, ContenuMail contenuMail, SouscriptionViewModel souscriptionViewModel, Tarif tarif)
        {
            var status = false;
            if (emailListe == null || emailListe.Count == 0)
                return status;
           var contenuHTML = contenuMail.Contenu;

            foreach (var email in emailListe)
            {
                try
                {          
                     contenuHTML = contenuMail.Contenu;
                    // Remplacer les variables avant l'envoi du mail
                    contenuHTML = RemplacerVariablesSouscription(contenuHTML, souscriptionViewModel, tarif);

                    // Envoyer email
                    MailMessage message = new MailMessage();
                    message = BuildMessage(EmailSender, email, contenuHTML, "", contenuMail.Sujet);

                    status = SendMail(message);
                }
                catch (Exception e)
                {
                    Logger.Error("SendRegistrationMailToContacts: Erreur lors de l'envoi de mail {0} a {1}", contenuHTML, email);
                    Logger.Error(e.StackTrace);
                }
            }
            return status;
        }

        public static bool EnvoyerMailSouscriptionAuWebmaster(string webMasterEmail, ContenuMail contenuMail, string souscripteur, string societe)
        {
            var status = false;
            var contenuHTML = contenuMail.Contenu;

            try
            {
                contenuHTML = contenuMail.Contenu;
                
                // Remplacer les variables avant l'envoi du mail
                contenuHTML = RemplacerVariablesMailWebmaster(contenuHTML, souscripteur, societe);

                // Envoyer email
                MailMessage message = new MailMessage();
                message = BuildMessage(EmailSender, webMasterEmail, contenuHTML, "", contenuMail.Sujet);

                status = SendMail(message);
            }
            catch (Exception e)
            {
                Logger.Error("EnvoyerMailSouscriptionAuWebmaster: Erreur lors de l'envoi de mail {0} a {1}", contenuHTML, webMasterEmail);
                Logger.Error(e.StackTrace);
            }
            
            return status;
        }


        /// <summary>
        /// Remplacer les variables du contenu mail de alerte nouvelle souscription au webmaster
        /// </summary>
        /// <param name="contenuHTML">contenu du mail</param>
        /// <param name="souscripteur">nom du souscripteur</param>
        /// <param name="societe">nom de la societe</param>
        /// <returns></returns>
        private static string RemplacerVariablesMailWebmaster(string contenuHTML, string souscripteur, string societe)
        {
            try
            {
                // Definir les pairs Key/Value à remplacer
                var remplacements = new Dictionary<string, string>();

                remplacements.Add("xxEnvironnement", ServerAddress);
                remplacements.Add("xxSouscripteur", souscripteur);
                remplacements.Add("xxSociete", societe);
                
                return RemplacerApresVerification(contenuHTML, remplacements);
            }
            catch (Exception e)
            {
                Logger.Error("RemplacerApresVerification contenu {0}", contenuHTML);
                Logger.Error(e.StackTrace);
                throw e;
            }

        }


        /// <summary>
        /// Remplacer les variables du contenu mail de souscription
        /// </summary>
        /// <param name="contenuHTML"></param>
        /// <param name="souscriptionViewModel"></param>
        /// <param name="tarif"></param>
        /// <returns></returns>
        private static string RemplacerVariablesSouscription(string contenuHTML, SouscriptionViewModel souscriptionViewModel, Tarif tarif)
        {
            try
            {
                // Definir les pairs Key/Value à remplacer
                var remplacements = new Dictionary<string, string>();
                var nombreDevices = tarif.NbreMinDevice + " - " + tarif.NbreMaxDevice;
                var redevanceAnnuelle = tarif.TarifAnnuel;//*12;

                // -------------------------------------  Les images  -------------------------------------
                remplacements.Add("/uploads/", ServerAddress + "/uploads/");
                remplacements.Add("xxServerAddress", ServerAddress);
                // ------------------------------------- Les libellés -------------------------------------
                // Titre
                remplacements.Add("xxSociete", Resource.Traduction.Societe);
                remplacements.Add("xxSouscripteur", Resource.Traduction.Souscripteur);
                remplacements.Add("xxAbonnement", Resource.Traduction.Abonnement);

                // Societe
                remplacements.Add("xxLblRaisonSociale", Resource.Traduction.RaisonSociale);
                remplacements.Add("xxLblAdressex", Resource.Traduction.Adresse);
                remplacements.Add("xxLblCP", Resource.Traduction.CP);
                remplacements.Add("xxLblVille", Resource.Traduction.Ville);
                remplacements.Add("xxLblTVA", Resource.Traduction.TVA);
                remplacements.Add("xxLblGroupe", Resource.Traduction.Groupe);
                remplacements.Add("xxLblPays", Resource.Traduction.Pays);
                remplacements.Add("xxLblIdentificationNational",
                                  Resource.Traduction.IdentificationNational);
                remplacements.Add("xxLblChiffreAffaires", Resource.Traduction.ChiffreAffaires);

                // Souscripteur
                remplacements.Add("xxLblNomx", Resource.Traduction.Nom);
                remplacements.Add("xxLblPrenom", Resource.Traduction.Prenom);
                remplacements.Add("xxLblFonction", Resource.Traduction.Fonction);
                remplacements.Add("xxLblAdresseMail", Resource.Traduction.AddresseMail);
                remplacements.Add("xxLblTelephone", Resource.Traduction.Telephone);

                // Autres
                remplacements.Add("xxLblNombreDevices", Resource.Traduction.NombreDevices);
                remplacements.Add("xxLblRedevanceAnnuelle", Resource.Traduction.RedevanceAnnuelle);

                // ------------------------------------- Les Valeurs --------------------------------------
                // Texte
                remplacements.Add("xxDate", DateTime.Now.ToShortDateString());
                remplacements.Add("xxTime", DateTime.Now.ToShortTimeString());

                // Societe
                remplacements.Add("xxRaisonSociale",
                                  souscriptionViewModel.SocieteInputSouscriptionViewModel.RaisonSociale);
                remplacements.Add("xxAdressex", souscriptionViewModel.SocieteInputSouscriptionViewModel.Addresse);
                remplacements.Add("xxCP", souscriptionViewModel.SocieteInputSouscriptionViewModel.CodePostal);
                remplacements.Add("xxVille", souscriptionViewModel.SocieteInputSouscriptionViewModel.Ville);
                remplacements.Add("xxTVA", souscriptionViewModel.SocieteInputSouscriptionViewModel.Tva);
                remplacements.Add("xxGroupe", souscriptionViewModel.SocieteInputSouscriptionViewModel.Groupe);
                remplacements.Add("xxPays", souscriptionViewModel.SocieteInputSouscriptionViewModel.Pays.LibellePays);
                remplacements.Add("xxIdentificationNational",
                                  souscriptionViewModel.SocieteInputSouscriptionViewModel.IdentificationNational);
                remplacements.Add("xxChiffreAffaires",
                                  souscriptionViewModel.SocieteInputSouscriptionViewModel.ChiffresAffaires.ToString());

                // Souscripteur
                remplacements.Add("xxNomx", souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Nom);
                remplacements.Add("xxPrenom", souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Prenom);
                remplacements.Add("xxFonction", souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Fonction);
                remplacements.Add("xxAdresseMail", souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Email);
                remplacements.Add("xxTelephone", souscriptionViewModel.SouscripteurInputSouscriptionViewModel.Telephone);

                // Autres
                remplacements.Add("xxNombreDevices", nombreDevices);
                remplacements.Add("xxRedevanceAnnuelle", redevanceAnnuelle.ToString());

                return RemplacerApresVerification(contenuHTML, remplacements);
            }
            catch (Exception e)
            {
                Logger.Error("RemplacerApresVerification contenu {0}", contenuHTML);
                Logger.Error(e.StackTrace);
                throw e;
            }
            
        }

        /// <summary>
        /// Remplacer les variables apres les vérifications
        /// </summary>
        /// <param name="contenuHTML"></param>
        /// <param name="remplacements"></param>
        /// <returns></returns>
        private static string RemplacerApresVerification(string contenuHTML, Dictionary<string, string> remplacements)
        {
            try
            {
                foreach (var remplacement in remplacements)
                {
                    contenuHTML = contenuHTML.Replace(remplacement.Key,
                                                      !string.IsNullOrEmpty(remplacement.Value)
                                                          ? remplacement.Value
                                                          : " - ");
                }
            }
            catch (Exception e)
            {
                Logger.Error("RemplacerApresVerification contenu {0}", contenuHTML);
                Logger.Error(e.StackTrace);
            }
            return contenuHTML;
        }

        /// <summary>
        /// Envoyer un email aux contacts validés par le modérateur
        /// </summary>
        /// <param name="emailListe"></param>
        /// <param name="contenuMail"></param>
        /// <param name="lien"></param>
        /// <returns></returns>
        public static bool SendMailModeration(Dictionary<string, string> emailListe, ContenuMail contenuMail, string lien)
        {
            var status = false;
            if (emailListe == null || emailListe.Count == 0)
                return status;

            // Definir les pairs Key/Value à remplacer
            var remplacements = new Dictionary<string, string>();
            
            foreach (var email in emailListe)
            {
                try
                {
                    var contenuHTML = contenuMail.Contenu;

                    remplacements.Clear();

                    // Construire le lien à envoyer
                    remplacements.Add("xxServerAddress", ServerAddress);
                    lien = RemplacerApresVerification(lien, remplacements);

                    // Ajouter email et code vérification
                    var lienEntier = lien + email.Key + "&vcode=" + email.Value;

                    // Ajouter html tag
                    lienEntier = "<a href=\"" + lienEntier + "\">MailLink</a>";
                    
                    // Remplacer le lien du contenu mail 
                    remplacements.Add("xxLink", lienEntier);
                    contenuHTML = RemplacerApresVerification(contenuHTML, remplacements);
                   
                    // Envoyer email
                    MailMessage message = new MailMessage();
                    message = BuildMessage(EmailSender, email.Key, contenuHTML, email.Value, contenuMail.Sujet);

                    if (message != null)
                    {
                        status = SendMail(message);
                    }
                    else
                    {
                        Logger.Error("SendMailModeration: Erreur lors de l'envoi de mail a {0}: message null", email);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("SendMailModeration: Erreur lors de l'envoi de mail a {0} ", email);
                    Logger.Error(e.StackTrace);
                    throw e;
                }
            }
            return status;
        }


        /// <summary>
        /// Envoyer un email aux contacts validés par le modérateur
        /// </summary>
        /// <param name="emailListe"></param>
        /// <param name="contenuMail"></param>
        /// <param name="lien"></param>
        /// <returns></returns>
        public static bool SendMailValidationFlow(Dictionary<string, string> emailListe, ContenuMail contenuMail, string lien, string noContrat)
        {
            var status = false;
            if (emailListe == null || emailListe.Count == 0)
                return status;

            // Definir les pairs Key/Value à remplacer
            var remplacements = new Dictionary<string, string>();

            foreach (var email in emailListe)
            {
                try
                {
                    var contenuHTML = contenuMail.Contenu;

                    remplacements.Clear();

                    // Construire le lien à envoyer
                    remplacements.Add("xxServerAddress", ServerAddress);
                    lien = RemplacerApresVerification(lien, remplacements);

                    // Ajouter email et code vérification
                    var lienEntier = lien + email.Key + "&vcode=" + email.Value;

                    // Ajouter html tag
                    lienEntier = "<a href=\"" + lienEntier + "\">MailLink</a>";

                    // Remplacer le lien du contenu mail 
                    remplacements.Add("xxLink", lienEntier);
                    remplacements.Add("xxNumeroContrat", noContrat);
                    contenuHTML = RemplacerApresVerification(contenuHTML, remplacements);

                    // Envoyer email
                    MailMessage message = new MailMessage();
                    message = BuildMessage(EmailSender, email.Key, contenuHTML, email.Value, contenuMail.Sujet);

                    if (message != null)
                    {
                        status = SendMail(message);
                    }
                    else
                    {
                        Logger.Error("SendMailValidationFlow: Erreur lors de l'envoi de mail a {0}: message null", email);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("SendMailValidationFlow: Erreur lors de l'envoi de mail a {0} ", email);
                    Logger.Error(e.StackTrace);
                    throw e;
                }
            }
            return status;
        }

        /// <summary>
        /// Envoyer l'email
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool SendMail(MailMessage message)
        {
            var smtpPort = 25;
            try
            {
                Int32.TryParse(mailPort, out smtpPort);
                var smtpClient = new SmtpClient(mailServer, smtpPort);
                var credentials = new System.Net.NetworkCredential(mailUserName, mailUserPassword);
                smtpClient.Credentials = credentials;
                Logger.Info("Sending mail to {0}", message.To);
                smtpClient.Send(message);
            }
            catch (SmtpFailedRecipientException e)
            {
                Logger.Error("SendMail body {0} to {1} using mailserver {2} port {3} from {4}", message.Body, message.To,
                             mailServer, smtpPort, EmailSender);
                Logger.Error(e.StackTrace);
                throw e;
            }
            catch (SmtpException e)
            {
                Logger.Error("SendMail body {0} to {1} using mailserver {2} port {3} from {4}", message.Body, message.To,
                             mailServer, smtpPort, EmailSender);
                Logger.Error(e.StackTrace);
                throw e;
            }           
            catch (Exception e)
            {
                Logger.Error("SendMail body {0} to {1} using mailserver {2} port {3} from {4}", message.Body, message.To,
                             mailServer, smtpPort, EmailSender);
                Logger.Error(e.StackTrace);
                throw e;
            }
            return true;
        }


        /// <summary>
        /// Construire le message à envoyer par email
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="body"></param>
        /// <param name="verificationCode"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static MailMessage BuildMessage(string from, string to, string body, string verificationCode, string subject)
        {
            try
            {
                var culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
                if (from == "")
                    from = EmailSender;

                var mailMessage = new MailMessage
                    {
                        From = new MailAddress(@from),
                        IsBodyHtml = true,
                        Subject = subject,
                        Body = body
                    };

                mailMessage.To.Add(new MailAddress(to));

                return mailMessage;
            }
            catch (Exception e)
            {
                Logger.Error("BuildMessage body {0} to {1} from {2} subject {3}", body, to, from, subject);
                Logger.Error(e.StackTrace);
                throw e;
            }
        }
    }
}
