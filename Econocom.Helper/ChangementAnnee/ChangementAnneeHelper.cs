using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Helper.Maths;
using Econocom.Helper.Rapport;
using Econocom.Model.Models.Benchmark;
using NCalc;
using NLog;

namespace Econocom.Helper.ChangementAnnee
{
    public class ChangementAnneeHelper
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ConfigSi RecalculerQuantite(ConfigSi configSi, List<ChangementAnneeCalendaire> changementAnneeCalendaires)
        {
            if (configSi == null || changementAnneeCalendaires == null)
                return configSi;
            configSi.DateModification = DateTime.Now;
            try
            {
                if (configSi.ConfigSiDevices != null)
                {
                    foreach (var configSiDevice in configSi.ConfigSiDevices)
                    {
                        RecalculerQuantiteConfigSiDevice(configSiDevice, changementAnneeCalendaires);
                    }
                }               
            }
            catch (Exception e)
            {
                Logger.Error(" configSi {0} ", configSi.Id);
                LogguerErreur(e);
                throw;
            }
            return configSi;
        }

        public ConfigSiDevice RecalculerQuantiteConfigSiDevice(ConfigSiDevice configSiDevice, List<ChangementAnneeCalendaire> changementAnneeCalendaires)
        {
            if (configSiDevice == null || configSiDevice.ConfigSiConsommations == null || changementAnneeCalendaires == null)
                return configSiDevice;

            try
            {
                configSiDevice.ConfigSiConsommations = RecalculerQuantiteAgeDevice(configSiDevice.ConfigSiConsommations, changementAnneeCalendaires);
                configSiDevice.Quantite = configSiDevice.ConfigSiConsommations.Sum(o => o.Quantite);
            }
            catch (Exception e)
            {
                Logger.Error(" configSiDevice {0} ", configSiDevice.Id);
                LogguerErreur(e);
                throw;
            }
            return configSiDevice;
        }

        public List<ConfigSiConsommation> RecalculerQuantiteAgeDevice(List<ConfigSiConsommation> consommations,List<ChangementAnneeCalendaire> changementAnneeCalendaires)
        {
            try
            {
                if (consommations == null || consommations.Count == 0)
                    return consommations;

                consommations = RecalculerQuantiteAgeDeviceParClasse(RapportHelper.ClasseDevice.EntreeDeGamme, consommations, changementAnneeCalendaires);
                consommations = RecalculerQuantiteAgeDeviceParClasse(RapportHelper.ClasseDevice.MoyenDeGamme, consommations, changementAnneeCalendaires);
                consommations = RecalculerQuantiteAgeDeviceParClasse(RapportHelper.ClasseDevice.HautDeGamme, consommations, changementAnneeCalendaires);
                
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return consommations;
        }

        public List<ConfigSiConsommation> RecalculerQuantiteAgeDeviceParClasse(RapportHelper.ClasseDevice classeDevice,List<ConfigSiConsommation> consommations,List<ChangementAnneeCalendaire> changementAnneeCalendaires)
        {
            try
            {

                var listeQuantite = new List<decimal>(capacity: consommations.Count);

                // recuperer les variables et formules pour la recalculation
                var listeQuery = consommations.Where(o => o.ClasseDeviceId == (int) classeDevice);
                var configSiConsommations = listeQuery as IList<ConfigSiConsommation> ?? listeQuery.ToList();
                var variables = GetVariables(configSiConsommations.ToList(), changementAnneeCalendaires);


                //calculer les nouvelles quantites
                foreach (var configSiConsommation in consommations)
                {
                    foreach (var changementAnneeCalendaire in changementAnneeCalendaires)
                    {
                        if ((changementAnneeCalendaire.AgeDeviceId == configSiConsommation.AgeDeviceId) &&
                            (configSiConsommation.ClasseDeviceId == (int) classeDevice))
                        {
                            Logger.Info("changementAnneeCalendaire.Expression {0}, ",
                                        changementAnneeCalendaire.Expression);
                            string expression = RemplaceVariableExpression(variables,
                                                                           changementAnneeCalendaire.Expression);
                            try
                            {
                                var e = new Expression(expression);
                                Logger.Info("expression {0}, ", expression);
                                var objet = e.Evaluate();
                                decimal nouvelleQuantite = 0;
                                Decimal.TryParse(objet.ToString(), out nouvelleQuantite);
                                Logger.Info("nouvelleQuantite {0}, ", nouvelleQuantite);
                                listeQuantite.Add(nouvelleQuantite);
                            }
                            catch (Exception e)
                            {
                                Logger.Error("Erreur parse expression {0}, ", expression);
                                LogguerErreur(e);    
                            }
                        }
                    }
                }

                //arrondir les nouvelles quantites
                try
                {
                    listeQuantite = MathHelper.ArrondirDecimal(listeQuantite);
                }
                catch (Exception e)
                {
                    Logger.Error("Erreur arrondir liste");
                    LogguerErreur(e);    
                }

                int index = 0;
                foreach (var consommation in consommations)
                {
                    foreach (var configSiConsommation in configSiConsommations)
                    {
                        if (consommation.Id == configSiConsommation.Id)
                        {
                            Logger.Info("avant: consommation.Quantite {0}", consommation.Quantite);
                            consommation.Quantite = (int) listeQuantite[index];
                            Logger.Info("apres: consommation.Quantite {0}", consommation.Quantite);
                            ++index;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return consommations;
        }

        private Dictionary<string, decimal> GetVariables(List<ConfigSiConsommation> consommations, List<ChangementAnneeCalendaire> changementAnneeCalendaires)
        {
            var variables = new Dictionary<string, decimal>();
            try
            {
                foreach (var configSiConsommation in consommations)
                {
                    foreach (var changementAnneeCalendaire in changementAnneeCalendaires)
                    {
                        if (changementAnneeCalendaire.AgeDeviceId == configSiConsommation.AgeDeviceId)
                        {
                            if (!variables.ContainsKey(changementAnneeCalendaire.Variable))
                                variables.Add(changementAnneeCalendaire.Variable, configSiConsommation.Quantite);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogguerErreur(e);
                throw;
            }
            return variables;
        }

        public string RemplaceVariableExpression(Dictionary<string, decimal> variables, string expression)
        {
            try
            {
                expression = variables.Aggregate(expression, (current, remplacement) => current.Replace(remplacement.Key, remplacement.Value.ToString()));
            }
            catch (Exception e)
            {
                Logger.Error("expression {0}", expression);
                LogguerErreur(e);
                throw;
            }
            
            return expression;
        }

        private void LogguerErreur(Exception e)
        {
            try
            {
                if (e == null)
                {
                    Logger.Error("Exception null");
                }
                else
                {
                    if (e.InnerException != null)
                    {
                        if (e.InnerException.InnerException != null)
                        {
                            Logger.Error(e.InnerException.InnerException.Message);
                        }
                        else
                        {
                            Logger.Error(e.InnerException.Message);
                        }
                    }
                    else
                    {
                        Logger.Error(e.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
        }
    }
}
