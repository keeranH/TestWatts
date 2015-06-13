using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Econocom.Business.Service;
using Econocom.Model.Interfaces;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Common;
using Econocom.Model.Models.Service;
using Econocom.Model.ViewModel;
using Econocom.Model.ViewModel.CMS;

namespace Econocom.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EconocomService" in code, svc and config file together.
    public class EconocomService : IEconocomService
    {
        private BusinessService service;

        public EconocomService()
        {
            service = new BusinessService();
        }

        public EconocomService(BusinessService businessService)
        {
            this.service = businessService;
        }


        public List<Person> GetPeople()
        {
            Person father = new Person { Age = 42, Name = "brad", Parent = null };
            father.Children = CreateChildren(father);
            Person mum = new Person { Age = 40, Name = "pam", Parent = null };
            mum.Children = CreateChildren(mum);

            List<Person> people = new List<Person>();
            people.Add(father);
            people.Add(mum);
            return people;
        }



        public Boolean SauveGardeClient(CLIENT client)
        {
            try
            {
                if (client != null)
                {
                    
                    return service.SauvegardeClient(client);

                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                throw;
            }

            return false;
        }


        public List<Person> GetPeopleWithOutCicularReferencing()
        {
            Person father =
                 new Person { Age = 42, Name = "brad", Parent = null };
            father.Children = CreateChildren(father);
            Person mum =
                new Person { Age = 40, Name = "pam", Parent = null };
            mum.Children = CreateChildren(mum);

            List<Person> people = new List<Person>();
            people.Add(father);
            people.Add(mum);
            return people;
        }

        public List<PAYS> ListePays()
        {
            return service.ListePays();
        }

        public List<Langue> ListeLangues()
        {
            return service.ListeLangues();
        }

        public List<CONSOWATTHEUR> ListeConsoWattHeur()
        {
            return service.ListeConsoWattHeur();
        }

        public CONTACT GetContact(CONTACT contact)
        {
           try
           {
               if (contact.EMAIL != null && contact.MOTPASSE != null)
               {
                  var contactExistant= service.GetContact(contact.EMAIL, contact.MOTPASSE);
                  return contactExistant;
               }
               else
               {
                   return null;
               }

           }
           catch (Exception e)
           {
               return null;
           }
        }

        public CLIENT SetClient(CLIENT client)
        {
            try
            {
                if (client == null)
                {
                    throw new FaultException("SetClientErreur", new FaultCode("Objet client vide"));
                }
                else
                {
                    if (client.ADRESSE == null)
                        throw new FaultException("SetClientErreur", new FaultCode("Adresse vide"));
                    if (client.IDENTITEBANCAIRE == null)
                        throw new FaultException("SetClientErreur", new FaultCode("Identite bancaire vide"));
                    if (client.CONTACTs == null || client.CONTACTs.Count == 0)
                        throw new FaultException("SetClientErreur", new FaultCode("Contacts vide"));

                    return service.SetClient(client);
                }
            }
            catch (FaultException f)
            {
                 throw;
            }
            catch (Exception e)
            {                             
                throw new FaultException("SetClientErreur", new FaultCode("ErreurSauvegardeClient"));
            }
        }

        private List<Person> CreateChildren(Person parent)
        {
            List<Person> children = new List<Person>();
            children.Add(new Person
            {
                Age = 15,
                Name = "sam",
                Parent = parent,
                Children = null
            });
            children.Add(new Person
            {
                Age = 20,
                Name = "greta",
                Parent = parent,
                Children = null
            });
            return children;
        }

        public void SaveDocument(Econocom.Model.Models.Service.DocumentUpload upload)
        {
            try
            {
                if (upload == null)
                {
                    throw new FaultException("document null");
                }
                else
                {
                    if (upload.data == null)
                        throw new FaultException("document data null");
                    if (upload.documentName == null)
                        throw new FaultException("document name null");

                    FileStream targetStream = null;
                    var sourceStream = upload.data;
                    var doc = new DocumentUpload {data = upload.data, documentName = upload.documentName};
                    service.SaveDocument(doc);
                }
            }
            catch (Exception e)
            {
                var code = new FaultCode(e.Message);
                throw new FaultException("Error parsing document", code);
            }

        }

        public Econocom.Model.Models.Service.UploadResult UploadDocument(Econocom.Model.Models.Service.DocumentUpload upload)
        {
            FileStream targetStream = null;
            byte[] sourceStream = upload.data;
            //List<string> st = GetFileContent(upload.InputStream); //if reading from stream
            List<string> st = GetFileContentFromByte(upload.data);
            string uploadFolder = @"C:\work\temp\";
            string filename = upload.documentName;
            string filePath = Path.Combine(uploadFolder, filename);

            string s = Encoding.UTF8.GetString(upload.data);
            var doc = new Econocom.Model.Models.Service.DocumentUpload();
            doc.data = upload.data;
            doc.documentName = upload.documentName;
            service.SaveDocument(doc);

            string c = "dasdas";

            var result = new Econocom.Model.Models.Service.UploadResult();
            result.Status = true;
            result.Error = "could not update some of the data";
            List<string> errors = new List<string>();
            errors.Add("1,2,3,4");
            errors.Add("1,2,3,5");
            result.ErrorLines = errors;

            System.Diagnostics.Debug.WriteLine(String.Format("Upload for Document {0} was {1} bytes", upload.documentName, sourceStream.Length));
            return result;
        }

        public Econocom.Model.Models.Service.UploadResult UploadFile(Econocom.Model.Models.Service.FileUpload fileUpload)
        {
            FileStream targetStream = null;
            // byte[] sourceStream = fileUpload.Data;
            List<string> st = GetFileContent(fileUpload.Data); //if reading from stream

            //service.UploadFile(fileUpload);

            string c = "dasdas";

            var result = new Econocom.Model.Models.Service.UploadResult();
            result.Status = true;
            result.Error = "could not update some of the data";
            List<string> errors = new List<string>();
            errors.Add("1,2,3,4");
            errors.Add("1,2,3,5");
            result.ErrorLines = errors;

            System.Diagnostics.Debug.WriteLine(String.Format("Upload for Document {0} was {1} bytes", fileUpload.FileName));
            return result;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static List<string> GetFileContentFromByte(byte[] content)
        {
            Encoding[] encodings = new Encoding[]
                {
                    Encoding.GetEncoding("UTF-8", new EncoderExceptionFallback(), new DecoderExceptionFallback()),
                    Encoding.GetEncoding(1250, new EncoderExceptionFallback(), new DecoderExceptionFallback())
                };


            var result = new List<string>();
            foreach (Encoding enc in encodings)
            {
                try
                {
                    result.Add(enc.GetString(content));
                    break;
                }
                catch (DecoderFallbackException e) { }
            }
            return result;
        }

        private static List<string> GetFileContent(Stream input)
        {
            Encoding[] encodings = new Encoding[]
                {
                    Encoding.GetEncoding("UTF-8", new EncoderExceptionFallback(), new DecoderExceptionFallback()),
                    Encoding.GetEncoding(1250, new EncoderExceptionFallback(), new DecoderExceptionFallback())
                };

            var inputArr = ReadFully(input);
            var result = new List<string>();
            foreach (Encoding enc in encodings)
            {
                try
                {
                    result.Add(enc.GetString(inputArr));
                    break;
                }
                catch (DecoderFallbackException e) { }
            }
            return result;
        }

        public List<ContenuModere> ListeContenuPublier(string nomDePage, string cultureDeLangue)
        {
            return service.ListeContenuPublier(nomDePage, cultureDeLangue);
        }

        public ContenuModere GetContenuPublier(string nomDePage)
        {
            return service.GetContenuPublier(nomDePage);
        }




        public bool ActualiserClient(CLIENT client,String btnStatus)
        {
            try
            {
                if (client != null)
                {
                    if (btnStatus.Equals("Accepté"))
                         return service.ActualiserClient(client);
                    else
                        return service.ActualiserClient(client);//need to confirm what to do...no status field on client
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return false;
        }
    


        public List<Page> ListePage()
        {
            return service.ListePage();
        }

        public ContenuModere ActualiserContenuPublier(ContenuModere publishedContent)
        {
            return service.ActualiserContenuPublier(publishedContent);
        }

        public List<CLIENT> GetClientNonModerer()
        {
            return service.ClientsNonModerer();
        }


        public CLIENT GetClientParId(int id)
        {
            return service.GetClientParId(id);
        }


        public List<PAYS> GetPays()
        {
            return service.GetPays();
        }

        public List<Section> ListeSection()
        {
            return service.ListeSection();
        }


        public CONTACT GetContactParVerificationCode(string verificationCode)
        {
            return service.GetContactParVerificationCode(verificationCode);
        }


        public List<QUESTION> GetQuestions()
        {
            return service.GetQuestions();
        }


        public QUESTION GetQuestionParId(int id)
        {
            return service.GetQuestionParId(id);
        }



        public CONTACT MAJContact(CONTACT contact)
        {
            return service.MAJContact(contact);
        }


        public CONTACT VerificationCompte(RegisterViewModel enregistrementModel)
        {
            return service.VerificationCompte(enregistrementModel);
        }

        public bool SauvegardeSection(SectionViewModel sectionViewModel)
        {
            return service.SauvegardeSection(sectionViewModel);
        }

        public bool SauvegardePage(PageViewModel pageViewModel)
        {
            return service.SauvegardePage(pageViewModel);
        }

        public List<Modele> ListeModele()
        {
            return service.ListeModele();
        }
    }

}
