using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Business;
using Econocom.Data;
using Econocom.Helper.File;
using Econocom.Helper.Session;
using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.Models.Common;
using Econocom.Model.ViewModel;
using Model;
using Model.Interfaces;
using Econocom.Helper;
using System.IO;
using RTE;
using Web.Models;
using Web.ServiceReference1;
using DocumentUpload = Econocom.Model.Models.Service.DocumentUpload;
using IEconocomService = Econocom.Model.Interfaces.IEconocomService;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        private Web.ServiceReference1.IEconocomService service;
        public HomeController()
           
        {
            service = new EconocomServiceClient();
            base.Service = service;
        }

        public ActionResult Edit(int id)
        {
            ViewBag.loginModel = new LoginViewModel();
            return PartialView();
        }

        [HttpPost]
        public JsonResult Edit(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginContact = new CONTACT {EMAIL = model.Username, MOTPASSE = model.Password};
                var contact = service.GetContact(loginContact);
                if (contact != null && contact.CLIENT.ACTIF)
                {
                    FormsAuthentication.SetAuthCookie(contact.PRENOMCONTACT, model.RememberMe);
                    return Json(JsonResponseFactory.SuccessResponse(contact), JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var result = Json(JsonResponseFactory.ErrorResponse("Username/Password error"),
                                      JsonRequestBehavior.DenyGet);
                    return result;
                }
            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse("Please review your form"), JsonRequestBehavior.DenyGet);
            }          
        }

        public void ExportPDF()
        {
            ViewBag.Message = "MVC 3 File Upload";
            try
            {                

                var c = RenderPartialViewToString("ExportView");

                //PdfHelper.GeneratePdf(c); // works great with license version
                PdfHelper.genPdf(c);
                var s = "";
            }
            catch (Exception e)
            {
                var s0 = e.Message;
                var s1 = e.Data;
                var s3 = e.Source;
                ModelState.AddModelError("RIB", "Please enter a vali RIB");
                var s = e.Message;
            }            
        }


        public ActionResult MonSI()
        {
            
            ViewBag.Message = "MVC 3 File Upload";
            try
            {
                //var adresse = new ADRESSE();
                //adresse.IDPAYS = 1;
                //adresse.CODEPOSTAL = "s234";
                //adresse.REGION = "regions1";
                //adresse.VILLE = "ville 3";

                //var banque = new IDENTITEBANCAIRE();
                //banque.BIC = "111111";
                //banque.CLERIB = "2222222";
                //banque.CODEBANQUE = "BA1111";
                //banque.GUICHET = "g02";
                //banque.IBAN = "0222202020";
                //banque.NUMCOMPTE = "09090909090";

                //var contact = new CONTACT();
                //contact.EMAIL = "r@f.t";
                //contact.FONCTION = "tl";               
                //contact.IDTYPECONTACT = 1;
                //contact.MOTPASSE = "fff";
                //contact.NOMCONTACT = "nom";
                //contact.NUMEROPHONE = "34343434";
                //contact.PRENOMCONTACT = "prenom";

                //var client = new CLIENT();
                //client.ADRESSE = adresse;
                //client.IDENTITEBANCAIRE = banque;
                //client.IDENTIFICATIONNATIONAL = "456789";
                //client.DATEDEBUT = DateTime.Now;
                //client.DATEFIN = DateTime.Now.AddYears(1);
                //client.GROUPE = "mo groupe";
                //client.NOM = "monom";
                //client.CODENAF = "codenaf";
                //client.IDSECTEURACTIVITE = 3;
                //client.CONTACTs.Add(contact);

                //service.SetClient(new CLIENT());              
            }
            catch (Exception e)
            {
                var s0 = e.Message;
                var s1 = e.Data;
                var s3 = e.Source;
                ModelState.AddModelError("RIB", "Please enter a vali RIB");
                var s = e.Message;
            }
            return View();
        }

       

        public ActionResult Upload()
        {
            ViewBag.Message = "MVC 3 File Upload";

            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
           
            if(file!=null)
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

                        var document1 = new DocumentUpload();
                        document1.documentName = file.FileName;
                        document1.data = input;
                        var x = new EconocomServiceClient();
                        x.SaveDocument(document1.documentName, document1.data);
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

       


        public ActionResult Login()
        {
            if (Request.IsAjaxRequest())
            {
                ViewData.ModelState.Clear();
                return PartialView();
            }
            else
            {
                return View();
            }
        }

        public ActionResult PartialLogOn()
        {
           
                return PartialView("_LogOnPartial");
           
        }

        [HttpPost]
        public ActionResult LogOn(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //  var loginAction = new LoginAction();                
                //var hashPassword = hashHelper.HashPassWord(model.Password);
                var contact = new CONTACT();// service.GetContact(model.Username, model.Password);
                if (contact != null && contact.CLIENT.ACTIF)
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "The user name or password provided is incorrect.");
                    return PartialView("Login", model);
                }
            }

            return PartialView("Login", model);
           // return PartialViewResult("",model);
        }


        public ActionResult Index()
        {
            base.InitLanguageDropDown();
            return View();
        }

        public ActionResult TinyMce()
        {
            var publishedContent = new ContenuModere { Contenu = "Tiny Mce Test" };
            return View(publishedContent);
        }

        public ActionResult RichTextEditorTest()
        {
            try
            {
                Editor Editor1 = new Editor(System.Web.HttpContext.Current, "Editor1");
                Editor1.LoadFormData("Type Here...");
                Editor1.MvcInit();
                ViewBag.Editor = Editor1.MvcGetString();
                return View();
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
        }

        public ActionResult TinyMce1()
        {
            try
            {
                base.InitierContentuPublier("TinyMce1",System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                return View();
            }
            catch (Exception e)
            {
                //log e
                return base.Erreur();
            }
        }
        
        public ActionResult About()
        {
            ViewBag.Culture = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

            return View();
        }


        public ActionResult Language(string language)
        {
            SessionHelper.Culture = new CultureInfo(language);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Test()
        {
            return View();
        }

       /*
        public ActionResult Demo()
        {           
            return View();
        }
        //Bind Web Grid and also do paging 
        public ActionResult WebGrid(int page = 1, string sort = "name", string sortDir = "ASC")
        {
            const int pageSize = 5;
            var totalRows = service.ListeTypeObjet().Count();
            sortDir = sortDir.Equals("desc", StringComparison.CurrentCultureIgnoreCase) ? sortDir : "asc";
            var validColumns = new[] { "custid", "name", "address", "contactno" };
            if (!validColumns.Any(c => c.Equals(sort, StringComparison.CurrentCultureIgnoreCase)))
            {
                sort = "custid";
            }
            var customer = mobjModel.GetCustomerPage(page, pageSize, "it." + sort + " " + sortDir);
            var data = new PagedCustomerModel()
            {
                TotalRows = totalRows,
                PageSize = pageSize,
                TypeObjet = customer
            };
            return View(data);
        }

        public ActionResult Create()
        {
            if (Request.IsAjaxRequest())
            {
                ViewBag.IsUpdate = false;
                return View("_Customer");
            }
            else return View();
        }
        public ActionResult View(int id)
        {
            var data = mobjModel.GetCustomer(id);
            if (Request.IsAjaxRequest())
            {
                CustomerModel cust = new CustomerModel();
                cust.CustID = data.CustID;
                cust.Name = data.Name;
                cust.Address = data.Address;
                cust.ContactNo = data.ContactNo;
                return View("_ViewCustomer", cust);
            }
            else
                return View(data);
        }
        public ActionResult Edit(int id)
        {
            var data = mobjModel.GetCustomer(id);
            if (Request.IsAjaxRequest())
            {
                CustomerModel cust = new CustomerModel();
                cust.CustID = data.CustID;
                cust.Name = data.Name;
                cust.Address = data.Address;
                cust.ContactNo = data.ContactNo;
                ViewBag.IsUpdate = true;
                return View("_Customer", cust);
            }
            else
                return View(data);
        }
        public ActionResult Delete(int id)
        {
            bool check = mobjModel.DeleteCustomer(id);
            var data = mobjModel.GetCustomer();
            return RedirectToAction("WebGrid");
        }
        [HttpPost]
        public ActionResult CreateEditCustomer(CustomerModel mCust, string Command)
        {
            // Validate the model being submitted
            if (!ModelState.IsValid)
            {
                return PartialView("_Customer", mCust);
            }
            else if (Command == "Save")
            {
                Customer mobjcust = new Customer();
                mobjcust.CustID = mCust.CustID;
                mobjcust.Address = mCust.Address;
                mobjcust.ContactNo = mCust.ContactNo;
                mobjcust.Name = mCust.Name;
                bool check = mobjModel.CreateCustomer(mobjcust);
                if (check)
                {
                    TempData["Msg"] = "Data has been saved succeessfully";
                    ModelState.Clear();
                    return RedirectToAction("WebGrid", "Home");
                }
            }
            else if (Command == "Update")
            {
                Customer mobjcust = new Customer();
                mobjcust.CustID = mCust.CustID;
                mobjcust.Address = mCust.Address;
                mobjcust.ContactNo = mCust.ContactNo;
                mobjcust.Name = mCust.Name;
                bool check = mobjModel.UpdateCustomer(mobjcust);
                if (check)
                {
                    TempData["Msg"] = "Data has been updated succeessfully";
                    ModelState.Clear();
                    return RedirectToAction("WebGrid", "Home");
                }
            }
            return PartialView("_Customer");
        }*/
        
    }
}
