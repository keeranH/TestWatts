using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business;
using Econocom.Data;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Common;
using Econocom.Model.ViewModel;
using Model.Interfaces;

namespace Econocom.Business
{
    public class EconocomManager: IEconocomService
    {
        public IUnitOfWork _unitOfWork;

        public EconocomManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PublishedContent GetPublishedContent(int pageId, int languageId)
        {
            var econocomDataManager = new EconocomDataManager(_unitOfWork);
            return econocomDataManager.GetPublishedContent(pageId, languageId);
        }

        public Client SetClient(Client client)
        {
            throw new NotImplementedException();
        }

        public Client GetClient(int clientId)
        {
            throw new NotImplementedException();
        }

        public Client UpdateClient(Client client)
        {
            throw new NotImplementedException();
        }

        public Client ValidateClient(string status, Client client)
        {
            throw new NotImplementedException();
        }

        public List<Client> GetClients()
        {
            throw new NotImplementedException();
        }

        public Contact GetContact(string loginName, string password)
        {
            throw new NotImplementedException();
        }

        public Contact GetContact(string email)
        {
            throw new NotImplementedException();
        }

        public Contact UpdateContactPassword(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact GetContact(int contactId)
        {
            throw new NotImplementedException();
        }

        public Contact SetContact(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Contact UpdateContact(Contact contact)
        {
            throw new NotImplementedException();
        }

        public List<DeviceFamily> GetFamilies(Client client)
        {
            throw new NotImplementedException();
        }

        public ClientDetail SetClientDetail(ClientDetail clientDetail)
        {
            throw new NotImplementedException();
        }
       
        public User GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string loginName, string hashPassword)
        {
            throw new NotImplementedException();
        }

        public User GetUserByEmail(string email, string loginName)
        {
            throw new NotImplementedException();
        }       

        public Contact GetContactByVerificationCode(string verificationCode)
        {
            throw new NotImplementedException();
        }

        public List<Question> GetAllQuestions()
        {
            throw new NotImplementedException();
        }

        public Contact UpdateVerificationCode(Contact contact)
        {
            throw new NotImplementedException();
        }

        public bool SendResetPasswordMail(Contact contact)
        {
            throw new NotImplementedException();
        }


        public Question GetQuestion(int id)
        {
            throw new NotImplementedException();
        }

        public ClientDetail GetClientDetails(int clientId)
        {
            throw new NotImplementedException();
        }

 		public List<Country> GetCountries()
        {
            throw new NotImplementedException();
        }

        public Client GetClientById(int clientId)
        {
            throw new NotImplementedException();
        }


        public object GetReportClientData(int clientId, string reportType)
        {
            throw new NotImplementedException();
        }



        public Client GetClientByLoginName(string loginName)
        {
            throw new NotImplementedException();
        }


        public ClientBenchmarkViewModel GetClientBenchmark(int clientId)
        {
            throw new NotImplementedException();
        }

        public ClientBenchmarkViewModel SaveClientBenchmarkDetails(ClientBenchmarkViewModel clientBenchmarkViewModel)
        {
            throw new NotImplementedException();
        }



        public string GetMessage(string lang)
        {
            throw new NotImplementedException();
        }

        public List<Language> GetLanguages()
        {
            throw new NotImplementedException();
        }

        public Language GetDefaultLanguage()
        {
            throw new NotImplementedException();
        }

        public List<PublishedContent> GetPublishedContentsForPage(string name, string languageCode)
        {
            throw new NotImplementedException();
        }
    }
}
