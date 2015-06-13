using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Econocom.Model.Models.Service;
using Econocom.Web4.Controllers.ApiControllers;

namespace Econocom.Web4.Controllers
{
    public class FileUploadController : Controller
    {
        //
        // GET: /FileUpload/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var uploadApi = new UploadController();
           
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    try
                    {
                        int FileLen;
                        System.IO.Stream MyStream;

                        var fileName = Path.GetFileName(file.FileName);
                        FileLen = file.ContentLength;
                        byte[] input = new byte[FileLen];

                        // Initialize the stream.
                        MyStream = file.InputStream;

                        // Read the file into the byte array.
                        MyStream.Read(input, 0, FileLen);

                        var document = new DocumentUpload();
                        document.Data = input;
                        document.DocumentName = fileName;
                        var objetsEnErreur = uploadApi.PostFile(document);
                        var statut = objetsEnErreur == null ? true : objetsEnErreur.Count == 0 ? true : false;

                        if (statut)
                        {
                            ViewBag.Success = "File upload successful";
                        }
                        else
                        {
                            ViewBag.Error = "File upload error";
                        }

                    }
                    catch (Exception e)
                    {
                        ViewBag.Error = "Please select a file to upload.";
                    }
                }
            }
            else
            {
                ViewBag.Error = "Please select a file to upload.";
            }

            return View();
        }

    }
}
