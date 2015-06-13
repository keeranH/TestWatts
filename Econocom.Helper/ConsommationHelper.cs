using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Helper
{
    public class ConsommationHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public int CalculerLongeurGrapheTotal(decimal? consommation)
        {
            try
            {
                var scale = 0.005;
                var longeur = (consommation*(Convert.ToDecimal(scale)));
                return (int) longeur;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }


        public int CalculerLongeurGrapheMilieu(decimal? consommation)
        {
            try
            {
                var bottom = 4;
                var top = 5;
                var scale = 0.005;
                var longeur = (consommation*(Convert.ToDecimal(scale))) - (bottom + top);
                return (int) longeur;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public decimal CalculerTotalQuantite(List<AgeDeviceViewModel> ageDevices)
        {
            try
            {
                var total = 0;
                if (ageDevices != null)
                {
                    foreach (var ageDevice in ageDevices)
                    {
                        total = total + ageDevice.Quantite;
                    }
                }
                return total;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public List<DetailConsommationViewModel> CalculerLongeurGraphe(List<DetailConsommationViewModel> detailConsommations)
        {
            try
            {
                var longeurMax = 500;

                if (detailConsommations != null && detailConsommations.Count > 0)
                {
                    //get max
                    var detailConsommation = detailConsommations.OrderByDescending(o => Convert.ToDecimal(o.ConsoWatt.Valeur)).First();
                    var consoMax = Convert.ToDecimal(detailConsommation.ConsoWatt.Valeur);

                    if (consoMax > 0)
                    {
                        foreach (var detailConso in detailConsommations)
                        {
                            var longeur = Math.Ceiling((longeurMax/ Convert.ToDecimal(consoMax)) * Convert.ToDecimal(detailConso.ConsoWatt.Valeur));

                            if ((int)longeur == 0 && Convert.ToDecimal(detailConso.ConsoWatt.Valeur) > 0)
                            {
                                detailConso.LongueurGrapheWattMilieu = 1;
                                detailConso.LongueurGrapheCO2Milieu = 1;
                            }
                            else
                            {
                                detailConso.LongueurGrapheWattTotale = (int)longeur;
                                detailConso.LongueurGrapheWattMilieu = (int) longeur - 9;
                                detailConso.LongueurGrapheCO2Totale = (int)longeur;
                                detailConso.LongueurGrapheCO2Milieu = (int)longeur - 9;
                            }
                        }
                    }
                }
                return detailConsommations;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public static string FormatterValeur(decimal value)
        {
            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberDecimalDigits = 0;
                nfi.NumberGroupSeparator = " ";
                var valeurFormatter = string.Format(nfi, "{0:n}", value);
                Logger.Debug("FormatterValeur {0}", valeurFormatter);
                return valeurFormatter;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return value.ToString();
        }

        public static string FormatterValeurTotal(decimal value)
        {
            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                nfi.NumberDecimalDigits = 0;
                nfi.NumberGroupSeparator = " ";
                var valeurFormatter = string.Format(nfi, "{0:n}", value);
                Logger.Debug("FormatterValeur {0}", valeurFormatter);
                return valeurFormatter;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }
            return value.ToString();
        }

        public List<DetailConsommationViewModel> CalculerLargeurGraphe(List<DetailConsommationViewModel> detailConsommations)
        {
            try
            {
                var largeurMax = 500;

                if (detailConsommations != null && detailConsommations.Count > 0)
                {
                    //get max
                    var detailConsommation = detailConsommations.OrderByDescending(o => Convert.ToDecimal(o.ConsoWatt.Valeur)).First();
                    var consoMax = Convert.ToDecimal(detailConsommation.ConsoWatt.Valeur);

                    if (consoMax > 0)
                    {
                        foreach (var detailConso in detailConsommations)
                        {
                            var largeur = (largeurMax/Convert.ToDecimal(consoMax)) * Convert.ToDecimal(detailConso.ConsoWatt.Valeur);

                            if ((int) largeur == 0 && Convert.ToDecimal(detailConso.ConsoWatt.Valeur) > 0)
                            {
                                detailConso.LongueurGrapheWattMilieu = 1;
                                detailConso.LongueurGrapheCO2Milieu = 1;
                            }
                            else
                            {
                                detailConso.LongueurGrapheWattMilieu = (int) largeur;
                                detailConso.LongueurGrapheCO2Milieu = (int) largeur;
                            }
                        }
                    }
                }
                return detailConsommations;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw e;
            }
        }

        public static DateTime GetCurrentTime()
        {
            var time = DateTime.Now;            
            return time;
        }

        public static DateTime GetStartTime(string text)
        {
            var start = GetCurrentTime();
            Logger.Debug("{0} Started {1} ", text, start.ToString());
            return start;
        }

        public static DateTime GetEndTime(string text)
        {
            var end = GetCurrentTime();
            Logger.Debug("{0} End {1} ", text, end.ToString());
            return end;
        }

        public static int GetTimeDifference(DateTime start, string text)
        {
            var end = GetCurrentTime();
            Logger.Debug("{0} End {1} ",text, end.ToString());

            var difference = end - start;            
            Logger.Debug("difference {0} {1}",text, difference.ToString());
            return difference.Seconds;
        }

        public static string FormatterLibelleDevice(string libelle, int length)
        {
            try
            {
                if (string.IsNullOrEmpty(libelle))
                {
                    return libelle;
                }

                if (libelle.Length > length)
                {
                    return libelle.Substring(0, length) + " ...";
                }

                else
                {
                    return libelle;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                return libelle;
            }
        }
    }
}
