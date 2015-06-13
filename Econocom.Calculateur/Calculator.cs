using System;
using System.Collections.Generic;
using System.Linq;
using Econocom.Helper;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Calculateur
{
    public class Calculator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const int NbJoursAnnee = 365;
        private DetaillesUsageViewModel _usage;
        private int _effectif;
        private decimal _prixKwatt;
        private decimal _coefficientConversion;
        private int _nbSitePays;
        private decimal _tauxEquipement;
        private decimal _nombreEquipement;
        private int _superficieDatacenter;

        enum TypeRatio
        {
	        NombreEquipement = 1, // On commence par 1
	        NombreEquipementEffectif,
	        NombreSites,
	        ChiffreDaffaires,
	        SuperficieTotale
        };

        #region Section Formules

        /// <summary>
        /// Calculer le nombre d'equipements basé sur le taux d'epuipement et l'effectif
        /// Formule: (Taux Equipement / 100 ) x Effectif
        /// </summary>
        private void CalculerNombreEquipement()
        {
            _nombreEquipement = (_tauxEquipement / 100) * _effectif;
            Logger.Debug("_nombreEquipement = " + _nombreEquipement);
        }

        /// <summary>
        /// Calculer le Nombre Total de Devices basé sur le type ratio
        /// </summary>
        /// <param name="typeRatio"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        private decimal CalculerTotalNombreDevice(int typeRatio, decimal ratio)
        {
            Logger.Debug("CalculerTotalNombreDevice typeRatio=" + typeRatio+", ratio="+ratio);
            switch (typeRatio)
            {
                case (int)TypeRatio.NombreEquipement:
                    return (ratio / 100) * _nombreEquipement;
                case (int)TypeRatio.NombreEquipementEffectif:
                    return (ratio / 100) * _effectif;
                case (int)TypeRatio.NombreSites:
                    return (ratio / 100) * _nbSitePays;
                case (int)TypeRatio.ChiffreDaffaires:
                    return (ratio / 100) * _effectif;       // Pas d'info sur ce type ratio 
                case (int)TypeRatio.SuperficieTotale:
                    return (ratio / 100) * _effectif;       // Pas d'info sur ce type ratio
                default:
                    return (ratio / 100) * _effectif;      
            }
        }

        /// <summary>
        /// Calculer le nombre de device pour une classe device
        /// Formule: (Ventilation du Device / 100 ) x Nombre total device
        /// </summary>
        /// <param name="pourcentage"></param>
        /// <param name="totalDevice"></param>
        /// <returns></returns>
        public decimal CalculerNombreDevice(decimal pourcentage, decimal totalDevice)
        {            
            decimal val= (pourcentage / 100) * totalDevice;
            Logger.Debug("CalculerNombreDevice porcentage={0}, totalDevice={1}, val={2}", pourcentage, totalDevice, val);

            return val;
        }
        
        /// <summary>
        /// Calculer le nombre d'heures usage 
        /// Formule: (Nombre heures usage x nombre device age x nombre jours actif par an)
        /// </summary>
        /// <param name="nbHrUsage"></param>
        /// <param name="nbDeviceAge"></param>
        /// <param name="nbJourTravaille"></param>
        /// <returns></returns>
        private decimal CalculerUsage(decimal nbHrUsage, decimal nbDeviceAge, decimal nbJourTravaille)
        {
            decimal val = (nbHrUsage*nbDeviceAge*nbJourTravaille);
            Logger.Debug("CalculerUsage, nbHrUsage={0}, nbDeviceAge={1}, nbJourTravaille={2}, (nbHrUsage*nbDeviceAge*nbJourTravaille)={3}", nbHrUsage, nbDeviceAge, nbJourTravaille, val);
            return val;
        }

        /// <summary>
        /// Calculer consommation Watt
        /// Formule: Consommation Hr x Ratio heure
        /// </summary>
        /// <param name="consoWattHr"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        private decimal CalculerConsommationWatt(decimal consoWattHr, decimal ratio)
        {
            decimal val = (consoWattHr * ratio);
            Logger.Debug("CalculerConsommationWatt, consoWattHr={0}, ratio={1}, (consoWattHr * ratio)={2}", consoWattHr, ratio, val);
            return val;
        }

        /// <summary>
        /// Calculer consommation Watt
        /// Formule: (Consommation Watt x Nombre heure utilisé) / 1000
        /// </summary>
        /// <param name="consoWatt"></param>
        /// <param name="nbHrUtilise"></param>
        /// <returns></returns>
        private decimal CalculerConsommationKWatt(decimal consoWatt, decimal nbHrUtilise)
        {
            decimal val = ((consoWatt * nbHrUtilise) / 1000);

            Logger.Debug("CalculerConsommationKWatt, consoWatt={0}, nbHrUtilise={1}, ((consoWatt * nbHrUtilise) / 1000 ={2}", consoWatt, nbHrUtilise, val);

            return val;
        }

        /// <summary>
        /// Calculer consommation Watt externe
        /// Formule: Total Watt consommé en interne x Taux utilisation externe x coefficient reduction watt externe
        /// </summary>
        /// <param name="totalWattInterne"></param>
        /// <param name="tauxUtilisationExterne"></param>
        /// <param name="coeffReductionWattExterne"></param>
        /// <returns></returns>
        private decimal CalculerConsommationWattExterne(decimal totalWattInterne, decimal tauxUtilisationExterne, decimal coeffReductionWattExterne)
        {
            decimal val = (totalWattInterne * tauxUtilisationExterne * coeffReductionWattExterne);
            Logger.Debug("CalculerTotalCarbonEmis, totalWattInterne={0}, tauxUtilisationExterne={1},coeffReductionWattExterne={2}, (totalWattInterne * tauxUtilisationExterne * coeffReductionWattExterne)={3}", 
                totalWattInterne, tauxUtilisationExterne, coeffReductionWattExterne, val);
            return val;
        }

        /// <summary>
        /// Calculer emission de C02
        /// Formule: (Total Watt consommé en interne x Coefficient Conversion) / 100
        /// </summary>
        /// <param name="totalWattInterne"></param>
        /// <returns></returns>
        private decimal CalculerTotalCarbonEmis(decimal totalWattInterne)
        {
            decimal val = (totalWattInterne * _coefficientConversion) / 1000;
            Logger.Debug("CalculerTotalCarbonEmis, totalWattInterne={0}, (totalWattInterne * _coefficientConversion) / 1000 ={1}", totalWattInterne, val);
            return val;
        }

        /// <summary>
        /// Calculer pourcentage consommation annuelle
        /// </summary>
        /// <param name="conso"></param>
        /// <param name="totalConso"></param>
        /// <returns></returns>
        public decimal CalculerPourcentageConsoAnnuelle(decimal conso, decimal totalConso)
        {
            decimal val = 0;
            if (totalConso > 0)
                val= ((conso/totalConso)*100);
            Logger.Debug("CalculerPourcentageConsoAnnuelle, conso={0}, totalConso={1},(totalWattInterne * _coefficientConversion) / 1000 ={2}", conso, totalConso, val);
            return val;
        }

        /// <summary>
        /// Calculer le total depensé
        /// Formule: total KWatt x Prix du Kwatt
        /// </summary>
        public decimal TotalDepense(decimal totalKwatt)
        {
            decimal val = (totalKwatt * _prixKwatt);
            Logger.Debug("CalculerPourcentageConsoAnnuelle, totalKwatt={0}, _prixKwatt={1},(totalKwatt * _prixKwatt) ={2}", totalKwatt, _prixKwatt, val);
            return val;
        }

        #endregion Section Formules

        /// <summary>
        /// Calculer total consommation watt & co2 pour toutes les deviceFamilles
        /// </summary>
        /// <param name="calculateurViewModel"></param>
        /// <returns></returns>
        public CalculateurViewModel CalculerTotalConsommation(CalculateurViewModel calculateurViewModel)
        {
            Logger.Debug("Start calculation");
            var debut2 = ConsommationHelper.GetStartTime("CalculerTotalConsommation start");
            try
            {
                var familleDeviceListe = new List<FamilleDeviceViewModel>();
                decimal? consommationWatt = 0;
                decimal? consommationCo2 = 0;
                decimal? prix = 0;

                _effectif = calculateurViewModel.Effectif;
                _prixKwatt = calculateurViewModel.PrixKWatt;
                _coefficientConversion = calculateurViewModel.CoefficientConversion ?? 0; 
                _nbSitePays = calculateurViewModel.NombreSite;
                _tauxEquipement = calculateurViewModel.TauxEquipement;
                _superficieDatacenter = calculateurViewModel.SuperficieDataCenter ?? 0;

                Logger.Debug("_effectif = " + _effectif);
                Logger.Debug("_prixKwatt = " + _prixKwatt);
                Logger.Debug("_coefficientConversion = " + _coefficientConversion);
                Logger.Debug("_nbSitePays = " + _nbSitePays);
                Logger.Debug("_tauxEquipement = " + _tauxEquipement);
                Logger.Debug("_superficieDatacenter = " + _superficieDatacenter);


                CalculerNombreEquipement();

                //Calculer total consommation pour chaque deviceFamille
                foreach (var deviceFamille in calculateurViewModel.FamilleDeviceViewModels)
                {
                    var familleDeviceViewModel = deviceFamille;
                    //if (familleDeviceViewModel.IsModified)
                        familleDeviceViewModel = CalculerTotalConsommationParFamille(familleDeviceViewModel);
                    familleDeviceListe.Add(familleDeviceViewModel);
                }

                //Calculer total consommation watt & co2 pour toutes les deviceFamilles
                foreach (var famille in familleDeviceListe)
                {
                    consommationWatt = consommationWatt + (famille.ConsoWatt.HasValue?famille.ConsoWatt.Value:0);
                    consommationCo2 = consommationCo2 + (famille.ConsoCo2.HasValue?famille.ConsoCo2.Value:0);
                    prix = prix + (famille.Prix.HasValue?famille.Prix.Value:0);
                }

                calculateurViewModel.TotalConsoWatt = consommationWatt;
                calculateurViewModel.TotalConsoCo2 = consommationCo2;
                calculateurViewModel.TotalPrix = prix;

                foreach (var deviceFamille in calculateurViewModel.FamilleDeviceViewModels)
                {
                    deviceFamille.Pourcentage = CalculerPourcentageConsoAnnuelle(deviceFamille.ConsoWatt ?? 0, calculateurViewModel.TotalConsoWatt??0);
                }

                Logger.Debug("END calculation ");
                return calculateurViewModel;
            }
            catch (Exception e)
            {
                Logger.Error("CalculerNbDeviceEtConsoParClasseAge, calculateurViewModel={0}", calculateurViewModel);
                throw e;
            }
        }

        /// <summary>
        /// Calculer total consommation pour chaque deviceFamille
        /// </summary>
        /// <param name="familleDeviceViewModel"></param>
        /// <returns></returns>
        public FamilleDeviceViewModel CalculerTotalConsommationParFamille(FamilleDeviceViewModel familleDeviceViewModel)
        {
            var debut2 = ConsommationHelper.GetStartTime("CalculerTotalConsommationParFamille pour :" + familleDeviceViewModel.LibelleFamilleDevice + "Start");

            try
            {
                var categorieDeviceListe = new List<CategorieDeviceViewModel>();
                decimal? consommationWatt = 0;
                decimal? consommationCo2 = 0;
                decimal? prix = 0;
                Logger.Debug("CalculerTotalConsommationParFamille famille=" +
                             familleDeviceViewModel.LibelleFamilleDevice);
                
                //Calculer total consommation pour chaque categorieDevice
                foreach (var categorieDevice in familleDeviceViewModel.CategorieDevices)
                {
                    Logger.Debug("categorieDevice " + categorieDevice.LibelleCategorieDevice);
                    var categorieDeviceViewModel = categorieDevice;
                    //if (categorieDeviceViewModel.IsModified)
                    categorieDeviceViewModel = CalculerTotalConsommationParCategorie(categorieDeviceViewModel);
                    categorieDeviceListe.Add(categorieDeviceViewModel);
                }

                //Calculer total consommation watt & co2 pour toutes les categorieDevices
                foreach (var categorie in categorieDeviceListe)
                {
                    consommationWatt = consommationWatt + (categorie.ConsoWatt.HasValue?categorie.ConsoWatt.Value:0);
                    consommationCo2 = consommationCo2 + (categorie.ConsoCo2.HasValue?categorie.ConsoCo2.Value:0);
                    prix = prix + (categorie.Prix.HasValue?categorie.Prix.Value:0);
                }

                familleDeviceViewModel.CategorieDevices = categorieDeviceListe;
                familleDeviceViewModel.ConsoWatt = consommationWatt;
                familleDeviceViewModel.ConsoCo2 = consommationCo2;
                familleDeviceViewModel.Prix = prix;
                Logger.Debug("CalculerTotalConsommationParCategorie categorie={0}, consowatt={1}, consoCO2={2}, prix={3}",
                             familleDeviceViewModel.LibelleFamilleDevice, consommationWatt, consommationCo2, prix);

                foreach (var categorieDevice in familleDeviceViewModel.CategorieDevices)
                {
                    categorieDevice.Pourcentage = CalculerPourcentageConsoAnnuelle(categorieDevice.ConsoWatt ?? 0,
                                                                                   familleDeviceViewModel.ConsoWatt ?? 0);
                }
            }
            catch (Exception e)
            {
                Logger.Error("CalculerNbDeviceEtConsoParClasseAge, familleDeviceViewModel={0}", familleDeviceViewModel);
                throw e;
            }
            var fin2 = ConsommationHelper.GetTimeDifference(debut2, "CalculerTotalConsommationParFamille pour :" + familleDeviceViewModel.LibelleFamilleDevice + "End");
            return familleDeviceViewModel;
        }

        /// <summary>
        /// Calculer total consommation pour chaque categorieDevice
        /// </summary>
        /// <param name="categorieDeviceViewModel"></param>
        /// <returns></returns>
        public CategorieDeviceViewModel CalculerTotalConsommationParCategorie(CategorieDeviceViewModel categorieDeviceViewModel)
        {

            var debut2 = ConsommationHelper.GetStartTime("CalculerTotalConsommationParCategorie pour :" + categorieDeviceViewModel.LibelleCategorieDevice + "Start");
            
            try
            {
                var typeDeviceListe = new List<TypeDeviceViewModel>();
                decimal? consommationWatt = 0;
                decimal? consommationCo2 = 0;
                decimal? prix = 0;
                Logger.Debug("CalculerTotalConsommationParCategorie categorie=" +
                             categorieDeviceViewModel.LibelleCategorieDevice);
                //Calculer total consommation pour chaque typeDevice
                foreach (var typeDevice in categorieDeviceViewModel.TypeDevices)
                {
                    Logger.Debug("typeDevice =" + typeDevice.LibelleTypeDevice);
                    var typeDeviceViewModel = typeDevice;
                    if(typeDeviceViewModel.IsModified || typeDeviceViewModel.ConfigSiDevice.ConfigSiConsommations == null)
                        typeDeviceViewModel = CalculerTotalKWattConsommeParTypeDevice(typeDevice);
                    typeDeviceListe.Add(typeDeviceViewModel);
                }

                //Calculer total consommation watt & co2 pour toutes les typeDevices
                foreach (var device in typeDeviceListe)
                {
                    consommationWatt = consommationWatt + device.ConsoWatt;
                    consommationCo2 = consommationCo2 + device.ConsoCo2;
                    prix = prix + device.Prix;
                }

                categorieDeviceViewModel.TypeDevices = typeDeviceListe;
                categorieDeviceViewModel.ConsoWatt = consommationWatt;
                categorieDeviceViewModel.ConsoCo2 = consommationCo2;
                categorieDeviceViewModel.Prix = prix;
                Logger.Debug("CalculerTotalConsommationParCategorie categorie={0}, consowatt={1}, consoCO2={2}, prix={3}",
                             categorieDeviceViewModel.LibelleCategorieDevice, consommationWatt, consommationCo2, prix);
            }
            catch (Exception e)
            {
                Logger.Error("CalculerNbDeviceEtConsoParClasseAge, categorieDeviceViewModel={0}", categorieDeviceViewModel.LibelleCategorieDevice);
                throw e;
            }

            var fin2 = ConsommationHelper.GetTimeDifference(debut2, "CalculerTotalConsommationParCategorie pour :" + categorieDeviceViewModel.LibelleCategorieDevice + "End");
            return categorieDeviceViewModel;
        }

        /// <summary>
        /// Calculer total consommation par Type Device
        /// z</summary>
        /// <returns></returns>
        public TypeDeviceViewModel CalculerTotalKWattConsommeParTypeDevice(TypeDeviceViewModel typeDeviceViewModel)
        {
            var debut2 = ConsommationHelper.GetStartTime("CalculerTotalKWattConsommeParTypeDevice pour :" + typeDeviceViewModel.LibelleTypeDevice + "Start");

            try
            {
                decimal totalConsoWattParDeviceParAge = 0;
                decimal totalConsoWattParDevice = 0;
                decimal totalConsoWattInterne = 0;
                decimal totalConsoWattExterne = 0;
                decimal totalConsoWattInterneExterne = 0;
                decimal totalCarbonInterne = 0;
                decimal totalCarbonExterne = 0;
                decimal totalConsoCo2InterneExterne = 0;
                decimal totalDepense = 0;

                bool estNouveau = false;
                Logger.Debug("CalculerTotalKWattConsommeParTypeDevice typeDevice ={0}",
                             typeDeviceViewModel.LibelleTypeDevice);
                //calculer pour la premiere fois
                if (typeDeviceViewModel.ConfigSiDevice.Quantite == 0 && typeDeviceViewModel.ConfigSiDevice.ConfigSiConsommations == null)
                {
                    estNouveau = true;
                    Logger.Debug("CalculerTotalKWattConsommeParTypeDevice estNouveau ={0}", estNouveau);

                    // Sauvegarder le nombre devices par classe dans le modele typeDeviceViewModel
                    Logger.Debug("CalculerTotalKWattConsommeParTypeDevice typeDevice ={0}, _effectif={1}",
                                 typeDeviceViewModel.LibelleTypeDevice, _effectif);
                    typeDeviceViewModel = CalculerNombreDeviceParClasse(typeDeviceViewModel, _effectif);

                    // Sauvegarder le nombre devices par classe et age + consommation dans le modele typeDeviceViewModel
                    Logger.Debug("CalculerTotalKWattConsommeParTypeDevice, CalculerNbDeviceEtConsoParClasseAge");
                    typeDeviceViewModel = CalculerNbDeviceEtConsoParClasseAge(typeDeviceViewModel);
                }
                else
                {
                    typeDeviceViewModel = CalculerNbDeviceEtConsoParClasseAgeExistant(typeDeviceViewModel);
                }

                // Calculer les totaux
                foreach (var classeDevice in typeDeviceViewModel.VentilationClasseDevices)
                {
                    totalConsoWattParDevice = 0;
                    foreach (var configSiConso in typeDeviceViewModel.ConfigSiDevice.ConfigSiConsommations)
                    {
                        if (configSiConso.ClasseDeviceId == classeDevice.ClasseDeviceId)
                        {
                            totalConsoWattParDeviceParAge = configSiConso.ConsommationWatt ?? 0;

                            // Ajouter la consommation interne pour chaque classe device 
                            totalConsoWattParDevice = totalConsoWattParDevice + totalConsoWattParDeviceParAge;
                        }
                    }
                    // Ajouter la consommation interne pour toutes les classes device (E, M et H)
                    totalConsoWattInterne = totalConsoWattInterne + totalConsoWattParDevice;
                }
                Logger.Debug("typeDeviceViewModel={0}, totalConsoWattInterne={1}", typeDeviceViewModel.LibelleTypeDevice,
                             totalConsoWattInterne);

                // Consommation Watt externe
                if (typeDeviceViewModel.ConfigSiDevice.UtiliseEnergieExterne)
                {
                    if (estNouveau)
                    {
                        totalConsoWattExterne = CalculerConsommationWattExterne(totalConsoWattInterne,
                            Convert.ToDecimal(
                                _usage.PourcentageConsoExterne),
                            Convert.ToDecimal(
                                _usage.CoeffReductionWattExterne));
                    }
                    else
                    {
                        totalConsoWattExterne = CalculerConsommationWattExterne(totalConsoWattInterne,
                            Convert.ToDecimal(
                                typeDeviceViewModel.ConfigSiDevice
                                    .PourcentConsomExtern),
                            Convert.ToDecimal(
                                typeDeviceViewModel.ConfigSiDevice
                                    .CoeffReductionWattExterne));
                    }
                }

                // Total interne + externe            
                totalConsoWattInterneExterne = totalConsoWattInterne + totalConsoWattExterne;
                Logger.Debug(
                    "CalculerTotalKWattConsommeParTypeDevice, totalConsoWattInterne={0}, totalConsoWattExterne={1}, totalConsoWattInterneExterne={2}",
                    totalConsoWattInterne, totalConsoWattExterne, totalConsoWattInterneExterne);

                // Total dépensé 
                totalDepense = TotalDepense(totalConsoWattInterneExterne); // Not used?
                Logger.Debug("CalculerTotalKWattConsommeParTypeDevice, totalDepense={0}, totalConsoWattExterne={1}",
                             totalDepense, totalConsoWattInterneExterne);
                typeDeviceViewModel.Prix = totalDepense;

                // Total CO2 emis en interne
                totalCarbonInterne = CalculerTotalCarbonEmis(totalConsoWattInterne);
                Logger.Debug(
                    "CalculerTotalKWattConsommeParTypeDevice, totalCarbonInterne={0}, totalConsoWattInterne={1}",
                    totalCarbonInterne, totalConsoWattInterne);

                // Total CO2 emis en externe
                totalCarbonExterne = CalculerTotalCarbonEmis(totalConsoWattExterne);
                Logger.Debug(
                    "CalculerTotalKWattConsommeParTypeDevice, totalCarbonExterne={0}, totalConsoWattExterne={1}",
                    totalCarbonExterne, totalConsoWattExterne);

                // Total interne + externe
                totalConsoCo2InterneExterne = totalCarbonInterne + totalCarbonExterne;
                Logger.Debug(
                    "CalculerTotalKWattConsommeParTypeDevice, totalConsoCo2InterneExterne={0}, totalCarbonInterne={1}, totalCarbonExterne={2}",
                    totalConsoCo2InterneExterne, totalCarbonInterne, totalCarbonExterne);

                typeDeviceViewModel.ConsoCo2 = totalCarbonInterne; //totalConsoCo2InterneExterne; => Modif demandée par Daniel 15/03/2015
                Logger.Debug("CalculerTotalKWattConsommeParTypeDevice, typeDeviceViewModel.ConsoCo2={0}",
                             typeDeviceViewModel.ConsoCo2);

                typeDeviceViewModel.ConsoWatt = totalConsoWattInterne; //totalConsoWattInterneExterne; => Modif demandée par Daniel 15/03/2015
                Logger.Debug("CalculerTotalKWattConsommeParTypeDevice, typeDeviceViewModel.ConsoWatt={0}",
                             typeDeviceViewModel.ConsoWatt);
            }
            catch (Exception e)
            {
                Logger.Error("CalculerNbDeviceEtConsoParClasseAge, typeDeviceViewModel={0}", typeDeviceViewModel.LibelleTypeDevice);
                throw e;
            }

            var fin2 = ConsommationHelper.GetTimeDifference(debut2, "CalculerTotalKWattConsommeParTypeDevice pour :" + typeDeviceViewModel.LibelleTypeDevice + "End");
            
            return typeDeviceViewModel;
        }
       
        /// <summary>
        /// Calculer consommation par age et classe pour les devices existants dans le modele typeDeviceViewModel
        /// </summary>
        /// <param name="typeDeviceViewModel"></param>
        /// <returns></returns>
        private TypeDeviceViewModel CalculerNbDeviceEtConsoParClasseAgeExistant(TypeDeviceViewModel typeDeviceViewModel)
        {
            var debut2 = ConsommationHelper.GetStartTime("CalculerNbDeviceEtConsoParClasseAgeExistant pour :" + typeDeviceViewModel.LibelleTypeDevice + "Start");
            
            try
            {
                decimal nbHeuresUtilise;
                decimal nbHrNonIntensif;
                decimal consoKWattIntensif;
                decimal consoKWattNonIntensif;
                decimal consoKWattModeOff;
                decimal consoWattHr;
                decimal consoWattHrNonIntensif;
                decimal consoWattHrOff;
                int totalConsoWattParDeviceParAge = 0;

                var configSiDevice = typeDeviceViewModel.ConfigSiDevice;
                Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, typeDeviceViewModel={0}", typeDeviceViewModel.LibelleTypeDevice);
                foreach (var classeDevice in typeDeviceViewModel.VentilationClasseDevices)
                {                   
                    foreach (var ageDevice in typeDeviceViewModel.VentilationClasseAgeDevices)
                    {                       
                        if (ageDevice.ClasseDeviceId == classeDevice.ClasseDeviceId)
                        {
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, classeDevice={0} ageDevice={1}", classeDevice.ClasseDeviceId, ageDevice.AgeDeviceId);
                            var configSiConsommation = configSiDevice.ConfigSiConsommations.Find(o =>(o.ClasseDeviceId == ageDevice.ClasseDeviceId &&o.AgeDeviceId == ageDevice.AgeDeviceId));

                            //------------- Intensif ---------------
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, INTENSIF NbrHeuresIntensifJour={0} ", configSiDevice.NbrHeuresIntensifJour);

                            nbHeuresUtilise = CalculerUsage(configSiDevice.NbrHeuresIntensifJour,configSiConsommation.Quantite,configSiDevice.NbrJourTravaille);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, INTENSIF nbHeuresUtilise={0} ", nbHeuresUtilise);

                            // Consomation KWATT
                            consoWattHr = configSiConsommation.ConsommationIntensifUnitaire;
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, INTENSIF consoWattHr={0} ", consoWattHr);

                            consoKWattIntensif = CalculerConsommationKWatt(consoWattHr, nbHeuresUtilise);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, INTENSIF consoKWattIntensif={0} ", consoKWattIntensif);

                            configSiConsommation.ConsommationIntensif = consoKWattIntensif;

                            //------------- Non Intensif ---------------
                            // Nombre heures non-intensif
                            nbHrNonIntensif = configSiDevice.NbrHeuresNonIntensifJour;
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, NON-INTENSIF nbHrNonIntensif={0} ", nbHrNonIntensif);

                            nbHeuresUtilise = CalculerUsage(nbHrNonIntensif, configSiConsommation.Quantite,configSiDevice.NbrJourTravaille);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, NON-INTENSIF nbHeuresUtilise={0} ", nbHeuresUtilise);

                            // Consomation KWATT
                            consoWattHrNonIntensif = CalculerConsommationWatt(consoWattHr,configSiDevice.CoeffNonIntensif);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, NON-INTENSIF consoWattHrNonIntensif={0} ", consoWattHrNonIntensif);

                            consoKWattNonIntensif = CalculerConsommationKWatt(consoWattHrNonIntensif, nbHeuresUtilise);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, NON-INTENSIF consoKWattNonIntensif={0} ", consoKWattNonIntensif);

                            configSiConsommation.ConsommationNonIntensif = consoKWattNonIntensif;

                            //------------- Mode Off ---------------
                            // Nombre heures en mode off
                            nbHeuresUtilise = CalculerUsage(configSiDevice.NbrHeuresOffJour, configSiConsommation.Quantite, configSiDevice.NbrJourTravaille);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, MODE OFF nbHeuresUtilise={0} ", nbHeuresUtilise);

                            // Consomation KWATT
                            consoWattHrOff = CalculerConsommationWatt(consoWattHr, configSiDevice.CoeffModeOff ?? 0);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, MODE OFF consoWattHrOff={0} ", consoWattHrOff);

                            consoKWattModeOff = CalculerConsommationKWatt(consoWattHrOff, nbHeuresUtilise);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant, MODE OFF consoKWattModeOff={0} ", consoKWattModeOff);

                            // Ajouter la consommation interne pour chaque type device par age
                            totalConsoWattParDeviceParAge = Convert.ToInt32(consoKWattIntensif + consoKWattNonIntensif + consoKWattModeOff);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAgeExistant,  totalConsoWattParDeviceParAge={0}+{1}+{2}={3} ",
                                consoKWattIntensif, consoKWattNonIntensif, consoKWattModeOff,totalConsoWattParDeviceParAge);

                            configSiConsommation.ConsommationWatt = totalConsoWattParDeviceParAge;
                            configSiConsommation.ConsommationCo2 = CalculerTotalCarbonEmis(totalConsoWattParDeviceParAge);
                            configSiConsommation.CoefficientConversion = _coefficientConversion;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("CalculerNbDeviceEtConsoParClasseAgeExistant, typeDeviceViewModel={0}", typeDeviceViewModel.LibelleTypeDevice);
                throw e;
            }
            
            var fin2 = ConsommationHelper.GetTimeDifference(debut2, "CalculerNbDeviceEtConsoParClasseAgeExistant pour :" + typeDeviceViewModel.LibelleTypeDevice + "End");
            return typeDeviceViewModel;
        }

        /// <summary>
        /// Sauvegarder le nombre devices par classe et age + consommation dans le modele typeDeviceViewModel
        /// </summary>
        /// <param name="typeDeviceViewModel"></param>
        /// <returns></returns>
        private TypeDeviceViewModel CalculerNbDeviceEtConsoParClasseAge(TypeDeviceViewModel typeDeviceViewModel)
        {
            var debut2 = ConsommationHelper.GetStartTime("CalculerNbDeviceEtConsoParClasseAge pour :" + typeDeviceViewModel.LibelleTypeDevice + "Start");

            try
            {
                decimal nbDeviceAge;
                decimal nbHeuresUtilise;
                decimal nbHrNonIntensif;
                decimal consoKWattIntensif;
                decimal consoKWattNonIntensif;
                decimal consoKWattModeOff;
                decimal consoWattHr;
                decimal consoWattHrNonIntensif;
                decimal consoWattHrOff;
                var nbDeviceAgeList = new List<decimal>();
                var listeConfigSiConsommation = new List<ConfigSiConsommation>();
                int count;
                int nombreDeviceAge;
                int totalConsoWattParDeviceParAge = 0;

                Logger.Debug("CalculerNbDeviceEtConsoParClasseAge,typeDeviceViewModel={0}",
                             typeDeviceViewModel.LibelleTypeDevice);
                foreach (var classeDevice in typeDeviceViewModel.VentilationClasseDevices)
                {
                    count = 0;
                    nbDeviceAgeList.Clear();

                    // Pour chaque device E, M et H, calculter quantité des devices par tranche d'age
                    foreach (var ageDevice in typeDeviceViewModel.VentilationClasseAgeDevices)
                    {
                        Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, ageDevice.ClasseDeviceId={0}",
                                     ageDevice.ClasseDeviceId);
                        if (ageDevice.ClasseDeviceId == classeDevice.ClasseDeviceId)
                        {
                            Logger.Debug(
                                "CalculerNbDeviceEtConsoParClasseAge, ageDevice.Coefficient={0}, classeDevice.Quantite={1}",
                                ageDevice.Coefficient, classeDevice.Quantite);
                            nbDeviceAge = CalculerNombreDevice(ageDevice.Coefficient, classeDevice.Quantite);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, nbDeviceAge={0}", nbDeviceAge);
                            nbDeviceAgeList.Add(nbDeviceAge);
                        }
                    }

                    nbDeviceAgeList = ArrondirDecimal(nbDeviceAgeList);

                    foreach (var ageDevice in typeDeviceViewModel.VentilationClasseAgeDevices)
                    {
                        Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, ageDevice={0}", ageDevice.AgeDeviceId);
                        if (ageDevice.ClasseDeviceId == classeDevice.ClasseDeviceId)
                        {
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, ageDevice={0}", ageDevice.AgeDeviceId);
                            nombreDeviceAge = Convert.ToInt32(nbDeviceAgeList[count]);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, nombreDeviceAge={0}", nombreDeviceAge);
                            var configSiConsommation = new ConfigSiConsommation();
                            configSiConsommation.ClasseDeviceId = classeDevice.ClasseDeviceId;
                            configSiConsommation.AgeDeviceId = ageDevice.AgeDeviceId;
                            configSiConsommation.ConfigSiDeviceId = typeDeviceViewModel.ConfigSiDevice.Id;
                            configSiConsommation.DateModification = DateTime.Now;
                            configSiConsommation.Quantite = nombreDeviceAge;

                            _usage = typeDeviceViewModel.Usage;

                            //------------- Intensif ---------------
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge,CalculerUsage");
                            nbHeuresUtilise = CalculerUsage(Convert.ToDecimal(_usage.NbrHeuresIntensifJour),
                                                            nombreDeviceAge,
                                                            _usage.NbrJourTravaille);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, CalculerUsage val nbHeuresUtilise={0}",
                                         nbHeuresUtilise);

                            // Consomation KWATT
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, GetConsoWattHr");
                            consoWattHr = GetConsoWattHr(typeDeviceViewModel.ConsoWattHeurs, ageDevice.ClasseDeviceId,
                                                         ageDevice.AgeDeviceId);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, GetConsoWattHr val consoWattHr={0}",
                                         consoWattHr);
                            configSiConsommation.ConsommationIntensifUnitaire = consoWattHr;

                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, CalculerConsommationKWatt");
                            consoKWattIntensif = CalculerConsommationKWatt(consoWattHr, nbHeuresUtilise);
                            Logger.Debug(
                                "CalculerNbDeviceEtConsoParClasseAge, CalculerConsommationKWatt val consoKWattIntensif={0}",
                                consoKWattIntensif);

                            configSiConsommation.ConsommationIntensif = consoKWattIntensif;

                            //------------- Non Intensif ---------------
                            // Nombre heures non-intensif
                            nbHrNonIntensif = Convert.ToDecimal(_usage.NbrHeuresNonIntensifJour);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, nbHrNonIntensif={0}", nbHrNonIntensif);

                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, CalculerUsage");
                            nbHeuresUtilise = CalculerUsage(nbHrNonIntensif, nombreDeviceAge, _usage.NbrJourTravaille);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, CalculerUsage val={0}", nbHeuresUtilise);

                            // Consomation KWATT
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, CalculerConsommationWatt");
                            consoWattHrNonIntensif = CalculerConsommationWatt(consoWattHr,
                                                                              _usage.CoeffNonIntensif.HasValue
                                                                                  ? _usage.CoeffNonIntensif.Value
                                                                                  : 0);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, consoWattHrNonIntensif={0}",
                                         consoWattHrNonIntensif);

                            consoKWattNonIntensif = CalculerConsommationKWatt(consoWattHrNonIntensif, nbHeuresUtilise);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, consoKWattNonIntensif={0}",
                                         consoKWattNonIntensif);

                            configSiConsommation.ConsommationNonIntensif = consoKWattNonIntensif;

                            //------------- Mode Off ---------------
                            // Nombre heures en mode off
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, CalculerUsage");
                            nbHeuresUtilise = CalculerUsage(Convert.ToDecimal(_usage.NbrHeuresOffJour), nombreDeviceAge, _usage.NbrJourTravaille);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, nbHeuresUtilise={0}", nbHeuresUtilise);

                            // Consomation KWATT
                            consoWattHrOff = CalculerConsommationWatt(consoWattHr, _usage.CoeffModeOff ?? 0);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, consoWattHrOff={0}", consoWattHrOff);

                            consoKWattModeOff = CalculerConsommationKWatt(consoWattHrOff, nbHeuresUtilise);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, consoKWattModeOff={0}", consoKWattModeOff);

                            // Ajouter la consommation interne pour chaque type device par age
                            totalConsoWattParDeviceParAge =
                                Convert.ToInt32(consoKWattIntensif + consoKWattNonIntensif + consoKWattModeOff);
                            Logger.Debug("CalculerNbDeviceEtConsoParClasseAge, totalConsoWattParDeviceParAge={0}",
                                         totalConsoWattParDeviceParAge);

                            configSiConsommation.ConsommationWatt = totalConsoWattParDeviceParAge;
                            Logger.Debug(
                                "CalculerNbDeviceEtConsoParClasseAge, configSiConsommation.ConsommationWatt={0}",
                                configSiConsommation.ConsommationWatt);

                            configSiConsommation.ConsommationCo2 = CalculerTotalCarbonEmis(totalConsoWattParDeviceParAge);
                            Logger.Debug(
                                "CalculerNbDeviceEtConsoParClasseAge, configSiConsommation.ConsommationCo2={0}",
                                configSiConsommation.ConsommationCo2);

                            configSiConsommation.CoefficientConversion = _coefficientConversion;
                            Logger.Debug(
                                "CalculerNbDeviceEtConsoParClasseAge, configSiConsommation.CoefficientConversion={0}",
                                configSiConsommation.CoefficientConversion);

                            listeConfigSiConsommation.Add(configSiConsommation);

                            count++;
                        }
                    }
                    typeDeviceViewModel.ConfigSiDevice.ConfigSiConsommations = listeConfigSiConsommation;
                }
            }
            catch (Exception e)
            {
                Logger.Error("CalculerNbDeviceEtConsoParClasseAge, typeDeviceViewModel={0}", typeDeviceViewModel.LibelleTypeDevice);
                throw e;
            }
            var fin2 = ConsommationHelper.GetTimeDifference(debut2, "CalculerNbDeviceEtConsoParClasseAge pour :" + typeDeviceViewModel.LibelleTypeDevice + "End");
            return typeDeviceViewModel;
        }

        /// <summary>
        /// Sauvegarder le nombre devices par classe dans le modele typeDeviceViewModel
        /// </summary>
        /// <param name="typeDeviceViewModel"></param>
        /// <param name="effectif"></param>
        /// <returns></returns>
        private TypeDeviceViewModel CalculerNombreDeviceParClasse(TypeDeviceViewModel typeDeviceViewModel, int effectif)
        {
            var debut2 = ConsommationHelper.GetStartTime("CalculerNombreDeviceParClasse pour :" + typeDeviceViewModel.LibelleTypeDevice + "Start");

            try
            {
                decimal nbDeviceClasse;
                var listNbDevice = new List<decimal>();
                int count = 0;
                decimal totalDevice = 0;

                Logger.Debug("CalculerNombreDeviceParClasse typeDeviceViewModel={0}",
                             typeDeviceViewModel.LibelleTypeDevice);

                if (typeDeviceViewModel.Ratio != null)
                {
                    // Calculer le nombre total de devices
                    totalDevice = CalculerTotalNombreDevice(typeDeviceViewModel.Ratio.TypeRatioId,
                                                                    typeDeviceViewModel.Ratio.Valeur ?? 0);
                    Logger.Debug("CalculerNombreDeviceParClasse totalDevice={0}", totalDevice);
                }

                // Arrondir totalDevice? 
                totalDevice = Math.Round(totalDevice);
                Logger.Debug("CalculerNombreDeviceParClasse round totalDevice={0}", totalDevice);

                // Set quantite pour configDevice
                typeDeviceViewModel.ConfigSiDevice.Quantite = Convert.ToInt32(totalDevice);
                Logger.Debug("CalculerNombreDeviceParClasse typeDeviceViewModel.ConfigSiDevice.Quantite={0}",
                             typeDeviceViewModel.ConfigSiDevice.Quantite);

                // Calculer nombre de devices par classe E, M et H
                foreach (var classeDevice in typeDeviceViewModel.VentilationClasseDevices)
                {
                    Logger.Debug("CalculerNombreDeviceParClasse classeDevice={0}", classeDevice.Coefficient);
                    nbDeviceClasse = CalculerNombreDevice(classeDevice.Coefficient, totalDevice);
                    listNbDevice.Add(nbDeviceClasse);
                }

                listNbDevice = ArrondirDecimal(listNbDevice);

                foreach (var classeDevice in typeDeviceViewModel.VentilationClasseDevices)
                {
                    classeDevice.Quantite = Convert.ToInt32(listNbDevice[count]);
                    Logger.Debug("CalculerNombreDeviceParClasse classeDevice.Quantite ={0}", classeDevice.Quantite);
                    count++;
                }
            }
            catch (Exception e)
            {
                Logger.Error("CalculerNombreDeviceParClasse, typeDeviceViewModel{0}", typeDeviceViewModel.LibelleTypeDevice);
                throw e;
            }
            var fin2 = ConsommationHelper.GetTimeDifference(debut2, "CalculerNombreDeviceParClasse pour :" + typeDeviceViewModel.LibelleTypeDevice + "End");
            return typeDeviceViewModel;
        }
        
        /// <summary>
        /// Arrondir les decimals pour le nombre de devices     
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public List<decimal> ArrondirDecimal(List<decimal> liste)
        {
            try
            {
                if (!liste.Any())
                    return liste;
                Logger.Debug("ArrondirDecimal Liste en entree");
                foreach (var val in liste)
                {
                    Logger.Debug(val);
                }
                var listeDecimal = liste.Where(o => (o % 1 > 0));
                var totaleDecimal = (int)listeDecimal.Sum();
                ArrondirListeDecimal(liste, totaleDecimal);
                Logger.Debug("ArrondirDecimal Liste en sorti");
                foreach (var val in liste)
                {
                    Logger.Debug(val);
                }
                return liste;
            }
            catch (Exception e)
            {
                Logger.Error("ArrondirDecimal, error {0}", e);
                throw e;
            }
            return liste;
        }


        /// <summary>
        /// arrondir les decimales
        /// </summary>
        /// <param name="liste"></param>
        /// <param name="totaleDecimal"></param>
        /// <returns></returns>
        public List<decimal> ArrondirListeDecimal(List<decimal> liste, int totaleDecimal)
        {
            try
            {
                if (!liste.Any())
                    return liste;

                Logger.Debug("ArrondirListeDecimal Liste en entree");
                foreach (var val in liste)
                {
                    Logger.Debug(val);
                }

                var nbDecimale = liste.Count(o => (o % 1 > 0));

                //on n'a plus rien a arrondir
                if (totaleDecimal == 0 || nbDecimale == 1)
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        if (liste[i] % 1 > 0)
                            liste[i] = totaleDecimal;
                    }
                }
                else
                {
                    var max = liste.Where(o => (o % 1 > 0)).Max();
                    var min = liste.Where(o => (o % 1 > 0)).Min();

                    var max1 = 0;
                    var min1 = 0;
                    if (max > 0 && min > 0)
                    {
                        if (max == min)
                        {
                            var fraction = totaleDecimal % nbDecimale;

                            if (fraction == 0)
                            {
                                max1 = totaleDecimal / 2;
                                min1 = max1;
                            }
                            else
                            {
                                max1 = (int)Math.Ceiling(max);
                                min1 = (int)Math.Floor(min);
                            }
                        }
                        else
                        {
                            max1 = (int)Math.Ceiling(max);
                            min1 = (int)Math.Floor(min);
                        }
                        totaleDecimal = totaleDecimal - (min1 + max1);

                        int maxIndex = liste.FindIndex(o => o.Equals(max));
                        liste[maxIndex] = max1;
                        int minIndex = liste.FindIndex(o => o.Equals(min));
                        liste[minIndex] = min1;

                        var liste2 = liste.Any(o => (o % 1 != 0));
                        if (liste2)
                            ArrondirListeDecimal(liste, totaleDecimal);

                        Logger.Debug("ArrondirListeDecimal Liste en sorti");
                        foreach (var val in liste)
                        {
                            Logger.Debug(val);
                        }

                        return liste;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ArrondirDecimal, error {0}", e);
                throw e;
            }
            return liste;

        }

        /// <summary>
        /// Recupere le consommation watt à l'heure par classe device et age device
        /// </summary>
        /// <param name="listConsoWattHr"></param>
        /// <param name="classeDeviceId"></param>
        /// <param name="ageDeviceId"></param>
        /// <returns></returns>
        private decimal GetConsoWattHr(IEnumerable<ConsoWattHeurViewModel> listConsoWattHr, int classeDeviceId, int ageDeviceId)
        {
            if (null == listConsoWattHr)
                return 0;

            foreach (var consoWattHr in listConsoWattHr)
            {
                if (consoWattHr.ClasseDeviceId == classeDeviceId && consoWattHr.AgeDeviceId == ageDeviceId)
                {
                    Logger.Debug("GetConsoWattHr, consoWattHr.Consommation={0}", consoWattHr.Consommation);
                    return consoWattHr.Consommation;
                }
            }
            return 0;
        }
         
        
    }
}
