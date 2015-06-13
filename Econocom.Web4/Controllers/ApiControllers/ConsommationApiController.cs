using System;
using System.Collections.Generic;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Helper;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;
using NLog;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class ConsommationApiController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public CalculateurViewModel GetResultatCalculateur(int idClient, string typeUtilisateur)
        {
            try
            {
                var debut = ConsommationHelper.GetStartTime("ConsommationApiController GetResultatCalculateur");
                var businessService = new BusinessService();
                var resultat = businessService.GetResultatCalculateur(idClient, typeUtilisateur);
                ConsommationHelper.GetTimeDifference(debut, "ConsommationApiController GetResultatCalculateur");
                return resultat;

            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw ;
            }
        }

        public ConsommationViewModel GetDetailsFamille(CalculateurViewModel calculateurViewModel, int idClient, string typeUtilisateur)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetDetailsFamille(calculateurViewModel, idClient, typeUtilisateur);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConsommationViewModel GetDetailsCategorie(string idClient, string idFamille, string typeUtilisateur)
        {
            try
            {
                var businessService = new BusinessService();
                //return businessService.GetDetailsCategorieParFamille(idClient, idFamille, typeUtilisateur);
                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConsommationViewModel GetDetailsDevice(CalculateurViewModel calculateurViewModel, string idClient, string idCategorie, string typeUtilisateur)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetDetailsDevice(calculateurViewModel, idClient, idCategorie, typeUtilisateur);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ClientDeviceTypeViewModel GetTypeDevicesClient(int idCategorie, CalculateurViewModel resultatCalculateur, int idClient, string typeUtilisateur)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetTypeDevicesClient(idCategorie, resultatCalculateur, idClient, typeUtilisateur);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSiDevice SauveGarderConfigSiDevice(ConfigSiDevice configSiDevice)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.SauveGarderConfigSiDevice(configSiDevice);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSiDevice MAJConfigSiDevice(ConfigSiDevice configSiDevice)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.MAJConfigSiDevice(configSiDevice);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSiConsommation SauveGarderConfigSiConsommation(ConfigSiConsommation configSiConsommation)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.SauveGarderConfigSiConsommation(configSiConsommation);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSiConsommation MAJConfigSiConsommation(ConfigSiConsommation configSiConsommation)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.MAJConfigSiConsommation(configSiConsommation);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSiConsommation GetConfigSiConsommation(int configSiDeviceId, int classeDeviceId, int ageDeviceId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetConfigSiConsommation(configSiDeviceId, classeDeviceId, ageDeviceId);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public CategorieDevice GetCategorieDevice(int idCategorie)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetCategorieDevice(idCategorie);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<Simulation> GetSimulations(int clientId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetSimulations(clientId);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }
        
        public ConfigSiDevice GetConfigSiDevice(int configSiDeviceId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetConfigSiDevice(configSiDeviceId);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<ClasseDevice> ListeClasseDevice()
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.ListeClasseDevice();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<AgeDevice> ListeAgeDevice()
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.ListeAgeDevice();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Resultat SauvegarderResultat(Resultat resultat)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.SauvegarderResultat(resultat);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSi GetConfigSi(int id)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetConfigSi(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSi MAJConfigSi(ConfigSi configSi)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.MAJConfigSi(configSi);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public List<ConfigSiDevice> GetConfigSiDeviceParLibelle(string libelle, int configSiId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetConfigSiDeviceParLibelle(libelle, configSiId);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public CalculateurViewModel ReCalculerEtMAJConfigSiConsommation(CalculateurViewModel calculateurViewModel)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.ReCalculerEtMAJConfigSiConsommation(calculateurViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public bool MAJResultat(CalculateurViewModel calculateurViewModel)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.MAJResultat(calculateurViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public CalculateurViewModel CalculerTotalConsommation(CalculateurViewModel calculateurViewModel)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.CalculerTotalConsommation(calculateurViewModel);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSiClient GetConfigSiClient(int idClient)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetDernierConfigSiPourClient(idClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Contact getContactParId(int idContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetContactById(idContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ConfigSiClient GetConfigSiPourClientAvecStatutModifie(int idClient)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetConfigSiPourClientAvecStatutModifie(idClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public bool GenererRapportPersonnalisation(int idClient, int idContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GenererRapportPersonnalisation(idClient, idContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public CalculateurViewModel GetDetailsConfigPourClientExistant(Client client, string typeUtilisateur, int idContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetDetailsConfigPourClientExistant(null, client, typeUtilisateur, idContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Client GetClientParId(int id)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetClientParId(id);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Simulation GetSimulationEnCours(int idContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetSimulationEnCours(idContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public ResultatSimulationViewModel GetResultatSimulation(int idContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetResultatSimulation(idContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Simulation MAJSimulation(Simulation simulation)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.MAJSimulation(simulation);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Contact GetContactParEmail(string contactEmail)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetContactParEmail(contactEmail);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public CompteEvaluation GetCompteEvaluation(int idClient)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetCompteEvaluation(idClient);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public Contact GetDetailsContact(int idContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetDetailsContact(idContact);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public FamilleDevice GetFamilleParId(int idFamille)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetFamilleParId(idFamille);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public CategorieDevice GetCategorieParId(int idCategorie)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetCategorieParId(idCategorie);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }
    }
}