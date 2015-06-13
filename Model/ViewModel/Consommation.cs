using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using NLog;

namespace Econocom.Model.ViewModel
{
    public class Consommation
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Consommation(string text, string valeur, string measure)
        {
            this.Text = text;
            this.Valeur = valeur;
            this.Measure = measure;

            var culture = CultureInfo.CurrentCulture;
            NumberFormatInfo nfiConsommation = (NumberFormatInfo)culture.NumberFormat.Clone();
            nfiConsommation.NumberDecimalSeparator = ".";
            nfiConsommation.NumberDecimalDigits = 0;
            nfiConsommation.NumberGroupSeparator = " ";

            try
            {
                decimal decimalVal = Decimal.Parse(valeur, nfiConsommation);


                var ValeurAfficher = string.Format(nfiConsommation, "{0:n}", decimalVal);


                this.ValeurAfficher = ValeurAfficher;
            }
            catch(Exception e)
            {
                Logger.Error(e.StackTrace);
                this.ValeurAfficher = valeur;
            }
        }

        public string Text { get; set; }
        public string Valeur { get; set; }
        public string Measure { get; set; }
        public string ValeurAfficher{get; set;}
    }
}
