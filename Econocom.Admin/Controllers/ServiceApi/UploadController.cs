using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Econocom.Business.Service;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.Service;
using NLog;

namespace Econocom.Admin.Controllers.ServiceApi
{
    [Authorize]
    public class UploadController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Task<HttpResponseMessage> PostMultipartStream()
        {
            // Check we're uploading a file
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            try
            {

                var streamProvider = new MultipartMemoryStreamProvider();
                // Read the MIME multipart content using the stream provider we just created.
                //var bodyparts =  Request.Content.ReadAsMultipartAsync(streamProvider);

                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<HttpResponseMessage>(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                        }

                        // This illustrates how to get the file names.
                        foreach (var file in streamProvider.Contents)
                        {
                            var fileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                            var headers = file.Headers;
                            var stream = file.ReadAsStreamAsync().Result;
                            //var contents = GetFileContent(stream);
                            var b = new BusinessService();
                            //var bytes = file.ReadAsByteArrayAsync();                     
                            b.SauvegardeDonneeDuFichier(stream, fileName);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK);
                    });
                return task;
            }
            catch (System.Exception e)
            {
                Logger.Error(e.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }           
        }

        public List<object> PostFile(DocumentUpload documentUpload)
        {
            try
            {
                var businessService = new BusinessService();
                var liste = businessService.SauvegardeDonneeDuFichier(documentUpload);
                return liste;
            }
            catch (System.Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
            return null;
        }

        public List<object> SauvegardeObjet(Type type, List<object> listeObjets)
        {
            try
            {
                var businessService = new BusinessService();
                var objetsEnErreur = businessService.SauvegardeListe(type, listeObjets);
                return objetsEnErreur;                
            }
            catch (System.Exception e)
            {
                Logger.Error(e.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return null;
        }    
        // GET api/upload/5
        public string Get(int id)
        {
            return "value";
        }


        public void SauvegarderDocument(Document document)
        {
            try
            {
                var businessService = new BusinessService();
                var statut = businessService.SauvegardeDocument(document);
            }
            catch (System.Exception e)
            {
                Logger.Error(e.StackTrace);
                throw;
            }
        }

        public string SauvegardeFichierPhysique(DocumentUpload doc)
        {
            try
            {
                if (doc != null)
                {
                    var fileName = Path.GetFileName(doc.DocumentName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(HttpContext.Current.Server.MapPath("~/uploads"), fileName);
                    System.IO.FileStream file = System.IO.File.Create(path);                                       
                    file.Write(doc.Data, 0, doc.Data.Length);
                    file.Flush();
                    file.Close();

                    return path;
                    //File.WriteAllBytes(path, doc.Data);
                }
            }
            catch (Exception)
            {
               
                throw;
            }
            return null;
        }


        public bool SupprimerFichierPhysique(string fileWithPath)
        {
            try
            {
                var filePath = HttpContext.Current.Server.MapPath(fileWithPath);
                File.Delete(filePath);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
            return false;
        }

        public bool UploadDoc(DocumentUpload document, string typeClient)
        {
            try
            {
                var businessService = new BusinessService();
                var importViewModel = businessService.UploadDoc(document, typeClient);
                return importViewModel.ImportParcValide;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }
    }
}
