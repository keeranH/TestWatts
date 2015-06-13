using System;
using System.Data;
using Econocom.Model.Models.Benchmark;
using System.Collections.Generic;

namespace Econocom.Calculateur
{
    public static class CalculatorData
    {
        public static double ratioEquipeCollabo = 60;
        public static double effectif = 1500;
        public static double prixKWatt = 0.097;
        public static double tauxUtilisationExterne = 0.3;
        public static double emissionCarbonParKWatt = 60;
        public static double coeffReductionWattExterne = 1;
        public static List<ClasseDevice> ListClasseDevice = new List<ClasseDevice>();
        public static List<AgeDevice> ListAgeDevice = new List<AgeDevice>();
        public static List<TypeUsage> ListTypeUsage = new List<TypeUsage>();
        public static List<VentilationClasseDevice> ListVentilationClasseDevice = new List<VentilationClasseDevice>();
        public static List<VentilationClasseAgeDevice> ListVentilationClasseAgeDevice = new List<VentilationClasseAgeDevice>();
        public static List<Usage> ListUsage = new List<Usage>();

        public static List<ClasseDevice> GetClasseDeviceList()
        {
            ClasseDevice classDevice = new ClasseDevice();
            classDevice.LibelleClasseDevice = "E";
            ListClasseDevice.Add(classDevice);

            classDevice = new ClasseDevice();
            classDevice.LibelleClasseDevice = "M";
            ListClasseDevice.Add(classDevice);

            classDevice = new ClasseDevice();
            classDevice.LibelleClasseDevice = "H";
            ListClasseDevice.Add(classDevice);

            return ListClasseDevice;
        }

        public static List<AgeDevice> GetAgeDeviceList()
        {
            AgeDevice ageDevice = new AgeDevice();
            ageDevice.LibelleAgeDevice = "MoinsUnAn";
            ListAgeDevice.Add(ageDevice);

            ageDevice = new AgeDevice();
            ageDevice.LibelleAgeDevice = "UnATrois";
            ListAgeDevice.Add(ageDevice);

            ageDevice = new AgeDevice();
            ageDevice.LibelleAgeDevice = "TroisACinq";
            ListAgeDevice.Add(ageDevice);

            ageDevice = new AgeDevice();
            ageDevice.LibelleAgeDevice = "PlusCinq";
            ListAgeDevice.Add(ageDevice);
            
            return ListAgeDevice;
        }

        public static List<TypeUsage> GetTypeUsageList()
        {
            TypeUsage typeUsage = new TypeUsage();
            typeUsage.LibelleTypeUsage = "Intensif";
            ListTypeUsage.Add(typeUsage);

            typeUsage = new TypeUsage();
            typeUsage.LibelleTypeUsage = "Non Intensif";
            ListTypeUsage.Add(typeUsage);

            typeUsage = new TypeUsage();
            typeUsage.LibelleTypeUsage = "Mode Off";
            ListTypeUsage.Add(typeUsage);

            return ListTypeUsage;
        }
        
        public static VentilationClasseDevice GetVentilationClasseDevice(int classeDeviceId)
        {
            VentilationClasseDevice ventilationClasseDevice = new VentilationClasseDevice();
            ventilationClasseDevice.ClasseDeviceId = classeDeviceId;

            if (classeDeviceId == 1) //E
            {
                ventilationClasseDevice.Coefficient = 30;
            }
            else if (classeDeviceId == 2) //M
            {
                ventilationClasseDevice.Coefficient = 50;
            }
            else //H
            {
                ventilationClasseDevice.Coefficient = 20;
            }
            
            return ventilationClasseDevice;
        }

        public static VentilationClasseAgeDevice GetVentilationClasseAgeDevice(int classeDeviceId, int ageDeviceId)
        {
            VentilationClasseAgeDevice ventilationClasseAgeDevice = new VentilationClasseAgeDevice();
            ventilationClasseAgeDevice.ClasseDeviceId = classeDeviceId;
            ventilationClasseAgeDevice.AgeDeviceId = ageDeviceId; 

            if (classeDeviceId == ClasseDeviceStruct.EntreeDeGamme) // E
            {
                if (ageDeviceId == AgeDeviceStruct.MoinsUnAn)
                {
                    //E Moins 1 an
                    ventilationClasseAgeDevice.Coefficient = 50;
                }
                else if (ageDeviceId == AgeDeviceStruct.UnATrois)
                {
                    //E 1 - 3 ans
                    ventilationClasseAgeDevice.Coefficient = 20;
                }
                else if (ageDeviceId == AgeDeviceStruct.TroisACinq)
                {
                    //E 3 - 5 ans
                    ventilationClasseAgeDevice.Coefficient = 15;
                }
                else
                {
                    //E > 5 ans
                    ventilationClasseAgeDevice.Coefficient = 15;
                }
            }
            else if (classeDeviceId == ClasseDeviceStruct.MilieuDeGamme) // M
            {
                if (ageDeviceId == AgeDeviceStruct.MoinsUnAn)
                {
                    //M Moins 1 an
                    ventilationClasseAgeDevice.Coefficient = 50;
                }
                else if (ageDeviceId == AgeDeviceStruct.UnATrois)
                {
                    //M 1 - 3 ans
                    ventilationClasseAgeDevice.Coefficient = 20;
                }
                else if (ageDeviceId == AgeDeviceStruct.TroisACinq)
                {
                    //M 3 - 5 ans
                    ventilationClasseAgeDevice.Coefficient = 15;
                }
                else
                {
                    //M > 5 ans
                    ventilationClasseAgeDevice.Coefficient = 15;
                }
            }
            else
            {
                if (ageDeviceId == AgeDeviceStruct.MoinsUnAn)
                {
                    //H Moins 1 an
                    ventilationClasseAgeDevice.Coefficient = 40;
                }
                else if (ageDeviceId == AgeDeviceStruct.UnATrois)
                {
                    //H 1 - 3 ans
                    ventilationClasseAgeDevice.Coefficient = 45;
                }
                else if (ageDeviceId == AgeDeviceStruct.TroisACinq)
                {
                    //H 3 - 5 ans
                    ventilationClasseAgeDevice.Coefficient = 0;
                }
                else
                {
                    //H > 5 ans
                    ventilationClasseAgeDevice.Coefficient = 15;
                }
            }
            return ventilationClasseAgeDevice;
        }

        public static Usage GetUsage(int classeDeviceId)
        {
            Usage usage = new Usage();
            //usage.ClasseDeviceId = classeDeviceId;
            usage.NbrJourTravaille = 240;
            //usage.NbrHeuresIntensifJour = Convert.ToDecimal(3.5);
            //usage.NbrHeuresOffJour = Convert.ToDecimal(16.5);

            //non-intensif = 4hrs
            if (classeDeviceId == 1)
            {   
                // E
                usage.CoeffNonIntensif = Convert.ToDecimal(0.55);
                usage.CoeffModeOff = Convert.ToDecimal(0.02);
            }
            else if (classeDeviceId == 2)
            {
                // M
                usage.CoeffNonIntensif = Convert.ToDecimal(0.55);
                usage.CoeffModeOff = Convert.ToDecimal(0.02);
            }
            else
            {
                // H
                usage.CoeffNonIntensif = Convert.ToDecimal(0.55);
                usage.CoeffModeOff = Convert.ToDecimal(0.02);
            }

            return usage;
        }

        

        public static decimal GetConsoWatt(int classeDeviceId, int ageDeviceId)
        {
            decimal consoWatt;
            if (classeDeviceId == ClasseDeviceStruct.EntreeDeGamme)
            {
                if (ageDeviceId == AgeDeviceStruct.MoinsUnAn)
                    consoWatt = ConsoWattE.moins1an;
                else if (ageDeviceId == AgeDeviceStruct.UnATrois)
                    consoWatt = ConsoWattE.unATroisAns;
                else if (ageDeviceId == AgeDeviceStruct.TroisACinq)
                    consoWatt = ConsoWattE.troisACinqAns;
                else
                    consoWatt = ConsoWattE.plusDeCinqAns;
            }
            else if (classeDeviceId == ClasseDeviceStruct.MilieuDeGamme)
            {
                if (ageDeviceId == AgeDeviceStruct.MoinsUnAn)
                    consoWatt = ConsoWattM.moins1an;
                else if (ageDeviceId == AgeDeviceStruct.UnATrois)
                    consoWatt = ConsoWattM.unATroisAns;
                else if (ageDeviceId == AgeDeviceStruct.TroisACinq)
                    consoWatt = ConsoWattM.troisACinqAns;
                else
                    consoWatt = ConsoWattM.plusDeCinqAns;
            }
            else
            {
                if (ageDeviceId == AgeDeviceStruct.MoinsUnAn)
                    consoWatt = ConsoWattH.moins1an;
                else if (ageDeviceId == AgeDeviceStruct.UnATrois)
                    consoWatt = ConsoWattH.unATroisAns;
                else if (ageDeviceId == AgeDeviceStruct.TroisACinq)
                    consoWatt = ConsoWattH.troisACinqAns;
                else
                    consoWatt = ConsoWattH.plusDeCinqAns;
            }
            return consoWatt;
        }

        public struct ClasseDeviceStruct
        {
            public static int EntreeDeGamme = 1;
            public static int MilieuDeGamme = 2;
            public static int HautDeGamme = 3;
        }

        public struct AgeDeviceStruct
        {
            public static int MoinsUnAn = 1;
            public static int UnATrois = 2;
            public static int TroisACinq = 3;
            public static int PlusCinqAns = 4;
        }

        public struct UsageStruct
        {
            public static int Intensif = 1;
            public static int NonIntensif = 2;
            public static int ModeOff = 3;
        }

        public struct ConsoWattE
        {
            public static int moins1an = 41;
            public static int unATroisAns = 46;
            public static int troisACinqAns = 51;
            public static int plusDeCinqAns = 56;
        }

        public struct ConsoWattM
        {
            public static int moins1an = 60;
            public static int unATroisAns = 65;
            public static int troisACinqAns = 70;
            public static int plusDeCinqAns = 75;
        }

        public struct ConsoWattH
        {
            public static int moins1an = 90;
            public static int unATroisAns = 95;
            public static int troisACinqAns = 100;
            public static int plusDeCinqAns = 105;
        }
        
    }
}
