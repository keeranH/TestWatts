//using System.Collections.Generic;
//using Business.CalculatorEngine;
//using Econocom.Business.CalculatorEngine;

using System;
using System.Collections.Generic;
using Econocom.Model.Models.Benchmark;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using Econocom.Model.ViewModel;


namespace Econocom.Calculateur
{
    public class CalculatorTest
    {
        private const int NbJoursAnnee = 365;
        private DetaillesUsageViewModel _usage;

        private decimal _coeffVentilationClasseDevice;
        private decimal _nbDeviceClasse;
        private decimal _coeffVentilationClasseAgeDevice;
        private decimal _nbDeviceAge;
        private decimal _nbHeuresUtilise;
        private decimal _nbHrNonIntensif;
        private decimal _consoKWatt;
        private decimal _consoWattHr;
        private decimal _totalConsoWattParDeviceParAge = 0;
        private decimal _totalConsoWattParDevice = 0;
        private decimal _totalConsoWattInterne;
        private decimal _totalConsoWattExterne;
        private decimal _totalConsoWattInterneExterne;
        private decimal _totalDepense;
        private decimal _totalCarbonInterne;
        private decimal _totalCarbonExterne;
        private decimal _totalConsoCo2InterneExterne;
        private int _effectif;
        private decimal _prixKwatt;
        private decimal? _coefficientConversion;


        /// <summary>
        /// Calculer total consommation watt & co2 pour toutes les deviceFamilles
        /// </summary>
        /// <param name="calculateurViewModel"></param>
        /// <returns></returns>
        public Resultat CalculerTotalConsommation(CalculateurViewModel calculateurViewModel)
        {
            var resultat = new Resultat();
            var resultats = new List<Resultat>();
            decimal? consommationWatt = 0;
            decimal? consommationCo2 = 0;

            _effectif = calculateurViewModel.Effectif;
            _prixKwatt = calculateurViewModel.PrixKWatt;
            //Processing to be done!!!
            _coefficientConversion = calculateurViewModel.CoefficientConversion;

            //Calculer total consommation pour chaque deviceFamille
            foreach (var deviceFamille in calculateurViewModel.FamilleDeviceViewModels)
            {
                resultat = CalculerTotalConsommationParFamille(deviceFamille);
                resultats.Add(resultat);
            }

            //Calculer total consommation watt & co2 pour toutes les deviceFamilles
            foreach (var resultat1 in resultats)
            {
                consommationWatt = consommationWatt + resultat1.ConsommationWatt;
                consommationCo2 = consommationCo2 + resultat1.ConsommationCo2;
            }

            resultat.ConsommationWatt = consommationWatt;
            resultat.ConsommationCo2 = consommationCo2;

            return resultat;
        }

        /// <summary>
        /// Calculer total consommation pour chaque deviceFamille
        /// </summary>
        /// <param name="familleDeviceViewModel"></param>
        /// <returns></returns>
        public Resultat CalculerTotalConsommationParFamille(FamilleDeviceViewModel familleDeviceViewModel)
        {
            var resultat = new Resultat();
            var resultats = new List<Resultat>();
            decimal? consommationWatt = 0;
            decimal? consommationCo2 = 0;

            //Calculer total consommation pour chaque categorieDevice
            foreach (var categorieDevice in familleDeviceViewModel.CategorieDevices)
            {
                resultat = CalculerTotalConsommationParCategorie(categorieDevice);
                resultats.Add(resultat);
            }

            //Calculer total consommation watt & co2 pour toutes les categorieDevices
            foreach (var resultat1 in resultats)
            {
                consommationWatt = consommationWatt + resultat1.ConsommationWatt;
                consommationCo2 = consommationCo2 + resultat1.ConsommationCo2;
            }

            resultat.ConsommationWatt = consommationWatt;
            resultat.ConsommationCo2 = consommationCo2;

            return resultat;
        }

        /// <summary>
        /// Calculer total consommation pour chaque categorieDevice
        /// </summary>
        /// <param name="categorieDeviceViewModel"></param>
        /// <returns></returns>
        public Resultat CalculerTotalConsommationParCategorie(CategorieDeviceViewModel categorieDeviceViewModel)
        {
            var resultat = new Resultat();
            var resultats = new List<Resultat>();
            decimal? consommationWatt = 0;
            decimal? consommationCo2 = 0;

            //Calculer total consommation pour chaque typeDevice
            foreach (var typeDevice in categorieDeviceViewModel.TypeDevices)
            {
                resultat = CalculerTotalKWattConsomme(typeDevice, 1);
                resultats.Add(resultat);
            }

            //Calculer total consommation watt & co2 pour toutes les typeDevices
            foreach (var resultat1 in resultats)
            {
                consommationWatt = consommationWatt + resultat1.ConsommationWatt;
                consommationCo2 = consommationCo2 + resultat1.ConsommationCo2;
            }

            resultat.ConsommationWatt = consommationWatt;
            resultat.ConsommationCo2 = consommationCo2;

            return resultat;
        }

        /// <summary>
        /// Main function
        /// </summary>
        /// <returns></returns>
        public Resultat CalculerTotalKWattConsomme(TypeDeviceViewModel typeDeviceViewModel, int effectif)
        {
            var resultat = new Resultat();
            var resultatCalculateur = new ResultatCalculateurViewModel();
            var ListeConfigSiDevice = new List<ConfigSiDevice>();
            var ListeConfigSiConsommation = new List<ConfigSiConsommation>();

            // Calculer le nombre total de devices
            decimal totalDevice = Convert.ToDecimal((typeDeviceViewModel.Ratio.Valeur / 100) * effectif); //900
            //typeDeviceViewModel.ConfigSiDevice.Quantite = totalDevice;
            ListeConfigSiDevice.Add(typeDeviceViewModel.ConfigSiDevice);

            // Calculer nombre de devices par classe E, M et H
            foreach (var classeDevice in typeDeviceViewModel.VentilationClasseDevices)
            {
                _coeffVentilationClasseDevice = classeDevice.Coefficient; // 30
                _nbDeviceClasse = CalculerNombreDevice(_coeffVentilationClasseDevice, totalDevice); // 30/100 x 900 = 270

                // Pour chaque device E, M et H, calculter quantité des devices par tranche d'age
                foreach (var ageDevice in typeDeviceViewModel.VentilationClasseAgeDevices)
                {
                    /*ConfigSiConsommation configSiConsommation = new ConfigSiConsommation();
                    configSiConsommation.ClasseDeviceId = classeDevice.ClasseDeviceId;
                    configSiConsommation.AgeDeviceId = ageDevice.AgeDeviceId;
                    configSiConsommation.ConfigSiDeviceId = typeDeviceViewModel.ConfigSiDevice.Id;
                    configSiConsommation.ClasseDeviceId = 1;
                    configSiConsommation.AgeDeviceId = 1;
                    configSiConsommation.ConsommationIntensif = 9;
                    configSiConsommation.ConsommationNonIntensif = 8;
                    configSiConsommation.Quantite = 7;
                    configSiConsommation.DateModification = DateTime.Now;
                    configSiConsommation.ConsommationWatt = 98;
                    configSiConsommation.ConsommationCo2 = 58;
                    configSiConsommation.CoefficientConversion = 6;
                    typeDeviceViewModel.ConfigSiConsommation = configSiConsommation;
                    ListeConfigSiConsommation.Add(typeDeviceViewModel.ConfigSiConsommation);*/
                   
                    _coeffVentilationClasseAgeDevice = ageDevice.Coefficient; //50
                    _nbDeviceAge = CalculerNombreDevice(_coeffVentilationClasseAgeDevice, _nbDeviceClasse); // 50/100 x 270 = 135

                    //configSiConsommation.Quantite = _nbDeviceAge;
                    
                    // Usage (Intensif/Non-Intensif/Mode Off) par classe device, par age
                    /*foreach (var typeUsage in typeDeviceViewModel.Usages)
                    {*/
                        //_usage = typeDeviceViewModel;
                        _usage = typeDeviceViewModel.Usage;
                        if (_usage.TypeUsageId == CalculatorData.UsageStruct.Intensif)
                        {
                            _nbHeuresUtilise = CalculerUsage(Convert.ToDecimal(_usage.NbrHeuresIntensifJour), _nbDeviceAge, _usage.NbrJourTravaille); // 3.5 x 135 x 240 = 113400

                            // Consomation KWATT
                            _consoWattHr = 1;//CalculatorData.GetConsoWatt(classeDevice.Id, ageDevice.Id); // 41
                            _consoKWatt = CalculerConsommationKWatt(_consoWattHr, _nbHeuresUtilise); // (41 x 113400) /1000 = 4649
                        }
                        else if (_usage.TypeUsageId == CalculatorData.UsageStruct.NonIntensif)
                        {
                            // Nombre heures non-intensif
                            _nbHrNonIntensif = 24 - (Convert.ToDecimal(_usage.NbrHeuresOffJour) + Convert.ToDecimal(_usage.NbrHeuresOffJour));
                            _nbHeuresUtilise = CalculerUsage(_nbHrNonIntensif, _nbDeviceAge, _usage.NbrJourTravaille); // 4 x 135 x 240 = 129600

                            // Consomation KWATT
                            _consoWattHr = 1;// CalculatorData.GetConsoWatt(classeDevice.Id, ageDevice.Id); //Conso Intensif
                            _consoWattHr = CalculerConsommationWatt(_consoWattHr, _usage.CoeffNonIntensif.HasValue ? _usage.CoeffNonIntensif.Value : 0); // 41 x 0.55 = 22.55
                            _consoKWatt = CalculerConsommationKWatt(_consoWattHr, _nbHeuresUtilise); // (22.55 x 129600) / 1000 = 2922
                        }
                        else
                        {
                            // Nombre heures en mode off
                            _nbHeuresUtilise = CalculerUsage(Convert.ToDecimal(_usage.NbrHeuresOffJour), NbJoursAnnee, _usage.NbrJourTravaille); // 16.5 x 365 x 135 = 813038

                            // Consomation KWATT
                            _consoWattHr = 1;//CalculatorData.GetConsoWatt(classeDevice.Id, ageDevice.Id); //Conso Intensif
                            _consoWattHr = CalculerConsommationWatt(_consoWattHr, _usage.CoeffModeOff ?? 0); // default value = 0, 41 x 0.02 = 0.82
                            _consoKWatt = CalculerConsommationKWatt(_consoWattHr, _nbHeuresUtilise); // (0.82 x 813038) / 1000 = 667
                        }

                        // Ajouter la consommation interne pour chaque type device par age
                        _totalConsoWattParDeviceParAge = _totalConsoWattParDeviceParAge + _consoKWatt; // 4649 + 2922 + 667 = 8239 (E<1an)
                    }

                    // Ajouter la consommation interne pour chaque type device 
                    _totalConsoWattParDevice = _totalConsoWattParDevice + _totalConsoWattParDeviceParAge; // 8239 + others = 18382 (Device E)
                //}

                // Ajouter la consommation interne pour toutes les classes device
                _totalConsoWattInterne = _totalConsoWattInterne + _totalConsoWattParDevice; // 18382 + total M + total H = 87067
            }

            // Consommation Watt externe
            _totalConsoWattExterne = CalculerConsommationWattExterne(_totalConsoWattInterne,
                                                                    Convert.ToDecimal(_usage.PourcentageConsoExterne),//to be checked!!!
                                                                    Convert.ToDecimal(_usage.CoeffReductionWattExterne));

            // Total interne + externe
            _totalConsoWattInterneExterne = _totalConsoWattInterne + _totalConsoWattExterne;

            // Total dépensé
            _totalDepense = _totalConsoWattInterneExterne * Convert.ToDecimal(_prixKwatt);

            // Total CO2 emis en interne
            _totalCarbonInterne = CalculerTotalCarbonEmis(_totalConsoWattInterne);

            // Total CO2 emis en externe
            _totalCarbonExterne = CalculerTotalCarbonEmis(_totalConsoWattExterne);

            // Total interne + externe
            _totalConsoCo2InterneExterne = _totalCarbonInterne + _totalCarbonExterne;

            resultat.ConsommationCo2 = _totalConsoCo2InterneExterne;
            resultat.ConsommationWatt = _totalConsoWattInterneExterne;

            return resultat;
        }


        /// <summary>
        /// Calculer le nombre de device pour une classe device
        /// Formule: (Ventilation du Device / 100 ) x Nombre total device
        /// </summary>
        /// <param name="pourcentage"></param>
        /// <param name="totalDevice"></param>
        /// <returns></returns>
        private decimal CalculerNombreDevice(decimal pourcentage, decimal totalDevice)
        {
            return (pourcentage / 100) * totalDevice;
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
            return (nbHrUsage * nbDeviceAge * nbJourTravaille);
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
            return (consoWattHr * ratio);
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
            return ((consoWatt * nbHrUtilise) / 1000);
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
            return (totalWattInterne * tauxUtilisationExterne * coeffReductionWattExterne);
        }

        /// <summary>
        /// Calculer emission de C02
        /// Formule: (Total Watt consommé en interne x Emission C02 par KWatt) / 1000
        /// </summary>
        /// <param name="totalWattInterne"></param>
        /// <returns></returns>
        private decimal CalculerTotalCarbonEmis(decimal totalWattInterne)
        {
            return ((totalWattInterne * Convert.ToDecimal(CalculatorData.emissionCarbonParKWatt)) / 1000);
        }

    }
}
