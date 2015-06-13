﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;
using CsvHelper;
using Econocom.Helper.Rapport;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;
using Econocom.Model.ViewModel.Report;

namespace Econocom.Admin.Controllers.GestionReference
{
    public class CsvMapper
    {


    }

    public class AgeDeviceViewModelMap : CsvClassMap<AgeDeviceViewModel>
    {        
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleAgeDevice);
            Map(m => m.DateDebut);           
            Map(m => m.DateModification);
            Map(m => m.DateFin);

            //Map(m => m.SecteurActiviteId).Ignore();
            //Map(m => m.IdDevice).Ignore();
            //Map(m => m.IdClasse).Ignore();
            //Map(m => m.LibelleDevice).Ignore();
            //Map(m => m.UniqueGuid).Ignore();
            //Map(m => m.IsModified).Ignore();
        }
    }

    public class ClasseDeviceViewModelMap : CsvClassMap<ClasseDeviceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleClasseDevice);
            Map(m => m.DateDebut);            
            Map(m => m.DateModification);
            Map(m => m.DateFin);

            //Map(m => m.SecteurActiviteId).Ignore();            
            //Map(m => m.AgeDeviceConsoViewModels).Ignore();           
        }
    }

    public class CategorieDeviceViewModelMap : CsvClassMap<CategorieDeviceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleCategorieDevice);
            Map(m => m.FamilleDeviceId);
            Map(m => m.DateDebut);           
            Map(m => m.DateModification);
            Map(m => m.DateFin);

           /* Map(m => m.TypeDevices).Ignore();
            Map(m => m.IsModified).Ignore();
            Map(m => m.ConsoWatt).Ignore();
            Map(m => m.Prix).Ignore();
            Map(m => m.Pourcentage).Ignore();*/
        }
    }
    public class FamilleDeviceViewModelMap : CsvClassMap<FamilleDeviceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleFamilleDevice);
            Map(m => m.DateDebut);           
            Map(m => m.DateModification);
            Map(m => m.DateFin);

            //Map(m => m.CategorieDevices).Ignore();
            //Map(m => m.SecteurActiviteId).Ignore();
            //Map(m => m.IsModified).Ignore();
            //Map(m => m.ConsoWatt).Ignore();
            //Map(m => m.Prix).Ignore();
            //Map(m => m.Pourcentage).Ignore();    
        }
    }

    public class ChangementAnneeCalendaireViewModelMap : CsvClassMap<ChangementAnneeCalendaireViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.AgeDeviceId);
            Map(m => m.CoefficientSource);
            Map(m => m.CoefficientDestination);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class ConsoWattHeurViewModelMap : CsvClassMap<ConsoWattHeurViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeDeviceId);
            Map(m => m.ClasseDeviceId);
            Map(m => m.AgeDeviceId);           
            Map(m => m.Consommation);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class ContenuMailViewModelMap : CsvClassMap<ContenuMailViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeMailId);
            Map(m => m.LangueId);
            Map(m => m.LangueId);
            Map(m => m.Contenu);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class EquivalenceViewModelMap : CsvClassMap<EquivalenceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleEquivalence);            
            Map(m => m.Mesure);
            Map(m => m.Valeur);
            Map(m => m.LienImage);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class EquivalenceTraductionViewModelMap : CsvClassMap<EquivalenceTraductionViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LangueId);
            Map(m => m.LibelleEquivalence);
            Map(m => m.Mesure);
            Map(m => m.Valeur);
            Map(m => m.LienImage);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class OrigineViewModelMap : CsvClassMap<OrigineViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleOrigine);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class PaysViewModelMap : CsvClassMap<PaysViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibellePays);
            Map(m => m.CoefficientConversion);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class PolitiqueViewModelMap : CsvClassMap<PolitiqueViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibellePolitique);
            Map(m => m.Objectif);
            Map(m => m.Benefice);
            Map(m => m.Device);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class PolitiqueTraductionViewModelMap : CsvClassMap<PolitiqueTraductionViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LangueId);
            Map(m => m.PolitiqueId);
            Map(m => m.LibellePolitique);            
            Map(m => m.Benefice);
            Map(m => m.Device);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class QuestionViewModelMap : CsvClassMap<QuestionViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleQuestion);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class RatioViewModelMap : CsvClassMap<RatioViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleRatio);
            Map(m => m.SecteurActiviteId);
            Map(m => m.TypeDeviceId);
            Map(m => m.TypeRatioId);
            Map(m => m.Valeur);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class SecteurActiviteViewModelMap : CsvClassMap<SecteurActiviteViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleSecteurActivite);
            Map(m => m.SegmentActivite);
            Map(m => m.TypeActivite);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class TarifViewModelMap : CsvClassMap<TarifViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleTarif);
            Map(m => m.NbreMaxDevice);
            Map(m => m.NbreMinDevice);
            Map(m => m.TarifAnnuel);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class TypeContactViewModelMap : CsvClassMap<TypeContactViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleTypeContact);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class TypeDeviceViewModelMap : CsvClassMap<TypeDeviceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleTypeDevice);
            Map(m => m.CategorieDeviceId);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class TypeMailViewModelMap : CsvClassMap<TypeMailViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleTypeMail);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class TypeObjetViewModelMap : CsvClassMap<TypeObjetViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.NomObjet);
            Map(m => m.NomTable);
            Map(m => m.Namespace);            
        }
    }

    public class TypeRatioViewModelMap : CsvClassMap<TypeRatioViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleTypeRatio);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class TypeUsageViewModelMap : CsvClassMap<TypeUsageViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.LibelleTypeUsage);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class UsageViewModelMap : CsvClassMap<UsageViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeUsageId);
            Map(m => m.NbrJourTravaille);
            Map(m => m.NbrHeuresIntensifJour);
            Map(m => m.NbrHeuresNonIntensifJour);
            Map(m => m.NbrHeuresOffJour);
            Map(m => m.UtiliseEnergieExterne);
            Map(m => m.CoeffNonIntensif);
            Map(m => m.CoeffModeOff);
            Map(m => m.CoeffReductionWattExterne);
            Map(m => m.PourcentageConsoExterne);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class UsageDeviceViewModelMap : CsvClassMap<UsageDeviceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeDeviceId);
            Map(m => m.UsageId);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class VentilationClasseAgeDeviceViewModelMap : CsvClassMap<VentilationClasseAgeDeviceViewModel>
    {
        public override void CreateMap()
        {           
            Map(m => m.SecteurActiviteId);
            Map(m => m.TypeDeviceId);
            Map(m => m.ClasseDeviceId);
            Map(m => m.AgeDeviceId);
            Map(m => m.Coefficient);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class VentilationClasseDeviceViewModelMap : CsvClassMap<VentilationClasseDeviceViewModel>
    {
        public override void CreateMap()
        {           
            Map(m => m.SecteurActiviteId);
            Map(m => m.TypeDeviceId);
            Map(m => m.ClasseDeviceId);            
            Map(m => m.Coefficient);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class DeviseViewModelMap : CsvClassMap<DeviseViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.Libelle);
            Map(m => m.CodeDevise);
            Map(m => m.UrlImage);           
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }  

    public class LangueViewModelMap : CsvClassMap<LangueViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.Code);
            Map(m => m.Nom);
            Map(m => m.Culture);
            Map(m => m.Defaut);
            Map(m => m.Pays);
            Map(m => m.Localise);
            Map(m => m.LienImage);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class CorrespondanceSecteurActiviteViewModelMap : CsvClassMap<CorrespondanceSecteurActiviteViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeSocieteId);
            Map(m => m.CodeActivite);
            Map(m => m.SecteurActiviteId);            
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class CorrespondanceTypeDeviceViewModelMap : CsvClassMap<CorrespondanceTypeDeviceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeSocieteId);
            Map(m => m.TypeDeviceId);
            Map(m => m.CodeCategorie);
            Map(m => m.CodeFamille);
            Map(m => m.CodeSousFamille);
            Map(m => m.Caracteristique);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class CorrespondanceProcesseurViewModelMap : CsvClassMap<CorrespondanceProcesseurViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeSocieteId);
            Map(m => m.CodeProcesseur);
            Map(m => m.ClasseDeviceId);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class CorrespondanceVitesseViewModelMap : CsvClassMap<CorrespondanceVitesseViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeSocieteId);
            Map(m => m.Vitesse1);
            Map(m => m.Vitesse2);
            Map(m => m.Vitesse3);
            Map(m => m.TypeDeviceId);
            Map(m => m.Vitesse1ClasseDeviceId);
            Map(m => m.Vitesse2ClasseDeviceId);
            Map(m => m.Vitesse3ClasseDeviceId);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class CorrespondanceTailleViewModelMap : CsvClassMap<CorrespondanceTailleViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeSocieteId);
            Map(m => m.Taille1ClasseDeviceId);
            Map(m => m.Taille2ClasseDeviceId);
            Map(m => m.Taille3ClasseDeviceId);
            Map(m => m.Taille1);
            Map(m => m.Taille2);
            Map(m => m.Taille3);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }

    public class CorrespondancePuissanceViewModelMap : CsvClassMap<CorrespondancePuissanceViewModel>
    {
        public override void CreateMap()
        {
            Map(m => m.Id);
            Map(m => m.TypeSocieteId);
            Map(m => m.Puissance1ClasseDeviceId);
            Map(m => m.Puissance2ClasseDeviceId);
            Map(m => m.Puissance3ClasseDeviceId);
            Map(m => m.Puissance1);
            Map(m => m.Puissance2);
            Map(m => m.Puissance3);
            Map(m => m.DateDebut);
            Map(m => m.DateModification);
            Map(m => m.DateFin);
        }
    }
}