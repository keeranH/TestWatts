using System;
using System.Collections.Generic;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;
using NLog;

namespace Econocom.Admin.Controllers.GestionClient
{
    public class ModerationApiController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private BusinessService service;
        
        public List<Client> GetClientNonModerer(int? page, int pageDimension, out int totalPages)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.ClientsNonModerer(page, pageDimension, out totalPages);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                totalPages = 0;
                return null;
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
                throw e;
            }

        }

        public bool ActualiserClient(Client client)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.ActualiserClient(client);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public Adresse GetAdresseById(int Id)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetAdresseById(Id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Pays getPaysById(int paysId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetPaysById(paysId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Contact GetContactByTypeAndClient(int typeId, int clientId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetContactByTypeAndClient(typeId, clientId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TypeContact GetTypeContactByString(String typeContact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetTypeContactByString(typeContact);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Contact> GetUtilisateursByContactAndTypeContact(int clientId, int typeContactId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetUtilisateursByClientIdAndTypeContactId(clientId, typeContactId);
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Client GetClientById(int clientId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetClientById(clientId);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Pays> ListePays()
        {
            try{
                var businessService = new BusinessService();
                return businessService.GetPays();
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public Adresse updateAdress(Adresse adresse)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.updateAdress(adresse);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public DetailsClient saveDetailsClient(DetailsClient detailsClient)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.saveDetailsClient(detailsClient);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public Client updateClient(Client client)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.updateClient(client);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public Contact updateContacte(Contact contact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.updateContacte(contact);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public Contact GetContactParEmail(string email)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetContactParEmail(email);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public TypeMail GetTypeMail(string libelleTypeMail)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetTypeMail(libelleTypeMail);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public ContenuMail GetContenuMail(int idTypeMail, int idLangue)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetContenuMail(idTypeMail, idLangue);
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public List<String> GetListeEmails()
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetListeEmails();
            }

            catch (Exception e)
            {
                throw e;
            }
        }

        public List<string> GetListeEmailPasValide(int clientId)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetListeEmailPasValide(clientId);
            }

            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
