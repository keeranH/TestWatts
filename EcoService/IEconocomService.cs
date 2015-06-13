using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Common;
using Econocom.Model.Models.Service;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;

namespace Econocom.Service
{
    [ServiceContract]
    
    public interface IEconocomService
    {
        [OperationContract]
        [ReferencePreservingDataContractFormat]
        List<Person> GetPeople();

        [OperationContract]
        List<Person> GetPeopleWithOutCicularReferencing();

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        List<PAYS> ListePays();

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        List<Langue> ListeLangues();

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        List<CONSOWATTHEUR> ListeConsoWattHeur();

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        CONTACT GetContact(CONTACT contact);

        [OperationContract]
        [ReferencePreservingDataContractFormat]        
        CLIENT SetClient(CLIENT client);

        [OperationContract]
        void SaveDocument(Econocom.Model.Models.Service.DocumentUpload upload);

        [OperationContract]
        UploadResult UploadDocument(Econocom.Model.Models.Service.DocumentUpload upload);

        [OperationContract]
        UploadResult UploadFile(Econocom.Model.Models.Service.FileUpload fileUpload);

        [OperationContract]
        bool SauveGardeClient(CLIENT client);

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        List<ContenuModere> ListeContenuPublier(string nomDePage, string cultureDeLangue);

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        ContenuModere GetContenuPublier(string nomDePage);


        [OperationContract]
        bool ActualiserClient(CLIENT client,String btnStatus);

        [OperationContract]
        List<Page> ListePage();

        [OperationContract]
        ContenuModere ActualiserContenuPublier(ContenuModere publishedContent);

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        List<CLIENT> GetClientNonModerer();

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        CLIENT GetClientParId(int id);

        [OperationContract]
        List<PAYS>GetPays();

        [OperationContract]
        [ReferencePreservingDataContractFormat]
        List<Section> ListeSection();
        
        [OperationContract]
        [ReferencePreservingDataContractFormat]
        CONTACT GetContactParVerificationCode(string verificationCode);

        [OperationContract]
        List<QUESTION> GetQuestions();

        [OperationContract]
        QUESTION GetQuestionParId(int id);

        [OperationContract]
        CONTACT MAJContact(CONTACT contact);

        [OperationContract]
        CONTACT VerificationCompte(RegisterViewModel enregistrementModel);

        [OperationContract]
        bool SauvegardeSection(SectionViewModel sectionViewModel);

        [OperationContract]
        bool SauvegardePage(PageViewModel pageViewModel);

        [OperationContract]
        List<Modele> ListeModele();
    }

    //A simple DataContract serializable class

    public class Person
    {

        int age = 0;
        string name = string.Empty;
        List<Person> children = new List<Person>();
        Person parent = null;


        public int Age
        {
            get { return age; }
            set { age = value; }
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public List<Person> Children
        {
            get { return children; }
            set { children = value; }
        }


        public Person Parent
        {
            get { return parent; }
            set { parent = value; }
        }

    }
}
