using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.ViewModel;

namespace Econocom.Web4.Controllers.ApiControllers
{
    public class ContactApiController : ApiController
    {
        public Contact GetContactParVerificationCode(string verificationCode)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.GetContactParVerificationCode(verificationCode);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public Contact VerificationCompte(RegisterViewModel enregistrementModel)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.VerificationCompte(enregistrementModel);
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


        public Contact SauvegardeContacte(Contact contact)
        {
            try
            {
                var businessService = new BusinessService();
                return businessService.SauvegardeContacte(contact);
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
    }
}
