using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CsvHelper;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Helper.Email;
using Econocom.Helper.File;
using Econocom.Model.Enum;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Service;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.Report;
using NLog;
using Econocom.Helper.Rapport;

namespace Econocom.Admin.Controllers.GestionClient
{
    [Authorize]
    public class ClientController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ServiceApiController _serviceApi;
        public static int PageSize = 10;
        public ClientController()
        {
            _serviceApi = new ServiceApiController();            
            PageSize = 10;
        }
        
        //
        // GET: /Client/
        public ActionResult Index(int? id, string search, int? page, string sort, string sortdir)
        {
            var sameController = Econocom.Helper.Url.UrlHelper.ReferrerController(Request.UrlReferrer, HttpContext.Request.RequestContext.RouteData);

            if (!sameController)
                Session["search"] = null;

            var listeClientViewModel = new ListeClientViewModel();
            try
            {
                if (search != null)
                    Session["search"] = search;
                else
                {
                    if (Session["search"] != null)
                        search = Session["search"].ToString();
                }

                var pageDimension = 10;
                int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);
                
                int totalPages = 1;
                listeClientViewModel = _serviceApi.GetListeClients(id, search, page, sort, sortdir, pageDimension, out totalPages);                
            }
            catch (Exception e)
            {
                Logger.Error("id={0}, recherche={1}, page={2}, sort={3}, sortdir={4}", id, search, page, sort, sortdir);
                Logger.Error(e.StackTrace);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Clients", listeClientViewModel);
            }
            else
                return View(listeClientViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult GetRapportClient(int id)
        {
            try
            {
                var rapport = _serviceApi.GetRapportClient(id);

                if (rapport != null)
                {
                    var liste = rapport.ConfigSi.ConfigSiDevices.ToList();
                    var rapportHelper = new RapportHelper();
                    var result = rapportHelper.GetRapportCsv(rapport);
                    if (result != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                            {
                                streamWriter.WriteLine(Econocom.Resource.Traduction.TitreRapportClient);
                                streamWriter.WriteLine(Econocom.Resource.Traduction.Date + ";" + result.DateRapport);
                                streamWriter.WriteLine(Econocom.Resource.Traduction.RaisonSociale + ";" + result.NomRapport);

                                var csvWriter1 = new CsvWriter(streamWriter);
                                csvWriter1.Configuration.Delimiter = ";";
                                const string nomFichier = "{0}_{1}_{2}.csv";
                                var date = DateTime.Now.ToString("yyyyMMddHHmmss");
                                var nomFichierFinale = String.Format(nomFichier, Resource.Traduction.TitreRapportClient.Replace(" ", "-"),
                                                                     result.NomRapport.Replace(" ", "-"), date);

                                csvWriter1.WriteHeader(typeof(RapportConfigViewModel));
                                result.Personnalisation.ForEach(csvWriter1.WriteRecord);

                                streamWriter.Flush();
                                return File(new MemoryStream(stream.ToArray()), "text/csv", nomFichierFinale);
                            }
                        }
                    }
                }
                else
                    Logger.Info("rapport id={0} is null", id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }

            return null;
        }

        public ActionResult Souscription()
        {
            SouscriptionViewModel svm = new SouscriptionViewModel();
            var listeTarif = _serviceApi.GetListeTarif();

            foreach (var tarif in listeTarif)
            {
                tarif.LibelleTarif = tarif.LibelleTarif + ": de " + tarif.NbreMinDevice + " à " + tarif.NbreMaxDevice +
                                     " devices ( " + tarif.TarifAnnuel + "€ )";
            }
            svm.ListTarif = listeTarif;

            SocieteSouscriptionViewModel societeModel = new SocieteSouscriptionViewModel();

            svm.SocieteInputSouscriptionViewModel = societeModel;
            societeModel.ListPays = _serviceApi.GetPays();

            return View(svm);
        }


        [HttpPost]
        public ActionResult SauvegarderSouscription(SouscriptionViewModel svm)
        {
            try
            {
                var tarifId = svm.TarifId;
                var societe = svm.SocieteInputSouscriptionViewModel;
                var souscripteur = svm.SouscripteurInputSouscriptionViewModel;
                var administrateur = svm.AdministrationInputSouscriptionViewModel;
                var listeUtilisateurs = svm.UtilisateurInputSouscriptionViewModel;

                var clientAdresse = new Adresse();
                var client = new Client();

                string expression = @"^([\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+\.)*[\w\!\#$\%\&\'\*\+\-\/\=\?\^\`{\|\}\~]+@((((([a-zA-Z0-9]{1}[a-zA-Z0-9\-]{0,62}[a-zA-Z0-9]{1})|[a-zA-Z])\.)+[a-zA-Z]{2,6})|(\d{1,3}\.){3}\d{1,3}(\:\d{1,5})?)$"; 
                var utilisateurs = svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs;
                for (int i = 0; i < utilisateurs.Count; i++)
                {
                    if ((utilisateurs[i].Nom == null || utilisateurs[i].Nom.Equals("")) &&
                        (utilisateurs[i].Prenom == null || utilisateurs[i].Prenom.Equals("")) &&
                        (utilisateurs[i].Email == null || utilisateurs[i].Email.Equals("")))
                    {

                    }
                    else
                    {
                        if (utilisateurs[i].Email != null && !utilisateurs[i].Email.Equals(""))
                        {
                            Match match = Regex.Match(utilisateurs[i].Email, expression, RegexOptions.IgnoreCase);
                            if (!match.Success)
                                ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorMail");
                        }
                        if (utilisateurs[i].Nom == null || utilisateurs[i].Nom.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Nom", "ErrorRequired");
                        }
                        if (utilisateurs[i].Prenom == null || utilisateurs[i].Prenom.Equals(""))
                        {
                            ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Prenom", "ErrorRequired");
                        }
                        if (utilisateurs[i].Email == null || utilisateurs[i].Email.Equals(""))
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

                var listeEmailsExistants = _serviceApi.GetListeEmails();

                if (listeEmailsSaisies != null && listeEmailsSaisies.Count > 0)
                {
                    //verifier si mail souscripteur est valide
                    var listeMail1 = listeEmailsSaisies.FindAll(o => o.Equals(svm.SouscripteurInputSouscriptionViewModel.Email));
                    var listeMail2 = listeEmailsExistants.FindAll(o => o.Equals(svm.SouscripteurInputSouscriptionViewModel.Email));
                    
                    //if (listeMail1.Count() > 1 || listeMail2.Count > 0)
                    //{
                    //    ModelState.AddModelError("SouscripteurInputSouscriptionViewModel.Email", "ErrorMail");
                    //    Logger.Error("email souscripteur existant ou en doublons: " + svm.SouscripteurInputSouscriptionViewModel.Email);
                    //}

                    if (listeMail1.Count() > 1)
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

                    //if (listeMail3.Count() > 1 || listeMail4.Count > 0)
                    //{
                    //    ModelState.AddModelError("AdministrationInputSouscriptionViewModel.Email", "ErrorMail");
                    //    Logger.Error("email adminstrateur existant ou en doublons: " + svm.AdministrationInputSouscriptionViewModel.Email);
                    //}

                    if (listeMail3.Count() > 1)
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
                    for (int i = 0; i < utilisateurs.Count(); i++)
                    {
                        var listeMail5 = listeEmailsSaisies.FindAll(o => o.Equals(svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email));
                        var listeMail6 = listeEmailsExistants.FindAll(o => o.Equals(svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email));
                        
                        //if (listeMail5.Count() > 1 || listeMail6.Count > 0)
                        //{
                        //    ModelState.AddModelError("UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[" + i + "].Email", "ErrorMail");
                        //    Logger.Error("email utilisateur existant ou en doublons: " + svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs[i].Email);
                        //}

                        if (listeMail5.Count() > 1)
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

                if (ModelState.IsValid)
                {
                    // Sauvegarder Adresse
                    try
                    {
                        var adresse = new Adresse()
                            {
                                Adresse1 = societe.Addresse,
                                CodePostal = societe.CodePostal,
                                Ville = societe.Ville,
                                PaysId = (int) societe.PaysId
                            };
                        clientAdresse = _serviceApi.saveAdress(adresse);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    //Sauvegarder tarif & chiffres affaires (details client)
                    try
                    {
                        var detailsClient = new DetailsClient()
                            {
                                ChiffreAffaire = societe.ChiffresAffaires,
                                TarifId = (int) tarifId,

                            };
                        DetailsClient dc = _serviceApi.saveDetailsClient(detailsClient);
                        Session.Add("detailsClientId", dc.Id);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    //Sauvegarder client
                    int recentAdresseId = clientAdresse.Id;
                    try
                    {
                        // Générer le code interne. On génére un autre code si un code est déjà associé à un client
                        var codeInterne = "";
                        do
                        {
                            codeInterne = Security.HashHelper.GenererCodeInterne();
                        } while (_serviceApi.CodeInterneExiste(codeInterne));

                        var clientz = new Client
                            {
                                AdresseId = recentAdresseId,
                                RaisonSociale = societe.RaisonSociale,
                                IdentificationNational = societe.IdentificationNational,
                                Groupe = societe.Groupe,
                                TVAIntraComm = societe.Tva,
                                ContactEconocom = societe.VotreContacte,
                                ClientEconocom = societe.ClientEconocom,
                                DetailsClientId = (int) Session["detailsClientId"],
                                DateDebut = DateTime.Now,
                                CodeInterneWattsGreen = codeInterne
                            };
                        client = _serviceApi.saveClient(clientz);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    var clientId = client.Id;
                    TypeContact contact_souscripteur = _serviceApi.GetTypeContactByString(TypeContactEnum.Souscripteur.ToString());
                    TypeContact contact_administrateur = _serviceApi.GetTypeContactByString(TypeContactEnum.Administrateur.ToString());
                    TypeContact contact_utilisateur = _serviceApi.GetTypeContactByString(TypeContactEnum.Utilisateur.ToString());

                    //Sauvegarder contact : Souscripteur
                    try
                    {
                        /*
                        var contact = new Contact()
                            {
                                NomContact = souscripteur.Nom,
                                PrenomContact = souscripteur.Prenom,
                                NumeroPhone = souscripteur.Telephone,
                                Email = souscripteur.Email,
                                Fonction = souscripteur.Fonction,
                                ClientId = clientId,
                                TypeContactId = contact_souscripteur.Id,
                                CodeVerification = Guid.NewGuid().ToString(),
                                Authorisations = (int) TypeContactEnum.Souscripteur

                            };

                        //Sauvegarder contact : Administrateur
                        var admin = new Contact()
                            {
                                NomContact = administrateur.Nom,
                                PrenomContact = administrateur.Prenom,
                                NumeroPhone = administrateur.Telephone,
                                Email = administrateur.Email,
                                Fonction = administrateur.Fonction,
                                ClientId = clientId,
                                TypeContactId = contact_administrateur.Id,
                                CodeVerification = Guid.NewGuid().ToString(),
                                Authorisations = (int) TypeContactEnum.Administrateur
                            };
                        _serviceApi.SauvegardeContacte(contact);
                        _serviceApi.SauvegardeContacte(admin);*/

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
                                Authorisations = (int)TypeContactEnum.SouscripteurAdmin,
                                ClientId = client.Id
                            };
                            _serviceApi.SauvegardeContacte(contact);
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
                                Authorisations = (int)TypeContactEnum.Souscripteur,
                                ClientId = client.Id
                            };

                            var admin = new Contact()
                            {
                                NomContact = administrateur.Nom,
                                PrenomContact = administrateur.Prenom,
                                NumeroPhone = administrateur.Telephone,
                                Email = administrateur.Email,
                                Fonction = administrateur.Fonction,
                                TypeContactId = contact_administrateur.Id,
                                CodeVerification = Guid.NewGuid().ToString(),
                                Authorisations = (int)TypeContactEnum.Administrateur,
                                ClientId = client.Id
                            };

                            _serviceApi.SauvegardeContacte(contact);
                            _serviceApi.SauvegardeContacte(admin);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    //Sauvegarder utilisateurs
                    try
                    {
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
                                        ClientId = clientId,
                                        CodeVerification = Guid.NewGuid().ToString(),
                                        Authorisations = (int) TypeContactEnum.Utilisateur
                                    };
                                _serviceApi.SauvegardeContacte(utilisateur);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    // Envoyer Email
                    EnvoyerEmailEnregistrement(svm);

                    return RedirectToAction("Index");
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

                    svm.SocieteInputSouscriptionViewModel.ListPays = _serviceApi.GetPays();
                    svm.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs = utilisateurs;

                    var listeTarif = _serviceApi.GetListeTarif();

                    foreach (var tarif in listeTarif)
                    {
                        tarif.LibelleTarif = tarif.LibelleTarif + ": de " + tarif.NbreMinDevice + " à " + tarif.NbreMaxDevice +
                                             " devices ( " + tarif.TarifAnnuel + "€ )";
                    }
                    svm.ListTarif = listeTarif;

                    return View("Souscription", svm);
                }
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

                // Ajouter l'adresse mail de l'administrateur
                if (!emailList.Contains(souscriptionViewModel.AdministrationInputSouscriptionViewModel.Email))
                    emailList.Add(souscriptionViewModel.AdministrationInputSouscriptionViewModel.Email);

                // Ajouter les adresses mail des utilisateurs
                foreach (var utilisateur in souscriptionViewModel.UtilisateurInputSouscriptionViewModel.ListeUtilisateurs)
                {
                    if (utilisateur.Email != null)
                    {
                        if (!emailList.Contains(utilisateur.Email))
                        {
                            emailList.Add(utilisateur.Email);
                        }
                    }
                }

                var LangueChoisi = (Langue)Session["LangueChoisi"];  //?? get langue from pays
                if (LangueChoisi == null)
                {
                    LangueChoisi = _serviceApi.GetLangueByCode("fr");
                }

                // Recuperer le sujet et le contenu du mail
                var typeMail = _serviceApi.GetTypeMail("Demande de souscription");
                var contenuMail = _serviceApi.GetContenuMail(typeMail.Id, LangueChoisi.Id);

                // Pays
                var pays = new Pays();
                if (souscriptionViewModel.SocieteInputSouscriptionViewModel.PaysId != null)
                {
                    pays = _serviceApi.GetPaysById(souscriptionViewModel.SocieteInputSouscriptionViewModel.PaysId ?? 0);
                    souscriptionViewModel.SocieteInputSouscriptionViewModel.Pays = pays;
                }

                // Tarif
                var tarif = _serviceApi.GetTarifById(souscriptionViewModel.TarifId.Value);

                // Envoyer Email
                return EmailHelper.SendRegistrationMailToContacts(emailList, contenuMail, souscriptionViewModel, tarif);
            }
            catch (Exception e)
            {
                throw e;
            }
            return false;
        }

        public ActionResult Import()
        {
            try
            {
                return View("Import");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            var uploadApi = new UploadController();
            var parc = ConfigurationManager.AppSettings["Parc"];
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    
                    try
                    {                        
                        if (!FileHelper.VerifierNomFichierParc(fileNameWithoutExtension))
                        {
                            ViewBag.Error = "Nom de fichier incorrect";
                            return View();
                        }

                        var clientId = FileHelper.GetClientId(fileNameWithoutExtension);
                        var fileNameToSave = String.Format("{0}_{1}{2}", fileNameWithoutExtension,
                                                           DateTime.Now.ToString("yyyyMMddHHmmss"),
                                                           Path.GetExtension(fileName));

                        var fileLen = file.ContentLength;
                        var input = new byte[fileLen];

                        // Initialize the stream.
                        var inputStream = file.InputStream;

                        // Read the file into the byte array.
                        inputStream.Read(input, 0, fileLen);
                        var document = new DocumentUpload {Data = input, DocumentName = fileName};

                        var ok = uploadApi.UploadDoc(document, "WattsGreen");

                        try
                        {
                            var filePath = "";
                            if (ok)
                            {
                                filePath = FileHelper.GetParcSuccessPath(parc, clientId);
                                ViewBag.Success = Resource.Traduction.ImportParcOK;
                            }
                            else
                            {
                                filePath = FileHelper.GetParcFailPath(parc, clientId);
                                ViewBag.Error = Resource.Traduction.ImportParcKO;
                            }
                            Directory.CreateDirectory(Server.MapPath(Url.Content(filePath)));
                            file.SaveAs(Server.MapPath(Url.Content(Path.Combine(filePath, fileNameToSave))));
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = Resource.Traduction.ImportParcKO;
                            Logger.Error(e);
                        }

                    }
                    catch (Exception e)
                    {
                        try
                        {
                            var filePath = FileHelper.GetParcFailPathInvalid(parc);
                            System.IO.Directory.CreateDirectory(Server.MapPath(Url.Content(filePath)));
                            var fileNameToSave = String.Format("{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                            file.SaveAs(Server.MapPath(Url.Content(Path.Combine(filePath, fileNameToSave))));                            
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }

                        ViewBag.Error = Resource.Traduction.ImportParcKO;
                        Logger.Error("Message: " + e.Message + ", " + e.StackTrace);
                    }
                }

            }
            else
            {
                ViewBag.Error = Resource.Traduction.Selection_Fichier;
            }

            return View();
        }
    }
}
