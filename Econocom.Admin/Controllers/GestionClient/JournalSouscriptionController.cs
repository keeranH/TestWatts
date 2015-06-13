using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Helper.Rapport;
using Econocom.Model.Enum;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.Report;
using NLog;

namespace Econocom.Admin.Controllers.GestionClient
{
    [Authorize]
    public class JournalSouscriptionController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;
        public static int PageSize = 10;

        public JournalSouscriptionController()
        {
            _service = new ServiceApiController();
            PageSize = 10;
        }

        public ActionResult Index()
        {
            try
            {
                Session.Remove("DatesChoisies");
                var choixDate = new ChoixDateViewModel();
                choixDate.DateDebut = DateTime.Now;
                choixDate.DateFin = DateTime.Now;
                return View(choixDate);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public ActionResult AfficherListeClients(ChoixDateViewModel choixDate, int? page)
        {
            try
            {
                if (choixDate != null && choixDate.DateDebut > new DateTime() && choixDate.DateFin > new DateTime())
                    Session.Add("DatesChoisies", choixDate);

                if (Session["DatesChoisies"] != null && (choixDate==null || choixDate.DateDebut == new DateTime() || choixDate.DateFin == new DateTime()))
                    choixDate = (ChoixDateViewModel) Session["DatesChoisies"];
                

                var pageDimension = 10;

                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                var pageDimensionValue = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PageSize"])
                                             ? 10
                                             : pageDimension;

                int totalPages = 1;
                var listeClientsSouscrits = _service.GetListeClientsSouscrits(choixDate.DateDebut, choixDate.DateFin, page, pageDimension, out totalPages);
                ViewBag.Total = totalPages;

                var clientsSouscrits = ListeJournalSouscriptions(listeClientsSouscrits);

                return View("ClientsSouscrits", clientsSouscrits);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public List<JournalSouscriptionViewModel> ListeJournalSouscriptions(List<Client> listeClientsSouscrits)
        {
            try
            {
                var clientsSouscrits = new List<JournalSouscriptionViewModel>();
                if (listeClientsSouscrits != null)
                {
                    foreach (var client in listeClientsSouscrits)
                    {
                        var journalSouscription = new JournalSouscriptionViewModel();

                        journalSouscription.IdClient = client.Id;

                        var statut = "";
                        switch (client.Statut)
                        {
                            case 0:
                                statut = "En cours";
                                break;

                            case 1:
                                statut = "Validé";
                                break;

                            case 2:
                                statut = "Refusé";
                                break;

                            default:
                                statut = "En cours";
                                break;
                        }

                        journalSouscription.DateSouscription = client.DateDebut;
                        journalSouscription.DateValidation = client.DateValidation;
                        journalSouscription.Statut = statut;
                        journalSouscription.Pays = client.Adresse.Pays.LibellePays;
                        journalSouscription.Groupe = client.Groupe;
                        journalSouscription.IdentificationNational = client.IdentificationNational;
                        journalSouscription.RaisonSociale = client.RaisonSociale;
                        journalSouscription.NoTva = client.TVAIntraComm;
                        journalSouscription.ClientEconocom = client.ClientEconocom;
                        journalSouscription.Adresse = client.Adresse.Adresse1;
                        journalSouscription.CodePostale = client.Adresse.CodePostal;
                        journalSouscription.Ville = client.Adresse.Ville;
                        journalSouscription.ChiffreAffaire = client.DetailsClient.ChiffreAffaire;

                        //details commercial
                        var commercial =
                            client.Contacts.Where(
                                o => o.TypeContact.LibelleTypeContact.Equals(TypeContactEnum.Commercial.ToString()));
                        journalSouscription.MailCommercial = commercial.Any() ? commercial.Single().Email : "";

                        //details souscripteur
                        var souscripteur =
                            client.Contacts.Where(
                                o => o.TypeContact.LibelleTypeContact.Equals(TypeContactEnum.Souscripteur.ToString()));
                        journalSouscription.NomSouscripteur = souscripteur.Any() ? souscripteur.Single().NomContact : "";
                        journalSouscription.PrenomSouscripteur = souscripteur.Any()
                                                                     ? souscripteur.Single().PrenomContact
                                                                     : "";
                        journalSouscription.FonctionSouscripteur = souscripteur.Any()
                                                                       ? souscripteur.Single().Fonction
                                                                       : "";
                        journalSouscription.MailSouscripteur = souscripteur.Any() ? souscripteur.Single().Email : "";
                        journalSouscription.NumeroTelephoneSouscripteur = souscripteur.Any()
                                                                              ? souscripteur.Single().NumeroPhone
                                                                              : "";

                        //details administrateur
                        var administrateur =
                            client.Contacts.Where(
                                o => o.TypeContact.LibelleTypeContact.Equals(TypeContactEnum.Administrateur.ToString()));
                        journalSouscription.NomAdministrateur = administrateur.Any()
                                                                    ? administrateur.Single().NomContact
                                                                    : "";
                        journalSouscription.PrenomAdministrateur = administrateur.Any()
                                                                       ? administrateur.Single().PrenomContact
                                                                       : "";
                        journalSouscription.FonctionAdministrateur = administrateur.Any()
                                                                         ? administrateur.Single().Fonction
                                                                         : "";
                        journalSouscription.MailAdministrateur = administrateur.Any()
                                                                     ? administrateur.Single().Email
                                                                     : "";
                        journalSouscription.NumeroTelephoneAdministrateur = administrateur.Any()
                                                                                ? administrateur.Single().NumeroPhone
                                                                                : "";

                        //details utilisateurs
                        var utilisateurs =
                            client.Contacts.Where(
                                o => o.TypeContact.LibelleTypeContact.Equals(TypeContactEnum.Utilisateur.ToString()))
                                  .ToList();
                        switch (utilisateurs.Count)
                        {
                            case 0:
                                journalSouscription.MailUser1 = "";
                                journalSouscription.MailUser2 = "";
                                journalSouscription.MailUser3 = "";
                                journalSouscription.MailUser4 = "";
                                journalSouscription.MailUser5 = "";
                                break;
                            case 1:
                                journalSouscription.MailUser1 = utilisateurs.ElementAt(0).Email;
                                journalSouscription.MailUser2 = "";
                                journalSouscription.MailUser3 = "";
                                journalSouscription.MailUser4 = "";
                                journalSouscription.MailUser5 = "";
                                break;
                            case 2:
                                journalSouscription.MailUser1 = utilisateurs.ElementAt(0).Email;
                                journalSouscription.MailUser2 = utilisateurs.ElementAt(1).Email;
                                journalSouscription.MailUser3 = "";
                                journalSouscription.MailUser4 = "";
                                journalSouscription.MailUser5 = "";
                                break;
                            case 3:
                                journalSouscription.MailUser1 = utilisateurs.ElementAt(0).Email;
                                journalSouscription.MailUser2 = utilisateurs.ElementAt(1).Email;
                                journalSouscription.MailUser3 = utilisateurs.ElementAt(2).Email;
                                journalSouscription.MailUser4 = "";
                                journalSouscription.MailUser5 = "";
                                break;
                            case 4:
                                journalSouscription.MailUser1 = utilisateurs.ElementAt(0).Email;
                                journalSouscription.MailUser2 = utilisateurs.ElementAt(1).Email;
                                journalSouscription.MailUser3 = utilisateurs.ElementAt(2).Email;
                                journalSouscription.MailUser4 = utilisateurs.ElementAt(3).Email;
                                journalSouscription.MailUser5 = "";
                                break;
                            case 5:
                                journalSouscription.MailUser1 = utilisateurs.ElementAt(0).Email;
                                journalSouscription.MailUser2 = utilisateurs.ElementAt(1).Email;
                                journalSouscription.MailUser3 = utilisateurs.ElementAt(2).Email;
                                journalSouscription.MailUser4 = utilisateurs.ElementAt(3).Email;
                                journalSouscription.MailUser5 = utilisateurs.ElementAt(4).Email;
                                break;
                            default:
                                journalSouscription.MailUser1 = "";
                                journalSouscription.MailUser2 = "";
                                journalSouscription.MailUser3 = "";
                                journalSouscription.MailUser4 = "";
                                journalSouscription.MailUser5 = "";
                                break;
                        }


                        journalSouscription.Tarification = client.DetailsClient.Tarif.TarifAnnuel.ToString();
                        journalSouscription.DateSouscription = client.DateDebut;
                        journalSouscription.DateValidation = client.DateModification;
                        clientsSouscrits.Add(journalSouscription);
                    }
                }
                return clientsSouscrits;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
            return null;
        }

        public FileResult GetListeSouscriptions()
        {
            try
            {
                var choixDate = (ChoixDateViewModel)Session["DatesChoisies"];
                var listeSouscriptions = _service.GetListeSouscriptions(choixDate.DateDebut, choixDate.DateFin);

                if (listeSouscriptions != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                        {
                            streamWriter.WriteLine(Econocom.Resource.Traduction.ListeClientsSouscritsEntre +" "+ choixDate.DateDebut.ToString("dd/MM/yyyy") + " " + Econocom.Resource.Traduction.Et + " " + choixDate.DateFin.ToString("dd/MM/yyyy"));
                                
                            var csvWriter1 = new CsvWriter(streamWriter);
                            csvWriter1.Configuration.Delimiter = ";";
                            const string nomFichier = "{0}_{1}_{2}_{3}.csv";
                            var nomFichierFinale = String.Format(nomFichier, Econocom.Resource.Traduction.ListeClientsSouscritsEntre,
                                                                choixDate.DateDebut.ToString("dd/MM/yyyy"), Econocom.Resource.Traduction.Et, choixDate.DateFin.ToString("dd/MM/yyyy"));

                            csvWriter1.WriteHeader(typeof(JournalSouscriptionViewModel));
                            var clientsSouscrits = ListeJournalSouscriptions(listeSouscriptions);
                            clientsSouscrits.ForEach(csvWriter1.WriteRecord);

                            streamWriter.Flush();
                            return File(new MemoryStream(stream.ToArray()), "text/csv", nomFichierFinale);
                        }
                    }
                }
                else
                    Logger.Info("Liste Souscriptions est nulle");
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }

            return null;
        }

    }
}
