using System;
using System.Collections.Generic;
using System.Linq;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel.Report;
using Econocom.Resource;
using NLog;

namespace Econocom.Helper.Rapport
{
    /// <summary>
    /// 
    /// </summary>
    public class RapportHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public enum ClasseDevice
        {
            EntreeDeGamme = 1, // On commence par 1
            MoyenDeGamme = 2,
            HautDeGamme = 3
        };

        public enum AgeDevice
        {
            MoinsDunAn = 1, // On commence par 1
            UnATroisAns = 2,
            TroisACinqAns = 3,
            PlusDeCinqAns = 4
        };

        public List<RapportConfigViewModel> GetRapportConfigViewModel(List<ConfigSiDevice> configSiDevices )
        {
            var listeRapportConfigViewModel = new List<RapportConfigViewModel>();
            RapportConfigViewModel rapportConfigViewModel = null;
            try
            {
                int idFamille = 0;
                int idCategorie = 0;
                CategorieDevice previousCategorie = null;
                bool nouvelleCategorie = true;
                bool nouvelleFamille = true;
                var orderedDevices = configSiDevices.OrderBy(o => o.TypeDevice.CategorieDevice.FamilleDevice.Id).ThenBy(o => o.TypeDevice.CategorieDevice.Id).ThenBy(o => o.TypeDevice.Id);
              
                var orderedList = orderedDevices.ToList();                
                for (int i = 0; i < orderedList.Count; i++)
                {
                    var configSiDevice = orderedList[i];

                    if (configSiDevice == null)
                        continue;

                    rapportConfigViewModel = new RapportConfigViewModel();
                    rapportConfigViewModel.Device = configSiDevice.LibelleDevice;

                    if (configSiDevice.TypeDevice != null && configSiDevice.TypeDevice.CategorieDevice != null)
                    {
                        if (configSiDevice.TypeDevice.CategorieDevice.FamilleDevice != null)
                        {
                            var currentIdFamille = configSiDevice.TypeDevice.CategorieDevice.FamilleDevice.Id;
                            if (idFamille != currentIdFamille)
                            {
                                nouvelleFamille = true;
                                idFamille = currentIdFamille;
                            }
                            else
                            {
                                nouvelleFamille = false;
                            }
                        }

                        var currentIdCategorie = configSiDevice.TypeDevice.CategorieDevice.Id;
                        if (idCategorie != currentIdCategorie)
                        {
                            nouvelleCategorie = true;
                            idCategorie = currentIdCategorie;
                        }
                        else
                        {
                            nouvelleCategorie = false;
                        }

                        // ajouter totale pour famille
                        if (nouvelleFamille)
                        {
                            var familleData = SetFamilleData(configSiDevices, configSiDevice.TypeDevice.CategorieDevice.FamilleDevice);
                            listeRapportConfigViewModel.Add(familleData);
                        }

                        // ajouter totale device
                        var device = new List<ConfigSiDevice>();
                        device.Add(configSiDevice);
                        SetRapportData(rapportConfigViewModel, device);
                        listeRapportConfigViewModel.Add(rapportConfigViewModel);

                        if (nouvelleCategorie)
                            rapportConfigViewModel.Categorie = configSiDevice.TypeDevice.CategorieDevice.LibelleCategorieDevice;

                        //ajouter totale categorie
                        var insererTotaleCategorie = InsererTotaleCategorie(orderedList, i, idCategorie);
                        if (insererTotaleCategorie)
                        {
                            var categorieData = SetCategorieData(configSiDevices, idCategorie, configSiDevice.TypeDevice.CategorieDevice.LibelleCategorieDevice);
                            listeRapportConfigViewModel.Add(categorieData);
                        }
                    }
                }
                //totale si
                var totalSI = SetConfigSiData(configSiDevices);
                if (totalSI != null)
                    listeRapportConfigViewModel.Add(totalSI);
            }
            catch (Exception)
            {
              
                throw;
            }
            return listeRapportConfigViewModel;
        }

        public RapportCsvPersonnalisationViewModel GetRapportCsv(Econocom.Model.Models.Benchmark.Rapport rapport)
        {
            if (rapport == null)
            {
                Logger.Info("Rapport {0} is null");
                return null;
            }

            if (rapport.ConfigSi==null)
            {
                Logger.Info("Rapport {0}, configsi is null", rapport.Id);
                return null;
            }

            if (rapport.ConfigSi.ConfigSiDevices==null)
            {
                Logger.Info("Rapport {0}, configsi {1}, configsidevice is null", rapport.Id, rapport.ConfigSi.Id);
                return null;
            }

            var configSiDevices = rapport.ConfigSi.ConfigSiDevices.ToList();
            var rapportCsvPersonnalisationViewModel = new RapportCsvPersonnalisationViewModel();
            rapportCsvPersonnalisationViewModel.NomRapport = rapport.LibelleRapport;
            rapportCsvPersonnalisationViewModel.TitreRapport = Econocom.Resource.Traduction.TitreRapportMaPersonnalisation;
            rapportCsvPersonnalisationViewModel.DateRapport = rapport.DateDebut;

            var personnalisation = new List<RapportConfigViewModel>();

            try
            {
                personnalisation = GetRapportConfigViewModel(configSiDevices);
                rapportCsvPersonnalisationViewModel.Personnalisation = personnalisation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }

            return rapportCsvPersonnalisationViewModel;
        }


        public List<RapportConfigViewModel> MAJPrix(List<RapportConfigViewModel> rapports, Contact contact)
        {
           try
           {
               var prixKwatt = contact.Client.DetailsClient.PrixKWH;
               var devise = contact.Client.DetailsClient.Devise;

               foreach (var rapport in rapports)
               {
                   rapport.CoutInterneAnnuelleKWH = (rapport.ConsoInterneAnnuelleKWH * prixKwatt).ToString("#.##");
                   rapport.CoutExterneAnnuelleKWH = (rapport.ConsoExterneAnnuelleKWH * prixKwatt).ToString("#.##");
                   rapport.CoutAnnuelkWtClasseE = (rapport.ConsoAnnuelleClasseEKWH * prixKwatt).ToString("#.##");
                   rapport.CoutAnnuelkWtClasseH = (rapport.ConsoAnnuelleClasseHKWH * prixKwatt).ToString("#.##");
                   rapport.CoutAnnuelkWtClasseM = (rapport.ConsoAnnuelleClasseMKWH * prixKwatt).ToString("#.##");
               }
           }
           catch (Exception e)
           {
               Logger.Error(e.StackTrace);
               throw;
           }

           return rapports;
        }

        private bool InsererTotaleCategorie(List<ConfigSiDevice> orderedList, int currentIndex, int currentCategorieId)
        {
            var insererTotaleCategorie = false;
            try
            {
                var j = currentIndex + 1;
                if (j < orderedList.Count)
                {
                    var nextDevice = orderedList.ElementAt(j);
                    if (nextDevice != null)
                    {
                        var nextCategorieId = nextDevice.TypeDevice.CategorieDevice.Id;
                        if (nextCategorieId != currentCategorieId)
                        {
                            insererTotaleCategorie = true;
                        }
                    }
                }
                else // on arrive a la fin de la liste
                {
                    insererTotaleCategorie = true;
                }
            }
            catch (Exception)
            {
                    
                throw;
            }
            return insererTotaleCategorie;
        }

        public void SetRapportData(RapportConfigViewModel rapportConfigViewModel, List<ConfigSiDevice> liste)
        {
            rapportConfigViewModel.ConsoInterneAnnuelleKWH = liste.Sum(o => o.ConfigSiConsommations.Sum(c => c.ConsommationWatt.HasValue ? c.ConsommationWatt.Value : 0));
            rapportConfigViewModel.ConsoInterneAnnuelleCO2 = liste.Sum(o => o.ConfigSiConsommations.Sum(c => c.ConsommationCo2.HasValue ? c.ConsommationCo2.Value : 0));
            rapportConfigViewModel.NombreTotalDevice = liste.Sum(o => o.ConfigSiConsommations.Sum(c => c.Quantite));

            rapportConfigViewModel = CalculerConsommationExterne(rapportConfigViewModel, liste);

            var listeE = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => p.ClasseDeviceId == (int)ClasseDevice.EntreeDeGamme));
            rapportConfigViewModel.ConsoAnnuelleClasseEKWH = listeE.Sum(p => p.ConsommationWatt.HasValue ? p.ConsommationWatt.Value : 0);
            rapportConfigViewModel.ConsoAnnuelleClasseECO2 = listeE.Sum(p => p.ConsommationCo2.HasValue ? p.ConsommationCo2.Value : 0);
            rapportConfigViewModel.NombreTotalClasseE = listeE.Sum(p => p.Quantite);

            var listeM = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => p.ClasseDeviceId == (int)ClasseDevice.MoyenDeGamme));
            rapportConfigViewModel.ConsoAnnuelleClasseMKWH = listeM.Sum(p => p.ConsommationWatt.HasValue ? p.ConsommationWatt.Value : 0);
            rapportConfigViewModel.ConsoAnnuelleClasseMCO2 = listeM.Sum(p => p.ConsommationCo2.HasValue ? p.ConsommationCo2.Value : 0);
            rapportConfigViewModel.NombreTotalClasseM = listeM.Sum(p => p.Quantite);

            var listeH = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => p.ClasseDeviceId == (int)ClasseDevice.HautDeGamme));
            rapportConfigViewModel.ConsoAnnuelleClasseHKWH = listeH.Sum(p => p.ConsommationWatt.HasValue ? p.ConsommationWatt.Value : 0);
            rapportConfigViewModel.ConsoAnnuelleClasseHCO2 = listeH.Sum(p => p.ConsommationCo2.HasValue ? p.ConsommationCo2.Value : 0);
            rapportConfigViewModel.NombreTotalClasseH = listeH.Sum(p => p.Quantite);

            rapportConfigViewModel.NombreClasseEMoinDunAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.EntreeDeGamme && p.AgeDeviceId == (int)AgeDevice.MoinsDunAn))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseEUnATroisAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.EntreeDeGamme && p.AgeDeviceId == (int)AgeDevice.UnATroisAns))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseETroisACinqAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.EntreeDeGamme && p.AgeDeviceId == (int)AgeDevice.TroisACinqAns))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseEPlusCinqAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.EntreeDeGamme && p.AgeDeviceId == (int)AgeDevice.PlusDeCinqAns))).Sum(s => s.Quantite);

            rapportConfigViewModel.NombreClasseMMoinDunAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.MoyenDeGamme && p.AgeDeviceId == (int)AgeDevice.MoinsDunAn))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseMUnATroisAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.MoyenDeGamme && p.AgeDeviceId == (int)AgeDevice.UnATroisAns))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseMTroisACinqAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.MoyenDeGamme && p.AgeDeviceId == (int)AgeDevice.TroisACinqAns))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseMPlusCinqAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.MoyenDeGamme && p.AgeDeviceId == (int)AgeDevice.PlusDeCinqAns))).Sum(s => s.Quantite);

            rapportConfigViewModel.NombreClasseHMoinDunAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.HautDeGamme && p.AgeDeviceId == (int)AgeDevice.MoinsDunAn))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseHUnATroisAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.HautDeGamme && p.AgeDeviceId == (int)AgeDevice.UnATroisAns))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseHTroisACinqAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.HautDeGamme && p.AgeDeviceId == (int)AgeDevice.TroisACinqAns))).Sum(s => s.Quantite);
            rapportConfigViewModel.NombreClasseHPlusCinqAns = liste.SelectMany(o => o.ConfigSiConsommations.Where(p => (p.ClasseDeviceId == (int)ClasseDevice.HautDeGamme && p.AgeDeviceId == (int)AgeDevice.PlusDeCinqAns))).Sum(s => s.Quantite);
        }


        public RapportConfigViewModel CalculerConsommationExterne(RapportConfigViewModel rapportConfigViewModel, List<ConfigSiDevice> configSiDevices)
        {
            decimal consoExterneAnnuelleKWH = 0;
            decimal consoExterneAnnuelleCO2 = 0;

            try
            {
                foreach (var configSiDevice in configSiDevices)
                {
                    decimal consoExterneKWH = 0;
                    decimal consoExterneCO2 = 0;

                    if (configSiDevice.UtiliseEnergieExterne)
                    {
                        var consoWattInterne = configSiDevice.ConfigSiConsommations.Sum(c => c.ConsommationWatt.HasValue ? c.ConsommationWatt.Value : 0);
                        consoExterneKWH = consoWattInterne * configSiDevice.PourcentConsomExtern * configSiDevice.CoeffReductionWattExterne;

                        var coeffConversion = configSiDevice.ConfigSiConsommations.FirstOrDefault().CoefficientConversion;
                        consoExterneCO2 = Convert.ToDecimal((consoExterneKWH * coeffConversion) / 1000);

                        consoExterneAnnuelleKWH = consoExterneAnnuelleKWH + consoExterneKWH;
                        consoExterneAnnuelleCO2 = consoExterneAnnuelleCO2 + consoExterneCO2;
                    }
                }

                rapportConfigViewModel.ConsoExterneAnnuelleKWH = consoExterneAnnuelleKWH;
                rapportConfigViewModel.ConsoExterneAnnuelleCO2 = consoExterneAnnuelleCO2;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }

            return rapportConfigViewModel;
        }


        public RapportConfigViewModel SetConfigSiData(List<ConfigSiDevice> configSiDevices)
        {
            var rapportConfigViewModel = new RapportConfigViewModel();
            try
            {
                var liste = configSiDevices;

                rapportConfigViewModel.Famille = Econocom.Resource.Traduction.TotalSI;
                SetRapportData(rapportConfigViewModel, liste);                
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return rapportConfigViewModel;
        }

        public RapportConfigViewModel SetFamilleData(List<ConfigSiDevice> configSiDevices, FamilleDevice famille)
        {
            var rapportConfigViewModel = new RapportConfigViewModel();
            try
            {
                var query = configSiDevices.Where(o => o.TypeDevice.CategorieDevice.FamilleDeviceId == famille.Id);
                var liste = query.ToList();

                rapportConfigViewModel.Famille = famille.LibelleFamilleDevice;
                SetRapportData(rapportConfigViewModel, liste);                
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return rapportConfigViewModel;
        }

        public RapportConfigViewModel SetCategorieData(List<ConfigSiDevice> configSiDevices, int idCategorie, string categorie)
        {
            var rapportConfigViewModel = new RapportConfigViewModel();
            try
            {
                var query = configSiDevices.Where(o => o.TypeDevice.CategorieDevice.Id == idCategorie);
                var liste = query.ToList();
                rapportConfigViewModel.Device = "TOTAL "+categorie.ToUpper();
                SetRapportData(rapportConfigViewModel, liste);                                
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return rapportConfigViewModel;
        }

        public void SetClasseAgeData(RapportConfigViewModel rapportConfigViewModel, ConfigSiDevice configSiDevice,
                                     ClasseDevice classeDevice, AgeDevice ageDevice)
        {
            switch (classeDevice)
            {
                case ClasseDevice.EntreeDeGamme:
                    switch (ageDevice)
                    {
                        case AgeDevice.MoinsDunAn:
                            rapportConfigViewModel.NombreClasseEMoinDunAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.EntreeDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.MoinsDunAn)).Sum(o => o.Quantite);

                            break;
                        case AgeDevice.UnATroisAns:
                            rapportConfigViewModel.NombreClasseEUnATroisAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.EntreeDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.UnATroisAns)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.TroisACinqAns:
                            rapportConfigViewModel.NombreClasseETroisACinqAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.EntreeDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.TroisACinqAns)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.PlusDeCinqAns:
                            rapportConfigViewModel.NombreClasseEPlusCinqAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.EntreeDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.PlusDeCinqAns)).Sum(o => o.Quantite);
                            break;
                    }
                    break;
                case ClasseDevice.MoyenDeGamme:
                    switch (ageDevice)
                    {
                        case AgeDevice.MoinsDunAn:
                            rapportConfigViewModel.NombreClasseMMoinDunAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.MoyenDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.MoinsDunAn)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.UnATroisAns:
                            rapportConfigViewModel.NombreClasseMUnATroisAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.MoyenDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.UnATroisAns)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.TroisACinqAns:
                            rapportConfigViewModel.NombreClasseMTroisACinqAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.MoyenDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.TroisACinqAns)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.PlusDeCinqAns:
                            rapportConfigViewModel.NombreClasseMPlusCinqAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.MoyenDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.PlusDeCinqAns)).Sum(o => o.Quantite);
                            break;
                    }
                    break;
                case ClasseDevice.HautDeGamme:
                    switch (ageDevice)
                    {
                        case AgeDevice.MoinsDunAn:
                            rapportConfigViewModel.NombreClasseHMoinDunAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.HautDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.MoinsDunAn)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.UnATroisAns:
                            rapportConfigViewModel.NombreClasseHUnATroisAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.HautDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.UnATroisAns)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.TroisACinqAns:
                            rapportConfigViewModel.NombreClasseHTroisACinqAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.HautDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.TroisACinqAns)).Sum(o => o.Quantite);
                            break;
                        case AgeDevice.PlusDeCinqAns:
                            rapportConfigViewModel.NombreClasseHPlusCinqAns =
                                configSiDevice.ConfigSiConsommations.Where(
                                    o =>
                                    (o.ClasseDeviceId == (int) ClasseDevice.HautDeGamme &&
                                     o.AgeDeviceId == (int) AgeDevice.PlusDeCinqAns)).Sum(o => o.Quantite);
                            break;
                    }
                    break;
            }
        }
        public void SetClasseData(RapportConfigViewModel rapportConfigViewModel, ConfigSiDevice configSiDevice,ClasseDevice classeDevice)
        {
            switch (classeDevice)
            {
                case ClasseDevice.EntreeDeGamme:
                    var listeE = configSiDevice.ConfigSiConsommations.Where(
                        o =>
                        (o.ClasseDeviceId == (int) ClasseDevice.EntreeDeGamme));

                    rapportConfigViewModel.NombreTotalClasseE = listeE.Sum(o => o.Quantite);
                    rapportConfigViewModel.ConsoAnnuelleClasseEKWH = listeE.Sum(o => o.ConsommationWatt.HasValue ? o.ConsommationWatt.Value : 0);
                    rapportConfigViewModel.ConsoAnnuelleClasseECO2 = listeE.Sum(o => o.ConsommationCo2.HasValue ? o.ConsommationCo2.Value : 0);

                    break;
                case ClasseDevice.MoyenDeGamme:
                    var listeM =
                               configSiDevice.ConfigSiConsommations.Where(
                                   o =>
                                   (o.ClasseDeviceId == (int)ClasseDevice.MoyenDeGamme));
                    rapportConfigViewModel.NombreTotalClasseM = listeM.Sum(o => o.Quantite);
                    rapportConfigViewModel.ConsoAnnuelleClasseMKWH = listeM.Sum(o => o.ConsommationWatt.HasValue ? o.ConsommationWatt.Value : 0);
                    rapportConfigViewModel.ConsoAnnuelleClasseMCO2 = listeM.Sum(o => o.ConsommationCo2.HasValue ? o.ConsommationCo2.Value : 0);
                    break;
                case ClasseDevice.HautDeGamme:
                    var listeH =
                               configSiDevice.ConfigSiConsommations.Where(
                                   o =>
                                   (o.ClasseDeviceId == (int)ClasseDevice.HautDeGamme));
                    rapportConfigViewModel.NombreTotalClasseH = listeH.Sum(o => o.Quantite);
                    rapportConfigViewModel.ConsoAnnuelleClasseHKWH = listeH.Sum(o => o.ConsommationWatt.HasValue ? o.ConsommationWatt.Value : 0);
                    rapportConfigViewModel.ConsoAnnuelleClasseHCO2 = listeH.Sum(o => o.ConsommationCo2.HasValue ? o.ConsommationCo2.Value : 0);
                    break;
            }

        }

        public RapportCsvSimulationViewModel GetRapportSimulationCsv(Simulation simulation)
        {
            if (simulation == null)
            {
                Logger.Info("Rapport {0} is null");
                return null;
            }

            if (simulation.ConfigSiSimule == null)
            {
                Logger.Info("Rapport {0}, ConfigSiSimule is null", simulation.Id);
                return null;
            }

            if (simulation.ConfigSiSimule.ConfigSiDevices == null)
            {
                Logger.Info("Rapport {0}, ConfigSiSimule {1}, configsidevice is null", simulation.Id, simulation.ConfigSiSimule.Id);
                return null;
            }

            if (simulation.ConfigSiSource == null)
            {
                Logger.Info("Rapport {0}, ConfigSiSource is null", simulation.Id);
                return null;
            }

            if (simulation.ConfigSiSource.ConfigSiDevices == null)
            {
                Logger.Info("Rapport {0}, ConfigSiSource {1}, configsidevice is null", simulation.Id, simulation.ConfigSiSource.Id);
                return null;
            }

            var rapportCsvSimulationViewModel = new RapportCsvSimulationViewModel();
            rapportCsvSimulationViewModel.NomRapport = simulation.LibelleSimulation;
            rapportCsvSimulationViewModel.TitreRapport = "Rapport Ma Simulation";
            rapportCsvSimulationViewModel.DateRapport = simulation.DateDebut;

            var configSiDevices = simulation.ConfigSiSource.ConfigSiDevices.ToList();
            var listePersonnalisation = new List<RapportConfigViewModel>();
            try
            {
                listePersonnalisation = GetRapportConfigViewModel(configSiDevices);
                rapportCsvSimulationViewModel.Personnalisation = listePersonnalisation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }

            configSiDevices = simulation.ConfigSiSimule.ConfigSiDevices.ToList();
            var listeSimulation = new List<RapportConfigViewModel>();
            try
            {
                listeSimulation = GetRapportConfigViewModel(configSiDevices);
                rapportCsvSimulationViewModel.Simulation = listeSimulation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }

            rapportCsvSimulationViewModel.Comparaison = GetComparaison(listeSimulation, listePersonnalisation);
            return rapportCsvSimulationViewModel;
        }

        public List<RapportConfigViewModel> GetComparaison(List<RapportConfigViewModel> simulations, List<RapportConfigViewModel> personnalisations)
        {
            var comparaisons = new List<RapportConfigViewModel>();
            try
            {
                foreach (var simulation in simulations)
                {
                    foreach (var personnalisation in personnalisations)
                    {
                        var comparaison = new RapportConfigViewModel();
                        var sameDevice = (simulation.Famille != null && simulation.Famille != null && simulation.Famille.Equals(personnalisation.Famille)) ||
                                          (simulation.Device!=null && personnalisation.Device!=null && simulation.Device.Equals(personnalisation.Device));

                        if (!sameDevice) continue;

                        if (simulation.Famille != null)
                            comparaison.Famille = simulation.Famille;

                        if (simulation.Categorie != null)
                            comparaison.Categorie = simulation.Categorie;

                        if (simulation.Device != null)
                            comparaison.Device = simulation.Device;
                        
                        comparaison.NombreTotalDevice = simulation.NombreTotalDevice - personnalisation.NombreTotalDevice;
                        comparaison.ConsoInterneAnnuelleKWH = simulation.ConsoInterneAnnuelleKWH - personnalisation.ConsoInterneAnnuelleKWH;
                        comparaison.ConsoExterneAnnuelleKWH = simulation.ConsoExterneAnnuelleKWH - personnalisation.ConsoExterneAnnuelleKWH;
                        comparaison.ConsoInterneAnnuelleCO2 = simulation.ConsoInterneAnnuelleCO2 - personnalisation.ConsoInterneAnnuelleCO2;
                        comparaison.ConsoExterneAnnuelleCO2 = simulation.ConsoExterneAnnuelleCO2 - personnalisation.ConsoExterneAnnuelleCO2;

                        comparaison.NombreTotalClasseE = simulation.NombreTotalClasseE - personnalisation.NombreTotalClasseE;
                        comparaison.ConsoAnnuelleClasseEKWH =simulation.ConsoAnnuelleClasseEKWH - personnalisation.ConsoAnnuelleClasseEKWH;
                        comparaison.ConsoAnnuelleClasseECO2 =simulation.ConsoAnnuelleClasseECO2 - personnalisation.ConsoAnnuelleClasseECO2;

                        comparaison.NombreTotalClasseM =simulation.NombreTotalClasseM - personnalisation.NombreTotalClasseM;
                        comparaison.ConsoAnnuelleClasseMKWH =simulation.ConsoAnnuelleClasseMKWH - personnalisation.ConsoAnnuelleClasseMKWH;
                        comparaison.ConsoAnnuelleClasseMCO2 =simulation.ConsoAnnuelleClasseMCO2 - personnalisation.ConsoAnnuelleClasseMCO2;

                        comparaison.NombreTotalClasseH =simulation.NombreTotalClasseH - personnalisation.NombreTotalClasseH;
                        comparaison.ConsoAnnuelleClasseHKWH =simulation.ConsoAnnuelleClasseHKWH - personnalisation.ConsoAnnuelleClasseHKWH;
                        comparaison.ConsoAnnuelleClasseHCO2 =simulation.ConsoAnnuelleClasseHCO2 - personnalisation.ConsoAnnuelleClasseHCO2;

                        comparaison.NombreClasseEMoinDunAns =simulation.NombreClasseEMoinDunAns - personnalisation.NombreClasseEMoinDunAns;
                        comparaison.NombreClasseEUnATroisAns =simulation.NombreClasseEUnATroisAns - personnalisation.NombreClasseEUnATroisAns;
                        comparaison.NombreClasseETroisACinqAns =simulation.NombreClasseETroisACinqAns - personnalisation.NombreClasseETroisACinqAns;
                        comparaison.NombreClasseEPlusCinqAns =simulation.NombreClasseEPlusCinqAns - personnalisation.NombreClasseEPlusCinqAns;

                        comparaison.NombreClasseMMoinDunAns =simulation.NombreClasseMMoinDunAns - personnalisation.NombreClasseMMoinDunAns;
                        comparaison.NombreClasseMUnATroisAns =simulation.NombreClasseMUnATroisAns - personnalisation.NombreClasseMUnATroisAns;
                        comparaison.NombreClasseMTroisACinqAns =simulation.NombreClasseMTroisACinqAns - personnalisation.NombreClasseMTroisACinqAns;
                        comparaison.NombreClasseMPlusCinqAns =simulation.NombreClasseMPlusCinqAns - personnalisation.NombreClasseMPlusCinqAns;

                        comparaison.NombreClasseHMoinDunAns =simulation.NombreClasseHMoinDunAns - personnalisation.NombreClasseHMoinDunAns;
                        comparaison.NombreClasseHUnATroisAns =simulation.NombreClasseHUnATroisAns - personnalisation.NombreClasseHUnATroisAns;
                        comparaison.NombreClasseHTroisACinqAns =simulation.NombreClasseHTroisACinqAns - personnalisation.NombreClasseHTroisACinqAns;
                        comparaison.NombreClasseHPlusCinqAns =simulation.NombreClasseHPlusCinqAns - personnalisation.NombreClasseHPlusCinqAns;

                        comparaisons.Add(comparaison);
                    }
                } 
            }
            catch (Exception e)
            {
                    Logger.Error(e.StackTrace);
                throw;
            }

            return comparaisons;
        }

        public CsvHelper.Configuration.CsvClassMap TraductionHeader(CsvHelper.Configuration.CsvClassMap map)
        {
           try
           {
               var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
               
               map.PropertyMaps[0].Name(Traduction.Famille);
               map.PropertyMaps[1].Name(Traduction.Categorie);
               map.PropertyMaps[2].Name(Traduction.Device);
               map.PropertyMaps[3].Name(Traduction.NombreTotalDevice);
               map.PropertyMaps[4].Name(Traduction.ConsoTotalAnnuelleKWH);
               map.PropertyMaps[5].Name(Traduction.ConsoExterneAnnuelleKWH);
               map.PropertyMaps[6].Name(Traduction.ConsoTotalAnnuelleCO2);
               map.PropertyMaps[7].Name(Traduction.ConsoExterneAnnuelleCO2);
               map.PropertyMaps[8].Name(Traduction.CoutTotalAnnuelleKWH);
               map.PropertyMaps[9].Name(Traduction.CoutExterneAnnuelleKWH);
               map.PropertyMaps[10].Name(Traduction.NombreTotalClasseE);
               map.PropertyMaps[11].Name(Traduction.ConsoAnnuelleClasseEKWH);
               map.PropertyMaps[12].Name(Traduction.ConsoAnnuelleClasseECO2);
               map.PropertyMaps[13].Name(Traduction.CoutAnnuelkWtClasseE);
               map.PropertyMaps[14].Name(Traduction.NombreTotalClasseM);
               map.PropertyMaps[15].Name(Traduction.ConsoAnnuelleClasseMKWH);
               map.PropertyMaps[16].Name(Traduction.ConsoAnnuelleClasseMCO2);
               map.PropertyMaps[17].Name(Traduction.CoutAnnuelkWtClasseM);
               map.PropertyMaps[18].Name(Traduction.NombreTotalClasseH);
               map.PropertyMaps[19].Name(Traduction.ConsoAnnuelleClasseHKWH);
               map.PropertyMaps[20].Name(Traduction.ConsoAnnuelleClasseHCO2);
               map.PropertyMaps[21].Name(Traduction.CoutAnnuelkWtClasseH);
               map.PropertyMaps[22].Name(Traduction.NombreClasseEMoinDunAns);
               map.PropertyMaps[23].Name(Traduction.NombreClasseEUnATroisAns);
               map.PropertyMaps[24].Name(Traduction.NombreClasseETroisACinqAns);
               map.PropertyMaps[25].Name(Traduction.NombreClasseEPlusCinqAns);
               map.PropertyMaps[26].Name(Traduction.NombreClasseMMoinDunAns);
               map.PropertyMaps[27].Name(Traduction.NombreClasseMUnATroisAns);
               map.PropertyMaps[28].Name(Traduction.NombreClasseMTroisACinqAns);
               map.PropertyMaps[29].Name(Traduction.NombreClasseMPlusCinqAns);
               map.PropertyMaps[30].Name(Traduction.NombreClasseHMoinDunAns);
               map.PropertyMaps[31].Name(Traduction.NombreClasseHUnATroisAns);
               map.PropertyMaps[32].Name(Traduction.NombreClasseHTroisACinqAns);
               map.PropertyMaps[33].Name(Traduction.NombreClasseHPlusCinqAns);

               return map;

           }
           catch (Exception e)
           {
               throw e;
           }
        }
    }
}

