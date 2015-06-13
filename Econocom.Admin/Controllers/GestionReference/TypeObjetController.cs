using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using CsvHelper;
using Econocom.Admin.Controllers.ServiceApi;
using Econocom.Model;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using Econocom.Model.ViewModel.CsvMap;
using Econocom.Model.ViewModel.Report;
using Infrastructure.Builder;
using NLog;
using Omu.ValueInjecter;
using WebGrease.Css.Extensions;
using Econocom.Model.Models.Traduction;

namespace Econocom.Admin.Controllers.GestionReference
{
    [Authorize]
    public class TypeObjetController : BaseController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceApiController _service;
        private static int pageDimension = 10;

        public TypeObjetController()
        {
            _service = new ServiceApiController();

            int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageDimension);            
        }

        [HttpPost]
        public ActionResult Index(ReferenceViewModel rm)
        {
            foreach (var entite in rm.Entitees)
            {
                int? e = entite.Id;
            }
            return View(rm);
        }

        //
        // GET: /Admin/TypeObjet/

        public ActionResult Index()
        {
            try
            {
                int totalPages = 1;
                var rm = GetReferenceViewModel(1, 1, pageDimension, out totalPages);
                rm.CurrentPage = 1;
                rm.TotalPages = totalPages;
                rm.PreviousPage = 1;
                return View(rm);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.ErreurPartielle();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sauvegarde(ReferenceViewModel rm)
        {
            var referenceMAJ = new ReferenceViewModel();
            try
            {
                var message = Resource.Traduction.Operation_Succes;

                //var c = System.Threading.Thread.CurrentThread.CurrentCulture;
                var listeSupprimer = new List<object>();
                var listeAjouter = new List<object>();

                listeSupprimer = rm.Entitees.Where(o => o.Supprimer).AsEnumerable().ToList<object>();
                listeAjouter = rm.Entitees.Where(o => !o.Supprimer).AsEnumerable().ToList<object>();
                ViewBag.MessageUnitaire = Resource.Traduction.Operation_Succes;

                var liste = GetObjet(rm);
                var typeListe = liste.ListType;

                var listeEnErreur = new List<dynamic>();

                if (ModelState.IsValid)
                {
                    if (liste.EntiteeCreer.Any())
                    {
                        listeEnErreur = _service.SauvegardeListe(liste.ListType, liste.EntiteeCreer);
                    }
                    if (liste.EntiteeSupprimer.Any())
                    {
                        listeEnErreur = _service.SupprimerListe(liste.ListType, liste.EntiteeSupprimer);
                    }

                    if (listeEnErreur.Any())
                    {
                        message = String.Format("{0}: {1} {2}", Resource.Traduction.Operation_Erreur,
                                                Resource.Traduction.Erreur_Doublons,
                                                Resource.Traduction.Erreur_VerifierLog);
                        ViewBag.Erreur = String.Format(Resource.Traduction.NombreObjetErreur, listeEnErreur.Count);
                    }
                }
                else
                {
                    ViewBag.MessageUnitaire = message;
                }

                var typeObjet = _service.GetTypeObjet(typeListe.ToString());

                int currentPage = 1;
                if (Session["CurrentPageIndex"] != null)
                    currentPage = (int) Session["CurrentPageIndex"];

                int totalPages = 1;
                referenceMAJ = GetReferenceViewModel(typeObjet.Id, currentPage, pageDimension, out totalPages);
                referenceMAJ.CurrentPage = currentPage;
                referenceMAJ.TotalPages = totalPages;
                referenceMAJ.PageDimension = pageDimension;
                if (currentPage > 1)
                    referenceMAJ.PreviousPage = currentPage - 1;
            }
            catch (Exception e)
            {
                var exception = HandleException(e);
                var message = String.Format("{0}: {1}", exception, Resource.Traduction.Erreur_VerifierLog);

                Type type = rm.Entitees.GetType().GetGenericArguments()[0];
                referenceMAJ = rm;
                referenceMAJ.ListType = type;
                Logger.Error(e.StackTrace);
                ViewBag.MessageUnitaire = Resource.Traduction.Operation_Erreur;
                ViewBag.Erreur = message;
            }

            return View("Detailles", referenceMAJ);

        }

        
        [ValidateAntiForgeryToken]
        public ActionResult AfficherPage(int id, int page)
        {
            try
            {
                
                int totalPages = 1;
                var rm = GetReferenceViewModel(id, page, pageDimension, out totalPages);
                rm.CurrentPage = page;
                rm.TotalPages = totalPages;
                rm.PageDimension = pageDimension;
                if (page > 1)
                    rm.PreviousPage = page - 1;

                Session.Add("CurrentPageIndex", page);
                return View("Detailles", rm);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Afficher(ReferenceViewModel rm)
        {
            try
            {
                int currentPage = rm.CurrentPage;
                int previousPage = rm.CurrentPage;
                int totalPages = 1;
                rm = GetReferenceViewModel(rm.Id, rm.CurrentPage, pageDimension, out totalPages);
                rm.CurrentPage = currentPage;
                rm.TotalPages = totalPages;
                rm.PageDimension = pageDimension;
                if (currentPage > 1)
                    rm.PreviousPage = currentPage - 1;

                Session.Add("CurrentPageIndex", currentPage);
                return View("Detailles", rm);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }

        }

        //
        // GET:  /Admin/TypeObjet/AfficherObjet
        [ValidateInput(false)]
        public PartialViewResult AfficherObjet(string nomObjet)
        {
            try
            {
                var liste = _service.GetListeTypeObjet();
                var rm = new ReferenceViewModel();
                List<EntityViewModel> collection = null;
                object objList = null;
                object newItem = null;
                foreach (var x in liste)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    var typeX = x.GetType();

                    rm.ListType = typeX;
                    if (objList == null)
                    {
                        objList = CreateGenericList(typeof(EntityViewModel));

                    }
                    if (typeX == typeof(TypeObjet))
                    {
                        newItem = Activator.CreateInstance(typeof(TypeObjetViewModel));
                    }
                    var mListAdd = objList.GetType().GetMethod("Add");
                    newItem.InjectFrom(x);
                    mListAdd.Invoke(objList, new object[] { newItem });
                }

                collection = (List<EntityViewModel>)objList;
                rm.Entitees = collection;
                return PartialView("TypeObjetPartial", rm);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.ErreurPartielle();
            }
        }

        public ActionResult Filter(string column, string order)
        {
            var columnName = column;
            var orderBy = order;
            return RedirectToAction("Index");
        }

        private Object CreateGenericList(Type typeX)
        {
            var listType = typeof(List<>);
            Type[] typeArgs = { typeX };
            var genericType = listType.MakeGenericType(typeArgs);
            var o = Activator.CreateInstance(genericType);
            return o;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detailles(int Id)
        {
            try
            {
                int totalPages = 1;
                var rm = GetReferenceViewModel(Id, 1, pageDimension, out totalPages);
                rm.CurrentPage = 1;
                rm.TotalPages = totalPages;
                rm.PageDimension = pageDimension;
                if (rm.CurrentPage > 1)
                    rm.PreviousPage = rm.CurrentPage - 1;
                Session["CurrentPageIndex"] = 1;
                return View(rm);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return base.Erreur();
            }
        }

        public ReferenceViewModel GetReferenceViewModel(int Id, int currentPage, int pageDimension, out int totalPages)
        {
            try
            {
                ViewBag.IdTable = Id;
                var liste = _service.GetListeObjet(Id, currentPage, pageDimension, out totalPages);
                Type typeObjet = liste.GetType().GetGenericArguments().Single();                
                var rm = new ReferenceViewModel();
                rm.Id = Id;
                List<EntityViewModel> collection = null;
                object objList = null;
                object newItem = null;                

                //set type objet
                rm.ListType = typeObjet;

                if (typeObjet == typeof(AgeDevice))
                {
                    rm.TemplateIndex = 0;
                    rm.DisplayModel = typeof(AgeDeviceViewModel);
                }
                else if (typeObjet == typeof(CategorieDevice))
                {
                    rm.TemplateIndex = 1;
                    rm.DisplayModel = typeof(CategorieDeviceViewModel);
                }
                else if (typeObjet == typeof(ChangementAnneeCalendaire))
                {
                    rm.TemplateIndex = 2;
                    rm.DisplayModel = typeof(ChangementAnneeCalendaireViewModel);
                }
                else if (typeObjet == typeof(ClasseDevice))
                {
                    rm.TemplateIndex = 3;
                    rm.DisplayModel = typeof(ClasseDeviceViewModel);
                }
                else if (typeObjet == typeof(ConsoWattHeur))
                {
                    rm.TemplateIndex = 4;
                    rm.DisplayModel = typeof(ConsoWattHeurViewModel);
                }               
                else if (typeObjet == typeof(Equivalence))
                {
                    rm.TemplateIndex = 5;
                    rm.DisplayModel = typeof(EquivalenceViewModel);
                }                
                else if (typeObjet == typeof(FamilleDevice))
                {
                    rm.TemplateIndex = 6;
                    rm.DisplayModel = typeof(FamilleDeviceViewModel);
                }
                else if (typeObjet == typeof(Langue))
                {
                    rm.TemplateIndex = 7;
                    rm.DisplayModel = typeof(LangueViewModel);
                }
                else if (typeObjet == typeof(Origine))
                {
                    rm.TemplateIndex = 8;
                    rm.DisplayModel = typeof(OrigineViewModel);
                }
                else if (typeObjet == typeof(Pays))
                {
                    rm.TemplateIndex = 9;
                    rm.DisplayModel = typeof(PaysViewModel);
                }
                else if (typeObjet == typeof(Politique))
                {
                    rm.TemplateIndex = 10;
                    rm.DisplayModel = typeof(PolitiqueViewModel);
                }                
                else if (typeObjet == typeof(Question))
                {
                    rm.TemplateIndex = 11;
                    rm.DisplayModel = typeof(QuestionViewModel);
                }
                else if (typeObjet == typeof(Ratio))
                {
                    rm.TemplateIndex = 12;
                    rm.DisplayModel = typeof(RatioViewModel);
                }
                else if (typeObjet == typeof(SecteurActivite))
                {
                    rm.TemplateIndex = 13;
                    rm.DisplayModel = typeof(SecteurActiviteViewModel);
                }
                else if (typeObjet == typeof(Tarif))
                {
                    rm.TemplateIndex = 14;
                    rm.DisplayModel = typeof(TarifViewModel);
                }
                else if (typeObjet == typeof(TypeContact))
                {
                    rm.TemplateIndex = 15;
                    rm.DisplayModel = typeof(TypeContactViewModel);
                }
                else if (typeObjet == typeof(TypeDevice))
                {
                    rm.TemplateIndex = 16;
                    rm.DisplayModel = typeof(TypeDeviceViewModel);
                }
                else if (typeObjet == typeof(TypeMail))
                {
                    rm.TemplateIndex = 17;
                    rm.DisplayModel = typeof(TypeMailViewModel);
                }                
                else if (typeObjet == typeof(TypeRatio))
                {
                    rm.TemplateIndex = 18;
                    rm.DisplayModel = typeof(TypeRatioViewModel);
                }
                else if (typeObjet == typeof(TypeUsage))
                {
                    rm.TemplateIndex = 19;
                    rm.DisplayModel = typeof(TypeUsageViewModel);
                }
                else if (typeObjet == typeof(Usage))
                {
                    rm.TemplateIndex = 20;
                    rm.DisplayModel = typeof(UsageViewModel);
                }
                else if (typeObjet == typeof(UsageDevice))
                {
                    rm.TemplateIndex = 21;
                    rm.DisplayModel = typeof(UsageDeviceViewModel);
                }
                else if (typeObjet == typeof(VentilationClasseAgeDevice))
                {
                    rm.TemplateIndex = 22;
                    rm.DisplayModel = typeof(VentilationClasseAgeDeviceViewModel);
                }
                else if (typeObjet == typeof(VentilationClasseDevice))
                {
                    rm.TemplateIndex = 23;
                    rm.DisplayModel = typeof(VentilationClasseDeviceViewModel);
                }
                else if (typeObjet == typeof(Devise))
                {
                    rm.TemplateIndex = 24;
                    rm.DisplayModel = typeof(DeviseViewModel);
                }
                else if (typeObjet == typeof(CorrespondanceSecteurActivite))
                {
                    rm.TemplateIndex = 25;
                    rm.DisplayModel = typeof(CorrespondanceSecteurActiviteViewModel);
                }
                else if (typeObjet == typeof(CorrespondanceTypeDevice))
                {
                    rm.TemplateIndex = 26;
                    rm.DisplayModel = typeof(CorrespondanceTypeDeviceViewModel);
                }
                else if (typeObjet == typeof(CorrespondanceProcesseur))
                {
                    rm.TemplateIndex = 27;
                    rm.DisplayModel = typeof(CorrespondanceProcesseurViewModel);
                }
                else if (typeObjet == typeof(CorrespondanceVitesse))
                {
                    rm.TemplateIndex = 28;
                    rm.DisplayModel = typeof(CorrespondanceVitesseViewModel);
                }
                else if (typeObjet == typeof(CorrespondanceTaille))
                {
                    rm.TemplateIndex = 29;
                    rm.DisplayModel = typeof(CorrespondanceTailleViewModel);
                }
                else if (typeObjet == typeof(CorrespondancePuissance))
                {
                    rm.TemplateIndex = 30;
                    rm.DisplayModel = typeof(CorrespondancePuissanceViewModel);
                }

                foreach (var objet in liste)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    //typeObjet = objet.GetType();
                    //rm.ListType = typeObjet;

                    if (objList == null)
                    {
                        objList = CreateGenericList(typeof(EntityViewModel));
                    }

                    if (typeObjet == typeof(AgeDevice))
                    {
                        rm.TemplateIndex = 0;
                        newItem = Activator.CreateInstance(typeof(AgeDeviceViewModel));
                        ((AgeDeviceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((AgeDevice)objet);
                    }
                    else if (typeObjet == typeof(CategorieDevice))
                    {
                        rm.TemplateIndex = 1;
                        newItem = Activator.CreateInstance(typeof(CategorieDeviceViewModel));
                        ((CategorieDeviceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((CategorieDevice)objet);
                    }
                    else if (typeObjet == typeof(ChangementAnneeCalendaire))
                    {
                        rm.TemplateIndex = 2;
                        newItem = Activator.CreateInstance(typeof(ChangementAnneeCalendaireViewModel));
                        ((ChangementAnneeCalendaireViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((ChangementAnneeCalendaire)objet);
                    }
                    else if (typeObjet == typeof(ClasseDevice))
                    {
                        rm.TemplateIndex = 3;
                        newItem = Activator.CreateInstance(typeof(ClasseDeviceViewModel));
                        ((ClasseDeviceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((ClasseDevice)objet);
                    }
                    else if (typeObjet == typeof(ConsoWattHeur))
                    {
                        rm.TemplateIndex = 4;
                        newItem = Activator.CreateInstance(typeof(ConsoWattHeurViewModel));                        
                        //((ConsoWattHeurViewModel) newItem).Id = objet.Id;
                        newItem.InjectFrom((ConsoWattHeur)objet);
                        newItem.InjectFrom<NullDateInjection>((ConsoWattHeur)objet);
                    }                   
                    else if (typeObjet == typeof(Equivalence))
                    {
                        rm.TemplateIndex = 5;
                        newItem = Activator.CreateInstance(typeof(EquivalenceViewModel));
                        ((EquivalenceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Equivalence)objet);
                    }                    
                    else if (typeObjet == typeof(FamilleDevice))
                    {
                        rm.TemplateIndex = 6;
                        newItem = Activator.CreateInstance(typeof(FamilleDeviceViewModel));
                        ((FamilleDeviceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((FamilleDevice)objet);
                    }
                    else if (typeObjet == typeof(Langue))
                    {
                        rm.TemplateIndex = 7;
                        newItem = Activator.CreateInstance(typeof(LangueViewModel));
                        ((LangueViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Langue)objet);
                    }
                    else if (typeObjet == typeof(Origine))
                    {
                        rm.TemplateIndex = 8;
                        newItem = Activator.CreateInstance(typeof(OrigineViewModel));
                        ((OrigineViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Origine)objet);
                    }
                    else if (typeObjet == typeof(Pays))
                    {
                        rm.TemplateIndex = 9;
                        newItem = Activator.CreateInstance(typeof(PaysViewModel));
                        ((PaysViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Pays)objet);
                    }
                    else if (typeObjet == typeof(Politique))
                    {
                        rm.TemplateIndex = 10;
                        newItem = Activator.CreateInstance(typeof(PolitiqueViewModel));
                        ((PolitiqueViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Politique)objet);
                    }                    
                    else if (typeObjet == typeof(Question))
                    {
                        rm.TemplateIndex = 11;
                        newItem = Activator.CreateInstance(typeof(QuestionViewModel));
                        ((QuestionViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Question)objet);
                    }
                    else if (typeObjet == typeof(Ratio))
                    {
                        rm.TemplateIndex = 12;
                        newItem = Activator.CreateInstance(typeof(RatioViewModel));
                        //((RatioViewModel) newItem).Id = objet.Id;
                        newItem.InjectFrom((Ratio)objet);
                    }
                    else if (typeObjet == typeof(SecteurActivite))
                    {
                        rm.TemplateIndex = 13;
                        newItem = Activator.CreateInstance(typeof(SecteurActiviteViewModel));
                        ((SecteurActiviteViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((SecteurActivite)objet);
                    }
                    else if (typeObjet == typeof(Tarif))
                    {
                        rm.TemplateIndex = 14;
                        newItem = Activator.CreateInstance(typeof(TarifViewModel));
                        ((TarifViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Tarif)objet);
                    }
                    else if (typeObjet == typeof(TypeContact))
                    {
                        rm.TemplateIndex = 15;
                        newItem = Activator.CreateInstance(typeof(TypeContactViewModel));
                        ((TypeContactViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((TypeContact)objet);
                    }
                    else if (typeObjet == typeof(TypeDevice))
                    {
                        rm.TemplateIndex = 16;
                        newItem = Activator.CreateInstance(typeof(TypeDeviceViewModel));
                        ((TypeDeviceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((TypeDevice)objet);
                    }
                    else if (typeObjet == typeof(TypeMail))
                    {
                        rm.TemplateIndex = 17;
                        newItem = Activator.CreateInstance(typeof(TypeMailViewModel));
                        ((TypeMailViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((TypeMail)objet);
                    }                    
                    else if (typeObjet == typeof(TypeRatio))
                    {
                        rm.TemplateIndex = 18;
                        newItem = Activator.CreateInstance(typeof(TypeRatioViewModel));
                        ((TypeRatioViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((TypeRatio)objet);
                    }
                    else if (typeObjet == typeof(TypeUsage))
                    {
                        rm.TemplateIndex = 19;
                        newItem = Activator.CreateInstance(typeof(TypeUsageViewModel));
                        ((TypeUsageViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((TypeUsage)objet);
                    }
                    else if (typeObjet == typeof(Usage))
                    {
                        rm.TemplateIndex = 20;
                        newItem = Activator.CreateInstance(typeof(UsageViewModel));
                        ((UsageViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Usage)objet);
                    }
                    else if (typeObjet == typeof(UsageDevice))
                    {
                        rm.TemplateIndex = 21;
                        newItem = Activator.CreateInstance(typeof(UsageDeviceViewModel));
                        ((UsageDeviceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((UsageDevice)objet);
                    }
                    else if (typeObjet == typeof(VentilationClasseAgeDevice))
                    {
                        rm.TemplateIndex = 22;
                        newItem = Activator.CreateInstance(typeof(VentilationClasseAgeDeviceViewModel));
                        // ((VentilationClasseAgeDeviceViewModel) newItem).Id = objet.Id;
                        newItem.InjectFrom((VentilationClasseAgeDevice)objet);
                    }
                    else if (typeObjet == typeof(VentilationClasseDevice))
                    {
                        rm.TemplateIndex = 23;
                        newItem = Activator.CreateInstance(typeof(VentilationClasseDeviceViewModel));
                        //((VentilationClasseDeviceViewModel) newItem).Id = objet.Id;
                        newItem.InjectFrom((VentilationClasseDevice)objet);
                    }
                    else if (typeObjet == typeof(Devise))
                    {
                        rm.TemplateIndex = 24;
                        newItem = Activator.CreateInstance(typeof(DeviseViewModel));
                        ((DeviseViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((Devise)objet);
                    }
                    else if (typeObjet == typeof(CorrespondanceSecteurActivite))
                    {
                        rm.TemplateIndex = 25;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceSecteurActiviteViewModel));
                        ((CorrespondanceSecteurActiviteViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((CorrespondanceSecteurActivite)objet);
                    }
                    else if (typeObjet == typeof(CorrespondanceTypeDevice))
                    {
                        rm.TemplateIndex = 26;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceTypeDeviceViewModel));
                        ((CorrespondanceTypeDeviceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((CorrespondanceTypeDevice)objet);
                    }                   
                    else if (typeObjet == typeof(CorrespondanceProcesseur))
                    {
                        rm.TemplateIndex = 27;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceProcesseurViewModel));
                        ((CorrespondanceProcesseurViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((CorrespondanceProcesseur)objet);
                    }
                    else if (typeObjet == typeof(CorrespondanceVitesse))
                    {
                        rm.TemplateIndex = 28;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceVitesseViewModel));
                        ((CorrespondanceVitesseViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((CorrespondanceVitesse)objet);
                    }
                    else if (typeObjet == typeof(CorrespondanceTaille))
                    {
                        rm.TemplateIndex = 29;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceTailleViewModel));
                        ((CorrespondanceTailleViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((CorrespondanceTaille)objet);
                    }
                    else if (typeObjet == typeof(CorrespondancePuissance))
                    {
                        rm.TemplateIndex = 30;
                        newItem = Activator.CreateInstance(typeof(CorrespondancePuissanceViewModel));
                        ((CorrespondancePuissanceViewModel)newItem).Id = objet.Id;
                        newItem.InjectFrom((CorrespondancePuissance)objet);
                    }
                    MethodInfo mListAdd = objList.GetType().GetMethod("Add");
                    //newItem.InjectFrom(x);

                    mListAdd.Invoke(objList, new object[] { newItem });
                }

                collection = (List<EntityViewModel>)objList;
                rm.Entitees = collection;
                return rm;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Reference GetObjet(ReferenceViewModel rm)
        {
            try
            {
                var reference = new Reference();
                List<BaseEntity> collection = null;
                object objList = null;

                object objListAjouter = null;
                object objListSupprimer = null;

                object newItem = null;
                var entities = new List<object>();
                bool supprimer = false;

                foreach (var entite in rm.Entitees)
                {
                    IValueInjecter injecter = new ValueInjecter();
                    Type typeX = entite.GetType();
                    supprimer = entite.Supprimer;

                    if (objList == null)
                    {
                        objList = CreateGenericList(typeof(BaseEntity));
                        objListAjouter = CreateGenericList(typeof(BaseEntity));
                        objListSupprimer = CreateGenericList(typeof(BaseEntity));
                    }

                    if (typeX == typeof(AgeDeviceViewModel))
                    {
                        reference.ListType = typeof(AgeDevice);
                        rm.TemplateIndex = 0;
                        newItem = Activator.CreateInstance(typeof(AgeDevice));
                        if (entite.Id.HasValue)
                            ((AgeDevice)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((AgeDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(CategorieDeviceViewModel))
                    {
                        reference.ListType = typeof(CategorieDevice);
                        rm.TemplateIndex = 1;
                        newItem = Activator.CreateInstance(typeof(CategorieDevice));
                        if (entite.Id.HasValue)
                            ((CategorieDevice)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((CategorieDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(ChangementAnneeCalendaireViewModel))
                    {
                        reference.ListType = typeof(ChangementAnneeCalendaire);
                        rm.TemplateIndex = 2;
                        newItem = Activator.CreateInstance(typeof(ChangementAnneeCalendaire));
                        if (entite.Id.HasValue)
                            ((ChangementAnneeCalendaire)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((ChangementAnneeCalendaireViewModel)entite);
                    }
                    else if (typeX == typeof(ClasseDeviceViewModel))
                    {
                        reference.ListType = typeof(ClasseDevice);
                        rm.TemplateIndex = 3;
                        newItem = Activator.CreateInstance(typeof(ClasseDevice));
                        if (entite.Id.HasValue)
                            ((ClasseDevice)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((ClasseDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(ConsoWattHeurViewModel))
                    {
                        reference.ListType = typeof(ConsoWattHeur);
                        rm.TemplateIndex = 4;
                        newItem = Activator.CreateInstance(typeof(ConsoWattHeur));
                        //((ConsoWattHeur)newItem).Id = x.Id;
                        newItem.InjectFrom((ConsoWattHeurViewModel)entite);                        
                    }                   
                    else if (typeX == typeof(EquivalenceViewModel))
                    {
                        reference.ListType = typeof(Equivalence);
                        rm.TemplateIndex = 5;
                        newItem = Activator.CreateInstance(typeof(Equivalence));
                        if (entite.Id.HasValue)
                            ((Equivalence)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((EquivalenceViewModel)entite);
                    }                   
                    else if (typeX == typeof(FamilleDeviceViewModel))
                    {
                        reference.ListType = typeof(FamilleDevice);
                        rm.TemplateIndex = 6;
                        newItem = Activator.CreateInstance(typeof(FamilleDevice));
                        if (entite.Id.HasValue)
                            ((FamilleDevice)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((FamilleDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(LangueViewModel))
                    {
                        reference.ListType = typeof(Langue);
                        rm.TemplateIndex = 7;
                        newItem = Activator.CreateInstance(typeof(Langue));
                        if (entite.Id.HasValue)
                            ((Langue)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((LangueViewModel)entite);
                    }   
                    else if (typeX == typeof(OrigineViewModel))
                    {
                        reference.ListType = typeof(Origine);
                        rm.TemplateIndex = 8;
                        newItem = Activator.CreateInstance(typeof(Origine));
                        if (entite.Id.HasValue)
                            ((Origine)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((OrigineViewModel)entite);
                    }
                    else if (typeX == typeof(PaysViewModel))
                    {
                        reference.ListType = typeof(Pays);
                        rm.TemplateIndex = 9;
                        newItem = Activator.CreateInstance(typeof(Pays));
                        if (entite.Id.HasValue)
                            ((Pays)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((PaysViewModel)entite);
                    }
                    else if (typeX == typeof(PolitiqueViewModel))
                    {
                        reference.ListType = typeof(Politique);
                        rm.TemplateIndex = 10;
                        newItem = Activator.CreateInstance(typeof(Politique));
                        if (entite.Id.HasValue)
                            ((Politique)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((PolitiqueViewModel)entite);
                    }                   
                    else if (typeX == typeof(QuestionViewModel))
                    {
                        reference.ListType = typeof(Question);
                        rm.TemplateIndex = 11;
                        newItem = Activator.CreateInstance(typeof(Question));
                        if (entite.Id.HasValue)
                            ((Question)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((QuestionViewModel)entite);
                    }
                    else if (typeX == typeof(RatioViewModel))
                    {
                        reference.ListType = typeof(Ratio);
                        rm.TemplateIndex = 12;
                        newItem = Activator.CreateInstance(typeof(Ratio));
                        //((Ratio)newItem).Id = x.Id;
                        newItem.InjectFrom((RatioViewModel)entite);
                    }
                    else if (typeX == typeof(SecteurActiviteViewModel))
                    {
                        reference.ListType = typeof(SecteurActivite);
                        rm.TemplateIndex = 13;
                        newItem = Activator.CreateInstance(typeof(SecteurActivite));
                        if (entite.Id.HasValue)
                            ((SecteurActivite)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((SecteurActiviteViewModel)entite);
                    }
                    else if (typeX == typeof(TarifViewModel))
                    {
                        reference.ListType = typeof(Tarif);
                        rm.TemplateIndex = 14;
                        newItem = Activator.CreateInstance(typeof(Tarif));
                        if (entite.Id.HasValue)
                            ((Tarif)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((TarifViewModel)entite);
                    }
                    else if (typeX == typeof(TypeContactViewModel))
                    {
                        reference.ListType = typeof(TypeContact);
                        rm.TemplateIndex = 15;
                        newItem = Activator.CreateInstance(typeof(TypeContact));
                        if (entite.Id.HasValue)
                            ((TypeContact)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((TypeContactViewModel)entite);
                    }
                    else if (typeX == typeof(TypeDeviceViewModel))
                    {
                        reference.ListType = typeof(TypeDevice);
                        rm.TemplateIndex = 16;
                        newItem = Activator.CreateInstance(typeof(TypeDevice));
                        if (entite.Id.HasValue)
                            ((TypeDevice)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((TypeDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(TypeMailViewModel))
                    {
                        reference.ListType = typeof(TypeMail);
                        rm.TemplateIndex = 17;
                        newItem = Activator.CreateInstance(typeof(TypeMail));
                        if (entite.Id.HasValue)
                            ((TypeMail)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((TypeMailViewModel)entite);
                    }                    
                    else if (typeX == typeof(TypeRatioViewModel))
                    {
                        reference.ListType = typeof(TypeRatio);
                        rm.TemplateIndex = 18;
                        newItem = Activator.CreateInstance(typeof(TypeRatio));
                        if (entite.Id.HasValue)
                            ((TypeRatio)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((TypeRatioViewModel)entite);
                    }
                    else if (typeX == typeof(TypeUsageViewModel))
                    {
                        reference.ListType = typeof(TypeUsage);
                        rm.TemplateIndex = 19;
                        newItem = Activator.CreateInstance(typeof(TypeUsage));
                        if (entite.Id.HasValue)
                            ((TypeUsage)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((TypeUsageViewModel)entite);
                    }
                    else if (typeX == typeof(UsageViewModel))
                    {
                        reference.ListType = typeof(Usage);
                        rm.TemplateIndex = 20;
                        newItem = Activator.CreateInstance(typeof(Usage));
                        if (entite.Id.HasValue)
                            ((Usage)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((UsageViewModel)entite);
                    }
                    else if (typeX == typeof(UsageDeviceViewModel))
                    {
                        reference.ListType = typeof(UsageDevice);
                        rm.TemplateIndex = 21;
                        newItem = Activator.CreateInstance(typeof(UsageDevice));
                        if (entite.Id.HasValue)
                            ((UsageDevice)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((UsageDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(VentilationClasseAgeDeviceViewModel))
                    {
                        reference.ListType = typeof(VentilationClasseAgeDevice);
                        rm.TemplateIndex = 22;
                        newItem = Activator.CreateInstance(typeof(VentilationClasseAgeDevice));
                        //((VentilationClasseAgeDevice)newItem).Id = x.Id;
                        newItem.InjectFrom((VentilationClasseAgeDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(VentilationClasseDeviceViewModel))
                    {
                        reference.ListType = typeof(VentilationClasseDevice);
                        rm.TemplateIndex = 23;
                        newItem = Activator.CreateInstance(typeof(VentilationClasseDevice));
                        //((VentilationClasseDevice)newItem).Id = x.Id;
                        newItem.InjectFrom((VentilationClasseDeviceViewModel)entite);
                    }                                                                            
                    else if (typeX == typeof(DeviseViewModel))
                    {
                        reference.ListType = typeof(Devise);
                        rm.TemplateIndex = 24;
                        newItem = Activator.CreateInstance(typeof(Devise));
                        if (entite.Id.HasValue)
                            ((Devise)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((DeviseViewModel)entite);
                    }
                    else if (typeX == typeof(CorrespondanceSecteurActiviteViewModel))
                    {
                        reference.ListType = typeof(CorrespondanceSecteurActivite);
                        rm.TemplateIndex = 25;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceSecteurActivite));
                        if (entite.Id.HasValue)
                            ((CorrespondanceSecteurActivite)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((CorrespondanceSecteurActiviteViewModel)entite);
                    }
                    else if (typeX == typeof(CorrespondanceTypeDeviceViewModel))
                    {
                        reference.ListType = typeof(CorrespondanceTypeDevice);
                        rm.TemplateIndex = 26;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceTypeDevice));
                        if (entite.Id.HasValue)
                            ((CorrespondanceTypeDevice)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((CorrespondanceTypeDeviceViewModel)entite);
                    }
                    else if (typeX == typeof(CorrespondanceProcesseurViewModel))
                    {
                        reference.ListType = typeof(CorrespondanceProcesseur);
                        rm.TemplateIndex = 27;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceProcesseur));
                        if (entite.Id.HasValue)
                            ((CorrespondanceProcesseur)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((CorrespondanceProcesseurViewModel)entite);
                    }
                    else if (typeX == typeof(CorrespondanceVitesseViewModel))
                    {
                        reference.ListType = typeof(CorrespondanceVitesse);
                        rm.TemplateIndex = 28;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceVitesse));
                        if (entite.Id.HasValue)
                            ((CorrespondanceVitesse)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((CorrespondanceVitesseViewModel)entite);
                    }
                    else if (typeX == typeof(CorrespondanceTailleViewModel))
                    {
                        reference.ListType = typeof(CorrespondanceTaille);
                        rm.TemplateIndex = 29;
                        newItem = Activator.CreateInstance(typeof(CorrespondanceTaille));
                        if (entite.Id.HasValue)
                            ((CorrespondanceTaille)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((CorrespondanceTailleViewModel)entite);
                    }
                    else if (typeX == typeof(CorrespondancePuissanceViewModel))
                    {
                        reference.ListType = typeof(CorrespondancePuissance);
                        rm.TemplateIndex = 30;
                        newItem = Activator.CreateInstance(typeof(CorrespondancePuissance));
                        if (entite.Id.HasValue)
                            ((CorrespondancePuissance)newItem).Id = entite.Id.Value;
                        newItem.InjectFrom((CorrespondancePuissanceViewModel)entite);
                    }
                    if (supprimer)
                    {
                        MethodInfo mListAdd = objList.GetType().GetMethod("Add");
                        mListAdd.Invoke(objListSupprimer, new object[] { newItem });
                    }
                    else
                    {
                        MethodInfo mListAdd = objList.GetType().GetMethod("Add");
                        mListAdd.Invoke(objListAjouter, new object[] { newItem });
                    }
                }

                collection = (List<BaseEntity>)objList;


                IEnumerable e = objList as IList;
                IEnumerable<object> c = e.OfType<object>();
                reference.Entitees = c.ToList();

                IEnumerable eAjouter = objListAjouter as IList;
                IEnumerable<object> oAjouter = eAjouter.OfType<object>();
                reference.EntiteeCreer = oAjouter.ToList();

                IEnumerable eSupprimer = objListSupprimer as IList;
                IEnumerable<object> oSupprimer = eSupprimer.OfType<object>();
                reference.EntiteeSupprimer = oSupprimer.ToList();

                return reference;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public FileResult GetFile(int Id)
        {
            try
            {
                ViewBag.IdTable = Id;
                int totalPages = -1;
                var referenceModel = GetReferenceViewModel(Id, 0, 0, out totalPages);
                var liste = referenceModel.Entitees;

                if (liste != null && liste.Any())
                {
                    var objet = liste.First();
                    var type = objet.GetType();

                    using (var stream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                        {
                            CsvWriter csvWriter1 = new CsvWriter(streamWriter);
                            csvWriter1.Configuration.Delimiter = ";";
                            var nomFichier = "Export_{0}_{1}.csv";
                            string typeObjet = type.ToString();
                            string nomObjet = typeObjet.Split('.').Last().Replace("ViewModel", "");
                            string date = DateTime.Now.ToString("yyyyMMddHHmmss");
                            var nomFichierFinale = String.Format(nomFichier, nomObjet, date);
                            if (type == typeof(AgeDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<AgeDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(AgeDeviceViewModel));
                                liste.Cast<AgeDeviceViewModel>().ForEach(csvWriter1.WriteRecord);                                                             
                            }
                            else if (type == typeof(CategorieDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<CategorieDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(CategorieDeviceViewModel));
                                liste.Cast<CategorieDeviceViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(ChangementAnneeCalendaireViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<ChangementAnneeCalendaireViewModelMap>();
                                csvWriter1.WriteHeader(typeof(ChangementAnneeCalendaireViewModel));
                                liste.Cast<ChangementAnneeCalendaireViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(ClasseDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<ClasseDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(ClasseDeviceViewModel));
                                liste.Cast<ClasseDeviceViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(ConsoWattHeurViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<ConsoWattHeurViewModelMap>();
                                csvWriter1.WriteHeader(typeof(ConsoWattHeurViewModel));
                                liste.Cast<ConsoWattHeurViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(ContenuMailViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<ContenuMailViewModelMap>();
                                csvWriter1.WriteHeader(typeof(ContenuMailViewModel));
                                liste.Cast<ContenuMailViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(EquivalenceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<EquivalenceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(EquivalenceViewModel));
                                liste.Cast<EquivalenceViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(EquivalenceTraductionViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<EquivalenceTraductionViewModelMap>();
                                csvWriter1.WriteHeader(typeof(EquivalenceTraductionViewModel));
                                liste.Cast<EquivalenceTraductionViewModel>().ForEach(csvWriter1.WriteRecord);                              
                            }
                            else if (type == typeof(FamilleDeviceViewModel))
                            {                                
                                csvWriter1.Configuration.RegisterClassMap<FamilleDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(FamilleDeviceViewModel));
                                liste.Cast<FamilleDeviceViewModel>().ForEach(csvWriter1.WriteRecord);                              
                            }
                            else if (type == typeof(OrigineViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<OrigineViewModelMap>();
                                csvWriter1.WriteHeader(typeof(OrigineViewModel));
                                liste.Cast<OrigineViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(PaysViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<PaysViewModelMap>();
                                csvWriter1.WriteHeader(typeof(PaysViewModel));
                                liste.Cast<PaysViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(PolitiqueViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<PolitiqueViewModelMap>();
                                csvWriter1.WriteHeader(typeof(PolitiqueViewModel));
                                liste.Cast<PolitiqueViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(PolitiqueTraductionViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<PolitiqueTraductionViewModelMap>();
                                csvWriter1.WriteHeader(typeof(PolitiqueTraductionViewModel));
                                liste.Cast<PolitiqueTraductionViewModel>().ForEach(csvWriter1.WriteRecord);                             
                            }
                            else if (type == typeof(QuestionViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<QuestionViewModelMap>();
                                csvWriter1.WriteHeader(typeof(QuestionViewModel));
                                liste.Cast<QuestionViewModel>().ForEach(csvWriter1.WriteRecord);                              
                            }
                            else if (type == typeof(RatioViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<RatioViewModelMap>();
                                csvWriter1.WriteHeader(typeof(RatioViewModel));
                                liste.Cast<RatioViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(SecteurActiviteViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<SecteurActiviteViewModelMap>();
                                csvWriter1.WriteHeader(typeof(SecteurActiviteViewModel));
                                liste.Cast<SecteurActiviteViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(TarifViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<TarifViewModelMap>();
                                csvWriter1.WriteHeader(typeof(TarifViewModel));
                                liste.Cast<TarifViewModel>().ForEach(csvWriter1.WriteRecord);                             
                            }
                            else if (type == typeof(TypeContactViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<TypeContactViewModelMap>();
                                csvWriter1.WriteHeader(typeof(TypeContactViewModel));
                                liste.Cast<TypeContactViewModel>().ForEach(csvWriter1.WriteRecord);                                
                            }
                            else if (type == typeof(TypeDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<TypeDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(TypeDeviceViewModel));
                                liste.Cast<TypeDeviceViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(TypeMailViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<TypeMailViewModelMap>();
                                csvWriter1.WriteHeader(typeof(TypeMailViewModel));
                                liste.Cast<TypeMailViewModel>().ForEach(csvWriter1.WriteRecord);                             
                            }
                            else if (type == typeof(TypeObjetViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<TypeObjetViewModelMap>();
                                csvWriter1.WriteHeader(typeof(TypeObjetViewModel));
                                liste.Cast<TypeObjetViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(TypeRatioViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<TypeRatioViewModelMap>();
                                csvWriter1.WriteHeader(typeof(TypeRatioViewModel));
                                liste.Cast<TypeRatioViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(TypeUsageViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<TypeUsageViewModelMap>();
                                csvWriter1.WriteHeader(typeof(TypeUsageViewModel));
                                liste.Cast<TypeUsageViewModel>().ForEach(csvWriter1.WriteRecord);                              
                            }
                            else if (type == typeof(UsageViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<UsageViewModelMap>();
                                csvWriter1.WriteHeader(typeof(UsageViewModel));
                                liste.Cast<UsageViewModel>().ForEach(csvWriter1.WriteRecord);                            
                            }
                            else if (type == typeof(UsageDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<UsageDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(UsageDeviceViewModel));
                                liste.Cast<UsageDeviceViewModel>().ForEach(csvWriter1.WriteRecord);                              
                            }
                            else if (type == typeof(VentilationClasseAgeDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<VentilationClasseAgeDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(VentilationClasseAgeDeviceViewModel));
                                liste.Cast<VentilationClasseAgeDeviceViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            else if (type == typeof(VentilationClasseDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<VentilationClasseDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(VentilationClasseDeviceViewModel));
                                liste.Cast<VentilationClasseDeviceViewModel>().ForEach(csvWriter1.WriteRecord);                              
                            }
                            //else if (type == typeof(CleModeleViewModel))
                            //{
                            //    csvWriter1.Configuration.RegisterClassMap<ConsoWattHeurViewModelMap>();
                            //    csvWriter1.WriteHeader(typeof(CleModeleViewModel));
                            //    liste.Cast<CleModeleViewModel>().ForEach(csvWriter1.WriteRecord);                           
                            //}
                            //else if (type == typeof(ContenuModereViewModel))
                            //{
                            //    csvWriter1.Configuration.RegisterClassMap<ConsoWattHeurViewModelMap>();
                            //    csvWriter1.WriteHeader(typeof(ContenuModereViewModel));
                            //    liste.Cast<ContenuModereViewModel>().ForEach(csvWriter1.WriteRecord);                             
                            //}
                            else if (type == typeof(LangueViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<LangueViewModelMap>();
                                csvWriter1.WriteHeader(typeof(LangueViewModel));
                                liste.Cast<LangueViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            }
                            //else if (type == typeof(ModeleViewModel))
                            //{
                            //    csvWriter1.Configuration.RegisterClassMap<ConsoWattHeurViewModelMap>();
                            //    csvWriter1.WriteHeader(typeof(ModeleViewModel));
                            //    liste.Cast<ModeleViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            //}
                            //else if (type == typeof(PageViewModel))
                            //{
                            //    csvWriter1.Configuration.RegisterClassMap<ConsoWattHeurViewModelMap>();
                            //    csvWriter1.WriteHeader(typeof(PageViewModel));
                            //    liste.Cast<PageViewModel>().ForEach(csvWriter1.WriteRecord);                                
                            //}
                            //else if (type == typeof(RoutageViewModel))
                            //{
                            //    csvWriter1.Configuration.RegisterClassMap<ConsoWattHeurViewModelMap>();
                            //    csvWriter1.WriteHeader(typeof(RoutageViewModel));
                            //    liste.Cast<RoutageViewModel>().ForEach(csvWriter1.WriteRecord);                               
                            //}
                            //else if (type == typeof(SectionViewModel))
                            //{
                            //    csvWriter1.Configuration.RegisterClassMap<ConsoWattHeurViewModelMap>();
                            //    csvWriter1.WriteHeader(typeof(SectionViewModel));
                            //    liste.Cast<SectionViewModel>().ForEach(csvWriter1.WriteRecord);
                            //}
                            else if (type == typeof(DeviseViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<DeviseViewModelMap>();
                                csvWriter1.WriteHeader(typeof(DeviseViewModel));
                                liste.Cast<DeviseViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(CorrespondanceSecteurActiviteViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<CorrespondanceSecteurActiviteViewModelMap>();
                                csvWriter1.WriteHeader(typeof(CorrespondanceSecteurActiviteViewModel));
                                liste.Cast<CorrespondanceSecteurActiviteViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(CorrespondanceTypeDeviceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<CorrespondanceTypeDeviceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(CorrespondanceTypeDeviceViewModel));
                                liste.Cast<CorrespondanceTypeDeviceViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(CorrespondanceProcesseurViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<CorrespondanceProcesseurViewModelMap>();
                                csvWriter1.WriteHeader(typeof(CorrespondanceProcesseurViewModel));
                                liste.Cast<CorrespondanceProcesseurViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(CorrespondanceVitesseViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<CorrespondanceVitesseViewModelMap>();
                                csvWriter1.WriteHeader(typeof(CorrespondanceVitesseViewModel));
                                liste.Cast<CorrespondanceVitesseViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(CorrespondanceTailleViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<CorrespondanceTailleViewModelMap>();
                                csvWriter1.WriteHeader(typeof(CorrespondanceTailleViewModel));
                                liste.Cast<CorrespondanceTailleViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            else if (type == typeof(CorrespondancePuissanceViewModel))
                            {
                                csvWriter1.Configuration.RegisterClassMap<CorrespondancePuissanceViewModelMap>();
                                csvWriter1.WriteHeader(typeof(CorrespondancePuissanceViewModel));
                                liste.Cast<CorrespondancePuissanceViewModel>().ForEach(csvWriter1.WriteRecord);
                            }
                            streamWriter.Flush();
                            return File(new MemoryStream(stream.ToArray()), "text/csv", nomFichierFinale);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                ViewBag.Erreur = e.StackTrace;
                ViewBag.MessageUnitaire = Resource.Traduction.Export_Erreur;
            }
            return null;
        }
    }
}
