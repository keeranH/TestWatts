using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Econocom.Helper.Maths
{
    public class MathHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Arrondir les decimals pour le nombre de devices     
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static List<decimal> ArrondirDecimal(List<decimal> liste)
        {
            try
            {
                if (!liste.Any())
                    return liste;
                Logger.Error("ArrondirDecimal Liste en entree");
                foreach (var val in liste)
                {
                    Logger.Error(val);
                }
                var listeDecimal = liste.Where(o => (o % 1 > 0));
                var totaleDecimal = (int)listeDecimal.Sum();
                ArrondirListeDecimal(liste, totaleDecimal);
                Logger.Error("ArrondirDecimal Liste en sorti");
                foreach (var val in liste)
                {
                    Logger.Error(val);
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
        public static List<decimal> ArrondirListeDecimal(List<decimal> liste, int totaleDecimal)
        {
            try
            {
                if (!liste.Any())
                    return liste;

                Logger.Error("ArrondirListeDecimal Liste en entree");
                foreach (var val in liste)
                {
                    Logger.Error(val);
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

                        Logger.Error("ArrondirListeDecimal Liste en sorti");
                        foreach (var val in liste)
                        {
                            Logger.Error(val);
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

    }
}
