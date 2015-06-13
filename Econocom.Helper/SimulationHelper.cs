using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Helper
{
    public class SimulationHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<DetailResultatSimulationViewModel> ComparerConsoSourceEtSimule(List<DetailResultatSimulationViewModel> listeDetailsSimulation)
        {
            Logger.Debug("ComparerConsoSourceEtSimule : start ");
            try
            {
                foreach (var detailSimulation in listeDetailsSimulation)
                {
                    var consoWatt = new Consommation(null, ((int)(detailSimulation.consoWattSimule - detailSimulation.consoWattSource)).ToString(), "kWh");
                    detailSimulation.ConsoWatt = consoWatt;

                    var consoCo2 = new Consommation(null, ((int)(detailSimulation.consoCo2Simule - detailSimulation.consoCo2Source)).ToString(), "teq-CO2");
                    detailSimulation.ConsoCo2 = consoCo2;
                }

                Logger.Debug("ComparerConsoSourceEtSimule : end ");
                return listeDetailsSimulation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public List<DetailResultatSimulationViewModel> CalculerPrixSimuler(List<DetailResultatSimulationViewModel> listeDetailsSimulation, decimal prixKwatt, string codeDevise)
        {
            Logger.Debug("CalculerPrixSimuler : start ");
            try
            {
                foreach (var detailSimulation in listeDetailsSimulation)
                {
                    var consoWatt = Convert.ToDecimal(detailSimulation.ConsoWatt.Valeur);
                    var prix = consoWatt * prixKwatt;
                    detailSimulation.Prix = new Consommation(null, Math.Round(prix).ToString(), codeDevise);
                }

                Logger.Debug("CalculerPrixSimuler : end ");
                return listeDetailsSimulation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public decimal CalculerTotalConsoSource(List<DetailResultatSimulationViewModel> listeDetailsSimulation)
        {
            Logger.Debug("CalculerTotalConsoSource : start ");
            try
            {
                decimal totalConsoWatt = listeDetailsSimulation.Aggregate<DetailResultatSimulationViewModel, decimal>(0, (current, detailSimulation) => current + detailSimulation.consoWattSource);
                Logger.Debug("totalConsoWatt = " + totalConsoWatt);

                Logger.Debug("CalculerTotalConsoSource : end ");
                return totalConsoWatt;
            }
            catch(Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public decimal CalculerTotalConsoWattSimule(List<DetailResultatSimulationViewModel> listeDetailsSimulation)
        {
            Logger.Debug("CalculerTotalConsoWattSimule : start ");
            try
            {
                decimal totalConsoWatt = listeDetailsSimulation.Aggregate<DetailResultatSimulationViewModel, decimal>(0, (current, detailSimulation) => current + detailSimulation.consoWattSimule);
                Logger.Debug("totalConsoWattSimule = " + totalConsoWatt);

                Logger.Debug("CalculerTotalConsoWattSimule : end ");
                return totalConsoWatt;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public decimal CalculerTotalConsoCo2Simule(List<DetailResultatSimulationViewModel> listeDetailsSimulation)
        {
            Logger.Debug("CalculerTotalConsoCo2Simule : start ");
            try
            {
                decimal totalConsoCo2 = listeDetailsSimulation.Aggregate<DetailResultatSimulationViewModel, decimal>(0, (current, detailSimulation) => current + detailSimulation.consoCo2Simule);
                Logger.Debug("totalConsoCo2Simule = " + totalConsoCo2);

                Logger.Debug("CalculerTotalConsoCo2Simule : end ");
                return totalConsoCo2;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public decimal CalculerTotalPrixSimule(List<DetailResultatSimulationViewModel> listeDetailsSimulation)
        {
            Logger.Debug("CalculerTotalPrixSimule : start ");
            try
            {
                decimal totalPrix = listeDetailsSimulation.Aggregate<DetailResultatSimulationViewModel, decimal>(0, (current, detailSimulation) => current + detailSimulation.prixSimule);
                Logger.Debug("totalPrixSimule = " + totalPrix);

                Logger.Debug("CalculerTotalPrixSimule : end ");
                return totalPrix;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public ResultatSimulationViewModel CalculerPourcentage(ResultatSimulationViewModel resultatSimulation)
        {
            Logger.Debug("CalculerPourcentage : start ");
            try
            {
                //pourcentage = (difference / totalConsoSource) * 100
                foreach (var detailSimulation in resultatSimulation.DetailsSimulation)
                {
                    var pourcent = (int)(((detailSimulation.consoWattSimule - detailSimulation.consoWattSource) / resultatSimulation.TotalConsoWattSource) * 100);
                   
                    if (pourcent < 0)
                    {
                        detailSimulation.Pourcentage = new Consommation(null, (-pourcent).ToString(), "%");
                    }
                    else
                    {
                        detailSimulation.Pourcentage = new Consommation(null, "0", "%");
                    }
                }

                Logger.Debug("CalculerPourcentage : end ");
                return resultatSimulation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        /*
        public ResultatSimulationViewModel CalculerLongeurGraphes(ResultatSimulationViewModel resultatSimulation)
        {
            Logger.Debug("CalculerLongeurGraphes : start ");
            try
            {
                var consommationHelper = new ConsommationHelper();

                foreach (var detailSimulation in resultatSimulation.DetailsSimulation)
                {
                    detailSimulation.LongueurGrapheWattSimuleTotale = consommationHelper.CalculerLongeurGrapheTotal(detailSimulation.consoWattSimule);
                    detailSimulation.LongueurGrapheWattSimuleMilieu = consommationHelper.CalculerLongeurGrapheMilieu(detailSimulation.consoWattSimule);
                    detailSimulation.LongueurGrapheWattSourceTotale = consommationHelper.CalculerLongeurGrapheTotal(detailSimulation.consoWattSource);
                    detailSimulation.LongueurGrapheWattSourceMilieu = consommationHelper.CalculerLongeurGrapheMilieu(detailSimulation.consoWattSource);

                    detailSimulation.LongueurGrapheCO2SourceTotale = consommationHelper.CalculerLongeurGrapheTotal(detailSimulation.consoCo2Simule);
                    detailSimulation.LongueurGrapheCO2SimuleMilieu = consommationHelper.CalculerLongeurGrapheMilieu(detailSimulation.consoCo2Simule);
                    detailSimulation.LongueurGrapheCO2SourceTotale = consommationHelper.CalculerLongeurGrapheTotal(detailSimulation.consoCo2Source);
                    detailSimulation.LongueurGrapheCO2SourceMilieu = consommationHelper.CalculerLongeurGrapheMilieu(detailSimulation.consoCo2Source);
                }

                Logger.Debug("CalculerLongeurGraphes : end ");
                return resultatSimulation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }*/


        public ResultatSimulationViewModel CalculerLongeurGraphes(ResultatSimulationViewModel resultatSimulation)
        {
            try
            {
                var longeurMax = 500;

                var detailSimulations = resultatSimulation.DetailsSimulation;

                if (detailSimulations != null && detailSimulations.Count > 0)
                {
                    //get max
                    var detailSimulation1 = detailSimulations.OrderByDescending(o => Convert.ToDecimal(o.consoWattSource)).First();
                    var consoMaxSource = Convert.ToDecimal(detailSimulation1.consoWattSource);

                    var detailSimulation2 = detailSimulations.OrderByDescending(o => Convert.ToDecimal(o.consoWattSimule)).First();
                    var consoMaxSimule = Convert.ToDecimal(detailSimulation2.consoWattSimule);

                    decimal consoMax = 0;
                    consoMax = consoMaxSource > consoMaxSimule ? consoMaxSource : consoMaxSimule;

                    if (consoMax > 0)
                    {
                        foreach (var detailSim in detailSimulations)
                        {
                            var longueurSource = Math.Ceiling((longeurMax / Convert.ToDecimal(consoMax)) * Convert.ToDecimal(detailSim.consoWattSource));
                            var longueurSimule = Math.Ceiling((longeurMax / Convert.ToDecimal(consoMax)) * Convert.ToDecimal(detailSim.consoWattSimule));

                            if ((int)longueurSource == 0 && Convert.ToDecimal(detailSim.consoWattSource) > 0)
                            {
                                detailSim.LongueurGrapheWattSourceMilieu = 1;
                                detailSim.LongueurGrapheCO2SourceMilieu = 1;

                                detailSim.LongueurGrapheWattSimuleMilieu = 1;
                                detailSim.LongueurGrapheCO2SimuleMilieu = 1;
                            }
                            else
                            {
                                detailSim.LongueurGrapheWattSourceTotale = (int)longueurSource;
                                detailSim.LongueurGrapheWattSourceMilieu = (int)longueurSource - 5;
                                detailSim.LongueurGrapheCO2SourceTotale = (int)longueurSource;
                                detailSim.LongueurGrapheCO2SourceMilieu = (int)longueurSource - 5;

                                detailSim.LongueurGrapheWattSimuleTotale = (int)longueurSimule;
                                detailSim.LongueurGrapheWattSimuleMilieu = (int)longueurSimule - 5;
                                detailSim.LongueurGrapheCO2SimuleTotale = (int)longueurSimule;
                                detailSim.LongueurGrapheCO2SimuleMilieu = (int)longueurSimule - 5;
                            }
                        }
                    }
                }
                return resultatSimulation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }
        

        public ResultatSimulationViewModel CalculerEquivalence(List<Equivalence> listeEquivalence, ResultatSimulationViewModel resultatSimulation)
        {
            Logger.Debug("CalculerEquivalence : start ");
            try
            {
                foreach (var detailSimulation in resultatSimulation.DetailsSimulation)
                {
                    var equivalenceQtyListe = new List<EquivalenceQtyViewModel>();

                    if (detailSimulation.Pourcentage.Valeur != "0")
                    {
                        if (listeEquivalence != null && listeEquivalence.Count > 0)
                        {
                            foreach (var equivalence in listeEquivalence)
                            {
                                var equivalenceQty = new EquivalenceQtyViewModel();
                                equivalenceQty.Id = equivalence != null ? equivalence.Id : 0;
                                equivalenceQty.LibelleEquivalence = equivalence != null ? equivalence.LibelleEquivalence : "";
                                equivalenceQty.LienImage = equivalence != null ? equivalence.LienImage : "";
                                equivalenceQty.Quantite = CalculerQty((detailSimulation.consoWattSource - detailSimulation.consoWattSimule), equivalence != null ? equivalence.Valeur : 0);

                                equivalenceQtyListe.Add(equivalenceQty);
                            }

                            
                        }
                    }
                    else
                    {
                        if (listeEquivalence != null && listeEquivalence.Count > 0)
                        {
                            foreach (var equivalence in listeEquivalence)
                            {
                                var equivalenceQty = new EquivalenceQtyViewModel();
                                equivalenceQty.Id = equivalence.Id;
                                equivalenceQty.LibelleEquivalence = equivalence.LibelleEquivalence;
                                equivalenceQty.LienImage = equivalence.LienImage;
                                equivalenceQty.Quantite = 0;

                                equivalenceQtyListe.Add(equivalenceQty);
                            }
                                                       
                        }
                    }
                    detailSimulation.Equivalences = equivalenceQtyListe;
                }

                Logger.Debug("CalculerEquivalence : end ");
                return resultatSimulation;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public int CalculerQty(decimal consommation, double equivalence)
        {
            Logger.Debug("CalculerQty : start ");
            try
            {
                var quantite = (int) Math.Ceiling((double) consommation/equivalence);
                Logger.Debug("CalculerQty : end: quantite = " + quantite);

                if (quantite < 0)
                {
                    return -quantite;
                }
                else
                {
                    return quantite;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }
    }
}
